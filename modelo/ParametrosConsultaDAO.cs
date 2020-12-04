/**
* @author: Daniel Ruelas 
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Clase ParametrosConsultaDAO para llamados a metodos de Entity
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
using Exitosw.Payroll.Entity.entidad;
using System.Text;
using System.Reflection;
using Exitosw.Payroll.Core.util;
using System.Linq;
using System.Data.Entity.Migrations;
using Exitosw.Payroll.Core.genericos.campos;
using System.Data.Entity;
using Exitosw.Payroll.Core.campos;

namespace Exitosw.Payroll.Core.modelo
{

    public class ParametrosConsultaDAO : GenericRepository<ParametrosConsulta>, ParametrosConsultaDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");


        public Mensaje agregar(ParametrosConsulta entity, Contenedor contenedorGrupoMenu, DBContextAdapter dbContextMaestra)
        {
            ParametrosConsulta parametroConsulta= new ParametrosConsulta();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaestra.context);
                getSession().Database.BeginTransaction();
                if (entity.contenedorGrupo == null)
                {
                    IQueryable<TipoElemento> sentecia = (from t in getSession().Set<TipoElemento>()
                                                         where t.id == 2
                                                         select t);

                    TipoElemento tipoElemento = sentecia.SingleOrDefault();

                    int id = getSession().Set<Contenedor>().DefaultIfEmpty().Max(c => (c == null ? 0 : c.id));

                    int ordenId = getSession().Set<Contenedor>().DefaultIfEmpty().Max(c => (c == null ? 0 : c.ordenId));

                    if (id == 0)
                    {
                        id = 1;
                    }
                    else
                    {
                        id++;
                    }
                    if (ordenId == 0)
                    {
                        ordenId = 1;
                    }
                    else
                    {
                        ordenId++;
                    }
                    Contenedor contenedor = new Contenedor();
                    contenedor.id = id;
                    /*contenedor.parentId = contenedorGrupoMenu.id;*/
                    contenedor.ordenId = ordenId;
                    contenedor.tipoElemento = (tipoElemento == null ? contenedorGrupoMenu.tipoElemento : tipoElemento);
                    contenedor.herramienta = contenedorGrupoMenu.herramienta;
                    contenedor.tipoAcciones = TipoAcciones.GRUPOCONSULTA;
                    entity.contenedorGrupo = contenedor;
                    parametroConsulta = getSession().Set<ParametrosConsulta>().Add(entity);
//                    contenedor.idMultiUsos = parametroConsulta.id;
                    getSession().Set<Contenedor>().AddOrUpdate(contenedor);

                }
                else
                {

                    getSession().Set<ParametrosConsulta>().Add(entity);

                }
                getSession().SaveChanges();
                mensajeResultado.resultado = parametroConsulta;
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
        public Mensaje actualizar(ParametrosConsulta entity, Contenedor contenedorGrupoMenu, DBContextAdapter dbContextMaestra)
        {
            ParametrosConsulta parametrosConsulta;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaestra.context);
                getSession().Database.BeginTransaction();
                if (entity.contenedorGrupo==null) {
                    TipoElemento tipoElemento = (from te in getSession().Set<TipoElemento>()
                                                 where te.id==2
                                                 select te).SingleOrDefault();
                    int id = (from c in getSession().Set<Contenedor>()
                              select new { c.id }).Max(p=>p.id);
                    int ordenId = (from c in getSession().Set<Contenedor>()
                                   select new { c.ordenId }).Max(p => p.ordenId);
                    if (id >0)
                    {
                        id = 1;
                    }
                    else
                    {
                        id++;
                    }

                    if (ordenId >0)
                    {
                        ordenId = 1;
                    }
                    else
                    {
                        ordenId++;
                    }

                    Contenedor con = new Contenedor(entity);
                    con.id = id;
                    /*con.parentId = contenedorGrupoMenu.id;*/
                    con.ordenId = ordenId;
                    con.tipoElemento = tipoElemento == null ? contenedorGrupoMenu.tipoElemento : tipoElemento;
                    con.herramienta = contenedorGrupoMenu.herramienta;
                    con.tipoAcciones = TipoAcciones.GRUPOCONSULTA;
                    entity.contenedorGrupo = con;
                    parametrosConsulta = getSession().Set<ParametrosConsulta>().Add(entity);
                   // con.idMultiUsos = parametrosConsulta.id;
                    getSession().Set<Contenedor>().AddOrUpdate(con);
                }
                else
                {
                    getSession().Set<ParametrosConsulta>().Add(entity);
                    
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
            return mensajeResultado;
        }
        public Mensaje eliminar(ParametrosConsulta entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                //var sentencia = (from g in getSession().Set<Genero>()
                //                 where g.id == entity.id
                //                 select g).FirstOrDefault();
                getSession().Set<ParametrosConsulta>().Attach(entity);
                getSession().Set<ParametrosConsulta>().Remove(entity);
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

        public Mensaje consultaPorFiltrosParametrosConsulta(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            List<ParametrosConsulta> parametrosConsulta = new List<ParametrosConsulta>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();

                List<CamposWhere> camposwheres = new List<CamposWhere>();
                foreach (var item in campos)
                {
                    if (!item.Value.ToString().Equals("") && item.Value != null)
                    {
                        CamposWhere campo = new CamposWhere();
                        campo.campo = "ParametrosConsulta." + item.Key.ToString();
                        campo.valor = item.Value;
                        if (operador == "=")
                        {
                            campo.operadorComparacion = OperadorComparacion.IGUAL;
                        }
                        else if (operador == "like")
                        {
                            campo.operadorComparacion = OperadorComparacion.LIKE;
                        }
                        campo.operadorLogico = OperadorLogico.AND;
                        camposwheres.Add(campo);
                    }


                }
                ValoresRango rangos = new ValoresRango(Convert.ToInt32(inicio), Convert.ToInt32(rango));
                mensajeResultado.resultado = consultaPorRangos(rangos, camposwheres, null);
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorFiltrosParametros()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje consultaPorRangosParametrosConsulta(Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            List<ParametrosConsulta> parametrosConsulta = new List<ParametrosConsulta>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();

                ValoresRango rangos = new ValoresRango(Convert.ToInt32(inicio), Convert.ToInt32(rango));

                mensajeResultado = consultaPorRangos(rangos, null, null);
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorRangos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }


        public Mensaje eliminarEspecifico(long idReporte, DBContextAdapter dbContextMaestra)
        {
            ParametrosConsulta parametrosConsulta;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaestra.context);
                getSession().Database.BeginTransaction();
                parametrosConsulta = (from pc in getSession().Set<ParametrosConsulta>()
                                      where pc.id == idReporte
                                      select pc).SingleOrDefault();
                getSession().Set<ParametrosConsulta>().Add(parametrosConsulta);


                mensajeResultado.resultado = parametrosConsulta;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("eliminarEspecifico()1_Error: ").Append(ex));
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

        public Mensaje getAllParametrosConsulta(DBContextAdapter dbContext)
        {
            List<ParametrosConsulta> parametrosConsulta=new List<ParametrosConsulta>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                parametrosConsulta = (from p in getSession().Set<ParametrosConsulta>()
                                      select p).ToList();

                mensajeResultado.resultado = parametrosConsulta;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getParametrosConsultaAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getParametrosConsultaAllEspecifico(DBContextAdapter dbContextMaestra)
        {
            List<ParametrosConsulta> parametrosConsulta = new List<ParametrosConsulta>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaestra.context);
                getSession().Database.BeginTransaction();
                parametrosConsulta = (from p in getSession().Set<ParametrosConsulta>()
                                      select new ParametrosConsulta()
                                      {
                                          id = p.id,
                                          nombre = p.nombre,
                                          nombreAbreviado=p.nombreAbreviado,
                                          reporteFuenteDatos=p.reporteFuenteDatos,
                                          contenedorGrupo=p.contenedorGrupo

                                      }).ToList();

                mensajeResultado.resultado = parametrosConsulta;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getParametrosConsultaAllEspecifico()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getParametrosConsultaPorID(decimal idParametrosConsulta, DBContextAdapter dbContextMaestra)
        {

            ParametrosConsulta parametrosConsulta;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaestra.context);
                getSession().Database.BeginTransaction();
                parametrosConsulta = (from p in getSession().Set<ParametrosConsulta>()
                                      where p.id == idParametrosConsulta
                                      select p).SingleOrDefault();

                mensajeResultado.resultado = parametrosConsulta;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getParametrosConsultaPorID()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje PorGrupoMenuFuenteDatos(string fuenteDatos, long idContenedor, DBContextAdapter dbContextMaestra)
        {
            List<ParametrosConsulta> parametrosConsulta = new List<ParametrosConsulta>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaestra.context);
                getSession().Database.BeginTransaction();
                fuenteDatos = fuenteDatos == null ? "" : fuenteDatos;
                idContenedor = idContenedor == 0 ? 0 : idContenedor;
                var query = from p in getSession().Set<ParametrosConsulta>()
                            select p;
                if (fuenteDatos.Length>0) {
                    query = from sub in query
                            where sub.reporteFuenteDatos.clave == fuenteDatos
                            select sub;
                }
                if (idContenedor>0) {
                    query = from sub2 in query
                            select sub2;
                }
                parametrosConsulta = (from p in query
                                      orderby p.contenedorGrupo.id, p.nombre
                                      select new ParametrosConsulta()
                                      {

                                          id = p.id,
                                          nombre = p.nombre,
                                          nombreAbreviado = p.nombreAbreviado,
                                          reporteFuenteDatos = p.reporteFuenteDatos,
                                          contenedorGrupo = p.contenedorGrupo
                                      }).ToList();
                           
                mensajeResultado.resultado = parametrosConsulta;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("PorGrupoMenuFuenteDatos()1_Error: ").Append(ex));
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
    }
}