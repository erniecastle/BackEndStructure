/**
* @author: Daniel Ruelas 
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Clase PerfilDAO para llamados a metodos de Entity
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
using Exitosw.Payroll.Core.util;
using System.Data.Entity.Migrations;
using Exitosw.Payroll.Core.genericos.campos;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public class PerfilDAO : GenericRepository<Perfiles>, PerfilDAOIF
    {

        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        public Mensaje deletePerfilMenusPermisos(Perfiles entity, List<Permisos> permisos, DBContextAdapter dbContextMaestra)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaestra.context);
                getSession().Database.BeginTransaction();
                if (permisos != null)
                {
                    int i;
                    for (i = 0; i < permisos.Count; i++)
                    {
                        getSession().Set<Permisos>().Attach(permisos[i]);
                        getSession().Set<Permisos>().Remove(permisos[i]);
                        getSession().SaveChanges();
                    }
                }
                getSession().Set<Perfiles>().Add(entity);
                getSession().SaveChanges();

                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("deletePerfilMenusPermisos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        //public Mensaje existeDato(string campo, object valor, DBContextAdapter dbContextMaestra)
        //{
        //    bool existe = false;
        //    try
        //    {
        //        inicializaVariableMensaje();
        //        setSession(dbContext);
        //        getSession().Database.BeginTransaction();
              
        //        mensajeResultado.resultado = existe;
        //        mensajeResultado.noError = 0;
        //        mensajeResultado.error = "";
        //        getSession().Database.CurrentTransaction.Commit();

        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("existeDato()1_Error: ").Append(ex));
        //        mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
        //        mensajeResultado.error = ex.GetBaseException().ToString();
        //        mensajeResultado.resultado = null;
        //        getSession().Database.CurrentTransaction.Rollback();
        //    }
        //    return mensajeResultado;
        //}

        public Mensaje getAllPerfiles(DBContextAdapter dbContextMaestra)
        {
            //List<Perfiles> perfiles = new List<Perfiles>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaestra.context);
                getSession().Database.BeginTransaction();
                var perfiles = (from m in getSession().Set<Perfiles>() select new {
                    m.clave,
                    m.id,
                    m.nivelAccesoSistema,
                    m.nombre,
                    m.reporte,
                    m.skin
                }).ToList();
                mensajeResultado.resultado = perfiles;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPerfilAll()1_Error: ").Append(ex));
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

        public Mensaje getPorClavePerfiles(string clave, DBContextAdapter dbContextMaestra)
        {
            Perfiles perfiles;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaestra.context);
                getSession().Database.BeginTransaction();
                perfiles = (from m in getSession().Set<Perfiles>()
                            where m.clave == clave
                            select m).SingleOrDefault();
                mensajeResultado.resultado = perfiles;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPerfilesPorClave()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorIdPerfiles(decimal? id, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var perfil = (from p in getSession().Set<Perfiles>()
                              where p.id == id
                              select new
                              {
                                  p.id,
                                  p.clave,
                                  p.nivelAccesoSistema,
                                  p.nombre,
                                  p.reporte,
                                  p.skin
                              }).SingleOrDefault();
                mensajeResultado.resultado = perfil;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorIdPerfiles()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje savePerfilMenusPermisos(Perfiles entity, List<object> menus, List<Permisos> permisos, DBContextAdapter dbContext)
        {
            Perfiles perfil = null;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (entity.id == 0)
                {
                    getSession().Set<Perfiles>().Add(entity);
                }
                else
                {
                    getSession().Set<Perfiles>().AddOrUpdate(entity);
                }
                if (menus != null)
                {
                    Object menu = null;
                    int i;
                    for (i = 0; i < menus.Count; i++)
                    {
                        if (menus[i].GetType() == typeof(HerramientaPersonalizada))
                        {
                            HerramientaPersonalizada hp = (HerramientaPersonalizada)menus[i];
                            if (hp.perfiles.id > 0)
                            {
                                hp.perfiles = entity;
                            }
                            menu = hp;
                            getSession().Set<HerramientaPersonalizada>().AddOrUpdate(hp);
                            getSession().SaveChanges();
                        } else if (menus[i].GetType() == typeof(ContenedorPersonalizado)) {

                            ContenedorPersonalizado cp = (ContenedorPersonalizado)menus[i];
                            if (cp.perfiles.id>0)
                            {
                                cp.perfiles = entity;
                            }
                            menu = cp;
                            getSession().Set<ContenedorPersonalizado>().AddOrUpdate(cp);
                            getSession().SaveChanges();
                        }
                        else if (menus[i].GetType() == typeof(ExternoPersonalizado))
                        {

                            ExternoPersonalizado ep = (ExternoPersonalizado)menus[i];
                            if (ep.perfiles.id > 0)
                            {
                                ep.perfiles = entity;
                            }
                            menu = ep;
                            getSession().Set<ExternoPersonalizado>().AddOrUpdate(ep);
                            getSession().SaveChanges();
                        }
                        
                    }
                }

                if (permisos.Count>0) {
                    int i;
                    for (i = 0; i < permisos.Count; i++)
                    {
                        if (permisos[i].perfiles.id ==0)
                        {
                            permisos[i].perfiles=perfil;
                        }
                        getSession().Set<Permisos>().AddOrUpdate(permisos[i]);
                        getSession().SaveChanges();
                    }

                }
                perfil = entity;
                getSession().SaveChanges();
                mensajeResultado.resultado = perfil;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("savePerfilMenusPermisos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
    }
}