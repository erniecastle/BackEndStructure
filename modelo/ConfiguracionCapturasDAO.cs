using Exitosw.Payroll.Entity.entidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Reflection;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public class ConfiguracionCapturasDAO : GenericRepository<ConfiguracionCapturas>, ConfiguracionCapturasDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        public Mensaje agregar(ConfiguracionCapturas entity, DBContextAdapter dbContextMaster)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaster.context);
                getSession().Database.BeginTransaction();
                getSession().Set<ConfiguracionCapturas>().Add(entity);
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

        public Mensaje actualizar(ConfiguracionCapturas entity, DBContextAdapter dbContextMaster)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaster.context);
                getSession().Database.BeginTransaction();
                var fatherEntity = (from b in getSession().Set<ConfiguracionCapturas>()
                                    where b.clave == entity.clave
                                    select b).SingleOrDefault();

                if (fatherEntity != null)
                {
                    // Actualiza padre
                    getSession().Entry(fatherEntity).CurrentValues.SetValues(entity);

                    // Elimina hijos
                    foreach (var existingChild in fatherEntity.detalleConfigCapturas.ToList())
                    {
                        if (!entity.detalleConfigCapturas.Any(c => c.id == existingChild.id))
                            getSession().Set<DetalleConfigCapturas>().Remove(existingChild);
                    }

                    // Actualiza e Inserta hijos
                    foreach (var childModel in entity.detalleConfigCapturas)
                    {
                        var existingChild = fatherEntity.detalleConfigCapturas
                            .Where(c => c.id == childModel.id)
                            .SingleOrDefault();

                        if (existingChild != null)
                            // Actualiza hijo
                            getSession().Entry(existingChild).CurrentValues.SetValues(childModel);
                        else
                        {
                            getSession().Set<DetalleConfigCapturas>().Add(childModel);
                        }
                    }
                }

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

        public Mensaje eliminar(ConfiguracionCapturas entity, DBContextAdapter dbContextMaster)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaster.context);
                getSession().Database.BeginTransaction();
                var getEntity = (from b in getSession().Set<ConfiguracionCapturas>()
                                 where b.clave == entity.clave
                                 select b).SingleOrDefault();
                if (getEntity != null)
                {
                    getSession().Entry(getEntity).State = EntityState.Deleted;
                    getSession().Set<ConfiguracionCapturas>().Remove(getEntity);
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
            getSession().Database.Connection.Close();
            return mensajeResultado;
        }

        public Mensaje getMaxConfiguracionCapturas(DBContextAdapter dbContextMaster)
        {
            try
            {
                long maxClave = 0;
                inicializaVariableMensaje();
                setSession(dbContextMaster.context);
                getSession().Database.BeginTransaction();
                var list = (from concap in getSession().Set<ConfiguracionCapturas>() select concap.clave).ToList();
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getMaxConfiguracionCapturas()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            getSession().Database.Connection.Close();
            return mensajeResultado;
        }

        public Mensaje getConfiguracionCapturasPorClave(string clave, DBContextAdapter dbContextMaster)
        {
            ConfiguracionCapturas configCap = new ConfiguracionCapturas();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaster.context);
                getSession().Database.BeginTransaction();

                var configCapDict = (from b in getSession().Set<ConfiguracionCapturas>()
                                     where b.clave == clave
                                     select new
                                     {
                                         id = b.id,
                                         clave = b.clave,
                                         nombre = b.nombre,
                                         tipoDeCaptura = b.tipoDeCaptura,
                                         origenDeDatos = new { b.origenDeDatos.id, b.origenDeDatos.nombre },
                                         selectRegistros = b.selectRegistros,
                                         busquedaRegistros = b.busquedaRegistros,
                                         fileForma1 = b.fileForma1,
                                         fileForma2 = b.fileForma2,
                                         fileForma3 = b.fileForma3,
                                         configuracion = b.configuracion,
                                         detalleConfigCapturas = b.detalleConfigCapturas
                                         .Select(a => new
                                         {
                                             id = a.id,
                                             origenDatos = a.origenDatos_ID,
                                             origenDeDatosNombre = a.origenDatos.nombre,
                                             fileFormaCaptura = a.fileFormaCaptura,
                                             configuracionCapturas = a.configuracionCapturas_ID
                                         }).ToList()

                                     }).FirstOrDefault();

                mensajeResultado.resultado = configCapDict;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getConfiguracionCapturasPorClave()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            getSession().Database.Connection.Close();
            return mensajeResultado;
        }

        public Mensaje getConfiguracionCapturasPorRango(int[] values, DBContextAdapter dbContextMaster)
        {
            List<ConfiguracionCapturas> listConfiguracionCapturas = null;
            try
            {
                inicializaVariableMensaje();
                int start = values[0];
                int end = values[1];
                setSession(dbContextMaster.context);
                getSession().Database.BeginTransaction();
                listConfiguracionCapturas = getSession().Set<ConfiguracionCapturas>().OrderByDescending(i => i.id)
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getConfiguracionCapturasPorRango()1_Error: ").Append(ex));
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

        public Mensaje getAllCapturas(DBContextAdapter dbContextMaster)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaster.context);
                getSession().Database.BeginTransaction();
                var myList = (from a in getSession().Set<ConfiguracionCapturas>()
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getAllCapturas()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            getSession().Database.Connection.Close();
            return mensajeResultado;
        }

        public Mensaje getTotalRegistros(DBContextAdapter dbContextMaster)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaster.context);
                getSession().Database.BeginTransaction();
                int total = (from a in getSession().Set<ConfiguracionCapturas>()
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
            getSession().Database.Connection.Close();
            return mensajeResultado;
        }
    }
}