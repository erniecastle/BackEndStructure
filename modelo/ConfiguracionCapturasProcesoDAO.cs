using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Entity.util;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Exitosw.Payroll.Core.modelo
{
  public  class ConfiguracionCapturasProcesoDAO : GenericRepository<ConfiguracionCapturasProceso>, ConfiguracionCapturasProcesoDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Server").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        public Mensaje agregar(ConfiguracionCapturasProceso entity, DBContextAdapter uuidCxn)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(uuidCxn.context);
                getSession().Database.BeginTransaction();
                getSession().Set<ConfiguracionCapturasProceso>().Add(entity);
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
            getSession().Database.Connection.Close();
            return mensajeResultado;
        }

        public Mensaje actualizar(ConfiguracionCapturasProceso entity, DBContextAdapter uuidCxn)
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
                getSession().Set<ConfiguracionCapturasProceso>().AddOrUpdate(entity);
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
            getSession().Database.Connection.Close();
            return mensajeResultado;
        }

        public Mensaje eliminar(ConfiguracionCapturasProceso entity, DBContextAdapter uuidCxn)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(uuidCxn.context);
                getSession().Database.BeginTransaction();
                getSession().Set<ConfiguracionCapturasProceso>().Attach(entity);
                getSession().Set<ConfiguracionCapturasProceso>().Remove(entity);
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
            getSession().Database.Connection.Close();
            return mensajeResultado;
        }

        public Mensaje getAllCapturasProceso(DBContextAdapter uuidCxn)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(uuidCxn.context);
                getSession().Database.BeginTransaction();
                var myList = (from a in getSession().Set<ConfiguracionCapturasProceso>()
                              select new
                              {
                                  id = a.id,
                                  clave = a.clave,
                                  nombre = a.nombre,
                                  tipoDeCaptura = a.tipoDeCaptura
                              }).ToList();

                mensajeResultado.resultado = myList;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getAllCapturasProceso()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getConfiguracionCapturasProcesoPorClave(string clave, DBContextAdapter uuidCxn)
        {
            ConfiguracionCapturasProceso configCap = new ConfiguracionCapturasProceso();
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

                var configCapDict = (from b in getSession().Set<ConfiguracionCapturasProceso>()
                                     where b.clave == clave
                                     select new
                                     {
                                         id = b.id,
                                         clave = b.clave,
                                         nombre = b.nombre,
                                         tipoDeCaptura = b.tipoDeCaptura,
                                         procesoOrigen_ID= b.procesoOrigen_ID,
                                         procesoOrigen =new{b.procesoOrigen.id,b.procesoOrigen.nombre,b.procesoOrigen.clave },
                                         fileForma1 = b.fileForma1,
                                         configuracion = b.configuracion
                                     }).FirstOrDefault();

                mensajeResultado.resultado = configCapDict;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getConfiguracionCapturasProcesoPorClave()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            getSession().Database.Connection.Close();
            return mensajeResultado;
        }

        public Mensaje getConfiguracionCapturasProcesoPorRango(int[] values, DBContextAdapter uuidCxn)
        {
            List<ConfiguracionCapturasProceso> listConfiguracionCapturas = null;
            try
            {
                inicializaVariableMensaje();
                int start = values[0];
                int end = values[1];
                if (uuidCxn == null)
                {
                    setSession(new DBContextMaster());
                }
                else
                {
                    setSession(uuidCxn.context);
                }
                getSession().Database.BeginTransaction();
                listConfiguracionCapturas = getSession().Set<ConfiguracionCapturasProceso>().OrderByDescending(i => i.id)
                    .Skip((start)).Take(end).ToList();

                var myList = listConfiguracionCapturas.Select(
                    x => new
                    {
                        id = x.GetType().GetProperty("id").GetValue(x, null),
                        clave = x.GetType().GetProperty("clave").GetValue(x, null),
                        nombre = x.GetType().GetProperty("nombre").GetValue(x, null)
                    }
                ).ToList();

                if (myList.Count == 0)
                {
                    myList = null;
                }

                mensajeResultado.resultado = myList;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
                // getSession().Database.Connection.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getConfiguracionCapturasProcesoPorRango()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            getSession().Database.Connection.Close();
            return mensajeResultado;
        }

        public Mensaje getMaxConfiguracionCapturasProceso(DBContextAdapter uuidCxn)
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
                var list = (from concap in getSession().Set<ConfiguracionCapturasProceso>() select concap.clave).ToList();
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getMaxConfiguracionCapturasProceso()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            getSession().Database.Connection.Close();
            return mensajeResultado;
        }

        public Mensaje getTotalRegistrosProceso(DBContextAdapter uuidCxn)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(uuidCxn.context);
                getSession().Database.BeginTransaction();
                int total = (from a in getSession().Set<ConfiguracionCapturasProceso>()
                             select a).Count();
                mensajeResultado.resultado = total;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getAllCapturas()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
    }
}
