/**
* @author: Daniel Ruelas 
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Clase VacacionesAplicacionDAO para llamados a metodos de Entity
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
using System.Data.Entity.Migrations;
using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.Core.genericos.campos;
using Exitosw.Payroll.Entity.util;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public class VacacionesAplicacionDAO : GenericRepository<VacacionesAplicacion>, VacacionesAplicacionDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        private List<VacacionesAplicacion> listEsp = new List<VacacionesAplicacion>();
        private bool commit = false;
        public Mensaje agregar(VacacionesAplicacion entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<VacacionesAplicacion>().Add(entity);
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
        public Mensaje modificar(VacacionesAplicacion entity, DBContextAdapter dbContext)
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
                getSession().Set<VacacionesAplicacion>().AddOrUpdate(entity);
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


        public Mensaje eliminar(VacacionesAplicacion entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                //var sentencia = (from g in getSession().Set<Genero>()
                //                 where g.id == entity.id
                //                 select g).FirstOrDefault();
                getSession().Set<VacacionesAplicacion>().Attach(entity);
                getSession().Set<VacacionesAplicacion>().Remove(entity);
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

        public Mensaje getVacacionesAplicacionAll(string claveRazonesSocial, DBContextAdapter dbContext)
        {
            List<VacacionesAplicacion> vacacionesAplicacion = new List<VacacionesAplicacion>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                vacacionesAplicacion = (from va in getSession().Set<VacacionesAplicacion>()

                                        select va).ToList();
                mensajeResultado.resultado = vacacionesAplicacion;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getVacacionesAplicacionAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getVacacionesPorEmpleado(string claveEmpleado, string claveRazonSocial, DBContextAdapter dbContext)
        {
            List<VacacionesAplicacion> vacacionesAplicacion = new List<VacacionesAplicacion>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                vacacionesAplicacion = (from va in getSession().Set<VacacionesAplicacion>()
                                        join vd in getSession().Set<VacacionesDisfrutadas>()
                                        on va.vacacionesDisfrutadas.id equals vd.id
                                        where vd.empleados.clave == claveEmpleado && vd.razonesSociales.clave == claveRazonSocial
                                        select va).ToList();
                mensajeResultado.resultado = vacacionesAplicacion;
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

        public Mensaje saveDeleteVacacionesAplicacion(List<VacacionesAplicacion> entitysCambios, object[] clavesDelete, int rango, DBContextAdapter dbContext)
        {
            listEsp = new List<VacacionesAplicacion>();
            try
            {
                commit = true;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (clavesDelete != null)
                {
                    commit = deleteListQuerys(typeof(VacacionesAplicacion).Name, new CamposWhere("VacacionesAplicacion.id", clavesDelete, OperadorComparacion.IN, OperadorLogico.AND), dbContext);

                    //deleteListQuerys("VacacionesAplicacion", "Id", clavesDelete);
                    if (commit)
                    {
                        getSession().SaveChanges();
                    }
                }
                entitysCambios = (entitysCambios == null ? new List<VacacionesAplicacion>() : entitysCambios);
                if (commit && entitysCambios.Count > 0)
                {
                    listEsp = agregarListaVacacionesAplicacion(entitysCambios, rango);

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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteVacacionesAplicacion()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        private List<VacacionesAplicacion> agregarListaVacacionesAplicacion(List<VacacionesAplicacion> entitys, int rango)
        {
            listEsp.Clear();
            try
            {
                int i = 0;
                for (i = 0; i < entitys.Count; i++)
                {
                    if (entitys[i].id == 0)
                    {
                        listEsp.Add(getSession().Set<VacacionesAplicacion>().Add(entitys[i]));
                    }
                    else
                    {
                        getSession().Set<VacacionesAplicacion>().AddOrUpdate(entitys[i]);
                    }
                    //if (i % rango == 0 & i > 0)
                    //{
                    getSession().SaveChanges();
                    //}
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("agregarListaVacacionesAplicacion()1_Error: ").Append(ex));
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("deleteListQuerys()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                exito = false;
            }
            return exito;
        }

        public Mensaje getVacacionesPorEmpleadoJS(string claveEmpleado, string claveRazonSocial, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var vacacionesAplicacion = (from va in getSession().Set<VacacionesAplicacion>()
                                            join vd in getSession().Set<VacacionesDisfrutadas>()
                                            on va.vacacionesDisfrutadas.id equals vd.id
                                            where vd.empleados.clave == claveEmpleado && vd.razonesSociales.clave == claveRazonSocial
                                            select new
                                            {
                                                va.diasPrima,
                                                va.diasVac,
                                                va.id,
                                                va.vacacionesDevengadas_ID,
                                                vacacionesDevengadas = new
                                                {
                                                    va.vacacionesDevengadas.diasPrimaVaca,
                                                    va.vacacionesDevengadas.diasVacaciones,
                                                    va.vacacionesDevengadas.ejercicio,
                                                    va.vacacionesDevengadas.factorPrima,
                                                    va.vacacionesDevengadas.id,
                                                    va.vacacionesDevengadas.razonesSociales_ID,
                                                    va.vacacionesDevengadas.empleados_ID,
                                                    va.vacacionesDevengadas.salarioAniversario

                                                },
                                                va.vacacionesDisfrutadas_ID,
                                                vacacionesDisfrutadas = new
                                                {
                                                    va.vacacionesDisfrutadas.diasPrimaDisfrutados,
                                                    va.vacacionesDisfrutadas.diasVacDisfrutados,
                                                    va.vacacionesDisfrutadas.ejercicioAplicacion,
                                                    va.vacacionesDisfrutadas.empleados_ID,
                                                    va.vacacionesDisfrutadas.id,
                                                    va.vacacionesDisfrutadas.observaciones,
                                                    va.vacacionesDisfrutadas.pagarPrimaVacacional,
                                                    va.vacacionesDisfrutadas.pagarVacaciones,
                                                    va.vacacionesDisfrutadas.periodoAplicacion_ID,
                                                    va.vacacionesDisfrutadas.periodoPago_ID,
                                                    periodoPago = new {
                                                        va.vacacionesDisfrutadas.periodoPago.clave
                                                    },
                                                    //va.vacacionesDisfrutadas.periodoVacacional_ID,
                                                    va.vacacionesDisfrutadas.razonesSociales_ID,
                                                    va.vacacionesDisfrutadas.registroInicial,
                                                    va.vacacionesDisfrutadas.salidaVacac,
                                                    va.vacacionesDisfrutadas.statusVacaciones,
                                                    va.vacacionesDisfrutadas.tipoCorridaAplicacion_ID,
                                                    va.vacacionesDisfrutadas.tipoNominaAplicacion_ID,
                                                    va.vacacionesDisfrutadas.tipoCorridaPago_ID,
                                                    va.vacacionesDisfrutadas.tipoNominaPago_ID,
                                                    tipoNominaPago = new { 
                                                        va.vacacionesDisfrutadas.tipoNominaPago.clave
                                                    
                                                    },
                                                    va.vacacionesDisfrutadas.tiposVacaciones_ID
                                                }

                                            }).ToList();
                mensajeResultado.resultado = vacacionesAplicacion;
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

        public Mensaje SaveDeleteVacAplicacionJS(List<VacacionesAplicacion> entitysCambios, object[] clavesDelete,List<VacacionesDevengadas> entytysDevengada, DBContextAdapter dbContext)
        {
            listEsp = new List<VacacionesAplicacion>();
            try
            {
                commit = true;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (clavesDelete != null)
                {
                    commit = deleteListQuerys(typeof(VacacionesAplicacion).Name, new CamposWhere("VacacionesAplicacion.id", clavesDelete, OperadorComparacion.IN, OperadorLogico.AND), dbContext);

                    //deleteListQuerys("VacacionesAplicacion", "Id", clavesDelete);
                    if (commit)
                    {
                        getSession().SaveChanges();
                    }
                }
                entitysCambios = (entitysCambios == null ? new List<VacacionesAplicacion>() : entitysCambios);
                if (commit && entitysCambios.Count > 0)
                {
                    listEsp = agregarListaVacacionesAplicacionJS(entitysCambios);

                }

                if (commit && entytysDevengada.Count > 0) {
                    for (int i = 0; i < entytysDevengada.Count; i++)
                    {
                        getSession().Set<VacacionesDevengadas>().Add(entytysDevengada[i]);
                        getSession().SaveChanges();
                    }
                }

                if (commit)
                {
                    mensajeResultado.resultado = true;
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteVacacionesAplicacion()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        private List<VacacionesAplicacion> agregarListaVacacionesAplicacionJS(List<VacacionesAplicacion> entitys)
        {
            listEsp.Clear();
            try
            {
                int i = 0;
                for (i = 0; i < entitys.Count; i++)
                {
                    if (entitys[i].id == 0)
                    {
                        //if (entitys[i].vacacionesDevengadas.id ==0) {
                        //    getSession().Set<VacacionesDevengadas>().Add(entitys[i].vacacionesDevengadas);
                        //    getSession().SaveChanges();
                        //    entitys[i].vacacionesDevengadas_ID = entitys[i].vacacionesDevengadas.id;
                        //    entitys[i].vacacionesDevengadas = null;
                        //}

                        //if (entitys[i].vacacionesDisfrutadas.id==0) {
                        //    getSession().Set<VacacionesDisfrutadas>().Add(entitys[i].vacacionesDisfrutadas);
                        //    getSession().SaveChanges();
                        //    entitys[i].vacacionesDisfrutadas_ID = entitys[i].vacacionesDisfrutadas.id;
                        //    entitys[i].vacacionesDisfrutadas = null;
                        //}

                        listEsp.Add(getSession().Set<VacacionesAplicacion>().Add(entitys[i]));
                    }
                    else
                    {
                        //if (entitys[i].vacacionesDevengadas.id > 0)
                        //{
                        //    getSession().Set<VacacionesDevengadas>().AddOrUpdate(entitys[i].vacacionesDevengadas);
                        //}

                        if (entitys[i].vacacionesDisfrutadas.id > 0)
                        {
                            getSession().Set<VacacionesDisfrutadas>().AddOrUpdate(entitys[i].vacacionesDisfrutadas);
                        }
                        getSession().Set<VacacionesAplicacion>().AddOrUpdate(entitys[i]);
                    }
                    //if (i % rango == 0 & i > 0)
                    //{
                    getSession().SaveChanges();
                    //}
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("agregarListaVacacionesAplicacion()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                commit = false;
            }
            return listEsp;
        }

        public Mensaje SaveDeleteVacAplicacionJSP(List<VacacionesAplicacion> entitysCambios, List<VacacionesAplicacion> entitysDelete, DBContextAdapter dbContext)
        {
            listEsp = new List<VacacionesAplicacion>();
            try
            {
                commit = true;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();

                if (entitysDelete.Count > 0)
                {
                    object[] clavesDelete = new object[entitysDelete.Count];
                    object[] claveDeleteDis = new object[entitysCambios.Count];
                    for (int i = 0; i < entitysDelete.Count; i++)
                    {
                        clavesDelete[i] = entitysDelete[i].id;
                        if (i == 0)
                        {
                            claveDeleteDis[i] = entitysDelete[i].vacacionesDisfrutadas_ID;
                        }
                        else if(entitysDelete[i].vacacionesDisfrutadas_ID !=  entitysDelete[i-1].vacacionesDisfrutadas_ID) {
                            claveDeleteDis[claveDeleteDis.Length-1] = entitysDelete[i].vacacionesDisfrutadas_ID;
                        }
                    }
                    commit = deleteListQuerys(typeof(VacacionesAplicacion).Name, new CamposWhere("VacacionesAplicacion.id", clavesDelete, OperadorComparacion.IN, OperadorLogico.AND), dbContext);

                    //deleteListQuerys("VacacionesAplicacion", "Id", clavesDelete);
                    if (commit)
                    {
                        getSession().SaveChanges();
                        commit = deleteListQuerys(typeof(VacacionesAplicacion).Name, new CamposWhere("VacacionesDisfrutadas.id", claveDeleteDis, OperadorComparacion.IN, OperadorLogico.AND), dbContext);

                        if (commit) {
                            getSession().SaveChanges();
                        }
                    }
                }


                for (int i = 0; i < entitysCambios.Count; i++)
                {
                    if (entitysCambios[i].id == 0)
                    {
                        VacacionesDisfrutadas vacDis = entitysCambios[i].vacacionesDisfrutadas;
                        getSession().Set<VacacionesDisfrutadas>().Add(vacDis);
                        getSession().SaveChanges();
                        entitysCambios[i].vacacionesDisfrutadas = null;
                        entitysCambios[i].vacacionesDisfrutadas_ID = vacDis.id;
                        getSession().Set<VacacionesAplicacion>().Add(entitysCambios[i]);
                        getSession().SaveChanges();
                    }
                    else
                    {
                        VacacionesDisfrutadas vacDis = entitysCambios[i].vacacionesDisfrutadas;
                        getSession().Set<VacacionesDisfrutadas>().Add(vacDis);
                        getSession().SaveChanges();
                        entitysCambios[i].vacacionesDisfrutadas = null;
                        entitysCambios[i].vacacionesDisfrutadas_ID = vacDis.id;
                        getSession().Set<VacacionesAplicacion>().AddOrUpdate(entitysCambios[i]);
                        getSession().SaveChanges();

                    }


                }

                if (commit)
                {
                    mensajeResultado.resultado = true;
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteVacacionesAplicacion()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getVacacionesAplicacionAllJS(DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var vacacionesAplicacion = (from va in getSession().Set<VacacionesAplicacion>()
                                            orderby va.vacacionesDisfrutadas.empleados.clave
                                            select new
                                            {
                                                va.diasPrima,
                                                va.diasVac,
                                                va.id,
                                                vacacionesDevengadas = new
                                                {
                                                    va.vacacionesDevengadas.id,
                                                    va.vacacionesDevengadas.diasPrimaVaca,
                                                    va.vacacionesDevengadas.diasVacaciones,
                                                    va.vacacionesDevengadas.ejercicio,
                                                    va.vacacionesDevengadas.empleados_ID,
                                                    va.vacacionesDevengadas.factorPrima,
                                                    va.vacacionesDevengadas.razonesSociales_ID,
                                                    va.vacacionesDevengadas.registroInicial,
                                                    va.vacacionesDevengadas.salarioAniversario
                                                },
                                                va.vacacionesDevengadas_ID,
                                                vacacionesDisfrutadas = new
                                                {
                                                    va.vacacionesDisfrutadas.diasPrimaDisfrutados,
                                                    va.vacacionesDisfrutadas.diasVacDisfrutados,
                                                    va.vacacionesDisfrutadas.ejercicioAplicacion,
                                                    empleados = new
                                                    {
                                                        va.vacacionesDisfrutadas.empleados.clave,
                                                        va.vacacionesDisfrutadas.empleados.nombre,
                                                        va.vacacionesDisfrutadas.empleados.apellidoPaterno,
                                                        va.vacacionesDisfrutadas.empleados.apellidoMaterno


                                                    },
                                                    va.vacacionesDisfrutadas.empleados_ID,
                                                    va.vacacionesDisfrutadas.fechaPago,
                                                    va.vacacionesDisfrutadas.id,
                                                    va.vacacionesDisfrutadas.observaciones,
                                                    va.vacacionesDisfrutadas.pagarPrimaVacacional,
                                                    va.vacacionesDisfrutadas.pagarVacaciones,
                                                    va.vacacionesDisfrutadas.periodoAplicacion_ID,
                                                    va.vacacionesDisfrutadas.periodoPago_ID,
                                                    va.vacacionesDisfrutadas.razonesSociales_ID,
                                                    va.vacacionesDisfrutadas.registroInicial,
                                                    va.vacacionesDisfrutadas.regresoVac,
                                                    va.vacacionesDisfrutadas.salidaVacac,
                                                    va.vacacionesDisfrutadas.statusVacaciones,
                                                    va.vacacionesDisfrutadas.tipoCorridaAplicacion_ID,
                                                    va.vacacionesDisfrutadas.tipoCorridaPago_ID,
                                                    va.vacacionesDisfrutadas.tipoNominaAplicacion_ID,
                                                    va.vacacionesDisfrutadas.tipoNominaPago_ID,
                                                    va.vacacionesDisfrutadas.tiposVacaciones_ID
                                                    
                                                    
                                                },
                                                va.vacacionesDisfrutadas_ID
                                            }).ToList();
                mensajeResultado.resultado = vacacionesAplicacion;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getVacacionesAplicacionAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
    }
}