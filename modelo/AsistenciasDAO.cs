/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: clase AsistenciasDAO para llamados a metodos de Entity
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
using Exitosw.Payroll.Core.util;
using System.Text;
using System.Reflection;
using Exitosw.Payroll.Core.genericos.campos;
using Exitosw.Payroll.Entity.util;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace Exitosw.Payroll.Core.modelo
{
    public class AsistenciasDAO : GenericRepository<Object>, AsistenciasDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");

        public Mensaje getAllAsistencias(string claveRazonesSociales, DBContextAdapter dbContext)
        {
            // List<Asistencias> listasistencias = new List<Asistencias>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var listasistencias = (from a in getSession().Set<Asistencias>()
                                       select new
                                       {
                                           a.cantidad,
                                           a.centroDeCosto_ID,
                                           a.empleados_ID,
                                           a.excepciones_ID,
                                           a.fecha,
                                           a.id,
                                           a.jornada,
                                           a.ordenId,
                                           a.periodosNomina_ID,
                                           a.razonesSociales_ID,
                                           a.tipoNomina_ID,
                                           a.tipoPantalla
                                       }).ToList();
                mensajeResultado.resultado = listasistencias;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("AsistenciasAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getAsistenciasPorRangoFechas(string Asistencias, DateTime fechaInicio, DateTime fechaFinal, string claveRazonesSociales, DBContextAdapter dbContext)
        {
            List<Asistencias> listasistencias = new List<Asistencias>();

            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listasistencias = (from a in getSession().Set<Asistencias>()
                                   where a.empleados.clave == Asistencias &&
                                   a.fecha >= fechaInicio && a.fecha <= fechaFinal &&
                                   a.razonesSociales.clave == claveRazonesSociales &&
                                   a.empleados.razonesSociales.clave == claveRazonesSociales
                                   select a).ToList();
                mensajeResultado.resultado = listasistencias;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("AsistenciasPorRangoFechas()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje saveDeleteAsistencias(List<Asistencias> AgreModif, List<Asistencias> Ordenados, object[] clavesDelete, List<DetalleAsistencia> AgreModifDet, object[] clavesDeleteDet, List<RegistroIncapacidad> incapacidades, object[] clavesDeleteIncapacidades, DBContextAdapter dbContext)
        {
            bool exito = true;
            int order = 0;
            Asistencias asistencia = null;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (clavesDelete != null)
                {
                    if (clavesDelete.Count() > 0)
                    {
                        exito = deleteListQuerys("Asistencias", new CamposWhere("Asistencias.id", clavesDelete, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                    }
                }
                //Guardado de modificados
                if (exito)
                {
                    AgreModif = (AgreModif == null ? new List<Asistencias>() : AgreModif);
                    foreach (Asistencias asis in AgreModif)
                    {
                        asistencia = asis;
                        getSession().Set<Asistencias>().Add(asis);
                        getSession().SaveChanges();
                    }
                    Ordenados = (Ordenados == null ? new List<Asistencias>() : Ordenados);
                    if (Ordenados.Count() > 0)
                    {
                        order = Ordenados[0].ordenId;
                    }
                    //Guardado de Ordenados
                    foreach (Asistencias ord in Ordenados)
                    {
                        ord.ordenId = order;
                        asistencia = ord;
                        getSession().Set<Asistencias>().Add(ord);
                        getSession().SaveChanges();
                        order++;
                    }
                    if (clavesDeleteDet != null)
                    {
                        exito =
                        deleteListQuerys("DetalleAsistencia", new CamposWhere("DetalleAsistencia.id", clavesDeleteDet, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                    }
                    if (exito)
                    {
                        //Guardado de detalles
                        AgreModifDet = (AgreModifDet == null ? new List<DetalleAsistencia>() : AgreModifDet);
                        foreach (DetalleAsistencia detalles in AgreModifDet)
                        {
                            getSession().Set<DetalleAsistencia>().Add(detalles);
                            getSession().SaveChanges();
                        }
                        exito = operacionIncidencias(incapacidades, clavesDeleteIncapacidades, dbContext);
                    }
                }
                if (exito)
                {
                    asistencia = null;
                    mensajeResultado.resultado = asistencia;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }
                else
                {
                    mensajeResultado.resultado = asistencia;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteAsistencias()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        private bool operacionIncidencias(List<RegistroIncapacidad> incapacidades, Object[] clavesDeleteIncapacidades, DBContextAdapter dbContext)
        {
            bool exito = true;
            try
            {
                if (clavesDeleteIncapacidades != null)
                {
                    if (clavesDeleteIncapacidades.Count() > 0)
                    {
                        exito =
                        deleteListQuerys(typeof(RegistroIncapacidad).Name, new CamposWhere("RegistroIncapacidad.id", clavesDeleteIncapacidades, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                    }
                }
                if (exito)
                {
                    incapacidades = incapacidades == null ? new List<RegistroIncapacidad>() : incapacidades;
                    int i;
                    for (i = 0; i < incapacidades.Count(); i++)
                    {
                        getSession().Set<RegistroIncapacidad>().Add(incapacidades[i]);
                        getSession().SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("operacionIncidencias()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
                exito = false;
            }
            return exito;
        }
        private bool deleteListQuerys(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext)
        {
            try
            {
                deleteListQuery(tabla, campoWhere, dbContext);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("deleteListQuerys()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
                return false;
            }
            return true;
        }

        public Mensaje getAsistenciaPorFiltros(string clavetipoNomina, decimal idPeriodoNomina, string claveEmpleado, string claveRazonSocial, string claveCentroCosto, string claveExepcion, DBContextAdapter dbcontext)
        {
            // List<Asistencias> listasistencias = new List<Asistencias>();

            try
            {
                inicializaVariableMensaje();
                setSession(dbcontext.context);
                getSession().Database.BeginTransaction();
                var query = from o in getSession().Set<Asistencias>()
                            where o.tipoNomina.clave == clavetipoNomina && o.periodosNomina_ID == idPeriodoNomina
                            && o.razonesSociales.clave == claveRazonSocial
                            select o;
                if (claveEmpleado != null)
                {
                    query = from sub in query
                            where sub.empleados.clave == claveEmpleado
                            select sub;
                }

                if (claveExepcion != null)
                {
                    query = from sub in query
                            where sub.excepciones.clave == claveExepcion
                            select sub;
                }

                if (claveCentroCosto != null)
                {
                    query = from sub in query
                            where sub.centroDeCosto.clave == claveCentroCosto
                            select sub;
                }

                var listasistencias = query.Select(sub => new
                {
                    sub.cantidad,
                    sub.centroDeCosto_ID,
                    sub.empleados_ID,
                    sub.excepciones_ID,
                    sub.fecha,
                    sub.id,
                    sub.jornada,
                    sub.ordenId,
                    sub.periodosNomina_ID,
                    sub.razonesSociales_ID,
                    sub.tipoNomina_ID,
                    sub.tipoPantalla
                }).ToList();
                mensajeResultado.resultado = listasistencias;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getAsistenciaPorFiltros()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje saveDeleteAsist(List<Asistencias> AgreModif, object[] clavesDelete, List<DetalleAsistencia> AgreModifDet, object[] clavesDeleteDet, DBContextAdapter dbContext)
        {
            bool exito = true;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (clavesDelete != null)
                {
                    if (clavesDelete.Count() > 0)
                    {
                        exito = deleteListQuerys("Asistencias", new CamposWhere("Asistencias.id", clavesDelete, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                    }
                }
                if (exito)
                {

                    if (AgreModif != null)
                    {
                        for (int i = 0; i < AgreModif.Count; i++)
                        {
                            if (AgreModif[i].id == 0)
                            {
                                getSession().Set<Asistencias>().Add(AgreModif[i]);
                            }
                            else
                            {
                                getSession().Set<Asistencias>().AddOrUpdate(AgreModif[i]);
                            }
                            getSession().SaveChanges();

                        }
                        
                    }
                    if (clavesDeleteDet != null)
                    {
                        exito =
                        deleteListQuerys("DetalleAsistencia", new CamposWhere("DetalleAsistencia.id", clavesDeleteDet, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                    }
                    if (exito)
                    {
                        if (AgreModifDet != null)
                        {
                            for (int i = 0; i < AgreModifDet.Count; i++)
                            {
                                if (AgreModifDet[i].id == 0)
                                {
                                    getSession().Set<DetalleAsistencia>().Add(AgreModifDet[i]);
                                }
                                else
                                {
                                    getSession().Set<DetalleAsistencia>().AddOrUpdate(AgreModifDet[i]);
                                }

                            }
                            getSession().SaveChanges();
                        }
                    }
                }


                if (exito)
                {

                    mensajeResultado.resultado = true;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }
                else
                {
                    mensajeResultado.resultado = null;
                    getSession().Database.CurrentTransaction.Rollback();
                }

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteAsist()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }

            return mensajeResultado;
        }

        public Mensaje getAsistenciaPorFiltrosIDS(decimal clavetipoNomina, decimal idPeriodoNomina, decimal claveEmpleado, decimal claveRazonSocial, decimal claveCentroCosto, int claveExepcion, DBContextAdapter dbcontext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbcontext.context);
                getSession().Database.BeginTransaction();
                var query = from o in getSession().Set<Asistencias>()
                            where o.tipoNomina_ID == clavetipoNomina && o.periodosNomina_ID == idPeriodoNomina
                            && o.razonesSociales_ID == claveRazonSocial
                            select o;
                if (claveEmpleado > 0)
                {
                    query = from sub in query
                            where sub.empleados_ID == claveEmpleado
                            select sub;
                }

                if (claveExepcion > 0)
                {
                    query = from sub in query
                            where sub.excepciones_ID == claveExepcion
                            select sub;
                }

                if (claveCentroCosto > 0)
                {
                    query = from sub in query
                            where sub.centroDeCosto_ID == claveCentroCosto
                            select sub;
                }

                var listasistencias = query.Select(sub => new
                {
                    sub.cantidad,
                    sub.centroDeCosto_ID,
                    centroDeCosto = sub.centroDeCosto == null ? null : new { sub.centroDeCosto.id, sub.centroDeCosto.descripcion },
                    sub.empleados_ID,
                    empleados = new { sub.empleados.id, sub.empleados.clave, sub.empleados.nombre, sub.empleados.apellidoMaterno, sub.empleados.apellidoPaterno },
                    sub.excepciones_ID,
                    excepciones = new { sub.excepciones.id, sub.excepciones.clave, sub.excepciones.excepcion },
                    sub.fecha,
                    sub.id,
                    sub.jornada,
                    sub.ordenId,
                    sub.periodosNomina_ID,
                    sub.razonesSociales_ID,
                    sub.tipoNomina_ID,
                    sub.tipoPantalla
                }).ToList();
                mensajeResultado.resultado = listasistencias;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getAsistenciaPorFiltros()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getDetalleAsistenciaPorFiltrosIDS(decimal clavetipoNomina, decimal idPeriodoNomina, decimal claveEmpleado, decimal claveRazonSocial, decimal claveCentroCosto, DBContextAdapter dbcontext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbcontext.context);
                getSession().Database.BeginTransaction();
                var query = from o in getSession().Set<DetalleAsistencia>()
                            where o.tipoNomina_ID == clavetipoNomina && o.periodosNomina_ID == idPeriodoNomina
                            && o.razonesSociales_ID == claveRazonSocial
                            select o;
                if (claveEmpleado > 0)
                {
                    query = from sub in query
                            where sub.empleados_ID == claveEmpleado
                            select sub;
                }

                if (claveCentroCosto > 0)
                {
                    query = from sub in query
                            where sub.centroDeCosto_ID == claveCentroCosto
                            select sub;
                }

                var listasistencias = query.Select(sub => new
                {
                    sub.centroDeCosto_ID,
                    centroDeCosto = sub.centroDeCosto == null ? null : new { sub.centroDeCosto.id, sub.centroDeCosto.descripcion },
                    sub.empleados_ID,
                    empleados = new { sub.empleados.id, sub.empleados.clave, sub.empleados.nombre, sub.empleados.apellidoMaterno, sub.empleados.apellidoPaterno },
                    sub.dia,
                    sub.id,
                    sub.periodosNomina_ID,
                    sub.razonesSociales_ID,
                    sub.tipoNomina_ID,
                    sub.tipoPantalla,
                    sub.horaDoble,
                    sub.horaTriple
                }).ToList();
                mensajeResultado.resultado = listasistencias;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getDetalleAsistenciaPorFiltrosIDS()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje buscaAsistenciaExitente(DateTime fecha, string claveEmpleado, string claveTipoNomina, string clavePeriodo, string claveRazon, DBContextAdapter dbcontext)
        {
            // Asistencias asistencias = new Asistencias();
            try
            {
                inicializaVariableMensaje();
                setSession(dbcontext.context);
                getSession().Database.BeginTransaction();
                var asistencias = (from a in getSession().Set<Asistencias>()
                                   where a.fecha == fecha && a.empleados.clave == claveEmpleado &&
                                   a.tipoNomina.clave == claveTipoNomina && a.periodosNomina.clave == clavePeriodo &&
                                   a.razonesSociales.clave == claveRazon
                                   select new
                                   {
                                       a.cantidad,
                                       a.centroDeCosto_ID,
                                       empleados = new
                                       {
                                           a.empleados.apellidoMaterno,
                                           a.empleados.apellidoPaterno,
                                           a.empleados.nombre,
                                           a.empleados.clave,
                                           a.empleados.id
                                       },
                                       a.empleados_ID,
                                       excepciones = new
                                       {
                                           a.excepciones.clave,
                                           a.excepciones.excepcion,
                                           a.excepciones.id
                                       },
                                       a.excepciones_ID,
                                       a.fecha,
                                       a.id,
                                       a.jornada,
                                       a.periodosNomina_ID,
                                       a.razonesSociales_ID,
                                       a.tipoNomina_ID,
                                       a.tipoPantalla
                                   }).SingleOrDefault();


                mensajeResultado.resultado = asistencias;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscaAsistenciaExitente()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
    }
}