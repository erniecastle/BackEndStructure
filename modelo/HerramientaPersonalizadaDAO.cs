/**
* @author: Daniel Ruelas 
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Clase HerramientaPersonalizadaDAO para llamados a metodos de Entity
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
using System.Data.Entity.Migrations;
using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.Entity.util;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public class HerramientaPersonalizadaDAO : GenericRepository<HerramientaPersonalizada>, HerramientaPersonalizadaDAOIF
    {

        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");


        public Mensaje agregar(HerramientaPersonalizada entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<HerramientaPersonalizada>().Add(entity);
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

        public Mensaje modificar(HerramientaPersonalizada entity, DBContextAdapter dbContext)
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
                getSession().Set<HerramientaPersonalizada>().AddOrUpdate(entity);
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

        public Mensaje eliminar(HerramientaPersonalizada entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                //var sentencia = (from g in getSession().Set<Genero>()
                //                 where g.id == entity.id
                //                 select g).FirstOrDefault();
                getSession().Set<HerramientaPersonalizada>().Attach(entity);
                getSession().Set<HerramientaPersonalizada>().Remove(entity);
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

        public Mensaje getAllHerramientaPersonal(DBContextAdapter dbContext)
        {
            List<HerramientaPersonalizada> herramientaPersonalizada = new List<HerramientaPersonalizada>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                herramientaPersonalizada = (from hp in getSession().Set<HerramientaPersonalizada>() select hp).ToList();
                getSession().SaveChanges();
                mensajeResultado.resultado = herramientaPersonalizada;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getHerramientaPersonalAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getAllHerrPerYContPorUsuario(string keyUser, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                int key = Int32.Parse(keyUser);
                var herrPorConYUs =
               (from h in getSession().Set<HerramientaPersonalizada>()
                join c in getSession().Set<ContenedorPersonalizado>() on h.id equals c.herramientaPersonalizada.id into joined
                where h.usuario.id == key
                group joined by new { h.id, h.nombre } into o
                select o).Select(x => new
                {
                    llave = x.Key.nombre,
                    ContenedoresPersonalizados = x.Select(v =>
                    v.Select(b => new { id = b.id, nombre = b.contenedor.nombre, accion = b.contenedor.accion, nombreIcono = b.contenedor.nombreIcono, descripcion = b.contenedor.descripcion }).ToList())
                }).ToList();
                mensajeResultado.resultado = herrPorConYUs;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
                getSession().Database.Connection.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getHerramientaPersonalAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getHerramientaPersonalPerfilHerr(Perfiles perfil, Herramienta herramienta, DBContextAdapter dbContext)
        {
            List<HerramientaPersonalizada> herramientaPersonalizada = new List<HerramientaPersonalizada>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                herramientaPersonalizada = (from hp in getSession().Set<HerramientaPersonalizada>()
                                            where hp.perfiles == perfil
                                            /* && hp.herramienta == herramienta */
                                            select hp).ToList();
                getSession().SaveChanges();
                mensajeResultado.resultado = herramientaPersonalizada;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getHerramientaPersonalPerfilHerr()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getHerramientaPersonalPrincipales(DBContextAdapter dbContext)
        {
            List<HerramientaPersonalizada> herramientaPersonalizada = new List<HerramientaPersonalizada>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                herramientaPersonalizada = (from hp in getSession().Set<HerramientaPersonalizada>()
                                                /*  orderby hp.herramienta.tipoHerramienta.id */
                                            select hp).ToList();
                getSession().SaveChanges();
                mensajeResultado.resultado = herramientaPersonalizada;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getHerramientaPersonalPrincipales()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getHerramientaPersonalUsuarioHerr(Usuario user, Herramienta herramienta, DBContextAdapter dbContext)
        {
            List<HerramientaPersonalizada> herramientaPersonalizada = new List<HerramientaPersonalizada>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                herramientaPersonalizada = (from hp in getSession().Set<HerramientaPersonalizada>()
                                            where hp.usuario == user || hp.perfiles == user.perfiles
                                            /* && hp.herramienta == herramienta */
                                            select hp).ToList();
                getSession().SaveChanges();
                mensajeResultado.resultado = herramientaPersonalizada;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getHerramientaPersonalPrincipales()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;

        }

        public Mensaje getHerramientasPersonalPerfil(Perfiles perfil, DBContextAdapter dbContext)
        {
            List<HerramientaPersonalizada> herramientaPersonalizada = new List<HerramientaPersonalizada>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                herramientaPersonalizada = (from hp in getSession().Set<HerramientaPersonalizada>()
                                            where hp.perfiles == perfil
                                            select hp).ToList();
                getSession().SaveChanges();
                mensajeResultado.resultado = herramientaPersonalizada;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getHerramientasPersonalPerfil()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getHerramientasPersonalUsuario(Usuario user, DBContextAdapter dbContext)
        {
            List<HerramientaPersonalizada> herramientaPersonalizada = new List<HerramientaPersonalizada>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                herramientaPersonalizada = (from hp in getSession().Set<HerramientaPersonalizada>()
                                            where hp.usuario == user || hp.perfiles == user.perfiles
                                            select hp).ToList();
                getSession().SaveChanges();
                mensajeResultado.resultado = herramientaPersonalizada;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getHerramientasPersonalUsuario()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
    }
}