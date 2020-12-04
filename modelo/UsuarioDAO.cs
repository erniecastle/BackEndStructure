/**
* @author: Daniel Ruelas 
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Clase UsuarioDAO para llamados a metodos de Entity
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
using System.Reflection;
using System.Text;
using System.Data.Entity.Migrations;
using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.Entity.util;
using System.Data.Entity;
using Newtonsoft.Json;

namespace Exitosw.Payroll.Core.modelo
{
    public class UsuarioDAO : GenericRepository<Usuario>, UsuarioDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");

        public Mensaje agregar(Usuario entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<Usuario>().Add(entity);
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

        public Mensaje modificar(Usuario entity, DBContextAdapter dbContext)
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
                getSession().Set<Usuario>().AddOrUpdate(entity);
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

        public Mensaje eliminar(Usuario entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<Usuario>().Attach(entity);
                getSession().Set<Usuario>().Remove(entity);
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

        public Mensaje getAccesoCorrecto(string apodo, string password, DBContextAdapter dbContext)
        {
            Usuario usuario;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                usuario = (from u in getSession().Set<Usuario>()
                           where u.nombre == apodo && u.password == password
                           select u).SingleOrDefault();
                mensajeResultado.resultado = usuario;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getAccesoCorrecto()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getAccesoCorrectoConRazonSocialYRazonesSociales(string apodo, string password, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            Usuario usuario;
            List<RazonSocial> listRazonSocial;

            List<RazonesSociales> listRazonesSociales = null;
            List<Object> listObject;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaestra.context);
                getSession().Database.BeginTransaction();
                usuario = (from u in getSession().Set<Usuario>()
                           where u.nombre == apodo && u.password == password
                           select u).SingleOrDefault();
                if (usuario != null)
                {
                    listRazonSocial = (from ra in getSession().Set<RazonSocialConfiguracion>()
                                       where ra.usuario.clave == usuario.clave && ra.permitido == true
                                       select
                                        ra.razonSocial
                                      ).Distinct().ToList();
                    if (listRazonSocial == null)
                    {
                        listRazonSocial = new List<RazonSocial>();
                    }
                    String[] listClaveRazonesSociales;
                    if (listRazonSocial.Count > 0)
                    {
                        if (usuario.perfiles.nivelAccesoSistema == 0)
                        {

                            listRazonSocial = (from u in getSession().Set<RazonSocial>()
                                               select u).ToList();
                            if (listRazonSocial == null)
                            {
                                listRazonSocial = new List<RazonSocial>();
                            }
                        }
                    }

                    listClaveRazonesSociales = new String[listRazonSocial.Count];
                    int i;
                    for (i = 0; i < listRazonSocial.Count; i++)
                    {
                        listClaveRazonesSociales[i] = listRazonSocial[i].claveRazonSocial;
                    }
                    if (listClaveRazonesSociales.Length > 0)
                    {
                        listRazonesSociales = (from ra in getSession().Set<RazonesSociales>()
                                               where listClaveRazonesSociales.Contains(ra.clave)
                                               select ra).ToList();
                        listObject = new List<Object>();
                        listObject.Add(usuario);
                        listObject.Add(listRazonesSociales);
                        listObject.Add(listRazonSocial);
                    }
                    else {
                        listObject = new List<Object>();
                        listObject.Add(usuario);
                    }
                    mensajeResultado.resultado = listObject;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getAccesoCorrectoConRazonSocialYRazonesSociales()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getAllUsuario(DBContextAdapter dbContext)
        {
            List<Usuario> usuarios = new List<Usuario>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                usuarios = (from u in getSession().Set<Usuario>() select u).ToList();
                mensajeResultado.resultado = usuarios;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getUsuarioAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorClaveUsuario(string clave, DBContextAdapter dbContext)
        {
            Usuario usuario;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                usuario = (from u in getSession().Set<Usuario>()
                           where u.clave == clave
                           select u).SingleOrDefault();
                mensajeResultado.resultado = usuario;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getUsuarioPorClave()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorIdUsuario(decimal? id, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var usuario = (from u in getSession().Set<Usuario>()
                               where u.id == id
                               select new
                               {
                                   u.activaFechaEx,
                                   u.clave,
                                   u.email,
                                   u.fechaExpiracion,
                                   u.id,
                                   u.idioma,
                                   u.nombre,
                                   u.password,
                               }).SingleOrDefault();
                mensajeResultado.resultado = usuario;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorIdUsuario()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }

            return mensajeResultado;
        }

        public Mensaje setLastConfiguraciones(decimal? idiUsuario, Dictionary<string, int> lastAcces, DBContextAdapter dbContextMaestra)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaestra.context);
                getSession().Database.BeginTransaction();
                var entity = getSession().Set<Usuario>().FirstOrDefault(g => g.id == idiUsuario);

                string json = JsonConvert.SerializeObject(lastAcces);
                entity.lastConfig = json;
                getSession().Set<Usuario>().AddOrUpdate(entity);
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

        public Mensaje getLastConfiguraciones(decimal? idiUsuario, DBContextAdapter dbContextMaestra)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaestra.context);
                getSession().Database.BeginTransaction();
                var entity = getSession().Set<Usuario>().FirstOrDefault(g => g.id == idiUsuario);
                mensajeResultado.resultado = entity.lastConfig;
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

        public Mensaje getClaveUser(decimal? id, DBContextAdapter dbContextMaestra)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaestra.context);
                getSession().Database.BeginTransaction();
                var usuario = (from u in getSession().Set<Usuario>()
                               where u.id == id
                               select new
                               {
                         
                                   u.clave
                                  
                               }).SingleOrDefault();
                mensajeResultado.resultado = usuario;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getClaveUser()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }

            return mensajeResultado;
        }
    }
}
