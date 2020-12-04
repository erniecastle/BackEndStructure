/**
* @author: Daniel Ruelas 
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Clase ReporteDinamicoDAO para llamados a metodos de Entity
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
using Exitosw.Payroll.Core.campos;

namespace Exitosw.Payroll.Core.modelo
{
    public class ReporteDinamicoDAO : GenericRepository<ReporteDinamico>, ReporteDinamicoDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        bool commit;
        private List<ReporteDinamico> listDinamico = new List<ReporteDinamico>();
        public Mensaje actualizaListaPorCampos(string[] campoModificar, object[] valoresModificado, string[] camposWhere, object[] valoresWhere, DBContextAdapter dbContext)
        {
            int resultado = 0;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();

                getSession().SaveChanges();
                mensajeResultado.resultado = resultado;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("actualizaListaPorCampos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje agregar(ReporteDinamico entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<ReporteDinamico>().Add(entity);
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
        public Mensaje actualizar(ReporteDinamico entity, DBContextAdapter dbContext)
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
                getSession().Set<ReporteDinamico>().AddOrUpdate(entity);
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

        public Mensaje eliminar(ReporteDinamico entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                //var sentencia = (from g in getSession().Set<Genero>()
                //                 where g.id == entity.id
                //                 select g).FirstOrDefault();
                getSession().Set<ReporteDinamico>().Attach(entity);
                getSession().Set<ReporteDinamico>().Remove(entity);
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

        public Mensaje consultaPorFiltrosReporteDinamico(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
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
                        campo.campo = "ReporteDinamico." + item.Key.ToString();
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorFiltrosReporte()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje consultaPorRangosReporteDinamico(Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
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



        public Mensaje eliminarEspecifico(decimal idReporte, DBContextAdapter dbContext)
        {
            ReporteDinamico entity = null;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                entity = (from r in getSession().Set<ReporteDinamico>()
                          where r.id == idReporte
                          select r).SingleOrDefault();
                getSession().Set<ReporteDinamico>().Attach(entity);
                getSession().Set<ReporteDinamico>().Remove(entity);
                //deleteListQueryConFiltrado(Contenedor.class.getSimpleName(), "idMultiUsos", new Long[]{idReporte
                ///}, new String[]{"tipoAcciones"}, new TipoAcciones[]{TipoAcciones.GRUPOREPORTE});
                mensajeResultado.resultado = entity;
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

        //public Mensaje existeDato(string campo, object valor, DBContextAdapter dbContext)
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

        public Mensaje getAllReporteDinamico(DBContextAdapter dbContext)
        {
            List<ReporteDinamico> reporteDinamico = new List<ReporteDinamico>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var query = (from p in getSession().Set<ReporteDinamico>()
                             orderby p.Contenedor.id, p.clave
                             select p);
                reporteDinamico = query.ToList();
                mensajeResultado.resultado = reporteDinamico;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getReporteDinamicoAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getReporteDinamicoAllEspecificos(DBContextAdapter dbContext)
        {
            List<Object> reporteDinamico = new List<Object>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var query = (from p in getSession().Set<ReporteDinamico>()
                             orderby p.Contenedor.id, p.clave
                             select new
                             {
                                 id = p.id,
                                 clave = p.clave,
                                 nombre = p.nombre,
                                 nombreAbreviado = p.nombreAbreviado,
                                 reporteFuenteDatosnombre = p.reporteFuenteDatos.nombre
                             });
                reporteDinamico = query.ToList<Object>();
                mensajeResultado.resultado = reporteDinamico;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getReporteDinamicoAllEspecificos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorClaveReporteDinamico(string clave, DBContextAdapter dbContext)
        {
            ReporteDinamico reporteDinamico;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                reporteDinamico = (from p in getSession().Set<ReporteDinamico>()
                                   where p.clave == clave
                                   select p).SingleOrDefault();

                mensajeResultado.resultado = reporteDinamico;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getReporteDinamicoPorClave()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getReporteDinamicoPorContenedor(int contenedorID, DBContextAdapter dbContext)
        {
            List<ReporteDinamico> reporteDinamico = new List<ReporteDinamico>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var query = (from p in getSession().Set<ReporteDinamico>()
                             where p.Contenedor.id == contenedorID
                             orderby p.Contenedor.id, p.nombre
                             select p);
                reporteDinamico = query.ToList();
                mensajeResultado.resultado = reporteDinamico;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getReporteDinamicoPorContenedor()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getReporteDinamicoPorFuenteYGrupo(string fuenteDatos, int idContenedor, DBContextAdapter dbContext)
        {
            List<Object> reporteDinamico = new List<Object>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var query = (from p in getSession().Set<ReporteDinamico>()
                             select p);

                //id = p.id,
                //clave = p.clave,
                //nombre = p.nombre,
                //nombreAbreviado = p.nombreAbreviado,
                //reporteFuenteDatosnombre = p.reporteFuenteDatos.nombre

                fuenteDatos = fuenteDatos == null ? "" : fuenteDatos;
                idContenedor = idContenedor == 0 ? -1 : idContenedor;
                if (fuenteDatos.Length > 0)
                {
                    query = from sub in query
                            where sub.reporteFuenteDatos.clave == fuenteDatos
                            select sub;
                }
                if (idContenedor > 0)
                {
                    query = from sub2 in query
                            where sub2.Contenedor.id == idContenedor
                            select sub2;
                }
                var query2 = from sub3 in query
                             orderby sub3.Contenedor.id, sub3.clave
                             select new
                             {
                                 id = sub3.id,
                                 clave = sub3.clave,
                                 nombre = sub3.nombre,
                                 nombreAbreviado = sub3.nombreAbreviado,
                                 reporteFuenteDatosnombre = sub3.reporteFuenteDatos.nombre
                             };
                reporteDinamico = query2.ToList<Object>();
                mensajeResultado.resultado = reporteDinamico;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getReporteDinamicoAllEspecificos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getReporteDinamicoPorID(decimal idReporte, DBContextAdapter dbContext)
        {
            ReporteDinamico reporteDinamico;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                reporteDinamico = (from p in getSession().Set<ReporteDinamico>()
                                   where p.id == idReporte
                                   select p).SingleOrDefault();

                mensajeResultado.resultado = reporteDinamico;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getReporteDinamicoPorID()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje saveDeleteReporteDinamico(ReporteDinamico entity, object[] eliminarDatosConsulta, object[] eliminarDatosIncluir, object[] eliminarDatosRepOpcGrupo, object[] eliminarDatosOrdenGrupo, object[] eliminarCamposWhere, object[] eliminarCamposEncabezados, object[] eliminarDatosResumen, object[] eliminarReporteEstilos, Contenedor contenedorGrupoMenu, DBContextAdapter dbContext)
        {
            ReporteDinamico p = null;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (entity.Contenedor == null)
                {
                    TipoElemento tipoElemento = (from t in getSession().Set<TipoElemento>()
                                                 where t.id == 2
                                                 select t).SingleOrDefault();
                    int id = (from con in getSession().Set<Contenedor>()
                              select new { con.id }).Max(s => s.id);

                    int ordenId = (from co in getSession().Set<Contenedor>()
                                   select new { co.ordenId }).Max(s => s.ordenId);
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
                    Contenedor c = new Contenedor(entity);
                    c.id = id;
                   /* c.parentId = contenedorGrupoMenu.parentId;*/
                    c.ordenId = ordenId;
                    c.tipoElemento = tipoElemento == null ? contenedorGrupoMenu.tipoElemento : tipoElemento;
                    c.herramienta = contenedorGrupoMenu.herramienta;
                    c.tipoAcciones = TipoAcciones.GRUPOREPORTE;
                    entity.Contenedor = c;
                    getSession().Set<ReporteDinamico>().AddOrUpdate(entity);
                    //c.idMultiUsos = entity.id;
                    getSession().Set<Contenedor>().AddOrUpdate(c);
                    getSession().SaveChanges();

                }
                else {
                    getSession().Set<ReporteDinamico>().AddOrUpdate(entity);
                }
                p = entity;
                if (eliminarDatosIncluir == null ? false : eliminarDatosIncluir.Length > 0 ? true : false)
                {
                    //deleteListQuery("ReporteDatosIncluir", "Id", eliminarDatosIncluir);
                    deleteListQuery("ReporteDatosIncluir", new CamposWhere("ReporteDatosIncluir.id", eliminarDatosIncluir, OperadorComparacion.IN, OperadorLogico.AND), dbContext);

                }
                if (eliminarDatosRepOpcGrupo == null ? false : eliminarDatosRepOpcGrupo.Length > 0 ? true : false)
                {
                    //     deleteListQuery(ReporteOpcionGrupos.class.getCanonicalName(), "Id", eliminarDatosRepOpcGrupo);
                    deleteListQuery("ReporteOpcionGrupos", new CamposWhere("ReporteOpcionGrupos.id", eliminarDatosRepOpcGrupo, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                if (eliminarDatosOrdenGrupo == null ? false : eliminarDatosOrdenGrupo.Length > 0 ? true : false)
                {
                    //7   deleteListQuery(ReporteOrdenGrupo.class.getCanonicalName(), "Id", eliminarDatosOrdenGrupo);
                    deleteListQuery("ReporteOrdenGrupo", new CamposWhere("ReporteOrdenGrupo.id", eliminarDatosOrdenGrupo, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                if (eliminarDatosResumen == null ? false : eliminarDatosResumen.Length > 0 ? true : false)
                {
                    ///deleteListQuery(ReporteDatosResumen.class.getCanonicalName(), "Id", eliminarDatosResumen);
                    deleteListQuery("ReporteDatosResumen", new CamposWhere("ReporteDatosResumen.id", eliminarDatosResumen, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }

                if (eliminarDatosConsulta == null ? false : eliminarDatosConsulta.Length > 0 ? true : false)
                {
                    //deleteListQuery(DatosConsulta.class.getCanonicalName(), "Id", eliminarDatosConsulta);
                    deleteListQuery("DatosConsulta", new CamposWhere("DatosConsulta.id", eliminarDatosConsulta, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                if (eliminarCamposWhere == null ? false : eliminarCamposWhere.Length > 0 ? true : false)
                {
                    //deleteListQuery(ReporteCamposWhere.class.getCanonicalName(), "Id", eliminarCamposWhere);
                    deleteListQuery("ReporteCamposWhere", new CamposWhere("ReporteCamposWhere.id", eliminarCamposWhere, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                if (eliminarCamposEncabezados == null ? false : eliminarCamposEncabezados.Length > 0 ? true : false)
                {
                    //deleteListQuery(ReporteOtrosDatosEncabezado.class.getCanonicalName(), "reporteCamposEncabezado_id", eliminarCamposEncabezados);
                    deleteListQuery("ReporteOtrosDatosEncabezado", new CamposWhere("ReporteOtrosDatosEncabezado.ReporteCamposEncabezado.id", eliminarCamposEncabezados, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                    //deleteListQuery(ReporteCamposEncabezado.class.getCanonicalName(), "Id", eliminarCamposEncabezados);
                    deleteListQuery("ReporteCamposEncabezado", new CamposWhere("ReporteCamposEncabezado.id", eliminarCamposEncabezados, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }

                if (eliminarReporteEstilos == null ? false : eliminarReporteEstilos.Length > 0 ? true : false)
                {
                    //deleteListQuery(ReporteEstilos.class.getCanonicalName(), "Id", eliminarReporteEstilos);
                    deleteListQuery("ReporteEstilos", new CamposWhere("ReporteEstilos.id", eliminarReporteEstilos, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                getSession().SaveChanges();
                mensajeResultado.resultado = p;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteReporteDinamico()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        private void deleteListQuery(Object[] eliminados, DBContextAdapter dbContext)
        {

            try
            {
                //deleteListQuery("ReporteDinamico", "Id", eliminados);
                deleteListQuery("ReporteDinamico", new CamposWhere("ReporteDinamico.id", eliminados, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("deleteListQuery()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                commit = false;
            }
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