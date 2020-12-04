/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: clase CalculaNominaDAO para llamados a metodos de Entity
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
using Exitosw.Payroll.Entity.entidad.cfdi;
using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.Core.genericos.campos;
using Exitosw.Payroll.Core.metodosCalculoNomina;
using System.Data.Entity.Migrations;
using System.Data.Entity;
using Spring.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Exitosw.Payroll.TestCompilador.funciones;
using Exitosw.Payroll.TestCompilador.compilador;
using System.Xml.Linq;
using System.Globalization;
using System.Data.SqlClient;
using Exitosw.Payroll.Entity.genericos;
using Exitosw.Payroll.Entity.util;
using Exitosw.Payroll.Core.genericos;
using System.Diagnostics;
using System.ServiceModel.Channels;
using NHibernate;

namespace Exitosw.Payroll.Core.modelo
{
    public class CalculaNominaDAO : GenericRepository<CalculoISR>, CalculaNominaDAOIF
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
        private List<TablaDatos> tablaDatosXml = new List<TablaDatos>();
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
        private double? valorUMI = 0.0;
        private double? valorSMG = 0.0;

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
        private MetodosDatosEmpleados metodosDatosEmpleados = new MetodosDatosEmpleados();
        private MetodosParaVacaciones metodosParaVacaciones = new MetodosParaVacaciones();
        private MetodosParaPtu metodosParaPtu = new MetodosParaPtu();
        private MetodosDatosAsistencias metodosDatosAsistencias = new MetodosDatosAsistencias();
        private MetodosParaMovimientosNomina metodosParaMovimientosNomina = new MetodosParaMovimientosNomina();
        private MetodosPeriodosNomina metodosPeriodosNomina = new MetodosPeriodosNomina();
        private MetodosBDMaestra metodosBDMaestra = new MetodosBDMaestra();
        private string clavePeriodoFuncion = "";
        private MovNomConcep movNomConcepGlobal;
        private DbContextTransaction transacion;
        private DbContext dbContextSimple;
        private DbContext dbContextMaestra;
        private DBContextAdapter dbContextAdapterSimple;
        private DBContextAdapter dbContextAdapterMaestra;
        //////"fechaSalidaVacaciones","fechaRegresoVacaciones", "tipoVacaciones", 
        private ValorTablaISR valoresTablaISR;
        Stopwatch tiempo = new Stopwatch();
        NHibernate.ISession sessionSiemple;

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
            fechaActual = getFechaDelSistema();
            cantidadSaveUpdate = 0;
            fechaBajaFiniq = null;
            finiquitosLiquidaciones = null;
            descontarFaltasModoAjustaMes = false;
            //////            retenidosISRACargoYSubsidioAlEmpleoEn2Meses = null;
            //////            agregaronPlazaPorEmpleadoRestantes = false;
        }
        private void cargarFactoresyTablasXml(DatosTablaXml factoresCalculo)
        {
            calculoSeparadoISR = factoresCalculo.calculoSeparadoISR;
            descontarFaltasModoAjustaMes = factoresCalculo.descontarFaltasModoAjustaMes;
            factorAnual = factoresCalculo.factorAnual;
            factorMensual = factoresCalculo.factorMensual;
            isUMA = factoresCalculo.isUMA;
            manejaPagoDiasNaturales = factoresCalculo.manejaPagoDiasNaturales;
            manejaPagoIMSSDiasNaturales = factoresCalculo.manejaPagoIMSSDiasNaturales;
            manejaPagosPorHora = factoresCalculo.manejaPagosPorHora;
            manejoHorasPor = factoresCalculo.manejoHorasPor;
            manejoSalarioDiario = factoresCalculo.manejoSalarioDiario;
            modoAjustarIngresosMes = factoresCalculo.modoAjustarIngresosMes;
            pagarVacaAuto = factoresCalculo.pagarVacaAuto;
            salarioVacaciones = factoresCalculo.salarioVacaciones;
            tipoTablaISR = factoresCalculo.tipoTablaISR;
            versionCalculoPrestamoAhorro = factoresCalculo.versionCalculoPrestamoAhorro;
            tablaIsr = factoresCalculo.tablaIsr;
            tablaFactorIntegracion = factoresCalculo.tablaFactorIntegracion;
            tablaSubsidio = factoresCalculo.tablaSubsidio;
            tablaZonaSalarial = factoresCalculo.tablaZonaSalarial;
            matrixcargaXml = factoresCalculo.matrixcargaXml;
            tablaIsrMes = factoresCalculo.tablaIsrMes;
            tablaSubsidioMes = factoresCalculo.tablaSubsidioMes;
            tablaDatosXml = factoresCalculo.tablaDatosXml;
            valorUMA = factoresCalculo.valorUMA;
            valorUMI = factoresCalculo.valorUMI;
            //filtroConceptosNomina = new List<ConcepNomDefi>();
            //filtroConceptosNomina = factoresCalculo.filtroConceptosNomina;
            //metodosParaMovimientosNomina = new MetodosParaMovimientosNomina(filtroConceptosNomina);

        }
        public Mensaje calculaNomina(string claveEmpIni, string claveEmpFin, string claveTipoNomina, string claveTipoCorrida, decimal? idPeriodoNomina,
            string clavePuesto, string claveCategoriasPuestos, string claveTurno, string claveRazonSocial, string claveRegPatronal, string claveFormaDePago,
            string claveDepto, string claveCtrCosto, int? tipoSalario, string tipoContrato, bool? status, string controlador, int uso,
            ParametrosExtra parametrosExtra, int ejercicioActivo, DatosTablaXml datosTablaXml, DBContextAdapter dbContextSimple1, DBContextAdapter dbContextMaestra1)
        {
            Boolean band = true;
            mensajeResultado = new Mensaje();
            mensajeResultado.error = "";
            mensajeResultado.noError = 0;
            dbContextAdapterSimple = dbContextSimple1;
            dbContextAdapterMaestra = dbContextMaestra1;
            dbContextMaestra = dbContextMaestra1.context;
            using (dbContextSimple = dbContextSimple1.context)
            {
                using (transacion = dbContextSimple.Database.BeginTransaction())
                {
                    try
                    {

                        initVariablesCalculo();
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
                        cargarFactoresyTablasXml(datosTablaXml);
                        #region busca la periocidad del periodo de nomina
                        buscaPeriodicidadesOrPeriodosNomina(claveTipoNomina, claveTipoCorrida, idPeriodoNomina);
                        if (mensajeResultado.noError != 0)
                        {
                            return mensajeResultado;
                        }
                        #endregion

                        #region busca Calculos de ptu
                        mensajeResultado = metodosParaPtu.buscaCalculoPTU(claveRazonSocial, periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault(), (DBContextSimple)dbContextSimple);
                        if (mensajeResultado.noError == 0)
                        {
                            ptuDatosGenerales = (PtuDatosGenerales)mensajeResultado.resultado;
                            isCalculoPTU = metodosParaPtu.isCalculoPTU;
                        }
                        else
                        {
                            return mensajeResultado;
                        }
                        #endregion

                        #region genera tablas Xml 
                        //  generaTablasXml(controlador, periodicidadTipoNomina, claveRazonSocial, periodosNomina.fechaFinal.Value, ejercicioActivo, dbContextMaestra);
                        /// generaTablasXmlH(controlador,periodicidadTipoNomina,claveRazonSocial,periodosNomina.fechaFinal.Value,ejercicioActivo,sessionMaster);

                        if (mensajeResultado.noError != 0)
                        {
                            return mensajeResultado;
                        }
                        #endregion

                        #region cargado de variables globales y configuraciones
                        cargarVariablesConceptosCompilador();

                        if (mensajeResultado.noError != 0)
                        {
                            return mensajeResultado;
                        }

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
                        #endregion

                        idPeriodoNomina = periodosNomina.id;

                        #region obtener los creditos y ahorros
                        listCreditoAhorro = obtenerCreditosAhorro(razonesSociales.clave);
                        #endregion

                        consultarConfiguracionAgui();
                        if (mensajeResultado.noError != 0)
                        {
                            return mensajeResultado;
                        }

                        #region calcula las vacaciones devengadas de empleado
                        mensajeResultado = metodosParaVacaciones.calcularVacacionesDevengadasEmpleados(razonesSociales, tablaFactorIntegracion, (DBContextSimple)dbContextSimple, (DBContextMaster)dbContextMaestra);
                        if (mensajeResultado.noError != 0)
                        {
                            return mensajeResultado;
                        }
                        //pendiente
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

                        #region metodos principal recorrido de empleados
                        #region obtiene las plazas de los empleados movimientos

                        mensajeResultado = metodosDatosEmpleados.obtenerPlazasPorEmpleados(claveEmpIni, claveEmpFin, claveTipoNomina, clavePuesto, claveCategoriasPuestos, claveTurno, claveRazonSocial,
                           claveRegPatronal, claveDepto, claveCtrCosto, tipoSalario, tipoContrato, status, claveTipoCorrida, claveFormaDePago, parametrosExtra.fechaInicioPeriodo.GetValueOrDefault(),
                        parametrosExtra.fechaFinPeriodo.GetValueOrDefault(), fechaBajaFiniq, periodosNomina.id, StatusTimbrado.TIMBRADO, dbContextAdapterSimple, true);



                        if (mensajeResultado.noError == 0)
                        {
                            filtroPlazasPorEmpleadosMov = ((IQueryable<PlazasPorEmpleadosMov>)mensajeResultado.resultado).ToList();
                        }
                        else
                        {
                            return mensajeResultado;
                        }
                        #endregion

                        #region carga la variables de configuracion del imss
                        cargaDatosVariableConfiguracionIMSS(periodosNomina.fechaFinal.GetValueOrDefault());

                        #endregion

                        if (filtroPlazasPorEmpleadosMov != null)
                        {

                            int i;
                            #region recorre la lista de plazas por empleado movimientos 
                            for (i = 0; i < filtroPlazasPorEmpleadosMov.Count; i++)
                            {

                                #region verifica si se le van a pagar las vacaciones en automatico
                                if (pagarVacaAuto != PagarPrimaVacionalyVacacionesAuto.MANUAL)
                                {
                                    mensajeResultado = metodosParaVacaciones.agregarVacacionesAuto(razonesSociales, filtroPlazasPorEmpleadosMov[i], periodosNomina, tablaFactorIntegracion, pagarVacaAuto, (DBContextSimple)dbContextSimple);
                                    if (mensajeResultado.noError == 0)
                                    {
                                        vacacionAplicacionActual = (VacacionesAplicacion)mensajeResultado.resultado;
                                    }
                                    else
                                    {
                                        break;
                                    }
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
                                            mensajeResultado = metodosDatosEmpleados.obtenerIngresosReIngresosBajas(filtroPlazasPorEmpleadosMov[i], periodosNomina.fechaFinal, (DBContextSimple)dbContextSimple);

                                            if (mensajeResultado.noError == 0)
                                            {
                                                ingresosReingresosBajas = (IngresosBajas)mensajeResultado.resultado;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {

                                    mensajeResultado = metodosDatosEmpleados.obtenerIngresosReIngresosBajas(filtroPlazasPorEmpleadosMov[i], periodosNomina.fechaFinal, (DBContextSimple)dbContextSimple);
                                    if (mensajeResultado.noError == 0)
                                    {
                                        ingresosReingresosBajas = (IngresosBajas)mensajeResultado.resultado;
                                    }
                                    else
                                    {
                                        break;
                                    }

                                }
                                #endregion

                                #region busca Calculos de ptu
                                ptuEmpleado = null;
                                if (isCalculoPTU)
                                {
                                    mensajeResultado = metodosParaPtu.buscaEmpleadoPTU(claveRazonSocial, filtroPlazasPorEmpleadosMov[i].plazasPorEmpleado.empleados.id, (DBContextSimple)dbContextSimple);
                                    if (mensajeResultado.noError == 0)
                                    {
                                        ptuEmpleado = (PtuEmpleados)mensajeResultado.resultado;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                #endregion

                                //agrega valoresConceptosGlobales a valoresConceptosEmpleados
                                valoresConceptosEmpleados = valoresConceptosEmpleados.Concat(valoresConceptosGlobales).ToDictionary(e => e.Key, e => e.Value);

                                #region crear o obtiene la lista de calculos de unidades 
                                List<CalculoUnidades> listCalculoUnidades = obtenerListaCalculoUnidadesUtilizar(claveRazonSocial, filtroPlazasPorEmpleadosMov[i].plazasPorEmpleado, claveTipoNomina, periodosNomina.id, claveTipoCorrida);
                                if (mensajeResultado.noError != 0)
                                {
                                    break;
                                }
                                List<object> idsUnidades = new List<object>();
                                int j;
                                for (j = 0; j < listCalculoUnidades.Count; j++)
                                {
                                    if (listCalculoUnidades[j].id != 0) //pendiente checaer validacion si al crear unidad inicializa con 0
                                    {
                                        idsUnidades.Add(listCalculoUnidades[j].id);
                                        listCalculoUnidades[j] = new CalculoUnidades(listCalculoUnidades[j]);
                                    }
                                }
                                if (idsUnidades.Count > 0)
                                {
                                    CamposWhere campoWhere = new CamposWhere(String.Concat(typeof(CalculoUnidades).Name, ".id"), idsUnidades, OperadorComparacion.IN, OperadorLogico.AND);
                                    mensajeResultado = deleteListQuery(typeof(CalculoUnidades).Name, campoWhere, /*new Conexion(uuidCxn)*/dbContextAdapterSimple);  ///pendiente Conexion BD
                                    if (mensajeResultado.noError != 0)
                                    {
                                        break;
                                    }
                                }
                                #endregion

                                #region carga las variables globales del empelado

                                cargarVariablesGlobalesEmpleadoPorPlaza(filtroPlazasPorEmpleadosMov[i], true, true, TipoSueldos.SUELDO_DIARIO_FINAL, listCalculoUnidades[0], null, false, null);

                                if (mensajeResultado.noError != 0)
                                {
                                    break;
                                }
                                #endregion

                                bool continueProsesoCal = true;
                                if (String.Equals(tipoCorrida.clave, "VAC", StringComparison.OrdinalIgnoreCase))
                                {
                                    int? diasVac = Convert.ToInt32(valoresConceptosEmpleados["DiasVacaciones".ToUpper()]);
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

                                    dbContextSimple.Set<CalculoUnidades>().AddOrUpdate(listCalculoUnidades[0]);
                                    dbContextSimple.SaveChanges();
                                    #region obtener movimientos nomina
                                    if (String.Equals(claveTipoCorrida, "FIN", StringComparison.OrdinalIgnoreCase) || String.Equals(claveTipoCorrida, "LIQ", StringComparison.OrdinalIgnoreCase))
                                    {
                                        mensajeResultado = metodosParaMovimientosNomina.obtenerMovimientosPlazasFiniquitos(claveTipoCorrida, claveTipoNomina, idPeriodoNomina.Value, filtroPlazasPorEmpleadosMov[i].plazasPorEmpleado,
                                            claveCtrCosto, claveRazonSocial, uso, finiquitosLiquidaciones, dbContextAdapterSimple);
                                        filtroMovimientosNominas = (List<MovNomConcep>)mensajeResultado.resultado;
                                        if (filtroConceptosNomina.Count == 0 && mensajeResultado.noError == 0)
                                        {
                                            mensajeResultado.noError = 26;
                                            mensajeResultado.error = "No contiene movimientos finiquitos agregados";
                                        }
                                    }
                                    else
                                    {

                                        mensajeResultado = metodosParaMovimientosNomina.obtenerMovimientosNominaPorPlaza(tipoCorrida, claveTipoNomina, idPeriodoNomina.GetValueOrDefault(), filtroPlazasPorEmpleadosMov[i].plazasPorEmpleado, claveCtrCosto, claveRazonSocial,
                                             periodosNomina, fechaActual, centroDeCostoMovimiento, dbContextAdapterSimple);

                                    }

                                    if (mensajeResultado.noError == 0)
                                    {
                                        filtroMovimientosNominas = (List<MovNomConcep>)mensajeResultado.resultado;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    #endregion
                                    #region obtener los movientos de credito y ahorro 
                                    mensajeResultado = metodosParaMovimientosNomina.obtenerMovimientosNominaCreditoAhorro(listCreditoAhorro, filtroMovimientosNominas);
                                    if (mensajeResultado.noError == 0)
                                    {
                                        object[] listMovTmp = (object[])mensajeResultado.resultado;
                                        filtroMovimientosNominasCreditosAhorro = (List<MovNomConcep>)listMovTmp[0];
                                        listMovNomConcepCreditosAhorroDescuentoActivo = (List<MovNomConcep>)listMovTmp[1];
                                        formulaDedudCreditos = (List<MovNomConcep>)listMovTmp[2];
                                        formulaDedudAhorros = (List<MovNomConcep>)listMovTmp[3];
                                        filtroMovimientosNominas = (List<MovNomConcep>)listMovTmp[4];
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    #endregion

                                    obtenerMovimientosNominaISRACargoYSubsidioAlEmpleado();
                                    filtroMovimientosNominas = filtroMovimientosNominas.Except(listMovNomConcepISRCARGO).ToList();  //checar remueve items lista
                                                                                                                                    // filtroMovimientosNominas = filtroMovimientosNominas.Except(listMovNomConcepSUBSIDIOALEMPLEO).ToList();
                                    int conta = 0;

                                    List<Object> idsMovDeleteCorrida = new List<Object>();
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
                                        metodosParaMovimientosNomina.deleteListQueryMov(typeof(MovNomConcep).Name, "id", idsMovDeleteCorrida.ToArray(), null, null, null, true, dbContextAdapterSimple, dbContextAdapterMaestra); //pendiente
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
                                    //filtroMovimientosNominas = (from list in filtroMovimientosNominas orderby list.numMovParticion select list).ToList();
                                    for (int h = 0; h < filtroMovimientosNominas.Count; h++)
                                    {

                                        if (filtroMovimientosNominas[h].concepNomDefi.activarPlaza)
                                        {
                                            List<PlazasPorEmpleadosMov> plazaEmpleadoMov;
                                            mensajeResultado = metodosDatosEmpleados.obtenerPlazasPorEmpleados(filtroMovimientosNominas[h].plazasPorEmpleado.empleados.clave,
                                                filtroMovimientosNominas[h].plazasPorEmpleado.empleados.clave,
                                                filtroMovimientosNominas[h].tipoNomina.clave, clavePuesto,
                                                filtroMovimientosNominas[h].concepNomDefi.categoriaPuestos == null ? "" : filtroMovimientosNominas[h].concepNomDefi.categoriaPuestos.clave,
                                                claveTurno, filtroMovimientosNominas[h].razonesSociales.clave,
                                                claveRegPatronal, claveDepto, claveCtrCosto, tipoSalario, tipoContrato, status, claveTipoCorrida, claveFormaDePago,
                                                filtroMovimientosNominas[h].periodosNomina.fechaInicial.GetValueOrDefault(),
                                                filtroMovimientosNominas[h].periodosNomina.fechaFinal.GetValueOrDefault(), fechaBajaFiniq, periodosNomina.id, StatusTimbrado.TIMBRADO, dbContextAdapterSimple, false);
                                            if (mensajeResultado.noError == 0)
                                            {
                                                plazaEmpleadoMov = (List<PlazasPorEmpleadosMov>)mensajeResultado.resultado;
                                            }
                                            else
                                            {
                                                return mensajeResultado;
                                            }
                                            if (filtroMovimientosNominas[h].concepNomDefi.activarDesglose)
                                            {
                                                for (int k = 0; k < plazaEmpleadoMov.Count; k++)
                                                {
                                                    if (plazaEmpleadoMov[k].plazasPorEmpleado.id != filtroPlazasPorEmpleadosMov[i].plazasPorEmpleado.id)
                                                    {
                                                        mensajeResultado = metodosParaMovimientosNomina.obtenerMovimientosNominaPorPlaza(tipoCorrida, claveTipoCorrida, idPeriodoNomina.GetValueOrDefault(), plazaEmpleadoMov[k].plazasPorEmpleado, claveCtrCosto, claveRazonSocial,
                                                                          periodosNomina, fechaActual, centroDeCostoMovimiento, dbContextAdapterSimple);
                                                        if (mensajeResultado.noError == 0)
                                                        {
                                                            filtroMovimientosNominasAuxPlaza = (List<MovNomConcep>)mensajeResultado.resultado;
                                                        }
                                                        else
                                                        {
                                                            return mensajeResultado;
                                                        }
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
                                                for (int k = 0; k < plazaEmpleadoMov.Count; k++)
                                                {
                                                    if (plazaEmpleadoMov[k].plazasPorEmpleado.id != filtroPlazasPorEmpleadosMov[i].plazasPorEmpleado.id)
                                                    {
                                                        mensajeResultado = metodosParaMovimientosNomina.obtenerMovimientosNominaPorPlaza(tipoCorrida, claveTipoCorrida, idPeriodoNomina.GetValueOrDefault(), plazaEmpleadoMov[k].plazasPorEmpleado, claveCtrCosto, claveRazonSocial,
                                                                           periodosNomina, fechaActual, centroDeCostoMovimiento, dbContextAdapterSimple);
                                                        if (mensajeResultado.noError == 0)
                                                        {
                                                            filtroMovimientosNominasAuxPlaza = (List<MovNomConcep>)mensajeResultado.resultado;
                                                        }
                                                        else
                                                        {
                                                            return mensajeResultado;
                                                        }
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
                                                    metodosParaMovimientosNomina.deleteListQueryMov(typeof(MovNomConcep).Name, "id", idsMovDeleteplaza.ToArray(), null, null, null, true, dbContextAdapterSimple, dbContextAdapterMaestra); //pendiente
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
                                            va.statusVacaciones = StatusVacaciones.CALCULADA;
                                            va.tipoCorridaPago_ID = periodosNomina.tipoCorrida_ID;
                                            va.tipoNominaPago_ID = periodosNomina.tipoNomina_ID;
                                            va.periodoPago_ID = periodosNomina.id;
                                            //va.tipoCorridaAplicacion = periodosNomina.tipoCorrida;
                                            dbContextSimple.Set<VacacionesDisfrutadas>().AddOrUpdate(va);
                                        }
                                    }

                                    if (ptuEmpleado != null)
                                    {
                                        dbContextSimple.Set<PtuEmpleados>().AddOrUpdate(ptuEmpleado);
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

                                        IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                                         where mov.concepNomDefi.naturaleza == Naturaleza.PERCEPCION
                                                                         select mov;
                                        var result = movimientosAcumuladoPorRangoMeses(TipoClasificacionFormula.DATOPERIODO, x, "", CamposAMostrar.MovNomConcepresultado, query, TipoMostrarCampo.SUMA, null, null);

                                        var res = (from mov in result
                                                   select new { suma = mov.resultado }).ToList();
                                        if (res.Count() > 0)
                                        {
                                            percepcion = res.Sum(P => P.suma);
                                        }
                                        if (mensajeResultado.noError != 0)
                                        {
                                            break;
                                        }
                                        IQueryable<MovNomConcep> query2 = from mov in dbContextSimple.Set<MovNomConcep>()
                                                                          where mov.concepNomDefi.naturaleza == Naturaleza.DEDUCCION
                                                                          select mov;
                                        var result2 = movimientosAcumuladoPorRangoMeses(TipoClasificacionFormula.DATOPERIODO, x, "", CamposAMostrar.MovNomConcepresultado, query2, TipoMostrarCampo.SUMA, null, null);

                                        var res2 = (from mov in result2
                                                    select new { suma = mov.resultado }).ToList();
                                        if (res2.Count() > 0)
                                        {
                                            deduccion = res2.Sum(P => P.suma);
                                        }
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
                                                eliminarMovimientosNominaBasura(new decimal[] { movNomConcepAjustePorRedondeo.id });
                                                //metodosParaMovimientosNomina.eliminarMovimientosNominaBasura(new object[] { movNomConcepAjustePorRedondeo.id }, dbContextAdapterSimple);
                                                //dbContextSimple.SaveChanges();
                                            }
                                        }
                                        else
                                        {
                                            dbContextSimple.Set<MovNomConcep>().AddOrUpdate(movNomConcepAjustePorRedondeo);
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
                                            dbContextSimple.Set<MovNomConcep>().AddOrUpdate(formulaDedudCreditos[k]);
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
                                                dbContextSimple.Set<MovNomConcep>().AddOrUpdate(formulaDedudAhorros[k]);
                                            }
                                        }
                                        if (mensajeResultado.noError != 0)
                                        {
                                            break;
                                        }
                                        //if (cantidadSaveUpdate % cantidadFlush == 0 & cantidadSaveUpdate > 0)
                                        //{

                                        dbContextSimple.SaveChanges();

                                        //}
                                    }

                                }

                                valoresConceptosEmpleados.Clear();
                                filtroMovimientosNominas = null;
                            }
                            #endregion

                            if (mensajeResultado.noError != 0)
                            {
                                transacion.Rollback();
                                return mensajeResultado;
                            }

                            if (filtroPlazasPorEmpleadosMov.Count() == 0)
                            {
                                mensajeResultado.noError = 999;
                                mensajeResultado.error = "No existen empleados";
                            }
                        }
                        else
                        {
                            mensajeResultado.noError = 999;
                            mensajeResultado.error = "No existen empleados";
                        }
                        #endregion

                        if (mensajeResultado.noError == 0)
                        {
                            mensajeResultado.resultado = true;
                            transacion.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("calculaNomina()1_Error: ").Append(ex));
                        mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                        mensajeResultado.error = ex.GetBaseException().ToString();
                        mensajeResultado.resultado = null;
                        transacion.Rollback();
                    }
                }
            }
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
                                Double cantidad = obtenerCantidadPeriodoNominaRango(plazasPorEmpleadosMov.tipoNomina, creditoMovimientosBloqueo.initPeriodNom, periodosNomina);
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
                                                if (!string.Equals(creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.concepNomiDefin.clave, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.cNDescuento.clave, StringComparison.OrdinalIgnoreCase))
                                                {
                                                    int k = 0;
                                                    while (k < creditoMovimientosDescuentoSistema.movNomConcep.Count())
                                                    {
                                                        if (string.Equals(creditoMovimientosDescuentoSistema.movNomConcep[k].concepNomDefi.clave, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.cNDescuento.clave, StringComparison.OrdinalIgnoreCase))
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
                                                        int dia = DateTime.DaysInMonth(cFecha.Year, cFecha.Month);
                                                        DateTime fec = new DateTime(cFecha.Year, cFecha.Month, dia);
                                                        // cFecha = creditoMovimientosCambioDescuento.movNomConcep[k].periodosNomina.fechaInicial.GetValueOrDefault();
                                                        // cFecha.AddDays(DateTime.DaysInMonth(cFecha.Year, cFecha.Month));
                                                        inicializaPeriodo2Meses(periodosNomina, periodosNomina.fechaInicial.GetValueOrDefault(), fec);
                                                        valoresConceptosGlobales.Add(parametroFechaFinal, fec);
                                                        valoresConceptosGlobales.Add(parametroFechaInicial, periodosNomina.fechaInicial);
                                                    }
                                                    else
                                                    {
                                                        cFecha = creditoMovimientosCambioDescuento.movNomConcep[k].periodosNomina.fechaFinal.GetValueOrDefault();
                                                        DateTime fec2 = new DateTime(cFecha.Year, cFecha.Month, 1);
                                                        //cFecha = creditoMovimientosCambioDescuento.movNomConcep[k].periodosNomina.fechaFinal.GetValueOrDefault();
                                                        //cFecha = new DateTime(cFecha.Year, cFecha.Month, 1);
                                                        inicializaPeriodo2Meses(periodosNomina, fec2, periodosNomina.fechaFinal.GetValueOrDefault());
                                                        valoresConceptosGlobales.Add(parametroFechaInicial, fec2);
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
                                                        dbContextSimple.Set<MovNomConcep>().AddOrUpdate(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                        //dbContextSimple.saveOrUpdate(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                    }
                                                    else
                                                    {
                                                        dbContextSimple.Set<MovNomConcep>().AddOrUpdate(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                        //dbContextSimple.merge(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                    }
                                                    cantidadSaveUpdate++;

                                                }
                                                else
                                                {
                                                    importeAcumulado -= importe;
                                                    if (creditoMovimientosDescuentoSistema.movNomConcep[k].id > 0)
                                                    {
                                                        dbContextSimple.Set<MovNomConcep>().Attach(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                        dbContextSimple.Set<MovNomConcep>().Remove(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                        // dbContextSimple.delete(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                        creditoMovimientosDescuentoSistema.movNomConcep.RemoveAt(k);
                                                        cantidadSaveUpdate++;
                                                    }

                                                    if (creditoMovimientosDescuentoSistema.id > 0 & !tieneMovOtrasCorridas)
                                                    {
                                                        if (creditoMovimientosDescuentoSistema.movNomConcep.Count() == 0)
                                                        {
                                                            dbContextSimple.Set<CreditoMovimientos>().Attach(creditoMovimientosDescuentoSistema);
                                                            dbContextSimple.Set<CreditoMovimientos>().Remove(creditoMovimientosDescuentoSistema);
                                                            // dbContextSimple.delete(creditoMovimientosDescuentoSistema);
                                                            cantidadSaveUpdate++;
                                                        }
                                                    }
                                                }
                                                //if (cantidadSaveUpdate % cantidadFlush == 0 & cantidadSaveUpdate > 0)
                                                //{

                                                dbContextSimple.SaveChanges();

                                                //}

                                            }//end for creditoMovimientosDescuentoSistema.movNomConcep.Count()
                                            if (listMovNomConcepCreditosAhorroDescuentoParaGuardar.Count() > 0)
                                            {
                                                creditoMovimientosDescuentoSistema.movNomConcep.AddRange(listMovNomConcepCreditosAhorroDescuentoParaGuardar);
                                                dbContextSimple.Set<CreditoMovimientos>().AddOrUpdate(creditoMovimientosDescuentoSistema);
                                                //dbContextSimple.saveOrUpdate(creditoMovimientosDescuentoSistema);
                                                for (int k = 0; k < creditoMovimientosDescuentoSistema.movNomConcep.Count(); k++)
                                                {
                                                    creditoMovimientosDescuentoSistema.movNomConcep[k].creditoMovimientos = (creditoMovimientosDescuentoSistema);
                                                    dbContextSimple.Set<MovNomConcep>().AddOrUpdate(creditoMovimientosDescuentoSistema.movNomConcep[k]);
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
                                        dbContextSimple.Set<CreditoPorEmpleado>().AddOrUpdate(creditoMovimientosDescuentoSistema.creditoPorEmpleado);
                                        dbContextSimple.Set<MovNomConcep>().Attach(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                        dbContextSimple.Set<MovNomConcep>().Remove(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                        //dbContextSimple.saveOrUpdate(creditoMovimientosDescuentoSistema.getCreditoPorEmpleado());
                                        //dbContextSimple.delete(creditoMovimientosDescuentoSistema.getMovNomConceps().get(k));
                                    }
                                    creditoMovimientosDescuentoSistema.movNomConcep.Clear();
                                    if (!tieneMovOtrasCorridas)
                                    {
                                        dbContextSimple.Set<CreditoMovimientos>().Attach(creditoMovimientosDescuentoSistema);
                                        dbContextSimple.Set<CreditoMovimientos>().Remove(creditoMovimientosDescuentoSistema);
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
                            MovimientosNominaDAO movimientosNominaDAO = new MovimientosNominaDAO();
                            //movimientosNominaDAO.setSession(dbContextSimple);
                            movimientosNominaDAO.deleteListQueryMov("MovNomConcep", "id", idsMovDelete.ToArray(), null, null, null, true, dbContextAdapterSimple);//pendiente la conexion
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
                                Double cantidad = obtenerCantidadPeriodoNominaRango(plazasPorEmpleadosMov.tipoNomina, creditoMovimientosBloqueo.initPeriodNom, periodosNomina);
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
                                                if (!string.Equals(creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.concepNomiDefin.clave, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.cNDescuento.clave, StringComparison.OrdinalIgnoreCase))
                                                {
                                                    int k = 0;
                                                    while (k < creditoMovimientosDescuentoSistema.movNomConcep.Count())
                                                    {
                                                        if (string.Equals(creditoMovimientosDescuentoSistema.movNomConcep[k].concepNomDefi.clave, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.cNDescuento.clave, StringComparison.OrdinalIgnoreCase))
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
                                                    diasPeriodo = (cantidadDiasEntreDosFechas(periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault())) + 1;
                                                    DateTime cFecha = DateTime.Now;
                                                    if (creditoMovimientosDescuentoSistema.movNomConcep[k].numMovParticion == 1)
                                                    {
                                                        cFecha = creditoMovimientosDescuentoSistema.movNomConcep[k].periodosNomina.fechaInicial.GetValueOrDefault();
                                                        int dia = DateTime.DaysInMonth(cFecha.Year, cFecha.Month);
                                                        DateTime fec = new DateTime(cFecha.Year, cFecha.Month, dia);
                                                        //cFecha = creditoMovimientosDescuentoSistema.movNomConcep[k].periodosNomina.fechaInicial.GetValueOrDefault();
                                                        //int dia = DateTime.DaysInMonth(cFecha.Year, cFecha.Month);
                                                        //cFecha.AddDays(DateTime.DaysInMonth(cFecha.Year, cFecha.Month));
                                                        inicializaPeriodo2Meses(periodosNomina, periodosNomina.fechaInicial.GetValueOrDefault(), fec);
                                                        valoresConceptosGlobales[parametroFechaFinal] = fec;
                                                        valoresConceptosGlobales[parametroFechaInicial] = periodosNomina.fechaInicial.GetValueOrDefault();
                                                    }
                                                    else
                                                    {
                                                        cFecha = creditoMovimientosDescuentoSistema.movNomConcep[k].periodosNomina.fechaFinal.GetValueOrDefault();
                                                        DateTime fec2 = new DateTime(cFecha.Year, cFecha.Month, 1);
                                                        //cFecha = creditoMovimientosDescuentoSistema.movNomConcep[k].periodosNomina.fechaFinal.GetValueOrDefault();
                                                        //cFecha = new DateTime(cFecha.Year, cFecha.Month, 1);
                                                        inicializaPeriodo2Meses(periodosNomina, fec2, periodosNomina.fechaFinal.GetValueOrDefault());
                                                        valoresConceptosGlobales[parametroFechaFinal] = fec2;
                                                        valoresConceptosGlobales[parametroFechaInicial] = periodosNomina.fechaInicial.GetValueOrDefault();
                                                    }

                                                    foreach (var a in valoresConceptosGlobales)
                                                    {
                                                        valoresConceptosEmpleados[a.Key] = a.Value;
                                                    }
                                                    cargaValoresDiasCotizados(plazasPorEmpleadosMov.fechaIMSS.GetValueOrDefault(), plazasPorEmpleadosMov.plazasPorEmpleado.fechaFinal.GetValueOrDefault(), true, null, null, false, false);//JSA30
                                                    cargaDatosVariableConfiguracionIMSS(((DateTime)valoresConceptosGlobales[parametroFechaFinal]));
                                                }
                                                Double diasIMSS = 0.0;
                                                if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.considerarIncap)
                                                {
                                                    diasIMSS = Convert.ToDouble(valoresConceptosEmpleados["DiasCotizados".ToUpper()].ToString());
                                                }
                                                else
                                                {
                                                    diasIMSS = Convert.ToDouble(valoresConceptosEmpleados["DiasNaturalesDelPeriodo".ToUpper()].ToString());
                                                }
                                                //Double diasIMSS = Convert.ToDouble(valoresConceptosEmpleados["DiasCotizados".ToUpper()].ToString());
                                                Double SMDF = 0.0;
                                                if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.modoDescuento == 1)
                                                {//Elegir Modo al Registrar el Credito
                                                    Double montoSDI = Convert.ToDouble(valoresConceptosEmpleados["SueldoIntIMSS".ToUpper()].ToString());
                                                    if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.valorVSMG != null)
                                                    {
                                                        if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.valorVSMG == 0)
                                                        {
                                                            SMDF = valorSMG.GetValueOrDefault();
                                                        }
                                                        else if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.valorVSMG == 1)
                                                        {
                                                            SMDF = valorUMA.GetValueOrDefault();
                                                        }
                                                        else if (creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.valorVSMG == 2)
                                                        {
                                                            SMDF = valorUMI.GetValueOrDefault();
                                                        }
                                                        else
                                                        {
                                                            SMDF = Convert.ToDouble(valoresConceptosEmpleados["SalarioMinDF".ToUpper()].ToString());
                                                        }
                                                    }
                                                    else
                                                    {
                                                        SMDF = Convert.ToDouble(valoresConceptosEmpleados["SalarioMinDF".ToUpper()].ToString());
                                                    }
                                                    // Double SMDF = Convert.ToDouble(valoresConceptosEmpleados["SalarioMinDF".ToUpper()].ToString());
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
                                                        dbContextSimple.Set<MovNomConcep>().AddOrUpdate(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                        //dbContextSimple.saveOrUpdate(creditoMovimientosDescuentoSistema.getMovNomConceps().get(k));
                                                    }
                                                    else
                                                    {
                                                        dbContextSimple.Set<MovNomConcep>().AddOrUpdate(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                        //dbContextSimple.merge(creditoMovimientosDescuentoSistema.getMovNomConceps().get(k));
                                                    }
                                                    cantidadSaveUpdate++;
                                                }
                                                else
                                                {
                                                    importeAcumulado -= importe;
                                                    if (creditoMovimientosDescuentoSistema.movNomConcep[k].id > 0)
                                                    {
                                                        dbContextSimple.Set<MovNomConcep>().Attach(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                        dbContextSimple.Set<MovNomConcep>().Remove(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                                        //dbContextSimple.delete(creditoMovimientosDescuentoSistema.getMovNomConceps().get(k));
                                                        creditoMovimientosDescuentoSistema.movNomConcep.RemoveAt(k);
                                                        cantidadSaveUpdate++;
                                                    }

                                                    if (creditoMovimientosDescuentoSistema.id > 0 & !tieneMovOtrasCorridas)
                                                    {
                                                        if (creditoMovimientosDescuentoSistema.movNomConcep.Count() == 0)
                                                        {
                                                            dbContextSimple.Set<CreditoMovimientos>().Attach(creditoMovimientosDescuentoSistema);
                                                            dbContextSimple.Set<CreditoMovimientos>().Remove(creditoMovimientosDescuentoSistema);
                                                            // dbContextSimple.delete(creditoMovimientosDescuentoSistema);
                                                            cantidadSaveUpdate++;
                                                        }
                                                    }
                                                }

                                                //if (cantidadSaveUpdate % cantidadFlush == 0 & cantidadSaveUpdate > 0)
                                                //{
                                                //                                                System.out.println("flush 4");
                                                dbContextSimple.SaveChanges();
                                                //                                                System.out.println("clear()");
                                                //}
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
                                                dbContextSimple.Set<CreditoMovimientos>().AddOrUpdate(creditoMovimientosDescuentoSistema);
                                                //dbContextSimple.saveOrUpdate(creditoMovimientosDescuentoSistema);
                                                for (int k = 0; k < creditoMovimientosDescuentoSistema.movNomConcep.Count(); k++)
                                                {
                                                    creditoMovimientosDescuentoSistema.movNomConcep[k].creditoMovimientos = (creditoMovimientosDescuentoSistema);
                                                    dbContextSimple.Set<MovNomConcep>().AddOrUpdate(creditoMovimientosDescuentoSistema.movNomConcep[k]);
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
                                        dbContextSimple.Set<CreditoPorEmpleado>().AddOrUpdate(creditoMovimientosDescuentoSistema.creditoPorEmpleado);
                                        // dbContextSimple.saveOrUpdate(creditoMovimientosDescuentoSistema.getCreditoPorEmpleado());
                                        dbContextSimple.Set<MovNomConcep>().Attach(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                        dbContextSimple.Set<MovNomConcep>().Remove(creditoMovimientosDescuentoSistema.movNomConcep[k]);
                                        //dbContextSimple.delete(creditoMovimientosDescuentoSistema.getMovNomConceps().get(k));

                                    }
                                    creditoMovimientosDescuentoSistema.movNomConcep.Clear();
                                    // dbContextSimple.SaveChanges();
                                    if (!tieneMovOtrasCorridas)
                                    {
                                        dbContextSimple.Set<CreditoMovimientos>().Attach(creditoMovimientosDescuentoSistema);
                                        dbContextSimple.Set<CreditoMovimientos>().Remove(creditoMovimientosDescuentoSistema);
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
                            MovimientosNominaDAO movimientosNominaDAO = new MovimientosNominaDAO();
                            //movimientosNominaDAO.setSession(dbContextSimple);
                            movimientosNominaDAO.deleteListQueryMov("MovNomConcep", "id", idsMovDelete.ToArray(), null, null, null, true, dbContextAdapterSimple);//pendiente la conexion
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
            List<MovNomConcep> values = null;
            try
            {
                values = (from o in dbContextSimple.Set<MovNomConcep>()
                          where o.razonesSociales.clave == movNomConcep.razonesSociales.clave && o.empleados.clave == movNomConcep.empleados.clave
                          && o.tipoNomina.clave == movNomConcep.tipoNomina.clave && o.periodosNomina.id == movNomConcep.periodosNomina.id &&
                          o.tipoCorrida.clave == movNomConcep.tipoCorrida.clave && o.concepNomDefi.clave == movNomConcep.concepNomDefi.clave &&
                          o.ejercicio == movNomConcep.ejercicio && o.mes == movNomConcep.mes
                          select o).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("existeMovimientoNomina()1_Error: ").Append(ex));
                mensajeResultado.noError = 27;
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return values;
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
                        if (!string.Equals(creditoMovimientosDescuentoSistema.movNomConcep[0].concepNomDefi.clave, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.cNDescuento.clave, StringComparison.OrdinalIgnoreCase))
                        {
                            #region Cuando son diferentes conceptos tanto para el credito principal como para el descuento, se tiene que buscar el movimiento, si no construirlo
                            indice = obtenerMovNomConcepCreditosAhorroDescuentoActivo(listMovNomConcepCreditosAhorroDescuentoActivo, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.cNDescuento, creditoMovimientosDescuentoSistema, movNomConcepAbarca2Meses);
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
                        if (!string.Equals(creditoMovimientosDescuentoSistema.movNomConcep[0].concepNomDefi.clave, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.cNDescuento.clave, StringComparison.OrdinalIgnoreCase))
                        {
                            #region Cuando son diferentes conceptos tanto para el credito principal como para el descuento, se tiene que buscar el movimiento, si no construirlo
                            indice = obtenerMovNomConcepCreditosAhorroDescuentoActivo(listMovNomConcepCreditosAhorroDescuentoActivo, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.cNDescuento, creditoMovimientosDescuentoSistema, movNomConcepAbarca2Meses);
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
                            if (!string.Equals(creditoMovimientosDescuentoSistema.movNomConcep[0].concepNomDefi.clave, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.cNDescuento.clave, StringComparison.OrdinalIgnoreCase))
                            {
                                #region Cuando son diferentes conceptos tanto para el credito principal como para el descuento, se tiene que buscar el movimiento, si no construirlo
                                indice = obtenerMovNomConcepCreditosAhorroDescuentoActivo(listMovNomConcepCreditosAhorroDescuentoActivo, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.cNDescuento, creditoMovimientosDescuentoSistema, movNomConcepAbarca2Meses);
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
                        if (!string.Equals(creditoMovimientosDescuentoSistema.movNomConcep[0].concepNomDefi.clave, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.cNDescuento.clave, StringComparison.OrdinalIgnoreCase))
                        {
                            #region Cuando son diferentes conceptos tanto para el credito principal como para el descuento, se tiene que buscar el movimiento, si no construirlo
                            indice = obtenerMovNomConcepCreditosAhorroDescuentoActivo(listMovNomConcepCreditosAhorroDescuentoActivo, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.cNDescuento, creditoMovimientosDescuentoSistema, movNomConcepAbarca2Meses);
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
                            if (!string.Equals(creditoMovimientosDescuentoSistema.movNomConcep[0].concepNomDefi.clave, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.cNDescuento.clave, StringComparison.OrdinalIgnoreCase))
                            {
                                #region   Cuando son diferentes conceptos tanto para el credito principal como para el descuento, se tiene que buscar el movimiento, si no construirlo
                                indice = obtenerMovNomConcepCreditosAhorroDescuentoActivo(listMovNomConcepCreditosAhorroDescuentoActivo, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.cNDescuento, creditoMovimientosDescuentoSistema, movNomConcepAbarca2Meses);
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
                       creditoMovimientosDescuentoSistema.creditoPorEmpleado.empleados.razonesSociales.id, tipoCorrida.id, periodosNomina.tipoNomina.id, periodosNomina.año.GetValueOrDefault(),
                       creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.cNDescuento.id,
                       creditoMovimientosDescuentoSistema.creditoPorEmpleado.id, 0);
                    if (!existeDescuentoAplicado)
                    {
                        if (isMov2Meses)
                        {
                            importeDescuento = (importeDescuento / dias) * diasIMSS;
                        }
                        if (!string.Equals(creditoMovimientosDescuentoSistema.movNomConcep[0].concepNomDefi.clave, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.cNDescuento.clave, StringComparison.OrdinalIgnoreCase))
                        {
                            #region Cuando son diferentes conceptos tanto para el credito principal como para el descuento, se tiene que buscar el movimiento, si no construirlo
                            indice = obtenerMovNomConcepCreditosAhorroDescuentoActivo(listMovNomConcepCreditosAhorroDescuentoActivo, creditoMovimientosDescuentoSistema.creditoPorEmpleado.creditoAhorro.cNDescuento, creditoMovimientosDescuentoSistema, movNomConcepAbarca2Meses);
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
                List<MovNomConcep> values = null;
                values = (from o in dbContextSimple.Set<MovNomConcep>()
                          where o.razonesSociales.id == idRazonesSociales && o.empleados.id == idEmpleado
                          && o.tipoNomina.id == idTipoNomina && o.tipoCorrida.id == idTipoCorrida
                          && o.concepNomDefi.id == idConcepto && o.ejercicio == ejercicio && o.creditoMovimientos.creditoPorEmpleado.id == idCreditoPorEmpleado
                          select o).ToList();

                if (values == null ? false : values.Count() > 0)
                {
                    existe = true;
                }
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscarManejoDescuentoUnicoExistente()1_Error: ").Append(ex));
                mensajeResultado.noError = 27;
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return existe;
        }
        private bool eresPeriodoDelMes(PeriodosNomina periodosNominaEjecutandose, bool isMes/*es para saber si buscara por mes o bimestre*/, bool ultimoPeriodoBimestre/*Para saber si va evaluar si es el ultimo periodo del bimestre y claro tiene que venir false isMes*/)
        {
            bool esPeriodoCorrecto = false;
            string claveMAx = "", claveMin = "";
            try
            {
                List<PeriodosNomina> listPeriodosNominas = null;
                if (isMes)
                {
                    var query = from pn in dbContextSimple.Set<PeriodosNomina>()
                                join tn in dbContextSimple.Set<TipoNomina>() on pn.tipoNomina.id equals tn.id
                                where Convert.ToInt32(pn.clave) < Convert.ToInt32(valoresConceptosEmpleados["NumPeriodo".ToUpper()].ToString())
                                && tn.clave == valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString() && pn.cierreMes == true
                                && pn.año == Convert.ToInt32(valoresConceptosEmpleados["AnioPeriodo".ToUpper()]) &&
                                pn.tipoCorrida.clave == valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()].ToString()
                                select new { clave = pn.clave };
                    if (query.Count() > 0)
                    {
                        claveMAx = query.Max(p => p.clave);
                        claveMin = query.Min(p => p.clave);
                    }
                    listPeriodosNominas = (from p in dbContextSimple.Set<PeriodosNomina>()
                                           join t in dbContextSimple.Set<TipoNomina>() on p.tipoNomina.id equals t.id
                                           join c in dbContextSimple.Set<TipoCorrida>() on p.tipoCorrida.id equals c.id
                                           join pd in dbContextSimple.Set<Periodicidad>() on t.periodicidad.id equals pd.id
                                           where (Convert.ToInt32(p.clave) > Convert.ToInt32(claveMAx) && Convert.ToInt32(p.clave) <= Convert.ToInt32(claveMin))
                                           && t.clave == valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString() && p.año == Convert.ToInt32(valoresConceptosEmpleados["AnioPeriodo".ToUpper()])
                                           && c.clave == valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()].ToString()
                                           orderby p.clave
                                           select p).ToList();



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
                    listPeriodosNominas = (from p in dbContextSimple.Set<PeriodosNomina>()
                                           join t in dbContextSimple.Set<TipoNomina>() on p.tipoNomina.id equals t.id
                                           join c in dbContextSimple.Set<TipoCorrida>() on p.tipoCorrida.id equals c.id
                                           join pd in dbContextSimple.Set<Periodicidad>() on t.periodicidad.id equals pd.id
                                           where t.clave == valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString()
                                           && p.año == Convert.ToInt32(valoresConceptosEmpleados["AnioPeriodo".ToUpper()])
                                           && c.clave == valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()].ToString()
                                           && (meses.Contains(p.acumularAMes.GetValueOrDefault().Month) && p.acumularAMes.GetValueOrDefault().Year == Convert.ToInt32(valoresConceptosEmpleados["AnioPeriodo".ToUpper()]))
                                           orderby p.clave
                                           select p).ToList();
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
                    listMovNomConcepCreditosAhorroDescuentoActivo[listMovNomConcepCreditosAhorroDescuentoActivo.Count() - 1].ejercicio = periodosNomina.año.GetValueOrDefault();
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
                if (string.Equals(creditoPorEmpleado.creditoAhorro.concepNomiDefin.clave, filtroMovimientosNominas[z].concepNomDefi.clave, StringComparison.OrdinalIgnoreCase))
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
                    listMovNomConcepCreditosAhorro[listMovNomConcepCreditosAhorro.Count() - 1].ejercicio = periodosNomina.año.GetValueOrDefault();
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
                creditoMovimientos.tiposMovimiento = TiposMovimiento.AbonoSistema;
                creditoMovimientos.fecha = periodosNomina.fechaFinal.GetValueOrDefault();
                creditoMovimientos.movNomConcep = listMovNomConcepCreditosAhorro;
                creditoMovimientos.importe = creditoPorEmpleado.montoDescuento;
            }
            return creditoMovimientos;
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

            if (movNomConcep.concepNomDefi.paraConcepDeNom == null ? false : movNomConcep.concepNomDefi.paraConcepDeNom.Count() == 0 ? false : true)
            {
                movNomConcep.movNomConceParam = (List<MovNomConceParam>)(creaMovNomConceParam(movNomConcep.concepNomDefi, movNomConcep)).resultado;
            }
            movNomConcep.fechaCierr = periodosNominas.fechaCierre;
            movNomConcep.fechaIni = periodosNominas.fechaInicial;
            movNomConcep.tipoPantalla = tipoPantallaSistema;
            movNomConcep.ordenId = 0;
            movNomConcep.resultado = 0.0;
            movNomConcep.numero = 1;
            movNomConcep.calculado = 0.0;
            fechaPeriodo = movNomConcep.periodosNomina.fechaInicial.GetValueOrDefault();
            movNomConcep.ejercicio = periodosNominas.año.GetValueOrDefault(); ;
            movNomConcep.mes = fechaPeriodo.Month;
            movNomConcep.numMovParticion = 1;
            movNomConcep.uso = 0;
            return movNomConcep;
        }

        public Mensaje creaMovNomConceParam(ConcepNomDefi concepNomDefi, MovNomConcep mnc)
        {
            List<MovNomConceParam> movNomConceParam = new List<MovNomConceParam>();
            try
            {
                MovNomConceParam m;
                if (mnc.movNomConceParam == null ? true : mnc.movNomConceParam.Count == 0)
                {
                    foreach (ParaConcepDeNom afecConcepNom in concepNomDefi.paraConcepDeNom)
                    {
                        m = new MovNomConceParam();
                        m.paraConcepDeNom = afecConcepNom;
                        m.movNomConcep = mnc;
                        m.valor = "0";
                        movNomConceParam.Add(m);
                    }
                }
                else if (concepNomDefi.paraConcepDeNom.Count == 0)
                {
                    if (mnc.movNomConceParam.Count > 0)
                    {
                        for (int j = 0; j < mnc.movNomConceParam.Count; j++)
                        {
                            dbContextSimple.Set<MovNomConceParam>().Attach(mnc.movNomConceParam[j]);
                            dbContextSimple.Set<MovNomConceParam>().Remove(mnc.movNomConceParam[j]);
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
                        mnc.movNomConceParam = movNominaBaseAfectasTmp;
                    }
                    for (int i = 0; i < concepNomDefi.paraConcepDeNom.Count; i++)
                    {
                        for (int j = 0; j < mnc.movNomConceParam.Count; j++)
                        {
                            bool existe = false;
                            if (concepNomDefi.paraConcepDeNom[i].id == mnc.movNomConceParam[j].paraConcepDeNom.id)
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
                                m.paraConcepDeNom = concepNomDefi.paraConcepDeNom[i];
                                m.movNomConcep = mnc;
                                m.valor = "0";
                                movNomConceParam.Add(m);
                            }
                        }
                    }
                    if (movNominaBaseAfectasTmp.Count > 0)
                    {
                        for (int j = 0; j < movNominaBaseAfectasTmp.Count; j++)
                        {
                            dbContextSimple.Set<MovNomConceParam>().Attach(movNominaBaseAfectasTmp[j]);
                            dbContextSimple.Set<MovNomConceParam>().Remove(movNominaBaseAfectasTmp[j]);
                        }
                    }
                }
                mensajeResultado.noError = 0;
                mensajeResultado.resultado = movNomConceParam;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("creaMovNomConceParam()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return mensajeResultado;
        }
        private List<MovNomBaseAfecta> creaMovimBaseAfectar(List<BaseAfecConcepNom> baseAfecConcepNominas, MovNomConcep mnc)
        {
            List<MovNomBaseAfecta> movNominaBaseAfectas = new List<MovNomBaseAfecta>(0);
            MovNomBaseAfecta m;
            if (mnc.movNomBaseAfecta == null ? true : mnc.movNomBaseAfecta.Count() == 0)
            {

                foreach (BaseAfecConcepNom afecConcepNom in baseAfecConcepNominas)
                {
                    m = new MovNomBaseAfecta();
                    m.baseAfecConcepNom = afecConcepNom;
                    m.movNomConcep = mnc;
                    m.uso = 0;
                    movNominaBaseAfectas.Add(m);
                }
            }
            else if (baseAfecConcepNominas.Count() == 0)
            {
                if (mnc.movNomBaseAfecta.Count > 0)
                {
                    for (int j = 0; j < mnc.movNomBaseAfecta.Count(); j++)
                    {
                        dbContextSimple.Set<MovNomBaseAfecta>().Attach(mnc.movNomBaseAfecta[j]);
                        dbContextSimple.Set<MovNomBaseAfecta>().Remove(mnc.movNomBaseAfecta[j]);

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
                    mnc.movNomBaseAfecta = movNominaBaseAfectasTmp;
                }
                for (int i = 0; i < baseAfecConcepNominas.Count(); i++)
                {
                    for (int j = 0; j < mnc.movNomBaseAfecta.Count(); j++)
                    {
                        bool existe = false;
                        if (string.Equals(baseAfecConcepNominas[i].baseNomina.clave, mnc.movNomBaseAfecta[j].baseAfecConcepNom.baseNomina.clave, StringComparison.OrdinalIgnoreCase))
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
                if (movNominaBaseAfectasTmp.Count() > 0)
                {
                    for (int j = 0; j < movNominaBaseAfectasTmp.Count(); j++)
                    {
                        dbContextSimple.Set<MovNomBaseAfecta>().Attach(movNominaBaseAfectasTmp[j]);
                        dbContextSimple.Set<MovNomBaseAfecta>().Remove(movNominaBaseAfectasTmp[j]);

                    }
                }
            }
            return movNominaBaseAfectas;
        }
        private Boolean obtenerNumeroParcialidadesCreditoMovimientos(CreditoPorEmpleado creditoPorEmpleado, TiposMovimiento tiposMovimiento, CreditoMovimientos creditoMovimientosDescuentoSistema)
        {
            Boolean continuar = false;
            try
            {
                long numeroCreditoMovimientos = (from credMov in dbContextSimple.Set<CreditoMovimientos>()
                                                 join credEm in dbContextSimple.Set<CreditoPorEmpleado>() on credMov.creditoPorEmpleado.id equals credEm.id
                                                 where credEm.id == creditoPorEmpleado.id && credMov.tiposMovimiento == tiposMovimiento
                                                 select credMov).Count();
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerNumeroParcialidadesCreditoMovimientos()1_Error: ").Append(ex));
                mensajeResultado.noError = 27;
                mensajeResultado.error = ex.GetBaseException().ToString();

            }
            return continuar;
        }
        private CreditoMovimientos obtenerCreditoMovimientosPorPeriodoNomina(CreditoPorEmpleado creditoPorEmpleado, TiposMovimiento tiposMovimiento)
        {
            CreditoMovimientos creditoMovimientos = null;
            try
            {
                creditoMovimientos = (from credMov in dbContextSimple.Set<CreditoMovimientos>()
                                      join credEm in dbContextSimple.Set<CreditoPorEmpleado>() on credMov.creditoPorEmpleado.id equals credEm.id
                                      where credMov.fecha >= periodosNomina.fechaInicial && credMov.fecha <= periodosNomina.fechaFinal
                                      && credEm.id == creditoPorEmpleado.id && credMov.tiposMovimiento == tiposMovimiento
                                      select credMov).SingleOrDefault();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerCreditoMovimientosPorPeriodoNomina()1_Error: ").Append(ex));
                mensajeResultado.noError = 27;
                mensajeResultado.error = ex.GetBaseException().ToString();

            }
            return creditoMovimientos;
        }
        private Double obtenerCantidadPeriodoNominaRango(TipoNomina tipoNomina, PeriodosNomina periodoInicial, PeriodosNomina periodoActual)
        {
            Double cantidad = 0;
            try
            {
                cantidad = (from p in dbContextSimple.Set<PeriodosNomina>()
                            where p.tipoNomina.clave == tipoNomina.clave && p.id >= periodoInicial.id
                            && p.id <= periodoActual.id
                            select p).Count();
                return cantidad;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerCantidadPeriodoNominaRango()1_Error: ").Append(ex));
                mensajeResultado.noError = 27;
                mensajeResultado.error = ex.GetBaseException().ToString();
                return 0.0;
            }
        }
        private CreditoMovimientos obtenerCreditoMovimientosMax(CreditoPorEmpleado creditoPorEmpleado, TiposMovimiento tiposMovimiento)
        {
            CreditoMovimientos creditoMovimientos = null;
            try
            {

                creditoMovimientos = (from credMov in dbContextSimple.Set<CreditoMovimientos>()
                                      join credEm in dbContextSimple.Set<CreditoPorEmpleado>() on credMov.creditoPorEmpleado.id equals credEm.id
                                      where credMov.fecha == ((from credMov2 in dbContextSimple.Set<CreditoMovimientos>()
                                                               join credEm2 in dbContextSimple.Set<CreditoPorEmpleado>() on credMov2.creditoPorEmpleado.id equals credEm2.id
                                                               where credEm2.id == creditoPorEmpleado.id && credMov2.tiposMovimiento == tiposMovimiento && credMov2.fecha <= periodosNomina.fechaFinal
                                                               select new { fecha = credMov2.fecha }).Max(p => p.fecha)) && credEm.id == creditoPorEmpleado.id && credMov.tiposMovimiento == tiposMovimiento
                                      select credMov).SingleOrDefault();


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerCreditoMovimientosMax()1_Error: ").Append(ex));
                mensajeResultado.noError = 27;
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return creditoMovimientos;
        }
        private List<CreditoPorEmpleado> obtenerCreditoPorEmpleado(PlazasPorEmpleadosMov plazasPorEmpleadosMov, String tipoConfiguracion, List<MovNomConcep> listMovNomConcep)
        {
            List<CreditoPorEmpleado> listCreditoPorEmpleado = null;
            try
            {
                List<String> listClaveConcep = new List<String>();
                for (int i = 0; i < listMovNomConcep.Count(); i++)
                {
                    if (listMovNomConcep[i].concepNomDefi != null)
                    {
                        listClaveConcep.Add(listMovNomConcep[i].concepNomDefi.clave);
                    }
                }
                DateTime fechaFinal = periodosNomina.fechaFinal.GetValueOrDefault();
                DateTime fechaInicial = periodosNomina.fechaInicial.GetValueOrDefault();
                listCreditoPorEmpleado = (from cred in dbContextSimple.Set<CreditoPorEmpleado>()
                                          join rs in dbContextSimple.Set<RazonesSociales>() on cred.razonesSociales.id equals rs.id
                                          join creaho in dbContextSimple.Set<CreditoAhorro>() on cred.creditoAhorro.id equals creaho.id
                                          join concep in dbContextSimple.Set<ConcepNomDefi>() on creaho.concepNomiDefin.id equals concep.id
                                          join em in dbContextSimple.Set<Empleados>() on cred.empleados.id equals em.id
                                          where em.clave == plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave
                                          && creaho.tipoConfiguracion == tipoConfiguracion && rs.clave == plazasPorEmpleadosMov.plazasPorEmpleado.razonesSociales.clave
                                          && cred.fechaAutorizacion <= fechaFinal.Date && cred.inicioDescuento <= fechaFinal.Date
                                          && cred.fechaVence >= fechaInicial.Date && listClaveConcep.Contains(concep.clave)
                                          orderby em.clave
                                          select cred).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerCreditoPorEmpleado()1_Error: ").Append(ex));
                mensajeResultado.noError = 27;
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return listCreditoPorEmpleado;
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
                pagos = (from a in dbContextSimple.Set<AguinaldoPagos>()
                         where a.razonesSociales.clave == valoresConceptosGlobales["RazonSocial".ToUpper()].ToString()
                         && a.empleados.clave == plazaEmpleadoaguinaldo.plazasPorEmpleado.empleados.clave
                         && a.tipoNomina.clave == valoresConceptosGlobales["TipoNomina".ToUpper()].ToString()
                         && a.periodosNomina.id == periodosNomina.id
                         select a).SingleOrDefault();

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
                    dbContextSimple.Set<AguinaldoPagos>().AddOrUpdate(pagos);
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
                        dbContextSimple.Set<AguinaldoPagos>().AddOrUpdate(pagos);
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
                                dbContextSimple.Set<MovNomConcep>().AddOrUpdate(filtroMovimientosNominas[i]);
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
                        dbContextSimple.Set<AguinaldoPagos>().AddOrUpdate(pagos);
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
                                dbContextSimple.Set<MovNomConcep>().AddOrUpdate(filtroMovimientosNominas[i]);
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
                        dbContextSimple.Set<AguinaldoPagos>().AddOrUpdate(pagos);
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
                                dbContextSimple.Set<MovNomConcep>().AddOrUpdate(filtroMovimientosNominas[i]);
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

        #region 1.- Calculo nuevo del SDI
        public Mensaje calculoSDI(string claveEmpleado, string claveRazonSocial, DateTime fechaCalculo,
            Dictionary<object, object> paramExtra, DBContextAdapter dbContext, DBContextAdapter dbContextMaster)
        {
            try
            {
                inicializaVariableMensaje();
                DateTime fechaActual = getFechaDelSistema();
                List<PlazasPorEmpleadosMov> listPlazasActivas = null;
                double? salarioDiarioFijo = null;
                double? salarioFijoDiarioIntegrado = null;
                double? salarioVariableDiario = null;
                double? salarioDiarioIntegrado = null;
                Empleados empleado = null;
                int antiguedad = 0;
                DateTime? fechaIngresoEmp = null;
                List<String> listTipoNomina = new List<string>();
                DbContext dbContextSimple = dbContext.context;

                empleado = dbContextSimple.Set<Empleados>().Where(x => x.clave == claveEmpleado &&
                x.razonesSociales.clave == claveRazonSocial).SingleOrDefault();

                #region 1.- Obtener plazas activas
                listPlazasActivas = (from o in dbContextSimple.Set<PlazasPorEmpleadosMov>()
                                     where (from m in dbContextSimple.Set<PlazasPorEmpleadosMov>()
                                            where m.plazasPorEmpleado.empleados.clave == claveEmpleado &&
                                                m.plazasPorEmpleado.razonesSociales.clave == claveRazonSocial &&
                                                fechaActual >= m.fechaInicial && fechaActual <= m.plazasPorEmpleado.fechaFinal
                                            group new { m.plazasPorEmpleado, m } by new
                                            {
                                                m.plazasPorEmpleado.referencia
                                            } into g
                                            select new
                                            {
                                                Column1 = g.Max(p => p.m.id)
                                            }).Contains(new { Column1 = o.id })
                                     orderby o.plazasPorEmpleado.referencia
                                     select o).ToList();

                //if (listPlazasActivas.Count > 0)
                //{
                //    empleado = listPlazasActivas[0].plazasPorEmpleado.empleados;
                //}

                #endregion

                #region 2.- Obtener el salario total de las plazas
                double? salarioTotal = 0.0;

                foreach (var listPlazas in listPlazasActivas)
                {
                    if (!listTipoNomina.Contains(listPlazas.tipoNomina.clave))
                    {
                        listTipoNomina.Add(listPlazas.tipoNomina.clave);
                    }

                    salarioTotal = salarioTotal + listPlazas.importe;
                }


                #endregion

                #region 3.- Obtener el salario diario fijo
                // salarioDiarioFijo = salarioTotal / dias; (Pendientes los días)
                salarioDiarioFijo = salarioTotal;

                #endregion

                #region 4.- Calcular la antiguedad del empleado
                antiguedad = (int)calculaAntiguedad(claveRazonSocial, claveEmpleado, fechaCalculo, TipoAntiguedad.ANTIGUEDAD_ENTERO, dbContext);
                #endregion

                #region 5.- Calcular el factor de integracion
                fechaIngresoEmp = empleado.fechaIngresoEmpresa;
                //fechaCalculo aqui??
                cargaTablaFactorIntegracion("RazonesSociales" + claveRazonSocial, null, true, true, false, false, fechaIngresoEmp.GetValueOrDefault(),
                    getFechaDelSistema().Year, dbContextMaster.context);
                FactorIntegracion fi = buscaFactorIntegracion(antiguedad);
                #endregion

                #region 6.- Salario fijo diario Integrado
                salarioFijoDiarioIntegrado = salarioDiarioFijo * fi.factor;
                #endregion salarioDiarioVariableBim

                #region 7.- Calcula las variables del Bimestre anterior
                if (listPlazasActivas.Count > 0)
                {// Si no hay plazas activas consultar los bimestres anteriores ?
                    salarioVariableDiario = calculaVariablesBimestre(fechaCalculo, claveRazonSocial, claveEmpleado, listTipoNomina, dbContext);
                }
                if (salarioVariableDiario == null)
                {
                    salarioVariableDiario = 0;
                }
                #endregion

                #region 8.- Obtiene el SDI
                salarioDiarioIntegrado = salarioFijoDiarioIntegrado + salarioVariableDiario;
                #endregion


                int? tipoDeSalario = null;

                decimal idRegPt = -1;
                if (paramExtra.ContainsKey("idRegistroPatronal"))
                {
                    idRegPt = Convert.ToDecimal(paramExtra["idRegistroPatronal"]);
                }

                decimal? idFiniq = null;
                if (paramExtra.ContainsKey("idFiniquito"))
                {
                    idFiniq = Convert.ToDecimal(paramExtra["idFiniquito"]);
                }


                SalariosIntegrados sdiToSent = null;
                decimal idSDI = 0;

                #region 9.- Busca si ya existe el SDI con esa llave foranea
                SalariosIntegrados existeSdi = null; /*¿Tiene que devolver mas de un SDI por empleado?*/
                existeSdi = (from si in dbContextSimple.Set<SalariosIntegrados>()
                             where si.fecha <= fechaCalculo.Date && si.empleados.id == empleado.id
                             orderby si.fecha descending
                             select si).Take(1).SingleOrDefault();
                if (existeSdi != null)
                {
                    idSDI = existeSdi.id;
                }

                #endregion

                if (idRegPt == -1)
                {
                    //Mandar error de que no hay Registro Patronal cargado
                }

                //Vamos a verificar el ultimo SDI existente ??
                sdiToSent = new SalariosIntegrados(idSDI,
                                          fi.factor, fechaCalculo,
                                           (double)salarioDiarioFijo, (double)salarioFijoDiarioIntegrado,
                                          (double)salarioVariableDiario, tipoDeSalario,
                                          empleado.id,
                                          idFiniq,
                                          idRegPt
                                         /*Que regisro patronal seva a cargar*/);

                mensajeResultado.resultado = sdiToSent;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";

                // getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("calculoSDI()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();

            }
            return mensajeResultado;

        }

        private object calculaAntiguedad(string claveRazonSocial, string claveEmpleado, DateTime? fecha, TipoAntiguedad tipoAntiguedad, DBContextAdapter dbContext)
        {
            try
            {
                DateTime? fechaInicialAnt = null;
                DateTime? fechaFinalAnt = null;
                DbContext dbContextSimple = dbContext.context;

                IngresosBajas ingresos = (from ingre in dbContextSimple.Set<IngresosBajas>()
                                          where ingre.razonesSociales.clave == claveRazonSocial
                                          && ingre.empleados.clave == claveEmpleado
                                          select ingre).SingleOrDefault();
                if (ingresos != null)
                {
                    fechaInicialAnt = ingresos.fechaIngreso;
                    fechaFinalAnt = fecha;
                }

                if (fechaInicialAnt != null && fechaFinalAnt != null)
                {
                    long diferencia;
                    diferencia = (long)((fechaFinalAnt.GetValueOrDefault() - fechaInicialAnt.GetValueOrDefault()).TotalMilliseconds);
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
            }

            catch (Exception ex)
            {
                mensajeResultado.noError = 22;
                mensajeResultado.error = String.Concat("Error al calcular antiguedad", " ", ex.GetBaseException().ToString());
            }

            return 0.0;
        }

        private FactorIntegracion buscaFactorIntegracion(int antiguedad)
        {
            FactorIntegracion factorDatInte = null;
            try
            {

                int antiguedadFactor = antiguedad == 0 ? 1 : antiguedad;
                List<int> aniosAntiguedad = new List<int>();
                for (int index = 0; index < tablaFactorIntegracion.GetLength(0); index++)
                {
                    aniosAntiguedad.Add(Int16.Parse((string)tablaFactorIntegracion[index, 0]));
                }
                int buscaAnioAnt = aniosAntiguedad.Aggregate((x, y) => Math.Abs(x - antiguedadFactor) < Math.Abs(y - antiguedadFactor) ? x : y);
                int indexPos = aniosAntiguedad.IndexOf(buscaAnioAnt);
                if (antiguedadFactor < buscaAnioAnt)
                {
                    indexPos--;
                }
                factorDatInte = new FactorIntegracion(new object[] { tablaFactorIntegracion[indexPos, 0],
                                tablaFactorIntegracion[indexPos, 1], tablaFactorIntegracion[indexPos, 2],
                                tablaFactorIntegracion[indexPos, 3] , tablaFactorIntegracion[indexPos, 4] });

            }
            catch (Exception ex)
            {
                mensajeResultado.noError = 23;
                mensajeResultado.error = String.Concat("Error al buscar factor integracion para SDI", " ", ex.GetBaseException().ToString());
            }
            return factorDatInte;
        }


        //FFC
        private Double calculaVariablesBimestre(DateTime fechaCalculo, string claveRazonSocial, string claveEmpleado, List<string> clavesTipoNomEmpl, DBContextAdapter dbContext)
        {
            DateTime fechaBimestre = fechaCalculo;
            DateTime fechaIni = getFechaDelSistema();
            DateTime fechaFin = getFechaDelSistema();
            int yearbim = fechaBimestre.Year;

            #region Extracion de Meses y Fecha incial y Fecha final de Bimestre Anterior
            if ((fechaBimestre.Month) % 2 == 0)
            {
                fechaBimestre.AddMonths(fechaBimestre.Month - 2);
            }
            else
            {
                fechaBimestre.AddMonths(fechaBimestre.Month - 1);
            }
            List<int?> meses = new List<int?>();
            if (fechaBimestre.Month == 1 || fechaBimestre.Month == 2)
            {
                meses.Add(11);
                meses.Add(12);
                yearbim = yearbim - 1;
                fechaIni = new DateTime(yearbim, 11, 1);
                fechaFin = new DateTime(yearbim, 12, 31);
            }
            else if (fechaBimestre.Month == 3 || fechaBimestre.Month == 4)
            {
                meses.Add(1);
                meses.Add(2);
                fechaIni = new DateTime(yearbim, 1, 1);
                if (((yearbim % 100 == 0) && (yearbim % 400 == 0)) || ((yearbim % 100 != 0) && (yearbim % 4 == 0)))
                {
                    fechaFin = new DateTime(yearbim, 2, 29);
                }
                else
                {
                    fechaFin = new DateTime(yearbim, 2, 28);
                }

            }
            else if (fechaBimestre.Month == 5 || fechaBimestre.Month == 6)
            {
                meses.Add(3);
                meses.Add(4);
                fechaIni = new DateTime(yearbim, 3, 1);
                fechaFin = new DateTime(yearbim, 4, 30);
            }
            else if (fechaBimestre.Month == 7 || fechaBimestre.Month == 8)
            {
                meses.Add(5);
                meses.Add(6);
                fechaIni = new DateTime(yearbim, 5, 1);
                fechaFin = new DateTime(yearbim, 6, 30);
            }
            else if (fechaBimestre.Month == 9 || fechaBimestre.Month == 10)
            {
                meses.Add(7);
                meses.Add(8);
                fechaIni = new DateTime(yearbim, 7, 1);
                fechaFin = new DateTime(yearbim, 8, 31);
            }
            else if (fechaBimestre.Month == 11 || fechaBimestre.Month == 12)
            {
                meses.Add(9);
                meses.Add(10);
                fechaIni = new DateTime(yearbim, 9, 1);
                fechaFin = new DateTime(yearbim, 10, 31);
            }
            #endregion

            DbContext dbContextSimple = dbContext.context;
            List<MovNomConcep> movNomCnc = null;
            var paramTipoBaseAfectaIMSSVariable = Convert.ToInt32(ClavesParametrosModulos.tipoBaseAfectaIMSSVariable);
            movNomCnc = (from m in dbContextSimple.Set<MovNomConcep>()
                         join p in dbContextSimple.Set<PeriodosNomina>() on m.periodosNomina.id equals p.id
                         join tn in dbContextSimple.Set<TipoNomina>() on m.tipoNomina.id equals tn.id
                         join em in dbContextSimple.Set<Empleados>() on m.empleados.id equals em.id
                         /*join tc in dbContextSimple.Set<TipoCorrida>() on m.tipoCorrida.id equals tc.id*/
                         join c in dbContextSimple.Set<ConcepNomDefi>() on m.concepNomDefi.id equals c.id
                         from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                         join ba in dbContextSimple.Set<BaseAfecConcepNom>() on mba.baseAfecConcepNom.id equals ba.id
                         join bn in dbContextSimple.Set<BaseNomina>() on ba.baseNomina.id equals bn.id
                         join rs in dbContextSimple.Set<RazonesSociales>() on m.razonesSociales.id equals rs.id
                         where mba.movNomConcep.id == m.id && m.uso == 0 && meses.Contains(m.mes) && rs.clave == claveRazonSocial
                         && clavesTipoNomEmpl.Contains(tn.clave)
                         && em.clave == claveEmpleado/* && tc.clave == paramClaveTipoCorrida*/
                         && bn.clave == ClavesParametrosModulos.claveBaseNominaIMSS.ToString() && ba.tipoAfecta == paramTipoBaseAfectaIMSSVariable
                         && p.año == yearbim &&
                         (p.fechaInicial >= fechaIni.Date && p.fechaFinal <= fechaFin.Date)
                         select m).ToList();


            if (movNomCnc.Count > 0)
            {
                double sumAcumVariable = (double)movNomCnc.Sum(item => item.calculado);
                double diasBimAnterior = (fechaFin - fechaIni).TotalDays + 1;
                Double acumuladoVariablesBimAnte = (sumAcumVariable / diasBimAnterior);
                return acumuladoVariablesBimAnte;
            }
            return 0.0;
        }


        #endregion

        public Mensaje calculaSalarioDiarioIntegerado(string claveEmpIni, string claveEmpFin, string claveTipoNomina, string claveTipoCorrida,
            decimal? idPeriodoNomina, string clavePuesto, string claveCategoriasPuestos, string claveTurno, string claveRazonSocial,
            string claveRegPatronal, string claveFormaDePago, string claveDepto, string claveCtrCosto, int? tipoSalario, string tipoContrato,
            bool? status, string controlador, int uso, ParametrosExtra parametrosExtra, bool soloCalculo,
            bool peticionModuloCalculoSalarioDiarioIntegrado, DBContextAdapter dbContext, DBContextAdapter dbContextMaster)
        {

            dbContextAdapterSimple = dbContext;
            dbContextAdapterMaestra = dbContextMaster;
            dbContextMaestra = dbContextMaster.context;

            double sdi = 0.0;
            manejaPagosPorHora = null;
            factorAnual = null;
            manejoHorasPor = null;
            manejoSalarioDiario = null;
            tipoTablaISR = null;
            //valoresConceptosGlobales = new dire
            isCalculoSDI = true;
            isUMA = false;
            DateTime cIni = DateTime.Now, cFin = DateTime.Now;


            using (dbContextSimple = dbContext.context)
            {
                var hasTransaction = dbContextSimple.Database.CurrentTransaction;
                using (transacion = dbContextSimple.Database.CurrentTransaction ?? dbContextSimple.Database.BeginTransaction())
                {

                    if (parametrosExtra.fechaInicioPeriodo == null || parametrosExtra.fechaFinPeriodo == null)
                    {
                        if (parametrosExtra.fechaCalculoSDI == null)
                        {
                            parametrosExtra.fechaFinPeriodo = cIni;
                            parametrosExtra.fechaFinPeriodo = cFin;
                            if (fechaActual == null)
                            {
                                fechaActual = DateTime.Now;
                                fechaActual = cIni;
                            }
                        }
                        else
                        {

                            parametrosExtra.fechaInicioPeriodo = parametrosExtra.fechaCalculoSDI;
                            parametrosExtra.fechaFinPeriodo = parametrosExtra.fechaCalculoSDI;
                            if (fechaActual == null)
                            {
                                fechaActual = DateTime.Now;
                                fechaActual = parametrosExtra.fechaCalculoSDI.GetValueOrDefault();
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
                            factorRedondeoGral = Convert.ToDouble(factorString);
                            minimoRedondeoGral = Convert.ToDouble(minimunString);
                        }
                    }

                    if (parametrosExtra.tipoAccionMascaras != null)
                    {
                        tipoAccionMascarasGral = parametrosExtra.tipoAccionMascaras.GetValueOrDefault();
                    }
                    cIni = parametrosExtra.fechaFinPeriodo.GetValueOrDefault();
                    cFin = parametrosExtra.fechaFinPeriodo.GetValueOrDefault();
                    if (fechaActual == null)
                    {
                        fechaActual = getFechaDelSistema();
                        fechaActual = cIni;
                    }

                    if (valoresConceptosGlobales == null)
                    {
                        valoresConceptosGlobales = new Dictionary<string, object>();
                    }

                    valoresConceptosGlobales.Add(parametroFechaInicial, cIni);
                    valoresConceptosGlobales.Add(parametroFechaFinal, cFin);
                    valoresConceptosGlobales.Add("ClaveTipoCorrida".ToUpper(), claveTipoCorrida);
                    SalariosIntegrados ultimoSalarioIntegrado;
                    mensajeResultado = new Mensaje();
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    mensajeResultado.resultado = sdi;
                    if (mensajeResultado.noError != 0)
                    {
                        return mensajeResultado;
                    }
                    try
                    {
                        buscaPeriodicidadesOrPeriodosNomina(claveTipoNomina, claveTipoCorrida, idPeriodoNomina);
                        if (mensajeResultado.noError != 0)
                        {
                            return mensajeResultado;

                        }
                        //setSession(dbContext);
                        //getSession().Database.BeginTransaction();

                        metodosBDMaestra.obtenerFactores(claveRazonSocial, periodicidadTipoNomina, dbContextMaestra);
                        factorAnual = metodosBDMaestra.factorAnual;
                        manejaPagosPorHora = metodosBDMaestra.manejaPagosPorHora;
                        manejoHorasPor = metodosBDMaestra.manejoHorasPor;
                        factorMensual = metodosBDMaestra.factorMensual;
                        tipoTablaISR = metodosBDMaestra.tipoTablaISR;
                        manejaPagoDiasNaturales = metodosBDMaestra.manejaPagoDiasNaturales;
                        versionCalculoPrestamoAhorro = metodosBDMaestra.versionCalculoPrestamoAhorro;
                        manejaPagoIMSSDiasNaturales = metodosBDMaestra.manejaPagoIMSSDiasNaturales;
                        descontarFaltasModoAjustaMes = metodosBDMaestra.descontarFaltasModoAjustaMes;
                        pagarVacaAuto = metodosBDMaestra.pagarVacaAuto;
                        salarioVacaciones = metodosBDMaestra.salarioVacaciones;
                        isUMA = metodosBDMaestra.isUMA;
                        manejoSalarioDiario = metodosBDMaestra.manejoSalarioDiario;
                        calculoSeparadoISR = metodosBDMaestra.calculoSeparadoISR;
                        mensajeResultado = metodosBDMaestra.mensajeResultado;

                        if (mensajeResultado.noError != 0)
                        {
                            return mensajeResultado;
                        }

                        cargaTablaFactorIntegracion(controlador, null, true, true, isUMA, false, cFin, cFin.Year, dbContextMaestra);
                        if (mensajeResultado.noError != 0)
                        {
                            return mensajeResultado;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("cargaTablaFactorIntegracion()1_Error: ").Append(ex));
                        mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                        mensajeResultado.error = ex.Message.ToString();
                        mensajeResultado.resultado = null;
                        return mensajeResultado;
                    }
                    //finally
                    //{
                    //    setSession(null);
                    //}
                    //  setSession(dbContext.context);
                    //getSession().Database.BeginTransaction();
                    cargarVariablesConceptosCompilador();
                    List<PlazasPorEmpleadosMov> plazasPorEmpleadosMov;
                    inicializaValoresPeriodoNomina(periodosNomina);
                    if (!soloCalculo)
                    {
                        plazasPorEmpleadosMov = obtenerPlazasPorEmpleados(claveEmpIni, claveEmpFin, claveTipoNomina, clavePuesto, claveCategoriasPuestos, claveTurno, claveRazonSocial, claveRegPatronal, claveDepto, claveCtrCosto, tipoSalario.GetValueOrDefault(), tipoContrato, status, claveTipoCorrida, claveFormaDePago, parametrosExtra.fechaInicioPeriodo.GetValueOrDefault(), parametrosExtra.fechaFinPeriodo.GetValueOrDefault());
                    }
                    else
                    {
                        plazasPorEmpleadosMov = (List<PlazasPorEmpleadosMov>)parametrosExtra.valoresExtras[0];
                    }
                    plazasPorEmpleadosMov = (plazasPorEmpleadosMov == null) ? new List<PlazasPorEmpleadosMov>() : plazasPorEmpleadosMov;
                    if (soloCalculo)
                    {
                        bool isEmpty = !plazasPorEmpleadosMov.Any();
                        if (isEmpty)//is empty
                        {
                            plazasPorEmpleadosMov.Add(new PlazasPorEmpleadosMov());
                        }
                    }
                    if (plazasPorEmpleadosMov.Any() || soloCalculo)
                    {
                        try
                        {
                            valoresConceptosEmpleados = new Dictionary<string, object>();
                            DateTime fechaActualCalculoSDI = DateTime.Now;
                            if (parametrosExtra.fechaCalculoSDI != null)
                            {
                                fechaActualCalculoSDI = parametrosExtra.fechaCalculoSDI.GetValueOrDefault();
                            }
                            int i, j;
                            if (isUMA)
                            {
                                salarioMinimoDF = valorUMA;
                                valoresConceptosGlobales.Add("SalarioMinDF".ToUpper(), valorUMA);
                            }
                            else
                            {
                                ZonaSalarial salarioZona = buscaSalarioPorZona('A');
                                if (salarioZona == null)
                                {
                                    mensajeResultado.noError = 40;
                                    mensajeResultado.error = "No existe Zona Salarial A";
                                    return mensajeResultado;
                                }
                                salarioMinimoDF = salarioZona.salario;
                                valoresConceptosEmpleados.Add("SalarioMinDF".ToUpper(), salarioMinimoDF);
                            }
                            String claveNominaTemp = "";
                            List<SalariosIntegradosDet> salarioIntegradosDet = new List<SalariosIntegradosDet>();
                            double sdf = 0, sdif, sdiv, factorIntegracion, sueldoDiario, diasTotalesDelPeriodo;//diasTotalesDelPeriodo nos indica los dias que tuvo el periodo para poner obtener un salario diario fijo.
                            for (i = 0; i < plazasPorEmpleadosMov.Count(); i++)
                            {
                                sdif = 0;
                                sdiv = 0;
                                if (!string.Equals(claveNominaTemp, soloCalculo ? claveTipoNomina : plazasPorEmpleadosMov[i].tipoNomina.clave, StringComparison.OrdinalIgnoreCase))
                                {
                                    mensajeResultado = metodosPeriodosNomina.buscaPeriodoNominaActual(soloCalculo ? claveTipoNomina : plazasPorEmpleadosMov[i].tipoNomina.clave, claveTipoCorrida, idPeriodoNomina, fechaActualCalculoSDI, (DBContextSimple)dbContextSimple);
                                    if (mensajeResultado.noError == 0)
                                    {
                                        periodosNomina = (PeriodosNomina)mensajeResultado.resultado;
                                    }
                                    else
                                    {
                                        return null;
                                    }
                                    claveNominaTemp = soloCalculo ? claveTipoNomina : plazasPorEmpleadosMov[i].tipoNomina.clave;
                                }
                                inicializaValoresPeriodoNomina(periodosNomina);
                                foreach (var item in valoresConceptosGlobales)
                                {
                                    valoresConceptosEmpleados.Add(item.Key, item.Value);
                                }
                                mensajeResultado = metodosDatosEmpleados.obtenerIngresosReIngresosBajas(plazasPorEmpleadosMov[i], periodosNomina.fechaFinal, (DBContextSimple)dbContextSimple);
                                if (mensajeResultado.noError == 0)
                                {
                                    ingresosReingresosBajas = (IngresosBajas)mensajeResultado.resultado;
                                }
                                else
                                {
                                    return null;
                                }
                                cargarVariablesGlobalesEmpleadoPorPlaza(plazasPorEmpleadosMov[i], true, true, TipoSueldos.SUELDO_DIARIO_FINAL, null, null, null, fechaActualCalculoSDI);
                                if (mensajeResultado.noError != 0)
                                {
                                    return mensajeResultado;
                                }
                                sueldoDiario = Convert.ToDouble(valoresConceptosEmpleados["SUELDODIARIO".ToUpper()]);
                                if (valoresConceptosEmpleados["FactorIntegracion".ToUpper()] == null)
                                {
                                    mensajeResultado.noError = 25;
                                    mensajeResultado.error = "No existen el factor de integracion capturado, favor de verificarlo";
                                    return mensajeResultado;
                                }
                                factorIntegracion = Convert.ToDouble(valoresConceptosEmpleados["FactorIntegracion".ToUpper()]);
                                var diasAPagar = Convert.ToInt32(valoresConceptosEmpleados["DiasNormalesAPagar".ToUpper()]);
                                diasTotalesDelPeriodo = diasAPagar;
                                if (peticionModuloCalculoSalarioDiarioIntegrado)
                                {
                                    if (periodosNomina != null)
                                    {
                                        sdf = salarioDiarioFijo(claveTipoCorrida, claveTipoNomina, periodosNomina.id, plazasPorEmpleadosMov[i].plazasPorEmpleado, claveCtrCosto, claveRazonSocial) / diasTotalesDelPeriodo; ;
                                    }
                                }
                                sdif = sdif + (((sdf == 0 ? sueldoDiario : sdf)) * factorIntegracion);
                                //sdif = sueldoDiario;
                                sdiv = sdiv + salarioDiarioVariableBim(fechaActualCalculoSDI);

                                ultimoSalarioIntegrado = buscarSalarioIntegradoActual(fechaActualCalculoSDI, plazasPorEmpleadosMov[i].plazasPorEmpleado.empleados, (DBContextSimple)dbContextSimple);
                                if (ultimoSalarioIntegrado != null ? ultimoSalarioIntegrado.salarioDiarioIntegrado > -1 : false)
                                {
                                    salarioIntegradosDet = buscarSalarioIntegradosDetalle(ultimoSalarioIntegrado, (DBContextSimple)dbContextSimple);
                                }

                                bool continuar = true, continuarElimacion = true;
                                if (ultimoSalarioIntegrado != null)
                                {
                                    // double sdiTrunc = Utilerias.truncateDecimal(ultimoSalarioIntegrado.salarioDiarioIntegrado, 2);
                                    double sdiTrunc = ultimoSalarioIntegrado.salarioDiarioIntegrado;
                                    //double sdfTrunc = Utilerias.truncateDecimal(ultimoSalarioIntegrado.salarioDiarioFijo, 2);
                                    double sdfTrunc = ultimoSalarioIntegrado.salarioDiarioFijo;
                                    //double sdvTrunc = Utilerias.truncateDecimal(ultimoSalarioIntegrado.salarioDiarioVariable, 2);
                                    double sdvTrunc = ultimoSalarioIntegrado.salarioDiarioVariable;
                                    //double sdifTrunc = Utilerias.truncateDecimal(sdif, 2);
                                    double sdifTrunc = sdif;
                                    double sdivTrunc = Utilerias.truncateDecimal(sdiv, 2);
                                    if (sdiTrunc == (sdifTrunc + sdivTrunc) & sdfTrunc == sdifTrunc
                                            & sdvTrunc == sdivTrunc)
                                    {
                                        continuar = false;
                                    }
                                }
                                if (continuar)
                                {
                                    if (ultimoSalarioIntegrado.fecha == null ? true : ultimoSalarioIntegrado.fecha.GetValueOrDefault().Date.CompareTo(fechaActualCalculoSDI.Date) != 0)
                                    {
                                        ultimoSalarioIntegrado = null;
                                        continuar = true;
                                        continuarElimacion = false;
                                        if (salarioIntegradosDet != null)
                                        {
                                            salarioIntegradosDet.Clear();
                                        }
                                    }
                                    else if (ultimoSalarioIntegrado.fecha.GetValueOrDefault().CompareTo(fechaActualCalculoSDI) == 0 & (salarioIntegradosDet == null ? true : salarioIntegradosDet.Any()))
                                    {
                                        salarioIntegradosDet = buscarSalarioIntegradosDetalle(ultimoSalarioIntegrado, (DBContextSimple)dbContextSimple);
                                    }
                                }
                                if (continuar)
                                {
                                    if (continuarElimacion)
                                    {
                                        if (filtroMovimientosNominas != null || filtroConceptosNomina != null)
                                        {
                                            if ((filtroMovimientosNominas == null ? false : filtroMovimientosNominas.Count() > 0) || (filtroConceptosNomina == null ? false : filtroConceptosNomina.Count() > 0))
                                            {
                                                for (int k = 0; k < salarioIntegradosDet.Count(); k++)
                                                {
                                                    dbContextSimple.Set<SalariosIntegradosDet>().Attach(salarioIntegradosDet[k]);
                                                    dbContextSimple.Set<SalariosIntegradosDet>().Remove(salarioIntegradosDet[k]);
                                                    //getSession().delete(salarioIntegradosDet.get(k));
                                                }
                                                //                                    System.out.println("flush 16");
                                                dbContextSimple.Set<SalariosIntegrados>().Attach(ultimoSalarioIntegrado);
                                                dbContextSimple.Set<SalariosIntegrados>().Remove(ultimoSalarioIntegrado);
                                                dbContextSimple.SaveChanges();
                                                salarioIntegradosDet.Clear();
                                            }
                                        }
                                        else
                                        {
                                            salarioIntegradosDet.Clear();
                                        }
                                    }

                                    List<ConcepNomDefi> conceptosVarFijos = new List<ConcepNomDefi>();
                                    if (filtroMovimientosNominas == null ? false : filtroMovimientosNominas.Count == 0 ? false : true)
                                    {
                                        Double max = 0.0;
                                        String conceptoActual = filtroMovimientosNominas[0].concepNomDefi.clave;
                                        j = 0;
                                        ConcepNomDefi concepNomDefi;
                                        while (j <= filtroMovimientosNominas.Count())
                                        {
                                            if (j == filtroMovimientosNominas.Count())
                                            {
                                                concepNomDefi = filtroMovimientosNominas[j - 1].concepNomDefi;
                                                concepNomDefi.resultado = max;
                                                conceptosVarFijos.Add(concepNomDefi);
                                                max = 0.0;
                                                break;
                                            }
                                            if (conceptoActual.Equals(filtroMovimientosNominas[j].concepNomDefi.clave))
                                            {
                                                max += filtroMovimientosNominas[j].resultado.GetValueOrDefault();
                                            }
                                            else
                                            {
                                                conceptoActual = filtroMovimientosNominas[j].concepNomDefi.clave;
                                                concepNomDefi = filtroMovimientosNominas[j - 1].concepNomDefi;
                                                concepNomDefi.resultado = max;

                                                conceptosVarFijos.Add(concepNomDefi);
                                                //
                                                max = filtroMovimientosNominas[j].resultado.GetValueOrDefault();
                                            }
                                            j++;
                                        }
                                    }
                                    //Reestructuracion de movimientos partidos
                                    /////filtroMovimientosNominas = sumaJuntaMovPartidos(filtroMovimientosNominas);//JEVC01
                                    for (j = 0; j < conceptosVarFijos.Count(); j++)
                                    {
                                        salarioIntegradosDet.Add(contruyeSalarioIntegradoDetallado(plazasPorEmpleadosMov[i], conceptosVarFijos[j], null, conceptosVarFijos[j].resultado, fechaActualCalculoSDI));
                                        //                                salarioIntegradosDet.add(contruyeSalarioIntegradoDetallado(plazasPorEmpleadosMov.get(i), filtroMovimientosNominas.get(j).getConcepNomDefi(), null, filtroMovimientosNominas.get(j).getResultado() == null ? 0.0 : filtroMovimientosNominas.get(j).getResultado(), fechaActualCalculoSDI));
                                    }

                                    if (filtroConceptosNomina != null)
                                    {
                                        for (j = 0; j < filtroConceptosNomina.Count(); j++)
                                        {
                                            salarioIntegradosDet.Add(contruyeSalarioIntegradoDetallado(plazasPorEmpleadosMov[i], filtroConceptosNomina[j], null, filtroConceptosNomina[j].resultado, fechaActualCalculoSDI));
                                        }
                                    }
                                    if (mensajeResultado.noError != 0)
                                    {
                                        break;
                                    }


                                }
                                valoresConceptosEmpleados.Clear();
                                if (continuar)
                                {
                                    ultimoSalarioIntegrado = contruyeSalarioIntegrado(plazasPorEmpleadosMov[i], factorIntegracion, fechaActualCalculoSDI, (sdf == 0 ? sueldoDiario : sdf), sdif, sdiv, ultimoSalarioIntegrado);



                                    if (!soloCalculo)
                                    {
                                        //Guarda salario diario Integrado
                                        cantidadSaveUpdate++;
                                        //ultimoSalarioIntegrado.empleados = null;
                                        //ultimoSalarioIntegrado.registroPatronal = null;
                                        dbContextSimple.Set<SalariosIntegrados>().AddOrUpdate(ultimoSalarioIntegrado);
                                        dbContextSimple.SaveChanges();
                                        //getSession().saveOrUpdate(ultimoSalarioIntegrado);

                                        //guarda conceptos nomina utilizados calculo SDI
                                        for (j = 0; j < salarioIntegradosDet.Count(); j++)
                                        {
                                            salarioIntegradosDet[j].salariosIntegrados = ultimoSalarioIntegrado;
                                            cantidadSaveUpdate++;
                                            dbContextSimple.Set<SalariosIntegradosDet>().AddOrUpdate(salarioIntegradosDet[j]);
                                            //getSession().saveOrUpdate(salarioIntegradosDet.get(j));
                                            if (salarioIntegradosDet.Count() >= 50)
                                            {
                                                //                                    
                                                dbContextSimple.SaveChanges();
                                                //                                  
                                            }

                                        }

                                        dbContextSimple.SaveChanges();
                                    }
                                    else
                                    {
                                        SalariosIntegrados sdiToSent =
                                        new SalariosIntegrados(ultimoSalarioIntegrado.id,
                                            ultimoSalarioIntegrado.factorIntegracion, ultimoSalarioIntegrado.fecha,
                                            ultimoSalarioIntegrado.salarioDiarioFijo, ultimoSalarioIntegrado.salarioDiarioIntegrado,
                                            ultimoSalarioIntegrado.salarioDiarioVariable, ultimoSalarioIntegrado.tipoDeSalario,
                                            ultimoSalarioIntegrado.empleados_ID, ultimoSalarioIntegrado.finiquitosLiquidaciones_ID,
                                            ultimoSalarioIntegrado.registroPatronal_ID);

                                        mensajeResultado.resultado = sdiToSent;
                                    }

                                }
                                else
                                {
                                    mensajeResultado.resultado = ultimoSalarioIntegrado;
                                }
                                //aqui voy



                            }

                            if (!soloCalculo)
                            {
                                dbContextSimple.Database.CurrentTransaction.Commit();
                            }
                            //else
                            //{
                            //dbContextSimple.Database.CurrentTransaction.Rollback();
                            // }

                            //if (hasTransaction == null)
                            //{
                            dbContextSimple.Database.Connection.Close();
                            // }

                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("calculaSalarioDiarioIntegerado()1_Error: ").Append(ex));
                            mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                            mensajeResultado.error = ex.Message.ToString();
                            mensajeResultado.resultado = null;
                        }
                        periodosNomina = null;
                        zonaGeografica = null;
                        salarioMinimoDF = null;
                        filtroConceptosNomina = null;
                        filtroMovimientosNominas = null;
                        valoresConceptosEmpleados = null;
                    }
                    if (!plazasPorEmpleadosMov.Any())
                    {
                        mensajeResultado.noError = 999;
                        claveEmpIni = claveEmpIni == null ? "" : claveEmpIni;
                        claveEmpFin = claveEmpFin == null ? "" : claveEmpFin;
                        if (claveEmpIni.Length > 0 & claveEmpFin.Length > 0)
                        {
                            mensajeResultado.error = "No encontro empleados";
                        }
                        else if (claveEmpIni.Length > 0 | claveEmpFin.Length > 0)
                        {
                            mensajeResultado.error = "No encontro el empleado";
                        }
                        else
                        {
                            mensajeResultado.error = "No encontro empleados";
                        }
                    }
                    isCalculoSDI = false;
                }
            }

            return mensajeResultado;

        }
        private SalariosIntegrados contruyeSalarioIntegrado(PlazasPorEmpleadosMov plazasPorEmpleadosMov, double factorIntegracion,
            DateTime fechaCalculoSDI, double salarioDiario, double salarioDiarioIntFijo, double salarioDiarioIntVariable, SalariosIntegrados nuevoSalariosIntegrados)
        {
            SalariosIntegrados salariosIntegrados;
            if (nuevoSalariosIntegrados == null)
            {
                salariosIntegrados = new SalariosIntegrados();
            }
            else
            {
                salariosIntegrados = nuevoSalariosIntegrados;
            }
            double sdi;
            salariosIntegrados.empleados_ID = plazasPorEmpleadosMov.plazasPorEmpleado.empleados_ID.GetValueOrDefault();
            salariosIntegrados.empleados = plazasPorEmpleadosMov.plazasPorEmpleado.empleados;
            salariosIntegrados.fecha = quitaHrsDeFecha(fechaCalculoSDI);
            salariosIntegrados.factorIntegracion = factorIntegracion;
            salariosIntegrados.registroPatronal_ID = plazasPorEmpleadosMov.plazasPorEmpleado.registroPatronal_ID;
            salariosIntegrados.registroPatronal = plazasPorEmpleadosMov.plazasPorEmpleado.registroPatronal;
            // salariosIntegrados.salarioDiarioFijo = aplicarMascara(null, salarioDiarioIntFijo, true);
            salariosIntegrados.salarioDiarioFijo = aplicarMascara(null, salarioDiario, true);
            salariosIntegrados.salarioDiarioIntegrado = aplicarMascara(null, salarioDiarioIntFijo, true);
            salariosIntegrados.salarioDiarioVariable = aplicarMascara(null, salarioDiarioIntVariable, true);
            //tipo de Salario
            if (salariosIntegrados.salarioDiarioFijo > 0 & salariosIntegrados.salarioDiarioVariable > 0)
            {
                salariosIntegrados.tipoDeSalario = Convert.ToInt32(ClavesParametrosModulos.tipoSalarioMixto);
            }
            else if (salariosIntegrados.salarioDiarioVariable > 0)
            {
                salariosIntegrados.tipoDeSalario = Convert.ToInt32(ClavesParametrosModulos.tipoSalarioVariable);
            }
            else
            {
                salariosIntegrados.tipoDeSalario = Convert.ToInt32(ClavesParametrosModulos.tipoSalarioFijo);
            }
            sdi = salariosIntegrados.salarioDiarioIntegrado + salariosIntegrados.salarioDiarioVariable;

            if (sdi > (salarioMinimoDF * 25))
            {  //Tope Salario Diario Integrado
                salariosIntegrados.salarioDiarioIntegrado = aplicarMascara(null, salarioMinimoDF.GetValueOrDefault() * 25, true);
            }
            else
            {
                salariosIntegrados.salarioDiarioIntegrado = sdi;
            }
            return salariosIntegrados;
        }
        private SalariosIntegradosDet contruyeSalarioIntegradoDetallado(PlazasPorEmpleadosMov plazasPorEmpleadosMov, ConcepNomDefi concepNomDefi, SalariosIntegrados sdi, double importe, DateTime fechaCalculoSDI)
        {
            SalariosIntegradosDet salariosIntegradosDet = new SalariosIntegradosDet();
            salariosIntegradosDet.salariosIntegrados = sdi;
            salariosIntegradosDet.ConcepNomDefi = concepNomDefi;

            salariosIntegradosDet.cambio = SalariosIntegradosDet.Cambio.CAMBIO;
            salariosIntegradosDet.fechaCambio = fechaCalculoSDI;
            salariosIntegradosDet.plazasPorEmpleado = plazasPorEmpleadosMov.plazasPorEmpleado;
            salariosIntegradosDet.importe = importe;
            if (concepNomDefi.paraConcepDeNom != null)
            {
                if (concepNomDefi.paraConcepDeNom.Count() > 0)
                {
                    if (string.Equals(concepNomDefi.paraConcepDeNom[0].unidad, "HORAS", StringComparison.OrdinalIgnoreCase))
                    {
                        salariosIntegradosDet.horas = Convert.ToDouble(concepNomDefi.paraConcepDeNom[0].numero);
                    }
                }
            }
            return salariosIntegradosDet;
        }
        private List<SalariosIntegradosDet> buscarSalarioIntegradosDetalle(SalariosIntegrados salariosIntegrados, DBContextSimple dbContextSimple)
        {
            List<SalariosIntegradosDet> salariosIntegradosDets = null;
            try
            {
                salariosIntegradosDets = (from si in dbContextSimple.Set<SalariosIntegradosDet>()
                                          where si.salariosIntegrados.id == salariosIntegrados.id
                                          select si).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscarSalarioIntegradosDetalle()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.Message.ToString();
                mensajeResultado.resultado = null;
                dbContextSimple.Database.CurrentTransaction.Rollback();
            }
            if (salariosIntegrados == null)
            {
                salariosIntegrados = new SalariosIntegrados();
                salariosIntegrados.salarioDiarioIntegrado = -1;
            }
            return salariosIntegradosDets;
        }
        private SalariosIntegrados buscarSalarioIntegradoActual(DateTime fechaCalculoSDI, Empleados empleados, DBContextSimple dbContextSimple)
        {
            SalariosIntegrados salariosIntegrados = null;
            try
            {
                salariosIntegrados = (from si in dbContextSimple.Set<SalariosIntegrados>()
                                      where si.fecha <= fechaCalculoSDI.Date && si.empleados.id == empleados.id
                                      orderby si.fecha descending
                                      select si).Take(1).SingleOrDefault();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscarSalarioIntegradoActual()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.Message.ToString();
                mensajeResultado.resultado = null;
                dbContextSimple.Database.CurrentTransaction.Rollback();
            }
            if (salariosIntegrados == null)
            {
                salariosIntegrados = new SalariosIntegrados();
                salariosIntegrados.salarioDiarioIntegrado = -1;
            }
            return salariosIntegrados;
        }
        private Double salarioDiarioVariableBim(DateTime fechaPeriodo)
        {
            Double acumuladoVariableBim = 0.0;
            try
            {
                DateTime fechaBimestre = DateTime.Now;
                DateTime fechaIni = DateTime.Now;
                DateTime fechaFin = DateTime.Now;
                fechaBimestre = fechaPeriodo;
                int yearbim = fechaBimestre.Year;
                if ((fechaBimestre.Month) % 2 == 0)
                {
                    fechaBimestre.AddMonths(fechaBimestre.Month - 2);
                }
                else
                {
                    fechaBimestre.AddMonths(fechaBimestre.Month - 1);
                }
                List<int?> meses = new List<int?>();
                if (fechaBimestre.Month == 1 || fechaBimestre.Month == 2)
                {
                    meses.Add(11);
                    meses.Add(12);
                    yearbim = yearbim - 1;
                    fechaIni = new DateTime(yearbim, 11, 1);
                    fechaFin = new DateTime(yearbim, 12, 31);
                }
                else if (fechaBimestre.Month == 3 || fechaBimestre.Month == 4)
                {
                    meses.Add(1);
                    meses.Add(2);
                    fechaIni = new DateTime(yearbim, 1, 1);
                    if (((yearbim % 100 == 0) && (yearbim % 400 == 0)) || ((yearbim % 100 != 0) && (yearbim % 4 == 0)))
                    {
                        fechaFin = new DateTime(yearbim, 2, 29);
                    }
                    else
                    {
                        fechaFin = new DateTime(yearbim, 2, 28);
                    }

                }
                else if (fechaBimestre.Month == 5 || fechaBimestre.Month == 6)
                {
                    meses.Add(3);
                    meses.Add(4);
                    fechaIni = new DateTime(yearbim, 3, 1);
                    fechaFin = new DateTime(yearbim, 4, 30);
                }
                else if (fechaBimestre.Month == 7 || fechaBimestre.Month == 8)
                {
                    meses.Add(5);
                    meses.Add(6);
                    fechaIni = new DateTime(yearbim, 5, 1);
                    fechaFin = new DateTime(yearbim, 6, 30);
                }
                else if (fechaBimestre.Month == 9 || fechaBimestre.Month == 10)
                {
                    meses.Add(7);
                    meses.Add(8);
                    fechaIni = new DateTime(yearbim, 7, 1);
                    fechaFin = new DateTime(yearbim, 8, 31);
                }
                else if (fechaBimestre.Month == 11 || fechaBimestre.Month == 12)
                {
                    meses.Add(9);
                    meses.Add(10);
                    fechaIni = new DateTime(yearbim, 9, 1);
                    fechaFin = new DateTime(yearbim, 10, 31);
                }
                List<PeriodosNomina> periodos = new List<PeriodosNomina>();
                mensajeResultado = metodosPeriodosNomina.buscarPeriodosPorRangoMeses(-1, fechaBimestre, valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString(), valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()].ToString(), (DBContextSimple)dbContextSimple);
                if (mensajeResultado.noError == 0)
                {
                    periodos = (List<PeriodosNomina>)mensajeResultado.resultado;
                }
                else if (mensajeResultado.noError != 0 && periodos.Any())
                {
                    return 0.0;
                }
                int diasIMSS = 0;
                DateTime cInicioImss = DateTime.Now, cInicioPeriodo = DateTime.Now;
                cInicioPeriodo = fechaPeriodo;
                int diasDif = 0;
                if (cInicioImss.CompareTo(cInicioPeriodo) > 0)
                {
                    diasDif = cInicioImss.Day - cInicioPeriodo.Day;
                }

                var paramPlazaEmpleado = valoresConceptosEmpleados["PlazaEmpleado".ToUpper()].ToString();
                var paramTipoNomina = valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString();
                var paramNumEmpleado = valoresConceptosEmpleados["NumEmpleado".ToUpper()].ToString();
                var paramClaveTipoCorrida = valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()].ToString();
                var paramRazonSocial = valoresConceptosEmpleados["RazonSocial".ToUpper()].ToString();
                var paramTipoBaseAfectaIMSSVariable = Convert.ToInt32(ClavesParametrosModulos.tipoBaseAfectaIMSSVariable);

                var query = from m in dbContextSimple.Set<MovNomConcep>()
                            join p in dbContextSimple.Set<PeriodosNomina>() on m.periodosNomina.id equals p.id
                            join tn in dbContextSimple.Set<TipoNomina>() on m.tipoNomina.id equals tn.id
                            join em in dbContextSimple.Set<Empleados>() on m.empleados.id equals em.id
                            join tc in dbContextSimple.Set<TipoCorrida>() on m.tipoCorrida.id equals tc.id
                            join c in dbContextSimple.Set<ConcepNomDefi>() on m.concepNomDefi.id equals c.id
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            join ba in dbContextSimple.Set<BaseAfecConcepNom>() on mba.baseAfecConcepNom.id equals ba.id
                            join bn in dbContextSimple.Set<BaseNomina>() on ba.baseNomina.id equals bn.id
                            join rs in dbContextSimple.Set<RazonesSociales>() on m.razonesSociales.id equals rs.id
                            join pemp in dbContextSimple.Set<PlazasPorEmpleado>() on m.plazasPorEmpleado.id equals pemp.id
                            where mba.movNomConcep.id == m.id && m.uso == 0 && meses.Contains(m.mes) && rs.clave == paramRazonSocial
                            && pemp.referencia == paramPlazaEmpleado && tn.clave == paramTipoNomina
                            && em.clave == paramNumEmpleado && tc.clave == paramClaveTipoCorrida
                            && bn.clave == ClavesParametrosModulos.claveBaseNominaIMSS.ToString() && ba.tipoAfecta == paramTipoBaseAfectaIMSSVariable
                            && p.tipoCorrida.clave == paramClaveTipoCorrida && p.año == yearbim &&
                            (p.fechaInicial >= fechaIni.Date && p.fechaFinal <= fechaFin.Date)
                            select new { m, p, tn, em, tc, c, mba, bn, rs, pemp };
                //if (periodos.Any())
                //{//pendiente revisar ok
                //    int i;
                //    for (i = 0; i < periodos.Count(); i++)
                //    {
                //        string clavePeriodo = periodos[i].clave;
                //        int anioo = periodos[i].año.GetValueOrDefault();
                //        query = from sub in query
                //                where (sub.p.clave == clavePeriodo && sub.p.año == anioo)
                //                select sub;
                //    }
                //}

                var subquery = from sub in query
                               select new { sub.m };

                List<MovNomConcep> movNominaVariables = subquery.Select(p => p.m).ToList();
                movNominaVariables = (movNominaVariables == null) ? new List<MovNomConcep>() : movNominaVariables;
                if (movNominaVariables.Any())
                {
                    if (filtroMovimientosNominas == null)
                    {
                        filtroMovimientosNominas = movNominaVariables;
                    }
                    else
                    {
                        filtroMovimientosNominas.AddRange(movNominaVariables);
                    }
                    var subquery2 = from sub in query
                                    select new { sub.mba.resultado, sub.p.diasIMSS };
                    object[] datosBimestrales = new object[2];
                    if (subquery2.Count() > 0)
                    {
                        var datosBimestrales2 = (from sub in subquery2
                                                 group sub by 1 into g
                                                 select new
                                                 {
                                                     res = g.Sum(x => x.resultado),
                                                     dias = g.Sum(x => x.diasIMSS)
                                                 }).ToArray();
                        //datosBimestrales = datosBimestrales2.Select(p=> p).ToList<object>();
                        int conta = 0;
                        datosBimestrales[0] = datosBimestrales2[0].res;
                        datosBimestrales[1] = datosBimestrales2[0].dias;
                        //datosBimestrales =(object[]) datosBimestrales2[i];


                    }
                    if (mensajeResultado.noError == -100)
                    {
                        mensajeResultado.noError = 64;
                        return 0.0;
                    }
                    int diasBim = 0;
                    if (datosBimestrales.Length > 0)
                    {

                        diasBim = datosBimestrales[1].GetType().Equals(typeof(int)) ? Convert.ToInt32(datosBimestrales[1]) : (int)datosBimestrales[1];
                    }
                    diasBim = Utilerias.diasBimestre(fechaIni);
                    if (valoresConceptosEmpleados.ContainsKey("DiasIncapacidaEmpleado".ToUpper()) && valoresConceptosEmpleados.ContainsKey("Ausentismo".ToUpper()))
                    {
                        diasIMSS = diasBim - Convert.ToInt32(valoresConceptosEmpleados["DiasIncapacidaEmpleado".ToUpper()] == null ? "0" : valoresConceptosEmpleados["DiasIncapacidaEmpleado".ToUpper()].ToString()) - Convert.ToInt32(valoresConceptosEmpleados["Ausentismo".ToUpper()] == null ? "0" : valoresConceptosEmpleados["Ausentismo".ToUpper()].ToString()) - diasDif;
                    }
                    else
                    {
                        diasIMSS = diasBim;
                    }

                    if (diasIMSS == 0)
                    {

                        acumuladoVariableBim = 0.0;
                    }
                    else
                    {
                        acumuladoVariableBim = (Double)datosBimestrales[0] / diasIMSS;
                    }
                    //aqui voy
                }


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("salarioDiarioVariable()1_Error: ").Append(ex));
                mensajeResultado.noError = 65;
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.Message.ToString();
                mensajeResultado.resultado = null;
            }
            return acumuladoVariableBim;
        }
        private Double salarioDiarioFijo(String claveTipoCorrida, String claveTipoNomina, decimal? idPeriodoNomina, PlazasPorEmpleado plazaPorEmpleado,
           String claveCtrCosto, String claveRazonSocial)
        {
            Double salarioFijoTotal = 0.0;
            try
            {
                tipoCorrida = (from tc in dbContextSimple.Set<TipoCorrida>()
                               where tc.clave == claveTipoCorrida
                               select tc).SingleOrDefault(); ;

                mensajeResultado = metodosParaMovimientosNomina.buscaMovimientosPlazasPorTipoYBaseAfecta(tipoCorrida, claveTipoNomina, idPeriodoNomina.GetValueOrDefault(), plazaPorEmpleado, claveCtrCosto, Tipo.NINGUNO, claveRazonSocial, ClavesParametrosModulos.claveBaseNominaIMSS.ToString(), (int)ClavesParametrosModulos.tipoBaseAfectaIMSSFijo, null, null, fechaActual, null, (DBContextSimple)dbContextSimple);
                if (mensajeResultado.noError == 0)
                {
                    filtroMovimientosNominas = (List<MovNomConcep>)mensajeResultado.resultado;
                }
                else
                {
                    return 0.0;
                }
                int tipoBaseAfectaImssFijo = Convert.ToInt32(ClavesParametrosModulos.tipoBaseAfectaIMSSFijo);
                var query = from c in dbContextSimple.Set<ConcepNomDefi>()
                            from ba in dbContextSimple.Set<BaseAfecConcepNom>()
                            join bn in dbContextSimple.Set<BaseNomina>() on ba.baseNomina.id equals bn.id
                            where ba.concepNomDefi.id == c.id && bn.clave == ClavesParametrosModulos.claveBaseNominaIMSS.ToString()
                            && ba.tipoAfecta == tipoBaseAfectaImssFijo
                            && c.fecha == (from cdn in dbContextSimple.Set<ConcepNomDefi>() where cdn.clave == c.clave select new { fecha = cdn.fecha }).Max(p => p.fecha)
                            && c.tipo == Tipo.AUTOMATICO
                            select new { c, ba, bn };
                if (filtroMovimientosNominas.Count() > 0)
                {
                    List<string> clavesConceptos = new List<string>();
                    filtroConceptosNomina = new List<ConcepNomDefi>();
                    int i;
                    for (i = 0; i < filtroMovimientosNominas.Count(); i++)
                    {
                        if (filtroMovimientosNominas[i].resultado == null)
                        {
                            //almacena conceptos que no han sido calculados para calcularlos posteriormente 
                            filtroConceptosNomina.Add(filtroMovimientosNominas[i].concepNomDefi);
                        }
                        else
                        {
                            salarioFijoTotal = salarioFijoTotal + filtroMovimientosNominas[i].resultado.GetValueOrDefault();
                        }
                        clavesConceptos.Add(filtroMovimientosNominas[i].concepNomDefi.clave);
                    }
                    //conceptos imss fijos automaticos de movimientos que no se han calculado 
                    salarioFijoTotal = salarioFijoTotal + ejecutaFormulaConceptosNomina(filtroConceptosNomina);
                    if (mensajeResultado.noError != 0)
                    {
                        return 0.0;
                    }
                    query = from sub in query
                            where !clavesConceptos.Contains(sub.c.clave)
                            select sub;
                }
                filtroConceptosNomina = query.Select(p => p.c).ToList();
                if (mensajeResultado.noError == -100)
                {
                    mensajeResultado.noError = 60;
                    return 0.0;
                }
                salarioFijoTotal = salarioFijoTotal + ejecutaFormulaConceptosNomina(filtroConceptosNomina);
                if (mensajeResultado.noError != 0)
                {
                    return 0.0;
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("salarioDiarioFijo()1_Error: ").Append(ex));
                mensajeResultado.noError = 57;
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.Message.ToString();
                mensajeResultado.resultado = null;

            }

            return salarioFijoTotal;
        }
        private Double ejecutaFormulaConceptosNomina(List<ConcepNomDefi> concepNomDefis)
        {
            Double fijo, salarioFijoTotal = 0.0, valorExento = 0.0;
            try
            {
                bool condicion;
                int i;
                for (i = 0; i < concepNomDefis.Count(); i++)
                {
                    agregaParametrosConceptosNominaFormula(concepNomDefis[i].paraConcepDeNom);
                    if (concepNomDefis[i].condicionConcepto.Length == 0)
                    {
                        condicion = true;

                    }
                    else
                    {
                        condicion = ejecutaFormula(concepNomDefis[i].condicionConcepto) == 0.0 ? false : true;
                    }
                    if (condicion)
                    {
                        fijo = ejecutaFormula(concepNomDefis[i].formulaConcepto);
                        foreach (BaseAfecConcepNom afecConcepNom in concepNomDefis[i].baseAfecConcepNom)
                        {
                            if (afecConcepNom.baseNomina.id == 2 && afecConcepNom.tipoAfecta == 0)
                            {
                                valorExento = afecConcepNom.formulaExenta.Any() ? 0.0 : ejecutaFormula(afecConcepNom.formulaExenta);

                            }

                        }
                        concepNomDefis[i].resultado = fijo - valorExento;
                        salarioFijoTotal = salarioFijoTotal + (fijo - valorExento);
                    }
                    else
                    {
                        concepNomDefis[i].resultado = 0.0;
                    }

                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ejecutaFormulaConceptosNomina()1_Error: ").Append(ex));
                mensajeResultado.noError = 59;
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.Message.ToString();
                mensajeResultado.resultado = null;
            }
            return salarioFijoTotal;
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
        //Usando PlazasPorEmpleado
        private List<PlazasPorEmpleadosMov> obtenerPlazasPorEmpleados(String claveEmpIni, String claveEmpFin, String claveTipoNomina, String clavePuesto,
                String claveCategoriasPuestos, String claveTurno, String claveRazonSocial, String claveRegPatronal, String claveDepto,
                String claveCtrCosto, int tipoSalario, String tipoContrato, bool? status, String claveTipoCorrida, String claveFormaPago, DateTime fechaInicioPeriodo, DateTime fechaFinPeriodo)
        {
            List<PlazasPorEmpleadosMov> filtroPlazasPorEmpleadosMov = null, filtroPlazasPorEmpleadosMovTmp;
            try
            {
                var query = from pMov in dbContextSimple.Set<PlazasPorEmpleadosMov>()
                            join pemp in dbContextSimple.Set<PlazasPorEmpleado>() on pMov.plazasPorEmpleado.id equals pemp.id
                            select new { pMov, pemp };
                var subquery = from pMovX in dbContextSimple.Set<PlazasPorEmpleadosMov>()
                               join pe in dbContextSimple.Set<PlazasPorEmpleado>() on pMovX.plazasPorEmpleado.id equals pe.id
                               join em in dbContextSimple.Set<Empleados>() on pe.empleados.id equals em.id
                               join pu in dbContextSimple.Set<Puestos>() on pMovX.puestos.id equals pu.id
                               select new { pMovX, pe, em, pu };
                claveRazonSocial = (claveRazonSocial == null ? "" : claveRazonSocial);
                if (claveRazonSocial.Any())
                {
                    subquery = from sub in subquery
                               join rs in dbContextSimple.Set<RazonesSociales>() on sub.pe.razonesSociales.id equals rs.id
                               where rs.clave == claveRazonSocial
                               select sub;
                }
                claveTurno = (claveTurno == null ? "" : claveTurno);
                if (claveTurno.Any())
                {
                    subquery = from sub in subquery
                               join tu in dbContextSimple.Set<Turnos>() on sub.pMovX.turnos.id equals tu.id
                               where tu.clave == claveTurno
                               select sub;
                }
                claveTipoNomina = (claveTipoNomina == null ? "" : claveTipoNomina);
                if (claveTipoNomina.Any())
                {
                    subquery = from sub in subquery
                               join t in dbContextSimple.Set<TipoNomina>() on sub.pMovX.tipoNomina.id equals t.id
                               where t.clave == claveTipoNomina
                               select sub;
                }
                claveRegPatronal = (claveRegPatronal == null ? "" : claveRegPatronal);
                if (claveRegPatronal.Any())
                {
                    subquery = from sub in subquery
                               join rp in dbContextSimple.Set<RegistroPatronal>() on sub.pe.registroPatronal.id equals rp.id
                               where rp.clave == claveRegPatronal
                               select sub;
                }
                claveDepto = (claveDepto == null ? "" : claveDepto);
                if (claveDepto.Any())
                {
                    subquery = from sub in subquery
                               join dp in dbContextSimple.Set<Departamentos>() on sub.pMovX.departamentos.id equals dp.id
                               where dp.clave == claveDepto
                               select sub;
                }
                claveCtrCosto = (claveCtrCosto == null ? "" : claveCtrCosto);
                if (claveCtrCosto.Any())
                {
                    subquery = from sub in subquery
                               join cc in dbContextSimple.Set<CentroDeCosto>() on sub.pMovX.centroDeCosto.id equals cc.id
                               where cc.clave == claveCtrCosto
                               select sub;
                }
                clavePuesto = (clavePuesto == null ? "" : clavePuesto);
                if (clavePuesto.Any())
                {
                    subquery = from sub in subquery
                               where sub.pu.clave == clavePuesto
                               select sub;
                }
                claveCategoriasPuestos = (claveCategoriasPuestos == null ? "" : claveCategoriasPuestos);
                if (claveCategoriasPuestos.Any())
                {

                    subquery = from sub in subquery
                               join cp in dbContextSimple.Set<CategoriasPuestos>() on sub.pu.categoriasPuestos.id equals cp.id
                               where cp.clave == claveCategoriasPuestos
                               select sub;
                }
                claveFormaPago = (claveFormaPago == null ? "" : claveFormaPago);
                //if (!claveCategoriasPuestos.Any())
                //{

                //    subquery = from sub in subquery
                //               join fp in getSession().Set<FormasDePago>() on sub.pMovX.formasDePago.id equals fp.id
                //               where fp.clave == claveFormaPago
                //               select sub;
                //}

                if (tipoContrato != null)
                {

                    subquery = from sub in subquery
                               join tc in dbContextSimple.Set<TipoContrato>() on sub.pMovX.tipoContrato.id equals tc.id
                               where tc.clave == tipoContrato
                               select sub;
                }

                if (status != null)
                {

                    subquery = from sub in subquery
                               where sub.em.status == status
                               select sub;
                }

                if (tipoSalario > 0)
                {
                    if (string.Equals(claveTipoCorrida, "FIN", StringComparison.OrdinalIgnoreCase))
                    {
                        subquery = from sub in subquery
                                   from si in dbContextSimple.Set<SalariosIntegrados>()
                                   where si.fecha == ((from s in dbContextSimple.Set<SalariosIntegrados>()
                                                       where s.fecha <= fechaBajaFiniq && s.empleados.id == si.empleados.id && s.empleados.id == sub.pe.empleados.id
                                                       select new { f = s.fecha }).Max(p => p.f)) && si.tipoDeSalario == tipoSalario
                                   select sub;
                    }
                    else
                    {
                        subquery = from sub in subquery
                                   from si in dbContextSimple.Set<SalariosIntegrados>()
                                   where si.fecha == ((from s in dbContextSimple.Set<SalariosIntegrados>()
                                                       where s.fecha <= fechaFinPeriodo && s.empleados.id == si.empleados.id && s.empleados.id == sub.pe.empleados.id
                                                       select new { f = s.fecha }).Max(p => p.f)) && si.tipoDeSalario == tipoSalario
                                   select sub;
                    }

                }
                if (string.Equals(claveTipoCorrida, "FIN", StringComparison.OrdinalIgnoreCase))
                {
                    subquery = from sub in subquery
                               where ((sub.pMovX.fechaInicial <= fechaBajaFiniq) || (sub.pMovX.fechaInicial >= fechaBajaFiniq && sub.pMovX.fechaInicial <= fechaFinPeriodo))
                               && ((sub.pMovX.plazasPorEmpleado.fechaFinal >= fechaFinPeriodo) || (sub.pMovX.plazasPorEmpleado.fechaFinal >= fechaBajaFiniq && sub.pMovX.plazasPorEmpleado.fechaFinal <= fechaFinPeriodo))
                               select sub;
                }
                else
                {
                    subquery = from sub in subquery
                               where ((sub.pMovX.fechaInicial <= fechaInicioPeriodo) || (sub.pMovX.fechaInicial >= fechaBajaFiniq && sub.pMovX.fechaInicial <= fechaFinPeriodo))
                               && ((sub.pMovX.plazasPorEmpleado.fechaFinal >= fechaFinPeriodo) || (sub.pMovX.plazasPorEmpleado.fechaFinal >= fechaInicioPeriodo && sub.pMovX.plazasPorEmpleado.fechaFinal <= fechaFinPeriodo))
                               select sub;
                }

                if (claveEmpIni.Length > 0 && claveEmpFin.Length > 0)
                {
                    subquery = from sub in subquery
                               where (sub.em.clave.CompareTo(claveEmpIni) >= 0 && sub.em.clave.CompareTo(claveEmpFin) <= 0)
                               select sub;
                }
                else if (claveEmpIni.Length > 0)
                {
                    subquery = from sub in subquery
                               where (sub.em.clave == claveEmpIni)
                               select sub;
                }
                else if (claveEmpFin.Length > 0)
                {
                    subquery = from sub in subquery
                               where (sub.em.clave == claveEmpFin)
                               select sub;
                }
                var subquery3 = (from sub in subquery
                                 group new { sub.pe, sub.pMovX } by new
                                 {
                                     sub.pe.referencia
                                 } into grupo
                                 select new
                                 {
                                     idPPMMax = grupo.Max(p => p.pMovX.id)
                                 });

                //var subquery3 = from sub in subquery
                //                group sub by sub.pe.referencia into c
                //                select new { referencia = c.Key, id = c.Select(m => m.pMovX.id) };

                decimal[] prueba = subquery3.Select(a => a.idPPMMax).ToArray();
                List<decimal?> idsPPM = new List<decimal?>();
                for (int i = 0; i < prueba.Length; i++)
                {
                    idsPPM.Add(prueba[i]);
                }
                query = from q in query
                        where idsPPM.Contains(q.pMov.id)
                        select q;
                if (claveRazonSocial.Any())
                {
                    query = from q in query
                            where !(from px in dbContextSimple.Set<PlazasPorEmpleado>()
                                    where px.razonesSociales.clave == claveRazonSocial && px.plazaReIngreso != null
                                    select px.plazaReIngreso.id).Contains(q.pemp.id)
                            select q;
                }
                else
                {
                    query = from q in query
                            where !(from px in dbContextSimple.Set<PlazasPorEmpleado>()
                                    where px.plazaReIngreso != null
                                    select px.plazaReIngreso.id).Contains(q.pemp.id)
                            select q;
                }

                if (!isCalculoSDI)
                {
                    var subquery2 = from o in dbContextSimple.Set<CFDIEmpleado>()
                                    select new { o };
                    claveTipoNomina = (claveTipoNomina == null ? "" : claveTipoNomina);

                    if (claveTipoNomina.Any())
                    {
                        subquery2 = from sub in subquery2
                                    where sub.o.razonesSociales.clave == claveRazonSocial && sub.o.tipoNomina.clave == claveTipoNomina
                                    select sub;
                    }
                    else
                    {
                        subquery2 = from sub in subquery2
                                    where sub.o.razonesSociales.clave == claveRazonSocial
                                    select sub;

                    }

                    if (claveTipoCorrida.Any())
                    {
                        subquery2 = from sub in subquery2
                                    where sub.o.tipoCorrida.clave == claveTipoCorrida
                                    select sub;
                    }

                    if (periodosNomina != null)
                    {
                        subquery2 = from sub in subquery2
                                    where sub.o.periodosNomina.id == periodosNomina.id
                                    select sub;
                    }
                    if (claveEmpIni.Length > 0 && claveEmpFin.Length > 0)
                    {
                        subquery2 = from sub in subquery2
                                    where (sub.o.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave.CompareTo(claveEmpIni) >= 0 &&
                                    sub.o.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave.CompareTo(claveEmpFin) <= 0)
                                    select sub;
                    }
                    else if (claveEmpIni.Length > 0)
                    {
                        subquery2 = from sub in subquery2
                                    where sub.o.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave == claveEmpIni
                                    select sub;
                    }
                    else if (claveEmpFin.Length > 0)
                    {
                        subquery2 = from sub in subquery2
                                    where sub.o.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave == claveEmpFin
                                    select sub;
                    }
                    subquery2 = from sub in subquery2
                                where sub.o.cfdiRecibo.statusTimbrado == StatusTimbrado.TIMBRADO
                                select sub;

                    query = from subq in query
                            where !(subquery2.Select(p => p.o.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave)).Contains(subq.pemp.empleados.clave)
                            select subq;
                }
                if (string.Equals(claveTipoCorrida, "FIN", StringComparison.OrdinalIgnoreCase))
                {
                    query = from sub in query
                            where (from flp in dbContextSimple.Set<FiniqLiquidPlazas>() where sub.pemp.referencia == flp.plazasPorEmpleado.referencia && flp.incluir == true select flp.plazasPorEmpleado.referencia).Contains(sub.pemp.referencia)
                            select sub;
                }
                query = from sub in query
                        where sub.pemp.plazaPrincipal == true
                        orderby sub.pemp.empleados.clave, sub.pemp.referencia
                        select sub;

                filtroPlazasPorEmpleadosMov = query.Select(p => p.pMov).ToList<PlazasPorEmpleadosMov>();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("cargaTablaFactorIntegracion()1_Error: ").Append(ex));
                mensajeResultado.noError = 27;
                mensajeResultado.error = ex.Message.ToString();
            }
            return filtroPlazasPorEmpleadosMov;
        }
        private void cargaTablaFactorIntegracion(String controlador, List<Object> clavesElementosAplicacion, bool factor, bool zonaSalarial, bool uma, bool parametro, DateTime fechaFinal, int ejercicio, DbContext dbContextMaster)
        {
            if (factor || zonaSalarial || parametro || uma)
            {
                DbContext dbContext = dbContextMaster;
                try
                {
                    // setSession(dbContextMaster);
                    // getSession().Database.BeginTransaction();

                    dbContext.Database.BeginTransaction();
                    List<TablaBase> tablasBaseSistema = buscaTablasBaseSistema(dbContextMaster);
                    if (mensajeResultado.noError != 0)
                    {
                        return;
                    }
                    List<TipoControlador> tipoControladores;
                    if (factor)
                    {
                        tipoControladores = obtieneTipoContoladorTablaBase(ClavesParametrosModulos.claveTipoTablaFactorIntegracion.ToString(), tablasBaseSistema);
                        if (mensajeResultado.noError != 0)
                        {
                            mensajeResultado.error = "no encontro controladores en la tabla tabla factor de integracion";
                            return;
                        }
                        tablaFactorIntegracion = construyeTablaXml(ClavesParametrosModulos.claveTipoTablaFactorIntegracion.ToString(), controlador, tipoControladores, fechaFinal, ejercicio, dbContextMaster);
                        if (mensajeResultado.noError == -10)
                        {
                            mensajeResultado.noError = 24;
                        }
                        if (tablaFactorIntegracion == null & mensajeResultado.noError == 0)
                        {
                            mensajeResultado.error = "no encontro datos de la tabla factor de integracion";
                            mensajeResultado.noError = 1000;
                            mensajeResultado.resultado = 0;
                        }
                        if (mensajeResultado.noError != 0)
                        {
                            return;
                        }
                    }
                    if (zonaSalarial)
                    {
                        tipoControladores = obtieneTipoContoladorTablaBase(ClavesParametrosModulos.claveTipoTablaZonaZalarial.ToString(), tablasBaseSistema);
                        if (mensajeResultado.noError != 0)
                        {
                            mensajeResultado.error = "no encontro controladores en la tabla zona salarial";
                            return;
                        }
                        tablaZonaSalarial = construyeTablaXml(ClavesParametrosModulos.claveTipoTablaZonaZalarial.ToString(), controlador, tipoControladores, fechaFinal, ejercicio, dbContextMaster);
                        if (mensajeResultado.noError == -10)
                        {
                            mensajeResultado.noError = 24;
                        }
                        if (tablaFactorIntegracion == null & mensajeResultado.noError == 0)
                        {
                            mensajeResultado.error = "no encontro datos de la tabla zona salarial";
                            mensajeResultado.noError = 1000;
                            mensajeResultado.resultado = 0;
                        }
                        if (mensajeResultado.noError != 0)
                        {
                            return;
                        }
                    }
                    if (uma)
                    {
                        tipoControladores = obtieneTipoContoladorTablaBase(ClavesParametrosModulos.claveTipoTablaUMA.ToString(), tablasBaseSistema);
                        if (mensajeResultado.noError != 0)
                        {
                            mensajeResultado.error = "no encontro controladores en la tabla UMA";
                            return;
                        }
                        Object[,] tablaUMA = construyeTablaXml(ClavesParametrosModulos.claveTipoTablaUMA.ToString(), "", tipoControladores, fechaFinal, ejercicio, dbContextMaster);
                        if (tablaUMA == null & mensajeResultado.noError == 0)
                        {
                            mensajeResultado.error = "no encontro datos de la tabla UMA";
                            mensajeResultado.noError = 1000;
                            mensajeResultado.resultado = 0;
                        }
                        else if (tablaUMA != null & mensajeResultado.noError == 0)
                        {
                            int i;
                            if (tablaUMA.Length == 0)
                            {
                                mensajeResultado.error = "no encontro datos de la tabla UMA";
                                mensajeResultado.noError = 1000;
                                mensajeResultado.resultado = 0;
                            }
                            else
                            {
                                for (i = 0; i < tablaUMA.Length; i++)
                                {
                                    if (String.IsNullOrEmpty(tablaUMA[i, 0].ToString().Trim()))
                                    {
                                        valorUMA = null;
                                    }
                                    else
                                    {
                                        valorUMA = Convert.ToDouble(tablaUMA[i, 1].ToString());
                                        // valorUMA = Double.Parse(tablaUMA[i, 0].ToString());
                                    }
                                    break;
                                }
                                if (valorUMA == null)
                                {
                                    mensajeResultado.error = "no encontro datos de la tabla UMA";
                                    mensajeResultado.noError = 1000;
                                    mensajeResultado.resultado = 0;
                                }
                            }
                        }
                        if (mensajeResultado.noError != 0)
                        {
                            return;
                        }
                    }
                    if (parametro & clavesElementosAplicacion != null)
                    {
                        obtenerDatoParametros(clavesElementosAplicacion);
                    }
                    //getSession().Database.CurrentTransaction.Commit();
                    // dbContext.Database.CurrentTransaction.Commit();
                    dbContext.Database.Connection.Close();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("cargaTablaFactorIntegracion()1_Error: ").Append(ex));
                    mensajeResultado.noError = 24;
                    mensajeResultado.error = ex.Message.ToString();
                    mensajeResultado.resultado = null;
                    // getSession().Database.CurrentTransaction.Rollback();
                    dbContext.Database.CurrentTransaction.Rollback();
                    return;
                }

            }
        }
        private void obtenerDatoParametros(List<Object> clavesElementosAplicacion)
        {
            String valor;
            //factor mensual
            clavesElementosAplicacion = clavesElementosAplicacion == null ? new List<Object>() : clavesElementosAplicacion;
            if (clavesElementosAplicacion.Count() > 0)
            {
                valor = (from cr in getSession().Set<Cruce>()
                         join pr in getSession().Set<Parametros>() on cr.parametros.id equals pr.id
                         join ea in getSession().Set<ElementosAplicacion>() on cr.elementosAplicacion.id equals ea.id
                         where pr.clave == Convert.ToDecimal(ClavesParametrosModulos.claveParametroFactorAplicaciónTablaMensual)
                         && ea.clave == ClavesParametrosModulos.claveElementoAplicacionRazonSocial.ToString()
                         && cr.claveElemento == clavesElementosAplicacion[0].ToString()
                         select cr.valor).Single().ToString();

                if (mensajeResultado.noError != 0)
                {
                    mensajeResultado.noError = 5;
                    return;
                }
                valor = (valor == null ? "" : valor);
                if (valor.Any())
                {
                    valor = (from pr in getSession().Set<Parametros>()
                             join m in getSession().Set<Modulo>() on pr.modulo.id equals m.id
                             where pr.clave == Convert.ToDecimal(ClavesParametrosModulos.claveParametroFactorAplicaciónTablaMensual)
                             && m.clave == ClavesParametrosModulos.claveModuloGlobal.ToString()
                             select pr.valor).Single().ToString();
                    if (mensajeResultado.noError != 0)
                    {
                        mensajeResultado.noError = 6;
                        return;
                    }
                }

                if (string.Equals(valor, "2", StringComparison.OrdinalIgnoreCase))
                {//Factor mensual = 30.4 2 = Dias naturales
                    DateTime fecha = DateTime.Now;
                    fecha = periodosNomina.fechaInicial.GetValueOrDefault();
                    factorMensual = (double)DateTime.DaysInMonth(fecha.Year, fecha.Month);
                }
                else
                {
                    factorMensual = 30.4D;
                }
                ////factorMensual = Double.valueOf(valor);BDIE01

                //factor anual
                valor = valor = (from cr in getSession().Set<Cruce>()
                                 join pr in getSession().Set<Parametros>() on cr.parametros.id equals pr.id
                                 join ea in getSession().Set<ElementosAplicacion>() on cr.elementosAplicacion.id equals ea.id
                                 where pr.clave == Convert.ToDecimal(ClavesParametrosModulos.claveParametroFactorAplicaciónTablaAnual)
                                 && ea.clave == ClavesParametrosModulos.claveElementoAplicacionRazonSocial.ToString()
                                 && cr.claveElemento == clavesElementosAplicacion[0].ToString()
                                 select cr.valor).Single().ToString();
                if (mensajeResultado.noError != 0)
                {
                    mensajeResultado.noError = 1;
                    return;
                }
                valor = (valor == null ? "" : valor);
                if (valor.Any())
                {
                    valor = (from pr in getSession().Set<Parametros>()
                             join m in getSession().Set<Modulo>() on pr.modulo.id equals m.id
                             where pr.clave == Convert.ToDecimal(ClavesParametrosModulos.claveParametroFactorAplicaciónTablaAnual)
                             && m.clave == ClavesParametrosModulos.claveModuloGlobal.ToString()
                             select pr.valor).Single().ToString();
                    if (mensajeResultado.noError != 0)
                    {
                        mensajeResultado.noError = 2;
                        return;
                    }
                }
                factorAnual = Convert.ToDouble(valor);
            }
            #region Manejo de Salario Diario
            if (manejoSalarioDiario == null)
            {
                valor = (from cr in getSession().Set<Cruce>()
                         join pr in getSession().Set<Parametros>() on cr.parametros.id equals pr.id
                         join ea in getSession().Set<ElementosAplicacion>() on cr.elementosAplicacion.id equals ea.id
                         where pr.clave == Convert.ToDecimal(ClavesParametrosModulos.claveParametroManejarSalarioDiarioPor)
                         && ea.clave == ClavesParametrosModulos.claveElementoAplicacionRazonSocial.ToString()
                         && cr.claveElemento == clavesElementosAplicacion[0].ToString()
                         select cr.valor).Single().ToString();
                if (mensajeResultado.noError != 0)
                {
                    mensajeResultado.noError = 1;
                    return;
                }
                valor = (valor == null ? "" : valor);
                if (valor.Any())
                {
                    valor = (from pr in getSession().Set<Parametros>()
                             join m in getSession().Set<Modulo>() on pr.modulo.id equals m.id
                             where pr.clave == Convert.ToDecimal(ClavesParametrosModulos.claveParametroManejarSalarioDiarioPor)
                             && m.clave == ClavesParametrosModulos.claveModuloGlobal.ToString()
                             select pr.valor).Single().ToString();
                    if (mensajeResultado.noError != 0)
                    {
                        mensajeResultado.noError = 2;
                        return;
                    }
                }
                if (valor.Equals(ClavesParametrosModulos.opcionParametroSalarioDiario))
                {
                    manejoSalarioDiario = ManejoSalarioDiario.DIARIO;
                }
                else if (valor.Equals(ClavesParametrosModulos.opcionParametroSalarioSemanal))
                {
                    manejoSalarioDiario = ManejoSalarioDiario.SEMANAL;
                }
                else if (valor.Equals(ClavesParametrosModulos.opcionParametroSalarioQuincenal))
                {
                    manejoSalarioDiario = ManejoSalarioDiario.QUINCENAL;
                }
                else
                {
                    manejoSalarioDiario = ManejoSalarioDiario.MENSUAL;
                }
            }
            #endregion

        }
        private Object[,] construyeTablaXml(String claveTablaBase, String controlador, List<TipoControlador> tipoControladores, DateTime fechaFinal, int ejercicio, DbContext dbContextMaster)
        {

            object[,] valores = null;
            controlador = (controlador == null) ? "" : controlador;
            DbContext Context = dbContextMaster;
            try
            {
                var subQueryMaxID = (from t in Context.Set<TablaDatos>() where t.tablaBase.clave == claveTablaBase select new { t });

                for (int i = 0; i < tipoControladores.Count; i++)
                {
                    if (tipoControladores[i] == TipoControlador.CONTROLADORENTIDAD)
                    {
                        subQueryMaxID = (from t_query in subQueryMaxID where t_query.t.controladores == controlador select t_query);
                    }
                    else if (tipoControladores[i] == TipoControlador.CONTROLPORFECHA)
                    {
                        subQueryMaxID = (from t_query in subQueryMaxID where t_query.t.controlPorFecha <= fechaFinal select t_query);

                    }
                    else if (tipoControladores[i] == TipoControlador.CONTROLPORAÑO)
                    {
                        subQueryMaxID = (from t_query in subQueryMaxID where t_query.t.controlPorAnio <= ejercicio select t_query);
                    }
                }

                subQueryMaxID = (from t_query in subQueryMaxID select t_query);
                int count = subQueryMaxID.Count();
                byte[] result = null;
                if (count > 0)
                {
                    int maxId = (from t_query in subQueryMaxID select new { t_query.t.id }).Max(p => p.id);

                    var queryPrincipal = (from t in Context.Set<TablaDatos>() where t.id == maxId select t.fileXml);
                    if (queryPrincipal.Single() != null)
                    {
                        result = queryPrincipal.Single().ToArray();
                    }
                }
                if (result == null)
                {
                    if (controlador.Any())
                    {
                        String[] controladores = controlador.Split('#');
                        controlador = controladores[1];
                    }
                    valores = construyeTablaXml(claveTablaBase, controlador, tipoControladores, fechaActual, fechaActual.Year, Context);
                }
                else
                {
                    valores = UtilidadesXML.extraeValoresNodos(UtilidadesXML.convierteBytesToXML(result));
                    if (UtilidadesXML.ERROR_XML > 0)
                    {
                        mensajeResultado = UtilidadesXML.mensajeError;
                        return null;
                    }
                }
                Context.Database.Connection.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("construyeTablaXml()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return valores;





            /*   Object[,] valores = null;
            byte[] result = null;
               DbContext dbContext = dbContextMaster;
            try
            {
                controlador = (controlador == null) ? "" : controlador;
                bool controlFecha = false, controlAño = false, controlEntidad = false;
                   var subQueryMaxID = from o in dbContext.Set<TablaDatos>()
                                    where o.tablaBase.clave == claveTablaBase

                                    select new { o };

                   var subquery = (from t in dbContext.Set<TablaDatos>()
                                from o in subQueryMaxID
                                where t.tablaBase.id == o.o.tablaBase.id
                                select new { t });
                foreach (TipoControlador control in tipoControladores)
                {
                    if (control == TipoControlador.CONTROLPORFECHA)
                    {
                        controlFecha = true;
                        subquery = from t in subquery where t.t.controlPorFecha <= fechaFinal select t;
                    }
                    else if (control == TipoControlador.CONTROLPORAÑO)
                    {
                        controlAño = true;
                        subquery = from t in subquery where t.t.controlPorAnio <= ejercicio select t;
                    }
                    else if (control == TipoControlador.CONTROLADORENTIDAD)
                    {
                        controlEntidad = true;

                    }

                }

                   if (!String.IsNullOrEmpty(controlador) && controlEntidad)
                {
                    subquery = from t in subquery
                               where t.t.controladores == controlador
                               select t;
                }
                   //if (!controlador.Any() && controlEntidad)
                   //{

                   //}
                if (!controlFecha & !controlAño)
                {
                    subQueryMaxID = from sub in subQueryMaxID
                                    where sub.o.id == subquery.Max(p => p.t.id)
                                    select sub;
                }
                else if (controlFecha)
                {
                    subQueryMaxID = from sub in subQueryMaxID
                                    where sub.o.controlPorFecha == subquery.Max(p => p.t.controlPorFecha)
                                    select sub;
                }
                else if (controlAño)
                {
                    subQueryMaxID = from sub in subQueryMaxID
                                    where sub.o.controlPorAnio == subquery.Max(p => p.t.controlPorAnio)
                                    select sub;
                }
                else
                {
                    subQueryMaxID = from sub in subQueryMaxID
                                    where sub.o.id == subquery.Max(p => p.t.id)
                                    select sub;
                }

                var queryPrincipal = (from sub in subQueryMaxID
                                      select sub.o.fileXml);
                if (queryPrincipal.Single() != null)
                {
                    result = queryPrincipal.Single().ToArray();
                }
                if (result == null)
                {
                       var subQueryMaxID2 = from o in dbContext.Set<TablaDatos>()
                                         where o.tablaBase.clave == claveTablaBase
                                         select new { o };

                       var subquery2 = (from t in dbContext.Set<TablaDatos>()
                                     from o in subQueryMaxID2
                                     where t.tablaBase.id == o.o.tablaBase.id
                                     select new { t });
                    foreach (TipoControlador control in tipoControladores)
                    {
                        if (control == TipoControlador.CONTROLPORFECHA)
                        {
                            controlFecha = true;
                            subquery2 = from t in subquery2 where t.t.controlPorFecha <= fechaActual select t;
                            break;
                        }
                        else if (control == TipoControlador.CONTROLPORAÑO)
                        {
                            controlAño = true;
                            subquery2 = from t in subquery2 where t.t.controlPorAnio <= fechaActual.Year select t;
                            break;
                        }
                        else if (control == TipoControlador.CONTROLADORENTIDAD)
                        {
                            if (!controlador.Any())
                            {
                                String[] controladores = controlador.Split('#');
                                controlador = controladores[0];
                            }
                            subquery2 = from t in subquery2
                                        where t.t.controladores == controlador
                                        select t;
                            break;

                        }

                    }
                    if (!controlFecha & !controlAño)
                    {
                        subQueryMaxID2 = from sub in subQueryMaxID2
                                         where sub.o.id == subquery2.Max(p => p.t.id)
                                         select sub;
                    }
                    else if (controlFecha)
                    {
                        subQueryMaxID2 = from sub in subQueryMaxID2
                                         where sub.o.controlPorFecha == subquery2.Max(p => p.t.controlPorFecha)
                                         select sub;
                    }
                    else if (controlAño)
                    {
                        subQueryMaxID2 = from sub in subQueryMaxID2
                                         where sub.o.controlPorAnio == subquery2.Max(p => p.t.controlPorAnio)
                                         select sub;
                    }
                    else
                    {
                        subQueryMaxID2 = from sub in subQueryMaxID2
                                         where sub.o.id == subquery2.Max(p => p.t.id)
                                         select sub;
                    }

                    result = (from sub in subQueryMaxID2
                              select sub.o.fileXml).Single();

                }
                if (result != null)
                {
                    valores = UtilidadesXML.extraeValoresNodos(UtilidadesXML.convierteBytesToXML(result));
                    if (UtilidadesXML.ERROR_XML > 0)
                    {
                        errorEstructuraXML(UtilidadesXML.ERROR_XML);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("construyeTablaXml()1_Error: ").Append(ex));
                mensajeResultado.noError = -10;
                mensajeResultado.error = ex.Message.ToString();
            }
            return valores;
               */

        }
        private void errorEstructuraXML(int error)
        {
            switch (error)
            {
                case 1:
                    mensajeResultado.noError = 15;
                    break;
                case 2:
                    mensajeResultado.noError = 16;
                    break;
                case 3:
                    mensajeResultado.noError = 17;
                    break;
                case 4:
                    mensajeResultado.noError = 18;
                    break;
                case 5:
                    mensajeResultado.noError = 19;
                    break;
                case 6:
                    mensajeResultado.noError = 20;
                    break;
            }
        }
        private List<TipoControlador> obtieneTipoContoladorTablaBase(String claveTablaBase, List<TablaBase> tablasBaseSistema)
        {
            String valorTipoControladores = buscaTipoControladorTablaBase(claveTablaBase, tablasBaseSistema);
            List<TipoControlador> tipoControladores = null;
            if (valorTipoControladores.Length > 0)
            {
                tipoControladores = getTipoControlador(valorTipoControladores.Split(','));
            }
            else
            {
                mensajeResultado.noError = (1000);
                mensajeResultado.resultado = (0);
            }
            return tipoControladores;
        }
        private String buscaTipoControladorTablaBase(String claveTablaBase, List<TablaBase> tablasBaseSistema)
        {
            foreach (TablaBase tablaBase in tablasBaseSistema)
            {
                if (string.Equals(claveTablaBase, tablaBase.clave, StringComparison.OrdinalIgnoreCase))
                {
                    return tablaBase.controladores;
                }
            }
            return "";
        }
        private List<TipoControlador> getTipoControlador(String[] valorTipoControlador)
        {
            List<TipoControlador> tipoControladores = new List<TipoControlador>();
            foreach (String tipo in valorTipoControlador)
            {

                tipoControladores.Add((TipoControlador)ManejadorEnum.GetValue(tipo, typeof(TipoControlador)));
            }
            return tipoControladores;
        }
        private List<TablaBase> buscaTablasBaseSistema(DbContext dbContextMaster)
        {
            List<TablaBase> tablaBases = null;
            DbContext dbContext = dbContextMaster;
            try
            {
                tablaBases = (from t in dbContext.Set<TablaBase>()
                              join tt in dbContext.Set<TipoTabla>() on t.tipoTabla.id equals tt.id
                              where tt.sistema == true
                              select t).ToList();
                tablaBases = tablaBases == null ? new List<TablaBase>() : tablaBases;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("cargaTablaFactorIntegracion()1_Error: ").Append(ex));
                mensajeResultado.noError = -10;
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.Message.ToString();
                mensajeResultado.resultado = null;
            }
            if (tablaBases.Count == 0)
            {
                mensajeResultado.error = "no encontro tablas base sistema";
                mensajeResultado.noError = -10;
            }
            return tablaBases;
        }
        //private void obtenerFactores(String claveRazonSocial)
        //{
        //    try
        //    {
        //        tipoTablaISR = TipoTablaISR.NORMAL;
        //        object[] valores = new object[] {
        //       (decimal)  ClavesParametrosModulos.claveParametroFactorAplicaciónTablaAnual,
        //        (decimal) ClavesParametrosModulos.claveParametroPagosPorHora,
        //       (decimal) ClavesParametrosModulos.claveParametroManejarHorasPor,
        //       (decimal)  ClavesParametrosModulos.claveParametroManejarSalarioDiarioPor,
        //       (decimal)  ClavesParametrosModulos.claveParametroFactorAplicaciónTablaMensual,
        //       (decimal)  ClavesParametrosModulos.claveParametroModoAjustarIngresosAlMes,
        //       (decimal)  ClavesParametrosModulos.claveParametroDesgloseInternoISR,
        //       (decimal)  ClavesParametrosModulos.clavePagarNominaDiasNaturales,
        //        (decimal) ClavesParametrosModulos.claveParametroVersionCalculoPrestamoAhorro,
        //        (decimal) ClavesParametrosModulos.claveParametroPagarIMSSDiasNaturales,
        //        (decimal) ClavesParametrosModulos.claveParametroDescontarFaltasModoAjusteMes,
        //       (decimal)  ClavesParametrosModulos.claveParametroPagarPrimayVacacionesAuto,
        //       (decimal)  ClavesParametrosModulos.claveParametroSalarioUtilizarVacaciones,
        //        (decimal) ClavesParametrosModulos.claveParametroUsaUMA
        //        };
        //        List<Object[]> listParametros;
        //        mensajeResultado = getParametrosYListCrucePorModuloYClaves((String)ClavesParametrosModulos.claveModuloGlobal, valores);
        //        if (mensajeResultado.noError == 0)
        //        {
        //            listParametros = (List<Object[]>)mensajeResultado.resultado;
        //        }
        //        else
        //        {
        //            return;
        //        }
        //        Object[] parametroManejarSalarioDiarioPor = null;
        //        DesgloseInternoISR desgloseInternoISR = DesgloseInternoISR.DESGLOSEISRNORMALANUAL;
        //        for (int i = 0; i < listParametros.Count(); i++)
        //        {
        //            if (((Parametros)(listParametros[i])[0]).clave.Equals((decimal)ClavesParametrosModulos.claveParametroFactorAplicaciónTablaAnual))
        //            {
        //                if (factorAnual == null ? true : factorAnual == 0) {
        //                    factorAnual = metodosBDMaestra.parametroFactorAplicacionTablaAnual((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<MetodosBDMaestra.ValoresElementosAplicacion>() { new MetodosBDMaestra.ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
        //                }
        //            } else if (((Parametros)(listParametros[i])[0]).clave.Equals((decimal)ClavesParametrosModulos.claveParametroPagosPorHora)) {
        //                if (manejaPagosPorHora==null) {
        //                    Object[] objects = metodosBDMaestra.parametroPagosPorHora((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
        //                    manejaPagosPorHora = (bool)objects[0];
        //                }
        //            }
        //            else if (((Parametros)(listParametros[i])[0]).clave.Equals((decimal)ClavesParametrosModulos.claveParametroManejarHorasPor))
        //            {
        //                if (manejaPagosPorHora == null)
        //                {
        //                    manejoHorasPor= metodosBDMaestra.parametroManejarHorasPor((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });

        //                }
        //            }
        //            else if (((Parametros)(listParametros[i])[0]).clave.Equals((decimal)ClavesParametrosModulos.claveParametroManejarSalarioDiarioPor))
        //            {
        //                parametroManejarSalarioDiarioPor = listParametros[i];
        //            }
        //            else if (((Parametros)(listParametros[i])[0]).clave.Equals((decimal)ClavesParametrosModulos.claveParametroFactorAplicaciónTablaMensual))
        //            {
        //                factorMensual= metodosBDMaestra.parametroFactorAplicacionTablaMensual((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
        //            }
        //            else if (((Parametros)(listParametros[i])[0]).clave.Equals((decimal)ClavesParametrosModulos.claveParametroModoAjustarIngresosAlMes))
        //            {
        //                modoAjustarIngresosMes = metodosBDMaestra.parametroModoAjustarIngresosAlMes((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
        //                if (modoAjustarIngresosMes == ProporcionaCadaPeriodoUtilizandoTablaPeriodo)
        //                {
        //                    tipoTablaISR = TipoTablaISR.PERIODICIDAD;
        //                }
        //            }
        //            else if (((Parametros)(listParametros[i])[0]).clave.Equals((decimal)ClavesParametrosModulos.claveParametroDesgloseInternoISR))
        //            {
        //                desgloseInternoISR= metodosBDMaestra.parametroDesgloseInternoISR((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
        //            }
        //            else if (((Parametros)(listParametros[i])[0]).clave.Equals((decimal)ClavesParametrosModulos.clavePagarNominaDiasNaturales))
        //            {
        //                desgloseInternoISR = metodosBDMaestra.parametroDesgloseInternoISR((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerFactores()1_Error: ").Append(ex));
        //        mensajeResultado.noError = 1;
        //        mensajeResultado.error = ex.GetBaseException().ToString();
        //    }
        //}

        //public Mensaje getParametrosYListCrucePorModuloYClaves(String claveModulo, Object[] clavesParametros)
        //{
        //    List<Object[]> listParametrosYListCruce = new List<Object[]>();
        //    try
        //    {
        //        inicializaVariableMensaje();
        //        List<Parametros> listparametros = (from p in getSession().Set<Parametros>()
        //                                           where p.modulo.clave == claveModulo && clavesParametros.Contains(p.clave)
        //                                           orderby p.clave
        //                                           select p).ToList();
        //        if (listparametros.Count() > 0)
        //        {
        //            for (int i = 0; i < listparametros.Count(); i++)
        //            {
        //                List<Cruce> values;//Si el parametro no tiene seleccionado elementos de aplicacion quiere decir que no se va filtrar o profuncidar por algun elemento de aplicacion
        //                if (listparametros[i].elementosAplicacion == null ? false : listparametros[i].elementosAplicacion.Count() > 0)
        //                {
        //                    values = (from c in getSession().Set<Cruce>()
        //                              where c.parametros.clave == listparametros[i].clave && listparametros[i].elementosAplicacion.Contains(c.elementosAplicacion)
        //                              orderby c.elementosAplicacion.ordenId descending
        //                              select c).ToList();
        //                }
        //                else
        //                {
        //                    values = new List<Cruce>();
        //                }
        //                Object[] objects = new Object[2];
        //                objects[0] = listparametros[i];
        //                objects[1] = values;
        //                listParametrosYListCruce.Add(objects);
        //                values = null;
        //            }
        //        }
        //        mensajeResultado.resultado = listParametrosYListCruce;
        //        mensajeResultado.noError = 0;
        //        mensajeResultado.error = "";
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getParametrosYListCrucePorModuloYClaves()1_Error: ").Append(ex));
        //        mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
        //        mensajeResultado.error = ex.GetBaseException().ToString();
        //    }
        //    return mensajeResultado;
        //}
        public Mensaje calculaSDIPorEmpleado(PlazasPorEmpleadosMov plazasPorEmpleadosMov, string controlador, ParametrosExtra parametrosExtra, bool soloCalculo, bool peticionModuloCalculoSalarioDiarioIntegrado, DBContextAdapter dbContextSimple, DBContextAdapter dbContextMaestra)
        {
            Empleados empleado = plazasPorEmpleadosMov.plazasPorEmpleado.empleados;

            return calculaSalarioDiarioIntegerado(empleado.clave, empleado.clave, plazasPorEmpleadosMov.tipoNomina.clave,/*
                 * tipoCorrida
                 */ "", /*
                 * periodoNomina
                 */ null,
                                                                                                                                                                                                                                                                                                                                        /*
                                                                                                                                                                                                                                                                                                                                         * clavePuesto
                                                                                                                                                                                                                                                                                                                                         */ "", /*
                 * categoriaPuestos
                 */ "",/*
                 * claveTurno
                 */ "", empleado.razonesSociales.clave, plazasPorEmpleadosMov.plazasPorEmpleado.registroPatronal.clave,
                                                                                                                                                                                                                                                                                                                                        /*
                                                                                                                                                                                                                                                                                                                                         * Forma Pago
                                                                                                                                                                                                                                                                                                                                         */ "", /*
                 * Depto
                 */ "", /*
                 * Centro Costo
                 */ "", /*
                 * tipo salario
                 */ null, /*
                 * contrato
                 */ null, /*
                 * status
                 */ null, controlador, 0, parametrosExtra, soloCalculo, peticionModuloCalculoSalarioDiarioIntegrado, dbContextSimple, dbContextMaestra);
        }

        public Mensaje busquedaQueryConsultaEmpleados(string[] tablas, string[] camposMostrar, string[] camposWhere, object[] valoresWhere, string[] camposOrden, string[] valoresDatosEspeciales, string[] camposWhereExtras, string nombreFuenteDatos, DateTime[] rangoFechas, string ordenado, string claveRazonSocial, string controladores, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            int i, j, k;
            List<Object> resultados;
            mensajeResultado = new Mensaje();
            mensajeResultado.noError = 0;
            mensajeResultado.error = "";
            isCalculoSDI = false;
            isUMA = false;
            try
            {
                bool isValorParametroOConcepto = false;
                isMov2Meses = false;
                bool usaMesesEnQuery = false;
                if (camposMostrar.Length == 0)
                {
                    mensajeResultado.resultado = new List<object>();
                    return mensajeResultado;
                }
                camposWhereExtras = camposWhereExtras == null ? new string[] { } : camposWhereExtras;
                usaFiniquitos = false;
                bool isDatoImss = false, isTablaFactorIntegracion = false, isTablaZonaSalarial = false, isParametro = true;
                nombreFuenteDatos = nombreFuenteDatos == null ? "" : nombreFuenteDatos;
                camposWhere = (camposWhere == null ? new String[] { } : camposWhere);
                propertieFuente = CompEjec.abrirPropiedadBundle(nombreFuenteDatos);
                if (propertieFuente == null)
                {
                    propertieFuente = CompEjec.abrirPropiedadBundle("FuentesDeDatos");
                }

                string[] camposFormula;
                TipoClasificacionFormula tipoClasifFormula;
                string field;
                string[] fields;
                TipoNodoConsulta tipoNodoConsulta;
                bool usaMovEnEmpleados = false;
                for (i = 0; i < camposMostrar.Length; i++)
                {
                    if (camposMostrar[i].Contains("#"))
                    {
                        fields = camposMostrar[i].Split('#');
                        field = fields[0];
                    }
                    else if (camposMostrar[i].Contains("|"))
                    {
                        fields = camposMostrar[i].Split('|');
                        if (fields.Length > 1 && string.Equals(nombreFuenteDatos, "FuenteDatos_Empleados", StringComparison.OrdinalIgnoreCase))
                        {
                            for (int l = 0; l < fields.Length; l++)
                            {
                                if (fields[l].StartsWith(typeof(MovNomConcep).GetType().Name))
                                {
                                    usaMovEnEmpleados = true;
                                }
                            }

                        }
                        field = fields[0];
                    }
                    else
                    {
                        field = camposMostrar[i];
                    }

                    if (propertieFuente != null)
                    {
                        if (propertieFuente.Contains(field))
                        {
                            if (string.Equals("TotalImportePorConcepto_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalImporteExentoPorConcepto_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalImporteGravablePorConcepto_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalImporteGravablePorConceptoDato_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalImportePorConceptoDato_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalImporteExentoPorConceptoDato_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISR_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISRNormal_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISRDirecto_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISRAnual_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISRGravable_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISRGravableNormal_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISRGravableDirecto_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISRGravableAnual_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISRGravableNormal_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISRGravableDirecto_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISRGravableAnual_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISRExento_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISRExentoNormal_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISRExentoDirecto_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISRExentoAnual_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseIMSS_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseIMSSFija_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseIMSSVariable_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseIMSSGravado_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseIMSSGravadoFija_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseIMSSGravadoVariable_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseIMSSExento_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseIMSSExentoFija_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseIMSSExentoVariable_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseInfonavit_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBasePTU_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseImpuestoNomina_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseDespensa_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseFondoAhorro_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseAguinaldo_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseOtros_Path", field, StringComparison.OrdinalIgnoreCase))
                            {

                                isValorParametroOConcepto = true;
                                isMov2Meses = true;
                            }
                            tipoNodoConsulta = (TipoNodoConsulta)ManejadorEnum.GetValue(propertieFuente.GetProperty(string.Concat(field, "_TipoNodo")), typeof(TipoNodoConsulta));
                            field = propertieFuente.GetProperty(field);
                            if (field.Contains("|"))
                            {
                                fields = field.Split('|');
                                field = fields[0];
                            }
                            if ((tipoNodoConsulta == TipoNodoConsulta.CAMPO || tipoNodoConsulta == TipoNodoConsulta.CAMPOESPECIAL) && (string.Equals(field, string.Concat("MovNomConcep", ".movNomConceParam.valor"), StringComparison.OrdinalIgnoreCase)
                                || string.Equals(field, string.Concat("MovNomConcep", ".resultado"), StringComparison.OrdinalIgnoreCase)
                                || string.Equals(field, string.Concat("MovNomConcep", ".movNomBaseAfecta.resultadoExento"), StringComparison.OrdinalIgnoreCase)
                                || string.Equals(field, string.Concat("MovNomConcep", ".movNomBaseAfecta.resultado"), StringComparison.OrdinalIgnoreCase)))
                            {
                                usaMesesEnQuery = true;
                            }
                            if (string.Equals(field, string.Concat("MovNomConcep", ".movNomConceParam.valor"), StringComparison.OrdinalIgnoreCase) && (
                                tipoNodoConsulta == TipoNodoConsulta.CAMPO || tipoNodoConsulta == TipoNodoConsulta.CAMPOESPECIAL))
                            {
                                isValorParametroOConcepto = true;
                                isMov2Meses = true;

                            }
                            else if (string.Equals(field, string.Concat("MovNomConcep", ".resultado"), StringComparison.OrdinalIgnoreCase)
                              || string.Equals(field, string.Concat("MovNomConcep", ".movNomBaseAfecta.resultadoExento"), StringComparison.OrdinalIgnoreCase)
                              || string.Equals(field, string.Concat("MovNomConcep", ".movNomBaseAfecta.resultado"), StringComparison.OrdinalIgnoreCase))
                            {
                                isMov2Meses = true;
                            }

                        }
                        else if (string.Equals(field, "MovNomConcep" + ".movNomConceParam.valor", StringComparison.OrdinalIgnoreCase))
                        {
                            isMov2Meses = true;
                            usaMesesEnQuery = true;
                        }
                        else if (string.Equals(field, "MovNomConcep" + ".resultado", StringComparison.OrdinalIgnoreCase) || string.Equals(field, "MovNomConcep" + ".movNomBaseAfecta.resultadoExento", StringComparison.OrdinalIgnoreCase)
                          || string.Equals(field, "MovNomConcep" + ".movNomBaseAfecta.resultado", StringComparison.OrdinalIgnoreCase))
                        {
                            isMov2Meses = true;
                            usaMesesEnQuery = true;
                        }

                    }
                    else if (string.Equals(field, "MovNomConcep" + ".movNomConceParam.valor", StringComparison.OrdinalIgnoreCase))
                    {
                        isMov2Meses = true;
                        usaMesesEnQuery = true;
                    }
                    else if (string.Equals(field, "MovNomConcep" + ".resultado", StringComparison.OrdinalIgnoreCase) || string.Equals(field, "MovNomConcep" + "..movNomBaseAfecta.resultadoExento", StringComparison.OrdinalIgnoreCase)
                      || string.Equals(field, "MovNomConcep" + ".movNomBaseAfecta.resultado", StringComparison.OrdinalIgnoreCase))
                    {
                        isMov2Meses = true;
                        usaMesesEnQuery = true;
                    }
                    if (string.Equals(nombreFuenteDatos, "FuenteDatos_Empleados", StringComparison.OrdinalIgnoreCase) && field.StartsWith("MovNomConcep"))
                    {
                        usaMovEnEmpleados = true;
                    }

                    if (camposMostrar[i].StartsWith("@"))
                    {
                        ////camposFormula = obtieneVariablesCampoFormula(camposMostrar[i].substring(1));
                        camposFormula = eliminaCaracteresSeparador(camposMostrar[i].Substring(1)).Split('|');
                        foreach (string strCampoaux in camposFormula)
                        {
                            tipoClasifFormula = TipoClasificacionFormula.SINCLASIFICAR;
                            string strCampo = strCampoaux;
                            if (strCampo.IndexOf('(') != -1)
                            {
                                strCampo = strCampo.Substring(0, strCampo.IndexOf('('));
                            }
                            if (propertieFuente.ContainsKey(string.Concat(strCampo, "_TipoDato")))
                            {
                                tipoClasifFormula = (TipoClasificacionFormula)ManejadorEnum.GetValue(propertieFuente.GetProperty(string.Concat(strCampo, "_TipoDato")), typeof(TipoClasificacionFormula));
                            }
                            else if (propertieFuente.ContainsKey(strCampo))
                            {
                                strCampo = propertieFuente.GetProperty(strCampo);
                                if (propertieFuente.ContainsKey(string.Concat(strCampo, "_TipoDato")))
                                {
                                    tipoClasifFormula = (TipoClasificacionFormula)ManejadorEnum.GetValue(propertieFuente.GetProperty(string.Concat(strCampo, "_TipoDato")), typeof(TipoClasificacionFormula));
                                }
                            }
                            if (tipoClasifFormula == TipoClasificacionFormula.DATOIMSS)
                            {
                                isDatoImss = true;
                            }
                            else if (string.Equals(strCampo, "VacacionesPorDisfrutar") || tipoClasifFormula == TipoClasificacionFormula.TABLAFACTORINTEGRACION)
                            {
                                isTablaFactorIntegracion = true;
                            }
                            else if (tipoClasifFormula == TipoClasificacionFormula.TABLAZONASALARIAL)
                            {
                                isTablaZonaSalarial = true;
                            }
                            else if (tipoClasifFormula == TipoClasificacionFormula.DATOPARAMETRO)
                            {
                                isParametro = true;
                            }
                        }

                    }

                }//end de for campos a mostrar
                if (string.Equals(nombreFuenteDatos, "FuenteDatos_Empleados", StringComparison.OrdinalIgnoreCase))
                {
                    if (camposOrden == null ? false : camposOrden.Length > 0)
                    {
                        for (i = 0; i < camposOrden.Length; i++)
                        {
                            if (camposOrden[i].Contains("#"))
                            {
                                fields = camposOrden[i].Split('#');
                                field = fields[0];
                            }
                            else if (camposOrden[i].Contains("|"))
                            {
                                fields = camposOrden[i].Split('|');
                                for (int l = 1; l < fields.Length; l++)
                                {
                                    if (fields[l].StartsWith(typeof(MovNomConcep).Name))
                                    {
                                        usaMovEnEmpleados = true;
                                    }
                                }
                                field = fields[0];
                            }
                            else
                            {

                                field = camposOrden[i];
                            }
                            if (field.StartsWith(typeof(MovNomConcep).Name))
                            {
                                usaMovEnEmpleados = true;
                            }
                        }
                    }
                }

                String corrida = "";
                if (!usaMovEnEmpleados && string.Equals(nombreFuenteDatos, "FuenteDatos_Empleados", StringComparison.OrdinalIgnoreCase))
                {


                }//!usaMovEnEmpleados & nombreFuenteDatos.equalsIgnoreCase("FuenteDatos_Empleados")

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("cargaDatosVariableConfiguracionIMSS()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return mensajeResultado;
        }
        private String eliminaCaracteresSeparador(String texto)
        {
            StringBuilder sb = new StringBuilder();
            texto = texto == null ? "" : texto;
            for (int i = 0; i < texto.Length; i++)
            {
                if (!Char.IsWhiteSpace(texto[i]))
                {
                    sb.Append(texto[i]);
                }

            }
            return sb.ToString();
        }
        #region metodos usados calculo nomina
        private void buscaPeriodicidadesOrPeriodosNomina(string claveTipoNomina, string claveTipoCorrida, decimal? idPeriodoNomina)
        {
            try
            {

                if (!String.IsNullOrEmpty(claveTipoNomina) | periodicidadTipoNomina == null)
                {

                    periodicidadTipoNomina = (from t in dbContextSimple.Set<TipoNomina>()
                                              where t.clave.Equals(claveTipoNomina)
                                              select t.periodicidad).SingleOrDefault<Periodicidad>();



                }

                if (periodosNomina == null)
                {
                    if (idPeriodoNomina == null && !isCalculoSDI)
                    {
                        periodosNomina = (from p in dbContextSimple.Set<PeriodosNomina>()
                                          where (fechaActual >= p.fechaInicial && fechaActual <= p.fechaFinal) && p.tipoNomina.clave.Equals(claveTipoNomina) && p.tipoCorrida.clave.Equals(claveTipoCorrida)
                                          select p).SingleOrDefault<PeriodosNomina>();
                    }
                    else if (isCalculoSDI)
                    {
                        periodosNomina = (from p in dbContextSimple.Set<PeriodosNomina>()
                                          where (fechaActual >= p.fechaInicial && fechaActual <= p.fechaFinal) && p.tipoNomina.clave.Equals(claveTipoNomina) && p.tipoCorrida.clave.Equals(claveTipoCorrida)
                                          select p).SingleOrDefault<PeriodosNomina>();
                    }
                    else
                    {
                        periodosNomina = (from p in dbContextSimple.Set<PeriodosNomina>()
                                          where p.id == idPeriodoNomina
                                          select p).SingleOrDefault<PeriodosNomina>();
                    }
                    if (periodosNomina == null && !isCalculoSDI)
                    {
                        mensajeResultado.noError = 1;
                        mensajeResultado.error = "Favor de verificar que existen periodos de nomina";
                    }
                }



            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscaPeriodicidadesOrPeriodosNomina()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;

            }
        }

        //Generera extructura xml
        private void generaTablasXml(string controlador, Periodicidad periodicidadTipoNomina, string claveRazonSocial, DateTime fechaFinal, int ejercicio, DbContext uuidCxnMaestra)
        {
            MetodosBDMaestra metodos = new MetodosBDMaestra(fechaActual);
            metodos.generaTablasXml(controlador, periodicidadTipoNomina, claveRazonSocial, fechaFinal, ejercicio, (DBContextMaster)uuidCxnMaestra);
            calculoSeparadoISR = metodos.calculoSeparadoISR;
            descontarFaltasModoAjustaMes = metodos.descontarFaltasModoAjustaMes;
            factorAnual = metodos.factorAnual;
            factorMensual = metodos.factorMensual;
            isUMA = metodos.isUMA;
            manejaPagoDiasNaturales = metodos.manejaPagoDiasNaturales;
            manejaPagoIMSSDiasNaturales = metodos.manejaPagoIMSSDiasNaturales;
            manejaPagosPorHora = metodos.manejaPagosPorHora;
            manejoHorasPor = metodos.manejoHorasPor;
            manejoSalarioDiario = metodos.manejoSalarioDiario;
            modoAjustarIngresosMes = metodos.modoAjustarIngresosMes;
            pagarVacaAuto = metodos.pagarVacaAuto;
            salarioVacaciones = metodos.salarioVacaciones;
            tablaDatosXml = metodos.tablaDatosXml;
            tablaFactorIntegracion = metodos.tablaFactorIntegracion;
            tablaIsr = metodos.tablaIsr;
            tablaSubsidio = metodos.tablaSubsidio;
            tablaZonaSalarial = metodos.tablaZonaSalarial;
            tipoTablaISR = metodos.tipoTablaISR;
            tablaIsrMes = metodos.tablaIsrMes;
            tablaSubsidioMes = metodos.tablaSubsidioMes;
            valorUMA = metodos.valorUMA;
            versionCalculoPrestamoAhorro = metodos.versionCalculoPrestamoAhorro;
            mensajeResultado = metodos.mensajeResultado;

        }

        #region cargado e inicializado de variables globales y del empleado
        private void cargarVariablesConceptosCompilador()
        {

            var queryConcep = (from c in dbContextSimple.Set<ConcepNomDefi>()
                               where c.fecha == (from cc in dbContextSimple.Set<ConcepNomDefi>() where c.clave == cc.clave select new { cc.fecha })
                                      .Max(p => p.fecha)
                               orderby c.clave
                               select new
                               {
                                   c.clave,
                                   c.descripcion
                               }).ToList();

            variablesConceptos = null;

            string[][] valores = queryConcep.Select(p => new[] { String.Concat("CONCEP_", p.clave), String.Concat("CONCEP_", p.descripcion).Replace(" ", "_") }).ToArray();//queryConcep.ToArray();
            if (valores != null)
            {
                variablesConceptos = new string[valores.Count(), 2];
                for (int i = 0; i < valores.Count(); i++)
                {
                    for (int j = 0; j < valores[i].Length; j++)
                    {
                        variablesConceptos[i, j] = valores[i][j];
                    }

                }
            }

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
                    ZonaSalarial salarioZona = buscaSalarioPorZona('A');
                    if (salarioZona == null)
                    {
                        mensajeResultado.noError = 20;
                        mensajeResultado.error = "No existe zona salarial A";
                        return;
                    }
                    valorSMG = salarioZona.salario;
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
                    valorSMG = salarioZona.salario;
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
                tipoCorrida = (from c in dbContextSimple.Set<TipoCorrida>() where c.clave == claveTipoCorrida select c).SingleOrDefault<TipoCorrida>();
                valoresConceptosGlobales["TipoCorridaAlfa".ToUpper()] = tipoCorrida == null ? "" : tipoCorrida.descripcion;
                razonesSociales = (from r in dbContextSimple.Set<RazonesSociales>() where r.clave == claveRazonSocial select r).SingleOrDefault<RazonesSociales>();
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

                valoresConceptosEmpleados["NumSubcuenta".ToUpper()] = plazaPorEmpleadoMov.departamentos == null ? "" : plazaPorEmpleadoMov.departamentos.subCuenta;
                valoresConceptosEmpleados["DepartamentoEmpleadoAlfa".ToUpper()] = plazaPorEmpleadoMov.departamentos == null ? "" : plazaPorEmpleadoMov.departamentos.descripcion;
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
                    valoresConceptosEmpleados["RazonSocial".ToUpper()] = claveRazonSocial;
                    razonesSociales = (from rs in dbContextSimple.Set<RazonesSociales>() where rs.clave == claveRazonSocial select rs).SingleOrDefault();
                }
                centroDeCostoMovimiento = plazaPorEmpleadoMov.centroDeCosto;
                cargaDatosSalarioDiario(plazaPorEmpleadoMov, periodosNomina);
                valoresConceptosEmpleados["FechaAlta".ToUpper()] = ingresosReingresosBajas == null ? plazaPorEmpleadoMov.fechaInicial : ingresosReingresosBajas.fechaIngreso;
                valoresConceptosEmpleados["FechaBaja".ToUpper()] = fechaBajaFiniq == null ? plazaPorEmpleadoMov.plazasPorEmpleado.fechaFinal : fechaBajaFiniq;
                string referencia = plazaPorEmpleadoMov.plazasPorEmpleado.referencia;

                ////DbContext contextSimple = new DBContextSimple();

                //DateTime? plazaEmpAltaImss = (from pmov in dbContextSimple.Set<PlazasPorEmpleadosMov>()
                //                              where pmov.plazasPorEmpleado.referencia == referencia
                //                              orderby pmov.fechaInicial ascending
                //                              select pmov.fechaIMSS).Skip(0).Take(1).SingleOrDefault();

                DateTime? plazaEmpAltaImss = (from pmov in dbContextSimple.Set<PlazasPorEmpleadosMov>()
                                              where pmov.plazasPorEmpleado.referencia == referencia &&
                                                  pmov.fechaInicial == (from pmovx in dbContextSimple.Set<PlazasPorEmpleadosMov>()
                                                                        where pmovx.plazasPorEmpleado.referencia == referencia
                                                                        select new { pmovx.fechaInicial }).Min(f => f.fechaInicial)
                                              select pmov.fechaIMSS).SingleOrDefault();


                if (plazaEmpAltaImss == null)
                {
                    valoresConceptosEmpleados["FechaAltaIMSS".ToUpper()] = plazaPorEmpleadoMov.fechaIMSS;
                }
                else
                {
                    valoresConceptosEmpleados["FechaAltaIMSS".ToUpper()] = plazaEmpAltaImss.GetValueOrDefault();
                }

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
                SalariosIntegrados salariosIntegrados = null;

                if (isCalculoSDI)
                {
                    salariosIntegrados = obtieneSalarioDiarioIntegrado(plazaPorEmpleadoMov.plazasPorEmpleado.empleados.clave, plazaPorEmpleadoMov.plazasPorEmpleado.razonesSociales.clave,
                  plazaPorEmpleadoMov.plazasPorEmpleado.registroPatronal == null ? "" : plazaPorEmpleadoMov.plazasPorEmpleado.registroPatronal.clave,
                   tipoCorrida == null ? "" : tipoCorrida.clave, periodosNomina, isCalculoSDI);
                }
                else
                {
                    DateTime fecha = periodosNomina.fechaFinal.GetValueOrDefault();
                    string claveEmpleado = plazaPorEmpleadoMov.plazasPorEmpleado.empleados.clave;
                    string claveRegPatronal = plazaPorEmpleadoMov.plazasPorEmpleado.registroPatronal == null ? "" : plazaPorEmpleadoMov.plazasPorEmpleado.registroPatronal.clave;
                    salariosIntegrados = (from s in dbContextSimple.Set<SalariosIntegrados>()
                                          where s.fecha <= fecha.Date && s.empleados.clave == claveEmpleado && s.registroPatronal.clave == claveRegPatronal &&
                                          s.empleados.razonesSociales.clave == claveRazonSocial
                                          orderby s.fecha descending
                                          select s).Skip(0).Take(1).SingleOrDefault();
                }

                if (mensajeResultado.noError != 0)
                {
                    return;
                }
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

                int? modoDescuentoInfonavit = (from ce in dbContextSimple.Set<CreditoPorEmpleado>()
                                               where ce.empleados.clave == plazaPorEmpleadoMov.plazasPorEmpleado.empleados.clave &&
                                               ce.razonesSociales.clave == plazaPorEmpleadoMov.plazasPorEmpleado.razonesSociales.clave && ce.creditoAhorro.clave == "005"
                                               select ce.modoDescuentoCredito).SingleOrDefault();
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

                int numPlazas = (from p in dbContextSimple.Set<PlazasPorEmpleado>()
                                 where p.empleados.clave == plazaPorEmpleadoMov.plazasPorEmpleado.empleados.clave && p.razonesSociales.clave == plazaPorEmpleadoMov.plazasPorEmpleado.razonesSociales.clave
                                 select p).Count();
                valoresConceptosEmpleados["NumeroPlazasEmpleado".ToUpper()] = numPlazas;
                int cont = 0;
                if (plazaPorEmpleadoMov.turnos != null)
                {
                    if (plazaPorEmpleadoMov.turnos.turnosHorariosFijos_turnos.Count == 0)
                    {
                        valoresConceptosEmpleados["DiaDescanso1".ToUpper()] = "";
                        valoresConceptosEmpleados["DiaDescanso2".ToUpper()] = "";
                    }
                    for (int i = 0; i < plazaPorEmpleadoMov.turnos.turnosHorariosFijos_turnos.Count; i++)
                    {
                        if (plazaPorEmpleadoMov.turnos.turnosHorariosFijos_turnos[i].statusDia == 1)
                        {
                            cont++;
                            if (cont == 1)
                            {
                                valoresConceptosEmpleados["DiaDescanso1".ToUpper()] = plazaPorEmpleadoMov.turnos.turnosHorariosFijos_turnos[i].diaSemana;
                            }
                            else if (cont == 2)
                            {
                                valoresConceptosEmpleados["DiaDescanso2".ToUpper()] = plazaPorEmpleadoMov.turnos.turnosHorariosFijos_turnos[i].diaSemana;
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
                if (periodosNomina == null)
                {
                    valoresConceptosEmpleados["FechaUltimoCambioSueldo".ToUpper()] = "";
                }
                else
                {
                    object fechaUltimoCambioSueldo = (from pm in dbContextSimple.Set<PlazasPorEmpleadosMov>()
                                                      where pm.plazasPorEmpleado.empleados.clave == plazaPorEmpleadoMov.plazasPorEmpleado.empleados.clave &&
                                                        pm.plazasPorEmpleado.razonesSociales.clave == plazaPorEmpleadoMov.plazasPorEmpleado.razonesSociales.clave /*&& pm.cambioSalarioPor == true*/ &&
                                                          (pm.fechaInicial >= periodosNomina.fechaInicial && pm.fechaInicial <= periodosNomina.fechaFinal)
                                                      orderby pm.fechaInicial descending
                                                      select pm.fechaInicial).FirstOrDefault();
                    valoresConceptosEmpleados["FechaUltimoCambioSueldo".ToUpper()] = fechaUltimoCambioSueldo == null ? "" : fechaUltimoCambioSueldo;
                }
                VacacionesDisfrutadas vacacionesDis = (from r in dbContextSimple.Set<VacacionesDisfrutadas>()
                                                       where r.empleados.clave == plazaPorEmpleadoMov.plazasPorEmpleado.empleados.clave &&
                                                           r.razonesSociales.clave == plazaPorEmpleadoMov.plazasPorEmpleado.razonesSociales.clave
                                                       select r).SingleOrDefault();
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
            cargarVariablesEmpleadoVacaciones(calculoUnidades, plazasPorEmpleadosMovEjecutandose, false);
            if (mensajeResultado.noError != 0)
            {
                return;
            }
            if (manejaPagoDiasNaturales)
            {
                valoresConceptosEmpleados["DiasNormalesAPagar".ToUpper()] = valoresConceptosEmpleados["DiasNaturalesDelPeriodo".ToUpper()];
                if (isMov2Meses && pagarPrimero3Dias == 3)
                {
                    double incapacidad = (double)valoresConceptosEmpleados["DiasIncapacidadEmpleado".ToUpper()];
                    if (pagarPrimero3Dias < Convert.ToInt32(incapacidad))
                    {
                        pagarPrimero3Dias = pagarPrimero3Dias - Convert.ToInt32(incapacidad);
                    }
                }
                //int diasVacaciones = tipoCorrida == null ? (Integer) valoresConceptosEmpleados.get("diasVacaciones".ToUpper()) : tipoCorrida.getClave().equalsIgnoreCase("VAC") ? (Integer) valoresConceptosEmpleados.get("diasVacaciones".ToUpper()) : 0;
                int diasVacaciones = Convert.ToInt32(valoresConceptosEmpleados["diasVacaciones".ToUpper()]);
                valoresConceptosEmpleados["DiasPago".ToUpper()] =
                        ((int)valoresConceptosEmpleados["DiasNormalesAPagar".ToUpper()]
                        - (diasDif
                        + ((double)valoresConceptosEmpleados["DiasIncapacidadEmpleado".ToUpper()]
                        + (double)valoresConceptosEmpleados["Faltas".ToUpper()]
                        + (double)valoresConceptosEmpleados["Ausentismo".ToUpper()]
                        + diasVacaciones))) + pagarPrimero3Dias;
                descontarDiasPago = (/*diasDif
                     + */((double)valoresConceptosEmpleados["DiasIncapacidadEmpleado".ToUpper()]
                        + (double)valoresConceptosEmpleados["Faltas".ToUpper()]//Activo suma inactivo no sum
                        + (double)valoresConceptosEmpleados["Ausentismo".ToUpper()]
                        + diasVacaciones)) - pagarPrimero3Dias;
            }
            else
            {
                valoresConceptosEmpleados["DiasNormalesAPagar".ToUpper()] = valoresConceptosEmpleados["PeriodicidadEnDias".ToUpper()];
                ///int diasVacaciones = tipoCorrida == null ? (Integer) valoresConceptosEmpleados.get("diasVacaciones".ToUpper()) : tipoCorrida.getClave().equalsIgnoreCase("VAC") ? (Integer) valoresConceptosEmpleados.get("diasVacaciones".ToUpper()) : 0;
                int diasVacaciones = Convert.ToInt32(valoresConceptosEmpleados["diasVacaciones".ToUpper()]);
                valoresConceptosEmpleados["DiasPago".ToUpper()] =
                        ((int)valoresConceptosEmpleados["DiasNormalesAPagar".ToUpper()]
                        - (diasDif
                        + ((double)valoresConceptosEmpleados["DiasIncapacidadEmpleado".ToUpper()]
                        + (double)valoresConceptosEmpleados["Faltas".ToUpper()]
                        + (double)valoresConceptosEmpleados["Ausentismo".ToUpper()]
                        + diasVacaciones))) + pagarPrimero3Dias;
                descontarDiasPago = (/*diasDif
                     + */((double)valoresConceptosEmpleados["DiasIncapacidadEmpleado".ToUpper()]
                        + (double)valoresConceptosEmpleados["Faltas".ToUpper()]
                        + (double)valoresConceptosEmpleados["Ausentismo".ToUpper()] + diasVacaciones)) - pagarPrimero3Dias;
            }
            if (calculoUnidades != null)
            {
                calculoUnidades.diasTrabajados = (double)valoresConceptosEmpleados["DiasPago".ToUpper()];
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
                        + ((double)valoresConceptosEmpleados["DiasIncapacidadEmpleado".ToUpper()]
                        + (double)valoresConceptosEmpleados["Ausentismo".ToUpper()])));
            }
            else
            {
                valoresConceptosEmpleados["DiasCotizados".ToUpper()] = ((int)valoresConceptosEmpleados["PeriodicidadEnDias".ToUpper()] - (diasDif
                        + ((double)valoresConceptosEmpleados["DiasIncapacidadEmpleado".ToUpper()]
                        + (double)valoresConceptosEmpleados["Ausentismo".ToUpper()])));
            }
        }

        private void cargarVariablesEmpleadoAsistencias(DateTime fechaInicial, DateTime fechaFinal, CalculoUnidades calculoUnidades, bool? modificarDiasTrabajados, bool acumularAsis)
        {
            mensajeResultado = metodosDatosAsistencias.cargarVariablesEmpleadoAsistencias(fechaInicial, fechaFinal, calculoUnidades, periodosNomina, fechaBajaFiniq, modificarDiasTrabajados, acumularAsis,
                valoresConceptosEmpleados["NumEmpleado".ToUpper()].ToString(), valoresConceptosEmpleados["RazonSocial".ToUpper()].ToString(), valoresConceptosEmpleados[typeof(TipoNomina).Name.ToUpper()].ToString(),
                valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()].ToString(), (DateTime)valoresConceptosEmpleados[parametroFechaInicial], (DateTime)valoresConceptosEmpleados[parametroFechaFinal], valoresConceptosEmpleados, (DBContextSimple)dbContextSimple);
            pagarPrimero3Dias = metodosDatosAsistencias.pagarPrimero3Dias;
            if (mensajeResultado.noError == 0)
            {
                valoresConceptosEmpleados = (Dictionary<string, object>)mensajeResultado.resultado;
            }
        }

        private void cargarVariablesEmpleadoVacaciones(CalculoUnidades calculoUnidades, PlazasPorEmpleadosMov plazasPorEmpleadosMovEjecutandose, bool acumularVac)
        {
            mensajeResultado = metodosParaVacaciones.cargarVariablesEmpleadoVacaciones(calculoUnidades, plazasPorEmpleadosMovEjecutandose, acumularVac, periodosNomina, fechaBajaFiniq,
                valoresConceptosEmpleados[typeof(TipoNomina).Name.ToUpper()].ToString(), valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()].ToString(),
                valoresConceptosEmpleados, (DBContextSimple)dbContextSimple);
            if (mensajeResultado.noError == 0)
            {
                valoresConceptosEmpleados = (Dictionary<string, object>)mensajeResultado.resultado;
            }
            vacacionesAplicacionStatus = metodosParaVacaciones.vacacionesAplicacionStatus;
        }

        private void cargaDatosVariableConfiguracionIMSS(DateTime fechaPeriodoFinal)
        {
            try
            {
                if (valoresConceptosGlobales == null)
                {
                    valoresConceptosGlobales = new Dictionary<string, object>();
                }
                // * ***********************************IMSS******************************************************************

                configuracionIMSS = (from imss in dbContextSimple.Set<ConfiguracionIMSS>()
                                     where imss.fechaAplica == (from im in dbContextSimple.Set<ConfiguracionIMSS>() where im.fechaAplica <= fechaPeriodoFinal select new { im.fechaAplica }).Max(f => f.fechaAplica)
                                     select imss).SingleOrDefault();
                if (mensajeResultado.noError != 0 || configuracionIMSS == null)
                {
                    configuracionIMSS = (from imss in dbContextSimple.Set<ConfiguracionIMSS>()
                                         where imss.fechaAplica == (from im in dbContextSimple.Set<ConfiguracionIMSS>() where im.fechaAplica >= fechaPeriodoFinal select new { im.fechaAplica }).Min(f => f.fechaAplica)
                                         select imss).SingleOrDefault();
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("cargaDatosVariableConfiguracionIMSS()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
        }

        private SalariosIntegrados obtieneSalarioDiarioIntegrado(string claveEmpleado, string claveRazonSocial, string claveRegPatronal, string claveTipoCorrida, PeriodosNomina periodoNomina, bool calculoSDI)
        {
            SalariosIntegrados salarioDiarioIntegrado = null;
            try
            {
                DateTime? fecha = periodosNomina == null ? null : periodosNomina.fechaFinal;
                if (calculoSDI)
                {
                    fecha = DateTime.Now;
                }
                // DbContext Context = new DBContextSimple();
                if (!String.IsNullOrEmpty(claveTipoCorrida))
                {
                    if (String.Equals(claveTipoCorrida, "FIN", StringComparison.OrdinalIgnoreCase))
                    {
                        salarioDiarioIntegrado = (from s in dbContextSimple.Set<SalariosIntegrados>()
                                                  where s.fecha == (from sdi in dbContextSimple.Set<SalariosIntegrados>()
                                                                    where sdi.empleados.id == s.empleados.id && sdi.registroPatronal.id == s.registroPatronal.id &&
                                                                        sdi.fecha <= fecha &&
                                                                        sdi.fecha != (from sdix in dbContextSimple.Set<SalariosIntegrados>()
                                                                                      where sdix.empleados.id == sdi.empleados.id && sdix.registroPatronal.id == sdi.registroPatronal.id
                                                                                      select new { sdix.fecha }).Max(f => f.fecha)
                                                                    select new { sdi.fecha }).Max(f => f.fecha)
                                                          && s.empleados.clave == claveEmpleado && s.registroPatronal.clave == claveRegPatronal && s.empleados.razonesSociales.clave == claveRazonSocial
                                                  select s
                            ).SingleOrDefault();
                    }
                    else
                    {
                        salarioDiarioIntegrado = (from s in dbContextSimple.Set<SalariosIntegrados>()
                                                  where s.fecha == (from sdi in dbContextSimple.Set<SalariosIntegrados>()
                                                                    where sdi.empleados.id == s.empleados.id && sdi.registroPatronal.id == s.registroPatronal.id &&
                                                                        sdi.fecha <= fecha && sdi.finiquitosLiquida == null
                                                                    select new { sdi.fecha }).Max(f => f.fecha)
                                                          && s.empleados.clave == claveEmpleado && s.registroPatronal.clave == claveRegPatronal && s.empleados.razonesSociales.clave == claveRazonSocial
                                                  select s
                                ).SingleOrDefault();
                    }
                }
                else
                {
                    salarioDiarioIntegrado = (from s in dbContextSimple.Set<SalariosIntegrados>()
                                              where s.fecha == (from sdi in dbContextSimple.Set<SalariosIntegrados>()
                                                                where sdi.empleados.id == s.empleados.id && sdi.registroPatronal.id == s.registroPatronal.id &&
                                                                    sdi.fecha <= fecha
                                                                select new { sdi.fecha }).Max(f => f.fecha) && s.empleados.clave == claveEmpleado && s.registroPatronal.clave == claveRegPatronal &&
                                                      s.empleados.razonesSociales.clave == claveRazonSocial
                                              select s).SingleOrDefault();
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtieneSalarioDiarioIntegrado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return salarioDiarioIntegrado;
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
            valoresConceptosGlobales["AnioActualNumerico".ToUpper()] = periodo == null ? null : periodo.año;

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

            configuracionIMSS = (from imss in dbContextSimple.Set<ConfiguracionIMSS>()
                                 where imss.fechaAplica == (from im in dbContextSimple.Set<ConfiguracionIMSS>() where im.fechaAplica <= fechaFinal select new { im.fechaAplica }).Max(f => f.fechaAplica)
                                 select imss).SingleOrDefault();
            if (configuracionIMSS == null)
            {
                configuracionIMSS = (from imss in dbContextSimple.Set<ConfiguracionIMSS>()
                                     where imss.fechaAplica == (from im in dbContextSimple.Set<ConfiguracionIMSS>() where im.fechaAplica >= fechaFinal select new { im.fechaAplica }).Max(f => f.fechaAplica)
                                     select imss).SingleOrDefault();

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

        private List<CreditoAhorro> obtenerCreditosAhorro(string claveRazonSocial)
        {
            List<CreditoAhorro> listCreditosAhorros = null;
            try
            {
                listCreditosAhorros = (from c in dbContextSimple.Set<CreditoAhorro>()
                                       where c.concepNomiDefin != null && c.razonesSociales.clave == claveRazonSocial
                                       select c).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerCreditosAhorro()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return listCreditosAhorros;
        }

        private void consultarConfiguracionAgui()
        {
            try
            {
                string claveRazon = valoresConceptosGlobales[typeof(RazonSocial).Name.ToUpper()].ToString();
                string claveTipoCorrida = valoresConceptosGlobales["ClaveTipoCorrida".ToUpper()].ToString();
                string claveTipoNomina = valoresConceptosGlobales[typeof(TipoNomina).Name.ToUpper()].ToString();
                diasAguinaldo = (from d in dbContextSimple.Set<DiasAguinaldo>() where d.razonesSociales.clave == claveRazon select d).ToList();
                if (diasAguinaldo == null)
                {
                    diasAguinaldo = new List<DiasAguinaldo>();
                }

                aguiConfiguracion = (from d in dbContextSimple.Set<AguinaldoConfiguracion>() where d.razonesSociales.clave == claveRazon select d).SingleOrDefault();
                aguiFechas = (from d in dbContextSimple.Set<AguinaldoFechas>() where d.razonesSociales.clave == claveRazon select d).ToList();
                if (aguiFechas == null)
                {
                    aguiFechas = new List<AguinaldoFechas>();
                }
                if (aguiFechas.Count > 0)
                {
                    int i;
                    PeriodosNomina peraxu = null;
                    for (i = 0; i < aguiFechas.Count; i++)
                    {
                        DateTime fechaProgramada = aguiFechas[i].fechaProgramada.GetValueOrDefault();
                        PeriodosNomina per = (from p in dbContextSimple.Set<PeriodosNomina>()
                                              where p.tipoCorrida.clave == claveTipoCorrida &&
                                                    p.tipoNomina.clave == claveTipoNomina &&
                                                    (fechaProgramada >= p.fechaInicial && fechaProgramada <= p.fechaFinal)
                                              select p).SingleOrDefault();
                        if (peraxu == null)
                        {
                            peraxu = per;
                            totalPagosAgui++;
                        }
                        else
                        {
                            if (peraxu.id != per.id)
                            {
                                totalPagosAgui++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultarConfiguracionAgui()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
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
                double? salarioFinal = (from pm in dbContextSimple.Set<PlazasPorEmpleadosMov>()
                                        where pm.plazasPorEmpleado.empleados.clave == plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave &&
                                        pm.plazasPorEmpleado.razonesSociales.clave == plazasPorEmpleadosMov.plazasPorEmpleado.razonesSociales.clave &&
                                        (pm.fechaInicial >= periodoNomina.fechaInicial && pm.fechaInicial <= periodoNomina.fechaFinal)
                                        /*&& pm.cambioSalarioPor == true*/
                                        orderby pm.fechaInicial descending
                                        select pm.importe).FirstOrDefault();
                if (salarioFinal != null)
                {
                    valoresConceptosEmpleados["SUELDODIARIOFINAL".ToUpper()] = salarioFinal;
                }
            }
            double? sueldoAnterior = (from pm in dbContextSimple.Set<PlazasPorEmpleadosMov>()
                                      where pm.plazasPorEmpleado.empleados.clave == plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave &&
                                        pm.plazasPorEmpleado.razonesSociales.clave == plazasPorEmpleadosMov.plazasPorEmpleado.razonesSociales.clave &&
                                        pm.plazasPorEmpleado.plazaPrincipal == true && pm.fechaInicial <= plazasPorEmpleadosMov.fechaInicial
                                      orderby pm.fechaInicial descending
                                      select pm.importe).FirstOrDefault();
            // Was SingleOrDefault
            if (sueldoAnterior != null)
            {
                valoresConceptosEmpleados["SueldoAnterior".ToUpper()] = sueldoAnterior;
            }

        }

        #endregion

        #region calculos de unidades
        private List<CalculoUnidades> obtenerListaCalculoUnidadesUtilizar(string claveRazonSocial, PlazasPorEmpleado plazaPorEmpleado, string claveTipoNomina, decimal idPeriodoNomina, string claveTipoCorrida)
        {
            List<CalculoUnidades> listCalculoUnidasdes = null;
            try
            {
                listCalculoUnidasdes = (from cu in dbContextSimple.Set<CalculoUnidades>()
                                        where cu.uso == 0 && cu.periodosNomina.id == idPeriodoNomina && cu.tipoCorrida.clave == claveTipoCorrida && cu.tipoNomina.clave == claveTipoNomina &&
                                            cu.empleados.id == plazaPorEmpleado.empleados.id && cu.razonesSociales.clave == claveRazonSocial && cu.plazasPorEmpleado.referencia == plazaPorEmpleado.referencia
                                        orderby cu.razonesSociales.clave, plazaPorEmpleado.empleados.clave, cu.tipoNomina.clave, cu.periodosNomina.clave, cu.tipoCorrida.clave, cu.numero, cu.ejercicio, cu.mes
                                        select cu).ToList();
                if (listCalculoUnidasdes == null ? true : listCalculoUnidasdes.Count == 0)
                {
                    listCalculoUnidasdes = new List<CalculoUnidades>();
                    listCalculoUnidasdes.Add(crearCalculoUnidades(plazaPorEmpleado, tipoCorrida, razonesSociales));
                    if (evaluaPeriodoAbarca2Meses(periodosNomina))
                    {
                        CalculoUnidades calculoUnidad2 = crearCalculoUnidades(plazaPorEmpleado, tipoCorrida, razonesSociales);
                        calculoUnidad2.mes = periodosNomina.fechaFinal.GetValueOrDefault().Month;
                        calculoUnidad2.numMovParticion = 2;
                        listCalculoUnidasdes.Add(calculoUnidad2);
                    }
                }
                else if (evaluaPeriodoAbarca2Meses(periodosNomina))
                {
                    List<CalculoUnidades> listTemp = new List<CalculoUnidades>();
                    int mesUno = periodosNomina.fechaInicial.GetValueOrDefault().Month, mesDos = periodosNomina.fechaFinal.GetValueOrDefault().Month;
                    bool mesUnoEncontrado, mesDosEncontrado;
                    int i = 0, j;
                    while (i < listCalculoUnidasdes.Count)
                    {
                        mesUnoEncontrado = false;
                        mesDosEncontrado = false;
                        for (j = 0; j < listCalculoUnidasdes.Count; j++)
                        {
                            if (listCalculoUnidasdes[i].numero == listCalculoUnidasdes[j].numero)
                            {
                                if (listCalculoUnidasdes[j].mes == mesUno)
                                {
                                    mesUnoEncontrado = true;
                                }
                                else if (listCalculoUnidasdes[j].mes == mesDos)
                                {
                                    mesDosEncontrado = true;
                                }
                            }
                        }
                        if (!mesUnoEncontrado || !mesDosEncontrado)
                        {
                            CalculoUnidades newUnidad = crearCalculoUnidades(plazaPorEmpleado, tipoCorrida, razonesSociales);
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
                    listCalculoUnidasdes.AddRange(listTemp);
                    //ordena la lista
                    listCalculoUnidasdes = (from cu in listCalculoUnidasdes orderby cu.numero, cu.ejercicio, cu.mes select cu).ToList();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerListaCalculoUnidadesUtilizar()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return listCalculoUnidasdes;
        }

        private CalculoUnidades crearCalculoUnidades(PlazasPorEmpleado plazaPorEmpleado, TipoCorrida tipoCorrida, RazonesSociales razonSocial)
        {
            CalculoUnidades calculoUnidad = new CalculoUnidades();
            calculoUnidad.empleado_ID = plazaPorEmpleado.empleados_ID;
            // calculoUnidad.empleados = plazaPorEmpleado.empleados;
            calculoUnidad.plazasPorEmpleado_ID = plazaPorEmpleado.id;
            //calculoUnidad.plazasPorEmpleado = plazaPorEmpleado;
            calculoUnidad.periodosNomina_ID = periodosNomina.id;
            //calculoUnidad.periodosNomina = periodosNomina;
            calculoUnidad.tipoCorrida_ID = periodosNomina.tipoCorrida.id;
            //calculoUnidad.tipoCorrida = periodosNomina.tipoCorrida;
            calculoUnidad.tipoNomina_ID = periodosNomina.tipoNomina.id;
            //calculoUnidad.tipoNomina = periodosNomina.tipoNomina;
            calculoUnidad.razonesSociales_ID = razonSocial.id;
            //calculoUnidad.razonesSociales = razonSocial;
            calculoUnidad.numero = 1;
            calculoUnidad.ejercicio = periodosNomina.año.GetValueOrDefault();
            calculoUnidad.mes = periodosNomina.fechaInicial.GetValueOrDefault().Month;
            calculoUnidad.numMovParticion = 1;
            calculoUnidad.uso = 0;
            return calculoUnidad;
        }
        #endregion

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

        #region operaciones calculo nomina

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
                        mensajeResultado = metodosDatosEmpleados.obtenerMinimoPlazasPorEmpleadosMovDentroPeriodo(tipoCorrida.clave, periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault(), fechaBajaFiniq, plazasPorEmpleadosMov, (DBContextSimple)dbContextSimple);
                        List<PlazasPorEmpleadosMov> listPromocionesDentroPeriodo = new List<PlazasPorEmpleadosMov>();
                        if (mensajeResultado.noError == 0)
                        {
                            listPromocionesDentroPeriodo = (List<PlazasPorEmpleadosMov>)mensajeResultado.resultado;
                        }
                        else
                        {
                            //  break;
                        }
                        cargoSueldoDiarioX = true;
                        if (listPromocionesDentroPeriodo == null ? false : listPromocionesDentroPeriodo.Count > 0)
                        {
                            cargaDatosSalarioDiario(listPromocionesDentroPeriodo[0], periodosNomina);
                        }
                        else
                        {
                            cargaDatosSalarioDiario(plazasPorEmpleadosMov, periodosNomina);
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
                        mensajeResultado = metodosDatosEmpleados.obtenerModificacionesDePlazasPorEmpleadoMov(configuracionSueldoDiarioVigente, configuracionPercepcion_Plaza, configuracionPercepcion_Plaza_Vigente, movimientosNomina, plazasPorEmpleadosMov, movimientosNominaAux, periodosNomina, (DateTime)valoresConceptosEmpleados[parametroFechaInicial.ToUpper()], (DateTime)valoresConceptosEmpleados[parametroFechaFinal.ToUpper()],
                            fechaActual, fechaBajaFiniq, centroDeCostoMovimiento, tipoCorrida, dbContextAdapterSimple);
                        if (mensajeResultado.noError == 0)
                        {
                            List<Object> tmp = (List<Object>)mensajeResultado.resultado;
                            listPlazasPorEmpleadosMovOficial = (List<PlazasPorEmpleadosMov>)tmp[0];
                            listMovNomConcepPromocional = (List<MovNomConcep>)tmp[1];
                            // filtroMovimientosNominas = (List<MovNomConcep>)tmp[2];   //pendiente por remove

                            if (listPlazasPorEmpleadosMovOficial.Count > listCalculoUnidadesTmp.Count)
                            {
                                for (k = listCalculoUnidadesTmp.Count; k < listPlazasPorEmpleadosMovOficial.Count; k++)
                                {
                                    CalculoUnidades calculoUnidades = crearCalculoUnidades(listMovNomConcepPromocional[k].plazasPorEmpleado, tipoCorrida, razonesSociales);
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
                                    dbContextSimple.Set<CalculoUnidades>().Attach(listCalculoUnidadesTmp[listCalculoUnidadesTmp.Count - 1]);
                                    dbContextSimple.Set<CalculoUnidades>().Remove(listCalculoUnidadesTmp[listCalculoUnidadesTmp.Count - 1]);
                                    listCalculoUnidadesTmp.Remove(listCalculoUnidadesTmp[listCalculoUnidadesTmp.Count - 1]);
                                }

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
                        }
                        else
                        {
                            // break;
                        }

                    } // end (configuracionPercepcion_Plaza | configuracionPercepcion_Plaza_Vigente | configuracionSueldoDiarioVigente)
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
                            mensajeResultado = metodosParaMovimientosNomina.creaMovimBaseAfectar(movimientosNomina.concepNomDefi.baseAfecConcepNom, movimientosNomina, (DBContextSimple)dbContextSimple);
                            if (mensajeResultado.noError == 0)
                            {
                                movimientosNomina.movNomBaseAfecta = (List<MovNomBaseAfecta>)mensajeResultado.resultado;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if ((movimientosNomina.concepNomDefi.paraConcepDeNom == null ? 0 : movimientosNomina.concepNomDefi.paraConcepDeNom.Count)
                                != (movimientosNomina.movNomConceParam == null ? 0 : movimientosNomina.movNomConceParam.Count))
                        {
                            mensajeResultado = metodosParaMovimientosNomina.creaMovNomConceParam(movimientosNomina.concepNomDefi, movimientosNomina, (DBContextSimple)dbContextSimple);
                            if (mensajeResultado.noError == 0)
                            {
                                movimientosNomina.movNomConceParam = (List<MovNomConceParam>)mensajeResultado.resultado;
                            }
                            else
                            {
                                break;
                            }

                        }
                        #endregion

                        isMov2Meses = false;
                        tempPagoDiasNat = manejaPagoDiasNaturales;    // respalda parametro pago dias naturales
                        if (metodosParaMovimientosNomina.evaluaPeriodoMovAbarca2Meses(movimientosNomina.periodosNomina))
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
                                    tfc = (TipoClasificacionFormula)ManejadorEnum.GetValue(propertieFuente.GetProperty(String.Concat(key, "_TipoDato").ToUpper()), typeof(TipoClasificacionFormula));
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
                                            dbContextSimple.Set<CalculoUnidades>().Add(listCalculoUnidadesTmp[i]);
                                        }
                                        else
                                        {
                                            dbContextSimple.Set<CalculoUnidades>().AddOrUpdate(listCalculoUnidadesTmp[i]);
                                        }
                                    }
                                    else
                                    {
                                        cargaValoresDiasPago(listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov], false, null, listCalculoUnidadesTmp[i], false, true);//JSA30

                                        if (listCalculoUnidadesTmp[i].id == 0)
                                        {
                                            dbContextSimple.Set<CalculoUnidades>().Add(listCalculoUnidadesTmp[i]);
                                        }
                                        else
                                        {
                                            dbContextSimple.Set<CalculoUnidades>().AddOrUpdate(listCalculoUnidadesTmp[i]);
                                        }
                                    }
                                }
                                else
                                {
                                    cargaValoresDiasPago(listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov], true, null, listCalculoUnidadesTmp[i], false, true);//JSA30
                                    if (listCalculoUnidadesTmp[i].id == 0)
                                    {
                                        dbContextSimple.Set<CalculoUnidades>().Add(listCalculoUnidadesTmp[i]);
                                    }
                                    else
                                    {
                                        dbContextSimple.Set<CalculoUnidades>().AddOrUpdate(listCalculoUnidadesTmp[i]);
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
                                    dbContextSimple.Set<CalculoUnidades>().Add(listCalculoUnidadesTmp[i]);
                                }
                                else
                                {
                                    dbContextSimple.Set<CalculoUnidades>().AddOrUpdate(listCalculoUnidadesTmp[i]);
                                }

                            }  //isDosMeses
                            else
                            {
                                cargaValoresDiasPago(listPlazasPorEmpleadosMovOficial[indicePlazasPorEmpleadoMov], false, i + 1 < listPlazasPorEmpleadosMovOficial.Count ? listPlazasPorEmpleadosMovOficial[i + 1] : null, listCalculoUnidadesTmp[i], false, true);//JSA30
                                if (listCalculoUnidadesTmp[i].id == 0)
                                {
                                    dbContextSimple.Set<CalculoUnidades>().Add(listCalculoUnidadesTmp[i]);
                                }
                                else
                                {
                                    dbContextSimple.Set<CalculoUnidades>().AddOrUpdate(listCalculoUnidadesTmp[i]);
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
                                    dbContextSimple.Set<CalculoUnidades>().Add(listCalculoUnidadesTmp[indiceCalculoUnidad]);
                                }
                                else
                                {
                                    dbContextSimple.Set<CalculoUnidades>().AddOrUpdate(listCalculoUnidadesTmp[indiceCalculoUnidad]);
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
                                dbContextSimple.Set<CalculoUnidades>().Add(listCalculoUnidadesTmp[i]);
                            }
                            else
                            {
                                dbContextSimple.Set<CalculoUnidades>().AddOrUpdate(listCalculoUnidadesTmp[i]);
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
                    } /// end for listMovNomConcepPromocional.Count
                    if (mensajeResultado.noError != 0)
                    {
                        // break;
                    }
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
            valoresConceptosGlobales["AnioActualNumerico".ToUpper()] = periodo == null ? 0 : periodo.año.GetValueOrDefault();

            DateTime a = periodo.fechaAsistenciInicial.GetValueOrDefault();
            int mes = a.Month;
            String mesNomLar = a.ToString("MMMM");
            String mesNomCor = a.ToString("MMM");
            valoresConceptosGlobales["DiasMesNumerico".ToUpper()] = mes;
            valoresConceptosGlobales["MesAlfanumCompleto".ToUpper()] = mesNomLar;
            valoresConceptosGlobales["MesAlfanumCorto".ToUpper()] = mesNomCor;
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

        //remueve conceptos aguinaldo si no tiene fecha de aguinaldo
        private void removerConceptosAguinaldo(string claveTipoCorrida)
        {
            try
            {
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
                                        mensajeResultado = metodosParaMovimientosNomina.duplicarMovNomConcep(filtroMovimientosNominas[ag], filtroMovimientosNominas[ag].numero.GetValueOrDefault(), filtroMovimientosNominas[ag].plazasPorEmpleado, (DBContextSimple)dbContextSimple);
                                        if (mensajeResultado.noError == 0)
                                        {
                                            movDuplicado = (MovNomConcep)mensajeResultado.resultado;
                                        }
                                        else
                                        {
                                            break;
                                        }
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
                        metodosParaMovimientosNomina.deleteListQueryMov(typeof(MovNomConcep).Name, "id", idsMovDelete.ToArray(), null, null, null, true, dbContextAdapterSimple, dbContextAdapterMaestra);
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
                case 7:
                    if (formula.Contains("AjusteSubCausado"))
                    {
                        isConceptoEspecial = true;
                    }
                    else if (formula.Contains("AjusteIsrMes"))
                    {
                        isConceptoEspecial = true;
                    }
                    else if (formula.Contains("AjusteSubPagado"))
                    {
                        isConceptoEspecial = true;
                    }

                    break;
            }

            return isConceptoEspecial;
        }

        private void operacionConceptos(MovNomConcep movimientosNomina, string claveTipoCorrida, PlazasPorEmpleadosMov plazasPorEmpleadoMov, bool activarSave)
        {
            try
            {


                bool isISR = false, isIMSS = false, isISRSubsidio = false, isImssPatronal = false, omitirMovimiento = false, isISRACargo = false, isSubsEmpleoCalculado = false;
                bool isAjusteIsrMes = false;
                isISR = isConceptoEspecial(1, movimientosNomina.concepNomDefi.formulaConcepto);
                isIMSS = isConceptoEspecial(2, movimientosNomina.concepNomDefi.formulaConcepto);
                isISRSubsidio = isConceptoEspecial(3, movimientosNomina.concepNomDefi.formulaConcepto);
                isImssPatronal = isConceptoEspecial(4, movimientosNomina.concepNomDefi.formulaConcepto);
                isISRACargo = isConceptoEspecial(5, movimientosNomina.concepNomDefi.formulaConcepto);
                isSubsEmpleoCalculado = isConceptoEspecial(6, movimientosNomina.concepNomDefi.formulaConcepto);
                isAjusteIsrMes = isConceptoEspecial(7, movimientosNomina.concepNomDefi.formulaConcepto);
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
                        ConcepNomDefi con = (from cdn in dbContextSimple.Set<ConcepNomDefi>()
                                             join ctc in dbContextSimple.Set<ConceptoPorTipoCorrida>() on cdn.id equals ctc.concepNomDefi_ID
                                             where ctc.tipoCorrida.clave == claveTipoCorrida && cdn.activado == true && cdn.formulaConcepto.Contains("ISRSubsidio") &&
                                                 cdn.fecha == (from c in dbContextSimple.Set<ConcepNomDefi>()
                                                               join ct in dbContextSimple.Set<ConceptoPorTipoCorrida>() on c.id equals ct.concepNomDefi_ID
                                                               where ct.tipoCorrida.clave == claveTipoCorrida && c.formulaConcepto.Contains("ISRSubsidio")
                                                               select new { c.fecha }).Max(f => f.fecha)
                                             select cdn).SingleOrDefault();
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
                                mensajeResultado = metodosParaMovimientosNomina.creaMovNomConceptoSubsidio(movimientosNomina, con, periodosNomina, tipoCorrida, razonesSociales, centroDeCostoMovimiento, (DBContextSimple)dbContextSimple);
                                if (mensajeResultado.noError == 0)
                                {
                                    movNomConcepSubsidio = (MovNomConcep)mensajeResultado.resultado;
                                }
                                else
                                {
                                    return;
                                }
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
                        {//JSA12
                            movNomConcepSubsidio.resultado = 0.0;
                        }
                    }
                    else
                    {
                        movimientosNomina.resultado = 0.0;
                        if (movNomConcepSubsidio != null)
                        {//JSA12
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
                                //dbContextSimple.Set<MovNomConcep>().Attach(movNomConcepSubsidio);
                                //dbContextSimple.Set<MovNomConcep>().Remove(movNomConcepSubsidio);
                                eliminarMovimientosNominaBasura(new decimal[] { movNomConcepSubsidio.id });
                                //  metodosParaMovimientosNomina.eliminarMovimientosNominaBasura(new object[] { movNomConcepSubsidio.id }, dbContextAdapterSimple);
                                if (mensajeResultado.noError == 0)
                                {
                                    dbContextSimple.SaveChanges();
                                }
                                iSRRetenidoSubsidio = null;
                            }
                        }
                        else
                        {
                            if (movNomConcepSubsidio.id == 0)
                            {
                                cantidadSaveUpdate++;
                                dbContextSimple.Set<MovNomConcep>().AddOrUpdate(movNomConcepSubsidio);

                            }
                            else
                            {

                                dbContextSimple.Set<MovNomConcep>().AddOrUpdate(movNomConcepSubsidio);
                            }

                            iSRRetenidoSubsidio.movNomConcep = movNomConcepSubsidio;
                            dbContextSimple.Set<CalculoISR>().AddOrUpdate(iSRRetenidoSubsidio);
                            dbContextSimple.SaveChanges();
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
                            eliminarMovimientosNominaBasura(new decimal[] { movimientosNomina.id });
                            // metodosParaMovimientosNomina.eliminarMovimientosNominaBasura(new object[] { movimientosNomina.id }, dbContextAdapterSimple);//pendiente para la conexion
                            //dbContextSimple.SaveChanges();
                        }
                    }
                    else
                    {
                        if (movimientosNomina.id == 0)
                        {
                            cantidadSaveUpdate++;
                            dbContextSimple.Set<MovNomConcep>().AddOrUpdate(movimientosNomina);
                        }
                        iSRRetenido.movNomConcep = movimientosNomina;
                        cantidadSaveUpdate++;
                        dbContextSimple.Set<CalculoISR>().AddOrUpdate(iSRRetenido);
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
                    if (movimientosNomina.resultado > 0)
                    {
                        if (movimientosNomina.id == 0)
                        {
                            cantidadSaveUpdate++;
                            movimientosNomina.empleado_ID = movimientosNomina.empleados.id;
                            movimientosNomina.empleados = null;
                            movimientosNomina.centroDeCosto = null;
                            movimientosNomina.concepNomDefi = null;
                            movimientosNomina.periodosNomina = null;
                            movimientosNomina.plazasPorEmpleado = null;
                            movimientosNomina.razonesSociales = null;
                            movimientosNomina.tipoCorrida = null;
                            movimientosNomina.tipoNomina = null;
                            dbContextSimple.Set<MovNomConcep>().AddOrUpdate(movimientosNomina);
                            dbContextSimple.SaveChanges();

                        }

                        for (int i = 0; i < listCalculoIMSS.Count(); i++)
                        {
                            listCalculoIMSS[i].configuracionIMSS_ID = configuracionIMSS.id;
                            listCalculoIMSS[i].configuracionIMSS = null;
                            listCalculoIMSS[i].movNomConcep_ID = movimientosNomina.id;
                            listCalculoIMSS[i].movNomConcep = null;
                            cantidadSaveUpdate++;

                            dbContextSimple.Set<CalculoIMSS>().AddOrUpdate(listCalculoIMSS[i]);
                            dbContextSimple.SaveChanges();

                        }
                    }
                    omitirMovimiento = true;
                    if (movimientosNomina.resultado == null ? true : movimientosNomina.resultado == 0 ? true : false)
                    {
                        //omitirMovimiento = true;
                        if (movimientosNomina.id > 0)
                        {
                            dbContextSimple.Database.ExecuteSqlCommand("delete o from CalculoIMSS AS o  where o.movNomConcep_ID= @valores", new SqlParameter("@valores", movimientosNomina.id));
                            dbContextSimple.SaveChanges();
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
                        dbContextSimple.Set<MovNomConcep>().AddOrUpdate(movimientosNomina);
                    }
                    calculoIMSSPatron.movNomConcep = movimientosNomina;
                    cantidadSaveUpdate++;
                    dbContextSimple.Set<CalculoIMSSPatron>().AddOrUpdate(calculoIMSSPatron);
                    omitirMovimiento = true;
                    if (movimientosNomina.resultado == null ? true : movimientosNomina.resultado == 0 ? true : false)
                    {
                        if (movimientosNomina.id > 0)
                        {
                            dbContextSimple.Database.ExecuteSqlCommand("delete from CalculoIMSSPatron o  where o.movNomConcep.id= @valores", new SqlParameter(" @valores", movimientosNomina.id));
                            dbContextSimple.SaveChanges();
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
                else if (isAjusteIsrMes && periodosNomina.cierreMes)
                {
                    ajusteISRalMes(movimientosNomina, acumuladoNormal);
                    movimientosNomina = ejecutarParametroCondicionYFormula(movimientosNomina, claveTipoCorrida);
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
        private Double calculaImssPatronal(Double salarioDiarioIntegrado, Double salarioMinimoDF, MovNomConcep movNominaImss, PlazasPorEmpleadosMov plazasPorEmpleadosMov)
        {
            Double calculoImss = 0.0;
            try
            {
                calculoIMSSPatron = (from imss in dbContextSimple.Set<CalculoIMSSPatron>()
                                     where imss.movNomConcep.id == movNominaImss.id && imss.configuracionIMSS.id == configuracionIMSS.id
                                     select imss).SingleOrDefault();
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
        private Double calculaImss(Double salarioDiarioIntegrado, Double salarioMinimoDF, MovNomConcep movNominaImss, PlazasPorEmpleadosMov plazasPorEmpleadosMov)
        {
            Double acumuladoIMSS = 0.0;
            List<SalariosIntegrados> listSalariosIntegrados = null;
            try
            {
                double valorEspecieEnfermeMater = 0.0, valorDineEnfermeMater = 0.0, valorGastosPension = 0.0, valorInvalidezVida = 0.0, valorCesantiaVejez = 0.0;
                listCalculoIMSS = (from imss in dbContextSimple.Set<CalculoIMSS>()
                                   where imss.movNomConcep.id == movNominaImss.id
                                   select imss).ToList();
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

                                dbContextSimple.Database.ExecuteSqlCommand("delete from CalculoIMSS o  where o.movNomConcep.id in(@valores)", new SqlParameter(" @valores", clavesMovEliminados));
                                dbContextSimple.SaveChanges();
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
        private List<SalariosIntegrados> obtenerAnteriorSalariosIntegrados(DateTime fechaInicial, String claveEmpleado, String claveRegistroPatronal, String claveRazonesSociales)
        {
            List<SalariosIntegrados> listSalariosIntegrados = null;
            try
            {
                listSalariosIntegrados = (from s in dbContextSimple.Set<SalariosIntegrados>()
                                          where s.fecha < fechaInicial && s.empleados.clave == claveEmpleado
                                          && s.registroPatronal.clave == claveRegistroPatronal && s.empleados.razonesSociales.clave == claveRazonesSociales
                                          orderby s.fecha descending
                                          select s).ToList();
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
                listSalariosIntegrados = (from s in dbContextSimple.Set<SalariosIntegrados>()
                                          where s.fecha >= fechaInicial && s.fecha <= fechaFinal && s.empleados.clave == claveEmpleado
                                          && s.registroPatronal.clave == claveRegistroPatronal && s.empleados.razonesSociales.clave == claveRazonesSociales
                                          select s).ToList();
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
        private SalariosIntegrados obtenerSalariosIntegradosActual(DateTime fechaInicial, String claveEmpleado, String claveRegistroPatronal, String claveRazonesSociales)
        {
            SalariosIntegrados s = null;
            try
            {
                s = (from ss in dbContextSimple.Set<SalariosIntegrados>()
                     where ss.fecha <= fechaInicial && ss.empleados.clave == claveEmpleado
                     && ss.registroPatronal.clave == claveRegistroPatronal && ss.empleados.razonesSociales.clave == claveRazonesSociales
                     orderby ss.fecha descending
                     select ss).SingleOrDefault();
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
        public MovNomConcep saveOrUpdateOrDeleteMovimientosNomina(MovNomConcep movimientosNomina, bool omitirMovimiento, bool isISRSubsidio)
        {
            try
            {
                if (movimientosNomina.resultado == 0 & !isISRSubsidio)
                {
                    if (movimientosNomina.id > 0)
                    {
                        eliminarMovimientosNominaBasura(new decimal[] { movimientosNomina.id });
                        //metodosParaMovimientosNomina.eliminarMovimientosNominaBasura(new object[] { movimientosNomina.id }, dbContextAdapterSimple);

                    }
                }
                else
                {

                    if (!omitirMovimiento)
                    {
                        cantidadSaveUpdate++;

                        movimientosNomina.empleado_ID = movimientosNomina.empleados.id;
                        if (movimientosNomina.id == 0)
                        {
                            movimientosNomina.empleados = null;
                            movimientosNomina.centroDeCosto = null;
                            movimientosNomina.concepNomDefi = null;
                            movimientosNomina.periodosNomina = null;
                            movimientosNomina.plazasPorEmpleado = null;
                            movimientosNomina.razonesSociales = null;
                            movimientosNomina.tipoCorrida = null;
                            movimientosNomina.tipoNomina = null;
                        }

                        dbContextSimple.Set<MovNomConcep>().AddOrUpdate(movimientosNomina);
                        dbContextSimple.SaveChanges();
                    }
                    if (cantidadSaveUpdate % cantidadFlush == 0 && cantidadSaveUpdate > 0)
                    {

                        dbContextSimple.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveOrUpdateOrDeleteMovimientosNomina()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;

            }
            return movimientosNomina;
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
        private void calculaConceptosBaseAfecta(List<MovNomBaseAfecta> afecConcepNominas, Double resultadoConcepto)
        {
            int Base;
            double calculo;
            try
            {
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
                                        if (movNominaBaseAfecta.movNomConcep.concepNomDefi.naturaleza == Naturaleza.PERCEPCION)
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
                                        else if (movNominaBaseAfecta.movNomConcep.concepNomDefi.naturaleza == Naturaleza.DEDUCCION)
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
                                        if (movNominaBaseAfecta.movNomConcep.concepNomDefi.naturaleza == Naturaleza.PERCEPCION)
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
                                        else if (movNominaBaseAfecta.movNomConcep.concepNomDefi.naturaleza == Naturaleza.DEDUCCION)
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
                                        if (movNominaBaseAfecta.movNomConcep.concepNomDefi.naturaleza == Naturaleza.PERCEPCION)
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
                                        else if (movNominaBaseAfecta.movNomConcep.concepNomDefi.naturaleza == Naturaleza.DEDUCCION)
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

                            dbContextSimple.Database.ExecuteSqlCommand("Update mov Set mov.resultado = @resultado from MovNomBaseAfecta as mov  Where mov.id = @id", new SqlParameter("@resultado", movNominaBaseAfecta.resultado), new SqlParameter("@id", movNominaBaseAfecta.id));
                            //ejecutaQueryExecuteUpdate("Update MovNomBaseAfecta mov Set mov.resultado = :resultado Where mov.id = :id", new String[] { "resultado", "id" }, new Object[] { movNominaBaseAfecta.getResultado(), movNominaBaseAfecta.getId() });
                            if (mensajeResultado.noError == -101)
                            {
                                mensajeResultado.noError = 54;
                            }
                        }
                    }
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
                                    int diasVaca = Convert.ToInt32(valoresConceptosEmpleados["diasVacaciones".ToUpper()]);
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
                                tfc = (TipoClasificacionFormula)ManejadorEnum.getEnum(propertieFuente.GetProperty(string.Concat(variable, "_TipoDato")), typeof(TipoClasificacionFormula));
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

        #region valores xml

        private void cargaValoresXMLtoTabla(TablaDatos datos)
        {
            if (datos.fileXml != null)
            {
                docXML = UtilidadesXML.convierteBytesToXML(datos.fileXml);
                if (UtilidadesXML.ERROR_XML != 0)
                {
                    mensajeResultado = UtilidadesXML.mensajeError;
                    return;
                }
                cargaValoresXML();
            }
        }
        private DateTime quitaHrsDeFecha(DateTime fecha)
        {
            //Calendar myCal = CultureInfo.InvariantCulture.Calendar;
            //fecha = myCal.AddHours(fecha, 0);
            //fecha = myCal.AddMinutes(fecha, 0);
            //fecha = myCal.AddSeconds(fecha, 0);
            //fecha = myCal.AddMilliseconds(fecha, 0);
            fecha = new DateTime(fecha.Year, fecha.Month, fecha.Day);
            return fecha;
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

        #endregion

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
                    object[][] vacaciones = (from va in dbContextSimple.Set<VacacionesAplicacion>()
                                             where va.vacacionesDisfrutadas.periodoAplicacion.id == (decimal)valoresConceptosEmpleados["IdPeriodo".ToUpper()] && va.vacacionesDisfrutadas.razonesSociales.clave == valoresConceptosEmpleados["RazonSocial".ToUpper()].ToString() && va.vacacionesDisfrutadas.empleados.clave == valoresConceptosEmpleados["NumEmpleado".ToUpper()].ToString()
                                             select new object[] {
                                                va.vacacionesDisfrutadas.salidaVacac,
                                                va.vacacionesDisfrutadas.regresoVac,
                                                va.diasVac,
                                                va.diasPrima
                                            }).ToArray();

                    if (vacaciones == null)
                    {
                        valoresConceptosEmpleados.Add("FechaSalidaVacacion".ToUpper(), null);
                        valoresConceptosEmpleados.Add("FechaRegresoVacacion".ToUpper(), null);
                        valoresConceptosEmpleados.Add("DiasVacaciones".ToUpper(), 0);
                        valoresConceptosEmpleados.Add("DiasPrima".ToUpper(), 0.0);
                    }
                    else
                    {
                        valoresConceptosEmpleados.Add("FechaSalidaVacacion".ToUpper(), (DateTime)vacaciones[0][0]);
                        valoresConceptosEmpleados.Add("FechaRegresoVacacion".ToUpper(), (DateTime)vacaciones[0][1]);
                        valoresConceptosEmpleados.Add("DiasVacaciones".ToUpper(), Convert.ToDouble(vacaciones[0][2]));
                        valoresConceptosEmpleados.Add("DiasPrima".ToUpper(), (double)vacaciones[0][3]);
                    }
                }
                else if (string.Equals(variable, "TipoVacaciones".ToUpper(), StringComparison.OrdinalIgnoreCase))
                {
                    resultado = (from va in dbContextSimple.Set<VacacionesAplicacion>()
                                 where va.vacacionesDisfrutadas.empleados.clave == valoresConceptosEmpleados["NumEmpleado".ToUpper()].ToString() && va.vacacionesDisfrutadas.razonesSociales.clave == valoresConceptosEmpleados["RazonSocial".ToUpper()].ToString() && va.vacacionesDisfrutadas.periodoAplicacion.id == (decimal)valoresConceptosEmpleados["IdPeriodo".ToUpper()]
                                 select va.vacacionesDisfrutadas.tiposVacaciones.nombre).SingleOrDefault();
                    valoresConceptosEmpleados.Add(variable, resultado);
                }
                else if (string.Equals(variable, "TotalDiasVacaciones".ToUpper(), StringComparison.OrdinalIgnoreCase))
                {
                    var totalDiasVacaciones = (from vd in dbContextSimple.Set<VacacionesDevengadas>()
                                               where vd.empleados.clave == valoresConceptosEmpleados["NumEmpleado".ToUpper()].ToString() && vd.razonesSociales.clave == valoresConceptosEmpleados["RazonSocial".ToUpper()].ToString()
                                               select new { vd.diasVacaciones });

                    int? diasVacaciones = 0;
                    if (totalDiasVacaciones.Count() > 0)
                    {
                        diasVacaciones = totalDiasVacaciones.Sum(s => s.diasVacaciones);
                    }
                    resultado = diasVacaciones;
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
                        resultado = valor;
                    }
                    if (calcula)
                    {
                        resultado = (from va in dbContextSimple.Set<VacacionesAplicacion>()
                                     where va.vacacionesDisfrutadas.empleados.clave == valoresConceptosEmpleados["NumEmpleado".ToUpper()].ToString() && va.vacacionesDisfrutadas.razonesSociales.clave == valoresConceptosEmpleados["RazonSocial".ToUpper()].ToString() && va.vacacionesDisfrutadas.periodoAplicacion.id == (decimal)valoresConceptosEmpleados["IdPeriodo".ToUpper()]
                                     select va.diasVac).SingleOrDefault();
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
                        resultado = valor;
                    }
                    if (calcula)
                    {
                        resultado = (from va in dbContextSimple.Set<VacacionesAplicacion>()
                                     where va.vacacionesDisfrutadas.empleados.clave == valoresConceptosEmpleados["NumEmpleado".ToUpper()].ToString() && va.vacacionesDisfrutadas.razonesSociales.clave == valoresConceptosEmpleados["RazonSocial".ToUpper()].ToString() && va.vacacionesDisfrutadas.periodoAplicacion.id == (decimal)valoresConceptosEmpleados["IdPeriodo".ToUpper()]
                                     select va.diasPrima).SingleOrDefault();
                        valoresConceptosEmpleados.Add(variable, resultado);
                    }
                }
                else if (string.Equals(variable, "DiasVacacionesPendientes".ToUpper(), StringComparison.OrdinalIgnoreCase))
                {
                    var resDiasVac = (from va in dbContextSimple.Set<VacacionesAplicacion>()
                                      where va.vacacionesDisfrutadas.empleados.clave == valoresConceptosEmpleados["NumEmpleado".ToUpper()].ToString() && va.vacacionesDisfrutadas.razonesSociales.clave == valoresConceptosEmpleados["RazonSocial".ToUpper()].ToString()
                                      select new { va.diasVac });

                    int? diasVac = 0;
                    if (resDiasVac.Count() > 0)
                    {
                        diasVac = resDiasVac.Sum(s => s.diasVac);
                    }

                    var resDiasVacacaciones = (from vd in dbContextSimple.Set<VacacionesDevengadas>()
                                               where vd.empleados.clave == valoresConceptosEmpleados["NumEmpleado".ToUpper()].ToString() && vd.razonesSociales.clave == valoresConceptosEmpleados["RazonSocial".ToUpper()].ToString()
                                               select new { vd.diasVacaciones });

                    int? diasVacaciones = 0;
                    if (resDiasVacacaciones.Count() > 0)
                    {
                        diasVacaciones = resDiasVacacaciones.Sum(s => s.diasVacaciones);
                    }

                    resultado = diasVacaciones - diasVac;
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
        enum TipoMostrarCampo
        {

            SUMA, COUNT, NORMAL, OPERACION
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
                            tipoDato = Utilerias.buscarTipoDatoCampo(propertieFuente.GetProperty(string.Concat(datosExtras[i], "_Path")));
                            valoresExtras.Add(Utilerias.castStringTo(tipoDato.GetType().Name, valores[i]));
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
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id && mov.concepNomDefi.naturaleza == Naturaleza.PERCEPCION
                                                 && mba.baseAfecConcepNom.tipoAfecta == Convert.ToInt32(ClavesParametrosModulos.tipoBaseAfectaISRNormal)
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaISR.ToString()
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);

                var resul = from mov in query
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where mov.id == mba.movNomConcep.id
                            select new { total = mba.resultado };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.total);
                }
                valoresConceptosEmpleados.Add(variable, resultado);



            }
            else if (string.Equals(variable, "PercepcionesGravablesDirTabla", StringComparison.OrdinalIgnoreCase))
            {
                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id && mov.concepNomDefi.naturaleza == Naturaleza.PERCEPCION
                                                 && mba.baseAfecConcepNom.tipoAfecta == Convert.ToInt32(ClavesParametrosModulos.tipoBaseAfectaISRDirecto)
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaISR.ToString()
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);

                var resul = from mov in query
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where mov.id == mba.movNomConcep.id
                            select new { total = mba.resultado };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.total);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "PercepcionesGravablesAnual", StringComparison.OrdinalIgnoreCase))
            {
                resultado = 0;

                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id && mov.concepNomDefi.naturaleza == Naturaleza.PERCEPCION
                                                 && mba.baseAfecConcepNom.tipoAfecta == Convert.ToInt32(ClavesParametrosModulos.tipoBaseAfectaISRAnual)
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaISR.ToString()
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from mov in query
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where mov.id == mba.movNomConcep.id
                            select new { total = mba.resultado };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.total);
                }

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
                resultado = isrAcumuladoPorRangoMeses(tipoAcumulado, fecha, "isrACargo", CamposAMostrar.isrACargo);
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "ISRSubsidioPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "ISRSubsidioMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "ISRSubsidioBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "ISRSubsidioAnual", StringComparison.OrdinalIgnoreCase))
            {
                resultado = isrAcumuladoPorRangoMeses(tipoAcumulado, fecha, "isrSubsidio", CamposAMostrar.isrSubsidio);
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "ISRNetoPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "ISRSubsidioMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "ISRSubsidioBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "ISRNetoAnual", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "ISRTotal", StringComparison.OrdinalIgnoreCase))
            {
                resultado = isrAcumuladoPorRangoMeses(tipoAcumulado, fecha, "isrNeto", CamposAMostrar.isrNeto);
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "TotalPercepcionesPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "TotalPercepcionesMes", StringComparison.OrdinalIgnoreCase)
      || string.Equals(variable, "TotalPercepcionesBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "TotalPercepcionesAnual", StringComparison.OrdinalIgnoreCase))
            {
                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id && mov.concepNomDefi.naturaleza == Naturaleza.PERCEPCION
                                                && !mov.concepNomDefi.formulaConcepto.Contains("TotalPercepciones")
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from mov in query
                            select new { total = mov.resultado };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.total);
                }
                valoresConceptosEmpleados.Add(variable, resultado);

            }
            else if (string.Equals(variable, "TotalDeduccionesPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "TotalDeduccionesMes", StringComparison.OrdinalIgnoreCase)
     || string.Equals(variable, "TotalDeduccionesBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "TotalDeduccionesAnual", StringComparison.OrdinalIgnoreCase))
            {
                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id && mov.concepNomDefi.naturaleza == Naturaleza.DEDUCCION
                                                && !mov.concepNomDefi.formulaConcepto.Contains("TotalDeducciones")
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from mov in query
                            select new { total = mov.resultado };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.total);
                }
                valoresConceptosEmpleados.Add(variable, resultado);

            }
            else if (string.Equals(variable, "BaseISRPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRMes", StringComparison.OrdinalIgnoreCase)
    || string.Equals(variable, "BaseISRBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRAnual", StringComparison.OrdinalIgnoreCase))
            {
                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaISR.ToString()
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado + mba.resultadoExento };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma.Value);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseISRNormalPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRNormalMes", StringComparison.OrdinalIgnoreCase)
   || string.Equals(variable, "BaseISRNormalBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRNormalAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaISR.ToString()
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado + mba.resultadoExento };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma.Value);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseISRDirectoPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRDirectoMes", StringComparison.OrdinalIgnoreCase)
  || string.Equals(variable, "BaseISRDirectoBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRDirectoAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaISR.ToString()
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado + mba.resultadoExento };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma.Value);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseISRAnualPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRAnualMes", StringComparison.OrdinalIgnoreCase)
          || string.Equals(variable, "BaseISRAnualBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRAnualAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaISR.ToString()
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado + mba.resultadoExento };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma.Value);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseISRGravablePeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRGravableMes", StringComparison.OrdinalIgnoreCase)
        || string.Equals(variable, "BaseISRGravableBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRGravableAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaISR.ToString()
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseISRGravableNormalPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRGravableNormalMes", StringComparison.OrdinalIgnoreCase)
         || string.Equals(variable, "BaseISRGravableNormalBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRGravableNormalAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaISR.ToString()
                                                 && mba.baseAfecConcepNom.tipoAfecta == Convert.ToInt32(ClavesParametrosModulos.tipoBaseAfectaISRNormal)
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseISRGravableDirectoPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRGravableDirectoMes", StringComparison.OrdinalIgnoreCase)
       || string.Equals(variable, "BaseISRGravableDirectoBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRGravableDirectoAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaISR.ToString()
                                                 && mba.baseAfecConcepNom.tipoAfecta == Convert.ToInt32(ClavesParametrosModulos.tipoBaseAfectaISRDirecto)
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseISRGravableAnualPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRGravableAnualMes", StringComparison.OrdinalIgnoreCase)
     || string.Equals(variable, "BaseISRGravableAnualBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRGravableAnualAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaISR.ToString()
                                                 && mba.baseAfecConcepNom.tipoAfecta == Convert.ToInt32(ClavesParametrosModulos.tipoBaseAfectaISRAnual)
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseISRExentoPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRExentoMes", StringComparison.OrdinalIgnoreCase)
  || string.Equals(variable, "BaseISRExentoBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRExentoAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaISR.ToString()
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultadoExento };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseISRExentoNormalPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRExentoNormalMes", StringComparison.OrdinalIgnoreCase)
|| string.Equals(variable, "BaseISRExentoNormalBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRExentoNormalAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaISR.ToString()
                                                  && mba.baseAfecConcepNom.tipoAfecta == Convert.ToInt32(ClavesParametrosModulos.tipoBaseAfectaISRNormal)
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultadoExento };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseISRExentoDirectoPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRExentoDirectoMes", StringComparison.OrdinalIgnoreCase)
|| string.Equals(variable, "BaseISRExentoDirectoBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRExentoDirectoAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaISR.ToString()
                                                  && mba.baseAfecConcepNom.tipoAfecta == Convert.ToInt32(ClavesParametrosModulos.tipoBaseAfectaISRDirecto)
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultadoExento };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseISRExentoAnualPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRExentoAnualMes", StringComparison.OrdinalIgnoreCase)
|| string.Equals(variable, "BaseISRExentoAnualBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseISRExentoAnualAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaISR.ToString()
                                                  && mba.baseAfecConcepNom.tipoAfecta == Convert.ToInt32(ClavesParametrosModulos.tipoBaseAfectaISRAnual)
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultadoExento };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseIMSSPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSMes", StringComparison.OrdinalIgnoreCase)
         || string.Equals(variable, "BaseIMSSBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaISR.ToString()
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado + mba.resultadoExento };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma.Value);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseIMSSFijaPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSFijaMes", StringComparison.OrdinalIgnoreCase)
       || string.Equals(variable, "BaseIMSSFijaBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSFijaAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaISR.ToString()
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado + mba.resultadoExento };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma.Value);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseIMSSVariablePeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSVariableMes", StringComparison.OrdinalIgnoreCase)
     || string.Equals(variable, "BaseIMSSVariableBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSVariableAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaISR.ToString()
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado + mba.resultadoExento };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma.Value);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseIMSSGravadoPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSGravableMes", StringComparison.OrdinalIgnoreCase)
|| string.Equals(variable, "BaseIMSSGravableBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSGravableAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaIMSS.ToString()

                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseIMSSGravadoFijaPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSGravableFijaMes", StringComparison.OrdinalIgnoreCase)
|| string.Equals(variable, "BaseIMSSGravableFijaBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSGravableFijaAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaIMSS.ToString()
                                                 && mba.baseAfecConcepNom.tipoAfecta == Convert.ToInt32(ClavesParametrosModulos.tipoBaseAfectaIMSSFijo)
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseIMSSGravadoVariablePeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSGravableVariableMes", StringComparison.OrdinalIgnoreCase)
|| string.Equals(variable, "BaseIMSSGravableVariableBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSGravableVariableAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaIMSS.ToString()
                                                 && mba.baseAfecConcepNom.tipoAfecta == Convert.ToInt32(ClavesParametrosModulos.tipoBaseAfectaIMSSVariable)
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseIMSSExentoPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSExentoMes", StringComparison.OrdinalIgnoreCase)
|| string.Equals(variable, "BaseIMSSExentoBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSExentoAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaIMSS.ToString()

                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultadoExento };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseIMSSExentoFijaPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSExentoFijaMes", StringComparison.OrdinalIgnoreCase)
|| string.Equals(variable, "BaseIMSSExentoFijaBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSExentoFijaAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaIMSS.ToString()
                                                 && mba.baseAfecConcepNom.tipoAfecta == Convert.ToInt32(ClavesParametrosModulos.tipoBaseAfectaIMSSFijo)
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultadoExento };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseIMSSExentoVariablePeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSExentoVariableMes", StringComparison.OrdinalIgnoreCase)
|| string.Equals(variable, "BaseIMSSExentoVariableBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseIMSSExentoVariableAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaIMSS.ToString()
                                                 && mba.baseAfecConcepNom.tipoAfecta == Convert.ToInt32(ClavesParametrosModulos.tipoBaseAfectaIMSSVariable)
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultadoExento };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseInfonavitPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseInfonavitMes", StringComparison.OrdinalIgnoreCase)
|| string.Equals(variable, "BaseInfonavitBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseInfonavitAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaInfonavit.ToString()

                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BasePTUPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BasePTUMes", StringComparison.OrdinalIgnoreCase)
|| string.Equals(variable, "BasePTUBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BasePTUAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaPTU.ToString()

                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseImpuestoNominaPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseImpuestoNominaMes", StringComparison.OrdinalIgnoreCase)
|| string.Equals(variable, "BaseImpuestoNominaBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseImpuestoNominaAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaISN.ToString()

                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseDespensaPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseDespensaMes", StringComparison.OrdinalIgnoreCase)
|| string.Equals(variable, "BaseDespensaBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseDespensaAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaDespensa.ToString()

                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseFondoAhorroPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseFondoAhorroMes", StringComparison.OrdinalIgnoreCase)
|| string.Equals(variable, "BaseFondoAhorroBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseFondoAhorroAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaFondoAhorro.ToString()

                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseAguinaldoPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseAguinaldoMes", StringComparison.OrdinalIgnoreCase)
|| string.Equals(variable, "BaseAguinaldoBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseAguinaldoAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaAguinaldo.ToString()

                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "BaseOtrosPeriodo", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseOtrosMes", StringComparison.OrdinalIgnoreCase)
|| string.Equals(variable, "BaseOtrosBim", StringComparison.OrdinalIgnoreCase) || string.Equals(variable, "BaseOtrosAnual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mov.id == mba.movNomConcep.id
                                                 && mba.baseAfecConcepNom.baseNomina.reservado == false

                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "PercepcionesExentas", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 where mov.concepNomDefi.naturaleza == Naturaleza.PERCEPCION
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultadoExento };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "DeduccionesGravables", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 where mov.concepNomDefi.naturaleza == Naturaleza.DEDUCCION
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "DeduccionesExentas", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 where mov.concepNomDefi.naturaleza == Naturaleza.DEDUCCION
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultadoExento };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "PercepcionesGravadasPer", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 where mov.concepNomDefi.naturaleza == Naturaleza.PERCEPCION
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "PercepcionesGravadasMesActual", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 where mov.concepNomDefi.naturaleza == Naturaleza.PERCEPCION
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "PercepcionesGravadasMesAnterior", StringComparison.OrdinalIgnoreCase))
            {
                clavePeriodoFuncion = "Anterior";
                fecha = new DateTime(fecha.Year, fecha.Month - 1, fecha.Day);
                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 where mov.concepNomDefi.naturaleza == Naturaleza.PERCEPCION
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "PercepcionesExentaPer", StringComparison.OrdinalIgnoreCase))
            {
                //clavePeriodoFuncion = "Anterior";
                //fecha = new DateTime(fecha.Year, fecha.Month - 1, fecha.Day);
                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 where mov.concepNomDefi.naturaleza == Naturaleza.PERCEPCION
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultadoExento };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "PercepcionesExentaMesActual", StringComparison.OrdinalIgnoreCase))
            {
                //clavePeriodoFuncion = "Anterior";
                //fecha = new DateTime(fecha.Year, fecha.Month - 1, fecha.Day);
                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 where mov.concepNomDefi.naturaleza == Naturaleza.PERCEPCION
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultadoExento };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "PercepcionesExentaMesAnterior", StringComparison.OrdinalIgnoreCase))
            {
                clavePeriodoFuncion = "Anterior";
                fecha = new DateTime(fecha.Year, fecha.Month - 1, fecha.Day);
                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 where mov.concepNomDefi.naturaleza == Naturaleza.PERCEPCION
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultadoExento };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "DeduccionesGravadasPer", StringComparison.OrdinalIgnoreCase))
            {
                //clavePeriodoFuncion = "Anterior";
                //fecha = new DateTime(fecha.Year, fecha.Month - 1, fecha.Day);
                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 where mov.concepNomDefi.naturaleza == Naturaleza.DEDUCCION
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "DeduccionesGravadasMesActual", StringComparison.OrdinalIgnoreCase))
            {
                //clavePeriodoFuncion = "Anterior";
                //fecha = new DateTime(fecha.Year, fecha.Month - 1, fecha.Day);
                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 where mov.concepNomDefi.naturaleza == Naturaleza.DEDUCCION
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "DeduccionesGravadasMesAnterior", StringComparison.OrdinalIgnoreCase))
            {
                clavePeriodoFuncion = "Anterior";
                fecha = new DateTime(fecha.Year, fecha.Month - 1, fecha.Day);
                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 where mov.concepNomDefi.naturaleza == Naturaleza.DEDUCCION
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "DeduccionesExentasPer", StringComparison.OrdinalIgnoreCase))
            {
                //clavePeriodoFuncion = "Anterior";
                //fecha = new DateTime(fecha.Year, fecha.Month - 1, fecha.Day);
                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 where mov.concepNomDefi.naturaleza == Naturaleza.DEDUCCION
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultadoExento };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "DeduccionesExentasMesActual", StringComparison.OrdinalIgnoreCase))
            {
                //clavePeriodoFuncion = "Anterior";
                //fecha = new DateTime(fecha.Year, fecha.Month - 1, fecha.Day);
                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 where mov.concepNomDefi.naturaleza == Naturaleza.DEDUCCION
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultadoExento };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "DeduccionesExentasMesAnterior", StringComparison.OrdinalIgnoreCase))
            {
                clavePeriodoFuncion = "Anterior";
                fecha = new DateTime(fecha.Year, fecha.Month - 1, fecha.Day);
                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 where mov.concepNomDefi.naturaleza == Naturaleza.DEDUCCION
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultadoExento };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "ISRNormal", StringComparison.OrdinalIgnoreCase))
            {
                resultado = isrAcumuladoPorRangoMeses(tipoAcumulado, fecha, "isrNetoNormal", CamposAMostrar.isrNetoNormal);
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "ISRAnual", StringComparison.OrdinalIgnoreCase))
            {
                resultado = isrAcumuladoPorRangoMeses(tipoAcumulado, fecha, "isrNetoAnual", CamposAMostrar.isrNetoNormal);
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "ISRDirectoTabla", StringComparison.OrdinalIgnoreCase))
            {
                resultado = isrAcumuladoPorRangoMeses(tipoAcumulado, fecha, "isrNetoDirecto", CamposAMostrar.isrNetoNormal);
                valoresConceptosEmpleados.Add(variable, resultado);
            }
            else if (string.Equals(variable, "PercepcionesGravablesNor", StringComparison.OrdinalIgnoreCase))
            {

                resultado = 0;
                IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>()
                                                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                 where mba.movNomConcep.id == mov.id && mov.concepNomDefi.naturaleza == Naturaleza.PERCEPCION
                                                 && mba.baseAfecConcepNom.tipoAfecta == Convert.ToInt32(ClavesParametrosModulos.tipoBaseAfectaISRNormal)
                                                 && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaISR.ToString()
                                                 select mov;
                IQueryable<MovNomConcep> res = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fecha, "", CamposAMostrar.MovNomBaseAfectaResultado, query, TipoMostrarCampo.SUMA, null, null);
                var resul = from a in res
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where a.id == mba.movNomConcep.id
                            select new { suma = mba.resultado };
                if (resul.Count() > 0)
                {
                    resultado = resul.Sum(p => p.suma);
                }
                valoresConceptosEmpleados.Add(variable, resultado);
            }

        }
        private Object isrAcumuladoPorRangoMeses(TipoClasificacionFormula tipoAcumulado, DateTime fechaPeriodoNomina, String campo, CamposAMostrar campos)
        {
            Object valor = 0.0;
            try
            {
                var query = from isr in dbContextSimple.Set<CalculoISR>()
                            join mov in dbContextSimple.Set<MovNomConcep>() on isr.movNomConcep.id equals mov.id
                            join em in dbContextSimple.Set<Empleados>() on mov.empleados.id equals em.id
                            join rs in dbContextSimple.Set<RazonesSociales>() on mov.razonesSociales.id equals rs.id
                            join con in dbContextSimple.Set<ConcepNomDefi>() on mov.concepNomDefi.id equals con.id
                            join tipoCorri in dbContextSimple.Set<TipoCorrida>() on mov.tipoCorrida.id equals tipoCorri.id
                            join fc in dbContextSimple.Set<FiniqLiquidCncNom>() on mov.finiqLiquidCncNom.id equals fc.id
                            join fl in dbContextSimple.Set<FiniquitosLiquida>() on fc.finiquitosLiquida.id equals fl.id
                            join p in dbContextSimple.Set<PeriodosNomina>() on mov.periodosNomina.id equals p.id
                            join t in dbContextSimple.Set<TipoNomina>() on mov.tipoNomina.id equals t.id
                            join cenc in dbContextSimple.Set<CentroDeCosto>() on mov.centroDeCosto.id equals cenc.id
                            //where em.clave== valoresConceptosEmpleados["NumEmpleado".ToUpper()].ToString() && rs.clave== valoresConceptosEmpleados["RazonSocial".ToUpper()].ToString()
                            select new { isr, mov, em, rs, con, tipoCorri, fc, fl, p, t, cenc };


                String cc = valoresConceptosEmpleados["CentroDeCosto".ToUpper()] == null ? "" : valoresConceptosEmpleados["CentroDeCosto".ToUpper()].ToString();
                if (cc.Trim().Length > 0)
                {
                    query = from isr in query
                            where isr.cenc.clave == cc
                            select isr;
                    // select new { isr.isr, isr.mov, isr.em, isr.rs, isr.con, isr.tipoCorri, isr.fc, isr.fl, isr.p, isr.t, isr.cenc };
                }
                if (valoresConceptosEmpleados.ContainsKey("TipoNomina".ToUpper()))
                {
                    String nomina = valoresConceptosEmpleados["TipoNomina".ToUpper()] == null ? "" : valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString();
                    if (nomina.Length > 0)
                    {
                        query = from isr in query
                                where isr.t.clave == nomina
                                select isr;
                    }

                }
                if (valoresConceptosEmpleados.ContainsKey("uso".ToUpper()))
                {
                    query = from isr in query
                            where isr.mov.uso == Convert.ToUInt32(valoresConceptosEmpleados["uso".ToUpper()])
                            select isr;
                }
                if (valoresConceptosEmpleados["ImprimeListado".ToUpper()] != null)
                {
                    query = from isr in query
                            where isr.con.imprimirEnListadoNomina == Convert.ToBoolean(valoresConceptosEmpleados["ImprimeListado".ToUpper()])
                            select isr;

                }
                if (valoresConceptosEmpleados["ImprimeRecibo".ToUpper()] != null)
                {
                    query = from isr in query
                            where isr.con.imprimirEnReciboNomina == Convert.ToBoolean(valoresConceptosEmpleados["ImprimeRecibo".ToUpper()])
                            select isr;

                }
                if (valoresConceptosEmpleados.ContainsKey("ClaveTipoCorrida".ToUpper()))
                {
                    String corrida = valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()] == null ? "" : valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()].ToString();
                    if (corrida.Length > 0)
                    {
                        query = from isr in query
                                where isr.tipoCorri.clave == corrida
                                select isr;
                    }
                }
                //falta los finiquitos

                DateTime fechaRango = DateTime.Now;
                fechaRango = fechaPeriodoNomina;
                int mesIni = -1, mesFin = -1;
                if (tipoAcumulado == TipoClasificacionFormula.DATOPERIODO)
                {
                    DateTime fecha = (DateTime)valoresConceptosEmpleados[parametroFechaFinal];
                    var subquery = from sub in dbContextSimple.Set<PeriodosNomina>()
                                   join nomina in dbContextSimple.Set<TipoNomina>() on sub.tipoNomina.id equals nomina.id
                                   where (fechaPeriodoNomina >= sub.fechaInicial && fechaPeriodoNomina <= sub.fechaFinal)
                                   && nomina.clave == valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString()
                                   select sub;
                    if (valoresConceptosEmpleados.ContainsKey("ClaveTipoCorrida".ToUpper()))
                    {
                        string corrida = valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()] == null ? "" : valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()].ToString();
                        if (corrida.Length > 0)
                        {
                            subquery = from sub in subquery
                                       where sub.tipoCorrida.clave == corrida
                                       select sub;
                        }
                    }
                    string clave = subquery.Select(p => p.clave).ToString();

                    query = from isr in query
                            where isr.p.clave == clave && isr.p.año == fecha.Year
                            select isr;

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
                    query = from isr in query
                            where (isr.p.fechaInicial >= fecha && isr.p.fechaInicial <= fechafinal) ||
                            (isr.p.fechaFinal >= fecha && isr.p.fechaFinal <= fechafinal)
                            select isr;

                }
                if (tipoAcumulado == TipoClasificacionFormula.DATOBIMESTRAL)
                {
                    DateTime fecha = (DateTime)valoresConceptosEmpleados[parametroFechaFinal];
                    query = from isr in query
                            where isr.p.año == fecha.Year && (isr.mov.mes == mesIni || isr.mov.mes == mesFin)
                            select isr;

                }
                else if (tipoAcumulado == TipoClasificacionFormula.DATOMENSUAL)
                {
                    DateTime fecha = (DateTime)valoresConceptosEmpleados[parametroFechaFinal];
                    query = from isr in query
                            where isr.p.año == fecha.Year && isr.mov.mes == fecha.Month + 1
                            select isr;

                }
                if (campos == CamposAMostrar.isrACargo)
                {
                    var res = from isr in query
                              select new { result = isr.isr.isrACargo };
                    if (res.Count() > 0)
                    {
                        valor = res.Sum(p => p.result);
                    }
                }
                else if (campos == CamposAMostrar.isrSubsidio)
                {
                    var res = from isr in query
                              select new { result = isr.isr.isrSubsidio };
                    if (res.Count() > 0)
                    {
                        valor = res.Sum(p => p.result);
                    }
                }
                else if (campos == CamposAMostrar.isrNeto)
                {
                    var res = from isr in query
                              select new { result = isr.isr.isrNeto };
                    if (res.Count() > 0)
                    {
                        valor = res.Sum(p => p.result);
                    }
                }
                else if (campos == CamposAMostrar.isrNetoNormal)
                {
                    var res = from isr in query
                              select new { result = isr.isr.isrNetoNormal };
                    if (res.Count() > 0)
                    {
                        valor = res.Sum(p => p.result);
                    }
                }
                else if (campos == CamposAMostrar.isrNetoAnual)
                {
                    var res = from isr in query
                              select new { result = isr.isr.isrNetoAnual };
                    if (res.Count() > 0)
                    {
                        valor = res.Sum(p => p.result);
                    }
                }
                else if (campos == CamposAMostrar.isrNetoDirecto)
                {
                    var res = from isr in query
                              select new { result = isr.isr.isrNetoDirecto };
                    if (res.Count() > 0)
                    {
                        valor = res.Sum(p => p.result);
                    }
                }

                if (mensajeResultado.noError == 100)
                {
                    mensajeResultado.noError = 62;
                }
            }
            catch (Exception ex)
            {
                mensajeResultado.noError = 62;
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("isrAcumuladoPorRangoMeses()1_Error: ").Append(ex));

            }
            return valor;

        }

        private Object asistenciasAcumuladoPorRangoMeses(TipoClasificacionFormula tipoAcumulado, DateTime fechaPeriodoNomina, Object[] claveExcepcion)
        {
            //esta pendiente lo de finiquito
            Object valor = 0.0;
            try
            {
                var query = from a in dbContextSimple.Set<Asistencias>()
                            join ex in dbContextSimple.Set<Excepciones>() on a.excepciones.id equals ex.id
                            join em in dbContextSimple.Set<Empleados>() on a.empleados.id equals em.id
                            join t in dbContextSimple.Set<TipoNomina>() on a.tipoNomina.id equals t.id
                            join rs in dbContextSimple.Set<RazonesSociales>() on a.razonesSociales.id equals rs.id
                            join p in dbContextSimple.Set<PeriodosNomina>() on a.periodosNomina.id equals p.id
                            where em.clave == valoresConceptosEmpleados["NumEmpleado".ToUpper()].ToString() && rs.clave == valoresConceptosEmpleados["RazonSocial".ToUpper()].ToString()
                            && t.clave == valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString() && p.tipoCorrida.clave == valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()].ToString() && claveExcepcion.ToString().Contains(ex.clave)
                            select a;
                string cc = valoresConceptosEmpleados["CentroDeCosto".ToUpper()] == null ? "" : valoresConceptosEmpleados["CentroDeCosto".ToUpper()].ToString();
                if (cc.Trim().Length > 0)
                {
                    query = from a in query
                            join c in dbContextSimple.Set<CentroDeCosto>() on a.centroDeCosto.id equals c.id
                            where c.clave == cc
                            select a;
                }
                DateTime fechaRango = DateTime.Now;
                fechaRango = fechaPeriodoNomina;
                int mesIni = -1, mesFin = -1;
                if (tipoAcumulado == TipoClasificacionFormula.DATOPERIODO)
                {
                    DateTime fechaIni = (DateTime)valoresConceptosEmpleados[parametroFechaInicial];
                    DateTime fechafin = (DateTime)valoresConceptosEmpleados[parametroFechaFinal];
                    query = from a in query
                            where (a.fecha >= fechaIni && a.fecha <= fechafin)
                            select a;
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
                    query = from a in query
                            where (a.fecha >= fechaIni && a.fecha <= fechafin)
                            select a;
                }
                if (tipoAcumulado == TipoClasificacionFormula.DATOBIMESTRAL || tipoAcumulado == TipoClasificacionFormula.DATOMENSUAL)
                {
                    DateTime fecha = (DateTime)valoresConceptosEmpleados[parametroFechaFinal];
                    if (fechaRango.Month == 1)
                    {
                        query = from a in query
                                where (a.periodosNomina.acumularAMes.GetValueOrDefault().Month <= mesFin && a.periodosNomina.acumularAMes.GetValueOrDefault().Year == fecha.Year)
                                || (a.tipoNomina.clave == valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString() && a.periodosNomina.año == fecha.Year - 1 &&
                                a.periodosNomina.acumularAMes.GetValueOrDefault().Month == mesIni && a.periodosNomina.acumularAMes.GetValueOrDefault().Year == fecha.Year)
                                select a;

                    }
                    else
                    {
                        query = from a in query
                                where (a.periodosNomina.acumularAMes.GetValueOrDefault().Month >= mesIni && a.periodosNomina.acumularAMes.GetValueOrDefault().Month <= mesFin &&
                                a.periodosNomina.acumularAMes.GetValueOrDefault().Year == fecha.Year)
                                select a;
                    }

                }
                valor = query.Count();
                if (mensajeResultado.noError == 100)
                {
                    mensajeResultado.noError = 62;
                }


            }
            catch (Exception ex)
            {
                mensajeResultado.noError = 62;
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("asistenciasAcumuladoPorRangoMeses()1_Error: ").Append(ex));

            }
            return valor;
        }
        private Object periodioAcumuladoPorRangoMeses(TipoClasificacionFormula tipoAcumulado, DateTime fechaPeriodoNomina, String variable)
        {
            //esta pendiente lo de finiquito
            Object valor = 0.0;
            try
            {
                var query = from p in dbContextSimple.Set<PeriodosNomina>()
                            join tn in dbContextSimple.Set<TipoNomina>() on p.tipoNomina.id equals tn.id
                            where tn.clave == valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString() && p.año == fechaPeriodoNomina.Year
                            select p;
                DateTime fechaRango = DateTime.Now;
                fechaRango = fechaPeriodoNomina;
                int mesIni = -1, mesFin = -1;
                if (tipoAcumulado == TipoClasificacionFormula.DATOPERIODO)
                {
                    string clavep = (from p in dbContextSimple.Set<PeriodosNomina>()
                                     join tn in dbContextSimple.Set<TipoNomina>() on p.tipoNomina.id equals tn.id
                                     where (fechaPeriodoNomina >= p.fechaInicial && fechaPeriodoNomina <= p.fechaFinal) && tn.clave == valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString()
                                     select p.clave).ToString();
                    query = from p in query
                            where p.clave == clavep
                            select p;

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
                    query = from p in query
                            where p.fechaInicial >= fecha && p.fechaFinal <= fechafinal || (fechafinal >= p.fechaInicial && fechafinal <= p.fechaFinal && p.tipoNomina.clave == valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString() && p.año == fechaPeriodoNomina.Year)
                            select p;

                }
                if (tipoAcumulado == TipoClasificacionFormula.DATOBIMESTRAL || tipoAcumulado == TipoClasificacionFormula.DATOMENSUAL)
                {
                    DateTime fecha = (DateTime)valoresConceptosGlobales[parametroFechaFinal];
                    if (fechaRango.Month == 1)
                    {
                        query = from p in query
                                where (p.acumularAMes.GetValueOrDefault().Month <= mesFin && p.acumularAMes.GetValueOrDefault().Year == fecha.Year)
                                || (p.tipoNomina.clave == valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString() && p.año == fecha.Year - 1 &&
                                p.acumularAMes.GetValueOrDefault().Month == mesIni && p.acumularAMes.GetValueOrDefault().Year == fecha.Year)
                                select p;
                    }
                    else
                    {

                        query = from p in query
                                where (p.acumularAMes.GetValueOrDefault().Month >= mesIni && p.acumularAMes.GetValueOrDefault().Month <= mesFin) && p.año == fecha.Year
                                select p;
                    }
                }
                List<PeriodosNomina> periodosNominas = query.ToList();
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("periodioAcumuladoPorRangoMeses()1_Error: ").Append(ex));
            }
            return valor;

        }
        private void variablesTipoFuncion(string variable, TipoClasificacionFormula tipoAcumulado, DateTime fechaPeriodo)
        {
            Object resultado = 0;
            string funcion = variable;
            string parametroFuncion = variable.Substring(funcion.IndexOf("(") + 1, funcion.IndexOf(")")).Replace("'", "");
            string funcionNombre = variable.Substring(0, funcion.IndexOf("("));
            String[] parametrosFuncion = variable.Split(',');
            List<string> camposFuncion = new List<string>();
            List<Object> valoresFuncion = new List<object>();
            int posAcum = 1;
            clavePeriodoFuncion = "";
            bool usaBase = false;
            string nombreBase = "";
            CamposAMostrar mostrar = new CamposAMostrar();
            IQueryable<MovNomConcep> query = from mov in dbContextSimple.Set<MovNomConcep>() select mov; ;

            if (string.Equals(funcionNombre, "ACUMCNC", StringComparison.OrdinalIgnoreCase))
            {
                //Concepto
                query = from mov in query
                        where mov.concepNomDefi.clave == parametrosFuncion[0].Replace("'", "")
                        select mov;
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
                query = from mov in query
                        where mov.concepNomDefi.clave == parametrosFuncion[0]
                        select mov;
                nombreBase = parametrosFuncion[1];
                posAcum = 2;

            }
            else if (string.Equals(funcionNombre, "DEDUCCREDITOS", StringComparison.OrdinalIgnoreCase))
            {
                //creditos
                query = from mov in query
                        where mov.creditoMovimientos.creditoPorEmpleado.creditoAhorro.concepNomiDefin.clave == parametrosFuncion[0] &&
                           mov.creditoMovimientos.creditoPorEmpleado.creditoAhorro.tipoConfiguracion == "1"
                        select mov;
            }
            else if (string.Equals(funcionNombre, "DEDUCAHORROS", StringComparison.OrdinalIgnoreCase))
            {
                //Ahorros
                query = from mov in query
                        where mov.creditoMovimientos.creditoPorEmpleado.creditoAhorro.concepNomiDefin.clave == parametrosFuncion[0] &&
                           mov.creditoMovimientos.creditoPorEmpleado.creditoAhorro.tipoConfiguracion == "2"
                        select mov;
            }
            if (usaBase)
            {
                if (string.Equals(nombreBase, "ISR", StringComparison.OrdinalIgnoreCase))
                {

                    query = from mov in query
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where mov.id == mba.movNomConcep.id && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaISR.ToString()
                            select mov;
                }
                else if (string.Equals(nombreBase, "IMSS", StringComparison.OrdinalIgnoreCase))
                {
                    query = from mov in query
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where mov.id == mba.movNomConcep.id && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaIMSS.ToString()
                            select mov;

                }
                else if (string.Equals(nombreBase, "INF", StringComparison.OrdinalIgnoreCase))
                {
                    query = from mov in query
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where mov.id == mba.movNomConcep.id && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaInfonavit.ToString()
                            select mov;

                }
                else if (string.Equals(nombreBase, "PTU", StringComparison.OrdinalIgnoreCase))
                {
                    query = from mov in query
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where mov.id == mba.movNomConcep.id && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaInfonavit.ToString()
                            select mov;

                }
                else if (string.Equals(nombreBase, "ISN", StringComparison.OrdinalIgnoreCase))
                {
                    query = from mov in query
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where mov.id == mba.movNomConcep.id && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaPTU.ToString()
                            select mov;

                }
                else if (string.Equals(nombreBase, "DES", StringComparison.OrdinalIgnoreCase))
                {
                    query = from mov in query
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where mov.id == mba.movNomConcep.id && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaDespensa.ToString()
                            select mov;

                }
                else if (string.Equals(nombreBase, "AHO", StringComparison.OrdinalIgnoreCase))
                {

                    query = from mov in query
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where mov.id == mba.movNomConcep.id && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaFondoAhorro.ToString()
                            select mov;
                }
                else if (string.Equals(nombreBase, "AGUI", StringComparison.OrdinalIgnoreCase))
                {

                    query = from mov in query
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where mov.id == mba.movNomConcep.id && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaAguinaldo.ToString()
                            select mov;
                }
                else
                {
                    query = from mov in query
                            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where mov.id == mba.movNomConcep.id && mba.baseAfecConcepNom.baseNomina.clave == ClavesParametrosModulos.claveBaseNominaISR.ToString()
                            select mov;
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

                //Valor Mostrar
                if (parametrosFuncion.Length > 4)
                {
                    if (string.Equals(parametrosFuncion[posAcum + 3], "EXENTO", StringComparison.OrdinalIgnoreCase))
                    {
                        mostrar = CamposAMostrar.MovNomBaseAfectaResultadoExento;
                        //resultado = from mov in query
                        //            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                        //            where mov.id == mba.movNomConcep.id
                        //            select mba.resultadoExento;

                    }
                    else if (string.Equals(parametrosFuncion[posAcum + 3], "GRAVABLE", StringComparison.OrdinalIgnoreCase))
                    {
                        mostrar = CamposAMostrar.MovNomBaseAfectaResultado;
                        //resultado = from mov in query
                        //            from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                        //            where mov.id == mba.movNomConcep.id
                        //            select mba.resultado;
                    }
                }
                else
                {
                    mostrar = CamposAMostrar.MovNomConcepresultado;

                    //resultado = from mov in query
                    //            select mov.resultado;
                }
                resultado = movimientosAcumuladoPorRangoMeses(tipoAcumulado, fechaPeriodo, "", mostrar, query, TipoMostrarCampo.SUMA, null, null);
                funcion = funcion.Replace("(", "").Replace("'", "").Replace(",", "").Replace(")", "");
                valoresConceptosEmpleados.Add(funcion, resultado);
            }
        }
        private IQueryable<MovNomConcep> movimientosAcumuladoPorRangoMeses(TipoClasificacionFormula tipoAcumulado, DateTime fechaPeriodoNomina, String campoMostrar, CamposAMostrar campoAMostrar, IQueryable<MovNomConcep> query, TipoMostrarCampo tmc, String campoOmitir, Object valorOmitir)
        {
            //esta pendiente lo de finiquito
            Object valor = 0.0;
            int i, posicionParametro = 0;
            string[] variables = obtieneVariablesFormula(campoMostrar);
            string claveEmp = (string)valoresConceptosEmpleados["NumEmpleado".ToUpper()];
            string razonSocial = (string)valoresConceptosEmpleados["RazonSocial".ToUpper()];
            query = from x0 in query
                    join x2 in dbContextSimple.Set<Empleados>() on x0.empleado_ID equals x2.id
                    join x4 in dbContextSimple.Set<RazonesSociales>() on x0.razonesSociales_ID equals x4.id
                    where x0.empleado_ID == x2.id
                    && x0.razonesSociales_ID == x4.id &&
                    x2.clave == claveEmp && x4.clave == razonSocial
                    select x0;

            if (valoresConceptosEmpleados.ContainsKey("TipoNomina".ToUpper()))
            {
                string nomina = (string)(valoresConceptosEmpleados["TipoNomina".ToUpper()] == null ? "" : valoresConceptosEmpleados["TipoNomina".ToUpper()]);
                if (nomina.Length > 0)
                {
                    query = from x0 in query
                            where x0.tipoNomina.clave == nomina
                            select x0;

                }

            }
            if (valoresConceptosEmpleados.ContainsKey("ClaveTipoCorrida".ToUpper()))
            {
                string corrida = (string)(valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()] == null ? "" : valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()]);
                if (corrida.Length > 0)
                {
                    query = from x0 in query
                            where x0.tipoCorrida.clave == corrida
                            select x0;

                }

            }
            if (valoresConceptosEmpleados.ContainsKey("CentroDeCosto".ToUpper()))
            {
                string centro = (string)(valoresConceptosEmpleados["CentroDeCosto".ToUpper()] == null ? "" : valoresConceptosEmpleados["CentroDeCosto".ToUpper()]);
                if (centro.Length > 0)
                {
                    query = from x0 in query
                            where x0.centroDeCosto.clave == centro
                            select x0;

                }

            }
            if (valoresConceptosEmpleados.ContainsKey("uso".ToUpper()))
            {
                if (valoresConceptosEmpleados["uso".ToUpper()] != null)
                {
                    query = from x0 in query
                            where x0.uso == Convert.ToInt32(valoresConceptosEmpleados["uso".ToUpper()])
                            select x0;

                }
            }
            if (valoresConceptosEmpleados.ContainsKey("ImprimeListado".ToUpper()))
            {
                query = from x0 in query
                        where x0.concepNomDefi.imprimirEnListadoNomina == Convert.ToBoolean(valoresConceptosEmpleados["ImprimeListado".ToUpper()])
                        select x0;
            }
            if (valoresConceptosEmpleados.ContainsKey("ImprimeRecibo".ToUpper()))
            {
                query = from x0 in query
                        where x0.concepNomDefi.imprimirEnReciboNomina == Convert.ToBoolean(valoresConceptosEmpleados["ImprimeRecibo".ToUpper()])
                        select x0;
            }

            DateTime fechaRango = DateTime.Now;
            int mesIni = -1, mesFin = -1;

            if (tipoAcumulado == TipoClasificacionFormula.DATOPERIODO)
            {
                var subquery = from periodo in dbContextSimple.Set<PeriodosNomina>()
                               join tn in dbContextSimple.Set<TipoNomina>() on periodo.tipoNomina.id equals tn.id
                               select periodo;
                if (periodosNomina != null)
                {
                    if (string.Equals(clavePeriodoFuncion, "Anterior", StringComparison.OrdinalIgnoreCase))
                    {
                        var subquery2 = from pp in dbContextSimple.Set<PeriodosNomina>()
                                        where pp.tipoNomina.clave == valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString()
                                        select pp;
                        if (valoresConceptosEmpleados.ContainsKey("ClaveTipoCorrida".ToUpper()))
                        {
                            string corrida = valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()] == null ? "" : valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()].ToString();
                            if (corrida.Length > 0)
                            {
                                subquery2 = from pp in subquery2
                                            where pp.tipoCorrida.clave == corrida
                                            select pp;
                            }
                        }

                        DateTime fecha = (from pp in subquery2
                                          where (pp.fechaInicial < fechaPeriodoNomina && !(fechaPeriodoNomina >= pp.fechaInicial && fechaPeriodoNomina <= pp.fechaFinal))
                                          select new { pp.fechaInicial }).Max(f => Convert.ToDateTime(f.fechaInicial));

                        subquery = from periodo in subquery
                                   where periodo.fechaInicial == fecha
                                   select periodo;


                    }
                    else if (isNumericaString(clavePeriodoFuncion))
                    {
                        subquery = from periodo in subquery
                                   where periodo.clave == clavePeriodoFuncion && periodo.tipoNomina.clave == valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString()
                                   select periodo;

                    }
                    else if (periodosNomina.tipoNomina.periodicidad.dias == 1)
                    {
                        subquery = from periodo in subquery
                                   where (fechaPeriodoNomina >= periodo.fechaInicial && fechaPeriodoNomina <= periodo.fechaFinal) &&
                                   periodo.tipoNomina.clave == valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString()
                                   select periodo;
                    }
                    else
                    {
                        string claveNomina = valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString();
                        subquery = from periodo in subquery
                                   where (fechaPeriodoNomina >= periodo.fechaInicial && fechaPeriodoNomina <= periodo.fechaFinal) &&
                                   periodo.tipoNomina.clave == claveNomina
                                   select periodo;
                    }
                }
                else if (string.Equals(clavePeriodoFuncion, "Anterior", StringComparison.OrdinalIgnoreCase))
                {

                    var subquery3 = from pp in dbContextSimple.Set<PeriodosNomina>()
                                    where pp.tipoNomina.clave == valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString()
                                    select pp;
                    if (valoresConceptosEmpleados.ContainsKey("ClaveTipoCorrida".ToUpper()))
                    {
                        string corrida = valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()] == null ? "" : valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()].ToString();
                        if (corrida.Length > 0)
                        {
                            subquery3 = from pp in subquery3
                                        where pp.tipoCorrida.clave == corrida
                                        select pp;
                        }
                        DateTime fecha = (from pp in subquery3
                                          where (pp.fechaInicial < fechaPeriodoNomina && !(fechaPeriodoNomina >= pp.fechaInicial && fechaPeriodoNomina <= pp.fechaFinal))
                                          select new { pp.fechaInicial }).Max(f => Convert.ToDateTime(f.fechaInicial));

                        subquery = from periodo in subquery
                                   where periodo.fechaInicial == fecha
                                   select periodo;
                    }
                }
                else if (isNumericaString(clavePeriodoFuncion))
                {

                    subquery = from periodo in subquery
                               where periodo.clave == clavePeriodoFuncion && periodo.tipoNomina.clave == valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString()
                               select periodo;
                }
                else
                {
                    subquery = from periodo in subquery
                               where (fechaPeriodoNomina >= periodo.fechaInicial && fechaPeriodoNomina <= periodo.fechaFinal) &&
                               periodo.tipoNomina.clave == valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString()
                               select periodo;
                }

                if (valoresConceptosEmpleados.ContainsKey("ClaveTipoCorrida".ToUpper()))
                {
                    string corrida = valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()] == null ? "" : valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()].ToString();
                    if (corrida.Length > 0)
                    {
                        subquery = from periodo in subquery
                                   where periodo.tipoCorrida.clave == corrida
                                   select periodo;
                    }
                }
                List<Decimal> idperiodos = (from periodo in subquery
                                            select periodo.id).ToList();
                query = from x0 in query
                        where idperiodos.Contains(x0.periodosNomina.id)
                        select x0;
                if (isNumericaString(clavePeriodoFuncion))
                {
                    DateTime fecha = (DateTime)valoresConceptosEmpleados[parametroFechaFinal];
                    query = from x0 in query
                            where x0.periodosNomina.año == fecha.Year
                            select x0;

                }

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
                DateTime fechaFinal = (DateTime)valoresConceptosEmpleados[parametroFechaInicial];
                DateTime fecha = DateTime.Now;
                fecha = new DateTime(fechaFinal.Year, 1, 1);
                if (string.Equals(clavePeriodoFuncion, "Anterior", StringComparison.OrdinalIgnoreCase) || isNumericaString(clavePeriodoFuncion))
                {
                    fechaFinal = fechaPeriodoNomina;
                    fecha = new DateTime(fechaFinal.Year, 1, 1);
                }

                query = from x0 in query
                        where ((x0.periodosNomina.fechaInicial >= fecha && x0.periodosNomina.fechaInicial <= fechaFinal) || (x0.periodosNomina.fechaFinal >= fecha && x0.periodosNomina.fechaFinal <= fechaFinal))
                        select x0;

            }

            if (tipoAcumulado == TipoClasificacionFormula.DATOBIMESTRAL)
            {
                DateTime fecha = (DateTime)valoresConceptosEmpleados[parametroFechaFinal];
                if (string.Equals(clavePeriodoFuncion, "Anterior", StringComparison.OrdinalIgnoreCase) || isNumericaString(clavePeriodoFuncion))
                {
                    fecha = fechaPeriodoNomina;

                }
                query = from x0 in query
                        where x0.periodosNomina.año == fecha.Year
                        select x0;
                if (mesIni == fecha.Month + 1)
                {

                    query = from x0 in query
                            where x0.mes == mesIni
                            select x0;
                    if (!periodosNomina.cierreMes)
                    {
                        DateTime fechainicio = DateTime.Now;
                        fechainicio = new DateTime(fecha.Year, fecha.Month, 1);
                        query = from x0 in query
                                where ((x0.periodosNomina.fechaInicial >= fechainicio && x0.periodosNomina.fechaFinal <= fecha) || (x0.periodosNomina.fechaFinal >= fechainicio && x0.periodosNomina.fechaFinal <= fecha))
                                select x0;

                    }
                }
                else if (mesFin == fecha.Month + 1)
                {
                    if (!periodosNomina.cierreMes)
                    {
                        DateTime fechainicio = DateTime.Now;
                        fechainicio = new DateTime(fecha.Year, fecha.Month, 1);
                        query = from x0 in query
                                where (x0.mes == mesIni || (x0.mes == mesFin && (x0.periodosNomina.fechaInicial >= fechainicio && x0.periodosNomina.fechaInicial <= fecha) ||
                                (x0.periodosNomina.fechaFinal >= fechainicio && x0.periodosNomina.fechaFinal <= fecha)))
                                select x0;
                    }
                    else
                    {

                        query = from x0 in query
                                where x0.mes == mesIni || x0.mes == mesFin
                                select x0;
                    }

                }
            }
            else if (tipoAcumulado == TipoClasificacionFormula.DATOMENSUAL)
            {
                DateTime fecha = (DateTime)valoresConceptosEmpleados[parametroFechaFinal];
                if (string.Equals(clavePeriodoFuncion, "Anterior", StringComparison.OrdinalIgnoreCase) || isNumericaString(clavePeriodoFuncion))
                {
                    fecha = fechaPeriodoNomina;

                }

                query = from x0 in query
                        where x0.periodosNomina.año == fecha.Year && x0.mes == fecha.Month
                        select x0;

                if (!periodosNomina.cierreMes)
                {
                    DateTime fechainicio = DateTime.Now;
                    fechainicio = new DateTime(fecha.Year, fecha.Month, 1);
                    query = from x0 in query
                            where ((x0.periodosNomina.fechaInicial >= fechainicio && x0.periodosNomina.fechaInicial <= fecha) ||
                            (x0.periodosNomina.fechaFinal >= fechainicio && x0.periodosNomina.fechaFinal <= fecha))
                            select x0;
                }
            }
            //if (tmc == TipoMostrarCampo.COUNT)
            //{
            //    if (campoAMostrar == CamposAMostrar.MovNomConcepresultado)
            //    {
            //        valor = (from x0 in query
            //                 select x0.resultado).Count();

            //    }
            //    else if (campoAMostrar == CamposAMostrar.MovNomBaseAfectaResultado)
            //    {
            //        valor = (from x0 in query
            //                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
            //                 where x0.id == mba.movNomConcep.id
            //                 select mba.resultado).Count();

            //    }
            //    else if (campoAMostrar == CamposAMostrar.MovNomBaseAfectaResultadoExento)
            //    {

            //        valor = (from x0 in query
            //                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
            //                 where x0.id == mba.movNomConcep.id
            //                 select mba.resultadoExento).Count();
            //    }
            //}
            //else if (tmc == TipoMostrarCampo.SUMA)
            //{
            //    if (campoAMostrar == CamposAMostrar.MovNomConcepresultado)
            //    {
            //        var res = (from x0 in query
            //                   select new { resul = x0.resultado });
            //        if (res.Count() > 0)
            //        {
            //            valor = res.Sum(p => p.resul);
            //        }

            //    }
            //    else if (campoAMostrar == CamposAMostrar.MovNomBaseAfectaResultado)
            //    {
            //        var res = (from x0 in query
            //                   from mba in dbContextSimple.Set<MovNomBaseAfecta>()
            //                   where x0.id == mba.movNomConcep.id
            //                   select new { resul = mba.resultado });
            //        if (res.Count() > 0)
            //        {
            //            valor = res.Sum(p => p.resul);
            //        }
            //    }
            //    else if (campoAMostrar == CamposAMostrar.MovNomBaseAfectaResultadoExento)
            //    {
            //        var res = (from x0 in query
            //                   from mba in dbContextSimple.Set<MovNomBaseAfecta>()
            //                   where x0.id == mba.movNomConcep.id
            //                   select new { resul = mba.resultadoExento });
            //        if (res.Count() > 0)
            //        {
            //            valor = res.Sum(p => p.resul);
            //        }
            //    }

            //}
            //else
            //{
            //    if (campoAMostrar == CamposAMostrar.MovNomConcepresultado)
            //    {
            //        valor = (from x0 in query
            //                 select new { resul = x0.resultado });


            //    }
            //    else if (campoAMostrar == CamposAMostrar.MovNomBaseAfectaResultado)
            //    {
            //        valor = (from x0 in query
            //                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
            //                 where x0.id == mba.movNomConcep.id
            //                 select new { resul = mba.resultado });

            //    }
            //    else if (campoAMostrar == CamposAMostrar.MovNomBaseAfectaResultadoExento)
            //    {
            //        valor = (from x0 in query
            //                 from mba in dbContextSimple.Set<MovNomBaseAfecta>()
            //                 where x0.id == mba.movNomConcep.id
            //                 select new { resul = mba.resultadoExento });

            //    }

            //}

            return query;
        }
        private Object buscarValoresPTU(String campo)
        {
            try
            {
                if (string.Equals(campo, "PTUDIAS", StringComparison.OrdinalIgnoreCase))
                {
                    ptuEmpleado.periodoPtuDias = periodosNomina;
                    ptuEmpleado.tipoCorridaPtuDias = tipoCorrida;
                    ptuEmpleado.tipoNominaPtuDias = (TipoNomina)valoresConceptosEmpleados["TIPONOMINAENTIDAD"];
                }
                else if (string.Equals(campo, "PTUPERCEPCIONES", StringComparison.OrdinalIgnoreCase))
                {
                    ptuEmpleado.periodoPtuPercep = periodosNomina;
                    ptuEmpleado.tipoCorridaPtuPercep = tipoCorrida;
                    ptuEmpleado.tipoNominaPtuPercep = (TipoNomina)valoresConceptosEmpleados["TIPONOMINAENTIDAD"];
                }
                else if (string.Equals(campo, "PTUTOTAL", StringComparison.OrdinalIgnoreCase))
                {
                    ptuEmpleado.periodoPtuDias = periodosNomina;
                    ptuEmpleado.periodoPtuPercep = periodosNomina;
                    ptuEmpleado.tipoCorridaPtuDias = tipoCorrida;
                    ptuEmpleado.tipoCorridaPtuPercep = tipoCorrida;
                    ptuEmpleado.tipoNominaPtuDias = (TipoNomina)valoresConceptosEmpleados["TIPONOMINAENTIDAD"];
                    ptuEmpleado.tipoNominaPtuPercep = (TipoNomina)valoresConceptosEmpleados["TIPONOMINAENTIDAD"];
                }
                mensajeResultado = metodosParaPtu.buscarValoresPTU(campo, valoresConceptosEmpleados["NumEmpleado".ToUpper()].ToString(), valoresConceptosEmpleados["RazonSocial".ToUpper()].ToString(), ptuDatosGenerales.ejercicio.GetValueOrDefault(), (DBContextSimple)dbContextSimple);
                if (mensajeResultado.noError != 0)
                {
                    return 0;
                }
                else
                {
                    return mensajeResultado.resultado;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscarValoresPTU()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return 0;
        }

        private void buscaFormulaConceptos(String concepto)
        {
            try
            {
                String variableConcepto = concepto.Substring(concepto.IndexOf("_") + 1);
                String formula;
                // DbContext contex = new DBContextSimple();
                if (isNumericaString(variableConcepto))
                {
                    formula = (from c in dbContextSimple.Set<ConcepNomDefi>()
                               where c.clave == variableConcepto && c.fecha == (from cc in dbContextSimple.Set<ConcepNomDefi>() where cc.clave == c.clave select new { cc.fecha }).Max(f => f.fecha)
                               select c.formulaConcepto).SingleOrDefault();
                }
                else
                {
                    variableConcepto = variableConcepto.Replace('_', ' ');
                    formula = (from c in dbContextSimple.Set<ConcepNomDefi>()
                               where c.descripcion == variableConcepto && c.fecha == (from cc in dbContextSimple.Set<ConcepNomDefi>() where cc.clave == c.clave select new { cc.fecha }).Max(f => f.fecha)
                               select c.formulaConcepto).SingleOrDefault();
                }
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

        private void agregaParametrosConceptosNomina(List<MovNomConceParam> movNomConceParametros)
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
                            if (movNomConceParametros[j].paraConcepDeNom.clasificadorParametro == ClasificadorParametro.ENTRADA)
                            {
                                compEjec.addVariableExtraNro(string.Concat("Param", movNomConceParametros[j].paraConcepDeNom.numero).ToUpper());
                            }
                            else
                            {
                                compEjec.addVariableExtraNro(movNomConceParametros[j].paraConcepDeNom.descripcion.ToUpper());
                            }
                        }
                        else if (movNomConceParametros[j].paraConcepDeNom.clasificadorParametro == ClasificadorParametro.ENTRADA)
                        {
                            compEjec.addVariableExtraStr(string.Concat("Param", movNomConceParametros[j].paraConcepDeNom.numero).ToUpper());
                        }
                        else
                        {
                            compEjec.addVariableExtraStr(movNomConceParametros[j].paraConcepDeNom.descripcion.ToUpper());
                        }

                        if (movNomConceParametros[j].paraConcepDeNom.clasificadorParametro == ClasificadorParametro.ENTRADA)
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

        #region Calculo Finiquitos
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

        private Object[,] acumuladosISRAnualPorTipoAfecta(string claveEmpleado, string referenciaPlazaEmp, string tipoCorrida, string tipoNomina, string clavePeriodo, int añoPeriodo, string claveRazonSocial, string claveBaseNomina)
        {
            Object[,] acumulados = null;
            try
            {
                var acumula = (from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                               join m in dbContextSimple.Set<MovNomConcep>() on mba.movNomConcep_ID equals m.id into m_join
                               from m in m_join.DefaultIfEmpty()
                               where m.uso == 0 && m.tipoCorrida.clave == tipoCorrida && m.tipoNomina.clave == tipoNomina && m.periodosNomina.año == añoPeriodo
                                 && m.periodosNomina.clave.CompareTo(clavePeriodo) < 0 && m.empleados.clave == claveEmpleado && m.razonesSociales.clave == claveRazonSocial &&
                                 mba.baseAfecConcepNom.baseNomina.clave == claveBaseNomina && m.plazasPorEmpleado.referencia == referenciaPlazaEmp
                               group new { mba } by new
                               {
                                   mba.baseAfecConcepNom.tipoAfecta
                               } into g
                               select new { suma = g.Sum(r => r.mba.resultado), afecta = g.Key.tipoAfecta });
                int count = acumula.Count();
                if (count == 0)
                {
                    return null;
                }
                else
                {
                    int index = 0;
                    acumulados = new Object[count, 2];
                    foreach (var item in acumula)
                    {
                        acumulados[index, 0] = item.suma;
                        acumulados[index, 1] = item.afecta;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("acumuladosPorTipoISRAnual()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return acumulados;
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

        private void IsrRetenidos(MovNomConcep movimientosNomina)
        {
            try
            {
                Object[] retenidosISR = new Object[9];
                string numEmpleado = valoresConceptosEmpleados["NumEmpleado".ToUpper()].ToString();
                string claveTipoCorridas = valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()].ToString();
                string claveRazonsocial = valoresConceptosEmpleados["RazonSocial".ToUpper()].ToString();
                string claveTiposNomina = valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString();
                string numPeriodo = valoresConceptosEmpleados["NumPeriodo".ToUpper()].ToString();
                int ejercicioActivo = (int)valoresConceptosGlobales["ejercicioActivo".ToUpper()];
                if (tipoTablaISR == TipoTablaISR.NORMAL && ((modoAjustarIngresosMes == ProporcionaPeriodoAjustadoFinalMes && periodosNomina.cierreMes)
                        | modoAjustarIngresosMes == ProporcionaPeriodoAjustadoCadaVez
                        | modoAjustarIngresosMes == ProporcionaPeriodoConTablaAnual
                        | modoAjustarIngresosMes == PropPeriodoIndepAjustadoAlUltimoPeriodoMes
                        | modoAjustarIngresosMes == ProporcionaTablaAnual))
                {


                    var query = (from m in dbContextSimple.Set<MovNomConcep>()
                                 where m.uso == 0 && m.empleados.clave == numEmpleado && m.tipoCorrida.clave == claveTipoCorridas &&
                                     (m.concepNomDefi.formulaConcepto.Contains("CalculoISR") || m.concepNomDefi.formulaConcepto.Contains("ISRSubsidio")) && m.razonesSociales.clave == claveRazonsocial
                                 select new { m }
                                );

                    if (modoAjustarIngresosMes == ProporcionaPeriodoConTablaAnual)
                    {
                        //   // sub.m.periodosNomina.clave.CompareTo(numPeriodo)
                        query = from sub in query
                                where (sub.m.periodosNomina.clave.CompareTo(numPeriodo) < 0 && sub.m.tipoNomina.clave == claveTiposNomina
                                    && sub.m.periodosNomina.año == ejercicioActivo && sub.m.periodosNomina.tipoCorrida.clave == claveTipoCorridas)
                                select sub;
                    }
                    else
                    {
                        query = from sub in query
                                where (sub.m.periodosNomina.clave.CompareTo(numPeriodo) < 0 /*&& sub.m.periodosNomina.clave.CompareTo(
                                (from pn in dbContextSimple.Set<PeriodosNomina>() where pn.id < periodosNomina.id && 
                                 pn.tipoNomina.clave == claveTiposNomina && pn.cierreMes==true && pn.año==ejercicioActivo && 
                                 pn.tipoCorrida.clave==claveTipoCorridas select pn).Max(a=> a.clave)) > 0*/)
                                select sub;
                        //    query = from sub in query
                        //            where (sub.m.periodosNomina.tipoCorrida.clave == claveTipoCorridas && sub.m.periodosNomina.id < periodosNomina.id &&
                        //            sub.m.periodosNomina.tipoNomina.clave==claveTiposNomina && sub.m.periodosNomina.cierreMes==true &&
                        //            sub.m.periodosNomina.año==ejercicioActivo
                        //                                                    /* sub.m.periodosNomina.id<((from pn in dbContextSimple.Set<PeriodosNomina>()
                        //                                                                                           where pn.id < periodosNomina.id && pn.tipoNomina.clave == claveTiposNomina &&
                        //                                                                                               pn.cierreMes == true && pn.año == ejercicioActivo && pn.tipoCorrida.clave == claveTipoCorridas
                        //                                                                                           select new { pn.id }).Max(p => p.id))*/)
                        //            select sub;
                    }
                    decimal[] idsMov = null;
                    var idsMovAux = (from sub in query select new { sub.m.id }).ToList();
                    idsMov = new decimal[idsMovAux.Count];
                    int cont = 0;
                    foreach (var item in idsMovAux)
                    {
                        idsMov[cont] = item.id;
                        cont++;
                    }
                    if (idsMov.Length > 0)
                    {
                        var resultado = (from isr in dbContextSimple.Set<CalculoISR>()
                                         where idsMov.Contains(isr.movNomConcep.id) && isr.movNomConcep.ejercicio == ejercicioActivo
                                         group isr by 1 into g
                                         select new
                                         {
                                             isrRetenidoNormal = g.Count() == 0 ? 0 : g.Sum(s => s.isrRetenidoNormal == null ? 0 : s.isrRetenidoNormal),
                                             isrRetenidoDirecto = g.Count() == 0 ? 0 : g.Sum(s => s.isrRetenidoDirecto == null ? 0 : s.isrRetenidoDirecto),
                                             isrRetenidoAnual = g.Count() == 0 ? 0 : g.Sum(s => s.isrRetenidoAnual == null ? 0 : s.isrRetenidoAnual),
                                             isrACargoNormal = g.Count() == 0 ? 0 : g.Sum(s => s.isrACargoNormal),
                                             isrACargoDirecto = g.Count() == 0 ? 0 : g.Sum(s => s.isrACargoDirecto),
                                             isrACargoAnual = g.Count() == 0 ? 0 : g.Sum(s => s.isrACargoAnual),
                                             subsidioEmpleoNormal = g.Count() == 0 ? 0 : g.Sum(s => s.subsidioEmpleoNormal),
                                             subsidioEmpleoDirecto = g.Count() == 0 ? 0 : g.Sum(s => s.subsidioEmpleoDirecto),
                                             subsidioEmpleoAnual = g.Count() == 0 ? 0 : g.Sum(s => s.subsidioEmpleoAnual)

                                         });
                        if (resultado.Count() > 0)
                        {
                            retenidosISR[0] = resultado.Select(s => s.isrRetenidoNormal).Single();
                            retenidosISR[1] = resultado.Select(s => s.isrRetenidoDirecto).Single();
                            retenidosISR[2] = resultado.Select(s => s.isrRetenidoAnual).Single();
                            retenidosISR[3] = resultado.Select(s => s.isrACargoNormal).Single();
                            retenidosISR[4] = resultado.Select(s => s.isrACargoDirecto).Single();
                            retenidosISR[5] = resultado.Select(s => s.isrACargoAnual).Single();
                            retenidosISR[6] = resultado.Select(s => s.subsidioEmpleoNormal).Single();
                            retenidosISR[7] = resultado.Select(s => s.subsidioEmpleoNormal).Single();
                            retenidosISR[8] = resultado.Select(s => s.subsidioEmpleoAnual).Single();
                        }
                    }
                }
                //Busca el ISRRetenido del concepto ISR que ya existe para modificarlo
                if (movimientosNomina.id > 0)
                {
                    iSRRetenido = (from isr in dbContextSimple.Set<CalculoISR>() where isr.movNomConcep.id == movimientosNomina.id select isr).SingleOrDefault();//JSA23
                }
                if (iSRRetenido == null)
                {
                    iSRRetenido = new CalculoISR();//JSA23
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
                        iSRRetenidoSubsidio = (from isr in dbContextSimple.Set<CalculoISR>() where isr.movNomConcep.id == movimientosNomina.id select isr).SingleOrDefault();//JSA23                                                                                                                                              
                    }
                }

                if (iSRRetenidoSubsidio == null)
                {
                    iSRRetenidoSubsidio = new CalculoISR();//JSA23
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

        private double obtenerISRAcumuladoMes(List<PeriodosNomina> periodos, string claveEmpleado, string claveRazonSocial, string claveTipoNomina, string claveTipoCorrida, string formulaConcepto, string clavePlazaEmpleado)
        {
            try
            {
                var queryMov = from m in dbContextSimple.Set<MovNomConcep>()
                               where m.uso == 0 && m.empleados.clave == claveEmpleado && m.tipoCorrida.clave == claveTipoCorrida && m.concepNomDefi.formulaConcepto.Contains(formulaConcepto) &&
                                   m.razonesSociales.clave == claveRazonSocial && m.plazasPorEmpleado.referencia == clavePlazaEmpleado && m.tipoNomina.clave == claveTipoNomina &&
                                   m.periodosNomina.tipoCorrida.clave == claveTipoCorrida
                               select new { m };
                if (periodos != null)
                {
                    if (periodos.Count > 0)
                    {
                        decimal[] idsPeriodos = new decimal[periodos.Count];
                        for (int i = 0; i < periodos.Count; i++)
                        {
                            idsPeriodos[i] = periodos[i].id;
                        }
                        queryMov = from sub in queryMov where idsPeriodos.Contains(sub.m.periodosNomina.id) select sub;
                    }
                }

                object[] idsMovs = (from sub in queryMov select new { sub.m.id }).ToArray();


                var queryISR = from isr in dbContextSimple.Set<CalculoISR>()
                               where idsMovs.Contains(new { isr.movNomConcep.id })
                               select new
                               {
                                   suma = ((isr.isrRetenidoNormal == null ? 0.0 : isr.isrRetenidoNormal) + (isr.isrRetenidoDirecto == null ? 0.0 : isr.isrRetenidoDirecto)
                                       + (isr.isrRetenidoAnual == null ? 0.0 : isr.isrRetenidoAnual))
                               };

                double baseGravable = queryISR.Sum(s => s.suma.Value);
                return baseGravable;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ObtenerISRAcumuladoMes()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return 0.0;
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

        //regresa conceptos acumulados del mes anterior
        private double[] baseGravableAcumuladaMesAnterior(DateTime fechaPeriodo, string tipoCorrida)
        {
            try
            {
                double acumuladosMesAnterior, acumuladoISRMesAnterior;
                double[] acumulados = new Double[] { 0.0, 0.0 };
                fechaPeriodo.AddMonths(fechaPeriodo.Month - 1);
                List<PeriodosNomina> periodos = null;
                mensajeResultado = metodosPeriodosNomina.buscarPeriodosPorRangoMeses(0, fechaPeriodo, valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString(), valoresConceptosEmpleados["ClaveTipoCorrida".ToUpper()].ToString(), (DBContextSimple)dbContextSimple);
                if (mensajeResultado.noError == 0)
                {
                    periodos = (List<PeriodosNomina>)mensajeResultado.resultado;
                }
                else
                {
                    return null;
                }
                mensajeResultado = metodosParaMovimientosNomina.calcularMovimientosPorMesTipoAfecta(periodos, valoresConceptosEmpleados["NumEmpleado".ToUpper()].ToString(), valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString(), tipoCorrida, ClavesParametrosModulos.claveBaseNominaISR.ToString(),
                    valoresConceptosEmpleados["RazonSocial".ToUpper()].ToString(), valoresConceptosEmpleados["PlazaEmpleado".ToUpper()].ToString(), (DBContextSimple)dbContextSimple);
                if (mensajeResultado.noError == 0)
                {
                    acumuladosMesAnterior = (double)mensajeResultado.resultado;
                }
                else
                {
                    return null;
                }
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

        #endregion

        #region Calculo Anual
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
                    valoresConceptosEmpleados["NumPeriodo".ToUpper()].ToString(), getFechaDelSistema().Year, valoresConceptosEmpleados["RazonSocial".ToUpper()].ToString(), ClavesParametrosModulos.claveBaseNominaISR.ToString());
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


        private void calculoDiasTranscurridos()
        {
            try
            {
                List<PeriodosNomina> periodosNominas = (from mov in dbContextSimple.Set<MovNomConcep>()
                                                        where mov.periodosNomina.clave.CompareTo(valoresConceptosEmpleados["NumPeriodo".ToUpper()].ToString()) < 0 && mov.periodosNomina.año == (int)valoresConceptosEmpleados["ejercicioActivo".ToUpper()] && mov.tipoNomina.clave == valoresConceptosEmpleados["TipoNomina".ToUpper()].ToString() &&
                                                            mov.tipoCorrida.clave == valoresConceptosEmpleados["TipoCorrida".ToUpper()].ToString() && mov.empleados.clave == valoresConceptosEmpleados["NumEmpleado".ToUpper()].ToString()
                                                        orderby mov.periodosNomina.clave
                                                        select mov.periodosNomina).Distinct().ToList();
                periodosNominas = (periodosNominas == null) ? new List<PeriodosNomina>() : periodosNominas;
                periodosNominas.Add(periodosNomina);

                decimal dias = 0;
                DateTime fechaAlta = (DateTime)valoresConceptosEmpleados["FechaAlta".ToUpper()];
                DateTime fechaBaja = (DateTime)valoresConceptosEmpleados["FechaBaja".ToUpper()];
                double diasVacacionesAcum = 0.0, diasIncapacidadEmpleadoAcum = 0.0, faltasAcum = 0.0, faltasAcumAusentismoAcum = 0.0;
                if (periodosNominas.Count > 0)
                {
                    DateTime fechaIni = periodosNominas[0].fechaInicial.GetValueOrDefault(), fechafin = periodosNominas[(periodosNominas.Count - 1)].fechaFinal.GetValueOrDefault();
                    cargarVariablesEmpleadoAsistencias(fechaIni, fechafin, null, null, true);
                    PlazasPorEmpleadosMov plaza = (PlazasPorEmpleadosMov)valoresConceptosEmpleados["PlazaEmpleadoMovimiento".ToUpper()];
                    cargarVariablesEmpleadoVacaciones(null, plaza, true);
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("calculoDiasTranscurridos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
        }

        private double acumuladosPorTipoISRAnual(string claveEmpleado, string referenciaPlazaEmp, string tipoCorrida, string tipoNomina, string clavePeriodo, int añoPeriodo, string claveRazonSocial, string claveBaseNomina)
        {
            try
            {
                var acumula = (from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                               where mba.movNomConcep.uso == 0 && mba.movNomConcep.tipoCorrida.clave == tipoCorrida && mba.movNomConcep.tipoNomina.clave == tipoNomina
                               && mba.movNomConcep.periodosNomina.año == añoPeriodo
                                 && mba.movNomConcep.periodosNomina.clave.CompareTo(clavePeriodo) < 0 && mba.movNomConcep.empleados.clave == claveEmpleado
                                 && mba.movNomConcep.razonesSociales.clave == claveRazonSocial &&
                                 mba.baseAfecConcepNom.baseNomina.clave == claveBaseNomina && mba.movNomConcep.plazasPorEmpleado.referencia == referenciaPlazaEmp
                               select new { mba.resultado });
                int count = acumula.Count();
                if (count == 0)
                {
                    return 0.0;
                }
                else
                {
                    return acumula.Sum(s => s.resultado.GetValueOrDefault());
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("acumuladosPorTipoISRAnual()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return 0.0;
        }
        #endregion

        #region Calcula ISR por Separado
        //checado NORMAL Y ANUAL
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
                double baseGravableNormal = calculoISRNormal(acumNorm, acumDir, acumAnu, movimientosNomina, false);//BDEI01
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

        private Object[,] acumuladosPorTipoISR(string claveEmpleado, string referenciaPlazaEmp, string tipoCorrida, string tipoNomina, string clavePeriodo, int añoPeriodo, string claveRazonSocial, string claveBaseNomina)
        {
            Object[,] acumulados = null;
            // DbContext context = new DBContextSimple();
            try
            {
                var acumula = (from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                               where mba.movNomConcep.uso == 0 && mba.movNomConcep.empleados.clave == claveEmpleado && mba.movNomConcep.tipoCorrida.clave == tipoCorrida && mba.movNomConcep.periodosNomina.año == añoPeriodo &&
                                   mba.movNomConcep.periodosNomina.tipoCorrida.clave == tipoCorrida && mba.movNomConcep.tipoNomina.clave == tipoNomina && mba.baseAfecConcepNom.baseNomina.clave == claveBaseNomina &&
                                   mba.movNomConcep.razonesSociales.clave == claveRazonSocial && mba.movNomConcep.plazasPorEmpleado.referencia == referenciaPlazaEmp &&
                                   (mba.movNomConcep.periodosNomina.clave.CompareTo((from pn in dbContextSimple.Set<PeriodosNomina>()
                                                                                     where pn.clave.CompareTo(clavePeriodo) < 0 && pn.tipoNomina.clave == tipoNomina && pn.cierreMes == true && pn.año == añoPeriodo
                                                                                        && pn.tipoCorrida.clave == tipoCorrida
                                                                                     select new { pn.clave }).Max(m => m.clave) ?? "0") > 0 &&
                                   mba.movNomConcep.periodosNomina.clave.CompareTo(clavePeriodo) < 0)
                               group new { mba.baseAfecConcepNom, mba.movNomConcep.concepNomDefi, mba } by new
                               {
                                   mba.baseAfecConcepNom.tipoAfecta
                               } into g
                               select new
                               {
                                   suma = g.Sum(s => (s.mba.movNomConcep.concepNomDefi.naturaleza == Naturaleza.PERCEPCION ? (s.mba.resultado == null ? 0.0 : (s.mba.resultado * 1.0)) : 0.0)) -
                                        g.Sum(s => (s.mba.movNomConcep.concepNomDefi.naturaleza == Naturaleza.DEDUCCION ? (s.mba.resultado == null ? 0.0 : (s.mba.resultado * 1.0)) : 0.0)),
                                   tipoAfecta = g.Key.tipoAfecta
                               });
                int count = acumula.Count();
                if (count == 0)
                {
                    return null;
                }
                else
                {
                    int index = 0;
                    acumulados = new Object[count, 2];
                    foreach (var item in acumula)
                    {
                        acumulados[index, 0] = item.suma;
                        acumulados[index, 1] = item.tipoAfecta;
                    }
                }
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
                    ValorTablaISR valorTablaISR = aplicacionTablaISR(baseGravableMensual, true, tipoCorrida);//JSA24
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
                {//parametro por periodicidad desactivado utiliza tabla mensual//JSA08
                    if (modoAjustarIngresosMes == ProporcionaPeriodoIndependiente //M1
                            | modoAjustarIngresosMes == ProporcionaPeriodoAjustadoFinalMes//M2
                            | modoAjustarIngresosMes == ProporcionaPeriodoAjustadoCadaVez//M3
                            | modoAjustarIngresosMes == ProporcionaPeriodoConTablaAnual/*M4*/)
                    {//& !periodoAjustadoMes) { 
                        if (modoAjustarIngresosMes == ProporcionaPeriodoIndependiente
                                | (!periodosNomina.cierreMes & modoAjustarIngresosMes == ProporcionaPeriodoAjustadoFinalMes)
                                | (periodosNomina.cierreMes & modoAjustarIngresosMes == ProporcionaPeriodoAjustadoFinalMes & descontarFaltasModoAjustaMes)
                                | modoAjustarIngresosMes == ProporcionaPeriodoAjustadoCadaVez)
                        {
                            bool acumulaperiodos = false;
                            if (modoAjustarIngresosMes == ProporcionaPeriodoAjustadoCadaVez)
                            {//Modo 3//BDEI01
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
                        {//Modo1
                            baseGravable = (acumuladoNormal / diasPago) * factorMensual.GetValueOrDefault();
                            if (acumuladoDirecto > 0)
                            {//BDEI01
                                baseGravable = baseGravable + acumuladoDirecto;
                            }
                            if (acumuladoAnual > 0)
                            {//BDEI01
                                baseGravable = baseGravable + acumuladoAnual;
                            }
                        }
                        else if (modoAjustarIngresosMes == ProporcionaPeriodoAjustadoFinalMes)
                        {//Modo 2
                            if (periodosNomina.cierreMes)
                            {
                                baseGravable = acumuladoNormal + acumuladoPeriodosNormal + acumuladoDirecto + acumuladoPeriodosDirecto;//BDEI01
                            }
                            else
                            {
                                // baseGravable = ((acumuladoNormal + acumuladoPeriodosNormal) / diasPago) * factorMensual;
                                baseGravable = (acumuladoNormal / diasPago) * factorMensual.GetValueOrDefault();
                                if (acumuladoDirecto > 0)
                                {//BDEI01
                                    baseGravable = baseGravable + acumuladoDirecto;
                                }
                                if (acumuladoAnual > 0)
                                {//BDEI01
                                    baseGravable = baseGravable + acumuladoAnual;
                                }
                            }
                        }
                        else if (modoAjustarIngresosMes == ProporcionaPeriodoAjustadoCadaVez)
                        {//Modo 3//BDEI01
                            if (periodosNomina.cierreMes)
                            {
                                baseGravable = acumuladoNormal + acumuladoPeriodosNormal + acumuladoDirecto + acumuladoPeriodosDirecto;
                            }
                            else if (acumuladoPeriodosNormal > 0 | acumuladoPeriodosDirecto > 0)
                            {//BDEI01
                                baseGravable = acumuladoNormal + acumuladoPeriodosNormal;
                                baseGravable = (baseGravable / diasPago) * factorMensual.GetValueOrDefault();
                                baseGravable = baseGravable + acumuladoDirecto + acumuladoAnual + acumuladoPeriodosDirecto;
                            }
                            else
                            {
                                baseGravable = (acumuladoNormal / diasPago) * factorMensual.GetValueOrDefault();
                                if (acumuladoDirecto > 0)
                                {//BDEI01
                                    baseGravable = baseGravable + acumuladoDirecto;
                                }
                                if (acumuladoAnual > 0)
                                {//BDEI01
                                    baseGravable = baseGravable + acumuladoAnual;
                                }
                            }
                        }
                        else if (modoAjustarIngresosMes == ProporcionaPeriodoConTablaAnual)
                        {//Modo 4//BDEI01
                            calculoDiasTranscurridos();
                            baseGravable = ((acumuladoNormal + acumuladoPeriodosNormal) / diasPago) * factorAnual.GetValueOrDefault();
                            baseGravable = baseGravable + acumuladoPeriodosDirecto + acumuladoPeriodosAnual;

                            if (acumuladoDirecto > 0)
                            {//BDEI01
                                baseGravable = baseGravable + acumuladoDirecto;
                            }
                            if (acumuladoAnual > 0)
                            {//BDEI01
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

                                    isrNormal = (valoresTablaISR.isrNeto / diasTotales) * (int)valoresConceptosEmpleados["DiasNaturalesDelPeriodo".ToUpper()];
                                    retenido.isrACargoNormal = (valoresTablaISR.isrCausado / diasTotales) * (int)valoresConceptosEmpleados["DiasNaturalesDelPeriodo".ToUpper()];
                                    retenido.subsidioEmpleoNormal = (valoresTablaISR.subsidioAlEmpleo / diasTotales) * (int)valoresConceptosEmpleados["DiasNaturalesDelPeriodo".ToUpper()];
                                    //isrNormal = (valoresTablaISR.isrNeto / diasTotales) * diasPago;
                                    //retenido.isrACargoNormal = (valoresTablaISR.isrCausado / diasTotales) * diasPago;
                                    //retenido.subsidioEmpleoNormal = (valoresTablaISR.subsidioAlEmpleo / diasTotales) * diasPago;
                                }
                            }
                            else
                            {
                                //baseGravable = (baseGravable / diasPago) * diasPagoTotal;
                                if (diasPago >= diasPagoTotal)
                                {
                                    isrNormal = (valoresTablaISR.isrNeto / diasPagoTotal) * diasPago;
                                    retenido.isrACargoNormal = (valoresTablaISR.isrCausado / diasPagoTotal) * diasPago;
                                    retenido.subsidioEmpleoNormal = (valoresTablaISR.subsidioAlEmpleo / diasPagoTotal) * diasPago;
                                }
                                else
                                {
                                    isrNormal = (valoresTablaISR.isrNeto / diasPago) * diasPago;
                                    retenido.isrACargoNormal = (valoresTablaISR.isrCausado / diasPago) * diasPago;
                                    retenido.subsidioEmpleoNormal = (valoresTablaISR.subsidioAlEmpleo / diasPago) * diasPago;
                                }
                                //isrNormal = (valoresTablaISR.isrNeto / diasPagoTotal) * diasPago;
                                //retenido.isrACargoNormal = (valoresTablaISR.isrCausado / diasPagoTotal) * diasPago;
                                //retenido.subsidioEmpleoNormal = (valoresTablaISR.subsidioAlEmpleo / diasPagoTotal) * diasPago;

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
            try
            {
                List<PeriodosNomina> listPeriodosNominas = null;
                if (acumularPeriodos)
                {
                    listPeriodosNominas = (from mov in dbContextSimple.Set<MovNomConcep>()
                                           where (mov.periodosNomina.clave.CompareTo((from pn in dbContextSimple.Set<PeriodosNomina>()
                                                                                      where pn.clave.CompareTo(clavePeriodoNomina) < 0 && pn.tipoNomina.clave == claveTipoNomina && pn.tipoCorrida.clave == claveTipoCorrida && pn.cierreMes == true && pn.año == anioPeriodo
                                                                                      select new { pn.clave }).Max(p => p.clave)) > 0)
                                                   && mov.periodosNomina.clave.CompareTo(clavePeriodoNomina) < 0 && mov.periodosNomina.tipoNomina.clave == claveTipoNomina && mov.periodosNomina.año == anioPeriodo &&
                                                   mov.periodosNomina.tipoCorrida.clave == claveTipoCorrida && mov.empleados.clave == claveEmpleado
                                                   && fechaBaja >= mov.periodosNomina.fechaInicial && fechaAlta <= mov.periodosNomina.fechaFinal
                                           orderby mov.periodosNomina.clave
                                           select mov.periodosNomina).Distinct().ToList();
                }
                listPeriodosNominas = listPeriodosNominas == null ? new List<PeriodosNomina>() : listPeriodosNominas;
                double diasVacacionesAcum = 0.0, diasIncapacidadEmpleadoAcum = 0.0, faltasAcum = 0.0, faltasAcumAusentismoAcum = 0.0;
                if (listPeriodosNominas.Count > 0)
                {
                    DateTime fechaIni = listPeriodosNominas[0].fechaInicial.GetValueOrDefault(), fechafin = listPeriodosNominas[(listPeriodosNominas.Count - 1)].fechaFinal.GetValueOrDefault();
                    cargarVariablesEmpleadoAsistencias(fechaIni, fechafin, null, null, true);
                    PlazasPorEmpleadosMov plaza = (PlazasPorEmpleadosMov)valoresConceptosEmpleados["PlazaEmpleadoMovimiento".ToUpper()];
                    cargarVariablesEmpleadoVacaciones(null, plaza, true);
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
                        int diasvac = Convert.ToInt32(valoresConceptosEmpleados["diasVacaciones".ToUpper()]);
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
                List<PeriodosNomina> periodosNominas;
                if (periodoNomina.cierreMes)
                {
                    periodosNominas = (from p in dbContextSimple.Set<PeriodosNomina>()
                                       where p.clave.CompareTo((from pn in dbContextSimple.Set<PeriodosNomina>()
                                                                where pn.clave.CompareTo(clavePeriodoNomina) < 0 && pn.tipoNomina.clave == claveTipoNomina && pn.cierreMes == true && pn.año == anioPeriodo && pn.tipoCorrida.clave == claveTipoCorrida
                                                                select new { pn.clave }).Max(pp => pp.clave)) > 0 &&
                                             p.clave.CompareTo(clavePeriodoNomina) <= 0 && p.tipoNomina.clave == claveTipoNomina && p.año == anioPeriodo && p.tipoCorrida.clave == claveTipoCorrida && fechaAlta <= p.fechaFinal && fechaBaja >= p.fechaInicial
                                       select p).ToList();
                }
                else
                {
                    periodosNominas = (from p in dbContextSimple.Set<PeriodosNomina>()
                                       where p.clave.CompareTo((from pn in dbContextSimple.Set<PeriodosNomina>()
                                                                where pn.clave.CompareTo(clavePeriodoNomina) < 0 && pn.tipoNomina.clave == claveTipoNomina && pn.cierreMes == true && pn.año == anioPeriodo && pn.tipoCorrida.clave == claveTipoCorrida
                                                                select new { pn.clave }).Max(pp => pp.clave)) > 0 &&
                                             p.clave.CompareTo((from pn in dbContextSimple.Set<PeriodosNomina>()
                                                                where pn.clave.CompareTo(clavePeriodoNomina) > 0 && pn.tipoNomina.clave == claveTipoNomina && pn.cierreMes == true && pn.año == anioPeriodo && pn.tipoCorrida.clave == claveTipoCorrida
                                                                select new { pn.clave }).Min(pp => pp.clave)) <= 0 &&
                                             p.tipoNomina.clave == claveTipoNomina && p.año == anioPeriodo && p.tipoCorrida.clave == claveTipoCorrida && fechaAlta <= p.fechaFinal && fechaBaja >= p.fechaInicial
                                       select p).ToList();
                }
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

        #endregion

        #region Calcula ISR (calculaISPT)

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
                        ajusteISRalMes(movimientosNomina, baseGravableActual);
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

                        double[] isrSubAcumAnte = buscaISRRetenidoAnt();
                        double isrAcumAnt = isrSubAcumAnte[0];
                        double subAcumAnt = isrSubAcumAnte[1];
                        int diasPeriodo = 0;
                        int diasMov = 0;
                        if (isMov2Meses)
                        {
                            diasPeriodo = (cantidadDiasEntreDosFechas(periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault())) + 1;

                            DateTime cFecha = DateTime.Now;
                            if (movimientosNomina.numMovParticion == 1)
                            {
                                cFecha = movimientosNomina.periodosNomina.fechaInicial.GetValueOrDefault();
                                int dia = DateTime.DaysInMonth(cFecha.Year, cFecha.Month);
                                DateTime fec = new DateTime(cFecha.Year, cFecha.Month, dia);
                                diasMov = (cantidadDiasEntreDosFechas(periodosNomina.fechaInicial.GetValueOrDefault(), fec)) + 1;

                            }
                            else
                            {
                                cFecha = movimientosNomina.periodosNomina.fechaFinal.GetValueOrDefault();
                                DateTime fec2 = new DateTime(cFecha.Year, cFecha.Month, 1);
                                diasMov = (cantidadDiasEntreDosFechas(fec2, movimientosNomina.periodosNomina.fechaFinal.GetValueOrDefault())) + 1;

                            }

                            if (isrAcumAnt > 0)
                            {
                                double ajusteIsr = valoresTablaISRMes.isrNeto - isrAcumAnt;
                                if (ajusteIsr > 0)
                                {
                                    double ajusteIsrAux = (ajusteIsr / diasPeriodo) * diasMov;
                                    isrNormal = ajusteIsrAux;
                                }

                            }
                        }
                        else
                        {
                            if (isrAcumAnt > 0)
                            {
                                double ajusteIsr = valoresTablaISRMes.isrNeto - isrAcumAnt;
                                if (ajusteIsr > 0)
                                {
                                    isrNormal = ajusteIsr;
                                }

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

        public void ajusteISRalMes(MovNomConcep movimientosNomina, double baseGravableActual)
        {


            Object[,] acumulados;
            double acumNorm = 0.0, acumDir = 0.0, acumAnu = 0.0;
            double dias;

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
            double[] isrSubAcumAnte = buscaISRRetenidoAnt();
            double isrAcumAnt = isrSubAcumAnte[0];
            double subAcumAnt = isrSubAcumAnte[1];
            int diasPeriodo = 0;
            int diasMov = 0;
            if (isMov2Meses)
            {
                diasPeriodo = (cantidadDiasEntreDosFechas(periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault())) + 1;

                DateTime cFecha = DateTime.Now;
                if (movimientosNomina.numMovParticion == 1)
                {
                    cFecha = movimientosNomina.periodosNomina.fechaInicial.GetValueOrDefault();
                    int dia = DateTime.DaysInMonth(cFecha.Year, cFecha.Month);
                    DateTime fec = new DateTime(cFecha.Year, cFecha.Month, dia);
                    diasMov = (cantidadDiasEntreDosFechas(periodosNomina.fechaInicial.GetValueOrDefault(), fec)) + 1;

                }
                else
                {
                    cFecha = movimientosNomina.periodosNomina.fechaFinal.GetValueOrDefault();
                    DateTime fec2 = new DateTime(cFecha.Year, cFecha.Month, 1);
                    diasMov = (cantidadDiasEntreDosFechas(fec2, movimientosNomina.periodosNomina.fechaFinal.GetValueOrDefault())) + 1;

                }

                if (valoresTablaISRMes.subsidioAlEmpleo > subsidioCausadoAcum)
                {
                    double ajusteSubAux = (ajusteSub / diasPeriodo) * diasMov;
                    valoresConceptosEmpleados["SubsEmpleoCalculado".ToUpper()] = ajusteSubAux;
                    //valoresTablasISRper.subsidioAlEmpleo = ajusteSub;
                }
                else if (valoresTablaISRMes.subsidioAlEmpleo < subsidioCausadoAcum)
                {
                    double ajusteSubAux = (Math.Abs(ajusteSub) / diasPeriodo) * diasMov;
                    valoresConceptosEmpleados["AjusteSubCausado".ToUpper()] = ajusteSubAux;
                    valoresConceptosEmpleados["SubsEmpleoCalculado".ToUpper()] = 0.0;
                }

                if (isrAcumAnt > 0)
                {
                    double ajusteIsr = valoresTablaISRMes.isrNeto - isrAcumAnt;
                    if (ajusteIsr > 0)
                    {
                        double ajusteIsrAux = (ajusteIsr / diasPeriodo) * diasMov;
                        isrNormal = ajusteIsrAux;
                    }
                    else if (ajusteIsr < 0)
                    {
                        double ajusteIsrAux = (Math.Abs(ajusteIsr) / diasPeriodo) * diasMov;
                        valoresConceptosEmpleados["AjusteIsrMes".ToUpper()] = ajusteIsrAux;
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
                        double ajusteIsrSubAux = (ajusteIsrSub / diasPeriodo) * diasMov;
                        valoresConceptosEmpleados["ISRSubsidio".ToUpper()] = ajusteIsrSubAux;
                    }
                    else if (ajusteIsrSub < 0)
                    {
                        double ajusteIsrSubAux = (Math.Abs(ajusteIsrSub) / diasPeriodo) * diasMov;
                        valoresConceptosEmpleados["AjusteSubPagado".ToUpper()] = ajusteIsrSubAux;
                        valoresConceptosEmpleados["ISRSubsidio".ToUpper()] = 0.0;


                    }
                }


            }
            else
            {
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


        public Mensaje busquedaQueryConsultaEmpleados2(TipoBD tipoBD, TipoOperacion tipoOperacion, string tabla, OperadorSelect operadorSelect, List<CamposFrom> listCamposFrom, List<CamposWhere> listCamposWhere, String[] valoresDatosEspeciales, List<CamposWhere> listCamposWhereExtras, List<CamposGrupo> listCamposGrupo, List<CamposOrden> listCamposOrden, DateTime[] rangoFechas, string ordenado, string claveRazonSocial, string controladores, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            int i, j, k;
            List<Object> resultados;
            mensajeResultado = new Mensaje();
            mensajeResultado.noError = 0;
            mensajeResultado.error = "";
            isCalculoSDI = false;
            isUMA = false;
            try
            {
                bool isValorParametroOConcepto = false;
                isMov2Meses = false;
                bool usaMesesEnQuery = false;
                if (operadorSelect.listCamposSelect.Count() == 0)
                {
                    mensajeResultado.resultado = new List<object>();
                    return mensajeResultado;
                }
                usaFiniquitos = false;
                bool isDatoImss = false, isTablaFactorIntegracion = false, isTablaZonaSalarial = false, isParametro = true;
                nombreFuenteDatos = nombreFuenteDatos == null ? "" : nombreFuenteDatos;
                listCamposWhere = (listCamposWhere == null ? new List<CamposWhere>() : listCamposWhere);
                propertieFuente = CompEjec.abrirPropiedadBundle(nombreFuenteDatos);
                if (propertieFuente == null)
                {
                    propertieFuente = CompEjec.abrirPropiedadBundle("FuentesDeDatos");
                }

                string[] camposFormula;
                TipoClasificacionFormula tipoClasifFormula;
                string field;
                string[] fields;
                TipoNodoConsulta tipoNodoConsulta;
                bool usaMovEnEmpleados = false;
                for (i = 0; i < operadorSelect.listCamposSelect.Count(); i++)
                {
                    if (operadorSelect.listCamposSelect[i].subCampos != null && string.Equals(nombreFuenteDatos, "FuenteDatos_Empleados", StringComparison.OrdinalIgnoreCase))
                    {
                        for (j = 0; j < operadorSelect.listCamposSelect[i].subCampos.Count; j++)
                        {
                            if (operadorSelect.listCamposSelect[i].subCampos[j].campoMostrar.StartsWith(typeof(MovNomConcep).Name))
                            {
                                usaMovEnEmpleados = true;
                            }
                        }
                    }
                    field = operadorSelect.listCamposSelect[i].campoMostrar;
                    if (propertieFuente != null)
                    {
                        if (propertieFuente.ContainsKey(field))
                        {
                            if (string.Equals("TotalImportePorConcepto_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalImporteExentoPorConcepto_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalImporteGravablePorConcepto_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalImporteGravablePorConceptoDato_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalImportePorConceptoDato_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalImporteExentoPorConceptoDato_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISR_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISRNormal_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISRDirecto_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISRAnual_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISRGravable_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISRGravableNormal_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISRGravableDirecto_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISRGravableAnual_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISRGravableNormal_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISRGravableDirecto_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISRGravableAnual_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISRExento_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISRExentoNormal_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISRExentoDirecto_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseISRExentoAnual_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseIMSS_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseIMSSFija_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseIMSSVariable_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseIMSSGravado_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseIMSSGravadoFija_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseIMSSGravadoVariable_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseIMSSExento_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseIMSSExentoFija_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseIMSSExentoVariable_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseInfonavit_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBasePTU_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseImpuestoNomina_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseDespensa_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseFondoAhorro_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseAguinaldo_Path", field, StringComparison.OrdinalIgnoreCase)
                                || string.Equals("TotalBaseOtros_Path", field, StringComparison.OrdinalIgnoreCase))
                            {
                                isValorParametroOConcepto = true;
                                isMov2Meses = true;

                            }
                            tipoNodoConsulta = (TipoNodoConsulta)ManejadorEnum.GetValue(propertieFuente.GetProperty(string.Concat(field, "_TipoNodo")), typeof(TipoNodoConsulta));
                            field = propertieFuente.GetProperty(field);
                            if (field.Contains("|"))
                            {
                                fields = field.Split('|');
                                field = fields[0];
                            }
                            if ((tipoNodoConsulta == TipoNodoConsulta.CAMPO || tipoNodoConsulta == TipoNodoConsulta.CAMPOESPECIAL) &&
                                (string.Equals(field, string.Concat(typeof(MovNomConcep).Name, ".movNomConceParam.valor"), StringComparison.OrdinalIgnoreCase)
                                || string.Equals(field, string.Concat(typeof(MovNomConcep).Name, ".resultado"), StringComparison.OrdinalIgnoreCase)
                                || string.Equals(field, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultadoExento"), StringComparison.OrdinalIgnoreCase)
                                || string.Equals(field, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), StringComparison.OrdinalIgnoreCase)))
                            {
                                usaMesesEnQuery = true;
                            }
                            if (string.Equals(field, string.Concat(typeof(MovNomConcep).Name, ".movNomConceParam.valor"), StringComparison.OrdinalIgnoreCase) && (tipoNodoConsulta == TipoNodoConsulta.CAMPO || tipoNodoConsulta == TipoNodoConsulta.CAMPOESPECIAL))
                            {
                                isValorParametroOConcepto = true;
                                isMov2Meses = true;
                            }
                            else if (string.Equals(field, string.Concat(typeof(MovNomConcep).Name, ".resultado"), StringComparison.OrdinalIgnoreCase)
                              || string.Equals(field, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultadoExento"), StringComparison.OrdinalIgnoreCase)
                              || string.Equals(field, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), StringComparison.OrdinalIgnoreCase))
                            {
                                isMov2Meses = true;

                            }
                        }
                        else if (string.Equals(field, string.Concat(typeof(MovNomConcep).Name, ".movNomConceParam.valor"), StringComparison.OrdinalIgnoreCase))
                        {
                            isValorParametroOConcepto = true;
                            isMov2Meses = true;
                            usaMesesEnQuery = true;
                        }
                        else if (string.Equals(field, string.Concat(typeof(MovNomConcep).Name, ".resultado"), StringComparison.OrdinalIgnoreCase)
                              || string.Equals(field, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultadoExento"), StringComparison.OrdinalIgnoreCase)
                              || string.Equals(field, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), StringComparison.OrdinalIgnoreCase))
                        {
                            isMov2Meses = true;
                            usaMesesEnQuery = true;
                        }
                    }
                    else if (string.Equals(field, string.Concat(typeof(MovNomConcep).Name, ".movNomConceParam.valor"), StringComparison.OrdinalIgnoreCase))
                    {
                        isValorParametroOConcepto = true;
                        isMov2Meses = true;
                        usaMesesEnQuery = true;
                    }
                    else if (string.Equals(field, string.Concat(typeof(MovNomConcep).Name, ".resultado"), StringComparison.OrdinalIgnoreCase)
                              || string.Equals(field, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultadoExento"), StringComparison.OrdinalIgnoreCase)
                              || string.Equals(field, string.Concat(typeof(MovNomConcep).Name, ".movNomBaseAfecta.resultado"), StringComparison.OrdinalIgnoreCase))
                    {
                        isMov2Meses = true;
                        usaMesesEnQuery = true;
                    }
                    if (string.Equals(nombreFuenteDatos, "FuenteDatos_Empleados", StringComparison.OrdinalIgnoreCase) && field.StartsWith(typeof(MovNomConcep).Name))
                    {
                        usaMovEnEmpleados = true;
                    }
                    if (operadorSelect.listCamposSelect[i].isFormula)
                    {
                        camposFormula = eliminaCaracteresSeparador(operadorSelect.listCamposSelect[i].campoMostrar.Substring(1)).Split('|');
                        foreach (string strCampoAux in camposFormula)
                        {
                            tipoClasifFormula = TipoClasificacionFormula.SINCLASIFICAR;
                            string strCampo = strCampoAux;
                            if (strCampo.IndexOf('(') != -1)
                            {
                                strCampo = strCampo.Substring(0, strCampo.IndexOf('('));
                            }
                            if (propertieFuente.ContainsKey(string.Concat(strCampo, "_TipoDato")))
                            {
                                tipoClasifFormula = (TipoClasificacionFormula)ManejadorEnum.GetValue(propertieFuente.GetProperty(string.Concat(strCampo, "_TipoDato")), typeof(TipoClasificacionFormula));
                            }
                            else if (propertieFuente.ContainsKey(strCampo))
                            {
                                strCampo = propertieFuente.GetProperty(strCampo);
                                if (propertieFuente.ContainsKey(string.Concat(strCampo, "_TipoDato")))
                                {
                                    tipoClasifFormula = (TipoClasificacionFormula)ManejadorEnum.GetValue(propertieFuente.GetProperty(string.Concat(strCampo, "_TipoDato")), typeof(TipoClasificacionFormula));
                                }
                            }
                            if (tipoClasifFormula == TipoClasificacionFormula.DATOIMSS)
                            {
                                isDatoImss = true;
                            }
                            else if (string.Equals(strCampo, "VacacionesPorDisfrutar") || tipoClasifFormula == TipoClasificacionFormula.TABLAFACTORINTEGRACION)
                            {
                                isTablaFactorIntegracion = true;
                            }
                            else if (tipoClasifFormula == TipoClasificacionFormula.TABLAZONASALARIAL)
                            {
                                isTablaZonaSalarial = true;
                            }
                            else if (tipoClasifFormula == TipoClasificacionFormula.DATOPARAMETRO)
                            {
                                isParametro = true;
                            }
                        }
                    }
                }//end for operadorSelect.listCamposSelect=camposmostrar

                if (string.Equals(nombreFuenteDatos, "FuenteDatos_Empleados", StringComparison.OrdinalIgnoreCase))
                {
                    if (listCamposOrden == null ? false : listCamposOrden.Count() > 0)
                    {
                        for (i = 0; i < listCamposOrden.Count(); i++)
                        {
                            if (listCamposOrden[i].campo.Contains("|"))
                            {
                                fields = listCamposOrden[i].campo.Split('|');
                                for (int l = 0; l < fields.Length; l++)
                                {
                                    if (fields[l].StartsWith(typeof(MovNomConcep).Name))
                                    {
                                        usaMovEnEmpleados = true;
                                    }
                                }
                                field = fields[0];
                            }
                            else
                            {

                                field = listCamposOrden[i].campo;
                            }
                            if (field.StartsWith(typeof(MovNomConcep).Name))
                            {
                                usaMovEnEmpleados = true;
                            }
                        }

                    }
                }
                String corrida = "";
                if (!usaMovEnEmpleados && string.Equals(nombreFuenteDatos, "FuenteDatos_Empleados", StringComparison.OrdinalIgnoreCase))
                {
                    List<CamposWhere> camposWhereFiltrado = new List<CamposWhere>();

                    String[] datos;
                    String valor;
                    for (i = 0; i < listCamposWhere.Count(); i++)
                    {
                        datos = listCamposWhere[i].campo.Split('|');
                        if (datos.Length > 1)
                        {
                            valor = datos[1];
                        }
                        else
                        {
                            valor = datos[0];
                        }
                        if (!valor.StartsWith(typeof(MovNomConcep).Name))
                        {
                            camposWhereFiltrado.Add(listCamposWhere[i]);
                        }
                        else if (valor.ToUpper().Contains(typeof(TipoCorrida).Name.ToUpper()))
                        {
                            corrida = listCamposWhere[i].valor.ToString();
                        }
                    }
                    listCamposWhere = camposWhereFiltrado;
                    if (listCamposWhereExtras != null)
                    {
                        List<CamposWhere> camposWhereFiltradoExtra = new List<CamposWhere>();
                        for (i = 0; i < listCamposWhereExtras.Count(); i++)
                        {
                            fields = listCamposWhereExtras[i].campo.Split('|');
                            if (!fields[1].StartsWith(typeof(MovNomConcep).Name))
                            {
                                camposWhereFiltradoExtra.Add(listCamposWhereExtras[i]);
                            }
                        }
                        listCamposWhereExtras = camposWhereFiltradoExtra;
                    }
                    if (listCamposFrom != null)
                    {
                        List<CamposFrom> camposTablasRelacionadas = new List<CamposFrom>();
                        for (i = 0; i < listCamposFrom.Count(); i++)
                        {
                            if (!listCamposFrom[i].campo.StartsWith(typeof(MovNomConcep).Name))
                            {
                                camposTablasRelacionadas.Add(listCamposFrom[i]);
                            }

                        }
                        listCamposFrom = camposTablasRelacionadas;
                    }
                }

                fechaActual = getFechaDelSistema();
                List<Object> clavesElementosAplicacion = new List<Object>();
                clavesElementosAplicacion.Add(claveRazonSocial);
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                obtenerFactores(claveRazonSocial);
                if (mensajeResultado.noError != 0)
                {
                    return mensajeResultado;
                }
                getSession().Database.CurrentTransaction.Commit();
                cargaTablaFactorIntegracion(controladores, clavesElementosAplicacion, isTablaFactorIntegracion, isTablaZonaSalarial, isUMA, isParametro, fechaActual, fechaActual.Year, new DBContextMaster());
                /**
             * ***********************crea
             * conexion***************************************************
             */
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                datosFormulas = new List<DatosEspeciales>();
                valoresConceptosEmpleados = new Dictionary<string, object>();
                valoresConceptosGlobales = new Dictionary<string, object>();
                DateTime cIni = DateTime.Now, cFin = DateTime.Now;
                if (rangoFechas.Length > 0)
                {
                    cIni = rangoFechas[0];
                    cFin = rangoFechas[1];
                }
                int ini = cIni.Month, fin = cFin.Month;
                List<int> meses = new List<int>();
                if (ini > fin)
                {
                    meses.Add(ini + 1);
                    meses.Add(fin + 1);
                }
                else
                {
                    for (i = ini; i <= fin; i++)
                    {
                        meses.Add(i + 1);
                    }
                }
                valoresConceptosEmpleados.Add(parametroFechaInicial, cIni);
                valoresConceptosEmpleados.Add(parametroFechaFinal, cFin);

                /*
           * carga datos configuracion IMSS
           */

                if (isDatoImss)
                {
                    cargaDatosVariableConfiguracionIMSS(rangoFechas[1]);
                }
                if (isParametro)
                {
                    valoresConceptosGlobales.Add("FactorElevMensual".ToUpper(), factorMensual);
                    valoresConceptosGlobales.Add("FactorElevAnual".ToUpper(), factorAnual);
                }
                List<PlazasPorEmpleadosMov> plazasPorEmpleadosMov = null;
                List<Object> idsPlazasValidas = new List<Object>();
                HashSet<object> claveEmpleados = new HashSet<object>();
                Object claveTipoNominaGloblal = null, claveRazonSocialGlobal = null, claveTipoCorrida = corrida, uso = null, claveCentroCostoGlobal = null, claveImprimeListado = null, ClaveImprimeRecibo = null;
                manejaPagoDiasNaturales = manejaPagoDiasNaturales == null ? false : manejaPagoDiasNaturales;
                manejaPagoIMSSDiasNaturales = manejaPagoIMSSDiasNaturales == null ? false : manejaPagoIMSSDiasNaturales;
                ///********************************************************/
                if (isTablaZonaSalarial)
                {
                    if (isUMA)
                    {
                        salarioMinimoDF = valorUMA;
                        valoresConceptosGlobales.Add("SalarioMinDF".ToUpper(), valorUMA);
                    }
                    else
                    {
                        ZonaSalarial salarioZona = buscaSalarioPorZona('A');
                        if (salarioZona == null)
                        {
                            mensajeResultado.noError = 40;
                            mensajeResultado.error = "No existe Zona Salarial A";
                            return mensajeResultado;
                        }
                        salarioMinimoDF = salarioZona.salario;
                        valoresConceptosGlobales.Add("SalarioMinDF".ToUpper(), salarioMinimoDF);
                    }
                    salarioMinimoDF = null;
                }
                HashSet<string> filtrosTipoMovimiento = new HashSet<string>();
                if (string.Equals(nombreFuenteDatos, "FuenteDatos_Empleados", StringComparison.OrdinalIgnoreCase))
                {


                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("cargaDatosVariableConfiguracionIMSS()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return mensajeResultado;
        }
        private List<PlazasPorEmpleadosMov> datosPlazaPorEmpleado(List<CamposWhere> camposWhere, List<CamposWhere> camposWhereExtras)
        {
            GeneradorQueries construyeQueries = new GeneradorQueries();
            int i;
            List<CamposWhere> camposWhereEmpleados = new List<CamposWhere>();

            List<CamposWhere> camposWhereExtrasEmpleados = new List<CamposWhere>();
            bool isEntidadSalarioDiario = false;
            for (i = 0; i < camposWhere.Count(); i++)
            {
                if (camposWhere[i].campo.StartsWith(typeof(PlazasPorEmpleadosMov).Name) || camposWhere[i].campo.StartsWith(string.Concat("|", typeof(PlazasPorEmpleadosMov).Name)))
                {
                    camposWhereEmpleados.Add(camposWhere[i]);

                }
                if (camposWhere[i].campo.StartsWith(string.Concat(typeof(SalariosIntegrados).Name, ".tipoDeSalario")))
                {
                    isEntidadSalarioDiario = true;
                    camposWhereEmpleados.Add(camposWhere[i]);
                    CamposWhere camp = new CamposWhere();
                    camp.campo = string.Concat(typeof(SalariosIntegrados).Name, ".empleados.id");
                    camp.operadorComparacion = OperadorComparacion.IGUAL;
                    camp.operadorLogico = OperadorLogico.AND;
                    camp.valor = string.Concat(typeof(PlazasPorEmpleadosMov).Name, ".plazasPorEmpleado.empleados.id");
                    camposWhereExtrasEmpleados.Add(camp);

                }
            }
            for (i = 0; i < camposWhereExtras.Count(); i++)
            {
                for (int j = 0; j < camposWhereExtras[i].listCamposAgrupados.Count(); j++)
                {
                    if (camposWhereExtras[i].listCamposAgrupados[j].campo.StartsWith(typeof(PlazasPorEmpleadosMov).Name))
                    {
                        camposWhereExtrasEmpleados.Add(camposWhereExtras[i].listCamposAgrupados[j]);
                    }
                }


            }
            OperadorSelect select = new OperadorSelect();
            CamposSelect camposelect = new CamposSelect();
            List<CamposSelect> listCampoSelect = new List<CamposSelect>();
            camposelect.campoMostrar = typeof(PlazasPorEmpleadosMov).Name;
            camposelect.tipoFuncion = TipoFuncion.NINGUNO;
            listCampoSelect.Add(camposelect);
            select.todosDatos = true;
            select.listCamposSelect = listCampoSelect;
            select.tipoFuncion = TipoFuncion.NINGUNO;



            DatosQuery datosQuery = construyeQueries.construyeQueryDatos(select, null, camposWhere, camposWhereExtras, null, null);
            return null;
        }
        private void obtenerFactores(String claveRazonSocial)
        {
            metodosBDMaestra.obtenerFactores(claveRazonSocial, periodicidadTipoNomina, new DBContextMaster());
            factorAnual = metodosBDMaestra.factorAnual;
            manejaPagosPorHora = metodosBDMaestra.manejaPagosPorHora;
            manejoHorasPor = metodosBDMaestra.manejoHorasPor;
            factorMensual = metodosBDMaestra.factorMensual;
            tipoTablaISR = metodosBDMaestra.tipoTablaISR;
            manejaPagoDiasNaturales = metodosBDMaestra.manejaPagoDiasNaturales;
            versionCalculoPrestamoAhorro = metodosBDMaestra.versionCalculoPrestamoAhorro;
            manejaPagoIMSSDiasNaturales = metodosBDMaestra.manejaPagoIMSSDiasNaturales;
            descontarFaltasModoAjustaMes = metodosBDMaestra.descontarFaltasModoAjustaMes;
            pagarVacaAuto = metodosBDMaestra.pagarVacaAuto;
            salarioVacaciones = metodosBDMaestra.salarioVacaciones;
            isUMA = metodosBDMaestra.isUMA;
            manejoSalarioDiario = metodosBDMaestra.manejoSalarioDiario;
            calculoSeparadoISR = metodosBDMaestra.calculoSeparadoISR;
            mensajeResultado = metodosBDMaestra.mensajeResultado;

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

                subsidioCausadoMes = (from m in dbContextSimple.Set<MovNomConcep>()
                                      where m.uso == 0 && m.empleados.clave == numEmpleado && m.tipoCorrida.clave == claveTipoCorridas &&
                                          (m.concepNomDefi.formulaConcepto.Contains("SubsEmpleoCalculado")) && m.razonesSociales.clave == claveRazonsocial &&
                                           m.periodosNomina.tipoNomina.clave == claveTiposNomina && m.periodosNomina.clave.CompareTo(numPeriodo) < 0 &&
                                           m.periodosNomina.año == ejercicioActivo
                                      select new { m.resultado }
                                ).Sum(p => p.resultado).GetValueOrDefault();

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

                isrAnte = (from m in dbContextSimple.Set<MovNomConcep>()
                           where m.uso == 0 && m.empleados.clave == numEmpleado && m.tipoCorrida.clave == claveTipoCorridas &&
                               (m.concepNomDefi.formulaConcepto.Contains("CalculoISR")) && m.razonesSociales.clave == claveRazonsocial &&
                                m.periodosNomina.tipoNomina.clave == claveTiposNomina && m.periodosNomina.clave.CompareTo(numPeriodo) < 0 &&
                                m.periodosNomina.año == ejercicioActivo
                           select new { m.resultado }
                                ).Sum(p => p.resultado).GetValueOrDefault();

                subAnte = (from m in dbContextSimple.Set<MovNomConcep>()
                           where m.uso == 0 && m.empleados.clave == numEmpleado && m.tipoCorrida.clave == claveTipoCorridas &&
                               (m.concepNomDefi.formulaConcepto.Contains("ISRSubsidio")) && m.razonesSociales.clave == claveRazonsocial &&
                                m.periodosNomina.tipoNomina.clave == claveTiposNomina && m.periodosNomina.clave.CompareTo(numPeriodo) < 0 &&
                                m.periodosNomina.año == ejercicioActivo
                           select new { m.resultado }
                              ).Sum(p => p.resultado).GetValueOrDefault();

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

        private Mensaje eliminarMovimientosNominaBasura(decimal[] ids)
        {

            try
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    //Elimina Bases Afectadas de Movimientos por Conceptos
                    decimal id = ids[i];
                    List<MovNomBaseAfecta> movNomBaseAfectas = (from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                                                                join m in dbContextSimple.Set<MovNomConcep>() on mba.movNomConcep_ID equals m.id into join_m
                                                                from m in join_m.DefaultIfEmpty()
                                                                where mba.movNomConcep_ID == id
                                                                select mba).ToList();
                    if (movNomBaseAfectas.Count > 0)
                    {
                        for (int j = 0; j < movNomBaseAfectas.Count; j++)
                        {
                            dbContextSimple.Set<MovNomBaseAfecta>().Attach(movNomBaseAfectas[j]);
                            dbContextSimple.Set<MovNomBaseAfecta>().Remove(movNomBaseAfectas[j]);
                            dbContextSimple.SaveChanges();
                        }
                    }

                    //Elimina Movimientos Por parametros de Movimientos por Conceptos
                    List<MovNomConceParam> movNomConceParams = (from mnp in dbContextSimple.Set<MovNomConceParam>()
                                                                join m in dbContextSimple.Set<MovNomConcep>() on mnp.movNomConcep_ID equals m.id into join_m
                                                                from m in join_m.DefaultIfEmpty()
                                                                where mnp.movNomConcep_ID == id
                                                                select mnp).ToList();
                    if (movNomConceParams.Count > 0)
                    {
                        for (int j = 0; j < movNomConceParams.Count; j++)
                        {
                            dbContextSimple.Set<MovNomConceParam>().Attach(movNomConceParams[j]);
                            dbContextSimple.Set<MovNomConceParam>().Remove(movNomConceParams[j]);
                            dbContextSimple.SaveChanges();
                        }
                    }
                    //Elimina Movimientos ISRRetenidos
                    List<CalculoISR> calculoISRs = (from isr in dbContextSimple.Set<CalculoISR>()
                                                    join m in dbContextSimple.Set<MovNomConcep>() on isr.movNomConcep_ID equals m.id into join_m
                                                    from m in join_m.DefaultIfEmpty()
                                                    where isr.movNomConcep_ID == id
                                                    select isr).ToList();
                    if (calculoISRs.Count > 0)
                    {
                        for (int j = 0; j < calculoISRs.Count; j++)
                        {
                            dbContextSimple.Set<CalculoISR>().Attach(calculoISRs[j]);
                            dbContextSimple.Set<CalculoISR>().Remove(calculoISRs[j]);
                            dbContextSimple.SaveChanges();
                        }
                    }
                    //Elimina Movimientos CalculoIMSS
                    List<CalculoIMSS> calculoIMSSes = (from imss in dbContextSimple.Set<CalculoIMSS>()
                                                       join m in dbContextSimple.Set<MovNomConcep>() on imss.movNomConcep_ID equals m.id into join_m
                                                       from m in join_m.DefaultIfEmpty()
                                                       where imss.movNomConcep_ID == id
                                                       select imss).ToList();
                    if (calculoIMSSes.Count > 0)
                    {
                        for (int j = 0; j < calculoIMSSes.Count; j++)
                        {
                            dbContextSimple.Set<CalculoIMSS>().Attach(calculoIMSSes[j]);
                            dbContextSimple.Set<CalculoIMSS>().Remove(calculoIMSSes[j]);
                            dbContextSimple.SaveChanges();
                        }
                    }
                    //Elimina Movimientos CalculoIMSSPatron
                    List<CalculoIMSSPatron> calculoIMSSPatrons = (from imss in dbContextSimple.Set<CalculoIMSSPatron>()
                                                                  join m in dbContextSimple.Set<MovNomConcep>() on imss.movNomConcep_ID equals m.id into join_m
                                                                  from m in join_m.DefaultIfEmpty()
                                                                  where imss.movNomConcep_ID == id
                                                                  select imss).ToList();
                    if (calculoIMSSPatrons.Count > 0)
                    {
                        for (int j = 0; j < calculoIMSSPatrons.Count; j++)
                        {
                            dbContextSimple.Set<CalculoIMSSPatron>().Attach(calculoIMSSPatrons[j]);
                            dbContextSimple.Set<CalculoIMSSPatron>().Remove(calculoIMSSPatrons[j]);
                            dbContextSimple.SaveChanges();
                        }
                    }
                    //Elimina Movimientos por Conceptos
                    List<MovNomConcep> movNomConceps = (from m in dbContextSimple.Set<MovNomConcep>()
                                                        where m.id == id
                                                        select m).ToList();
                    if (movNomConceps.Count > 0)
                    {
                        for (int j = 0; j < movNomConceps.Count; j++)
                        {
                            dbContextSimple.Set<MovNomConcep>().Attach(movNomConceps[j]);
                            dbContextSimple.Set<MovNomConcep>().Remove(movNomConceps[j]);
                            dbContextSimple.SaveChanges();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("eliminarMovimientosNominaBasura()1_Error: ").Append(ex));
                mensajeResultado.noError = 50;
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return mensajeResultado;
        }

        #endregion

        #endregion

        #endregion

    }
}