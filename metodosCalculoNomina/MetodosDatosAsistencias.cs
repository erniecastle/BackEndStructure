using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Core.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Exitosw.Payroll.Entity.util;

namespace Exitosw.Payroll.Core.metodosCalculoNomina
{
    public class MetodosDatosAsistencias
    {

        public int pagarPrimero3Dias;
        public Mensaje mensajeResultado = new Mensaje();
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");

        public Mensaje cargarVariablesEmpleadoAsistencias(DateTime fechaInicial, DateTime fechaFinal, CalculoUnidades calculoUnidades, PeriodosNomina periodoNomina, DateTime? fechaBajaFiniq, bool? modificarDiasTrabajados, bool acumularAsis,
              string claveEmpleado, string claveRazonSocial, string claveTipoNomina, string claveTipoCorrida, DateTime fechaRangoInicial, DateTime fechaRangoFinal, Dictionary<string, object> valoresConceptosEmpleados, DBContextSimple dbContextSimple)
        {
            try
            {
                inicializaVariableMensaje();
                pagarPrimero3Dias = 0;
                if (valoresConceptosEmpleados == null)
                {
                    valoresConceptosEmpleados = new Dictionary<string, object>();
                }
                double diasAusencias = 0, diasIncapacidadEnfermedad = 0,
                   diasIncapacidadAccidente = 0, diasIncapacidadMaternidad = 0, diasOtrasIncapacidad = 0,
                   festivo = 0, descanso = 0, laborados = 0;
                double hrsExtraDoble = 0.0, hrsExtraTriple = 0.0, retardos = 0.0, permisoSinSueldo = 0.0, permisoConSueldo = 0.0,
                        descansoLaborado = 0.0, festivoLaborado = 0.0, domingoLaborado = 0.0, diasRetardos = 0.0, diasFaltas = 0.0;

                List<Asistencias> listAsistencias = obtenerAsistencias(fechaInicial, fechaFinal, periodoNomina, fechaBajaFiniq, claveEmpleado, claveRazonSocial, claveTipoNomina, claveTipoCorrida,
                    fechaRangoInicial, fechaRangoFinal, dbContextSimple);
                if (mensajeResultado.noError != 0)
                {
                    return mensajeResultado;
                }
                List<Asistencias> listAsistenciaIncapacidadEnfermedad = new List<Asistencias>();
                int i;
                for (i = 0; i < listAsistencias.Count; i++)
                {
                    switch (Convert.ToInt16(listAsistencias[i].excepciones.clave))
                    {
                        case 0:
                            laborados++;
                            break;
                        case 1:
                            retardos += listAsistencias[i].cantidad.GetValueOrDefault();
                            diasRetardos++;
                            break;
                        case 2:
                            if (listAsistencias[i].cantidad == null)
                            {
                                diasFaltas++;
                            }
                            else if (listAsistencias[i].cantidad.GetValueOrDefault() == 0.50)
                            {
                                diasFaltas = diasFaltas + 0.5;
                            }
                            else
                            {
                                diasFaltas++;
                            }
                            break;
                        case 3:
                            diasAusencias++;
                            break;
                        case 4:
                            permisoConSueldo++;
                            break;
                        case 5:
                            permisoSinSueldo++;
                            break;
                        case 6:
                            listAsistenciaIncapacidadEnfermedad.Add(listAsistencias[i]);
                            diasIncapacidadEnfermedad++;
                            break;
                        case 7:
                            diasIncapacidadAccidente++;
                            break;
                        case 8:
                            diasIncapacidadMaternidad++;
                            break;
                        case 9:
                            diasOtrasIncapacidad++;
                            break;
                        case 10:
                            descansoLaborado = listAsistencias[i].cantidad.GetValueOrDefault();
                            break;
                        case 11:
                            if (listAsistencias[i].excepciones.tipoDatoExcepcion == TipoDatoExcepcion.SINDATO)
                            {
                                festivoLaborado++;
                            }
                            else
                            {
                                festivoLaborado = listAsistencias[i].cantidad.GetValueOrDefault();
                            }
                            break;
                        case 12:
                            domingoLaborado = listAsistencias[i].cantidad.GetValueOrDefault();
                            break;
                        case 13:  ///"Tiempo extra == 13" falta
                            break;
                        case 14:
                            hrsExtraDoble = listAsistencias[i].cantidad.GetValueOrDefault();
                            break;
                        case 15:
                            hrsExtraTriple = listAsistencias[i].cantidad.GetValueOrDefault();
                            break;
                        case 16:
                            descanso++;
                            break;
                        case 17:
                            festivo++;
                            break;
                    }
                }

                if (listAsistencias.Count == 0)
                {
                    if (fechaRangoInicial.CompareTo(fechaRangoFinal) == 0)
                    {
                        laborados = 1;
                    }
                    else
                    {
                        laborados = cantidadDiasEntreDosFechas(fechaRangoInicial, fechaRangoFinal) + 1;
                    }
                }
                if (!acumularAsis)
                {
                    if (modificarDiasTrabajados == null ? true : modificarDiasTrabajados.GetValueOrDefault())
                    {
                        if (valoresConceptosEmpleados.ContainsKey("Laborado".ToUpper()))
                        {
                            valoresConceptosEmpleados["Laborado".ToUpper()] = laborados;
                        }
                        else
                        {
                            valoresConceptosEmpleados.Add("Laborado".ToUpper(), laborados);
                        }
                    }
                    valoresConceptosEmpleados["HorasExtrasDobles".ToUpper()] = hrsExtraDoble;
                    //valoresConceptosEmpleados.Add("HorasExtrasDobles".ToUpper(), hrsExtraDoble);
                    valoresConceptosEmpleados["HorasExtrasTriples".ToUpper()] = hrsExtraTriple;
                }

                int diasAPagar = 0;
                int diasApagarIMSS = 0;
                pagarPrimero3Dias = 0;
                if (listAsistenciaIncapacidadEnfermedad.Count > 0)
                {
                    listAsistenciaIncapacidadEnfermedad = (from list in listAsistenciaIncapacidadEnfermedad orderby list.fecha select list).ToList();

                    List<RegistroIncapacidad> listRegistroIncapacidad = obtenerIncapacidadesPorEnfermedad(listAsistenciaIncapacidadEnfermedad[0].fecha.GetValueOrDefault(), listAsistenciaIncapacidadEnfermedad[listAsistenciaIncapacidadEnfermedad.Count - 1].fecha.GetValueOrDefault(), claveEmpleado, claveRazonSocial, dbContextSimple);
                    if (mensajeResultado.noError != 0)
                    {
                        return mensajeResultado;
                    }
                    for (i = 0; i < listRegistroIncapacidad.Count; i++)
                    {
                        if (listRegistroIncapacidad[i].pagarTresPrimeroDias)
                        {
                            int diasIncapacidadEnf = listRegistroIncapacidad[i].diasIncapacidad.GetValueOrDefault();
                            diasApagarIMSS = diasIncapacidadEnf;
                            if (diasIncapacidadEnf <= 3)
                            {
                                diasAPagar = diasIncapacidadEnf;
                            }
                            else
                            {
                                diasAPagar = 3;
                            }
                            DateTime fechaIncap = listRegistroIncapacidad[i].fechaInicial.GetValueOrDefault();

                            for (int dias = 0; dias < diasIncapacidadEnf; dias++)
                            {
                                if ((fechaIncap.CompareTo(periodoNomina.fechaInicial) > 0 || fechaIncap.CompareTo(periodoNomina.fechaInicial) == 0)
                                        & (fechaIncap.CompareTo(periodoNomina.fechaFinal) == 0 || fechaIncap.CompareTo(periodoNomina.fechaFinal) < 0))
                                {
                                    if (diasAPagar > 0)
                                    {
                                        pagarPrimero3Dias++;
                                    }
                                    diasAPagar--;
                                    diasApagarIMSS--;
                                }
                                else if (fechaIncap.CompareTo(periodoNomina.fechaInicial) < 0)
                                {
                                    diasAPagar--;
                                    diasApagarIMSS--;
                                }
                                else if (fechaIncap.CompareTo(periodoNomina.fechaFinal) > 0)
                                {
                                    break;
                                }
                                if (diasAPagar == 0 | pagarPrimero3Dias == 3)
                                {
                                    break;
                                }
                                fechaIncap.AddDays(fechaIncap.Day + 1);
                            }
                            //diasIncapacidadEnfermedad = diasIncapacidadEnfermedad > 3 ? diasIncapacidadEnfermedad - 3 : 0;
                            ///diasIncapacidadEnfermedad = diasIncapacidadEnfermedad - diasUsadosPagar;
                        }
                    }
                }

                if (acumularAsis)
                {
                    valoresConceptosEmpleados["DiasIncapacidadEmpleadoAcum".ToUpper()] = diasIncapacidadEnfermedad + diasIncapacidadAccidente + diasIncapacidadMaternidad + diasOtrasIncapacidad;
                    valoresConceptosEmpleados["FaltasAcum".ToUpper()] = diasFaltas;
                    valoresConceptosEmpleados["AusentismoAcum".ToUpper()] = diasAusencias;
                }
                else
                {
                    valoresConceptosEmpleados["IncapacidadEnfermedad".ToUpper()] = diasIncapacidadEnfermedad;
                    valoresConceptosEmpleados["IncapacidadAccidente".ToUpper()] = diasIncapacidadAccidente;
                    valoresConceptosEmpleados["IncapacidadMaternidad".ToUpper()] = diasIncapacidadMaternidad;
                    valoresConceptosEmpleados["OtrasIncapacidad".ToUpper()] = diasOtrasIncapacidad;
                    valoresConceptosEmpleados["DiasIncapacidadEmpleado".ToUpper()] = diasIncapacidadEnfermedad + diasIncapacidadAccidente + diasIncapacidadMaternidad + diasOtrasIncapacidad;
                    valoresConceptosEmpleados["Faltas".ToUpper()] = diasFaltas;//Parametro 128 0 dias dias faltas
                    if (modificarDiasTrabajados == null ? true : !modificarDiasTrabajados.GetValueOrDefault())
                    {
                        valoresConceptosEmpleados["Ausentismo".ToUpper()] = diasAusencias;
                    }
                    valoresConceptosEmpleados["TExtrasDiaDescanso".ToUpper()] = descansoLaborado;
                    valoresConceptosEmpleados["TExtrasDiaFestivo".ToUpper()] = festivoLaborado;
                    valoresConceptosEmpleados["TExtrasDiaDomingo".ToUpper()] = domingoLaborado;
                    valoresConceptosEmpleados["Retardos".ToUpper()] = retardos;
                    valoresConceptosEmpleados["DiasRetardos".ToUpper()] = diasRetardos;
                    valoresConceptosEmpleados["PermisoConSueldo".ToUpper()] = permisoConSueldo;
                    valoresConceptosEmpleados["PermisoSinSueldo".ToUpper()] = permisoSinSueldo;
                    valoresConceptosEmpleados["DiasFestivos".ToUpper()] = festivo;
                    valoresConceptosEmpleados["DiasDescanso".ToUpper()] = descanso;
                    valoresConceptosEmpleados["DiasIncapacidadEmpresa".ToUpper()] = pagarPrimero3Dias;
                    valoresConceptosEmpleados["DiasIncapacidadIMSS".ToUpper()] = diasApagarIMSS;
                    if (calculoUnidades != null)
                    {
                        if (modificarDiasTrabajados == null ? true : modificarDiasTrabajados.GetValueOrDefault())
                        {
                            calculoUnidades.diasTrabajados = laborados;
                        }
                        calculoUnidades.hrsExtraDoble = hrsExtraDoble;
                        calculoUnidades.hrsExtraTriple = hrsExtraTriple;
                        calculoUnidades.diasIncapacidadEnfermedad =Convert.ToInt32(diasIncapacidadEnfermedad);
                        calculoUnidades.diasIncapacidadAccidente = Convert.ToInt32(diasIncapacidadAccidente);
                        calculoUnidades.diasIncapacidadMaternidad = Convert.ToInt32(diasIncapacidadMaternidad);
                        calculoUnidades.diasOtrasIncapacidades = Convert.ToInt32(diasOtrasIncapacidad);
                        calculoUnidades.diasFalta = diasFaltas;
                        if (modificarDiasTrabajados == null ? true : !modificarDiasTrabajados.GetValueOrDefault())
                        {
                            calculoUnidades.diasAusentismo = Convert.ToInt32(diasAusencias);
                        }
                        calculoUnidades.diasDescansoLaborado = descansoLaborado;
                        calculoUnidades.diasFestivoLaborado = festivoLaborado;
                        calculoUnidades.diasDomingoLaborado = domingoLaborado;
                        calculoUnidades.diasRetardo = retardos;
                        calculoUnidades.diasPermisoConSueldo = permisoConSueldo;
                        calculoUnidades.diasPermisoSinSueldo = permisoSinSueldo;
                        calculoUnidades.diasFestivo = Convert.ToInt32(festivo);
                        calculoUnidades.diasDescanso = Convert.ToInt32(descanso);
                    }
                }
                mensajeResultado.noError = 0;
                mensajeResultado.resultado = valoresConceptosEmpleados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("cargarVariablesEmpleadoAsistencias()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return mensajeResultado;
        }

        private List<Asistencias> obtenerAsistencias(DateTime fechaInicial, DateTime fechaFinal, PeriodosNomina periodoNomina, DateTime? fechaBajaFiniq, string claveEmpleado, string claveRazonSocial,
            string claveTipoNomina, string claveTipoCorrida, DateTime fechaRangoInicial, DateTime fechaRangoFinal, DBContextSimple dbContextSimple)
        {
            List<Asistencias> listAsistencias = null;
            try
            {
                if (periodoNomina == null)
                {
                    return new List<Asistencias>();
                }
                if (fechaBajaFiniq != null)
                {
                    PeriodosNomina periodoNomTemp = (from p in dbContextSimple.Set<PeriodosNomina>()
                                                     where p.tipoNomina.clave == claveTipoNomina &&
                                                           p.tipoCorrida.clave == claveTipoCorrida &&
                                                           (fechaBajaFiniq >= p.fechaInicial && fechaBajaFiniq <= p.fechaFinal)
                                                     select p).SingleOrDefault();
                    if (periodoNomTemp == null)
                    {
                        fechaInicial = fechaRangoInicial;
                        fechaFinal = fechaRangoFinal;
                    }
                    else
                    {
                        fechaInicial = periodoNomTemp.fechaInicial.GetValueOrDefault();
                        fechaFinal = periodoNomTemp.fechaFinal.GetValueOrDefault();
                    }
                }
                listAsistencias = (from a in dbContextSimple.Set<Asistencias>()
                                   where a.empleados.clave == claveEmpleado && a.tipoNomina.clave == claveTipoNomina && a.razonesSociales.clave == claveRazonSocial &&
                                         a.periodosNomina.tipoCorrida.clave == claveTipoCorrida && (a.fecha >= fechaInicial && a.fecha <= fechaFinal)
                                   select a).ToList();
                listAsistencias = listAsistencias == null ? new List<Asistencias>() : listAsistencias;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerAsistencias()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                listAsistencias = new List<Asistencias>();
            }
            return listAsistencias;
        }

        private List<RegistroIncapacidad> obtenerIncapacidadesPorEnfermedad(DateTime fechaInicial, DateTime fechaFinal, string claveEmpleado, string razonSocial, DBContextSimple dbContextSimple)
        {
            List<RegistroIncapacidad> listRegistroIncapacidad = null;
            try
            {

                int ramoEnfermedadGeneral = (int)RamoSeguro.ENFERMEDAD_GENERAL;
                listRegistroIncapacidad = (from a in dbContextSimple.Set<RegistroIncapacidad>()
                                           where a.ramoSeguro == ramoEnfermedadGeneral && a.empleados.clave == claveEmpleado &&
a.empleados.razonesSociales.clave == razonSocial && ((a.fechaInicial >= fechaInicial && a.fechaInicial <= fechaFinal) || (a.fechaFinal >= fechaInicial && a.fechaInicial <= fechaFinal))
                                           orderby a.fechaInicial
                                           select a).ToList();
                listRegistroIncapacidad = listRegistroIncapacidad == null ? new List<RegistroIncapacidad>() : listRegistroIncapacidad;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerIncapacidadesPorEnfermedad()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                listRegistroIncapacidad = new List<RegistroIncapacidad>();
            }
            return listRegistroIncapacidad;
        }

        private int cantidadDiasEntreDosFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            TimeSpan span = fechaFin - fechaInicio;
            return span.Days;
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