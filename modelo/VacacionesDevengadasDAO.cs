/**
* @author: Daniel Ruelas 
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Clase VacacionesDevengadasDAO para llamados a metodos de Entity
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
using Exitosw.Payroll.Core.util;
using System.Xml.Linq;
using Exitosw.Payroll.TestCompilador.funciones;
using Exitosw.Payroll.Entity.util;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public class VacacionesDevengadasDAO : GenericRepository<VacacionesDevengadas>, VacacionesDevengadasDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        private Dictionary<String, VacacionesDevengadas> devengadaActual = null;
        private DBContextSimple dbContextSimple = null;
        private DBContextMaster dbContextMaster = null;

        //usado calculo de nomina
        public Mensaje calcularVacacionesDevengadasEmpleados(RazonesSociales razonesSociales, object[,] factorIntegracion, DBContextAdapter dbContextSimple, DBContextAdapter dbContextMaster)
        {
            this.dbContextSimple =(DBContextSimple) dbContextSimple.context;
            this.dbContextMaster =(DBContextMaster) dbContextMaster.context;

            mensajeResultado = calcularVacacionesDevengadasEmpleados(razonesSociales, factorIntegracion,(DBContextSimple) dbContextSimple.context,(DBContextMaster) dbContextMaster.context, true);

            dbContextSimple = null;
            dbContextMaster = null;

            return mensajeResultado;
        }

        //usado calculo de nomina
        public Mensaje calcularVacacionesDevengadasEmpleados(RazonesSociales razonesSociales, object[,] factorIntegracion, DBContextSimple dbContextSimple, DBContextMaster dbContextMaestra)
        {
            return calcularVacacionesDevengadasEmpleados(razonesSociales, factorIntegracion,dbContextSimple, dbContextMaestra, true);
        }

        public Mensaje calcularVacacionesDevengadasEmpleados(RazonesSociales razonesSociales, object[,] factorIntegracion, DBContextSimple dbContextSimple, DBContextMaster dbContextMaestra, bool usaCommit)
        {
            Object[,] reglaFactor = factorIntegracion;
            bool usoBDMaestra = false;
            devengadaActual = new Dictionary<string, VacacionesDevengadas>();
            try
            {
                //inicializaVariableMensaje();
                //if (dbContextSimple == null)
                //{
                //    setSession(dbContext);
                //}
                //else
                //{
                //    setSession(dbContextSimple);
                //}

               // dbContextSimple.Database.BeginTransaction();
                ControlVacDeveng control = (from c in dbContextSimple.Set<ControlVacDeveng>()
                                            where c.razonesSociales.clave == razonesSociales.clave &&
                                             c.fecha == (from ct in dbContextSimple.Set<ControlVacDeveng>()
                                                         select new { ct.fecha }).Max(p => p.fecha)
                                            select c).SingleOrDefault();

                DateTime fechaUltimDev;
                List<DateTime> diasPendientes = null;
                if (control == null)
                {
                    diasPendientes = new List<DateTime>();
                    diasPendientes.Add(DateTime.Now);
                }
                else
                {
                    fechaUltimDev = Convert.ToDateTime(control.fecha);
                    diasPendientes = getDaysBetweenDates(fechaUltimDev, new DateTime());
                }
                int d = 0;
                ControlVacDeveng controlCalculadas = null;
                for (d = 0; d < diasPendientes.Count; d++)
                {
                    if (diasPendientes.Count > 0)
                    {
                        //Obtiene empleados que cumplen aniversario en la empresa al día
                        List<PlazasPorEmpleado> plazasEmpleados = null;
                        DateTime fechapen = diasPendientes[d];
                        plazasEmpleados = (from o in dbContextSimple.Set<PlazasPorEmpleadosMov>()
                                           where
                                           (from m in dbContextSimple.Set<PlazasPorEmpleadosMov>()
                                            where m.plazasPorEmpleado.razonesSociales.clave == razonesSociales.clave
                                            && m.plazasPorEmpleado.fechaFinal >= fechapen &&
                                            o.plazasPorEmpleado.fechaPrestaciones.Value.Month == fechapen.Month
                                            && o.plazasPorEmpleado.fechaPrestaciones.Value.Day == fechapen.Day
                                            group new { m.plazasPorEmpleado.empleados, m } by new
                                            {
                                                m.plazasPorEmpleado.empleados.clave
                                            } into g
                                            select new
                                            {
                                                Column1 = g.Max(p => p.m.id)
                                            }).Contains(new { Column1 = o.id })
                                           select o.plazasPorEmpleado).ToList();

                        plazasEmpleados = plazasEmpleados == null ? new List<PlazasPorEmpleado>() : plazasEmpleados;
                        if (plazasEmpleados.Count > 0)
                        {
                            if (reglaFactor == null)
                            {//Obtiene  los factores de integracion
                                usoBDMaestra = true;
                                //if (dbContextMaster == null)
                                //{
                                //    setSession(dbContext);
                                //}
                                //else
                                //{
                                //    setSession(dbContextMaster);
                                //}
                                List<TablaDatos> values;
                                values = (from o in dbContextMaster.Set<TablaDatos>()
                                          where o.tablaBase.clave == ClavesParametrosModulos.claveTipoTablaFactorIntegracion.ToString() &&
                                          o.id == (from t in dbContextMaster.Set<TablaDatos>()
                                                   where t.tablaBase.id == o.tablaBase.id
                                                   select new { t.id }).Max(p => p.id)
                                          select o).ToList();
                                values = values == null ? new List<TablaDatos>() : values;
                                if (values.Count > 0)
                                {
                                    byte[] convert = ((TablaDatos)values[0]).fileXml;
                                    XDocument doc = UtilidadesXML.convierteBytesToXML(convert);
                                    reglaFactor = UtilidadesXML.extraeValoresNodos(doc);
                                }
                            }
                            //Llenar tabla de Vacaciones Devengadas por día
                            VacacionesDevengadas vd = null;
                            if (usoBDMaestra)
                            {
                                if (dbContextSimple == null)
                                {
                                    setSession(dbContextSimple);
                                }
                                else
                                {
                                    getSession().Database.Connection.Close();
                                    setSession(dbContextSimple);
                                }
                            }
                            for (int i = 0; i < plazasEmpleados.Count; i++)
                            {
                                Double antiguedad = (Double)calcularAntiguedadExacta(Convert.ToDateTime(plazasEmpleados[i].fechaPrestaciones));
                                //Obtiene vacaciones devengadas por año a ese empleado
                                VacacionesDevengadas vaca = (from a in dbContextSimple.Set<VacacionesDevengadas>()
                                                             where a.empleados.id == plazasEmpleados[i].empleados.id
                                                             && a.ejercicio == Convert.ToInt32(antiguedad)
                                                             select a).SingleOrDefault();
                                if (vaca == null)
                                {
                                    vd = new VacacionesDevengadas();
                                    vd.razonesSociales = plazasEmpleados[i].razonesSociales;
                                    vd.empleados = plazasEmpleados[i].empleados;
                                    vd.ejercicio = Convert.ToInt32(antiguedad);
                                    Object[] factorEmpleado = (Object[])obtieneFactorIntegracion(reglaFactor, Convert.ToInt32(antiguedad));
                                    Object[] salarioAniv = (from sdi in dbContextSimple.Set<SalariosIntegrados>()
                                                            where sdi.empleados.id == plazasEmpleados[i].empleados.id
                                                            && sdi.fecha <= plazasEmpleados[i].fechaPrestaciones
                                                            group sdi by new
                                                            {
                                                                sdi.salarioDiarioFijo
                                                            } into g
                                                            select new
                                                            {
                                                                Column1 = g.Max(p => p.fecha),
                                                                g.Key.salarioDiarioFijo
                                                            }).ToArray();
                                    if (salarioAniv == null)
                                    {
                                        vd.salarioAniversario = 0.0;
                                    }
                                    else
                                    {
                                        vd.salarioAniversario = (double)salarioAniv[1];
                                    }
                                    vd.factorPrima = Convert.ToDouble(factorEmpleado[4].ToString());
                                    vd.diasVacaciones = Convert.ToInt32(factorEmpleado[3].ToString());
                                    vd.registroInicial = false;
                                    double primaVac = Convert.ToDouble(factorEmpleado[4].ToString()) / 100 * Convert.ToInt32(factorEmpleado[3].ToString());
                                    vd.diasPrimaVaca = primaVac;
                                    dbContextSimple.Set<VacacionesDevengadas>().Add(vd);
                                    devengadaActual.Add(plazasEmpleados[i].empleados.clave, vd);
                                }

                            }

                        }


                    }
                    controlCalculadas = new ControlVacDeveng();
                    controlCalculadas.fecha = diasPendientes[d];
                    controlCalculadas.razonesSociales_ID = razonesSociales.id;

                    dbContextSimple.Set<ControlVacDeveng>().Add(controlCalculadas);
                    dbContextSimple.SaveChanges();

                }
                if (usaCommit)
                {
                    dbContextSimple.Database.CurrentTransaction.Commit();
                }
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("calcularVacacionesDevengadasEmpleados()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                dbContextSimple.Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getVacacionesDenvengadasPorEmpleado(string claveEmpleado, string claveRazonSocial, DBContextAdapter dbContext)
        {
            List<VacacionesDevengadas> vacacionesDevengadas = new List<VacacionesDevengadas>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                vacacionesDevengadas = (from vd in getSession().Set<VacacionesDevengadas>()
                                        where vd.empleados.clave == claveEmpleado &&
                                        vd.razonesSociales.clave == claveRazonSocial
                                        select vd).ToList();
                mensajeResultado.resultado = vacacionesDevengadas;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getVacacionesDenvengadasPorEmpleado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getAllVacacionesDevengadas(DBContextAdapter dbContext)
        {
            List<VacacionesDevengadas> vacacionesDevengadas = new List<VacacionesDevengadas>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                vacacionesDevengadas = (from vd in getSession().Set<VacacionesDevengadas>()
                                        select vd).ToList();
                mensajeResultado.resultado = vacacionesDevengadas;
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

        public Mensaje saveDeleteVacacionesDevengadas(List<VacacionesDevengadas> entitysCambios, int rango, DBContextAdapter dbContext)
        {
            List<VacacionesDevengadas> vacacionesDevengadas = new List<VacacionesDevengadas>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                for (int i = 0; i < entitysCambios.Count; i++)
                {
                    getSession().Set<VacacionesDevengadas>().Add(entitysCambios[i]);
                    getSession().SaveChanges();
                }

                mensajeResultado.resultado = vacacionesDevengadas;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteVacacionesDevengadas()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        private List<DateTime> getDaysBetweenDates(DateTime startdate, DateTime enddate)
        {
            List<DateTime> dates = new List<DateTime>();
            TimeSpan span = enddate - startdate;
            startdate = startdate.AddDays(1);
            int contador = 1;
            while (span.Days >= contador)
            {
                DateTime result = startdate;
                dates.Add(result);
                startdate = startdate.AddDays(1);


                contador++;
            }
            return dates;
        }

        private Object calcularAntiguedadExacta(DateTime fechaInicial)
        {

            //DateTime fechaFinal = new DateTime();
            double antiguedad;
            try
            {
                antiguedad = DateTime.Today.AddTicks(-fechaInicial.Ticks).Year - 1;
                return antiguedad;
            }
            catch (FormatException es)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("calcularAntiguedadExacta()1_Error: ").Append(es));
            }
            return 0.0;
        }
        private Object obtieneFactorIntegracion(Object[,] reglaFactor, int antiguedad)
        {
            int i = 0;
            for (i = 0; i < reglaFactor.Length; i++)
            {
                int dataFact = Convert.ToInt32(reglaFactor[i, 0].ToString());

                if (antiguedad <= dataFact)
                {
                    break;
                }
            }
            return reglaFactor[i, 0];
        }
        public Dictionary<String, VacacionesDevengadas> getDevengadaActual()
        {
            return devengadaActual;
        }

        public Mensaje getVacacionesDenvengadasPorEmpleadoJS(string claveEmpleado, string claveRazonSocial, DBContextAdapter dbContext)
        {
            //List<VacacionesDevengadas> vacacionesDevengadas = new List<VacacionesDevengadas>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
               var  vacacionesDevengadas = (from vd in getSession().Set<VacacionesDevengadas>()
                                        where vd.empleados.clave == claveEmpleado &&
                                        vd.razonesSociales.clave == claveRazonSocial
                                        select new { 
                                        vd.diasPrimaVaca,
                                        vd.diasVacaciones,
                                        vd.ejercicio,
                                        vd.empleados_ID,
                                        vd.factorPrima,
                                        vd.id,
                                        vd.razonesSociales_ID,
                                        vd.registroInicial,
                                        vd.salarioAniversario
                                        }).ToList();
                mensajeResultado.resultado = vacacionesDevengadas;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getVacacionesDenvengadasPorEmpleado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje agregar(VacacionesDevengadas entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<VacacionesDevengadas>().Add(entity);
                getSession().SaveChanges();
                mensajeResultado.resultado = true;
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

    }
}