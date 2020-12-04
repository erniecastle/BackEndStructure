/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Clase ContenedorDAO para llamados a metodos de Entity
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
    public class ContenedorDAO : GenericRepository<Contenedor>, ContenedorDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");

        public Mensaje agregar(Contenedor entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<Contenedor>().Add(entity);
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
        public Mensaje actualizar(Contenedor entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<Contenedor>().AddOrUpdate(entity);
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
        public Mensaje eliminar(List<Contenedor> entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                for (int i = 0; i < entity.Count(); i++)
                {
                    getSession().Set<Contenedor>().Attach(entity[i]);
                    getSession().Set<Contenedor>().Remove(entity[i]);
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

        public Mensaje SaveContenedor(List<Contenedor> c, DBContextAdapter dbContext)
        {
            Contenedor co = new Contenedor();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                for (int i = 0; i < c.Count; i++)
                {
                    getSession().Set<Contenedor>().Add(c[i]);
                    if (i % 100 == 0)
                    {
                        getSession().SaveChanges();
                    }
                }
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("DeleteContenedor()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje DeleteContenedor(List<Contenedor> c, DBContextAdapter dbContext)
        {
            Contenedor co = new Contenedor();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                for (int i = 0; i < c.Count; i++)
                {
                    getSession().Set<Contenedor>().Attach(c[i]);
                    getSession().Set<Contenedor>().Remove(c[i]);
                    /* if (i % 100 == 0)
                     {
                         flush();
                     }*/
                    getSession().SaveChanges();

                }
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("DeleteContenedor()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getContenedor(int parentId, Herramienta herramienta, List<string> ventanasAOmitir, DBContextAdapter dbContext)
        {
            List<Contenedor> listcontenedor = new List<Contenedor>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listcontenedor = (from c in getSession().Set<Contenedor>()
                                  join v in getSession().Set<Ventana>() on c.ventana.id equals v.id into cv
                                  from v in cv.DefaultIfEmpty()
                                  where /*c.parentId == parentId &&*/
                                  c.herramienta.id == herramienta.id
                                  select c).ToList();
                if (ventanasAOmitir == null)
                {
                    ventanasAOmitir = new List<string>();
                }

                mensajeResultado.resultado = listcontenedor;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("Contenedor()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getContenedorAll(List<string> ventanasAOmitir, DBContextAdapter dbContext)
        {
            List<Contenedor> listcontenedor = new List<Contenedor>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listcontenedor = (from c in getSession().Set<Contenedor>()
                                  join v in getSession().Set<Ventana>() on c.ventana.id equals v.id into com
                                  from v in com.DefaultIfEmpty()
                                  select c).ToList();
                if (ventanasAOmitir == null)
                {
                    ventanasAOmitir = new List<string>();
                }

                mensajeResultado.resultado = listcontenedor;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ContenedorAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getContenedoresPorTipoAcciones(TipoAcciones[] tipoAcciones, DBContextAdapter dbContext)
        {
            List<Contenedor> listcontenedor = new List<Contenedor>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listcontenedor = (from c in getSession().Set<Contenedor>()
                                  where tipoAcciones.Contains(c.tipoAcciones)
                                  select c).ToList();
                mensajeResultado.resultado = listcontenedor;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ContenedoresPorTipoAcciones()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getContenedorPorHerramienta(int herramientaId, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();

                var listcontenedor = (from c in getSession().Set<Contenedor>()
                                      where c.herramienta.id == herramientaId
                                      orderby c.categoriaHerramienta.id
                                      select new
                                      {
                                          idCather = (c.categoriaHerramienta.id == null ? -1 : c.categoriaHerramienta.id),
                                          nombreCather = c.categoriaHerramienta.nombre == null ? "" : c.categoriaHerramienta.nombre,
                                          titulo = (c.tipoHerramienta_ID == null ? (c.herramienta_ID == null ? "" : c.herramienta.nombre) : c.tipoHerramienta.nombre),
                                          titulodesc = (c.tipoHerramienta_ID == null ? (c.herramienta_ID == null ? "" : c.herramienta.descripcion) : c.tipoHerramienta.descripcion),
                                          c.id,
                                          c.nombre,
                                          c.accion,
                                          c.nombreIcono,
                                          c.descripcion
                                      }

                                     ).ToList();

                List<Object> categoria = new List<Object>();
                List<Object> contenedores = new List<Object>();

                bool change = true;

                for (int i = 0; i < listcontenedor.Count(); i++)
                {
                    if (i > 0)
                    {
                        Object[] item = (Object[])categoria[categoria.Count - 1];

                        if (item[0].Equals(listcontenedor[i].idCather))
                        {
                            change = false;
                        }
                        else
                        {
                            change = true;
                            contenedores = new List<Object>();
                        }

                    }
                    contenedores.Add(new Object[] { listcontenedor[i].id, listcontenedor[i].nombre, listcontenedor[i].accion, listcontenedor[i].nombreIcono, listcontenedor[i].descripcion });
                    if (change)
                    {
                        categoria.Add(new Object[] { listcontenedor[i].idCather, listcontenedor[i].nombreCather, contenedores, listcontenedor[i].titulo, listcontenedor[i].titulodesc });
                    }

                    change = false;
                }
                mensajeResultado.resultado = categoria;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
                getSession().Database.Connection.Close();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ContenedoresPorTipoAcciones()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        public Mensaje getContenedorHert(Herramienta herramienta, List<string> ventanasAOmitir, Usuario usuario, RazonSocial razonSocial, DBContextAdapter dbContext)
        {
            List<Contenedor> listcontenedor = new List<Contenedor>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var query = (from c in getSession().Set<Contenedor>()
                             join v in getSession().Set<Ventana>() on c.ventana.id equals v.id into cv
                             from vv in cv.DefaultIfEmpty()
                             join r in getSession().Set<RazonSocial>() on c.razonSocial.id equals r.id into cr
                             from rr in cr.DefaultIfEmpty()
                             join t in getSession().Set<TipoElemento>() on c.tipoElemento.id equals t.id into ct
                             where c.herramienta.id == herramienta.id &&
                             rr == null
                             select new
                             {
                                 id = c.id,
                                 herramienta = c.herramienta,
                                 nombre = c.nombre,
                                 accion = c.accion,
                                 tipoAcciones = c.tipoAcciones,
                                 ordenId = c.ordenId,
                                 tipoElemento = c.tipoElemento,
                                 nombreIcono = c.nombreIcono,
                                 tipoIcono = c.tipoIcono,
                                 habilitado = c.habilitado,
                                 visible = c.visible,
                                 ventana = vv,
                                 razonSocial = c.razonSocial
                             }).ToList().Select(x => new Contenedor
                             {
                                 id = x.id,
                                 herramienta = x.herramienta,
                                 nombre = x.nombre,
                                 accion = x.accion,
                                 tipoAcciones = x.tipoAcciones,
                                 ordenId = x.ordenId,
                                 tipoElemento = x.tipoElemento,
                                 nombreIcono = x.nombreIcono,
                                 tipoIcono = x.tipoIcono,
                                 habilitado = x.habilitado,
                                 visible = x.visible,
                                 ventana = x.ventana,
                                 razonSocial = x.razonSocial
                             }).ToList();

                if (ventanasAOmitir == null)
                {
                    ventanasAOmitir = new List<string>();
                }
                if (ventanasAOmitir.Count() > 0)
                {
                    for (int i = 0; i < ventanasAOmitir.Count() - 1; i++)
                    {
                        query = (from sub in query
                                 where (sub.ventana == null || (
                                 !ventanasAOmitir.Contains(sub.ventana.nombre)))
                                 orderby sub.ordenId ascending
                                 select sub).ToList();
                    }
                }

                listcontenedor = query.ToList();

                if (usuario != null && listcontenedor != null)
                {
                    listcontenedor = getObtenerContenedoresPersonalizados(listcontenedor, usuario, herramienta);
                }
                if (mensajeResultado.noError != 0)
                {
                    try
                    {
                        getSession().Database.CurrentTransaction.Rollback();
                    }
                    catch (Exception ex)
                    {
                        mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                        mensajeResultado.error = ex.GetBaseException().ToString();
                        mensajeResultado.resultado = null;
                    }
                    return mensajeResultado;
                }

                if (listcontenedor != null && razonSocial != null)
                {
                    query = (from c in getSession().Set<Contenedor>()
                             join v in getSession().Set<Ventana>() on c.ventana.id equals v.id into cv
                             from vv in cv.DefaultIfEmpty()
                             join r in getSession().Set<RazonSocial>() on c.razonSocial.id equals r.id into cr
                             from rr in cr.DefaultIfEmpty()
                             join t in getSession().Set<TipoElemento>() on c.tipoElemento.id equals t.id into ct
                             where c.herramienta.id == herramienta.id &&
                             rr.claveRazonSocial.Equals(razonSocial.claveRazonSocial)
                             select new Contenedor()
                             {
                                 id = c.id,
                                 herramienta = c.herramienta,
                                 nombre = c.nombre,
                                 accion = c.accion,
                                 tipoAcciones = c.tipoAcciones,
                                 ordenId = c.ordenId,
                                 tipoElemento = c.tipoElemento,
                                 nombreIcono = c.nombreIcono,
                                 tipoIcono = c.tipoIcono,
                                 habilitado = c.habilitado,
                                 visible = c.visible,
                                 ventana = vv,
                                 razonSocial = c.razonSocial
                             }).ToList().Select(x => new Contenedor
                             {
                                 id = x.id,
                                 herramienta = x.herramienta,
                                 nombre = x.nombre,
                                 accion = x.accion,
                                 tipoAcciones = x.tipoAcciones,
                                 ordenId = x.ordenId,
                                 tipoElemento = x.tipoElemento,
                                 nombreIcono = x.nombreIcono,
                                 tipoIcono = x.tipoIcono,
                                 habilitado = x.habilitado,
                                 visible = x.visible,
                                 ventana = x.ventana,
                                 razonSocial = x.razonSocial
                             }).ToList();
                    if (ventanasAOmitir == null)
                    {
                        ventanasAOmitir = new List<string>();
                    }
                    if (ventanasAOmitir.Count() > 0)
                    {
                        for (int i = 0; i < ventanasAOmitir.Count() - 1; i++)
                        {
                            query = (from sub in query
                                     where (sub.ventana == null || (
                                     !ventanasAOmitir.Contains(sub.ventana.nombre)))
                                     orderby sub.ordenId ascending
                                     select sub).ToList();

                        }
                    }
                }
                listcontenedor = query.ToList();
                mensajeResultado.resultado = listcontenedor;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ContenedoresPorTipoAcciones()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        private List<Contenedor> getObtenerContenedoresPersonalizados(List<Contenedor> listconte, Usuario usuario, Herramienta herramienta)
        {
            try
            {
                List<ExternoPersonalizado> externoPersonalizados = getExternoPersonalUsuarioHerr(usuario, herramienta);
                if (mensajeResultado.noError != 0)
                {
                    return null;
                }
                if (externoPersonalizados == null ? true : (externoPersonalizados.Count() > 0 ? true : false))
                {
                    externoPersonalizados = getExternoPersonalPerfilHerr(usuario.perfiles, herramienta);
                    if (mensajeResultado.noError != 0)
                    {
                        return null;
                    }
                }
                List<ContenedorPersonalizado> contenedorPersonalizados = new List<ContenedorPersonalizado>();
                contenedorPersonalizados = getContenedorPersonalUsuarioHerr(usuario, herramienta);
                if (mensajeResultado.noError != 0)
                {
                    return null;
                }
                if (contenedorPersonalizados == null ? true : (contenedorPersonalizados.Count() > 0 ? true : false))
                {
                    contenedorPersonalizados = getContenedorPersonalPerfilHerr(usuario.perfiles, herramienta);
                    if (mensajeResultado.noError != 0)
                    {
                        return null;
                    }
                }
                List<ElementoExterno> listexterno = getElementoExAll();
                int i;
                for (i = 0; i < listconte.Count(); i++)
                {
                    foreach (ElementoExterno externo in listexterno)
                    {
                        if (listconte[i].id.Equals(externo.contenedor.id))
                        {
                            listexterno[i].ubicacion = externo.ubicacion;
                        }
                    }
                    foreach (ContenedorPersonalizado cp in contenedorPersonalizados)
                    {
                        /*if (listconte[i].id.Equals(cp.contenedor.id))
                        {
                            listconte[i].nombre = cp.nombre;
                            listconte[i].visible = cp.visible;
                            listconte[i].habilitado = cp.habilitado;
                            listconte[i].ordenId = cp.ordenId;
                            listconte[i].tipoIcono = cp.tipoIcono;
                        }*/
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ObtenerContenedoresPersonalizados()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return listconte;
        }
        private List<ElementoExterno> getElementoExAll()
        {
            List<ElementoExterno> listelemexterno = new List<ElementoExterno>();
            try
            {
                listelemexterno = (from m in getSession().Set<ElementoExterno>()
                                   select m).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ElementoExAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return listelemexterno;
        }
        private List<ContenedorPersonalizado> getContenedorPersonalUsuarioHerr(Usuario user, Herramienta herramienta)
        {
            List<ContenedorPersonalizado> listContenedorPersonalizado = new List<ContenedorPersonalizado>();
            try
            {

                listContenedorPersonalizado = (from cp in getSession().Set<ContenedorPersonalizado>()
                                               where /*cp.usuario.id == user.id ||*/
                                               cp.perfiles.id == user.perfiles.id
                                               /*  && cp.herramienta.id == herramienta.id*/
                                               select cp).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ContenedorPersonalUsuarioHerr()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }

            return listContenedorPersonalizado;
        }
        private List<ContenedorPersonalizado> getContenedorPersonalPerfilHerr(Perfiles perfil, Herramienta herramienta)
        {
            List<ContenedorPersonalizado> listContenedorPersonalizado = new List<ContenedorPersonalizado>();
            try
            {

                listContenedorPersonalizado = (from cp in getSession().Set<ContenedorPersonalizado>()
                                               where cp.perfiles.id == perfil.id
                                               /* && cp.herramienta.id == herramienta.id*/
                                               select cp).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ContenedorPersonalPerfilHerr()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }

            return listContenedorPersonalizado;
        }
        private List<ExternoPersonalizado> getExternoPersonalUsuarioHerr(Usuario user, Herramienta herramienta)
        {
            List<ExternoPersonalizado> ListexternoUserHer = new List<ExternoPersonalizado>();
            try
            {

                ListexternoUserHer = (from exp in getSession().Set<ExternoPersonalizado>()
                                      where exp.usuario.id == user.id ||
                                      exp.perfiles.id == user.perfiles.id &&
                                      exp.herramienta.id == herramienta.id
                                      select exp).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ExternoPersonalUsuarioHerr()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }

            return ListexternoUserHer;
        }
        private List<ExternoPersonalizado> getExternoPersonalPerfilHerr(Perfiles perfil, Herramienta herramienta)
        {
            List<ExternoPersonalizado> ListexternoPerfilHer = new List<ExternoPersonalizado>();
            try
            {

                ListexternoPerfilHer = (from exp in getSession().Set<ExternoPersonalizado>()
                                        where exp.perfiles.id == perfil.id ||
                                        exp.herramienta.id == herramienta.id
                                        select exp).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ExternoPersonalPerfilHerr()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }

            return ListexternoPerfilHer;
        }
        public Mensaje getContenedorHertCompartida(Herramienta herramienta, List<string> ventanasAOmitir, Usuario usuario, RazonSocial razonSocial, DBContextAdapter dbContext)
        {
            List<Contenedor> listcontenedor = new List<Contenedor>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var query = (from c in getSession().Set<Contenedor>()
                             join v in getSession().Set<Ventana>() on c.ventana.id equals v.id into cv
                             from vv in cv.DefaultIfEmpty()
                             join r in getSession().Set<RazonSocial>() on c.razonSocial.id equals r.id into cr
                             from rr in cr.DefaultIfEmpty()
                             join t in getSession().Set<TipoElemento>() on c.tipoElemento.id equals t.id into ct
                             where c.herramienta.id == herramienta.id &&
                             rr == null
                             select new Contenedor()
                             {
                                 id = c.id,
                                 herramienta = c.herramienta,
                                 nombre = c.nombre,
                                 accion = c.accion,
                                 tipoAcciones = c.tipoAcciones,
                                 ordenId = c.ordenId,
                                 tipoElemento = c.tipoElemento,
                                 nombreIcono = c.nombreIcono,
                                 tipoIcono = c.tipoIcono,
                                 habilitado = c.habilitado,
                                 visible = c.visible,
                                 ventana = vv,
                                 razonSocial = c.razonSocial
                             }).ToList().Select(x => new Contenedor
                             {
                                 id = x.id,
                                 herramienta = x.herramienta,
                                 nombre = x.nombre,
                                 accion = x.accion,
                                 tipoAcciones = x.tipoAcciones,
                                 ordenId = x.ordenId,
                                 tipoElemento = x.tipoElemento,
                                 nombreIcono = x.nombreIcono,
                                 tipoIcono = x.tipoIcono,
                                 habilitado = x.habilitado,
                                 visible = x.visible,
                                 ventana = x.ventana,
                                 razonSocial = x.razonSocial
                             }).ToList();

                if (ventanasAOmitir == null)
                {
                    ventanasAOmitir = new List<string>();
                }
                if (ventanasAOmitir.Count() > 0)
                {
                    for (int i = 0; i < ventanasAOmitir.Count() - 1; i++)
                    {
                        query = (from sub in query
                                 where (sub.ventana == null || (
                                 !ventanasAOmitir.Contains(sub.ventana.nombre)))
                                 orderby sub.ordenId ascending
                                 select sub).ToList();

                    }
                }

                listcontenedor = query.ToList();

                if (usuario != null && listcontenedor != null)
                {
                    listcontenedor = getObtenerContenedoresPersonalizados(listcontenedor, usuario, herramienta);
                }
                if (mensajeResultado.noError != 0)
                {
                    try
                    {
                        getSession().Database.CurrentTransaction.Rollback();
                    }
                    catch (Exception ex)
                    {
                        mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                        mensajeResultado.error = ex.GetBaseException().ToString();
                        mensajeResultado.resultado = null;
                    }
                    return mensajeResultado;
                }

                if (listcontenedor != null && razonSocial != null)
                {
                    query = (from c in getSession().Set<Contenedor>()
                             join v in getSession().Set<Ventana>() on c.ventana.id equals v.id into cv
                             from vv in cv.DefaultIfEmpty()
                             join r in getSession().Set<RazonSocial>() on c.razonSocial.id equals r.id into cr
                             from rr in cr.DefaultIfEmpty()
                             join t in getSession().Set<TipoElemento>() on c.tipoElemento.id equals t.id into ct
                             where c.herramienta.id == herramienta.id &&
                             rr.claveRazonSocial.Equals(razonSocial.claveRazonSocial)
                             select new Contenedor()
                             {
                                 id = c.id,
                                 herramienta = c.herramienta,
                                 nombre = c.nombre,
                                 accion = c.accion,
                                 tipoAcciones = c.tipoAcciones,
                                 ordenId = c.ordenId,
                                 tipoElemento = c.tipoElemento,
                                 nombreIcono = c.nombreIcono,
                                 tipoIcono = c.tipoIcono,
                                 habilitado = c.habilitado,
                                 visible = c.visible,
                                 ventana = vv,
                                 razonSocial = c.razonSocial
                             }).ToList().Select(x => new Contenedor
                             {
                                 id = x.id,
                                 herramienta = x.herramienta,
                                 nombre = x.nombre,
                                 accion = x.accion,
                                 tipoAcciones = x.tipoAcciones,
                                 ordenId = x.ordenId,
                                 tipoElemento = x.tipoElemento,
                                 nombreIcono = x.nombreIcono,
                                 tipoIcono = x.tipoIcono,
                                 habilitado = x.habilitado,
                                 visible = x.visible,
                                 ventana = x.ventana,
                                 razonSocial = x.razonSocial
                             }).ToList();
                    if (ventanasAOmitir == null)
                    {
                        ventanasAOmitir = new List<string>();
                    }
                    if (ventanasAOmitir.Count() > 0)
                    {
                        for (int i = 0; i < ventanasAOmitir.Count() - 1; i++)
                        {
                            query = (from sub in query
                                     where (sub.ventana == null || (
                                     !ventanasAOmitir.Contains(sub.ventana.nombre)))
                                     orderby sub.ordenId ascending
                                     select sub).ToList();

                        }
                    }
                }
                listcontenedor = query.ToList();
                mensajeResultado.resultado = listcontenedor;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ContenedorHertCompartida()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getContenedorMax(DBContextAdapter dbContext)
        {
            int tam = 0;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                tam = (from c in getSession().Set<Contenedor>()
                       select new { c.id }).Max(p => p.id) + 1;
                mensajeResultado.resultado = tam;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ContenedorMax()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getContenedorOrder(List<string> ventanasAOmitir, DBContextAdapter dbContext)
        {
            List<Contenedor> listcont = new List<Contenedor>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var query = (from c in getSession().Set<Contenedor>()
                             join v in getSession().Set<Ventana>() on c.ventana.id equals v.id into cv
                             from vv in cv.DefaultIfEmpty()
                             select c).ToList();
                if (ventanasAOmitir == null)
                {
                    ventanasAOmitir = new List<string>();
                }
                if (ventanasAOmitir.Count() > 0)
                {
                    for (int i = 0; i < ventanasAOmitir.Count() - 1; i++)
                    {
                        query = (from sub in query
                                 where (sub.ventana == null || (
                                 !ventanasAOmitir.Contains(sub.ventana.nombre)))
                                 orderby sub.ordenId ascending
                                 select sub).ToList();

                    }
                }
                listcont = query.ToList();
                mensajeResultado.resultado = listcont;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ContenedorOrder()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getContenedorPorId(int id, DBContextAdapter dbContext)
        {
            Contenedor contenedor = new Contenedor();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                contenedor = (from c in getSession().Set<Contenedor>()
                              where c.id == id
                              select c).SingleOrDefault();
                //contenedor= findById(id);
                mensajeResultado.resultado = contenedor;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ContenedorPorId()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getContenedorPorNombreVentana(string nombreVentana, DBContextAdapter dbContext)
        {
            List<Contenedor> listcontenedorXnombre = new List<Contenedor>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listcontenedorXnombre = (from c in getSession().Set<Contenedor>()
                                         where c.ventana.nombre.Equals(nombreVentana) &&
                                         c.tipoAcciones.Equals(TipoAcciones.VENTANA)
                                         select c).ToList();
                mensajeResultado.resultado = listcontenedorXnombre;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ContenedorPorNombreVentana()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje buscaPorTipoAccionesYidMultiUsos(TipoAcciones[] tipoAcciones, decimal?[] idMultiUsos, string claveRazonSocial, DBContextAdapter dbContext)
        {
            List<Contenedor> listContenedor = new List<Contenedor>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (claveRazonSocial != null)
                {
                    listContenedor = (from c in getSession().Set<Contenedor>()
                                      join r in getSession().Set<RazonSocial>() on c.razonSocial.id equals r.id into com
                                      from r in com.DefaultIfEmpty()
                                      where tipoAcciones.Contains(c.tipoAcciones) &&
                                      r.claveRazonSocial.Equals(claveRazonSocial)
                                      orderby c.ordenId
                                      select c).ToList();
                }
                else
                {
                    listContenedor = (from c in getSession().Set<Contenedor>()
                                      join r in getSession().Set<RazonSocial>() on c.razonSocial.id equals r.id into com
                                      from r in com.DefaultIfEmpty()
                                      where tipoAcciones.Contains(c.tipoAcciones)
                                      orderby c.ordenId
                                      select c).ToList();
                }
                mensajeResultado.resultado = listContenedor;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ConfiguraTimbradoPrincipal()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getContenedorPorTipoHerramienta(int tipoherramientaId, DBContextAdapter dbContext)
        {
            try
            {

                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();

                var listcontenedor = (from c in getSession().Set<Contenedor>()
                                      where c.tipoHerramienta_ID == tipoherramientaId
                                      orderby c.categoriaHerramienta.id
                                      select new
                                      {
                                          idCather = (c.categoriaHerramienta.id == null ? -1 : c.categoriaHerramienta.id),
                                          nombreCather = c.categoriaHerramienta.nombre == null ? "" : c.categoriaHerramienta.nombre,
                                          titulo = (c.tipoHerramienta_ID == null ? (c.herramienta_ID == null ? "" : c.herramienta.nombre) : c.tipoHerramienta.nombre),
                                          titulodesc = (c.tipoHerramienta_ID == null ? (c.herramienta_ID == null ? "" : c.herramienta.descripcion) : c.tipoHerramienta.descripcion),
                                          c.id,
                                          c.nombre,
                                          c.accion,
                                          c.nombreIcono,
                                          c.descripcion
                                      }

                                     ).ToList();

                List<Object> categoria = new List<Object>();
                List<Object> contenedores = new List<Object>();

                bool change = true;

                for (int i = 0; i < listcontenedor.Count(); i++)
                {
                    if (i > 0)
                    {
                        Object[] item = (Object[])categoria[categoria.Count - 1];

                        if (item[0].Equals(listcontenedor[i].idCather))
                        {
                            change = false;
                        }
                        else
                        {
                            change = true;
                            contenedores = new List<Object>();
                        }

                    }
                    contenedores.Add(new Object[] { listcontenedor[i].id, listcontenedor[i].nombre, listcontenedor[i].accion, listcontenedor[i].nombreIcono, listcontenedor[i].descripcion });
                    if (change)
                    {
                        categoria.Add(new Object[] { listcontenedor[i].idCather, listcontenedor[i].nombreCather, contenedores, listcontenedor[i].titulo, listcontenedor[i].titulodesc });
                    }

                    change = false;
                }
                mensajeResultado.resultado = categoria;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
                getSession().Database.Connection.Close();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ContenedoresPorTipoAcciones()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getContenedorIDMax(DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                decimal idMax = (from c in getSession().Set<Contenedor>()
                                  select c.id).Max(p=> p);
                mensajeResultado.resultado = idMax;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getContenedorIDMax()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getContenedorPorControlForma(string claveControl, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var contenedor = (from c in getSession().Set<Contenedor>()
                                 where c.controlPorForma == claveControl
                                 select new { 
                                 c.accesoMenu,
                                 c.accion,
                                 c.categoriaHerramienta_ID,
                                 c.controlPorForma,
                                 c.descripcion,
                                 
                                 c.habilitado,
                                 c.herramienta_ID,
                                 c.id,
                                 c.nombre,
                                 c.nombreIcono,
                                 c.ordenId,
                                 c.razonSocial_ID,
                                 c.tipoElemento_ID,
                                 c.tipoHerramienta_ID,
                                 c.tipoIcono,
                                 c.ventana_ID,
                                 c.visible
                                 }).SingleOrDefault();

                mensajeResultado.resultado = contenedor;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getContenedorPorControlForma()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
    }
}