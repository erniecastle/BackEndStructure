/**
* @author: Daniel Ruelas 
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Clase MovNominaBaseAfectaDAO para llamados a metodos de Entity
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
using System.Text;
using System.Reflection;
using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.Core.genericos.campos;
using Exitosw.Payroll.Entity.util;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public class MovNominaBaseAfectaDAO : GenericRepository<MovNomBaseAfecta>, MovNominaBaseAfectaDAOIF
    {

        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        public Mensaje getAllMovNominaBaseAfecta(DBContextAdapter dbContext)
        {
            List<MovNomBaseAfecta> movNominaBaseAfectas = new List<MovNomBaseAfecta>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                movNominaBaseAfectas = (from m in getSession().Set<MovNomBaseAfecta>() select m).ToList();
                mensajeResultado.resultado = movNominaBaseAfectas;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getMovNominaBaseAfectaAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getMovNominaBaseAfectaAsc(DBContextAdapter dbContext)
        {
            List<MovNomBaseAfecta> movNominaBaseAfectas = new List<MovNomBaseAfecta>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                movNominaBaseAfectas = (from m in getSession().Set<MovNomBaseAfecta>()
                                        orderby m.id
                                        select m).ToList();
                mensajeResultado.resultado = movNominaBaseAfectas;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getMovNominaBaseAfectaAsc()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje saveDeleteMovNominaBaseAfecta(List<MovNomBaseAfecta> AgreModif, List<MovNomBaseAfecta> Ordenados, object[] clavesDelete, DBContextAdapter dbContext)
        {
            MovNomBaseAfecta mov = null;
            try
            {
                bool exito = true;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (clavesDelete != null)
                {
                    exito = deleteListQuerys("MovNominaBaseAfecta", new CamposWhere("MovNominaBaseAfecta.id", clavesDelete, OperadorComparacion.IN, OperadorLogico.AND), dbContext);

                    //deleteListQuerys("MovNominaBaseAfecta", "id", clavesDelete);
                }
                AgreModif = (AgreModif == null ? new List<MovNomBaseAfecta>() : AgreModif);
                if (exito && AgreModif.Count > 0)
                {
                    //Guardado de modificados
                    foreach (MovNomBaseAfecta Am in AgreModif)
                    {
                        mov = Am;
                        getSession().Set<MovNomBaseAfecta>().Add(mov);
                    }
                }
                if (exito)
                {
                    getSession().SaveChanges();
                    mensajeResultado.resultado = mov;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }
                else
                {
                    getSession().Database.CurrentTransaction.Rollback();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteMovNominaBaseAfecta()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        private bool deleteListQuerys(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext)
        {
            bool exito = true;
            try
            {
                deleteListQuery(tabla, campoWhere, dbContext);
                //deleteListQuery(tabla, campo, valores);
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