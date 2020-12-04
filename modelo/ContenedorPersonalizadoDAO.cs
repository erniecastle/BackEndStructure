/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Clase ContenedorPersonalizadoDAO para llamados a metodos de Entity
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
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Text;
using Exitosw.Payroll.Entity.util;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public class ContenedorPersonalizadoDAO : GenericRepository<ContenedorPersonalizado>, ContenedorPersonalizadoDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");

        public Mensaje agregar(ContenedorPersonalizado entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<ContenedorPersonalizado>().Add(entity);
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

        public Mensaje actualizar(ContenedorPersonalizado entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<ContenedorPersonalizado>().AddOrUpdate(entity);
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

        public Mensaje eliminar(ContenedorPersonalizado entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<ContenedorPersonalizado>().Attach(entity);
                getSession().Set<ContenedorPersonalizado>().Remove(entity);
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

        public Mensaje getContenedoresPersonalesSinUsuarios(Perfiles perfil, DBContextAdapter dbContext)
        {
            List<ContenedorPersonalizado> listcontenedorPer = new List<ContenedorPersonalizado>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listcontenedorPer = (from cp in getSession().Set<ContenedorPersonalizado>()
                                     where cp.perfiles.id == perfil.id
                                    /* && cp.usuario == null*/
                                     select cp).ToList();
                mensajeResultado.resultado = listcontenedorPer;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ContenedoresPersonalesSinUsuarios()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getContenedoresPersonalPerfil(Perfiles perfil, DBContextAdapter dbContext)
        {
            List<ContenedorPersonalizado> listcontenedorPer = new List<ContenedorPersonalizado>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listcontenedorPer = (from cp in getSession().Set<ContenedorPersonalizado>()
                                     where cp.perfiles.id == perfil.id
                                     select cp).ToList();
                mensajeResultado.resultado = listcontenedorPer;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ContenedoresPersonalPerfil()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getContenedoresPersonalUsuario(Usuario user, DBContextAdapter dbContext)
        {
            List<ContenedorPersonalizado> listcontenedorPerUser = new List<ContenedorPersonalizado>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listcontenedorPerUser = (from cp in getSession().Set<ContenedorPersonalizado>()
                                         where /*cp.usuario.id == user.id ||*/
                                         cp.perfiles == user.perfiles
                                         select cp).ToList();
                mensajeResultado.resultado = listcontenedorPerUser;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ContenedoresPersonalUsuario()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getContenedorPersonal(int parentId, Herramienta herramienta, DBContextAdapter dbContext)
        {
            List<ContenedorPersonalizado> listcontenedorPersonal = new List<ContenedorPersonalizado>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listcontenedorPersonal = (from cp in getSession().Set<ContenedorPersonalizado>()
                                         /* where cp.parentId == parentId*/
                                          /* && cp.herramienta.id == herramienta.id*/
                                          orderby cp.ordenId ascending
                                          select cp).ToList();
                mensajeResultado.resultado = listcontenedorPersonal;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ContenedoresPersonalUsuario()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getContenedorPersonalAll(DBContextAdapter dbContext)
        {
            List<ContenedorPersonalizado> listcontenedorPerAll = new List<ContenedorPersonalizado>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listcontenedorPerAll = (from cp in getSession().Set<ContenedorPersonalizado>()
                                        select cp).ToList();
                mensajeResultado.resultado = listcontenedorPerAll;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ContenedorPersonalAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getContenedorPersonalHert(Herramienta herramienta, DBContextAdapter dbContext)
        {
            List<ContenedorPersonalizado> listcontenedorPersonalHert = new List<ContenedorPersonalizado>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listcontenedorPersonalHert = (from cp in getSession().Set<ContenedorPersonalizado>()
                                                  /* where cp.herramienta.id == herramienta.id*/
                                              select cp).ToList();
                mensajeResultado.resultado = listcontenedorPersonalHert;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ContenedorPersonalHert()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getContenedorPersonalPerfilHerr(Perfiles perfil, Herramienta herramienta, DBContextAdapter dbContext)
        {
            List<ContenedorPersonalizado> listcontenedorPerfilHert = new List<ContenedorPersonalizado>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listcontenedorPerfilHert = (from cp in getSession().Set<ContenedorPersonalizado>()
                                            where cp.perfiles.id == perfil.id
                                            /* && cp.herramienta.id == herramienta.id */
                                            select cp).ToList();
                mensajeResultado.resultado = listcontenedorPerfilHert;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ContenedorPersonalPerfilHerr()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getContenedorPersonalUsuarioHerr(Usuario user, Herramienta herramienta, DBContextAdapter dbContext)
        {
            List<ContenedorPersonalizado> listcontenedorUserHert = new List<ContenedorPersonalizado>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listcontenedorUserHert = (from cp in getSession().Set<ContenedorPersonalizado>()
                                          where /*cp.usuario.id == user.id &&*/
                                          cp.perfiles.id == user.perfiles.id
                                          /* && cp.herramienta.id == herramienta.id */
                                          select cp).ToList();
                mensajeResultado.resultado = listcontenedorUserHert;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ContenedorPersonalUsuarioHerr()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
    }
}