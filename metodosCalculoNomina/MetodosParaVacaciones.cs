using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Core.modelo;
using Exitosw.Payroll.Core.util;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Reflection;
using System.Text;
using Exitosw.Payroll.Entity.util;

namespace Exitosw.Payroll.Core.metodosCalculoNomina
{
    public class MetodosParaVacaciones
    {
        private Mensaje mensajeResultado = new Mensaje();
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        private VacacionesDevengadasDAO vacDev = new VacacionesDevengadasDAO();
        private VacacionesAplicacion vacacionAplicacionActual = null;
        public List<VacacionesAplicacion> vacacionesAplicacionStatus = new List<VacacionesAplicacion>();

        public Mensaje calcularVacacionesDevengadasEmpleados(RazonesSociales razonesSociales, object[,] tablaFactorIntegracion, DBContextSimple dbContextSimple, DBContextMaster dbContextMaster)
        {
            mensajeResultado = vacDev.calcularVacacionesDevengadasEmpleados(razonesSociales, tablaFactorIntegracion, dbContextSimple, dbContextMaster,false);
            return mensajeResultado;
        }

        public Mensaje agregarVacacionesAuto(RazonesSociales razonSocial, PlazasPorEmpleadosMov plaEmp, PeriodosNomina periodoNomina, object[,] tablaFactorIntegracion, PagarPrimaVacionalyVacacionesAuto pagarVacaAuto, DBContextSimple dbContextSimple)
        {
            try
            {
                VacacionesDisfrutadas vacDis = new VacacionesDisfrutadas();
                Mensaje vacDisgau = new Mensaje();
                VacacionesDisfrutadas vacDisconId = new VacacionesDisfrutadas();
                VacacionesDisfrutadasDAO vacaDisfrutadas = new VacacionesDisfrutadasDAO();
                VacacionesAplicacion VacAplic = new VacacionesAplicacion();
                VacacionesDevengadas vacacionesDeven = null;
                DateTime fechaEmplado = new DateTime();

                if (vacDev.getDevengadaActual() == null || vacDev.getDevengadaActual().Count == 0)
                {
                    mensajeResultado = calcularVacacionesDevengadasEmpleados(razonSocial, tablaFactorIntegracion, dbContextSimple, null);
                    if (mensajeResultado.noError != 0)
                    {
                        return mensajeResultado;
                    }
                }

                if (vacDev.getDevengadaActual().Count > 0)
                {
                    vacacionesDeven = vacDev.getDevengadaActual()[plaEmp.plazasPorEmpleado.empleados.clave];
                    DateTime calfecha = new DateTime();
                    DateTime fechaActual = DateTime.Now.Date;
                    DateTime calfechaActual = DateTime.Now;
                    if (vacacionesDeven != null)
                    {
                        vacacionAplicacionActual = new VacacionesAplicacion();
                        calfecha = plaEmp.plazasPorEmpleado.fechaPrestaciones.Value;
                        calfecha.AddYears(calfecha.Year + vacacionesDeven.ejercicio.Value);
                        fechaEmplado = calfecha.Date;
                        DateTime fechaPerIni = periodoNomina.fechaInicial.Value.Date;
                        DateTime fechaPerFinal = periodoNomina.fechaFinal.Value.Date;
                        if (fechaEmplado.CompareTo(fechaPerIni) >= 0 && fechaEmplado.CompareTo(fechaPerFinal) <= 0)
                        {
                            if (pagarVacaAuto == PagarPrimaVacionalyVacacionesAuto.PRIMACACIONALALANIVERSARIO)
                            {
                                vacDis.empleados = plaEmp.plazasPorEmpleado.empleados;
                                vacDis.diasPrimaDisfrutados = vacacionesDeven.diasPrimaVaca;
                                vacDis.diasVacDisfrutados = (0);
                                vacDis.ejercicioAplicacion = calfechaActual.Year;
                                vacDis.pagarPrimaVacacional = true;
                                vacDis.pagarVacaciones = false;
                                vacDis.periodoAplicacion = periodoNomina;
                                vacDis.periodoPago = periodoNomina;
                                vacDis.razonesSociales = razonSocial;
                                vacDis.registroInicial = false;
                                vacDis.regresoVac = calfechaActual;
                                vacDis.salidaVacac = calfechaActual;
                                vacDis.statusVacaciones = StatusVacaciones.PORCALCULAR;
                                vacDis.tipoNominaAplicacion = periodoNomina.tipoNomina;
                                vacDis.tipoCorridaAplicacion = periodoNomina.tipoCorrida;
                                dbContextSimple.Set<VacacionesDisfrutadas>().Add(vacDis);
                                dbContextSimple.SaveChanges();

                                vacDisconId = vacDis;
                                VacAplic.diasPrima = vacacionesDeven.diasPrimaVaca;
                                VacAplic.diasVac = 0;
                                VacAplic.vacacionesDevengadas = vacacionesDeven;
                                VacAplic.vacacionesDisfrutadas = vacDisconId;
                                dbContextSimple.Set<VacacionesAplicacion>().AddOrUpdate(VacAplic);
                                dbContextSimple.SaveChanges();
                                vacacionAplicacionActual = VacAplic;
                            }
                            else if (pagarVacaAuto == PagarPrimaVacionalyVacacionesAuto.PRIMACACIONALYVACACIONESALANIVERSARIO)
                            {
                                vacDis.empleados = plaEmp.plazasPorEmpleado.empleados;
                                vacDis.diasPrimaDisfrutados = vacacionesDeven.diasPrimaVaca;
                                vacDis.diasVacDisfrutados = vacacionesDeven.diasVacaciones;
                                vacDis.ejercicioAplicacion = calfechaActual.Year;
                                vacDis.pagarPrimaVacacional = true;
                                vacDis.pagarVacaciones = false;
                                vacDis.periodoAplicacion = periodoNomina;
                                vacDis.periodoPago = periodoNomina;
                                vacDis.razonesSociales = razonSocial;
                                vacDis.registroInicial = false;
                                vacDis.regresoVac = calfechaActual;
                                vacDis.salidaVacac = calfechaActual;
                                vacDis.statusVacaciones = StatusVacaciones.PORCALCULAR;
                                vacDis.tipoNominaAplicacion = periodoNomina.tipoNomina;
                                dbContextSimple.Set<VacacionesDisfrutadas>().Add(vacDis);
                                dbContextSimple.SaveChanges();

                                vacDisconId = vacDis;
                                VacAplic.diasPrima = vacacionesDeven.diasPrimaVaca;
                                VacAplic.diasVac = vacacionesDeven.diasVacaciones;
                                VacAplic.vacacionesDevengadas = vacacionesDeven;
                                VacAplic.vacacionesDisfrutadas = vacDisconId;
                                dbContextSimple.Set<VacacionesAplicacion>().AddOrUpdate(VacAplic);
                                dbContextSimple.SaveChanges();
                                vacacionAplicacionActual = VacAplic;
                            }
                        }
                    }
                }
                mensajeResultado.resultado = vacacionAplicacionActual;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("agregarVacacionesAuto()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                dbContextSimple.Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }


        public Mensaje cargarVariablesEmpleadoVacaciones(CalculoUnidades calculoUnidades, PlazasPorEmpleadosMov plazasPorEmpleadosMovEjecutandose, bool acumularVac, PeriodosNomina periodoNomina,
            DateTime? fechaBajaFiniq, string claveTipoNomina, string claveCorrida, Dictionary<string, object> valoresConceptosEmpleados, DBContextSimple dbContextSimple)
        {
            int x;
            int diasVacaciones = 0;
            double diasPrimaVacacional = 0.0;
            try
            {
                bool corridaVacaciones = false;
                if (String.Equals(claveCorrida, "VAC", StringComparison.OrdinalIgnoreCase))
                {
                    corridaVacaciones = true;
                }
                List<VacacionesAplicacion> vacacionesAplicacion = obtenerVacaciones(periodoNomina, plazasPorEmpleadosMovEjecutandose, fechaBajaFiniq, claveTipoNomina, claveCorrida, corridaVacaciones, dbContextSimple);
                if (mensajeResultado.noError != 0)
                {
                    return mensajeResultado;
                }
                if (vacacionAplicacionActual != null)
                {
                    //                vacacionesAplicacion.add(vacacionAplicacionActual);
                }
                vacacionesAplicacionStatus = vacacionesAplicacion;
                DateTime fechaSalidaVacaciones = new DateTime(), fechaRegresoVacaciones = new DateTime();
                fechaSalidaVacaciones.AddYears(1900);
                fechaRegresoVacaciones.AddYears(1900);
                TiposVacaciones tipoVacaciones = null;
                if (vacacionesAplicacion != null)
                {
                    for (x = 0; x < vacacionesAplicacion.Count; x++)
                    {
                        fechaSalidaVacaciones = vacacionesAplicacion[x].vacacionesDisfrutadas.salidaVacac.GetValueOrDefault();
                        if (vacacionesAplicacion[x].vacacionesDisfrutadas.regresoVac != null)
                        {
                            fechaRegresoVacaciones = vacacionesAplicacion[x].vacacionesDisfrutadas.regresoVac.GetValueOrDefault();
                        }
                        if (vacacionesAplicacion[x].diasVac == null)
                        {
                            vacacionesAplicacion[x].diasVac = 0;
                        }
                        diasVacaciones += vacacionesAplicacion[x].diasVac.GetValueOrDefault();
                        if (vacacionesAplicacion[x].diasPrima == null)
                        {
                            vacacionesAplicacion[x].diasPrima = 0.0;
                        }
                        diasPrimaVacacional += vacacionesAplicacion[x].diasPrima.GetValueOrDefault();
                        tipoVacaciones = vacacionesAplicacion[x].vacacionesDisfrutadas.tiposVacaciones;
                        if (corridaVacaciones)
                        {
                            bool asigno = false;
                            if (vacacionesAplicacion[x].diasPrima > 0.0)
                            {
                                vacacionesAplicacion[x].vacacionesDisfrutadas.periodoPago = periodoNomina;
                                asigno = true;
                            }
                            if (vacacionesAplicacion[x].diasVac > 0)
                            {
                                vacacionesAplicacion[x].vacacionesDisfrutadas.periodoPago = periodoNomina;
                                asigno = true;
                            }
                            if (asigno)
                            {
                                dbContextSimple.Set<VacacionesDisfrutadas>().AddOrUpdate(vacacionesAplicacion[x].vacacionesDisfrutadas);
                            }
                        }
                        //                    fechaContador.setTime(fechaInicialVacaciones.getTime());
                        //                    while (!fechaContador.after(fechaRegresoVacaciones)) {
                        //                        if ((fechaContador.getTime().compareTo(fechaInicial) > 0 || fechaContador.getTime().compareTo(fechaInicial) == 0)
                        //                                & (fechaContador.getTime().compareTo(fechaFinal) == 0 || fechaContador.getTime().compareTo(fechaFinal) < 0)) {
                        //                            diasVacacionesDisfPeriodo += 1;
                        //                        }
                        //
                        //                        fechaContador.add(Calendar.DATE, 1);
                        //                    }
                    }
                }

                if (acumularVac)
                {
                    valoresConceptosEmpleados["diasVacacionesAcum".ToUpper()]= diasVacaciones;
                }
                else
                {

                    valoresConceptosEmpleados["fechaSalidaVacaciones".ToUpper()]= fechaSalidaVacaciones;
                    valoresConceptosEmpleados["fechaRegresoVacaciones".ToUpper()]= fechaRegresoVacaciones;
                    ////////            valoresConceptosEmpleados.put("fechaInicialTrabajadas".toUpperCase(), (Date) fechaInicialTrabajadas.getTime());
                    ////////            valoresConceptosEmpleados.put("fechaFinalTrabajadas".toUpperCase(), (Date) fechaFinalTrabajadas.getTime());
                    ////////////            valoresConceptosEmpleados.put("diasVacacionesDisfrutadas".toUpperCase(), (Integer) diasVacacionesDisfrutadas);
                    ////////////            valoresConceptosEmpleados.put("diasVacacionesTrabajadas".toUpperCase(), (Integer) diasVacacionesTrabajadas);
                    ////////            valoresConceptosEmpleados.put("diasVacacionesDisfrutadas".toUpperCase(), (Integer) diasVacacionesDisfPeriodo);
                    ////////            valoresConceptosEmpleados.put("diasVacacionesTrabajadas".toUpperCase(), (Integer) diasVacacionesTrabPeriodo);
                    ////////////            valoresConceptosEmpleados.put("diasVacacionesDisfPeriodo".toUpperCase(), (Integer) diasVacacionesDisfPeriodo);
                    ////////////            valoresConceptosEmpleados.put("diasVacacionesTrabPeriodo".toUpperCase(), (Integer) diasVacacionesTrabPeriodo);

                    valoresConceptosEmpleados["diasVacaciones".ToUpper()]=Convert.ToDouble(diasVacaciones);
                    valoresConceptosEmpleados["diasPrima".ToUpper()]= diasPrimaVacacional;
                    valoresConceptosEmpleados["tipoVacaciones".ToUpper()]= tipoVacaciones == null ? "" : tipoVacaciones.nombre;
                    if (calculoUnidades != null)
                    {
                        calculoUnidades.diasPrimaVacacional = diasPrimaVacacional;
                        calculoUnidades.diasVacaciones = diasVacaciones;
                        calculoUnidades.tiposVacaciones = tipoVacaciones;
                    }
                }
                mensajeResultado.noError = 0;
                mensajeResultado.resultado = valoresConceptosEmpleados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("cargarVariablesEmpleadoVacaciones()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return mensajeResultado;
        }

        private List<VacacionesAplicacion> obtenerVacaciones(PeriodosNomina periodo, PlazasPorEmpleadosMov plazasPorEmpleadosMovEjecutandose, DateTime? fechaBajaFiniq, string claveTipoNomina, string claveTipoCorrida, bool isCorridaVacaciones, DBContextSimple dbContextSimple)
        {
            List<VacacionesAplicacion> listVacacionesAplicacion = null;
            try
            {
                if (periodo == null)
                {
                    return new List<VacacionesAplicacion>();
                }


                if (fechaBajaFiniq != null)
                {
                    PeriodosNomina periodosNominaTmp = null;
                    if (fechaBajaFiniq != null)
                    {
                        periodosNominaTmp = (from p in dbContextSimple.Set<PeriodosNomina>()
                                             where p.tipoNomina.clave == claveTipoNomina && p.tipoCorrida.clave == claveTipoCorrida &&
                                            (fechaBajaFiniq >= p.fechaInicial && fechaBajaFiniq <= p.fechaFinal)
                                             select p).SingleOrDefault();
                    }
                    decimal idPeriodo = periodo.id;
                    if (periodosNominaTmp != null)
                    {
                        idPeriodo = periodosNominaTmp.id;
                    }
                    listVacacionesAplicacion = (from va in dbContextSimple.Set<VacacionesAplicacion>()
                                                    //join vd in dbContextSimple.Set<VacacionesDisfrutadas>() on va.vacacionesDisfrutadas.id equals vd.id
                                                    //join em in dbContextSimple.Set<Empleados>() on vd.empleados.id equals em.id
                                                    //join pa in dbContextSimple.Set<PeriodosNomina>() on vd.periodoAplicacion.id equals pa.id
                                                where va.vacacionesDisfrutadas.periodoAplicacion_ID == idPeriodo && va.vacacionesDisfrutadas.empleados_ID == plazasPorEmpleadosMovEjecutandose.plazasPorEmpleado.empleados.id
                                                select va).ToList();
                }
                else {

                    listVacacionesAplicacion = (from va in dbContextSimple.Set<VacacionesAplicacion>()
                                                join vd in dbContextSimple.Set<VacacionesDisfrutadas>() on va.vacacionesDisfrutadas.id equals vd.id
                                                join em in dbContextSimple.Set<Empleados>() on vd.empleados.id equals em.id
                                                join pa in dbContextSimple.Set<PeriodosNomina>() on vd.periodoAplicacion.id equals pa.id
                                                where em.id == plazasPorEmpleadosMovEjecutandose.plazasPorEmpleado.empleados.id &&
                                                (vd.fechaPago >= periodo.fechaInicial && vd.fechaPago <= periodo.fechaFinal)
                                                select va).ToList();
                }

               
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerVacaciones()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return listVacacionesAplicacion;
        }

        private void inicializaVariableMensaje()
        {
            if (mensajeResultado == null)
            {
                mensajeResultado = new Mensaje();

            }
            mensajeResultado.resultado = null;
            mensajeResultado.noError = 0;
            mensajeResultado.error = "";
        }
    }
}