/**
* @author: Daniel Ruelas 
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Clase VacacionesDisfrutadasDAO para llamados a metodos de Entity
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
    public class VacacionesDisfrutadasDAO : GenericRepository<VacacionesDisfrutadas>, VacacionesDisfrutadasDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        private List<VacacionesDisfrutadas> listEsp = new List<VacacionesDisfrutadas>();
        private bool commit = false;
        public Mensaje agregar(VacacionesDisfrutadas entity, DBContextAdapter dbContext, bool usacommit)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<VacacionesDisfrutadas>().Add(entity);
                getSession().SaveChanges();
                mensajeResultado.resultado = entity;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                if (usacommit)
                {
                    getSession().Database.CurrentTransaction.Commit();
                }
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
        public Mensaje modificar(VacacionesDisfrutadas entity, DBContextAdapter dbContext)
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
                getSession().Set<VacacionesDisfrutadas>().AddOrUpdate(entity);
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

        public Mensaje agregar(VacacionesDisfrutadas entity, DBContextAdapter dbContext)
        {
            return agregar(entity, dbContext, true);
        }


        public Mensaje eliminar(VacacionesDisfrutadas entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                //var sentencia = (from g in getSession().Set<Genero>()
                //                 where g.id == entity.id
                //                 select g).FirstOrDefault();
                getSession().Set<VacacionesDisfrutadas>().Attach(entity);
                getSession().Set<VacacionesDisfrutadas>().Remove(entity);
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

        public Mensaje EliminarVacacionesDisfrutadas(List<VacacionesAplicacion> vacAplicacion, DBContextAdapter dbContext)
        {
            try
            {
                VacacionesDisfrutadas disfrutadas = null;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                for (int i = 0; i < vacAplicacion.Count; i++)
                {
                    if (disfrutadas == null)
                    {
                        disfrutadas = vacAplicacion[i].vacacionesDisfrutadas;
                    }
                    getSession().Set<VacacionesAplicacion>().Attach(vacAplicacion[i]);
                    getSession().Set<VacacionesAplicacion>().Remove(vacAplicacion[i]);
                    getSession().SaveChanges();
                }
                getSession().Set<VacacionesDisfrutadas>().Attach(disfrutadas);
                getSession().Set<VacacionesDisfrutadas>().Remove(disfrutadas);
                getSession().SaveChanges();

                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("EliminarVacacionesDisfrutadas()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getVacacionesDisfrutadasAll(string claveRazonesSocial, DBContextAdapter dbContext)
        {
            List<VacacionesDisfrutadas> vacacionesDisfrutadas = new List<VacacionesDisfrutadas>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                vacacionesDisfrutadas = (from vd in getSession().Set<VacacionesDisfrutadas>()
                                         where vd.razonesSociales.clave == claveRazonesSocial
                                         select vd).ToList();
                mensajeResultado.resultado = vacacionesDisfrutadas;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getVacacionesDisfrutadasAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getVacacionesPorEmpleado(string claveEmpleado, string claveRazonSocial, DBContextAdapter dbContext)
        {
            List<VacacionesDisfrutadas> vacacionesDisfrutadas = new List<VacacionesDisfrutadas>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                vacacionesDisfrutadas = (from vd in getSession().Set<VacacionesDisfrutadas>()
                                         where vd.razonesSociales.clave == claveRazonSocial && vd.empleados.clave == claveEmpleado
                                         select vd).ToList();
                mensajeResultado.resultado = vacacionesDisfrutadas;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getVacacionesPorEmpleado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje ObtenerVacacionesDisfruradasMaxima(string claveEmpleado, string claveRazonSocial, DBContextAdapter dbContext)
        {
            VacacionesDisfrutadas vacacionesDisfrutadas;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                vacacionesDisfrutadas = (from r in getSession().Set<VacacionesDisfrutadas>()
                                         where
                                           r.empleados.clave == claveEmpleado &&
                                           r.razonesSociales.clave == claveRazonSocial &&
                                           r.salidaVacac == (from a in getSession().Set<VacacionesDisfrutadas>()
                                                             join rs1 in getSession().Set<RazonesSociales>() on r.id equals rs1.id
                                                             where
                                                              a.empleados.clave == claveEmpleado &&
                                                              rs1.clave == claveRazonSocial
                                                             select new
                                                             {
                                                                 a.salidaVacac
                                                             }).Max(p => p.salidaVacac)
                                         select r).SingleOrDefault();
                mensajeResultado.resultado = vacacionesDisfrutadas;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ObtenerVacacionesDisfruradasMaxima()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje saveDeleteVacacionesDisfrutadas(List<VacacionesDisfrutadas> entitysCambios, object[] clavesDelete, int rango, DBContextAdapter dbContext)
        {
            listEsp = new List<VacacionesDisfrutadas>();
            try
            {
                commit = true;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (clavesDelete != null)
                {
                    commit = deleteListQuerys(typeof(VacacionesDisfrutadas).Name, new CamposWhere("VacacionesDisfrutadas.id", clavesDelete, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                    //deleteListQuerys("VacacionesDisfrutadas", "Id", clavesDelete);
                    if (commit)
                    {
                        getSession().SaveChanges();
                    }
                }
                entitysCambios = (entitysCambios == null ? new List<VacacionesDisfrutadas>() : entitysCambios);
                if (commit && entitysCambios.Count > 0)
                {
                    listEsp = agregarListaVacacionesDisfrutadas(entitysCambios, rango);

                }
                if (commit)
                {
                    mensajeResultado.resultado = listEsp;
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteVacacionesDisfrutadas()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        private List<VacacionesDisfrutadas> agregarListaVacacionesDisfrutadas(List<VacacionesDisfrutadas> entitys, int rango)
        {
            listEsp.Clear();
            try
            {
                int i = 0;
                for (i = 0; i < entitys.Count; i++)
                {
                    if (entitys[i].id == 0)
                    {
                        listEsp.Add(getSession().Set<VacacionesDisfrutadas>().Add(entitys[i]));
                    }
                    else
                    {
                        getSession().Set<VacacionesDisfrutadas>().Add(entitys[i]);
                    }
                    if (i % rango == 0 & i > 0)
                    {
                        getSession().SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ObtenerVacacionesDisfruradasMaxima()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                commit = false;
            }
            return listEsp;
        }

        private bool deleteListQuerys(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext)
        {
            bool exito = true;
            try
            {
                deleteListQuery(tabla, campoWhere, dbContext);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ObtenerVacacionesDisfruradasMaxima()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                exito = false;
            }
            return exito;
        }

        public Mensaje getVacacionesPorEmpleadoJS(string claveEmpleado, string claveRazonSocial, DBContextAdapter dbContext)
        {
            //List<VacacionesDisfrutadas> vacacionesDisfrutadas = new List<VacacionesDisfrutadas>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
               var vacacionesDisfrutadas = (from vd in getSession().Set<VacacionesDisfrutadas>()
                                         where vd.razonesSociales.clave == claveRazonSocial && vd.empleados.clave == claveEmpleado
                                         select new { 
                                         vd.diasPrimaDisfrutados,
                                         vd.diasVacDisfrutados,
                                         vd.ejercicioAplicacion,
                                         vd.empleados_ID,
                                         vd.id,
                                         vd.observaciones,
                                         vd.pagarPrimaVacacional,
                                         vd.pagarVacaciones,
                                         vd.periodoAplicacion_ID,
                                         
                                         vd.periodoPago_ID,
                                         vd.razonesSociales_ID,
                                         vd.registroInicial,
                                         vd.regresoVac,
                                         vd.salidaVacac,
                                         vd.statusVacaciones,
                                         vd.tipoCorridaAplicacion_ID,
                                         vd.tipoNominaAplicacion_ID,
                                         vd.tipoCorridaPago_ID,
                                         vd.tipoNominaPago_ID,
                                         vd.tiposVacaciones_ID
                                         
                                         }).ToList();
                mensajeResultado.resultado = vacacionesDisfrutadas;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getVacacionesPorEmpleado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje ObtenerVacacionesDisfruradasMaximaJS(decimal claveEmpleado, string claveRazonSocial, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var vacacionesDisfrutadas = (from r in dbContext.context.Set<VacacionesDisfrutadas>()
                                             where
                                               r.empleados.id == claveEmpleado &&
                                               r.razonesSociales.clave == claveRazonSocial &&
                                               r.salidaVacac == (from vc in dbContext.context.Set<VacacionesDisfrutadas>()
                                                                 where
                                                                  vc.empleados.id == claveEmpleado &&
                                                                  vc.razonesSociales.clave == claveRazonSocial
                                                                 select vc.salidaVacac).Max(p => p)
                                             select new
                                             {
                                                 r.id,
                                                 r.diasPrimaDisfrutados,
                                                 r.diasVacDisfrutados,
                                                 r.ejercicioAplicacion,
                                                 r.empleados_ID,
                                                 r.fechaPago,
                                                 r.observaciones,
                                                 r.pagarPrimaVacacional,
                                                 r.pagarVacaciones,
                                                 r.periodoAplicacion_ID,
                                                 r.periodoPago_ID,
                                                 r.razonesSociales_ID,
                                                 r.registroInicial,
                                                 r.regresoVac,
                                                 r.salidaVacac,
                                                 r.tipoCorridaAplicacion_ID,
                                                 r.tipoCorridaPago_ID,
                                                 r.tipoNominaAplicacion_ID,
                                                 r.tipoNominaPago_ID,
                                                 r.tiposVacaciones_ID
                                             }).SingleOrDefault();

               

                mensajeResultado.resultado = vacacionesDisfrutadas;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ObtenerVacacionesDisfruradasMaxima()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
    }
}