using Exitosw.Payroll.Entity.entidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Reflection;
using System.Data.Entity.Migrations;
using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.Entity.util;

namespace Exitosw.Payroll.Core.modelo
{
    public class ProcesosOrigenDAO : GenericRepository<ProcesoOrigen>, ProcesosOrigenDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Server").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        bool commit;
        public Mensaje actualizar(ProcesoOrigen entity, DBContextAdapter uuidCxn)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(uuidCxn.context);
                getSession().Database.BeginTransaction();
                getSession().Set<ProcesoOrigen>().AddOrUpdate(entity);
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

        public Mensaje agregar(ProcesoOrigen entity, DBContextAdapter uuidCxn)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(uuidCxn.context);
                getSession().Database.BeginTransaction();
                getSession().Set<ProcesoOrigen>().Add(entity);
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

        public Mensaje eliminar(ProcesoOrigen entity, DBContextAdapter uuidCxn)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(uuidCxn.context);
                getSession().Database.BeginTransaction();
                var getEntity = (from b in getSession().Set<ProcesoOrigen>()
                                 where b.clave == entity.clave
                                 select b).SingleOrDefault();
                if (getEntity != null)
                {
                    getSession().Set<ProcesoOrigen>().Attach(getEntity);
                    getSession().Set<ProcesoOrigen>().Remove(getEntity);
                }
                
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

        public Mensaje getAllProcesoOrigen(DBContextAdapter uuidCxn)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(uuidCxn.context);
                getSession().Database.BeginTransaction();
                var mylist = (from a in getSession().Set<ProcesoOrigen>()
                              select new
                              {
                                  a.id,
                                  a.clave,
                                  a.nombre
                              }).ToList();
                mensajeResultado.resultado = mylist;
                mensajeResultado.error = "";
                mensajeResultado.noError = 0;
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getAllProcesoOrigen()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getMaxProcesoOrigen(DBContextAdapter uuidCxn)
        {
            try
            {
                long maxClave = 0;
                inicializaVariableMensaje();
                if (uuidCxn == null)
                {
                    setSession(new DBContextMaster());
                }
                else
                {
                    setSession(uuidCxn.context);
                }
                getSession().Database.BeginTransaction();
                var list = (from concap in getSession().Set<ProcesoOrigen>() select concap.clave).ToList();
                if (list.Count > 0)
                {
                    maxClave = list.Max(p => Convert.ToInt64(p));
                }
                mensajeResultado.resultado = maxClave;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getMaxProcesoOrigen()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            getSession().Database.Connection.Close();
            return mensajeResultado;
        }

        public Mensaje getParametrosProcesoOrigen(decimal idProceso, bool toDictionary, DBContextAdapter uuidCxn)
        {
            List<ParametrosProcesoOrigen> parametrosProceso = new List<ParametrosProcesoOrigen>();
            try
            {
                inicializaVariableMensaje();
                if (uuidCxn == null)
                {
                    setSession(new DBContextMaster());
                }
                else
                {
                    setSession(uuidCxn.context);
                }

                getSession().Database.BeginTransaction();
                parametrosProceso = (from ca in getSession().Set<ParametrosProcesoOrigen>()
                                     where ca.procesoOrigen.id == idProceso
                                     select ca).ToList();

                var cmapos = parametrosProceso.Select(

                    x => new
                    {
                        id = x.GetType().GetProperty("id").GetValue(x, null),
                        idproceso = GetPropValue("procesoOrigen.id", x).ToString(),
                        procesoorigenNombre = GetPropValue("procesoOrigen.nombre", x).ToString(),
                        campo = x.GetType().GetProperty("campo").GetValue(x, null),
                        estado = x.GetType().GetProperty("estado").GetValue(x, null),
                        capturaRango = x.GetType().GetProperty("capturaRango").GetValue(x, null),
                        requerido = x.GetType().GetProperty("requerido").GetValue(x, null),
                        idEtiqueta = x.GetType().GetProperty("idEtiqueta").GetValue(x, null),
                        tipoDeDato = x.GetType().GetProperty("tipoDeDato").GetValue(x, null),
                        compAncho = x.GetType().GetProperty("compAncho").GetValue(x, null),
                        configuracionTipoCaptura = x.GetType().GetProperty("configuracionTipoCaptura").GetValue(x, null),
                        // nombreFuente = x.GetType().GetProperty("id").GetValue(x.origenDatos.nombre, null),
                    }).ToList();

                mensajeResultado.resultado = cmapos;

                if (toDictionary)
                {
                    var cmaposDic = cmapos.ToDictionary(ele => ele.id, ele => ele);
                    mensajeResultado.resultado = cmaposDic;
                }

                mensajeResultado.error = "";
                mensajeResultado.noError = 0;
                getSession().Database.CurrentTransaction.Commit();

            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getParametrosProcesoOrigen()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            getSession().Database.Connection.Close();
            return mensajeResultado;
        }
        public Object GetPropValue(String name, Object obj)
        {
            foreach (String part in name.Split('.'))
            {
                if (obj == null) { return null; }

                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }

        public Mensaje getParametrosProcesoOrigenID(decimal idProceso, bool toDictionary, DBContextAdapter uuidCxn)
        {
            try
            {
                inicializaVariableMensaje();
                if (uuidCxn == null)
                {
                    setSession(new DBContextMaster());
                }
                else
                {
                    setSession(uuidCxn.context);
                }

                getSession().Database.BeginTransaction();
                var parametrosProceso = (from ca in getSession().Set<ParametrosProcesoOrigen>()
                                         where ca.id == idProceso
                                         select new
                                         {
                                             id = ca.id,
                                             idproceso = ca.procesoOrigen_ID,
                                             procesoorigenNombre = ca.procesoOrigen.nombre,
                                             campo = ca.campo,
                                             estado = ca.estado,
                                             capturaRango = ca.capturaRango,
                                             requerido = ca.requerido,
                                             idEtiqueta = ca.idEtiqueta,
                                             tipoDeDato = ca.tipoDeDato,
                                             compAncho = ca.compAncho,
                                             configuracionTipoCaptura = ca.configuracionTipoCaptura

                                         }).SingleOrDefault();

                
                mensajeResultado.resultado = parametrosProceso;
                mensajeResultado.error = "";
                mensajeResultado.noError = 0;
                getSession().Database.CurrentTransaction.Commit();

            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getParametrosProcesoOrigenID()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            getSession().Database.Connection.Close();
            return mensajeResultado;
        }

        public Mensaje getPorClaveProcesoOrigen(string clave, DBContextAdapter uuidCxn)
        {
            throw new NotImplementedException();
        }

        public Mensaje getPorIdProcesoOrigen(decimal clave, DBContextAdapter uuidCxn)
        {
            try
            {
                inicializaVariableMensaje();
                if (uuidCxn.Equals(""))
                {
                    setSession(uuidCxn.context);
                }
                else
                {
                    setSession(uuidCxn.context);
                }
                getSession().Database.BeginTransaction();
                var myList = (from a in getSession().Set<ProcesoOrigen>()
                              where a.id == clave
                              select new
                              {
                                  id = a.id,
                                  clave = a.clave,
                                  nombre = a.nombre,
                                  estado = a.estado,
                                  idEtiqueta = a.idEtiqueta,
                                  parametrosProcesoOrigen = a.parametrosProcesoOrigen.Select(e => new {
                                      e.id,
                                      e.campo,
                                      e.estado,
                                      e.requerido,
                                      e.capturaRango,
                                      e.compAncho,
                                      e.configuracionTipoCaptura,
                                      e.idEtiqueta,
                                      e.tipoDeDato
                                  }).ToList(),
                                  accionesProcesoOrigen = a.accionesProcesoOrigen.Select(e => new {
                                      e.id,
                                      e.descripcion,
                                      e.estado,
                                      e.idEtiqueta,
                                      e.requerido,
                                      e.rutaImagen
                                  }).ToList()
                              }
                             ).SingleOrDefault();


                mensajeResultado.resultado = myList;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
                // getSession().Database.Connection.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorIdProcesoOrigen()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            getSession().Database.Connection.Close();
            return mensajeResultado;
        }

        public Mensaje getProcesoOrigenPorRango(int[] values, DBContextAdapter uuidCxn)
        {
            try
            {
                inicializaVariableMensaje();
                int start = values[0];
                int end = values[1];
                if (uuidCxn.Equals(""))
                {
                    setSession(uuidCxn.context);
                }
                else
                {
                    setSession(uuidCxn.context);
                }
                getSession().Database.BeginTransaction();
                var myList = (from a in getSession().Set<ProcesoOrigen>()
                              orderby a.id ascending
                              select new
                              {
                                  a.id,
                                  a.clave,
                                  a.nombre,
                                  a.estado,
                                  a.idEtiqueta
                              }
                             ).Skip(start).Take(end).ToList();

                int count = (from a in getSession().Set<ProcesoOrigen>()
                             select a).Count();
                object[] valores = new object[2];
                valores[0] = myList;
                valores[1] = count;
                mensajeResultado.resultado = valores;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
                // getSession().Database.Connection.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getProcesoOrigenPorRango()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            getSession().Database.Connection.Close();
            return mensajeResultado;
        }

        public Mensaje saveDetallesProcesoOrigens(ProcesoOrigen entity, decimal[] deleteParam, decimal[] deleteAcciones, DBContextAdapter uuidCxn)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(uuidCxn.context);
                getSession().Database.BeginTransaction();
                if (entity.id == 0)
                {
                    getSession().Set<ProcesoOrigen>().AddOrUpdate(entity);
                    getSession().SaveChanges();
                    mensajeResultado.resultado = true;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }
                else
                {
                    if (entity.parametrosProcesoOrigen.Count > 0)
                    {
                        mensajeResultado = AddUpdateParametros(entity.id, entity.parametrosProcesoOrigen);
                    }

                    if (mensajeResultado.error.Equals("") && entity.accionesProcesoOrigen.Count > 0)
                    {
                        mensajeResultado = AddUpdateAcciones(entity.id, entity.accionesProcesoOrigen);
                    }

                    if (mensajeResultado.error.Equals(""))
                    {
                        //eliminar Detalles
                        if (deleteParam != null)
                        {
                            for (int i = 0; i < deleteParam.Length; i++)
                            {
                                var parametros = new ParametrosProcesoOrigen();
                                parametros.id = deleteParam[i];
                                getSession().Set<ParametrosProcesoOrigen>().Attach(parametros);
                                getSession().Set<ParametrosProcesoOrigen>().Remove(parametros);
                                getSession().SaveChanges();
                            }
                        }
                        if (deleteAcciones != null)
                        {
                            for (int i = 0; i < deleteAcciones.Length; i++)
                            {
                                var acciones = new AccionesProcesoOrigen();
                                acciones.id = deleteAcciones[i];
                                getSession().Set<AccionesProcesoOrigen>().Attach(acciones);
                                getSession().Set<AccionesProcesoOrigen>().Remove(acciones);
                                getSession().SaveChanges();
                            }
                        }


                        getSession().Set<ProcesoOrigen>().AddOrUpdate(entity);
                        getSession().SaveChanges();
                        mensajeResultado.resultado = true;
                        mensajeResultado.noError = 0;
                        mensajeResultado.error = "";
                        getSession().Database.CurrentTransaction.Commit();
                    }
                    else
                    {
                        getSession().Database.CurrentTransaction.Rollback();

                    }
                }

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDetallesProcesoOrigens()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        public Mensaje AddUpdateParametros(decimal idProceso, List<ParametrosProcesoOrigen> parametros)
        {

            try
            {
                inicializaVariableMensaje();
                // setSession(new DBContextMaster());
                for (int i = 0; i < parametros.Count; i++)
                {
                    parametros[i].procesoOrigen_ID = idProceso;
                    getSession().Set<ParametrosProcesoOrigen>().AddOrUpdate(parametros[i]);
                }
                getSession().SaveChanges();
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("AddUpdateParametros()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();

            }
            return mensajeResultado;
        }
        public Mensaje AddUpdateAcciones(decimal idProceso, List<AccionesProcesoOrigen> Acciones)
        {

            try
            {
                inicializaVariableMensaje();
                // setSession(new DBContextMaster());
                for (int i = 0; i < Acciones.Count; i++)
                {
                    Acciones[i].procesoOrigen_ID = idProceso;
                    getSession().Set<AccionesProcesoOrigen>().AddOrUpdate(Acciones[i]);
                }
                getSession().SaveChanges();
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("AddUpdateParametros()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();

            }
            return mensajeResultado;
        }

        public Mensaje getAccionesProcesoOrigenID(decimal idProceso, DBContextAdapter uuidCxn)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(uuidCxn.context);
                getSession().Database.BeginTransaction();
                var acciones = (from a in getSession().Set<AccionesProcesoOrigen>() 
                                where a.procesoOrigen_ID==idProceso
                                select new {
                                    a.id,
                                    a.clave,
                                    a.descripcion,
                                    a.estado,
                                    a.idEtiqueta,
                                    a.procesoOrigen_ID,
                                    a.requerido,
                                    a.rutaImagen
                                }).ToList();
                mensajeResultado.resultado = acciones;
                mensajeResultado.error = "";
                mensajeResultado.noError = 0;
                getSession().Database.CurrentTransaction.Commit();

            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getParametrosProcesoOrigen()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            getSession().Database.Connection.Close();
            return mensajeResultado;
        }
    }
}
