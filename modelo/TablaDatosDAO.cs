/**
* @author: Daniel Ruelas 
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Clase TablaDatosDAO para llamados a metodos de Entity
* -----------------------------------------------------------------------------
* MODIFICACIONES:
* -----------------------------------------------------------------------------
* Clave: 
* Autor: 
* Fecha:
* Descripción: 
* -----------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using System.Linq;
using Exitosw.Payroll.Entity.entidad;
using System.Text;
using System.Reflection;
using System.Data.Entity.Migrations;
using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.Core.genericos.campos;
using Exitosw.Payroll.Entity.util;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public class TablaDatosDAO : GenericRepository<TablaDatos>, TablaDatosDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");

        public Mensaje agregar(TablaDatos entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<TablaDatos>().Add(entity);
                getSession().SaveChanges();
                mensajeResultado.resultado = entity;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("agregar()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        public Mensaje actualizar(TablaDatos entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                //var genero = getSession().Set<Genero>().FirstOrDefault(g => g.id == entity.id);
                //genero.clave = entity.clave;
                //genero.descripcion = entity.descripcion;
                //genero.empleados = entity.empleados;
                getSession().Set<TablaDatos>().AddOrUpdate(entity);
                getSession().SaveChanges();
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("actualizar()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje eliminar(TablaDatos entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                //var sentencia = (from g in getSession().Set<Genero>()
                //                 where g.id == entity.id
                //                 select g).FirstOrDefault();
                getSession().Set<TablaDatos>().Attach(entity);
                getSession().Set<TablaDatos>().Remove(entity);
                getSession().SaveChanges();

                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("eliminar()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje consultaPorControladores(string controladores, DBContextAdapter dbContext)
        {
            List<TablaDatos> tablaDatos = new List<TablaDatos>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                tablaDatos = (from m in getSession().Set<TablaDatos>()
                              where m.controladores.Contains(controladores)
                              select m).ToList();

                mensajeResultado.resultado = tablaDatos;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorControladores()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje consultaPorListaControladores(string[] controladores, DBContextAdapter dbContext)
        {
            List<TablaDatos> tablaDatos = new List<TablaDatos>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                tablaDatos = (from m in getSession().Set<TablaDatos>()
                              where controladores.Contains(m.controladores)
                              select m).ToList();

                mensajeResultado.resultado = tablaDatos;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorListaControladores()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }


        public Mensaje getExisteDatosPorTablas(string tablaBaseClave, string tablaPersonalizadaClave, DBContextAdapter dbContext)
        {
            int resul = 0;
            bool exite = false;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                tablaBaseClave = tablaBaseClave == null ? "" : tablaBaseClave.Trim();
                tablaPersonalizadaClave = tablaPersonalizadaClave == null ? "" : tablaPersonalizadaClave.Trim();
                if (tablaBaseClave.Length > 0)
                {
                    resul = (from tb in getSession().Set<TablaDatos>()
                             where tb.tablaBase.clave == tablaBaseClave
                             select tb).Count();
                }
                else if (tablaPersonalizadaClave.Length > 0)
                {
                    resul = (from tb in getSession().Set<TablaDatos>()
                             where tb.tablaPersonalizada.clave == tablaPersonalizadaClave
                             select tb).Count();
                }
                if (resul > 0)
                {
                    exite = true;
                }

                mensajeResultado.resultado = exite;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getExisteDatosPorTablas()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getListTablaDatosPorTablaBase(string clave, DBContextAdapter dbContext)
        {
            List<TablaDatos> tablaDatos = new List<TablaDatos>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                tablaDatos = (from m in getSession().Set<TablaDatos>()
                              where m.tablaBase.clave == clave
                              select m).ToList();

                mensajeResultado.resultado = tablaDatos;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getListTablaDatosPorTablaBase()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getAllTablaDatos(DBContextAdapter dbContext)
        {
            List<TablaDatos> tablaDatos = new List<TablaDatos>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                tablaDatos = (from m in getSession().Set<TablaDatos>() select m).ToList();

                mensajeResultado.resultado = tablaDatos;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getTablaDatosAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getTablaDatosPorTablas(TablaBase tablaBase, TablaPersonalizada tablaPersonalizada, DBContextAdapter dbContext)
        {
            List<TablaDatos> tablaDatos = new List<TablaDatos>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                mensajeResultado.resultado = (new List<TablaDatos>());
                if (tablaBase != null)
                {
                    tablaDatos = (from tb in getSession().Set<TablaDatos>()
                                  where tb.tablaBase.id == tablaBase.id
                                  select tb).ToList();
                    mensajeResultado.resultado = tablaDatos;
                }
                else if (tablaPersonalizada != null)
                {
                    tablaDatos = (from tb in getSession().Set<TablaDatos>()
                                  where tb.tablaPersonalizada.id == tablaPersonalizada.id
                                  select tb).ToList();
                    mensajeResultado.resultado = tablaDatos;
                }
                //mensajeResultado.resultado = tablaDatos;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getTablaDatosPorTablas()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getTablaDatosPorTablasPorControlador(string controladores, TablaBase tablaBase, TablaPersonalizada tablaPersonalizada, DBContextAdapter dbContext)
        {
            TablaDatos tablaDatos;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                mensajeResultado.resultado = (new List<TablaDatos>());
                if (tablaBase != null)
                {
                    tablaDatos = (from tb in getSession().Set<TablaDatos>()
                                  where tb.tablaBase.id == tablaBase.id &&
                                  tb.controladores.Contains(controladores)
                                  select tb).SingleOrDefault();
                    mensajeResultado.resultado = tablaDatos;
                }
                else if (tablaPersonalizada != null)
                {
                    tablaDatos = (from tb in getSession().Set<TablaDatos>()
                                  where tb.tablaPersonalizada.id == tablaPersonalizada.id &&
                                  tb.controladores.Contains(controladores)
                                  select tb).SingleOrDefault();
                    mensajeResultado.resultado = tablaDatos;
                }
                //mensajeResultado.resultado = tablaDatos;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getTablaDatosPorTablasPorControlador()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getTablaDatosPorTablasPorControladorPorFiltrosEspeciales(DateTime controlFecha, int controlAnio, string controladores, string tablaBaseClave, string tablaPersonalizadaClave, DBContextAdapter dbContext)
        {
            TablaDatos tablaDatos;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();


                tablaBaseClave = tablaBaseClave == null ? "" : tablaBaseClave.Trim();
                tablaPersonalizadaClave = tablaPersonalizadaClave == null ? "" : tablaPersonalizadaClave.Trim();
                controladores = controladores == null ? "" : controladores.Trim();
                var query = from o in getSession().Set<TablaDatos>() select o;
                if (tablaBaseClave.Length > 0)
                {
                    query = from sub in query
                            where sub.tablaBase.clave == tablaBaseClave
                            select sub;

                }
                else if (tablaPersonalizadaClave.Length > 0)
                {
                    query = from sub1 in query
                            where sub1.tablaPersonalizada.clave == tablaPersonalizadaClave
                            select sub1;
                }
                int id = query.Select(p => p.id).SingleOrDefault();
                if (controlFecha != null)
                {
                    var query2 = from tb in getSession().Set<TablaDatos>()
                                 select tb;
                    if (tablaBaseClave.Length > 0)
                    {
                        query2 = from tb in query2
                                 where query.Select(p => p.tablaBase.id).ToList().Contains(tb.tablaBase.id)
                                 select tb;
                    }
                    else
                    {

                        query2 = from tb in query2
                                 where query.Select(p => p.tablaBase.id).ToList().Contains(tb.tablaBase.id)
                                 select tb;
                    }
                    query2 = from sub in query2
                             where sub.controlPorFecha == controlFecha
                             select sub;

                    if (controlAnio > 0)
                    {
                        query2 = from sub in query2
                                 where sub.controlPorAnio == controlAnio
                                 select sub;
                    }
                    if (controladores.Length > 0)
                    {
                        query2 = from sub in query2
                                 where sub.controladores == controladores
                                 select sub;
                    }
                    query = from sub in query
                            where sub.id == (from a in query2
                                             select new {
                                                 a.id
                                             }).Max(p=>p.id)
                            select sub;
                }
                else {

                    if (controlAnio > 0)
                    {
                        query = from sub in query
                                 where sub.controlPorAnio == controlAnio
                                 select sub;
                    }
                    if (controladores.Length > 0)
                    {
                        query = from sub in query
                                 where sub.controladores == controladores
                                 select sub;
                    }
                }

                tablaDatos = query.SingleOrDefault();
                mensajeResultado.resultado = tablaDatos;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getTablaDatosPorTablasPorControladorPorFiltrosEspeciales()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                mensajeResultado = existeClave(tabla, campoWhere, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getExisteClave()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                getSession().Database.CurrentTransaction.Rollback();
                mensajeResultado.resultado = null;
            }
            return mensajeResultado;
        }

        public Mensaje getTablaDatosPorTablaBaseyFechaControl(string claveTablaBase, DateTime fecha, DBContextAdapter dbContextMaestra)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaestra.context);
                getSession().Database.BeginTransaction();
                var tablaDatos = (from o in getSession().Set<TablaDatos>()
                                  where o.tablaBase.clave==claveTablaBase &&
                                  o.id==((from t in dbContextMaestra.context.Set<TablaDatos>()  
                                        where t.tablaBase_ID == o.tablaBase_ID && t.controlPorFecha<=fecha
                                      select new { t.id}).Max(p=> p.id))
                                  select o).ToList();
                mensajeResultado.resultado = tablaDatos;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getTablaDatosPorTablaBaseyFechaControl()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                getSession().Database.CurrentTransaction.Rollback();
                mensajeResultado.resultado = null;
            }
            return mensajeResultado;
        }

        public Mensaje getTablaDatosPorTablasPorControladorPorFiltrosEspecialesJS(DateTime? controlFecha, int controlAnio, string controladores, string tablaBaseClave, string tablaPersonalizadaClave, DBContextAdapter dbContextMaestra)
        {
            TablaDatos tablaDatos;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaestra.context);
                getSession().Database.BeginTransaction();


                tablaBaseClave = tablaBaseClave == null ? "" : tablaBaseClave.Trim();
                tablaPersonalizadaClave = tablaPersonalizadaClave == null ? "" : tablaPersonalizadaClave.Trim();
                controladores = controladores == null ? "" : controladores.Trim();
                var query = from o in getSession().Set<TablaDatos>() select o;
                if (tablaBaseClave.Length > 0)
                {
                    query = from sub in query
                            where sub.tablaBase.clave == tablaBaseClave
                            select sub;

                }
                else if (tablaPersonalizadaClave.Length > 0)
                {
                    query = from sub1 in query
                            where sub1.tablaPersonalizada.clave == tablaPersonalizadaClave
                            select sub1;
                }
                int id = query.Select(p => p.id).SingleOrDefault();
                if (controlFecha != null)
                {
                    var query2 = from tb in getSession().Set<TablaDatos>()
                                 select tb;
                    if (tablaBaseClave.Length > 0)
                    {
                        query2 = from tb in query2
                                 where query.Select(p => p.tablaBase.id).ToList().Contains(tb.tablaBase.id)
                                 select tb;
                    }
                    else
                    {

                        query2 = from tb in query2
                                 where query.Select(p => p.tablaBase.id).ToList().Contains(tb.tablaBase.id)
                                 select tb;
                    }
                    query2 = from sub in query2
                             where sub.controlPorFecha == controlFecha
                             select sub;

                    if (controlAnio > 0)
                    {
                        query2 = from sub in query2
                                 where sub.controlPorAnio == controlAnio
                                 select sub;
                    }
                    if (controladores.Length > 0)
                    {
                        query2 = from sub in query2
                                 where sub.controladores == controladores
                                 select sub;
                    }
                    query = from sub in query
                            where sub.id == (from a in query2
                                             select new
                                             {
                                                 a.id
                                             }).Max(p => p.id)
                            select sub;
                }
                else
                {

                    if (controlAnio > 0)
                    {
                        query = from sub in query
                                where sub.controlPorAnio == controlAnio
                                select sub;
                    }
                    if (controladores.Length > 0)
                    {
                        query = from sub in query
                                where sub.controladores == controladores
                                select sub;
                    }
                }
                tablaDatos = query.SingleOrDefault();
                var tablaDatoss = (from a in query
                                  select new
                                  {
                                      a.controladores,
                                      a.controlPorAnio,
                                      a.controlPorFecha,
                                      a.descripcion,
                                      a.id,
                                      a.renglonSeleccionado,
                                      a.tablaBase_ID,
                                      a.tablaPersonalizada_ID
                                  }).SingleOrDefault();
                object[,] valores = null;
                valores = UtilidadesXML.extraeValoresNodos(UtilidadesXML.convierteBytesToXML(tablaDatos.fileXml));
                if (UtilidadesXML.ERROR_XML > 0)
                {
                    mensajeResultado = UtilidadesXML.mensajeError;
                    return null;
                }
                object[] datos = new object[2];
                datos[0] = tablaDatoss;
                datos[1] = valores;
                var res = datos;
                mensajeResultado.resultado = res;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getTablaDatosPorTablasPorControladorPorFiltrosEspeciales()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
    }
}