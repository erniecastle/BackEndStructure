
using Exitosw.Payroll.Core.modeloHB;
using Exitosw.Payroll.Hibernate.entidad;
using Exitosw.Payroll.Hibernate.modelo;
using Exitosw.Payroll.Hibernate.util;
using Exitosw.Payroll.TestCompilador.compilador;
using Exitosw.Payroll.TestCompilador.funciones;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Exceptions;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Exitosw.Payroll.Core.modelo
{
    public class CalculoNominaHDAO : NHibernateRepository<Object>, CalculoNominaHIF
    {

        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");

        private object[,] tablaIsr;
        private object[,] tablaFactorIntegracion;
        private object[,] tablaSubsidio;
        private object[,] tablaZonaSalarial;
        private Object[,] matrixcargaXml;
        private object[,] tablaIsrMes;
        private object[,] tablaSubsidioMes;
        public static XDocument docXML;
        //public static Element root;
        private List<ConcepNomDefi> filtroConceptosNomina;
        private List<MovNomConcep> filtroMovimientosNominas;
        private List<ConcepNomDefi> filtroConceptosNominaAux;
        private List<MovNomConcep> filtroMovimientosNominasAux;
        private List<ConcepNomDefi> filtroConceptosNominaAuxPlaza;
        private List<MovNomConcep> filtroMovimientosNominasAuxPlaza;
        private StringBuilder strQuery = new StringBuilder();
        private StringBuilder strWhere = new StringBuilder();
        private List<String> camposParametro;
        private List<Object> valoresParametro;
        private double? salarioMinimoDF;
        private ConfiguracionIMSS configuracionIMSS;
        private ZonaGeografica? zonaGeografica = null;
        private CompEjec compEjec = new CompEjec();
        private CompEjec.Compilador compilador = CompEjec.generaInstanciaCompilador();
        private Dictionary<string, object> valoresConceptosGlobales = new Dictionary<string, object>();
        private Dictionary<string, object> valoresConceptosEmpleados = new Dictionary<string, object>();
        private DateTime fechaActual = DateTime.Now;
        private DateTime? fechaBajaFiniq = null;
        private PeriodosNomina periodosNomina = null;
        private PtuDatosGenerales ptuDatosGenerales = null;
        private PtuEmpleados ptuEmpleado = null;
        private double isrNormal = 0, isrDirecto = 0, isrAnual = 0, isrNormalSinAjustar = 0, isrACargoNormalSinAjustar = 0, subsidioAlEmpleoNormalSinAjustar = 0;
        private double? factorMensual = 0.0, factorAnual = 0.0;
        private double acumuladoNormal = 0, acumuladoDirecto = 0, acumuladoAnual = 0, acumuladoImssFijo = 0;
        private bool calculoSeparadoISR = false, isMov2Meses, usaFiniquitos;
        private TipoTablaISR? tipoTablaISR = null;
        private double diasPago = 0.0;
        private int modoAjustarIngresosMes = -1;
        private int versionCalculoPrestamoAhorro = 1;
        private string[,] variablesConceptos = null;
        //////    private Periodicidad periodicidadAnual = null;
        private Periodicidad periodicidadTipoNomina = null;
        private bool? manejaPagosPorHora = null;
        private bool manejaPagoDiasNaturales = false;
        private bool manejaPagoIMSSDiasNaturales = false;
        private bool descontarFaltasModoAjustaMes = false;
        private ManejoHorasPor? manejoHorasPor = null;
        private ManejoSalarioDiario? manejoSalarioDiario = null;
        private CalculoISR retenido = null;//JSA23
        private CentroDeCosto centroDeCostoMovimiento = null;
        private List<DatosEspeciales> datosFormulas = null;
        private Spring.Util.Properties propertieFuente = null;
        private TipoCorrida tipoCorrida = null;
        private RazonesSociales razonesSociales = null;
        private int tipoPantallaSistema = 100;
        private int cantidadSaveUpdate = 0, cantidadFlush = 50;
        //    private MovNomConcep movNomConcepSubsidio = null;
        private List<MovNomConcep> listMovNomConcepSubsidio = null;
        private List<MovNomConcep> listMovNomConcepISRCARGO = null;
        private List<MovNomConcep> listMovNomConcepSUBSIDIOALEMPLEO = null;
        private MovNomConcep movNomConcepAjustePorRedondeo = null;
        private List<CalculoIMSS> listCalculoIMSS = null;
        private CalculoIMSSPatron calculoIMSSPatron = null;
        private CalculoISR iSRRetenido = null;//Para el concepto ISR JSA23
        private CalculoISR iSRRetenidoSubsidio = null;//Para el concepto subsidio JSA23
        private FiniquitosLiquida finiquitosLiquidaciones = null;
        private List<String> camposFiniquitos;
        private List<Object> valoresCamposFiniquitos;
        private String valoresDatosEspecialesFormula = "";
        private List<PlazasPorEmpleadosMov> filtroPlazasPorEmpleadosMov = null;
        private PlazasPorEmpleadosMov plazaEmpleadoaguinaldo = new PlazasPorEmpleadosMov();
        private IngresosBajas ingresosReingresosBajas = null;
        private String[] mascaraResultadoGral = new String[] { "#########", "##" };
        private Double factorRedondeoGral = .01, minimoRedondeoGral = .005;
        private TipoAccionMascaras tipoAccionMascarasGral = TipoAccionMascaras.Redondear;
        private double importeRedondeo = 0.0;
        private IList<TablaDatos> tablaDatosXml = new List<TablaDatos>();
        private List<DiasAguinaldo> diasAguinaldo = new List<DiasAguinaldo>();
        private AguinaldoConfiguracion aguiConfiguracion = new AguinaldoConfiguracion();
        private List<AguinaldoFechas> aguiFechas = new List<AguinaldoFechas>();
        private List<VacacionesAplicacion> vacacionesAplicacionStatus = new List<VacacionesAplicacion>();
        //////    //Se usa esta variable para guardar los retenidos cuando un periodo abarca 2 meses y estos aun no han sido guardados(commit).
        //////    double[] retenidosISRACargoYSubsidioAlEmpleoEn2Meses;
        private bool isCalculoSDI = false;
        private bool isCalculoPTU = false;
        private bool isCalculoAgui = false; ///isCalculoAgui = false;
        private bool isVacacionesStatus = false;
        private bool isUMA = false;
        private PagarPrimaVacionalyVacacionesAuto pagarVacaAuto;
        private ManejoSalarioVacaciones salarioVacaciones;
        private double? valorUMA = 0.0;

        //intentos para intentar obtener dato en tabla bloqueada
        private int contLockAcquisition = 0;
        private int pagarPrimero3Dias = 0;
        private double descontarDiasPago = 0.0;
        private int totalPagosAgui = 0;
        private VacacionesAplicacion vacacionAplicacionActual = null;
        private static int ProporcionaPeriodoIndependiente = 1, ProporcionaPeriodoAjustadoFinalMes = 2, ProporcionaPeriodoAjustadoCadaVez = 3,
                ProporcionaPeriodoConTablaAnual = 4, ProporcionaCadaPeriodoUtilizandoTablaPeriodo = 5, PropPeriodoIndepAjustadoAlUltimoPeriodoMes = 6, UltimoPeriodoSinAjustarMes = 7,
                ProporcionaTablaAnual = 8, PropPeriodoIndepDiasNaturales = 9;
        private List<String> variablesAjustadasEnDosPeriodos = new List<String>() { "Laborado".ToUpper(), "HorasExtrasDobles".ToUpper(), "HorasExtrasTriples".ToUpper(), "IncapacidadEnfermedad".ToUpper(), "IncapacidadAccidente".ToUpper(), "IncapacidadMaternidad".ToUpper(), "OtrasIncapacidad".ToUpper(), "DiasIncapacidadEmpleado".ToUpper(), "Faltas".ToUpper(), "Ausentismo".ToUpper(),
                "TExtrasDiaDescanso".ToUpper(), "TExtrasDiaFestivo".ToUpper(), "TExtrasDiaDomingo".ToUpper(), "Retardos".ToUpper(), "PermisoConSueldo".ToUpper().ToUpper(), "PermisoSinSueldo".ToUpper(), "DiasFestivos".ToUpper(), "DiasDescanso".ToUpper(), "diasVacaciones".ToUpper(), "diasPrima".ToUpper(), "DiasNormalesAPagar".ToUpper(), "DiasPago".ToUpper(), "DiasCotizados".ToUpper(),
                "ISRNeto".ToUpper(), "ISRSubsidio".ToUpper(), "ISRNeto".ToUpper(), "ISRSubsidio".ToUpper(), "ISRACARGO".ToUpper(), "SubsEmpleoCalculado".ToUpper() };
        private string parametroFechaFinal = "FechaRangoFinal".ToUpper();
        private string parametroFechaInicial = "FechaRangoInicial".ToUpper();
        private string nombreFuenteDatos = "";
        VacacionesDevengadasHDAO vacDev = new VacacionesDevengadasHDAO();
        //private MetodosDatosEmpleados metodosDatosEmpleados = new MetodosDatosEmpleados();
        //private MetodosParaVacaciones metodosParaVacaciones = new MetodosParaVacaciones();
        //private MetodosParaPtu metodosParaPtu = new MetodosParaPtu();
        //private MetodosDatosAsistencias metodosDatosAsistencias = new MetodosDatosAsistencias();
        //private MetodosParaMovimientosNomina metodosParaMovimientosNomina = new MetodosParaMovimientosNomina();
        //private MetodosPeriodosNomina metodosPeriodosNomina = new MetodosPeriodosNomina();
        //private MetodosBDMaestra metodosBDMaestra = new MetodosBDMaestra();
        private string clavePeriodoFuncion = "";
        private MovNomConcep movNomConcepGlobal;
        //private DbContextTransaction transacion;
        //private DbContext dbContextSimple;
        //private DbContext dbContextMaestra;
        //private DBContextAdapter dbContextAdapterSimple;
        //private DBContextAdapter dbContextAdapterMaestra;
        //////"fechaSalidaVacaciones","fechaRegresoVacaciones", "tipoVacaciones", 
        private ValorTablaISR valoresTablaISR;
        private int ENERO = 1;
        Stopwatch tiempo = new Stopwatch();
        static String CONSULTA_CONCEPTO_CON_NOMENCLATURA = "Select new list(concat('CONCEP_',c.clave) , concat('CONCEP_',c.descripcion)) From ConcepNomDefi c Where c.fecha in (Select Max(cc.fecha) From ConcepNomDefi cc Where cc.clave = c.clave) Order By c.clave";
        private void initVariablesCalculo()
        {
            valoresDatosEspecialesFormula = "";
            contLockAcquisition = 0;
            filtroPlazasPorEmpleadosMov = null;
            isCalculoSDI = false;
            isUMA = false;
            factorMensual = 0.0;
            factorAnual = 0.0;
            acumuladoNormal = 0;
            acumuladoDirecto = 0;
            acumuladoAnual = 0;
            acumuladoImssFijo = 0;
            isrNormal = 0;
            isrDirecto = 0;
            isrAnual = 0;
            isrNormalSinAjustar = 0;
            isrACargoNormalSinAjustar = 0;
            subsidioAlEmpleoNormalSinAjustar = 0;
            listMovNomConcepSubsidio = new List<MovNomConcep>();
            listMovNomConcepISRCARGO = new List<MovNomConcep>();
            listMovNomConcepSUBSIDIOALEMPLEO = new List<MovNomConcep>();
            listCalculoIMSS = null;
            calculoIMSSPatron = null;
            iSRRetenido = null;
            iSRRetenidoSubsidio = null;
            importeRedondeo = 0.0;
            valoresConceptosGlobales = new Dictionary<string, object>();
            valoresConceptosEmpleados = new Dictionary<string, object>();
            modoAjustarIngresosMes = -1;
            tipoTablaISR = null;
            manejoHorasPor = null;
            manejaPagoDiasNaturales = false;
            manejaPagoIMSSDiasNaturales = false;
            manejoSalarioDiario = null;
            manejaPagosPorHora = null;
            fechaActual = DateTime.Now;
            cantidadSaveUpdate = 0;
            fechaBajaFiniq = null;
            finiquitosLiquidaciones = null;
            descontarFaltasModoAjustaMes = false;
            //////            retenidosISRACargoYSubsidioAlEmpleoEn2Meses = null;
            //////            agregaronPlazaPorEmpleadoRestantes = false;
        }
        //private void cargarFactoresyTablasXml(EntityPruebaObtFacCal factoresCalculo)
        //{
        //    calculoSeparadoISR = factoresCalculo.calculoSeparadoISR;
        //    descontarFaltasModoAjustaMes = factoresCalculo.descontarFaltasModoAjustaMes;
        //    factorAnual = factoresCalculo.factorAnual;
        //    factorMensual = factoresCalculo.factorMensual;
        //    isUMA = factoresCalculo.isUMA;
        //    manejaPagoDiasNaturales = factoresCalculo.manejaPagoDiasNaturales;
        //    manejaPagoIMSSDiasNaturales = factoresCalculo.manejaPagoIMSSDiasNaturales;
        //    manejaPagosPorHora = factoresCalculo.manejaPagosPorHora;
        //    manejoHorasPor = factoresCalculo.manejoHorasPor;
        //    manejoSalarioDiario = factoresCalculo.manejoSalarioDiario;
        //    modoAjustarIngresosMes = factoresCalculo.modoAjustarIngresosMes;
        //    pagarVacaAuto = factoresCalculo.pagarVacaAuto;
        //    salarioVacaciones = factoresCalculo.salarioVacaciones;
        //    tipoTablaISR = factoresCalculo.tipoTablaISR;
        //    versionCalculoPrestamoAhorro = factoresCalculo.versionCalculoPrestamoAhorro;
        //    tablaIsr = factoresCalculo.tablaIsr;
        //    tablaFactorIntegracion = factoresCalculo.tablaFactorIntegracion;
        //    tablaSubsidio = factoresCalculo.tablaSubsidio;
        //    tablaZonaSalarial = factoresCalculo.tablaZonaSalarial;
        //    matrixcargaXml = factoresCalculo.matrixcargaXml;
        //    tablaIsrMes = factoresCalculo.tablaIsrMes;
        //    tablaSubsidioMes = factoresCalculo.tablaIsrMes;
        //    tablaDatosXml = factoresCalculo.tablaDatosXml;

        //}
        public Mensaje calculaNomina(string claveEmpIni, string claveEmpFin, string claveTipoNomina, string claveTipoCorrida, decimal? idPeriodoNomina,
            string clavePuesto, string claveCategoriasPuestos, string claveTurno, string claveRazonSocial, string claveRegPatronal, string claveFormaDePago,
            string claveDepto, string claveCtrCosto, int? tipoSalario, string tipoContrato, bool? status, string controlador, int uso,
            ParametrosExtra parametrosExtra, int ejercicioActivo, Object factoresCalculo, ISession sessionSimple, ISession sessionMaster)
        {
            bool band = true;
            try
            {


                mensajeResultado = new Mensaje();
                mensajeResultado.error = "";
                mensajeResultado.noError = 0;
                initVariablesCalculo();
                //cargarFactoresyTablasXml(factoresCalculo);
                fechaActual.AddYears(ejercicioActivo);
                List<CreditoAhorro> listCreditoAhorro;
                List<MovNomConcep> filtroMovimientosNominasCreditosAhorro;
                List<MovNomConcep> listMovNomConcepCreditosAhorroDescuentoActivo;
                List<MovNomConcep> formulaDedudCreditos;
                List<MovNomConcep> formulaDedudAhorros;
                #region asignacion de fechas mediante los parametrosExtra
                if (parametrosExtra != null)
                {
                    if (parametrosExtra.fechaCalculoFiniq != null)
                    {
                        fechaActual = parametrosExtra.fechaCalculoFiniq.GetValueOrDefault();
                    }
                    if (parametrosExtra.fechaBajaFiniq != null)
                    {
                        fechaBajaFiniq = parametrosExtra.fechaBajaFiniq;
                    }
                    if (parametrosExtra.valoresExtras != null)
                    {
                        for (int i = 0; i < parametrosExtra.valoresExtras.Count(); i++)
                        {
                            if (parametrosExtra.valoresExtras[i].GetType().Equals(typeof(FiniquitosLiquida)))
                            {
                                finiquitosLiquidaciones = (FiniquitosLiquida)parametrosExtra.valoresExtras[i];
                            }
                        }
                    }
                    if (parametrosExtra.mascaraResultado != null)
                    {
                        mascaraResultadoGral = parametrosExtra.mascaraResultado;
                        if (mascaraResultadoGral[1].Length > 0)
                        {
                            String factorString = ".", minimunString = ".";
                            for (int i = 0; i < mascaraResultadoGral[1].Length - 1; i++)
                            {
                                factorString += "0";
                            }
                            minimunString = factorString;
                            factorString += "1";
                            minimunString += "05";
                            factorRedondeoGral = Double.Parse(factorString);
                            minimoRedondeoGral = Double.Parse(minimunString);
                        }
                    }
                    if (parametrosExtra.tipoAccionMascaras != null)
                    {
                        tipoAccionMascarasGral = parametrosExtra.tipoAccionMascaras == null ? TipoAccionMascaras.Ninguno : parametrosExtra.tipoAccionMascaras.GetValueOrDefault();
                    }
                }
                DateTime cIni, cFin;
                if (parametrosExtra.fechaInicioPeriodo == null | parametrosExtra.fechaInicioPeriodo == null)
                {
                    parametrosExtra.fechaInicioPeriodo = fechaActual;
                    parametrosExtra.fechaFinPeriodo = fechaActual;
                }
                cIni = parametrosExtra.fechaInicioPeriodo.GetValueOrDefault();
                cFin = parametrosExtra.fechaFinPeriodo.GetValueOrDefault();
                #endregion

                #region busca la periocidad del periodo de nomina
                buscaPeriodicidadesOrPeriodosNomina(claveTipoNomina, claveTipoCorrida, idPeriodoNomina, sessionSimple);

                if (mensajeResultado.noError != 0)
                {
                    return mensajeResultado;
                }
                //tiempo.Stop();
                #endregion
                #region busca Calculos de ptu
                buscaCalculoPTU(claveRazonSocial, periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault(), sessionSimple);
                if (mensajeResultado.noError != 0)
                {
                    return mensajeResultado;
                }
                #endregion

                #region genera tablas Xml 
                //generaTablasXml(controlador, periodicidadTipoNomina, claveRazonSocial, periodosNomina.fechaFinal.Value, ejercicioActivo, sessionMaster);
                //if (mensajeResultado.noError != 0)
                //{
                //    return mensajeResultado;
                //}
                #endregion
                //tiempo.Stop();
                //tiempo.Start();
                setSession(sessionSimple);  //crea conexion
                getSession().BeginTransaction();
                cargarVariablesConceptosCompilador();
                if (mensajeResultado.noError != 0)
                {
                    return mensajeResultado;
                }
                //tiempo.Stop();
                cargaVariablesGlobales(claveTipoNomina, claveTipoCorrida, clavePuesto, claveCategoriasPuestos, claveTurno, claveRazonSocial,
                                    claveRegPatronal, claveDepto, claveCtrCosto);
                if (mensajeResultado.noError != 0)
                {
                    return mensajeResultado;
                }
                valoresConceptosGlobales.Add(parametroFechaInicial, cIni);
                valoresConceptosGlobales.Add(parametroFechaFinal, cFin);
                valoresConceptosGlobales.Add("ejercicioActivo".ToUpper(), ejercicioActivo);
                inicializaValoresPeriodoNomina(periodosNomina);
                cargaDatosCalculoIMSS(periodosNomina.fechaFinal.Value);
                idPeriodoNomina = periodosNomina.id;
                tiempo.Start();
                listCreditoAhorro = obtenerCreditosAhorro(razonesSociales);
                tiempo.Stop();
                consultarConfiguracionAgui();
                if (mensajeResultado.noError != 0)
                {
                    return mensajeResultado;
                }

                #region calcula las vacaciones devengadas de empleado
                //mensajeResultado = metodosParaVacaciones.calcularVacacionesDevengadasEmpleados(razonesSociales, tablaFactorIntegracion, (DBContextSimple)dbContextSimple, (DBContextMaster)dbContextMaestra);
                //if (mensajeResultado.noError != 0)
                //{
                //    return mensajeResultado;
                //}pendiente
                Mensaje men = vacDev.calcularVacacionesDevengadasEmpleados(razonesSociales, sessionSimple, sessionMaster, false);

                if (men.noError != 0)
                {
                    return men;
                }

                #endregion

                #region verifica si hay calculo de aguinaldo en el periodo
                DateTime fechaIniPer = periodosNomina.fechaInicial.Value.Date;
                DateTime fechaFinPer = periodosNomina.fechaFinal.Value.Date;
                for (int i = 0; i < aguiFechas.Count; i++)
                {
                    DateTime fechaAgui = aguiFechas[i].fechaProgramada.Value.Date;
                    if ((fechaAgui.CompareTo(fechaIniPer) == 0 || fechaAgui.CompareTo(fechaIniPer) > 0)
                            && (fechaAgui.CompareTo(fechaFinPer) == 0 || fechaAgui.CompareTo(fechaFinPer) < 0))
                    {
                        isCalculoAgui = true;
                        break;
                    }
                }
                #endregion
                //tiempo.Stop();


                //tiempo.Start();
                filtroPlazasPorEmpleadosMov = obtenerPlazasPorEmpleados(claveEmpIni, claveEmpFin, claveTipoNomina, clavePuesto, claveCategoriasPuestos,
                    claveTurno, claveRazonSocial, claveRegPatronal, claveDepto, claveCtrCosto, tipoSalario, tipoContrato, status, claveTipoCorrida,
                    claveFormaDePago, parametrosExtra.fechaInicioPeriodo.GetValueOrDefault(), parametrosExtra.fechaFinPeriodo.GetValueOrDefault(), false);

                if (mensajeResultado.noError != 0)
                {
                    return mensajeResultado;
                }

                #region carga la variables de configuracion del imss
                cargaDatosVariableConfiguracionIMSS(periodosNomina.fechaFinal.GetValueOrDefault());

                #endregion
                //  tiempo.Stop();
                if (filtroPlazasPorEmpleadosMov != null)
                {
                    int i;
                    #region recorre la lista de plazas por empleado movimientos 
                    // tiempo.Start();
                    for (i = 0; i < filtroPlazasPorEmpleadosMov.Count; i++)
                    {
                        #region verifica si se le van a pagar las vacaciones en automatico
                        if (pagarVacaAuto != PagarPrimaVacionalyVacacionesAuto.MANUAL)
                        {
                            agregarVacacionesAuto(razonesSociales, filtroPlazasPorEmpleadosMov[i], periodosNomina, tablaFactorIntegracion, pagarVacaAuto);

                        }
                        #endregion

                        #region en esta parte se verifica si el empleado tiene reingreso o baja 
                        if (i > 0)
                        {
                            if (!String.Equals(filtroPlazasPorEmpleadosMov[i - 1].plazasPorEmpleado.empleados.clave, filtroPlazasPorEmpleadosMov[i].plazasPorEmpleado.empleados.clave, StringComparison.OrdinalIgnoreCase) ||
                                (String.Equals(filtroPlazasPorEmpleadosMov[i - 1].plazasPorEmpleado.empleados.clave, filtroPlazasPorEmpleadosMov[i].plazasPorEmpleado.empleados.clave, StringComparison.OrdinalIgnoreCase) &&
                                !String.Equals(filtroPlazasPorEmpleadosMov[i - 1].plazasPorEmpleado.referencia, filtroPlazasPorEmpleadosMov[i].plazasPorEmpleado.referencia, StringComparison.OrdinalIgnoreCase)))
                            {
                                acumuladoNormal = 0.0;
                                acumuladoDirecto = 0.0;
                                acumuladoAnual = 0.0;
                                acumuladoImssFijo = 0.0;
                                listMovNomConcepSubsidio.Clear();
                                listMovNomConcepISRCARGO.Clear();
                                listMovNomConcepSUBSIDIOALEMPLEO.Clear();
                                listCalculoIMSS = null;
                                calculoIMSSPatron = null;
                                iSRRetenido = null;
                                iSRRetenidoSubsidio = null;
                                importeRedondeo = 0.0;
                                valoresConceptosGlobales[parametroFechaInicial] = cIni;
                                valoresConceptosGlobales[parametroFechaFinal] = cFin;
                                valoresConceptosEmpleados.Clear();
                                if (!String.Equals(filtroPlazasPorEmpleadosMov[i - 1].plazasPorEmpleado.empleados.clave,
                                    filtroPlazasPorEmpleadosMov[i].plazasPorEmpleado.empleados.clave, StringComparison.OrdinalIgnoreCase))
                                {
                                    //  mensajeResultado = metodosDatosEmpleados.obtenerIngresosReIngresosBajas(filtroPlazasPorEmpleadosMov[i], periodosNomina.fechaFinal, (DBContextSimple)dbContextSimple);
                                    obtenerIngresosReingresosBajas(filtroPlazasPorEmpleadosMov[i]);

                                }
                            }
                        }
                        else
                        {
                            obtenerIngresosReingresosBajas(filtroPlazasPorEmpleadosMov[i]);
                            //  mensajeResultado = metodosDatosEmpleados.obtenerIngresosReIngresosBajas(filtroPlazasPorEmpleadosMov[i], periodosNomina.fechaFinal, (DBContextSimple)dbContextSimple);

                        }
                        #endregion
                        #region busca Calculos de ptu
                        ptuEmpleado = null;
                        if (isCalculoPTU)
                        {
                            buscaEmpleadoPTU(claveRazonSocial, filtroPlazasPorEmpleadosMov[i].plazasPorEmpleado.empleados.clave);
                        }
                        #endregion
                        //agrega valoresConceptosGlobales a valoresConceptosEmpleados
                        valoresConceptosEmpleados = valoresConceptosEmpleados.Concat(valoresConceptosGlobales).ToDictionary(e => e.Key, e => e.Value);

                        #region crear o obtiene la lista de calculos de unidades 
                        List<CalculoUnidades> listCalculoUnidades = obtenerListCalculoUnidadesUtilizar(claveRazonSocial, filtroPlazasPorEmpleadosMov[i].plazasPorEmpleado, claveTipoNomina, periodosNomina.id, claveTipoCorrida);
                        List<object> ids = new List<object>();
                        int j;
                        for (j = 0; j < listCalculoUnidades.Count(); j++)
                        {

                            if (listCalculoUnidades[j].id > 0)
                            {
                                ids.Add(listCalculoUnidades[j].id);
                                listCalculoUnidades[j] = new CalculoUnidades(listCalculoUnidades[j]);
                            }

                        }
                        if (ids.Count() > 0)
                        {
                            deleteListQuery(typeof(CalculoUnidades).Name, "id", ids.ToArray());
                            getSession().Flush();
                            getSession().Clear();

                        }
                        #endregion
                        cargarVariablesGlobalesEmpleadoPorPlaza(filtroPlazasPorEmpleadosMov[i], true, true, TipoSueldos.SUELDO_DIARIO_FINAL, listCalculoUnidades[0], null, false, null);
                        if (mensajeResultado.noError != 0)
                        {
                            return mensajeResultado;
                        }

                        bool continueProsesoCal = true;
                        if (String.Equals(tipoCorrida.clave, "VAC", StringComparison.OrdinalIgnoreCase))
                        {
                            int? diasVac = (int)valoresConceptosEmpleados["DiasVacaciones".ToUpper()];
                            double? diasPrima = (double)valoresConceptosEmpleados["diasPrima".ToUpper()];
                            if ((diasVac == null || diasVac == 0) && (diasPrima == null || diasPrima == 0.0))
                            {
                                continueProsesoCal = false;
                            }
                        }

                        if (continueProsesoCal)
                        {

                            #region en esta parte hace una parte del calculo de aguinaldo
                            if (isCalculoAgui)
                            {
                                if (diasAguinaldo.Count > 0)
                                {
                                    int antiguedad = (int)valoresConceptosEmpleados["Antiguedad".ToUpper()];
                                    for (j = 0; j < diasAguinaldo.Count; j++)
                                    {
                                        int ant = Convert.ToInt32(diasAguinaldo[j].antiguedad);
                                        if (ant == antiguedad)
                                        {
                                            //valoresConceptosEmpleados.put("FactorDiasAguinaldo".ToUpper(), diasAguinaldo.get(j).getDias());
                                            valoresConceptosEmpleados.Add("DiasAguinaldo".ToUpper(), Convert.ToInt32(diasAguinaldo[j].dias));
                                            break;
                                        }
                                        else if (antiguedad > 0 && antiguedad < ant)
                                        {
                                            //                                        valoresConceptosEmpleados.put("FactorDiasAguinaldo".ToUpper(), diasAguinaldo.get(j - 1 < 0 ? 0 : j - 1).getDias());
                                            valoresConceptosEmpleados.Add("DiasAguinaldo".ToUpper(), Convert.ToInt32(diasAguinaldo[j - 1 < 0 ? 0 : j - 1].dias));
                                            break;
                                        }
                                        else if (diasAguinaldo.Count - 1 == j && antiguedad != 0)
                                        {
                                            //                                        valoresConceptosEmpleados.put("FactorDiasAguinaldo".ToUpper(), diasAguinaldo.get(j).getDias());
                                            valoresConceptosEmpleados.Add("DiasAguinaldo".ToUpper(), Convert.ToInt32(diasAguinaldo[j].dias));
                                        }
                                    }

                                }
                                double sueldoFinal = (double)valoresConceptosEmpleados["SUELDODIARIOFINAL".ToUpper()];
                                int? diasAginaldo = (int?)valoresConceptosEmpleados["DiasAguinaldo".ToUpper()];
                                double aguinaldoTotal = sueldoFinal * Convert.ToDouble((diasAginaldo == null ? 0 : diasAginaldo));
                                valoresConceptosEmpleados.Add("ImporteAguinaldo".ToUpper(), aguinaldoTotal);
                            }
                            #endregion

                            try
                            {
                                getSession().SaveOrUpdate(listCalculoUnidades[0]);
                            }
                            catch (Exception ex)
                            {
                                mensajeResultado.error = ex.GetBaseException().Message;
                                mensajeResultado.noError = 222;
                            }

                            if (mensajeResultado.noError != 0)
                            {
                                break;
                            }

                            #region obtener movimientos nomina
                            if (String.Equals(claveTipoCorrida, "FIN", StringComparison.OrdinalIgnoreCase) || String.Equals(claveTipoCorrida, "LIQ", StringComparison.OrdinalIgnoreCase))
                            {

                                filtroMovimientosNominas.AddRange(obtenerMovimientosPlazasFiniquitos(claveTipoCorrida, claveTipoNomina, idPeriodoNomina.GetValueOrDefault(), filtroPlazasPorEmpleadosMov[i].plazasPorEmpleado, claveCtrCosto, claveRazonSocial, uso));
                                if (!filtroMovimientosNominas.Any() & mensajeResultado.noError == 0)
                                {
                                    mensajeResultado.noError = 888;
                                    mensajeResultado.error = "No tiene conceptos finiquitos agregados";
                                }
                            }
                            else
                            {

                                obtenerMovimientosNominaPorPlaza(claveTipoCorrida, claveTipoNomina, periodosNomina.id, filtroPlazasPorEmpleadosMov[i].plazasPorEmpleado, claveCtrCosto, claveRazonSocial);

                            }

                            if (mensajeResultado.noError != 0)
                            {
                                break;
                            }
                            #endregion

                            //  NHibernateUtil.Initialize(listCreditoAhorro[0]);
                        
                            Object[] listMovTmp = obtenerMovimientosNominaCreditoAhorro(listCreditoAhorro);
                            filtroMovimientosNominasCreditosAhorro = (List<MovNomConcep>)listMovTmp[0];
                            listMovNomConcepCreditosAhorroDescuentoActivo = (List<MovNomConcep>)listMovTmp[1];
                            formulaDedudCreditos = (List<MovNomConcep>)listMovTmp[2];
                            formulaDedudAhorros = (List<MovNomConcep>)listMovTmp[3];
                            obtenerMovimientosNominaISRACargoYSubsidioAlEmpleado();

                            filtroMovimientosNominas = filtroMovimientosNominas.Except(listMovNomConcepISRCARGO).ToList();
                            // filtroMovimientosNominas = filtroMovimientosNominas.Except(listMovNomConcepSUBSIDIOALEMPLEO).ToList();

                            int conta = 0;

                            List<Object> idsMovDeleteCorrida = new List<Object>();
                            MovimientosNominaHDAO movimientosNominaDAO2 = new MovimientosNominaHDAO();
                            movimientosNominaDAO2.setSession(getSession());
                            while (conta < filtroMovimientosNominas.Count)
                            {
                                int numCorridas = filtroMovimientosNominas[conta].concepNomDefi.conceptoPorTipoCorrida.Count;
                                for (j = 0; j < numCorridas; j++)
                                {
                                    if (String.Equals(filtroMovimientosNominas[conta].concepNomDefi.conceptoPorTipoCorrida[j].tipoCorrida.clave, claveTipoCorrida))
                                    {
                                        break;
                                    }
                                    else if (j == numCorridas - 1)
                                    {
                                        idsMovDeleteCorrida.Add(filtroMovimientosNominas[conta].id);
                                        filtroMovimientosNominas.Remove(filtroMovimientosNominas[conta]);
                                    }

                                }
                                conta++;
                            }

                            if (idsMovDeleteCorrida.Count > 0)
                            {
                                movimientosNominaDAO2.deleteListQueryMov(typeof(MovNomConcep).Name, "id", idsMovDeleteCorrida.ToArray(), null, null, null, true);
                                if (mensajeResultado.noError != 0)
                                {
                                    break;
                                }
                            }

                            removerConceptosAguinaldo(claveTipoCorrida);
                            if (mensajeResultado.noError != 0)
                            {
                                break;
                            }
                            #region se agrego esto para poder calcular cuando tiene mas plazas el empleado y tambien es para una plaza
                            List<MovNomConcep> listaAuxMov = new List<MovNomConcep>();

                            for (int h = 0; h < filtroMovimientosNominas.Count; h++)
                            {

                                if (filtroMovimientosNominas[h].concepNomDefi.activarPlaza)
                                {
                                    List<PlazasPorEmpleadosMov> plazaEmpleadoMov = obtenerPlazasPorEmpleados(filtroMovimientosNominas[h].plazasPorEmpleado.empleados.clave,
                                        filtroMovimientosNominas[h].plazasPorEmpleado.empleados.clave,
                                        filtroMovimientosNominas[h].tipoNomina.clave, clavePuesto,
                                        filtroMovimientosNominas[h].concepNomDefi.categoriaPuestos == null ? "" : filtroMovimientosNominas[h].concepNomDefi.categoriaPuestos.clave,
                                        claveTurno, filtroMovimientosNominas[h].razonesSociales.clave,
                                        claveRegPatronal, claveDepto,
                                        claveCtrCosto, tipoSalario, tipoContrato, status,
                                        claveTipoCorrida, claveFormaDePago,
                                        filtroMovimientosNominas[h].periodosNomina.fechaInicial.GetValueOrDefault(),
                                        filtroMovimientosNominas[h].periodosNomina.fechaFinal.GetValueOrDefault(), true);

                                    if (filtroMovimientosNominas[h].concepNomDefi.activarDesglose)
                                    {
                                        for (int k = 0; k < plazaEmpleadoMov.Count; k++)
                                        {
                                            if (plazaEmpleadoMov[k].plazasPorEmpleado.id != filtroPlazasPorEmpleadosMov[i].plazasPorEmpleado.id)
                                            {
                                                obtenerMovimientosNominaPorPlaza2(claveTipoCorrida, claveTipoNomina, periodosNomina.id, plazaEmpleadoMov[j].plazasPorEmpleado, claveCtrCosto, claveRazonSocial, true);

                                                for (int d = 0; d < filtroMovimientosNominasAuxPlaza.Count; d++)
                                                {
                                                    if (filtroMovimientosNominasAuxPlaza[d].concepNomDefi.id == filtroMovimientosNominas[h].concepNomDefi.id)
                                                    {
                                                        cargarVariablesGlobalesEmpleadoPorPlaza(plazaEmpleadoMov[k], true, true, TipoSueldos.SUELDO_DIARIO_FINAL, listCalculoUnidades[0], null, false, null);
                                                        filtroMovimientosNominasAuxPlaza[d].numero = filtroMovimientosNominas[h].numero + 1;
                                                        ejecutaConceptosPorMovimientoNomina(filtroMovimientosNominasAuxPlaza[d], claveTipoCorrida, plazaEmpleadoMov[k], k, listCalculoUnidades, true);
                                                        //listaAuxMov.Add(movNomConcepGlobal);
                                                        break;
                                                    }
                                                }

                                            }
                                        }
                                        cargarVariablesGlobalesEmpleadoPorPlaza(filtroPlazasPorEmpleadosMov[i], true, true, TipoSueldos.SUELDO_DIARIO_FINAL, listCalculoUnidades[0], null, false, null);
                                        ejecutaConceptosPorMovimientoNomina(filtroMovimientosNominas[h], claveTipoCorrida, filtroPlazasPorEmpleadosMov[i], i, listCalculoUnidades, true);
                                    }
                                    else
                                    {
                                        List<Object> idsMovDeleteplaza = new List<object>();
                                        MovimientosNominaHDAO movimientosNominaDAOplaza = new MovimientosNominaHDAO();
                                        movimientosNominaDAOplaza.setSession(getSession());
                                        for (int k = 0; k < plazaEmpleadoMov.Count; k++)
                                        {
                                            if (plazaEmpleadoMov[k].plazasPorEmpleado.id != filtroPlazasPorEmpleadosMov[i].plazasPorEmpleado.id)
                                            {
                                                obtenerMovimientosNominaPorPlaza2(claveTipoCorrida, claveTipoNomina, periodosNomina.id, plazaEmpleadoMov[j].plazasPorEmpleado, claveCtrCosto, claveRazonSocial, true);

                                                for (int d = 0; d < filtroMovimientosNominasAuxPlaza.Count; d++)
                                                {
                                                    if (filtroMovimientosNominasAuxPlaza[d].concepNomDefi.id == filtroMovimientosNominas[h].concepNomDefi.id)
                                                    {
                                                        cargarVariablesGlobalesEmpleadoPorPlaza(plazaEmpleadoMov[k], true, true, TipoSueldos.SUELDO_DIARIO_FINAL, listCalculoUnidades[0], null, false, null);
                                                        filtroMovimientosNominasAuxPlaza[d].numero = filtroMovimientosNominas[h].numero + 1;
                                                        ejecutaConceptosPorMovimientoNomina(filtroMovimientosNominasAuxPlaza[d], claveTipoCorrida, plazaEmpleadoMov[k], k, listCalculoUnidades, false);
                                                        if (movNomConcepGlobal.id > 0)
                                                        {
                                                            idsMovDeleteplaza.Add(movNomConcepGlobal.id);
                                                        }
                                                        listaAuxMov.Add(movNomConcepGlobal);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        if (idsMovDeleteplaza.Count > 0)
                                        {
                                            movimientosNominaDAOplaza.deleteListQueryMov(typeof(MovNomConcep).Name, "id", idsMovDeleteplaza.ToArray(), null, null, null, true); //pendiente
                                            if (mensajeResultado.noError != 0)
                                            {
                                                break;
                                            }
                                        }
                                        cargarVariablesGlobalesEmpleadoPorPlaza(filtroPlazasPorEmpleadosMov[i], true, true, TipoSueldos.SUELDO_DIARIO_FINAL, listCalculoUnidades[0], null, false, null);
                                        ejecutaConceptosPorMovimientoNomina(filtroMovimientosNominas[h], claveTipoCorrida, filtroPlazasPorEmpleadosMov[i], i, listCalculoUnidades, false);
                                        listaAuxMov.Add(movNomConcepGlobal);
                                        double resultado = 0.0;
                                        double calculado = 0.0;
                                        for (int l = 0; l < listaAuxMov.Count; l++)
                                        {
                                            resultado = resultado + listaAuxMov[l].resultado.GetValueOrDefault();
                                            calculado = calculado + listaAuxMov[l].calculado.GetValueOrDefault();
                                        }
                                        filtroMovimientosNominas[h].resultado = (resultado);
                                        filtroMovimientosNominas[h].calculado = (calculado);
                                        saveOrUpdateOrDeleteMovimientosNomina(filtroMovimientosNominas[h], false, true);
                                        if (mensajeResultado.noError == -101)
                                        {
                                            mensajeResultado.noError = (54);
                                            break;
                                        }

                                    }
                                }
                                else
                                {

                                    ejecutaConceptosPorMovimientoNomina(filtroMovimientosNominas[h], claveTipoCorrida, filtroPlazasPorEmpleadosMov[i], i, listCalculoUnidades, true);

                                }

                            }
                            #endregion
                            #region Calculo de aguinaldo
                            if (isCalculoAgui)
                            {
                                plazaEmpleadoaguinaldo = filtroPlazasPorEmpleadosMov[i];
                                calcularAguinaldo();
                            }
                            if (mensajeResultado.noError != 0)
                            {
                                break;
                            }
                            #endregion
                            if (isVacacionesStatus)
                            {
                                for (int k = 0; k < vacacionesAplicacionStatus.Count(); k++)
                                {
                                    VacacionesDisfrutadas va = new VacacionesDisfrutadas();
                                    va = vacacionesAplicacionStatus[k].vacacionesDisfrutadas;
                                    va.statusVacaciones = (int)StatusVacaciones.CALCULADA;
                                    va.tipoCorridaAplicacion = periodosNomina.tipoCorrida;
                                    getSession().SaveOrUpdate(va);
                                }
                            }

                            if (ptuEmpleado != null)
                            {
                                getSession().SaveOrUpdate(ptuEmpleado);
                            }

                            DateTime x = DateTime.Now;
                            x = periodosNomina.fechaFinal.GetValueOrDefault();
                            object percepcion = 0.0, deduccion = 0.0;
                            if (!isCalculoAgui && filtroMovimientosNominas.Count() == 0)
                            {
                                percepcion = 0.0;
                                deduccion = 0.0;
                            }
                            else
                            {
                                percepcion = movimientosAcumuladoPorRangoMeses(TipoClasificacionFormula.DATOPERIODO, x, string.Concat(typeof(MovNomConcep).Name, ".resultado"), new String[] { string.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.naturaleza") }, new Object[] { Naturaleza.PERCEPCION }, TipoMostrarCampo.SUMA, null, null);
                                if (mensajeResultado.noError != 0)
                                {
                                    break;
                                }
                                deduccion = movimientosAcumuladoPorRangoMeses(TipoClasificacionFormula.DATOPERIODO, x, string.Concat(typeof(MovNomConcep).Name, ".resultado"), new String[] { string.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.naturaleza") }, new Object[] { Naturaleza.DEDUCCION }, TipoMostrarCampo.SUMA, null, null);
                                if (mensajeResultado.noError != 0)
                                {
                                    break;
                                }

                                for (int n = 0; n < filtroMovimientosNominasCreditosAhorro.Count(); n++)
                                {
                                    if (filtroMovimientosNominasCreditosAhorro[n].resultado != null ? filtroMovimientosNominasCreditosAhorro[n].resultado > 0 : false)
                                    {
                                        if (filtroMovimientosNominasCreditosAhorro[n].concepNomDefi.naturaleza.Equals(Naturaleza.PERCEPCION))
                                        {
                                            percepcion = Convert.ToDouble(percepcion.ToString()) - filtroMovimientosNominasCreditosAhorro[n].resultado;
                                        }
                                        else if (filtroMovimientosNominasCreditosAhorro[n].concepNomDefi.naturaleza.Equals(Naturaleza.DEDUCCION))
                                        {
                                            deduccion = Convert.ToDouble(deduccion.ToString()) - filtroMovimientosNominasCreditosAhorro[n].resultado;
                                        }
                                    }
                                }

                            }
                            bool continuarProcesos = true;
                            if ((Convert.ToDouble(percepcion.ToString()) - Convert.ToDouble(deduccion.ToString())) <= 0 || filtroMovimientosNominasCreditosAhorro.Count() == 0)
                            {
                                continuarProcesos = false;
                            }
                            if (movNomConcepAjustePorRedondeo != null)
                            {
                                double importeRedondeoTmp = importeRedondeo;
                                if (importeRedondeoTmp < 0)
                                {
                                    importeRedondeoTmp = -(importeRedondeoTmp);
                                }
                                importeRedondeoTmp = aplicarMascara(null, importeRedondeoTmp, true);
                                valoresConceptosEmpleados.Add("AjustePorRedondeo".ToUpper(), importeRedondeoTmp);
                                movNomConcepAjustePorRedondeo.resultado = ejecutaFormula(movNomConcepAjustePorRedondeo.concepNomDefi.formulaConcepto);
                                movNomConcepAjustePorRedondeo.calculado = movNomConcepAjustePorRedondeo.resultado;
                                if (movNomConcepAjustePorRedondeo.resultado == null)
                                {
                                    movNomConcepAjustePorRedondeo.resultado = 0.0;
                                    movNomConcepAjustePorRedondeo.calculado = movNomConcepAjustePorRedondeo.resultado;
                                }
                                if (movNomConcepAjustePorRedondeo.resultado == 0)
                                {
                                    if (movNomConcepAjustePorRedondeo.id > 0)
                                    {
                                        eliminarMovimientosNominaBasura(new object[] { movNomConcepAjustePorRedondeo.id });
                                        //metodosParaMovimientosNomina.eliminarMovimientosNominaBasura(new object[] { movNomConcepAjustePorRedondeo.id }, dbContextAdapterSimple);
                                        //dbContextSimple.SaveChanges();
                                        getSession().Flush();
                                        getSession().Clear();
                                    }
                                }
                                else
                                {
                                    getSession().SaveOrUpdate(movNomConcepAjustePorRedondeo);
                                    cantidadSaveUpdate++;
                                }
                            }


                            if (continuarProcesos)
                            {
                                double importeNeto = (Convert.ToDouble(percepcion.ToString())) - (Convert.ToDouble(deduccion.ToString()));
                                importeNeto = ejecutaDescuentosPrestamos(filtroPlazasPorEmpleadosMov[i], filtroMovimientosNominasCreditosAhorro, importeNeto, listMovNomConcepCreditosAhorroDescuentoActivo);
                                for (int k = 0; k < formulaDedudCreditos.Count(); k++)
                                {
                                    double res = ejecutaFormula(formulaDedudCreditos[k].concepNomDefi.formulaConcepto);
                                    formulaDedudCreditos[k].resultado = res;

                                    getSession().SaveOrUpdate(formulaDedudCreditos[k]);
                                }
                                if (mensajeResultado.noError != 0)
                                {
                                    break;
                                }
                                if (filtroMovimientosNominasCreditosAhorro.Count() > 0)
                                {
                                    ejecutaDescuentosAhorro(filtroPlazasPorEmpleadosMov[i], filtroMovimientosNominasCreditosAhorro, importeNeto, listMovNomConcepCreditosAhorroDescuentoActivo);
                                    for (int k = 0; k < formulaDedudAhorros.Count(); k++)
                                    {
                                        double res = ejecutaFormula(formulaDedudAhorros[k].concepNomDefi.formulaConcepto);
                                        formulaDedudAhorros[k].resultado = res;

                                        getSession().SaveOrUpdate(formulaDedudAhorros[k]);
                                    }
                                }
                                if (mensajeResultado.noError != 0)
                                {
                                    break;
                                }
                                if (cantidadSaveUpdate % cantidadFlush == 0 & cantidadSaveUpdate > 0)
                                {

                                    getSession().Flush();
                                    getSession().Clear();

                                }
                            }



                        }

                        valoresConceptosEmpleados.Clear();
                        filtroMovimientosNominas = null;
                    }
                    #endregion
                    if (mensajeResultado.noError != 0)
                    {
                        getSession().Transaction.Rollback();
                        return mensajeResultado;
                    }

                    if (filtroPlazasPorEmpleadosMov.Count() == 0)
                    {
                        mensajeResultado.noError = 999;
                        mensajeResultado.error = "No existen empleados";
                    }
                    // tiempo.Stop();
                }
                else
                {
                    mensajeResultado.noError = 999;
                    mensajeResultado.error = "No existen empleados";
                }

                if (mensajeResultado.noError == 0)
                {
                    mensajeResultado.resultado = true;
                    getSession().Transaction.Commit();
                }

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("calculaNomina()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Transaction.Rollback();
                band = false;

            }
            finally
            {
                if (band & mensajeResultado.noError != 0)
                {
                    if (getSession() != null)
                    {
                        if (getSession().IsOpen)
                        {
                            if (getSession().Transaction != null)
                            {
                                if (getSession().Transaction.IsActive)
                                {
                                    getSession().Transaction.Rollback();
                                }
                            }
                        }
                    }
                }

            }
            tiempo.Stop();
            return mensajeResultado;
        }

        private void ejecutaDescuentosAhorro(PlazasPorEmpleadosMov plazasPorEmpleadosMov, List<MovNomConcep> filtroMovimientosNominas, Double importeNeto, List<MovNomConcep> listMovNomConcepCreditosAhorroDescuentoActivo)
        {
            try
            {
                int i, j;
                Double importeAcumulado = 0.0;
                bool tieneMovOtrasCorridas = false;
                List<CreditoPorEmpleado> listCreditoPorEmpleado = obtenerCreditoPorEmpleado(plazasPorEmpleadosMov, "2", filtroMovimientosNominas);
                int[] diasMeses = new int[2];
                DateTime fechaBimestre = DateTime.Now;
                fechaBimestre = periodosNomina.acumularAMes.GetValueOrDefault();
                diasMeses[0] = DateTime.DaysInMonth(fechaBimestre.Year, fechaBimestre.Month);
                if ((fechaBimestre.Month + 1) % 2 == 0)
                {
                    fechaBimestre.AddMonths(fechaBimestre.Month - 1);
                }
                else
                {
                    fechaBimestre.AddMonths(fechaBimestre.Month + 1);
                }
                diasMeses[1] = DateTime.DaysInMonth(fechaBimestre.Year, fechaBimestre.Month);
                double diasMes, diasBimestre;
                diasBimestre = Convert.ToDouble((diasMeses[0] + diasMeses[1]));
                if (mensajeResultado.noError == 0)
                {
                    for (i = 0; i < listCreditoPorEmpleado.Count(); i++)
                    {
                        CreditoMovimientos creditoMovimientosCambioDescuento = null, creditoMovimientosBloqueo = null;
                        Boolean continuarEjecucion = true;
                        if (listCreditoPorEmpleado[i].creditoAhorro.inicioDescuento)
                        {
                            if (listCreditoPorEmpleado[i].periodosNomina.fechaInicial.GetValueOrDefault().CompareTo(periodosNomina.fechaInicial) > 0)
                            {
                                continuarEjecucion = false;
                            }
                        }
                        else if (listCreditoPorEmpleado[i].fechaCredito.CompareTo(periodosNomina.fechaFinal) > 0)
                        {
                            continuarEjecucion = false;
                        }
                        creditoMovimientosBloqueo = obtenerCreditoMovimientosMax(listCreditoPorEmpleado[i], TiposMovimiento.Bloqueo);
                        if (mensajeResultado.noError == 0)
                        {
                            if (creditoMovimientosBloqueo != null)
                            {
                                Double cantidad = obtenerCantidadPeriodoNominaRango(plazasPorEmpleadosMov.tipoNomina, creditoMovimientosBloqueo.periodosNomina, periodosNomina);
                                if (cantidad <= creditoMovimientosBloqueo.numeroPeriodosBloquear)
                                {
                                    continuarEjecucion = false;
                                }
                            }
                            if (continuarEjecucion)
                            {
                                Double montoDescuento, montoDescuentoOriginal = listCreditoPorEmpleado[i].montoDescuento;
                                creditoMovimientosCambioDescuento = obtenerCreditoMovimientosMax(listCreditoPorEmpleado[i], TiposMovimiento.ModificarDescuento);
                                if (mensajeResultado.noError == 0 && continuarEjecucion)
                                {
                                    if (creditoMovimientosCambioDescuento != null)
                                    {
                                        montoDescuentoOriginal = creditoMovimientosCambioDescuento.importe.GetValueOrDefault();
                                    }
                                    if (montoDescuentoOriginal > 0)
                                    {
                                        CreditoMovimientos creditoMovimientosDescuentoSistema = null;
                                        creditoMovimientosDescuentoSistema = obtenerCreditoMovimientosPorPeriodoNomina(listCreditoPorEmpleado[i], TiposMovimiento.AbonoSistema);
                                        if (listCreditoPorEmpleado[i].creditoAhorro.modoDescuento == 2)
                                        {//Especificar Número de Parcialidades
                                            if (!obtenerNumeroParcialidadesCreditoMovimientos(listCreditoPorEmpleado[i], TiposMovimiento.AbonoSistema, creditoMovimientosDescuentoSistema))
                                            {
                                                continuarEjecucion = false;
                                            }
                                        }
                                        if (continuarEjecucion)
                                        {
                                            if (creditoMovimientosDescuentoSistema == null)
                                            {
                                                creditoMovimientosDescuentoSistema = crearCreditoMovimientoSistema(listCreditoPorEmpleado[i], false, filtroMovimientosNominas, null);
                                            }
                                            else
                                            {
                                                bool addMovimiento = true;
                                                if (creditoMovimientosDescuentoSistema.movNomConcep == null ? false : creditoMovimientosDescuentoSistema.movNomConcep.Count() > 0)
                                                {
                                                    int recorre;
                                                    for (recorre = 0; recorre < creditoMovimientosDescuentoSistema.movNomConcep.Count(); recorre++)
                                                    {
                                                        if (tipoCorrida.clave.Equals(creditoMovimientosDescuentoSistema.movNomConcep[recorre].tipoCorrida.clave))
                                                        {
                                                            addMovimiento = false;
                                                        }
                                                    }
                                                    recorre = 0;
                                                    while (recorre < creditoMovimientosDescuentoSistema.movNomConcep.Count())
                                                    {
                                                        if (tipoCorrida.clave.Equals(creditoMovimientosDescuentoSistema.movNomConcep[recorre].tipoCorrida.clave))
                                                        {
                                                            recorre++;
                                                        }
                                                        else
                                                        {
                                                            creditoMovimientosDescuentoSistema.movNomConcep.RemoveAt(recorre);
                                                            tieneMovOtrasCorridas = true;
                                                        }
                                                    }
                                                }
                                                if (creditoMovimientosDescuentoSistema.movNomConcep == null ? true : creditoMovimientosDescuentoSistema.movNomConcep.Count() == 0)
                                                {
                                                    if (addMovimiento)
                                                    {
                                                        creditoMovimientosDescuentoSistema = crearCreditoMovimientoSistema(listCreditoPorEmpleado[i], true, filtroMovimientosNominas, creditoMovimientosDescuentoSistema);
                                                    }
                                                    else if (isMov2Meses)
                                                    {
                                                        creditoMovimientosDescuentoSistema = crearCreditoMovimientoSistema(listCreditoPorEmpleado[i], true, filtroMovimientosNominas, creditoMovimientosDescuentoSistema);
                                                    }
                                                }

                                            }
                                            #region Aqui te separa el movimiento del concepto del manejo del descuento
                                            if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.activarManejoDescuento)
                                            {
                                                if (!string.Equals(creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.concepNomDefi_concepNomiDefin_ID.clave, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.concepNomDefi_cNDescuento_ID.clave, StringComparison.OrdinalIgnoreCase))
                                                {
                                                    int k = 0;
                                                    while (k < creditoMovimientosDescuentoSistema.movNomConcep.Count())
                                                    {
                                                        if (string.Equals(creditoMovimientosDescuentoSistema.movNomConcep[k].concepNomDefi.clave, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.concepNomDefi_cNDescuento_ID.clave, StringComparison.OrdinalIgnoreCase))
                                                        {
                                                            listMovNomConcepCreditosAhorroDescuentoActivo.Add(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                            creditoMovimientosDescuentoSistema.movNomConcep.RemoveAt(k);
                                                        }
                                                        else
                                                        {
                                                            k++;
                                                        }
                                                    }
                                                }
                                            }
                                            #endregion
                                            DateTime cFechax = DateTime.Now;
                                            cFechax = periodosNomina.acumularAMes.GetValueOrDefault();
                                            diasMes = creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.factorProporcional == 1 ? factorMensual.GetValueOrDefault() : DateTime.DaysInMonth(cFechax.Year, cFechax.Month);
                                            List<MovNomConcep> listMovNomConcepCreditosAhorroDescuentoParaGuardar = new List<MovNomConcep>();
                                            importeAcumulado = 0.0;
                                            for (int k = 0; k < creditoMovimientosDescuentoSistema.movNomConcep.Count(); k++)
                                            {
                                                Double importe = Convert.ToDouble(montoDescuentoOriginal.ToString()), importeSinMascara;
                                                montoDescuento = montoDescuentoOriginal;
                                                int diasPeriodo = 0;
                                                if (isMov2Meses)
                                                {
                                                    diasPeriodo = (cantidadDiasEntreDosFechas(periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault()) + 1); //+1 para contar el dia actual//obtener dias reales del periodo;
                                                    DateTime cFecha = DateTime.Now;
                                                    if (creditoMovimientosDescuentoSistema.movNomConcep[k].numMovParticion == 1)
                                                    {
                                                        cFecha = creditoMovimientosCambioDescuento.movNomConcep[k].periodosNomina.fechaInicial.GetValueOrDefault();
                                                        cFecha.AddDays(DateTime.DaysInMonth(cFecha.Year, cFecha.Month));
                                                        inicializaPeriodo2Meses(periodosNomina, periodosNomina.fechaInicial.GetValueOrDefault(), cFecha);
                                                        valoresConceptosGlobales.Add(parametroFechaFinal, cFecha);
                                                        valoresConceptosGlobales.Add(parametroFechaInicial, periodosNomina.fechaInicial);
                                                    }
                                                    else
                                                    {
                                                        cFecha = creditoMovimientosCambioDescuento.movNomConcep[k].periodosNomina.fechaFinal.GetValueOrDefault();
                                                        cFecha = new DateTime(cFecha.Year, cFecha.Month, 1);
                                                        inicializaPeriodo2Meses(periodosNomina, cFecha, periodosNomina.fechaFinal.GetValueOrDefault());
                                                        valoresConceptosGlobales.Add(parametroFechaInicial, cFecha);
                                                        valoresConceptosGlobales.Add(parametroFechaFinal, periodosNomina.fechaFinal);
                                                    }
                                                    foreach (var a in valoresConceptosGlobales)
                                                    {
                                                        valoresConceptosEmpleados.Add(a.Key, a.Value);
                                                    }

                                                    cargaValoresDiasCotizados(plazasPorEmpleadosMov.fechaIMSS.GetValueOrDefault(), plazasPorEmpleadosMov.plazasPorEmpleado.fechaFinal.GetValueOrDefault(), true, null, null, false, false);//JSA30
                                                    cargaDatosVariableConfiguracionIMSS(((DateTime)valoresConceptosGlobales[parametroFechaFinal]));
                                                }
                                                Double diasIMSS = Convert.ToDouble(valoresConceptosEmpleados["DiasCotizados".ToUpper()].ToString());
                                                if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoDescuento == 1)
                                                {//Elegir Modo al Registrar el Credito
                                                    Double montoSDI = Convert.ToDouble(valoresConceptosEmpleados["SueldoIntIMSS".ToUpper()].ToString());
                                                    Double SMDF = Convert.ToDouble(valoresConceptosEmpleados["SalarioMinDF".ToUpper()].ToString());
                                                    //0.-Importe, 1.-VSM, 2.-Porcentaje
                                                    if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.modoDescuentoCredito == 1)
                                                    {//VSM
                                                        if (versionCalculoPrestamoAhorro == 4 || versionCalculoPrestamoAhorro == 5)
                                                        {
                                                            //Solo para nominas quincenales
                                                            if (plazasPorEmpleadosMov.tipoNomina.periodicidad.dias != 15)
                                                            {
                                                                versionCalculoPrestamoAhorro = 1;
                                                            }
                                                        }
                                                        switch (versionCalculoPrestamoAhorro)
                                                        {
                                                            case 2:
                                                                //Bimestral, Mensual y Por Periodo
                                                                importe = (SMDF * (montoDescuentoOriginal * 2)) * (diasIMSS / diasBimestre);
                                                                //Original: importe = (SMDF * (montoDescuentoOriginal * 2)) * (diasIMSS / diasBimestre);
                                                                break;
                                                            case 3:
                                                                if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuento.Equals(2) || creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuento.Equals(1))
                                                                {//Bimestral, Mensual
                                                                    importe = (SMDF * montoDescuentoOriginal) * (diasIMSS / diasMes);
                                                                }
                                                                else
                                                                {
                                                                    importe = SMDF * montoDescuentoOriginal;
                                                                }
                                                                //Original: importe = (SMDF * montoDescuentoOriginal) * (diasIMSS / diasMes);
                                                                break;
                                                            case 4:
                                                                if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuento.Equals(2))
                                                                {//Bimestral
                                                                    importe = (SMDF * montoDescuentoOriginal) / 4;
                                                                }
                                                                else if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuento.Equals(1))
                                                                {//Mensual
                                                                    importe = (SMDF * montoDescuentoOriginal) / 2;
                                                                }
                                                                else
                                                                {
                                                                    importe = SMDF * montoDescuentoOriginal;
                                                                }
                                                                //Original: importe = (SMDF * montoDescuentoOriginal) / 2;//mensual
                                                                break;
                                                            case 5:
                                                                if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuento.Equals(2))
                                                                {//Bimestral
                                                                    importe = SMDF * (montoDescuentoOriginal / 4);
                                                                }
                                                                else if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuento.Equals(1))
                                                                {//Mensual
                                                                    importe = SMDF * (montoDescuentoOriginal / 2);
                                                                }
                                                                else
                                                                {
                                                                    importe = SMDF * montoDescuentoOriginal;
                                                                }
                                                                //Original: importe = SMDF * (montoDescuentoOriginal / 2);//mensual
                                                                break;
                                                            default:
                                                                if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuentoVSMG.Equals(0))
                                                                {//Por periodo
                                                                    if (isMov2Meses)
                                                                    {
                                                                        importe = SMDF * ((montoDescuento / diasPeriodo) * diasIMSS);
                                                                    }
                                                                    else
                                                                    {
                                                                        importe = SMDF * montoDescuento;
                                                                    }
                                                                }
                                                                else if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuentoVSMG.Equals(2))
                                                                {//Bimestral
                                                                    montoDescuento = (montoDescuentoOriginal / diasBimestre);
                                                                    importe = (SMDF * montoDescuento) * diasIMSS;//montoDescuento debe ser mensual
                                                                }
                                                                else
                                                                {
                                                                    montoDescuento = (montoDescuentoOriginal / diasMes);
                                                                    importe = (SMDF * montoDescuento) * diasIMSS;//montoDescuento debe ser mensual
                                                                }
                                                                break;
                                                        }
                                                    }
                                                    else if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.modoDescuentoCredito == 2)
                                                    {//Porcentaje
                                                        if (versionCalculoPrestamoAhorro == 3)
                                                        {
                                                            //Solo para nominas quincenales
                                                            if (plazasPorEmpleadosMov.tipoNomina.periodicidad.dias != 15)
                                                            {
                                                                versionCalculoPrestamoAhorro = 1;
                                                            }
                                                        }
                                                        switch (versionCalculoPrestamoAhorro)
                                                        {
                                                            case 2:
                                                                Double factorIntegracion = (Double)valoresConceptosEmpleados["FactorIntegracion".ToUpper()];
                                                                importe = (acumuladoNormal + acumuladoDirecto + acumuladoAnual) * (montoDescuentoOriginal / 100) * factorIntegracion;
                                                                //Original: importe = (acumuladoNormal + acumuladoDirecto + acumuladoAnual) * (montoDescuentoOriginal / 100) * factorIntegracion;
                                                                break;
                                                            case 3:
                                                                //Solo para nominas quincenales
                                                                importe = ((montoSDI * diasBimestre) * (montoDescuentoOriginal / 100)) / 4;
                                                                //Original: importe = ((montoSDI * diasBimestre) * (montoDescuentoOriginal / 100)) / 4;
                                                                break;
                                                            default:
                                                                if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuentoPorc.Equals(1))
                                                                {//Mensual
                                                                    importe = (((montoSDI * diasMes) * (montoDescuentoOriginal / 100)) / diasMes) * diasIMSS;
                                                                }
                                                                else if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuentoPorc.Equals(2))
                                                                {//Bimestral
                                                                    importe = (((montoSDI * diasBimestre) * (montoDescuentoOriginal / 100)) / diasBimestre) * diasIMSS;
                                                                }
                                                                else
                                                                {
                                                                    importe = (montoSDI * (montoDescuentoOriginal / 100)) * diasIMSS;
                                                                }
                                                                break;
                                                        }

                                                    }
                                                    else
                                                    {//Importe
                                                        if (versionCalculoPrestamoAhorro == 3)
                                                        {
                                                            //Solo para nominas quincenales
                                                            if (plazasPorEmpleadosMov.tipoNomina.periodicidad.dias != 15)
                                                            {
                                                                versionCalculoPrestamoAhorro = 1;
                                                            }
                                                        }
                                                        switch (versionCalculoPrestamoAhorro)
                                                        {
                                                            case 2:
                                                                if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuento.Equals(2))
                                                                {//Bimestral
                                                                    importe = (montoDescuentoOriginal / diasBimestre) * diasIMSS;
                                                                }
                                                                else if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuento.Equals(1))
                                                                {//Mensual
                                                                    importe = ((montoDescuentoOriginal * 2) / diasBimestre) * diasIMSS;
                                                                }
                                                                else
                                                                {
                                                                    importe = montoDescuentoOriginal;
                                                                }
                                                                //Original: importe = ((montoDescuentoOriginal * 2) / diasBimestre) * diasIMSS;
                                                                break;
                                                            case 3:
                                                                //Solo para nominas quincenales
                                                                if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuento.Equals(2))
                                                                {//Bimestral
                                                                    importe = montoDescuentoOriginal / 4;
                                                                }
                                                                else if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuento.Equals(1))
                                                                {//Mensual
                                                                    importe = montoDescuentoOriginal / 2;
                                                                }
                                                                else
                                                                {
                                                                    importe = montoDescuentoOriginal;
                                                                }
                                                                //Original: importe = montoDescuentoOriginal / 2;
                                                                break;
                                                            default:
                                                                if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuento.Equals(1))
                                                                {//Mensual
                                                                    importe = (montoDescuentoOriginal / diasMes) * diasIMSS;
                                                                }
                                                                else if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuento.Equals(2))
                                                                {//Bimestral
                                                                    importe = (montoDescuentoOriginal / diasBimestre) * diasIMSS;
                                                                }
                                                                else if (isMov2Meses)
                                                                {
                                                                    importe = (montoDescuentoOriginal / diasPeriodo) * diasIMSS;
                                                                }
                                                                else
                                                                {
                                                                    importe = montoDescuento;
                                                                }
                                                                break;
                                                        }
                                                    }
                                                }
                                                else if (isMov2Meses)
                                                {
                                                    importe = (montoDescuento / diasPeriodo) * diasIMSS;
                                                }
                                                else
                                                {
                                                    importe = montoDescuento;
                                                }
                                                List<Object> listobject = null;
                                                if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.activarManejoDescuento)
                                                {
                                                    listobject = ejercutarManejoDescuento(creditoMovimientosDescuentoSistema, diasIMSS, importe, listMovNomConcepCreditosAhorroDescuentoActivo, diasMes, diasBimestre, creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                    importe = (Double)listobject[0];
                                                    listMovNomConcepCreditosAhorroDescuentoActivo = (List<MovNomConcep>)listobject[1];
                                                    if (listobject[2] != null)
                                                    {
                                                        listMovNomConcepCreditosAhorroDescuentoParaGuardar.Add((MovNomConcep)listobject[2]);
                                                    }
                                                }
                                                importeSinMascara = importe;
                                                importe = aplicarMascara(creditoMovimientosDescuentoSistema.movNomConcep[0].concepNomDefi, importe, false);
                                                importeAcumulado += importe;
                                                if (importe > 0)
                                                {
                                                    if (creditoMovimientosDescuentoSistema.id == 0)
                                                    {
                                                        List<MovNomConcep> values = existeMovimientoNomina(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                        int numero = creditoMovimientosDescuentoSistema.movNomConcep[k].numero.GetValueOrDefault();
                                                        if (values != null)
                                                        {
                                                            for (j = 0; j < values.Count(); j++)
                                                            {
                                                                if (values[j].numero >= numero)
                                                                {
                                                                    numero = values[j].numero.GetValueOrDefault();
                                                                }
                                                            }
                                                            if (values.Count() > 0)
                                                            {
                                                                numero++;
                                                            }
                                                            creditoMovimientosDescuentoSistema.movNomConcep[k].numero = numero;
                                                        }
                                                    }
                                                    creditoMovimientosDescuentoSistema.movNomConcep[k].resultado = importe;
                                                    creditoMovimientosDescuentoSistema.movNomConcep[k].calculado = importeSinMascara;

                                                    creditoMovimientosDescuentoSistema.importe = importeAcumulado;
                                                    creditoMovimientosDescuentoSistema.movNomConcep[k].creditoMovimientos = (creditoMovimientosDescuentoSistema);
                                                    cantidadSaveUpdate++;

                                                    if (creditoMovimientosDescuentoSistema.movNomConcep[k].id == 0)
                                                    {
                                                        getSession().SaveOrUpdate(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                        //dbContextSimple.saveOrUpdate(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                    }
                                                    else
                                                    {
                                                        getSession().SaveOrUpdate(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                        //dbContextSimple.merge(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                    }
                                                    cantidadSaveUpdate++;

                                                }
                                                else
                                                {
                                                    importeAcumulado -= importe;
                                                    if (creditoMovimientosDescuentoSistema.movNomConcep[k].id > 0)
                                                    {
                                                        getSession().Delete(creditoMovimientosDescuentoSistema.movNomConcep[k]);

                                                        // dbContextSimple.delete(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                        creditoMovimientosDescuentoSistema.movNomConcep.RemoveAt(k);
                                                        cantidadSaveUpdate++;
                                                    }

                                                    if (creditoMovimientosDescuentoSistema.id > 0 & !tieneMovOtrasCorridas)
                                                    {
                                                        if (creditoMovimientosDescuentoSistema.movNomConcep.Count() == 0)
                                                        {
                                                            getSession().Delete(creditoMovimientosDescuentoSistema);

                                                            // dbContextSimple.delete(creditoMovimientosDescuentoSistema);
                                                            cantidadSaveUpdate++;
                                                        }
                                                    }
                                                }
                                                if (cantidadSaveUpdate % cantidadFlush == 0 & cantidadSaveUpdate > 0)
                                                {

                                                    getSession().Flush();
                                                    getSession().Clear();

                                                }

                                            }//end for creditoMovimientosDescuentoSistema.movNomConcep.Count()
                                            if (listMovNomConcepCreditosAhorroDescuentoParaGuardar.Count() > 0)
                                            {
                                                creditoMovimientosDescuentoSistema.movNomConcep.AddRange(listMovNomConcepCreditosAhorroDescuentoParaGuardar);
                                                getSession().SaveOrUpdate(creditoMovimientosDescuentoSistema);
                                                //dbContextSimple.saveOrUpdate(creditoMovimientosDescuentoSistema);
                                                for (int k = 0; k < creditoMovimientosDescuentoSistema.movNomConcep.Count(); k++)
                                                {
                                                    creditoMovimientosDescuentoSistema.movNomConcep[k].creditoMovimientos = (creditoMovimientosDescuentoSistema);
                                                    getSession().SaveOrUpdate(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                    //dbContextSimple.saveOrUpdate(creditoMovimientosDescuentoSistema.getMovNomConceps().get(k));

                                                }
                                            }


                                        }
                                    }

                                }

                            }
                            else if (creditoMovimientosBloqueo != null)
                            {
                                CreditoMovimientos creditoMovimientosDescuentoSistema = null;
                                creditoMovimientosDescuentoSistema = obtenerCreditoMovimientosPorPeriodoNomina(listCreditoPorEmpleado[i], TiposMovimiento.AbonoSistema);
                                if (creditoMovimientosDescuentoSistema != null)
                                {
                                    for (int k = 0; k < creditoMovimientosDescuentoSistema.movNomConcep.Count(); k++)
                                    {
                                        getSession().SaveOrUpdate(creditoMovimientosDescuentoSistema.creditoPorEmpleado);

                                        getSession().Delete(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                        //dbContextSimple.saveOrUpdate(creditoMovimientosDescuentoSistema.getCreditoPorEmpleado());
                                        //dbContextSimple.delete(creditoMovimientosDescuentoSistema.getMovNomConceps().get(k));
                                    }
                                    creditoMovimientosDescuentoSistema.movNomConcep.Clear();
                                    if (!tieneMovOtrasCorridas)
                                    {
                                        getSession().Delete(creditoMovimientosDescuentoSistema);
                                        //dbContextSimple.delete(creditoMovimientosDescuentoSistema);
                                    }
                                }
                            }


                        }// end if mensajeResultado.noError == 0 2

                    }//end del for listCreditoPorEmpleado
                    if (listCreditoPorEmpleado.Count() == 0)
                    {
                        List<object> idsMovDelete = new List<object>();
                        for (int k = 0; k < filtroMovimientosNominas.Count(); k++)
                        {
                            if (filtroMovimientosNominas[k].id > 0 & filtroMovimientosNominas[k].creditoMovimientos != null)
                            {
                                if (string.Equals(filtroMovimientosNominas[k].creditoMovimientos.creditoPorEmpleado.creditoAhorro.tipoConfiguracion, "2", StringComparison.OrdinalIgnoreCase))
                                {//ahorro
                                    idsMovDelete.Add(filtroMovimientosNominas[k].id);
                                }
                            }
                        }
                        if (idsMovDelete.Count() > 0)
                        {
                            MovimientosNominaHDAO movimientosNominaDAO = new MovimientosNominaHDAO();
                            //movimientosNominaDAO.setSession(dbContextSimple);
                            movimientosNominaDAO.deleteListQueryMov("MovNomConcep", "id", idsMovDelete.ToArray(), null, null, null, true);//pendiente la conexion
                                                                                                                                          // movimientosNominaDAO.deleteListQueryMov(MovNomConcep.class.getSimpleName(), "id", idsMovDelete.toArray(), null, null, null, true);
                        }
                    }
                }//end if mensajeResultado.noError==0

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ejecutaDescuentosAhorro()1_Error: ").Append(ex));
                mensajeResultado.noError = 27;
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
        }

        private Double ejecutaDescuentosPrestamos(PlazasPorEmpleadosMov plazasPorEmpleadosMov, List<MovNomConcep> filtroMovimientosNominas, Double importeNeto, List<MovNomConcep> listMovNomConcepCreditosAhorroDescuentoActivo)
        {
            int i, j;
            double importeAcumulado = 0.0;
            bool tieneMovOtrasCorridas = false;
            try
            {
                List<CreditoPorEmpleado> listCreditoPorEmpleado = obtenerCreditoPorEmpleado(plazasPorEmpleadosMov, "1", filtroMovimientosNominas);
                int[] diasMeses = new int[2];
                DateTime fechaBimestre = DateTime.Now;
                fechaBimestre = periodosNomina.acumularAMes.GetValueOrDefault();
                diasMeses[0] = DateTime.DaysInMonth(fechaBimestre.Year, fechaBimestre.Month);
                if ((fechaBimestre.Month + 1) % 2 == 0)
                {
                    fechaBimestre.AddMonths(fechaBimestre.Month - 1);
                }
                else
                {
                    fechaBimestre.AddMonths(fechaBimestre.Month + 1);
                }
                diasMeses[1] = DateTime.DaysInMonth(fechaBimestre.Year, fechaBimestre.Month);
                double diasMes, diasBimestre;
                diasBimestre = Convert.ToDouble((diasMeses[0] + diasMeses[1]));
                if (mensajeResultado.noError == 0)
                {
                    for (i = 0; i < listCreditoPorEmpleado.Count(); i++)
                    {
                        // ejecutaDeleteCreditoMovimiento(listCreditoPorEmpleado[i],periodosNomina.fechaFinal.GetValueOrDefault());
                        CreditoMovimientos creditoMovimientosCambioDescuento = null, creditoMovimientosBloqueo = null;
                        bool continuarEjecucion = true;
                        versionCalculoPrestamoAhorro = listCreditoPorEmpleado[i].creditoAhorro.versionCalculoPrestamoAhorro;
                        if (listCreditoPorEmpleado[i].creditoAhorro.inicioDescuento)
                        {
                            if (listCreditoPorEmpleado[i].periodosNomina.fechaInicial.GetValueOrDefault().CompareTo(periodosNomina.fechaInicial.GetValueOrDefault()) > 0)
                            {
                                continuarEjecucion = false;
                            }
                        }
                        else if (listCreditoPorEmpleado[i].fechaCredito.CompareTo(periodosNomina.fechaFinal.GetValueOrDefault()) > 0)
                        {
                            continuarEjecucion = false;
                        }
                        creditoMovimientosBloqueo = obtenerCreditoMovimientosMax(listCreditoPorEmpleado[i], TiposMovimiento.Bloqueo);
                        if (mensajeResultado.noError == 0)
                        {
                            if (creditoMovimientosBloqueo != null)
                            {
                                Double cantidad = obtenerCantidadPeriodoNominaRango(plazasPorEmpleadosMov.tipoNomina, creditoMovimientosBloqueo.periodosNomina, periodosNomina);
                                if (cantidad > 0 && cantidad <= creditoMovimientosBloqueo.numeroPeriodosBloquear)
                                {
                                    continuarEjecucion = false;
                                }
                            }
                            if (continuarEjecucion)
                            {
                                Double montoDescuento, montoDescuentoOriginal = listCreditoPorEmpleado[i].montoDescuento;
                                creditoMovimientosCambioDescuento = obtenerCreditoMovimientosMax(listCreditoPorEmpleado[i], TiposMovimiento.ModificarDescuento);
                                if (mensajeResultado.noError == 0 && continuarEjecucion)
                                {
                                    if (creditoMovimientosCambioDescuento != null)
                                    {
                                        montoDescuentoOriginal = creditoMovimientosCambioDescuento.importe.GetValueOrDefault();
                                    }
                                    if (montoDescuentoOriginal > 0)
                                    {
                                        CreditoMovimientos creditoMovimientosDescuentoSistema = null;
                                        creditoMovimientosDescuentoSistema = obtenerCreditoMovimientosPorPeriodoNomina(listCreditoPorEmpleado[i], TiposMovimiento.AbonoSistema);
                                        if (listCreditoPorEmpleado[i].creditoAhorro.modoDescuento == 2)
                                        {//Especificar Número de Parcialidades
                                            if (!obtenerNumeroParcialidadesCreditoMovimientos(listCreditoPorEmpleado[i], TiposMovimiento.AbonoSistema, creditoMovimientosDescuentoSistema))
                                            {
                                                continuarEjecucion = false;
                                            }

                                        }
                                        if (continuarEjecucion)
                                        {
                                            if (creditoMovimientosDescuentoSistema == null)
                                            {
                                                creditoMovimientosDescuentoSistema = crearCreditoMovimientoSistema(listCreditoPorEmpleado[i], false, filtroMovimientosNominas, null);
                                            }
                                            else
                                            {

                                                creditoMovimientosDescuentoSistema.creditoPorEmpleado.saldo = creditoMovimientosDescuentoSistema.creditoPorEmpleado.saldo + creditoMovimientosDescuentoSistema.importe.GetValueOrDefault();
                                                bool addMovimiento = true;
                                                if (creditoMovimientosDescuentoSistema.movNomConcep == null ? false : creditoMovimientosDescuentoSistema.movNomConcep.Count > 0)
                                                {
                                                    int recorre;
                                                    for (recorre = 0; recorre < creditoMovimientosDescuentoSistema.movNomConcep.Count(); recorre++)
                                                    {
                                                        if (tipoCorrida.clave.Equals(creditoMovimientosDescuentoSistema.movNomConcep[recorre].tipoCorrida.clave))
                                                        {
                                                            addMovimiento = false;
                                                        }
                                                    }
                                                    recorre = 0;
                                                    while (recorre < creditoMovimientosDescuentoSistema.movNomConcep.Count())
                                                    {
                                                        if (tipoCorrida.clave.Equals(creditoMovimientosDescuentoSistema.movNomConcep[recorre].tipoCorrida.clave))
                                                        {
                                                            recorre++;
                                                        }
                                                        else
                                                        {
                                                            creditoMovimientosDescuentoSistema.movNomConcep.RemoveAt(recorre);
                                                            tieneMovOtrasCorridas = true;
                                                        }
                                                    }
                                                }
                                                if (creditoMovimientosDescuentoSistema.movNomConcep == null ? true : creditoMovimientosDescuentoSistema.movNomConcep.Count() == 0)
                                                {
                                                    if (addMovimiento)
                                                    {
                                                        creditoMovimientosDescuentoSistema = crearCreditoMovimientoSistema(listCreditoPorEmpleado[i], true, filtroMovimientosNominas, creditoMovimientosDescuentoSistema);
                                                    }
                                                    else if (isMov2Meses)
                                                    {
                                                        creditoMovimientosDescuentoSistema = crearCreditoMovimientoSistema(listCreditoPorEmpleado[i], true, filtroMovimientosNominas, creditoMovimientosDescuentoSistema);
                                                    }
                                                }
                                            }
                                            #region Aqui te separa el movimiento del concepto del manejo del descuento
                                            //Aqui te separa el movimiento del concepto del manejo del descuento.
                                            if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.activarManejoDescuento)
                                            {
                                                if (!string.Equals(creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.concepNomDefi_concepNomiDefin_ID.clave, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.concepNomDefi_cNDescuento_ID.clave, StringComparison.OrdinalIgnoreCase))
                                                {
                                                    int k = 0;
                                                    while (k < creditoMovimientosDescuentoSistema.movNomConcep.Count())
                                                    {
                                                        if (string.Equals(creditoMovimientosDescuentoSistema.movNomConcep[k].concepNomDefi.clave, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.concepNomDefi_cNDescuento_ID.clave, StringComparison.OrdinalIgnoreCase))
                                                        {
                                                            listMovNomConcepCreditosAhorroDescuentoActivo.Add(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                            creditoMovimientosDescuentoSistema.movNomConcep.RemoveAt(k);
                                                        }
                                                        else
                                                        {
                                                            k++;
                                                        }
                                                    }
                                                }
                                            }
                                            #endregion
                                            DateTime cFechax = DateTime.Now;
                                            cFechax = periodosNomina.acumularAMes.GetValueOrDefault();
                                            diasMes = creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.factorProporcional == 1 ? factorMensual.GetValueOrDefault() : DateTime.DaysInMonth(cFechax.Year, cFechax.Month);
                                            List<MovNomConcep> listMovNomConcepCreditosAhorroDescuentoParaGuardar = new List<MovNomConcep>();
                                            importeAcumulado = 0.0;
                                            for (int k = 0; k < creditoMovimientosDescuentoSistema.movNomConcep.Count(); k++)
                                            {
                                                Double importe = Convert.ToDouble(montoDescuentoOriginal.ToString()), importeSinMascara;
                                                montoDescuento = montoDescuentoOriginal;
                                                int diasPeriodo = 0;
                                                if (isMov2Meses)
                                                {
                                                    diasPeriodo = (cantidadDiasEntreDosFechas(periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault()));
                                                    DateTime cFecha = DateTime.Now;
                                                    if (creditoMovimientosDescuentoSistema.movNomConcep[k].numMovParticion == 1)
                                                    {
                                                        cFecha = creditoMovimientosDescuentoSistema.movNomConcep[k].periodosNomina.fechaInicial.GetValueOrDefault();
                                                        cFecha.AddDays(DateTime.DaysInMonth(cFecha.Year, cFecha.Month));
                                                        inicializaPeriodo2Meses(periodosNomina, periodosNomina.fechaInicial.GetValueOrDefault(), cFecha);
                                                        valoresConceptosGlobales.Add(parametroFechaFinal, cFecha);
                                                        valoresConceptosGlobales.Add(parametroFechaInicial, periodosNomina.fechaInicial.GetValueOrDefault());
                                                    }
                                                    else
                                                    {
                                                        cFecha = creditoMovimientosDescuentoSistema.movNomConcep[k].periodosNomina.fechaFinal.GetValueOrDefault();
                                                        cFecha = new DateTime(cFecha.Year, cFecha.Month, 1);
                                                        inicializaPeriodo2Meses(periodosNomina, cFecha, periodosNomina.fechaFinal.GetValueOrDefault());
                                                        valoresConceptosGlobales.Add(parametroFechaFinal, cFecha);
                                                        valoresConceptosGlobales.Add(parametroFechaInicial, periodosNomina.fechaInicial.GetValueOrDefault());
                                                    }

                                                    foreach (var a in valoresConceptosGlobales)
                                                    {
                                                        valoresConceptosEmpleados.Add(a.Key, a.Value);
                                                    }
                                                    cargaValoresDiasCotizados(plazasPorEmpleadosMov.fechaIMSS.GetValueOrDefault(), plazasPorEmpleadosMov.plazasPorEmpleado.fechaFinal.GetValueOrDefault(), true, null, null, false, false);//JSA30
                                                    cargaDatosVariableConfiguracionIMSS(((DateTime)valoresConceptosGlobales[parametroFechaFinal]));
                                                }
                                                Double diasIMSS = Convert.ToDouble(valoresConceptosEmpleados["DiasCotizados".ToUpper()].ToString());
                                                if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoDescuento == 1)
                                                {//Elegir Modo al Registrar el Credito
                                                    Double montoSDI = Convert.ToDouble(valoresConceptosEmpleados["SueldoIntIMSS".ToUpper()].ToString());
                                                    Double SMDF = Convert.ToDouble(valoresConceptosEmpleados["SalarioMinDF".ToUpper()].ToString());
                                                    //0.-Importe, 1.-VSM, 2.-Porcentaje

                                                    if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.modoDescuentoCredito == 1)
                                                    {//VSM
                                                        if (versionCalculoPrestamoAhorro == 4 || versionCalculoPrestamoAhorro == 5)
                                                        {
                                                            //Solo para nominas quincenales
                                                            if (plazasPorEmpleadosMov.tipoNomina.periodicidad.dias != 15)
                                                            {
                                                                versionCalculoPrestamoAhorro = 1;
                                                            }
                                                        }
                                                        switch (versionCalculoPrestamoAhorro)
                                                        {
                                                            case 2:
                                                                //Bimestral, Mensual y Por Periodo
                                                                importe = (SMDF * (montoDescuentoOriginal * 2)) * (diasIMSS / diasBimestre);
                                                                //Original: importe = (SMDF * (montoDescuentoOriginal * 2)) * (diasIMSS / diasBimestre);
                                                                break;
                                                            case 3:
                                                                if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuento.Equals(2)
                                                                        || creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuento.Equals(1))
                                                                {//Bimestral y mensual
                                                                    importe = (SMDF * montoDescuentoOriginal) * (diasIMSS / diasMes);
                                                                }
                                                                else
                                                                {
                                                                    importe = SMDF * montoDescuentoOriginal;
                                                                }
                                                                //Original: importe = (SMDF * montoDescuentoOriginal) * (diasIMSS / diasMes);
                                                                break;
                                                            case 4:
                                                                //Solo para nominas quincenales
                                                                if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuento.Equals(2))
                                                                {//Bimestral
                                                                    importe = (SMDF * montoDescuentoOriginal) / 4;
                                                                }
                                                                else if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuento.Equals(1))
                                                                {//Mensual
                                                                    importe = (SMDF * montoDescuentoOriginal) / 2;
                                                                }
                                                                else
                                                                {
                                                                    importe = SMDF * montoDescuentoOriginal;
                                                                }
                                                                //Original: importe = (SMDF * montoDescuentoOriginal) / 2;//mensual
                                                                break;
                                                            case 5:
                                                                //Solo para nominas quincenales
                                                                if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuento.Equals(2))
                                                                {//Bimestral
                                                                    importe = SMDF * (montoDescuentoOriginal / 4);
                                                                }
                                                                else if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuento.Equals(1))
                                                                {//Mensual
                                                                    importe = SMDF * (montoDescuentoOriginal / 2);
                                                                }
                                                                else
                                                                {
                                                                    importe = SMDF * montoDescuentoOriginal;
                                                                }
                                                                //Original: importe = SMDF * (montoDescuentoOriginal / 2);//mensual
                                                                break;
                                                            default:
                                                                if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuentoVSMG.Equals(0))
                                                                {//Por periodo
                                                                    if (isMov2Meses)
                                                                    {
                                                                        importe = SMDF * ((montoDescuento / diasPeriodo) * diasIMSS);
                                                                    }
                                                                    else
                                                                    {
                                                                        importe = SMDF * montoDescuento;
                                                                    }
                                                                }
                                                                else if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuentoVSMG.Equals(2))
                                                                {//Bimestral
                                                                    montoDescuento = (montoDescuentoOriginal / diasBimestre);
                                                                    importe = (SMDF * montoDescuento) * diasIMSS;//montoDescuento debe ser mensual
                                                                }
                                                                else
                                                                {
                                                                    montoDescuento = (montoDescuentoOriginal / diasMes);
                                                                    importe = (SMDF * montoDescuento) * diasIMSS;//montoDescuento debe ser mensual
                                                                }
                                                                break;
                                                        }
                                                    }
                                                    else if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.modoDescuentoCredito == 2)
                                                    {//Porcentaje
                                                        if (versionCalculoPrestamoAhorro == 3)
                                                        {
                                                            //Solo para nominas quincenales
                                                            if (plazasPorEmpleadosMov.tipoNomina.periodicidad.dias != 15)
                                                            {
                                                                versionCalculoPrestamoAhorro = 1;
                                                            }
                                                        }
                                                        switch (versionCalculoPrestamoAhorro)
                                                        {
                                                            case 2:
                                                                Double factorIntegracion = (Double)valoresConceptosEmpleados["FactorIntegracion".ToUpper()];
                                                                importe = (acumuladoNormal + acumuladoDirecto + acumuladoAnual) * (montoDescuentoOriginal / 100) * factorIntegracion;
                                                                //Original: importe = (acumuladoNormal + acumuladoDirecto + acumuladoAnual) * (montoDescuentoOriginal / 100) * factorIntegracion;
                                                                break;
                                                            case 3:
                                                                //Solo para nominas quincenales
                                                                importe = ((montoSDI * diasBimestre) * (montoDescuentoOriginal / 100)) / 4;
                                                                //Original: importe = ((montoSDI * diasBimestre) * (montoDescuentoOriginal / 100)) / 4;
                                                                break;
                                                            default:
                                                                if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuentoPorc.Equals(1))
                                                                {//Mensual
                                                                    importe = (((montoSDI * diasMes) * (montoDescuentoOriginal / 100)) / diasMes) * diasIMSS;
                                                                }
                                                                else if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuentoPorc.Equals(2))
                                                                {//Bimestral
                                                                    importe = (((montoSDI * diasBimestre) * (montoDescuentoOriginal / 100)) / diasBimestre) * diasIMSS;
                                                                }
                                                                else
                                                                {
                                                                    importe = (montoSDI * (montoDescuentoOriginal / 100)) * diasIMSS;
                                                                }
                                                                break;
                                                        }
                                                        //                                                    System.out.println("montoSDI " + montoSDI + " montoDescuento " + montoDescuento + " diasIMSS " + diasIMSS);
                                                    }
                                                    else
                                                    {//Importe
                                                        if (versionCalculoPrestamoAhorro == 3)
                                                        {
                                                            //Solo para nominas quincenales
                                                            if (plazasPorEmpleadosMov.tipoNomina.periodicidad.dias != 15)
                                                            {
                                                                versionCalculoPrestamoAhorro = 1;
                                                            }
                                                        }
                                                        switch (versionCalculoPrestamoAhorro)
                                                        {
                                                            case 2:
                                                                if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuento.Equals(2))
                                                                {//Bimestral
                                                                    importe = (montoDescuentoOriginal / diasBimestre) * diasIMSS;
                                                                }
                                                                else if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuento.Equals(1))
                                                                {//Mensual
                                                                    importe = ((montoDescuentoOriginal * 2) / diasBimestre) * diasIMSS;
                                                                }
                                                                else
                                                                {
                                                                    importe = montoDescuentoOriginal;
                                                                }
                                                                //Original: importe = ((montoDescuentoOriginal * 2) / diasBimestre) * diasIMSS;
                                                                break;
                                                            case 3:
                                                                //Solo para nominas quincenales
                                                                if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuento.Equals(2))
                                                                {//Bimestral
                                                                    importe = montoDescuentoOriginal / 4;
                                                                }
                                                                else if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuento.Equals(1))
                                                                {//Mensual
                                                                    importe = montoDescuentoOriginal / 2;
                                                                }
                                                                else
                                                                {
                                                                    importe = montoDescuentoOriginal;
                                                                }
                                                                //Original: importe = montoDescuentoOriginal / 2;
                                                                break;
                                                            default:
                                                                if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuento.Equals(1))
                                                                {//Mensual
                                                                    importe = (montoDescuentoOriginal / diasMes) * diasIMSS;
                                                                }
                                                                else if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoCapturaDescuento.Equals(2))
                                                                {//Bimestral
                                                                    importe = (montoDescuentoOriginal / diasBimestre) * diasIMSS;
                                                                }
                                                                else if (isMov2Meses)
                                                                {
                                                                    importe = (montoDescuentoOriginal / diasPeriodo) * diasIMSS;
                                                                }
                                                                else
                                                                {
                                                                    importe = montoDescuento;
                                                                }
                                                                break;
                                                        }
                                                    }
                                                }
                                                else if (isMov2Meses)
                                                {
                                                    importe = (montoDescuento / diasPeriodo) * diasIMSS;
                                                }
                                                else
                                                {
                                                    importe = montoDescuento;
                                                }
                                                List<Object> listobject = null;
                                                if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.activarManejoDescuento)
                                                {
                                                    listobject = ejercutarManejoDescuento(creditoMovimientosDescuentoSistema, diasIMSS, importe, listMovNomConcepCreditosAhorroDescuentoActivo, diasMes, diasBimestre, creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                    importe = (Double)listobject[0];
                                                    listMovNomConcepCreditosAhorroDescuentoActivo = (List<MovNomConcep>)listobject[1];
                                                    if (listobject[2] != null)
                                                    {
                                                        listMovNomConcepCreditosAhorroDescuentoParaGuardar.Add((MovNomConcep)listobject[2]);
                                                    }
                                                }//aqui voy

                                                importeSinMascara = importe;
                                                importe = aplicarMascara(creditoMovimientosDescuentoSistema.movNomConcep[0].concepNomDefi, importe, false);
                                                if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.totalCredito > 0)
                                                {
                                                    if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.saldo - importe < 0)
                                                    {
                                                        importe = importe + (creditoMovimientosDescuentoSistema.creditoPorEmpleado.saldo - importe);
                                                    }
                                                }

                                                importeAcumulado += importe;
                                                if (importe > 0)
                                                {
                                                    if (creditoMovimientosDescuentoSistema.id == 0)
                                                    {
                                                        List<MovNomConcep> values = existeMovimientoNomina(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                        int numero = creditoMovimientosDescuentoSistema.movNomConcep[k].numero.GetValueOrDefault();
                                                        if (values != null)
                                                        {
                                                            for (j = 0; j < values.Count(); j++)
                                                            {
                                                                if (values[j].numero >= numero)
                                                                {
                                                                    numero = values[j].numero.GetValueOrDefault();
                                                                }
                                                            }
                                                            if (values.Count() > 0)
                                                            {
                                                                numero++;
                                                            }
                                                            creditoMovimientosDescuentoSistema.movNomConcep[k].numero = numero;
                                                        }
                                                    }
                                                    creditoMovimientosDescuentoSistema.movNomConcep[k].resultado = importe;
                                                    creditoMovimientosDescuentoSistema.movNomConcep[k].calculado = importeSinMascara;

                                                    creditoMovimientosDescuentoSistema.importe = (importeAcumulado);

                                                    cantidadSaveUpdate++;
                                                    creditoMovimientosDescuentoSistema.movNomConcep[k].creditoMovimientos = (creditoMovimientosDescuentoSistema);

                                                    if (creditoMovimientosDescuentoSistema.movNomConcep[k].id == 0)
                                                    {
                                                        //dbContextSimple.Set<MovNomConcep>().AddOrUpdate(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                        getSession().SaveOrUpdate(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                    }
                                                    else
                                                    {
                                                        getSession().Merge(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                        //dbContextSimple.merge(creditoMovimientosDescuentoSistema.getMovNomConceps().get(k));
                                                    }
                                                    cantidadSaveUpdate++;
                                                }
                                                else
                                                {
                                                    importeAcumulado -= importe;
                                                    if (creditoMovimientosDescuentoSistema.movNomConcep[k].id > 0)
                                                    {
                                                        getSession().Delete(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                        // dbContextSimple.Set<MovNomConcep>().Remove(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                        //dbContextSimple.delete(creditoMovimientosDescuentoSistema.getMovNomConceps().get(k));
                                                        creditoMovimientosDescuentoSistema.movNomConcep.RemoveAt(k);
                                                        cantidadSaveUpdate++;
                                                    }

                                                    if (creditoMovimientosDescuentoSistema.id > 0 & !tieneMovOtrasCorridas)
                                                    {
                                                        if (creditoMovimientosDescuentoSistema.movNomConcep.Count() == 0)
                                                        {
                                                            getSession().Delete(creditoMovimientosDescuentoSistema);
                                                            //dbContextSimple.Set<CreditoMovimientos>().Remove(creditoMovimientosDescuentoSistema);
                                                            // dbContextSimple.delete(creditoMovimientosDescuentoSistema);
                                                            cantidadSaveUpdate++;
                                                        }
                                                    }
                                                }

                                                if (cantidadSaveUpdate % cantidadFlush == 0 & cantidadSaveUpdate > 0)
                                                {
                                                    getSession().Flush();
                                                    getSession().Clear();
                                                }
                                            }//end for creditoMovimientosDescuentoSistema.movNomConcep
                                            if (creditoMovimientosDescuentoSistema != null)
                                            {
                                                if (creditoMovimientosDescuentoSistema.id > 0)
                                                {
                                                    ////dbContextSimple.refresh(creditoMovimientosDescuentoSistema);//JEVC02
                                                }
                                            }
                                            if (listMovNomConcepCreditosAhorroDescuentoParaGuardar.Count() > 0)
                                            {
                                                creditoMovimientosDescuentoSistema.movNomConcep.AddRange(listMovNomConcepCreditosAhorroDescuentoParaGuardar);
                                                getSession().SaveOrUpdate(creditoMovimientosDescuentoSistema);
                                                //dbContextSimple.saveOrUpdate(creditoMovimientosDescuentoSistema);
                                                for (int k = 0; k < creditoMovimientosDescuentoSistema.movNomConcep.Count(); k++)
                                                {
                                                    creditoMovimientosDescuentoSistema.movNomConcep[k].creditoMovimientos = (creditoMovimientosDescuentoSistema);
                                                    getSession().SaveOrUpdate(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                    //dbContextSimple.saveOrUpdate(creditoMovimientosDescuentoSistema.getMovNomConceps().get(k));

                                                }
                                            }
                                        }
                                    }
                                }//end if de mensajeResultado.noError y continuarEjecucion

                            }//end if de continuarEjecucion
                            else if (creditoMovimientosBloqueo != null)
                            {
                                CreditoMovimientos creditoMovimientosDescuentoSistema = null;
                                creditoMovimientosDescuentoSistema = obtenerCreditoMovimientosPorPeriodoNomina(listCreditoPorEmpleado[i], TiposMovimiento.AbonoSistema);
                                if (creditoMovimientosDescuentoSistema != null)
                                {
                                    for (int k = 0; k < creditoMovimientosDescuentoSistema.movNomConcep.Count(); k++)
                                    {
                                        //                                        creditoMovimientosDescuentoSistema.getCreditoPorEmpleado().setSaldo((creditoMovimientosDescuentoSistema.getCreditoPorEmpleado().getSaldo() + creditoMovimientosDescuentoSistema.getImporte()));
                                        getSession().SaveOrUpdate(creditoMovimientosDescuentoSistema.creditoPorEmpleado);
                                        // dbContextSimple.saveOrUpdate(creditoMovimientosDescuentoSistema.getCreditoPorEmpleado());
                                        getSession().Delete(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                        //dbContextSimple.Set<MovNomConcep>().Remove(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                        //dbContextSimple.delete(creditoMovimientosDescuentoSistema.getMovNomConceps().get(k));

                                    }
                                    creditoMovimientosDescuentoSistema.movNomConcep.Clear();
                                    // dbContextSimple.SaveChanges();
                                    if (!tieneMovOtrasCorridas)
                                    {
                                        //dbContextSimple.Set<CreditoMovimientos>().Attach(creditoMovimientosDescuentoSistema);
                                        getSession().Delete(creditoMovimientosDescuentoSistema);
                                        //dbContextSimple.delete(creditoMovimientosDescuentoSistema);
                                    }
                                    //dbContextSimple.SaveChanges();
                                }
                            }
                        }//if de mensajeresultado.noerror2

                    }// end for listCreditoPorEmpleado
                    if (listCreditoPorEmpleado.Count() == 0)
                    {
                        List<object> idsMovDelete = new List<object>();
                        for (int k = 0; k < filtroMovimientosNominas.Count(); k++)
                        {
                            if (filtroMovimientosNominas[k].id > 0 & filtroMovimientosNominas[k].creditoMovimientos != null)
                            {
                                if (string.Equals(filtroMovimientosNominas[k].creditoMovimientos.creditoPorEmpleado.creditoAhorro.tipoConfiguracion, "1", StringComparison.OrdinalIgnoreCase))
                                { //credito
                                    idsMovDelete.Add(filtroMovimientosNominas[k].id);
                                }
                            }
                        }
                        if (idsMovDelete.Count() > 0)
                        {
                            MovimientosNominaHDAO movimientosNominaDAO = new MovimientosNominaHDAO();
                            //movimientosNominaDAO.setSession(dbContextSimple);
                            movimientosNominaDAO.deleteListQueryMov("MovNomConcep", "id", idsMovDelete.ToArray(), null, null, null, true);//pendiente la conexion
                                                                                                                                          //  movimientosNominaDAO.deleteListQueryMov(MovNomConcep.class.getSimpleName(), "id", idsMovDelete.toArray(), null, null, null, true);
                        }
                    }
                }//end if mensajeResultado.noError==0
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ejecutaDescuentosPrestamos()1_Error: ").Append(ex));
                mensajeResultado.noError = 27;
                mensajeResultado.error = ex.GetBaseException().ToString();


            }
            return importeNeto - importeAcumulado;
        }

        private List<MovNomConcep> existeMovimientoNomina(MovNomConcep movNomConcep)
        {
            IList<MovNomConcep> values = null;
            try
            {
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);

                strQuery.Remove(0, strQuery.Length).Append("from MovNomConcep o");

                strWhere.Remove(0, strWhere.Length).Append(" WHERE ");
                strWhere.Append(" o.razonesSociales.clave = :razonesSociales and o.empleado.clave = :empleado and o.tipoNomina.clave = :tipoNomina and o.periodosNomina.id = :periodosNomina and ");
                strWhere.Append(" o.tipoCorrida.clave = :tipoCorrida and o.concepNomDefi.clave = :concepNomDefi and o.ejercicio = :ejercicio and o.mes = :mes ");
                camposParametro.Add("razonesSociales");
                valoresParametro.Add(movNomConcep.razonesSociales.clave);
                camposParametro.Add("empleado");
                valoresParametro.Add(movNomConcep.empleados.clave);
                camposParametro.Add("tipoNomina");
                valoresParametro.Add(movNomConcep.tipoNomina.clave);
                camposParametro.Add("periodosNomina");
                valoresParametro.Add(movNomConcep.periodosNomina.id);
                camposParametro.Add("tipoCorrida");
                valoresParametro.Add(movNomConcep.tipoCorrida.clave);
                camposParametro.Add("concepNomDefi");
                valoresParametro.Add(movNomConcep.concepNomDefi.clave);
                camposParametro.Add("ejercicio");
                valoresParametro.Add(movNomConcep.ejercicio);
                camposParametro.Add("mes");
                valoresParametro.Add(movNomConcep.mes);

                strQuery.Append(strWhere);
                IQuery q = getSession().CreateQuery(strQuery.ToString());
                q = cargarParametrosQuery(q, camposParametro, valoresParametro);
                values = q.List<MovNomConcep>();
                //values = (List<MovNomConcep>)ejecutaQueryList(strQuery.toString(), camposParametro.toArray(new String[] { }), valoresParametro.toArray(), 0);
                camposParametro = null;
                valoresParametro = null;
            }
            catch (HibernateException ex)
            {

                getSession().Transaction.Rollback();
            }
            return (List<MovNomConcep>)values;
        }

        private List<Object> ejercutarManejoDescuento(CreditoMovimientos creditoMovimientosDescuentoSistema, Double diasIMSS, Double importe, List<MovNomConcep> listMovNomConcepCreditosAhorroDescuentoActivo, double factorMensual, double diasBimestre, MovNomConcep movNomConcepAbarca2Meses)
        {
            double importeDescuento;
            int indice = -1;
            try
            {
                importeDescuento = creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.importeDescuento.GetValueOrDefault();
                int dias = 0;
                if (isMov2Meses)
                {
                    dias = (cantidadDiasEntreDosFechas(periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault()) + 1); //+1 para contar el dia actual//obtener dias reales del periodo;
                }
                if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.periodicidadDescuento == 0)
                {//Al descontar XXXXX
                    if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoManejoDescuento == 2)
                    {//Por importe fijo
                        if (isMov2Meses)
                        {
                            importeDescuento = (importeDescuento / dias) * diasIMSS;
                        }
                        if (!string.Equals(creditoMovimientosDescuentoSistema.movNomConcep[0].concepNomDefi.clave, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.concepNomDefi_cNDescuento_ID.clave, StringComparison.OrdinalIgnoreCase))
                        {
                            #region Cuando son diferentes conceptos tanto para el credito principal como para el descuento, se tiene que buscar el movimiento, si no construirlo
                            indice = obtenerMovNomConcepCreditosAhorroDescuentoActivo(listMovNomConcepCreditosAhorroDescuentoActivo, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.concepNomDefi_cNDescuento_ID, creditoMovimientosDescuentoSistema, movNomConcepAbarca2Meses);
                            if (indice > -1)
                            {
                                listMovNomConcepCreditosAhorroDescuentoActivo[indice].calculado = importeDescuento;
                                listMovNomConcepCreditosAhorroDescuentoActivo[indice].resultado = importeDescuento;
                            }
                            #endregion
                        }

                        importe += importeDescuento;
                    }

                }
                else if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.periodicidadDescuento == 1)
                {//Mensual
                    if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.cuandoDescontar == 0)
                    {// Descontar proporcionalmente cada periodo de nómina
                        importeDescuento = (importeDescuento / factorMensual) * diasIMSS;
                        if (!string.Equals(creditoMovimientosDescuentoSistema.movNomConcep[0].concepNomDefi.clave, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.concepNomDefi_cNDescuento_ID.clave, StringComparison.OrdinalIgnoreCase))
                        {
                            #region Cuando son diferentes conceptos tanto para el credito principal como para el descuento, se tiene que buscar el movimiento, si no construirlo
                            indice = obtenerMovNomConcepCreditosAhorroDescuentoActivo(listMovNomConcepCreditosAhorroDescuentoActivo, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.concepNomDefi_cNDescuento_ID, creditoMovimientosDescuentoSistema, movNomConcepAbarca2Meses);
                            if (indice > -1)
                            {
                                listMovNomConcepCreditosAhorroDescuentoActivo[indice].calculado = importeDescuento;
                                listMovNomConcepCreditosAhorroDescuentoActivo[indice].resultado = importeDescuento;
                            }

                            #endregion
                        }
                        importe += importeDescuento;
                    }
                    else if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.cuandoDescontar == 1 //Descontar en el primer periodo de nómina del mes
                      || creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.cuandoDescontar == 2/*descontar en el último periodo del mes*/)
                    {
                        bool continuar = false;
                        if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.cuandoDescontar == 1)
                        {//descontar en el primer periodo del mes
                            if (eresPeriodoDelMes(periodosNomina, true, false))
                            {
                                continuar = true;
                            }
                        }
                        else if (periodosNomina.cierreMes)
                        {//descontar en el último periodo del mes
                            continuar = true;
                        }
                        if (continuar)
                        {
                            if (isMov2Meses)
                            {
                                importeDescuento = (importeDescuento / factorMensual) * diasIMSS;
                            }
                            if (!string.Equals(creditoMovimientosDescuentoSistema.movNomConcep[0].concepNomDefi.clave, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.concepNomDefi_cNDescuento_ID.clave, StringComparison.OrdinalIgnoreCase))
                            {
                                #region Cuando son diferentes conceptos tanto para el credito principal como para el descuento, se tiene que buscar el movimiento, si no construirlo
                                indice = obtenerMovNomConcepCreditosAhorroDescuentoActivo(listMovNomConcepCreditosAhorroDescuentoActivo, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.concepNomDefi_cNDescuento_ID, creditoMovimientosDescuentoSistema, movNomConcepAbarca2Meses);
                                if (indice > -1)
                                {
                                    listMovNomConcepCreditosAhorroDescuentoActivo[indice].calculado = importeDescuento;
                                    listMovNomConcepCreditosAhorroDescuentoActivo[indice].resultado = importeDescuento;
                                }
                                #endregion

                            }
                            importe += importeDescuento;
                        }

                    }

                }
                else if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.periodicidadDescuento == 2)
                {
                    if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.cuandoDescontar == 0)
                    {
                        importeDescuento = (importeDescuento / diasBimestre) * diasIMSS;
                        if (!string.Equals(creditoMovimientosDescuentoSistema.movNomConcep[0].concepNomDefi.clave, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.concepNomDefi_cNDescuento_ID.clave, StringComparison.OrdinalIgnoreCase))
                        {
                            #region Cuando son diferentes conceptos tanto para el credito principal como para el descuento, se tiene que buscar el movimiento, si no construirlo
                            indice = obtenerMovNomConcepCreditosAhorroDescuentoActivo(listMovNomConcepCreditosAhorroDescuentoActivo, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.concepNomDefi_cNDescuento_ID, creditoMovimientosDescuentoSistema, movNomConcepAbarca2Meses);
                            if (indice > -1)
                            {
                                listMovNomConcepCreditosAhorroDescuentoActivo[indice].calculado = importeDescuento;
                                listMovNomConcepCreditosAhorroDescuentoActivo[indice].resultado = importeDescuento;
                            }
                            #endregion
                        }
                        importe += importeDescuento;
                    }
                    else if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.cuandoDescontar == 1 //Descontar en el primer periodo de nómina del bimestral
                      || creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.cuandoDescontar == 2/*descontar en el último periodo del bimestral*/)
                    {
                        bool continuar = false;
                        if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.cuandoDescontar == 1)
                        {//descontar en el primer periodo del bimestral
                            if (eresPeriodoDelMes(periodosNomina, false, false))
                            {
                                continuar = true;
                            }
                        }
                        else if (eresPeriodoDelMes(periodosNomina, false, true))
                        {//descontar en el último periodo del bimestral
                            continuar = true;
                        }
                        if (continuar)
                        {
                            if (isMov2Meses)
                            {
                                importeDescuento = (importeDescuento / diasBimestre) * diasIMSS;
                            }
                            if (!string.Equals(creditoMovimientosDescuentoSistema.movNomConcep[0].concepNomDefi.clave, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.concepNomDefi_cNDescuento_ID.clave, StringComparison.OrdinalIgnoreCase))
                            {
                                #region   Cuando son diferentes conceptos tanto para el credito principal como para el descuento, se tiene que buscar el movimiento, si no construirlo
                                indice = obtenerMovNomConcepCreditosAhorroDescuentoActivo(listMovNomConcepCreditosAhorroDescuentoActivo, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.concepNomDefi_cNDescuento_ID, creditoMovimientosDescuentoSistema, movNomConcepAbarca2Meses);
                                if (indice > -1)
                                {
                                    listMovNomConcepCreditosAhorroDescuentoActivo[indice].calculado = importeDescuento;
                                    listMovNomConcepCreditosAhorroDescuentoActivo[indice].resultado = importeDescuento;
                                }
                                #endregion
                            }
                            importe += importeDescuento;
                        }

                    }


                }
                else if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.periodicidadDescuento == 3)
                {//Unico
                    bool existeDescuentoAplicado = buscarManejoDescuentoUnicoExistente(creditoMovimientosDescuentoSistema.creditoPorEmpleado.empleados.id,
                       creditoMovimientosDescuentoSistema.creditoPorEmpleado.empleados.razonesSociales.id, tipoCorrida.id, periodosNomina.tipoNomina.id, periodosNomina.año,
                       creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.concepNomDefi_cNDescuento_ID.id,
                       creditoMovimientosDescuentoSistema.creditoPorEmpleado.id, 0);
                    if (!existeDescuentoAplicado)
                    {
                        if (isMov2Meses)
                        {
                            importeDescuento = (importeDescuento / dias) * diasIMSS;
                        }
                        if (!string.Equals(creditoMovimientosDescuentoSistema.movNomConcep[0].concepNomDefi.clave, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.concepNomDefi_cNDescuento_ID.clave, StringComparison.OrdinalIgnoreCase))
                        {
                            #region Cuando son diferentes conceptos tanto para el credito principal como para el descuento, se tiene que buscar el movimiento, si no construirlo
                            indice = obtenerMovNomConcepCreditosAhorroDescuentoActivo(listMovNomConcepCreditosAhorroDescuentoActivo, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.concepNomDefi_cNDescuento_ID, creditoMovimientosDescuentoSistema, movNomConcepAbarca2Meses);
                            if (indice > -1)
                            {
                                listMovNomConcepCreditosAhorroDescuentoActivo[indice].calculado = importeDescuento;
                                listMovNomConcepCreditosAhorroDescuentoActivo[indice].resultado = importeDescuento;
                            }
                            #endregion
                        }
                        importe += importeDescuento;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ejercutarManejoDescuento()1_Error: ").Append(ex));
                mensajeResultado.noError = 27;
                mensajeResultado.error = ex.GetBaseException().ToString();

            }
            if (indice > -1)
            {
                //            for (int l = 0; l < listMovNomConcepCreditosAhorroDescuentoPendienteDeEliminar.size(); l++) {
                //                if (listMovNomConcepCreditosAhorroDescuentoActivo.get(indice).getId() != null) {
                //                    if (listMovNomConcepCreditosAhorroDescuentoActivo.get(indice).getId().equals(listMovNomConcepCreditosAhorroDescuentoPendienteDeEliminar.get(l).getId())) {
                //                        listMovNomConcepCreditosAhorroDescuentoPendienteDeEliminar.remove(l);
                //                    }
                //                }
                //            }
                listMovNomConcepCreditosAhorroDescuentoActivo[indice].creditoMovimientos = creditoMovimientosDescuentoSistema;
            }
            List<Object> listobject = new List<Object>();
            listobject.Add(importe);
            listobject.Add(listMovNomConcepCreditosAhorroDescuentoActivo);
            listobject.Add(indice > -1 ? listMovNomConcepCreditosAhorroDescuentoActivo[indice] : null);//envio el movimiento que fue modificado
            return listobject;
        }

        private bool buscarManejoDescuentoUnicoExistente(decimal idEmpleado, decimal idRazonesSociales, decimal idTipoCorrida, decimal idTipoNomina, int ejercicio, decimal idConcepto, decimal idCreditoPorEmpleado, decimal idMovimientosDescuentoExistente)
        {
            bool existe = false;
            try
            {
                IList<MovNomConcep> values = null;
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);

                strQuery.Remove(0, strQuery.Length).Append("from MovNomConcep o");

                strWhere.Remove(0, strWhere.Length).Append(" WHERE ");
                strWhere.Append(" o.razonesSociales.id = :razonesSociales and o.empleado.id = :empleado and o.tipoNomina.id = :tipoNomina and ");
                strWhere.Append(" o.tipoCorrida.id = :tipoCorrida and o.concepNomDefi.id = :concepNomDefi and o.ejercicio = :ejercicio and o.creditoMovimientos.creditoPorEmpleado.id = :creditoPorEmpleado ");
                camposParametro.Add("razonesSociales");
                valoresParametro.Add(idRazonesSociales);
                camposParametro.Add("empleado");
                valoresParametro.Add(idEmpleado);
                camposParametro.Add("tipoNomina");
                valoresParametro.Add(idTipoNomina);
                camposParametro.Add("tipoCorrida");
                valoresParametro.Add(idTipoCorrida);
                camposParametro.Add("concepNomDefi");
                valoresParametro.Add(idConcepto);
                camposParametro.Add("ejercicio");
                valoresParametro.Add(ejercicio);
                camposParametro.Add("creditoPorEmpleado");
                valoresParametro.Add(idCreditoPorEmpleado);
                strQuery.Append(strWhere);
                IQuery q = getSession().CreateQuery(strQuery.ToString());
                q = cargarParametrosQuery(q, camposParametro, valoresParametro);
                values = q.List<MovNomConcep>();
                if (values == null ? false : values.Any())
                {
                    existe = true;
                }
                camposParametro = null;
                valoresParametro = null;
            }
            catch (HibernateException ex)
            {
                mensajeResultado.error = ex.GetBaseException().Message;
                mensajeResultado.noError = 27;
                //System.err.println(concatena.delete(0, concatena.length()).append(msgError).append("buscarManejoDescuentoUnicoExistente()1_Error: ").append(ex));
            }
            return existe;
        }

        private bool eresPeriodoDelMes(PeriodosNomina periodosNominaEjecutandose, bool isMes/*es para saber si buscara por mes o bimestre*/, bool ultimoPeriodoBimestre/*Para saber si va evaluar si es el ultimo periodo del bimestre y claro tiene que venir false isMes*/)
        {
            bool esPeriodoCorrecto = false;
            string claveMAx = "", claveMin = "";
            try
            {
                IList<PeriodosNomina> listPeriodosNominas = null;
                if (isMes)
                {
                    strQuery.Remove(0, strQuery.Length).Append("SELECT p ");
                    strQuery.Append("FROM PeriodosNomina p INNER JOIN p.tipoNomina t INNER JOIN p.tipoCorrida c INNER JOIN t.periodicidad pd WHERE ");
                    //if (HibernateUtil.usaTypeBigInt)
                    //{
                    //    strQuery.append("(p.clave > (SELECT CASE WHEN (count(pn) > 0) THEN MAX(CAST(pn.clave as int)) ELSE 0 END FROM PeriodosNomina pn INNER JOIN pn.tipoNomina tn WHERE pn.clave < cast(:clavePeriodoNomina as int) AND tn.clave = :claveTipoNomina AND pn.cierreMes = true AND pn.año = :yearPeriodo AND pn.tipoCorrida.clave = :claveTipoCorrida) ");
                    //}
                    //else
                    //{
                    strQuery.Append("(p.clave > (SELECT CASE WHEN (count(pn) > 0) THEN MAX(CAST(pn.clave as long)) ELSE 0 END FROM PeriodosNomina pn INNER JOIN pn.tipoNomina tn WHERE pn.clave < cast(:clavePeriodoNomina as long) AND tn.clave = :claveTipoNomina AND pn.cierreMes = true AND pn.año = :yearPeriodo AND pn.tipoCorrida.clave = :claveTipoCorrida) ");
                    //}

                    camposParametro = new List<String>(0);
                    valoresParametro = new List<Object>(0);
                    //if (HibernateUtil.usaTypeBigInt)
                    //{
                    //    strQuery.append("AND p.clave <= (SELECT CASE WHEN (count(pn) > 0) THEN MIN(CAST(pn.clave as int)) ELSE 0 END FROM PeriodosNomina pn INNER JOIN pn.tipoNomina tn WHERE pn.clave > cast(:clavePeriodoNomina as int) AND tn.clave = :claveTipoNomina AND pn.cierreMes = true AND pn.año = :yearPeriodo AND pn.tipoCorrida.clave = :claveTipoCorrida)) ");
                    //}
                    //else
                    //{
                    strQuery.Append("AND p.clave <= (SELECT CASE WHEN (count(pn) > 0) THEN MIN(CAST(pn.clave as long)) ELSE 0 END FROM PeriodosNomina pn INNER JOIN pn.tipoNomina tn WHERE pn.clave > cast(:clavePeriodoNomina as long) AND tn.clave = :claveTipoNomina AND pn.cierreMes = true AND pn.año = :yearPeriodo AND pn.tipoCorrida.clave = :claveTipoCorrida)) ");
                    // }

                    strQuery.Append("AND t.clave = :claveTipoNomina AND p.año = :yearPeriodo AND c.clave = :claveTipoCorrida ");//AND :fechaIngresoEmp <= p.fechaFinal AND :fechaFinEmp >= p.fechaInicial");
                    strQuery.Append(" ORDER BY p.clave ");
                    camposParametro.Add("clavePeriodoNomina");
                    camposParametro.Add("claveTipoNomina");
                    camposParametro.Add("yearPeriodo");
                    camposParametro.Add("claveTipoCorrida");
                    valoresParametro.Add(valoresConceptosEmpleados["NumPeriodo".ToUpper()]);
                    valoresParametro.Add(valoresConceptosEmpleados["TipoNomina".ToUpper()]);
                    valoresParametro.Add(valoresConceptosEmpleados["AnioPeriodo".ToUpper()]);
                    valoresParametro.Add(valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()]);
                    IQuery q = getSession().CreateQuery(strQuery.ToString());
                    q = cargarParametrosQuery(q, camposParametro, valoresParametro);
                    listPeriodosNominas = q.List<PeriodosNomina>();

                }
                else
                {

                    DateTime fechaBimestre = DateTime.Now;
                    fechaBimestre = periodosNomina.acumularAMes.GetValueOrDefault();
                    int[] meses = new int[2];
                    meses[0] = fechaBimestre.Month + 1;
                    if ((fechaBimestre.Month + 1) % 2 == 0)
                    {
                        fechaBimestre.AddMonths(fechaBimestre.Month - 1);
                    }
                    else
                    {
                        fechaBimestre.AddMonths(fechaBimestre.Month + 1);
                    }
                    meses[1] = fechaBimestre.Month + 1;
                    strQuery.Remove(0, strQuery.Length).Append("SELECT p ");
                    strQuery.Append("FROM PeriodosNomina p INNER JOIN p.tipoNomina t INNER JOIN p.tipoCorrida c WHERE t.clave = :claveTipoNomina AND p.año = :yearPeriodo AND c.clave = :claveTipoCorrida AND (MONTH(p.AcumularAMes) in(:valoresMeses) AND YEAR(p.AcumularAMes) = :yearPeriodo) ");

                    camposParametro = new List<String>(0);
                    valoresParametro = new List<Object>(0);
                    strQuery.Append(" ORDER BY p.clave ");
                    camposParametro.Add("claveTipoNomina");
                    camposParametro.Add("yearPeriodo");
                    camposParametro.Add("claveTipoCorrida");
                    camposParametro.Add("valoresMeses");
                    valoresParametro.Add(valoresConceptosEmpleados["TipoNomina".ToUpper()]);
                    valoresParametro.Add(valoresConceptosEmpleados["AnioPeriodo".ToUpper()]);
                    valoresParametro.Add(valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()]);
                    valoresParametro.Add(meses);
                }
                listPeriodosNominas = (listPeriodosNominas == null) ? new List<PeriodosNomina>() : listPeriodosNominas;
                if (listPeriodosNominas.Count() > 0)
                {
                    if (ultimoPeriodoBimestre)
                    {
                        if (listPeriodosNominas[listPeriodosNominas.Count() - 1].id.Equals(periodosNominaEjecutandose.id))
                        {
                            esPeriodoCorrecto = true;
                        }
                    }
                    else if (listPeriodosNominas[0].id.Equals(periodosNominaEjecutandose.id))
                    {
                        esPeriodoCorrecto = true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("eresPeriodoDelMes()1_Error: ").Append(ex));
                mensajeResultado.noError = 27;
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return esPeriodoCorrecto;

        }

        private int obtenerMovNomConcepCreditosAhorroDescuentoActivo(List<MovNomConcep> listMovNomConcepCreditosAhorroDescuentoActivo, ConcepNomDefi conceptoDescuento, CreditoMovimientos creditoMovimientos, MovNomConcep movNomConcepAbarca2Meses)
        {
            int indice = -1;
            bool continuar = false;
            MovNomConcep movNomConcepTmp = null;//utilizado para cuando en 2 creditosAhorros tienen el mismo concepto.
            int numero = 1;
            for (int i = 0; i < listMovNomConcepCreditosAhorroDescuentoActivo.Count(); i++)
            {
                if (string.Equals(listMovNomConcepCreditosAhorroDescuentoActivo[i].concepNomDefi.clave, conceptoDescuento.clave, StringComparison.OrdinalIgnoreCase))
                {
                    continuar = true;
                    if (listMovNomConcepCreditosAhorroDescuentoActivo[i].numero > numero)
                    {
                        numero = listMovNomConcepCreditosAhorroDescuentoActivo[i].numero.GetValueOrDefault();
                    }
                    if (listMovNomConcepCreditosAhorroDescuentoActivo[i].creditoMovimientos != null)
                    {
                        if (!listMovNomConcepCreditosAhorroDescuentoActivo[i].creditoMovimientos.creditoPorEmpleado.id.Equals(creditoMovimientos.creditoPorEmpleado.id))
                        {
                            continuar = false;
                            movNomConcepTmp = listMovNomConcepCreditosAhorroDescuentoActivo[i];
                        }
                        else
                        {
                            movNomConcepTmp = null;
                        }
                    }
                    if (continuar)
                    {
                        if (movNomConcepAbarca2Meses != null)
                        {
                            if (!movNomConcepAbarca2Meses.mes.Equals(listMovNomConcepCreditosAhorroDescuentoActivo[i].mes))
                            {
                                continuar = false;
                                movNomConcepTmp = listMovNomConcepCreditosAhorroDescuentoActivo[i];
                            }
                            else
                            {
                                movNomConcepTmp = null;
                            }
                        }
                    }
                    if (continuar)
                    {
                        indice = i;
                        break;
                    }
                }
            }
            if (movNomConcepTmp != null)
            {
                movNomConcepTmp = creaMovNomConcep(movNomConcepTmp.concepNomDefi, movNomConcepTmp.plazasPorEmpleado, periodosNomina, tipoCorrida, razonesSociales, centroDeCostoMovimiento);
                movNomConcepTmp.creditoMovimientos = creditoMovimientos;
                movNomConcepTmp.numero = numero + 1;
                listMovNomConcepCreditosAhorroDescuentoActivo.Add(movNomConcepTmp);
                indice = listMovNomConcepCreditosAhorroDescuentoActivo.Count() - 1;
                if (evaluaPeriodoAbarca2Meses(movNomConcepTmp.periodosNomina))
                {
                    MovNomConcep newMov = MovNomConcep.copiaMovimiento(movNomConcepTmp);
                    DateTime fechaPeriodo = DateTime.Now, fechaPromocion = DateTime.Now;
                    fechaPromocion = newMov.periodosNomina.fechaInicial.GetValueOrDefault();
                    fechaPeriodo = newMov.periodosNomina.fechaFinal.GetValueOrDefault();
                    newMov.mes = fechaPeriodo.Month + 1;
                    newMov.numMovParticion = 2;
                    listMovNomConcepCreditosAhorroDescuentoActivo.Add(newMov);
                    int mesUno = fechaPromocion.Month + 1;
                    listMovNomConcepCreditosAhorroDescuentoActivo[listMovNomConcepCreditosAhorroDescuentoActivo.Count() - 1].ejercicio = periodosNomina.año;
                    listMovNomConcepCreditosAhorroDescuentoActivo[listMovNomConcepCreditosAhorroDescuentoActivo.Count() - 1].mes = fechaPromocion.Month + 1;
                    if (mesUno.Equals(fechaPromocion.Month + 1))
                    {
                        listMovNomConcepCreditosAhorroDescuentoActivo[listMovNomConcepCreditosAhorroDescuentoActivo.Count() - 1].numMovParticion = 1;
                    }
                    else
                    {
                        listMovNomConcepCreditosAhorroDescuentoActivo[listMovNomConcepCreditosAhorroDescuentoActivo.Count() - 1].numMovParticion = 2;
                    }
                }
            }
            return indice;
        }

        private CreditoMovimientos crearCreditoMovimientoSistema(CreditoPorEmpleado creditoPorEmpleado, bool soloAgregarMovimientosNomina, List<MovNomConcep> filtroMovimientosNominas, CreditoMovimientos creditoMovimientos)
        {
            List<MovNomConcep> listMovNomConcepCreditosAhorro = new List<MovNomConcep>();
            int z = 0;
            bool continuar = false;
            MovNomConcep movNomConcepTmp = null;//utilizado para cuando en 2 creditosAhorros tienen el mismo concepto.
            int numero = 1;
            while (z < filtroMovimientosNominas.Count())
            {
                if (string.Equals(creditoPorEmpleado.creditoAhorro.concepNomDefi_concepNomiDefin_ID.clave, filtroMovimientosNominas[z].concepNomDefi.clave, StringComparison.OrdinalIgnoreCase))
                {
                    continuar = true;
                    if (filtroMovimientosNominas[z].numero > numero)
                    {
                        numero = filtroMovimientosNominas[z].numero.GetValueOrDefault();
                    }
                    if (filtroMovimientosNominas[z].creditoMovimientos != null)
                    {
                        if (!filtroMovimientosNominas[z].creditoMovimientos.creditoPorEmpleado.id.Equals(creditoPorEmpleado.id))
                        {
                            continuar = false;
                            movNomConcepTmp = filtroMovimientosNominas[z];
                        }
                        else
                        {
                            movNomConcepTmp = null;
                        }
                    }
                    else
                    {
                        movNomConcepTmp = null;
                    }
                }
                if (continuar)
                {
                    listMovNomConcepCreditosAhorro.Add(filtroMovimientosNominas[z]);
                    continuar = false;
                }
                {
                    z++;
                }
            }
            if (movNomConcepTmp != null)
            {
                movNomConcepTmp = creaMovNomConcep(movNomConcepTmp.concepNomDefi, movNomConcepTmp.plazasPorEmpleado, periodosNomina, tipoCorrida, razonesSociales, centroDeCostoMovimiento);
                movNomConcepTmp.creditoMovimientos = creditoMovimientos;
                movNomConcepTmp.numero = numero + 1;
                listMovNomConcepCreditosAhorro.Add(movNomConcepTmp);
                if (evaluaPeriodoAbarca2Meses(movNomConcepTmp.periodosNomina))
                {
                    MovNomConcep newMov = MovNomConcep.copiaMovimiento(movNomConcepTmp);
                    DateTime fechaPeriodo = DateTime.Now, fechaPromocion = DateTime.Now;
                    fechaPromocion = newMov.periodosNomina.fechaInicial.GetValueOrDefault();
                    fechaPeriodo = newMov.periodosNomina.fechaFinal.GetValueOrDefault();
                    newMov.mes = fechaPeriodo.Month + 1;
                    newMov.numMovParticion = 2;
                    listMovNomConcepCreditosAhorro.Add(newMov);
                    int mesUno = fechaPromocion.Month + 1;
                    listMovNomConcepCreditosAhorro[listMovNomConcepCreditosAhorro.Count() - 1].ejercicio = periodosNomina.año;
                    listMovNomConcepCreditosAhorro[listMovNomConcepCreditosAhorro.Count() - 1].mes = fechaPromocion.Month + 1;
                    if (mesUno.Equals(fechaPromocion.Month + 1))
                    {
                        listMovNomConcepCreditosAhorro[listMovNomConcepCreditosAhorro.Count() - 1].numMovParticion = 1;
                    }
                    else
                    {
                        listMovNomConcepCreditosAhorro[listMovNomConcepCreditosAhorro.Count() - 1].numMovParticion = 2;
                    }
                }
            }
            listMovNomConcepCreditosAhorro = (from list in listMovNomConcepCreditosAhorro orderby list.ejercicio, list.mes select list).ToList();


            if (soloAgregarMovimientosNomina)
            {
                creditoMovimientos.movNomConcep = listMovNomConcepCreditosAhorro;
            }
            else
            {
                creditoMovimientos = new CreditoMovimientos();
                creditoMovimientos.creditoPorEmpleado = creditoPorEmpleado;
                creditoMovimientos.tiposMovimiento = (int)TiposMovimiento.AbonoSistema;
                creditoMovimientos.fecha = periodosNomina.fechaFinal.GetValueOrDefault();
                creditoMovimientos.movNomConcep = listMovNomConcepCreditosAhorro;
                creditoMovimientos.importe = creditoPorEmpleado.montoDescuento;
            }
            return creditoMovimientos;
        }

        private Boolean obtenerNumeroParcialidadesCreditoMovimientos(CreditoPorEmpleado creditoPorEmpleado, TiposMovimiento tiposMovimiento, CreditoMovimientos creditoMovimientosDescuentoSistema)
        {
            Boolean continuar = false;
            try
            {
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                strQuery.Remove(0, strQuery.Length).Append("select count(credMov) from CreditoMovimientos credMov inner join credMov.creditoPorEmpleado credEm ");
                strWhere.Remove(0, strWhere.Length).Append("WHERE ");
                strWhere.Append("credEm.id = :creditoPorEmpleadoID AND credMov.tiposMovimiento = :tiposMovimiento ");

                camposParametro.Add("creditoPorEmpleadoID");
                valoresParametro.Add(creditoPorEmpleado.id);

                camposParametro.Add("tiposMovimiento");
                valoresParametro.Add(tiposMovimiento);

                strQuery.Append(strWhere);
                double numeroCreditoMovimientos = (double)ejecutaQueryUnico(strQuery.ToString(), camposParametro.ToArray<string>(), valoresParametro.ToArray());
                if (numeroCreditoMovimientos < creditoPorEmpleado.numeroParcialidades)
                {
                    continuar = true;
                }
                else if (creditoMovimientosDescuentoSistema != null)
                {
                    if (creditoMovimientosDescuentoSistema.id > 0)
                    {
                        continuar = true;
                    }
                }
                camposParametro = null;
                valoresParametro = null;
            }
            catch (HibernateException ex)
            {
                mensajeResultado.error = ex.GetBaseException().Message;
                mensajeResultado.noError = 27;

            }
            return continuar;
        }

        private CreditoMovimientos obtenerCreditoMovimientosMax(CreditoPorEmpleado creditoPorEmpleado, TiposMovimiento tiposMovimiento)
        {
            CreditoMovimientos creditoMovimientos = null;
            try
            {
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                strQuery.Remove(0, strQuery.Length).Append("select credMov from CreditoMovimientos credMov inner join credMov.creditoPorEmpleado credEm ");
                strWhere.Remove(0, strWhere.Length).Append("WHERE ");
                strWhere.Append("credMov.fecha= ( select max(credMov2.fecha) from CreditoMovimientos credMov2 inner join credMov2.creditoPorEmpleado credEm2 ");
                strWhere.Append("WHERE credEm2.id = :creditoPorEmpleadoID AND credMov2.tiposMovimiento = :tiposMovimiento AND credMov2.fecha <= :fechaFinal ) ");
                strWhere.Append("AND credEm.id = :creditoPorEmpleadoID AND credMov.tiposMovimiento = :tiposMovimiento ");

                camposParametro.Add("creditoPorEmpleadoID");
                valoresParametro.Add(creditoPorEmpleado.id);

                camposParametro.Add("tiposMovimiento");
                valoresParametro.Add(tiposMovimiento);

                camposParametro.Add("fechaFinal");
                valoresParametro.Add(periodosNomina.fechaFinal);

                strQuery.Append(strWhere);
                creditoMovimientos = (CreditoMovimientos)ejecutaQueryUnico(strQuery.ToString(), camposParametro.ToArray<string>(), valoresParametro.ToArray());
                camposParametro = null;
                valoresParametro = null;
            }
            catch (HibernateException ex)
            {
                mensajeResultado.error = ex.GetBaseException().Message;
                mensajeResultado.noError = 27;

            }
            return creditoMovimientos;
        }


        private CreditoMovimientos obtenerCreditoMovimientosPorPeriodoNomina(CreditoPorEmpleado creditoPorEmpleado, TiposMovimiento tiposMovimiento)
        {
            CreditoMovimientos creditoMovimientos = null;
            try
            {
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                strQuery.Remove(0, strQuery.Length).Append("select credMov from CreditoMovimientos credMov inner join credMov.creditoPorEmpleado credEm ");
                strWhere.Remove(0, strWhere.Length).Append("WHERE ");
                strWhere.Append("credMov.fecha BETWEEN  :fechaInicial AND :fechaFinal ");
                strWhere.Append("AND credEm.id = :creditoPorEmpleadoID AND credMov.tiposMovimiento = :tiposMovimiento ");

                camposParametro.Add("creditoPorEmpleadoID");
                valoresParametro.Add(creditoPorEmpleado.id);

                camposParametro.Add("tiposMovimiento");
                valoresParametro.Add(tiposMovimiento);

                camposParametro.Add("fechaInicial");
                valoresParametro.Add(periodosNomina.fechaInicial);
                camposParametro.Add("fechaFinal");
                valoresParametro.Add(periodosNomina.fechaFinal);

                strQuery.Append(strWhere);
                creditoMovimientos = (CreditoMovimientos)ejecutaQueryUnico(strQuery.ToString(), camposParametro.ToArray<string>(), valoresParametro.ToArray());
                camposParametro = null;
                valoresParametro = null;
            }
            catch (HibernateException ex)
            {
                mensajeResultado.error = ex.GetBaseException().Message;
                mensajeResultado.noError = 27;

            }
            return creditoMovimientos;
        }

        private List<CreditoPorEmpleado> obtenerCreditoPorEmpleado(PlazasPorEmpleadosMov plazasPorEmpleadosMov, String tipoConfiguracion, List<MovNomConcep> listMovNomConcep)
        {
            IList<CreditoPorEmpleado> listCreditoPorEmpleado = null;
            try
            {
                List<String> listClaveConcep = new List<String>();
                for (int i = 0; i < listMovNomConcep.Count; i++)
                {
                    listClaveConcep.Add(listMovNomConcep[i].concepNomDefi.clave);
                }
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                strQuery.Remove(0, strQuery.Length).Append("select cred from CreditoPorEmpleado cred ");
                strQuery.Append(" inner join cred.razonesSociales rs inner join  cred.creditoAhorro creaho inner join creaho.concepNomDefi_concepNomiDefin_ID concep inner join  cred.empleados em ");

                strWhere.Remove(0, strWhere.Length).Append(" WHERE ");
                strWhere.Append(" em.clave =:claveEmpleado AND ");
                camposParametro.Add("claveEmpleado");
                valoresParametro.Add(plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave);

                strWhere.Append(" creaho.tipoConfiguracion = :tipoConfiguracion AND ");
                camposParametro.Add("tipoConfiguracion");
                valoresParametro.Add(tipoConfiguracion);

                strWhere.Append(" rs.clave = :claveRazonSocial AND ");
                camposParametro.Add("claveRazonSocial");
                valoresParametro.Add(plazasPorEmpleadosMov.plazasPorEmpleado.razonesSociales.clave);

                strWhere.Append(" cred.fechaAutorizacion <= :fechaAutorizacion AND cred.inicioDescuento <= :fechaAutorizacion AND ");
                camposParametro.Add("fechaAutorizacion");
                valoresParametro.Add(periodosNomina.fechaFinal);

                strWhere.Append(" cred.fechaVence >= :fechaVence AND ");
                camposParametro.Add("fechaVence");
                valoresParametro.Add(periodosNomina.fechaInicial);

                strWhere.Append(" concep.clave in (:clavesConceptos) ");
                camposParametro.Add("clavesConceptos");
                valoresParametro.Add(listClaveConcep.ToArray());

                // strWhere.append(" AND  cred.saldo > 0  ");
                strWhere.Append("ORDER BY em.clave ");
                strQuery.Append(strWhere);
                IQuery q = getSession().CreateQuery(strQuery.ToString());
                q = cargarParametrosQuery(q, camposParametro, valoresParametro);

                listCreditoPorEmpleado = q.List<CreditoPorEmpleado>();
                camposParametro = null;
                valoresParametro = null;
            }
            catch (HibernateException ex)
            {
                mensajeResultado.error = (ex.GetBaseException().Message);
                mensajeResultado.noError = 27;

            }
            return (List<CreditoPorEmpleado>)listCreditoPorEmpleado;
        }

        private Double obtenerCantidadPeriodoNominaRango(TipoNomina tipoNomina, PeriodosNomina periodoInicial, PeriodosNomina periodoActual)
        {
            double cantidad = 0.0;
            try
            {
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                strQuery.Remove(0, strQuery.Length).Append("select count(p) from PeriodosNomina p ");

                strWhere.Remove(0, strWhere.Length).Append(" WHERE ");
                strWhere.Append("p.tipoNomina.clave = :tipoNomina AND ");
                camposParametro.Add("tipoNomina");
                valoresParametro.Add(tipoNomina.clave);

                strWhere.Append("p.id > :periodoInicialID AND ");
                camposParametro.Add("periodoInicialID");
                valoresParametro.Add(periodoInicial.id);

                strWhere.Append("p.id <= :periodoActualID ");
                camposParametro.Add("periodoActualID");
                valoresParametro.Add(periodoActual.id);
                strQuery.Append(strWhere);
                cantidad = (double)ejecutaQueryUnico(strQuery.ToString(), camposParametro.ToArray<string>(), valoresParametro.ToArray());

                camposParametro = null;
                valoresParametro = null;
                return cantidad;
            }
            catch (HibernateException ex)
            {
                mensajeResultado.error = ex.GetBaseException().Message;
                mensajeResultado.noError = 27;

                return 0.0;
            }
        }

        private void calcularAguinaldo()
        {
            try
            {
                AguinaldoPagos pagos = null;
                double AgunaldoTotal = 0.0;
                double diaspagados = 0.0;
                double parteExenta = 0.0;
                double isr = 0.0;

                if (aguiConfiguracion == null)
                {
                    mensajeResultado.error = "No se puede calcular aguinaldo, no tiene configuracion aguinaldo agregada";
                    mensajeResultado.noError = 800;
                    return;
                }
                int numPagos = aguiConfiguracion.numPagos;
                strQuery.Remove(0, strQuery.Length);
                strQuery.Append("FROM AguinaldoPagos a WHERE a.razonesSociales.clave=:claveRazonSocial and a.empleado.clave=:claveEmpleado");
                strQuery.Append(" and a.tipoNomina.clave=:claveTipoNomina and a.periodosNomina.id=:clavePeriodosNomina");
                camposParametro = new List<String>();
                valoresParametro = new List<Object>();
                camposParametro.Add("claveRazonSocial");
                camposParametro.Add("claveEmpleado");
                camposParametro.Add("claveTipoNomina");
                camposParametro.Add("clavePeriodosNomina");
                valoresParametro.Add(valoresConceptosGlobales["RazonSocial".ToUpper()]);
                valoresParametro.Add(plazaEmpleadoaguinaldo.plazasPorEmpleado.empleados.clave);
                valoresParametro.Add(valoresConceptosGlobales["TipoNomina".ToUpper()]);
                valoresParametro.Add(periodosNomina.id);
                pagos = (AguinaldoPagos)ejecutaQueryUnico(strQuery.ToString(), camposParametro.ToArray<string>(), valoresParametro.ToArray());

                if (pagos == null)
                {
                    pagos = new AguinaldoPagos();
                }
                if (aguiConfiguracion.pagarEnUnaSolaExhibicion == 0)
                {
                    for (int i = 0; i < filtroMovimientosNominas.Count(); i++)
                    {
                        if (filtroMovimientosNominas[i].concepNomDefi.formulaConcepto.ToUpper().Contains("ImporteAguinaldo".ToUpper()))
                        {
                            pagos.razonesSociales = filtroMovimientosNominas[i].razonesSociales;
                            pagos.empleados = filtroMovimientosNominas[0].empleados;
                            pagos.ejercicio = filtroMovimientosNominas[0].ejercicio;
                            pagos.periodosNomina = filtroMovimientosNominas[0].periodosNomina;
                            pagos.tipoNomina = filtroMovimientosNominas[0].tipoNomina;
                            pagos.diasAguinaldos = Convert.ToDouble(valoresConceptosEmpleados["FactorDiasAguinaldo".ToUpper()]);
                            valoresConceptosEmpleados.Add("DiasAguinaldo".ToUpper(), pagos.diasAguinaldos);
                            pagos.diasPagados = Convert.ToDouble(valoresConceptosEmpleados["FactorDiasAguinaldo".ToUpper()]);
                            pagos.aguinaldo = filtroMovimientosNominas[i].resultado;
                            valoresConceptosEmpleados.Add("ImporteAguinaldo".ToUpper(), pagos.aguinaldo);
                        }
                        else if (filtroMovimientosNominas[i].concepNomDefi.formulaConcepto.ToUpper().Contains("CalculoISR".ToUpper()))
                        {
                            pagos.isr = filtroMovimientosNominas[i].resultado;
                            pagos.razonesSociales = filtroMovimientosNominas[i].razonesSociales;
                            pagos.empleados = filtroMovimientosNominas[i].empleados;
                            pagos.ejercicio = filtroMovimientosNominas[i].ejercicio;
                            pagos.periodosNomina = filtroMovimientosNominas[i].periodosNomina;
                            pagos.tipoNomina = filtroMovimientosNominas[i].tipoNomina;
                        }
                    }
                    getSession().SaveOrUpdate(pagos);
                }
                else if (aguiConfiguracion.pagarEnUnaSolaExhibicion == 1)
                {
                    numPagos = totalPagosAgui;
                    for (int i = 0; i < filtroMovimientosNominas.Count(); i++)
                    {
                        if (filtroMovimientosNominas[i].concepNomDefi.formulaConcepto.ToUpper().Contains("ImporteAguinaldo".ToUpper()))
                        {
                            pagos.razonesSociales = filtroMovimientosNominas[i].razonesSociales;
                            pagos.empleados = filtroMovimientosNominas[0].empleados;
                            pagos.ejercicio = filtroMovimientosNominas[0].ejercicio;
                            pagos.periodosNomina = filtroMovimientosNominas[0].periodosNomina;
                            pagos.tipoNomina = filtroMovimientosNominas[0].tipoNomina;
                            pagos.diasAguinaldos = Convert.ToDouble(valoresConceptosEmpleados["FactorDiasAguinaldo".ToUpper()]);
                            diaspagados = Convert.ToDouble(valoresConceptosEmpleados["FactorDiasAguinaldo".ToUpper()]);
                            AgunaldoTotal = filtroMovimientosNominas[i].resultado.GetValueOrDefault();
                            for (int j = 0; j < filtroMovimientosNominas[i].movNomBaseAfecta.Count(); j++)
                            {
                                MovNomBaseAfecta mov = filtroMovimientosNominas[i].movNomBaseAfecta[j];
                                if (mov.movNomConcep.concepNomDefi.formulaConcepto.ToUpper().Contains("ImporteAguinaldo".ToUpper()))
                                {
                                    parteExenta = mov.resultadoExento.GetValueOrDefault();
                                }
                            }

                        }
                        else if (filtroMovimientosNominas[i].concepNomDefi.formulaConcepto.ToUpper().Contains("CalculoISR".ToUpper()))
                        {
                            isr = filtroMovimientosNominas[i].resultado.GetValueOrDefault();

                        }
                    }
                    if (aguiConfiguracion.modoCalculo == 0)
                    {//Calcular parte Exenta separado
                        double diasPagarTotal = diaspagados / numPagos;
                        double aguiTotal = AgunaldoTotal / numPagos;
                        double totalExenta = parteExenta / numPagos;
                        double isrtotal = isr / numPagos;
                        pagos.diasPagados = diasPagarTotal;
                        valoresConceptosEmpleados.Add("DiasAguinaldo".ToUpper(), diasPagarTotal);
                        pagos.aguinaldo = aguiTotal;
                        valoresConceptosEmpleados.Add("ImporteAguinaldo".ToUpper(), aguiTotal);
                        pagos.isr = isrtotal;
                        for (int i = 0; i < filtroMovimientosNominas.Count(); i++)
                        {
                            if (filtroMovimientosNominas[i].concepNomDefi.formulaConcepto.ToUpper().Contains("ImporteAguinaldo".ToUpper()))
                            {
                                filtroMovimientosNominas[i].resultado = aguiTotal;
                                for (int j = 0; j < filtroMovimientosNominas[i].movNomBaseAfecta.Count(); j++)
                                {
                                    MovNomBaseAfecta mov = filtroMovimientosNominas[i].movNomBaseAfecta[j];
                                    if (mov.movNomConcep.concepNomDefi.formulaConcepto.ToUpper().Contains("ImporteAguinaldo".ToUpper()))
                                    {
                                        filtroMovimientosNominas[i].movNomBaseAfecta[j].resultadoExento = totalExenta;
                                    }
                                }

                            }
                            else if (filtroMovimientosNominas[i].concepNomDefi.formulaConcepto.ToUpper().Contains("CalculoISR".ToUpper()))
                            {

                                filtroMovimientosNominas[i].resultado = isrtotal;

                            }
                        }
                        getSession().SaveOrUpdate(pagos);
                        for (int i = 0; i < filtroMovimientosNominas.Count(); i++)
                        {
                            if (filtroMovimientosNominas[i].concepNomDefi.formulaConcepto.ToUpper().Equals("ImporteAguinaldo".ToUpper())
                                || filtroMovimientosNominas[i].concepNomDefi.formulaConcepto.ToUpper().Equals("DiasAguinaldo".ToUpper()))
                            {
                                double res = ejecutaFormula(filtroMovimientosNominas[i].concepNomDefi.formulaConcepto);
                                filtroMovimientosNominas[i].resultado = res;
                            }
                            if (filtroMovimientosNominas[i].resultado > 0)
                            {
                                getSession().SaveOrUpdate(filtroMovimientosNominas[i]);
                            }
                        }

                    }
                    else if (aguiConfiguracion.modoCalculo == 1)
                    {//Calcular aguinaldo total y dividir
                        double diasPagarTotal = diaspagados / numPagos;
                        double aguiTotal = AgunaldoTotal / numPagos;
                        double totalExenta = parteExenta / numPagos;
                        double isrtotal = isr / numPagos;
                        pagos.diasPagados = diasPagarTotal;
                        valoresConceptosEmpleados.Add("DiasAguinaldo".ToUpper(), diasPagarTotal);
                        pagos.aguinaldo = aguiTotal;
                        valoresConceptosEmpleados.Add("ImporteAguinaldo".ToUpper(), aguiTotal);
                        pagos.isr = isrtotal;
                        for (int i = 0; i < filtroMovimientosNominas.Count(); i++)
                        {
                            if (filtroMovimientosNominas[i].concepNomDefi.formulaConcepto.ToUpper().Contains("ImporteAguinaldo".ToUpper()))
                            {
                                filtroMovimientosNominas[i].resultado = aguiTotal;
                                for (int j = 0; j < filtroMovimientosNominas[i].movNomBaseAfecta.Count(); j++)
                                {
                                    MovNomBaseAfecta mov = filtroMovimientosNominas[i].movNomBaseAfecta[j];
                                    if (mov.movNomConcep.concepNomDefi.formulaConcepto.ToUpper().Contains("ImporteAguinaldo".ToUpper()))
                                    {
                                        filtroMovimientosNominas[i].movNomBaseAfecta[j].resultadoExento = totalExenta;
                                    }
                                }
                            }
                            else if (filtroMovimientosNominas[i].concepNomDefi.formulaConcepto.ToUpper().Contains("CalculoISR".ToUpper()))
                            {

                                filtroMovimientosNominas[i].resultado = isrtotal;

                            }
                        }
                        getSession().SaveOrUpdate(pagos);
                        for (int i = 0; i < filtroMovimientosNominas.Count(); i++)
                        {
                            if (filtroMovimientosNominas[i].concepNomDefi.formulaConcepto.ToUpper().Equals("ImporteAguinaldo".ToUpper())
                                || filtroMovimientosNominas[i].concepNomDefi.formulaConcepto.ToUpper().Equals("DiasAguinaldo".ToUpper()))
                            {
                                double res = ejecutaFormula(filtroMovimientosNominas[i].concepNomDefi.formulaConcepto);
                                filtroMovimientosNominas[i].resultado = res;
                            }
                            if (filtroMovimientosNominas[i].resultado > 0)
                            {
                                getSession().SaveOrUpdate(filtroMovimientosNominas[i]);
                            }
                        }

                    }
                    else if (aguiConfiguracion.modoCalculo == 2)
                    {
                        //descontar isr en primer pago
                        bool pagarIsr = false;
                        double diasPagarTotal = diaspagados / numPagos;
                        double aguiTotal = AgunaldoTotal / numPagos;
                        double totalExenta = parteExenta / numPagos;
                        pagos.diasPagados = diasPagarTotal;
                        valoresConceptosEmpleados.Add("DiasAguinaldo".ToUpper(), diasPagarTotal);
                        pagos.aguinaldo = aguiTotal;
                        valoresConceptosEmpleados.Add("ImporteAguinaldo".ToUpper(), aguiTotal);
                        DateTime fecper = quitaHrsDeFecha(periodosNomina.fechaInicial.GetValueOrDefault());
                        for (int k = 0; k < aguiFechas.Count(); k++)
                        {
                            DateTime fecha = quitaHrsDeFecha(aguiFechas[k].fechaProgramada.GetValueOrDefault());
                            if (fecha.CompareTo(fecper) < 0)
                            {
                                pagarIsr = true;
                                break;
                            }
                        }
                        if (pagarIsr)
                        {
                            pagos.isr = 0.0;
                        }
                        else
                        {
                            pagos.isr = isr;
                        }
                        for (int i = 0; i < filtroMovimientosNominas.Count(); i++)
                        {
                            if (filtroMovimientosNominas[i].concepNomDefi.formulaConcepto.ToUpper().Contains("ImporteAguinaldo".ToUpper()))
                            {
                                filtroMovimientosNominas[i].resultado = aguiTotal;
                                for (int j = 0; j < filtroMovimientosNominas[i].movNomBaseAfecta.Count(); j++)
                                {
                                    MovNomBaseAfecta mov = filtroMovimientosNominas[i].movNomBaseAfecta[j];
                                    if (mov.movNomConcep.concepNomDefi.formulaConcepto.ToUpper().Contains("ImporteAguinaldo".ToUpper()))
                                    {
                                        filtroMovimientosNominas[i].movNomBaseAfecta[j].resultadoExento = totalExenta;
                                    }
                                }
                            }
                            else if (filtroMovimientosNominas[i].concepNomDefi.formulaConcepto.ToUpper().Contains("CalculoISR".ToUpper()))
                            {
                                if (pagarIsr)
                                {
                                    filtroMovimientosNominas[i].resultado = 0.0;
                                }
                                else
                                {

                                    filtroMovimientosNominas[i].resultado = isr;
                                }


                            }
                        }
                        getSession().SaveOrUpdate(pagos);
                        for (int i = 0; i < filtroMovimientosNominas.Count(); i++)
                        {
                            if (filtroMovimientosNominas[i].concepNomDefi.formulaConcepto.ToUpper().Equals("ImporteAguinaldo".ToUpper())
                                || filtroMovimientosNominas[i].concepNomDefi.formulaConcepto.ToUpper().Equals("DiasAguinaldo".ToUpper()))
                            {
                                double res = ejecutaFormula(filtroMovimientosNominas[i].concepNomDefi.formulaConcepto);
                                filtroMovimientosNominas[i].resultado = res;
                            }
                            if (filtroMovimientosNominas[i].resultado > 0)
                            {
                                getSession().SaveOrUpdate(filtroMovimientosNominas[i]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("calcularAguinaldo()1_Error: ").Append(ex));
                mensajeResultado.noError = 27;
                mensajeResultado.error = ex.GetBaseException().ToString();

            }

        }

        private void ejecutaConceptosPorMovimientoNomina(MovNomConcep filtroMovimientosNominas, String claveTipoCorrida, PlazasPorEmpleadosMov plazasPorEmpleadosMov, int posicionPlazaPorEmpleadoMovEjecutandose, List<CalculoUnidades> listCalculoUnidadesTmp, bool activarSave)
        {
            try
            {
                if (filtroMovimientosNominas != null)
                {
                    int j, i, k, indicePlazasPorEmpleadoMov;
                    MovNomConcep movimientosNomina;
                    List<PlazasPorEmpleadosMov> listPlazasPorEmpleadosMovOficial = new List<PlazasPorEmpleadosMov>();
                    List<MovNomConcep> movimientosNominaAux = new List<MovNomConcep>();
                    bool cargoSueldoDiarioX = false, guardarCambiosCalculosUnidades = true, tempPagoDiasNat, b = false;
                    //for (j = 0; j < filtroMovimientosNominas.Count; j++)
                    //{
                    movimientosNominaAux.Add(filtroMovimientosNominas);
                    movimientosNomina = filtroMovimientosNominas;
                    if (movimientosNomina.concepNomDefi.formulaConcepto == null)
                    {
                        movimientosNomina.concepNomDefi.formulaConcepto = "";
                    }
                    listPlazasPorEmpleadosMovOficial.Clear();
                    listPlazasPorEmpleadosMovOficial.Add(plazasPorEmpleadosMov);
                    bool configuracionSueldoDiarioVigente = false, configuracionPercepcion_Plaza = false, configuracionPercepcion_Plaza_Vigente = false;
                    #region Esta programacion es especial ya que es solo para cuando vienen las variables:SueldoDiarioVigente,percep_plaza_vigente,SueldoDiarioInicial,percep_plaza y SueldoDiarioFinal (este ultimo no tiene ya que desde el metodo obtenerPlazasPorEmpleados ya viene con el movimiento maximo en el periodo)
                    if (movimientosNomina.concepNomDefi.formulaConcepto.ToUpper().Contains("SueldoDiarioVigente".ToUpper()))
                    {
                        configuracionSueldoDiarioVigente = true;
                    }
                    else if (movimientosNomina.concepNomDefi.formulaConcepto.ToUpper().Contains("SueldoDiarioInicial".ToUpper()) & !cargoSueldoDiarioX)
                    {//Te busca el minimo movimiento de la plaza por empleado que tuvo dentro de periodo.
                        List<PlazasPorEmpleadosMov> listPromocionesDentroPeriodo = obtenerMinimoPlazasPorEmpleadosMovDentroPeriodo(tipoCorrida.clave, periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault(), plazasPorEmpleadosMov);
                        if (mensajeResultado.noError != 0)
                        {
                            //break;
                        }
                        cargoSueldoDiarioX = true;
                        if (listPromocionesDentroPeriodo == null ? false : listPromocionesDentroPeriodo.Count > 0)
                        {
                            cargaDatosSalarioDiario(listPromocionesDentroPeriodo[0], periodosNomina);
                        }
                        else
                        {
                            cargaDatosSalarioDiario(listPromocionesDentroPeriodo[0], periodosNomina);
                        }
                    }
                    else if (movimientosNomina.concepNomDefi.formulaConcepto.ToUpper().Contains("percep_plaza_vigente".ToUpper()))
                    {
                        configuracionPercepcion_Plaza_Vigente = true;
                    }
                    else if (movimientosNomina.concepNomDefi.formulaConcepto.ToUpper().Contains("percep_plaza".ToUpper()))
                    {
                        configuracionPercepcion_Plaza = true;
                    }
                    #endregion
                    List<MovNomConcep> listMovNomConcepPromocional = new List<MovNomConcep>();
                    if (configuracionPercepcion_Plaza | configuracionPercepcion_Plaza_Vigente | configuracionSueldoDiarioVigente)
                    {
                        List<Object> tmp = obtenerModificacionesDePlazasPorEmpleadoMov(configuracionSueldoDiarioVigente, configuracionPercepcion_Plaza, configuracionPercepcion_Plaza_Vigente, movimientosNomina, plazasPorEmpleadosMov);
                        listPlazasPorEmpleadosMovOficial = (List<PlazasPorEmpleadosMov>)tmp[0];
                        listMovNomConcepPromocional = (List<MovNomConcep>)tmp[1];
                        if (listPlazasPorEmpleadosMovOficial.Count > listCalculoUnidadesTmp.Count)
                        {
                            for (k = listCalculoUnidadesTmp.Count; k < listPlazasPorEmpleadosMovOficial.Count; k++)
                            {
                                CalculoUnidades calculoUnidades = crearCalculoUnidades(listMovNomConcepPromocional[k].plazasPorEmpleado);
                                calculoUnidades.mes = listMovNomConcepPromocional[k].mes;
                                calculoUnidades.numero = listMovNomConcepPromocional[k].numero;
                                calculoUnidades.numMovParticion = listMovNomConcepPromocional[k].numMovParticion;
                                calculoUnidades.ejercicio = listMovNomConcepPromocional[k].ejercicio;
                                listCalculoUnidadesTmp.Add(calculoUnidades);
                            }
                        }
                        else if (listPlazasPorEmpleadosMovOficial.Count < listCalculoUnidadesTmp.Count & listPlazasPorEmpleadosMovOficial.Count > 1)
                        {
                            while (listCalculoUnidadesTmp.Count > listMovNomConcepPromocional.Count)
                            {
                                getSession().Delete(listCalculoUnidadesTmp[listCalculoUnidadesTmp.Count - 1]);
                                listCalculoUnidadesTmp.RemoveAt(listCalculoUnidadesTmp.Count - 1);
                            }
                            //                            System.out.println("flush 5");
                            getSession().Flush();
                            getSession().Clear();
                        }
                        if (listPlazasPorEmpleadosMovOficial.Count > 1)
                        {
                            for (int l = 0; l < listMovNomConcepPromocional.Count; l++)
                            {
                                listCalculoUnidadesTmp[l].mes = listMovNomConcepPromocional[l].mes;
                                listCalculoUnidadesTmp[l].ejercicio = listMovNomConcepPromocional[l].ejercicio;
                                listCalculoUnidadesTmp[l].numMovParticion = listMovNomConcepPromocional[l].numMovParticion;
                                listCalculoUnidadesTmp[l].numero = listMovNomConcepPromocional[l].numero;
                                listCalculoUnidadesTmp[l].uso = listMovNomConcepPromocional[l].uso;
                            }
                        }
                    }// end (configuracionPercepcion_Plaza | configuracionPercepcion_Plaza_Vigente | configuracionSueldoDiarioVigente)
                    else
                    {
                        listMovNomConcepPromocional.Add(movimientosNomina);
                    }
                    indicePlazasPorEmpleadoMov = 0;
                    bool aumentarIndicePlazasPorEmpleadoMov = true;
                    for (i = 0; i < listMovNomConcepPromocional.Count; i++)
                    {
                        movimientosNomina = listMovNomConcepPromocional[i];
                        #region validacion de Bases afecta y parametros, esto se agrego por los movimientos que ya existen 
                        if ((movimientosNomina.concepNomDefi.baseAfecConcepNom == null ? 0 : movimientosNomina.concepNomDefi.baseAfecConcepNom.Count)
                               != (movimientosNomina.movNomBaseAfecta == null ? 0 : movimientosNomina.movNomBaseAfecta.Count))
                        {
                            movimientosNomina.movNomBaseAfecta = creaMovimBaseAfectar(movimientosNomina.concepNomDefi.baseAfecConcepNom, movimientosNomina);

                        }

                        if ((movimientosNomina.concepNomDefi.paraConcepDeNom == null ? 0 : movimientosNomina.concepNomDefi.paraConcepDeNom.Count)
                                != (movimientosNomina.movNomConceParam == null ? 0 : movimientosNomina.movNomConceParam.Count))
                        {
                            movimientosNomina.movNomConceParam = creaMovNomConceParam(movimientosNomina.concepNomDefi, movimientosNomina);

                        }
                        #endregion
                        isMov2Meses = false;
                        tempPagoDiasNat = manejaPagoDiasNaturales;    // respalda parametro pago dias naturales
                        if (evaluaPeriodoMovAbarca2Meses(movimientosNomina.periodosNomina))
                        {
                            isMov2Meses = true;
                            manejaPagoDiasNaturales = true;
                        }
                        if (listMovNomConcepPromocional.Count > 1)
                        {
                            guardarCambiosCalculosUnidades = false;
                            #region programacion para cuando existan modificaciones salariales
                            if (i > 0)
                            {
                                movimientosNomina.numero = i + 1;
                                List<String> keys = valoresConceptosEmpleados.Keys.ToList();
                                TipoClasificacionFormula tfc;
                                foreach (String key in keys)
                                {
                                    tfc = (TipoClasificacionFormula)Entity.util.ManejadorEnum.GetValue(propertieFuente.GetProperty(String.Concat(key, "_TipoDato").ToUpper()), typeof(TipoClasificacionFormula));
                                    if (tfc == TipoClasificacionFormula.DATOCALCULO)
                                    {
                                        valoresConceptosEmpleados.Add(key, "");
                                    }
                                }
                            }
                            if (configuracionPercepcion_Plaza || configuracionPercepcion_Plaza_Vigente)
                            {
                                if (indicePlazasPorEmpleadoMov + 1 <= listPlazasPorEmpleadosMovOficial.Count - 1)
                                {
                                    if (String.Equals(listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov + 1].plazasPorEmpleado.referencia, listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov].plazasPorEmpleado.referencia, StringComparison.OrdinalIgnoreCase))
                                    {
                                        cargaValoresDiasPago(listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov], false, listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov + 1], listCalculoUnidadesTmp[i], false, true);
                                        if (listCalculoUnidadesTmp[i].id == 0)
                                        {
                                            getSession().SaveOrUpdate(listCalculoUnidadesTmp[i]);
                                        }
                                        else
                                        {
                                            getSession().Merge(listCalculoUnidadesTmp[i]);
                                        }
                                    }
                                    else
                                    {
                                        cargaValoresDiasPago(listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov], false, null, listCalculoUnidadesTmp[i], false, true);//JSA30

                                        if (listCalculoUnidadesTmp[i].id == 0)
                                        {
                                            getSession().SaveOrUpdate(listCalculoUnidadesTmp[i]);
                                        }
                                        else
                                        {
                                            getSession().Merge(listCalculoUnidadesTmp[i]);
                                        }
                                    }
                                }
                                else
                                {
                                    cargaValoresDiasPago(listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov], true, null, listCalculoUnidadesTmp[i], false, true);//JSA30
                                    if (listCalculoUnidadesTmp[i].id == 0)
                                    {
                                        getSession().SaveOrUpdate(listCalculoUnidadesTmp[i]);
                                    }
                                    else
                                    {
                                        getSession().Merge(listCalculoUnidadesTmp[i]);
                                    }
                                }
                            }
                            else if (isMov2Meses)
                            {
                                DateTime fechaInicio = DateTime.Now, fechaFinal = DateTime.Now;
                                if (movimientosNomina.numMovParticion == 1)
                                {
                                    if (indicePlazasPorEmpleadoMov == 0)
                                    {
                                        fechaInicio = periodosNomina.fechaInicial.GetValueOrDefault();
                                    }
                                    else
                                    {
                                        fechaInicio = listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov].fechaInicial.GetValueOrDefault();
                                    }
                                    if (listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov].fechaInicial.GetValueOrDefault().CompareTo(periodosNomina.fechaInicial) < 0)
                                    {
                                        fechaFinal = listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov + 1].fechaInicial.GetValueOrDefault();
                                        fechaFinal.AddDays(fechaFinal.Day - 1);
                                        //fechaFinal.set(Calendar.DATE, fechaFinal.get(Calendar.DATE) - 1);
                                    }
                                    else if (indicePlazasPorEmpleadoMov + 1 < listPlazasPorEmpleadosMovOficial.Count)
                                    {
                                        fechaFinal = listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov + 1].fechaInicial.GetValueOrDefault();
                                        DateTime c;
                                        c = periodosNomina.fechaFinal.GetValueOrDefault();
                                        if (fechaFinal.Month == c.Month)
                                        {
                                            fechaFinal = listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov].fechaInicial.GetValueOrDefault();
                                            fechaFinal.AddDays(DateTime.DaysInMonth(fechaFinal.Year, fechaFinal.Month));
                                        }
                                        else
                                        {
                                            fechaFinal = listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov + 1].fechaInicial.GetValueOrDefault();
                                            fechaFinal.AddDays(fechaFinal.Day - 1);
                                            //fechaFinal.set(Calendar.DATE, fechaFinal.get(Calendar.DATE) - 1);
                                        }
                                    }
                                    else if (indicePlazasPorEmpleadoMov + 1 == listPlazasPorEmpleadosMovOficial.Count)
                                    {
                                        fechaFinal = periodosNomina.fechaFinal.GetValueOrDefault();
                                    }
                                }
                                else
                                {
                                    if (i == 0 ? false : listMovNomConcepPromocional[i].mes != listMovNomConcepPromocional[i - 1].mes)
                                    {
                                        fechaInicio = listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov].fechaInicial.GetValueOrDefault();
                                        fechaInicio.AddDays(DateTime.DaysInMonth(fechaInicio.Year, fechaInicio.Month));
                                        if (fechaInicio.CompareTo(listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov].fechaInicial) > 0)
                                        {
                                            fechaInicio = listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov].fechaInicial.GetValueOrDefault();
                                        }
                                        else
                                        {
                                            aumentarIndicePlazasPorEmpleadoMov = false;
                                        }
                                    }
                                    else
                                    {
                                        fechaInicio = listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov].fechaInicial.GetValueOrDefault();
                                    }

                                    if (fechaInicio.CompareTo(listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov].fechaInicial) < 0)
                                    {
                                        fechaFinal = listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov].fechaInicial.GetValueOrDefault();
                                        fechaFinal.AddDays(fechaFinal.Day - 1);
                                        //fechaFinal.set(Calendar.DATE, fechaFinal.get(Calendar.DATE) - 1);
                                    }
                                    else if (indicePlazasPorEmpleadoMov + 1 < listPlazasPorEmpleadosMovOficial.Count)
                                    {
                                        fechaFinal = listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov + 1].fechaInicial.GetValueOrDefault();
                                        fechaFinal.AddDays(fechaFinal.Day - 1);
                                        //fechaFinal.set(Calendar.DATE, fechaFinal.get(Calendar.DATE) - 1);
                                    }
                                    else
                                    {
                                        fechaFinal = periodosNomina.fechaFinal.GetValueOrDefault();
                                    }
                                }
                                inicializaPeriodo2Meses(periodosNomina, fechaInicio, fechaFinal);
                                valoresConceptosGlobales[parametroFechaFinal] = fechaFinal;
                                valoresConceptosGlobales[parametroFechaInicial] = fechaInicio;
                                foreach (var item in valoresConceptosGlobales)
                                {
                                    valoresConceptosEmpleados[item.Key] = item.Value;
                                }
                                // valoresConceptosEmpleados = valoresConceptosEmpleados.Concat(valoresConceptosGlobales).ToDictionary(e => e.Key, e => e.Value);
                                cargaValoresDiasPago(listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov], indicePlazasPorEmpleadoMov < 1,
                                        null, listCalculoUnidadesTmp[i], i == 0 ? false : listMovNomConcepPromocional[i].mes != listMovNomConcepPromocional[i - 1].mes, true);//JSA30
                                cargaValoresDiasCotizados(listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov].fechaIMSS.GetValueOrDefault(), listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov].plazasPorEmpleado.fechaFinal.GetValueOrDefault(),
                                    (listPlazasPorEmpleadosMovOficial.Count <= 1), null, null, i == 0 ? false : listMovNomConcepPromocional[i].mes != listMovNomConcepPromocional[i - 1].mes, false);//JSA30
                                cargaDatosVariableConfiguracionIMSS((DateTime)valoresConceptosGlobales[parametroFechaFinal]);
                                if (listCalculoUnidadesTmp[i].id == 0)
                                {
                                    getSession().SaveOrUpdate(listCalculoUnidadesTmp[i]);
                                }
                                else
                                {
                                    getSession().Merge(listCalculoUnidadesTmp[i]);
                                }

                            }
                            else
                            {
                                cargaValoresDiasPago(listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov], false, i + 1 < listPlazasPorEmpleadosMovOficial.Count ? listPlazasPorEmpleadosMovOficial[i + 1] : null, listCalculoUnidadesTmp[i], false, true);//JSA30
                                if (listCalculoUnidadesTmp[i].id == 0)
                                {
                                    getSession().SaveOrUpdate(listCalculoUnidadesTmp[i]);
                                }
                                else
                                {
                                    getSession().Merge(listCalculoUnidadesTmp[i]);
                                }
                            }

                            if (configuracionSueldoDiarioVigente)
                            {
                                cargaDatosSalarioDiario(indicePlazasPorEmpleadoMov < listPlazasPorEmpleadosMovOficial.Count ? listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov] : listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov - 1], periodosNomina);
                            }
                            else if (configuracionPercepcion_Plaza)
                            {
                                cargaDatosSalarioDiario(indicePlazasPorEmpleadoMov < listPlazasPorEmpleadosMovOficial.Count ? listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov] : listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov - 1], periodosNomina);
                            }
                            else
                            {
                                cargaDatosSalarioDiario(indicePlazasPorEmpleadoMov < listPlazasPorEmpleadosMovOficial.Count ? listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov] : listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov - 1], periodosNomina);
                            }

                            #endregion
                        }
                        else if (configuracionSueldoDiarioVigente | configuracionPercepcion_Plaza_Vigente)
                        {
                            if (configuracionSueldoDiarioVigente)
                            {
                                cargaDatosSalarioDiario(listPlazasPorEmpleadosMovOficial[0], periodosNomina);
                            }
                            else
                            {
                                cargaDatosSalarioDiario(listPlazasPorEmpleadosMovOficial[0], periodosNomina);
                            }
                        }
                        if (configuracionPercepcion_Plaza_Vigente | configuracionPercepcion_Plaza)
                        {
                            movimientosNomina.plazasPorEmpleado = listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov].plazasPorEmpleado;
                        }
                        #region Esta programacion es para saber si el periodo abarco 2 meses
                        if (isMov2Meses & listPlazasPorEmpleadosMovOficial.Count == 1)
                        {
                            if (!configuracionSueldoDiarioVigente || !configuracionPercepcion_Plaza || !configuracionPercepcion_Plaza_Vigente)
                            {
                                guardarCambiosCalculosUnidades = false;
                            }
                            DateTime cFecha;
                            int indiceCalculoUnidad;
                            if (movimientosNomina.numMovParticion == 1)
                            {
                                cFecha = movimientosNomina.periodosNomina.fechaInicial.GetValueOrDefault();
                                int dia = DateTime.DaysInMonth(cFecha.Year, cFecha.Month);
                                DateTime fec = new DateTime(cFecha.Year, cFecha.Month, dia);
                                //  cFecha.Date.AddDays(5.0);
                                inicializaPeriodo2Meses(periodosNomina, periodosNomina.fechaInicial.GetValueOrDefault(), fec);
                                valoresConceptosGlobales[parametroFechaFinal] = fec;
                                valoresConceptosGlobales[parametroFechaInicial] = periodosNomina.fechaInicial;
                            }
                            else
                            {
                                cFecha = movimientosNomina.periodosNomina.fechaFinal.GetValueOrDefault();
                                DateTime fec2 = new DateTime(cFecha.Year, cFecha.Month, 1);

                                inicializaPeriodo2Meses(periodosNomina, fec2, periodosNomina.fechaFinal.GetValueOrDefault());
                                valoresConceptosGlobales[parametroFechaInicial] = fec2;
                                valoresConceptosGlobales[parametroFechaFinal] = periodosNomina.fechaFinal;
                            }
                            indiceCalculoUnidad = obtenerPosicionCalculoUnidades(listCalculoUnidadesTmp, movimientosNomina);
                            foreach (var item in valoresConceptosGlobales)
                            {
                                valoresConceptosEmpleados[item.Key] = item.Value;
                            }
                            //valoresConceptosEmpleados = valoresConceptosEmpleados.Concat(valoresConceptosGlobales).ToDictionary(e => e.Key, e => e.Value);
                            ///valoresConceptosEmpleados.putAll(valoresConceptosGlobales);
                            cargaValoresDiasPago(plazasPorEmpleadosMov, (listPlazasPorEmpleadosMovOficial.Count <= 1), null, listCalculoUnidadesTmp[indiceCalculoUnidad], false, true);//JSA30
                            cargaValoresDiasCotizados(plazasPorEmpleadosMov.fechaIMSS.GetValueOrDefault(), plazasPorEmpleadosMov.plazasPorEmpleado.fechaFinal.GetValueOrDefault(), (listPlazasPorEmpleadosMovOficial.Count <= 1), null, null, false, false);//JSA30
                            cargaDatosVariableConfiguracionIMSS(((DateTime)valoresConceptosGlobales[parametroFechaFinal]));
                            if (guardarCambiosCalculosUnidades)
                            {
                                if (listCalculoUnidadesTmp[indiceCalculoUnidad].id == 0)
                                {
                                    getSession().SaveOrUpdate(listCalculoUnidadesTmp[indiceCalculoUnidad]);
                                }
                                else
                                {
                                    getSession().Merge(listCalculoUnidadesTmp[indiceCalculoUnidad]);
                                }
                            }
                        }
                        else
                        {
                            manejaPagoDiasNaturales = tempPagoDiasNat;
                        }
                        #endregion
                        operacionConceptos(movimientosNomina, claveTipoCorrida, plazasPorEmpleadosMov, activarSave);
                        if (mensajeResultado.noError != 0)
                        {
                            break;
                        }
                        if (b)
                        {
                            if (listCalculoUnidadesTmp[i].id == 0)
                            {
                                getSession().SaveOrUpdate(listCalculoUnidadesTmp[i]);
                            }
                            else
                            {
                                getSession().Merge(listCalculoUnidadesTmp[i]);
                            }
                            break;
                        }
                        if (aumentarIndicePlazasPorEmpleadoMov)
                        {
                            indicePlazasPorEmpleadoMov++;
                        }
                        else
                        {
                            aumentarIndicePlazasPorEmpleadoMov = true;
                        }
                    }/// end for listMovNomConcepPromocional.Count

                    if (listMovNomConcepPromocional.Count > 1)
                    {
                        cargaValoresDiasPago(plazasPorEmpleadosMov, true, null, null, false, true);//JSA30
                        cargaValoresDiasCotizados(plazasPorEmpleadosMov.fechaIMSS.GetValueOrDefault(), plazasPorEmpleadosMov.plazasPorEmpleado.fechaFinal.GetValueOrDefault(), true, null, null, false, false);//JSA30
                        if (isMov2Meses)
                        {
                            cargaDatosVariableConfiguracionIMSS(plazasPorEmpleadosMov.fechaIMSS.GetValueOrDefault());
                        }
                    }
                    i = 0;
                    //  } // end for filtroMovimientosNominas.Count
                    if (mensajeResultado.noError != 0)
                    {
                        return;
                    }

                } // end filtroMovimientosNominas != null
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ejecutaConceptosPorMovimientoNomina()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
        }

        private void operacionConceptos(MovNomConcep movimientosNomina, string claveTipoCorrida, PlazasPorEmpleadosMov plazasPorEmpleadoMov, bool activarSave)
        {
            try
            {
                bool isISR = false, isIMSS = false, isISRSubsidio = false, isImssPatronal = false, omitirMovimiento = false, isISRACargo = false, isSubsEmpleoCalculado = false;
                isISR = isConceptoEspecial(1, movimientosNomina.concepNomDefi.formulaConcepto);
                isIMSS = isConceptoEspecial(2, movimientosNomina.concepNomDefi.formulaConcepto);
                isISRSubsidio = isConceptoEspecial(3, movimientosNomina.concepNomDefi.formulaConcepto);
                isImssPatronal = isConceptoEspecial(4, movimientosNomina.concepNomDefi.formulaConcepto);
                isISRACargo = isConceptoEspecial(5, movimientosNomina.concepNomDefi.formulaConcepto);
                isSubsEmpleoCalculado = isConceptoEspecial(6, movimientosNomina.concepNomDefi.formulaConcepto);
                if (isImssPatronal)
                {
                    isIMSS = false;
                }

                if (isISR)
                {
                    #region Calculo ISR
                    isISR = false;
                    Double valorISR;
                    MovNomConcep movNomConcepSubsidio = null;
                    if (isMov2Meses & listMovNomConcepSubsidio.Count > 0)
                    {
                        int pos = movimientosNomina.numMovParticion == 1 ? 0 : 1;
                        movNomConcepSubsidio = listMovNomConcepSubsidio[pos];
                    }
                    else if (listMovNomConcepSubsidio.Count > 0)
                    {
                        movNomConcepSubsidio = listMovNomConcepSubsidio[0];
                    }
                    if (movNomConcepSubsidio == null)
                    {
                        camposParametro = new List<String>();
                        valoresParametro = new List<Object>();
                        strQuery.Remove(0, strQuery.Length).Append("SELECT cdn FROM ConcepNomDefi cdn INNER JOIN cdn.conceptoPorTipoCorrida ctc INNER JOIN ctc.tipoCorrida tc  ");
                        strWhere.Remove(0, strWhere.Length).Append(" WHERE tc.clave = :claveTipoCorrida AND cdn.activado = true and cdn.formulaConcepto LIKE CONCAT('%', :formulaConcepto, '%') ");
                        strWhere.Append(" AND cdn.fecha =(SELECT MAX(fecha) FROM ConcepNomDefi c WHERE c.formulaConcepto LIKE CONCAT('%', :formulaConcepto, '%')) ");
                        strQuery.Append(strWhere);
                        camposParametro.Add("claveTipoCorrida");
                        valoresParametro.Add(claveTipoCorrida);
                        camposParametro.Add("formulaConcepto");
                        valoresParametro.Add("ISRSubsidio");
                        ConcepNomDefi con = (ConcepNomDefi)ejecutaQueryUnico(strQuery.ToString(), camposParametro.ToArray<string>(), valoresParametro.ToArray());

                        bool asignaSubsidio = true;
                        if (String.Equals(claveTipoCorrida, "PTU", StringComparison.OrdinalIgnoreCase) && con == null)
                        {
                            asignaSubsidio = false;
                        }
                        if (asignaSubsidio)
                        {
                            if (con == null)
                            {
                                mensajeResultado.error = "Concepto Subsidio no encontrado en la corrida " + claveTipoCorrida;
                                mensajeResultado.noError = 27;
                                return;
                            }
                            else
                            {
                                movNomConcepSubsidio = creaMovNomConceptoSubsidio(movimientosNomina, con);

                            }
                        }

                    }
                    if (String.Equals(claveTipoCorrida, "FIN", StringComparison.OrdinalIgnoreCase) | String.Equals(claveTipoCorrida, "LIQ", StringComparison.OrdinalIgnoreCase))
                    {
                        valorISR = calculoISRFiniquitos(movimientosNomina.tipoCorrida);
                        if (mensajeResultado.noError != 0)
                        {
                            return;
                        }
                        if (valorISR > 0)
                        {
                            movimientosNomina.resultado = aplicarMascara(movimientosNomina.concepNomDefi, valorISR, false);
                        }
                        else
                        {
                            movimientosNomina.resultado = 0.0;
                        }
                        movimientosNomina.calculado = valorISR > 0 ? valorISR : 0.0;
                        IsrRetenidos(movimientosNomina);
                    }
                    else if (modoAjustarIngresosMes == ProporcionaTablaAnual)
                    {
                        valorISR = calculoISPTAnual(movimientosNomina);
                    }
                    else if (calculoSeparadoISR)
                    {
                        valorISR = calculaISPTSeparado(movimientosNomina);
                    }
                    else
                    {
                        valorISR = calculaISPT(movimientosNomina);
                    }

                    if (mensajeResultado.noError != 0)
                    {
                        return;
                    }
                    if (valorISR > 0)
                    {
                        movimientosNomina.resultado = aplicarMascara(movimientosNomina.concepNomDefi, valorISR, false);
                        if (movNomConcepSubsidio != null)
                        {
                            movNomConcepSubsidio.resultado = 0.0;
                        }
                    }
                    else
                    {
                        movimientosNomina.resultado = 0.0;
                        if (movNomConcepSubsidio != null)
                        {
                            movNomConcepSubsidio = ejecutarParametroCondicionYFormula(movNomConcepSubsidio, claveTipoCorrida);
                            agregaParametrosConceptosNomina(movNomConcepSubsidio.movNomConceParam);
                        }
                    }
                    movimientosNomina.calculado = valorISR > 0 ? valorISR : 0.0;
                    agregaParametrosConceptosNomina(movimientosNomina.movNomConceParam);
                    if (movNomConcepSubsidio != null)
                    {
                        if (movNomConcepSubsidio.resultado == null ? true : movNomConcepSubsidio.resultado == 0 ? true : false)
                        {
                            if (movNomConcepSubsidio.id > 0)
                            {
                                eliminarMovimientosNominaBasura(new Object[] { movNomConcepSubsidio.id });

                                getSession().Flush();
                                iSRRetenidoSubsidio = null;
                            }
                        }
                        else
                        {
                            //Se guarda el concepto Subsidio..El del ISR se guarda abajo.
                            if (movNomConcepSubsidio.id == 0)
                            {
                                cantidadSaveUpdate++;
                                getSession().SaveOrUpdate(movNomConcepSubsidio);
                            }
                            else
                            {
                                getSession().Merge(movNomConcepSubsidio);
                            }
                            iSRRetenidoSubsidio.movNomConcep = movNomConcepSubsidio;
                            getSession().SaveOrUpdate(iSRRetenidoSubsidio);
                            getSession().Flush();
                            getSession().Clear();
                            cantidadSaveUpdate++;
                        }
                        movNomConcepSubsidio = null;
                        iSRRetenidoSubsidio = null;
                    }

                    if (listMovNomConcepISRCARGO != null)
                    {
                        if (isMov2Meses && listMovNomConcepISRCARGO.Count > 0)
                        {
                            int pos = movimientosNomina.numMovParticion == 1 ? 0 : 1;
                            movNomConcepSubsidio = listMovNomConcepISRCARGO[pos];
                        }
                        else if (listMovNomConcepISRCARGO.Count > 0)
                        {
                            movNomConcepSubsidio = listMovNomConcepISRCARGO[0];

                        }
                        if (movNomConcepSubsidio != null)
                        {
                            movNomConcepSubsidio = ejecutarParametroCondicionYFormula(movNomConcepSubsidio, claveTipoCorrida);
                            saveOrUpdateOrDeleteMovimientosNomina(movNomConcepSubsidio, false, isISRSubsidio);
                            if (mensajeResultado.noError == -101)
                            {
                                mensajeResultado.noError = 54;
                                return;
                            }
                            movNomConcepSubsidio = null;
                        }
                    }
                    if (listMovNomConcepSUBSIDIOALEMPLEO != null)
                    {
                        if (isMov2Meses & listMovNomConcepSUBSIDIOALEMPLEO.Count > 0)
                        {
                            int pos = movimientosNomina.numMovParticion == 1 ? 0 : 1;
                            movNomConcepSubsidio = listMovNomConcepSUBSIDIOALEMPLEO[pos];

                        }
                        else if (listMovNomConcepSUBSIDIOALEMPLEO.Count > 0)
                        {
                            movNomConcepSubsidio = listMovNomConcepSUBSIDIOALEMPLEO[0];

                        }
                        if (movNomConcepSubsidio != null)
                        {
                            movNomConcepSubsidio = ejecutarParametroCondicionYFormula(movNomConcepSubsidio, claveTipoCorrida);
                            saveOrUpdateOrDeleteMovimientosNomina(movNomConcepSubsidio, false, isISRSubsidio);
                            if (mensajeResultado.noError == -101)
                            {
                                mensajeResultado.noError = 54;
                                return;
                            }
                            movNomConcepSubsidio = null;
                        }

                    }
                    if (movimientosNomina.resultado == null ? true : movimientosNomina.resultado == 0 ? true : false)
                    {
                        if (movimientosNomina.id > 0)
                        {
                            eliminarMovimientosNominaBasura(new Object[] { movimientosNomina.id });

                        }
                    }
                    else
                    {
                        if (movimientosNomina.id == 0)
                        {
                            cantidadSaveUpdate++;
                            getSession().SaveOrUpdate(movimientosNomina);
                        }
                        iSRRetenido.movNomConcep = movimientosNomina;
                        cantidadSaveUpdate++;
                        getSession().SaveOrUpdate(iSRRetenido);
                        iSRRetenido = null;
                        omitirMovimiento = true;

                    }
                    if (mensajeResultado.noError == -101)
                    {
                        mensajeResultado.noError = 54;
                        return;
                    }

                    #endregion
                }
                else if (isIMSS)
                {
                    #region Calculo IMSS
                    //calcula seguro social calculaSalarioDiarioIntegerado(); 100 es el valor diario integrado
                    isIMSS = false;
                    agregaParametrosConceptosNomina(movimientosNomina.movNomConceParam);
                    double resultadoImss = calculaImss(Convert.ToDouble(valoresConceptosEmpleados["SueldoIntIMSS".ToUpper()]), Convert.ToDouble(valoresConceptosEmpleados["SalarioMinDF".ToUpper()]), movimientosNomina, plazasPorEmpleadoMov);
                    movimientosNomina.resultado = aplicarMascara(movimientosNomina.concepNomDefi, resultadoImss, false);
                    movimientosNomina.calculado = resultadoImss;
                    if (movimientosNomina.id == 0)
                    {
                        cantidadSaveUpdate++;
                        getSession().SaveOrUpdate(movimientosNomina);
                    }
                    for (int i = 0; i < listCalculoIMSS.Count; i++)
                    {
                        listCalculoIMSS[i].movNomConcep = movimientosNomina;
                        cantidadSaveUpdate++;
                        getSession().SaveOrUpdate(listCalculoIMSS[i]);
                    }

                    omitirMovimiento = true;
                    if (movimientosNomina.resultado == null ? true : movimientosNomina.resultado == 0 ? true : false)
                    {
                        if (movimientosNomina.id > 0)
                        {
                            ejecutaQueryExecuteUpdate("Delete CalculoIMSS mov Where mov.movNomConcep.id = :id", new String[] { "id" }, new Object[] { movimientosNomina.id });
                            getSession().Flush();
                        }
                    }

                    #endregion
                }
                else if (isImssPatronal)
                {
                    #region Calculo IMSSPatronal
                    agregaParametrosConceptosNomina(movimientosNomina.movNomConceParam);
                    double resultadoImssPat = calculaImssPatronal(Convert.ToDouble(valoresConceptosEmpleados["SueldoIntIMSS".ToUpper()]), Convert.ToDouble(valoresConceptosEmpleados["SalarioMinDF".ToUpper()]), movimientosNomina, plazasPorEmpleadoMov);
                    movimientosNomina.resultado = resultadoImssPat;
                    if (movimientosNomina.id == 0)
                    {
                        cantidadSaveUpdate++;
                        getSession().SaveOrUpdate(movimientosNomina);
                    }
                    calculoIMSSPatron.movNomConcep = movimientosNomina;
                    cantidadSaveUpdate++;
                    getSession().SaveOrUpdate(calculoIMSSPatron);
                    omitirMovimiento = true;
                    if (movimientosNomina.resultado == null ? true : movimientosNomina.resultado == 0 ? true : false)
                    {
                        if (movimientosNomina.id > 0)
                        {
                            ejecutaQueryExecuteUpdate("Delete CalculoIMSSPatron mov Where mov.movNomConcep.id = :id", new String[] { "id" }, new Object[] { movimientosNomina.id });
                            getSession().Flush();
                        }
                    }
                    #endregion
                }
                else if (isISRSubsidio)
                {
                    listMovNomConcepSubsidio.Add(movimientosNomina);
                    omitirMovimiento = true;
                }
                else if (isISRACargo)
                {
                    listMovNomConcepISRCARGO.Add(movimientosNomina);
                    omitirMovimiento = true;
                }
                else if (isSubsEmpleoCalculado)
                {
                    listMovNomConcepSUBSIDIOALEMPLEO.Add(movimientosNomina);
                    omitirMovimiento = true;
                }
                else if (movimientosNomina.concepNomDefi.formulaConcepto.ToUpper().Contains("AjustePorRedondeo".ToUpper()))
                {
                    movNomConcepAjustePorRedondeo = movimientosNomina;
                    omitirMovimiento = true;
                }
                else
                {
                    movimientosNomina = ejecutarParametroCondicionYFormula(movimientosNomina, claveTipoCorrida);
                }

                if (mensajeResultado.noError != 0)
                {
                    return;
                }
                if (movimientosNomina.resultado == null)
                {
                    movimientosNomina.resultado = 0.0;
                    movimientosNomina.calculado = 0.0;
                }
                if (activarSave)
                {
                    saveOrUpdateOrDeleteMovimientosNomina(movimientosNomina, omitirMovimiento, isISRSubsidio);
                    movNomConcepGlobal = movimientosNomina;
                }
                else
                {
                    movNomConcepGlobal = movimientosNomina;
                }

                omitirMovimiento = false;
                if (mensajeResultado.noError == -101)
                {
                    mensajeResultado.noError = 54;
                    return;
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("operacionConceptos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                //transacion.Rollback();
            }
        }

        private MovNomConcep ejecutarParametroCondicionYFormula(MovNomConcep movimientosNomina, String claveTipoCorrida)
        {
            bool condicion;
            try
            {
                agregaParametrosConceptosNomina(movimientosNomina.movNomConceParam);
                if (movimientosNomina.concepNomDefi.condicionConcepto.Length == 0)
                {
                    condicion = true;
                }
                else
                {
                    condicion = ejecutaFormula(movimientosNomina.concepNomDefi.condicionConcepto) == 0.0 ? false : true;
                }
                if (condicion)
                {
                    double resultado;
                    if (string.Equals(claveTipoCorrida, "FIN", StringComparison.OrdinalIgnoreCase) | string.Equals(claveTipoCorrida, "LIQ", StringComparison.OrdinalIgnoreCase))
                    {
                        resultado = movimientosNomina.resultado.GetValueOrDefault();
                        if (movimientosNomina.resultado.GetValueOrDefault() <= 0)
                        {//esto es para cuando en el modulo del finiquito le ingresen un importe al concepto.
                            resultado = ejecutaFormula(movimientosNomina.concepNomDefi.formulaConcepto);
                            movimientosNomina.resultado = aplicarMascara(movimientosNomina.concepNomDefi, resultado, false);
                        }

                    }
                    else
                    {
                        resultado = ejecutaFormula(movimientosNomina);//se manda el movimiento em ves de la formula
                        movimientosNomina.resultado = aplicarMascara(movimientosNomina.concepNomDefi, resultado, false);
                    }
                    movimientosNomina.calculado = resultado;
                    calculaConceptosBaseAfecta(movimientosNomina.movNomBaseAfecta, movimientosNomina.resultado.GetValueOrDefault());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ejecutarParametroCondicionYFormula()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return movimientosNomina;
        }

        private void calculaConceptosBaseAfecta(IList<MovNomBaseAfecta> afecConcepNominas, Double resultadoConcepto)
        {
            int Base;
            double calculo;
            if (afecConcepNominas != null && afecConcepNominas.Count > 0)
            {
                MovNomBaseAfecta movNominaBaseAfecta;
                for (Base = 0; Base < afecConcepNominas.Count; Base++)
                {
                    movNominaBaseAfecta = afecConcepNominas[Base];
                    if (movNominaBaseAfecta.baseAfecConcepNom != null)
                    {
                        if (movNominaBaseAfecta.baseAfecConcepNom.formulaExenta != null)
                        {
                            if (afecConcepNominas[Base].baseAfecConcepNom.formulaExenta.Length > 0)
                            {
                                //                            System.out.println("Formula Exenta " + afecConcepNominas.get(base).getBaseAfecConcepNom().getFormulaExenta());
                                calculo = ejecutaFormula(afecConcepNominas[Base].baseAfecConcepNom.formulaExenta);
                                if (calculo >= resultadoConcepto)
                                {
                                    movNominaBaseAfecta.resultado = 0.0;
                                    movNominaBaseAfecta.resultadoExento = resultadoConcepto;
                                }
                                else
                                {
                                    movNominaBaseAfecta.resultado = resultadoConcepto - calculo;
                                    movNominaBaseAfecta.resultadoExento = calculo;
                                }
                            }
                            else
                            {
                                movNominaBaseAfecta.resultado = resultadoConcepto;
                                movNominaBaseAfecta.resultadoExento = 0.0;
                            }
                        }
                        else
                        {
                            movNominaBaseAfecta.resultado = resultadoConcepto;
                            movNominaBaseAfecta.resultadoExento = 0.0;
                        }

                        if (movNominaBaseAfecta.baseAfecConcepNom.baseNomina.clave.Equals(ClavesParametrosModulos.claveBaseNominaISR) && movNominaBaseAfecta.resultado > 0)
                        {
                            bool modificoResultado = false;
                            switch (movNominaBaseAfecta.baseAfecConcepNom.tipoAfecta)
                            {
                                case 0:
                                    if (movNominaBaseAfecta.movNomConcep.concepNomDefi.naturaleza == (int)Naturaleza.PERCEPCION)
                                    {
                                        /// para cuando desconte faltas y es de tipo ajustar al mes el calculo del isr
                                        if (descontarFaltasModoAjustaMes && periodosNomina.cierreMes && (modoAjustarIngresosMes == ProporcionaPeriodoAjustadoFinalMes || modoAjustarIngresosMes == PropPeriodoIndepAjustadoAlUltimoPeriodoMes)
                                            && movNominaBaseAfecta.movNomConcep.concepNomDefi.formulaConcepto.ToUpper().Contains("DiasPago".ToUpper()))
                                        {
                                            modificoResultado = true;
                                            acumuladoNormal = acumuladoNormal + ((movNominaBaseAfecta.resultado.GetValueOrDefault() / (Double)valoresConceptosEmpleados["DiasPago".ToUpper()] * (int)valoresConceptosEmpleados["DiasNormalesAPagar".ToUpper()]));
                                        }
                                        if (!modificoResultado)
                                        {
                                            acumuladoNormal = acumuladoNormal + movNominaBaseAfecta.resultado.GetValueOrDefault();
                                        }
                                    }
                                    else if (movNominaBaseAfecta.movNomConcep.concepNomDefi.naturaleza == (int)Naturaleza.DEDUCCION)
                                    {
                                        /// para cuando desconte faltas y es de tipo ajustar al mes el calculo del isr
                                        if (descontarFaltasModoAjustaMes && periodosNomina.cierreMes && (modoAjustarIngresosMes == ProporcionaPeriodoAjustadoFinalMes || modoAjustarIngresosMes == PropPeriodoIndepAjustadoAlUltimoPeriodoMes)
                                            && movNominaBaseAfecta.movNomConcep.concepNomDefi.formulaConcepto.ToUpper().Contains("DiasPago".ToUpper()))
                                        {
                                            modificoResultado = true;
                                            acumuladoNormal = acumuladoNormal - ((movNominaBaseAfecta.resultado.GetValueOrDefault() / (Double)valoresConceptosEmpleados["DiasPago".ToUpper()] * (int)valoresConceptosEmpleados["DiasNormalesAPagar".ToUpper()]));
                                        }
                                        if (!modificoResultado)
                                        {
                                            acumuladoNormal = acumuladoNormal - movNominaBaseAfecta.resultado.GetValueOrDefault();
                                        }
                                    }
                                    break;
                                case 1:
                                    if (movNominaBaseAfecta.movNomConcep.concepNomDefi.naturaleza == (int)Naturaleza.PERCEPCION)
                                    {
                                        /// para cuando desconte faltas y es de tipo ajustar al mes el calculo del isr
                                        if (descontarFaltasModoAjustaMes && periodosNomina.cierreMes && (modoAjustarIngresosMes == ProporcionaPeriodoAjustadoFinalMes || modoAjustarIngresosMes == PropPeriodoIndepAjustadoAlUltimoPeriodoMes)
                                            && movNominaBaseAfecta.movNomConcep.concepNomDefi.formulaConcepto.ToUpper().Contains("DiasPago".ToUpper()))
                                        {
                                            modificoResultado = true;
                                            acumuladoDirecto = acumuladoDirecto + ((movNominaBaseAfecta.resultado.GetValueOrDefault() / (Double)valoresConceptosEmpleados["DiasPago".ToUpper()] * (int)valoresConceptosEmpleados["DiasNormalesAPagar".ToUpper()]));
                                        }
                                        if (!modificoResultado)
                                        {
                                            acumuladoDirecto = acumuladoDirecto + movNominaBaseAfecta.resultado.GetValueOrDefault();
                                        }
                                    }
                                    else if (movNominaBaseAfecta.movNomConcep.concepNomDefi.naturaleza == (int)Naturaleza.DEDUCCION)
                                    {
                                        /// para cuando desconte faltas y es de tipo ajustar al mes el calculo del isr
                                        if (descontarFaltasModoAjustaMes && periodosNomina.cierreMes && (modoAjustarIngresosMes == ProporcionaPeriodoAjustadoFinalMes || modoAjustarIngresosMes == PropPeriodoIndepAjustadoAlUltimoPeriodoMes)
                                            && movNominaBaseAfecta.movNomConcep.concepNomDefi.formulaConcepto.ToUpper().Contains("DiasPago".ToUpper()))
                                        {
                                            modificoResultado = true;
                                            acumuladoDirecto = acumuladoDirecto - ((movNominaBaseAfecta.resultado.GetValueOrDefault() / (Double)valoresConceptosEmpleados["DiasPago".ToUpper()] * (int)valoresConceptosEmpleados["DiasNormalesAPagar".ToUpper()]));
                                        }
                                        if (!modificoResultado)
                                        {
                                            acumuladoDirecto = acumuladoDirecto - movNominaBaseAfecta.resultado.GetValueOrDefault();
                                        }
                                    }
                                    break;
                                case 2:
                                    if (movNominaBaseAfecta.movNomConcep.concepNomDefi.naturaleza == (int)Naturaleza.PERCEPCION)
                                    {
                                        /// para cuando desconte faltas y es de tipo ajustar al mes el calculo del isr
                                        if (descontarFaltasModoAjustaMes && periodosNomina.cierreMes && (modoAjustarIngresosMes == ProporcionaPeriodoAjustadoFinalMes || modoAjustarIngresosMes == PropPeriodoIndepAjustadoAlUltimoPeriodoMes)
                                            && movNominaBaseAfecta.movNomConcep.concepNomDefi.formulaConcepto.ToUpper().Contains("DiasPago".ToUpper()))
                                        {
                                            modificoResultado = true;
                                            acumuladoAnual = acumuladoAnual + ((movNominaBaseAfecta.resultado.GetValueOrDefault() / (Double)valoresConceptosEmpleados["DiasPago".ToUpper()] * (int)valoresConceptosEmpleados["DiasNormalesAPagar".ToUpper()]));
                                        }
                                        if (!modificoResultado)
                                        {
                                            acumuladoAnual = acumuladoAnual + movNominaBaseAfecta.resultado.GetValueOrDefault();
                                        }
                                    }
                                    else if (movNominaBaseAfecta.movNomConcep.concepNomDefi.naturaleza == (int)Naturaleza.DEDUCCION)
                                    {
                                        /// para cuando desconte faltas y es de tipo ajustar al mes el calculo del isr
                                        if (descontarFaltasModoAjustaMes && periodosNomina.cierreMes && (modoAjustarIngresosMes == ProporcionaPeriodoAjustadoFinalMes || modoAjustarIngresosMes == PropPeriodoIndepAjustadoAlUltimoPeriodoMes)
                                            && movNominaBaseAfecta.movNomConcep.concepNomDefi.formulaConcepto.ToUpper().Contains("DiasPago".ToUpper()))
                                        {
                                            modificoResultado = true;
                                            acumuladoAnual = acumuladoAnual - ((movNominaBaseAfecta.resultado.GetValueOrDefault() / (Double)valoresConceptosEmpleados["DiasPago".ToUpper()] * (int)valoresConceptosEmpleados["DiasNormalesAPagar".ToUpper()]));
                                        }
                                        if (!modificoResultado)
                                        {
                                            acumuladoAnual = acumuladoAnual - movNominaBaseAfecta.resultado.GetValueOrDefault();
                                        }
                                    }
                                    break;
                            }
                        }
                        else if (movNominaBaseAfecta.baseAfecConcepNom.baseNomina.clave.Equals(ClavesParametrosModulos.claveBaseNominaIMSS))
                        {
                            switch (movNominaBaseAfecta.baseAfecConcepNom.tipoAfecta)
                            {
                                case 0:
                                    acumuladoImssFijo = acumuladoImssFijo + movNominaBaseAfecta.resultado.GetValueOrDefault();
                                    break;
                                case 1:
                                    //acumuladoDirecto = acumuladoDirecto + movNominaBaseAfecta.getResultado();
                                    break;
                            }

                        }

                        ejecutaQueryExecuteUpdate("Update MovNomBaseAfecta mov Set mov.resultado = :resultado Where mov.id = :id", new String[] { "resultado", "id" }, new Object[] { movNominaBaseAfecta.resultado, movNominaBaseAfecta.id });
                        //ejecutaQueryExecuteUpdate("Update MovNomBaseAfecta mov Set mov.resultado = :resultado Where mov.id = :id", new String[] { "resultado", "id" }, new Object[] { movNominaBaseAfecta.getResultado(), movNominaBaseAfecta.getId() });
                        if (mensajeResultado.noError == -101)
                        {
                            mensajeResultado.noError = 54;
                        }
                    }
                }
            }

        }

        private Double ejecutaFormula(Object formula)
        {

            int ren;
            MovNomConcep mnc = null;
            String formulaCalculo = "";

            if (formula.GetType().Equals(typeof(string)))
            {
                formulaCalculo = (String)formula;
            }
            else
            {
                mnc = (MovNomConcep)formula;
                formulaCalculo = mnc.concepNomDefi.formulaConcepto;

            }
            Object[] valores;
            double resultado = 0;

            bool variableTipoConcepto = false, usaVariable2Periodos = false;
            if (formulaCalculo.Length > 0)
            {
                try
                {
                    agregaVariableConceptos();
                    if (!isCalculoPTU && (formulaCalculo.Contains("PTUDIAS".ToUpper())
                            || formulaCalculo.Contains("PTUPERCEPCIONES".ToUpper())
                            || formulaCalculo.Contains("PTUTOTAL".ToUpper())))
                    {
                        return 0.0;
                    }
                    formulaCalculo = convierteClaveFormulaANombre(formulaCalculo);
                    valores = compilador.compila(formulaCalculo.ToUpper(), new Reg_Polish[] { }, "", false);
                    TipoClasificacionFormula tfc;
                    String variable, tipoDato = "S";
                    if ((Boolean)valores[2])
                    {
                        String[] identificadores = ((HashSet<String>)valores[3]).ToArray();
                        for (ren = 0; ren < identificadores.Length; ren++)
                        {
                            identificadores[ren] = identificadores[ren].Trim();
                            if (identificadores[ren].ToUpper().Equals("IngresoVacaciones".ToUpper())
                               || identificadores[ren].ToUpper().Equals("IngresoPrimaVacacional".ToUpper()))
                            {
                                if (salarioVacaciones == ManejoSalarioVacaciones.SALARIOANIVERSARIO)
                                {
                                    if (mnc.vacacionesAplicacion != null)
                                    {
                                        valoresConceptosEmpleados.Add("DiasVacaciones".ToUpper(), mnc.vacacionesAplicacion.diasVac);
                                        valoresConceptosEmpleados.Add("DiasPrima".ToUpper(), mnc.vacacionesAplicacion.diasPrima);
                                        if (mnc.vacacionesAplicacion.vacacionesDevengadas.salarioAniversario != null)
                                        {
                                            valoresConceptosEmpleados.Add("SueldoDiario".ToUpper(), mnc.vacacionesAplicacion.vacacionesDevengadas.salarioAniversario);
                                        }
                                    }

                                }
                                if (identificadores[ren].ToUpper().Equals("IngresoVacaciones".ToUpper()))
                                {
                                    int diasVaca = (int)valoresConceptosEmpleados["DiasVacaciones".ToUpper()];
                                    double sueldo = (double)valoresConceptosEmpleados["SueldoDiario".ToUpper()];
                                    double res = diasVaca * sueldo;
                                    valoresConceptosEmpleados.Add("IngresoVacaciones".ToUpper(), res);
                                }
                                else if (identificadores[ren].ToUpper().Equals("IngresoPrimaVacacional".ToUpper()))
                                {
                                    Double diasVacaPrima = (Double)valoresConceptosEmpleados["DiasPrima".ToUpper()];
                                    double sueldo = (double)valoresConceptosEmpleados["SueldoDiario".ToUpper()];
                                    double res = diasVacaPrima * sueldo;
                                    valoresConceptosEmpleados.Add("IngresoPrimaVacacional".ToUpper(), res);
                                }
                                isVacacionesStatus = true;
                            }
                            if (identificadores[ren].StartsWith("CONCEP"))
                            {
                                buscaFormulaConceptos(identificadores[ren]);
                                variableTipoConcepto = true;
                            }
                            else
                            {

                                variable = formulaCalculo.Substring(formulaCalculo.ToUpper().IndexOf(identificadores[ren]), identificadores[ren].Length);
                                string pr = propertieFuente.GetProperty(string.Concat(variable, "_TipoDato"));
                                tfc = (TipoClasificacionFormula)Entity.util.ManejadorEnum.getEnum(propertieFuente.GetProperty(string.Concat(variable, "_TipoDato")), typeof(TipoClasificacionFormula));
                                tipoDato = propertieFuente.GetProperty(string.Concat(variable, "_Tipo"));
                                if (tfc == TipoClasificacionFormula.DATOCALCULO | tfc == TipoClasificacionFormula.DATOPERIODO | tfc == TipoClasificacionFormula.DATOMENSUAL | tfc == TipoClasificacionFormula.DATOBIMESTRAL | tfc == TipoClasificacionFormula.DATOANUAL)
                                {
                                    if (tfc != TipoClasificacionFormula.DATOCALCULO)
                                    {
                                        valoresConceptosEmpleados.Add(identificadores[ren], "");
                                    }
                                    buscaVaricablesCalcular(identificadores[ren], tfc);
                                }
                                else if (tfc == TipoClasificacionFormula.DATOFUNCION)
                                {
                                    String funcion = formulaCalculo.ToUpper();
                                    int posParenAb = -1, posParenCerr = 0, inicioFun = funcion.ToUpper().IndexOf(identificadores[ren]);
                                    for (int i = inicioFun; i < funcion.Length; i++)
                                    {
                                        if (funcion[i] == '(')
                                        {
                                            posParenAb = i;
                                        }
                                        else if (funcion[i] == ')' & posParenAb > -1)
                                        {
                                            posParenCerr = i;
                                            break;
                                        }
                                    }
                                    funcion = funcion.Substring(funcion.IndexOf(identificadores[ren]), posParenCerr + 1);
                                    buscaVaricablesCalcular(funcion, tfc);
                                }
                                else if (tfc == TipoClasificacionFormula.DATOTABLA)
                                {
                                    String funcion = formulaCalculo.ToUpper();
                                    variablesTipoTabla(funcion);
                                }
                            }

                            if (valoresConceptosEmpleados.ContainsKey(identificadores[ren]))
                            {
                                Object valor = valoresConceptosEmpleados[identificadores[ren]];
                                if (valor == null)
                                {
                                    valoresConceptosEmpleados[identificadores[ren]] = string.Equals(tipoDato, "N", StringComparison.OrdinalIgnoreCase) ? 0.0 : 0.0;
                                }
                            }
                            else
                            {
                                valoresConceptosEmpleados[identificadores[ren]] = string.Equals(tipoDato, "N", StringComparison.OrdinalIgnoreCase) ? 0.0 : 0.0;
                            }

                            if (variablesAjustadasEnDosPeriodos.Contains(identificadores[ren]))
                            {
                                usaVariable2Periodos = true;
                            }

                        }
                        if (variableTipoConcepto)
                        {
                            agregaVariableConceptos();
                            valores = compilador.compila(formulaCalculo.ToUpper(), new Reg_Polish[] { }, "", false);
                        }
                        resultado = compilador.calcula((Reg_Polish[])valores[0], valoresConceptosEmpleados, resultado);

                    }
                    else
                    {
                        resultado = 0;
                    }

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ejecutaFormula()1_Error: ").Append(ex));
                    mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                    mensajeResultado.error = ex.GetBaseException().ToString();
                    mensajeResultado.resultado = null;
                }
            }
            else
            {


                return 0.0;
            }
            return resultado;
        }

        private void variablesTipoTabla(String variable)
        {
            Object resultado = null;
            String funcion = variable;
            String parametroFuncion = variable.Substring(funcion.IndexOf("(") + 1, funcion.IndexOf(")")).Replace("'", "");
            String funcionNombre = variable.Substring(0, funcion.IndexOf("("));
            String[] parametrosFuncion = parametroFuncion.Split(',');
            String nombreTable;
            String valorBuscar;
            String buscarEn;
            int valorCol;
            String y;
            String x;
            Object valorb = 0;
            if (String.Equals(funcionNombre, "ValorTabla", StringComparison.OrdinalIgnoreCase))
            {
                nombreTable = parametrosFuncion[0].Replace("'", "");
                valorBuscar = parametrosFuncion[1].Replace("'", "");
                buscarEn = parametrosFuncion[2].Replace("'", "");
                valorCol = Convert.ToInt32(parametrosFuncion[3].Replace("'", ""));
                nombreTable = nombreTable.Replace("_", " ");
                for (int i = 0; i < tablaDatosXml.Count; i++)
                {
                    if (tablaDatosXml[i].tablaPersonalizada != null)
                    {
                        if (tablaDatosXml[i].tablaPersonalizada.descripcion.ToUpper().Equals(nombreTable))
                        {
                            cargaValoresXMLtoTabla(tablaDatosXml[i]);
                            TipoDatoTableXml tipo = new TipoDatoTableXml();
                            tipo = convertirDato(valorBuscar);
                            if (tipo.getTipoDato() == typeof(string))
                            {
                                if ("VERDADERO".Equals(valorBuscar.ToUpper()))
                                {
                                    valorb = true;
                                }
                                else if ("FALSO".Equals(valorBuscar.ToUpper()))
                                {
                                    valorb = false;
                                }
                                else
                                {
                                    valorb = valoresConceptosEmpleados[valorBuscar.ToUpper()];
                                }
                            }
                            else
                            {
                                valorb = tipo.getValor();
                            }
                            resultado = buscarValorTablaXML(funcionNombre, valorb, buscarEn, valorCol, null, null);
                            break;
                        }
                    }
                    else if (tablaDatosXml[i].tablaBase.descripcion.ToUpper().Equals(nombreTable))
                    {
                        cargaValoresXMLtoTabla(tablaDatosXml[i]);
                        TipoDatoTableXml tipo = new TipoDatoTableXml();
                        tipo = convertirDato(valorBuscar);
                        if (tipo.getTipoDato() == typeof(string))
                        {
                            if ("VERDADERO".Equals(valorBuscar.ToUpper()))
                            {
                                valorb = true;
                            }
                            else if ("FALSO".Equals(valorBuscar.ToUpper()))
                            {
                                valorb = false;
                            }
                            else
                            {
                                valorb = valoresConceptosEmpleados[valorBuscar.ToUpper()];
                            }
                        }
                        else
                        {
                            valorb = tipo.getValor();
                        }
                        resultado = buscarValorTablaXML(funcionNombre, valorb, buscarEn, valorCol, null, null);
                        break;
                    }

                }

            }
            else if (String.Equals(funcionNombre, "ValorTablaXY", StringComparison.OrdinalIgnoreCase))
            {

                nombreTable = parametrosFuncion[0].Replace("'", "");
                x = parametrosFuncion[1].Replace("'", "");
                y = parametrosFuncion[2].Replace("'", "");
                nombreTable = nombreTable.Replace("_", " ");
                for (int i = 0; i < tablaDatosXml.Count; i++)
                {
                    if (tablaDatosXml[i].tablaPersonalizada != null)
                    {
                        if (tablaDatosXml[i].tablaPersonalizada.descripcion.ToUpper().Equals(nombreTable))
                        {
                            cargaValoresXMLtoTabla(tablaDatosXml[i]);
                            TipoDatoTableXml tipo = new TipoDatoTableXml();
                            tipo = convertirDato(y);
                            if (tipo.getTipoDato() == typeof(string))
                            {
                                if ("VERDADERO".Equals(y.ToUpper()))
                                {
                                    valorb = true;
                                }
                                else if ("FALSO".Equals(y.ToUpper()))
                                {
                                    valorb = false;
                                }
                                else
                                {
                                    valorb = valoresConceptosEmpleados[y.ToUpper()];
                                }
                            }
                            else
                            {
                                valorb = tipo.getValor();
                            }
                            resultado = buscarValorTablaXML(funcionNombre, null, null, 0, x, y);
                            break;
                        }
                    }
                    else if (tablaDatosXml[i].tablaBase.descripcion.ToUpper().Equals(nombreTable))
                    {
                        cargaValoresXMLtoTabla(tablaDatosXml[i]);
                        TipoDatoTableXml tipo = new TipoDatoTableXml();
                        tipo = convertirDato(y);
                        if (tipo.getTipoDato() == typeof(string))
                        {
                            valorb = valoresConceptosEmpleados[y.ToUpper()];
                        }
                        else
                        {
                            valorb = tipo.getValor();
                        }
                        resultado = buscarValorTablaXML(funcionNombre, null, null, 0, x, y);
                        break;
                    }
                }
            }
            funcion = funcion.Replace("(", "").Replace("'", "").Replace(",", "").Replace(")", "");
            valoresConceptosEmpleados.Add(funcion, resultado);
        }

        private Object buscarValorTablaXML(String funcion, Object valorBuscar, String buscarEn, int valorCol, String x, Object y)
        {
            Object res = 0;
            int fila = 0;
            int ini = 0;
            int fin = 0;
            double valorBuscarDo = 0;
            bool valorBuscarBo = false;
            //Calendar valorBuscarCal = Calendar.getInstance();
            DateTime valorBuscarDa = new DateTime();
            string valorBuscarStr = "";
            try
            {
                if (string.Equals(funcion, "ValorTabla".ToUpper(), StringComparison.OrdinalIgnoreCase))
                {
                    string[] rangod = buscarEn.Split(':');
                    TipoDatoTableXml tipoValorBuscar = new TipoDatoTableXml();
                    tipoValorBuscar = convertirDato(Convert.ToString(valorBuscar));
                    if (tipoValorBuscar.getTipoDato() == typeof(double))
                    {
                        valorBuscarDo = Convert.ToDouble(Convert.ToString(valorBuscar));
                    }
                    else if (tipoValorBuscar.getTipoDato() == typeof(bool))
                    {
                        valorBuscarBo = Convert.ToBoolean(Convert.ToString(valorBuscar));
                    }
                    else if (tipoValorBuscar.getTipoDato() == typeof(DateTime))
                    {
                        valorBuscarDa = (DateTime)valorBuscar;
                        valorBuscarDa = quitaHrsDeFecha(valorBuscarDa);//pendiente de revisar bien
                    }
                    else if (tipoValorBuscar.getTipoDato() == typeof(string))
                    {
                        valorBuscarStr = (string)valorBuscar;
                    }
                    if (rangod.Length == 1)//cuando buscar por una sola columna en la funcion valor tabla
                    {
                        ini = Convert.ToInt32(rangod[0]);
                        for (int i = 0; i < matrixcargaXml.Length; i++)
                        {
                            String valorExtraer = (string)matrixcargaXml[i, ini - 1];
                            TipoDatoTableXml tipo = new TipoDatoTableXml();
                            tipo = convertirDato(valorExtraer);
                            if (tipo.getTipoDato() == typeof(double))
                            {
                                if (tipo.getTipoDato() == valorBuscarDo.GetType())
                                {
                                    double valor = (double)tipo.getValor();
                                    if (valorBuscarDo < valor && i > 0)
                                    {
                                        fila = i - 1;
                                        break;
                                    }
                                    else if (valorBuscarDo.Equals(valor) && i == matrixcargaXml.Length - 1)
                                    {
                                        fila = i;
                                        break;
                                    }
                                    else if (valorBuscarDo.Equals(valor) && i > 0)
                                    {
                                        fila = i;
                                        break;
                                    }
                                    else if (valorBuscarDo.Equals(valor) && i == 0)
                                    {
                                        fila = i;
                                        break;
                                    }
                                }
                            }
                            else if (tipo.getTipoDato() == typeof(DateTime))
                            {
                                if (tipo.getTipoDato() == valorBuscarDa.GetType())
                                {
                                    DateTime valorhrs = (DateTime)tipo.getValor();
                                    DateTime valor = quitaHrsDeFecha(valorhrs);
                                    if (valorBuscarDa.CompareTo(valor) < 0 && i > 0)
                                    {
                                        fila = i - 1;
                                        break;
                                    }
                                    else if (valorBuscarDa.CompareTo(valor) == 0 && i == matrixcargaXml.Length - 1)
                                    {
                                        fila = i;
                                        break;
                                    }
                                    else if (valorBuscarDa.CompareTo(valor) == 0 && i == 0)
                                    {
                                        fila = i;
                                        break;
                                    }
                                }
                            }
                            else if (tipo.getTipoDato() == typeof(string))
                            {
                                if (tipo.getTipoDato() == valorBuscarStr.GetType())
                                {
                                    string valor = (string)tipo.getValor();
                                    if (valorBuscarStr.Equals(valor))
                                    {
                                        fila = i;
                                        break;
                                    }
                                }
                            }
                            else if (tipo.getTipoDato() == typeof(bool))
                            {
                                if (tipo.getTipoDato() == valorBuscarBo.GetType())
                                {
                                    bool valor = (bool)tipo.getValor();
                                    if (valor == valorBuscarBo)
                                    {
                                        fila = i;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else if (rangod.Length == 2)//cuando es por rangos y es funcion valortabla 
                    {
                        ini = Convert.ToInt32(rangod[0]);
                        fin = Convert.ToInt32(rangod[1]);
                        for (int i = 0; i < matrixcargaXml.Length; i++)
                        {
                            TipoDatoTableXml tipo = new TipoDatoTableXml();
                            TipoDatoTableXml tipo2 = new TipoDatoTableXml();
                            string valor1 = (string)matrixcargaXml[i, ini - 1];
                            string valor2 = (string)matrixcargaXml[i, fin - 1];
                            tipo = convertirDato(valor1);
                            tipo2 = convertirDato(valor2);
                            if (tipo.getTipoDato() == typeof(double) && tipo2.getTipoDato() == typeof(double))
                            {
                                double valor = (double)tipo.getValor();
                                double valor3 = (double)tipo2.getValor();
                                if (tipo.getTipoDato() == valorBuscarDo.GetType() && tipo2.getTipoDato() == valorBuscarDo.GetType())
                                {
                                    if (valorBuscarDo >= valor && valorBuscarDo <= valor3)
                                    {
                                        fila = i;
                                        break;
                                    }
                                }

                            }
                            else if (tipo.getTipoDato() == typeof(DateTime) && tipo2.getTipoDato() == typeof(DateTime))
                            {
                                if (tipo.getTipoDato() == valorBuscarDa.GetType() && tipo2.getTipoDato() == valorBuscarDa.GetType())
                                {
                                    DateTime valorhrs = (DateTime)tipo.getValor();
                                    DateTime valorhrs2 = (DateTime)tipo2.getValor();
                                    DateTime fecha = quitaHrsDeFecha(valorhrs);
                                    DateTime fecha2 = quitaHrsDeFecha(valorhrs2);
                                    if ((valorBuscarDa.CompareTo(fecha) > 0 || valorBuscarDa.CompareTo(fecha) == 0)
                                        && (valorBuscarDa.CompareTo(fecha2) < 0 || valorBuscarDa.CompareTo(fecha2) == 0))
                                    {
                                        fila = i;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (fila >= 0)
                    {
                        TipoDatoTableXml restipo;
                        restipo = convertirDato((string)matrixcargaXml[fila, valorCol - 1]);
                        res = restipo.getValor();
                    }
                    else
                    {
                        res = 0;
                    }
                }
                else if (string.Equals(funcion, "ValorTablaXY".ToUpper(), StringComparison.OrdinalIgnoreCase))
                {
                    TipoDatoTableXml tipo4 = new TipoDatoTableXml();
                    tipo4 = convertirDato(Convert.ToString(y));
                    if (tipo4.getTipoDato() == typeof(double))
                    {
                        valorBuscarDo = Convert.ToDouble(Convert.ToString(y));
                    }
                    else if (tipo4.getTipoDato() == typeof(bool))
                    {
                        valorBuscarBo = Convert.ToBoolean(Convert.ToString(y));
                    }
                    else if (tipo4.getTipoDato() == typeof(DateTime))
                    {
                        valorBuscarDa = (DateTime)y;
                        valorBuscarDa = quitaHrsDeFecha(valorBuscarDa);
                    }
                    else if (tipo4.getTipoDato() == typeof(string))
                    {
                        valorBuscarStr = (string)y;
                    }
                    ini = 0;
                    for (int i = 0; i < matrixcargaXml.Length; i++)
                    {
                        string valorExtraer = (string)matrixcargaXml[i, ini];
                        TipoDatoTableXml tipo = new TipoDatoTableXml();
                        tipo = convertirDato(valorExtraer);
                        if (tipo.getTipoDato() == typeof(double))
                        {
                            if (tipo.getTipoDato() == valorBuscarDo.GetType())
                            {
                                double valor = (double)tipo.getValor();
                                if (valorBuscarDo < valor && i > 0)
                                {
                                    fila = i - 1;
                                    break;
                                }
                                else if (valorBuscarDo.Equals(valor) && i == matrixcargaXml.Length - 1)
                                {
                                    fila = i;
                                    break;
                                }
                                else if (valorBuscarDo.Equals(valor) && i > 0)
                                {
                                    fila = i;
                                    break;
                                }
                                else if (valorBuscarDo.Equals(valor) && i == 0)
                                {
                                    fila = i;
                                    break;
                                }
                            }
                        }
                        else if (tipo.getTipoDato() == typeof(DateTime))
                        {
                            if (tipo.getTipoDato() == valorBuscarDa.GetType())
                            {
                                DateTime valorhrs = (DateTime)tipo.getValor();
                                DateTime valor = quitaHrsDeFecha(valorhrs);
                                if (valorBuscarDa.CompareTo(valor) < 0 && i > 0)
                                {
                                    fila = i - 1;
                                    break;
                                }
                                else if (valorBuscarDa.CompareTo(valor) == 0 && i == matrixcargaXml.Length - 1)
                                {
                                    fila = i;
                                    break;
                                }
                                else if (valorBuscarDa.CompareTo(valor) == 0 && i == 0)
                                {
                                    fila = i;
                                    break;
                                }
                            }
                        }
                        else if (tipo.getTipoDato() == typeof(string))
                        {

                            if (tipo.getTipoDato() == valorBuscarStr.GetType())
                            {
                                string valor = (string)tipo.getValor();
                                if (valorBuscarStr.Equals(valor))
                                {
                                    fila = i;
                                    break;
                                }
                            }
                        }
                        else if (tipo.getTipoDato() == typeof(bool))
                        {
                            if (tipo.getTipoDato() == valorBuscarBo.GetType())
                            {
                                bool valor = (bool)tipo.getValor();
                                if (valor == valorBuscarBo)
                                {
                                    fila = i;
                                    break;
                                }
                            }
                        }
                    }
                    if (fila >= 0)
                    {
                        int col = Convert.ToInt32(x);
                        TipoDatoTableXml restipo;
                        restipo = convertirDato((string)matrixcargaXml[fila, col - 1]);
                        res = restipo.getValor();
                    }
                    else
                    {
                        res = 0;
                    }
                }
            }
            catch (Exception e)
            {
                mensajeResultado.noError = -101;
                mensajeResultado.error = "ERROR al procesar la funcion " + funcion + " " + e.Message.ToString();
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ejecutaFormula()1_Error: ").Append(e));

            }

            return res;
        }

        private DateTime quitaHrsDeFecha(DateTime fecha)
        {
            Calendar myCal = CultureInfo.InvariantCulture.Calendar;
            fecha = myCal.AddHours(fecha, 0);
            fecha = myCal.AddMinutes(fecha, 0);
            fecha = myCal.AddSeconds(fecha, 0);
            fecha = myCal.AddMilliseconds(fecha, 0);

            return fecha;
        }

        private class TipoDatoTableXml
        {

            private Type tipoDato;
            private Object valor;

            public Type getTipoDato()
            {
                return tipoDato;
            }

            public void setTipoDato(Type tipoDato)
            {
                this.tipoDato = tipoDato;
            }

            public Object getValor()
            {
                return valor;
            }

            public void setValor(Object valor)
            {
                this.valor = valor;
            }

            public TipoDatoTableXml()
            {
            }

        }

        private void cargaValoresXMLtoTabla(TablaDatos datos)
        {
            if (datos.fileXml != null)
            {
                docXML = util.UtilidadesXML.convierteBytesToXML(datos.fileXml);
                if (util.UtilidadesXML.ERROR_XML != 0)
                {
                    mensajeResultado.resultado = util.UtilidadesXML.mensajeError.resultado;
                    mensajeResultado.error = util.UtilidadesXML.mensajeError.error;
                    mensajeResultado.noError = util.UtilidadesXML.mensajeError.noError;
                    return;
                }
                cargaValoresXML();
            }
        }

        private void cargaValoresXML()
        {
            if (docXML != null)
            {
                var valoresDatos = from a in docXML.Descendants("dato")
                                   select a;
                if (valoresDatos != null)
                {
                    XElement valorDato = valoresDatos.FirstOrDefault();
                    int fila = valoresDatos.Count();
                    int columna = valorDato.Elements().Count();
                    matrixcargaXml = new Object[fila, columna];
                    for (int i = 0; i < fila; i++)
                    {
                        XElement itemDato = valoresDatos.ElementAt(i);
                        for (int j = 0; j < columna; j++)
                        {
                            matrixcargaXml[i, j] = itemDato.Elements().ToList()[j].Value;
                        }
                    }

                }
            }
        }

        private TipoDatoTableXml convertirDato(String valor)
        {
            TipoDatoTableXml dato = new TipoDatoTableXml();
            try
            {
                double valor1 = Convert.ToDouble(valor.ToString());

                dato.setTipoDato(typeof(double));
                dato.setValor(valor1);
            }
            catch (Exception ex)
            {
                try
                {
                    DateTime valor1 = DateTime.Parse((string)valor);
                    dato.setTipoDato(typeof(DateTime));
                    dato.setValor(valor1);

                }
                catch (Exception a)
                {
                    try
                    {
                        bool valor1 = Convert.ToBoolean((string)valor);
                        dato.setTipoDato(typeof(bool));
                        dato.setValor(valor1);

                    }
                    catch (Exception e)
                    {
                        dato.setTipoDato(typeof(string));
                        dato.setValor(valor);

                    }

                }

            }
            return dato;
        }

        private void buscaVaricablesCalcular(String variable, TipoClasificacionFormula tipoAcumulado)
        {
            try
            {
                StringBuilder query = new StringBuilder(0);
                Object resultado;
                String nombreOriginal = variable;
                variable = variable.ToUpper();

                if (tipoAcumulado == TipoClasificacionFormula.DATOANUAL | tipoAcumulado == TipoClasificacionFormula.DATOBIMESTRAL | tipoAcumulado == TipoClasificacionFormula.DATOMENSUAL | tipoAcumulado == TipoClasificacionFormula.DATOPERIODO | tipoAcumulado == TipoClasificacionFormula.DATOFUNCION)
                {
                    buscaVaricablesTipoAcumulados(nombreOriginal, tipoAcumulado);
                }
                else if (string.Equals(variable, "NumeroBimestre", StringComparison.OrdinalIgnoreCase))
                {
                    DateTime fecha = ((DateTime)valoresConceptosEmpleados[parametroFechaFinal]);
                    if ((fecha.Month) % 2 == 1)
                    {
                        resultado = (fecha.Month + 1) / 2;
                    }
                    else
                    {
                        resultado = fecha.Month / 2;
                    }
                    valoresConceptosEmpleados.Add(variable, resultado);
                }
                else if (string.Equals(variable, "FechaInicioBimestre", StringComparison.OrdinalIgnoreCase))
                {
                    DateTime fecha = ((DateTime)valoresConceptosEmpleados[parametroFechaFinal]);
                    if ((fecha.Month) % 2 == 0)
                    {
                        fecha.AddMonths(fecha.Month - 1);

                    }
                    fecha.AddDays(1);
                    resultado = fecha;
                    valoresConceptosEmpleados.Add(variable, resultado);
                }
                else if (string.Equals(variable, "FechaFinalBimestre", StringComparison.OrdinalIgnoreCase))
                {
                    DateTime fecha = ((DateTime)valoresConceptosEmpleados[parametroFechaFinal]);
                    if (fecha.Month % 2 == 1)
                    {
                        fecha.AddMonths(fecha.Month + 1);
                    }
                    fecha.AddDays(DateTime.DaysInMonth(fecha.Year, fecha.Month));
                    resultado = fecha;
                    valoresConceptosEmpleados.Add(variable, resultado);
                }
                else if (string.Equals(variable, "DiasBimestre", StringComparison.OrdinalIgnoreCase))
                {
                    DateTime fecha = ((DateTime)valoresConceptosEmpleados[parametroFechaFinal]);
                    resultado = DateTime.DaysInMonth(fecha.Year, fecha.Month);
                    if (fecha.Month % 2 == 1)
                    {
                        fecha.AddMonths(fecha.Month + 1);
                    }
                    else
                    {
                        fecha.AddMonths(fecha.Month - 1);
                    }
                    resultado = (int)resultado + DateTime.DaysInMonth(fecha.Year, fecha.Month);
                    valoresConceptosEmpleados.Add(variable, resultado);
                }
                else if (string.Equals(variable, "ISRACargo", StringComparison.OrdinalIgnoreCase) | string.Equals(variable, "ISRSubsidio", StringComparison.OrdinalIgnoreCase) | string.Equals(variable, "ISRNeto", StringComparison.OrdinalIgnoreCase)
                      | string.Equals(variable, "SubsEmpleoCalculado", StringComparison.OrdinalIgnoreCase))
                {
                    calculaISPT(null);
                }
                else if (string.Equals(variable, "FALTAS", StringComparison.OrdinalIgnoreCase) | string.Equals(variable, "HorasExtrasDobles".ToUpper(), StringComparison.OrdinalIgnoreCase) | string.Equals(variable, "HorasExtrasTriples".ToUpper(), StringComparison.OrdinalIgnoreCase)
                    | string.Equals(variable, "IncapacidadEnfermedad".ToUpper(), StringComparison.OrdinalIgnoreCase) | string.Equals(variable, "IncapacidadAccidente".ToUpper(), StringComparison.OrdinalIgnoreCase) | string.Equals(variable, "IncapacidadMaternidad".ToUpper(), StringComparison.OrdinalIgnoreCase) | string.Equals(variable, "OtrasIncapacidad".ToUpper(), StringComparison.OrdinalIgnoreCase)
                    | string.Equals(variable, "DiasIncapacidadEmpleado".ToUpper(), StringComparison.OrdinalIgnoreCase) | string.Equals(variable, "Ausentismo".ToUpper(), StringComparison.OrdinalIgnoreCase) | string.Equals(variable, "TExtrasDiaDescanso".ToUpper(), StringComparison.OrdinalIgnoreCase) | string.Equals(variable, "TExtrasDiaFestivo".ToUpper(), StringComparison.OrdinalIgnoreCase)
                    | string.Equals(variable, "TExtrasDiaDomingo".ToUpper(), StringComparison.OrdinalIgnoreCase) | string.Equals(variable, "Retardos".ToUpper(), StringComparison.OrdinalIgnoreCase) | string.Equals(variable, "PermisoConSueldo".ToUpper(), StringComparison.OrdinalIgnoreCase) | string.Equals(variable, "PermisoSinSueldo".ToUpper(), StringComparison.OrdinalIgnoreCase)
                    | string.Equals(variable, "DiasFestivos".ToUpper(), StringComparison.OrdinalIgnoreCase) | string.Equals(variable, "DiasDescanso".ToUpper(), StringComparison.OrdinalIgnoreCase) | string.Equals(variable, "FALTAS", StringComparison.OrdinalIgnoreCase))
                {
                    if (!valoresConceptosEmpleados.ContainsKey(variable) ? true : (valoresConceptosEmpleados[variable] == null ? true : (string.IsNullOrEmpty(valoresConceptosEmpleados[variable].ToString()) ? true : false)))
                    {
                        cargarVariablesEmpleadoAsistencias((DateTime)valoresConceptosEmpleados[parametroFechaInicial], (DateTime)valoresConceptosEmpleados[parametroFechaFinal], null, null, false);//JSA30
                    }
                }
                else if (string.Equals(variable, "DiasCreditoPeriodo".ToUpper(), StringComparison.OrdinalIgnoreCase))
                { //Pendiente
                  //valoresConceptosEmpleados.Add(variable, resultado);
                }
                else if (string.Equals(variable, "FechaSalidaVacacion".ToUpper(), StringComparison.OrdinalIgnoreCase) | string.Equals(variable, "FechaRegresoVacacion".ToUpper(), StringComparison.OrdinalIgnoreCase))
                {
                    query.Remove(0, query.Length);
                    query.Append("Select rv.salidaVacac,rv.regresoVac,va.diasVac,va.diasPrima from VacacionesAplicacion va inner join va.vacacionesDisfrutadas rv  inner join rv.empleados em ");
                    query.Append("inner join rv.periodoAplicacion p inner join rv.razonesSociales rs where p.id = :idPeriodo and em.clave = :claveEmp and rs.clave = :claveRazonSocial");
                    Object[] vacaciones = (Object[])ejecutaQueryUnico(query.ToString(), new String[] { "idPeriodo", "claveEmp", "claveRazonSocial" }, new Object[] { valoresConceptosEmpleados["IdPeriodo".ToUpper()], valoresConceptosEmpleados["NumEmpleado".ToUpper()], valoresConceptosEmpleados["RazonSocial".ToUpper()] });

                    if (vacaciones == null)
                    {
                        valoresConceptosEmpleados["FechaSalidaVacacion".ToUpper()] = null;
                        valoresConceptosEmpleados["FechaRegresoVacacion".ToUpper()] = null;
                        valoresConceptosEmpleados["DiasVacaciones".ToUpper()] = 0;
                        valoresConceptosEmpleados["DiasPrima".ToUpper()] = 0.0;
                    }
                    else
                    {
                        valoresConceptosEmpleados["FechaSalidaVacacion".ToUpper()] = (DateTime)vacaciones[0];
                        valoresConceptosEmpleados["FechaRegresoVacacion".ToUpper()] = (DateTime)vacaciones[1];
                        valoresConceptosEmpleados["DiasVacaciones".ToUpper()] = (int)vacaciones[2];
                        valoresConceptosEmpleados["DiasPrima".ToUpper()] = (double)vacaciones[3];
                    }
                }
                else if (string.Equals(variable, "TipoVacaciones".ToUpper(), StringComparison.OrdinalIgnoreCase))
                {
                    resultado = buscarValoresVacaciones("tiposVacaciones.nombre", TipoMostrarCampo.NORMAL, EntidadesVacaciones.DISFRUTADAS);
                    valoresConceptosEmpleados.Add(variable, resultado);
                }
                else if (string.Equals(variable, "TotalDiasVacaciones".ToUpper(), StringComparison.OrdinalIgnoreCase))
                {
                    resultado = buscarValoresVacaciones("diasVacaciones", TipoMostrarCampo.SUMA, EntidadesVacaciones.DEVENGADAS);
                    valoresConceptosEmpleados.Add(variable, resultado);
                }
                else if (string.Equals(variable, "DiasVacaciones".ToUpper(), StringComparison.OrdinalIgnoreCase))
                {
                    bool calcula = false;
                    if (valoresConceptosEmpleados.ContainsKey("DiasVacaciones".ToUpper()))
                    {
                        Object valor = valoresConceptosEmpleados["DiasVacaciones".ToUpper()];
                        if (valor == null)
                        {
                            calcula = true;
                        }
                    }
                    if (calcula)
                    {
                        resultado = buscarValoresVacaciones("diasVac", TipoMostrarCampo.NORMAL, EntidadesVacaciones.APLICACION);
                        valoresConceptosEmpleados.Add(variable, resultado);
                    }
                }
                else if (string.Equals(variable, "DiasPrima".ToUpper(), StringComparison.OrdinalIgnoreCase))
                {
                    bool calcula = false;
                    if (valoresConceptosEmpleados.ContainsKey("DiasPrima".ToUpper()))
                    {
                        Object valor = valoresConceptosEmpleados["DiasPrima".ToUpper()];
                        if (valor == null)
                        {
                            calcula = true;
                        }
                    }
                    if (calcula)
                    {
                        resultado = buscarValoresVacaciones("diasPrima", TipoMostrarCampo.NORMAL, EntidadesVacaciones.APLICACION);
                        valoresConceptosEmpleados.Add(variable, resultado);
                    }
                }
                else if (string.Equals(variable, "DiasVacacionesPendientes".ToUpper(), StringComparison.OrdinalIgnoreCase))
                {
                    resultado = buscarValoresVacaciones("DiasVacacionesPendientes", TipoMostrarCampo.OPERACION, EntidadesVacaciones.APLICACION);
                    valoresConceptosEmpleados.Add(variable, resultado);
                }
                else if (string.Equals(variable, "PTUDIAS".ToUpper(), StringComparison.OrdinalIgnoreCase)
                      || string.Equals(variable, "PTUPERCEPCIONES".ToUpper(), StringComparison.OrdinalIgnoreCase)
                      || string.Equals(variable, "PTUTOTAL".ToUpper(), StringComparison.OrdinalIgnoreCase))
                {
                    if (isCalculoPTU && ptuEmpleado != null)
                    {
                        resultado = buscarValoresPTU(variable);
                        valoresConceptosEmpleados.Add(variable, resultado);
                    }
                    else
                    {
                        resultado = 0;
                    }

                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscaVaricablesCalcular()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
        }

        private Object buscarValoresPTU(String campo)
        {
            Object resultado = null;
            try
            {
                StringBuilder query = new StringBuilder(0);
                String alias = "ptu";
                if (campo.Equals("PTUDIAS"))
                {
                    campo = "ptuDias";
                    ptuEmpleado.periodosNomina_periodoPtuDias_ID = periodosNomina;
                    ptuEmpleado.tipoCorrida_tipoCorridaPtuDias_ID = tipoCorrida;
                    ptuEmpleado.tipoNomina_tipoNominaPtuDias_ID = (TipoNomina)valoresConceptosEmpleados["TIPONOMINAENTIDAD"];
                }
                else if (campo.Equals("PTUPERCEPCIONES"))
                {
                    campo = "ptuPercepciones";
                    ptuEmpleado.periodosNomina_periodoPtuPercep_ID = periodosNomina;
                    ptuEmpleado.tipoCorrida_tipoCorridaPtuPercep_ID = tipoCorrida;
                    ptuEmpleado.tipoNomina_tipoNominaPtuPercep_ID = (TipoNomina)valoresConceptosEmpleados["TIPONOMINAENTIDAD"];
                }
                else if (campo.Equals("PTUTOTAL"))
                {
                    campo = "ptuDias+ptuPercepciones";
                    ptuEmpleado.periodosNomina_periodoPtuDias_ID = periodosNomina;
                    ptuEmpleado.periodosNomina_periodoPtuPercep_ID = periodosNomina;
                    ptuEmpleado.tipoCorrida_tipoCorridaPtuDias_ID = tipoCorrida;
                    ptuEmpleado.tipoCorrida_tipoCorridaPtuPercep_ID = tipoCorrida;
                    ptuEmpleado.tipoNomina_tipoNominaPtuDias_ID = (TipoNomina)valoresConceptosEmpleados["TIPONOMINAENTIDAD"];
                    ptuEmpleado.tipoNomina_tipoNominaPtuPercep_ID = (TipoNomina)valoresConceptosEmpleados["TIPONOMINAENTIDAD"];
                }

                query.Append("SELECT ");
                query.Append(alias).Append(".").Append(campo).Append(" ");
                query.Append("FROM PtuEmpleados ptu WHERE ptu.razonesSociales.clave= :claveRazonsocial ");
                query.Append("AND ptu.ejercicio= :ejercicio AND ptu.empleados.clave= :claveEmpleado ");
                List<string> camposWhere = new List<string>();
                List<object> valoresWhere = new List<object>();
                camposWhere.Add("claveRazonsocial");
                camposWhere.Add("ejercicio");
                camposWhere.Add("claveEmpleado");
                valoresWhere.Add(valoresConceptosEmpleados["RazonSocial".ToUpper()]);
                valoresWhere.Add(ptuDatosGenerales.ejercicio);
                valoresWhere.Add(valoresConceptosEmpleados["NumEmpleado".ToUpper()]);
                resultado = (Object)ejecutaQueryUnico(query.ToString(), camposWhere.ToArray<string>(), valoresWhere.ToArray());
                if (resultado == null)
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscarValoresPTU()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return resultado;
        }

        public enum EntidadesVacaciones
        {
            APLICACION, DEVENGADAS, DISFRUTADAS
        }

        enum TipoMostrarCampo
        {

            SUMA, COUNT, NORMAL, OPERACION
        }

        private Object buscarValoresVacaciones(String campo, TipoMostrarCampo tmc, EntidadesVacaciones entidadesVacaciones)
        {
            StringBuilder query = new StringBuilder(0);
            Object resultado = null;
            String alias = "";
            if (EntidadesVacaciones.APLICACION == entidadesVacaciones)
            {
                alias = "va";
            }
            else if (EntidadesVacaciones.DEVENGADAS == entidadesVacaciones)
            {
                alias = "vd";
            }
            else
            {
                alias = "rv";
            }

            query.Append("SELECT ");
            if (tmc == TipoMostrarCampo.COUNT)
            {
                query.Append("COUNT(").Append(alias).Append(".").Append(campo).Append(") ");
            }
            else if (tmc == TipoMostrarCampo.SUMA)
            {
                query.Append("CASE WHEN (COUNT(").Append(alias).Append(") = 0) THEN 0 ELSE SUM(").Append(alias).Append(".").Append(campo).Append(") END ");
            }
            else if (tmc == TipoMostrarCampo.OPERACION)
            {
                if (string.Equals(campo, "DiasVacacionesPendientes", StringComparison.OrdinalIgnoreCase))
                {
                    query.Append("CASE WHEN (COUNT(vd) = 0) THEN 0 ELSE sum(CASE WHEN (vd.diasVacaciones IS NULL) THEN 0 ELSE vd.diasVacaciones END) END-CASE WHEN (COUNT(va) = 0) THEN 0 ELSE sum(CASE WHEN (va.diasVac IS NULL) THEN 0 ELSE va.diasVac END) END");
                }
                else
                {
                    return 0; //no hay campo a validar
                }

            }
            else
            {
                query.Append(alias).Append(".").Append(campo).Append(" ");
            }

            List<String> camposWhere = new List<String>();
            List<Object> valoresWhere = new List<Object>();

            query.Append("FROM VacacionesAplicacion va INNER JOIN va.vacacionesDisfrutadas rv INNER JOIN rv.empleados em INNER JOIN rv.razonesSociales rz INNER JOIN rv.periodoAplicacion pn "
                    + "RIGHT OUTER JOIN va.vacacionesDevengadas vd INNER JOIN vd.razonesSociales drz INNER JOIN vd.plazasPorEmpleado pem INNER JOIN pem.empleados dem");
            if (tmc == TipoMostrarCampo.OPERACION)
            {
                camposWhere.AddRange((new String[] { "claveEmp", "claveRazonSocial" }).ToList());
                valoresWhere.AddRange((new Object[] { valoresConceptosEmpleados["NumEmpleado".ToUpper()], valoresConceptosEmpleados["RazonSocial".ToUpper()] }).ToList());
                query.Append("WHERE drz.clave = :claveRazonSocial AND dem.clave = :claveEmp  OR  (rz.clave = :claveRazonSocial AND em.clave =:claveEmp)");
            }
            else if (EntidadesVacaciones.APLICACION == entidadesVacaciones || EntidadesVacaciones.DISFRUTADAS == entidadesVacaciones)
            {
                camposWhere.AddRange((new String[] { "claveEmp", "claveRazonSocial", "idPeriodo" }).ToList());
                valoresWhere.AddRange((new Object[] { valoresConceptosEmpleados["NumEmpleado".ToUpper()], valoresConceptosEmpleados["RazonSocial".ToUpper()], valoresConceptosEmpleados["IdPeriodo".ToUpper()] }).ToList());
                query.Append("WHERE em.clave = :claveEmp AND rz.clave = :claveRazonSocial AND pn.id = :idPeriodo ");
            }
            else
            {
                camposWhere.AddRange((new String[] { "claveEmp", "claveRazonSocial" }).ToList());
                valoresWhere.AddRange((new Object[] { valoresConceptosEmpleados["NumEmpleado".ToUpper()], valoresConceptosEmpleados["RazonSocial".ToUpper()] }).ToList());
                query.Append("WHERE dem.clave = :claveEmp AND drz.clave = :claveRazonSocial ");
            }

            resultado = (Object)ejecutaQueryUnico(query.ToString(), camposWhere.ToArray<string>(), valoresWhere.ToArray());
            if (resultado == null)
            {
                resultado = 0.0;
            }
            return resultado;
        }

        private void buscaVaricablesTipoAcumulados(string variable, TipoClasificacionFormula tipoAcumulado)
        {

            Object resultado;
            List<String> variablesExtras = new List<string>();
            List<Object> valoresExtras = new List<object>();
            ////        BaseOtrosPeriodo_Datos=Dato22
            if (propertieFuente != null)
            {
                if (propertieFuente.ContainsKey(string.Concat(variable, "_Datos")))
                {
                    String[] datosExtras = propertieFuente.GetProperty(string.Concat(variable, "_Datos")).Split(','), valores = valoresDatosEspecialesFormula.Split('|');
                    Type tipoDato;
                    int i;
                    for (i = 0; i < datosExtras.Length; i++)
                    {
                        if (propertieFuente.ContainsKey(string.Concat(datosExtras[i], "_Path")))
                        {
                            variablesExtras.Add(propertieFuente.GetProperty(string.Concat(datosExtras[i], "_Path")));
                            tipoDato = util.Utilerias.buscarTipoDatoCampo(propertieFuente.GetProperty(string.Concat(datosExtras[i], "_Path")));
                            valoresExtras.Add(util.Utilerias.castStringTo(tipoDato.GetType().Name, valores[i]));
                        }
                    }
                    valoresDatosEspecialesFormula = "";
                    for (i = datosExtras.Length; i < valores.Length; i++)
                    {
                        valoresDatosEspecialesFormula = string.Concat(valoresDatosEspecialesFormula, string.Concat(valores[i], '|'));
                    }
                    if (valoresDatosEspecialesFormula.Length > 0)
                    {
                        if (valoresDatosEspecialesFormula[valoresDatosEspecialesFormula.Length - 1] == '|')
                        {
                            valoresDatosEspecialesFormula = valoresDatosEspecialesFormula.Substring(0, valoresDatosEspecialesFormula.Length - 1);
                        }
                    }
                }
            }

            clavePeriodoFuncion = "";
            variable = variable.ToUpper();
            DateTime fecha = DateTime.Now;
            fecha = (DateTime)valoresConceptosEmpleados[parametroFechaFinal];
            if (tipoAcumulado == TipoClasificacionFormula.DATOFUNCION)
            {
                variablesTipoFuncion(variable, tipoAcumulado, fecha);
            }
            else if (string.Equals(variable, "PercepcionesGravablesNor", StringComparison.OrdinalIgnoreCase))
            {
                resultado = 0;
                string[] camposParam = new string[] {string.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.naturaleza"),
                string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.tipoAfecta"),
                string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave")};
                Object[] valoresParam = new Object[] { Naturaleza.PERCEPCION, ClavesParametrosModulos.tipoBaseAfectaISRNormal, ClavesParametrosModulos.claveBaseNominaISR };

                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), camposParam, valoresParam,
                    TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);



            }
            else if (string.Equals(variable, "PercepcionesGravablesDirTabla", StringComparison.OrdinalIgnoreCase))
            {
                resultado = 0;
                string[] camposParam = new string[] {string.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.naturaleza"),
                string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.tipoAfecta"),
                string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave")};
                Object[] valoresParam = new Object[] { Naturaleza.PERCEPCION, ClavesParametrosModulos.tipoBaseAfectaISRDirecto, ClavesParametrosModulos.claveBaseNominaISR };

                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), camposParam, valoresParam,
                    TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "PercepcionesGravablesAnual", StringComparison.OrdinalIgnoreCase))
            {
                resultado = 0;
                string[] camposParam = new string[] {string.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.naturaleza"),
                string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.tipoAfecta"),
                string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave")};
                Object[] valoresParam = new Object[] { Naturaleza.PERCEPCION, ClavesParametrosModulos.tipoBaseAfectaISRAnual, ClavesParametrosModulos.claveBaseNominaISR };

                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), camposParam, valoresParam,
                    TipoMostrarCampo.SUMA, null, null);


                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "DiasPagoPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "DiasPagoMes", StringComparison.OrdinalIgnoreCase)
              || string.Equals(variable, "DiasPagoBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "DiasPagoAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = periodioAcumuladoPorRangoMeses(tipoAcumulado, fecha, variable);
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "DiasLaboradosPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "DiasLaboradosMes", StringComparison.OrdinalIgnoreCase)
              || string.Equals(variable, "DiasLaboradosBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "DiasLaboradosAnual", StringComparison.OrdinalIgnoreCase))
            {
                resultado = asistenciasAcumuladoPorRangoMeses(tipoAcumulado, fecha, new Object[] { ClavesParametrosModulos.claveExcepcionLaborado });
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "DiasCotizadosPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "DiasCotizadosMes", StringComparison.OrdinalIgnoreCase)
              || string.Equals(variable, "DiasCotizadosBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "DiasCotizadosAnual", StringComparison.OrdinalIgnoreCase))
            {
                resultado = periodioAcumuladoPorRangoMeses(tipoAcumulado, fecha, variable);
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "PermisosPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "PermisosMes", StringComparison.OrdinalIgnoreCase)
              || string.Equals(variable, "PermisosBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "PermisosAnual", StringComparison.OrdinalIgnoreCase))
            {
                resultado = asistenciasAcumuladoPorRangoMeses(tipoAcumulado, fecha, new Object[] { ClavesParametrosModulos.claveExcepcionPermisoConSueldo, ClavesParametrosModulos.claveExcepcionPermisoSinSueldo });
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "IncapacidadEnfermedadPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "IncapacidadEnfermedadMes", StringComparison.OrdinalIgnoreCase)
              || string.Equals(variable, "IncapacidadEnfermedadBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "IncapacidadEnfermedadAnual", StringComparison.OrdinalIgnoreCase))
            {
                resultado = asistenciasAcumuladoPorRangoMeses(tipoAcumulado, fecha, new Object[] { ClavesParametrosModulos.claveExcepcionIncapacidadPorEnfermedad });
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "IncapacidadAccidentePeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "IncapacidadAccidenteMes", StringComparison.OrdinalIgnoreCase)
              || string.Equals(variable, "IncapacidadAccidenteBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "IncapacidadAccidenteAnual", StringComparison.OrdinalIgnoreCase))
            {
                resultado = asistenciasAcumuladoPorRangoMeses(tipoAcumulado, fecha, new Object[] { ClavesParametrosModulos.claveExcepcionIncapacidadPorAccidente });
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "TotalIncapacidadesPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "IncapacidadMaternidadMes", StringComparison.OrdinalIgnoreCase)
              || string.Equals(variable, "IncapacidadMaternidadBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "IncapacidadMaternidadAnual", StringComparison.OrdinalIgnoreCase))
            {
                resultado = asistenciasAcumuladoPorRangoMeses(tipoAcumulado, fecha, new Object[] { ClavesParametrosModulos.claveExcepcionIncapacidadPorMaternidad });
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "IncapacidadMaternidadPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "TotalIncapacidadesMes", StringComparison.OrdinalIgnoreCase)
            || string.Equals(variable, "TotalIncapacidadesBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "TotalIncapacidadesAnual", StringComparison.OrdinalIgnoreCase))
            {
                resultado = asistenciasAcumuladoPorRangoMeses(tipoAcumulado, fecha, new Object[] { ClavesParametrosModulos.claveExcepcionIncapacidadPorAccidente, ClavesParametrosModulos.claveExcepcionIncapacidadPorEnfermedad, ClavesParametrosModulos.claveExcepcionIncapacidadPorMaternidad });
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "TotalFaltasPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "TotalFaltasMes", StringComparison.OrdinalIgnoreCase)
            || string.Equals(variable, "TotalFaltasBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "TotalFaltasAnual", StringComparison.OrdinalIgnoreCase))
            {
                resultado = asistenciasAcumuladoPorRangoMeses(tipoAcumulado, fecha, new Object[] { ClavesParametrosModulos.claveExcepcionIncapacidadPorAccidente, ClavesParametrosModulos.claveExcepcionIncapacidadPorEnfermedad, ClavesParametrosModulos.claveExcepcionIncapacidadPorMaternidad, /*ClavesParametrosModulos.claveExcepcionOtrasIncapacidades,*/ ClavesParametrosModulos.claveExcepcionAusentismo,
                ClavesParametrosModulos.claveExcepcionFalta, ClavesParametrosModulos.claveExcepcionPermisoConSueldo, ClavesParametrosModulos.claveExcepcionPermisoSinSueldo});
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "FaltasPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "FaltasMes", StringComparison.OrdinalIgnoreCase)
            || string.Equals(variable, "FaltasBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "FaltasAnual", StringComparison.OrdinalIgnoreCase))
            {
                resultado = asistenciasAcumuladoPorRangoMeses(tipoAcumulado, fecha, new Object[] { ClavesParametrosModulos.claveExcepcionFalta });
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "ISRACargoPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "ISRACargoMes", StringComparison.OrdinalIgnoreCase)
         || string.Equals(variable, "ISRACargoBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "ISRACargoAnual", StringComparison.OrdinalIgnoreCase))
            {
                resultado = isrAcumuladoPorRangoMeses(tipoAcumulado, fecha, "isrACargo");
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "ISRSubsidioPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "ISRSubsidioMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "ISRSubsidioBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "ISRSubsidioAnual", StringComparison.OrdinalIgnoreCase))
            {
                resultado = isrAcumuladoPorRangoMeses(tipoAcumulado, fecha, "isrSubsidio");
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "ISRNetoPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "ISRSubsidioMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "ISRSubsidioBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "ISRNetoAnual", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "ISRTotal", StringComparison.OrdinalIgnoreCase))
            {
                resultado = isrAcumuladoPorRangoMeses(tipoAcumulado, fecha, "isrNeto");
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "TotalPercepcionesPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "TotalPercepcionesMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "TotalPercepcionesBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "TotalPercepcionesAnual", StringComparison.OrdinalIgnoreCase))
            {
                resultado = 0;
                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".resultado"), new String[] { string.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.naturaleza"), }, new Object[]{
               Naturaleza.PERCEPCION
                     }, TipoMostrarCampo.SUMA, string.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.formulaConcepto#LIKE"), "TotalPercepciones");
                valoresConceptosEmpleados.Add(variable, resultado);

            }
            else if (string.Equals(variable, "TotalDeduccionesPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "TotalDeduccionesMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "TotalDeduccionesBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "TotalDeduccionesAnual", StringComparison.OrdinalIgnoreCase))
            {
                resultado = 0;
                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".resultado"), new String[] { string.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.naturaleza"), }, new Object[]{
                Naturaleza.DEDUCCION }, TipoMostrarCampo.SUMA, string.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.formulaConcepto#LIKE"), "TotalDeducciones");
                valoresConceptosEmpleados.Add(variable, resultado);

            }
            else if (string.Equals(variable, "BaseISRPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "BaseISRBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRAnual", StringComparison.OrdinalIgnoreCase))
            {
                resultado = 0;
                string suma = string.Concat(typeof(MovNomConcep).Name, string.Concat(".movNomBaseAfecta.resultado", string.Concat("+", string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultadoExento"))));
                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, suma, new String[] { string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave") }, new Object[]{
                ClavesParametrosModulos.claveBaseNominaISR}, TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseISRNormalPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRNormalMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "BaseISRNormalBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRNormalAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                string suma = string.Concat(typeof(MovNomConcep).Name, string.Concat(".movNomBaseAfecta.resultado", string.Concat("+", string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultadoExento"))));
                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, suma, new String[] { string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave") }, new Object[]{
                ClavesParametrosModulos.claveBaseNominaISR}, TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseISRDirectoPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRDirectoMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "BaseISRDirectoBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRDirectoAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                string suma = string.Concat(typeof(MovNomConcep).Name, string.Concat(".movNomBaseAfecta.resultado", string.Concat("+", string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultadoExento"))));
                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, suma, new String[] { string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave") }, new Object[]{
                ClavesParametrosModulos.claveBaseNominaISR}, TipoMostrarCampo.SUMA, null, null);
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseISRAnualPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRAnualMes", StringComparison.OrdinalIgnoreCase)
          || string.Equals(variable, "BaseISRAnualBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRAnualAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                string suma = string.Concat(typeof(MovNomConcep).Name, string.Concat(".movNomBaseAfecta.resultado", string.Concat("+", string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultadoExento"))));
                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, suma, new String[] { string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave") }, new Object[]{
                ClavesParametrosModulos.claveBaseNominaISR}, TipoMostrarCampo.SUMA, null, null);
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseISRGravablePeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRGravableMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "BaseISRGravableBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRGravableAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), new String[] { string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave") }, new Object[]{
                ClavesParametrosModulos.claveBaseNominaISR}, TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseISRGravableNormalPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRGravableNormalMes", StringComparison.OrdinalIgnoreCase)
         || string.Equals(variable, "BaseISRGravableNormalBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRGravableNormalAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), new String[] { string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave"), string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.tipoAfecta") }, new Object[]{
                ClavesParametrosModulos.claveBaseNominaISR, ClavesParametrosModulos.tipoBaseAfectaISRNormal}, TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseISRGravableDirectoPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRGravableDirectoMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "BaseISRGravableDirectoBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRGravableDirectoAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), new String[] { string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave"), string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.tipoAfecta") }, new Object[]{
                ClavesParametrosModulos.claveBaseNominaISR, ClavesParametrosModulos.tipoBaseAfectaISRDirecto}, TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseISRGravableAnualPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRGravableAnualMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "BaseISRGravableAnualBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRGravableAnualAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), new String[] { string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave"), string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.tipoAfecta") }, new Object[]{
                ClavesParametrosModulos.claveBaseNominaISR, ClavesParametrosModulos.tipoBaseAfectaISRAnual}, TipoMostrarCampo.SUMA, null, null);
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseISRExentoPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRExentoMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "BaseISRExentoBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRExentoAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultadoExento"), new String[] { string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave") }, new Object[]{
                                ClavesParametrosModulos.claveBaseNominaISR}, TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseISRExentoNormalPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRExentoNormalMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "BaseISRExentoNormalBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRExentoNormalAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultadoExento"), new String[] { string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave"), string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.tipoAfecta") }, new Object[]{
                ClavesParametrosModulos.claveBaseNominaISR, ClavesParametrosModulos.tipoBaseAfectaISRNormal}, TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseISRExentoDirectoPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRExentoDirectoMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "BaseISRExentoDirectoBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRExentoDirectoAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultadoExento"), new String[] { string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave"), string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.tipoAfecta") }, new Object[]{
                                ClavesParametrosModulos.claveBaseNominaISR, ClavesParametrosModulos.tipoBaseAfectaISRDirecto}, TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseISRExentoAnualPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRExentoAnualMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "BaseISRExentoAnualBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRExentoAnualAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultadoExento"), new String[] { string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave"), string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.tipoAfecta") }, new Object[]{
                                ClavesParametrosModulos.claveBaseNominaISR, ClavesParametrosModulos.tipoBaseAfectaISRAnual
                }, TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseIMSSPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSMes", StringComparison.OrdinalIgnoreCase)
         || string.Equals(variable, "BaseIMSSBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSAnual", StringComparison.OrdinalIgnoreCase))
            {
                string suma = string.Concat(typeof(MovNomConcep).Name, string.Concat(".movNomBaseAfecta.resultado", string.Concat("+", string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultadoExento"))));

                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, suma, new String[] { string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave") }, new Object[]{
                                ClavesParametrosModulos.claveBaseNominaISR}, TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseIMSSFijaPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSFijaMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "BaseIMSSFijaBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSFijaAnual", StringComparison.OrdinalIgnoreCase))
            {
                string suma = string.Concat(typeof(MovNomConcep).Name, string.Concat(".movNomBaseAfecta.resultado", string.Concat("+", string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultadoExento"))));
                //                String suma = MovNomConcep.class.getSimpleName().concat(".movNomBaseAfecta.resultado").concat("+").concat(MovNomConcep.class.getSimpleName().concat(".movNomBaseAfecta.resultadoExento"));
                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, suma, new String[] { string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave") }, new Object[]{
                                ClavesParametrosModulos.claveBaseNominaISR}, TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseIMSSVariablePeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSVariableMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "BaseIMSSVariableBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSVariableAnual", StringComparison.OrdinalIgnoreCase))
            {
                string suma = string.Concat(typeof(MovNomConcep).Name, string.Concat(".movNomBaseAfecta.resultado", string.Concat("+", string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultadoExento"))));
                //                String suma = MovNomConcep.class.getSimpleName().concat(".movNomBaseAfecta.resultado").concat("+").concat(MovNomConcep.class.getSimpleName().concat(".movNomBaseAfecta.resultadoExento"));
                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, suma, new String[] { string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave") }, new Object[]{
                                ClavesParametrosModulos.claveBaseNominaISR}, TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseIMSSGravadoPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSGravableMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "BaseIMSSGravableBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSGravableAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), new String[] { string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave") }, new Object[]{
                                ClavesParametrosModulos.claveBaseNominaIMSS}, TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseIMSSGravadoFijaPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSGravableFijaMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "BaseIMSSGravableFijaBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSGravableFijaAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), new String[] { string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave"), string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.tipoAfecta") }, new Object[]{
                                ClavesParametrosModulos.claveBaseNominaIMSS, ClavesParametrosModulos.tipoBaseAfectaIMSSFijo}, TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseIMSSGravadoVariablePeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSGravableVariableMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "BaseIMSSGravableVariableBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSGravableVariableAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), new String[] { string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave"), string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.tipoAfecta") }, new Object[]{
                 ClavesParametrosModulos.claveBaseNominaIMSS, ClavesParametrosModulos.tipoBaseAfectaIMSSVariable}, TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseIMSSExentoPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSExentoMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "BaseIMSSExentoBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSExentoAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultadoExento"), new String[] { string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave") }, new Object[]{
                                ClavesParametrosModulos.claveBaseNominaIMSS}, TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseIMSSExentoFijaPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSExentoFijaMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "BaseIMSSExentoFijaBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSExentoFijaAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultadoExento"), new String[] { string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave"), string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.tipoAfecta") }, new Object[]{
                                ClavesParametrosModulos.claveBaseNominaIMSS, ClavesParametrosModulos.tipoBaseAfectaIMSSFijo}, TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseIMSSExentoVariablePeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSExentoVariableMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "BaseIMSSExentoVariableBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSExentoVariableAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultadoExento"), new String[] { string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave"), string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.tipoAfecta") }, new Object[]{
                                ClavesParametrosModulos.claveBaseNominaIMSS, ClavesParametrosModulos.tipoBaseAfectaIMSSVariable}, TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseInfonavitPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseInfonavitMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "BaseInfonavitBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseInfonavitAnual", StringComparison.OrdinalIgnoreCase))
            {
                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), new String[] { string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave") }, new Object[]{
                                ClavesParametrosModulos.claveBaseNominaInfonavit}, TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BasePTUPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BasePTUMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "BasePTUBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BasePTUAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), new String[] { string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave") }, new Object[]{
                                ClavesParametrosModulos.claveBaseNominaPTU}, TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseImpuestoNominaPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseImpuestoNominaMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "BaseImpuestoNominaBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseImpuestoNominaAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), new String[] { string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave") }, new Object[]{
                                ClavesParametrosModulos.claveBaseNominaISN}, TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseDespensaPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseDespensaMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "BaseDespensaBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseDespensaAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), new String[] { string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave") }, new Object[]{
                                ClavesParametrosModulos.claveBaseNominaDespensa}, TipoMostrarCampo.SUMA, null, null);
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseFondoAhorroPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseFondoAhorroMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "BaseFondoAhorroBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseFondoAhorroAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), new String[] { string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave") }, new Object[]{
                                ClavesParametrosModulos.claveBaseNominaFondoAhorro}, TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseAguinaldoPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseAguinaldoMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "BaseAguinaldoBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseAguinaldoAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), new String[] { string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave") }, new Object[]{
                                ClavesParametrosModulos.claveBaseNominaAguinaldo}, TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseOtrosPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseOtrosMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "BaseOtrosBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseOtrosAnual", StringComparison.OrdinalIgnoreCase))
            {

                variablesExtras.Add(string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.reservado"));
                valoresExtras.Add(false);
                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), variablesExtras.ToArray<string>(), valoresExtras.ToArray(), TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "PercepcionesExentas", StringComparison.OrdinalIgnoreCase))
            {

                String[] camposParam = new String[] { string.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.naturaleza") };
                Object[] valoresParam = new Object[] { Naturaleza.PERCEPCION };
                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultadoExento"), camposParam, valoresParam,
                TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "DeduccionesGravables", StringComparison.OrdinalIgnoreCase))
            {

                String[] camposParam = new String[] { string.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.naturaleza") };
                Object[] valoresParam = new Object[] { Naturaleza.DEDUCCION };
                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), camposParam, valoresParam,
                                TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "DeduccionesExentas", StringComparison.OrdinalIgnoreCase))
            {

                String[] camposParam = new String[] { string.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.naturaleza") };
                Object[] valoresParam = new Object[] { Naturaleza.DEDUCCION };
                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultadoExento"), camposParam, valoresParam,
                TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "PercepcionesGravadasPer", StringComparison.OrdinalIgnoreCase))
            {

                String[] camposParam = new String[] { string.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.naturaleza") };
                Object[] valoresParam = new Object[] { Naturaleza.PERCEPCION };
                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), camposParam, valoresParam,
                                TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "PercepcionesGravadasMesActual", StringComparison.OrdinalIgnoreCase))
            {

                String[] camposParam = new String[] { string.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.naturaleza") };
                Object[] valoresParam = new Object[] { Naturaleza.PERCEPCION };
                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), camposParam, valoresParam,
                                TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "PercepcionesGravadasMesAnterior", StringComparison.OrdinalIgnoreCase))
            {
                clavePeriodoFuncion = "Anterior";
                fecha = new DateTime(fecha.Year, fecha.Month - 1, fecha.Day);
                String[] camposParam = new String[] { string.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.naturaleza") };
                Object[] valoresParam = new Object[] { Naturaleza.PERCEPCION };
                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), camposParam, valoresParam,
                                TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "PercepcionesExentaPer", StringComparison.OrdinalIgnoreCase))
            {
                //clavePeriodoFuncion = "Anterior";
                //fecha = new DateTime(fecha.Year, fecha.Month - 1, fecha.Day);
                String[] camposParam = new String[] { string.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.naturaleza") };
                Object[] valoresParam = new Object[] { Naturaleza.PERCEPCION };
                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultadoExento"), camposParam, valoresParam,
                                TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "PercepcionesExentaMesActual", StringComparison.OrdinalIgnoreCase))
            {
                //clavePeriodoFuncion = "Anterior";
                //fecha = new DateTime(fecha.Year, fecha.Month - 1, fecha.Day);
                String[] camposParam = new String[] { string.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.naturaleza") };
                Object[] valoresParam = new Object[] { Naturaleza.PERCEPCION };
                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultadoExento"), camposParam, valoresParam,
                                TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "PercepcionesExentaMesAnterior", StringComparison.OrdinalIgnoreCase))
            {
                clavePeriodoFuncion = "Anterior";
                fecha = new DateTime(fecha.Year, fecha.Month - 1, fecha.Day);
                String[] camposParam = new String[] { string.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.naturaleza") };
                Object[] valoresParam = new Object[] { Naturaleza.PERCEPCION };
                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultadoExento"), camposParam, valoresParam,
                                TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "DeduccionesGravadasPer", StringComparison.OrdinalIgnoreCase))
            {
                //clavePeriodoFuncion = "Anterior";
                //fecha = new DateTime(fecha.Year, fecha.Month - 1, fecha.Day);
                String[] camposParam = new String[] { string.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.naturaleza") };
                Object[] valoresParam = new Object[] { Naturaleza.DEDUCCION };
                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), camposParam, valoresParam,
                                TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "DeduccionesGravadasMesActual", StringComparison.OrdinalIgnoreCase))
            {
                //clavePeriodoFuncion = "Anterior";
                //fecha = new DateTime(fecha.Year, fecha.Month - 1, fecha.Day);
                String[] camposParam = new String[] { string.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.naturaleza") };
                Object[] valoresParam = new Object[] { Naturaleza.DEDUCCION };
                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), camposParam, valoresParam,
                                TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "DeduccionesGravadasMesAnterior", StringComparison.OrdinalIgnoreCase))
            {
                clavePeriodoFuncion = "Anterior";
                fecha = new DateTime(fecha.Year, fecha.Month - 1, fecha.Day);
                String[] camposParam = new String[] { string.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.naturaleza") };
                Object[] valoresParam = new Object[] { Naturaleza.DEDUCCION };
                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), camposParam, valoresParam,
                                TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "DeduccionesExentasPer", StringComparison.OrdinalIgnoreCase))
            {
                //clavePeriodoFuncion = "Anterior";
                //fecha = new DateTime(fecha.Year, fecha.Month - 1, fecha.Day);
                String[] camposParam = new String[] { string.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.naturaleza") };
                Object[] valoresParam = new Object[] { Naturaleza.DEDUCCION };
                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultadoExento"), camposParam, valoresParam,
                                TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "DeduccionesExentasMesActual", StringComparison.OrdinalIgnoreCase))
            {
                //clavePeriodoFuncion = "Anterior";
                //fecha = new DateTime(fecha.Year, fecha.Month - 1, fecha.Day);
                String[] camposParam = new String[] { string.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.naturaleza") };
                Object[] valoresParam = new Object[] { Naturaleza.DEDUCCION };
                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultadoExento"), camposParam, valoresParam,
                                TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "DeduccionesExentasMesAnterior", StringComparison.OrdinalIgnoreCase))
            {
                clavePeriodoFuncion = "Anterior";
                fecha = new DateTime(fecha.Year, fecha.Month - 1, fecha.Day);
                String[] camposParam = new String[] { string.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.naturaleza") };
                Object[] valoresParam = new Object[] { Naturaleza.DEDUCCION };
                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultadoExento"), camposParam, valoresParam,
                                TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "ISRNormal", StringComparison.OrdinalIgnoreCase))
            {
                resultado = isrAcumuladoPorRangoMeses(tipoAcumulado, fecha, "isrNetoNormal");
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "ISRAnual", StringComparison.OrdinalIgnoreCase))
            {
                resultado = isrAcumuladoPorRangoMeses(tipoAcumulado, fecha, "isrNetoAnual");
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "ISRDirectoTabla", StringComparison.OrdinalIgnoreCase))
            {
                resultado = isrAcumuladoPorRangoMeses(tipoAcumulado, fecha, "isrNetoDirecto");
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "PercepcionesGravablesNor", StringComparison.OrdinalIgnoreCase))
            {


                String[] camposParam = new String[] {string.Concat(typeof(MovNomConcep).Name,".concepNomDefi.naturaleza"),
                            string.Concat(typeof(MovNomConcep).Name,".movNomBaseAfecta.baseAfecConcepNom.tipoAfecta"),
                            string.Concat(typeof(MovNomConcep).Name,".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave")};
                Object[] valoresParam = new Object[] { Naturaleza.PERCEPCION, ClavesParametrosModulos.tipoBaseAfectaISRNormal, ClavesParametrosModulos.claveBaseNominaISR };
                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), camposParam, valoresParam,
                                TipoMostrarCampo.SUMA, null, null);

                valoresConceptosEmpleados.Add(variable, resultado);
            }

        }

        private Object asistenciasAcumuladoPorRangoMeses(TipoClasificacionFormula tipoAcumulado, DateTime fechaPeriodoNomina, Object[] claveExcepcion)
        {
            Object valor = 0.0;
            try
            {
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                strQuery.Remove(0, strQuery.Length).Append("SELECT COUNT(a) FROM ").Append(typeof(Asistencias).Name).Append(" a INNER JOIN a.excepciones ex INNER JOIN a.empleados em INNER JOIN a.razonesSociales rs INNER JOIN a.tipoNomina t ");
                strQuery.Append("INNER JOIN a.periodosNomina p Left Outer Join a.centroDeCosto cc WHERE em.clave = :claveEmp AND rs.clave = :razonSocial AND t.clave = :claveTipoNomina AND p.tipoCorrida.clave = :claveTipoCorrida AND ex.clave IN (:claveExcepcion) ");
                String cc = valoresConceptosEmpleados[typeof(CentroDeCosto).Name.ToUpper()] == null ? "" : valoresConceptosEmpleados[typeof(CentroDeCosto).Name.ToUpper().ToUpper()].ToString();
                if (cc.Trim().Length > 0)
                {
                    strQuery.Append("AND cc.clave = :CentroDeCosto ");
                    camposParametro.Add("CentroDeCosto");
                    valoresParametro.Add(valoresConceptosEmpleados[typeof(CentroDeCosto).Name.ToUpper()]);
                }

                DateTime fechaRango = DateTime.Now;
                fechaRango = fechaPeriodoNomina;
                int mesIni = -1, mesFin = -1;
                if (tipoAcumulado == TipoClasificacionFormula.DATOPERIODO)
                {
                    DateTime fechaIni = (DateTime)valoresConceptosEmpleados[parametroFechaInicial];
                    DateTime fechafin = (DateTime)valoresConceptosEmpleados[parametroFechaFinal];
                    strQuery.Append("AND a.fecha BETWEEN :fechaInicial AND :fechaFinal ");
                    camposParametro.Add("fechaInicial");
                    valoresParametro.Add(fechaIni.Date);
                    camposParametro.Add("fechaFinal");
                    valoresParametro.Add(fechafin.Date);
                }
                else if (tipoAcumulado == TipoClasificacionFormula.DATOBIMESTRAL)
                {
                    if ((fechaPeriodoNomina.Month + 1) % 2 == 0)
                    {
                        mesFin = fechaPeriodoNomina.Month + 1;
                        mesIni = fechaPeriodoNomina.Month;
                        fechaRango.AddMonths(fechaPeriodoNomina.Month - 1);
                    }
                    else
                    {
                        mesFin = fechaPeriodoNomina.Month + 2;
                        mesIni = fechaPeriodoNomina.Month + 1;
                    }

                }
                else if (tipoAcumulado == TipoClasificacionFormula.DATOMENSUAL)
                {
                    mesFin = fechaPeriodoNomina.Month + 1;
                    mesIni = fechaPeriodoNomina.Month + 1;

                }
                else if (tipoAcumulado == TipoClasificacionFormula.DATOANUAL)
                {
                    DateTime fechaIni = (DateTime)valoresConceptosEmpleados[parametroFechaInicial];
                    DateTime fechafin = (DateTime)valoresConceptosEmpleados[parametroFechaFinal];
                    fechaIni = new DateTime(fechaIni.Year, 1, 1);
                    strQuery.Append("AND a.fecha BETWEEN :fechaInicial AND :fechaFinal ");
                    camposParametro.Add("fechaInicial");
                    valoresParametro.Add(fechaIni.Date);
                    camposParametro.Add("fechaFinal");
                    valoresParametro.Add(fechafin.Date);
                }
                camposParametro.Add("claveEmp");
                camposParametro.Add("razonSocial");
                camposParametro.Add("claveTipoNomina");
                camposParametro.Add("claveTipoCorrida");
                camposParametro.Add("claveExcepcion");
                valoresParametro.Add(valoresConceptosEmpleados["NumEmpleado".ToUpper()]);
                valoresParametro.Add(valoresConceptosEmpleados["RazonSocial".ToUpper()]);
                valoresParametro.Add(valoresConceptosEmpleados["TipoNomina".ToUpper()]);
                valoresParametro.Add(valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()]);
                valoresParametro.Add(claveExcepcion);
                if (tipoAcumulado == TipoClasificacionFormula.DATOBIMESTRAL || tipoAcumulado == TipoClasificacionFormula.DATOMENSUAL)
                {
                    DateTime fecha = (DateTime)valoresConceptosEmpleados[parametroFechaFinal];
                    if (fechaRango.Month == 1)
                    {
                        strQuery.Append("AND (MONTH(p.AcumularAMes) <= :mesPeriodoRango AND YEAR(p.AcumularAMes) = :yearPeriodo) ");
                        strQuery.Append("OR (t.clave = :claveTipoNomina AND p.año = :yearPeriodo - 1 AND MONTH(p.AcumularAMes) = :mesPeriodo AND YEAR(p.AcumularAMes) = :yearPeriodo) ");

                    }
                    else
                    {
                        strQuery.Append("AND (MONTH(p.AcumularAMes) >= :mesPeriodo AND MONTH(p.AcumularAMes) <= :mesPeriodoRango) AND p.año = :yearPeriodo ");
                    }
                    camposParametro.Add("mesPeriodo");
                    camposParametro.Add("mesPeriodoRango");
                    valoresParametro.Add(mesIni);
                    valoresParametro.Add(mesFin);
                    camposParametro.Add("yearPeriodo");

                    valoresParametro.Add(fecha.Year);
                }
                valor = (Object)ejecutaQueryUnico(strQuery.ToString(), camposParametro.ToArray<string>(), valoresParametro.ToArray());
                if (mensajeResultado.noError == -100)
                {
                    mensajeResultado.noError = 62;
                }

            }
            catch (Exception ex)
            {
                mensajeResultado.noError = 62;

            }
            camposParametro = null;
            valoresParametro = null;
            //strQuery = null;
            return valor;
        }

        private Object isrAcumuladoPorRangoMeses(TipoClasificacionFormula tipoAcumulado, DateTime fechaPeriodoNomina, String campo)
        {
            Object valor = 0.0;
            try
            {
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                strQuery.Remove(0, strQuery.Length).Append("SELECT CASE WHEN (COUNT(isr) = 0) THEN 0.0 ELSE SUM(isr.").Append(campo).Append(") END FROM ").Append(typeof(CalculoISR).Name);
                strQuery.Append(" isr INNER JOIN isr.movNomConcep mov INNER JOIN mov.empleado em INNER JOIN mov.razonesSociales rs INNER JOIN mov.concepNomDefi con ");
                strQuery.Append(" INNER JOIN mov.tipoCorrida tipoCorri ");
                if (usaFiniquitos)
                {
                    if (camposFiniquitos != null)
                    {
                        strQuery.Append("LEFT OUTER JOIN mov.finiqLiquidCncNom fc LEFT OUTER JOIN fc.finiquitosLiquidacion fl ");
                    }
                }
                strQuery.Append(" INNER JOIN mov.periodosNomina p INNER JOIN mov.tipoNomina t Left Outer Join mov.centroDeCosto cc WHERE em.clave = :claveEmp AND rs.clave = :razonSocial ");
                camposParametro.Add("claveEmp");
                camposParametro.Add("razonSocial");
                valoresParametro.Add(valoresConceptosEmpleados["NumEmpleado".ToUpper()]);
                valoresParametro.Add(valoresConceptosEmpleados["RazonSocial".ToUpper()]);
                String cc = valoresConceptosEmpleados[typeof(CentroDeCosto).Name.ToUpper()] == null ? "" : valoresConceptosEmpleados[typeof(CentroDeCosto).Name.ToUpper().ToUpper()].ToString();
                if (cc.Trim().Length > 0)
                {
                    strQuery.Append("AND cc.clave = :CentroDeCosto ");
                    camposParametro.Add("CentroDeCosto");
                    valoresParametro.Add(valoresConceptosEmpleados["CentroDeCosto".ToUpper()]);
                }
                if (valoresConceptosEmpleados.ContainsKey("TipoNomina".ToUpper()))
                {
                    String nomina = valoresConceptosEmpleados["TipoNomina".ToUpper()] == null ? "" : valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString();
                    if (nomina.Length > 0)
                    {
                        strQuery.Append("AND t.clave = :TipoNomina ");
                        camposParametro.Add("TipoNomina");
                        valoresParametro.Add(nomina);
                    }
                }
                if (valoresConceptosEmpleados["uso".ToUpper()] != null)
                {
                    strQuery.Append(" AND mov.uso = :uso ");
                    camposParametro.Add("uso");
                    valoresParametro.Add(valoresConceptosEmpleados["uso".ToUpper()]);
                }

                if (valoresConceptosEmpleados["ImprimeListado".ToUpper()] != null)
                {
                    strQuery.Append(" AND con.imprimirEnListadoNomina = :ImprimeListado ");
                    camposParametro.Add("ImprimeListado");
                    valoresParametro.Add(valoresConceptosEmpleados["ImprimeListado".ToUpper()]);
                }

                if (valoresConceptosEmpleados["ImprimeRecibo".ToUpper()] != null)
                {
                    strQuery.Append(" AND con.imprimirEnReciboNomina = :ImprimeRecibo ");
                    camposParametro.Add("ImprimeRecibo");
                    valoresParametro.Add(valoresConceptosEmpleados["ImprimeRecibo".ToUpper()]);
                }

                if (valoresConceptosEmpleados.ContainsKey("ClaveTipoCorrida".ToUpper()))
                {
                    String corrida = valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()] == null ? "" : valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()].ToString();
                    if (corrida.Length > 0)
                    {
                        strQuery.Append(" AND tipoCorri.clave = :claveTipoCorrida AND p.tipoCorrida.clave = :claveTipoCorrida ");
                        camposParametro.Add("claveTipoCorrida");
                        valoresParametro.Add(corrida);
                    }
                }

                if (usaFiniquitos)
                {
                    int i;
                    if (camposFiniquitos != null & valoresCamposFiniquitos != null)
                    {
                        String campoFiniq;
                        for (i = 0; i < camposFiniquitos.Count; i++)
                        {
                            campoFiniq = getCampoFinal(camposFiniquitos[i]);
                            strQuery.Append(" AND fl.").Append(campoFiniq).Append(" = :parametro").Append(i);
                            camposParametro.Add(string.Concat("parametro", i.ToString()));
                            valoresParametro.Add(valoresCamposFiniquitos[i]);
                        }
                    }
                }

                DateTime fechaRango = DateTime.Now;
                fechaRango = fechaPeriodoNomina;
                int mesIni = -1, mesFin = -1;
                if (tipoAcumulado == TipoClasificacionFormula.DATOPERIODO)
                {
                    strQuery.Append("AND p.clave = (SELECT periodo.clave FROM ").Append(typeof(PeriodosNomina).Name).Append(" periodo INNER JOIN periodo.tipoNomina nomina ");
                    strQuery.Append("WHERE (:fechaActual BETWEEN periodo.fechaInicial AND periodo.fechaFinal + 1) AND nomina.clave = :claveTipoNomina ");
                    if (valoresConceptosEmpleados.ContainsKey("ClaveTipoCorrida".ToUpper()))
                    {
                        String corrida = valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()] == null ? "" : valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()].ToString();
                        if (corrida.Length > 0)
                        {
                            strQuery.Append(" AND periodo.tipoCorrida.clave = :claveTipoCorrida ");
                        }
                    }
                    strQuery.Append(") AND p.año = :yearPeriodo ");
                    camposParametro.Add("fechaActual");
                    valoresParametro.Add(fechaPeriodoNomina.Date);
                    camposParametro.Add("claveTipoNomina");
                    valoresParametro.Add(valoresConceptosEmpleados["TipoNomina".ToUpper()]);
                    camposParametro.Add("yearPeriodo");
                    DateTime fecha = (DateTime)valoresConceptosEmpleados[parametroFechaFinal];
                    valoresParametro.Add(fecha.Year);
                }
                else if (tipoAcumulado == TipoClasificacionFormula.DATOBIMESTRAL)
                {
                    if ((fechaPeriodoNomina.Month + 1) % 2 == 0)
                    {
                        mesFin = fechaPeriodoNomina.Month + 1;
                        mesIni = fechaPeriodoNomina.Month;
                        fechaRango.AddMonths(fechaPeriodoNomina.Month - 1);
                    }
                    else
                    {
                        mesFin = fechaPeriodoNomina.Month + 2;
                        mesIni = fechaPeriodoNomina.Month + 1;

                    }

                }
                else if (tipoAcumulado == TipoClasificacionFormula.DATOANUAL)
                {
                    DateTime fecha = (DateTime)valoresConceptosEmpleados[parametroFechaInicial];
                    fecha = new DateTime(fecha.Year, 1, 1);
                    DateTime fechafinal = (DateTime)valoresConceptosEmpleados[parametroFechaFinal];
                    strQuery.Append("AND ((p.fechaInicial BETWEEN :fechaInicial AND :fechaFinal) ");
                    strQuery.Append("OR (p.fechaFinal BETWEEN :fechaInicial AND :fechaFinal)) ");
                    camposParametro.Add("fechaInicial");

                    valoresParametro.Add(fecha.Date);
                    camposParametro.Add("fechaFinal");

                    valoresParametro.Add(fechafinal.Date);
                }

                if (tipoAcumulado == TipoClasificacionFormula.DATOBIMESTRAL)
                {

                    strQuery.Append(" AND p.año = :yearPeriodo AND (mov.mes = :mesIni OR mov.mes = :mesFin) ");
                    camposParametro.Add("mesIni");
                    camposParametro.Add("mesFin");
                    valoresParametro.Add(mesIni);
                    valoresParametro.Add(mesFin);
                    camposParametro.Add("yearPeriodo");
                    DateTime fecha = ((DateTime)valoresConceptosEmpleados[parametroFechaFinal]);
                    valoresParametro.Add(fecha.Year);
                }
                else if (tipoAcumulado == TipoClasificacionFormula.DATOMENSUAL)
                {
                    strQuery.Append(" AND p.año = :yearPeriodo AND mov.mes = :mesActual  ");
                    camposParametro.Add("yearPeriodo");
                    DateTime fecha = ((DateTime)valoresConceptosEmpleados[parametroFechaFinal]);
                    valoresParametro.Add(fecha.Year);
                    camposParametro.Add("mesActual");
                    valoresParametro.Add(fecha.Month);
                }

                valor = (Object)ejecutaQueryUnico(strQuery.ToString(), camposParametro.ToArray<string>(), valoresParametro.ToArray());
                if (mensajeResultado.noError == -100)
                {
                    mensajeResultado.noError = 62;
                }


            }
            catch (Exception ex)
            {
                mensajeResultado.noError = 62;

            }
            camposParametro = null;
            valoresParametro = null;
            //strQuery = null;
            return valor;
        }

        private String getCampoFinal(String campoSelect)
        {
            String[] ruta = campoSelect.Split('.');
            campoSelect = ruta[ruta.Length - 1];
            return campoSelect;
        }

        private Object periodioAcumuladoPorRangoMeses(TipoClasificacionFormula tipoAcumulado, DateTime fechaPeriodoNomina, String variable)
        {
            Object valor = 0.0;
            try
            {
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                strQuery.Remove(0, strQuery.Length).Append("SELECT p FROM ").Append(typeof(PeriodosNomina).Name).Append(" p INNER JOIN p.tipoNomina t WHERE t.clave = :claveTipoNomina AND p.año = :yearPeriodo ");
                DateTime fechaRango = DateTime.Now;
                fechaRango = fechaPeriodoNomina;
                int mesIni = -1, mesFin = -1;
                if (tipoAcumulado == TipoClasificacionFormula.DATOPERIODO)
                {
                    strQuery.Append("AND p.clave = (SELECT periodo.clave FROM ").Append(typeof(PeriodosNomina).Name).Append(" periodo INNER JOIN periodo.tipoNomina nomina ");
                    strQuery.Append("WHERE (:fechaActual BETWEEN periodo.fechaInicial AND periodo.fechaFinal + 1) AND nomina.clave = :claveTipoNomina)");
                    camposParametro.Add("fechaActual");
                    valoresParametro.Add(fechaPeriodoNomina.Date);
                }
                else if (tipoAcumulado == TipoClasificacionFormula.DATOBIMESTRAL)
                {
                    if ((fechaPeriodoNomina.Month + 1) % 2 == 0)
                    {
                        mesFin = fechaPeriodoNomina.Month + 1;
                        mesIni = fechaPeriodoNomina.Month;
                        fechaRango.AddMonths(fechaPeriodoNomina.Month - 1);
                    }
                    else
                    {

                        mesFin = fechaPeriodoNomina.Month + 2;
                        mesIni = fechaPeriodoNomina.Month + 1;
                    }
                }
                else if (tipoAcumulado == TipoClasificacionFormula.DATOMENSUAL)
                {

                    mesFin = fechaPeriodoNomina.Month + 1;
                    mesIni = fechaPeriodoNomina.Month + 1;
                }
                else if (tipoAcumulado == TipoClasificacionFormula.DATOANUAL)
                {
                    DateTime fecha = (DateTime)valoresConceptosGlobales[parametroFechaInicial];
                    fecha = new DateTime(fecha.Year, 1, 1);
                    DateTime fechafinal = (DateTime)valoresConceptosGlobales[parametroFechaFinal];
                    strQuery.Append("AND p.fechaInicial >= :fechaInicial AND p.fechaFinal <= :fechaFinal OR (:fechaFinal BETWEEN p.fechaInicial AND p.fechaFinal AND t.clave = :claveTipoNomina AND p.año = :yearPeriodo) ");
                    camposParametro.Add("fechaInicial");
                    valoresParametro.Add(fecha.Date);
                    camposParametro.Add("fechaFinal");
                    valoresParametro.Add(fechafinal.Date);

                }
                camposParametro.Add("claveTipoNomina");
                camposParametro.Add("yearPeriodo");
                valoresParametro.Add(valoresConceptosEmpleados["TipoNomina".ToUpper()]);
                valoresParametro.Add(fechaPeriodoNomina.Year);
                if (tipoAcumulado == TipoClasificacionFormula.DATOBIMESTRAL || tipoAcumulado == TipoClasificacionFormula.DATOMENSUAL)
                {
                    DateTime fecha = (DateTime)valoresConceptosGlobales[parametroFechaFinal];
                    if (fechaRango.Month == 1)
                    {
                        strQuery.Append("AND (MONTH(p.AcumularAMes) <= :mesPeriodoRango AND YEAR(p.AcumularAMes) = :yearPeriodo) ");
                        strQuery.Append("OR (t.clave = :claveTipoNomina AND p.año = :yearPeriodo - 1 AND MONTH(p.AcumularAMes) = :mesPeriodo AND YEAR(p.AcumularAMes) = :yearPeriodo) ");
                    }
                    else
                    {
                        strQuery.Append("AND (MONTH(p.AcumularAMes) >= :mesPeriodo AND MONTH(p.AcumularAMes) <= :mesPeriodoRango) AND p.año = :yearPeriodo ");

                    }
                    camposParametro.Add("mesPeriodo");
                    camposParametro.Add("mesPeriodoRango");
                    valoresParametro.Add(mesIni);
                    valoresParametro.Add(mesFin);
                    camposParametro.Add("yearPeriodo");
                    valoresParametro.Add(fecha.Year);
                }

                IQuery q = getSession().CreateQuery(strQuery.ToString());
                q = cargarParametrosQuery(q, camposParametro, valoresParametro);
                IList<PeriodosNomina> periodosNominas = q.List<PeriodosNomina>();
                periodosNominas = (periodosNominas == null) ? new List<PeriodosNomina>() : periodosNominas;

                int dias = 0, diasDif = 0;

                foreach (PeriodosNomina p in periodosNominas)
                {
                    if (manejaPagoDiasNaturales || manejaPagoIMSSDiasNaturales)
                    {
                        dias = dias + (cantidadDiasEntreDosFechas(p.fechaInicial.GetValueOrDefault(), p.fechaFinal.GetValueOrDefault()) + 1);
                    }
                    else
                    {
                        dias = dias + Convert.ToInt32(p.tipoNomina.periodicidad.dias);
                    }

                }
                if (periodosNominas.Count > 0)
                {
                    DateTime fechaInicial = DateTime.Now, fechaIMSS = DateTime.Now, fechaBaja = DateTime.Now;
                    fechaInicial = (DateTime)valoresConceptosEmpleados["FechaAlta".ToUpper()];
                    fechaIMSS = (DateTime)valoresConceptosEmpleados["FechaAltaIMSS".ToUpper()];
                    fechaBaja = (DateTime)valoresConceptosEmpleados["FechaBaja".ToUpper()];
                    if (variable.ToUpper().Contains("DiasPago".ToUpper()))
                    {
                        if (fechaIMSS.CompareTo(periodosNominas[0].fechaInicial) > 0)
                        {
                            diasDif += cantidadDiasEntreDosFechas(Convert.ToDateTime(periodosNominas[0].fechaInicial), fechaIMSS);
                        }

                    }
                    else if (variable.ToUpper().Contains("DiasCotizados".ToUpper()))
                    {
                        if (fechaIMSS.CompareTo(periodosNominas[0].fechaInicial) > 0)
                        {
                            diasDif += cantidadDiasEntreDosFechas(Convert.ToDateTime(periodosNominas[0].fechaInicial), fechaIMSS);
                        }

                    }
                    if (fechaBaja.CompareTo(periodosNominas[periodosNominas.Count() - 1].fechaFinal) < 0)
                    {
                        if (fechaPeriodoNomina.CompareTo(fechaBaja) < 0)
                        {
                            diasDif += cantidadDiasEntreDosFechas(fechaPeriodoNomina, Convert.ToDateTime(periodosNominas[periodosNominas.Count() - 1].fechaFinal));
                        }
                        else
                        {
                            diasDif += cantidadDiasEntreDosFechas(fechaBaja, Convert.ToDateTime(periodosNominas[periodosNominas.Count() - 1].fechaFinal));
                        }

                    }
                    else if (fechaPeriodoNomina.CompareTo(periodosNominas[periodosNominas.Count() - 1].fechaFinal) < 0)
                    {

                        diasDif += cantidadDiasEntreDosFechas(fechaPeriodoNomina, Convert.ToDateTime(periodosNominas[periodosNominas.Count() - 1].fechaFinal));
                    }

                }
                valor = dias - diasDif;
                if (mensajeResultado.noError == 100)
                {
                    mensajeResultado.noError = 62;
                }

            }
            catch (Exception ex)
            {
                mensajeResultado.noError = 62;

            }
            camposParametro = null;
            valoresParametro = null;
            //strQuery = null;
            return valor;
        }

        private void variablesTipoFuncion(String variable, TipoClasificacionFormula tipoAcumulado, DateTime fechaPeriodo)
        {
            Object resultado;
            String funcion = variable;
            String parametroFuncion = variable.Substring(funcion.IndexOf("(") + 1, funcion.IndexOf(")")).Replace("'", "");
            String funcionNombre = variable.Substring(0, funcion.IndexOf("("));
            String[] parametrosFuncion = parametroFuncion.Split(',');
            List<String> camposFuncion = new List<string>();
            List<Object> valoresFuncion = new List<object>();
            int posAcum = 1;
            clavePeriodoFuncion = "";
            bool usaBase = false;
            String nombreBase = "";
            if (string.Equals(funcionNombre, "ACUMCNC", StringComparison.OrdinalIgnoreCase))
            {
                //Concepto
                camposFuncion.Add(string.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.clave"));
                valoresFuncion.Add(parametrosFuncion[0].Replace("'", ""));
            }
            else if (string.Equals(funcionNombre, "ACUMBASE", StringComparison.OrdinalIgnoreCase))
            {
                //Base
                usaBase = true;
                nombreBase = parametrosFuncion[0];
            }
            else if (string.Equals(funcionNombre, "ACUMCNCBASE", StringComparison.OrdinalIgnoreCase))
            {
                //Concepto,Base
                camposFuncion.Add(string.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.clave"));
                valoresFuncion.Add(parametrosFuncion[0]);
                nombreBase = parametrosFuncion[1];
                posAcum = 2;
            }
            else if (string.Equals(funcionNombre, "DEDUCCREDITOS", StringComparison.OrdinalIgnoreCase))
            {
                //creditos
                camposFuncion.Add(string.Concat(typeof(MovNomConcep).Name, ".creditoMovimientos.creditoPorEmpleado.creditoAhorro.concepNomiDefin.clave"));
                camposFuncion.Add(string.Concat(typeof(MovNomConcep).Name, ".creditoMovimientos.creditoPorEmpleado.creditoAhorro.tipoConfiguracion"));
                valoresFuncion.Add(parametrosFuncion[0]);
                valoresFuncion.Add("1");
            }
            else if (string.Equals(funcionNombre, "DEDUCAHORROS", StringComparison.OrdinalIgnoreCase))
            {
                //Ahorros
                camposFuncion.Add(string.Concat(typeof(MovNomConcep).Name, ".creditoMovimientos.creditoPorEmpleado.creditoAhorro.concepNomiDefin.clave"));
                camposFuncion.Add(string.Concat(typeof(MovNomConcep).Name, ".creditoMovimientos.creditoPorEmpleado.creditoAhorro.tipoConfiguracion"));
                valoresFuncion.Add(parametrosFuncion[0]);
                valoresFuncion.Add("2");
            }

            if (usaBase)
            {
                camposFuncion.Add(string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.baseAfecConcepNom.baseNomina.clave"));

                if (string.Equals(nombreBase, "ISR", StringComparison.OrdinalIgnoreCase))
                {
                    valoresFuncion.Add(ClavesParametrosModulos.claveBaseNominaISR);
                }
                else if (string.Equals(nombreBase, "IMSS", StringComparison.OrdinalIgnoreCase))
                {
                    valoresFuncion.Add(ClavesParametrosModulos.claveBaseNominaIMSS);
                }
                else if (string.Equals(nombreBase, "INF", StringComparison.OrdinalIgnoreCase))
                {
                    valoresFuncion.Add(ClavesParametrosModulos.claveBaseNominaInfonavit);
                }
                else if (string.Equals(nombreBase, "PTU", StringComparison.OrdinalIgnoreCase))
                {
                    valoresFuncion.Add(ClavesParametrosModulos.claveBaseNominaPTU);
                }
                else if (string.Equals(nombreBase, "ISN", StringComparison.OrdinalIgnoreCase))
                {
                    valoresFuncion.Add(ClavesParametrosModulos.claveBaseNominaISN);
                }
                else if (string.Equals(nombreBase, "DES", StringComparison.OrdinalIgnoreCase))
                {
                    valoresFuncion.Add(ClavesParametrosModulos.claveBaseNominaDespensa);
                }
                else if (string.Equals(nombreBase, "AHO", StringComparison.OrdinalIgnoreCase))
                {
                    valoresFuncion.Add(ClavesParametrosModulos.claveBaseNominaFondoAhorro);
                }
                else if (string.Equals(nombreBase, "AGUI", StringComparison.OrdinalIgnoreCase))
                {
                    valoresFuncion.Add(ClavesParametrosModulos.claveBaseNominaAguinaldo);
                }
                else
                {
                    valoresFuncion.Add(ClavesParametrosModulos.claveBaseNominaISR);
                }
            }
            ////TipoAcumulado
            if (string.Equals(parametrosFuncion[posAcum], "MENSUAL", StringComparison.OrdinalIgnoreCase))
            {
                tipoAcumulado = TipoClasificacionFormula.DATOMENSUAL;
            }
            else if (string.Equals(parametrosFuncion[posAcum], "BIMESTRAL", StringComparison.OrdinalIgnoreCase))
            {
                tipoAcumulado = TipoClasificacionFormula.DATOBIMESTRAL;
            }
            else if (string.Equals(parametrosFuncion[posAcum], "ANUAL", StringComparison.OrdinalIgnoreCase))
            {
                tipoAcumulado = TipoClasificacionFormula.DATOANUAL;
            }
            else
            {
                tipoAcumulado = TipoClasificacionFormula.DATOPERIODO;
            }


            ///Ejercicio
            if (string.Equals(parametrosFuncion[posAcum + 1], "ANTERIOR", StringComparison.OrdinalIgnoreCase))
            {
                clavePeriodoFuncion = parametrosFuncion[posAcum + 1];
                fechaPeriodo = fechaPeriodo.AddYears(1);
            }
            else if (isNumericaString(parametrosFuncion[posAcum + 1]))
            {

                clavePeriodoFuncion = parametrosFuncion[posAcum + 1];
                fechaPeriodo = fechaPeriodo.AddYears(Convert.ToInt32(parametrosFuncion[posAcum + 1]));
            }
            //Numero
            if (string.Equals(parametrosFuncion[posAcum + 2], "ANTERIOR", StringComparison.OrdinalIgnoreCase))
            {

                clavePeriodoFuncion = parametrosFuncion[posAcum + 2];
                if (tipoAcumulado == TipoClasificacionFormula.DATOMENSUAL)
                {
                    fechaPeriodo = fechaPeriodo.AddMonths(-1);
                }
                else if (tipoAcumulado == TipoClasificacionFormula.DATOBIMESTRAL)
                {
                    if ((fechaPeriodo.Month + 1) % 2 == 0)
                    {
                        fechaPeriodo = fechaPeriodo.AddMonths(-3);
                    }
                    else
                    {
                        fechaPeriodo = fechaPeriodo.AddMonths(-2);
                    }
                }
            }
            else if (isNumericaString(parametrosFuncion[posAcum + 2]))
            {
                clavePeriodoFuncion = parametrosFuncion[posAcum + 2].Replace("'", "");
                if (tipoAcumulado == TipoClasificacionFormula.DATOMENSUAL)
                {
                    fechaPeriodo = fechaPeriodo.AddMonths(Convert.ToInt32(parametrosFuncion[posAcum + 2]) - 1);
                }
                else if (tipoAcumulado == TipoClasificacionFormula.DATOBIMESTRAL)
                {
                    int bimestre = Convert.ToInt32(parametrosFuncion[posAcum + 2]);
                    switch (bimestre)
                    {
                        case 1:

                            fechaPeriodo = new DateTime(fechaPeriodo.Year, 1, fechaPeriodo.Day);
                            break;
                        case 2:

                            fechaPeriodo = new DateTime(fechaPeriodo.Year, 3, fechaPeriodo.Day);
                            break;
                        case 3:

                            fechaPeriodo = new DateTime(fechaPeriodo.Year, 5, fechaPeriodo.Day);
                            break;
                        case 4:
                            fechaPeriodo = new DateTime(fechaPeriodo.Year, 7, fechaPeriodo.Day);

                            break;
                        case 5:

                            fechaPeriodo = new DateTime(fechaPeriodo.Year, 9, fechaPeriodo.Day);
                            break;
                        case 6:

                            fechaPeriodo = new DateTime(fechaPeriodo.Year, 11, fechaPeriodo.Day);
                            break;

                    }

                }
            }

            //Valor Mostrar
            String mostrar = string.Concat(typeof(MovNomConcep).Name, ".resultado");
            if (parametrosFuncion.Length > 4)
            {
                if (string.Equals(parametrosFuncion[posAcum + 3], "EXENTO", StringComparison.OrdinalIgnoreCase))
                {
                    mostrar = string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultadoExento");
                }
                else if (string.Equals(parametrosFuncion[posAcum + 3], "GRAVABLE", StringComparison.OrdinalIgnoreCase))
                {
                    mostrar = string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado");
                }
            }

            resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fechaPeriodo, mostrar, camposFuncion.ToArray<string>(), valoresFuncion.ToArray(), TipoMostrarCampo.SUMA, null, null);
            funcion = funcion.Replace("(", "").Replace("'", "").Replace(",", "").Replace(")", "");
            valoresConceptosEmpleados.Add(funcion, resultado);

        }

        private Object movimientosAcumuladoPorRangoMeses(TipoClasificacionFormula tipoAcumulado, DateTime fechaPeriodoNomina, String campoMostrar, String[] camposWhere, Object[] valoresWhere, TipoMostrarCampo tmc, String campoOmitir, Object valorOmitir)
        {
            //pendiente para cuando termine lo demas del calculo
            Object valor = 0.0;
            int i, posicionParametro = 0;
            try
            {
                campoOmitir = campoOmitir == null ? "" : campoOmitir;
                String tablPadre = typeof(MovNomConcep).Name, path;
                String[] tablasPublicas = new String[]{string.Concat(tablPadre,".empleados.clave"), string.Concat(tablPadre,".periodosNomina.clave"), string.Concat(tablPadre,".razonesSociales.clave"), string.Concat(tablPadre,".tipoNomina.clave"),
                string.Concat(tablPadre,".centroDeCosto.clave"), string.Concat(tablPadre,".tipoCorrida.clave"), string.Concat(tablPadre,".concepNomDefi.clave")};
                ConstruyeQueries cq = new ConstruyeQueries();
                String[] variables = obtieneVariablesFormula(campoMostrar);
                List<String> mapeoVariables = new List<String>();
                mapeoVariables.Add(string.Concat(tablPadre, ".resultado"));
                foreach (String var in variables)
                {
                    if (var.Split('.').Length > 2)
                    {
                        mapeoVariables.Add(var);
                    }
                }
                if (campoOmitir.Any())
                {
                    mapeoVariables.Add(campoOmitir);

                }

                cq.generaListaTablasMapeadas(mapeoVariables.ToArray<string>(), camposWhere, tablasPublicas, null);

                if (usaFiniquitos)
                {
                    if (camposFiniquitos != null)
                    {
                        cq.mapeaTablasCampo(camposFiniquitos.ToArray<string>());
                    }
                }
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                camposWhere = camposWhere == null ? new String[] { } : camposWhere;
                valoresWhere = valoresWhere == null ? new String[] { } : valoresWhere;
                strQuery.Remove(0, strQuery.Length).Append("SELECT ");
                path = campoMostrar.Substring(0, campoMostrar.LastIndexOf("."));
                if (tmc == TipoMostrarCampo.COUNT)
                {
                    foreach (string var in variables)
                    {
                        if (var.Split('.').Length > 2)
                        {
                            path = var;
                            break;
                        }
                    }
                }
                else if (tmc == TipoMostrarCampo.SUMA)
                {
                    strQuery.Append("CASE WHEN (COUNT(").Append(cq.getAliasTablaOuter()[tablPadre]).Append(") = 0 ) THEN 0.0 ELSE (SUM(");
                    foreach (String var in variables)
                    {
                        if (var.Split('.').Length < 2)
                        {
                            strQuery.Append(var);
                        }
                        else
                        {
                            path = var.Substring(0, var.LastIndexOf('.'));
                            path = cq.getAliasTablaOuter()[path.Replace(".", "_")];
                            strQuery.Append("CASE WHEN (").Append(path).Append(" IS NULL) THEN 0.0 ELSE CASE WHEN(").Append(path).Append(var.Substring(var.LastIndexOf("."))).Append(" IS NULL) THEN 0.0 ELSE ");
                            strQuery.Append(path).Append(var.Substring(var.LastIndexOf("."))).Append(" END END");
                        }
                    }
                    strQuery.Append(")) END ");
                }
                else
                {
                    strQuery.Append(cq.getAliasTablaOuter()[path.Replace(".", "_")]).Append(campoMostrar.Substring(campoMostrar.LastIndexOf(".")));
                }
                strQuery.Append(" ").Append(cq.construyeFromConsulta(ConstruyeQueries.LEFTJOIN));
                strQuery.Append(" WHERE ").Append(cq.getAliasTablaOuter()[string.Concat(tablPadre, "_empleados")]).Append(".clave = :claveEmp AND ").Append(cq.getAliasTablaOuter()[string.Concat(tablPadre, "_razonesSociales")]).Append(".clave = :razonSocial ");
                camposParametro.Add("claveEmp");
                camposParametro.Add("razonSocial");
                valoresParametro.Add(valoresConceptosEmpleados["NumEmpleado".ToUpper()]);
                valoresParametro.Add(valoresConceptosEmpleados["RazonSocial".ToUpper()]);
                if (valoresConceptosEmpleados.ContainsKey("TipoNomina".ToUpper()))
                {
                    String nomina = valoresConceptosEmpleados["TipoNomina".ToUpper()] == null ? "" : valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString();
                    if (nomina.Length > 0)
                    {
                        path = cq.getAliasTablaOuter()[string.Concat(tablPadre, "_tipoNomina")];
                        strQuery.Append(" AND ").Append(path).Append(".clave = :TipoNomina");
                        camposParametro.Add("TipoNomina");
                        valoresParametro.Add(nomina);
                    }
                }

                if (valoresConceptosEmpleados.ContainsKey("ClaveTipoCorrida".ToUpper()))
                {
                    String corrida = valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()] == null ? "" : valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()].ToString();
                    if (corrida.Length > 0)
                    {
                        path = cq.getAliasTablaOuter()[string.Concat(tablPadre, "_tipoCorrida")];
                        strQuery.Append(" AND ").Append(path).Append(".clave = :TipoCorrida");
                        camposParametro.Add("TipoCorrida");
                        valoresParametro.Add(corrida);
                    }
                }

                if (valoresConceptosEmpleados.ContainsKey(typeof(CentroDeCosto).Name.ToUpper()))
                {
                    String centro = valoresConceptosEmpleados["CentroDeCosto".ToUpper()] == null ? "" : valoresConceptosEmpleados["CentroDeCosto".ToUpper()].ToString();
                    if (centro.Length > 0)
                    {
                        path = cq.getAliasTablaOuter()[string.Concat(tablPadre, "_centroDeCosto")];
                        strQuery.Append(" AND ").Append(path).Append(".clave = :CentroDeCosto");
                        camposParametro.Add("CentroDeCosto");
                        valoresParametro.Add(centro);
                    }
                }
                if (valoresConceptosEmpleados.ContainsKey("uso".ToUpper()))
                {
                    path = cq.getAliasTablaOuter()[tablPadre];
                    strQuery.Append(" AND ").Append(path).Append(".uso = :uso");
                    camposParametro.Add("uso");
                    valoresParametro.Add(valoresConceptosEmpleados["uso".ToUpper()]);
                }

                if (valoresConceptosEmpleados.ContainsKey("ImprimeListado".ToUpper()))
                {
                    path = cq.getAliasTablaOuter()[string.Concat(tablPadre, "_concepNomDefi")];
                    strQuery.Append(" AND ").Append(path).Append(".imprimirEnListadoNomina = :ImprimeListado");
                    camposParametro.Add("ImprimeListado");
                    valoresParametro.Add(valoresConceptosEmpleados["ImprimeListado".ToUpper()]);
                }

                if (valoresConceptosEmpleados.ContainsKey("ImprimeRecibo".ToUpper()))
                {
                    path = cq.getAliasTablaOuter()[string.Concat(tablPadre, "_concepNomDefi")];
                    strQuery.Append(" AND ").Append(path).Append(".imprimirEnReciboNomina = :ImprimeRecibo");
                    camposParametro.Add("ImprimeRecibo");
                    valoresParametro.Add(valoresConceptosEmpleados["ImprimeRecibo".ToUpper()]);
                }

                if (!campoOmitir.Any() & valorOmitir != null)
                {
                    //variable
                    String[] value = campoOmitir.ToString().Split('#');
                    String alias = value[0].Substring(0, value[0].LastIndexOf("."));
                    path = cq.getAliasTablaOuter()[alias.Replace(".", "_")];
                    //path = cq.getAliasTablaOuter().get(tablPadre.concat("_concepNomDefi"));
                    strQuery.Append(" AND NOT ").Append(path);
                    strQuery.Append(value[0].Substring(value[0].LastIndexOf("."))).Append(" ");
                    if (value.Length > 1)
                    {
                        if (string.Equals(value[1], "LIKE", StringComparison.OrdinalIgnoreCase))
                        {
                            strQuery.Append("LIKE ");
                            strQuery.Append("'%").Append(valorOmitir).Append("%' ");
                        }
                        else
                        {
                            strQuery.Append(value[1]).Append(":valorOmitir");
                            camposParametro.Add("valorOmitir");
                            valoresParametro.Add(valorOmitir);
                        }

                    }
                    else
                    {
                        strQuery.Append("= :valorOmitir");
                        camposParametro.Add("valorOmitir");
                        valoresParametro.Add(valorOmitir);
                    }

                }

                if (usaFiniquitos)
                {
                    if (camposFiniquitos != null & valoresCamposFiniquitos != null)
                    {
                        for (i = 0; i < camposFiniquitos.Count; i++)
                        {
                            path = camposFiniquitos[i].Substring(0, camposFiniquitos[i].LastIndexOf("."));
                            strQuery.Append(" AND ").Append(cq.getAliasTablaOuter()[path.Replace(".", "_")]).Append(camposFiniquitos[i].Substring(camposFiniquitos[i].LastIndexOf("."))).Append(" = :parametro").Append(posicionParametro);
                            camposParametro.Add(string.Concat("parametro", posicionParametro.ToString()));
                            valoresParametro.Add(valoresCamposFiniquitos[i]);
                            posicionParametro++;
                        }
                    }
                }

                DateTime fechaRango = DateTime.Now;
                fechaRango = fechaPeriodoNomina;
                int mesIni = -1, mesFin = -1;
                if (tipoAcumulado == TipoClasificacionFormula.DATOPERIODO)
                {
                    path = cq.getAliasTablaOuter()[string.Concat(tablPadre, "_periodosNomina")];
                    strQuery.Append(" AND ").Append(path).Append(".id in (SELECT periodo.id FROM ").Append(typeof(PeriodosNomina).Name).Append(" periodo INNER JOIN periodo.tipoNomina nomina ");
                    if (periodosNomina != null)
                    {
                        if (string.Equals(clavePeriodoFuncion, "Anterior", StringComparison.OrdinalIgnoreCase))
                        {
                            strQuery.Append("WHERE periodo.fechaInicial = (Select MAX(pp.fechaInicial)  from  PeriodosNomina pp where pp.tipoNomina.clave = :claveTipoNomina  ");
                            if (valoresConceptosEmpleados.ContainsKey("ClaveTipoCorrida".ToUpper()))
                            {
                                String corrida = valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()] == null ? "" : valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()].ToString();
                                if (corrida.Length > 0)
                                {
                                    strQuery.Append(" AND pp.tipoCorrida.clave = :TipoCorrida ");
                                }
                            }
                            strQuery.Append("AND (pp.fechaInicial < :fechaActual AND :fechaActual not between pp.fechaInicial AND pp.fechaFinal))");
                        }
                        else if (isNumericaString(clavePeriodoFuncion))
                        {
                            strQuery.Append("WHERE periodo.clave = :clavePeriodo AND nomina.clave = :claveTipoNomina ");
                            camposParametro.Add("clavePeriodo");
                            valoresParametro.Add(clavePeriodoFuncion);
                        }
                        else if (periodosNomina.tipoNomina.periodicidad.dias == 1)
                        {
                            strQuery.Append("WHERE (:fechaActual BETWEEN periodo.fechaInicial AND periodo.fechaFinal) AND nomina.clave = :claveTipoNomina ");
                        }
                        else
                        {
                            strQuery.Append("WHERE (:fechaActual BETWEEN periodo.fechaInicial AND periodo.fechaFinal) AND nomina.clave = :claveTipoNomina ");
                        }
                    }
                    else if (string.Equals(clavePeriodoFuncion, "Anterior", StringComparison.OrdinalIgnoreCase))
                    {
                        strQuery.Append("WHERE periodo.fechaInicial = (Select MAX(pp.fechaInicial)  from  PeriodosNomina pp where pp.tipoNomina.clave = :claveTipoNomina  ");
                        if (valoresConceptosEmpleados.ContainsKey("ClaveTipoCorrida".ToUpper()))
                        {
                            String corrida = valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()] == null ? "" : valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()].ToString();
                            if (corrida.Length > 0)
                            {
                                strQuery.Append(" AND pp.tipoCorrida.clave = :TipoCorrida ");
                            }
                        }
                        strQuery.Append("AND (pp.fechaInicial < :fechaActual AND :fechaActual not between pp.fechaInicial AND pp.fechaFinal))");
                    }
                    else if (isNumericaString(clavePeriodoFuncion))
                    {
                        strQuery.Append("WHERE periodo.clave = :clavePeriodo AND nomina.clave = :claveTipoNomina ");
                        camposParametro.Add("clavePeriodo");
                        valoresParametro.Add(clavePeriodoFuncion);
                    }
                    else
                    {
                        strQuery.Append("WHERE (:fechaActual BETWEEN periodo.fechaInicial AND periodo.fechaFinal + 1) AND nomina.clave = :claveTipoNomina ");
                    }
                    if (valoresConceptosEmpleados.ContainsKey("ClaveTipoCorrida".ToUpper()))
                    {
                        String corrida = valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()] == null ? "" : valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()].ToString();
                        if (corrida.Length > 0)
                        {
                            strQuery.Append(" AND periodo.tipoCorrida.clave = :TipoCorrida ");
                        }
                    }

                    if (isNumericaString(clavePeriodoFuncion))
                    {
                        strQuery.Append(") AND ").Append(path).Append(".año = :yearPeriodo ");
                        camposParametro.Add("yearPeriodo");
                        DateTime fecha = ((DateTime)valoresConceptosEmpleados[parametroFechaFinal]);
                        valoresParametro.Add(fecha.Year);
                    }
                    else
                    {
                        strQuery.Append(") ");
                        camposParametro.Add("fechaActual");
                        valoresParametro.Add(fechaPeriodoNomina.Date);
                    }

                    camposParametro.Add("claveTipoNomina");
                    valoresParametro.Add(valoresConceptosEmpleados["TipoNomina".ToUpper()]);
                }
                else if (tipoAcumulado == TipoClasificacionFormula.DATOBIMESTRAL)
                {
                    if ((fechaPeriodoNomina.Month + 1) % 2 == 0)
                    {
                        mesFin = fechaPeriodoNomina.Month + 1;
                        mesIni = fechaPeriodoNomina.Month;
                        fechaRango.AddMonths(fechaPeriodoNomina.Month - 1);
                    }
                    else
                    {
                        mesFin = fechaPeriodoNomina.Month + 2;
                        mesIni = fechaPeriodoNomina.Month + 1;


                    }
                }
                else if (tipoAcumulado == TipoClasificacionFormula.DATOANUAL)
                {
                    path = cq.getAliasTablaOuter()[string.Concat(tablPadre, "_periodosNomina")];
                    strQuery.Append(" AND ((").Append(path).Append(".fechaInicial BETWEEN :fechaInicial AND :fechaFinal) ");
                    strQuery.Append(" OR (").Append(path).Append(".fechaFinal BETWEEN :fechaInicial AND :fechaFinal)) ");
                    camposParametro.Add("fechaInicial");
                    DateTime fechaFinal = (DateTime)valoresConceptosEmpleados[parametroFechaInicial];
                    DateTime fecha = DateTime.Now;
                    fecha = new DateTime(fechaFinal.Year, 1, 1);
                    if (string.Equals(clavePeriodoFuncion, "Anterior", StringComparison.OrdinalIgnoreCase) || isNumericaString(clavePeriodoFuncion))
                    {
                        fechaFinal = fechaPeriodoNomina;
                        fecha = new DateTime(fechaFinal.Year, 1, 1);
                    }
                    valoresParametro.Add(fecha.Date);
                    camposParametro.Add("fechaFinal");
                    valoresParametro.Add(fechaFinal.Date);
                }

                if (tipoAcumulado == TipoClasificacionFormula.DATOBIMESTRAL)
                {
                    path = cq.getAliasTablaOuter()[string.Concat(tablPadre, "_periodosNomina")];
                    strQuery.Append(" AND ").Append(path).Append(".año = :yearPeriodo ").Append(" AND ");
                    camposParametro.Add("yearPeriodo");
                    DateTime fecha = (DateTime)valoresConceptosEmpleados[parametroFechaFinal];
                    if (string.Equals(clavePeriodoFuncion, "Anterior", StringComparison.OrdinalIgnoreCase) || isNumericaString(clavePeriodoFuncion))
                    {
                        fecha = fechaPeriodoNomina;
                    }
                    valoresParametro.Add(fecha.Year);
                    camposParametro.Add("mesIni");
                    // camposParametro.add("mesFin");
                    valoresParametro.Add(mesIni);
                    //valoresParametro.add(mesFin);
                    if (mesIni == fecha.Month + 1)
                    {
                        strQuery.Append(cq.getAliasTablaOuter()[tablPadre]).Append(".mes = :mesIni");
                        if (!periodosNomina.cierreMes)
                        {
                            DateTime fechainicio = DateTime.Now;
                            fechainicio = new DateTime(fecha.Year, fecha.Month, 1);
                            strQuery.Append(" AND ((").Append(path).Append(".fechaInicial BETWEEN :fechainical and :fechafinal) OR (");
                            strQuery.Append(path).Append(".fechaFinal BETWEEN :fechainical and :fechafinal)) ");
                            valoresParametro.Add(fechainicio.Date);
                            valoresParametro.Add(fecha.Date);
                            camposParametro.Add("fechainical");
                            camposParametro.Add("fechafinal");
                        }

                    }
                    else if (mesFin == fecha.Month + 1)
                    {

                        camposParametro.Add("mesFin");
                        valoresParametro.Add(mesFin);
                        if (!periodosNomina.cierreMes)
                        {
                            strQuery.Append(" (").Append(cq.getAliasTablaOuter()[tablPadre]).Append(".mes = :mesIni OR  ");
                            DateTime fechainicio = DateTime.Now;
                            fechainicio = new DateTime(fecha.Year, fecha.Month, 1);
                            strQuery.Append("  (").Append(cq.getAliasTablaOuter()[tablPadre]).Append(".mes = :mesFin AND ").Append("(").Append(path).Append(".fechaInicial BETWEEN :fechainical and :fechafinal) OR (");
                            strQuery.Append(path).Append(".fechaFinal BETWEEN :fechainical and :fechafinal))) ");
                            valoresParametro.Add(fechainicio.Date);
                            valoresParametro.Add(fecha.Date);
                            camposParametro.Add("fechainical");
                            camposParametro.Add("fechafinal");
                        }
                        else
                        {
                            strQuery.Append(" (").Append(cq.getAliasTablaOuter()[tablPadre]).Append(".mes = :mesIni OR  ").Append(cq.getAliasTablaOuter()[tablPadre]).Append(".mes = :mesFin) ");
                        }

                    }
                }
                else if (tipoAcumulado == TipoClasificacionFormula.DATOMENSUAL)
                {

                    path = cq.getAliasTablaOuter()[string.Concat(tablPadre, "_periodosNomina")];
                    strQuery.Append(" AND ").Append(path).Append(".año = :yearPeriodo ").Append(" AND ").Append(cq.getAliasTablaOuter()[tablPadre]).Append(".mes = :mesActual  ");

                    camposParametro.Add("yearPeriodo");

                    DateTime fecha = (DateTime)valoresConceptosEmpleados[parametroFechaFinal];
                    if (string.Equals(clavePeriodoFuncion, "Anterior", StringComparison.OrdinalIgnoreCase) || isNumericaString(clavePeriodoFuncion))
                    {
                        fecha = fechaPeriodoNomina;
                    }
                    valoresParametro.Add(fecha.Year);
                    camposParametro.Add("mesActual");
                    valoresParametro.Add(fecha.Month + 1);
                    if (!periodosNomina.cierreMes)
                    {
                        DateTime fechainicio = DateTime.Now;
                        fechainicio = new DateTime(fecha.Year, fecha.Month, 1);
                        strQuery.Append(" AND ((").Append(path).Append(".fechaInicial BETWEEN :fechainical and :fechafinal) OR (");
                        strQuery.Append(path).Append(".fechaFinal BETWEEN :fechainical and :fechafinal)) ");
                        valoresParametro.Add(fechainicio.Date);
                        valoresParametro.Add(fecha.Date);
                        camposParametro.Add("fechainical");
                        camposParametro.Add("fechafinal");
                    }

                }

                for (i = 0; i < camposWhere.Length; i++)
                {
                    path = camposWhere[i].Substring(0, camposWhere[i].LastIndexOf("."));
                    strQuery.Append(" AND ").Append(cq.getAliasTablaOuter()[path.Replace(".", "_")]).Append(camposWhere[i].Substring(camposWhere[i].LastIndexOf("."))).Append(" = :parametro").Append(posicionParametro);
                    camposParametro.Add(string.Concat("parametro", posicionParametro.ToString()));
                    valoresParametro.Add(valoresWhere[i]);
                    posicionParametro++;
                }

                valor = (Object)ejecutaQueryUnico(strQuery.ToString(), camposParametro.ToArray<string>(), valoresParametro.ToArray());
                if (mensajeResultado.noError == -100)
                {
                    mensajeResultado.noError = 62;
                }
            }
            catch (Exception ex)
            {
                mensajeResultado.error = ex.GetBaseException().Message;
                mensajeResultado.noError = 62;
            }
            camposParametro = null;
            valoresParametro = null;
            //strQuery = null;
            return valor;
        }

        private void buscaFormulaConceptos(String concepto)
        {
            try
            {
                strQuery.Remove(0, strQuery.Length).Append("SELECT c.formulaConcepto FROM ConcepNomDefi c WHERE ");
                String variableConcepto = concepto.Substring(concepto.IndexOf("_") + 1);
                String formula;
                // DbContext contex = new DBContextSimple();
                if (isNumericaString(variableConcepto))
                {
                    strQuery.Append("c.clave = :valorBusqueda ");
                }
                else
                {
                    variableConcepto = variableConcepto.Replace('_', ' ');
                    strQuery.Append("c.descripcion = :valorBusqueda ");
                }
                strQuery.Append("AND c.fecha = (SELECT MAX(cc.fecha) FROM ConcepNomDefi cc WHERE cc.clave = c.clave)");
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                camposParametro.Add("valorBusqueda");
                valoresParametro.Add(variableConcepto);
                formula = (String)ejecutaQueryUnico(strQuery.ToString(), camposParametro.ToArray<string>(), valoresParametro.ToArray());
                if (formula != null)
                {
                    double resultado = ejecutaFormula(formula);
                    valoresConceptosEmpleados[concepto] = (double)resultado;
                    //int i;
                    //for (i = 0; i < variablesConceptos.Length; i++)
                    //{
                    //    if (string.Equals(variablesConceptos[i, 0], concepto, StringComparison.OrdinalIgnoreCase))
                    //    {
                    //        valoresConceptosEmpleados[concepto] = (Double)resultado;
                    //        break;
                    //    }
                    //    else if (string.Equals(variablesConceptos[i, 1], concepto, StringComparison.OrdinalIgnoreCase))
                    //    {
                    //        valoresConceptosEmpleados[concepto] = (Double)resultado;
                    //        break;
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscaFormulaConceptos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
        }

        private bool isNumericaString(String cadena)
        {
            try
            {
                Convert.ToInt32(cadena);
                return true;
            }
            catch (Exception nfe)
            {
                return false;
            }
        }

        private String convierteClaveFormulaANombre(String formula)
        {
            if (propertieFuente == null)
            {
                propertieFuente = CompEjec.fuenteDatos;
            }

            String[] variables = obtieneVariablesFormula(formula);
            String variable;
            StringBuilder variablesFormula = new StringBuilder();
            foreach (String str in variables)
            {
                if (propertieFuente.ContainsKey(str))
                {
                    if (propertieFuente.ContainsKey(String.Concat(str, "_TipoDato")))
                    {
                        variable = str;
                    }
                    else if (propertieFuente.ContainsKey(String.Concat(str, "_TipoNodo")))
                    {
                        variable = str;
                    }
                    else
                    {
                        variable = propertieFuente.GetProperty(str);
                    }
                    variablesFormula.Append(variable);
                }
                else
                {
                    variablesFormula.Append(str);
                }
            }
            return variablesFormula.ToString();
        }

        private String[] obtieneVariablesFormula(String strFormula)
        {
            strFormula = strFormula == null ? "" : strFormula;
            List<String> variablesFormulas = new List<String>();
            if (strFormula.Length > 0)
            {
                strFormula = strFormula.Replace(" ", "");//eliminaCaracteresEspacios(strFormula);
                int i;
                StringBuilder valor = new StringBuilder();
                for (i = 0; i < strFormula.Length; i++)
                {
                    if (Char.IsLetterOrDigit(strFormula[i]) | strFormula[i] == '.' | strFormula[i] == '_')
                    {
                        valor.Append(strFormula[i]);
                    }
                    else
                    {
                        if (valor.Length > 0)
                        {
                            if (String.Equals(valor.ToString(), "IF", StringComparison.OrdinalIgnoreCase) | String.Equals(valor.ToString(), "SI", StringComparison.OrdinalIgnoreCase))
                            {
                                valor.Append(" ");
                            }
                            else if (String.Equals(valor.ToString(), "ELSE", StringComparison.OrdinalIgnoreCase) | String.Equals(valor.ToString(), "THEN", StringComparison.OrdinalIgnoreCase) | String.Equals(valor.ToString(), "SINO", StringComparison.OrdinalIgnoreCase) | String.Equals(valor.ToString(), "ENTONCES", StringComparison.OrdinalIgnoreCase)
                                  | String.Equals(valor.ToString(), "AND", StringComparison.OrdinalIgnoreCase) | String.Equals(valor.ToString(), "Y", StringComparison.OrdinalIgnoreCase) | String.Equals(valor.ToString(), "OR", StringComparison.OrdinalIgnoreCase) | String.Equals(valor.ToString(), "o", StringComparison.OrdinalIgnoreCase)
                                  | String.Equals(valor.ToString(), "NO", StringComparison.OrdinalIgnoreCase) | String.Equals(valor.ToString(), "NOT", StringComparison.OrdinalIgnoreCase))
                            {
                                valor.Insert(0, " ");
                                valor.Append(" ");
                            }
                            variablesFormulas.Add(valor.ToString());
                            valor.Remove(0, valor.Length);
                        }
                        if (!Char.IsWhiteSpace(strFormula[i]))
                        {
                            variablesFormulas.Add(strFormula[i].ToString());
                        }
                    }
                }
                if (valor.Length > 0)
                {
                    variablesFormulas.Add(valor.ToString());
                }
            }
            return variablesFormulas.ToArray();
        }

        private void agregaVariableConceptos()
        {
            if (variablesConceptos != null)
            {
                int j;
                for (j = 0; j < variablesConceptos.GetLength(0); j++)
                {
                    DatosConceptosNomina.addVariable(variablesConceptos[j, 0].ToUpper());
                    DatosConceptosNomina.addVariable(variablesConceptos[j, 1].Replace(' ', '_').ToUpper());
                    compEjec.addVariableExtraNro(variablesConceptos[j, 0].ToUpper());
                    compEjec.addVariableExtraNro(variablesConceptos[j, 1].Replace(' ', '_').ToUpper());
                }
            }
        }

        private void agregaParametrosConceptosNomina(IList<MovNomConceParam> movNomConceParametros)
        {
            try
            {
                if (movNomConceParametros != null)
                {
                    int j;
                    for (j = 0; j < movNomConceParametros.Count; j++)
                    {
                        DatosConceptosNomina.addVariable(movNomConceParametros[j].paraConcepDeNom.descripcion.ToUpper());
                        if (String.Equals(movNomConceParametros[j].paraConcepDeNom.tipo, "INTEGER", StringComparison.OrdinalIgnoreCase) | movNomConceParametros[j].paraConcepDeNom.mascara.Contains("#"))
                        {
                            if (movNomConceParametros[j].paraConcepDeNom.clasificadorParametro == (int)ClasificadorParametro.ENTRADA)
                            {
                                compEjec.addVariableExtraNro(string.Concat("Param", movNomConceParametros[j].paraConcepDeNom.numero).ToUpper());
                            }
                            else
                            {
                                compEjec.addVariableExtraNro(movNomConceParametros[j].paraConcepDeNom.descripcion.ToUpper());
                            }
                        }
                        else if (movNomConceParametros[j].paraConcepDeNom.clasificadorParametro == (int)ClasificadorParametro.ENTRADA)
                        {
                            compEjec.addVariableExtraStr(string.Concat("Param", movNomConceParametros[j].paraConcepDeNom.numero).ToUpper());
                        }
                        else
                        {
                            compEjec.addVariableExtraStr(movNomConceParametros[j].paraConcepDeNom.descripcion.ToUpper());
                        }

                        if (movNomConceParametros[j].paraConcepDeNom.clasificadorParametro == (int)ClasificadorParametro.ENTRADA)
                        {
                            valoresConceptosEmpleados[string.Concat("Param", movNomConceParametros[j].paraConcepDeNom.numero).ToUpper()] = movNomConceParametros[j].valor;
                        }
                        else
                        {

                            bool x = false;//Solo el primer parametro y solo aplica para la cantidad en los finiquitos o liquidaciones.
                            if (String.Equals(movNomConceParametros[j].movNomConcep.tipoCorrida.clave, "FIN", StringComparison.OrdinalIgnoreCase)
                                || String.Equals(movNomConceParametros[j].movNomConcep.tipoCorrida.clave, "LIQ", StringComparison.OrdinalIgnoreCase) && j == 0)
                            {
                                if (String.Equals(movNomConceParametros[j].paraConcepDeNom.tipo, "INTEGER", StringComparison.OrdinalIgnoreCase) | movNomConceParametros[j].paraConcepDeNom.mascara.Contains("#"))
                                {
                                    if (Convert.ToDouble(movNomConceParametros[j].valor) > 0)
                                    {
                                        x = true;
                                    }
                                }
                                else if (movNomConceParametros[j].valor != null)
                                {
                                    x = true;
                                }
                            }

                            if (x)
                            {
                                valoresConceptosEmpleados[string.Concat("Param", movNomConceParametros[j].paraConcepDeNom.numero).ToUpper()] = movNomConceParametros[j].valor;
                            }
                            else
                            {
                                valoresConceptosEmpleados[string.Concat("Param", movNomConceParametros[j].paraConcepDeNom.numero).ToUpper()] = ejecutaFormula(movNomConceParametros[j].paraConcepDeNom.descripcion);
                            }

                            if (valoresConceptosEmpleados.ContainsKey(movNomConceParametros[j].paraConcepDeNom.descripcion.ToUpper()))
                            {
                                String valor = valoresConceptosEmpleados[movNomConceParametros[j].paraConcepDeNom.descripcion.ToUpper()].ToString();
                                movNomConceParametros[j].valor = valor;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("agregaParametrosConceptosNomina()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
        }

        private void agregaParametrosConceptosNominaFormula(List<ParaConcepDeNom> paraConcepDeNoms)
        {
            if (paraConcepDeNoms != null)
            {
                int j;
                for (j = 0; j < paraConcepDeNoms.Count(); j++)
                {
                    DatosConceptosNomina.addVariable(paraConcepDeNoms[j].descripcion.ToUpper());
                    compEjec.addVariableExtraNro(paraConcepDeNoms[j].descripcion.ToUpper());
                }
            }
        }

        private int ejecutaQueryExecuteUpdate(String strQuery, String[] camposParametro, Object[] valoresParametro)
        {
            int result = 0;
            try
            {
                IQuery query = getSession().CreateQuery(strQuery);
                int i;
                if (camposParametro != null & valoresParametro != null)
                {
                    for (i = 0; i < camposParametro.Length; i++)
                    {
                        query.SetParameter(camposParametro[i], valoresParametro[i]);
                    }
                }

                result = query.ExecuteUpdate();
            }
            catch (HibernateException ex)
            {
                mensajeResultado.noError = -101;

            }
            return result;
        }

        private MovNomConcep creaMovNomConceptoSubsidio(MovNomConcep movNomi, ConcepNomDefi concepNomDefi)
        {
            MovNomConcep movNomConcepSubsidio = new MovNomConcep();
            try
            {
                movNomConcepSubsidio.empleados = movNomi.plazasPorEmpleado.empleados;
                movNomConcepSubsidio.plazasPorEmpleado = movNomi.plazasPorEmpleado;
                movNomConcepSubsidio.periodosNomina = periodosNomina;
                movNomConcepSubsidio.concepNomDefi = concepNomDefi;
                movNomConcepSubsidio.tipoCorrida = tipoCorrida;
                movNomConcepSubsidio.tipoNomina = periodosNomina.tipoNomina;
                movNomConcepSubsidio.centroDeCosto = centroDeCostoMovimiento;
                movNomConcepSubsidio.razonesSociales = razonesSociales;
                if (concepNomDefi.baseAfecConcepNom != null)
                {
                    movNomConcepSubsidio.movNomBaseAfecta = creaMovimBaseAfectar(concepNomDefi.baseAfecConcepNom, movNomConcepSubsidio);
                }

                movNomConcepSubsidio.fechaCierr = periodosNomina.fechaCierre;
                movNomConcepSubsidio.fechaIni = periodosNomina.fechaInicial;
                movNomConcepSubsidio.tipoPantalla = tipoPantallaSistema;
                movNomConcepSubsidio.ordenId = movNomi.ordenId == 0 ? 0 : movNomi.ordenId + 1;
                movNomConcepSubsidio.resultado = 0.0;
                movNomConcepSubsidio.numero = movNomi.numero == null ? 1 : movNomi.numero + 1;
                movNomConcepSubsidio.calculado = 0.0;
                movNomConcepSubsidio.mes = movNomi.mes;
                movNomConcepSubsidio.ejercicio = movNomi.ejercicio;
            }
            catch (HibernateException ex)
            {
                mensajeResultado.noError = 53;

            }
            return movNomConcepSubsidio;
        }

        private int obtenerPosicionCalculoUnidades(List<CalculoUnidades> listCalculoUnidades, MovNomConcep movNomConcep)
        {
            int posicion = 0;
            for (int i = 0; i < listCalculoUnidades.Count; i++)
            {
                if (movNomConcep.numero == listCalculoUnidades[i].numero & movNomConcep.numMovParticion == listCalculoUnidades[i].numMovParticion)
                {
                    posicion = i;
                    break;
                }
            }
            return posicion;
        }

        private void buscaPeriodicidadesOrPeriodosNomina(string claveTipoNomina, string claveTipoCorrida, decimal? idPeriodoNomina, ISession sessionSimple)
        {
            try
            {
                setSession(sessionSimple);
                getSession().BeginTransaction();
                if (mensajeResultado.noError != 0)
                {
                    return;
                }
                claveTipoNomina = claveTipoNomina == null ? "" : claveTipoNomina;
                if (claveTipoNomina.Trim().Length > 0 | periodicidadTipoNomina == null)
                {
                    periodicidadTipoNomina = (Periodicidad)ejecutaQueryUnico("Select t.periodicidad from TipoNomina t Where t.clave = :clave", new String[] { "clave" }, new Object[] { claveTipoNomina });
                }
                else
                {
                }

                if (periodosNomina == null)
                {
                    if (idPeriodoNomina == null && !isCalculoSDI)
                    {
                        strQuery.Remove(0, strQuery.Length).Append("Select p ");
                        strQuery.Append(" from PeriodosNomina p inner join p.tipoNomina  t ");
                        strQuery.Append(" Where (:fecha BETWEEN p.fechaInicial AND p.fechaFinal) "); //+1
                        strQuery.Append(" and t.clave = :claveTipoNomina AND p.tipoCorrida.clave = :claveTipoCorrida");
                        periodosNomina = (PeriodosNomina)ejecutaQueryUnico(strQuery.ToString(), new String[] { "claveTipoNomina", "fecha", "claveTipoCorrida" },
                                new Object[] { claveTipoNomina, fechaActual, claveTipoCorrida });
                    }
                    else if (isCalculoSDI)
                    {
                        strQuery.Remove(0, strQuery.Length).Append("Select p ");
                        strQuery.Append(" from PeriodosNomina p inner join p.tipoNomina  t ");
                        strQuery.Append(" Where (:fecha BETWEEN p.fechaInicial AND p.fechaFinal) "); //+1
                        strQuery.Append(" and t.clave = :claveTipoNomina AND p.tipoCorrida.clave = :claveTipoCorrida");
                        periodosNomina = (PeriodosNomina)ejecutaQueryUnico(strQuery.ToString(), new String[] { "claveTipoNomina", "fecha", "claveTipoCorrida" },
                                new Object[] { claveTipoNomina, fechaActual, claveTipoCorrida });
                    }
                    else
                    {
                        strQuery.Remove(0, strQuery.Length).Append("Select p ");
                        strQuery.Append(" from PeriodosNomina p inner join p.tipoNomina  t ");
                        strQuery.Append(" Where p.id = :idPeriodoNomina ");
                        periodosNomina = (PeriodosNomina)ejecutaQueryUnico(strQuery.ToString(), new String[] { "idPeriodoNomina" }, new Object[] { idPeriodoNomina });
                    }

                    if (periodosNomina == null && !isCalculoSDI)
                    {
                        mensajeResultado.noError = 1;
                        mensajeResultado.error = "Favor de verificar que existen periodos de nomina";
                    }
                }

                getSession().Transaction.Commit();
            }
            catch (Exception ex)
            {
                getSession().Transaction.Rollback();
            }
            finally
            {
                setSession(null);
            }

        }

        private Object ejecutaQueryUnico(String strQuery, String[] camposParametro, Object[] valoresParametro)
        {
            Object result = null;
            try
            {
                IQuery query = getSession().CreateQuery(strQuery);
                int i;
                if (camposParametro != null && valoresParametro != null)
                {
                    for (i = 0; i < valoresParametro.Length; i++)
                    {
                        if (valoresParametro[i].GetType() == typeof(object[]))
                        {
                            query.SetParameterList(camposParametro[i], (Object[])valoresParametro[i]);
                        }
                        else if (valoresParametro[i].GetType() == typeof(List<>))
                        {
                            query.SetParameterList(camposParametro[i], ((List<object>)valoresParametro[i]).ToArray());
                        }
                        else if (valoresParametro[i].GetType() == typeof(DateTime))
                        {
                            DateTime dateTime = (DateTime)valoresParametro[i];
                            query.SetParameter(camposParametro[i], dateTime.Date);
                        }
                        else
                        {
                            query.SetParameter(camposParametro[i], valoresParametro[i]);
                        }
                    }

                }
                query.SetMaxResults(1);//JSA02
                result = query.UniqueResult();
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("calculaNomina()1_Error: ").Append(ex));
                mensajeResultado.noError = -100;
                mensajeResultado.error = ex.GetBaseException().ToString();

            }

            return result;
        }


        private IList<object> ejecutaQueryList(String strQuery, String[] camposParametro, Object[] valoresParametro, int? maxResultados)
        {
            IList<object> result = null;
            try
            {
                IQuery query = getSession().CreateQuery(strQuery);

                int i;
                if (camposParametro != null & valoresParametro != null)
                {
                    for (i = 0; i < camposParametro.Length; i++)
                    {
                        if (valoresParametro[i].GetType() == typeof(object[]))
                        {
                            query.SetParameterList(camposParametro[i], (Object[])valoresParametro[i]);
                        }
                        else if (valoresParametro[i].GetType() == typeof(List<>))
                        {
                            query.SetParameterList(camposParametro[i], ((List<object>)valoresParametro[i]).ToArray());
                        }
                        else if (valoresParametro[i].GetType() == typeof(DateTime))
                        {
                            DateTime c = (DateTime)valoresParametro[i];
                            query.SetParameter(camposParametro[i], c.Date);
                        }
                        else
                        {
                            query.SetParameter(camposParametro[i], valoresParametro[i]);
                        }
                    }
                }
                if (maxResultados == null)
                {
                    maxResultados = 0;
                }

                if (maxResultados > 0)
                {
                    query.SetMaxResults(maxResultados.GetValueOrDefault());
                }

                result = query.List<object>();
                //result = result2.Select(r => r).ToList<object>();
                if (result == null)
                {
                    result = new List<object>();
                }

            }
            catch (Exception ex)
            {

                mensajeResultado.noError = -100;
                mensajeResultado.error = ex.GetBaseException().ToString();
            }

            return result;
        }

        private List<object> ejecutaQueryList(String consulta, bool conParametros, List<String> listParametros, Dictionary<String, Object> parametrosCampos, int? maxResultados)
        {
            List<Object> result = null;
            try
            {
                IQuery query = getSession().CreateQuery(consulta);
                int i;
                Object valor;
                String parametro;
                if (conParametros & listParametros != null)
                {
                    for (i = 0; i < listParametros.Count; i++)
                    {
                        parametro = listParametros[i];
                        valor = parametrosCampos[parametro];
                        if (valor.GetType() == typeof(object[]))
                        {
                            query.SetParameterList(parametro, (Object[])valor);
                        }
                        else if (valor.GetType() == typeof(List<>))
                        {
                            query.SetParameterList(parametro, ((List<object>)valor).ToArray());
                        }
                        else if (valor.GetType() == typeof(DateTime))
                        {
                            DateTime c = (DateTime)valor;
                            query.SetParameter(parametro, c.Date);
                        }
                        else
                        {
                            query.SetParameter(parametro, valor);
                        }
                    }

                }
                if (maxResultados == null)
                {
                    maxResultados = 0;
                }

                if (maxResultados > 0)
                {
                    query.SetMaxResults(maxResultados.GetValueOrDefault());
                }
                var result2 = query.List<object>();
                result = result2.Select(r => r).ToList<object>();
                if (result == null)
                {
                    result = new List<object>();
                }

            }
            catch (Exception ex)
            {

                mensajeResultado.noError = -100;
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return result;
        }

        private void buscaCalculoPTU(String claveRazonsocial, DateTime fechaInicio, DateTime fechaFin, ISession sessionSimple)
        {
            try
            {
                setSession(sessionSimple);
                getSession().BeginTransaction();
                strQuery.Remove(0, strQuery.Length).Append("Select ptu ");
                strQuery.Append(" from PtuDatosGenerales ptu WHERE ptu.razonesSociales.clave = :claveRazonsocial ");
                strQuery.Append("AND ptu.fechaCalculo BETWEEN :fechaInicial and :fechaFinal ");
                ptuDatosGenerales = (PtuDatosGenerales)ejecutaQueryUnico(strQuery.ToString(), new String[] { "claveRazonsocial", "fechaInicial", "fechaFinal" },
                        new Object[] { claveRazonsocial, fechaInicio, fechaFin });
                if (ptuDatosGenerales != null)
                {
                    DateTime fechaDeCalculo = ptuDatosGenerales.fechaCalculo.GetValueOrDefault(),
                            fechaPeriodoInicial = periodosNomina.fechaInicial.GetValueOrDefault(),
                            fechaPeriodoFinal = periodosNomina.fechaFinal.GetValueOrDefault();
                    if ((fechaDeCalculo.CompareTo(fechaPeriodoInicial) > 0
                            || fechaDeCalculo.CompareTo(fechaPeriodoInicial) == 0)
                            && (fechaDeCalculo.CompareTo(fechaPeriodoFinal) < 0
                            || fechaDeCalculo.CompareTo(fechaPeriodoFinal) == 0))
                    {
                        isCalculoPTU = true;
                    }
                }
                getSession().Transaction.Commit();
            }
            catch (Exception ex)
            {

                getSession().Transaction.Rollback();
            }
            finally
            {
                setSession(null);
            }
        }

        #region metodos para las tablas xml y factores
        private void generaTablasXml(String controlador, Periodicidad periodicidadTipoNomina, String claveRazonSocial, DateTime fechaFinal, int ejercicio, ISession uuidCxnMaestra)
        {
            try
            {
                setSession(uuidCxnMaestra);
                getSession().BeginTransaction();
                // obtenerFactores(claveRazonSocial);
                if (mensajeResultado.noError != 0)
                {
                    return;
                }
                List<TablaBase> tablasBaseSistema = getCargarTablaBaseSistema();
                List<TipoControlador> tipoControladores;
                if (modoAjustarIngresosMes == ProporcionaPeriodoConTablaAnual)
                {

                    tipoControladores = obtieneTipoControladorTablaBase(ClavesParametrosModulos.claveTipoTablaISRAnual.ToString(), tablasBaseSistema);
                    if (mensajeResultado.noError != 0)
                    {
                        mensajeResultado.error = ("no encontro controladores en la tabla ISR anual");
                        return;
                    }
                    tablaIsr = construyeTablaXmlPorTipoNomina(ClavesParametrosModulos.claveTipoTablaISRAnual.ToString(), "", tipoControladores, fechaFinal, ejercicio);
                    if (mensajeResultado.noError == -10)
                    {
                        mensajeResultado.noError = (13);
                    }
                    if (tablaIsr == null & mensajeResultado.noError == 0)
                    {
                        mensajeResultado.error = ("no encontro datos de la tabla ISR Anual");
                        mensajeResultado.noError = (1000);
                        mensajeResultado.resultado = (0);
                    }
                    if (mensajeResultado.noError != 0)
                    {
                        return;
                    }

                    tipoControladores = obtieneTipoControladorTablaBase(ClavesParametrosModulos.claveTipoTablaSUBSIDIOAnual.ToString(), tablasBaseSistema);
                    if (mensajeResultado.noError != 0)
                    {
                        mensajeResultado.error = ("no encontro controladores en la tabla Subsidio anual");
                        return;
                    }
                    tablaSubsidio = construyeTablaXmlPorTipoNomina(ClavesParametrosModulos.claveTipoTablaSUBSIDIOAnual.ToString(), "", tipoControladores, fechaFinal, ejercicio);
                    if (mensajeResultado.noError == -10)
                    {
                        mensajeResultado.noError = (14);
                    }
                    if (tablaSubsidio == null & mensajeResultado.noError == 0)
                    {
                        mensajeResultado.error = ("no encontro datos de la tabla Subsidio Anual");
                        mensajeResultado.noError = (1000);
                        mensajeResultado.resultado = (0);
                    }
                    if (mensajeResultado.noError != 0)
                    {
                        return;
                    }
                }
                else if (tipoTablaISR == TipoTablaISR.PERIODICIDAD)
                {
                    if (periodicidadTipoNomina != null)
                    {

                        String controladorPeriodicidad = string.Concat(typeof(Periodicidad).Name, periodicidadTipoNomina.clave);
                        tipoControladores = obtieneTipoControladorTablaBase(ClavesParametrosModulos.claveTipoTablaISRPeriodicidad.ToString(), tablasBaseSistema);

                        if (mensajeResultado.noError != 0)
                        {
                            mensajeResultado.error = ("no encontro controladores en la tabla ISR por periodicidad");
                            return;
                        }
                        tablaIsr = construyeTablaXmlPorTipoNomina(ClavesParametrosModulos.claveTipoTablaISRPeriodicidad.ToString(), controladorPeriodicidad, tipoControladores, fechaFinal, ejercicio);

                        if (mensajeResultado.noError == -10)
                        {
                            mensajeResultado.noError = (11);
                        }
                        if (tablaIsr == null & mensajeResultado.noError == 0)
                        {
                            mensajeResultado.error = ("no encontro datos de la tabla ISR por periodicidad");
                            mensajeResultado.noError = (1000);
                            mensajeResultado.resultado = (0);
                        }

                        if (mensajeResultado.noError != 0)
                        {
                            return;
                        }
                        tipoControladores = obtieneTipoControladorTablaBase(ClavesParametrosModulos.claveTipoTablaSubsidioPeriodicidad.ToString(), tablasBaseSistema);

                        if (mensajeResultado.noError != 0)
                        {
                            mensajeResultado.error = ("no encontro controladores en la tabla Subsidio por periodicidad");
                            return;
                        }
                        tablaSubsidio = construyeTablaXmlPorTipoNomina(ClavesParametrosModulos.claveTipoTablaSubsidioPeriodicidad.ToString(), controladorPeriodicidad, tipoControladores, fechaFinal, ejercicio);

                        if (mensajeResultado.noError == -10)
                        {
                            mensajeResultado.noError = (12);
                        }
                        if (tablaSubsidio == null & mensajeResultado.noError
                                == 0)
                        {
                            mensajeResultado.error = ("no encontro datos de la tabla Subsidio por periodicidad");
                            mensajeResultado.noError = (1000);
                            mensajeResultado.resultado = (0);
                        }

                        if (mensajeResultado.noError != 0)
                        {
                            return;
                        }
                    }
                }
                else
                {
                    String tipoTabla = ClavesParametrosModulos.claveTipoTablaISR.ToString();
                    if (modoAjustarIngresosMes == ProporcionaPeriodoConTablaAnual)
                    {//BDEI01
                        tipoTabla = "06";
                    }
                    tipoControladores = obtieneTipoControladorTablaBase(tipoTabla, tablasBaseSistema);
                    if (mensajeResultado.noError != 0)
                    {
                        mensajeResultado.error = ("no encontro controladores en la tabla ISR");
                        return;
                    }
                    tablaIsr = construyeTablaXmlPorTipoNomina(tipoTabla, "", tipoControladores, fechaFinal, ejercicio);
                    if (mensajeResultado.noError == -10)
                    {
                        mensajeResultado.noError = (21);
                    }
                    if (tablaIsr == null & mensajeResultado.noError == 0)
                    {
                        mensajeResultado.error = ("no encontro datos de la tabla ISR");
                        mensajeResultado.noError = (1000);
                        mensajeResultado.resultado = (0);
                    }
                    if (mensajeResultado.noError != 0)
                    {
                        return;
                    }
                    tipoControladores = obtieneTipoControladorTablaBase(ClavesParametrosModulos.claveTipoTablaSubsidio.ToString(), tablasBaseSistema);
                    if (mensajeResultado.noError != 0)
                    {
                        mensajeResultado.error = ("no encontro controladores en la tabla Subsidio");
                        return;
                    }
                    tablaSubsidio = construyeTablaXmlPorTipoNomina(ClavesParametrosModulos.claveTipoTablaSubsidio.ToString(), "", tipoControladores, fechaFinal, ejercicio);
                    if (mensajeResultado.noError == -10)
                    {
                        mensajeResultado.noError = (22);
                    }
                    if (tablaSubsidio == null & mensajeResultado.noError == 0)
                    {
                        mensajeResultado.error = ("no encontro datos de la tabla Subsidio");
                        mensajeResultado.noError = (1000);
                        mensajeResultado.resultado = (0);
                    }
                    if (mensajeResultado.noError != 0)
                    {
                        return;
                    }
                }

                tipoControladores = obtieneTipoControladorTablaBase(ClavesParametrosModulos.claveTipoTablaZonaZalarial.ToString(), tablasBaseSistema);
                if (mensajeResultado.noError != 0)
                {
                    mensajeResultado.error = ("no encontro controladores en la tabla zona salarial");
                    return;
                }
                tablaZonaSalarial = construyeTablaXml(ClavesParametrosModulos.claveTipoTablaZonaZalarial.ToString(), controlador, tipoControladores, fechaFinal, ejercicio);
                if (mensajeResultado.noError == -10)
                {
                    mensajeResultado.noError = (23);
                }
                if (tablaZonaSalarial == null & mensajeResultado.noError == 0)
                {
                    mensajeResultado.error = ("no encontro datos de la tabla zona salarial");
                    mensajeResultado.noError = (1000);
                    mensajeResultado.resultado = (0);
                }
                if (mensajeResultado.noError != 0)
                {
                    return;
                }
                if (isUMA)
                {
                    Object[,] tablaUMA = construyeTablaXml(ClavesParametrosModulos.claveTipoTablaUMA.ToString(), controlador, tipoControladores, fechaFinal, ejercicio);
                    if (mensajeResultado.noError != 0)
                    {
                        return;
                    }
                    else if (tablaUMA != null & mensajeResultado.noError == 0)
                    {
                        if (tablaUMA.Length == 0)
                        {
                            mensajeResultado.noError = 17;
                            mensajeResultado.error = "No encontro datos en la tabla UMA";
                        }
                        else
                        {
                            for (int i = 0; i < tablaUMA.Length; i++)
                            {
                                if (String.IsNullOrEmpty(tablaUMA[i, 0].ToString().Trim()))
                                {
                                    valorUMA = null;
                                }
                                else
                                {
                                    valorUMA = Double.Parse(tablaUMA[i, 0].ToString());
                                }
                                break;
                            }

                            if (valorUMA == null)
                            {
                                mensajeResultado.noError = 17;
                                mensajeResultado.error = "No encontro datos en la tabla UMA";
                            }
                        }
                    }
                }

                tipoControladores = obtieneTipoControladorTablaBase(ClavesParametrosModulos.claveTipoTablaFactorIntegracion.ToString(), tablasBaseSistema);
                tablaFactorIntegracion = construyeTablaXml(ClavesParametrosModulos.claveTipoTablaFactorIntegracion.ToString(), controlador, tipoControladores, fechaFinal, ejercicio);
                if (mensajeResultado.noError == -10)
                {
                    mensajeResultado.noError = (24);
                }
                if (tablaFactorIntegracion == null & mensajeResultado.noError == 0)
                {
                    mensajeResultado.error = ("no encontro datos de la tabla factor de integracion");
                    mensajeResultado.noError = (1000);
                    mensajeResultado.resultado = (0);
                }
                if (mensajeResultado.noError != 0)
                {
                    return;
                }

                tipoControladores = obtieneTipoControladorTablaBase(ClavesParametrosModulos.claveTipoTablaISR.ToString(), tablasBaseSistema);
                if (mensajeResultado.noError != 0)
                {
                    mensajeResultado.error = ("no encontro controladores en la tabla ISR Mensual");
                    return;
                }
                tablaIsrMes = construyeTablaXmlPorTipoNomina(ClavesParametrosModulos.claveTipoTablaISR.ToString(), "", tipoControladores, fechaFinal, ejercicio);
                tipoControladores = obtieneTipoControladorTablaBase(ClavesParametrosModulos.claveTipoTablaSubsidio.ToString(), tablasBaseSistema);
                if (mensajeResultado.noError != 0)
                {
                    mensajeResultado.error = ("no encontro controladores en la tabla subsidio Mensual");
                    return;
                }
                tablaSubsidioMes = construyeTablaXmlPorTipoNomina(ClavesParametrosModulos.claveTipoTablaSubsidio.ToString(), "", tipoControladores, fechaFinal, ejercicio);

                String queryTabla = "from TablaDatos";
                IQuery q = getSession().CreateQuery(queryTabla.ToString());
                tablaDatosXml = q.List<TablaDatos>();
                // List<object> tablaDatos2 =(List<object>)ejecutaQueryList(queryTabla, null, null, null);
                //tablaDatosXml = (List<TablaDatos>)ejecutaQueryList(queryTabla, null, null, null);

                getSession().Transaction.Commit();
            }
            catch (Exception ex)
            {

                getSession().Transaction.Rollback();
            }
            finally
            {
                setSession(null);
            }
        }

        private void obtenerFactores(String claveRazonSocial)
        {
            try
            {
                tipoTablaISR = TipoTablaISR.NORMAL;
                Object[] valores = new Object[]{
                (decimal) ClavesParametrosModulos.claveParametroFactorAplicaciónTablaAnual,
                (decimal) ClavesParametrosModulos.claveParametroPagosPorHora,
                (decimal) ClavesParametrosModulos.claveParametroManejarHorasPor,
                (decimal) ClavesParametrosModulos.claveParametroManejarSalarioDiarioPor,
                (decimal) ClavesParametrosModulos.claveParametroFactorAplicaciónTablaMensual,
                (decimal) ClavesParametrosModulos.claveParametroModoAjustarIngresosAlMes,
                (decimal) ClavesParametrosModulos.claveParametroDesgloseInternoISR,
                (decimal) ClavesParametrosModulos.clavePagarNominaDiasNaturales,
                (decimal) ClavesParametrosModulos.claveParametroVersionCalculoPrestamoAhorro,
                (decimal) ClavesParametrosModulos.claveParametroPagarIMSSDiasNaturales,
                (decimal) ClavesParametrosModulos.claveParametroDescontarFaltasModoAjusteMes,
                (decimal) ClavesParametrosModulos.claveParametroPagarPrimayVacacionesAuto,
                (decimal) ClavesParametrosModulos.claveParametroSalarioUtilizarVacaciones,
                (decimal) ClavesParametrosModulos.claveParametroUsaUMA};
                List<Object[]> listParametros;
                mensajeResultado = getParametrosYListCrucePorModuloYClaves((String)ClavesParametrosModulos.claveModuloGlobal, valores);
                if (mensajeResultado.noError == 0)
                {
                    listParametros = (List<Object[]>)mensajeResultado.resultado;
                }
                else
                {
                    return;
                }
                Object[] parametroManejarSalarioDiarioPor = null;
                DesgloseInternoISR desgloseInternoISR = DesgloseInternoISR.DESGLOSEISRNORMALANUAL;
                for (int i = 0; i < listParametros.Count(); i++)
                {
                    if (((Parametros)(listParametros[i])[0]).clave.Equals((decimal)ClavesParametrosModulos.claveParametroFactorAplicaciónTablaAnual))
                    {
                        if (factorAnual == null ? true : factorAnual == 0)
                        {
                            factorAnual = parametroFactorAplicacionTablaAnual((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
                        }
                    }
                    else if (((Parametros)(listParametros[i])[0]).clave.Equals((decimal)ClavesParametrosModulos.claveParametroPagosPorHora))
                    {
                        if (manejaPagosPorHora == null)
                        {
                            Object[] objects = parametroPagosPorHora((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
                            manejaPagosPorHora = (bool)objects[0];
                        }
                    }
                    else if (((Parametros)(listParametros[i])[0]).clave == (decimal)ClavesParametrosModulos.claveParametroManejarHorasPor)
                    {
                        if (manejoHorasPor == null)
                        {
                            manejoHorasPor = parametroManejarHorasPor((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
                        }
                    }
                    else if (((Parametros)(listParametros[i])[0]).clave == (decimal)ClavesParametrosModulos.claveParametroManejarSalarioDiarioPor)
                    {
                        parametroManejarSalarioDiarioPor = listParametros[i];
                    }
                    else if (((Parametros)(listParametros[i])[0]).clave == (decimal)ClavesParametrosModulos.claveParametroFactorAplicaciónTablaMensual)
                    {
                        factorMensual = parametroFactorAplicacionTablaMensual((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
                    }
                    else if (((Parametros)(listParametros[i])[0]).clave == (decimal)ClavesParametrosModulos.claveParametroModoAjustarIngresosAlMes)
                    {
                        modoAjustarIngresosMes = parametroModoAjustarIngresosAlMes((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
                        if (modoAjustarIngresosMes == ProporcionaCadaPeriodoUtilizandoTablaPeriodo)
                        {
                            tipoTablaISR = TipoTablaISR.PERIODICIDAD;
                        }
                    }
                    else if (((Parametros)(listParametros[i])[0]).clave == (decimal)ClavesParametrosModulos.claveParametroDesgloseInternoISR)
                    {
                        desgloseInternoISR = parametroDesgloseInternoISR((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
                    }
                    else if (((Parametros)(listParametros[i])[0]).clave == (decimal)ClavesParametrosModulos.clavePagarNominaDiasNaturales)
                    {
                        if (periodicidadTipoNomina != null)
                        {
                            manejaPagoDiasNaturales = pagarNominaDiasNaturales((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial), new ValoresElementosAplicacion(typeof(Periodicidad), periodicidadTipoNomina.clave) });
                        }
                        else
                        {
                            manejaPagoDiasNaturales = pagarNominaDiasNaturales((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
                        }
                    }
                    else if (((Parametros)(listParametros[i])[0]).clave == (decimal)ClavesParametrosModulos.claveParametroVersionCalculoPrestamoAhorro)
                    {
                        versionCalculoPrestamoAhorro = parametroVersionCalculoPrestamoAhorro((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
                    }
                    else if (((Parametros)(listParametros[i])[0]).clave == (decimal)ClavesParametrosModulos.claveParametroPagarIMSSDiasNaturales)
                    {
                        manejaPagoIMSSDiasNaturales = pagarIMSSDiasNaturales((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
                    }
                    else if (((Parametros)(listParametros[i])[0]).clave == (decimal)ClavesParametrosModulos.claveParametroDescontarFaltasModoAjusteMes)
                    {
                        descontarFaltasModoAjustaMes = descuentoFaltasModoAjustaMes((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
                    }
                    else if (((Parametros)(listParametros[i])[0]).clave == (decimal)ClavesParametrosModulos.claveParametroPagarPrimayVacacionesAuto)
                    {
                        pagarVacaAuto = parametroPagarPrimayVacacionesAuto((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
                    }
                    else if (((Parametros)(listParametros[i])[0]).clave == (decimal)ClavesParametrosModulos.claveParametroSalarioUtilizarVacaciones)
                    {
                        salarioVacaciones = parametroSalarioVacaciones((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
                    }
                    else if (((Parametros)(listParametros[i])[0]).clave == (decimal)ClavesParametrosModulos.claveParametroUsaUMA)
                    {
                        isUMA = activarUMA((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
                    }
                }
                manejoSalarioDiario = parametroManejadorSalarioDiarioPor((Parametros)parametroManejarSalarioDiarioPor[0], (List<Cruce>)parametroManejarSalarioDiarioPor[1], manejaPagoDiasNaturales, new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
                if (modoAjustarIngresosMes != ProporcionaTablaAnual)
                {
                    if (desgloseInternoISR == DesgloseInternoISR.DESGLOSEISRNORMALANUAL)
                    {
                        calculoSeparadoISR = true;
                    }
                }

            }
            catch (Exception ex)
            {

                mensajeResultado.noError = 1;
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
        }

        public Mensaje getParametrosYListCrucePorModuloYClaves(String claveModulo, Object[] clavesParametros)
        {
            List<Object[]> listParametrosYListCruce = new List<Object[]>();
            try
            {
                inicializaVariableMensaje();
                //            setSession(HibernateUtil.currentSession(uuidCxnMaestra));
                //getSession().beginTransaction();

                IQuery q = getSession().CreateQuery("from Parametros p where p.modulo.clave = :claveModulo and p.clave in(:clavesParametros) order by clave");
                q.SetString("claveModulo", claveModulo);
                q.SetParameterList("clavesParametros", clavesParametros);

                IList<Parametros> listparametros = q.List<Parametros>();
                if ((listparametros != null) && (listparametros.Any()))
                {
                    for (int i = 0; i < listparametros.Count(); i++)
                    {

                        IList<Cruce> values;//Si el parametro no tiene seleccionado elementos de aplicacion quiere decir que no se va filtrar o profuncidar por algun elemento de aplicacion
                        if ((listparametros[i].elementosAplicacion != null) && (listparametros[i].elementosAplicacion.Any()))
                        {
                            int conta = listparametros[i].elementosAplicacion.Select(p => p.cruce).Select(a => a.Count).FirstOrDefault();
                            if (conta > 0)
                            {
                                q = getSession().CreateQuery("from Cruce c where c.parametros.clave = :parametro  and c.elementosAplicacion in (:values) order by c.elementosAplicacion.ordenId desc");
                                q.SetParameter("parametro", listparametros[i].clave);
                                q.SetParameterList("values", listparametros[i].elementosAplicacion);
                                values = q.List<Cruce>();
                            }
                            else
                            {
                                values = new List<Cruce>();
                            }

                        }
                        else
                        {
                            values = new List<Cruce>();

                        }


                        Object[] objects = new Object[2];
                        objects[0] = listparametros[i];
                        objects[1] = values;
                        listParametrosYListCruce.Add(objects);
                        values = null;
                    }
                }

                mensajeResultado.resultado = (listParametrosYListCruce);
                mensajeResultado.noError = (0);
                mensajeResultado.error = ("");
            }
            catch (Exception ex)
            {

                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = (null);
            }

            return mensajeResultado;
        }

        public double parametroFactorAplicacionTablaAnual(Parametros parametros, List<Cruce> listCruces, List<ValoresElementosAplicacion> listValoresElementosAplicacion)
        {
            double topeHorasDoblesDiario = 3.0;
            string valorParametros = (string)valorParametroCruce(parametros, listCruces, true, listValoresElementosAplicacion);
            topeHorasDoblesDiario = Double.Parse(valorParametros);
            return topeHorasDoblesDiario;
        }
        private object valorParametroCruce(Parametros parametrosGlobal, List<Cruce> listCruce, bool regresarValor/*true=valor,false=byte[]*/, List<ValoresElementosAplicacion> listValoresElementosAplicacion)
        {
            object valorParametros = null;
            try
            {
                if (regresarValor)
                {
                    valorParametros = parametrosGlobal.valor;
                }
                else
                {
                    valorParametros = parametrosGlobal.imagen;
                }
                if (listCruce.Count > 1)
                {
                    listCruce = (from cr in listCruce orderby cr.elementosAplicacion.id select cr).ToList();
                }
                for (int i = listCruce.Count - 1; i > -1; i--)
                {
                    Type tipo = Type.GetType(listCruce[i].elementosAplicacion.entidad);
                    if (findClaveElementoAplicacion(tipo, listCruce[i].claveElemento, listValoresElementosAplicacion))
                    {
                        if (regresarValor)
                        {
                            valorParametros = listCruce[0].valor;
                        }
                        else
                        {
                            valorParametros = listCruce[0].imagen;
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("valorParametroCruce()1_Error: ").Append(ex));
                mensajeResultado.resultado = null;
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return valorParametros;
        }
        private bool findClaveElementoAplicacion(Type classElementoAplicacion, string valorCruce, List<ValoresElementosAplicacion> listValoresElementosAplicacion)
        {
            bool continuar = false;
            for (int i = 0; i < listValoresElementosAplicacion.Count; i++)
            {
                if (classElementoAplicacion.Equals(listValoresElementosAplicacion[i].classElementoAplicacion))
                {
                    if (valorCruce.Equals(listValoresElementosAplicacion[i].valor))
                    {
                        continuar = true;
                    }
                }
            }
            return continuar;
        }
        public class ValoresElementosAplicacion
        {

            public Type classElementoAplicacion { get; set; }
            public string valor { get; set; }

            public ValoresElementosAplicacion()
            {
            }

            public ValoresElementosAplicacion(Type classElementoAplicacion, string valor)
            {
                this.classElementoAplicacion = classElementoAplicacion;
                this.valor = valor;
            }
        }
        private object[] parametroPagosPorHora(Parametros parametros, List<Cruce> listCruces, List<ValoresElementosAplicacion> listValoresElementosAplicacion)
        {
            bool manejarPagosXHora = false;
            List<Cruce> listCrucesPagosHoraaRegistroPatronal = new List<Cruce>();
            string valorParametroPagosPorHora = (string)valorParametroCruce(parametros, listCruces, true, listValoresElementosAplicacion);
            manejarPagosXHora = valorParametroPagosPorHora.Equals("1");
            for (int i = 0; i < listCruces.Count; i++)
            {
                if (listCruces[i].elementosAplicacion.entidad.Equals(typeof(RegistroPatronal).FullName))
                {
                    listCrucesPagosHoraaRegistroPatronal.Add(listCruces[i]);
                }
            }
            if (listCrucesPagosHoraaRegistroPatronal.Count > 0)
            {
                manejarPagosXHora = true;
            }
            return new object[] { manejarPagosXHora, listCrucesPagosHoraaRegistroPatronal };
        }
        private ManejoHorasPor parametroManejarHorasPor(Parametros parametros, List<Cruce> listCruces, List<ValoresElementosAplicacion> listValoresElementosAplicacion)
        {
            ManejoHorasPor tipoManejoDeHoraPor;
            string valorParametroHoraPor = "2";
            valorParametroHoraPor = (string)valorParametroCruce(parametros, listCruces, true, listValoresElementosAplicacion);
            if (valorParametroHoraPor.Equals("1"))
            {
                tipoManejoDeHoraPor = ManejoHorasPor.HORASNATURALES;
            }
            else
            {
                tipoManejoDeHoraPor = ManejoHorasPor.HSM;
            }
            return tipoManejoDeHoraPor;
        }
        private double parametroFactorAplicacionTablaMensual(Parametros parametros, List<Cruce> listCruces, List<ValoresElementosAplicacion> listValoresElementosAplicacion)
        {
            Double factorTablaMensual = 30.4;
            string valorParametros = (string)valorParametroCruce(parametros, listCruces, true, listValoresElementosAplicacion);
            try
            {
                factorTablaMensual = Double.Parse(valorParametros);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("parametroFactorAplicacionTablaMensual()1_Error: ").Append(ex));
            }
            return factorTablaMensual;
        }

        private int parametroModoAjustarIngresosAlMes(Parametros parametros, List<Cruce> listCruces, List<ValoresElementosAplicacion> listValoresElementosAplicacion)
        {
            int factorTablaMensual = 1;
            string valorParametros = (string)valorParametroCruce(parametros, listCruces, true, listValoresElementosAplicacion);
            try
            {
                factorTablaMensual = int.Parse(valorParametros);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("parametroModoAjustarIngresosAlMes()1_Error: ").Append(ex));
            }
            return factorTablaMensual;
        }

        private DesgloseInternoISR parametroDesgloseInternoISR(Parametros parametros, List<Cruce> listCruces, List<ValoresElementosAplicacion> listValoresElementosAplicacion)
        {
            DesgloseInternoISR desgloseInternoISR;
            string valorParametros = (string)valorParametroCruce(parametros, listCruces, true, listValoresElementosAplicacion);
            if (valorParametros.Equals("1"))
            {
                desgloseInternoISR = DesgloseInternoISR.DESGLOSEISRNORMALANUAL;
            }
            else if (valorParametros.Equals("2"))
            {
                desgloseInternoISR = DesgloseInternoISR.DESGLOSEISRNORMALDIRECTOANUAL;
            }
            else
            {
                desgloseInternoISR = DesgloseInternoISR.DESGLOSEISRANUAL;
            }
            return desgloseInternoISR;
        }

        private bool pagarNominaDiasNaturales(Parametros parametros, List<Cruce> listCruces, List<ValoresElementosAplicacion> listValoresElementosAplicacion)
        {
            bool pagaDiasNaturales = false;
            string valorParametros = (string)valorParametroCruce(parametros, listCruces, true, listValoresElementosAplicacion);
            pagaDiasNaturales = valorParametros.Equals("1");
            return pagaDiasNaturales;
        }

        private int parametroVersionCalculoPrestamoAhorro(Parametros parametros, List<Cruce> listCruces, List<ValoresElementosAplicacion> listValoresElementosAplicacion)
        {
            int versionCalculo = 1;
            string valorParametros = (string)valorParametroCruce(parametros, listCruces, true, listValoresElementosAplicacion);
            try
            {
                versionCalculo = int.Parse(valorParametros);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("parametroVersionCalculoPrestamoAhorro()1_Error: ").Append(ex));
            }
            return versionCalculo;
        }

        private bool pagarIMSSDiasNaturales(Parametros parametros, List<Cruce> listCruces, List<ValoresElementosAplicacion> listValoresElementosAplicacion)
        {
            bool pagarIMSS = false;
            try
            {
                string valorParametros = (string)valorParametroCruce(parametros, listCruces, true, listValoresElementosAplicacion);
                pagarIMSS = valorParametros.Equals("1");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("pagarIMSSDiasNaturales()1_Error: ").Append(ex));
            }
            return pagarIMSS;
        }

        private bool descuentoFaltasModoAjustaMes(Parametros parametros, List<Cruce> listCruces, List<ValoresElementosAplicacion> listValoresElementosAplicacion)
        {
            bool descontarFaltas = false;
            try
            {
                string valorParametros = (string)valorParametroCruce(parametros, listCruces, true, listValoresElementosAplicacion);
                descontarFaltas = valorParametros.Equals("1");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("descuentoFaltasModoAjustaMes()1_Error: ").Append(ex));
            }
            return descontarFaltas;
        }

        private PagarPrimaVacionalyVacacionesAuto parametroPagarPrimayVacacionesAuto(Parametros parametros, List<Cruce> listCruces, List<ValoresElementosAplicacion> listValoresElementosAplicacion)
        {
            PagarPrimaVacionalyVacacionesAuto pagarVaca;

            string valorParametro = (string)valorParametroCruce(parametros, listCruces, true, listValoresElementosAplicacion);
            if (valorParametro.Equals("2"))
            {
                pagarVaca = PagarPrimaVacionalyVacacionesAuto.PRIMACACIONALALANIVERSARIO;
            }
            else if (valorParametro.Equals("3"))
            {
                pagarVaca = PagarPrimaVacionalyVacacionesAuto.PRIMACACIONALYVACACIONESALANIVERSARIO;
            }
            else
            {
                pagarVaca = PagarPrimaVacionalyVacacionesAuto.MANUAL;
            }

            return pagarVaca;
        }

        private ManejoSalarioVacaciones parametroSalarioVacaciones(Parametros parametros, List<Cruce> listCruces, List<ValoresElementosAplicacion> listValoresElementosAplicacion)
        {
            ManejoSalarioVacaciones salarioVaca;

            string valorParametro = (string)valorParametroCruce(parametros, listCruces, true, listValoresElementosAplicacion);
            if (valorParametro.Equals("1"))
            {
                salarioVaca = ManejoSalarioVacaciones.SALARIOANIVERSARIO;
            }
            else
            {
                salarioVaca = ManejoSalarioVacaciones.SALARIOACTUAL;
            }

            return salarioVaca;
        }

        private bool activarUMA(Parametros parametros, List<Cruce> listCruces, List<ValoresElementosAplicacion> listValoresElementosAplicacion)
        {
            bool activaUMA = false;
            string valorParametros = (string)valorParametroCruce(parametros, listCruces, true, listValoresElementosAplicacion);
            activaUMA = valorParametros.Equals("1");
            return activaUMA;
        }

        private ManejoSalarioDiario parametroManejadorSalarioDiarioPor(Parametros parametros, List<Cruce> listCruces, bool pagaDiasNaturales, List<ValoresElementosAplicacion> listValoresElementosAplicacion)
        {
            ManejoSalarioDiario tipoManejoSalarioDiario = ManejoSalarioDiario.QUINCENAL;
            if (pagaDiasNaturales)
            {
                tipoManejoSalarioDiario = ManejoSalarioDiario.DIARIO;
            }
            else
            {
                string valorParametro = (string)valorParametroCruce(parametros, listCruces, true, listValoresElementosAplicacion);
                if (valorParametro.Equals("1"))
                {
                    tipoManejoSalarioDiario = ManejoSalarioDiario.DIARIO;
                }
                else if (valorParametro.Equals("2"))
                {
                    tipoManejoSalarioDiario = ManejoSalarioDiario.SEMANAL;
                }
                else if (valorParametro.Equals("4"))
                {
                    tipoManejoSalarioDiario = ManejoSalarioDiario.MENSUAL;
                }
                else
                {//por default es quincenal = 3
                    tipoManejoSalarioDiario = ManejoSalarioDiario.QUINCENAL;
                }
            }
            return tipoManejoSalarioDiario;
        }

        private List<TablaBase> getCargarTablaBaseSistema()
        {
            List<TablaBase> listTablaBase = null;
            try
            {
                IQuery q = getSession().CreateQuery("SELECT new TablaBase(tb.clave, tt.nombre, tb.controladores) FROM TablaBase tb INNER JOIN tb.tipoTabla tt WHERE tt.sistema = true");
                var listTablaBase1 = q.List<TablaBase>();
                listTablaBase = listTablaBase1.Select(p => p).ToList();
            }
            catch (Exception ex)
            {
                mensajeResultado.noError = (99);
                mensajeResultado.resultado = (0);
            }
            return listTablaBase;
        }

        private List<TipoControlador> obtieneTipoControladorTablaBase(string claveTablaBase, List<TablaBase> tablasBaseSistema)
        {
            string valorTipoControlador = (from tabla in tablasBaseSistema where tabla.clave.Equals(claveTablaBase) select tabla.controladores).SingleOrDefault<string>();
            List<TipoControlador> tipoControladores = null;
            if (String.IsNullOrEmpty(valorTipoControlador))
            {
                mensajeResultado.noError = 1000;
                mensajeResultado.error = "No tiene controlador la tabla base";
                mensajeResultado.resultado = 0;
            }
            else
            {
                tipoControladores = obtieneTipoControladorDeString(valorTipoControlador.Split(','));
            }
            return tipoControladores;
        }

        private List<TipoControlador> obtieneTipoControladorDeString(string[] valorControladores)
        {
            List<TipoControlador> tipoControladores = new List<TipoControlador>();
            foreach (var item in valorControladores)
            {
                tipoControladores.Add((TipoControlador)Entity.util.ManejadorEnum.GetValue(item, typeof(TipoControlador)));
            }
            return tipoControladores;
        }

        private Object[,] construyeTablaXmlPorTipoNomina(String claveTablaBase, String clavePeriodicidad, List<TipoControlador> tipoControladores, DateTime fechaFinal, int ejercicio)
        {
            Object[,] valores = null;
            try
            {
                strQuery.Remove(0, strQuery.Length).Append("SELECT o.fileXml FROM TablaDatos o ");
                strQuery.Append("WHERE o.tablaBase.clave  = :claveTablaBase ");
                strQuery.Append("  AND o.id =  (SELECT MAX(t.id) FROM TablaDatos t WHERE t.tablaBase.id  = o.tablaBase.id ");
                bool controlFecha = false, controlAño = false;
                for (int i = 0; i < tipoControladores.Count; i++)
                {
                    if (tipoControladores[i] == TipoControlador.CONTROLADORENTIDAD)
                    {
                        strQuery.Append("AND t.controladores LIKE :controlador ");
                    }
                    else if (tipoControladores[i] == TipoControlador.CONTROLPORFECHA)
                    {
                        controlFecha = true;
                        strQuery.Append("AND t.controlPorFecha <= :fechaActual ");

                    }
                    else if (tipoControladores[i] == TipoControlador.CONTROLPORAÑO)
                    {
                        strQuery.Append("AND t.controlPorAnio <= :year ");
                        controlAño = true;
                    }
                }
                strQuery.Append(")");

                IQuery query = getSession().CreateQuery(strQuery.ToString());
                query.SetString("claveTablaBase", claveTablaBase);
                if (clavePeriodicidad.Length > 0)
                {
                    query.SetString("controlador", clavePeriodicidad);
                }
                if (controlFecha)
                {
                    query.SetParameter("fechaActual", fechaFinal);
                }
                if (controlAño)
                {
                    query.SetParameter("year", ejercicio);
                }
                byte[] result = (byte[])query.UniqueResult();
                if (result == null)
                {
                    if (controlFecha)
                    {
                        query.SetParameter("fechaActual", fechaActual.Date);
                    }
                    if (controlAño)
                    {
                        query.SetParameter("year", fechaActual.Year);
                    }
                    result = (byte[])query.UniqueResult();
                }
                if (result != null)
                {
                    valores = util.UtilidadesXML.extraeValoresNodos(util.UtilidadesXML.convierteBytesToXML(result));
                    if (util.UtilidadesXML.ERROR_XML > 0)
                    {
                        mensajeResultado.noError = util.UtilidadesXML.mensajeError.noError;
                        mensajeResultado.error = util.UtilidadesXML.mensajeError.error;
                        mensajeResultado.resultado = util.UtilidadesXML.mensajeError.resultado;
                        return null;
                    }
                }
                //            //strQuery = null;
            }
            catch (Exception ex)
            {
                mensajeResultado.noError = (-10);
                mensajeResultado.error = ex.GetBaseException().ToString();
                // System.err.println(concatena.delete(0, concatena.length()).append(msgError).append("construyeTablaXmlPorTipoNomina()1_Error: ").append(ex));
            }
            return valores;
        }

        private Object[,] construyeTablaXml(String claveTablaBase, String controlador, List<TipoControlador> tipoControladores, DateTime fechaFinal, int ejercicio)
        {
            Object[,] valores = null;
            try
            {
                controlador = (controlador == null) ? "" : controlador;
                bool controlFecha = false, controlAño = false, controlEntidad = false;
                strQuery.Remove(0, strQuery.Length).Append("SELECT o.fileXml FROM TablaDatos o WHERE  o.tablaBase.clave  = :claveTablaBase ");
                String controler = "";
                for (int i = 0; i < tipoControladores.Count; i++)
                {
                    if (tipoControladores[i] == TipoControlador.CONTROLPORFECHA)
                    {
                        controlFecha = true;
                        controler = string.Concat(controler, "AND t.controlPorFecha <= :fechaActual ");

                    }
                    else if (tipoControladores[i] == TipoControlador.CONTROLPORAÑO)
                    {
                        controler = string.Concat(controler, "AND t.controlPorAnio <= :year ");
                        controlAño = true;
                    }
                    else if (tipoControladores[i] == TipoControlador.CONTROLADORENTIDAD)
                    {
                        controlEntidad = true;
                    }
                }

                if (!controlFecha & !controlAño)
                {
                    strQuery.Append("AND o.id = (SELECT MAX(t.id) ");
                }
                else if (controlFecha)
                {
                    strQuery.Append("AND o.controlPorFecha = (SELECT MAX(t.controlPorFecha) ");
                }
                else if (controlFecha)
                {
                    strQuery.Append("AND o.controlPorAnio = (SELECT MAX(t.controlPorAnio) ");
                }
                else
                {
                    strQuery.Append("AND o.id = (SELECT MAX(t.id) ");
                }
                strQuery.Append("FROM TablaDatos t WHERE t.tablaBase.id  = o.tablaBase.id ").Append(controler);
                if (controlador.Any() & controlEntidad)
                {
                    strQuery.Append("AND t.controladores = :controlador");
                }
                strQuery.Append(")");

                IQuery query = getSession().CreateQuery(strQuery.ToString());
                query.SetString("claveTablaBase", claveTablaBase);
                if (controlFecha)
                {
                    query.SetParameter("fechaActual", fechaFinal);
                }
                if (controlAño)
                {
                    query.SetParameter("year", ejercicio);
                }
                if (controlador.Any() & controlEntidad)
                {
                    query.SetString("controlador", controlador);
                    controlEntidad = true;
                }
                byte[] result = (byte[])query.UniqueResult();  // busca con varios controladores definidos
                if (result == null)
                {
                    strQuery.Remove(0, strQuery.Length).Append("SELECT o.fileXml FROM TablaDatos o WHERE  o.tablaBase.clave  = :claveTablaBase ");
                    ////strQuery.append("AND o.id =  (SELECT MAX(t.id) FROM TablaDatos t WHERE t.tablaBase.id  = o.tablaBase.id ");
                    controlFecha = false;
                    controler = "";
                    for (int i = 0; i < tipoControladores.Count; i++)
                    {
                        if (tipoControladores[i] == TipoControlador.CONTROLPORFECHA)
                        {
                            controlFecha = true;
                            controler = string.Concat(controler, "AND t.controlPorFecha <= :fechaActual ");
                            break;

                        }
                        else if (tipoControladores[i] == TipoControlador.CONTROLPORAÑO)
                        {
                            controler = string.Concat(controler, "AND t.controlPorAnio <= :year ");
                            controlAño = true;
                            break;
                        }
                        else if (tipoControladores[i] == TipoControlador.CONTROLADORENTIDAD)
                        {
                            if (controlador.Any())
                            {
                                String[] controladores = controlador.Split('#');
                                controlador = controladores[1];
                            }
                            controler = string.Concat(controler, "AND t.controladores = :controlador) ");

                            break;
                        }
                    }
                    if (!controlFecha & !controlAño)
                    {
                        strQuery.Append("AND o.id = (SELECT MAX(t.id) ");
                    }
                    else if (controlFecha)
                    {
                        strQuery.Append("AND o.controlPorFecha = (SELECT MAX(t.controlPorFecha) ");
                    }
                    else if (controlFecha)
                    {
                        strQuery.Append("AND o.controlPorAnio = (SELECT MAX(t.controlPorAnio) ");
                    }
                    else
                    {
                        strQuery.Append("AND o.id = (SELECT MAX(t.id) ");
                    }
                    strQuery.Append("FROM TablaDatos t WHERE t.tablaBase.id  = o.tablaBase.id ").Append(controler);
                    query = getSession().CreateQuery(strQuery.ToString());
                    query.SetString("claveTablaBase", claveTablaBase);
                    if (controlFecha)
                    {
                        query.SetParameter("fechaActual", fechaActual.Date);
                    }
                    else if (controlAño)
                    {
                        query.SetParameter("year", fechaActual.Year);
                    }
                    else if (controlador.Any() & controlEntidad)
                    {
                        query.SetString("controlador", controlador);
                    }
                    result = (byte[])query.UniqueResult();  // busca con controlador necesario nomas
                }

                if (result != null)
                {
                    valores = util.UtilidadesXML.extraeValoresNodos(util.UtilidadesXML.convierteBytesToXML(result));
                    if (util.UtilidadesXML.ERROR_XML > 0)
                    {
                        mensajeResultado.noError = util.UtilidadesXML.mensajeError.noError;
                        mensajeResultado.error = util.UtilidadesXML.mensajeError.error;
                        mensajeResultado.resultado = util.UtilidadesXML.mensajeError.resultado;
                        return null;
                    }
                }




            }
            catch (Exception ex)
            {

                mensajeResultado.noError = (-10);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }


            return valores;
        }

        #endregion

        #region carga de variables
        private void cargarVariablesConceptosCompilador()
        {
            try
            {
                IQuery q = getSession().CreateQuery(CONSULTA_CONCEPTO_CON_NOMENCLATURA.ToString());
                var valores = q.List();
                if (valores != null)
                {
                    int i, j;
                    variablesConceptos = new String[valores.Count, 2];
                    for (i = 0; i < valores.Count; i++)
                    {
                        IList<object> subValores = (IList<object>)valores[i];
                        for (j = 0; j < subValores.Count; j++)
                        {
                            variablesConceptos[i, j] = subValores[j].ToString().Replace(" ", "_");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensajeResultado.error = ex.GetBaseException().Message;
                mensajeResultado.noError = (25);
                return;
            }
            //if (valores != null)
            //{
            //    int i, j;
            //    variablesConceptos = new String[valores.Count(),2];
            //    for ( i = 0; i < valores.Count(); i++)
            //    {
            //        for ( j = 0; j < valores[i]; j++)
            //        {
            //            variablesConceptos[i, j] = valores[i][j];
            //        }

            //    }
            //}
        }

        private void cargaVariablesGlobales(string claveTipoNomina, string claveTipoCorrida, string clavePuesto, string claveCategoriasPuesto, string claveTurno,
          string claveRazonSocial, string claveRegPatronal, string claveDepto, string claveCtrCosto)
        {
            try
            {
                if (isUMA)
                {
                    salarioMinimoDF = valorUMA;
                    valoresConceptosGlobales["SalarioMinDF".ToUpper()] = salarioMinimoDF;
                }
                else
                {
                    ZonaSalarial salarioZona = buscaSalarioPorZona('A');
                    if (salarioZona == null)
                    {
                        mensajeResultado.noError = 20;
                        mensajeResultado.error = "No existe zona salarial A";
                        return;
                    }
                    salarioMinimoDF = salarioZona.salario;
                    valoresConceptosGlobales["SalarioMinDF".ToUpper()] = salarioMinimoDF;
                }

                // valoresConceptosGlobales[typeof(RazonesSociales).Name.ToUpper()] = claveRazonSocial;/*se puede quitar mas adelante*/
                valoresConceptosGlobales[typeof(RazonSocial).Name.ToUpper()] = claveRazonSocial;
                //valoresConceptosGlobales[typeof(TipoCorrida).Name.ToUpper()] = claveTipoCorrida;
                valoresConceptosGlobales[typeof(CentroDeCosto).Name.ToUpper()] = claveCtrCosto;
                valoresConceptosGlobales["NumCentroCostos".ToUpper()] = claveCtrCosto;/*se puede quitar mas adelante*/
                valoresConceptosGlobales["ClaveTipoCorrida".ToUpper()] = claveTipoCorrida;
                valoresConceptosGlobales["NumPuesto".ToUpper()] = clavePuesto;
                valoresConceptosGlobales["NumCategoria".ToUpper()] = claveCategoriasPuesto;
                valoresConceptosGlobales["NumDepartamento".ToUpper()] = claveDepto;
                valoresConceptosGlobales["NumTurno".ToUpper()] = claveTurno == null ? "" : claveTurno;
                valoresConceptosGlobales[typeof(TipoNomina).Name.ToUpper()] = claveTipoNomina;
                valoresConceptosGlobales["NumRegistroPatronal".ToUpper()] = claveRegPatronal;
                tipoCorrida = (TipoCorrida)ejecutaQueryUnico("From TipoCorrida tc Where tc.clave = :clave", new String[] { "clave" }, new Object[] { claveTipoCorrida });
                //tipoCorrida = (from c in dbContextSimple.Set<TipoCorrida>() where c.clave == claveTipoCorrida select c).SingleOrDefault<TipoCorrida>();
                valoresConceptosGlobales["TipoCorridaAlfa".ToUpper()] = tipoCorrida == null ? "" : tipoCorrida.descripcion;
                razonesSociales = (RazonesSociales)ejecutaQueryUnico("From RazonesSociales rs Where rs.clave = :clave", new String[] { "clave" }, new Object[] { claveRazonSocial });
                salarioMinimoDF = null;
                configuracionIMSS = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("cargaVariablesGlobales()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }

        }

        private ZonaSalarial buscaSalarioPorZona(char zona)
        {
            ZonaSalarial salario = null;
            for (int i = 0; i < tablaZonaSalarial.GetLength(0); i++)
            {
                if (string.Equals(tablaZonaSalarial[i, 0].ToString(), zona.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    salario = new ZonaSalarial(new object[] { tablaZonaSalarial[i, 0], tablaZonaSalarial[i, 1] });
                }
            }
            return salario;
        }

        private void inicializaValoresPeriodoNomina(PeriodosNomina periodo)
        {
            valoresConceptosGlobales["IdPeriodo".ToUpper()] = periodo == null ? 0L : periodo.id;
            valoresConceptosGlobales["NumPeriodo".ToUpper()] = periodo == null ? "" : periodo.clave;
            DateTime fecha = new DateTime(1900, 1, 1);
            valoresConceptosGlobales["FechaInicioPeriodo".ToUpper()] = periodo == null ? fecha : periodo.fechaInicial;
            valoresConceptosGlobales["FechaFinalPeriodo".ToUpper()] = periodo == null ? fecha : periodo.fechaFinal;
            valoresConceptosGlobales["FechaPago".ToUpper()] = periodo == null ? fecha : periodo.fechaPago;
            valoresConceptosGlobales["NumMesAfectar".ToUpper()] = periodo == null ? 0 : periodosNomina.acumularAMes.Value.Month;
            valoresConceptosGlobales["DiasNaturalesDelPeriodo".ToUpper()] = periodo == null ? 0 : (cantidadDiasEntreDosFechas(periodo.fechaInicial.Value, periodo.fechaFinal.Value) + 1);
            valoresConceptosGlobales["PeriodicidadEnDias".ToUpper()] = periodo == null ? 0 : int.Parse(periodo.tipoNomina.periodicidad.dias.ToString());
            valoresConceptosGlobales["AnioPeriodo".ToUpper()] = periodo == null ? periodosNomina == null ? 0 : periodosNomina.acumularAMes.Value.Year : periodo.año;
            valoresConceptosGlobales["TipoPeriodicidadNumerico".ToUpper()] = periodo == null ? "" : periodo.tipoNomina.periodicidad.clave;
            valoresConceptosGlobales["AnioActualNumerico".ToUpper()] = periodo == null ? 0 : periodo.año;

            if (periodo != null)
            {
                DateTime fecInicio = new DateTime();
                fecInicio = periodo.fechaInicial.Value;
                int mes = fecInicio.Month;
                //SimpleDateFormat mesLar = new SimpleDateFormat("MMMMM");
                //SimpleDateFormat mesCor = new SimpleDateFormat("MMM");
                String mesNomLar = fecInicio.ToString("MMMM");
                String mesNomCor = fecInicio.ToString("MMM");
                valoresConceptosGlobales["DiasMesNumerico".ToUpper()] = mes;
                valoresConceptosGlobales["MesAlfanumCompleto".ToUpper()] = mesNomLar;
                valoresConceptosGlobales["MesAlfanumCorto".ToUpper()] = mesNomCor;

                int anio = fecInicio.Year;
                if ((anio % 4 == 0) && ((anio % 100 != 0) || (anio % 400 == 0)))
                {
                    valoresConceptosGlobales["DiasPrimerSemestre".ToUpper()] = 182;
                }
                else
                {
                    valoresConceptosGlobales["DiasPrimerSemestre".ToUpper()] = 181;
                }
                valoresConceptosGlobales["DiasSegundoSemestre".ToUpper()] = 184;
                //para sacar el numero de semestre
                DateTime fecha1 = new DateTime(fecInicio.Year, 1, 1);
                DateTime fecha2 = new DateTime(fecInicio.Year, 6, 30);

                if ((fecInicio.CompareTo(fecha1.Date) == 0 || fecInicio.CompareTo(fecha1.Date) == 1)
                    && (fecInicio.CompareTo(fecha2.Date) == 0 || fecInicio.CompareTo(fecha2.Date) == -1))
                {
                    valoresConceptosGlobales["NumeroSemestre".ToUpper()] = 1;
                }
                else
                {
                    valoresConceptosGlobales["NumeroSemestre".ToUpper()] = 2;
                }
            }

        }

        private int cantidadDiasEntreDosFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            TimeSpan span = fechaFin - fechaInicio;
            return span.Days;
        }

        private void cargaDatosCalculoIMSS(DateTime fechaFinal)
        {

            if (valoresConceptosGlobales == null)
            {
                valoresConceptosGlobales = new Dictionary<string, object>();
            }

            configuracionIMSS = (ConfiguracionIMSS)ejecutaQueryUnico("from ConfiguracionIMSS where fechaAplica = ( select max(fechaAplica) from ConfiguracionIMSS where fechaAplica <= :fecha ) ", new String[] { "fecha" }, new Object[] { fechaFinal });
            if (mensajeResultado.noError == -100 || configuracionIMSS == null)
            {
                configuracionIMSS = (ConfiguracionIMSS)ejecutaQueryUnico("from ConfiguracionIMSS where fechaAplica = ( select min(fechaAplica) from ConfiguracionIMSS where fechaAplica >= :fecha ) ", new String[] { "fecha" }, new Object[] { fechaFinal });

            }
            valoresConceptosGlobales.Add("TasaExcedenteEmp".ToUpper(), configuracionIMSS.tasaEspecieEnfermeMater);
            valoresConceptosGlobales.Add("TasaGtosPensEmp".ToUpper(), configuracionIMSS.tasaGastosPension);
            valoresConceptosGlobales.Add("TasaPrestDinEmp".ToUpper(), configuracionIMSS.tasaDineEnfermeMater);
            valoresConceptosGlobales.Add("TasaInvYVidaEmp".ToUpper(), configuracionIMSS.tasaInvalidezVida);
            valoresConceptosGlobales.Add("TasaCesYVejezEmp".ToUpper(), configuracionIMSS.tasaCesantiaVejez);

            valoresConceptosGlobales.Add("TasaInfonavit".ToUpper(), configuracionIMSS.tasaAportacionInfonavitPatron);
            valoresConceptosGlobales.Add("TasaCesYVejezPat".ToUpper(), configuracionIMSS.tasaCesanVejezPatron);
            valoresConceptosGlobales.Add("TasaFijaPatron".ToUpper(), configuracionIMSS.tasaFijaPatron);
            valoresConceptosGlobales.Add("TasaExcedentePat".ToUpper(), configuracionIMSS.tasaExcedentePatron);
            valoresConceptosGlobales.Add("TasaGuarderiasPat".ToUpper(), configuracionIMSS.tasaGuarderiaPatron);
            valoresConceptosGlobales.Add("TasaInvYVidaPat".ToUpper(), configuracionIMSS.tasaInvaliVidaPatron);
            valoresConceptosGlobales.Add("TasaGtosPensPat".ToUpper(), configuracionIMSS.tasaGastosPensPatron);
            valoresConceptosGlobales.Add("TasaRetiro".ToUpper(), configuracionIMSS.tasaAportacionRetiroPatron);
            valoresConceptosGlobales.Add("TasaRiesgoTrabajoPat".ToUpper(), configuracionIMSS.tasaRiesgosPatron);

            valoresConceptosGlobales.Add("TopeEnfermedadYMat".ToUpper(), configuracionIMSS.topeEnfermedadMaternidad);
            valoresConceptosGlobales.Add("TopeRiesgoTrabajoGuarderia".ToUpper(), configuracionIMSS.topeRiesgoTrabajoGuarderias);
            valoresConceptosGlobales.Add("TopeInvalidezYVida".ToUpper(), configuracionIMSS.topeCesanVejez);
            valoresConceptosGlobales.Add("TopeRetiro".ToUpper(), configuracionIMSS.topeRetiro);
            valoresConceptosGlobales.Add("TopeCuotaInfonavit".ToUpper(), configuracionIMSS.topeInfonavit);
            //            valoresConceptos.Add("TasaRetiro".ToUpper(), configuracionIMSS.salarioRetiro);
            //            valoresConceptos.Add("TasaRetiro".ToUpper(), configuracionIMSS.salarioRiesgoTrabajoGuarderias);




        }

        private void cargaDatosVariableConfiguracionIMSS(DateTime fechaPeriodoFinal)
        {
            try
            {

                if (valoresConceptosGlobales == null)
                {
                    valoresConceptosGlobales = new Dictionary<string, object>();
                }

                configuracionIMSS = (ConfiguracionIMSS)ejecutaQueryUnico("from ConfiguracionIMSS where fechaAplica = ( select max(fechaAplica) from ConfiguracionIMSS where fechaAplica <= :fecha ) ", new String[] { "fecha" }, new Object[] { fechaPeriodoFinal });
                if (mensajeResultado.noError == -100 || configuracionIMSS == null)
                {
                    configuracionIMSS = (ConfiguracionIMSS)ejecutaQueryUnico("from ConfiguracionIMSS where fechaAplica = ( select min(fechaAplica) from ConfiguracionIMSS where fechaAplica >= :fecha ) ", new String[] { "fecha" }, new Object[] { fechaPeriodoFinal });

                }
                valoresConceptosGlobales["TasaExcedenteEmp".ToUpper()] = configuracionIMSS.tasaEspecieEnfermeMater;
                valoresConceptosGlobales["TasaGtosPensEmp".ToUpper()] = configuracionIMSS.tasaGastosPension;
                valoresConceptosGlobales["TasaPrestDinEmp".ToUpper()] = configuracionIMSS.tasaDineEnfermeMater;
                valoresConceptosGlobales["TasaInvYVidaEmp".ToUpper()] = configuracionIMSS.tasaInvalidezVida;
                valoresConceptosGlobales["TasaCesYVejezEmp".ToUpper()] = configuracionIMSS.tasaCesantiaVejez;

                valoresConceptosGlobales["TasaInfonavit".ToUpper()] = configuracionIMSS.tasaAportacionInfonavitPatron;
                valoresConceptosGlobales["TasaPrestDinPat".ToUpper()] = configuracionIMSS.tasaPrestDinePatron;
                valoresConceptosGlobales["TasaCesYVejezPat".ToUpper()] = configuracionIMSS.tasaCesanVejezPatron;
                valoresConceptosGlobales["TasaFijaPatron".ToUpper()] = configuracionIMSS.tasaFijaPatron;
                valoresConceptosGlobales["TasaExcedentePat".ToUpper()] = configuracionIMSS.tasaExcedentePatron;
                valoresConceptosGlobales["TasaGuarderiasPat".ToUpper()] = configuracionIMSS.tasaGuarderiaPatron;
                valoresConceptosGlobales["TasaInvYVidaPat".ToUpper()] = configuracionIMSS.tasaInvaliVidaPatron;
                valoresConceptosGlobales["TasaGtosPensPat".ToUpper()] = configuracionIMSS.tasaGastosPensPatron;
                valoresConceptosGlobales["TasaRetiro".ToUpper()] = configuracionIMSS.tasaAportacionRetiroPatron;
                valoresConceptosGlobales["TasaRiesgoTrabajoPat".ToUpper()] = configuracionIMSS.tasaRiesgosPatron;

                valoresConceptosGlobales["TopeEnfermedadYMat".ToUpper()] = configuracionIMSS.topeEnfermedadMaternidad;
                valoresConceptosGlobales["TopeRiesgoTrabGuarderia".ToUpper()] = configuracionIMSS.topeRiesgoTrabajoGuarderias;
                valoresConceptosGlobales["TopeInvalidezYVida".ToUpper()] = configuracionIMSS.topeCesanVejez;
                valoresConceptosGlobales["TopeRetiro".ToUpper()] = configuracionIMSS.topeRetiro;
                valoresConceptosGlobales["TopeCuotaInfonavit".ToUpper()] = configuracionIMSS.topeInfonavit;
            }
            catch (HibernateException ex)
            {
                mensajeResultado.noError = (28);
                // System.err.println(concatena.delete(0, concatena.length()).append(msgError).append("cargaDatosVariableConfiguracionIMSS()1_Error: ").append(ex));
            }
        }

        private void cargarVariablesGlobalesEmpleadoPorPlaza(PlazasPorEmpleadosMov plazaPorEmpleadoMov, bool factor, bool zonaSalario, TipoSueldos tipoSueldos, CalculoUnidades calculoUnidad,
          bool? modificarDiasTrabajados, bool? modificarDiasCotizacion, DateTime? fechaActualCalculoSDI)
        {
            try
            {

                if (!isCalculoSDI)
                {
                    cargaVariablesGlobalesEmpleado(plazaPorEmpleadoMov.plazasPorEmpleado.empleados);
                    if (mensajeResultado.noError != 0)
                    {
                        return;
                    }
                    valoresConceptosEmpleados["Antiguedad".ToUpper()] = calcularAntiguedadExacta(plazaPorEmpleadoMov.plazasPorEmpleado.fechaPrestaciones.GetValueOrDefault(), TipoAntiguedad.ANTIGUEDAD_ENTERO);
                    valoresConceptosEmpleados["AntiguedadExacta".ToUpper()] = calcularAntiguedadExacta(plazaPorEmpleadoMov.plazasPorEmpleado.fechaPrestaciones.GetValueOrDefault(), TipoAntiguedad.ANTIGUEDAD_EXACTA);
                    valoresConceptosEmpleados["PorcionAntiguedad".ToUpper()] = calcularAntiguedadExacta(plazaPorEmpleadoMov.plazasPorEmpleado.fechaPrestaciones.GetValueOrDefault(), TipoAntiguedad.PORCION_ANTIGUEDAD);
                    valoresConceptosEmpleados["PorcionDias".ToUpper()] = calcularAntiguedadExacta(plazaPorEmpleadoMov.plazasPorEmpleado.fechaPrestaciones.GetValueOrDefault(), TipoAntiguedad.PORCION_DIAS);
                }
                else
                {
                    valoresConceptosEmpleados["NumEmpleado".ToUpper()] = plazaPorEmpleadoMov.plazasPorEmpleado.empleados.clave;
                    valoresConceptosEmpleados["Estatus".ToUpper()] = plazaPorEmpleadoMov.plazasPorEmpleado.empleados.status;
                }
                valoresConceptosEmpleados["PlazaEmpleado".ToUpper()] = plazaPorEmpleadoMov.plazasPorEmpleado.referencia;
                valoresConceptosEmpleados["PlazaEmpleadoMovimiento".ToUpper()] = plazaPorEmpleadoMov;
                valoresConceptosEmpleados["NumDepartamento".ToUpper()] = plazaPorEmpleadoMov.departamentos == null ? "" : plazaPorEmpleadoMov.departamentos.clave;

                valoresConceptosEmpleados.Add("NumSubcuenta".ToUpper(), plazaPorEmpleadoMov.departamentos == null ? "" : plazaPorEmpleadoMov.departamentos.subCuenta);
                valoresConceptosEmpleados.Add("DepartamentoEmpleadoAlfa".ToUpper(), plazaPorEmpleadoMov.departamentos == null ? "" : plazaPorEmpleadoMov.departamentos.descripcion);
                valoresConceptosEmpleados["NumTurno".ToUpper()] = plazaPorEmpleadoMov.turnos == null ? "" : plazaPorEmpleadoMov.turnos.clave;

                valoresConceptosEmpleados["TipoTurno".ToUpper()] = plazaPorEmpleadoMov.turnos == null ? -1 : plazaPorEmpleadoMov.turnos.tipoDeTurno;
                valoresConceptosEmpleados["HrsTurno".ToUpper()] = plazaPorEmpleadoMov.turnos == null ? 0 : plazaPorEmpleadoMov.turnos.horaJornada;
                valoresConceptosEmpleados["DiasJornada".ToUpper()] = plazaPorEmpleadoMov.turnos == null ? 0 : plazaPorEmpleadoMov.turnos.diasJornada;

                /*  valoresConceptosEmpleados.Add("FormaPago".ToUpper(), plazaPorEmpleadoMov.formasDePago == null ? "" : plazaPorEmpleadoMov.formasDePago.clave);*/

                valoresConceptosEmpleados["NumDelegacion".ToUpper()] = plazaPorEmpleadoMov.plazasPorEmpleado.registroPatronal == null ? "" : plazaPorEmpleadoMov.plazasPorEmpleado.registroPatronal.delegacion;
                valoresConceptosEmpleados["NumSubdelegacion".ToUpper()] = plazaPorEmpleadoMov.plazasPorEmpleado.registroPatronal == null ? "" : plazaPorEmpleadoMov.plazasPorEmpleado.registroPatronal.subdelegacion;

                if (factor)
                {

                    FactorIntegracion factorIntegracion = buscaFactorIntegracion(plazaPorEmpleadoMov.plazasPorEmpleado.fechaPrestaciones.GetValueOrDefault());

                    if (mensajeResultado.noError != 0)
                    {
                        return;
                    }
                    if (factorIntegracion != null)
                    {
                        valoresConceptosEmpleados["AntiguedadEntero".ToUpper()] = factorIntegracion.antiguedad;
                        double aux = Convert.ToDouble(factorIntegracion.diasAguinaldo);
                        valoresConceptosEmpleados["FactorDiasAguinaldo".ToUpper()] = aux;
                        valoresConceptosEmpleados["FactorIntegracion".ToUpper()] = factorIntegracion.factor;
                        valoresConceptosEmpleados["FactorDiasVacaciones".ToUpper()] = factorIntegracion.diasVacaciones;
                        valoresConceptosEmpleados["FactorPrimaVacacional".ToUpper()] = factorIntegracion.primaVacacional / 100;
                        valoresConceptosEmpleados["DiasVacacionesTotales".ToUpper()] = factorIntegracion.diasVacacionesTotales;
                    }
                    else
                    {
                        mensajeResultado.noError = 25;
                        mensajeResultado.error = "No existen el factor de integracion capturado, favor de verificarlo";
                        return;
                    }
                }
                int diasAguinaldo = 0, diasTrabajados;
                double diasAguinaldoExacta, porcionAguinaldo = 0.0, factorDiasAguinaldo = 0;
                if (valoresConceptosEmpleados["FactorDiasAguinaldo".ToUpper()] != null)
                {
                    factorDiasAguinaldo = (double)valoresConceptosEmpleados["FactorDiasAguinaldo".ToUpper()];
                }
                diasTrabajados = Convert.ToInt16(calcularAntiguedadExacta(plazaPorEmpleadoMov.plazasPorEmpleado.fechaPrestaciones.GetValueOrDefault(), TipoAntiguedad.PORCION_DIAS));
                diasAguinaldoExacta = factorDiasAguinaldo * diasTrabajados / 365.0;
                diasAguinaldo = (int)diasAguinaldoExacta;

                porcionAguinaldo = diasAguinaldoExacta - diasAguinaldo;
                valoresConceptosEmpleados["DiasAguinaldo".ToUpper()] = diasAguinaldo;
                valoresConceptosEmpleados["PorcionAguinaldo".ToUpper()] = porcionAguinaldo;
                string claveRazonSocial = "";
                if (valoresConceptosEmpleados.ContainsKey("RazonSocial".ToUpper()))
                {
                    claveRazonSocial = valoresConceptosEmpleados["RazonSocial".ToUpper()] == null ? "" : valoresConceptosEmpleados["RazonSocial".ToUpper()].ToString();
                }
                if (String.IsNullOrEmpty(claveRazonSocial))
                {   //Aqui
                    claveRazonSocial = plazaPorEmpleadoMov.plazasPorEmpleado.razonesSociales.clave;
                    valoresConceptosEmpleados.Add("RazonSocial".ToUpper(), claveRazonSocial);
                    razonesSociales = razonesSociales = (RazonesSociales)ejecutaQueryUnico("From RazonesSociales rs Where rs.clave = :clave", new String[] { "clave" }, new Object[] { claveRazonSocial });
                }
                centroDeCostoMovimiento = plazaPorEmpleadoMov.centroDeCosto;
                cargaDatosSalarioDiario(plazaPorEmpleadoMov, periodosNomina);
                valoresConceptosEmpleados["FechaAlta".ToUpper()] = ingresosReingresosBajas == null ? plazaPorEmpleadoMov.fechaInicial : ingresosReingresosBajas.fechaIngreso;
                valoresConceptosEmpleados["FechaBaja".ToUpper()] = fechaBajaFiniq == null ? plazaPorEmpleadoMov.plazasPorEmpleado.fechaFinal : fechaBajaFiniq;
                valoresConceptosEmpleados["FechaAltaIMSS".ToUpper()] = obtenerPrimerPlazasPorEmpleadosMov(plazaPorEmpleadoMov);
                //if (plazaPorEmpleadoMov.zonaGeografica != null)
                //{
                //    String emplZonaGeog = plazaPorEmpleadoMov.zonaGeografica == (int)ZonaGeografica.ZonaGeograficaA ? "A" : "B";
                //    valoresConceptosEmpleados.Add("AreaGeografica".ToUpper(), emplZonaGeog);
                //    valoresConceptosEmpleados.Add("NumZonaSalario".ToUpper(), emplZonaGeog);
                //    if (isUMA)
                //    {
                //        valoresConceptosEmpleados.Add("SalarioMin".ToUpper(), valorUMA);
                //    }
                //    else if (zonaSalario)
                //    {
                //        ZonaSalarial salarioZona = buscaSalarioPorZona(emplZonaGeog[0]);
                //        if (salarioZona == null)
                //        {
                //            mensajeResultado.noError = 40;
                //            mensajeResultado.error = String.Concat("No existe Zona Salarial ", emplZonaGeog[0]);
                //            return;
                //        }
                //        else
                //        {
                //            valoresConceptosEmpleados.Add("SalarioMin".ToUpper(), salarioZona.salario);
                //        }
                //    }
                //}
                //else
                //{
                //    if (isUMA)
                //    {
                //        valoresConceptosEmpleados.Add("SalarioMin".ToUpper(), valorUMA);
                //    }
                //    else
                //    {
                //        valoresConceptosEmpleados.Add("SalarioMin".ToUpper(), 0.0);
                //    }
                //    valoresConceptosEmpleados.Add("AreaGeografica".ToUpper(), "");
                //    valoresConceptosEmpleados.Add("NumZonaSalario".ToUpper(), "");
                //}
                valoresConceptosEmpleados["NumCentroCostos".ToUpper()] = plazaPorEmpleadoMov.centroDeCosto == null ? "" : plazaPorEmpleadoMov.centroDeCosto.clave;

                valoresConceptosEmpleados["CentroCostoAlfa".ToUpper()] = plazaPorEmpleadoMov.centroDeCosto == null ? "" : plazaPorEmpleadoMov.centroDeCosto.descripcion;

                if (plazaPorEmpleadoMov.puestos != null)
                {
                    valoresConceptosEmpleados["NumPuesto".ToUpper()] = plazaPorEmpleadoMov.puestos.clave;
                    valoresConceptosEmpleados["SueldoPuesto".ToUpper()] = plazaPorEmpleadoMov.puestos.salarioTabular;
                    valoresConceptosEmpleados["TopeSalarial".ToUpper()] = plazaPorEmpleadoMov.puestos.maximo;
                    if (plazaPorEmpleadoMov.puestos.categoriasPuestos != null)
                    {
                        valoresConceptosEmpleados["NumCategoria".ToUpper()] = plazaPorEmpleadoMov.puestos.categoriasPuestos.clave;
                        valoresConceptosEmpleados["CategoriaEmpleadoAlfanum".ToUpper()] = plazaPorEmpleadoMov.puestos.categoriasPuestos.descripcion;
                    }
                }
                else
                {
                    valoresConceptosEmpleados["NumPuesto".ToUpper()] = "";
                    valoresConceptosEmpleados["SueldoPuesto".ToUpper()] = 0.0;
                    valoresConceptosEmpleados["TopeSalarial".ToUpper()] = 0.0;
                    valoresConceptosEmpleados["NumCategoria".ToUpper()] = "";
                    valoresConceptosEmpleados["CategoriaEmpleadoAlfanum".ToUpper()] = "";
                }
                valoresConceptosEmpleados["TipoNomina".ToUpper()] = plazaPorEmpleadoMov.tipoNomina == null ? "" : plazaPorEmpleadoMov.tipoNomina.clave;
                valoresConceptosEmpleados["TipoNominaEntidad".ToUpper()] = plazaPorEmpleadoMov.tipoNomina;
                valoresConceptosEmpleados["TipoNominaAlfa".ToUpper()] = plazaPorEmpleadoMov.tipoNomina == null ? "" : plazaPorEmpleadoMov.tipoNomina.descripcion;
                valoresConceptosEmpleados["NumRegistroPatronal".ToUpper()] = plazaPorEmpleadoMov.plazasPorEmpleado.registroPatronal == null ? "" : plazaPorEmpleadoMov.plazasPorEmpleado.registroPatronal.clave;

                strQuery = new StringBuilder("select s FROM ").Append(typeof(SalariosIntegrados).Name).Append(" s ");
                strQuery.Append(" inner join s.empleados e ");
                strQuery.Append(" WHERE s.fecha = ");
                strQuery.Append(" ( select max(sdi.fecha) FROM ").Append(typeof(SalariosIntegrados).Name).Append(" sdi ");
                strQuery.Append(" WHERE sdi.empleados.id = e.id AND sdi.registroPatronal.id = s.registroPatronal.id AND sdi.fecha <= :fecha ");
                if (tipoCorrida != null)
                {
                    if (string.Equals(tipoCorrida.clave, "FIN", StringComparison.OrdinalIgnoreCase))
                    {
                        strQuery.Append("  and sdi.fecha not in  ");
                        strQuery.Append(" ( select max(sdix.fecha) FROM SalariosIntegrados sdix ");
                        strQuery.Append(" WHERE sdix.empleados.id = e.id AND sdix.registroPatronal.id = s.registroPatronal.id) ");
                    }
                    else
                    {
                        strQuery.Append(" and sdi.finiquitosLiquida is null ");
                    }
                }

                strQuery.Append(" ) and e.clave = :claveEmpleado AND s.registroPatronal.clave = :claveRegPatronal  and e.razonesSociales.clave = :claveRazonesSociales ");
                DateTime? fecha = periodosNomina == null ? null : periodosNomina.fechaFinal;
                if (isCalculoSDI)
                {
                    fecha = DateTime.Now;

                }

                ///fecha = fechaActualCalculoSDI.getTime();//probar con la fecha actual
                SalariosIntegrados salariosIntegrados = (SalariosIntegrados)ejecutaQueryUnico(strQuery.ToString(), new String[] { "claveEmpleado", "claveRegPatronal", "claveRazonesSociales", "fecha" },
                        new Object[] { plazaPorEmpleadoMov.plazasPorEmpleado.empleados.clave, plazaPorEmpleadoMov.plazasPorEmpleado.registroPatronal == null ? "" : plazaPorEmpleadoMov.plazasPorEmpleado.registroPatronal.clave, plazaPorEmpleadoMov.plazasPorEmpleado.razonesSociales.clave, fecha });//JDRA01
                if (salariosIntegrados == null)
                {
                    valoresConceptosEmpleados["SueldoIntIMSS".ToUpper()] = 0.0;
                }
                else
                {
                    valoresConceptosEmpleados["SueldoIntIMSS".ToUpper()] = salariosIntegrados.salarioDiarioIntegrado;
                    valoresConceptosEmpleados["FechaSDI".ToUpper()] = salariosIntegrados.fecha;
                }

                DateTime fechaIngreso = plazaPorEmpleadoMov.plazasPorEmpleado.empleados.fechaIngresoEmpresa.GetValueOrDefault().Date; //quita hrs de fecha
                if (periodosNomina == null)
                {
                    valoresConceptosEmpleados["DiasPeriodoEmpleado".ToUpper()] = 0;
                }
                else
                {
                    DateTime fechaInicioPer = periodosNomina.fechaInicial.GetValueOrDefault().Date;
                    DateTime fechaFinalPer = periodosNomina.fechaFinal.GetValueOrDefault().Date;
                    if (fechaIngreso.CompareTo(fechaInicioPer) == 1)
                    {
                        long diferencia = (long)((fechaFinalPer - fechaIngreso).TotalMilliseconds);
                        double diasdelPer = (double)Math.Floor((double)diferencia / (1000 * 60 * 60 * 24));
                        int diasPer = (int)diasdelPer + 1;
                        valoresConceptosEmpleados["DiasPeriodoEmpleado".ToUpper()] = diasPer;
                    }
                    valoresConceptosEmpleados["TipoCorrida".ToUpper()] = periodosNomina.tipoCorrida == null ? "" : periodosNomina.tipoCorrida.clave;

                }
                valoresConceptosEmpleados["TipoRelacionLabEmpleado".ToUpper()] = plazaPorEmpleadoMov.tipoRelacionLaboral;
                valoresConceptosEmpleados["TipoContratoEmpleado".ToUpper()] = plazaPorEmpleadoMov.tipoContrato.descripcion;
                valoresConceptosEmpleados["RegimenContratacion".ToUpper()] = plazaPorEmpleadoMov.regimenContratacion;

                int? tipoSalario = plazaPorEmpleadoMov.plazas == null ? 0 : plazaPorEmpleadoMov.plazas.tipoSalario;
                if (tipoSalario == 1)
                {
                    valoresConceptosEmpleados["TipoSalarioAlfa".ToUpper()] = "Fijo";
                }
                else if (tipoSalario == 2)
                {
                    valoresConceptosEmpleados["TipoSalarioAlfa".ToUpper()] = "Variable";
                }
                else if (tipoSalario == 3)
                {
                    valoresConceptosEmpleados["TipoSalarioAlfa".ToUpper()] = "Mixto";
                }

                /*  valoresConceptosEmpleados.Add("FormaPagoEmpleadoAlfa".ToUpper(), plazaPorEmpleadoMov.formasDePago == null ? "" : plazaPorEmpleadoMov.formasDePago.descripcion);*/
                bool sindicalizacion = plazaPorEmpleadoMov.tipoContrato.esSindicalizado;
                if (sindicalizacion)
                {
                    valoresConceptosEmpleados["TipoSindicalizadoAlfa".ToUpper()] = "Sindicalizado";
                }
                else
                {
                    valoresConceptosEmpleados["TipoSindicalizadoAlfa".ToUpper()] = "No Sindicalizado";
                }
                /*Se puede eliminar mas adelante
                //int? estadocivil;
                //if (plazaPorEmpleadoMov.plazasPorEmpleado.empleados.estadoCivil == null)
                //{
                //    estadocivil = 0;
                //}
                //else
                //{
                //    estadocivil = plazaPorEmpleadoMov.plazasPorEmpleado.empleados.estadoCivil;
                //}
                //if (estadocivil == 1)
                //{
                //    valoresConceptosEmpleados["EstadoCivilAlfa".ToUpper()] = "Casado";
                //}
                //else if (estadocivil == 2)
                //{
                //    valoresConceptosEmpleados["EstadoCivilAlfa".ToUpper()] = "Soltero";
                //}
                //else if (estadocivil == 3)
                //{
                //    valoresConceptosEmpleados["EstadoCivilAlfa".ToUpper()] = "Divorciado";
                //}
                //else if (estadocivil == 4)
                //{
                //    valoresConceptosEmpleados["EstadoCivilAlfa".ToUpper()] = "Viudo";
                //}
                //else if (estadocivil == 5)
                //{
                //    valoresConceptosEmpleados["EstadoCivilAlfa".ToUpper()] = "UnionLibre";
                //}
                //else if (estadocivil == 6)
                //{
                //    valoresConceptosEmpleados["EstadoCivilAlfa".ToUpper()] = "Separado";
                //}
                //else
                //{
                //    valoresConceptosEmpleados["EstadoCivilAlfa".ToUpper()] = "";
                //}*/
                valoresConceptosEmpleados["FechaPrestaciones".ToUpper()] = plazaPorEmpleadoMov.plazasPorEmpleado.fechaPrestaciones;
                strQuery = new StringBuilder("SELECT  ce.modoDescuentoCredito  FROM ").Append(typeof(CreditoPorEmpleado).Name).Append(" ce ");
                strQuery.Append("WHERE ce.empleados.clave=:claveEmpleado and ce.razonesSociales.clave=:claveRazonSocial and ce.creditoAhorro.clave='005'");
                int? modoDescuentoInfonavit = (int?)ejecutaQueryUnico(strQuery.ToString(), new String[] { "claveEmpleado", "claveRazonSocial" },
                        new Object[] { plazaPorEmpleadoMov.plazasPorEmpleado.empleados.clave, plazaPorEmpleadoMov.plazasPorEmpleado.razonesSociales.clave });
                if (modoDescuentoInfonavit == null)
                {
                    valoresConceptosEmpleados["ModoDescuentoInfonavitAlfa".ToUpper()] = "";
                }
                else if (modoDescuentoInfonavit == 0)
                {
                    valoresConceptosEmpleados["ModoDescuentoInfonavitAlfa".ToUpper()] = "Importe Fijo";
                }
                else if (modoDescuentoInfonavit == 1)
                {
                    valoresConceptosEmpleados["ModoDescuentoInfonavitAlfa".ToUpper()] = "VSM";
                }
                else if (modoDescuentoInfonavit == 2)
                {
                    valoresConceptosEmpleados["ModoDescuentoInfonavitAlfa".ToUpper()] = "Porcentaje";
                }
                strQuery = new StringBuilder("select count(*) FROM ").Append(typeof(PlazasPorEmpleado).Name).Append(" p ");
                strQuery.Append("where p.empleados.clave=:claveEmpleado and p.razonesSociales.clave=:claveRazon");
                int numPlazas = Convert.ToInt32(ejecutaQueryUnico(strQuery.ToString(), new String[] { "claveEmpleado", "claveRazon" },
                        new Object[] { plazaPorEmpleadoMov.plazasPorEmpleado.empleados.clave, plazaPorEmpleadoMov.plazasPorEmpleado.razonesSociales.clave }));

                valoresConceptosEmpleados["NumeroPlazasEmpleado".ToUpper()] = numPlazas;
                int cont = 0;
                if (plazaPorEmpleadoMov.turnos != null)
                {
                    if (plazaPorEmpleadoMov.turnos.turnosHorariosFijos.Count == 0)
                    {
                        valoresConceptosEmpleados["DiaDescanso1".ToUpper()] = "";
                        valoresConceptosEmpleados["DiaDescanso2".ToUpper()] = "";
                    }
                    for (int i = 0; i < plazaPorEmpleadoMov.turnos.turnosHorariosFijos.Count; i++)
                    {
                        if (plazaPorEmpleadoMov.turnos.turnosHorariosFijos[i].statusDia == 1)
                        {
                            cont++;
                            if (cont == 1)
                            {
                                valoresConceptosEmpleados["DiaDescanso1".ToUpper()] = plazaPorEmpleadoMov.turnos.turnosHorariosFijos[i].diaSemana;
                            }
                            else if (cont == 2)
                            {
                                valoresConceptosEmpleados["DiaDescanso2".ToUpper()] = plazaPorEmpleadoMov.turnos.turnosHorariosFijos[i].diaSemana;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    valoresConceptosEmpleados["DiaDescanso1".ToUpper()] = "";
                    valoresConceptosEmpleados["DiaDescanso2".ToUpper()] = "";
                }
                valoresConceptosEmpleados["FechaIngresoEmpresa".ToUpper()] = plazaPorEmpleadoMov.plazasPorEmpleado.empleados.fechaIngresoEmpresa;
                strQuery = new StringBuilder("SELECT pm.fechaInicial FROM ").Append(typeof(PlazasPorEmpleadosMov).Name).Append(" pm ");
                strQuery.Append("WHERE pm.plazasPorEmpleado.empleados.clave=:claveEmpleado ");
                strQuery.Append("and pm.plazasPorEmpleado.razonesSociales.clave=:claveRazonSocial ");
                strQuery.Append("and pm.cambioSalario=1  and pm.fechaInicial BETWEEN :fechaInicial and :fechaFinal ");
                strQuery.Append("order by pm.fechaInicial desc");
                if (periodosNomina == null)
                {
                    valoresConceptosEmpleados["FechaUltimoCambioSueldo".ToUpper()] = "";
                }
                else
                {
                    Object fechaUltimoCambioSueldo = ejecutaQueryUnico(strQuery.ToString(),
                        new String[] { "claveEmpleado", "claveRazonSocial", "fechaInicial", "fechaFinal" },
                        new Object[]{plazaPorEmpleadoMov.plazasPorEmpleado.empleados.clave,
                            plazaPorEmpleadoMov.plazasPorEmpleado.razonesSociales.clave,
                            periodosNomina.fechaInicial, periodosNomina.fechaFinal});

                    valoresConceptosEmpleados["FechaUltimoCambioSueldo".ToUpper()] = fechaUltimoCambioSueldo == null ? "" : fechaUltimoCambioSueldo;
                }
                strQuery = new StringBuilder("from VacacionesDisfrutadas r where r.empleados.clave = :claveEmpleado and r.razonesSociales.clave = :claveRazonSocial");
                Object vacacionesDis = ejecutaQueryUnico(strQuery.ToString(), new String[] { "claveEmpleado", "claveRazonSocial" },
                        new Object[]{plazaPorEmpleadoMov.plazasPorEmpleado.empleados.clave,
                        plazaPorEmpleadoMov.plazasPorEmpleado.razonesSociales.clave});
                if (vacacionesDis != null)
                {
                    VacacionesDisfrutadas vDis = (VacacionesDisfrutadas)vacacionesDis;
                    valoresConceptosEmpleados["DiasDisfrutadosPrimaVacacional".ToUpper()] = vDis.diasPrimaDisfrutados;
                    valoresConceptosEmpleados["DiasDisfrutadosVacaciones".ToUpper()] = vDis.diasVacDisfrutados;
                }
                cargaValoresDiasPago(plazaPorEmpleadoMov, true, null, calculoUnidad, false, modificarDiasTrabajados);
                if (mensajeResultado.noError != 0)
                {
                    return;
                }
                cargaValoresDiasCotizados(plazaPorEmpleadoMov.fechaIMSS.GetValueOrDefault(), plazaPorEmpleadoMov.plazasPorEmpleado.fechaFinal.GetValueOrDefault(), true, null, calculoUnidad, false, modificarDiasCotizacion);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("cargarVariablesGlobalesEmpleadoPorPlaza()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
        }

        private void cargaValoresDiasCotizados(DateTime fechaIMSSEjecutandose, DateTime fechaFinalEjecutandose, bool inicaPlazaPorEmpleadosMov, SalariosIntegrados plazasPorEmpleadosMovSiguiente, CalculoUnidades calculoUnidades, bool inicia2doMes, bool? modificarDiasTrabajados)
        {
            int diasDif = 0;
            DateTime fechaIni, fechaFin, fechaIniAsistenVacacion, fechaFinAsistenVacacion;
            fechaIni = (DateTime)valoresConceptosEmpleados[parametroFechaInicial];
            fechaFin = (DateTime)valoresConceptosEmpleados[parametroFechaFinal];
            fechaIniAsistenVacacion = (DateTime)valoresConceptosEmpleados[parametroFechaInicial];
            fechaFinAsistenVacacion = (DateTime)valoresConceptosEmpleados[parametroFechaFinal];
            if (periodosNomina != null & !isMov2Meses)
            {
                fechaIni = periodosNomina.fechaInicial.GetValueOrDefault();
                fechaFin = periodosNomina.fechaFinal.GetValueOrDefault();
                fechaIniAsistenVacacion = periodosNomina.fechaInicial.GetValueOrDefault();
                fechaFinAsistenVacacion = periodosNomina.fechaFinal.GetValueOrDefault();
            }
            bool seAplicoDiasDif = false;
            if (((DateTime)valoresConceptosEmpleados["FechaAltaIMSS".ToUpper()]).CompareTo(fechaIni) > 0)//Es mayor fechaAlta a fechaInicial
            {
                diasDif += cantidadDiasEntreDosFechas(fechaIni, ((DateTime)valoresConceptosEmpleados["FechaAltaIMSS".ToUpper()]));
            }
            else if (fechaIMSSEjecutandose.CompareTo(fechaIni) > 0 & !inicaPlazaPorEmpleadosMov | plazasPorEmpleadosMovSiguiente != null)
            {//aqui huvo una promocion o modificacion. inicaPlazaPorEmpleadosMov es para saber si existen mas de una promocion o es unica
                if (fechaIMSSEjecutandose.CompareTo(fechaIni) > 0 & !inicia2doMes)
                {
                    diasDif += cantidadDiasEntreDosFechas(fechaIni, fechaIMSSEjecutandose);

                    fechaFinAsistenVacacion = fechaIMSSEjecutandose;
                    fechaFinAsistenVacacion.AddDays(fechaFinAsistenVacacion.Day - 1);
                    seAplicoDiasDif = true;
                }
                if (plazasPorEmpleadosMovSiguiente != null)
                {
                    if (fechaIMSSEjecutandose.CompareTo(fechaFin) == 0 | plazasPorEmpleadosMovSiguiente.fecha.GetValueOrDefault().CompareTo(fechaFin) == 0)
                    {
                        diasDif += 1;
                        if (fechaIMSSEjecutandose.CompareTo(fechaFin) == 0)
                        {
                            fechaFinAsistenVacacion = fechaIMSSEjecutandose;
                        }
                        else
                        {
                            fechaFinAsistenVacacion = plazasPorEmpleadosMovSiguiente.fecha.GetValueOrDefault();
                        }
                        fechaFinAsistenVacacion.AddDays(fechaFinAsistenVacacion.Day - 1);
                    }
                    else
                    {
                        if (plazasPorEmpleadosMovSiguiente.fecha.GetValueOrDefault().CompareTo(fechaFin) > 0) //after java
                        {
                            diasDif += cantidadDiasEntreDosFechas(fechaFin, plazasPorEmpleadosMovSiguiente.fecha.GetValueOrDefault());
                        }
                        else
                        {
                            diasDif += cantidadDiasEntreDosFechas(plazasPorEmpleadosMovSiguiente.fecha.GetValueOrDefault(), fechaFin);
                        }
                        diasDif += 1;
                        fechaFinAsistenVacacion = plazasPorEmpleadosMovSiguiente.fecha.GetValueOrDefault();
                        fechaFinAsistenVacacion.AddDays(fechaFinAsistenVacacion.Day - 1);
                    }
                    seAplicoDiasDif = true;
                }
            }

            if (fechaFinalEjecutandose.CompareTo(fechaFin) < 0 || fechaBajaFiniq != null)
            {
                if (fechaBajaFiniq == null)
                {
                    diasDif = diasDif + cantidadDiasEntreDosFechas(fechaFinalEjecutandose, fechaFin);
                }
                else if (plazasPorEmpleadosMovSiguiente == null ? false : seAplicoDiasDif)
                {
                    //                    System.out.println("ekelele");
                }
                else if (fechaBajaFiniq.GetValueOrDefault().CompareTo(periodosNomina.fechaInicial) >= 0 & fechaBajaFiniq.GetValueOrDefault().CompareTo(periodosNomina.fechaFinal) <= 0)
                {
                    if (fechaBajaFiniq.GetValueOrDefault().CompareTo(periodosNomina.fechaInicial) != 0)
                    {//esto es cuando ambas fechas son la misma. si se hace ejecuta el metodo cantidadDiasEntreDosFechas me va a regresar 365 dias y esta mal.
                        if (fechaBajaFiniq.GetValueOrDefault().CompareTo(periodosNomina.fechaFinal) != 0)
                        {
                            diasDif = diasDif + cantidadDiasEntreDosFechas(fechaBajaFiniq.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault());
                        }
                    }
                    else if (manejaPagoIMSSDiasNaturales)
                    {
                        diasDif = diasDif + (int)valoresConceptosEmpleados["DiasNaturalesDelPeriodo".ToUpper()];
                    }
                    else
                    {
                        diasDif = diasDif + (int)valoresConceptosEmpleados["PeriodicidadEnDias".ToUpper()];
                    }
                }
                else //este es para cuando se calcule el finiquito desde la ventana de finiquitos por si se calcula en un 
                     //periodo distinto al del periodo que tiene la fecha de baja.
                {
                    if (manejaPagoIMSSDiasNaturales)
                    {
                        diasDif = diasDif + (int)valoresConceptosEmpleados["DiasNaturalesDelPeriodo".ToUpper()];
                    }
                    else
                    {
                        diasDif = diasDif + (int)valoresConceptosEmpleados["PeriodicidadEnDias".ToUpper()];
                    }
                }
            }
            cargarVariablesEmpleadoAsistencias(fechaIniAsistenVacacion, fechaFinAsistenVacacion, calculoUnidades, modificarDiasTrabajados, false);
            if (manejaPagoIMSSDiasNaturales)
            {
                valoresConceptosEmpleados["DiasCotizados".ToUpper()] = ((int)valoresConceptosEmpleados["DiasNaturalesDelPeriodo".ToUpper()] - (diasDif
                        + ((int)valoresConceptosEmpleados["DiasIncapacidadEmpleado".ToUpper()]
                        + (int)valoresConceptosEmpleados["Ausentismo".ToUpper()])));
            }
            else
            {
                valoresConceptosEmpleados["DiasCotizados".ToUpper()] = ((int)valoresConceptosEmpleados["PeriodicidadEnDias".ToUpper()] - (diasDif
                        + ((int)valoresConceptosEmpleados["DiasIncapacidadEmpleado".ToUpper()]
                        + (int)valoresConceptosEmpleados["Ausentismo".ToUpper()])));
            }
        }

        private void cargaVariablesGlobalesEmpleado(Empleados empleado)
        {
            try
            {
                valoresConceptosEmpleados["NumEmpleado".ToUpper()] = empleado.clave;
                valoresConceptosEmpleados["Estatus".ToUpper()] = empleado.status;
                valoresConceptosEmpleados["FechaNacimiento".ToUpper()] = empleado.fechaNacimiento.GetValueOrDefault();
                valoresConceptosEmpleados["Edad".ToUpper()] = calcularEdad(empleado.fechaNacimiento.GetValueOrDefault());
                valoresConceptosEmpleados["Cumple".ToUpper()] = empleado.fechaNacimiento.GetValueOrDefault().ToString("dd/MMM").ToUpper();
                valoresConceptosEmpleados["MesNacimiento".ToUpper()] = empleado.fechaNacimiento.GetValueOrDefault().Month;
                valoresConceptosEmpleados["DiaNacimiento".ToUpper()] = empleado.fechaNacimiento.GetValueOrDefault().Day;
            }
            catch (Exception ex)
            {
                mensajeResultado.noError = 21;
                mensajeResultado.error = String.Concat("Error cargar variables globales del empleado", " ", ex.GetBaseException().ToString());
            }
        }

        private void cargaValoresDiasPago(PlazasPorEmpleadosMov plazasPorEmpleadosMovEjecutandose, bool primeraPlazaPorEmpleadosMov, PlazasPorEmpleadosMov plazasPorEmpleadosMovSiguiente, CalculoUnidades calculoUnidades, bool inicia2doMes, bool? modificarDiasTrabajados)
        {
            int diasDif = 0;
            descontarDiasPago = 0.0;
            DateTime fechaIni = (DateTime)valoresConceptosEmpleados[parametroFechaInicial], fechaFin = (DateTime)valoresConceptosEmpleados[parametroFechaFinal],
                fechaIniAsistenVacacion = (DateTime)valoresConceptosEmpleados[parametroFechaInicial], fechaFinAsistenVacacion = (DateTime)valoresConceptosEmpleados[parametroFechaFinal];
            if (periodosNomina != null & !isMov2Meses)
            {
                fechaIni = periodosNomina.fechaInicial.GetValueOrDefault();
                fechaFin = periodosNomina.fechaFinal.GetValueOrDefault();
                fechaIniAsistenVacacion = periodosNomina.fechaInicial.GetValueOrDefault();
                fechaFinAsistenVacacion = periodosNomina.fechaFinal.GetValueOrDefault();
            }

            if (((DateTime)valoresConceptosEmpleados["FechaAlta".ToUpper()]).CompareTo(fechaIni) > 0)//Es mayor fechaAlta a fechaInicial
            {
                diasDif += cantidadDiasEntreDosFechas(fechaIni, (DateTime)valoresConceptosEmpleados["FechaAlta".ToUpper()]);
                fechaIniAsistenVacacion = (DateTime)valoresConceptosEmpleados["FechaAlta".ToUpper()];
            }
            else if (plazasPorEmpleadosMovEjecutandose.fechaInicial.GetValueOrDefault().CompareTo(fechaIni) > 0 & !primeraPlazaPorEmpleadosMov | plazasPorEmpleadosMovSiguiente != null)
            { //aqui huvo una promocion o modificacion. inicaPlazaPorEmpleadosMov es para saber si existen mas de una promocion o es unica
                if (plazasPorEmpleadosMovEjecutandose.fechaInicial.GetValueOrDefault().CompareTo(fechaIni) > 0 & !inicia2doMes)
                {
                    diasDif += cantidadDiasEntreDosFechas(fechaIni, plazasPorEmpleadosMovEjecutandose.fechaInicial.GetValueOrDefault());
                    fechaFinAsistenVacacion = plazasPorEmpleadosMovEjecutandose.fechaInicial.GetValueOrDefault();
                    fechaFinAsistenVacacion.AddDays(fechaFinAsistenVacacion.Day - 1);
                }
                if (plazasPorEmpleadosMovSiguiente != null)
                {
                    if (plazasPorEmpleadosMovEjecutandose.fechaInicial.GetValueOrDefault().CompareTo(fechaFin) == 0 | plazasPorEmpleadosMovSiguiente.fechaInicial.GetValueOrDefault().CompareTo(fechaFin) == 0)
                    {
                        diasDif += 1;
                        if (plazasPorEmpleadosMovEjecutandose.fechaInicial.GetValueOrDefault().CompareTo(fechaFin) == 0)
                        {
                            fechaFinAsistenVacacion = plazasPorEmpleadosMovEjecutandose.fechaInicial.GetValueOrDefault();
                        }
                        else
                        {
                            fechaFinAsistenVacacion = plazasPorEmpleadosMovSiguiente.fechaInicial.GetValueOrDefault();
                        }
                        fechaFinAsistenVacacion.AddDays(fechaFinAsistenVacacion.Day - 1);
                    }
                    else
                    {
                        diasDif += cantidadDiasEntreDosFechas(plazasPorEmpleadosMovSiguiente.fechaInicial.GetValueOrDefault(), fechaFin);
                        diasDif += 1;
                        fechaFinAsistenVacacion = plazasPorEmpleadosMovSiguiente.fechaInicial.GetValueOrDefault();
                        fechaFinAsistenVacacion.AddDays(fechaFinAsistenVacacion.Day - 1);
                    }
                }
            }
            if (plazasPorEmpleadosMovEjecutandose.plazasPorEmpleado.fechaFinal.GetValueOrDefault().CompareTo(fechaFin) < 0 || fechaBajaFiniq != null)
            {
                if (fechaBajaFiniq == null)
                {
                    diasDif = diasDif + cantidadDiasEntreDosFechas(plazasPorEmpleadosMovEjecutandose.plazasPorEmpleado.fechaFinal.GetValueOrDefault(), fechaFin);
                }
                else if (fechaBajaFiniq.GetValueOrDefault().CompareTo(periodosNomina.fechaInicial) >= 0 & fechaBajaFiniq.GetValueOrDefault().CompareTo(periodosNomina.fechaFinal) <= 0)
                {
                    if (fechaBajaFiniq.GetValueOrDefault().CompareTo(periodosNomina.fechaInicial) != 0)
                    {//esto es cuando ambas fechas son la misma. si se hace ejecuta el metodo cantidadDiasEntreDosFechas me va a regresar 365 dias y esta mal.
                        if (fechaBajaFiniq.GetValueOrDefault().CompareTo(periodosNomina.fechaFinal) != 0)
                        {
                            diasDif = diasDif + cantidadDiasEntreDosFechas(fechaBajaFiniq.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault());
                        }
                    }
                    else if (manejaPagoDiasNaturales)
                    {
                        diasDif = diasDif + (int)valoresConceptosEmpleados["DiasNaturalesDelPeriodo".ToUpper()];
                    }
                    else
                    {
                        diasDif = diasDif + (int)valoresConceptosEmpleados["PeriodicidadEnDias".ToUpper()];
                    }
                }
                else //este es para cuando se calcule el finiquito desde la ventana de finiquitos por si se calcula en un 
                     //periodo distinto al del periodo que tiene la fecha de baja.
                {
                    if (manejaPagoDiasNaturales)
                    {
                        diasDif = diasDif + (int)valoresConceptosEmpleados["DiasNaturalesDelPeriodo".ToUpper()];
                    }
                    else
                    {
                        diasDif = diasDif + (int)valoresConceptosEmpleados["PeriodicidadEnDias".ToUpper()];
                    }
                }
            }
            valoresConceptosEmpleados[parametroFechaInicial] = fechaIniAsistenVacacion;
            valoresConceptosEmpleados[parametroFechaFinal] = fechaFinAsistenVacacion;
            cargarVariablesEmpleadoAsistencias(fechaIniAsistenVacacion, fechaFinAsistenVacacion, calculoUnidades, modificarDiasTrabajados, false);
            if (mensajeResultado.noError != 0)
            {
                return;
            }
            cargarVariablesEmpleadoVacaciones(fechaIni, fechaFin, calculoUnidades, plazasPorEmpleadosMovEjecutandose, false);
            if (mensajeResultado.noError != 0)
            {
                return;
            }
            if (manejaPagoDiasNaturales)
            {
                valoresConceptosEmpleados["DiasNormalesAPagar".ToUpper()] = valoresConceptosEmpleados["DiasNaturalesDelPeriodo".ToUpper()];
                //int diasVacaciones = tipoCorrida == null ? (Integer) valoresConceptosEmpleados.get("diasVacaciones".ToUpper()) : tipoCorrida.getClave().equalsIgnoreCase("VAC") ? (Integer) valoresConceptosEmpleados.get("diasVacaciones".ToUpper()) : 0;
                int diasVacaciones = (int)valoresConceptosEmpleados["diasVacaciones".ToUpper()];
                valoresConceptosEmpleados["DiasPago".ToUpper()] =
                        ((int)valoresConceptosEmpleados["DiasNormalesAPagar".ToUpper()]
                        - (diasDif
                        + ((int)valoresConceptosEmpleados["DiasIncapacidadEmpleado".ToUpper()]
                        + (double)valoresConceptosEmpleados["Faltas".ToUpper()]
                        + (int)valoresConceptosEmpleados["Ausentismo".ToUpper()]
                        + diasVacaciones))) + pagarPrimero3Dias;
                descontarDiasPago = (/*diasDif
                     + */((int)valoresConceptosEmpleados["DiasIncapacidadEmpleado".ToUpper()]
                        + (double)valoresConceptosEmpleados["Faltas".ToUpper()]//Activo suma inactivo no sum
                        + (int)valoresConceptosEmpleados["Ausentismo".ToUpper()]
                        + diasVacaciones)) - pagarPrimero3Dias;
            }
            else
            {
                valoresConceptosEmpleados["DiasNormalesAPagar".ToUpper()] = valoresConceptosEmpleados["PeriodicidadEnDias".ToUpper()];
                ///int diasVacaciones = tipoCorrida == null ? (Integer) valoresConceptosEmpleados.get("diasVacaciones".ToUpper()) : tipoCorrida.getClave().equalsIgnoreCase("VAC") ? (Integer) valoresConceptosEmpleados.get("diasVacaciones".ToUpper()) : 0;
                int diasVacaciones = (int)valoresConceptosEmpleados["diasVacaciones".ToUpper()];
                valoresConceptosEmpleados["DiasPago".ToUpper()] =
                        ((int)valoresConceptosEmpleados["DiasNormalesAPagar".ToUpper()]
                        - (diasDif
                        + ((int)valoresConceptosEmpleados["DiasIncapacidadEmpleado".ToUpper()]
                        + (double)valoresConceptosEmpleados["Faltas".ToUpper()]
                        + (int)valoresConceptosEmpleados["Ausentismo".ToUpper()]
                        + diasVacaciones))) + pagarPrimero3Dias;
                descontarDiasPago = (/*diasDif
                     + */((int)valoresConceptosEmpleados["DiasIncapacidadEmpleado".ToUpper()]
                        + (double)valoresConceptosEmpleados["Faltas".ToUpper()]
                        + (int)valoresConceptosEmpleados["Ausentismo".ToUpper()] + diasVacaciones)) - pagarPrimero3Dias;
            }
            if (calculoUnidades != null)
            {
                calculoUnidades.diasTrabajados = (double)valoresConceptosEmpleados["DiasPago".ToUpper()];
            }
        }

        private void cargarVariablesEmpleadoAsistencias(DateTime fechaInicial, DateTime fechaFinal, CalculoUnidades calculoUnidades, bool? modificarDiasTrabajados, bool acumularAsis)
        {
            int diasAusencias = 0, diasIncapacidadEnfermedad = 0,
                    diasIncapacidadAccidente = 0, diasIncapacidadMaternidad = 0, diasOtrasIncapacidad = 0,
                    festivo = 0, descanso = 0, laborados = 0;
            Double hrsExtraDoble = 0.0, hrsExtraTriple = 0.0, retardos = 0.0, permisoSinSueldo = 0.0, permisoConSueldo = 0.0,
                    descansoLaborado = 0.0, festivoLaborado = 0.0, domingoLaborado = 0.0, diasRetardos = 0.0, diasFaltas = 0.0;
            List<Asistencias> listAsistencias = obtenerAsistencias(fechaInicial, fechaFinal);
            int x;
            //        System.out.println("************************* Fecha Inicial " + fechaInicial + "   Fecha Final " + fechaFinal);
            List<Asistencias> listAsistenciasIncapacidadEnfermedad = new List<Asistencias>();
            for (x = 0; x < listAsistencias.Count(); x++)
            {
                switch (Convert.ToInt32(listAsistencias[x].excepciones.clave))
                {
                    case 0://Laborado = "0"
                        laborados++;
                        break;
                    case 1://Retardo = "1";
                        retardos += listAsistencias[x].cantidad.GetValueOrDefault();
                        diasRetardos++;
                        break;
                    case 2://Falta = "2";
                        if (listAsistencias[x].cantidad == null)
                        {
                            diasFaltas++;
                        }
                        else if (listAsistencias[x].cantidad.GetValueOrDefault() == 0.50)
                        {
                            diasFaltas = diasFaltas + 0.5;
                        }
                        else
                        {
                            diasFaltas++;
                        }
                        break;
                    case 3://Ausentismo = "3";
                        diasAusencias++;
                        break;
                    case 4://PermisoConSueldo = "4";
                        permisoConSueldo++;
                        break;
                    case 5://PermisoSinSueldo = "5";
                        permisoSinSueldo++;
                        break;
                    case 6://IncapacidadPorEnfermedad = "6";
                        listAsistenciasIncapacidadEnfermedad.Add(listAsistencias[x]);
                        diasIncapacidadEnfermedad++;
                        break;
                    case 7://IncapacidadPorAccidente = "7";
                        diasIncapacidadAccidente++;
                        break;
                    case 8://IncapacidadPorMaternidad = "8";
                        diasIncapacidadMaternidad++;
                        break;
                    case 9://OtrasIncapacidades = "9";
                        diasOtrasIncapacidad++;
                        break;
                    case 10://DescansoLaborado = "10";
                        descansoLaborado += listAsistencias[x].cantidad.GetValueOrDefault();
                        break;
                    case 11://FestivoLaborado = "11";
                        if (listAsistencias[x].excepciones.tipoDatoExcepcion == Convert.ToInt32(TipoDatoExcepcion.SINDATO))
                        {
                            festivoLaborado++;
                        }
                        else
                        {
                            festivoLaborado += listAsistencias[x].cantidad.GetValueOrDefault();
                        }
                        break;
                    case 12://DomingoLaborado = "12";
                        domingoLaborado += listAsistencias[x].cantidad.GetValueOrDefault();
                        break;
                    case 13://TiempoExtra = "13";
                        break;
                    case 14://ExtraDoble = "14";
                        hrsExtraDoble += listAsistencias[x].cantidad.GetValueOrDefault();
                        break;
                    case 15://ExtraTriple = "15";
                        hrsExtraTriple += listAsistencias[x].cantidad.GetValueOrDefault();
                        break;
                    case 16://Festivo = "16";
                        festivo++;
                        break;
                    case 17://Descanso = "17";
                        descanso++;
                        break;

                }
            }
            if (listAsistencias.Count == 0)
            {
                if (((DateTime)valoresConceptosEmpleados[parametroFechaInicial]).Date.CompareTo(((DateTime)valoresConceptosEmpleados[parametroFechaFinal]).Date) == 0)
                {
                    laborados = 1;
                }
                else
                {
                    laborados = cantidadDiasEntreDosFechas(((DateTime)valoresConceptosEmpleados[parametroFechaInicial]).Date, ((DateTime)valoresConceptosEmpleados[parametroFechaFinal]).Date) + 1;
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
            if (listAsistenciasIncapacidadEnfermedad.Any())
            {
                listAsistenciasIncapacidadEnfermedad = (from list in listAsistenciasIncapacidadEnfermedad orderby list.fecha select list).ToList();
                List<RegistroIncapacidad> listRegistroIncapacidad = obtenerIncapacidadesPorEnfermedad(listAsistenciasIncapacidadEnfermedad[0].fecha.GetValueOrDefault(), listAsistenciasIncapacidadEnfermedad[listAsistenciasIncapacidadEnfermedad.Count() - 1].fecha.GetValueOrDefault());

                for (int i = 0; i < listRegistroIncapacidad.Count(); i++)
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
                            if ((fechaIncap.CompareTo(periodosNomina.fechaInicial) > 0 || fechaIncap.CompareTo(periodosNomina.fechaInicial) == 0)
                                    & (fechaIncap.CompareTo(periodosNomina.fechaFinal) == 0 || fechaIncap.CompareTo(periodosNomina.fechaFinal) < 0))
                            {
                                if (diasAPagar > 0)
                                {
                                    pagarPrimero3Dias++;
                                }
                                diasAPagar--;
                                diasApagarIMSS--;
                            }
                            else if (fechaIncap.CompareTo(periodosNomina.fechaInicial) < 0)
                            {
                                diasAPagar--;
                                diasApagarIMSS--;
                            }
                            else if (fechaIncap.CompareTo(periodosNomina.fechaFinal) > 0)
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
                    calculoUnidades.diasIncapacidadEnfermedad = diasIncapacidadEnfermedad;
                    calculoUnidades.diasIncapacidadAccidente = diasIncapacidadAccidente;
                    calculoUnidades.diasIncapacidadMaternidad = diasIncapacidadMaternidad;
                    calculoUnidades.diasOtrasIncapacidades = diasOtrasIncapacidad;
                    calculoUnidades.diasFalta = diasFaltas;
                    if (modificarDiasTrabajados == null ? true : !modificarDiasTrabajados.GetValueOrDefault())
                    {
                        calculoUnidades.diasAusentismo = diasAusencias;
                    }
                    calculoUnidades.diasDescansoLaborado = descansoLaborado;
                    calculoUnidades.diasFestivoLaborado = festivoLaborado;
                    calculoUnidades.diasDomingoLaborado = domingoLaborado;
                    calculoUnidades.diasRetardo = retardos;
                    calculoUnidades.diasPermisoConSueldo = permisoConSueldo;
                    calculoUnidades.diasPermisoSinSueldo = permisoSinSueldo;
                    calculoUnidades.diasFestivo = festivo;
                    calculoUnidades.diasDescanso = descanso;
                }
            }

        }

        private void cargaDatosSalarioDiario(PlazasPorEmpleadosMov plazasPorEmpleadosMov, PeriodosNomina periodoNomina)
        {
            //if (plazasPorEmpleadosMov.salarioPor == 2)
            //{
            valoresConceptosEmpleados["SUELDODIARIO"] = plazasPorEmpleadosMov.importe;
            //}
            //else
            //{
            //    valoresConceptosEmpleados.Add("SUELDODIARIO", plazasPorEmpleadosMov.puestos.salarioTabular);
            //}
            if (plazasPorEmpleadosMov.puestos != null)
            {
                if (plazasPorEmpleadosMov.puestos.categoriasPuestos != null)
                {
                    if (plazasPorEmpleadosMov.puestos.categoriasPuestos.pagarPorHoras)
                    {
                        double sueldo = (double)valoresConceptosEmpleados["SUELDODIARIO"];
                        sueldo = (sueldo * plazasPorEmpleadosMov.horas.GetValueOrDefault()) / periodosNomina.diasPago.GetValueOrDefault();
                        valoresConceptosEmpleados.Add("SUELDODIARIO", sueldo);
                    }
                }
            }
            valoresConceptosEmpleados["SUELDODIARIOFINAL"] = (double)valoresConceptosEmpleados["SUELDODIARIO"];
            valoresConceptosEmpleados["percep_plaza".ToUpper()] = (double)valoresConceptosEmpleados["SUELDODIARIO"];
            valoresConceptosEmpleados["SUELDODIARIOINICIAL"] = (double)valoresConceptosEmpleados["SUELDODIARIO"];
            valoresConceptosEmpleados["SUELDODIARIOVIGENTE"] = (double)valoresConceptosEmpleados["SUELDODIARIO"];
            valoresConceptosEmpleados["percep_plaza_vigente".ToUpper()] = (double)valoresConceptosEmpleados["SUELDODIARIO"];
            if (periodoNomina == null)
            {
                valoresConceptosEmpleados.Add("SUELDODIARIOFINAL".ToUpper(), 0.0);
            }
            else
            {
                strQuery = new StringBuilder("SELECT pm.importe FROM ").Append(typeof(PlazasPorEmpleadosMov).Name).Append(" pm ");
                strQuery.Append("WHERE pm.plazasPorEmpleado.empleados.clave=:claveEmpleado ");
                strQuery.Append("and pm.plazasPorEmpleado.razonesSociales.clave=:claveRazonSocial ");
                strQuery.Append("and pm.cambioSalario=1  and pm.fechaInicial BETWEEN :fechaInicial and :fechaFinal ");
                strQuery.Append("order by pm.fechaInicial desc");
                Object salarioFinal = ejecutaQueryUnico(strQuery.ToString(),
                   new String[] { "claveEmpleado", "claveRazonSocial", "fechaInicial", "fechaFinal" },
                   new Object[]{plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave,
                        plazasPorEmpleadosMov.plazasPorEmpleado.razonesSociales.clave,
                        periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault()});
                if (salarioFinal != null)
                {
                    if (salarioFinal.GetType() == typeof(double))
                    {
                        double valor = (double)salarioFinal;
                        valoresConceptosEmpleados["SUELDODIARIOFINAL".ToUpper()] = valor;
                    }
                    else
                    {
                        valoresConceptosEmpleados["SUELDODIARIOFINAL".ToUpper()] = salarioFinal;
                    }
                }
            }

            strQuery = new StringBuilder("SELECT pm.importe FROM ").Append(typeof(PlazasPorEmpleadosMov).Name).Append(" pm ");
            strQuery.Append("WHERE pm.plazasPorEmpleado.empleados.clave=:claveEmpleado ");
            strQuery.Append("and pm.plazasPorEmpleado.razonesSociales.clave=:claveRazonSocial ");
            strQuery.Append("and pm.plazasPorEmpleado.plazaPrincipal=1 and pm.fechaInicial < :fechaMov ");
            strQuery.Append("order by pm.fechaInicial desc");

            double? sueldoAnterior = (double?)ejecutaQueryUnico(strQuery.ToString(),
                    new String[] { "claveEmpleado", "claveRazonSocial", "fechaMov" },
                    new Object[]{plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave,
                    plazasPorEmpleadosMov.plazasPorEmpleado.razonesSociales.clave,
                    plazasPorEmpleadosMov.fechaInicial.GetValueOrDefault()});

            if (sueldoAnterior != null)
            {
                valoresConceptosEmpleados["SueldoAnterior".ToUpper()] = sueldoAnterior;
            }

        }

        private void inicializaPeriodo2Meses(PeriodosNomina periodo, DateTime fechaInicial, DateTime fechaFinal)
        {
            valoresConceptosGlobales["IdPeriodo".ToUpper()] = periodo == null ? 0L : periodo.id;
            valoresConceptosGlobales["NumPeriodo".ToUpper()] = periodo.clave;
            valoresConceptosGlobales["FechaInicioPeriodo".ToUpper()] = fechaInicial;
            valoresConceptosGlobales["FechaFinalPeriodo".ToUpper()] = fechaFinal;
            valoresConceptosGlobales["FechaPago".ToUpper()] = periodo.fechaPago;
            valoresConceptosGlobales["NumMesAfectar".ToUpper()] = periodosNomina.acumularAMes.GetValueOrDefault().Month;
            valoresConceptosGlobales["DiasNaturalesDelPeriodo".ToUpper()] = (cantidadDiasEntreDosFechas(fechaInicial, fechaFinal) + 1);
            valoresConceptosGlobales["PeriodicidadEnDias".ToUpper()] = periodo.tipoNomina.periodicidad.dias;
            valoresConceptosGlobales["AnioPeriodo".ToUpper()] = periodo == null ? periodosNomina.acumularAMes.GetValueOrDefault().Year : periodo.año;
            valoresConceptosGlobales["TipoPeriodicidadNumerico".ToUpper()] = periodo == null ? "" : periodo.tipoNomina.periodicidad.clave;
            valoresConceptosGlobales["AnioActualNumerico".ToUpper()] = periodo == null ? 0 : periodo.año;

            DateTime a = periodo.fechaAsistenciInicial.GetValueOrDefault();
            int mes = a.Month;
            String mesNomLar = a.ToString("MMMM");
            String mesNomCor = a.ToString("MMM");
            valoresConceptosGlobales["DiasMesNumerico".ToUpper()] = mes;
            valoresConceptosGlobales["MesAlfanumCompleto".ToUpper()] = mesNomLar;
            valoresConceptosGlobales["MesAlfanumCorto".ToUpper()] = mesNomCor;
        }

        #endregion
        private List<CreditoAhorro> obtenerCreditosAhorro(RazonesSociales razonesSociales)
        {
            IList<CreditoAhorro> listCreditoPorEmpleado = null;
            try
            {
                //camposParametro = new List<String>(0);
                //valoresParametro = new List<Object>(0);
                //strQuery.Remove(0, strQuery.Length).Append("from CreditoAhorro c ");
                //strWhere.Remove(0, strWhere.Length).Append(" WHERE ");
                //strWhere.Append(" c.concepNomDefi_concepNomiDefin_ID is not null and c.razonesSociales.clave= :claveRazonesSociales");
                //strQuery.Append(strWhere);
                //IQuery q = getSession().CreateQuery(strQuery.ToString());
                //q.SetParameter("claveRazonesSociales", razonesSociales.clave);
                //listCreditoPorEmpleado = q.Enumerable<CreditoAhorro>().ToList();
                listCreditoPorEmpleado= getSession().CreateCriteria("CreditoAhorro").List<CreditoAhorro>();
            }
            catch (Exception ex)
            {
                mensajeResultado.error = (ex.GetBaseException().Message);
                mensajeResultado.noError = (27);
            }
            return (List<CreditoAhorro>)listCreditoPorEmpleado;
        }

        private void consultarConfiguracionAgui()
        {
            try
            {
                strQuery.Remove(0, strQuery.Length);
                strQuery.Append("from DiasAguinaldo d where d.razonesSociales.clave=:claveRazonsocial");
                IQuery q = getSession().CreateQuery(strQuery.ToString());
                q.SetParameter("claveRazonsocial", valoresConceptosGlobales["RazonSocial".ToUpper()].ToString());
                diasAguinaldo = q.Enumerable<DiasAguinaldo>().ToList();
                //diasAguinaldo = (List<DiasAguinaldo>)q.List<DiasAguinaldo>();
                if (diasAguinaldo == null)
                {
                    diasAguinaldo = new List<DiasAguinaldo>();
                }

                strQuery.Remove(0, strQuery.Length);
                strQuery.Append("from AguinaldoConfiguracion d where d.razonesSociales.clave=:claveRazonsocial");
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                camposParametro.Add("claveRazonsocial");
                valoresParametro.Add(valoresConceptosGlobales["RazonSocial".ToUpper()]);
                aguiConfiguracion = (AguinaldoConfiguracion)ejecutaQueryUnico(strQuery.ToString(), camposParametro.ToArray<string>(), valoresParametro.ToArray());

                strQuery.Remove(0, strQuery.Length);
                strQuery.Append("from AguinaldoFechas d where d.razonesSociales.clave=:claveRazonsocial");


                q = getSession().CreateQuery(strQuery.ToString());
                q.SetParameter("claveRazonsocial", valoresConceptosGlobales["RazonSocial".ToUpper()].ToString());
                aguiFechas = q.Enumerable<AguinaldoFechas>().ToList();
                // aguiFechas = (List<AguinaldoFechas>)q.List<AguinaldoFechas>();
                if (aguiFechas == null)
                {
                    aguiFechas = new List<AguinaldoFechas>();
                }

                if (aguiFechas.Any())
                {
                    PeriodosNomina peraxu = null;
                    for (int i = 0; i < aguiFechas.Count(); i++)
                    {
                        strQuery.Remove(0, strQuery.Length);
                        strQuery.Append("FROM PeriodosNomina p ");
                        strQuery.Append("where p.tipoCorrida.clave=:claveCorrida and p.tipoNomina.clave=:claveNomina ");
                        strQuery.Append("and (:fecha BETWEEN p.fechaInicial AND p.fechaFinal + 1)");
                        camposParametro = new List<String>(0);
                        valoresParametro = new List<Object>(0);
                        camposParametro.Add("claveCorrida");
                        camposParametro.Add("claveNomina");
                        camposParametro.Add("fecha");
                        valoresParametro.Add(valoresConceptosGlobales["ClaveTipoCorrida".ToUpper()]);
                        valoresParametro.Add(valoresConceptosGlobales["TipoNomina".ToUpper()]);
                        valoresParametro.Add(aguiFechas[i].fechaProgramada);
                        PeriodosNomina per = (PeriodosNomina)ejecutaQueryUnico(strQuery.ToString(), camposParametro.ToArray<string>(), valoresParametro.ToArray());
                        if (peraxu != null)
                        {
                            if (peraxu != per)
                            {
                                totalPagosAgui++;
                            }

                        }
                        else
                        {
                            peraxu = per;
                            totalPagosAgui++;
                        }
                    }

                }

            }
            catch (Exception ex)
            {

                mensajeResultado.error = (ex.GetBaseException().Message);
                mensajeResultado.noError = (27);
            }
        }

        private List<PlazasPorEmpleadosMov> obtenerPlazasPorEmpleados(String claveEmpIni, String claveEmpFin, String claveTipoNomina, String clavePuesto,
           String claveCategoriasPuestos, String claveTurno, String claveRazonSocial, String claveRegPatronal, String claveDepto,
           String claveCtrCosto, int? tipoSalario, String tipoContrato, bool? status, String claveTipoCorrida, String claveFormaPago, DateTime fechaInicioPeriodo, DateTime fechaFinPeriodo, Boolean todos)
        {
            IList<PlazasPorEmpleadosMov> filtroPlazasPorEmpleadosMov = null, filtroPlazasPorEmpleadosMovTmp;
            try
            {
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);

                strQuery.Remove(0, strQuery.Length).Append("SELECT pMov ");
                strQuery.Append("from PlazasPorEmpleadosMov pMov inner join pMov.plazasPorEmpleado pemp  where pMov.id IN ");
                strQuery.Append(" (Select MAX(pMovX.id) from PlazasPorEmpleadosMov pMovX ");
                strQuery.Append("INNER JOIN pMovX.plazasPorEmpleado pe ");
                strQuery.Append("INNER JOIN pe.empleados em ");
                strWhere.Remove(0, strWhere.Length).Append(" WHERE ");

                claveRazonSocial = (claveRazonSocial == null ? "" : claveRazonSocial);
                if (claveRazonSocial.Any())
                {
                    strQuery.Append(" INNER JOIN pe.razonesSociales rs ");
                    strWhere.Append(" rs.clave = :claveRazonSocial ");
                    camposParametro.Add("claveRazonSocial");
                    valoresParametro.Add(claveRazonSocial);
                }

                claveTurno = (claveTurno == null ? "" : claveTurno);
                if (claveTurno.Any())
                {
                    strQuery.Append(" LEFT OUTER JOIN pMovX.turnos tu ");
                    strWhere.Append(" AND tu.clave = :claveTurno ");
                    camposParametro.Add("claveTurno");
                    valoresParametro.Add(claveTurno);
                }

                claveTipoNomina = (claveTipoNomina == null ? "" : claveTipoNomina);
                if (claveTipoNomina.Any())
                {
                    strQuery.Append(" LEFT OUTER JOIN pMovX.tipoNomina t ");
                    strWhere.Append(" AND t.clave = :claveTipoNomina ");
                    camposParametro.Add("claveTipoNomina");
                    valoresParametro.Add(claveTipoNomina);
                }

                claveRegPatronal = (claveRegPatronal == null ? "" : claveRegPatronal);
                if (claveRegPatronal.Any())
                {
                    strQuery.Append(" LEFT OUTER JOIN pe.registroPatronal rp ");
                    strWhere.Append(" AND rp.clave = :claveRegPatronal ");
                    camposParametro.Add("claveRegPatronal");
                    valoresParametro.Add(claveRegPatronal);
                }

                claveDepto = (claveDepto == null ? "" : claveDepto);
                if (claveDepto.Any())
                {
                    strQuery.Append(" LEFT OUTER JOIN pMovX.departamentos dp ");
                    strWhere.Append(" AND dp.clave = :claveDepto ");
                    camposParametro.Add("claveDepto");
                    valoresParametro.Add(claveDepto);
                }

                claveCtrCosto = (claveCtrCosto == null ? "" : claveCtrCosto);
                if (claveCtrCosto.Any())
                {
                    strQuery.Append(" LEFT OUTER JOIN pMovX.centroDeCosto cc ");
                    strWhere.Append(" AND cc.clave = :claveCtrCosto ");
                    camposParametro.Add("claveCtrCosto");
                    valoresParametro.Add(claveCtrCosto);
                }

                clavePuesto = (clavePuesto == null ? "" : clavePuesto);
                if (clavePuesto.Any())
                {
                    strQuery.Append(" LEFT OUTER JOIN pMovX.puestos pu ");
                    strWhere.Append(" AND pu.clave = :clavePuesto ");
                    camposParametro.Add("clavePuesto");
                    valoresParametro.Add(clavePuesto);
                }

                claveCategoriasPuestos = (claveCategoriasPuestos == null ? "" : claveCategoriasPuestos);
                if (claveCategoriasPuestos.Any())
                {
                    if (!clavePuesto.Any())
                    {
                        strQuery.Append(" LEFT OUTER JOIN pMovX.puestos pu ");
                    }
                    strQuery.Append(" LEFT OUTER JOIN pu.categoriasPuestos cp ");
                    strWhere.Append(" AND cp.clave = :claveCategoriasPuestos ");
                    camposParametro.Add("claveCategoriasPuestos");
                    valoresParametro.Add(claveCategoriasPuestos);
                }

                claveFormaPago = (claveFormaPago == null ? "" : claveFormaPago);
                if (claveFormaPago.Any())
                {
                    strQuery.Append(" LEFT OUTER JOIN pMovX.formasDePago fp ");
                    strWhere.Append(" AND fp.clave = :claveFormaPago ");
                    camposParametro.Add("claveFormaPago");
                    valoresParametro.Add(claveFormaPago);
                }

                if (tipoContrato.Any())
                {
                    //if (tipoContrato != -1) {
                    strQuery.Append(" LEFT OUTER JOIN pMovX.tipoContrato tc ");
                    strWhere.Append(" AND tc.clave = :tipoContrato ");
                    camposParametro.Add("tipoContrato");
                    valoresParametro.Add(tipoContrato);
                    //}
                }

                if (status != null)
                {
                    strWhere.Append(" AND em.status = :status ");
                    camposParametro.Add("status");
                    valoresParametro.Add(status);
                }

                if (tipoSalario != null)
                {
                    strQuery.Append(",SalariosIntegrados si ");
                }

                if (tipoSalario != null)
                {  ///pendiente modifcar cambio a tabla salario diario integrado
                    strWhere.Append("AND  si.fecha = (SELECT MAX (s.fecha) FROM SalariosIntegrados s WHERE s.fecha <= :fechaActual AND s.empleados.id = si.empleados.id AND s.empleados.id = pe.empleados.id) ");
                    strWhere.Append("AND si.tipoDeSalario = :tipoSalario ");
                    camposParametro.Add("tipoSalario");
                    valoresParametro.Add(tipoSalario);
                    camposParametro.Add("fechaActual");
                    if (string.Equals(claveTipoCorrida, "FIN", StringComparison.OrdinalIgnoreCase))
                    {
                        valoresParametro.Add(fechaBajaFiniq);
                    }
                    else
                    {
                        valoresParametro.Add(fechaFinPeriodo);
                    }
                }


                strWhere.Append(" AND ((pMovX.fechaInicial <= :fechaInicialPeriodo ) OR (pMovX.fechaInicial between :fechaInicialPeriodo AND :fechaFinalPeriodo ))  ");
                strWhere.Append(" AND ((pMovX.plazasPorEmpleado.fechaFinal >= :fechaFinalPeriodo ) OR   (pMovX.plazasPorEmpleado.fechaFinal between :fechaInicialPeriodo AND :fechaFinalPeriodo )) ");
                camposParametro.Add("fechaInicialPeriodo");
                if (string.Equals(claveTipoCorrida, "FIN", StringComparison.OrdinalIgnoreCase))
                {
                    valoresParametro.Add(fechaBajaFiniq);
                }
                else
                {
                    valoresParametro.Add(fechaInicioPeriodo);
                }
                camposParametro.Add("fechaFinalPeriodo");
                valoresParametro.Add(fechaFinPeriodo);

                if (claveEmpIni.Length > 0 & claveEmpFin.Length > 0)
                {
                    strWhere.Append(" AND (em.clave BETWEEN :claveEmpIni AND :claveEmpFin) ");
                    camposParametro.Add("claveEmpIni");
                    valoresParametro.Add(claveEmpIni);
                    camposParametro.Add("claveEmpFin");
                    valoresParametro.Add(claveEmpFin);
                }
                else if (claveEmpIni.Length > 0)
                {
                    strWhere.Append(" AND em.clave >= :claveEmpIni ");
                    camposParametro.Add("claveEmpIni");
                    valoresParametro.Add(claveEmpIni);
                }
                else if (claveEmpFin.Length > 0)
                {
                    strWhere.Append(" AND em.clave <= :claveEmpFin");
                    camposParametro.Add("claveEmpFin");
                    valoresParametro.Add(claveEmpFin);
                }
                strWhere.Append(" GROUP BY pe.referencia) ");
                if (claveRazonSocial.Any())
                {
                    strWhere.Append(" AND pemp.id not in (Select px.plazasPorEmpleado_reIngreso_ID.id from PlazasPorEmpleado px Where px.razonesSociales.clave = :claveRazonSocial AND px.plazasPorEmpleado_reIngreso_ID != null ) ");
                }
                else
                {
                    strWhere.Append(" AND pemp.id not in (Select px.plazasPorEmpleado_reIngreso_ID.id from PlazasPorEmpleado px Where px.plazasPorEmpleado_reIngreso_ID != null ) ");
                }
                if (!isCalculoSDI)
                {
                    strWhere.Append(" AND pemp.empleados.clave not in ( ");
                    claveTipoNomina = (claveTipoNomina == null ? "" : claveTipoNomina);
                    if (claveTipoNomina.Any())
                    {
                        strWhere.Append("select o.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave from CFDIEmpleado o where o.razonesSociales.clave = :claveRazonSocial and o.tipoNomina.clave = :claveTipoNomina ");
                    }
                    else
                    {
                        strWhere.Append("select o.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave from CFDIEmpleado o where o.razonesSociales.clave = :claveRazonSocial ");

                    }
                    if (claveTipoCorrida.Any())
                    {
                        strWhere.Append(" and o.tipoCorrida.clave = :claveTipoCorrida ");
                        camposParametro.Add("claveTipoCorrida");
                        valoresParametro.Add(claveTipoCorrida);
                    }
                    if (periodosNomina != null)
                    {
                        strWhere.Append(" and o.periodosNomina.id = :idPeriodoNomina ");
                        camposParametro.Add("idPeriodoNomina");
                        valoresParametro.Add(periodosNomina.id);
                    }

                    if (claveEmpIni.Length > 0 & claveEmpFin.Length > 0)
                    {
                        strWhere.Append(" AND (o.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave BETWEEN :claveEmpIni AND :claveEmpFin) ");
                    }
                    else if (claveEmpIni.Length > 0)
                    {
                        strWhere.Append(" AND o.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave >= :claveEmpIni ");
                    }
                    else if (claveEmpFin.Length > 0)
                    {
                        strWhere.Append(" AND o.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave <= :claveEmpFin");
                    }

                    strWhere.Append(" AND o.cfdiRecibo.statusTimbrado = :statusTimbre ");
                    camposParametro.Add("statusTimbre");
                    valoresParametro.Add(StatusTimbrado.TIMBRADO);

                    strWhere.Append(" ) ");
                }
                if (string.Equals(claveTipoCorrida, "FIN", StringComparison.OrdinalIgnoreCase))
                {
                    strWhere.Append(" AND pemp.referencia IN (SELECT flp.plazasPorEmpleado.referencia FROM FiniqLiquidPlazas flp WHERE pemp.referencia = flp.plazasPorEmpleado.referencia ");
                    strWhere.Append(" AND flp.incluir = true) ");
                }
                if (!todos)
                {
                    strWhere.Append(" AND pemp.plazaPrincipal = true");
                }
                // strWhere.append(" AND pemp.plazaPrincipal = true");
                strWhere.Append(" ORDER BY pemp.empleados.clave, pemp.referencia");
                strQuery.Append(strWhere);


                IQuery query = getSession().CreateQuery(strQuery.ToString());
                for (int i = 0; i < valoresParametro.Count; i++)
                {
                    if (valoresParametro[i].GetType() == typeof(object[]))
                    {
                        query.SetParameterList(camposParametro[i], (Object[])valoresParametro[i]);
                    }
                    else if (valoresParametro[i].GetType() == typeof(List<>))
                    {
                        query.SetParameterList(camposParametro[i], ((List<object>)valoresParametro[i]).ToArray());
                    }
                    else if (valoresParametro[i].GetType() == typeof(DateTime))
                    {
                        DateTime dateTime = (DateTime)valoresParametro[i];
                        query.SetParameter(camposParametro[i], dateTime.Date);
                    }
                    else
                    {
                        query.SetParameter(camposParametro[i], valoresParametro[i]);
                    }
                }

                //   filtroPlazasPorEmpleadosMov = query.SetResultTransformer(Transformers.AliasToBeanConstructor()).List<PlazasPorEmpleadosMov>();
                filtroPlazasPorEmpleadosMov = query.Enumerable<PlazasPorEmpleadosMov>().ToList();
                // filtroPlazasPorEmpleadosMov = query.List<PlazasPorEmpleadosMov>();


            }
            catch (Exception ex)
            {

                mensajeResultado.error = (ex.GetBaseException().Message);
                mensajeResultado.noError = (27);
            }

            return (List<PlazasPorEmpleadosMov>)filtroPlazasPorEmpleadosMov;
        }

        public Mensaje agregarVacacionesAuto(RazonesSociales razonSocial, PlazasPorEmpleadosMov plaEmp, PeriodosNomina periodoNomina, object[,] tablaFactorIntegracion, PagarPrimaVacionalyVacacionesAuto pagarVacaAuto)
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

                if (vacDev.devengadaActual == null || vacDev.devengadaActual.Count == 0)
                {
                    //mensajeResultado = vacDev.calcularVacacionesDevengadasEmpleados(razonSocial,)
                    // mensajeResultado = calcularVacacionesDevengadasEmpleados(razonSocial, tablaFactorIntegracion, dbContextSimple, null);
                    if (mensajeResultado.noError != 0)
                    {
                        return mensajeResultado;
                    }
                }

                if (vacDev.devengadaActual.Count > 0)
                {
                    vacacionesDeven = vacDev.devengadaActual[plaEmp.plazasPorEmpleado.empleados.clave];
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
                                vacDis.periodosNomina_periodoAplicacion_ID = periodoNomina;
                                vacDis.periodosNomina_periodoPago_ID = periodoNomina;
                                vacDis.razonesSociales = razonSocial;
                                vacDis.registroInicial = false;
                                vacDis.regresoVac = calfechaActual;
                                vacDis.salidaVacac = calfechaActual;
                                vacDis.statusVacaciones = (int)StatusVacaciones.PORCALCULAR;
                                vacDis.tipoNominaAplicacion = periodoNomina.tipoNomina;
                                vacDis.tipoCorridaAplicacion = periodoNomina.tipoCorrida;
                                //dbContextSimple.Set<VacacionesDisfrutadas>().Add(vacDis);
                                //dbContextSimple.SaveChanges();

                                vacDisconId = vacDis;
                                VacAplic.diasPrima = vacacionesDeven.diasPrimaVaca;
                                VacAplic.diasVac = 0;
                                VacAplic.vacacionesDevengadas = vacacionesDeven;
                                VacAplic.vacacionesDisfrutadas = vacDisconId;
                                //dbContextSimple.Set<VacacionesAplicacion>().AddOrUpdate(VacAplic);
                                //dbContextSimple.SaveChanges();
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
                                vacDis.periodosNomina_periodoAplicacion_ID = periodoNomina;
                                vacDis.periodosNomina_periodoPago_ID = periodoNomina;
                                vacDis.razonesSociales = razonSocial;
                                vacDis.registroInicial = false;
                                vacDis.regresoVac = calfechaActual;
                                vacDis.salidaVacac = calfechaActual;
                                vacDis.statusVacaciones = (int)StatusVacaciones.PORCALCULAR;
                                vacDis.tipoNominaAplicacion = periodoNomina.tipoNomina;
                                //dbContextSimple.Set<VacacionesDisfrutadas>().Add(vacDis);
                                //dbContextSimple.SaveChanges();

                                vacDisconId = vacDis;
                                VacAplic.diasPrima = vacacionesDeven.diasPrimaVaca;
                                VacAplic.diasVac = vacacionesDeven.diasVacaciones;
                                VacAplic.vacacionesDevengadas = vacacionesDeven;
                                VacAplic.vacacionesDisfrutadas = vacDisconId;
                                //dbContextSimple.Set<VacacionesAplicacion>().AddOrUpdate(VacAplic);
                                //dbContextSimple.SaveChanges();
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
                //dbContextSimple.Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        private void obtenerIngresosReingresosBajas(PlazasPorEmpleadosMov plazasPorEmpleadosMov)
        {
            camposParametro = new List<String>(0);
            valoresParametro = new List<Object>(0);
            strQuery.Remove(0, strQuery.Length).Append("FROM IngresosBajas ing where ing.empleados.clave = :claveEmpleado");
            if (plazasPorEmpleadosMov.plazasPorEmpleado.registroPatronal != null)
            {
                strQuery.Append(" and ing.registroPatronal.clave = :claveRegPat");
            }
            strQuery.Append(" and ing.razonesSociales.clave = :claveRazonSocial");
            camposParametro.Add("claveEmpleado");
            valoresParametro.Add(plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave);

            if (plazasPorEmpleadosMov.plazasPorEmpleado.registroPatronal != null)
            {
                camposParametro.Add("claveRegPat");
                valoresParametro.Add(plazasPorEmpleadosMov.plazasPorEmpleado.registroPatronal.clave);
            }
            camposParametro.Add("claveRazonSocial");
            valoresParametro.Add(plazasPorEmpleadosMov.plazasPorEmpleado.razonesSociales.clave);
            try
            {
                IQuery q = getSession().CreateQuery(strQuery.ToString());
                for (int i = 0; i < valoresParametro.Count; i++)
                {
                    if (valoresParametro[i].GetType() == typeof(object[]))
                    {
                        q.SetParameterList(camposParametro[i], (Object[])valoresParametro[i]);
                    }
                    else if (valoresParametro[i].GetType() == typeof(List<>))
                    {
                        q.SetParameterList(camposParametro[i], ((List<object>)valoresParametro[i]).ToArray());
                    }
                    else if (valoresParametro[i].GetType() == typeof(DateTime))
                    {
                        DateTime dateTime = (DateTime)valoresParametro[i];
                        q.SetParameter(camposParametro[i], dateTime.Date);
                    }
                    else
                    {
                        q.SetParameter(camposParametro[i], valoresParametro[i]);
                    }
                }
                IList<IngresosBajas> listIngresosReingresosBajas = q.List<IngresosBajas>();
                if (listIngresosReingresosBajas != null)
                {
                    DateTime fechaActual = DateTime.Now;
                    if (periodosNomina != null)
                    {
                        fechaActual = periodosNomina.fechaFinal.GetValueOrDefault();
                    }
                    for (int i = 0; i < listIngresosReingresosBajas.Count; i++)
                    {
                        if (listIngresosReingresosBajas[i].fechaBaja.GetValueOrDefault().CompareTo(fechaActual) <= 0)
                        {
                            ingresosReingresosBajas = listIngresosReingresosBajas[i];
                            //for (int j = 0; j < listIngresosReingresosBajas.Count; j++)
                            //{
                            //    if (listIngresosReingresosBajas[i]) { 

                            //    }
                            //}
                        }
                        else
                        {
                            ingresosReingresosBajas = listIngresosReingresosBajas[i];
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                mensajeResultado.error = (ex.GetBaseException().Message);
                mensajeResultado.noError = (168);
            }
        }

        private void buscaEmpleadoPTU(String claveRazonsocial, String claveEmpleado)
        {
            try
            {
                strQuery.Remove(0, strQuery.Length).Append("Select ptuEm ");
                strQuery.Append(" from PtuEmpleados ptuEm WHERE ptuEm.razonesSociales.clave = :claveRazonsocial ");
                strQuery.Append("AND ptuEm.empleados.clave = :claveEmpleado  ");
                ptuEmpleado = (PtuEmpleados)ejecutaQueryUnico(strQuery.ToString(), new String[] { "claveRazonsocial", "claveEmpleado" },
                        new Object[] { claveRazonsocial, claveEmpleado });
            }
            catch (HibernateException ex)
            {

                mensajeResultado.error = (ex.GetBaseException().Message);
                mensajeResultado.noError = (27);
            }
        }


        private List<CalculoUnidades> obtenerListCalculoUnidadesUtilizar(String claveRazonSocial, PlazasPorEmpleado plazaPorEmpleado, String claveTipoNomina, decimal ideriodoNomina, String claveTipoCorrida)
        {
            List<CalculoUnidades> listCalculoUnidades = null;
            try
            {
                camposParametro = new List<string>(0);
                valoresParametro = new List<Object>(0);
                strQuery.Remove(0, strQuery.Length).Append("SELECT mn FROM ").Append(typeof(CalculoUnidades).Name);
                strQuery.Append(" mn INNER JOIN mn.empleados em INNER JOIN mn.plazasPorEmpleado pem INNER JOIN mn.tipoCorrida tc INNER JOIN mn.tipoNomina tn INNER JOIN mn.razonesSociales rs ");
                strWhere = new StringBuilder("INNER JOIN mn.periodosNomina pn WHERE mn.uso = 0 AND pn.id = :ideriodoNomina AND pn.año = :yearPeriodo AND tc.clave = :claveTipoCorrida AND tn.clave = :claveTipoNomina AND em.clave = :claveEmpleado AND rs.clave = :claveRazonSocial AND pem.referencia = :clavePlazaEmpleado ");
                camposParametro.Add("ideriodoNomina");
                valoresParametro.Add(ideriodoNomina);
                camposParametro.Add("yearPeriodo");
                DateTime fecha = ((DateTime)valoresConceptosEmpleados[parametroFechaFinal]);
                valoresParametro.Add(fecha.Year);

                camposParametro.Add("claveTipoCorrida");
                valoresParametro.Add(claveTipoCorrida);
                camposParametro.Add("claveTipoNomina");
                valoresParametro.Add(claveTipoNomina);
                camposParametro.Add("claveEmpleado");
                valoresParametro.Add(plazaPorEmpleado.empleados.clave);
                camposParametro.Add("claveRazonSocial");
                valoresParametro.Add(claveRazonSocial);
                camposParametro.Add("clavePlazaEmpleado");
                valoresParametro.Add(plazaPorEmpleado.referencia);

                strQuery.Append(strWhere);
                strQuery.Append(" order by rs.clave,em.clave,tn.clave,pn.clave,tc.clave,mn.numero,mn.ejercicio,mn.mes ");
                IQuery q = getSession().CreateQuery(strQuery.ToString());
                for (int i = 0; i < valoresParametro.Count; i++)
                {
                    if (valoresParametro[i].GetType() == typeof(object[]))
                    {
                        q.SetParameterList(camposParametro[i], (Object[])valoresParametro[i]);
                    }
                    else if (valoresParametro[i].GetType() == typeof(List<>))
                    {
                        q.SetParameterList(camposParametro[i], ((List<object>)valoresParametro[i]).ToArray());
                    }
                    else if (valoresParametro[i].GetType() == typeof(DateTime))
                    {
                        DateTime dateTime = (DateTime)valoresParametro[i];
                        q.SetParameter(camposParametro[i], dateTime.Date);
                    }
                    else
                    {
                        q.SetParameter(camposParametro[i], valoresParametro[i]);
                    }
                }
                IList<CalculoUnidades> listCalculoUnidades2 = q.List<CalculoUnidades>();
                listCalculoUnidades = (List<CalculoUnidades>)listCalculoUnidades2;
                if (listCalculoUnidades == null ? true : listCalculoUnidades.Count == 0)
                {
                    listCalculoUnidades = new List<CalculoUnidades>();
                    listCalculoUnidades.Add(crearCalculoUnidades(plazaPorEmpleado));
                    if (evaluaPeriodoAbarca2Meses(periodosNomina))
                    {
                        CalculoUnidades calculoUnidad2 = crearCalculoUnidades(plazaPorEmpleado);
                        calculoUnidad2.mes = periodosNomina.fechaFinal.GetValueOrDefault().Month;
                        calculoUnidad2.numMovParticion = 2;
                        listCalculoUnidades.Add(calculoUnidad2);
                    }
                }
                else if (evaluaPeriodoAbarca2Meses(periodosNomina))
                {
                    List<CalculoUnidades> listTemp = new List<CalculoUnidades>();
                    int mesUno = periodosNomina.fechaInicial.GetValueOrDefault().Month, mesDos = periodosNomina.fechaFinal.GetValueOrDefault().Month;
                    bool mesUnoEncontrado, mesDosEncontrado;
                    int i = 0, j;
                    while (i < listCalculoUnidades.Count)
                    {
                        mesUnoEncontrado = false;
                        mesDosEncontrado = false;
                        for (j = 0; j < listCalculoUnidades.Count; j++)
                        {
                            if (listCalculoUnidades[i].numero == listCalculoUnidades[j].numero)
                            {
                                if (listCalculoUnidades[j].mes == mesUno)
                                {
                                    mesUnoEncontrado = true;
                                }
                                else if (listCalculoUnidades[j].mes == mesDos)
                                {
                                    mesDosEncontrado = true;
                                }
                            }
                        }
                        if (!mesUnoEncontrado || !mesDosEncontrado)
                        {
                            CalculoUnidades newUnidad = crearCalculoUnidades(plazaPorEmpleado);
                            if (!mesUnoEncontrado)
                            {
                                newUnidad.mes = periodosNomina.fechaInicial.GetValueOrDefault().Month;
                                newUnidad.numMovParticion = 1;
                            }
                            else
                            { //existe el mes dos genera el mes 1
                                newUnidad.mes = periodosNomina.fechaFinal.GetValueOrDefault().Month;
                                newUnidad.numMovParticion = 2;
                            }
                            listTemp.Add(newUnidad);
                        }
                        i++;
                    }
                    listCalculoUnidades.AddRange(listTemp);
                    //ordena la lista
                    listCalculoUnidades = (from cu in listCalculoUnidades orderby cu.numero, cu.ejercicio, cu.mes select cu).ToList();
                }
            }
            catch (Exception ex)
            {

                mensajeResultado.error = (ex.GetBaseException().Message);
                mensajeResultado.noError = (ControlErroresEntity.buscaNoErrorPorExcepcion(ex));
            }

            return listCalculoUnidades;
        }
        private CalculoUnidades crearCalculoUnidades(PlazasPorEmpleado plazaPorEmpleado)
        {
            DateTime fechaPeriodo = DateTime.Now;
            CalculoUnidades calculoUnidades = new CalculoUnidades();
            calculoUnidades.empleados = plazaPorEmpleado.empleados;
            calculoUnidades.plazasPorEmpleado = plazaPorEmpleado;
            calculoUnidades.periodosNomina = periodosNomina;
            calculoUnidades.tipoCorrida = tipoCorrida;
            calculoUnidades.tipoNomina = periodosNomina.tipoNomina;
            calculoUnidades.razonesSociales = razonesSociales;
            calculoUnidades.numero = 1;
            fechaPeriodo = (calculoUnidades.periodosNomina.fechaInicial.GetValueOrDefault());
            calculoUnidades.ejercicio = (periodosNomina.año);
            calculoUnidades.mes = (fechaPeriodo.Month);
            calculoUnidades.numMovParticion = 1;
            calculoUnidades.uso = 0;
            return calculoUnidades;
        }

        private bool evaluaPeriodoAbarca2Meses(PeriodosNomina periodo)
        {
            if (periodo == null)
                return false;
            if (periodo.fechaInicial.GetValueOrDefault().Month != periodo.fechaFinal.GetValueOrDefault().Month)
            {
                return true;
            }
            return false;
        }

        private int calcularEdad(DateTime birthDate)
        {
            DateTime now = DateTime.Now;
            int age = now.Year - birthDate.Year;
            if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day))
                age--;
            return age;
        }

        enum TipoAntiguedad
        {
            ANTIGUEDAD_ENTERO, ANTIGUEDAD_EXACTA, PORCION_ANTIGUEDAD, PORCION_DIAS
        }

        private object calcularAntiguedadExacta(DateTime fechaInicial, TipoAntiguedad tipoAntiguedad)
        {
            if (valoresConceptosEmpleados == null)
            {
                valoresConceptosEmpleados = new Dictionary<string, object>();
            }
            DateTime? fechaFinal = null;
            if (fechaBajaFiniq == null)
            {
                fechaFinal = DateTime.Now;
            }
            else
            {
                fechaFinal = fechaBajaFiniq;
            }
            try
            {
                fechaInicial = fechaInicial.Date;
                fechaFinal = fechaFinal.GetValueOrDefault().Date;

                long diferencia;
                diferencia = (long)((fechaFinal.GetValueOrDefault() - fechaInicial).TotalMilliseconds);
                double dias, antiguedad, antiguedadDias;
                dias = (double)Math.Floor((double)(diferencia / (1000 * 60 * 60 * 24)));
                antiguedad = dias / 365;
                antiguedadDias = (dias % 365);
                if (tipoAntiguedad == TipoAntiguedad.ANTIGUEDAD_EXACTA)
                {
                    return antiguedad;
                }
                else if (tipoAntiguedad == TipoAntiguedad.PORCION_ANTIGUEDAD)
                {
                    return antiguedad - (double)((int)antiguedad);
                }
                else if (tipoAntiguedad == TipoAntiguedad.PORCION_DIAS)
                {
                    return (int)antiguedadDias;
                }
                else
                {
                    return (int)antiguedad;
                }
            }
            catch (Exception ex)
            {
                mensajeResultado.noError = 22;
                mensajeResultado.error = String.Concat("Error al calcular antiguedad exacta", " ", ex.GetBaseException().ToString());
            }
            return 0.0;
        }


        private enum TipoSueldos
        {
            SUELDO_DIARIO_INICIAL, SUELDO_DIARIO_FINAL, SUELDO_DIARIO_VIGENTE, PERCEP_PLAZA, PERCEP_PLAZA_VIGENTE
        }

        private FactorIntegracion buscaFactorIntegracion(DateTime fechaIngreso)
        {
            FactorIntegracion factorDato = null;//factorDatoTmp
            try
            {
                int i, anioIngresoTmp = Convert.ToInt16(calcularAntiguedadExacta(fechaIngreso, TipoAntiguedad.ANTIGUEDAD_ENTERO)), anioIngreso;
                if (mensajeResultado.noError != 0)
                {
                    return factorDato;
                }
                anioIngreso = anioIngresoTmp;
                if (anioIngreso < 1)
                {
                    anioIngreso = 1;
                }
                double diasTotalesDeVacaciones = 0.0, y;
                for (i = 0; i < tablaFactorIntegracion.Length; i++)
                {
                    if (anioIngreso == Convert.ToInt16(tablaFactorIntegracion[i, 0]))
                    {
                        int x = 0;
                        if (anioIngresoTmp > 0)
                        {
                            x = Convert.ToInt16((calcularAntiguedadExacta(fechaIngreso, TipoAntiguedad.PORCION_DIAS)));
                        }
                        if (x > 0)
                        {
                            factorDato = new FactorIntegracion(new object[] { tablaFactorIntegracion[i, 0], tablaFactorIntegracion[i, 1], tablaFactorIntegracion[i, 2],
                                tablaFactorIntegracion[i, 3] , tablaFactorIntegracion[i, 4] });
                        }
                        else
                        {
                            factorDato = new FactorIntegracion(new object[] { tablaFactorIntegracion[i, 0], tablaFactorIntegracion[i, 1], tablaFactorIntegracion[i, 2],
                                tablaFactorIntegracion[i, 3] , tablaFactorIntegracion[i, 4] });
                        }
                        diasTotalesDeVacaciones += factorDato.diasVacaciones;
                        break;
                    }
                    else if (anioIngreso < Convert.ToInt16(tablaFactorIntegracion[i, 0]))
                    {
                        factorDato = new FactorIntegracion(new object[] { tablaFactorIntegracion[i - 1, 0], tablaFactorIntegracion[i - 1, 1], tablaFactorIntegracion[i - 1, 2],
                                tablaFactorIntegracion[i - 1, 3] , tablaFactorIntegracion[i - 1, 4] });
                        diasTotalesDeVacaciones += factorDato.diasVacaciones;
                        break;
                    }
                    else if (i == tablaFactorIntegracion.Length - 1 & anioIngreso > Convert.ToInt16(tablaFactorIntegracion[i, 0]))
                    {
                        factorDato = new FactorIntegracion(new object[] { tablaFactorIntegracion[i, 0], tablaFactorIntegracion[i, 1], tablaFactorIntegracion[i, 2],
                                tablaFactorIntegracion[i, 3] , tablaFactorIntegracion[i, 4] });
                        diasTotalesDeVacaciones += factorDato.diasVacaciones;
                        break;
                    }
                    else
                    {
                        diasTotalesDeVacaciones += Convert.ToInt16((tablaFactorIntegracion[i, 3]));
                    }
                }
                if (factorDato != null)
                {
                    diasTotalesDeVacaciones = diasTotalesDeVacaciones - factorDato.diasVacaciones;
                    int x = Convert.ToInt16(calcularAntiguedadExacta(fechaIngreso, TipoAntiguedad.PORCION_DIAS));
                    y = factorDato.diasVacaciones * x / 365.0;
                    diasTotalesDeVacaciones += y;
                    factorDato.diasVacacionesTotales = diasTotalesDeVacaciones;
                }
            }
            catch (Exception ex)
            {
                mensajeResultado.noError = 23;
                mensajeResultado.error = String.Concat("Error al buscar factor integracion", " ", ex.GetBaseException().ToString());
            }
            return factorDato;
        }

        private DateTime obtenerPrimerPlazasPorEmpleadosMov(PlazasPorEmpleadosMov plazasPorEmpleadosMov)
        {
            IList<PlazasPorEmpleadosMov> filtroPlazasPorEmpleadosMov2 = null;
            try
            {
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                strQuery.Remove(0, strQuery.Length).Append("SELECT pmov ");
                strQuery.Append(" FROM PlazasPorEmpleadosMov pmov ");
                strQuery.Append(" INNER JOIN pmov.plazasPorEmpleado pemp");
                strWhere.Remove(0, strWhere.Length).Append(" WHERE ");
                strWhere.Append(" pemp.id = :IdPlazasPorEmpleado");
                camposParametro.Add("IdPlazasPorEmpleado");
                valoresParametro.Add(plazasPorEmpleadosMov.plazasPorEmpleado.id);
                strWhere.Append(" AND pmov.fechaInicial = (SELECT MIN(pmovx.fechaInicial) FROM PlazasPorEmpleadosMov pmovx INNER JOIN pmovx.plazasPorEmpleado pemp ");
                strWhere.Append(" WHERE  pemp.id = :IdPlazasPorEmpleado )");
                strQuery.Append(strWhere.ToString());
                IQuery q = getSession().CreateQuery(strQuery.ToString());
                q = cargarParametrosQuery(q, camposParametro, valoresParametro);
                filtroPlazasPorEmpleadosMov2 = q.List<PlazasPorEmpleadosMov>();
                camposParametro = null;
                valoresParametro = null;
            }
            catch (HibernateException ex)
            {

                mensajeResultado.error = ex.GetBaseException().Message;
                mensajeResultado.noError = 27;
            }

            if (filtroPlazasPorEmpleadosMov2 == null ? true : !filtroPlazasPorEmpleadosMov2.Any() ? true : false)
            {
                return plazasPorEmpleadosMov.fechaIMSS.GetValueOrDefault();
            }
            else
            {
                return filtroPlazasPorEmpleadosMov2[0].fechaIMSS.GetValueOrDefault();
            }

        }

        private List<Asistencias> obtenerAsistencias(DateTime fechaInicial, DateTime fechaFinal)
        {
            IList<Asistencias> listAsistencias = null;
            try
            {
                if (periodosNomina == null)
                {
                    return new List<Asistencias>();
                }
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                camposParametro.Add("claveEmpleado");
                camposParametro.Add("claveTipoNomina");
                camposParametro.Add("claveTipoCorrida");

                camposParametro.Add("fechaInicio");
                camposParametro.Add("fechaFin");
                valoresParametro.Add(valoresConceptosEmpleados["NumEmpleado".ToUpper()]);
                valoresParametro.Add(valoresConceptosEmpleados["TipoNomina".ToUpper()]);
                valoresParametro.Add(valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()]);
                if (fechaBajaFiniq != null)
                {
                    strQuery.Remove(0, strQuery.Length).Append("Select p ");
                    strQuery.Append(" from PeriodosNomina p inner join p.tipoNomina  t ");
                    strQuery.Append(" Where (:fecha BETWEEN p.fechaInicial AND p.fechaFinal + 1) ");
                    strQuery.Append(" and t.clave = :claveTipoNomina AND p.tipoCorrida.clave = :claveTipoCorrida ");
                    PeriodosNomina periodosNominaTmp = (PeriodosNomina)ejecutaQueryUnico(strQuery.ToString(), new String[] { "claveTipoNomina", "fecha", "claveTipoCorrida" },
                            new Object[] { valoresConceptosEmpleados["TipoNomina".ToUpper()], fechaBajaFiniq.GetValueOrDefault().Date, valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()] });
                    if (periodosNominaTmp != null)
                    {
                        valoresParametro.Add(periodosNominaTmp.fechaInicial);
                        valoresParametro.Add(periodosNominaTmp.fechaFinal);
                    }
                    else
                    {
                        valoresParametro.Add(((DateTime)valoresConceptosEmpleados[parametroFechaInicial]).Date);
                        valoresParametro.Add(((DateTime)valoresConceptosEmpleados[parametroFechaFinal]).Date);
                    }
                }
                else
                {
                    valoresParametro.Add(fechaInicial);
                    valoresParametro.Add(fechaFinal);
                }
                camposParametro.Add("razonSocial");
                valoresParametro.Add(valoresConceptosEmpleados["RazonSocial".ToUpper()]);
                strQuery.Remove(0, strQuery.Length).Append("Select a From Asistencias a Inner Join a.tipoNomina tn Inner Join a.empleados em INNER JOIN a.razonesSociales rs Inner Join a.excepciones ex  Inner Join a.periodosNomina p ");
                strQuery.Append("Where em.clave = :claveEmpleado And tn.clave = :claveTipoNomina AND rs.clave  = :razonSocial And a.fecha between :fechaInicio And :fechaFin AND p.tipoCorrida.clave = :claveTipoCorrida ");
                IQuery q = getSession().CreateQuery(strQuery.ToString());
                q = cargarParametrosQuery(q, camposParametro, valoresParametro);
                listAsistencias = q.List<Asistencias>();
                listAsistencias = (listAsistencias == null ? new List<Asistencias>() : listAsistencias);
                if (mensajeResultado.noError == -100)
                {
                    mensajeResultado.noError = 51;
                    return (List<Asistencias>)listAsistencias;
                }

            }
            catch (HibernateException ex)
            {
                mensajeResultado.noError = 53;
            }

            return (List<Asistencias>)listAsistencias;
        }

        private List<RegistroIncapacidad> obtenerIncapacidadesPorEnfermedad(DateTime fechaInicial, DateTime fechaFinal)
        {
            IList<RegistroIncapacidad> listRegistroIncapacidad = null;
            try
            {
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                camposParametro.Add("claveEmpleado");
                camposParametro.Add("fechaInicio");
                camposParametro.Add("fechaFin");
                camposParametro.Add("razonSocial");
                valoresParametro.Add(valoresConceptosEmpleados["NumEmpleado".ToUpper()]);
                valoresParametro.Add(fechaInicial);
                valoresParametro.Add(fechaFinal);
                valoresParametro.Add(valoresConceptosEmpleados["RazonSocial".ToUpper()]);
                strQuery.Remove(0, strQuery.Length).Append("Select a From RegistroIncapacidad a Inner Join a.empleados em INNER JOIN a.empleados.razonesSociales rs ");
                strQuery.Append("Where a.ramoSeguro = 1 and em.clave = :claveEmpleado  AND rs.clave  = :razonSocial And (a.fechaInicial between :fechaInicio And :fechaFin or a.fechaFinal between :fechaInicio And :fechaFin) Order by a.fechaInicial");
                IQuery q = getSession().CreateQuery(strQuery.ToString());
                q = cargarParametrosQuery(q, camposParametro, valoresParametro);
                listRegistroIncapacidad = q.List<RegistroIncapacidad>();
                listRegistroIncapacidad = (listRegistroIncapacidad == null ? new List<RegistroIncapacidad>() : listRegistroIncapacidad);
                if (mensajeResultado.noError == -100)
                {
                    mensajeResultado.noError = 51;
                    return (List<RegistroIncapacidad>)listRegistroIncapacidad;
                }
            }
            catch (HibernateException ex)
            {
                mensajeResultado.noError = 53;

            }
            return (List<RegistroIncapacidad>)listRegistroIncapacidad;
        }

        private void cargarVariablesEmpleadoVacaciones(DateTime fechaInicial, DateTime fechaFinal, CalculoUnidades calculoUnidades, PlazasPorEmpleadosMov plazasPorEmpleadosMovEjecutandose, bool acumularVac)
        {
            int x;
            int diasVacaciones = 0;
            double diasPrimaVacacional = 0.0;
            try
            {
                bool corridaVacaciones = false;
                String claveCorrida = valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()].ToString();
                if (String.Equals(claveCorrida, "VAC", StringComparison.OrdinalIgnoreCase))
                {
                    corridaVacaciones = true;
                }
                List<VacacionesAplicacion> vacacionesAplicacion = obtenerVacaciones(periodosNomina, plazasPorEmpleadosMovEjecutandose, corridaVacaciones);

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
                                vacacionesAplicacion[x].vacacionesDisfrutadas.periodosNomina_periodoPago_ID = periodosNomina;
                                asigno = true;
                            }
                            if (vacacionesAplicacion[x].diasVac > 0)
                            {
                                vacacionesAplicacion[x].vacacionesDisfrutadas.periodosNomina_periodoPago_ID = periodosNomina;
                                asigno = true;
                            }
                            if (asigno)
                            {
                                getSession().SaveOrUpdate(vacacionesAplicacion[x].vacacionesDisfrutadas);
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
                    valoresConceptosEmpleados["diasVacacionesAcum".ToUpper()] = diasVacaciones;
                }
                else
                {

                    valoresConceptosEmpleados["fechaSalidaVacaciones".ToUpper()] = fechaSalidaVacaciones;
                    valoresConceptosEmpleados["fechaRegresoVacaciones".ToUpper()] = fechaRegresoVacaciones;
                    ////////            valoresConceptosEmpleados.put("fechaInicialTrabajadas".toUpperCase(), (Date) fechaInicialTrabajadas.getTime());
                    ////////            valoresConceptosEmpleados.put("fechaFinalTrabajadas".toUpperCase(), (Date) fechaFinalTrabajadas.getTime());
                    ////////////            valoresConceptosEmpleados.put("diasVacacionesDisfrutadas".toUpperCase(), (Integer) diasVacacionesDisfrutadas);
                    ////////////            valoresConceptosEmpleados.put("diasVacacionesTrabajadas".toUpperCase(), (Integer) diasVacacionesTrabajadas);
                    ////////            valoresConceptosEmpleados.put("diasVacacionesDisfrutadas".toUpperCase(), (Integer) diasVacacionesDisfPeriodo);
                    ////////            valoresConceptosEmpleados.put("diasVacacionesTrabajadas".toUpperCase(), (Integer) diasVacacionesTrabPeriodo);
                    ////////////            valoresConceptosEmpleados.put("diasVacacionesDisfPeriodo".toUpperCase(), (Integer) diasVacacionesDisfPeriodo);
                    ////////////            valoresConceptosEmpleados.put("diasVacacionesTrabPeriodo".toUpperCase(), (Integer) diasVacacionesTrabPeriodo);

                    valoresConceptosEmpleados["diasVacaciones".ToUpper()] = diasVacaciones;
                    valoresConceptosEmpleados["diasPrima".ToUpper()] = diasPrimaVacacional;
                    valoresConceptosEmpleados["tipoVacaciones".ToUpper()] = tipoVacaciones == null ? "" : tipoVacaciones.nombre;
                    if (calculoUnidades != null)
                    {
                        calculoUnidades.diasPrimaVacacional = diasPrimaVacacional;
                        calculoUnidades.diasVacaciones = diasVacaciones;
                        calculoUnidades.tiposVacaciones = tipoVacaciones;
                    }
                }
            }
            catch (HibernateException ex)
            {
                mensajeResultado.noError = 70;

            }
        }

        private List<VacacionesAplicacion> obtenerVacaciones(PeriodosNomina periodo, PlazasPorEmpleadosMov plazasPorEmpleadosMovEjecutandose, bool isCorridaVacaciones)
        {
            IList<VacacionesAplicacion> listVacacionesAplicacion = null;
            try
            {
                if (periodo == null)
                {
                    return new List<VacacionesAplicacion>();
                }
                camposParametro.Clear();
                valoresParametro.Clear();

                if (isCorridaVacaciones)
                {
                    strQuery.Remove(0, strQuery.Length);
                    strQuery.Append("Select va from VacacionesAplicacion va inner join va.vacacionesDisfrutadas vd inner join vd.empleados em  ");
                    strQuery.Append("inner join vd.periodosNomina_periodoAplicacion_ID pa  ");
                    strQuery.Append("where em.id = :idEmp AND (pa.fechaInicial BETWEEN :fechaInicial AND :fechaFinal) ");
                    camposParametro.Add("fechaInicial");
                    valoresParametro.Add(periodo.fechaInicial);
                    camposParametro.Add("fechaFinal");
                    valoresParametro.Add(periodo.fechaFinal);
                }
                else
                {
                    if (fechaBajaFiniq != null)
                    {
                        strQuery.Remove(0, strQuery.Length).Append("Select p ");
                        strQuery.Append(" from PeriodosNomina p inner join p.tipoNomina  t inner join p.tipoCorrida  c ");
                        strQuery.Append(" Where (:fecha BETWEEN p.fechaInicial AND p.fechaFinal + 1) ");
                        strQuery.Append(" and t.clave = :claveTipoNomina and c.clave = :claveTipoCorrida  ");
                        PeriodosNomina periodosNominaTmp = (PeriodosNomina)ejecutaQueryUnico(strQuery.ToString(), new String[] { "claveTipoNomina", "fecha", "claveTipoCorrida" },
                                new Object[] { valoresConceptosEmpleados["TipoNomina".ToUpper()], fechaBajaFiniq.GetValueOrDefault().Date, valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()] });
                        if (periodosNominaTmp != null)
                        {
                            valoresParametro.Add(periodosNominaTmp.id);
                        }
                        else
                        {
                            valoresParametro.Add(periodo.id);
                        }
                    }
                    else
                    {
                        valoresParametro.Add(periodo.id);
                    }
                    strQuery.Remove(0, strQuery.Length);
                    strQuery.Append("Select va from VacacionesAplicacion va inner join va.vacacionesDisfrutadas vd inner join vd.empleados em  ");
                    strQuery.Append("inner join vd.periodosNomina_periodoAplicacion_ID pa  ");
                    strQuery.Append("where pa.id = :idPeriodo and em.id = :idEmp ");
                    camposParametro.Add("idPeriodo");
                }
                camposParametro.Add("idEmp");
                valoresParametro.Add(plazasPorEmpleadosMovEjecutandose.plazasPorEmpleado.empleados.id);
                IQuery q = getSession().CreateQuery(strQuery.ToString());
                q = cargarParametrosQuery(q, camposParametro, valoresParametro);
                listVacacionesAplicacion = q.List<VacacionesAplicacion>();
                if (mensajeResultado.noError == -100)
                {
                    mensajeResultado.noError = 71;
                    //System.err.println(concatena.delete(0, concatena.length()).append(msgError).append("obtenerRegistraVacaciones()1_Error: "));
                    return (List<VacacionesAplicacion>)listVacacionesAplicacion;
                }
                camposParametro = null;
                valoresParametro = null;
            }
            catch (HibernateException ex)
            {
                mensajeResultado.noError = 71;

            }
            return (List<VacacionesAplicacion>)listVacacionesAplicacion;
        }

        private IQuery cargarParametrosQuery(IQuery q, List<String> camposParametros, List<Object> valoresParametros)
        {
            try
            {
                for (int i = 0; i < valoresParametros.Count; i++)
                {
                    if (valoresParametros[i].GetType() == typeof(object[]))
                    {
                        q.SetParameterList(camposParametros[i], (Object[])valoresParametros[i]);
                    }
                    else if (valoresParametros[i].GetType() == typeof(List<>))
                    {
                        q.SetParameterList(camposParametros[i], ((List<object>)valoresParametros[i]).ToArray());
                    }
                    else if (valoresParametros[i].GetType() == typeof(string[]))
                    {
                        q.SetParameterList(camposParametros[i], ((string[])valoresParametros[i]));
                    }
                    else if (valoresParametros[i].GetType() == typeof(DateTime))
                    {
                        DateTime dateTime = (DateTime)valoresParametros[i];
                        q.SetParameter(camposParametros[i], dateTime.Date);
                    }
                    else
                    {
                        q.SetParameter(camposParametros[i], valoresParametros[i]);
                    }
                }
            }
            catch (Exception ex)
            {

                mensajeResultado.noError = 100;
                mensajeResultado.error = ex.GetBaseException().Message;
            }
            return q;
        }

        private void obtenerMovimientosNominaPorPlaza(String claveTipoCorrida, String claveTipoNomina, decimal idPeriodoNomina, PlazasPorEmpleado plazaPorEmpleado,
            String claveCtrCosto, String claveRazonSocial)
        {
            filtroMovimientosNominas = new List<MovNomConcep>();
            filtroConceptosNomina = new List<ConcepNomDefi>();
            try
            {

                filtroMovimientosNominas.AddRange(buscaMovimientosPlazasPorTipoYBaseAfecta(claveTipoCorrida, claveTipoNomina, idPeriodoNomina, plazaPorEmpleado, claveCtrCosto, Tipo.AUTOMATICO, claveRazonSocial, null, -1, null));

                if (mensajeResultado.noError != 0)
                {
                    return;
                }

                buscaConceptosTipoAutomatico(claveTipoCorrida);

                if (mensajeResultado.noError != 0)
                {
                    return;
                }
                int i = 0, j = 0;
                if (obtenerConceptosUnicos(filtroMovimientosNominas) != filtroConceptosNomina.Count)
                {

                    while (i < filtroMovimientosNominas.Count)
                    {
                        while (j < filtroConceptosNomina.Count)
                        {
                            if (String.Equals(filtroMovimientosNominas[i].concepNomDefi.clave, filtroConceptosNomina[j].clave, StringComparison.OrdinalIgnoreCase))
                            {
                                filtroConceptosNomina.Remove(filtroConceptosNomina[j]);
                                break;
                            }
                            else
                            {
                                j++;
                            }
                        }
                        j = 0;
                        i++;
                    }

                    filtroMovimientosNominas.AddRange(creaMovimientosPlazasConceptosAutomaticos(plazaPorEmpleado, periodosNomina, claveTipoCorrida, claveRazonSocial, claveCtrCosto));
                    if (mensajeResultado.noError != 0)
                    {
                        return;
                    }
                }

                //busca conceptos del periodo en los movimientos
                filtroMovimientosNominas.AddRange(buscaMovimientosPlazasPorTipoYBaseAfecta(claveTipoCorrida, claveTipoNomina, idPeriodoNomina, plazaPorEmpleado, claveCtrCosto, Tipo.PERIODO, claveRazonSocial, null, -1, null));
                if (mensajeResultado.noError != 0)
                {
                    return;
                }
                //busca conceptos repetitivos en los movimientos.
                filtroMovimientosNominas.AddRange(buscaMovimientosPlazasPorTipoYBaseAfecta(claveTipoCorrida, claveTipoNomina, idPeriodoNomina, plazaPorEmpleado, claveCtrCosto, Tipo.REPETITIVO, claveRazonSocial, null, -1, null));
                if (mensajeResultado.noError != 0)
                {
                    return;
                }
                ordenarMovimientosNomina();
                generarMovimientosAbarca2Meses();
                filtroConceptosNomina = null;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerMovimientosNominaPorPlaza()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();

            }

        }

        private void obtenerMovimientosNominaPorPlaza2(String claveTipoCorrida, String claveTipoNomina, decimal idPeriodoNomina, PlazasPorEmpleado plazaPorEmpleado,
            String claveCtrCosto, String claveRazonSocial, bool usaPlaza)
        {
            filtroMovimientosNominasAux = new List<MovNomConcep>(0);
            filtroConceptosNominaAux = new List<ConcepNomDefi>(0);
            try
            {
                filtroMovimientosNominasAux.AddRange(buscaMovimientosPlazasPorTipoYBaseAfecta(claveTipoCorrida, claveTipoNomina, idPeriodoNomina, plazaPorEmpleado, claveCtrCosto, Tipo.AUTOMATICO, claveRazonSocial, null, -1, null));
                //  }
                if (mensajeResultado.noError != 0)
                {
                    return;
                }
                buscaConceptosTipoAutomatico2(claveTipoCorrida);
                //            System.out.println("tamaños filtroMovimientosNominas" + filtroMovimientosNominas.size() + " filtroConceptosNomina " + filtroConceptosNomina.size());
                if (mensajeResultado.noError != 0)
                {
                    return;
                }
                int i = 0, j = 0;
                if (obtenerConceptosUnicos(filtroMovimientosNominasAux) != filtroConceptosNominaAux.Count)
                {

                    while (i < filtroMovimientosNominasAux.Count)
                    {
                        while (j < filtroConceptosNominaAux.Count)
                        {
                            if (String.Equals(filtroMovimientosNominasAux[i].concepNomDefi.clave, filtroConceptosNominaAux[j].clave, StringComparison.OrdinalIgnoreCase))
                            {
                                filtroConceptosNominaAux.Remove(filtroConceptosNominaAux[j]);
                                break;
                            }
                            else
                            {
                                j++;
                            }
                        }
                        j = 0;
                        i++;
                    }

                    filtroMovimientosNominasAux.AddRange(creaMovimientosPlazasConceptosAutomaticos(plazaPorEmpleado, periodosNomina, claveTipoCorrida, claveRazonSocial, claveCtrCosto));
                    if (mensajeResultado.noError != 0)
                    {
                        return;
                    }
                }

                //busca conceptos del periodo en los movimientos
                filtroMovimientosNominasAux.AddRange(buscaMovimientosPlazasPorTipoYBaseAfecta(claveTipoCorrida, claveTipoNomina, idPeriodoNomina, plazaPorEmpleado, claveCtrCosto, Tipo.PERIODO, claveRazonSocial, null, -1, null));
                if (mensajeResultado.noError != 0)
                {
                    return;
                }
                //busca conceptos repetitivos en los movimientos.
                filtroMovimientosNominasAux.AddRange(buscaMovimientosPlazasPorTipoYBaseAfecta(claveTipoCorrida, claveTipoNomina, idPeriodoNomina, plazaPorEmpleado, claveCtrCosto, Tipo.REPETITIVO, claveRazonSocial, null, -1, null));
                if (mensajeResultado.noError != 0)
                {
                    return;
                }
                ordenarMovimientosNomina2();
                generarMovimientosAbarca2Meses2();
                filtroConceptosNominaAux = null;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerMovimientosNominaPorPlaza()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();

            }
        }

        private List<MovNomConcep> obtenerMovimientosPlazasFiniquitos(String claveTipoCorrida, String claveTipoNomina, decimal idPeriodoNomina, PlazasPorEmpleado plazaPorEmpleado,
           String claveCtrCosto, String claveRazonSocial, int uso)
        {
            IList<MovNomConcep> movNomConceptos = null;
            try
            {
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                if (finiquitosLiquidaciones == null)
                {
                    strQuery.Remove(0, strQuery.Length).Append("SELECT mn FROM MovNomConcep mn INNER JOIN mn.empleado em INNER JOIN mn.plazasPorEmpleado pem INNER JOIN mn.tipoCorrida tc INNER JOIN mn.tipoNomina tn INNER JOIN mn.periodosNomina pn INNER JOIN mn.razonesSociales rs ");
                    strWhere.Remove(0, strWhere.Length).Append(" WHERE mn.uso = :uso AND pn.id = :idPeriodoNomina AND tc.clave = :claveTipoCorrida AND tn.clave = :claveTipoNomina AND em.clave = :claveEmpleado AND rs.clave = :claveRazonSocial AND pem.clave = :clavePlazaEmpleado ");
                    camposParametro.Add("idPeriodoNomina");
                    valoresParametro.Add(idPeriodoNomina);
                    camposParametro.Add("claveTipoCorrida");
                    valoresParametro.Add(claveTipoCorrida);
                    camposParametro.Add("claveTipoNomina");
                    valoresParametro.Add(claveTipoNomina);
                    camposParametro.Add("claveEmpleado");
                    valoresParametro.Add(plazaPorEmpleado.empleados.clave);
                    camposParametro.Add("claveRazonSocial");
                    valoresParametro.Add(claveRazonSocial);
                    camposParametro.Add("clavePlazaEmpleado");
                    valoresParametro.Add(plazaPorEmpleado.referencia);
                    camposParametro.Add("uso");
                    valoresParametro.Add(uso);
                    //                if (usaNominaAsimiladosAsalarios) {
                    //                    strWhere.append("AND mn.concepNomDefi.nominaAsimilados = :nominaAsimilados ");
                    //                    camposParametro.add("nominaAsimilados");
                    //                    valoresParametro.add(true);
                    //                }
                    claveCtrCosto = (claveCtrCosto == null ? "" : claveCtrCosto);
                    if (claveCtrCosto.Any())
                    {
                        strQuery.Append("INNER JOIN mn.centroDeCosto cc ");
                        strWhere.Append("AND cc.clave = :claveCtrCosto ");
                        camposParametro.Add("claveCtrCosto");
                        valoresParametro.Add(claveCtrCosto);
                    }
                }
                else
                {
                    strQuery.Remove(0, strQuery.Length).Append("SELECT mn FROM MovNomConcep mn INNER JOIN mn.finiqLiquidCncNom finiCnc INNER JOIN finiCnc.finiquitosLiquidacion fini ");
                    strWhere.Remove(0, strWhere.Length).Append(" WHERE fini.id = :finiquitosLiquidacion_ID ");
                    camposParametro.Add("finiquitosLiquidacion_ID");
                    valoresParametro.Add(finiquitosLiquidaciones.id);
                }
                strWhere.Append("Order By mn.concepNomDefi.prioridadDeCalculo ");
                strQuery.Append(strWhere);
                IQuery q = getSession().CreateQuery(strQuery.ToString());
                q = cargarParametrosQuery(q, camposParametro, valoresParametro);
                movNomConceptos = q.List<MovNomConcep>();
                if (mensajeResultado.noError == -100)
                {
                    mensajeResultado.noError = 31;
                    return new List<MovNomConcep>(0);
                }
                camposParametro = null;
                valoresParametro = null;
            }
            catch (Exception ex)
            {
                mensajeResultado.noError = 100;
                mensajeResultado.error = ex.GetBaseException().Message;
                //System.err.println(concatena.delete(0, concatena.length()).append(msgError).append("buscaMovimientosTipoPeriodo()1_Error: ").append(ex));
            }
            return (List<MovNomConcep>)movNomConceptos;

        }

        private List<MovNomConcep> buscaMovimientosPlazasPorTipoYBaseAfecta(String claveTipoCorrida, String claveTipoNomina, decimal idPeriodoNomina, PlazasPorEmpleado plazaPorEmpleado,
           String claveCtrCosto, Tipo? tipo, String claveRazonSocial, String claveBaseNomina, int tipoBaseAfecta, String claveConcepto)
        {
            IList<MovNomConcep> movNomConceptos = null;
            try
            {
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                strQuery.Remove(0, strQuery.Length).Append("SELECT mn FROM MovNomConcep mn INNER JOIN mn.concepNomDefi con INNER JOIN mn.empleados em INNER JOIN mn.plazasPorEmpleado pem INNER JOIN mn.tipoCorrida tc INNER JOIN mn.tipoNomina tn INNER JOIN mn.razonesSociales rs ");
                bool usaBaseNomina = false;
                if (claveBaseNomina != null & tipoBaseAfecta > -1)
                {
                    usaBaseNomina = true;
                    strQuery.Append("INNER JOIN mn.movNomBaseAfecta mba INNER JOIN  mba.baseAfecConcepNom bac INNER JOIN bac.baseNomina bn ");
                }
                //si no usa tipo de concepto
                if (tipo == null)
                {
                    strWhere = new StringBuilder("INNER JOIN mn.periodosNomina pn WHERE mn.uso = 0 AND pn.id = :idPeriodoNomina AND tc.clave = :claveTipoCorrida AND tn.clave = :claveTipoNomina AND em.clave = :claveEmpleado AND rs.clave = :claveRazonSocial AND pem.referencia = :clavePlazaEmpleado ");
                    camposParametro.Add("idPeriodoNomina");
                    valoresParametro.Add(idPeriodoNomina);
                }
                else if (tipo == Tipo.REPETITIVO)
                {
                    strWhere.Remove(0, strWhere.Length).Append("INNER JOIN mn.periodosNomina pn WHERE tc.clave = :claveTipoCorrida AND tn.clave = :claveTipoNomina AND em.clave = :claveEmpleado AND mn.uso = 0 AND rs.clave = :claveRazonSocial AND pem.referencia = :clavePlazaEmpleado AND pn.tipoCorrida.clave = :claveTipoCorrida ");
                    //.append("AND (mn.fechaIni Is Null or  mn.fechaCierr is NULL) or (current_date() between mn.fechaIni and mn.fechaCierr)");

                    /*
                     * SELECT mn FROM MovNomConcep mn INNER JOIN mn.concepNomDefi
                     * con INNER JOIN mn.periodosNomina pn INNER JOIN mn.empleado em
                     * WHERE con.tipo = 2 AND em.clave = 1 And (mn.fechaIni Is Null
                     * or mn.fechaCierr is NULL) or (current_date() between
                     * mn.fechaIni and mn.fechaCierr)
                     */
                }
                else
                {
                    strWhere.Remove(0, strWhere.Length).Append("INNER JOIN mn.periodosNomina pn WHERE mn.uso = 0 AND pn.id = :idPeriodoNomina AND tc.clave = :claveTipoCorrida AND tn.clave = :claveTipoNomina AND em.clave = :claveEmpleado and con.tipo = :tipo AND rs.clave = :claveRazonSocial AND pem.referencia = :clavePlazaEmpleado ");
                    camposParametro.Add("idPeriodoNomina");
                    valoresParametro.Add(idPeriodoNomina);
                }
                if (tipo != null)
                {
                    strWhere.Append("AND con.tipo = :tipo ");
                    camposParametro.Add("tipo");
                    valoresParametro.Add(tipo);
                }
                if (usaBaseNomina)
                {
                    strWhere.Append("AND bn.clave = :claveBaseNomina AND bac.tipoAfecta = :tipoBaseAfecta ");
                    camposParametro.Add("claveBaseNomina");
                    valoresParametro.Add(claveBaseNomina);
                    camposParametro.Add("tipoBaseAfecta");
                    valoresParametro.Add(tipoBaseAfecta);
                }

                camposParametro.Add("claveTipoCorrida");
                valoresParametro.Add(claveTipoCorrida);
                camposParametro.Add("claveTipoNomina");
                valoresParametro.Add(claveTipoNomina);
                camposParametro.Add("claveEmpleado");
                valoresParametro.Add(plazaPorEmpleado.empleados.clave);
                camposParametro.Add("claveRazonSocial");
                valoresParametro.Add(claveRazonSocial);
                camposParametro.Add("clavePlazaEmpleado");
                valoresParametro.Add(plazaPorEmpleado.referencia);

                claveCtrCosto = (claveCtrCosto == null ? "" : claveCtrCosto);
                if (claveCtrCosto.Any())
                {
                    strQuery.Append("LEFT OUTER JOIN mn.centroDeCosto cc ");
                    strWhere.Append("AND cc.clave = :claveCtrCosto ");
                    camposParametro.Add("claveCtrCosto");
                    valoresParametro.Add(claveCtrCosto);
                }
                claveConcepto = (claveConcepto == null ? "" : claveConcepto);
                if (claveConcepto.Any())
                {
                    strWhere.Append(" AND con.clave = :claveConcepto ");
                    camposParametro.Add("claveConcepto");
                    valoresParametro.Add(claveConcepto);
                }
                //            //Se agrego este codigo para que no se agregaran los conceptos que ya fueron asignados por los creditos o ahorros
                //            strWhere.append(" AND mn.id not in  (select mov.id from CreditoMovimientos credMov inner join credMov.movNomConcep mov ");
                //            strWhere.append(" inner join credMov.creditoPorEmpleado credEm inner join credEm.empleados em inner join credEm.razonesSociales rs ");
                //            strWhere.append(" where em.clave = :claveEmpleado and rs.clave = :claveRazonSocial) ");
                strQuery.Append(strWhere);
                if (tipo == Tipo.REPETITIVO)
                {
                    strQuery.Append(" AND (pn.clave <= :clavePeriodoNomina AND pn.año = :yearPeriodo AND pn.tipoCorrida.clave = :claveTipoCorrida) Order by con.id, pn.id");
                    camposParametro.Add("clavePeriodoNomina");
                    valoresParametro.Add(periodosNomina.clave);
                    camposParametro.Add("yearPeriodo");
                    valoresParametro.Add(periodosNomina.año);
                }
                IQuery q = getSession().CreateQuery(strQuery.ToString());
                q = cargarParametrosQuery(q, camposParametro, valoresParametro);
                movNomConceptos = q.List<MovNomConcep>();
                if (mensajeResultado.noError == -100)
                {
                    if (tipo == Tipo.AUTOMATICO)
                    {
                        mensajeResultado.noError = 29;
                    }
                    else if (tipo == Tipo.PERIODO)
                    {
                        mensajeResultado.noError = 30;
                    }
                    else
                    {
                        mensajeResultado.noError = 31;
                    }
                    return null;
                }
                movNomConceptos = (movNomConceptos == null ? new List<MovNomConcep>(0) : movNomConceptos);
                if (movNomConceptos.Count == 0 | tipo == Tipo.REPETITIVO)
                {
                    //codigi cardex nomina
                    /**
                     * *********************FILTRA REPETITIVOS
                     * VALIDOS*****************************
                     */
                    if (tipo == Tipo.REPETITIVO & movNomConceptos.Count > 0)
                    {
                        int i;
                        List<MovNomConcep> movimientosRepetitivos = new List<MovNomConcep>();
                        MovNomConcep movRepetitivo;
                        for (i = 0; i < movNomConceptos.Count; i++)
                        {
                            movRepetitivo = movNomConceptos[i];
                            //  After > 0 before  < 0
                            if ((fechaActual.CompareTo(movRepetitivo.fechaIni) > 0 | fechaActual.Equals(movRepetitivo.fechaIni))
                                    && (fechaActual.CompareTo(movRepetitivo.fechaCierr) < 0 | fechaActual.Equals(movRepetitivo.fechaCierr)))
                            {
                                if (movimientosRepetitivos.Count >= 1)
                                {
                                    int x = 0;
                                    bool noExisteMov = true;
                                    while (x < movimientosRepetitivos.Count)
                                    {
                                        //movimientosRepetitivos.get(x).getId() < movRepetitivo.getId() & 
                                        if (String.Equals(movimientosRepetitivos[x].concepNomDefi.clave, movRepetitivo.concepNomDefi.clave, StringComparison.OrdinalIgnoreCase))
                                        {
                                            noExisteMov = false;
                                            if (movRepetitivo.periodosNomina.id == periodosNomina.id)
                                            {
                                                if (movimientosRepetitivos[x].periodosNomina.id == periodosNomina.id)
                                                {
                                                    movimientosRepetitivos.Add(movRepetitivo);
                                                }
                                                else
                                                {
                                                    movimientosRepetitivos[x] = movRepetitivo;
                                                }
                                                break;
                                            }
                                            break;
                                        }
                                        x++;
                                    }
                                    if (noExisteMov)
                                    {
                                        movimientosRepetitivos.Add(movRepetitivo);
                                    }
                                }
                                else
                                {
                                    movimientosRepetitivos.Add(movRepetitivo);
                                }
                            }
                            else //si no pasa y existe mismo concepto remplazarlo
                            {
                                if (movimientosRepetitivos.Count >= 1)
                                {
                                    int x = 0;
                                    bool noExisteMov = true;
                                    while (x < movimientosRepetitivos.Count)
                                    {
                                        ///movimientosRepetitivos.get(x).getId() < movRepetitivo.getId() &
                                        if (String.Equals(movimientosRepetitivos[x].concepNomDefi.clave, movRepetitivo.concepNomDefi.clave, StringComparison.OrdinalIgnoreCase))
                                        {
                                            noExisteMov = false;
                                            if (movRepetitivo.periodosNomina.id == periodosNomina.id)
                                            {
                                                if (movimientosRepetitivos[x].periodosNomina.id == periodosNomina.id)
                                                {
                                                    movimientosRepetitivos.Add(movRepetitivo);
                                                }
                                                else
                                                {
                                                    movimientosRepetitivos.Insert(x, movRepetitivo);
                                                }
                                                break;
                                            }
                                            break;
                                        }
                                        x++;
                                    }
                                    if (noExisteMov)
                                    {
                                        movimientosRepetitivos.Add(movRepetitivo);
                                    }
                                }
                                else
                                {
                                    movimientosRepetitivos.Add(movRepetitivo);
                                }
                            }
                        } //end for
                        movNomConceptos = movimientosRepetitivos;
                        for (i = 0; i < movNomConceptos.Count; i++)
                        {
                            if (movNomConceptos[i].periodosNomina.id != periodosNomina.id)
                            {
                                MovNomConcep mnc = movNomConceptos[i];
                                movNomConceptos[i] = creaMovNomConcep(movNomConceptos[i].concepNomDefi, plazaPorEmpleado, periodosNomina, tipoCorrida, razonesSociales, centroDeCostoMovimiento);

                                if (mnc.movNomConceParam != null)
                                {
                                    int j;
                                    for (j = 0; j < mnc.movNomConceParam.Count; j++)
                                    {
                                        movNomConceptos[i].movNomConceParam[j].valor = mnc.movNomConceParam[j].valor;
                                    }
                                }

                            }
                        }
                    }
                    /*
                     * ********************TERMINA FILTRAdo REPETITIVOS VALIDOS
                     */
                }
                camposParametro = null;
                valoresParametro = null;
            }
            catch (Exception ex)
            {
                mensajeResultado.error = ex.GetBaseException().Message;
                if (tipo == Tipo.AUTOMATICO)
                {
                    mensajeResultado.noError = 29;
                }
                else if (tipo == Tipo.PERIODO)
                {
                    mensajeResultado.noError = 30;
                }
                else
                {
                    mensajeResultado.noError = 31;
                }
                //System.err.println(concatena.delete(0, concatena.length()).append(msgError).append("buscaMovimientosTipoPeriodo()1_Error: ").append(ex));
            }
            return (List<MovNomConcep>)movNomConceptos;
        }

        private MovNomConcep creaMovNomConcep(ConcepNomDefi concepNomDefi, PlazasPorEmpleado plazaPorEmpleado, PeriodosNomina periodosNominas, TipoCorrida tipoCorrida, RazonesSociales razonesSociales, CentroDeCosto centroDeCosto)
        {

            DateTime fechaPeriodo = DateTime.Now;
            MovNomConcep movNomConcep = new MovNomConcep();
            movNomConcep.empleados = plazaPorEmpleado.empleados;
            movNomConcep.plazasPorEmpleado = plazaPorEmpleado;
            movNomConcep.periodosNomina = periodosNominas;
            movNomConcep.concepNomDefi = concepNomDefi;
            movNomConcep.tipoCorrida = tipoCorrida;
            movNomConcep.tipoNomina = periodosNominas.tipoNomina;
            movNomConcep.centroDeCosto = centroDeCosto;
            movNomConcep.razonesSociales = razonesSociales;
            if (concepNomDefi.baseAfecConcepNom != null)
            {
                movNomConcep.movNomBaseAfecta = (creaMovimBaseAfectar(concepNomDefi.baseAfecConcepNom, movNomConcep));
            }

            if (movNomConcep.concepNomDefi.paraConcepDeNom == null ? false : !movNomConcep.concepNomDefi.paraConcepDeNom.Any() ? false : true)
            {
                movNomConcep.movNomConceParam = (creaMovNomConceParam(movNomConcep.concepNomDefi, movNomConcep));
            }
            movNomConcep.fechaCierr = periodosNominas.fechaCierre;
            movNomConcep.fechaIni = periodosNominas.fechaInicial;
            movNomConcep.tipoPantalla = tipoPantallaSistema;
            movNomConcep.ordenId = 0;
            movNomConcep.resultado = 0.0;
            movNomConcep.numero = 1;
            movNomConcep.calculado = 0.0;
            fechaPeriodo = movNomConcep.periodosNomina.fechaInicial.GetValueOrDefault();
            movNomConcep.ejercicio = periodosNominas.año;
            movNomConcep.mes = fechaPeriodo.Month;
            movNomConcep.numMovParticion = 1;
            movNomConcep.uso = 0;
            return movNomConcep;
        }

        private List<MovNomBaseAfecta> creaMovimBaseAfectar(IList<BaseAfecConcepNom> baseAfecConcepNominas, MovNomConcep mnc)
        {
            List<MovNomBaseAfecta> movNominaBaseAfectas = new List<MovNomBaseAfecta>(0);
            MovNomBaseAfecta m;
            if (mnc.movNomBaseAfecta == null ? true : !mnc.movNomBaseAfecta.Any())
            {
                for (int i = 0; i < baseAfecConcepNominas.Count; i++)
                {
                    m = new MovNomBaseAfecta();
                    m.baseAfecConcepNom = baseAfecConcepNominas[i];
                    m.movNomConcep = mnc;
                    m.uso = 0;
                    movNominaBaseAfectas.Add(m);
                }

            }
            else if (!baseAfecConcepNominas.Any())
            {
                if (mnc.movNomBaseAfecta.Any())
                {
                    for (int j = 0; j < mnc.movNomBaseAfecta.Count; j++)
                    {
                        getSession().Delete(mnc.movNomBaseAfecta[j]);
                    }
                }
            }
            else
            {
                List<MovNomBaseAfecta> movNominaBaseAfectasTmp = new List<MovNomBaseAfecta>();
                movNominaBaseAfectasTmp.AddRange(mnc.movNomBaseAfecta);
                if (movNominaBaseAfectasTmp == null)
                {
                    movNominaBaseAfectasTmp = new List<MovNomBaseAfecta>();
                    mnc.movNomBaseAfecta = (movNominaBaseAfectasTmp);
                }
                for (int i = 0; i < baseAfecConcepNominas.Count(); i++)
                {
                    for (int j = 0; j < mnc.movNomBaseAfecta.Count; j++)
                    {
                        bool existe = false;
                        if (String.Equals(baseAfecConcepNominas[i].baseNomina.clave, mnc.movNomBaseAfecta[j].baseAfecConcepNom.baseNomina.clave, StringComparison.OrdinalIgnoreCase))
                        {
                            existe = true;
                        }
                        if (existe)
                        {
                            movNominaBaseAfectas.Add(mnc.movNomBaseAfecta[j]);
                            movNominaBaseAfectasTmp.Remove(mnc.movNomBaseAfecta[j]);
                        }
                        else
                        {
                            m = new MovNomBaseAfecta();
                            m.baseAfecConcepNom = baseAfecConcepNominas[i];
                            m.movNomConcep = mnc;
                            m.uso = 0;
                            movNominaBaseAfectas.Add(m);
                        }
                    }
                }
                if (movNominaBaseAfectasTmp.Any())
                {
                    for (int j = 0; j < movNominaBaseAfectasTmp.Count; j++)
                    {
                        getSession().Delete(movNominaBaseAfectasTmp[j]);
                    }
                }
            }
            return movNominaBaseAfectas;
        }

        private List<MovNomConceParam> creaMovNomConceParam(ConcepNomDefi concepNomDefi, MovNomConcep mnc)
        {
            List<MovNomConceParam> movNomConceParam = new List<MovNomConceParam>(0);
            MovNomConceParam m;
            if (mnc.movNomConceParam == null ? true : mnc.movNomConceParam.Any())
            {
                for (int i = 0; i < concepNomDefi.paraConcepDeNom.Count; i++)
                {
                    m = new MovNomConceParam();
                    m.paraConcepDeNom = (concepNomDefi.paraConcepDeNom[i]);
                    m.movNomConcep = mnc;
                    m.valor = "0";
                    movNomConceParam.Add(m);
                }

            }
            else if (concepNomDefi.paraConcepDeNom.Any())
            {
                if (mnc.movNomConceParam.Any())
                {
                    for (int j = 0; j < mnc.movNomConceParam.Count; j++)
                    {
                        getSession().Delete(mnc.movNomConceParam[j]);
                    }
                }
            }
            else
            {
                List<MovNomConceParam> movNominaBaseAfectasTmp = new List<MovNomConceParam>();
                movNominaBaseAfectasTmp.AddRange(mnc.movNomConceParam);
                if (movNominaBaseAfectasTmp == null)
                {
                    movNominaBaseAfectasTmp = new List<MovNomConceParam>();
                    mnc.movNomConceParam = (movNominaBaseAfectasTmp);
                }
                for (int i = 0; i < concepNomDefi.paraConcepDeNom.Count; i++)
                {
                    for (int j = 0; j < mnc.movNomConceParam.Count; j++)
                    {
                        bool existe = false;
                        if (concepNomDefi.paraConcepDeNom[i].id.Equals(mnc.movNomConceParam[j].paraConcepDeNom.id))
                        {
                            existe = true;
                        }
                        if (existe)
                        {
                            movNomConceParam.Add(mnc.movNomConceParam[j]);
                            movNominaBaseAfectasTmp.Remove(mnc.movNomConceParam[j]);
                        }
                        else
                        {
                            m = new MovNomConceParam();
                            m.paraConcepDeNom = (concepNomDefi.paraConcepDeNom[i]);
                            m.movNomConcep = mnc;
                            m.valor = "0";
                            movNomConceParam.Add(m);
                        }
                    }
                }
                if (movNominaBaseAfectasTmp.Any())
                {
                    for (int j = 0; j < movNominaBaseAfectasTmp.Count; j++)
                    {
                        getSession().Delete(movNominaBaseAfectasTmp[j]);
                    }
                }
            }
            return movNomConceParam;
        }

        private void generarMovimientosAbarca2Meses()
        {
            if (filtroMovimientosNominas.Any())
            {
                if (evaluaPeriodoMovAbarca2Meses(filtroMovimientosNominas[0].periodosNomina))
                {
                    //Este codigo vuelve a generar los movimientos de nomina que no tengan su 2do movimiento ya que el periodo
                    //abarca 2 meses y se tienen que calcular la informacion por cada mes.
                    List<MovNomConcep> listTmp = new List<MovNomConcep>();
                    int mesUno;
                    DateTime fechaInicio = DateTime.Now, fechaFinal = DateTime.Now;
                    fechaInicio = filtroMovimientosNominas[0].periodosNomina.fechaInicial.GetValueOrDefault();
                    fechaFinal = filtroMovimientosNominas[0].periodosNomina.fechaFinal.GetValueOrDefault();
                    mesUno = fechaInicio.Month;
                    int i = 0;
                    while (i < filtroMovimientosNominas.Count)
                    {
                        //                System.out.println("claves concep " + filtroMovimientosNominas.get(i).getConcepNomDefi().getDescripcion() + " " + filtroMovimientosNominas.get(i).getConcepNomDefi().getClave() + " - " + filtroMovimientosNominas.get(i + 1).getConcepNomDefi().getClave());
                        if (!String.Equals(filtroMovimientosNominas[i].concepNomDefi.clave, filtroMovimientosNominas[i + 1].concepNomDefi.clave, StringComparison.OrdinalIgnoreCase))
                        {
                            MovNomConcep newMov = duplicarMovNomConcep(filtroMovimientosNominas[i], filtroMovimientosNominas[i].numero, filtroMovimientosNominas[i].plazasPorEmpleado);
                            if (filtroMovimientosNominas[i].mes.Equals(mesUno))
                            {//Existe el mesUno generare el mesDos
                                fechaInicio = newMov.periodosNomina.fechaFinal.GetValueOrDefault();
                                newMov.mes = fechaInicio.Month;
                                newMov.numMovParticion = 2;
                                filtroMovimientosNominas[i].numMovParticion = 1;
                            }
                            else
                            {//Existe el mesDos generare el mesUno
                                fechaInicio = newMov.periodosNomina.fechaInicial.GetValueOrDefault();
                                newMov.mes = fechaInicio.Month;
                                newMov.numMovParticion = 1;
                                filtroMovimientosNominas[i].numMovParticion = 2;
                            }
                            listTmp.Add(newMov);
                            i++;
                            if (i + 1 >= filtroMovimientosNominas.Count())
                            {
                                newMov = duplicarMovNomConcep(filtroMovimientosNominas[i], filtroMovimientosNominas[i].numero, filtroMovimientosNominas[i].plazasPorEmpleado);
                                if (filtroMovimientosNominas[i].mes.Equals(mesUno))
                                {//Existe el mesUno generare el mesDos
                                    fechaInicio = newMov.periodosNomina.fechaFinal.GetValueOrDefault();
                                    newMov.mes = fechaInicio.Month;
                                    newMov.numMovParticion = 2;
                                    filtroMovimientosNominas[i].numMovParticion = 1;
                                }
                                else
                                {//Existe el mesDos generare el mesUno
                                    fechaInicio = newMov.periodosNomina.fechaInicial.GetValueOrDefault();
                                    newMov.mes = fechaInicio.Month;
                                    newMov.numMovParticion = 1;
                                    filtroMovimientosNominas[i].numMovParticion = 2;
                                }
                                listTmp.Add(newMov);
                                break;
                            }
                        }
                        else
                        {
                            List<Object> clavesMovEliminados = new List<Object>();
                            int x = i + 2;
                            if (filtroMovimientosNominas.Count > x)
                            {
                                while (String.Equals(filtroMovimientosNominas[x].concepNomDefi.clave, filtroMovimientosNominas[i].concepNomDefi.clave, StringComparison.OrdinalIgnoreCase))
                                {
                                    //                            System.out.println("claves concep xx " + filtroMovimientosNominas.get(x).getConcepNomDefi().getClave() + " - " + filtroMovimientosNominas.get(i).getConcepNomDefi().getClave());
                                    clavesMovEliminados.Add(filtroMovimientosNominas[x].id);
                                    filtroMovimientosNominas.RemoveAt(x);
                                }
                            }
                            if (clavesMovEliminados.Any())
                            {
                                eliminarMovimientosNominaBasura(clavesMovEliminados.ToArray());
                                //                            System.out.println("flush 14");
                                if (mensajeResultado.noError != 0)
                                {
                                    break;
                                }
                                getSession().Flush();
                                getSession().Clear();
                            }
                            i = x;//+ 1;
                        }
                    }
                    filtroMovimientosNominas.AddRange(listTmp);
                    ordenarMovimientosNomina();
                }
            }
        }

        private void generarMovimientosAbarca2Meses2()
        {
            if (filtroMovimientosNominasAux.Any())
            {
                if (evaluaPeriodoMovAbarca2Meses(filtroMovimientosNominasAux[0].periodosNomina))
                {
                    //Este codigo vuelve a generar los movimientos de nomina que no tengan su 2do movimiento ya que el periodo
                    //abarca 2 meses y se tienen que calcular la informacion por cada mes.
                    List<MovNomConcep> listTmp = new List<MovNomConcep>();
                    int mesUno;
                    DateTime fechaInicio = DateTime.Now, fechaFinal = DateTime.Now;
                    fechaInicio = filtroMovimientosNominasAux[0].periodosNomina.fechaInicial.GetValueOrDefault();
                    fechaFinal = filtroMovimientosNominasAux[0].periodosNomina.fechaFinal.GetValueOrDefault();
                    mesUno = fechaInicio.Month;
                    int i = 0;
                    while (i < filtroMovimientosNominasAux.Count)
                    {
                        //                System.out.println("claves concep " + filtroMovimientosNominas.get(i).getConcepNomDefi().getDescripcion() + " " + filtroMovimientosNominas.get(i).getConcepNomDefi().getClave() + " - " + filtroMovimientosNominas.get(i + 1).getConcepNomDefi().getClave());
                        if (!String.Equals(filtroMovimientosNominasAux[i].concepNomDefi.clave, filtroMovimientosNominasAux[i + 1].concepNomDefi.clave, StringComparison.OrdinalIgnoreCase))
                        {
                            MovNomConcep newMov = duplicarMovNomConcep(filtroMovimientosNominasAux[i], filtroMovimientosNominasAux[i].numero, filtroMovimientosNominasAux[i].plazasPorEmpleado);
                            if (filtroMovimientosNominasAux[i].mes.Equals(mesUno))
                            {//Existe el mesUno generare el mesDos
                                fechaInicio = newMov.periodosNomina.fechaFinal.GetValueOrDefault();
                                newMov.mes = fechaInicio.Month;
                                newMov.numMovParticion = 2;
                                filtroMovimientosNominasAux[i].numMovParticion = 1;
                            }
                            else
                            {//Existe el mesDos generare el mesUno
                                fechaInicio = newMov.periodosNomina.fechaInicial.GetValueOrDefault();
                                newMov.mes = fechaInicio.Month;
                                newMov.numMovParticion = 1;
                                filtroMovimientosNominasAux[i].numMovParticion = 2;
                            }
                            listTmp.Add(newMov);
                            i++;
                            if (i + 1 >= filtroMovimientosNominasAux.Count())
                            {
                                newMov = duplicarMovNomConcep(filtroMovimientosNominasAux[i], filtroMovimientosNominasAux[i].numero, filtroMovimientosNominasAux[i].plazasPorEmpleado);
                                if (filtroMovimientosNominasAux[i].mes.Equals(mesUno))
                                {//Existe el mesUno generare el mesDos
                                    fechaInicio = newMov.periodosNomina.fechaFinal.GetValueOrDefault();
                                    newMov.mes = fechaInicio.Month;
                                    newMov.numMovParticion = 2;
                                    filtroMovimientosNominasAux[i].numMovParticion = 1;
                                }
                                else
                                {//Existe el mesDos generare el mesUno
                                    fechaInicio = newMov.periodosNomina.fechaInicial.GetValueOrDefault();
                                    newMov.mes = fechaInicio.Month;
                                    newMov.numMovParticion = 1;
                                    filtroMovimientosNominasAux[i].numMovParticion = 2;
                                }
                                listTmp.Add(newMov);
                                break;
                            }
                        }
                        else
                        {
                            List<Object> clavesMovEliminados = new List<Object>();
                            int x = i + 2;
                            if (filtroMovimientosNominasAux.Count > x)
                            {
                                while (String.Equals(filtroMovimientosNominasAux[x].concepNomDefi.clave, filtroMovimientosNominasAux[i].concepNomDefi.clave, StringComparison.OrdinalIgnoreCase))
                                {
                                    //                            System.out.println("claves concep xx " + filtroMovimientosNominas.get(x).getConcepNomDefi().getClave() + " - " + filtroMovimientosNominas.get(i).getConcepNomDefi().getClave());
                                    clavesMovEliminados.Add(filtroMovimientosNominasAux[x].id);
                                    filtroMovimientosNominas.RemoveAt(x);
                                }
                            }
                            if (clavesMovEliminados.Any())
                            {
                                eliminarMovimientosNominaBasura(clavesMovEliminados.ToArray());
                                //                            System.out.println("flush 14");
                                if (mensajeResultado.noError != 0)
                                {
                                    break;
                                }
                                getSession().Flush();
                                getSession().Clear();
                            }
                            i = x;//+ 1;
                        }
                    }
                    filtroMovimientosNominasAux.AddRange(listTmp);
                    ordenarMovimientosNomina2();
                }
            }
        }

        private MovNomConcep duplicarMovNomConcep(MovNomConcep movNomConcepTmp, int? numero, PlazasPorEmpleado plazasPorEmpleado)
        {
            MovNomConcep movNomConcep = new MovNomConcep();
            movNomConcep.empleados = movNomConcepTmp.plazasPorEmpleado.empleados;
            movNomConcep.plazasPorEmpleado = plazasPorEmpleado == null ? movNomConcepTmp.plazasPorEmpleado : plazasPorEmpleado;
            movNomConcep.periodosNomina = movNomConcepTmp.periodosNomina;
            movNomConcep.concepNomDefi = movNomConcepTmp.concepNomDefi;
            movNomConcep.tipoCorrida = movNomConcepTmp.tipoCorrida;
            movNomConcep.tipoNomina = movNomConcepTmp.tipoNomina;
            movNomConcep.centroDeCosto = movNomConcepTmp.centroDeCosto;
            movNomConcep.razonesSociales = movNomConcepTmp.razonesSociales;
            if (movNomConcepTmp.concepNomDefi.baseAfecConcepNom != null)
            {
                movNomConcep.movNomBaseAfecta = creaMovimBaseAfectar(movNomConcepTmp.concepNomDefi.baseAfecConcepNom, movNomConcep);
            }

            if (movNomConcepTmp.concepNomDefi.paraConcepDeNom == null ? false : !movNomConcepTmp.concepNomDefi.paraConcepDeNom.Any() ? false : true)
            {
                movNomConcep.movNomConceParam = creaMovNomConceParam(movNomConcepTmp.concepNomDefi, movNomConcep);
            }
            movNomConcep.fechaCierr = movNomConcepTmp.periodosNomina.fechaCierre;
            movNomConcep.fechaIni = movNomConcepTmp.periodosNomina.fechaInicial;
            movNomConcep.tipoPantalla = tipoPantallaSistema;
            movNomConcep.ordenId = movNomConcepTmp.ordenId;
            movNomConcep.resultado = 0.0;
            movNomConcep.numero = numero;
            movNomConcep.calculado = 0.0;
            movNomConcep.ejercicio = movNomConcepTmp.ejercicio;
            movNomConcep.mes = movNomConcepTmp.mes;
            movNomConcep.numMovParticion = movNomConcepTmp.numMovParticion;
            movNomConcep.uso = movNomConcepTmp.uso;
            return movNomConcep;
        }

        public bool evaluaPeriodoMovAbarca2Meses(PeriodosNomina per)
        {
            DateTime fechaInicio = per.fechaInicial.GetValueOrDefault(), fechaFinal = per.fechaFinal.GetValueOrDefault();
            if (fechaInicio.Month != fechaFinal.Month)
            {
                return true;
            }
            return false;
        }

        private void ordenarMovimientosNomina()
        {
            filtroMovimientosNominas = (from list in filtroMovimientosNominas orderby list.concepNomDefi.prioridadDeCalculo select list).ToList();
        }
        private void ordenarMovimientosNomina2()
        {
            filtroMovimientosNominasAux = (from list in filtroMovimientosNominasAux orderby list.concepNomDefi.prioridadDeCalculo select list).ToList();
        }
        private int eliminarMovimientosNominaBasura(Object[] valores)
        {
            int exito = 1;
            StringBuilder consulta = new StringBuilder();
            //consulta.Append("delete ");
            String[] campo = new String[] { "valores" };
            try
            {
                consulta.Remove(0, consulta.Length);
                //Elimina Bases Afectadas de Movimientos por Conceptos
                consulta.Append("delete MovNomBaseAfecta o where o.movNomConcep.id in(:valores)");
                IQuery q = getSession().CreateQuery(consulta.ToString());
                q.SetParameterList("valores", valores);
                exito = q.ExecuteUpdate();
                ////////            System.out.println("exito " + exito);
                consulta.Remove(0, consulta.Length);

                //Elimina Movimientos Por parametros de Movimientos por Conceptos
                consulta.Append("delete MovNomConceParam o where o.movNomConcep.id in(:valores)");
                q = getSession().CreateQuery(consulta.ToString());
                q.SetParameterList("valores", valores);
                exito = q.ExecuteUpdate();
                ////////            System.out.println("exito " + exito);
                consulta.Remove(0, consulta.Length);

                //Elimina Movimientos ISRRetenidos
                consulta.Append("delete ").Append(typeof(CalculoISR).Name).Append(" o where o.movNomConcep.id in(:valores)");//JSA23
                q = getSession().CreateQuery(consulta.ToString());
                q.SetParameterList("valores", valores);
                exito = q.ExecuteUpdate();
                ////////            System.out.println("exito " + exito);
                consulta.Remove(0, consulta.Length);

                //Elimina Movimientos CalculoIMSS
                consulta.Append("delete ").Append(typeof(CalculoIMSS).Name).Append(" o where o.movNomConcep.id in(:valores)");
                q = getSession().CreateQuery(consulta.ToString());
                q.SetParameterList("valores", valores);
                exito = q.ExecuteUpdate();
                ////////            System.out.println("exito " + exito);
                consulta.Remove(0, consulta.Length);

                //Elimina Movimientos CalculoIMSSPatron
                consulta.Append("delete ").Append(typeof(CalculoIMSSPatron).Name).Append(" o where o.movNomConcep.id in(:valores)");
                q = getSession().CreateQuery(consulta.ToString());
                q.SetParameterList("valores", valores);
                exito = q.ExecuteUpdate();
                ////////            System.out.println("exito " + exito);
                consulta.Remove(0, consulta.Length);

                //Elimina Movimientos por Conceptos
                consulta.Append("delete ").Append(typeof(MovNomConcep).Name).Append(" where id in(:valores)");
                q = getSession().CreateQuery(consulta.ToString());
                q.SetParameterList("valores", valores);
                exito = q.ExecuteUpdate();
                mensajeResultado.error = "";
                mensajeResultado.noError = 0;
                ////////            System.out.println("exito " + exito);
            }

            catch (HibernateException ex)
            {
                mensajeResultado.error = ("eliminarMovimientosNominaBasura " + ex.GetBaseException().Message);
                mensajeResultado.noError = -200;
                //System.err.println(concatena.delete(0, concatena.length()).append(msgError).append("eliminarMovimientosNominaBasura()1_Error: ").append(ex));
                exito = -1;
            }
            return exito;
        }

        private void buscaConceptosTipoAutomatico(String claveTipoCorrida)
        {
            try
            {
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                strQuery.Remove(0, strQuery.Length).Append("SELECT cdn FROM ConcepNomDefi cdn INNER JOIN cdn.conceptoPorTipoCorrida ctc INNER JOIN ctc.tipoCorrida tc  ");
                strWhere.Remove(0, strWhere.Length).Append(" WHERE tc.clave = :claveTipoCorrida AND cdn.activado = true ");
                camposParametro.Add("claveTipoCorrida");
                valoresParametro.Add(claveTipoCorrida);
                //            if (usaNominaAsimiladosAsalarios) {
                //                strWhere.append(" AND cdn.nominaAsimilados = :nominaAsimilados ");
                //                camposParametro.add("nominaAsimilados");
                //                valoresParametro.add(true);
                //            }
                strWhere.Append(" AND cdn.fecha =(SELECT MAX(fecha) FROM ConcepNomDefi c WHERE c.clave= cdn.clave) AND cdn.tipo = :automatico ");
                camposParametro.Add("automatico");
                valoresParametro.Add(Tipo.AUTOMATICO);
                strQuery.Append(strWhere);
                IQuery q = getSession().CreateQuery(strQuery.ToString());
                q = cargarParametrosQuery(q, camposParametro, valoresParametro);
                IList<ConcepNomDefi> aux = q.List<ConcepNomDefi>();
                filtroConceptosNomina.AddRange((List<ConcepNomDefi>)aux);
                if (mensajeResultado.noError == 100)
                {
                    mensajeResultado.noError = 32;
                    return;
                }
                camposParametro = null;
                valoresParametro = null;
            }
            catch (HibernateException ex)
            {
                mensajeResultado.noError = 32;
                //System.err.println(concatena.delete(0, concatena.length()).append(msgError).append("buscaConceptosTipoAutomatico()1_Error: ").append(ex));
            }

        }
        private void buscaConceptosTipoAutomatico2(String claveTipoCorrida)
        {
            try
            {
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                strQuery.Remove(0, strQuery.Length).Append("SELECT cdn FROM ConcepNomDefi cdn INNER JOIN cdn.conceptoPorTipoCorrida ctc INNER JOIN ctc.tipoCorrida tc  ");
                strWhere.Remove(0, strWhere.Length).Append(" WHERE tc.clave = :claveTipoCorrida AND cdn.activado = true ");
                camposParametro.Add("claveTipoCorrida");
                valoresParametro.Add(claveTipoCorrida);
                //            if (usaNominaAsimiladosAsalarios) {
                //                strWhere.append(" AND cdn.nominaAsimilados = :nominaAsimilados ");
                //                camposParametro.add("nominaAsimilados");
                //                valoresParametro.add(true);
                //            }
                strWhere.Append(" AND cdn.fecha =(SELECT MAX(fecha) FROM ConcepNomDefi c WHERE c.clave= cdn.clave) AND cdn.tipo = :automatico ");
                camposParametro.Add("automatico");
                valoresParametro.Add(Tipo.AUTOMATICO);
                strQuery.Append(strWhere);
                IQuery q = getSession().CreateQuery(strQuery.ToString());
                q = cargarParametrosQuery(q, camposParametro, valoresParametro);
                IList<ConcepNomDefi> aux = q.List<ConcepNomDefi>();
                filtroConceptosNominaAux.AddRange((List<ConcepNomDefi>)aux);
                if (mensajeResultado.noError == 100)
                {
                    mensajeResultado.noError = 32;
                    return;
                }
                camposParametro = null;
                valoresParametro = null;
            }
            catch (HibernateException ex)
            {
                mensajeResultado.noError = 32;
                //System.err.println(concatena.delete(0, concatena.length()).append(msgError).append("buscaConceptosTipoAutomatico()1_Error: ").append(ex));
            }

        }
        private int obtenerConceptosUnicos(List<MovNomConcep> filtroMovimientosNominas)
        {
            List<ConcepNomDefi> conceptosUnicos = new List<ConcepNomDefi>();
            for (int i = 0; i < filtroMovimientosNominas.Count; i++)
            {
                if (!conceptosUnicos.Contains(filtroMovimientosNominas[i].concepNomDefi))
                {
                    conceptosUnicos.Add(filtroMovimientosNominas[i].concepNomDefi);
                }
            }
            return conceptosUnicos.Count;
        }


        private List<MovNomConcep> creaMovimientosPlazasConceptosAutomaticos(PlazasPorEmpleado plazaPorEmpleado, PeriodosNomina periodosNominas, String claveTipoCorrida, String claveRazonSocial, String claveCentroCosto)
        {
            List<MovNomConcep> movNomConceptos = new List<MovNomConcep>(0);
            try
            {
                if (filtroConceptosNomina != null)
                {
                    int j;
                    for (j = 0; j < filtroConceptosNomina.Count(); j++)
                    {
                        MovNomConcep movNomConcep;
                        DateTime fechaPeriodo = DateTime.Now;
                        //                    System.out.println("Concep " + filtroConceptosNomina.get(j).getDescripcion() + " formu " + filtroConceptosNomina.get(j).getFormulaConcepto());
                        movNomConcep = creaMovNomConcep(filtroConceptosNomina[j], plazaPorEmpleado, periodosNominas, tipoCorrida, razonesSociales, centroDeCostoMovimiento);
                        movNomConceptos.Add(movNomConcep);
                        if (evaluaPeriodoMovAbarca2Meses(movNomConcep.periodosNomina))
                        {
                            MovNomConcep newMov = MovNomConcep.copiaMovimiento(movNomConcep);
                            fechaPeriodo = newMov.periodosNomina.fechaFinal.GetValueOrDefault();
                            newMov.mes = fechaPeriodo.Month;
                            newMov.numMovParticion = 2;
                            movNomConceptos.Add(newMov);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensajeResultado.noError = 33;
                // System.err.println(concatena.delete(0, concatena.length()).append(msgError).append("creaMovimientosPlazasConceptosAutomaticos()1_Error: ").append(ex));
            }
            return movNomConceptos;
        }

        private Object[] obtenerMovimientosNominaCreditoAhorro(List<CreditoAhorro> listCreditoAhorro)
        {

            object[] listMovTmp = new object[5];
            List<MovNomConcep> listMovNomConcepCreditosAhorro = new List<MovNomConcep>();
            List<MovNomConcep> listMovNomConcepCreditosAhorroDescuentoActivo = new List<MovNomConcep>();
            List<MovNomConcep> listMovNomConcepFormulaDeducCreditos = new List<MovNomConcep>();
            List<MovNomConcep> listMovNomConcepFormulaDeducAhorros = new List<MovNomConcep>();
            int i, z, j;
            bool asignadoACredito;
            for (i = 0; i < listCreditoAhorro.Count; i++)
            {
                z = 0;
                while (z < filtroMovimientosNominas.Count)
                {
                    //                System.out.println("concepto credito " + listCreditoAhorro.get(i).getConcepNomiDefin().getClave() + " concepto mov " + filtroMovimientosNominas.get(z).getConcepNomDefi().getClave());
                    if (String.Equals(listCreditoAhorro[i].concepNomDefi_concepNomiDefin_ID.clave, filtroMovimientosNominas[z].concepNomDefi.clave, StringComparison.OrdinalIgnoreCase))
                    {
                        listMovNomConcepCreditosAhorro.Add(filtroMovimientosNominas[z]);
                        filtroMovimientosNominas.RemoveAt(z);
                    }
                    else if (listCreditoAhorro[i].activarManejoDescuento)
                    {
                        if (String.Equals(listCreditoAhorro[i].concepNomDefi_cNDescuento_ID.clave, filtroMovimientosNominas[z].concepNomDefi.clave, StringComparison.OrdinalIgnoreCase))
                        {
                            if (!String.Equals(listCreditoAhorro[i].concepNomDefi_cNDescuento_ID.clave, listCreditoAhorro[i].concepNomDefi_concepNomiDefin_ID.clave, StringComparison.OrdinalIgnoreCase))
                            {
                                listMovNomConcepCreditosAhorroDescuentoActivo.Add(filtroMovimientosNominas[z]);
                                filtroMovimientosNominas.RemoveAt(z);
                            }
                            else
                            {
                                z++;
                            }
                        }
                        else
                        {
                            z++;
                        }
                    }
                    else if (filtroMovimientosNominas[z].concepNomDefi.formulaConcepto != null)
                    {
                        if (filtroMovimientosNominas[z].concepNomDefi.formulaConcepto.ToUpper().StartsWith("DEDUCCREDITOS"))
                        {
                            asignadoACredito = false;
                            for (j = 0; j < listCreditoAhorro.Count(); j++)
                            {
                                if (String.Equals(listCreditoAhorro[j].concepNomDefi_concepNomiDefin_ID.clave, filtroMovimientosNominas[z].concepNomDefi.clave, StringComparison.OrdinalIgnoreCase))
                                {
                                    asignadoACredito = true;
                                    break;
                                }
                            }
                            if (!asignadoACredito)
                            {
                                listMovNomConcepFormulaDeducCreditos.Add(filtroMovimientosNominas[z]);
                                filtroMovimientosNominas.RemoveAt(z);
                            }
                            else
                            {
                                z++;
                            }
                        }
                        else if (filtroMovimientosNominas[z].concepNomDefi.formulaConcepto.ToUpper().StartsWith("DEDUCAHORROS"))
                        {
                            asignadoACredito = false;
                            for (j = 0; j < listCreditoAhorro.Count(); j++)
                            {
                                if (String.Equals(listCreditoAhorro[j].concepNomDefi_concepNomiDefin_ID.clave, filtroMovimientosNominas[z].concepNomDefi.clave, StringComparison.OrdinalIgnoreCase))
                                {
                                    asignadoACredito = true;
                                    break;
                                }
                            }
                            if (!asignadoACredito)
                            {
                                listMovNomConcepFormulaDeducAhorros.Add(filtroMovimientosNominas[z]);
                                filtroMovimientosNominas.RemoveAt(z);
                            }
                            else
                            {
                                z++;
                            }
                        }
                        else
                        {
                            z++;
                        }
                    }
                    else
                    {
                        z++;
                    }
                }
            }
            listMovTmp[0] = listMovNomConcepCreditosAhorro;
            listMovTmp[1] = listMovNomConcepCreditosAhorroDescuentoActivo;
            listMovTmp[2] = listMovNomConcepFormulaDeducCreditos;
            listMovTmp[3] = listMovNomConcepFormulaDeducAhorros;


            return listMovTmp;

        }

        private void obtenerMovimientosNominaISRACargoYSubsidioAlEmpleado()
        {
            int i;
            bool isISRACargo, isSubsEmpleoCalculado;
            for (i = 0; i < filtroMovimientosNominas.Count; i++)
            {
                isISRACargo = isConceptoEspecial(5, filtroMovimientosNominas[i].concepNomDefi.formulaConcepto);
                isSubsEmpleoCalculado = isConceptoEspecial(6, filtroMovimientosNominas[i].concepNomDefi.formulaConcepto);
                if (isISRACargo)
                {
                    listMovNomConcepISRCARGO.Add(filtroMovimientosNominas[i]);
                }
                else if (isSubsEmpleoCalculado)
                {
                    listMovNomConcepSUBSIDIOALEMPLEO.Add(filtroMovimientosNominas[i]);
                }
            }
        }

        private bool isConceptoEspecial(int tipoConceptoEspecial, string formula)
        {
            bool isConceptoEspecial = false;
            if (formula == null)
            {
                formula = "";
            }
            switch (tipoConceptoEspecial)
            {
                case 1:
                    if (formula.Contains("CalculoISR"))
                    {
                        isConceptoEspecial = true;
                    }
                    break;
                case 2:
                    if (formula.Contains("CalculoIMSS"))
                    {
                        isConceptoEspecial = true;
                    }
                    break;
                case 3:
                    if (formula.Contains("ISRSubsidio"))
                    {//ConceptoSubsidioEmpleado
                        isConceptoEspecial = true;
                    }
                    break;
                case 4:
                    if (formula.Contains("CalculoIMSSPatronal"))
                    {
                        isConceptoEspecial = true;
                    }
                    break;
                case 5:
                    if (formula.Contains("ISRACargo"))
                    {
                        isConceptoEspecial = true;
                    }
                    break;
                case 6:
                    if (formula.Contains("SubsEmpleoCalculado"))
                    {
                        isConceptoEspecial = true;
                    }
                    break;
            }

            return isConceptoEspecial;
        }

        private void removerConceptosAguinaldo(string claveTipoCorrida)
        {
            try
            {
                MovimientosNominaHDAO movimientosNominaDAO = new MovimientosNominaHDAO();
                movimientosNominaDAO.setSession(getSession());
                if (!isCalculoAgui)
                {
                    int ag = 0;
                    List<Object> idsMovDelete = new List<Object>();
                    List<MovNomConcep> listTempMov = new List<MovNomConcep>();
                    while (ag < filtroMovimientosNominas.Count)
                    {
                        string formula = filtroMovimientosNominas[ag].concepNomDefi.formulaConcepto;
                        if (formula == null)
                        {
                            formula = "";
                        }
                        if (String.Equals(claveTipoCorrida, "AGUI", StringComparison.OrdinalIgnoreCase))
                        {
                            if (filtroMovimientosNominas[ag].id != 0) //checar null a lo mejor dif 0
                            {
                                idsMovDelete.Add(filtroMovimientosNominas[ag].id);
                            }
                            filtroMovimientosNominas.Remove(filtroMovimientosNominas[ag]);
                        }
                        else if (formula.ToUpper().Contains("ImporteAguinaldo".ToUpper()))
                        {
                            if (filtroMovimientosNominas[ag].id != 0)
                            {
                                idsMovDelete.Add(filtroMovimientosNominas[ag].id);
                            }
                            filtroMovimientosNominas.Remove(filtroMovimientosNominas[ag]);
                        }
                        else if ((formula.ToUpper().Equals("IngresoVacaciones".ToUpper())
                              || formula.ToUpper().Equals("IngresoPrimaVacacional".ToUpper())) && salarioVacaciones == ManejoSalarioVacaciones.SALARIOANIVERSARIO)
                        {
                            int ejercio = 0;
                            VacacionesDisfrutadas vac = new VacacionesDisfrutadas();

                            MovNomConcep movDuplicado = null;
                            int numDupli = 1;
                            for (int j = 0; j < vacacionesAplicacionStatus.Count; j++)
                            {
                                if (j == 0)
                                {
                                    ejercio = vacacionesAplicacionStatus[j].vacacionesDevengadas.ejercicio.GetValueOrDefault();
                                    vac = vacacionesAplicacionStatus[j].vacacionesDisfrutadas;
                                    filtroMovimientosNominas[ag].vacacionesAplicacion = vacacionesAplicacionStatus[j];
                                }
                                else if (ejercio != vacacionesAplicacionStatus[j].vacacionesDevengadas.ejercicio)
                                {
                                    if (vac.id == vacacionesAplicacionStatus[j].vacacionesDisfrutadas.id)
                                    {
                                        movDuplicado = new MovNomConcep();
                                        numDupli++;
                                        movDuplicado = duplicarMovNomConcep(filtroMovimientosNominas[ag], filtroMovimientosNominas[ag].numero, filtroMovimientosNominas[ag].plazasPorEmpleado);

                                        ejercio = vacacionesAplicacionStatus[j].vacacionesDevengadas.ejercicio.GetValueOrDefault();
                                        vac = vacacionesAplicacionStatus[j].vacacionesDisfrutadas;
                                        movDuplicado.vacacionesAplicacion = vacacionesAplicacionStatus[j];
                                        movDuplicado.numMovParticion = numDupli;
                                        movDuplicado.numero = numDupli;
                                        filtroMovimientosNominas[ag].numMovParticion = 1;
                                    }
                                }
                                if (movDuplicado != null)
                                {
                                    listTempMov.Add(movDuplicado);
                                }

                            }
                            if (mensajeResultado.noError != 0)
                            {
                                return;
                            }
                            ag++;
                        }
                        else
                        {
                            ag++;
                        }
                    }
                    if (listTempMov.Count > 0)
                    {
                        for (int j = 0; j < listTempMov.Count; j++)
                        {
                            filtroMovimientosNominas.Add(listTempMov[j]);
                        }
                    }
                    if (idsMovDelete.Count > 0)
                    {
                        movimientosNominaDAO.deleteListQueryMov(typeof(MovNomConcep).Name, "id", idsMovDelete.ToArray(), null, null, null, true);
                        if (mensajeResultado.noError != 0)
                        {
                            return;
                        }
                    }
                }
                else
                {
                    int ag;
                    bool existeConceptoAguinaldo = false;
                    for (ag = 0; ag < filtroMovimientosNominas.Count; ag++)
                    {
                        if (filtroMovimientosNominas[ag].concepNomDefi.formulaConcepto.ToUpper().Contains("ImporteAguinaldo".ToUpper())
                                | filtroMovimientosNominas[ag].concepNomDefi.formulaConcepto.ToUpper().Contains("AguinaldoPagos".ToUpper()))
                        {
                            existeConceptoAguinaldo = true;
                            break;
                        }
                    }
                    if (!existeConceptoAguinaldo)
                    {
                        isCalculoAgui = existeConceptoAguinaldo;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("removerConceptosAguinaldo()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
        }

        private MovNomConcep saveOrUpdateOrDeleteMovimientosNomina(MovNomConcep movimientosNomina, bool omitirMovimiento, bool isISRSubsidio)
        {
            //        System.out.println("Concepto " + movimientosNomina.getConcepNomDefi().getDescripcion() + " Resultado " + movimientosNomina.getResultado() + " omitirMovimiento " + omitirMovimiento);
            try
            {
                if (movimientosNomina.resultado == 0 & !isISRSubsidio)
                {
                    if (movimientosNomina.id > 0)
                    {
                        eliminarMovimientosNominaBasura(new Object[] { movimientosNomina.id });
                        //                    System.out.println("flush 12");
                        getSession().Flush();
                    }
                }
                else
                {
                    if (!omitirMovimiento)
                    {
                        cantidadSaveUpdate++;
                        getSession().SaveOrUpdate(movimientosNomina);
                    }
                    if (cantidadSaveUpdate % cantidadFlush == 0 & cantidadSaveUpdate > 0)
                    {
                        //                    System.out.println("flush 13");
                        getSession().Flush();
                        getSession().Clear();
                        //                    System.out.println("clear()");
                    }
                }
            }
            catch (Exception ex)
            {
                mensajeResultado.error = "ERROR saveOrUpdateOrDeleteMovimientosNomina " + ex.GetBaseException().Message;
                mensajeResultado.noError = -101;
                //System.err.println(concatena.delete(0, concatena.length()).append(msgError).append("saveOrUpdateOrDeleteMovimientosNomina()1_Error: ").append(ex));
            }
            //        System.out.println("--------------------------------FINAL saveOrUpdateOrDeleteMovimientosNomina");
            return movimientosNomina;
        }

        private List<PlazasPorEmpleadosMov> obtenerMinimoPlazasPorEmpleadosMovDentroPeriodo(String claveTipoCorrida, DateTime fechaInicioPeriodo, DateTime fechaFinPeriodo, PlazasPorEmpleadosMov plazasPorEmpleadosMov)
        {
            IList<PlazasPorEmpleadosMov> filtroPlazasPorEmpleadosMov = null;
            try
            {
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                strQuery.Remove(0, strQuery.Length).Append("SELECT pmov ");
                strQuery.Append(" FROM PlazasPorEmpleadosMov pmov ");
                strQuery.Append(" INNER JOIN pmov.plazasPorEmpleado pemp");
                strWhere.Remove(0, strWhere.Length).Append(" WHERE ");

                camposParametro.Add("fechaInicialPeriodo");
                if (string.Equals(claveTipoCorrida, "FIN", StringComparison.OrdinalIgnoreCase))
                {
                    valoresParametro.Add(fechaBajaFiniq.GetValueOrDefault().Date);
                }
                else
                {
                    valoresParametro.Add(fechaInicioPeriodo);
                }
                camposParametro.Add("fechaFinalPeriodo");
                valoresParametro.Add(fechaFinPeriodo);

                strWhere.Append(" pmov.id != :idPlazaPorEmpleadoMov");
                camposParametro.Add("idPlazaPorEmpleadoMov");
                valoresParametro.Add(plazasPorEmpleadosMov.id);

                strWhere.Append(" AND pemp.id = :IdPlazasPorEmpleado");
                camposParametro.Add("IdPlazasPorEmpleado");
                valoresParametro.Add(plazasPorEmpleadosMov.plazasPorEmpleado.id);
                strWhere.Append(" AND pmov.fechaInicial = (SELECT MIN(pmov.fechaInicial) FROM PlazasPorEmpleadosMov pmov INNER JOIN pmov.plazasPorEmpleado pemp ");
                strWhere.Append(" WHERE  (pmov.fechaInicial BETWEEN :fechaInicialPeriodo and :fechaFinalPeriodo) AND pemp.fechaFinal >= :fechaInicialPeriodo");
                strWhere.Append(" AND  pemp.fechaFinal >= :fechaInicialPeriodo and pmov.fechaInicial <= :fechaFinalPeriodo");
                strWhere.Append(" AND pmov.id != :idPlazaPorEmpleadoMov");
                strWhere.Append(" AND pemp.id = :IdPlazasPorEmpleado )");
                strWhere.Append(" AND pmov.cambioSalarioPor = :salarioPor ");
                camposParametro.Add("salarioPor");
                valoresParametro.Add(true);
                strQuery.Append(strWhere.ToString());
                IQuery q = getSession().CreateQuery(strQuery.ToString());
                q = cargarParametrosQuery(q, camposParametro, valoresParametro);
                filtroPlazasPorEmpleadosMov = q.List<PlazasPorEmpleadosMov>();
                camposParametro = null;
                valoresParametro = null;
            }
            catch (HibernateException ex)
            {
                mensajeResultado.error = (ex.GetBaseException().Message);
                mensajeResultado.noError = (27);
                //System.err.println(concatena.delete(0, concatena.length()).append(msgError).append("obtenerPlazasPorEmpleados()1_Error: ").append(ex));
            }
            return (List<PlazasPorEmpleadosMov>)filtroPlazasPorEmpleadosMov;
        }

        private List<PlazasPorEmpleadosMov> obtenerPlazasPorEmpleadosMovDentroPeriodo(String claveTipoCorrida, DateTime fechaInicioPeriodo, DateTime fechaFinPeriodo, Object[] plazasPorEmpleadosMov)
        {
            IList<PlazasPorEmpleadosMov> filtroPlazasPorEmpleadosMov = null;
            try
            {
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                strQuery.Remove(0, strQuery.Length).Append("SELECT pmov ");
                strQuery.Append(" FROM PlazasPorEmpleadosMov pmov ");
                strQuery.Append(" INNER JOIN pmov.plazasPorEmpleado pemp");
                strWhere.Remove(0, strWhere.Length).Append(" WHERE ");
                strWhere.Append("  (pmov.fechaInicial BETWEEN :fechaInicialPeriodo and :fechaFinalPeriodo) AND pemp.fechaFinal >= :fechaInicialPeriodo");
                strWhere.Append(" AND  pemp.fechaFinal >= :fechaInicialPeriodo and pmov.fechaInicial <= :fechaFinalPeriodo");
                camposParametro.Add("fechaInicialPeriodo");
                if (string.Equals(claveTipoCorrida, "FIN", StringComparison.OrdinalIgnoreCase))
                {
                    valoresParametro.Add(fechaBajaFiniq.GetValueOrDefault().Date);
                }
                else
                {
                    valoresParametro.Add(fechaInicioPeriodo);
                }
                camposParametro.Add("fechaFinalPeriodo");
                valoresParametro.Add(fechaFinPeriodo);

                strWhere.Append(" AND pmov.id not in (:idPlazaPorEmpleadoMov)");
                camposParametro.Add("idPlazaPorEmpleadoMov");
                List<Object> listIdPlazaPorEmpleadoMov = new List<Object>(), listIdPlazasPorEmpleado = new List<Object>();
                int i;
                for (i = 0; i < plazasPorEmpleadosMov.Length; i++)
                {
                    listIdPlazaPorEmpleadoMov.Add(((PlazasPorEmpleadosMov)plazasPorEmpleadosMov[i]).id);
                    listIdPlazasPorEmpleado.Add(((PlazasPorEmpleadosMov)plazasPorEmpleadosMov[i]).plazasPorEmpleado.id);
                }
                valoresParametro.Add(listIdPlazaPorEmpleadoMov.ToArray());
                strWhere.Append(" AND pemp.id in( :IdPlazasPorEmpleado)");
                camposParametro.Add("IdPlazasPorEmpleado");
                valoresParametro.Add(listIdPlazasPorEmpleado.ToArray());
                strWhere.Append(" AND pmov.cambioSalarioPor = :salarioPor ");
                camposParametro.Add("salarioPor");
                valoresParametro.Add(true);
                strQuery.Append(strWhere.ToString());
                IQuery q = getSession().CreateQuery(strQuery.ToString());
                q = cargarParametrosQuery(q, camposParametro, valoresParametro);
                filtroPlazasPorEmpleadosMov = q.List<PlazasPorEmpleadosMov>();
                camposParametro = null;
                valoresParametro = null;

            }
            catch (HibernateException ex)
            {
                mensajeResultado.error = ex.GetBaseException().Message;
                mensajeResultado.noError = 27;
                // System.err.println(concatena.delete(0, concatena.length()).append(msgError).append("obtenerPlazasPorEmpleadosMovDentroPeriodo()1_Error: ").append(ex));
            }
            return (List<PlazasPorEmpleadosMov>)filtroPlazasPorEmpleadosMov;
        }

        private List<PlazasPorEmpleadosMov> obtenerAnteriorPlazasPorEmpleadosMov(String claveTipoCorrida, DateTime fechaInicioPeriodo, PlazasPorEmpleadosMov plazasPorEmpleadosMov)
        {
            IList<PlazasPorEmpleadosMov> filtroPlazasPorEmpleadosMov = null;
            try
            {
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                strQuery.Remove(0, strQuery.Length).Append("SELECT pmov ");
                strQuery.Append(" FROM PlazasPorEmpleadosMov pmov ");
                strQuery.Append(" INNER JOIN pmov.plazasPorEmpleado pemp");
                strWhere.Remove(0, strWhere.Length).Append(" WHERE ");

                camposParametro.Add("fechaInicialPeriodo");
                valoresParametro.Add(plazasPorEmpleadosMov.fechaInicial);

                strWhere.Append(" pmov.id != :idPlazaPorEmpleadoMov");
                camposParametro.Add("idPlazaPorEmpleadoMov");
                valoresParametro.Add(plazasPorEmpleadosMov.id);

                strWhere.Append(" AND pemp.id = :IdPlazasPorEmpleado");
                camposParametro.Add("IdPlazasPorEmpleado");
                valoresParametro.Add(plazasPorEmpleadosMov.plazasPorEmpleado.id);
                strWhere.Append(" AND pmov.fechaInicial = (SELECT MAX(pmovx.fechaInicial) FROM PlazasPorEmpleadosMov pmovx INNER JOIN pmovx.plazasPorEmpleado pempx ");
                strWhere.Append(" WHERE  pempx.fechaFinal >= :fechaInicialPeriodo");
                strWhere.Append(" AND  pmovx.fechaInicial <= :fechaInicialPeriodo");
                strWhere.Append(" AND pmovx.id != :idPlazaPorEmpleadoMov");
                strWhere.Append(" AND pempx.id = :IdPlazasPorEmpleado )");

                strQuery.Append(strWhere.ToString());
                IQuery q = getSession().CreateQuery(strQuery.ToString());
                q = cargarParametrosQuery(q, camposParametro, valoresParametro);
                filtroPlazasPorEmpleadosMov = q.List<PlazasPorEmpleadosMov>();

                camposParametro = null;
                valoresParametro = null;

            }
            catch (HibernateException ex)
            {
                mensajeResultado.error = ex.GetBaseException().Message;
                mensajeResultado.noError = 27;

            }
            return (List<PlazasPorEmpleadosMov>)filtroPlazasPorEmpleadosMov;
        }

        //Usando PlazasPorEmpleado
        private List<PlazasPorEmpleadosMov> obtenerPlazasPorEmpleadosMovRestantes(String claveRazonSocial, decimal idRegPatronal, DateTime fechaInicioPeriodo, DateTime fechaFinPeriodo, PlazasPorEmpleadosMov plazasPorEmpleadosMovEjecutandose)
        {
            IList<PlazasPorEmpleadosMov> filtroPlazasPorEmpleadosMov = null, filtroPlazasPorEmpleadosMovTmp;
            try
            {
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                strQuery.Remove(0, strQuery.Length).Append("SELECT pMov ");
                strQuery.Append("from PlazasPorEmpleadosMov pMov inner join pMov.plazasPorEmpleado pemp where pMov.id IN ");
                strQuery.Append(" (Select MAX(pMovX.id) from PlazasPorEmpleadosMov pMovX ");
                strQuery.Append("INNER JOIN pMovX.plazasPorEmpleado pe ");
                strQuery.Append("INNER JOIN pe.empleados em ");
                strWhere.Remove(0, strWhere.Length).Append(" WHERE ");
                strQuery.Append(" LEFT OUTER JOIN pe.razonesSociales rs ");
                strWhere.Append(" rs.clave = :claveRazonSocial ");
                camposParametro.Add("claveRazonSocial");
                valoresParametro.Add(claveRazonSocial);
                strQuery.Append(" LEFT OUTER JOIN pe.registroPatronal rp ");
                strWhere.Append(" AND rp.id = :claveRegPatronal ");
                camposParametro.Add("claveRegPatronal");
                valoresParametro.Add(idRegPatronal);

                strWhere.Append(" AND ((pMovX.fechaInicial <= :fechaInicialPeriodo ) OR (pMovX.fechaInicial between :fechaInicialPeriodo AND :fechaFinalPeriodo ))  ");
                strWhere.Append(" AND ((pMovX.plazasPorEmpleado.fechaFinal >= :fechaFinalPeriodo ) OR   (pMovX.plazasPorEmpleado.fechaFinal between :fechaInicialPeriodo AND :fechaFinalPeriodo )) ");
                camposParametro.Add("fechaInicialPeriodo");
                if (string.Equals(tipoCorrida.clave, "FIN", StringComparison.OrdinalIgnoreCase))
                {
                    valoresParametro.Add(fechaBajaFiniq);
                }
                else
                {
                    valoresParametro.Add(fechaInicioPeriodo);
                }
                camposParametro.Add("fechaFinalPeriodo");
                valoresParametro.Add(fechaFinPeriodo);
                strWhere.Append(" AND em in (:listEmpleado)");
                camposParametro.Add("listEmpleado");
                valoresParametro.Add(new Empleados[] { plazasPorEmpleadosMovEjecutandose.plazasPorEmpleado.empleados });
                strWhere.Append(" GROUP BY pe.clave) ");
                if (claveRazonSocial.Any())
                {
                    strWhere.Append(" AND pemp.id not in (Select px.reIngreso.id from PlazasPorEmpleado px Where px.razonesSociales.clave = :claveRazonSocial AND px.reIngreso != null ) ");
                }
                else
                {
                    strWhere.Append(" AND pemp.id not in (Select px.reIngreso.id from PlazasPorEmpleado px Where px.reIngreso != null ) ");
                }
                strWhere.Append(" AND pemp.empleados.clave not in ( ");
                strWhere.Append("select o.plazaPorEmpleadoMov.plazasPorEmpleado.empleados.clave from CFDIEmpleado o where o.razonesSociales.clave = :claveRazonSocial  ");
                if (tipoCorrida != null)
                {
                    strWhere.Append(" and o.tipoCorrida.clave = :claveTipoCorrida ");
                    camposParametro.Add("claveTipoCorrida");
                    valoresParametro.Add(tipoCorrida.clave);
                }
                if (periodosNomina != null)
                {
                    strWhere.Append(" and o.periodosNomina.id = :idPeriodoNomina ");
                    camposParametro.Add("idPeriodoNomina");
                    valoresParametro.Add(periodosNomina.id);
                }
                strWhere.Append(" AND o.plazaPorEmpleadoMov.plazasPorEmpleado.empleados in (:listEmpleado)");
                strWhere.Append(" AND o.cfdiRecibo.statusTimbrado = :statusTimbre ");
                camposParametro.Add("statusTimbre");
                valoresParametro.Add(StatusTimbrado.TIMBRADO);
                strWhere.Append(" ) ");
                if (string.Equals(tipoCorrida.clave, "FIN", StringComparison.OrdinalIgnoreCase))
                {
                    strWhere.Append(" AND pemp.clave IN (SELECT flp.plazasPorEmpleado.clave FROM FiniqLiquidPlazas flp WHERE pemp.clave = flp.plazasPorEmpleado.clave ");
                    strWhere.Append(" AND flp.incluir = true) ");
                }
                strWhere.Append(" AND pMov.id != :plazaPorEmpleadoMov_ID");
                camposParametro.Add("plazaPorEmpleadoMov_ID");
                valoresParametro.Add(plazasPorEmpleadosMovEjecutandose.id);
                strWhere.Append(" ORDER BY pemp.empleados.clave, pemp.clave");
                strQuery.Append(strWhere);
                IQuery q = getSession().CreateQuery(strQuery.ToString());
                q = cargarParametrosQuery(q, camposParametro, valoresParametro);
                filtroPlazasPorEmpleadosMov = q.List<PlazasPorEmpleadosMov>();
                //filtroPlazasPorEmpleadosMov = (List<PlazasPorEmpleadosMov>)ejecutaQueryList(strQuery.toString(), camposParametro.toArray(new String[] { }), valoresParametro.toArray(), 0);
                camposParametro = null;
                valoresParametro = null;
            }
            catch (HibernateException ex)
            {
                mensajeResultado.error = ex.GetBaseException().Message;
                mensajeResultado.noError = 27;

            }
            return (List<PlazasPorEmpleadosMov>)filtroPlazasPorEmpleadosMov;
        }

        private List<Object> obtenerModificacionesDePlazasPorEmpleadoMov(bool configuracionSueldoDiarioVigente, bool configuracionPercepcion_Plaza, bool configuracionPercepcion_Plaza_Vigente, MovNomConcep movimientosNomina, PlazasPorEmpleadosMov plazasPorEmpleadosMov)
        {
            IList<Object> listObject = new List<Object>();
            List<MovNomConcep> listMovNomConcepPromocional = new List<MovNomConcep>();
            List<PlazasPorEmpleadosMov> listPlazasPorEmpleadosMovOficial = new List<PlazasPorEmpleadosMov>();
            int i;
            bool continuar = false;
            if (configuracionSueldoDiarioVigente)
            {
                #region Programacion para cuando se aplica modificaciones salariales, aqui se obtienen los movimientos del empleado dentro del periodo
                listMovNomConcepPromocional.Add(movimientosNomina);
                DateTime fechaIni = DateTime.Now, fechaFin = DateTime.Now;
                fechaIni = (DateTime)valoresConceptosEmpleados[parametroFechaInicial];
                fechaFin = (DateTime)valoresConceptosEmpleados[parametroFechaFinal];

                if (periodosNomina != null)
                {
                    fechaIni = periodosNomina.fechaInicial.GetValueOrDefault();
                    fechaFin = periodosNomina.fechaFinal.GetValueOrDefault();
                }
                List<PlazasPorEmpleadosMov> listPlazasPorEmpleadosMovTmp = new List<PlazasPorEmpleadosMov>();
                listPlazasPorEmpleadosMovTmp.AddRange(obtenerPlazasPorEmpleadosMovDentroPeriodo(tipoCorrida.clave, periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault(), new Object[] { plazasPorEmpleadosMov }));
                List<MovNomConcep> listMovNomTmp = new List<MovNomConcep>();
                for (i = 0; i < filtroMovimientosNominas.Count; i++)
                {
                    if (String.Equals(movimientosNomina.concepNomDefi.clave, filtroMovimientosNominas[i].concepNomDefi.clave, StringComparison.OrdinalIgnoreCase))
                    {
                        continuar = false;
                        if (movimientosNomina.id != filtroMovimientosNominas[i].id)
                        {
                            continuar = true;
                            if (movimientosNomina.plazasPorEmpleado.id == filtroMovimientosNominas[i].plazasPorEmpleado.id
                                    & movimientosNomina.numero == filtroMovimientosNominas[i].numero)
                            {
                                if (movimientosNomina.mes != filtroMovimientosNominas[i].mes)
                                {
                                    continuar = false;
                                }
                            }
                        }
                        if (continuar)
                        {
                            listMovNomTmp.Add(filtroMovimientosNominas[i]);
                        }
                    }
                }
                filtroMovimientosNominas = filtroMovimientosNominas.Except(listMovNomTmp).ToList();
                listMovNomConcepPromocional.AddRange(listMovNomTmp);
                int numero = 1;
                listMovNomConcepPromocional[0].numero = 1;
                if (listMovNomConcepPromocional.Count < listPlazasPorEmpleadosMovTmp.Count + 1)
                {
                    for (i = 0; i < listPlazasPorEmpleadosMovTmp.Count; i++)
                    {
                        numero++;
                        listMovNomConcepPromocional.Add(duplicarMovNomConcep(listMovNomConcepPromocional[0], numero, listPlazasPorEmpleadosMovTmp[i].plazasPorEmpleado));
                        if (evaluaPeriodoMovAbarca2Meses(periodosNomina))
                        {
                            int mesUno;
                            DateTime fechaPromocion, fechaInicio, fechaFinal;
                            fechaInicio = filtroMovimientosNominas[0].periodosNomina.fechaInicial.GetValueOrDefault();
                            fechaFinal = filtroMovimientosNominas[0].periodosNomina.fechaFinal.GetValueOrDefault();
                            fechaPromocion = listPlazasPorEmpleadosMovTmp[i].fechaInicial.GetValueOrDefault();
                            mesUno = fechaInicio.Month;
                            listMovNomConcepPromocional[listMovNomConcepPromocional.Count - 1].ejercicio = periodosNomina.año;
                            listMovNomConcepPromocional[listMovNomConcepPromocional.Count - 1].mes = fechaPromocion.Month;
                            if (mesUno == fechaPromocion.Month)
                            {
                                listMovNomConcepPromocional[listMovNomConcepPromocional.Count - 1].numMovParticion = 1;
                            }
                            else
                            {
                                listMovNomConcepPromocional[listMovNomConcepPromocional.Count - 1].numMovParticion = 2;
                            }
                        }
                    }
                }
                listPlazasPorEmpleadosMovOficial.AddRange(listPlazasPorEmpleadosMovTmp);
                listPlazasPorEmpleadosMovOficial.Add(plazasPorEmpleadosMov);
                if (listPlazasPorEmpleadosMovOficial[0].fechaInicial.GetValueOrDefault().CompareTo(fechaIni) > 0)
                {
                    bool agregarMovNomConcepPromocional = true;
                    if (evaluaPeriodoMovAbarca2Meses(periodosNomina))
                    {
                        for (i = 0; i < listPlazasPorEmpleadosMovOficial.Count; i++)
                        {
                            if (i + 1 < listPlazasPorEmpleadosMovOficial.Count)
                            {
                                DateTime fechaUno, fechaDos;
                                fechaUno = listPlazasPorEmpleadosMovOficial[i].fechaInicial.GetValueOrDefault();
                                fechaDos = listPlazasPorEmpleadosMovOficial[i + 1].fechaInicial.GetValueOrDefault();
                                if (fechaUno.Month != fechaDos.Month)
                                {
                                    fechaUno = fechaDos;
                                    fechaUno.AddDays(1);//Obtener dia 1 del mes.
                                    if (fechaDos.CompareTo(fechaUno) > 0)
                                    {
                                        if (listMovNomConcepPromocional.Count > listPlazasPorEmpleadosMovOficial.Count)
                                        {

                                        }
                                    }
                                }

                            }
                        }
                    }
                    else if (listPlazasPorEmpleadosMovOficial.Count + 1 == listMovNomConcepPromocional.Count)
                    {
                        agregarMovNomConcepPromocional = false;
                    }

                    listPlazasPorEmpleadosMovOficial.InsertRange(0, obtenerAnteriorPlazasPorEmpleadosMov(tipoCorrida.clave, periodosNomina.fechaInicial.GetValueOrDefault(), listPlazasPorEmpleadosMovOficial[0]));
                    if (agregarMovNomConcepPromocional)
                    {
                        listMovNomConcepPromocional.Add(duplicarMovNomConcep(listMovNomConcepPromocional[0], listMovNomConcepPromocional[listMovNomConcepPromocional.Count - 1].numero.GetValueOrDefault() + 1, listPlazasPorEmpleadosMovOficial[0].plazasPorEmpleado));
                    }
                }
                #endregion
            }
            else if (configuracionPercepcion_Plaza)
            {
                #region Programacion para cuando se aplica modificaciones salariales, aqui se obtienen los movimientos del empleado dentro del periodo
                DateTime fechaIni = DateTime.Now, fechaFin = DateTime.Now;
                fechaIni = (DateTime)valoresConceptosEmpleados[parametroFechaInicial];
                fechaFin = (DateTime)valoresConceptosEmpleados[parametroFechaFinal];

                if (periodosNomina != null)
                {
                    fechaIni = periodosNomina.fechaInicial.GetValueOrDefault();
                    fechaFin = periodosNomina.fechaFinal.GetValueOrDefault();
                }
                List<PlazasPorEmpleadosMov> listPlazasPorEmpleadosMovTmp = new List<PlazasPorEmpleadosMov>();
                listPlazasPorEmpleadosMovTmp.AddRange(obtenerPlazasPorEmpleadosMovRestantes(plazasPorEmpleadosMov.plazasPorEmpleado.razonesSociales.clave, plazasPorEmpleadosMov.plazasPorEmpleado.registroPatronal.id, periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault(), plazasPorEmpleadosMov));
                List<MovNomConcep> listMovNomTmp = new List<MovNomConcep>();
                for (i = 0; i < filtroMovimientosNominas.Count; i++)
                {
                    if (String.Equals(movimientosNomina.concepNomDefi.clave, filtroMovimientosNominas[i].concepNomDefi.clave, StringComparison.OrdinalIgnoreCase))
                    {
                        if (movimientosNomina.id != filtroMovimientosNominas[i].id)
                        {
                            listMovNomTmp.Add(filtroMovimientosNominas[i]);
                        }
                    }
                }
                for (i = 0; i < listPlazasPorEmpleadosMovTmp.Count; i++)
                {
                    listMovNomTmp.AddRange(buscaMovimientosPlazasPorTipoYBaseAfecta(movimientosNomina.tipoCorrida.clave,
                        movimientosNomina.tipoNomina.clave, movimientosNomina.periodosNomina.id,
                        listPlazasPorEmpleadosMovTmp[i].plazasPorEmpleado, movimientosNomina.centroDeCosto == null ? null :
                        movimientosNomina.centroDeCosto.clave, Tipo.AUTOMATICO, movimientosNomina.razonesSociales.clave, null, -1,
                        movimientosNomina.concepNomDefi.clave));

                    listMovNomTmp.AddRange(buscaMovimientosPlazasPorTipoYBaseAfecta(movimientosNomina.tipoCorrida.clave,
                        movimientosNomina.tipoNomina.clave, movimientosNomina.periodosNomina.id,
                        listPlazasPorEmpleadosMovTmp[i].plazasPorEmpleado, movimientosNomina.centroDeCosto == null ? null :
                        movimientosNomina.centroDeCosto.clave, Tipo.PERIODO, movimientosNomina.razonesSociales.clave, null, -1,
                        movimientosNomina.concepNomDefi.clave));

                    listMovNomTmp.AddRange(buscaMovimientosPlazasPorTipoYBaseAfecta(movimientosNomina.tipoCorrida.clave,
                        movimientosNomina.tipoNomina.clave, movimientosNomina.periodosNomina.id,
                        listPlazasPorEmpleadosMovTmp[i].plazasPorEmpleado, movimientosNomina.centroDeCosto == null ? null :
                        movimientosNomina.centroDeCosto.clave, Tipo.REPETITIVO, movimientosNomina.razonesSociales.clave, null, -1,
                        movimientosNomina.concepNomDefi.clave));
                }
                filtroMovimientosNominas = filtroMovimientosNominas.Except(listMovNomTmp).ToList();
                listMovNomConcepPromocional.AddRange(listMovNomTmp);
                int numero = 1;
                movimientosNomina.numero = 1;
                if (listMovNomConcepPromocional.Count < listPlazasPorEmpleadosMovTmp.Count)
                {
                    for (i = 0; i < listPlazasPorEmpleadosMovTmp.Count; i++)
                    {
                        numero++;
                        listMovNomConcepPromocional.Add(duplicarMovNomConcep(movimientosNomina, numero, listPlazasPorEmpleadosMovTmp[i].plazasPorEmpleado));
                    }
                }
                listMovNomConcepPromocional.Add(movimientosNomina);
                listPlazasPorEmpleadosMovOficial.AddRange(listPlazasPorEmpleadosMovTmp);
                listPlazasPorEmpleadosMovOficial.Add(plazasPorEmpleadosMov);
                #endregion
            }
            else if (configuracionPercepcion_Plaza_Vigente)
            {
                #region Programacion para cuando se aplica modificaciones salariales, aqui se obtienen los movimientos del empleado dentro del periodo
                listMovNomConcepPromocional.Add(movimientosNomina);
                DateTime fechaIni = DateTime.Now, fechaFin = DateTime.Now;
                fechaIni = (DateTime)valoresConceptosEmpleados[parametroFechaInicial];
                fechaFin = (DateTime)valoresConceptosEmpleados[parametroFechaFinal];

                if (periodosNomina != null)
                {
                    fechaIni = periodosNomina.fechaInicial.GetValueOrDefault();
                    fechaFin = periodosNomina.fechaFinal.GetValueOrDefault();
                }
                List<PlazasPorEmpleadosMov> listPlazasPorEmpleadosMovTmp = new List<PlazasPorEmpleadosMov>();
                listPlazasPorEmpleadosMovTmp.AddRange(obtenerPlazasPorEmpleadosMovRestantes(plazasPorEmpleadosMov.plazasPorEmpleado.razonesSociales.clave, plazasPorEmpleadosMov.plazasPorEmpleado.registroPatronal.id, periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault(), plazasPorEmpleadosMov));
                List<MovNomConcep> listMovNomTmp = new List<MovNomConcep>();
                for (i = 0; i < listPlazasPorEmpleadosMovTmp.Count(); i++)
                {
                    listMovNomTmp.AddRange(buscaMovimientosPlazasPorTipoYBaseAfecta(movimientosNomina.tipoCorrida.clave,
                        movimientosNomina.tipoNomina.clave, movimientosNomina.periodosNomina.id,
                        listPlazasPorEmpleadosMovTmp[i].plazasPorEmpleado, movimientosNomina.centroDeCosto == null ? null :
                        movimientosNomina.centroDeCosto.clave, Tipo.AUTOMATICO, movimientosNomina.razonesSociales.clave, null, -1,
                        movimientosNomina.concepNomDefi.clave));
                    listMovNomTmp.AddRange(buscaMovimientosPlazasPorTipoYBaseAfecta(movimientosNomina.tipoCorrida.clave,
                        movimientosNomina.tipoNomina.clave, movimientosNomina.periodosNomina.id,
                        listPlazasPorEmpleadosMovTmp[i].plazasPorEmpleado, movimientosNomina.centroDeCosto == null ? null :
                        movimientosNomina.centroDeCosto.clave, Tipo.PERIODO, movimientosNomina.razonesSociales.clave, null, -1,
                        movimientosNomina.concepNomDefi.clave));
                    listMovNomTmp.AddRange(buscaMovimientosPlazasPorTipoYBaseAfecta(movimientosNomina.tipoCorrida.clave,
                        movimientosNomina.tipoNomina.clave, movimientosNomina.periodosNomina.id,
                        listPlazasPorEmpleadosMovTmp[i].plazasPorEmpleado, movimientosNomina.centroDeCosto == null ? null :
                        movimientosNomina.centroDeCosto.clave, Tipo.REPETITIVO, movimientosNomina.razonesSociales.clave, null, -1,
                        movimientosNomina.concepNomDefi.clave));
                }
                listPlazasPorEmpleadosMovTmp.Add(plazasPorEmpleadosMov);
                listPlazasPorEmpleadosMovTmp.AddRange(obtenerPlazasPorEmpleadosMovDentroPeriodo(tipoCorrida.clave, periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault(), listPlazasPorEmpleadosMovTmp.ToArray()));
                for (i = 0; i < filtroMovimientosNominas.Count; i++)
                {
                    if (String.Equals(movimientosNomina.concepNomDefi.clave, filtroMovimientosNominas[i].concepNomDefi.clave, StringComparison.OrdinalIgnoreCase))
                    {
                        if (movimientosNomina.id != filtroMovimientosNominas[i].id)
                        {
                            listMovNomTmp.Add(filtroMovimientosNominas[i]);
                        }
                    }
                }
                filtroMovimientosNominas = filtroMovimientosNominas.Except(listMovNomTmp).ToList();
                List<Object> clavesMovEliminados = new List<Object>();
                int limite = listMovNomTmp.Count;
                for (int j = 0; j < limite; j++)
                {
                    if (listMovNomTmp[j].id != 0)
                    {
                        clavesMovEliminados.Add(listMovNomTmp[j].id);
                    }
                }
                if (clavesMovEliminados.Any())
                {
                    eliminarMovimientosNominaBasura(clavesMovEliminados.ToArray());
                    //                System.out.println("flush 6");
                    getSession().Flush();
                }
                listMovNomTmp = listMovNomTmp.Except(listMovNomTmp).ToList();
                if (listPlazasPorEmpleadosMovTmp.Count > 1)
                {
                    listPlazasPorEmpleadosMovTmp.Sort(new rowComparator());
                }
                listMovNomConcepPromocional[0].numero = 1;
                int numero = 1;
                if (listMovNomConcepPromocional.Count() < listPlazasPorEmpleadosMovTmp.Count)
                {
                    for (i = 1; i < listPlazasPorEmpleadosMovTmp.Count; i++)
                    {
                        numero++;
                        listMovNomConcepPromocional.Add(duplicarMovNomConcep(listMovNomConcepPromocional[0], numero, listPlazasPorEmpleadosMovTmp[i].plazasPorEmpleado));
                    }
                    if (listMovNomConcepPromocional.Count > listPlazasPorEmpleadosMovTmp.Count)
                    {
                        foreach (MovNomConcep movNomConcep in listMovNomConcepPromocional)
                        {
                            if (movNomConcep.id == 0)
                            {
                                if (movNomConcep.plazasPorEmpleado.id == plazasPorEmpleadosMov.plazasPorEmpleado.id)
                                {
                                    listMovNomConcepPromocional.Remove(movNomConcep);
                                    break;
                                }
                            }
                        }
                    }
                }
                listPlazasPorEmpleadosMovOficial.AddRange(listPlazasPorEmpleadosMovTmp);
                String clavePlazaPorEmpleado = "";
                for (i = 0; i < listPlazasPorEmpleadosMovOficial.Count; i++)
                {
                    if (!String.Equals(listPlazasPorEmpleadosMovOficial[i].plazasPorEmpleado.referencia, clavePlazaPorEmpleado, StringComparison.OrdinalIgnoreCase))
                    {
                        clavePlazaPorEmpleado = listPlazasPorEmpleadosMovOficial[i].plazasPorEmpleado.referencia;
                        if (listPlazasPorEmpleadosMovOficial[i].fechaInicial.GetValueOrDefault().CompareTo(fechaIni) > 0)
                        {
                            if (i == 0)
                            {
                                listPlazasPorEmpleadosMovOficial.InsertRange(0, obtenerAnteriorPlazasPorEmpleadosMov(tipoCorrida.clave, periodosNomina.fechaInicial.GetValueOrDefault(), listPlazasPorEmpleadosMovOficial[i]));
                            }
                            else
                            {
                                listPlazasPorEmpleadosMovOficial.InsertRange(i - 1, obtenerAnteriorPlazasPorEmpleadosMov(tipoCorrida.clave, periodosNomina.fechaInicial.GetValueOrDefault(), listPlazasPorEmpleadosMovOficial[i]));
                            }
                            if (listPlazasPorEmpleadosMovOficial.Count != listMovNomConcepPromocional.Count)
                            {
                                listMovNomConcepPromocional.Add(duplicarMovNomConcep(listMovNomConcepPromocional[0], listMovNomConcepPromocional[listMovNomConcepPromocional.Count - 1].numero + 1, listPlazasPorEmpleadosMovOficial[i].plazasPorEmpleado));

                            }
                        }
                    }
                }
                #endregion
            }
            else
            {
                listPlazasPorEmpleadosMovOficial.Add(plazasPorEmpleadosMov);
                listMovNomConcepPromocional = listMovNomConcepPromocional.Except(listMovNomConcepPromocional).ToList();
                listMovNomConcepPromocional.Add(movimientosNomina);
            }

            continuar = false;
            if (listMovNomConcepPromocional.Count > listPlazasPorEmpleadosMovOficial.Count)
            {
                continuar = true;
                if (evaluaPeriodoMovAbarca2Meses(periodosNomina))
                {
                    for (i = 0; i < listPlazasPorEmpleadosMovOficial.Count; i++)
                    {
                        {
                            if (i + 1 < listPlazasPorEmpleadosMovOficial.Count)
                            {
                                DateTime fechaUno, fechaDos;
                                fechaUno = listPlazasPorEmpleadosMovOficial[i].fechaInicial.GetValueOrDefault();
                                fechaDos = listPlazasPorEmpleadosMovOficial[i + 1].fechaInicial.GetValueOrDefault();
                                if (fechaUno.Month != fechaDos.Month)
                                {
                                    fechaUno = periodosNomina.fechaInicial.GetValueOrDefault();
                                    if (fechaUno.Month != fechaDos.Month)
                                    {
                                        fechaUno = fechaDos;
                                        fechaUno.AddDays(1);//Obtener dia 1 del mes.
                                        if (fechaDos.CompareTo(fechaUno) > 0)
                                        {
                                            if (listMovNomConcepPromocional.Count == listPlazasPorEmpleadosMovOficial.Count + 1)
                                            {
                                                continuar = false;
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }
            if (continuar)
            {
                List<Object> clavesMovEliminados = new List<Object>();
                int contador = 0;
                for (int j = 0; j < listPlazasPorEmpleadosMovOficial.Count; j++)
                {
                    if (evaluaPeriodoMovAbarca2Meses(periodosNomina))
                    {
                        for (int k = 0; k < listMovNomConcepPromocional.Count; k++)
                        {
                            if (listMovNomConcepPromocional[k].plazasPorEmpleado.id == listPlazasPorEmpleadosMovOficial[j].plazasPorEmpleado.id)
                            {
                                contador++;
                                if (contador > 2)
                                {
                                    clavesMovEliminados.Add(listMovNomConcepPromocional[k].id);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (listMovNomConcepPromocional[listPlazasPorEmpleadosMovOficial.Count - 1].id != 0)
                        {
                            clavesMovEliminados.Add(listMovNomConcepPromocional[listPlazasPorEmpleadosMovOficial.Count - 1].id);
                        }
                        listMovNomConcepPromocional.Remove(listMovNomConcepPromocional[listPlazasPorEmpleadosMovOficial.Count - 1]);
                    }
                }
                if (clavesMovEliminados.Count > 0)
                {
                    eliminarMovimientosNominaBasura(clavesMovEliminados.ToArray());  // pendiente
                    getSession().Flush();
                    getSession().Clear();
                }
            }
            if (listPlazasPorEmpleadosMovOficial.Count > 1)
            {
                if (evaluaPeriodoMovAbarca2Meses(periodosNomina))
                {
                    bool mesUnoEncontrado = false;
                    int indiceMovimientoNomina = 0;
                    DateTime fechaPromocion, fechaUno, fechaDos;
                    for (i = 0; i < listPlazasPorEmpleadosMovOficial.Count; i++)
                    {
                        mesUnoEncontrado = false;
                        if (i + 1 < listPlazasPorEmpleadosMovOficial.Count)
                        {
                            fechaUno = listPlazasPorEmpleadosMovOficial[i].fechaInicial.GetValueOrDefault();
                            if (fechaUno.CompareTo(periodosNomina.fechaInicial) < 0)
                            {
                                fechaPromocion = periodosNomina.fechaInicial.GetValueOrDefault();
                            }
                            else
                            {
                                fechaPromocion = listPlazasPorEmpleadosMovOficial[i].fechaInicial.GetValueOrDefault();
                            }
                            fechaUno = periodosNomina.fechaInicial.GetValueOrDefault();
                            if (fechaPromocion.Month == fechaUno.Month)
                            {
                                mesUnoEncontrado = true;
                            }
                            listMovNomConcepPromocional[indiceMovimientoNomina].ejercicio = periodosNomina.año;
                            listMovNomConcepPromocional[indiceMovimientoNomina].mes = fechaPromocion.Month;
                            if (mesUnoEncontrado)
                            {
                                listMovNomConcepPromocional[indiceMovimientoNomina].numMovParticion = 1;
                            }
                            else
                            {//Existe el mesDos generare el mesUno
                                listMovNomConcepPromocional[indiceMovimientoNomina].numMovParticion = 2;
                            }

                            fechaUno = listPlazasPorEmpleadosMovOficial[i].fechaInicial.GetValueOrDefault();
                            fechaDos = listPlazasPorEmpleadosMovOficial[i + 1].fechaInicial.GetValueOrDefault();
                            fechaPromocion = listPlazasPorEmpleadosMovOficial[i].fechaInicial.GetValueOrDefault();
                            bool incremento = false;
                            if (fechaUno.Month != fechaDos.Month)
                            {
                                fechaUno = periodosNomina.fechaInicial.GetValueOrDefault();
                                if (fechaUno.Month != fechaDos.Month)
                                {
                                    fechaUno = fechaDos;
                                    fechaUno.AddDays(1);
                                    if (fechaDos.CompareTo(fechaUno) > 0)
                                    {
                                        fechaPromocion = fechaUno;
                                        indiceMovimientoNomina++;
                                        incremento = true;
                                        listMovNomConcepPromocional[indiceMovimientoNomina].ejercicio = periodosNomina.año;
                                        listMovNomConcepPromocional[indiceMovimientoNomina].mes = fechaPromocion.Month;
                                        listMovNomConcepPromocional[indiceMovimientoNomina].numMovParticion = 2;
                                    }
                                }
                            }
                            if (!incremento)
                            {
                                indiceMovimientoNomina++;
                                incremento = false;
                            }
                        }
                        else
                        {
                            fechaUno = listPlazasPorEmpleadosMovOficial[i].fechaInicial.GetValueOrDefault();
                            if (fechaUno.CompareTo(periodosNomina.fechaInicial) < 0)
                            {
                                fechaPromocion = periodosNomina.fechaInicial.GetValueOrDefault();
                            }
                            else
                            {
                                fechaPromocion = listPlazasPorEmpleadosMovOficial[i].fechaInicial.GetValueOrDefault();
                            }
                            fechaUno = periodosNomina.fechaInicial.GetValueOrDefault();
                            if (fechaPromocion.Month == fechaUno.Month)
                            {
                                mesUnoEncontrado = true;
                            }
                            listMovNomConcepPromocional[indiceMovimientoNomina].ejercicio = periodosNomina.año;
                            listMovNomConcepPromocional[indiceMovimientoNomina].mes = fechaPromocion.Month;
                            if (mesUnoEncontrado)
                            {
                                listMovNomConcepPromocional[indiceMovimientoNomina].numMovParticion = 1;
                            }
                            else
                            {//Existe el mesDos generare el mesUno
                                listMovNomConcepPromocional[indiceMovimientoNomina].numMovParticion = 2;
                            }
                        }
                    }
                }

            }
            listObject.Add(listPlazasPorEmpleadosMovOficial);
            listObject.Add(listMovNomConcepPromocional);
            listObject.Add(filtroMovimientosNominas);
            return (List<object>)listObject;
        }
        class rowComparator : IComparer<PlazasPorEmpleadosMov>
        {

            public int Compare(PlazasPorEmpleadosMov o1, PlazasPorEmpleadosMov o2)
            {
                if (ReferenceEquals(o1.plazasPorEmpleado.id, o2.plazasPorEmpleado.id))
                {
                    return 0;
                }
                int resultado = o1.fechaInicial.GetValueOrDefault().CompareTo(o2.fechaInicial.GetValueOrDefault()) > 0 ? 1 : -1;
                if (resultado != 0)
                {
                    return resultado;
                }
                return resultado;
            }
        }

        #region calculos nomina
        private Double calculaImss(Double salarioDiarioIntegrado, Double salarioMinimoDF, MovNomConcep movNominaImss, PlazasPorEmpleadosMov plazasPorEmpleadosMov)
        {
            Double acumuladoIMSS = 0.0;
            List<SalariosIntegrados> listSalariosIntegrados = null;
            try
            {
                double valorEspecieEnfermeMater = 0.0, valorDineEnfermeMater = 0.0, valorGastosPension = 0.0, valorInvalidezVida = 0.0, valorCesantiaVejez = 0.0;
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                strQuery.Remove(0, strQuery.Length).Append("FROM CalculoIMSS imss WHERE imss.movNomConcep.id = :idMovNom ");
                camposParametro.Add("idMovNom");
                valoresParametro.Add(movNominaImss.id);
                IQuery q = getSession().CreateQuery(strQuery.ToString());
                q = cargarParametrosQuery(q, camposParametro, valoresParametro);
                IList<CalculoIMSS> lista = q.List<CalculoIMSS>();
                listCalculoIMSS = (List<CalculoIMSS>)lista;
                if (listCalculoIMSS == null ? true : listCalculoIMSS.Count() == 0 ? true : false)
                {
                    if (listCalculoIMSS == null)
                    {
                        listCalculoIMSS = new List<CalculoIMSS>();
                    }
                    CalculoIMSS calculoIMSS = new CalculoIMSS();
                    calculoIMSS.configuracionIMSS = configuracionIMSS;
                    listCalculoIMSS.Add(calculoIMSS);
                }

                if (salarioDiarioIntegrado > 0)
                {
                    if ((Convert.ToDouble(valoresConceptosEmpleados["SUELDODIARIO".ToUpper()].ToString())) > salarioMinimoDF)
                    {
                        try
                        {
                            bool buscaSDIAnterior = true;
                            if (evaluaPeriodoAbarca2Meses(periodosNomina))
                            {
                                DateTime cFecha = DateTime.Now;
                                if (movNominaImss.numMovParticion == 2)
                                {
                                    cFecha = periodosNomina.fechaFinal.GetValueOrDefault();
                                    cFecha = new DateTime(cFecha.Year, cFecha.Month, 1);
                                    listSalariosIntegrados = obtenerSalariosIntegradosDentroDelPeriodo(cFecha, Convert.ToDateTime(periodosNomina.fechaFinal), plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave, plazasPorEmpleadosMov.plazasPorEmpleado.registroPatronal.clave, plazasPorEmpleadosMov.plazasPorEmpleado.razonesSociales.clave);
                                    buscaSDIAnterior = false;
                                }
                                else
                                {
                                    cFecha = periodosNomina.fechaInicial.GetValueOrDefault();
                                    cFecha.AddDays(DateTime.DaysInMonth(cFecha.Year, cFecha.Month));
                                    listSalariosIntegrados = obtenerSalariosIntegradosDentroDelPeriodo(periodosNomina.fechaInicial.GetValueOrDefault(), cFecha, plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave, plazasPorEmpleadosMov.plazasPorEmpleado.registroPatronal.clave, plazasPorEmpleadosMov.plazasPorEmpleado.razonesSociales.clave);
                                    if (listSalariosIntegrados.Count() == 0)
                                    {
                                        buscaSDIAnterior = false;
                                        listSalariosIntegrados.AddRange(obtenerAnteriorSalariosIntegrados(periodosNomina.fechaInicial.GetValueOrDefault(), plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave, plazasPorEmpleadosMov.plazasPorEmpleado.registroPatronal.clave, plazasPorEmpleadosMov.plazasPorEmpleado.razonesSociales.clave));

                                    }
                                }
                            }
                            else
                            {
                                listSalariosIntegrados = obtenerSalariosIntegradosDentroDelPeriodo(periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault(), plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave, plazasPorEmpleadosMov.plazasPorEmpleado.registroPatronal.clave, plazasPorEmpleadosMov.plazasPorEmpleado.razonesSociales.clave);

                            }
                            if (listSalariosIntegrados.Count() > 0)
                            {
                                if (listSalariosIntegrados[0].fecha.GetValueOrDefault().CompareTo(periodosNomina.fechaInicial.GetValueOrDefault()) > 0 && buscaSDIAnterior)
                                {

                                    listSalariosIntegrados.AddRange(obtenerAnteriorSalariosIntegrados(periodosNomina.fechaInicial.GetValueOrDefault(), plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave, plazasPorEmpleadosMov.plazasPorEmpleado.registroPatronal.clave, plazasPorEmpleadosMov.plazasPorEmpleado.razonesSociales.clave));
                                }

                            }
                            else
                            {
                                SalariosIntegrados salariosIntegrados = new SalariosIntegrados();
                                salariosIntegrados.salarioDiarioIntegrado = salarioDiarioIntegrado;
                                listSalariosIntegrados.Add(salariosIntegrados);
                            }
                            int i;
                            if (listCalculoIMSS.Count() < listSalariosIntegrados.Count())
                            {
                                int cantidad = listSalariosIntegrados.Count() - listCalculoIMSS.Count();
                                for (i = 0; i < cantidad; i++)
                                {
                                    CalculoIMSS calculoIMSS = new CalculoIMSS();
                                    calculoIMSS.configuracionIMSS = configuracionIMSS;
                                    listCalculoIMSS.Add(calculoIMSS);
                                }
                            }
                            for (i = 0; i < listSalariosIntegrados.Count(); i++)
                            {
                                salarioDiarioIntegrado = listSalariosIntegrados[i].salarioDiarioIntegrado;
                                if (listSalariosIntegrados.Count() > 1)
                                {
                                    #region programacion para cuando existan modificaciones salariales
                                    if (i + 1 <= listSalariosIntegrados.Count() - 1)
                                    {
                                        cargaValoresDiasCotizados(listSalariosIntegrados[i].fecha.GetValueOrDefault(), plazasPorEmpleadosMov.plazasPorEmpleado.fechaFinal.GetValueOrDefault(), false, listSalariosIntegrados[i + 1], null, false, false);
                                    }
                                    else
                                    {
                                        cargaValoresDiasCotizados(listSalariosIntegrados[i].fecha.GetValueOrDefault(), plazasPorEmpleadosMov.plazasPorEmpleado.fechaFinal.GetValueOrDefault(), false, null, null, false, false);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    cargaValoresDiasCotizados(plazasPorEmpleadosMov.fechaIMSS.GetValueOrDefault(), plazasPorEmpleadosMov.plazasPorEmpleado.fechaFinal.GetValueOrDefault(), true, null, null, false, false);
                                }
                                if ((salarioMinimoDF * configuracionIMSS.excedenteEspecie) > salarioDiarioIntegrado)
                                {
                                    valorEspecieEnfermeMater = 0.0;
                                }
                                else
                                {
                                    valorEspecieEnfermeMater = ((configuracionIMSS.tasaEspecieEnfermeMater / 100) * (salarioDiarioIntegrado - (salarioMinimoDF * configuracionIMSS.excedenteEspecie)));
                                }
                                valorDineEnfermeMater = ((configuracionIMSS.tasaDineEnfermeMater / 100) * salarioDiarioIntegrado);
                                valorGastosPension = ((configuracionIMSS.tasaGastosPension / 100) * salarioDiarioIntegrado);
                                valorInvalidezVida = ((configuracionIMSS.tasaInvalidezVida / 100) * salarioDiarioIntegrado);
                                valorCesantiaVejez = ((configuracionIMSS.tasaCesantiaVejez / 100) * salarioDiarioIntegrado);
                                acumuladoIMSS += ((valorEspecieEnfermeMater + valorDineEnfermeMater + valorGastosPension + valorInvalidezVida + valorCesantiaVejez) * (Convert.ToDouble(valoresConceptosEmpleados["DiasCotizados".ToUpper()].ToString())));

                                listCalculoIMSS[i].valorEspecieEnfermeMater = valorEspecieEnfermeMater;
                                listCalculoIMSS[i].valorDineEnfermeMater = valorDineEnfermeMater;
                                listCalculoIMSS[i].valorGastosPension = valorGastosPension;
                                listCalculoIMSS[i].valorInvalidezVida = valorInvalidezVida;
                                listCalculoIMSS[i].valorCesantiaVejez = valorCesantiaVejez;
                                listCalculoIMSS[i].diasCotizados = (Convert.ToDouble(valoresConceptosEmpleados["DiasCotizados".ToUpper()].ToString()));



                            }

                            if (listSalariosIntegrados.Count() > 1)
                            {
                                cargaValoresDiasCotizados(listSalariosIntegrados[0].fecha.GetValueOrDefault(), plazasPorEmpleadosMov.plazasPorEmpleado.fechaFinal.GetValueOrDefault(), true, null, null, false, false);
                            }

                        }
                        catch (Exception ex)
                        {

                            mensajeResultado.noError = 88;
                            mensajeResultado.error = ex.GetBaseException().ToString();
                            mensajeResultado.resultado = null;
                        }
                        if (listCalculoIMSS.Count() > listSalariosIntegrados.Count())
                        {
                            List<object> clavesMovEliminados = new List<object>();
                            int limite = listCalculoIMSS.Count() - listSalariosIntegrados.Count();
                            for (int i = 0; i < limite; i++)
                            {
                                if (listCalculoIMSS[listCalculoIMSS.Count()].id > 0)
                                {
                                    clavesMovEliminados.Add(listCalculoIMSS[listCalculoIMSS.Count()].id);
                                }
                                listCalculoIMSS.RemoveAt(listCalculoIMSS.Count());
                            }
                            if (clavesMovEliminados.Count() > 0)
                            {

                                StringBuilder consulta = new StringBuilder();
                                consulta.Append("delete ");
                                //Elimina Movimientos CalculoIMSS
                                consulta.Append(typeof(CalculoIMSS).Name).Append(" o where o.movNomConcep.id in(:valores)");
                                IQuery q2 = getSession().CreateQuery(consulta.ToString());
                                q2.SetParameterList("valores", clavesMovEliminados.ToArray());
                                q2.ExecuteUpdate();
                                //                            System.out.println("flush 15");
                                getSession().Flush();
                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("calculaImss()1_Error: ").Append(ex));
                mensajeResultado.noError = 50;
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return acumuladoIMSS;

        }

        private Double calculaImssPatronal(Double salarioDiarioIntegrado, Double salarioMinimoDF, MovNomConcep movNominaImss, PlazasPorEmpleadosMov plazasPorEmpleadosMov)
        {
            Double calculoImss = 0.0;
            try
            {
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                strQuery.Remove(0, strQuery.Length).Append("FROM CalculoIMSSPatron imss WHERE imss.movNomConcep.id = :idMovNom AND imss.configuracionIMSS.id = :idConfigIMSS");
                camposParametro.Add("idMovNom");
                valoresParametro.Add(movNominaImss.id);
                camposParametro.Add("idConfigIMSS");
                valoresParametro.Add(configuracionIMSS.id);

                calculoIMSSPatron = (CalculoIMSSPatron)ejecutaQueryUnico(strQuery.ToString(), camposParametro.ToArray<string>(), valoresParametro.ToArray());
                if (calculoIMSSPatron == null)
                {
                    calculoIMSSPatron = new CalculoIMSSPatron();
                    calculoIMSSPatron.configuracionIMSS = configuracionIMSS;
                }
                if (salarioDiarioIntegrado > 0)
                {
                    try
                    {
                        if (evaluaPeriodoAbarca2Meses(periodosNomina))
                        {
                            DateTime cFecha = DateTime.Now;
                            if (movNominaImss.numMovParticion == 2)
                            {
                                cFecha = periodosNomina.fechaFinal.GetValueOrDefault();
                                cFecha = new DateTime(cFecha.Year, cFecha.Month, 1);
                                SalariosIntegrados salariosIntegrados = obtenerSalariosIntegradosActual(periodosNomina.fechaFinal.GetValueOrDefault(), plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave, plazasPorEmpleadosMov.plazasPorEmpleado.registroPatronal.clave, plazasPorEmpleadosMov.plazasPorEmpleado.razonesSociales.clave);
                                if (salariosIntegrados != null)
                                {
                                    salarioDiarioIntegrado = salariosIntegrados.salarioDiarioIntegrado;
                                }
                            }
                            else
                            {
                                cFecha = periodosNomina.fechaInicial.GetValueOrDefault();
                                cFecha.AddDays(DateTime.DaysInMonth(cFecha.Year, cFecha.Month));
                                SalariosIntegrados salariosIntegrados = obtenerSalariosIntegradosActual(cFecha, plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave, plazasPorEmpleadosMov.plazasPorEmpleado.registroPatronal.clave, plazasPorEmpleadosMov.plazasPorEmpleado.razonesSociales.clave);
                                if (salariosIntegrados != null)
                                {
                                    salarioDiarioIntegrado = salariosIntegrados.salarioDiarioIntegrado;
                                }
                            }
                        }
                        calculoIMSSPatron.valorTasaFijaPatron = ((configuracionIMSS.tasaFijaPatron / 100) * salarioMinimoDF);
                        if ((salarioMinimoDF * configuracionIMSS.tasaExcedentePatron) > salarioDiarioIntegrado)
                        {
                            calculoIMSSPatron.valorTasaExcedentePatron = 0.0;
                        }
                        else
                        {
                            calculoIMSSPatron.valorTasaExcedentePatron = (configuracionIMSS.tasaExcedentePatron / 100) * (salarioDiarioIntegrado - (salarioMinimoDF * configuracionIMSS.tasaExcedentePatron));
                        }
                        calculoIMSSPatron.valorTasaPrestDinePatron = (configuracionIMSS.tasaPrestDinePatron / 100) * salarioDiarioIntegrado;
                        calculoIMSSPatron.valorTasaInvaliVidaPatron = (configuracionIMSS.tasaInvaliVidaPatron / 100) * salarioDiarioIntegrado;
                        calculoIMSSPatron.valorTasaCesanVejezPatron = (configuracionIMSS.tasaCesanVejezPatron / 100) * salarioDiarioIntegrado;
                        calculoIMSSPatron.valorTasaGastosPensPatron = (configuracionIMSS.tasaGastosPensPatron / 100) * salarioDiarioIntegrado;
                        calculoIMSSPatron.valorTasaRiesgosPatron = (configuracionIMSS.tasaRiesgosPatron / 100) * salarioDiarioIntegrado;
                        calculoIMSSPatron.valorTasaGuarderiaPatron = (configuracionIMSS.tasaGuarderiaPatron / 100) * salarioDiarioIntegrado;
                        calculoIMSSPatron.valorTasaAportacionRetiroPatron = (configuracionIMSS.tasaAportacionRetiroPatron / 100) * salarioDiarioIntegrado;
                        calculoIMSSPatron.valorTasaAportacionInfonavitPatron = (configuracionIMSS.tasaAportacionInfonavitPatron / 100) * salarioDiarioIntegrado;
                        calculoImss = calculoIMSSPatron.valorTasaFijaPatron + calculoIMSSPatron.valorTasaExcedentePatron + calculoIMSSPatron.valorTasaPrestDinePatron + calculoIMSSPatron.valorTasaInvaliVidaPatron
                                + calculoIMSSPatron.valorTasaCesanVejezPatron + calculoIMSSPatron.valorTasaGastosPensPatron + calculoIMSSPatron.valorTasaRiesgosPatron + calculoIMSSPatron.valorTasaGuarderiaPatron
                                + calculoIMSSPatron.valorTasaAportacionRetiroPatron + calculoIMSSPatron.valorTasaAportacionInfonavitPatron;
                    }
                    catch (Exception ex)
                    {
                        mensajeResultado.noError = 88;
                        mensajeResultado.error = ex.GetBaseException().ToString();
                        mensajeResultado.resultado = null;
                    }
                    calculoImss = calculoImss * ((Convert.ToInt32(valoresConceptosEmpleados["DiasCotizados".ToUpper()])));

                }
                else
                {

                    calculoIMSSPatron.valorTasaFijaPatron = 0.0;
                    calculoIMSSPatron.valorTasaExcedentePatron = 0.0;
                    calculoIMSSPatron.valorTasaPrestDinePatron = 0.0;
                    calculoIMSSPatron.valorTasaInvaliVidaPatron = 0.0;
                    calculoIMSSPatron.valorTasaCesanVejezPatron = 0.0;
                    calculoIMSSPatron.valorTasaGastosPensPatron = 0.0;
                    calculoIMSSPatron.valorTasaRiesgosPatron = 0.0;
                    calculoIMSSPatron.valorTasaGuarderiaPatron = 0.0;
                    calculoIMSSPatron.valorTasaAportacionRetiroPatron = 0.0;
                    calculoIMSSPatron.valorTasaAportacionInfonavitPatron = 0.0;
                }
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("calculaImssPatronal()1_Error: ").Append(ex));
                mensajeResultado.noError = 50;
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return calculoImss;
        }

        private SalariosIntegrados obtenerSalariosIntegradosActual(DateTime fechaInicial, String claveEmpleado, String claveRegistroPatronal, String claveRazonesSociales)
        {
            SalariosIntegrados s = null;
            try
            {
                strQuery = new StringBuilder("SELECT s FROM ").Append(typeof(SalariosIntegrados).Name).Append(" s ");
                strQuery.Append(" WHERE s.fecha <= :fechaInicialPeriodo and s.empleados.clave = :claveEmpleado ");
                strQuery.Append(" AND s.registroPatronal.clave = :claveRegPatronal and s.empleados.razonesSociales.clave = :claveRazonesSociales  order by s.fecha desc");
                s = (SalariosIntegrados)ejecutaQueryUnico(strQuery.ToString(), new String[] { "fechaInicialPeriodo", "claveEmpleado", "claveRegPatronal", "claveRazonesSociales" },
                        new Object[]{fechaInicial, claveEmpleado, claveRegistroPatronal, claveRazonesSociales
        });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerSalariosIntegradosDentroDelPeriodo()1_Error: ").Append(ex));
                mensajeResultado.noError = 27;
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            if (s != null)
            {
                if (s.finiquitosLiquida != null)
                {
                    s = null;
                }
            }
            return s;
        }

        private List<SalariosIntegrados> obtenerAnteriorSalariosIntegrados(DateTime fechaInicial, String claveEmpleado, String claveRegistroPatronal, String claveRazonesSociales)
        {
            List<SalariosIntegrados> listSalariosIntegrados = null;
            try
            {
                strQuery = new StringBuilder("FROM ").Append(typeof(SalariosIntegrados).Name).Append(" s ");
                strQuery.Append(" WHERE s.fecha < :fechaInicialPeriodo and s.empleados.clave = :claveEmpleado ");
                strQuery.Append(" AND s.registroPatronal.clave = :claveRegPatronal and s.empleados.razonesSociales.clave = :claveRazonesSociales order by s.fecha desc");
                IQuery q = getSession().CreateQuery(strQuery.ToString());
                q = cargarParametrosQuery(q, (new String[] { "fechaInicialPeriodo", "claveEmpleado", "claveRegPatronal", "claveRazonesSociales" }).ToList(),
                    (new Object[] { fechaInicial, claveEmpleado, claveRegistroPatronal, claveRazonesSociales }).ToList());
                q.SetMaxResults(1);
                IList<SalariosIntegrados> salariosIntegrados = q.List<SalariosIntegrados>();
                listSalariosIntegrados = (List<SalariosIntegrados>)salariosIntegrados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerAnteriorSalariosIntegrados()1_Error: ").Append(ex));
                mensajeResultado.noError = 27;
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return listSalariosIntegrados;
        }

        private List<SalariosIntegrados> obtenerSalariosIntegradosDentroDelPeriodo(DateTime fechaInicial, DateTime fechaFinal, String claveEmpleado, String claveRegistroPatronal, String claveRazonesSociales)
        {
            List<SalariosIntegrados> listSalariosIntegrados = null;
            try
            {
                strQuery = new StringBuilder("FROM ").Append(typeof(SalariosIntegrados).Name).Append(" s ");
                strQuery.Append(" WHERE s.fecha between :fechaInicialPeriodo AND :fechaFinalPeriodo  and s.empleados.clave = :claveEmpleado ");
                strQuery.Append(" AND s.registroPatronal.clave = :claveRegPatronal and s.empleados.razonesSociales.clave = :claveRazonesSociales");
                IQuery q = getSession().CreateQuery(strQuery.ToString());
                q = cargarParametrosQuery(q, (new String[] { "fechaInicialPeriodo", "fechaFinalPeriodo", "claveEmpleado", "claveRegPatronal", "claveRazonesSociales" }).ToList(),
                    (new Object[] { fechaInicial, fechaFinal, claveEmpleado, claveRegistroPatronal, claveRazonesSociales }).ToList());
                IList<SalariosIntegrados> salariosIntegrados = q.List<SalariosIntegrados>();
                listSalariosIntegrados = (List<SalariosIntegrados>)salariosIntegrados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerSalariosIntegradosDentroDelPeriodo()1_Error: ").Append(ex));
                mensajeResultado.noError = 27;
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return listSalariosIntegrados;
        }

        private double calculoISRFiniquitos(TipoCorrida tipoCorrida)
        {
            try
            {
                DateTime fechaBajaEmpleado;
                Double baseGravadaFiniquitos = 0.0, isrMesAnterior = 0.0, baseGravableMesAnterior = 0.0, isrFiniquitos = 0.0, factor = 0.0, baseGravadaAnual = 0.0;
                ValorTablaISR tablaISR;
                Object[,] acumula = acumuladosISRAnualPorTipoAfecta(valoresConceptosEmpleados["NumEmpleado".ToUpper()].ToString(), valoresConceptosEmpleados["PlazaEmpleado".ToUpper()].ToString(), "FIN", valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString(),
                    valoresConceptosEmpleados["NumPeriodo".ToUpper()].ToString(), (int)valoresConceptosEmpleados["ejercicioActivo".ToUpper()], valoresConceptosEmpleados["RazonSocial".ToUpper()].ToString(), ClavesParametrosModulos.claveBaseNominaISR.ToString());   /// no trae nada de acuulados por que busca por corrida finiquito y no hay
                double acumAnual = 0.0;
                int i;
                if (acumula != null)
                {
                    for (i = 0; i < acumula.Length; i++)
                    {//BDEI01
                        acumAnual += (Double)acumula[i, 0];
                    }
                }
                retenido = new CalculoISR();
                if (fechaBajaFiniq != null)
                {
                    fechaBajaEmpleado = fechaBajaFiniq.GetValueOrDefault();
                }
                else
                {
                    fechaBajaEmpleado = (DateTime)valoresConceptosEmpleados["FechaBaja".ToUpper()];
                }
                baseGravadaFiniquitos = acumuladoAnual + acumuladoDirecto + acumuladoNormal;

                if (fechaBajaEmpleado.Month == 12) // 12 == Diciembre
                {
                    double diasPagoTotal = (Double)valoresConceptosEmpleados["DiasPago".ToUpper()];
                    baseGravadaAnual = ((baseGravadaFiniquitos + acumAnual) / diasPagoTotal) * factorAnual.GetValueOrDefault();
                    if (baseGravadaAnual > 0)
                    {
                        tablaISR = aplicacionTablaISR(baseGravadaAnual, false, tipoCorrida);
                        retenido.isrACargoNormal = (Double)valoresConceptosEmpleados["ISRACARGO".ToUpper()];
                        retenido.subsidioEmpleoNormal = (Double)(valoresConceptosEmpleados["SubsEmpleoCalculado".ToUpper()] == null ? 0.0 : valoresConceptosEmpleados["SubsEmpleoCalculado".ToUpper()]);
                        factor = tablaISR.isrCausado / baseGravadaAnual;
                        isrFiniquitos = factor * baseGravadaFiniquitos;
                    }
                    else
                    {
                        retenido.isrACargoNormal = 0.0;
                        retenido.subsidioEmpleoNormal = 0.0;
                        retenido.isrNetoNormal = 0.0;
                        retenido.isrSubsidioNormal = 0.0;
                    }
                }
                else
                {
                    //****************************baja es antes de diciembre***********************************//
                    //Mes Anterior a la fecha de baja
                    //fechaBajaEmpleado.set(Calendar.MONTH, fechaBajaEmpleado.get(Calendar.MONTH) - 1);
                    //no trae nada por que busca por corrida finiquito e
                    Double[] acumulados = baseGravableAcumuladaMesAnterior(fechaBajaEmpleado, "PER"); //falta fecha
                    if (mensajeResultado.noError != 0)
                    {
                        return 0.0;
                    }
                    baseGravableMesAnterior = acumulados[0];
                    if (baseGravadaFiniquitos > baseGravableMesAnterior)
                    {
                        isrMesAnterior = acumulados[1];
                        if (isrMesAnterior == 0.0 | baseGravableMesAnterior == 0.0)
                        {
                            factor = 0.0;
                        }
                        else
                        {
                            factor = isrMesAnterior / baseGravableMesAnterior;
                        }
                        isrFiniquitos = baseGravadaFiniquitos * factor;
                    }
                    else if (baseGravadaFiniquitos > 0)
                    {
                        tablaISR = aplicacionTablaISR(baseGravadaFiniquitos, false, tipoCorrida);
                        retenido.isrACargoNormal = (Double)valoresConceptosEmpleados["ISRACARGO".ToUpper()];
                        retenido.subsidioEmpleoNormal = (Double)(valoresConceptosEmpleados["SubsEmpleoCalculado".ToUpper()] == null ? 0.0 : valoresConceptosEmpleados["SubsEmpleoCalculado".ToUpper()]);
                        isrFiniquitos = tablaISR.isrCausado; // isrNeto
                    }
                    else
                    {
                        retenido.isrACargoNormal = 0.0;
                        retenido.subsidioEmpleoNormal = 0.0;
                        retenido.isrNetoNormal = 0.0;
                        retenido.isrSubsidioNormal = 0.0;
                    }
                }
                return isrFiniquitos;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("calculoISRFiniquitos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return 0;
        }

        private double calculoISPTAnual(MovNomConcep movimientosNomina)
        {
            try
            {
                retenido = new CalculoISR();
                calculoDiasTranscurridos();
                if (mensajeResultado.noError != 0)
                {
                    return 0.0;
                }
                double acumuladoPeriodoAnual = acumuladosPorTipoISRAnual(valoresConceptosEmpleados["NumEmpleado".ToUpper()].ToString(), valoresConceptosEmpleados["PlazaEmpleado".ToUpper()].ToString(), valoresConceptosEmpleados["TipoCorrida".ToUpper()].ToString(), valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString(),
                    valoresConceptosEmpleados["NumPeriodo".ToUpper()].ToString(), DateTime.Now.Year, valoresConceptosEmpleados["RazonSocial".ToUpper()].ToString(), ClavesParametrosModulos.claveBaseNominaISR.ToString());
                if (mensajeResultado.noError != 0)
                {
                    return 0.0;
                }

                double baseGravableAnual = ((acumuladoNormal + acumuladoDirecto + acumuladoAnual + acumuladoPeriodoAnual) / diasPago) * factorAnual.GetValueOrDefault();
                ValorTablaISR valorTablaISR = aplicacionTablaISR(baseGravableAnual, false, movimientosNomina.tipoCorrida);
                if (mensajeResultado.noError != 0)
                {
                    return 0.0;
                }
                retenido.isrACargoAnual = (Double)valoresConceptosEmpleados["ISRACARGO".ToUpper()];
                retenido.subsidioEmpleoAnual = (Double)(valoresConceptosEmpleados["SubsEmpleoCalculado".ToUpper()] == null ? 0.0 : valoresConceptosEmpleados["SubsEmpleoCalculado".ToUpper()]);

                retenido.isrACargoNormal = 0.0;
                retenido.subsidioEmpleoNormal = 0.0;
                retenido.isrNetoNormal = 0.0;
                retenido.isrSubsidioNormal = 0.0;

                retenido.isrACargoDirecto = 0.0;
                retenido.subsidioEmpleoDirecto = 0.0;
                retenido.isrNetoDirecto = 0.0;
                retenido.isrSubsidioDirecto = 0.0;

                isrAnual = (valorTablaISR.isrNeto / factorAnual.GetValueOrDefault()) * diasPago;
                if (isMov2Meses)
                {
                    int diasPagoTotal = (int)valoresConceptosEmpleados["DiasNormalesAPagar".ToUpper()];
                    int diasPeriodo;
                    if (manejaPagoDiasNaturales)
                    {
                        diasPeriodo = (cantidadDiasEntreDosFechas(periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault()) + 1); //+1 para contar el dia actual
                    }
                    else
                    {
                        diasPeriodo = Convert.ToInt16(periodosNomina.tipoNomina.periodicidad.dias);
                    }
                    isrAnual = (isrAnual * diasPagoTotal) / diasPeriodo;
                }
                isrNormal = 0;
                isrDirecto = 0;
                if (movimientosNomina != null)
                {
                    IsrRetenidos(movimientosNomina);
                    if (mensajeResultado.noError != 0)
                    {
                        return 0.0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("calculoISPTAnual()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return isrNormal + isrDirecto + isrAnual;
        }

        private double calculaISPTSeparado(MovNomConcep movimientosNomina)
        {
            try
            {
                Object[,] acumulados = acumuladosPorTipoISR(valoresConceptosEmpleados["NumEmpleado".ToUpper()].ToString(), valoresConceptosEmpleados["PlazaEmpleado".ToUpper()].ToString(), valoresConceptosEmpleados["TipoCorrida".ToUpper()].ToString(), valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString(),
                   valoresConceptosEmpleados["NumPeriodo".ToUpper()].ToString(), (int)valoresConceptosEmpleados["ejercicioActivo".ToUpper()], valoresConceptosEmpleados["RazonSocial".ToUpper()].ToString(), ClavesParametrosModulos.claveBaseNominaISR.ToString());
                retenido = new CalculoISR();
                if (mensajeResultado.noError != 0)
                {
                    return 0.0;
                }
                double acumNorm = 0.0, acumDir = 0.0, acumAnu = 0.0;
                double dias;
                if (tipoTablaISR != TipoTablaISR.PERIODICIDAD)
                {
                    int i;
                    for (i = 0; i < acumulados.Length; i++)
                    {
                        if (acumulados[i, 1].Equals(0))
                        {
                            acumNorm = (Double)acumulados[i, 0];
                        }
                        else if (acumulados[i, 1].Equals(1))
                        {
                            acumDir = (Double)acumulados[i, 0];
                        }
                        else if (acumulados[i, 1].Equals(2))
                        {
                            acumAnu = (Double)acumulados[i, 0];
                        }
                    }
                    dias = 30.4;
                }
                else
                {
                    dias = Decimal.ToDouble(periodosNomina.tipoNomina.periodicidad.dias);
                }
                double baseGravableNormal = calculoISRNormal(acumNorm, acumDir, acumAnu, movimientosNomina, false);
                if (mensajeResultado.noError != 0)
                {
                    return 0.0;
                }
                calculoISRDirecto(baseGravableNormal, acumDir, movimientosNomina.tipoCorrida);
                if (mensajeResultado.noError != 0)
                {
                    return 0.0;
                }
                calculoISRAnual(baseGravableNormal, acumuladoDirecto + (acumDir), acumAnu, dias, movimientosNomina.tipoCorrida);
                if (mensajeResultado.noError != 0)
                {
                    return 0.0;
                }
                isrNormal = 0;
                isrNormalSinAjustar = 0;
                isrACargoNormalSinAjustar = 0;
                subsidioAlEmpleoNormalSinAjustar = 0;
                if (movimientosNomina != null)
                {
                    IsrRetenidos(movimientosNomina);
                    if (mensajeResultado.noError != 0)
                    {
                        return 0.0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("calculaISPTSeparado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return isrNormal + isrDirecto + isrAnual;
        }

        private void calculoISRAnual(Double baseGravableNormal, Double baseGravableDirecto, Double acumuladosEnPeriodoAnual, double dias, TipoCorrida tipoCorrida)
        {
            try
            {
                if (acumuladoAnual != 0.0)
                {
                    //Double acumuladoPeriodoAnual = acumuladosPorTipoISRAnual();
                    //se calcula cuando es el ultimo periodo del año
                    double baseGravableGlobal, baseGravableMensual;
                    baseGravableGlobal = baseGravableNormal + baseGravableDirecto + acumuladoAnual + acumuladosEnPeriodoAnual;
                    baseGravableMensual = (((baseGravableGlobal / 365) * dias));
                    ValorTablaISR valorTablaISR = aplicacionTablaISR(baseGravableMensual, true, tipoCorrida);
                    retenido.isrACargoAnual = valoresConceptosEmpleados["ISRACARGO".ToUpper()] == null ? 0.0 : (Double)valoresConceptosEmpleados["ISRACARGO".ToUpper()];
                    retenido.subsidioEmpleoAnual = valoresConceptosEmpleados["SubsEmpleoCalculado".ToUpper()] == null ? 0.0 : (double)valoresConceptosEmpleados["SubsEmpleoCalculado".ToUpper()];
                    //retenido.setIsrNetoAnual((Double) valoresConceptosEmpleados.get("ISRNeto".ToUpper()));
                    //retenido.setIsrSubsidioAnual((Double) valoresConceptosEmpleados.get("ISRSubsidio".ToUpper()));
                    if (mensajeResultado.noError != 0)
                    {
                        return;
                    }
                    isrAnual = (valorTablaISR.isrNeto / baseGravableGlobal);//Se obtiene un factor el cual se multiplica por la basegravableanualtotal.
                    isrAnual = acumuladoAnual * isrAnual;
                    retenido.isrACargoAnual = (retenido.isrACargoAnual / baseGravableMensual) * acumuladoAnual;
                    retenido.subsidioEmpleoAnual = (retenido.subsidioEmpleoAnual / baseGravableMensual) * acumuladoAnual;
                    if (isMov2Meses)
                    {
                        int diasPagoTotal = (int)valoresConceptosEmpleados["DiasNormalesAPagar".ToUpper()];
                        int diasPeriodo;
                        if (manejaPagoDiasNaturales)
                        {
                            diasPeriodo = (cantidadDiasEntreDosFechas(periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault()) + 1); //+1 para contar el dia actual
                        }
                        else
                        {
                            diasPeriodo = Convert.ToInt16(periodosNomina.tipoNomina.periodicidad.dias);
                        }
                        isrAnual = (isrAnual * diasPagoTotal) / diasPeriodo;
                        retenido.isrACargoAnual = (retenido.isrACargoAnual * diasPagoTotal) / diasPeriodo;
                        retenido.subsidioEmpleoAnual = (retenido.subsidioEmpleoAnual * diasPagoTotal) / diasPeriodo;
                    }
                }
                else
                {
                    acumuladoAnual = 0.0;
                    retenido.isrACargoAnual = 0.0;
                    retenido.subsidioEmpleoAnual = 0.0;
                    retenido.isrNetoAnual = 0.0;
                    retenido.isrSubsidioAnual = 0.0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("calculoISRAnual()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
        }

        private void calculoISRDirecto(Double baseGravableNormal, Double acumuladoPeriodosDirecto, TipoCorrida tipoCorrida)
        {
            try
            {
                if (acumuladoDirecto != 0.0)
                {
                    ValorTablaISR valorTablaISR = aplicacionTablaISR(baseGravableNormal + acumuladoDirecto + acumuladoPeriodosDirecto, false, tipoCorrida);
                    retenido.isrACargoDirecto = valoresConceptosEmpleados["ISRACARGO".ToUpper()] == null ? 0.0 : (Double)valoresConceptosEmpleados["ISRACARGO".ToUpper()];
                    retenido.subsidioEmpleoDirecto = valoresConceptosEmpleados["SubsEmpleoCalculado".ToUpper()] == null ? 0.0 : (double)valoresConceptosEmpleados["SubsEmpleoCalculado".ToUpper()];
                    //retenido.setIsrNetoDirecto((Double) valoresConceptosEmpleados.get("ISRNeto".ToUpper()));
                    //retenido.setIsrSubsidioDirecto((Double) valoresConceptosEmpleados.get("ISRSubsidio".ToUpper()));
                    if (mensajeResultado.noError != 0)
                    {
                        return;
                    }
                    isrDirecto = valorTablaISR.isrNeto - isrNormalSinAjustar;
                    retenido.isrACargoDirecto = retenido.isrACargoDirecto - isrACargoNormalSinAjustar;
                    retenido.subsidioEmpleoDirecto = retenido.subsidioEmpleoDirecto - subsidioAlEmpleoNormalSinAjustar;
                    if (isMov2Meses)
                    {
                        int diasPagoTotal = (int)valoresConceptosEmpleados["DiasNormalesAPagar".ToUpper()];
                        int diasPeriodo;
                        if (manejaPagoDiasNaturales)
                        {
                            diasPeriodo = (cantidadDiasEntreDosFechas(periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault()) + 1); //+1 para contar el dia actual
                        }
                        else
                        {
                            diasPeriodo = Convert.ToInt16(periodosNomina.tipoNomina.periodicidad.dias);
                        }
                        isrDirecto = (isrDirecto * diasPagoTotal) / diasPeriodo;
                        retenido.isrACargoDirecto = (retenido.isrACargoDirecto * diasPagoTotal) / diasPeriodo;
                        retenido.subsidioEmpleoDirecto = (retenido.subsidioEmpleoDirecto * diasPagoTotal) / diasPeriodo;
                    }
                }
                else
                {
                    retenido.isrACargoDirecto = 0.0;
                    retenido.subsidioEmpleoDirecto = 0.0;
                    retenido.isrNetoDirecto = 0.0;
                    retenido.isrSubsidioDirecto = 0.0;
                    isrDirecto = 0.0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("calculoISRDirecto()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
        }

        private Object[,] acumuladosPorTipoISR(string claveEmpleado, string referenciaPlazaEmp, string tipoCorrida, string tipoNomina, string clavePeriodo, int añoPeriodo, string claveRazonSocial, string claveBaseNomina)
        {
            Object[,] acumulados = null;
            // DbContext context = new DBContextSimple();
            try
            {
                strQuery.Remove(0, strQuery.Length).Append("SELECT SUM(CASE WHEN (mba.resultado is NULL) THEN 0.0 ELSE (mba.resultado * 1.0) END), ba.tipoAfecta FROM MovNomConcep m INNER JOIN m.periodosNomina p INNER JOIN p.tipoCorrida c INNER JOIN m.empleado em ");
                strQuery.Append("INNER JOIN m.tipoCorrida tc INNER JOIN m.movNomBaseAfecta mba INNER JOIN mba.baseAfecConcepNom ba INNER JOIN ba.baseNomina bn INNER JOIN m.tipoNomina t INNER JOIN m.razonesSociales rs INNER JOIN m.plazasPorEmpleado pemp ");
                //if (HibernateUtil.usaTypeBigInt)
                //{
                //    strQuery.append("WHERE m.uso = 0 AND p.clave < CAST(:clavePeriodoNomina as int) AND em.clave = :claveEmp  AND tc.clave = :claveTipoCorrida AND p.año = :yearPeriodo AND t.clave = :claveTipoNomina AND bn.clave = :claveBaseNomina ");
                //}
                //else
                //{
                strQuery.Append("WHERE m.uso = 0 AND p.clave < CAST(:clavePeriodoNomina as long) AND em.clave = :claveEmp  AND tc.clave = :claveTipoCorrida AND p.año = :yearPeriodo AND t.clave = :claveTipoNomina AND bn.clave = :claveBaseNomina ");
                //}
                strQuery.Append("AND rs.clave = :claveRazonSocial AND pemp.clave = :clavePlazaEmpleado  GROUP BY ba.tipoAfecta");
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                camposParametro.Add("claveEmp");
                camposParametro.Add("claveTipoCorrida");
                camposParametro.Add("clavePeriodoNomina");
                camposParametro.Add("claveTipoNomina");
                camposParametro.Add("claveBaseNomina");
                camposParametro.Add("claveRazonSocial");
                camposParametro.Add("clavePlazaEmpleado");
                camposParametro.Add("yearPeriodo");
                valoresParametro.Add(claveEmpleado);
                valoresParametro.Add(tipoCorrida);
                valoresParametro.Add(clavePeriodo);
                valoresParametro.Add(tipoNomina);
                valoresParametro.Add(claveBaseNomina); //BaseNomina ISR
                valoresParametro.Add(claveRazonSocial);
                valoresParametro.Add(referenciaPlazaEmp);
                valoresParametro.Add(añoPeriodo);
                IQuery q = getSession().CreateQuery(strQuery.ToString());
                q = cargarParametrosQuery(q, camposParametro, valoresParametro);
                IList<object> valores = q.List<object>();
                if (mensajeResultado.noError == -100)
                {
                    mensajeResultado.noError = 44;
                    return null;
                }

                int i, j;
                acumulados = new Object[valores.Count, 2];
                Object[] acumulad;
                for (i = 0; i < valores.Count; i++)
                {
                    acumulad = (Object[])valores[i];
                    for (j = 0; j < acumulad.Length; j++)
                    {
                        acumulados[i, j] = acumulad[j];
                    }
                }
                return acumulados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("acumuladosPorTipoISR()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return acumulados;
        }

        private double calculoISRNormal(Double acumuladoPeriodosNormal, Double acumuladoPeriodosDirecto, Double acumuladoPeriodosAnual, MovNomConcep movimientosNomina, bool isMesAjuste)
        {
            double baseGravable = 0;
            try
            {
                double diasMes = 1;  ////por que -1 pendiente                DiasPago       
                int diasPagoTotal = (int)valoresConceptosEmpleados["DiasNormalesAPagar".ToUpper()];
                if (factorMensual == 2)
                {//Factor mensual = 30.4 2 = Dias naturales
                    factorMensual = Convert.ToDouble(DateTime.DaysInMonth(periodosNomina.fechaInicial.GetValueOrDefault().Year, periodosNomina.fechaInicial.GetValueOrDefault().Month));
                }
                else
                {
                    factorMensual = 30.4;
                }
                DateTime fechaAlta = (DateTime)valoresConceptosEmpleados["FechaAlta".ToUpper()];
                DateTime fechaBaja = (DateTime)valoresConceptosEmpleados["FechaBaja".ToUpper()];
                string clavePeriodoNomina = valoresConceptosEmpleados["NumPeriodo".ToUpper()].ToString();
                string claveTipoNomina = valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString();
                string claveTipoCorrida = valoresConceptosEmpleados["TipoCorrida".ToUpper()].ToString();
                string claveEmpleado = valoresConceptosEmpleados["NumEmpleado".ToUpper()].ToString();
                int anioPeriodo = (int)valoresConceptosEmpleados["ejercicioActivo".ToUpper()];

                if (tipoTablaISR == TipoTablaISR.PERIODICIDAD)
                { //parametro por periodicidad activado utiliza tabla segun la periodicidad
                    if (!isMesAjuste)
                    {
                        baseGravable = acumuladoNormal;
                        if (modoAjustarIngresosMes == ProporcionaCadaPeriodoUtilizandoTablaPeriodo)
                        {//M5//BDEI01
                            diasPago = calculoDiasDepago(manejaPagoDiasNaturales, null, false, clavePeriodoNomina, claveTipoNomina, claveTipoCorrida, anioPeriodo, fechaAlta, fechaBaja,
                                claveEmpleado, periodosNomina);
                            baseGravable = acumuladoNormal + acumuladoDirecto + acumuladoAnual;

                        }
                    }
                    else
                    {

                        baseGravable = acumuladoPeriodosNormal;
                    }
                }
                else
                {//parametro por periodicidad desactivado utiliza tabla mensual
                    if (modoAjustarIngresosMes == ProporcionaPeriodoIndependiente
                            | modoAjustarIngresosMes == ProporcionaPeriodoAjustadoFinalMes
                            | modoAjustarIngresosMes == ProporcionaPeriodoAjustadoCadaVez
                            | modoAjustarIngresosMes == ProporcionaPeriodoConTablaAnual)
                    {//& !periodoAjustadoMes) { 
                        if (modoAjustarIngresosMes == ProporcionaPeriodoIndependiente
                                | (!periodosNomina.cierreMes & modoAjustarIngresosMes == ProporcionaPeriodoAjustadoFinalMes)
                                | (periodosNomina.cierreMes & modoAjustarIngresosMes == ProporcionaPeriodoAjustadoFinalMes & descontarFaltasModoAjustaMes)
                                | modoAjustarIngresosMes == ProporcionaPeriodoAjustadoCadaVez)
                        {
                            bool acumulaperiodos = false;
                            if (modoAjustarIngresosMes == ProporcionaPeriodoAjustadoCadaVez)
                            {
                                acumulaperiodos = true;
                            }

                            diasPago = calculoDiasDepago(manejaPagoDiasNaturales, null, acumulaperiodos, clavePeriodoNomina, claveTipoNomina, claveTipoCorrida, anioPeriodo, fechaAlta, fechaBaja,
                                claveEmpleado, periodosNomina);
                            if (mensajeResultado.noError != 0)
                            {
                                return 0.0;
                            }
                        }
                        if (modoAjustarIngresosMes == ProporcionaPeriodoIndependiente)
                        {
                            baseGravable = (acumuladoNormal / diasPago) * factorMensual.GetValueOrDefault();
                            if (acumuladoDirecto > 0)
                            {
                                baseGravable = baseGravable + acumuladoDirecto;
                            }
                            if (acumuladoAnual > 0)
                            {
                                baseGravable = baseGravable + acumuladoAnual;
                            }
                        }
                        else if (modoAjustarIngresosMes == ProporcionaPeriodoAjustadoFinalMes)
                        {
                            if (periodosNomina.cierreMes)
                            {
                                baseGravable = acumuladoNormal + acumuladoPeriodosNormal + acumuladoDirecto + acumuladoPeriodosDirecto;//BDEI01
                            }
                            else
                            {
                                // baseGravable = ((acumuladoNormal + acumuladoPeriodosNormal) / diasPago) * factorMensual;
                                baseGravable = (acumuladoNormal / diasPago) * factorMensual.GetValueOrDefault();
                                if (acumuladoDirecto > 0)
                                {
                                    baseGravable = baseGravable + acumuladoDirecto;
                                }
                                if (acumuladoAnual > 0)
                                {
                                    baseGravable = baseGravable + acumuladoAnual;
                                }
                            }
                        }
                        else if (modoAjustarIngresosMes == ProporcionaPeriodoAjustadoCadaVez)
                        {
                            if (periodosNomina.cierreMes)
                            {
                                baseGravable = acumuladoNormal + acumuladoPeriodosNormal + acumuladoDirecto + acumuladoPeriodosDirecto;
                            }
                            else if (acumuladoPeriodosNormal > 0 | acumuladoPeriodosDirecto > 0)
                            {
                                baseGravable = acumuladoNormal + acumuladoPeriodosNormal;
                                baseGravable = (baseGravable / diasPago) * factorMensual.GetValueOrDefault();
                                baseGravable = baseGravable + acumuladoDirecto + acumuladoAnual + acumuladoPeriodosDirecto;
                            }
                            else
                            {
                                baseGravable = (acumuladoNormal / diasPago) * factorMensual.GetValueOrDefault();
                                if (acumuladoDirecto > 0)
                                {
                                    baseGravable = baseGravable + acumuladoDirecto;
                                }
                                if (acumuladoAnual > 0)
                                {//
                                    baseGravable = baseGravable + acumuladoAnual;
                                }
                            }
                        }
                        else if (modoAjustarIngresosMes == ProporcionaPeriodoConTablaAnual)
                        {
                            calculoDiasTranscurridos();
                            baseGravable = ((acumuladoNormal + acumuladoPeriodosNormal) / diasPago) * factorAnual.GetValueOrDefault();
                            baseGravable = baseGravable + acumuladoPeriodosDirecto + acumuladoPeriodosAnual;

                            if (acumuladoDirecto > 0)
                            {
                                baseGravable = baseGravable + acumuladoDirecto;
                            }
                            if (acumuladoAnual > 0)
                            {//
                                baseGravable = baseGravable + acumuladoAnual;
                            }
                        }
                    }
                    else if (modoAjustarIngresosMes == 6 /*PropPeriodoIndepDiasNaturales*/ | modoAjustarIngresosMes == PropPeriodoIndepAjustadoAlUltimoPeriodoMes)
                    {// & !periodoAjustadoMes) {
                        if (/*modoAjustarIngresosMes == PropPeriodoIndepDiasNaturales |*/(!periodosNomina.cierreMes & modoAjustarIngresosMes == PropPeriodoIndepAjustadoAlUltimoPeriodoMes)
                                | (periodosNomina.cierreMes & modoAjustarIngresosMes == PropPeriodoIndepAjustadoAlUltimoPeriodoMes & descontarFaltasModoAjustaMes))
                        {
                            diasPago = calculoDiasDepago(manejaPagoDiasNaturales, null, modoAjustarIngresosMes == PropPeriodoIndepAjustadoAlUltimoPeriodoMes, clavePeriodoNomina, claveTipoNomina, claveTipoCorrida, anioPeriodo, fechaAlta, fechaBaja,
                                claveEmpleado, periodosNomina);
                            if (mensajeResultado.noError != 0)
                            {
                                return 0.0;
                            }

                            diasMes = Convert.ToDouble(DateTime.DaysInMonth(periodosNomina.fechaInicial.GetValueOrDefault().Year, periodosNomina.fechaInicial.GetValueOrDefault().Month));
                        }
                        if (modoAjustarIngresosMes == PropPeriodoIndepAjustadoAlUltimoPeriodoMes)
                        {
                            if (periodosNomina.cierreMes)
                            {
                                baseGravable = acumuladoNormal + acumuladoPeriodosNormal;
                            }
                            else
                            {
                                baseGravable = ((acumuladoNormal + acumuladoPeriodosNormal) / diasPago) * diasMes;
                            }
                        }
                        else
                        {
                            baseGravable = ((acumuladoNormal) / diasPago) * diasMes;
                        }
                    }
                    else if (periodosNomina.cierreMes & modoAjustarIngresosMes == UltimoPeriodoSinAjustarMes)
                    {// | periodoAjustadoMes) {
                        baseGravable = acumuladoNormal + acumuladoPeriodosNormal;
                    }
                    else
                    {  // periodosNomina no es cierre de mes & modoAjustarIngresosMes == UltimoPeriodoSinAjustarMes
                        diasPago = calculoDiasDepago(manejaPagoDiasNaturales, null, modoAjustarIngresosMes == UltimoPeriodoSinAjustarMes, clavePeriodoNomina, claveTipoNomina, claveTipoCorrida, anioPeriodo, fechaAlta, fechaBaja,
                            claveEmpleado, periodosNomina);
                        ///baseGravable = ((acumuladoNormal) / diasPago) * factorMensual;  //opc1
                        baseGravable = ((acumuladoNormal + acumuladoPeriodosNormal) / diasPago) * factorMensual.GetValueOrDefault();  // opc 2
                    }
                }
                if (baseGravable == Double.NaN || Double.IsPositiveInfinity(baseGravable) || Double.IsPositiveInfinity(baseGravable))
                {
                    baseGravable = 0.0;
                }
                if (baseGravable > 0 & !calculoSeparadoISR)
                {
                    valoresTablaISR = aplicacionTablaISR(baseGravable, false, movimientosNomina.tipoCorrida);
                    if (mensajeResultado.noError != 0)
                    {
                        return 0.0;
                    }
                    DateTime c = DateTime.Now;
                    if (isMov2Meses)
                    {
                        c = periodosNomina.fechaFinal.GetValueOrDefault();
                    }
                    isrNormalSinAjustar = valoresTablaISR.isrNeto;
                    isrACargoNormalSinAjustar = valoresTablaISR.isrCausado;
                    subsidioAlEmpleoNormalSinAjustar = valoresTablaISR.subsidioAlEmpleo;
                    if (tipoTablaISR == TipoTablaISR.PERIODICIDAD)
                    {
                        if (modoAjustarIngresosMes == ProporcionaCadaPeriodoUtilizandoTablaPeriodo)
                        {////Modo 5
                            if (isMov2Meses)
                            {
                                int diasTotales;
                                if (manejaPagoDiasNaturales)
                                {
                                    diasTotales = (cantidadDiasEntreDosFechas(periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault()) + 1); //+1 para contar el dia actual
                                }
                                else
                                {
                                    diasTotales = Convert.ToInt16(periodosNomina.tipoNomina.periodicidad.dias);
                                }
                                diasPago = calculoDiasDepago(manejaPagoDiasNaturales, diasPagoTotal, false, clavePeriodoNomina, claveTipoNomina, claveTipoCorrida, anioPeriodo, fechaAlta, fechaBaja,
                                    claveEmpleado, periodosNomina);
                                ///}
                                if (periodosNomina.cierreMes & modoAjustarIngresosMes == ProporcionaPeriodoAjustadoFinalMes)
                                {
                                    isrNormal = (valoresTablaISR.isrNeto * diasPago) / diasTotales;
                                    retenido.isrACargoNormal = (valoresTablaISR.isrCausado * diasPago) / diasTotales;
                                    retenido.subsidioEmpleoNormal = (valoresTablaISR.subsidioAlEmpleo * diasPago) / diasTotales;
                                }
                                else
                                {
                                    isrNormal = (valoresTablaISR.isrNeto / diasTotales) * diasPago;
                                    retenido.isrACargoNormal = (valoresTablaISR.isrCausado / diasTotales) * diasPago;
                                    retenido.subsidioEmpleoNormal = (valoresTablaISR.subsidioAlEmpleo / diasTotales) * diasPago;
                                }
                            }
                            else
                            {
                                //baseGravable = (baseGravable / diasPago) * diasPagoTotal;
                                isrNormal = (valoresTablaISR.isrNeto / diasPagoTotal) * diasPago;
                                retenido.isrACargoNormal = (valoresTablaISR.isrCausado / diasPagoTotal) * diasPago;
                                retenido.subsidioEmpleoNormal = (valoresTablaISR.subsidioAlEmpleo / diasPagoTotal) * diasPago;

                            }

                        }
                        else
                        {
                            if (periodosNomina.tipoNomina.periodicidad.dias == diasPagoTotal)
                            {
                                isrNormal = valoresTablaISR.isrNeto;
                                retenido.isrACargoNormal = valoresTablaISR.isrCausado;
                                retenido.subsidioEmpleoNormal = valoresTablaISR.subsidioAlEmpleo;
                            }
                            else
                            {
                                isrNormal = (valoresTablaISR.isrNeto * diasPagoTotal) / Convert.ToDouble(periodosNomina.tipoNomina.periodicidad.dias);
                                retenido.isrACargoNormal = (valoresTablaISR.isrCausado * diasPagoTotal) / Convert.ToDouble(periodosNomina.tipoNomina.periodicidad.dias);
                                retenido.subsidioEmpleoNormal = (valoresTablaISR.subsidioAlEmpleo * diasPagoTotal) / Convert.ToDouble(periodosNomina.tipoNomina.periodicidad.dias);
                            }
                        }
                    }
                    else if (modoAjustarIngresosMes == ProporcionaPeriodoIndependiente | modoAjustarIngresosMes == ProporcionaPeriodoAjustadoFinalMes)
                    {// & !periodoAjustadoMes) {
                        if (isMov2Meses)
                        {
                            ///if (!movimientosNomina.getMes().equals(c.get(Calendar.MONTH) + 1)) {
                            int diasTotales;
                            if (manejaPagoDiasNaturales)
                            {
                                diasTotales = (cantidadDiasEntreDosFechas(periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault()) + 1); //+1 para contar el dia actual
                            }
                            else
                            {
                                diasTotales = Convert.ToInt16(periodosNomina.tipoNomina.periodicidad.dias);
                            }
                            diasPago = calculoDiasDepago(manejaPagoDiasNaturales, diasPagoTotal, !periodosNomina.cierreMes & modoAjustarIngresosMes == ProporcionaPeriodoAjustadoFinalMes, clavePeriodoNomina, claveTipoNomina, claveTipoCorrida, anioPeriodo, fechaAlta, fechaBaja,
                                claveEmpleado, periodosNomina);
                            ///}
                            if (periodosNomina.cierreMes & modoAjustarIngresosMes == ProporcionaPeriodoAjustadoFinalMes)
                            {
                                isrNormal = (valoresTablaISR.isrNeto * diasPago) / diasTotales;
                                retenido.isrACargoNormal = (valoresTablaISR.isrCausado * diasPago) / diasTotales;
                                retenido.subsidioEmpleoNormal = (valoresTablaISR.subsidioAlEmpleo * diasPago) / diasTotales;
                            }
                            else
                            {
                                isrNormal = (valoresTablaISR.isrNeto / factorMensual.GetValueOrDefault()) * diasPago;
                                retenido.isrACargoNormal = (valoresTablaISR.isrCausado / factorMensual.GetValueOrDefault()) * diasPago;
                                retenido.subsidioEmpleoNormal = (valoresTablaISR.subsidioAlEmpleo / factorMensual.GetValueOrDefault()) * diasPago;
                            }
                        }
                        else if (periodosNomina.cierreMes & modoAjustarIngresosMes == ProporcionaPeriodoAjustadoFinalMes)
                        {
                            isrNormal = valoresTablaISR.isrNeto;
                            retenido.isrACargoNormal = valoresTablaISR.isrCausado;
                            retenido.subsidioEmpleoNormal = valoresTablaISR.subsidioAlEmpleo;
                        }
                        else
                        {
                            isrNormal = (valoresTablaISR.isrNeto / factorMensual.GetValueOrDefault()) * diasPago;//---here
                            retenido.isrACargoNormal = (valoresTablaISR.isrCausado / factorMensual.GetValueOrDefault()) * diasPago;
                            retenido.subsidioEmpleoNormal = (valoresTablaISR.subsidioAlEmpleo / factorMensual.GetValueOrDefault()) * diasPago;
                        }
                    }
                    else if (modoAjustarIngresosMes == PropPeriodoIndepDiasNaturales | modoAjustarIngresosMes == PropPeriodoIndepAjustadoAlUltimoPeriodoMes)
                    {// & !periodoAjustadoMes) {
                        if (isMov2Meses)
                        {
                            /// if (!movimientosNomina.getMes().equals(c.get(Calendar.MONTH) + 1)) {
                            int diasTotales;
                            if (manejaPagoDiasNaturales)
                            {
                                diasTotales = (cantidadDiasEntreDosFechas(periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault()) + 1); //+1 para contar el dia actual
                            }
                            else
                            {
                                diasTotales = Convert.ToInt16(periodosNomina.tipoNomina.periodicidad.dias);
                            }

                            diasPago = calculoDiasDepago(manejaPagoDiasNaturales, diasPagoTotal, !periodosNomina.cierreMes & modoAjustarIngresosMes == PropPeriodoIndepAjustadoAlUltimoPeriodoMes, clavePeriodoNomina, claveTipoNomina, claveTipoCorrida, anioPeriodo, fechaAlta, fechaBaja,
                                claveEmpleado, periodosNomina);
                            //}
                            if (periodosNomina.cierreMes & modoAjustarIngresosMes == PropPeriodoIndepAjustadoAlUltimoPeriodoMes)
                            {
                                isrNormal = (valoresTablaISR.isrNeto * diasPago) / diasTotales;
                                retenido.isrACargoNormal = (valoresTablaISR.isrCausado * diasPago) / diasTotales;
                                retenido.subsidioEmpleoNormal = (valoresTablaISR.subsidioAlEmpleo * diasPago) / diasTotales;
                            }
                            else
                            {
                                isrNormal = (valoresTablaISR.isrNeto / diasMes) * diasPago;
                                retenido.isrACargoNormal = (valoresTablaISR.isrCausado / diasMes) * diasPago;
                                retenido.subsidioEmpleoNormal = (valoresTablaISR.subsidioAlEmpleo / diasMes) * diasPago;
                            }
                        }
                        else if (periodosNomina.cierreMes & modoAjustarIngresosMes == PropPeriodoIndepAjustadoAlUltimoPeriodoMes)
                        {
                            isrNormal = valoresTablaISR.isrNeto;
                            retenido.isrACargoNormal = valoresTablaISR.isrCausado;
                            retenido.subsidioEmpleoNormal = valoresTablaISR.subsidioAlEmpleo;
                        }
                        else
                        {
                            isrNormal = (valoresTablaISR.isrNeto / diasMes) * diasPago;
                            retenido.isrACargoNormal = (valoresTablaISR.isrCausado / diasMes) * diasPago;
                            retenido.subsidioEmpleoNormal = (valoresTablaISR.subsidioAlEmpleo / diasMes) * diasPago;
                        }
                    }
                    else if (periodosNomina.cierreMes & modoAjustarIngresosMes == UltimoPeriodoSinAjustarMes)
                    {// | periodoAjustadoMes) {
                        if (isMov2Meses)
                        { //& (c == null ? false : !movimientosNomina.getMes().equals(c.get(Calendar.MONTH) + 1))
                            diasPago = calculoDiasDepago(manejaPagoDiasNaturales, diasPagoTotal, modoAjustarIngresosMes == UltimoPeriodoSinAjustarMes, clavePeriodoNomina, claveTipoNomina, claveTipoCorrida, anioPeriodo, fechaAlta, fechaBaja,
                                claveEmpleado, periodosNomina);
                            diasMes = calculoDiasDeMes(true, clavePeriodoNomina, claveTipoNomina, claveTipoCorrida, anioPeriodo, fechaAlta, fechaBaja, periodosNomina);
                            isrNormal = (valoresTablaISR.isrNeto / diasMes) * diasPago;
                            retenido.isrACargoNormal = (valoresTablaISR.isrCausado / diasMes) * diasPago;
                            retenido.subsidioEmpleoNormal = (valoresTablaISR.subsidioAlEmpleo / diasMes) * diasPago;
                        }
                        else
                        {
                            isrNormal = valoresTablaISR.isrNeto;
                            retenido.isrACargoNormal = valoresTablaISR.isrCausado;
                            retenido.subsidioEmpleoNormal = valoresTablaISR.subsidioAlEmpleo;
                        }
                    }
                    else if (periodosNomina.cierreMes & modoAjustarIngresosMes == ProporcionaPeriodoAjustadoCadaVez)
                    {//Modo 3//BDEI01
                        if (isMov2Meses)
                        { //& (c == null ? false : !movimientosNomina.getMes().equals(c.get(Calendar.MONTH) + 1))
                            diasPago = calculoDiasDepago(manejaPagoDiasNaturales, diasPagoTotal, false, clavePeriodoNomina, claveTipoNomina, claveTipoCorrida, anioPeriodo, fechaAlta, fechaBaja,
                                claveEmpleado, periodosNomina);
                            diasMes = periodosNomina.diasPago.GetValueOrDefault();
                            isrNormal = (valoresTablaISR.isrNeto / diasMes) * diasPago;
                            retenido.isrACargoNormal = (valoresTablaISR.isrCausado / diasMes) * diasPago;
                            retenido.subsidioEmpleoNormal = (valoresTablaISR.subsidioAlEmpleo / diasMes) * diasPago;
                        }
                        else
                        {
                            isrNormal = valoresTablaISR.isrNeto;
                            retenido.isrACargoNormal = valoresTablaISR.isrCausado;
                            retenido.subsidioEmpleoNormal = valoresTablaISR.subsidioAlEmpleo;
                        }

                    }
                    else if (modoAjustarIngresosMes == ProporcionaPeriodoConTablaAnual)
                    {//Modo 4//BDEI01
                        if (isMov2Meses)
                        {

                            //double diasPeriodo = periodosNomina.getDiasPago();
                            int diasTotales;
                            if (manejaPagoDiasNaturales)
                            {
                                diasTotales = (cantidadDiasEntreDosFechas(periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault()) + 1); //+1 para contar el dia actual
                            }
                            else
                            {
                                diasTotales = Convert.ToInt16(periodosNomina.tipoNomina.periodicidad.dias);
                            }

                            isrNormal = (valoresTablaISR.isrNeto / factorAnual.GetValueOrDefault()) * diasPago;
                            isrNormal = isrNormal / diasTotales * diasPagoTotal;
                            double retenidoCargo = (valoresTablaISR.isrCausado / factorAnual.GetValueOrDefault()) * diasTotales;
                            retenidoCargo = retenidoCargo / diasTotales * diasPagoTotal;
                            retenido.isrACargoNormal = retenidoCargo;
                            double subsidioRetenido = (valoresTablaISR.subsidioAlEmpleo / factorAnual.GetValueOrDefault()) * diasTotales;
                            subsidioRetenido = subsidioRetenido / diasPago * diasPagoTotal;
                            retenido.subsidioEmpleoNormal = subsidioRetenido;
                        }
                        else
                        {
                            isrNormal = (valoresTablaISR.isrNeto / factorAnual.GetValueOrDefault()) * diasPago;
                            retenido.isrACargoNormal = (valoresTablaISR.isrCausado / factorAnual.GetValueOrDefault()) * diasPago;
                            retenido.subsidioEmpleoNormal = (valoresTablaISR.subsidioAlEmpleo / factorAnual.GetValueOrDefault()) * diasPago;
                        }

                    }
                    else // periodosNomina no es cierre de mes & modoAjustarIngresosMes == UltimoPeriodoSinAjustarMes
                    {
                        if (isMov2Meses)
                        {
                            ///if (!movimientosNomina.getMes().equals(c.get(Calendar.MONTH) + 1)) {
                            diasPago = calculoDiasDepago(manejaPagoDiasNaturales, diasPagoTotal, modoAjustarIngresosMes == UltimoPeriodoSinAjustarMes, clavePeriodoNomina, claveTipoNomina, claveTipoCorrida, anioPeriodo, fechaAlta, fechaBaja,
                                claveEmpleado, periodosNomina);
                            ///}
                            isrNormal = (valoresTablaISR.isrNeto / factorMensual.GetValueOrDefault()) * diasPago;
                            retenido.isrACargoNormal = (valoresTablaISR.isrCausado / factorMensual.GetValueOrDefault()) * diasPago;
                            retenido.subsidioEmpleoNormal = (valoresTablaISR.subsidioAlEmpleo / factorMensual.GetValueOrDefault()) * diasPago;
                        }
                        else
                        {
                            isrNormal = (valoresTablaISR.isrNeto / factorMensual.GetValueOrDefault()) * diasPago;
                            retenido.isrACargoNormal = (valoresTablaISR.isrCausado / factorMensual.GetValueOrDefault()) * diasPago;
                            retenido.subsidioEmpleoNormal = (valoresTablaISR.subsidioAlEmpleo / factorMensual.GetValueOrDefault()) * diasPago;
                        }
                    }
                }
                else if (baseGravable == 0)
                {
                    retenido.isrACargoNormal = 0.0;
                    retenido.subsidioEmpleoNormal = 0.0;
                    retenido.isrNetoNormal = 0.0;
                    retenido.isrSubsidioNormal = 0.0;
                    isrNormal = 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("calculoISRNormal()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();

            }
            return baseGravable;
        }

        private int calculoDiasDepago(bool calculaDiasNaturales, int? diasDivPor2Meses, bool acumularPeriodos, string clavePeriodoNomina, string claveTipoNomina, string claveTipoCorrida, int anioPeriodo, DateTime fechaAlta, DateTime fechaBaja, string claveEmpleado, PeriodosNomina periodosNomina)
        {
            int diasPago = 0;
            IList<PeriodosNomina> listPeriodosNominas;
            try
            {
                if (acumularPeriodos)
                {
                    //if (HibernateUtil.usaTypeBigInt)
                    //{
                    //    strQuery.delete(0, strQuery.length()).append("SELECT DISTINCT p ");//JSA10
                    //    strQuery.append(" FROM MovNomConcep mov INNER JOIN mov.periodosNomina p INNER JOIN p.tipoNomina t INNER JOIN p.tipoCorrida c INNER JOIN mov.empleado emp  WHERE  ");
                    //    strQuery.append(" (p.clave > (SELECT CASE WHEN (count(pn) > 0) THEN MAX(CAST(pn.clave as int)) ELSE 0 END FROM PeriodosNomina pn INNER JOIN pn.tipoNomina tn INNER JOIN pn.tipoCorrida tc ");
                    //    strQuery.append(" WHERE pn.clave < cast(:clavePeriodoNomina as int) AND tn.clave = :claveTipoNomina AND tc.clave = :claveTipoCorrida ");
                    //    strQuery.append(" AND pn.cierreMes = true AND pn.año = :yearPeriodo) AND p.clave < cast(:clavePeriodoNomina as int)) AND t.clave = :claveTipoNomina AND p.año = :yearPeriodo AND c.clave = :claveTipoCorrida ");
                    //    strQuery.append(" AND :fechaIngresoEmp  <= p.fechaFinal AND  :fechaFinEmp  >= p.fechaInicial ");
                    //    strQuery.append(" AND emp.clave = :empleado ");
                    //}
                    //else
                    //{
                    strQuery.Remove(0, strQuery.Length).Append("SELECT DISTINCT p ");
                    strQuery.Append(" FROM MovNomConcep mov INNER JOIN mov.periodosNomina p INNER JOIN p.tipoNomina t INNER JOIN p.tipoCorrida c INNER JOIN mov.empleado emp  WHERE  ");
                    strQuery.Append(" (p.clave > (SELECT CASE WHEN (count(pn) > 0) THEN MAX(CAST(pn.clave as long)) ELSE 0 END FROM PeriodosNomina pn INNER JOIN pn.tipoNomina tn INNER JOIN pn.tipoCorrida tc");
                    strQuery.Append(" WHERE pn.clave < cast(:clavePeriodoNomina as long) AND tn.clave = :claveTipoNomina AND tc.clave = :claveTipoCorrida");
                    strQuery.Append(" AND pn.cierreMes = true AND pn.año = :yearPeriodo) AND p.clave < cast(:clavePeriodoNomina as long)) AND t.clave = :claveTipoNomina AND p.año = :yearPeriodo AND c.clave = :claveTipoCorrida ");
                    strQuery.Append(" AND :fechaIngresoEmp  <= p.fechaFinal AND  :fechaFinEmp  >= p.fechaInicial ");
                    strQuery.Append(" AND emp.clave = :empleado ");
                    //}
                    strQuery.Append("order by p.clave");
                    camposParametro = new List<String>(0);
                    valoresParametro = new List<Object>(0);
                    camposParametro.Add("clavePeriodoNomina");
                    camposParametro.Add("claveTipoNomina");
                    camposParametro.Add("claveTipoCorrida");
                    camposParametro.Add("yearPeriodo");
                    camposParametro.Add("fechaIngresoEmp");
                    camposParametro.Add("fechaFinEmp");
                    camposParametro.Add("empleado");
                    valoresParametro.Add(clavePeriodoNomina);
                    valoresParametro.Add(claveTipoNomina);
                    valoresParametro.Add(claveTipoCorrida);
                    valoresParametro.Add(anioPeriodo);
                    valoresParametro.Add(fechaAlta);
                    valoresParametro.Add(fechaBaja);
                    valoresParametro.Add(claveEmpleado);
                    IQuery q = getSession().CreateQuery(strQuery.ToString());
                    q = cargarParametrosQuery(q, camposParametro, valoresParametro);
                    listPeriodosNominas = q.List<PeriodosNomina>();
                    //listPeriodosNominas = (List<PeriodosNomina>)ejecutaQueryList(strQuery.toString(), camposParametro.toArray(new String[] { }), valoresParametro.toArray(), 0);
                    listPeriodosNominas = (listPeriodosNominas == null) ? new List<PeriodosNomina>() : listPeriodosNominas;
                }
                else
                {
                    listPeriodosNominas = new List<PeriodosNomina>();
                }

                double diasVacacionesAcum = 0.0, diasIncapacidadEmpleadoAcum = 0.0, faltasAcum = 0.0, faltasAcumAusentismoAcum = 0.0;
                if (listPeriodosNominas.Count > 0)
                {
                    DateTime fechaIni = listPeriodosNominas[0].fechaInicial.GetValueOrDefault(), fechafin = listPeriodosNominas[(listPeriodosNominas.Count - 1)].fechaFinal.GetValueOrDefault();
                    cargarVariablesEmpleadoAsistencias(fechaIni, fechafin, null, null, true);
                    PlazasPorEmpleadosMov plaza = (PlazasPorEmpleadosMov)valoresConceptosEmpleados["PlazaEmpleadoMovimiento".ToUpper()];
                    cargarVariablesEmpleadoVacaciones(fechaIni, fechafin, null, plaza, true);
                    diasVacacionesAcum = (valoresConceptosEmpleados["diasVacacionesAcum".ToUpper()] == null ? 0.0 : Convert.ToDouble(valoresConceptosEmpleados["diasVacacionesAcum".ToUpper()]));
                    diasIncapacidadEmpleadoAcum = (valoresConceptosEmpleados["DiasIncapacidadEmpleadoAcum".ToUpper()] == null ? 0.0 : Convert.ToDouble(valoresConceptosEmpleados["DiasIncapacidadEmpleadoAcum".ToUpper()]));
                    if (descontarFaltasModoAjustaMes)
                    {
                        faltasAcum = (valoresConceptosEmpleados["FaltasAcum".ToUpper()] == null ? 0.0 : Convert.ToDouble(valoresConceptosEmpleados["FaltasAcum".ToUpper()]));
                        faltasAcumAusentismoAcum = (valoresConceptosEmpleados["AusentismoAcum".ToUpper()] == null ? 0.0 : Convert.ToDouble(valoresConceptosEmpleados["AusentismoAcum".ToUpper()]));
                    }
                }
                int dias = 0;
                listPeriodosNominas.Add(periodosNomina);
                int i;
                for (i = 0; i < listPeriodosNominas.Count; i++)
                {
                    if (!calculaDiasNaturales)
                    {
                        if (diasDivPor2Meses != null & i == listPeriodosNominas.Count - 1)
                        {
                            dias = dias + diasDivPor2Meses.GetValueOrDefault();
                        }
                        else
                        {
                            dias = dias + Convert.ToInt16(listPeriodosNominas[i].tipoNomina.periodicidad.dias);
                        }
                    }
                    else if (diasDivPor2Meses != null & i == listPeriodosNominas.Count - 1)
                    {
                        dias = dias + diasDivPor2Meses.GetValueOrDefault();
                    }
                    else
                    {
                        dias = dias + (cantidadDiasEntreDosFechas(listPeriodosNominas[i].fechaInicial.GetValueOrDefault(), listPeriodosNominas[i].fechaFinal.GetValueOrDefault()) + 1); //+1 para contar el dia actual
                    }


                    if (fechaAlta.CompareTo(listPeriodosNominas[i].fechaInicial) > 0)  //after
                    {
                        dias = dias - cantidadDiasEntreDosFechas(listPeriodosNominas[i].fechaInicial.GetValueOrDefault(), fechaAlta);
                    }
                    if (fechaBaja.CompareTo(listPeriodosNominas[i].fechaFinal) < 0) //before
                    {
                        dias = dias - cantidadDiasEntreDosFechas(fechaBaja, listPeriodosNominas[i].fechaFinal.GetValueOrDefault());
                    }

                    if (String.Equals(claveTipoCorrida, "VAC", StringComparison.OrdinalIgnoreCase))
                    {//JDRA01
                        int diasvac = (int)valoresConceptosEmpleados["diasVacaciones".ToUpper()];
                        diasPago = dias - Convert.ToInt16(descontarDiasPago) + diasvac;
                    }
                    else
                    {
                        diasPago = dias - Convert.ToInt16(descontarDiasPago);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("calculoDiasDepago()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();

            }
            return diasPago;
        }

        private int calculoDiasDeMes(bool calculaDiasNaturales, string clavePeriodoNomina, string claveTipoNomina, string claveTipoCorrida, int anioPeriodo, DateTime fechaAlta, DateTime fechaBaja, PeriodosNomina periodoNomina)
        {
            try
            {
                strQuery.Remove(0, strQuery.Length).Append("SELECT p ");
                strQuery.Append("FROM PeriodosNomina p INNER JOIN p.tipoNomina t INNER JOIN p.tipoCorrida tc INNER JOIN t.periodicidad pd WHERE ");

                //if (HibernateUtil.usaTypeBigInt)
                //{
                //    strQuery.append("(p.clave > (SELECT CASE WHEN (count(pn) > 0) THEN MAX(CAST(pn.clave as int)) ELSE 0 END FROM PeriodosNomina pn INNER JOIN pn.tipoNomina tn INNER JOIN pn.tipoCorrida c WHERE pn.clave < cast(:clavePeriodoNomina as int) AND tn.clave = :claveTipoNomina AND pn.cierreMes = true AND pn.año = :yearPeriodo AND c.clave = :claveTipoCorrida) ");
                //    camposParametro = new ArrayList<String>(0);
                //    valoresParametro = new ArrayList<Object>(0);
                //    if (periodosNomina.isCierreMes())
                //    {
                //        strQuery.append("AND p.clave <= cast(:clavePeriodoNomina as int)) AND t.clave = :claveTipoNomina AND p.año = :yearPeriodo AND tc.clave = :claveTipoCorrida AND :fechaIngresoEmp <= p.fechaFinal AND :fechaFinEmp >= p.fechaInicial");
                //    }
                //    else
                //    {
                //        strQuery.append("AND p.clave <= (SELECT CASE WHEN (count(pn) > 0) THEN MIN(CAST(pn.clave as int)) ELSE 0 END FROM PeriodosNomina pn INNER JOIN pn.tipoNomina tn INNER JOIN pn.tipoCorrida c WHERE pn.clave > cast(:clavePeriodoNomina as int) AND tn.clave = :claveTipoNomina AND pn.cierreMes = true AND pn.año = :yearPeriodo AND c.clave = :claveTipoCorrida)) ");
                //        strQuery.append("AND t.clave = :claveTipoNomina AND p.año = :yearPeriodo AND tc.clave = :claveTipoCorrida AND  :fechaIngresoEmp <= p.fechaFinal AND :fechaFinEmp >= p.fechaInicial");
                //    }
                //}
                //else
                //{
                strQuery.Append("(p.clave > (SELECT CASE WHEN (count(pn) > 0) THEN MAX(CAST(pn.clave as long)) ELSE 0 END FROM PeriodosNomina pn INNER JOIN pn.tipoNomina tn INNER JOIN pn.tipoCorrida c WHERE pn.clave < cast(:clavePeriodoNomina as long) AND tn.clave = :claveTipoNomina AND pn.cierreMes = true AND pn.año = :yearPeriodo AND c.clave = :claveTipoCorrida) ");
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                if (periodosNomina.cierreMes)
                {
                    strQuery.Append("AND p.clave <= cast(:clavePeriodoNomina as long)) AND t.clave = :claveTipoNomina AND p.año = :yearPeriodo AND tc.clave = :claveTipoCorrida AND :fechaIngresoEmp <= p.fechaFinal AND :fechaFinEmp >= p.fechaInicial");
                }
                else
                {
                    strQuery.Append("AND p.clave <= (SELECT CASE WHEN (count(pn) > 0) THEN MIN(CAST(pn.clave as long)) ELSE 0 END FROM PeriodosNomina pn INNER JOIN pn.tipoNomina tn INNER JOIN pn.tipoCorrida c WHERE pn.clave > cast(:clavePeriodoNomina as long) AND tn.clave = :claveTipoNomina AND pn.cierreMes = true AND pn.año = :yearPeriodo AND c.clave = :claveTipoCorrida)) ");
                    strQuery.Append("AND t.clave = :claveTipoNomina AND p.año = :yearPeriodo AND tc.clave = :claveTipoCorrida AND :fechaIngresoEmp <= p.fechaFinal AND :fechaFinEmp >= p.fechaInicial");
                }
                //}

                camposParametro.Add("clavePeriodoNomina");
                camposParametro.Add("claveTipoNomina");
                camposParametro.Add("yearPeriodo");
                camposParametro.Add("claveTipoCorrida");
                camposParametro.Add("fechaIngresoEmp");
                camposParametro.Add("fechaFinEmp");
                valoresParametro.Add(clavePeriodoNomina);
                valoresParametro.Add(claveTipoNomina);
                valoresParametro.Add(anioPeriodo);
                valoresParametro.Add(claveTipoCorrida);
                valoresParametro.Add(fechaAlta);
                valoresParametro.Add(fechaBaja);

                IQuery q = getSession().CreateQuery(strQuery.ToString());
                q = cargarParametrosQuery(q, camposParametro, valoresParametro);
                IList<PeriodosNomina> periodosNominas = q.List<PeriodosNomina>();
                periodosNominas = (periodosNominas == null) ? new List<PeriodosNomina>() : periodosNominas;
                int dias = 0;
                foreach (PeriodosNomina p in periodosNominas)
                {
                    if (!calculaDiasNaturales)
                    {
                        dias = dias + Convert.ToInt16(p.tipoNomina.periodicidad.dias);
                    }
                    else
                    {
                        dias = dias + (cantidadDiasEntreDosFechas(p.fechaInicial.GetValueOrDefault(), p.fechaFinal.GetValueOrDefault()) + 1); //+1 para contar el dia actual
                    }
                }
                return dias;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("calculoDiasDeMes()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return 0;
        }

        private double calculaISPT(MovNomConcep movimientosNomina)
        {
            try
            {
                retenido = new CalculoISR();
                Object[,] acumulados;
                double acumNorm = 0.0, acumDir = 0.0, acumAnu = 0.0;
                double dias;
                valoresConceptosEmpleados["AjusteSubCausado".ToUpper()] = 0.0;
                valoresConceptosEmpleados["AjusteIsrMes".ToUpper()] = 0.0;
                valoresConceptosEmpleados["AjusteSubPagado".ToUpper()] = 0.0;
                if (tipoTablaISR != TipoTablaISR.PERIODICIDAD)
                {//quitar esta validacion por que va hacer por periodicidad
                    if (modoAjustarIngresosMes == ProporcionaPeriodoConTablaAnual)
                    {//quitar por que siempre va hacer ajustando al mes
                        acumulados = acumuladosISRAnualPorTipoAfecta(valoresConceptosEmpleados["NumEmpleado".ToUpper()].ToString(), valoresConceptosEmpleados["PlazaEmpleado".ToUpper()].ToString(), valoresConceptosEmpleados["TipoCorrida".ToUpper()].ToString(), valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString(),
                            valoresConceptosEmpleados["NumPeriodo".ToUpper()].ToString(), (int)valoresConceptosEmpleados["ejercicioActivo".ToUpper()], valoresConceptosEmpleados["RazonSocial".ToUpper()].ToString(), ClavesParametrosModulos.claveBaseNominaISR.ToString());
                    }
                    else
                    {
                        acumulados = acumuladosPorTipoISR(valoresConceptosEmpleados["NumEmpleado".ToUpper()].ToString(), valoresConceptosEmpleados["PlazaEmpleado".ToUpper()].ToString(), valoresConceptosEmpleados["TipoCorrida".ToUpper()].ToString(), valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString(),
                            valoresConceptosEmpleados["NumPeriodo".ToUpper()].ToString(), (int)valoresConceptosEmpleados["ejercicioActivo".ToUpper()], valoresConceptosEmpleados["RazonSocial".ToUpper()].ToString(), ClavesParametrosModulos.claveBaseNominaISR.ToString());
                    }

                    if (mensajeResultado.noError != 0 /*|| acumulados == null*/)
                    {
                        return 0.0;
                    }
                    int i;
                    if (acumulados != null)
                    {
                        for (i = 0; i < acumulados.GetLength(0); i++)
                        {
                            if (acumulados[i, 1].Equals(0))
                            {
                                acumNorm = (Double)acumulados[i, 0];
                            }
                            else if (acumulados[i, 1].Equals(1))
                            {
                                acumDir = (Double)acumulados[i, 0];
                            }
                            else if (acumulados[i, 1].Equals(2))
                            {
                                acumAnu = (Double)acumulados[i, 0];
                            }
                        }
                    }
                    dias = 30.4;
                }
                else
                {
                    dias = Convert.ToDouble(periodosNomina.tipoNomina.periodicidad.dias);
                }
                double baseGravableActual = calculoISRNormal(acumNorm, acumDir, acumAnu, movimientosNomina, false);
                ValorTablaISR valoresTablasISRper = valoresTablaISR;
                if (mensajeResultado.noError != 0)
                {
                    return 0.0;
                }
                if (movimientosNomina != null)
                {
                    IsrRetenidos(movimientosNomina);
                    if (mensajeResultado.noError != 0)
                    {
                        return 0.0;
                    }

                    if (periodosNomina.cierreMes == true)
                    {
                        acumulados = acumuladosPorTipoISR(valoresConceptosEmpleados["NumEmpleado".ToUpper()].ToString(), valoresConceptosEmpleados["PlazaEmpleado".ToUpper()].ToString(), valoresConceptosEmpleados["TipoCorrida".ToUpper()].ToString(), valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString(),
                           valoresConceptosEmpleados["NumPeriodo".ToUpper()].ToString(), (int)valoresConceptosEmpleados["ejercicioActivo".ToUpper()], valoresConceptosEmpleados["RazonSocial".ToUpper()].ToString(), ClavesParametrosModulos.claveBaseNominaISR.ToString());

                        if (acumulados != null)
                        {
                            for (int i = 0; i < acumulados.GetLength(0); i++)
                            {
                                if (acumulados[i, 1].Equals(0))
                                {
                                    acumNorm = (Double)acumulados[i, 0];
                                }
                                else if (acumulados[i, 1].Equals(1))
                                {
                                    acumDir = (Double)acumulados[i, 0];
                                }
                                else if (acumulados[i, 1].Equals(2))
                                {
                                    acumAnu = (Double)acumulados[i, 0];
                                }
                            }

                            acumNorm = acumNorm + baseGravableActual;
                        }
                        retenido = new CalculoISR();
                        double baseGavableAlMes = calculoISRNormal(acumNorm, acumDir, acumAnu, movimientosNomina, true);
                        ValorTablaISR valoresTablaISRMes = valoresTablaISR;
                        double subsidioCausadoAcum = buscaSubsidioCausado();
                        double ajusteSub = valoresTablaISRMes.subsidioAlEmpleo - subsidioCausadoAcum;
                        if (valoresTablaISRMes.subsidioAlEmpleo > subsidioCausadoAcum)
                        {

                            valoresConceptosEmpleados["SubsEmpleoCalculado".ToUpper()] = ajusteSub;
                            //valoresTablasISRper.subsidioAlEmpleo = ajusteSub;
                        }
                        else if (valoresTablaISRMes.subsidioAlEmpleo < subsidioCausadoAcum)
                        {
                            valoresConceptosEmpleados["AjusteSubCausado".ToUpper()] = Math.Abs(ajusteSub);
                            valoresConceptosEmpleados["SubsEmpleoCalculado".ToUpper()] = 0.0;
                        }
                        double[] isrSubAcumAnte = buscaISRRetenidoAnt();
                        double isrAcumAnt = isrSubAcumAnte[0];
                        double subAcumAnt = isrSubAcumAnte[1];
                        if (isrAcumAnt > 0)
                        {
                            double ajusteIsr = valoresTablaISRMes.isrNeto - isrAcumAnt;
                            if (ajusteIsr > 0)
                            {
                                isrNormal = ajusteIsr;
                            }
                            else if (ajusteIsr < 0)
                            {
                                valoresConceptosEmpleados["AjusteIsrMes".ToUpper()] = Math.Abs(ajusteIsr);
                            }
                        }

                        if (subAcumAnt > 0)
                        {
                            double isrSub = valoresTablaISRMes.isrCausado - valoresTablaISRMes.subsidioAlEmpleo;

                            if (isrSub > 0)
                            {
                                isrSub = 0.0;
                            }
                            else
                            {
                                isrSub = Math.Abs(isrSub);
                            }


                            double ajusteIsrSub = isrSub - subAcumAnt;
                            if (ajusteIsrSub > 0)
                            {
                                valoresConceptosEmpleados["ISRSubsidio".ToUpper()] = ajusteIsrSub;
                            }
                            else if (ajusteIsrSub < 0)
                            {
                                valoresConceptosEmpleados["AjusteSubPagado".ToUpper()] = Math.Abs(ajusteIsrSub);
                                valoresConceptosEmpleados["ISRSubsidio".ToUpper()] = 0.0;


                            }
                        }



                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("calculaISPT()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return isrNormal;////BDEI01 + isrDirecto + isrAnual;
        }

        #endregion

        private void calculoDiasTranscurridos()
        {
            try
            {
                strQuery.Remove(0, strQuery.Length).Append("SELECT  DISTINCT p FROM MovNomConcep mov INNER JOIN mov.periodosNomina p INNER JOIN mov.tipoNomina t INNER JOIN ");//JSA10
                strQuery.Append(" mov.empleado emp INNER JOIN p.tipoCorrida c ");
                //if (HibernateUtil.usaTypeBigInt)
                //{
                //    strQuery.append(" WHERE p.clave < CAST(:clavePeriodoNomina as int) AND t.clave = :claveTipoNomina AND p.año = :yearPeriodo AND c.clave =:claveTipoCorrida AND emp.clave = :empleado ");
                //}
                //else
                //{
                strQuery.Append(" WHERE p.clave < CAST(:clavePeriodoNomina as long) AND t.clave = :claveTipoNomina AND p.año = :yearPeriodo AND c.clave =:claveTipoCorrida AND emp.clave = :empleado ");
                //}
                strQuery.Append("order by p.clave");
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                camposParametro.Add("clavePeriodoNomina");
                camposParametro.Add("claveTipoNomina");
                camposParametro.Add("claveTipoCorrida");
                camposParametro.Add("yearPeriodo");
                camposParametro.Add("empleado");
                valoresParametro.Add(valoresConceptosEmpleados["NumPeriodo".ToUpper()]);
                valoresParametro.Add(valoresConceptosEmpleados["TipoNomina".ToUpper()]);
                valoresParametro.Add(valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()]);
                valoresParametro.Add(valoresConceptosGlobales["ejercicioActivo".ToUpper()]);
                valoresParametro.Add(valoresConceptosEmpleados["NumEmpleado".ToUpper()]);

                decimal dias = 0;
                DateTime fechaAlta = (DateTime)valoresConceptosEmpleados["FechaAlta".ToUpper()];
                DateTime fechaBaja = (DateTime)valoresConceptosEmpleados["FechaBaja".ToUpper()];
                IQuery q = getSession().CreateQuery(strQuery.ToString());
                q = cargarParametrosQuery(q, camposParametro, valoresParametro);
                //  List<PeriodosNomina> periodosNominas = (List<PeriodosNomina>)ejecutaQueryList(strQuery.ToString(), camposParametro.ToArray<string>(), valoresParametro.ToArray(), 0);
                IList<PeriodosNomina> periodosNominas = q.List<PeriodosNomina>();
                periodosNominas = (periodosNominas == null) ? new List<PeriodosNomina>() : periodosNominas;
                periodosNominas.Add(periodosNomina);

                double diasVacacionesAcum = 0.0, diasIncapacidadEmpleadoAcum = 0.0, faltasAcum = 0.0, faltasAcumAusentismoAcum = 0.0;
                if (periodosNominas.Count > 0)
                {
                    DateTime fechaIni = periodosNominas[0].fechaInicial.GetValueOrDefault(), fechafin = periodosNominas[(periodosNominas.Count - 1)].fechaFinal.GetValueOrDefault();
                    cargarVariablesEmpleadoAsistencias(fechaIni, fechafin, null, null, true);
                    PlazasPorEmpleadosMov plaza = (PlazasPorEmpleadosMov)valoresConceptosEmpleados["PlazaEmpleadoMovimiento".ToUpper()];
                    cargarVariablesEmpleadoVacaciones(fechaIni, fechafin, null, plaza, true);
                    diasVacacionesAcum = (valoresConceptosEmpleados["diasVacacionesAcum".ToUpper()] == null ? 0.0 : (double)valoresConceptosEmpleados["diasVacacionesAcum".ToUpper()]);
                    diasIncapacidadEmpleadoAcum = (valoresConceptosEmpleados["DiasIncapacidadEmpleadoAcum".ToUpper()] == null ? 0.0 : (double)valoresConceptosEmpleados["DiasIncapacidadEmpleadoAcum".ToUpper()]);
                    if (descontarFaltasModoAjustaMes)
                    {
                        faltasAcum = (valoresConceptosEmpleados["FaltasAcum".ToUpper()] == null ? 0.0 : (double)valoresConceptosEmpleados["FaltasAcum".ToUpper()]);
                        faltasAcumAusentismoAcum = (valoresConceptosEmpleados["AusentismoAcum".ToUpper()] == null ? 0.0 : (double)valoresConceptosEmpleados["AusentismoAcum".ToUpper()]);
                    }
                }

                foreach (PeriodosNomina p in periodosNominas)
                {
                    dias = dias + p.tipoNomina.periodicidad.dias;
                    if (fechaAlta.CompareTo(p.fechaInicial) > 0) //  After > 0
                    {
                        dias = dias - cantidadDiasEntreDosFechas(p.fechaInicial.GetValueOrDefault(), fechaAlta);
                    }
                    if (fechaBaja.CompareTo(p.fechaFinal) < 0) //before < 0
                    {
                        dias = dias - cantidadDiasEntreDosFechas(fechaBaja, p.fechaFinal.GetValueOrDefault()) + 1;
                    }
                }
                diasPago = Convert.ToDouble(dias);
                diasPago -= diasVacacionesAcum + diasIncapacidadEmpleadoAcum + faltasAcum + faltasAcumAusentismoAcum;


            }
            catch (Exception e)
            {
                mensajeResultado.error = e.GetBaseException().Message;
                mensajeResultado.noError = 35;

            }
        }

        private double aplicarMascara(ConcepNomDefi concepNomDefi, double resultado, bool obligarTruncar)
        {//JSA15
            try
            {
                string[] mascaraSeparada = null;
                string[] resultadoSeparado;
                TipoAccionMascaras tipoAccionMascaras = TipoAccionMascaras.Ninguno;
                double factor = factorRedondeoGral, minimun = minimoRedondeoGral;
                if (concepNomDefi == null ? false : (concepNomDefi.tipoAccionMascaras != TipoAccionMascaras.Ninguno))
                {
                    tipoAccionMascaras = concepNomDefi.tipoAccionMascaras;
                }
                else
                {
                    tipoAccionMascaras = tipoAccionMascarasGral;
                }
                Double resultWithMask;
                if (concepNomDefi == null ? false : (concepNomDefi.mascara == null ? false : concepNomDefi.mascara.Length > 0))
                {
                    if (concepNomDefi.mascara.Contains("."))
                    {
                        mascaraSeparada = concepNomDefi.mascara.Split('.');
                    }
                    else
                    {
                        mascaraSeparada = new String[] { concepNomDefi.mascara, "" };
                    }
                    if (mascaraSeparada[1].Length > 0)
                    {
                        String factorString = ".", minimunString = ".";
                        for (int i = 0; i < mascaraSeparada[1].Length - 1; i++)
                        {
                            factorString += "0";
                        }
                        minimunString = factorString;
                        factorString += "1";
                        minimunString += "05";
                        factor = Double.Parse(factorString);
                        minimun = Double.Parse(minimunString);
                    }
                }
                else if (mascaraResultadoGral != null)
                {
                    mascaraSeparada = mascaraResultadoGral;
                }
                if (tipoAccionMascaras == TipoAccionMascaras.Redondear & !obligarTruncar)
                {
                    resultWithMask = RoundingANumber.round(resultado, factor, minimun);
                    if (resultado != resultWithMask)
                    {
                        if (resultado.ToString().Contains("."))
                        {
                            resultadoSeparado = resultado.ToString().Split('.');
                        }
                        else
                        {
                            resultadoSeparado = new String[] { resultado.ToString(), "" };
                        }
                        if (resultadoSeparado[1].Length > mascaraSeparada[1].Length)
                        {
                            resultadoSeparado[1] = resultadoSeparado[1].Substring(0, mascaraSeparada[1].Length);
                        }
                        StringBuilder builder = new StringBuilder(resultadoSeparado[0]);
                        if (resultadoSeparado[1].Length > 0)
                        {
                            builder.Append(".").Append(resultadoSeparado[1]);
                        }
                        resultado = Double.Parse(builder.ToString());
                        importeRedondeo += resultWithMask - resultado;
                    }
                }
                else
                {
                    if (resultado.ToString().Contains("."))
                    {
                        resultadoSeparado = resultado.ToString().Split('.');
                    }
                    else
                    {
                        resultadoSeparado = new String[] { resultado.ToString(), "" };
                    }
                    if (resultadoSeparado[1].Length > mascaraSeparada[1].Length)
                    {
                        resultadoSeparado[1] = resultadoSeparado[1].Substring(0, mascaraSeparada[1].Length);
                    }
                    StringBuilder builder = new StringBuilder(resultadoSeparado[0]);
                    if (resultadoSeparado[1].Length > 0)
                    {
                        builder.Append(".").Append(resultadoSeparado[1]);
                    }
                    resultWithMask = Double.Parse(builder.ToString());
                }
                return resultWithMask;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("aplicarMascara()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return 0;
        }

        private double[] baseGravableAcumuladaMesAnterior(DateTime fechaPeriodo, string tipoCorrida)
        {
            try
            {
                double acumuladosMesAnterior, acumuladoISRMesAnterior;
                double[] acumulados = new Double[] { 0.0, 0.0 };
                fechaPeriodo.AddMonths(fechaPeriodo.Month - 1);
                List<PeriodosNomina> periodos = null;
                periodos = buscarPeriodosPorRangoMeses(0, fechaPeriodo, valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString(), valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()].ToString());

                acumuladosMesAnterior = calcularMovimientosPorMesTipoAfecta(periodos, valoresConceptosEmpleados["NumEmpleado".ToUpper()].ToString(), valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString(), tipoCorrida, ClavesParametrosModulos.claveBaseNominaISR.ToString(),
                    valoresConceptosEmpleados["RazonSocial".ToUpper()].ToString(), valoresConceptosEmpleados["PlazaEmpleado".ToUpper()].ToString());

                acumulados[0] = acumuladosMesAnterior;
                acumuladoISRMesAnterior = obtenerISRAcumuladoMes(periodos, valoresConceptosEmpleados["NumEmpleado".ToUpper()].ToString(), valoresConceptosEmpleados["RazonSocial".ToUpper()].ToString(), valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString(), tipoCorrida, "CalculoISR", valoresConceptosEmpleados["PlazaEmpleado".ToUpper()].ToString());
                acumulados[1] = acumuladoISRMesAnterior;
                return acumulados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("baseGravableAcumuladaMesAnterior()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return new double[] { 0.0, 0.0 };
        }

        private double calcularMovimientosPorMesTipoAfecta(List<PeriodosNomina> periodos, string claveEmpleado, string tipoNomina, string tipoCorrida, string claveBaseNomina, string claveRazonSocial, string clavePlazaEmpleado)
        {
            camposParametro = new List<String>();
            valoresParametro = new List<Object>();
            strQuery.Remove(0, strQuery.Length).Append("SELECT CASE WHEN (COUNT(m) > 0) THEN SUM(mba.resultado) ELSE 0 END * 1.0 ");
            strQuery.Append("FROM MovNomConcep m INNER JOIN m.periodosNomina p INNER JOIN m.tipoNomina tn INNER JOIN m.empleado em INNER JOIN m.tipoCorrida tc INNER JOIN m.concepNomDefi c ");
            strQuery.Append("INNER JOIN m.movNomBaseAfecta mba INNER JOIN mba.baseAfecConcepNom ba INNER JOIN ba.baseNomina bn INNER JOIN m.razonesSociales rs INNER JOIN m.plazasPorEmpleado pemp ");
            strQuery.Append("WHERE m.uso = 0 AND tn.clave = :claveTipoNomina AND em.clave = :claveEmp AND tc.clave = :claveTipoCorrida AND bn.clave = :claveBaseNomina AND rs.clave = :claveRazonSocial AND pemp.clave = :clavePlazaEmpleado AND p.tipoCorrida.clave = :claveTipoCorrida ");
            camposParametro.Add("claveEmp");
            camposParametro.Add("claveTipoCorrida");
            camposParametro.Add("claveTipoNomina");
            camposParametro.Add("claveBaseNomina");
            camposParametro.Add("claveRazonSocial");
            camposParametro.Add("clavePlazaEmpleado");
            valoresParametro.Add(claveEmpleado);
            valoresParametro.Add(tipoCorrida);
            valoresParametro.Add(tipoNomina);
            valoresParametro.Add(claveBaseNomina); //basenomina clave
            valoresParametro.Add(claveRazonSocial);
            valoresParametro.Add(clavePlazaEmpleado);
            if (periodos.Any())
            {
                int i;
                strQuery.Append("AND (");
                for (i = 0; i < periodos.Count; i++)
                {
                    strQuery.Append("(p.clave = :clavePeriodo").Append(i).Append(" AND  p.año = :yearPeriodo").Append(i).Append(")");
                    camposParametro.Add(string.Concat("clavePeriodo", i.ToString()));
                    camposParametro.Add(string.Concat("yearPeriodo", i.ToString()));
                    valoresParametro.Add(periodos[i].clave);
                    valoresParametro.Add(periodos[i].año);
                    if (i < periodos.Count - 1)
                    {
                        strQuery.Append(" OR ");
                    }
                }
                strQuery.Append(") ");
            }

            double baseGravable = (Double)ejecutaQueryUnico(strQuery.ToString(), camposParametro.ToArray<string>(), valoresParametro.ToArray());
            if (mensajeResultado.noError == -100)
            {
                mensajeResultado.noError = 60;
                return 0.0;
            }
            return baseGravable;
        }

        private List<PeriodosNomina> buscarPeriodosPorRangoMeses(int rangoDeMes, DateTime fechaPeriodoNomina, string claveTipoNomina, string claveTipoCorrida)
        {
            IList<PeriodosNomina> periodos = null;
            try
            {
                DateTime fechaRango = fechaPeriodoNomina;
                fechaRango.AddMonths(fechaPeriodoNomina.Month + rangoDeMes);
                int mesIni = -1, mesFin = -1;
                int yearPeriodo = fechaPeriodoNomina.Year;
                int mesPeriodo;
                int mesPeriodoRango;
                int mesEnero = -1;
                strQuery.Remove(0, strQuery.Length).Append("SELECT new PeriodosNomina(p.clave, p.año, p.diasPago, p.diasIMSS, p.AcumularAMes) FROM PeriodosNomina p INNER JOIN p.tipoNomina t INNER JOIN p.tipoCorrida c  WHERE ");
                if (fechaPeriodoNomina.Year > fechaRango.Year)
                {
                    strQuery.Append("t.clave = :claveTipoNomina AND c.clave = :claveTipoCorrida AND p.año = :yearPeriodo AND (MONTH(p.AcumularAMes) <= :mesPeriodo) ");
                    strQuery.Append("OR (t.clave = :claveTipoNomina AND p.año = :yearPeriodo - 1 AND MONTH(p.AcumularAMes) = :mesEnero AND YEAR(p.AcumularAMes) = :yearPeriodo) ");
                    strQuery.Append("OR (t.clave = :claveTipoNomina AND p.año = :yearPeriodo - 1 AND MONTH(p.AcumularAMes) >= :mesPeriodoRango)");
                }
                else if (fechaPeriodoNomina.Year < fechaRango.Year)
                {
                    strQuery.Append("t.clave = :claveTipoNomina AND c.clave = :claveTipoCorrida AND p.año = :yearPeriodo AND (MONTH(p.AcumularAMes) = :mesPeriodo) ");
                    strQuery.Append("OR (t.clave = :claveTipoNomina AND c.clave = :claveTipoCorrida AND p.año = :yearPeriodo AND MONTH(p.AcumularAMes) = :mesEnero AND YEAR(p.AcumularAMes) = :yearPeriodo + 1) ");
                    strQuery.Append("OR (t.clave = :claveTipoNomina AND c.clave = :claveTipoCorrida AND p.año = :yearPeriodo + 1 AND MONTH(p.AcumularAMes) <= :mesPeriodoRango AND YEAR(p.AcumularAMes) != :yearPeriodo + 2) ");
                }
                else
                {
                    if (rangoDeMes < 0)
                    {
                        mesFin = fechaPeriodoNomina.Month;
                        mesIni = fechaRango.Month;
                    }
                    else
                    {
                        mesIni = fechaPeriodoNomina.Month;
                        mesFin = fechaRango.Month;
                    }
                    if (fechaRango.Month == ENERO)
                    {
                        strQuery.Append("t.clave = :claveTipoNomina AND c.clave = :claveTipoCorrida AND p.año = :yearPeriodo AND (MONTH(p.AcumularAMes) <= :mesPeriodo AND YEAR(p.AcumularAMes) = :yearPeriodo) ");
                        strQuery.Append("OR (t.clave = :claveTipoNomina AND c.clave = :claveTipoCorrida AND p.año = :yearPeriodo - 1 AND MONTH(p.AcumularAMes) = :mesPeriodoRango AND YEAR(p.AcumularAMes) = :yearPeriodo) ");
                        mesIni = fechaPeriodoNomina.Month;
                        mesFin = fechaRango.Month;
                    }
                    else
                    {
                        strQuery.Append("t.clave = :claveTipoNomina AND c.clave = :claveTipoCorrida AND p.año = :yearPeriodo AND (MONTH(p.AcumularAMes) >= :mesPeriodo AND MONTH(p.AcumularAMes) <= :mesPeriodoRango)  ");
                    }
                }

                if (mesIni == -1 & mesFin == -1)
                {
                    mesIni = fechaPeriodoNomina.Month;
                    mesFin = fechaRango.Month;
                    camposParametro.Add("mesEnero");
                    valoresParametro.Add(ENERO);
                }

                camposParametro.Add("mesPeriodo");
                camposParametro.Add("mesPeriodoRango");
                camposParametro.Add("claveTipoNomina");
                camposParametro.Add("claveTipoCorrida");
                camposParametro.Add("yearPeriodo");
                valoresParametro.Add(mesIni);
                valoresParametro.Add(mesFin);
                valoresParametro.Add(claveTipoNomina);
                valoresParametro.Add(claveTipoCorrida);
                valoresParametro.Add(fechaPeriodoNomina.Year);
                IQuery q = getSession().CreateQuery(strQuery.ToString());
                q = cargarParametrosQuery(q, camposParametro, valoresParametro);
                periodos = q.List<PeriodosNomina>();
                if (mensajeResultado.noError == -100)
                {
                    mensajeResultado.noError = 62;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscarPeriodosPorRangoMeses()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }

            camposParametro = null;
            valoresParametro = null;
            //strQuery = null;
            return (List<PeriodosNomina>)periodos;

        }

        private ValorTablaISR aplicacionTablaISR(Double ingresoGravado, bool tipoISRANUAL, TipoCorrida corrida)
        {
            double ispt;
            ValorTablaISR valorTablaISR = new ValorTablaISR();
            try
            {
                Isr isrDato = buscaISR(ingresoGravado);
                if (mensajeResultado.noError != 0)
                {
                    return null;
                }
                ispt = (ingresoGravado - isrDato.limiteInferior) * (isrDato.porcentaje / 100) + isrDato.cuotaFija;
                valoresConceptosEmpleados["ISRACARGO".ToUpper()] = ispt;
                Double subsidioAlEmpleado = 0.0;
                valorTablaISR.isrCausado = ispt;
                if (!tipoISRANUAL)
                {
                    if (String.Equals(corrida.clave, "ASI", StringComparison.OrdinalIgnoreCase) || String.Equals(corrida.clave, "AGI", StringComparison.OrdinalIgnoreCase))
                    {
                        valorTablaISR.subsidioAlEmpleo = 0.0;
                        valoresConceptosEmpleados["SubsEmpleoCalculado".ToUpper()] = 0.0;
                    }
                    else
                    {
                        subsidioAlEmpleado = aplicacionTablaSubsidio(ingresoGravado);
                        valorTablaISR.subsidioAlEmpleo = subsidioAlEmpleado;
                    }

                }
                ispt = ispt - subsidioAlEmpleado;
                valorTablaISR.isrNeto = ispt;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("aplicacionTablaISR()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return valorTablaISR;
        }

        private Isr buscaISR(double ingresoGravado)
        {
            Isr isrDato = null;
            try
            {
                if (!periodosNomina.cierreMes)
                {
                    int i;
                    for (i = 0; i < tablaIsr.GetLength(0); i++)
                    {
                        if (ingresoGravado < Convert.ToDouble(tablaIsr[i, 0].ToString()))
                        {
                            isrDato = new Isr(Convert.ToDouble(tablaIsr[i - 1, 0].ToString()), Convert.ToDouble(tablaIsr[i - 1, 1].ToString()), Convert.ToDouble(tablaIsr[i - 1, 2].ToString()));
                            break;
                        }
                    }
                    if (isrDato == null)
                    {
                        isrDato = new Isr(Convert.ToDouble(tablaIsr[tablaIsr.GetLength(0) - 1, 0].ToString()), Convert.ToDouble(tablaIsr[tablaIsr.GetLength(0) - 1, 1].ToString()), Convert.ToDouble(tablaIsr[tablaIsr.GetLength(0) - 1, 2].ToString()));
                        //new Isr((double)tablaIsr[tablaIsr.GetLength(0) - 1, 0], (double)tablaIsr[tablaIsr.GetLength(0) - 1, 1], (double)tablaIsr[tablaIsr.GetLength(0) - 1, 2]);
                    }
                }
                else
                {
                    int i;
                    for (i = 0; i < tablaIsrMes.GetLength(0); i++)
                    {
                        if (ingresoGravado < Convert.ToDouble(tablaIsrMes[i, 0].ToString()))
                        {
                            isrDato = new Isr(Convert.ToDouble(tablaIsrMes[i - 1, 0].ToString()), Convert.ToDouble(tablaIsrMes[i - 1, 1].ToString()), Convert.ToDouble(tablaIsrMes[i - 1, 2].ToString()));
                            break;
                        }
                    }
                    if (isrDato == null)
                    {
                        isrDato = new Isr(Convert.ToDouble(tablaIsrMes[tablaIsrMes.GetLength(0) - 1, 0].ToString()), Convert.ToDouble(tablaIsrMes[tablaIsrMes.GetLength(0) - 1, 1].ToString()), Convert.ToDouble(tablaIsrMes[tablaIsrMes.GetLength(0) - 1, 2].ToString()));
                        isrDato = new Isr(Convert.ToDouble(tablaIsrMes[tablaIsrMes.GetLength(0) - 1, 0].ToString()), Convert.ToDouble(tablaIsrMes[tablaIsrMes.GetLength(0) - 1, 1].ToString()), Convert.ToDouble(tablaIsrMes[tablaIsrMes.GetLength(0) - 1, 2].ToString()));
                        //new Isr((double)tablaIsr[tablaIsr.GetLength(0) - 1, 0], (double)tablaIsr[tablaIsr.GetLength(0) - 1, 1], (double)tablaIsr[tablaIsr.GetLength(0) - 1, 2]);
                    }

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscaISR()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return isrDato;
        }

        private double aplicacionTablaSubsidio(Double ingresoGravado)
        {
            double subsidio = 0.0;
            try
            {
                if (!String.Equals(tipoCorrida.clave, "FIN", StringComparison.OrdinalIgnoreCase) & !String.Equals(tipoCorrida.clave, "LIQ", StringComparison.OrdinalIgnoreCase))
                {//Segun el diagrama del calculo ISR FINIQUITO no lleva subsidio.
                    Subsidio subsidioDato = buscaSubsidio(ingresoGravado);
                    if (mensajeResultado.noError != 0)
                    {
                        return 0.0;
                    }
                    valoresConceptosEmpleados["SubsEmpleoCalculado".ToUpper()] = subsidioDato.getCuota();
                    if (subsidioDato.getCuota() != 0)
                    {
                        subsidio = subsidioDato.getCuota();
                    }
                    else
                    {
                        valoresConceptosEmpleados["SubsEmpleoCalculado".ToUpper()] = 0.0;
                        subsidio = 0.0;
                    }
                }
                else
                {
                    valoresConceptosEmpleados["SubsEmpleoCalculado".ToUpper()] = 0.0;
                    subsidio = 0.0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("aplicacionTablaSubsidio()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return subsidio;
        }

        private Subsidio buscaSubsidio(double ingresoGravado)
        {
            Subsidio subsidioDato = null;
            try
            {
                if (!periodosNomina.cierreMes)
                {
                    int i;
                    for (i = 0; i < tablaSubsidio.GetLength(0); i++)
                    {
                        if (ingresoGravado < Convert.ToDouble(tablaSubsidio[i, 0].ToString()))
                        {
                            subsidioDato = new Subsidio(Convert.ToDouble(tablaSubsidio[i - 1, 0].ToString()), Convert.ToDouble(tablaSubsidio[i - 1, 1].ToString()));
                            break;
                        }
                    }
                    if (subsidioDato == null)
                    {
                        subsidioDato = new Subsidio(Convert.ToDouble(tablaSubsidio[tablaSubsidio.GetLength(0) - 1, 0].ToString()), Convert.ToDouble(tablaSubsidio[tablaSubsidio.GetLength(0) - 1, 1].ToString()));
                    }
                }
                else
                {
                    int i;
                    for (i = 0; i < tablaSubsidioMes.GetLength(0); i++)
                    {
                        if (ingresoGravado < Convert.ToDouble(tablaSubsidioMes[i, 0].ToString()))
                        {
                            subsidioDato = new Subsidio(Convert.ToDouble(tablaSubsidioMes[i - 1, 0].ToString()), Convert.ToDouble(tablaSubsidioMes[i - 1, 1].ToString()));
                            break;
                        }
                    }
                    if (subsidioDato == null)
                    {
                        subsidioDato = new Subsidio(Convert.ToDouble(tablaSubsidioMes[tablaSubsidioMes.GetLength(0) - 1, 0].ToString()), Convert.ToDouble(tablaSubsidioMes[tablaSubsidioMes.GetLength(0) - 1, 1].ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscaSubsidio()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return subsidioDato;
        }

        private Object[,] acumuladosISRAnualPorTipoAfecta(string claveEmpleado, string referenciaPlazaEmp, string tipoCorrida, string tipoNomina, string clavePeriodo, int añoPeriodo, string claveRazonSocial, string claveBaseNomina)
        {
            Object[,] acumulados = null;
            try
            {
                strQuery.Remove(0, strQuery.Length).Append("SELECT SUM(CASE WHEN (mba.resultado is NULL) THEN 0.0 ELSE (mba.resultado * 1.0) END), ba.tipoAfecta FROM MovNomConcep m INNER JOIN m.periodosNomina p INNER JOIN p.tipoCorrida c INNER JOIN m.empleado em ");
                strQuery.Append("INNER JOIN m.tipoCorrida tc INNER JOIN m.movNomBaseAfecta mba INNER JOIN mba.baseAfecConcepNom ba INNER JOIN ba.baseNomina bn INNER JOIN m.tipoNomina t INNER JOIN m.razonesSociales rs INNER JOIN m.plazasPorEmpleado pemp ");
                //if (HibernateUtil.usaTypeBigInt)
                //{
                //    strQuery.append("WHERE m.uso = 0 AND p.clave < CAST(:clavePeriodoNomina as int) AND em.clave = :claveEmp  AND tc.clave = :claveTipoCorrida AND p.año = :yearPeriodo AND t.clave = :claveTipoNomina AND bn.clave = :claveBaseNomina ");
                //}
                //else
                //{
                strQuery.Append("WHERE m.uso = 0 AND p.clave < CAST(:clavePeriodoNomina as long) AND em.clave = :claveEmp  AND tc.clave = :claveTipoCorrida AND p.año = :yearPeriodo AND t.clave = :claveTipoNomina AND bn.clave = :claveBaseNomina ");
                //}
                strQuery.Append("AND rs.clave = :claveRazonSocial AND pemp.clave = :clavePlazaEmpleado  GROUP BY ba.tipoAfecta");
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                camposParametro.Add("claveEmp");
                camposParametro.Add("claveTipoCorrida");
                camposParametro.Add("clavePeriodoNomina");
                camposParametro.Add("claveTipoNomina");
                camposParametro.Add("claveBaseNomina");
                camposParametro.Add("claveRazonSocial");
                camposParametro.Add("clavePlazaEmpleado");
                camposParametro.Add("yearPeriodo");
                valoresParametro.Add(claveEmpleado);
                valoresParametro.Add(tipoCorrida);
                valoresParametro.Add(clavePeriodo);
                valoresParametro.Add(tipoNomina);
                valoresParametro.Add(claveBaseNomina); //BaseNomina ISR
                valoresParametro.Add(claveRazonSocial);
                valoresParametro.Add(referenciaPlazaEmp);
                valoresParametro.Add(añoPeriodo);
                IQuery q = getSession().CreateQuery(strQuery.ToString());
                q = cargarParametrosQuery(q, camposParametro, valoresParametro);
                IList<object> valores = q.List<object>();

                if (mensajeResultado.noError == -100)
                {
                    mensajeResultado.noError = 44;
                    return null;
                }

                int index = 0;
                acumulados = new Object[valores.Count, 2];
                Object[] acumulad;
                for (int i = 0; i < valores.Count; i++)
                {
                    acumulad = (Object[])valores[i];
                    for (int j = 0; j < acumulad.Length; j++)
                    {
                        acumulados[i, j] = acumulad[j];
                    }
                }



            }
            catch (Exception e)
            {
                mensajeResultado.error = e.GetBaseException().Message;
                mensajeResultado.noError = 36;

            }
            return acumulados;
        }
        private double acumuladosPorTipoISRAnual(string claveEmpleado, string referenciaPlazaEmp, string tipoCorrida, string tipoNomina, string clavePeriodo, int añoPeriodo, string claveRazonSocial, string claveBaseNomina)
        {
            double acumulados = 0.0;
            try
            {
                strQuery.Remove(0, strQuery.Length).Append("SELECT SUM(CASE WHEN (mba.resultado is NULL) THEN 0.0 ELSE (mba.resultado * 1.0) END) FROM MovNomConcep m INNER JOIN m.periodosNomina p INNER JOIN p.tipoCorrida c INNER JOIN m.empleado em ");
                strQuery.Append("INNER JOIN m.tipoCorrida tc INNER JOIN m.movNomBaseAfecta mba INNER JOIN mba.baseAfecConcepNom ba INNER JOIN ba.baseNomina bn INNER JOIN m.tipoNomina t INNER JOIN m.razonesSociales rs INNER JOIN m.plazasPorEmpleado pemp ");
                //if (HibernateUtil.usaTypeBigInt)
                //{
                //    strQuery.append("WHERE m.uso = 0 AND p.clave < CAST(:clavePeriodoNomina as int) AND em.clave = :claveEmp  AND tc.clave = :claveTipoCorrida AND p.año = :yearPeriodo AND t.clave = :claveTipoNomina AND bn.clave = :claveBaseNomina ");
                //}
                //else
                //{
                strQuery.Append("WHERE m.uso = 0 AND p.clave < CAST(:clavePeriodoNomina as long) AND em.clave = :claveEmp  AND tc.clave = :claveTipoCorrida AND p.año = :yearPeriodo AND t.clave = :claveTipoNomina AND bn.clave = :claveBaseNomina ");
                //}
                strQuery.Append("AND rs.clave = :claveRazonSocial AND pemp.clave = :clavePlazaEmpleado  GROUP BY ba.tipoAfecta");
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                camposParametro.Add("claveEmp");
                camposParametro.Add("claveTipoCorrida");
                camposParametro.Add("clavePeriodoNomina");
                camposParametro.Add("claveTipoNomina");
                camposParametro.Add("claveBaseNomina");
                camposParametro.Add("claveRazonSocial");
                camposParametro.Add("clavePlazaEmpleado");
                camposParametro.Add("yearPeriodo");
                valoresParametro.Add(claveEmpleado);
                valoresParametro.Add(tipoCorrida);
                valoresParametro.Add(clavePeriodo);
                valoresParametro.Add(tipoNomina);
                valoresParametro.Add(claveBaseNomina); //BaseNomina ISR
                valoresParametro.Add(claveRazonSocial);
                valoresParametro.Add(referenciaPlazaEmp);
                valoresParametro.Add(añoPeriodo);
                IQuery q = getSession().CreateQuery(strQuery.ToString());
                q = cargarParametrosQuery(q, camposParametro, valoresParametro);
                acumulados = (double)q.UniqueResult();

                if (mensajeResultado.noError == -100)
                {
                    mensajeResultado.noError = 44;
                    return 0.0;
                }




            }
            catch (Exception e)
            {
                mensajeResultado.error = e.GetBaseException().Message;
                mensajeResultado.noError = 36;

            }
            return acumulados;
        }

        private Double obtenerISRAcumuladoMes(List<PeriodosNomina> periodos, string claveEmpleado, string claveRazonSocial, string claveTipoNomina, string claveTipoCorrida, string formulaConcepto, string clavePlazaEmpleado)
        {
            camposParametro = new List<String>();
            valoresParametro = new List<Object>();
            strQuery.Remove(0, strQuery.Length).Append("SELECT CASE WHEN (COUNT(isr) = 0 ) THEN 0.0 ELSE (SUM(CASE WHEN (isr.isrRetenidoNormal IS NULL) THEN 0.0 ELSE (isr.isrRetenidoNormal) END)) END + ");
            strQuery.Append("CASE WHEN (COUNT(isr) = 0 ) THEN 0.0 ELSE (SUM(CASE WHEN (isr.isrRetenidoDirecto IS NULL) THEN 0.0 ELSE (isr.isrRetenidoDirecto) END)) END + ");
            strQuery.Append("CASE WHEN (COUNT(isr) = 0 ) THEN 0.0 ELSE (SUM(CASE WHEN (isr.isrRetenidoAnual IS NULL) THEN 0.0 ELSE (isr.isrRetenidoAnual) END)) END ");
            strQuery.Append("FROM ").Append(typeof(CalculoISR).Name).Append(" isr INNER JOIN isr.movNomConcep mov WHERE mov.id in ");
            strQuery.Append("(SELECT m.id FROM MovNomConcep m INNER JOIN m.periodosNomina p INNER JOIN m.empleado em INNER JOIN m.concepNomDefi c ");
            strQuery.Append("INNER JOIN m.tipoCorrida tc INNER JOIN m.razonesSociales rs INNER JOIN m.plazasPorEmpleado pemp  INNER JOIN m.tipoNomina tn ");
            strQuery.Append("WHERE m.uso = 0 AND em.clave = :claveEmp AND tc.clave = :claveTipoCorrida AND c.formulaConcepto LIKE CONCAT('%', :formulaConcepto, '%') ");
            strQuery.Append("AND rs.clave = :claveRazonSocial AND pemp.clave = :clavePlazaEmpleado AND tn.clave = :claveTipoNomina AND p.tipoCorrida.clave = :claveTipoCorrida ");
            camposParametro.Add("claveEmp");
            valoresParametro.Add(claveEmpleado);
            camposParametro.Add("claveTipoCorrida");
            valoresParametro.Add(claveTipoCorrida);
            camposParametro.Add("claveTipoNomina");
            valoresParametro.Add(claveTipoNomina);
            camposParametro.Add("claveRazonSocial");
            valoresParametro.Add(claveRazonSocial);
            camposParametro.Add("clavePlazaEmpleado");
            valoresParametro.Add(clavePlazaEmpleado);
            camposParametro.Add("formulaConcepto");
            valoresParametro.Add(formulaConcepto);
            if (periodos.Any())
            {
                int i;
                strQuery.Append("AND (");
                for (i = 0; i < periodos.Count; i++)
                {
                    strQuery.Append("(p.clave = :clavePeriodo").Append(i).Append(" AND  p.año = :yearPeriodo").Append(i).Append(")");
                    camposParametro.Add(string.Concat("clavePeriodo", i.ToString()));
                    camposParametro.Add(string.Concat("yearPeriodo", i.ToString()));
                    valoresParametro.Add(periodos[i].clave);
                    valoresParametro.Add(periodos[i].año);
                    if (i < periodos.Count - 1)
                    {
                        strQuery.Append(" OR ");
                    }
                }
                strQuery.Append(") ");
            }
            strQuery.Append(") ");

            Double baseGravable = (Double)ejecutaQueryUnico(strQuery.ToString(), camposParametro.ToArray<string>(), valoresParametro.ToArray());
            if (mensajeResultado.noError == -100)
            {
                mensajeResultado.noError = 60;
                return 0.0;
            }
            return baseGravable;
        }

        private void IsrRetenidos(MovNomConcep movimientosNomina)
        {
            try
            {
                Object[] retenidosISR = new Object[9];
                if (tipoTablaISR == TipoTablaISR.NORMAL && ((modoAjustarIngresosMes == ProporcionaPeriodoAjustadoFinalMes && periodosNomina.cierreMes)
                    | modoAjustarIngresosMes == ProporcionaPeriodoAjustadoCadaVez
                    | modoAjustarIngresosMes == ProporcionaPeriodoConTablaAnual
                    | modoAjustarIngresosMes == PropPeriodoIndepAjustadoAlUltimoPeriodoMes
                    | modoAjustarIngresosMes == ProporcionaTablaAnual))
                {
                    strQuery.Remove(0, strQuery.Length).Append("SELECT ");
                    strQuery.Append(" CASE WHEN (COUNT(isr) = 0 ) THEN 0.0 ELSE (SUM(CASE WHEN (isr.isrRetenidoNormal IS NULL) THEN 0.0 ELSE (isr.isrRetenidoNormal) END)) END , ");
                    strQuery.Append(" CASE WHEN (COUNT(isr) = 0 ) THEN 0.0 ELSE (SUM(CASE WHEN (isr.isrRetenidoDirecto IS NULL) THEN 0.0 ELSE (isr.isrRetenidoDirecto) END)) END, ");
                    strQuery.Append(" CASE WHEN (COUNT(isr) = 0 ) THEN 0.0 ELSE (SUM(CASE WHEN (isr.isrRetenidoAnual IS NULL) THEN 0.0 ELSE (isr.isrRetenidoAnual) END)) END, ");
                    strQuery.Append(" CASE WHEN (COUNT(isr) = 0 ) THEN 0.0 ELSE (SUM(CASE WHEN (isr.isrACargoNormal IS NULL) THEN 0.0 ELSE (isr.isrACargoNormal) END)) END , ");
                    strQuery.Append(" CASE WHEN (COUNT(isr) = 0 ) THEN 0.0 ELSE (SUM(CASE WHEN (isr.isrACargoDirecto IS NULL) THEN 0.0 ELSE (isr.isrACargoDirecto) END)) END, ");
                    strQuery.Append(" CASE WHEN (COUNT(isr) = 0 ) THEN 0.0 ELSE (SUM(CASE WHEN (isr.isrACargoAnual IS NULL) THEN 0.0 ELSE (isr.isrACargoAnual) END)) END, ");
                    strQuery.Append(" CASE WHEN (COUNT(isr) = 0 ) THEN 0.0 ELSE (SUM(CASE WHEN (isr.subsidioEmpleoNormal IS NULL) THEN 0.0 ELSE (isr.subsidioEmpleoNormal) END)) END, ");
                    strQuery.Append(" CASE WHEN (COUNT(isr) = 0 ) THEN 0.0 ELSE (SUM(CASE WHEN (isr.subsidioEmpleoDirecto IS NULL) THEN 0.0 ELSE (isr.subsidioEmpleoDirecto) END)) END, ");
                    strQuery.Append(" CASE WHEN (COUNT(isr) = 0 ) THEN 0.0 ELSE (SUM(CASE WHEN (isr.subsidioEmpleoAnual IS NULL) THEN 0.0 ELSE (isr.subsidioEmpleoAnual) END)) END");
                    strQuery.Append(" FROM ").Append(typeof(CalculoISR).Name).Append(" isr INNER JOIN isr.movNomConcep mov ");
                    strQuery.Append(" WHERE mov.id in ");
                    strQuery.Append(" (SELECT m.id FROM MovNomConcep m INNER JOIN m.periodosNomina p ");
                    strQuery.Append(" INNER JOIN m.empleado em INNER JOIN m.concepNomDefi c ");
                    strQuery.Append(" INNER JOIN m.tipoCorrida tc INNER JOIN m.razonesSociales rs ");
                    strQuery.Append(" INNER JOIN m.plazasPorEmpleado pemp ");
                    if (modoAjustarIngresosMes == ProporcionaPeriodoConTablaAnual)
                    {
                        strQuery.Append(" INNER JOIN m.tipoNomina tn ");
                    }
                    strQuery.Append(" WHERE m.uso = 0 ");
                    strQuery.Append(" AND em.clave = :claveEmp  ");
                    strQuery.Append(" AND tc.clave = :claveTipoCorrida ");
                    strQuery.Append(" AND (c.formulaConcepto LIKE CONCAT('%', :formulaConcepto, '%') OR c.formulaConcepto LIKE CONCAT('%', :formulaConcepto1, '%'))");
                    strQuery.Append(" AND rs.clave = :claveRazonSocial AND p.tipoCorrida.clave = :claveTipoCorrida ");
                    //            strQuery.append(" AND pemp.clave = :clavePlazaEmpleado ");
                    //////            if (isMov2Meses) {
                    //////                strQuery.append(" AND m.mes = :mesMovim ");
                    //////            }
                    //if (HibernateUtil.usaTypeBigInt)
                    //{
                    //    strQuery.append(" AND (p.clave < cast(:clavePeriodoNomina as int) ");
                    //    if (modoAjustarIngresosMes != ProporcionaPeriodoConTablaAnual)
                    //    {
                    //        strQuery.append(" AND p.clave > ");
                    //        strQuery.append(" (SELECT CASE WHEN (count(pn) > 0) THEN");
                    //        strQuery.append(" MAX(CAST(pn.clave as int)) ");
                    //        strQuery.append(" ELSE 0 END ");
                    //        strQuery.append(" FROM PeriodosNomina pn INNER JOIN pn.tipoNomina t ");
                    //        strQuery.append(" WHERE pn.clave < cast(:clavePeriodoNomina as int) ");
                    //        strQuery.append(" AND t.clave = :claveTipoNomina ");
                    //        strQuery.append(" AND pn.cierreMes = true ");
                    //        strQuery.append(" AND pn.año = :yearPeriodo");
                    //        strQuery.append(" AND pn.tipoCorrida.clave = :claveTipoCorrida)");
                    //    }
                    //    else
                    //    {
                    //        strQuery.append(" AND tn.clave = :claveTipoNomina ");
                    //        strQuery.append(" AND p.año = :yearPeriodo ");
                    //        strQuery.append(" AND p.tipoCorrida.clave = :claveTipoCorrida ");
                    //    }
                    //    strQuery.append(" )) AND mov.ejercicio=:yearPeriodo");
                    //}
                    //else
                    //{
                    strQuery.Append(" AND (p.clave < cast(:clavePeriodoNomina as long) ");
                    if (modoAjustarIngresosMes != ProporcionaPeriodoConTablaAnual)
                    {
                        strQuery.Append(" AND p.clave > ");
                        strQuery.Append(" (SELECT CASE WHEN (count(pn) > 0) THEN");
                        strQuery.Append(" MAX(CAST(pn.clave as long)) ");
                        strQuery.Append(" ELSE 0 END ");
                        strQuery.Append(" FROM PeriodosNomina pn INNER JOIN pn.tipoNomina t ");
                        strQuery.Append(" WHERE pn.clave < cast(:clavePeriodoNomina as long) ");
                        strQuery.Append(" AND t.clave = :claveTipoNomina ");
                        strQuery.Append(" AND pn.cierreMes = true ");
                        strQuery.Append(" AND pn.año = :yearPeriodo");
                        strQuery.Append(" AND pn.tipoCorrida.clave = :claveTipoCorrida)");
                    }
                    else
                    {
                        strQuery.Append(" AND tn.clave = :claveTipoNomina ");
                        strQuery.Append(" AND p.año = :yearPeriodo ");
                        strQuery.Append(" AND p.tipoCorrida.clave = :claveTipoCorrida ");
                    }
                    strQuery.Append(" )) AND mov.ejercicio=:yearPeriodo");
                    //}

                    camposParametro = new List<String>(0);
                    valoresParametro = new List<Object>(0);
                    //////            if (isMov2Meses) {
                    //////                camposParametro.add("mesMovim");
                    //////                valoresParametro.add(movimientosNomina.getMes());
                    //////            }
                    camposParametro.Add("claveEmp");
                    valoresParametro.Add(valoresConceptosEmpleados["NumEmpleado".ToUpper()]);
                    camposParametro.Add("claveTipoCorrida");
                    valoresParametro.Add(valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()]);
                    camposParametro.Add("formulaConcepto");
                    valoresParametro.Add("CalculoISR"); //ConceptoISR
                    camposParametro.Add("formulaConcepto1");
                    valoresParametro.Add("ISRSubsidio"); //ConceptoISRSubsidio
                    camposParametro.Add("clavePeriodoNomina");
                    valoresParametro.Add(valoresConceptosEmpleados["NumPeriodo".ToUpper()]);
                    camposParametro.Add("claveRazonSocial");
                    valoresParametro.Add(valoresConceptosEmpleados["RazonSocial".ToUpper()]);
                    //            camposParametro.add("clavePlazaEmpleado");
                    //            valoresParametro.add(valoresConceptosEmpleados.get("PlazaEmpleado".toUpperCase()));
                    camposParametro.Add("claveTipoNomina");
                    valoresParametro.Add(valoresConceptosEmpleados["TipoNomina".ToUpper()]);
                    camposParametro.Add("yearPeriodo");
                    valoresParametro.Add(valoresConceptosGlobales["ejercicioActivo".ToUpper()]);
                    retenidosISR = (Object[])ejecutaQueryUnico(strQuery.ToString(), camposParametro.ToArray<string>(), valoresParametro.ToArray());
                    if (mensajeResultado.noError == 100)
                    {
                        mensajeResultado.noError = 42;
                        return;
                    }
                }
                //Busca el ISRRetenido del concepto ISR que ya existe para modificarlo
                if (movimientosNomina.id > 0)
                {
                    iSRRetenido = (CalculoISR)ejecutaQueryUnico("From " + typeof(CalculoISR).Name + " isr Where isr.movNomConcep.id = :id", new String[] { "id" }, new Object[] { movimientosNomina.id });

                }
                if (mensajeResultado.noError != 0)
                {
                    mensajeResultado.noError = 43;
                    return;
                }
                if (iSRRetenido == null)
                {
                    iSRRetenido = new CalculoISR();
                }
                MovNomConcep movNomConcepSubsidio = null;
                if (isMov2Meses & listMovNomConcepSubsidio.Count > 0)
                {
                    int pos = movimientosNomina.numMovParticion == 1 ? 0 : 1;
                    movNomConcepSubsidio = listMovNomConcepSubsidio[pos];
                }
                else if (listMovNomConcepSubsidio.Count > 0)
                {
                    movNomConcepSubsidio = listMovNomConcepSubsidio[0];
                }

                if (movNomConcepSubsidio != null)
                {
                    if (movNomConcepSubsidio.id > 0)
                    {
                        //Busca el ISRRetenido del concepto Subsidio que ya existe para modificarlo
                        iSRRetenidoSubsidio = (CalculoISR)ejecutaQueryUnico("From " + typeof(CalculoISR).Name + " isr Where isr.movNomConcep.id = :id", new String[] { "id" }, new Object[] { movNomConcepSubsidio.id });
                    }
                }
                if (mensajeResultado.noError != 0)
                {
                    mensajeResultado.noError = 43;
                    return;
                }

                if (iSRRetenidoSubsidio == null)
                {
                    iSRRetenidoSubsidio = new CalculoISR();
                }
                if (isrNormal != 0.0)
                {
                    if (tipoTablaISR == TipoTablaISR.NORMAL
                       && (modoAjustarIngresosMes == ProporcionaPeriodoAjustadoFinalMes
                       | modoAjustarIngresosMes == ProporcionaPeriodoAjustadoCadaVez
                       | modoAjustarIngresosMes == PropPeriodoIndepAjustadoAlUltimoPeriodoMes
                       | modoAjustarIngresosMes == ProporcionaPeriodoConTablaAnual))
                    {
                        if (isMov2Meses)
                        {
                            int diasTotales;
                            if (manejaPagoDiasNaturales)
                            {
                                diasTotales = (cantidadDiasEntreDosFechas(periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault()) + 1); //+1 para contar el dia actual
                            }
                            else
                            {
                                diasTotales = Decimal.ToInt16(periodosNomina.tipoNomina.periodicidad.dias);
                            }
                            if (modoAjustarIngresosMes == ProporcionaPeriodoConTablaAnual)
                            {
                                int diasPagoTotal = (int)valoresConceptosEmpleados["DiasNormalesAPagar".ToUpper()];
                                isrNormal = isrNormal - (retenidosISR[0] == null ? 0.0 : ((double)retenidosISR[0] * diasPagoTotal) / diasTotales);
                                retenido.isrACargoNormal = retenido.isrACargoNormal - (retenidosISR[3] == null ? 0.0 : ((double)retenidosISR[3] * diasPagoTotal) / diasTotales);
                                retenido.subsidioEmpleoNormal = retenido.subsidioEmpleoNormal - (retenidosISR[6] == null ? 0.0 : ((double)retenidosISR[6] * diasPagoTotal) / diasTotales);
                                //isrNormal = isrNormal - (retenidosISR[0] == null ? 0.0 : (Double.valueOf(retenidosISR[0].toString()) * diasPago) / diasTotales);
                                // isrNormal = isrNormal / diasPago * diasPagoTotal;
                                //  double retenidoCargo = retenido.getIsrACargoNormal() - (retenidosISR[3] == null ? 0.0 : (Double.valueOf(retenidosISR[3].toString()) * diasPago) / diasTotales);
                                //retenidoCargo = retenidoCargo / diasPago * diasPagoTotal;
                                // retenido.setIsrACargoNormal(retenidoCargo);
                                //double retenidosubsidio =retenido.getSubsidioEmpleoNormal() - (retenidosISR[6] == null ? 0.0 : (Double.valueOf(retenidosISR[6].toString()) * diasPago) / diasTotales);
                                //retenidosubsidio = retenidosubsidio / diasPago * diasPagoTotal;
                                //retenido.setSubsidioEmpleoNormal(retenidosubsidio);
                            }
                            else
                            {
                                isrNormal = isrNormal - (retenidosISR[0] == null ? 0.0 : ((double)retenidosISR[0] * diasPago) / diasTotales);
                                retenido.isrACargoNormal = retenido.isrACargoNormal - (retenidosISR[3] == null ? 0.0 : ((double)retenidosISR[3] * diasPago) / diasTotales);
                                retenido.subsidioEmpleoNormal = retenido.subsidioEmpleoNormal - (retenidosISR[6] == null ? 0.0 : ((double)retenidosISR[6] * diasPago) / diasTotales);
                            }
                        }
                        else
                        {
                            ////                        if (descontarFaltasModoAjustaMes & periodosNomina.isCierreMes() & descontarDiasPago > 0) {
                            ////                            isrNormal = (isrNormal / factorMensual) * diasPago;
                            ////                        }
                            isrNormal = isrNormal - (retenidosISR[0] == null ? 0.0 : (double)retenidosISR[0]);
                            retenido.isrACargoNormal = retenido.isrACargoNormal - (retenidosISR[3] == null ? 0.0 : (double)retenidosISR[3]);
                            retenido.subsidioEmpleoNormal = retenido.subsidioEmpleoNormal - (retenidosISR[6] == null ? 0.0 : (double)retenidosISR[6]);
                        }
                    }
                }
                else
                {
                    isrNormal = 0.0;
                    retenido.isrACargoNormal = 0.0;
                    retenido.subsidioEmpleoNormal = 0.0;
                }

                if (String.Equals(tipoCorrida.clave, "FIN", StringComparison.OrdinalIgnoreCase) || String.Equals(tipoCorrida.clave, "LIQ", StringComparison.OrdinalIgnoreCase))
                {
                    isrNormal = movimientosNomina.resultado.GetValueOrDefault();
                }

                if (isrNormal >= 0)
                {
                    retenido.isrNetoNormal = isrNormal;
                    retenido.isrSubsidioNormal = 0.0;
                }
                else
                {
                    retenido.isrNetoNormal = 0.0;
                    retenido.isrSubsidioNormal = isrNormal * -1;
                }

                if (isrDirecto != 0.0)
                {
                    if (tipoTablaISR == TipoTablaISR.NORMAL && (modoAjustarIngresosMes == ProporcionaPeriodoAjustadoFinalMes | modoAjustarIngresosMes == PropPeriodoIndepAjustadoAlUltimoPeriodoMes))
                    {
                        if (isMov2Meses)
                        {
                            int diasPeriodo;
                            if (manejaPagoDiasNaturales)
                            {
                                diasPeriodo = (cantidadDiasEntreDosFechas(periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault()) + 1); //+1 para contar el dia actual
                            }
                            else
                            {
                                diasPeriodo = Decimal.ToInt16(periodosNomina.tipoNomina.periodicidad.dias);
                            }
                            int diasPagoTotal = (int)valoresConceptosEmpleados["DiasNormalesAPagar".ToUpper()];
                            isrDirecto = isrDirecto - (retenidosISR[1] == null ? 0.0 : ((double)retenidosISR[1] * diasPagoTotal) / diasPeriodo);
                            retenido.isrACargoDirecto = retenido.isrACargoDirecto - (retenidosISR[4] == null ? 0.0 : ((double)retenidosISR[4] * diasPagoTotal) / diasPeriodo);
                            retenido.subsidioEmpleoDirecto = retenido.subsidioEmpleoDirecto - (retenidosISR[7] == null ? 0.0 : ((double)retenidosISR[7] * diasPagoTotal) / diasPeriodo);
                        }
                        else
                        {
                            isrDirecto = isrDirecto - (retenidosISR[1] == null ? 0.0 : (double)retenidosISR[1]);
                            retenido.isrACargoDirecto = retenido.isrACargoDirecto - (retenidosISR[4] == null ? 0.0 : (double)retenidosISR[4]);
                            retenido.subsidioEmpleoDirecto = retenido.subsidioEmpleoDirecto - (retenidosISR[7] == null ? 0.0 : (double)retenidosISR[7]);
                        }
                    }
                }
                else
                {
                    isrDirecto = 0.0;
                    retenido.isrACargoDirecto = 0.0;
                    retenido.subsidioEmpleoDirecto = 0.0;
                }

                if (isrDirecto >= 0)
                {
                    retenido.isrNetoDirecto = isrDirecto;
                    retenido.isrSubsidioDirecto = 0.0;
                }
                else
                {
                    retenido.isrNetoDirecto = 0.0;
                    retenido.isrSubsidioDirecto = isrDirecto * -1;
                }

                if (isrAnual != 0.0)
                {
                    if (tipoTablaISR == TipoTablaISR.NORMAL && (modoAjustarIngresosMes == ProporcionaPeriodoAjustadoFinalMes | modoAjustarIngresosMes == PropPeriodoIndepAjustadoAlUltimoPeriodoMes | modoAjustarIngresosMes == ProporcionaTablaAnual))
                    {
                        if (isMov2Meses)
                        {
                            int diasTotales;
                            if (manejaPagoDiasNaturales)
                            {
                                diasTotales = (cantidadDiasEntreDosFechas(periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault()) + 1); //+1 para contar el dia actual
                            }
                            else
                            {
                                diasTotales = Decimal.ToInt16(periodosNomina.tipoNomina.periodicidad.dias);
                            }
                            int diasPagoTotal = (int)valoresConceptosEmpleados["DiasNormalesAPagar".ToUpper()];
                            isrAnual = isrAnual - (retenidosISR[2] == null ? 0 : ((double)retenidosISR[2] * diasPagoTotal) / diasTotales);
                            retenido.isrACargoAnual = retenido.isrACargoAnual - (retenidosISR[5] == null ? 0.0 : ((double)retenidosISR[5] * diasPagoTotal) / diasTotales);
                            retenido.subsidioEmpleoAnual = retenido.subsidioEmpleoAnual - (retenidosISR[8] == null ? 0.0 : ((double)retenidosISR[8] * diasPagoTotal) / diasTotales);
                        }
                        else
                        {
                            isrAnual = isrAnual - (retenidosISR[2] == null ? 0 : (double)retenidosISR[2]);
                            retenido.isrACargoAnual = retenido.isrACargoAnual - (retenidosISR[5] == null ? 0.0 : (double)retenidosISR[5]);
                            retenido.subsidioEmpleoAnual = retenido.subsidioEmpleoAnual - (retenidosISR[8] == null ? 0.0 : (double)retenidosISR[8]);
                        }
                    }
                }
                else
                {
                    isrAnual = 0.0;
                    retenido.isrACargoAnual = 0.0;
                    retenido.subsidioEmpleoAnual = 0.0;
                }

                if (isrAnual >= 0)
                {
                    retenido.isrNetoAnual = isrAnual;
                    retenido.isrSubsidioAnual = 0.0;
                }
                else
                {
                    retenido.isrNetoAnual = 0.0;
                    retenido.isrSubsidioAnual = isrAnual * -1;
                }

                if (isrNormal + isrDirecto + isrAnual >= 0)
                {
                    valoresConceptosEmpleados["ISRNeto".ToUpper()] = isrNormal + isrDirecto + isrAnual;
                    valoresConceptosEmpleados["ISRSubsidio".ToUpper()] = 0.0;
                }
                else
                {
                    valoresConceptosEmpleados["ISRNeto".ToUpper()] = 0.0;
                    valoresConceptosEmpleados["ISRSubsidio".ToUpper()] = (isrNormal + isrDirecto + isrAnual) * -1;
                }
                valoresConceptosEmpleados["ISRACARGO".ToUpper()] = retenido.isrACargoNormal + retenido.isrACargoDirecto + retenido.isrACargoAnual;
                valoresConceptosEmpleados["SubsEmpleoCalculado".ToUpper()] = retenido.subsidioEmpleoNormal + retenido.subsidioEmpleoDirecto + retenido.subsidioEmpleoAnual;
                //Aqui se llena los datos del ISR del concepto ISR
                iSRRetenido = contruirISRRetenido(iSRRetenido, retenido, isrNormal, isrDirecto, isrAnual);
                //Aqui se llena los datos del ISR del concepto Subsidio
                iSRRetenidoSubsidio = contruirISRRetenido(iSRRetenidoSubsidio, retenido, isrNormal, isrDirecto, isrAnual);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("IsrRetenidos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            retenido = null;
        }
        private CalculoISR contruirISRRetenido(CalculoISR iSRRetenido, CalculoISR retenido, double isrNormal, double isrDirecto, double isrAnual)
        {
            iSRRetenido.isrRetenidoNormal = isrNormal;
            iSRRetenido.isrRetenidoDirecto = isrDirecto;
            iSRRetenido.isrRetenidoAnual = isrAnual;
            if (retenido != null)
            {
                iSRRetenido.isrACargoAnual = retenido.isrACargoAnual;
                iSRRetenido.isrACargoDirecto = retenido.isrACargoDirecto;
                iSRRetenido.isrACargoNormal = retenido.isrACargoNormal;
                iSRRetenido.isrNetoAnual = retenido.isrNetoAnual;
                iSRRetenido.isrNetoDirecto = retenido.isrNetoDirecto;
                iSRRetenido.isrNetoNormal = retenido.isrNetoNormal;
                iSRRetenido.isrSubsidioAnual = retenido.isrSubsidioAnual;
                iSRRetenido.isrSubsidioDirecto = retenido.isrSubsidioDirecto;
                iSRRetenido.isrSubsidioNormal = retenido.isrSubsidioNormal;
                iSRRetenido.subsidioEmpleoAnual = retenido.subsidioEmpleoAnual;
                iSRRetenido.subsidioEmpleoDirecto = retenido.subsidioEmpleoDirecto;
                iSRRetenido.subsidioEmpleoNormal = retenido.subsidioEmpleoNormal;
            }
            return iSRRetenido;
        }

        private double buscaSubsidioCausado()
        {
            double subsidioCausadoMes = 0.0;
            try
            {
                string numEmpleado = valoresConceptosEmpleados["NumEmpleado".ToUpper()].ToString();
                string claveTipoCorridas = valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()].ToString();
                string claveRazonsocial = valoresConceptosEmpleados["RazonSocial".ToUpper()].ToString();
                string claveTiposNomina = valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString();
                string numPeriodo = valoresConceptosEmpleados["NumPeriodo".ToUpper()].ToString();
                int ejercicioActivo = (int)valoresConceptosGlobales["ejercicioActivo".ToUpper()];
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                strQuery.Remove(0, strQuery.Length).Append("elect SUM(CASE WHEN (m.resultado is NULL) THEN 0.0 ELSE (m.resultado * 1.0) END)  from MovNomConcep m ");
                strQuery.Append("where m.uso = 0  and m.empleados.clave = :claveEmpleado && m.tipoCorrida.clave = :claveTipoCorrida ");
                strQuery.Append("and (m.concepNomDefi.formulaConcepto LIKE CONCAT('%', :formulaConcepto, '%')) and m.razonesSociales.clave = :claveRazonsocial ");
                strQuery.Append("and m.periodosNomina.tipoNomina.clave = :claveTiposNomina and m.periodosNomina.clave < CAST(:clavePeriodoNomina as long) ");
                strQuery.Append("and  m.periodosNomina.año = :ejercicioActivo");
                camposParametro.Add("claveEmpleado");
                camposParametro.Add("claveTipoCorrida");
                camposParametro.Add("formulaConcepto");
                camposParametro.Add("claveRazonsocial");
                camposParametro.Add("claveTiposNomina");
                camposParametro.Add("clavePeriodoNomina");
                camposParametro.Add("ejercicioActivo");
                valoresParametro.Add(numEmpleado);
                valoresParametro.Add(claveTipoCorridas);
                valoresParametro.Add("SubsEmpleoCalculado");
                valoresParametro.Add(claveRazonsocial);
                valoresParametro.Add(claveTiposNomina);
                valoresParametro.Add(numPeriodo);
                valoresParametro.Add(ejercicioActivo);

                IQuery q = getSession().CreateQuery(strQuery.ToString());
                q = cargarParametrosQuery(q, camposParametro, valoresParametro);
                subsidioCausadoMes = (double)q.UniqueResult();
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscaSubsidioCausado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return subsidioCausadoMes;
        }
        private double[] buscaISRRetenidoAnt()
        {

            double isrAnte = 0.0;
            double subAnte = 0.0;
            double[] valores = new double[2];
            try
            {
                string numEmpleado = valoresConceptosEmpleados["NumEmpleado".ToUpper()].ToString();
                string claveTipoCorridas = valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()].ToString();
                string claveRazonsocial = valoresConceptosEmpleados["RazonSocial".ToUpper()].ToString();
                string claveTiposNomina = valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString();
                string numPeriodo = valoresConceptosEmpleados["NumPeriodo".ToUpper()].ToString();
                int ejercicioActivo = (int)valoresConceptosGlobales["ejercicioActivo".ToUpper()];
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                strQuery.Remove(0, strQuery.Length).Append("elect SUM(CASE WHEN (m.resultado is NULL) THEN 0.0 ELSE (m.resultado * 1.0) END)  from MovNomConcep m ");
                strQuery.Append("where m.uso = 0  and m.empleados.clave = :claveEmpleado && m.tipoCorrida.clave = :claveTipoCorrida ");
                strQuery.Append("and (m.concepNomDefi.formulaConcepto LIKE CONCAT('%', :formulaConcepto, '%')) and m.razonesSociales.clave = :claveRazonsocial ");
                strQuery.Append("and m.periodosNomina.tipoNomina.clave = :claveTiposNomina and m.periodosNomina.clave < CAST(:clavePeriodoNomina as long) ");
                strQuery.Append("and  m.periodosNomina.año = :ejercicioActivo");
                camposParametro.Add("claveEmpleado");
                camposParametro.Add("claveTipoCorrida");
                camposParametro.Add("formulaConcepto");
                camposParametro.Add("claveRazonsocial");
                camposParametro.Add("claveTiposNomina");
                camposParametro.Add("clavePeriodoNomina");
                camposParametro.Add("ejercicioActivo");
                valoresParametro.Add(numEmpleado);
                valoresParametro.Add(claveTipoCorridas);
                valoresParametro.Add("CalculoISR");
                valoresParametro.Add(claveRazonsocial);
                valoresParametro.Add(claveTiposNomina);
                valoresParametro.Add(numPeriodo);
                valoresParametro.Add(ejercicioActivo);

                IQuery qIsrAnt = getSession().CreateQuery(strQuery.ToString());
                qIsrAnt = cargarParametrosQuery(qIsrAnt, camposParametro, valoresParametro);
                isrAnte = (double)qIsrAnt.UniqueResult();
                camposParametro = new List<String>(0);
                valoresParametro = new List<Object>(0);
                strQuery.Remove(0, strQuery.Length).Append("elect SUM(CASE WHEN (m.resultado is NULL) THEN 0.0 ELSE (m.resultado * 1.0) END)  from MovNomConcep m ");
                strQuery.Append("where m.uso = 0  and m.empleados.clave = :claveEmpleado && m.tipoCorrida.clave = :claveTipoCorrida ");
                strQuery.Append("and (m.concepNomDefi.formulaConcepto LIKE CONCAT('%', :formulaConcepto, '%')) and m.razonesSociales.clave = :claveRazonsocial ");
                strQuery.Append("and m.periodosNomina.tipoNomina.clave = :claveTiposNomina and m.periodosNomina.clave < CAST(:clavePeriodoNomina as long) ");
                strQuery.Append("and  m.periodosNomina.año = :ejercicioActivo");
                camposParametro.Add("claveEmpleado");
                camposParametro.Add("claveTipoCorrida");
                camposParametro.Add("formulaConcepto");
                camposParametro.Add("claveRazonsocial");
                camposParametro.Add("claveTiposNomina");
                camposParametro.Add("clavePeriodoNomina");
                camposParametro.Add("ejercicioActivo");
                valoresParametro.Add(numEmpleado);
                valoresParametro.Add(claveTipoCorridas);
                valoresParametro.Add("CalculoISR");
                valoresParametro.Add(claveRazonsocial);
                valoresParametro.Add(claveTiposNomina);
                valoresParametro.Add(numPeriodo);
                valoresParametro.Add(ejercicioActivo);

                IQuery qsubAnte = getSession().CreateQuery(strQuery.ToString());
                qsubAnte = cargarParametrosQuery(qsubAnte, camposParametro, valoresParametro);
                subAnte = (double)qsubAnte.UniqueResult();

                valores[0] = isrAnte;
                valores[1] = subAnte;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscaISRRetenidoAnt()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return valores;

        }


    }
}
