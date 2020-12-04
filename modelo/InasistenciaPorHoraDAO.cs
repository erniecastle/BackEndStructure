/**
* @author: Daniel Ruelas 
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Clase InasistenciaPorHoraDAO para llamados a metodos de Entity
* -----------------------------------------------------------------------------
* MODIFICACIONES:
* -----------------------------------------------------------------------------
* Clave: 
* Autor: 
* Fecha:
* Descripción: 
* -----------------------------------------------------------------------------
*/
using Exitosw.Payroll.Entity.entidad;
using System;
using System.Collections.Generic;
using System.Linq;
using Exitosw.Payroll.Core.util;
using System.Text;
using System.Reflection;
using System.Data.Entity.Migrations;
using Exitosw.Payroll.Core.genericos.campos;
using Exitosw.Payroll.Entity.util;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public class InasistenciaPorHoraDAO : GenericRepository<InasistenciaPorHora>, InasistenciaPorHoraDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");

        public Mensaje getAllInasistenciaPorHora(DBContextAdapter dbContext)
        {
            List<InasistenciaPorHora> inasistenciaPorHora = new List<InasistenciaPorHora>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                inasistenciaPorHora = (from ina in getSession().Set<InasistenciaPorHora>() select ina).ToList();
                getSession().SaveChanges();
                mensajeResultado.resultado = inasistenciaPorHora;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getInasistenciaPorHoraAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getInasistenciaPorNominaPeriodo(string claveTipoNomina, string claveRazonSocial, long idPeriodo, DBContextAdapter dbContext)
        {
            List<InasistenciaPorHora> inasistenciaPorHora = new List<InasistenciaPorHora>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                inasistenciaPorHora = (from inas in getSession().Set<InasistenciaPorHora>()
                                       join t in getSession().Set<TipoNomina>() on inas.tipoNomina.id equals t.id
                                       join p in getSession().Set<PeriodosNomina>() on inas.periodosNomina.id equals p.id
                                       join pem in getSession().Set<PlazasPorEmpleadosMov>() on inas.plazasPorEmpleadosMov.id equals pem.id
                                       join pe in getSession().Set<PlazasPorEmpleado>() on pem.plazasPorEmpleado.id equals pe.id
                                       join em in getSession().Set<Empleados>() on pe.empleados.id equals em.id
                                       where t.clave == claveTipoNomina && p.id == idPeriodo && pe.razonesSociales.clave == claveRazonSocial
                                       orderby em.clave
                                       select inas).ToList();
                mensajeResultado.resultado = inasistenciaPorHora;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getInasistenciaPorNominaPeriodo()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje saveDeleteInasistenciaPorHora(List<InasistenciaPorHora> AgreModif, object[] clavesDelete, DBContextAdapter dbContext)
        {
            bool exito = true;
            
            InasistenciaPorHora asis = null;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (clavesDelete!=null)
                {
                    if (clavesDelete.Length>0) {
                        exito = deleteListQuerys("InasistenciaPorHora", new CamposWhere("InasistenciaPorHora.id", clavesDelete, OperadorComparacion.IN, OperadorLogico.AND), dbContext); 
                          //  deleteListQuerys("InasistenciaPorHora", "id", clavesDelete);
                    }
                }
                //Guardado de modificados
                if (exito) {
                    AgreModif = (AgreModif == null ? new List<InasistenciaPorHora>() : AgreModif);
                    foreach (InasistenciaPorHora item in AgreModif)
                    {
                        asis = item;
                        getSession().Set<InasistenciaPorHora>().AddOrUpdate(item);
                    }
                }
                if (exito)
                {
                    asis = null;
                    mensajeResultado.resultado = asis;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();

                }
                else {
                    mensajeResultado.resultado = asis;
                    getSession().Database.CurrentTransaction.Rollback();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteInasistenciaPorHora()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = asis;
                getSession().Database.CurrentTransaction.Rollback();
                
            }
            return mensajeResultado;
        }

        private bool deleteListQuerys(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext)
        {
            bool exito = true;
            try
            {
                //deleteListQuery(tabla, campo, valores);
                deleteListQuery(tabla, campoWhere, dbContext);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("deleteListQuerys()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
                exito = false;
            }
            return exito;
        }
    }
}