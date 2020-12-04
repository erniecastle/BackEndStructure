/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Clase ElementosAplicacionDAO para llamados a metodos de Entity
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
using Exitosw.Payroll.Core.util;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using Exitosw.Payroll.Core.genericos.campos;
using Exitosw.Payroll.Entity.util;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public class ElementosAplicacionDAO : GenericRepository<ElementosAplicacion>, ElementosAplicacionDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        public Mensaje getAllElementosAplicacion(DBContextAdapter dbContext)
        {
           // List<ElementosAplicacion> listaElementosAplicacion = new List<ElementosAplicacion>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
             var  listaElementosAplicacion = (from e in getSession().Set<ElementosAplicacion>()
                                            select new {
                                                e.clave,
                                                e.entidad,
                                                e.id,
                                                e.nombre,
                                                e.ordenId,
                                                e.parentId

                                            }).ToList();
                mensajeResultado.resultado = listaElementosAplicacion;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ElementosAplicacionAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getElementosAplicacionHert(DBContextAdapter dbContext, long nodoPadre)
        {
            List<ElementosAplicacion> listaElementosApliHert = new List<ElementosAplicacion>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listaElementosApliHert = (from e in getSession().Set<ElementosAplicacion>()
                                          where e.parentId == nodoPadre
                                          orderby e.parentId, e.ordenId ascending
                                          select e).ToList();
                mensajeResultado.resultado = listaElementosApliHert;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ElementosAplicacionHert()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getElementosAplicacionMaximo(DBContextAdapter dbContext)
        {
            decimal result = 0;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                //result = obtenerIdMax();
                if (result == 0)
                {
                    result = 0L;
                }
                mensajeResultado.resultado = result;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ElementosAplicacionMaximo()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getElementosAplicacionPorClave(DBContextAdapter dbContext, string clave, long parentID)
        {
            ElementosAplicacion elementosApliXClav = new ElementosAplicacion();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                elementosApliXClav = (from e in getSession().Set<ElementosAplicacion>()
                                      where e.clave.Equals(clave) &&
                                      e.parentId == parentID
                                      orderby e.parentId, e.ordenId ascending
                                      select e).SingleOrDefault();
                mensajeResultado.resultado = elementosApliXClav;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ElementosAplicacionPorClave()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje guardarElementosAplicacion(DBContextAdapter dbContext, List<ElementosAplicacion> agrega, object[] eliminados)
        {
            List<ElementosAplicacion> listaElementosAplicacion = new List<ElementosAplicacion>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                int i;
                bool exito = true;
                if (eliminados.Count() > 0)
                {
                    //exito = deleteListQuerys("ElementosAplicacion", "id", eliminados);
                    exito = deleteListQuerys("ElementosAplicacion", new CamposWhere("ElementosAplicacion.id", eliminados, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                agrega = (agrega == null ? new List<ElementosAplicacion>() : agrega);
                if (exito && !agrega.Any())
                {
                    for (i = 0; i < agrega.Count(); i++)
                    {
                        listaElementosAplicacion.Add(getSession().Set<ElementosAplicacion>().Add(agrega[i]));
                        if (i % 100 == 0 && i > 0)
                        {
                            getSession().SaveChanges();
                        }
                    }
                }
                if (exito)
                {
                    mensajeResultado.resultado = agrega;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("guardarElementosAplicacion()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            throw new NotImplementedException();
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
            }
            return exito;
        }
    }
}