/**
* @author: Daniel Ruelas 
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Clase HerramientaDAO para llamados a metodos de Entity
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
    public class HerramientaDAO : GenericRepository<Herramienta>, HerramientaDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        bool comit = true;
        public Mensaje agregar(Herramienta entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<Herramienta>().Add(entity);
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
        public Mensaje modificar(Herramienta entity, DBContextAdapter dbContext)
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
                getSession().Set<Herramienta>().AddOrUpdate(entity);
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
        public Mensaje eliminar(Herramienta entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                //var sentencia = (from g in getSession().Set<Genero>()
                //                 where g.id == entity.id
                //                 select g).FirstOrDefault();
                getSession().Set<Herramienta>().Attach(entity);
                getSession().Set<Herramienta>().Remove(entity);
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
        public Mensaje DeleteHerramientas(List<Herramienta> h, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                for (int i = 0; i < h.Count; i++)
                {
                    getSession().Set<Herramienta>().Attach(h[i]);
                    getSession().Set<Herramienta>().Remove(h[i]);
                    getSession().SaveChanges();

                }
                getSession().SaveChanges();
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("DeleteHerramientas()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        public Mensaje getAllHerramienta(DBContextAdapter dbContext)
        {
            List<Herramienta> herramienta = new List<Herramienta>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                herramienta = (from h in getSession().Set<Herramienta>() select h).ToList();
                getSession().SaveChanges();
                mensajeResultado.resultado = herramienta;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getHerramientaAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        public Mensaje getHerramientaCompartidas(DBContextAdapter dbContext)
        {
            List<Herramienta> herramienta = new List<Herramienta>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                herramienta = (from h in getSession().Set<Herramienta>() select h).ToList();
                getSession().SaveChanges();
                mensajeResultado.resultado = herramienta;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getHerramientaAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        public Mensaje getHerramientasPrincipales(Usuario usuario, DBContextAdapter dbContext)
        {
            List<Herramienta> values, herramientasConContenedores = new List<Herramienta>();
            try
            {
                comit = true;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                values = (from h in getSession().Set<Herramienta>()
                          orderby h.tipoHerramienta.id
                          select h).ToList();
                if (values != null || values.Count != 0)
                {
                    int i;
                    int cat;
                    for (i = 0; i < values.Count; i++)
                    {
                        cat = (from c in getSession().Set<Contenedor>() where c.herramienta == values[i] select c).Count();
                        if (cat > 0)
                        {
                            herramientasConContenedores.Add(values[i]);
                        }
                    }
                    if (usuario != null && herramientasConContenedores.Count != 0)
                    {
                        herramientasConContenedores = getHerramientasPersonalizadas(usuario, values, dbContext);
                    }
                }
                if (comit)
                {
                    getSession().SaveChanges();
                    mensajeResultado.resultado = herramientasConContenedores;
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
                comit = false;
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getHerramientasPrincipales()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        public Mensaje getHerramientasPrincipalesCompartidas(Usuario usuario, DBContextAdapter dbContext)
        {
            List<Herramienta> values, herramientasConContenedores = new List<Herramienta>();
            try
            {
                comit = true;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                values = (from h in getSession().Set<Herramienta>()
                          orderby h.tipoHerramienta.id
                          select h).ToList();
                if (values != null || values.Count != 0)
                {
                    int i;
                    int cat;
                    for (i = 0; i < values.Count; i++)
                    {
                        cat = (from c in getSession().Set<Contenedor>() where c.herramienta == values[i] select c).Count();
                        if (cat > 0)
                        {
                            herramientasConContenedores.Add(values[i]);
                        }
                    }
                    if (usuario != null && herramientasConContenedores.Count != 0)
                    {
                        herramientasConContenedores = getHerramientasPersonalizadas(usuario, values, dbContext);
                    }
                }
                if (comit)
                {
                    getSession().SaveChanges();
                    mensajeResultado.resultado = herramientasConContenedores;
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
                comit = false;
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getHerramientasPrincipalesCompartidas()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        public Mensaje SaveHerramientas(List<Herramienta> h, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                for (int i = 0; i < h.Count; i++)
                {
                    getSession().Set<Herramienta>().Add(h[i]);
                    getSession().SaveChanges();

                }
                getSession().SaveChanges();
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("SaveHerramientas()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        private List<Herramienta> getHerramientasPersonalizadas(Usuario usuario, List<Herramienta> values, DBContextAdapter dbContext)
        {
            int i, x;
            try
            {
                comit = true;
                for (i = 0; i < values.Count; i++)
                {
                    List<HerramientaPersonalizada> herramientaPersonalizadas = getHerramientaPersonalUsuarioHerr(usuario, values[i], dbContext);
                    if (!comit)
                    {
                        break;
                    }
                    if (herramientaPersonalizadas == null ? true : (herramientaPersonalizadas.Count == 0))
                    {
                        herramientaPersonalizadas = getHerramientaPersonalPerfilHerr(usuario.perfiles, values[i], dbContext);
                        if (!comit)
                        {
                            break;
                        }
                    }
                    foreach (HerramientaPersonalizada hp in herramientaPersonalizadas)
                    {
                        for (x = 0; x < values.Count; x++)
                        {
                          /*  if (values[x].id == hp.herramienta.id)
                            {
                                values[x].nombre = hp.nombre;
                                values[x].visible = hp.visible;
                                values[x].habilitado = hp.habilitado;
                            }
                            */
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                comit = false;
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getHerramientasPersonalizadas()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return values;
        }
        private List<HerramientaPersonalizada> getHerramientaPersonalUsuarioHerr(Usuario user, Herramienta herramienta, DBContextAdapter dbContext)
        {
            List<HerramientaPersonalizada> values = null;
            try
            {

                values = (from o in getSession().Set<HerramientaPersonalizada>()
                          where (o.usuario == user || o.perfiles == user.perfiles)
                          /* && o.herramienta == herramienta*/
                          select o).ToList();


            }
            catch (Exception ex)
            {
                comit = false;
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getHerramientaPersonalUsuarioHerr()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }

            return values;
        }
        private List<HerramientaPersonalizada> getHerramientaPersonalPerfilHerr(Perfiles perfil, Herramienta herramienta, DBContextAdapter dbContext)
        {
            List<HerramientaPersonalizada> values = null;
            try
            {

                values = (from o in getSession().Set<HerramientaPersonalizada>()
                          where o.perfiles == perfil
                        /*  && o.herramienta == herramienta*/
                          select o).ToList();


            }
            catch (Exception ex)
            {
                comit = false;
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getHerramientaPersonalPerfilHerr()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return values;
        }
    }
}