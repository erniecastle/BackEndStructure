using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Core.util;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace Exitosw.Payroll.Core.modelo
{
    public class CamposOrigenDatosDAO : GenericRepository<CamposOrigenDatos>, CamposOrigenDatosDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");

        public Mensaje agregar(CamposOrigenDatos entity, List<DetalleOrigenDatos> detalles, int[] deleteDetalles, DBContextAdapter dbContextMaster)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaster.context);
                getSession().Database.BeginTransaction();

                long maxClave = 0;
                var list = (from oriDat in getSession()
                            .Set<OrigenDatos>()
                            select oriDat.clave).ToList();
                if (list.Count > 0)
                {
                    maxClave = list.Max(p => Convert.ToInt64(p));
                }

                //OrigenDatos
                var checkOrigenDatos = (from b in getSession().Set<OrigenDatos>()
                                        where b.origen == entity.origenDatos.origen &&
                                        b.recurso == entity.origenDatos.recurso
                                        select b).SingleOrDefault();
                if (checkOrigenDatos == null)
                {
                    maxClave++;
                    entity.origenDatos.clave = maxClave.ToString();
                }
                else {
                    entity.origenDatos = checkOrigenDatos;// will not needed
                    entity.origenDatos_ID = checkOrigenDatos.id;
                    entity.origenDatos.id = checkOrigenDatos.id;
                }

                getSession().Set<OrigenDatos>().AddOrUpdate(entity.origenDatos);

                //CamposOrigenDatos
                var entityCampos = (from b in getSession().Set<CamposOrigenDatos>()
                                    where b.id == entity.id
                                    select b).SingleOrDefault();

                if (entityCampos != null)
                {
                    entity.id = entityCampos.id;
                }
                if (entity.campo != null)
                {
                    getSession().Set<CamposOrigenDatos>().AddOrUpdate(entity);

                }

                for (int i = 0; i < detalles.Count; i++)
                {
                    detalles[i].origenDatos_ID = entity.origenDatos.id;
                    detalles[i].origenDatos = checkOrigenDatos;
                    getSession().Set<DetalleOrigenDatos>().AddOrUpdate(detalles[i]);
                }

                if (deleteDetalles != null)
                {
                    for (int i = 0; i < deleteDetalles.Length; i++)
                    {
                        var delete = deleteDetalles[i];
                        var entityDeleteDet = (from b in getSession().Set<DetalleOrigenDatos>()
                                               where b.id == delete
                                               select b).SingleOrDefault();
                        getSession().Set<DetalleOrigenDatos>().Remove(entityDeleteDet);
                    }
                }
                getSession().SaveChanges();
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
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

        public Mensaje actualizar(CamposOrigenDatos entity, DBContextAdapter dbContextMaster)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaster.context);
                getSession().Database.BeginTransaction();
                var entityCampos = (from b in getSession().Set<CamposOrigenDatos>()
                                    where b.id == entity.id
                                    select b).SingleOrDefault();

                if (entityCampos != null)
                {
                    // Actualiza padre
                    getSession().Entry(entityCampos).CurrentValues.SetValues(entity);
                }
                else {
                    getSession().Set<CamposOrigenDatos>().Add(entity);
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

        public Mensaje eliminar(CamposOrigenDatos entity, DBContextAdapter dbContextMaster)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaster.context);
                getSession().Database.BeginTransaction();
                var getEntity = (from b in getSession().Set<CamposOrigenDatos>()
                                 where b.id == entity.id
                                 select b).SingleOrDefault();
                if (getEntity != null)
                {
                    getSession().Entry(getEntity).State = EntityState.Deleted;
                    getSession().Set<CamposOrigenDatos>().Remove(getEntity);
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

        public Mensaje getCamposPorOrigen(string fuente, DBContextAdapter dbContextMaster)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaster.context);
                getSession().Database.BeginTransaction();

                //OrigenDatos
                var checkOrigenDatos = (from b in getSession().Set<OrigenDatos>()
                                        where b.origen == "BD" &&
                                        b.recurso == fuente
                                        select b).SingleOrDefault();

                if (checkOrigenDatos == null)
                {
                    mensajeResultado.resultado = null;
                }
                else {
                    var camposOr = (from b in getSession().Set<CamposOrigenDatos>()
                                    where b.origenDatos_ID == checkOrigenDatos.id
                                    select new
                                    {
                                        id = b.id,
                                        campo = b.campo,
                                        estado = b.estado,
                                        llave = b.llave,
                                        requerido = b.requerido,
                                        idEtiqueta = b.idEtiqueta,
                                        tipoDeDato = b.tipoDeDato,
                                        compAncho = b.compAncho,
                                        origenDatos_ID = b.origenDatos_ID,
                                        activarGlobal = b.activarGlobal,
                                        activarCaptura = b.activarCaptura,
                                        configuracionTipoCaptura = b.configuracionTipoCaptura
                                    }).ToList();
                    mensajeResultado.resultado = camposOr;
                }

                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getCamposPorOrigen()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            getSession().Database.Connection.Close();
            return mensajeResultado;
        }

        public Mensaje getOrigen(string fuente, DBContextAdapter dbContextMaster)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaster.context);
                getSession().Database.BeginTransaction();
                var checkOrigenDatos = (from b in getSession().Set<OrigenDatos>()
                                        where b.origen == "BD" &&
                                        b.recurso == fuente
                                        select new {
                                            b.id,
                                            b.clave,
                                            b.estado,
                                            b.nombre,
                                            b.origen,
                                            b.recurso
                                        }).SingleOrDefault();
                if (checkOrigenDatos == null)
                {
                    mensajeResultado.resultado = null;
                }
                else {
                    mensajeResultado.resultado = checkOrigenDatos;
                }

                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getOrigen()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            getSession().Database.Connection.Close();
            return mensajeResultado;
        }

        public Mensaje getDetallesOrigen(string fuente, DBContextAdapter dbContextMaster)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaster.context);
                getSession().Database.BeginTransaction();

                //OrigenDatos
                var checkOrigenDatos = (from b in getSession().Set<OrigenDatos>()
                                        where b.origen == "BD" &&
                                        b.recurso == fuente
                                        select b).SingleOrDefault();
                if (checkOrigenDatos == null)
                {
                    mensajeResultado.resultado = null;
                }
                else {
                    var detallesOrigenDatos = (from b in getSession().Set<DetalleOrigenDatos>()
                                               where b.origenDatos_ID == checkOrigenDatos.id
                                               select new
                                               {
                                                   id = b.id,
                                                   clave = b.origenDatosFuente.clave,
                                                   nombre = b.origenDatosFuente.nombre,
                                                   recurso = b.origenDatosFuente.recurso,
                                                   idOrigenFuente = b.origenDatosFuente_ID
                                               }).ToList();

                    if (detallesOrigenDatos == null)
                    {
                        mensajeResultado.resultado = null;
                    }
                    else {
                        mensajeResultado.resultado = detallesOrigenDatos;
                    }
                }

                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getDetallesOrigen()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            getSession().Database.Connection.Close();
            return mensajeResultado;
        }

        public Mensaje getCampoPorID(decimal? idCampo, DBContextAdapter dbContextMaster)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaster.context);
                getSession().Database.BeginTransaction();
                var campoOrigen = (from cm in getSession().Set<CamposOrigenDatos>()
                                   where cm.id == idCampo
                                   select new
                                   {
                                       id = cm.id,
                                       campo = cm.campo,
                                       estado = cm.estado,
                                       llave = cm.llave,
                                       requerido = cm.requerido,
                                       idEtiqueta = cm.idEtiqueta,
                                       tipoDeDato = cm.tipoDeDato,
                                       compAncho = cm.compAncho,
                                       origenDatos_ID = cm.origenDatos_ID,
                                       activarGlobal = cm.activarGlobal,
                                       activarCaptura = cm.activarCaptura,
                                       configuracionTipoCaptura = cm.configuracionTipoCaptura
                                   }).SingleOrDefault();

                mensajeResultado.resultado = campoOrigen;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getCampoPorID()1_Error: ").Append(ex));
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

