using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.TestCompilador.funciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Exitosw.Payroll.Entity.util;
using System.Data.Entity;
using System.Diagnostics;

namespace Exitosw.Payroll.Core.metodosCalculoNomina
{
    /*Carga parametros y configuraciones de tablas xml*/
    public class MetodosBDMaestra
    {
        public Mensaje mensajeResultado = new Mensaje();
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");

        public TipoTablaISR tipoTablaISR { get; set; }
        public double? factorAnual { get; set; } = 0.0;
        public bool? manejaPagosPorHora { get; set; } = null;
        public ManejoHorasPor? manejoHorasPor { get; set; } = null;
        public double? factorMensual { get; set; } = 0.0;
        public int modoAjustarIngresosMes { get; set; }
        public bool manejaPagoDiasNaturales { get; set; } = false;
        public int versionCalculoPrestamoAhorro { get; set; } = 1;
        public bool manejaPagoIMSSDiasNaturales { get; set; } = false;
        public bool descontarFaltasModoAjustaMes { get; set; } = false;
        public PagarPrimaVacionalyVacacionesAuto pagarVacaAuto { get; set; }
        public ManejoSalarioVacaciones salarioVacaciones { get; set; }
        public bool isUMA { get; set; } = false;
        public ManejoSalarioDiario? manejoSalarioDiario { get; set; } = null;
        public bool calculoSeparadoISR { get; set; } = false;
        public object[,] tablaIsr { get; set; }
        public object[,] tablaFactorIntegracion { get; set; }
        public object[,] tablaSubsidio { get; set; }
        public object[,] tablaZonaSalarial { get; set; }
        public double? valorUMA { get; set; } = 0.0;
        public double? valorUMI { get; set; } = 0.0;
        public object[,] tablaIsrMes { get; set; }

        public object[,] tablaSubsidioMes { get; set; }
        public List<TablaDatos> tablaDatosXml { get; set; }

        Stopwatch tiempo = new Stopwatch();

        private static int ProporcionaPeriodoIndependiente = 1, ProporcionaPeriodoAjustadoFinalMes = 2, ProporcionaPeriodoAjustadoCadaVez = 3,
               ProporcionaPeriodoConTablaAnual = 4, ProporcionaCadaPeriodoUtilizandoTablaPeriodo = 5, PropPeriodoIndepAjustadoAlUltimoPeriodoMes = 6, UltimoPeriodoSinAjustarMes = 7,
               ProporcionaTablaAnual = 8, PropPeriodoIndepDiasNaturales = 9;
        private DateTime fechaActual;

        public MetodosBDMaestra()
        {
            fechaActual = DateTime.Now;
        }
        public MetodosBDMaestra(DateTime fechaActual)
        {
            this.fechaActual = fechaActual;
        }

        public Mensaje generaTablasXml(string controlador, Periodicidad periodicidadTipoNomina, string claveRazonSocial, DateTime fechaFinal, int ejercicio, DBContextMaster dbContextMaster)
        {
            inicializaVariableMensaje();
            try
            {
                // dbContextMaster.Database.BeginTransaction();
                
                obtenerFactores(claveRazonSocial, periodicidadTipoNomina, dbContextMaster);
                
                if (mensajeResultado.noError != 0)
                {
                    dbContextMaster.Database.CurrentTransaction.Rollback();
                    return mensajeResultado;
                }

                List<TablaBase> tablaBaseSistema = getCargarTablaBaseSistema(dbContextMaster);
                List<TipoControlador> tipoControladores;
                
                if (modoAjustarIngresosMes == ProporcionaPeriodoConTablaAnual)
                {
                    #region tabla isr anual
                    tipoControladores = obtieneTipoControladorTablaBase(ClavesParametrosModulos.claveTipoTablaISRAnual.ToString(), tablaBaseSistema);

                    if (mensajeResultado.noError != 0)
                    {
                        mensajeResultado.noError = 3;
                        mensajeResultado.error = "No encontro controladores en la tabla ISR anual";
                        return mensajeResultado;
                    }
                    tablaIsr = construyeTablaXmlPorTipoNomina(ClavesParametrosModulos.claveTipoTablaISRAnual.ToString(), "", tipoControladores, fechaFinal, ejercicio, dbContextMaster);
                    mensajeResultado = evaluaContenidoObject(tablaIsr, "No se encontro datos en la tabla ISR Anual", 4, mensajeResultado);

                    if (mensajeResultado.noError != 0)
                    {
                        return mensajeResultado;
                    }
                    #endregion

                    #region tabla subsidio anual
                    tipoControladores = obtieneTipoControladorTablaBase(ClavesParametrosModulos.claveTipoTablaSUBSIDIOAnual.ToString(), tablaBaseSistema);

                    if (mensajeResultado.noError != 0)
                    {
                        mensajeResultado.noError = 5;
                        mensajeResultado.error = "No encontro controladores en la tabla Subsidio anual";
                        return mensajeResultado;
                    }
                    tablaSubsidio = construyeTablaXmlPorTipoNomina(ClavesParametrosModulos.claveTipoTablaSUBSIDIOAnual.ToString(), "", tipoControladores, fechaFinal, ejercicio, dbContextMaster);
                    mensajeResultado = evaluaContenidoObject(tablaSubsidio, "No se encontro datos en la tabla Subsidio Anual", 6, mensajeResultado);

                    if (mensajeResultado.noError != 0)
                    {
                        return mensajeResultado;
                    }
                    #endregion
                }
                else if (tipoTablaISR == TipoTablaISR.PERIODICIDAD)
                {
                    if (periodicidadTipoNomina != null)
                    {
                       
                        #region tabla isr por periodicidad
                        string controladorPeriodicidad = String.Concat(typeof(Periodicidad).Name, periodicidadTipoNomina.clave);
                        tipoControladores = obtieneTipoControladorTablaBase(ClavesParametrosModulos.claveTipoTablaISRPeriodicidad.ToString(), tablaBaseSistema);
                        if (mensajeResultado.noError != 0)
                        {
                            mensajeResultado.noError = 7;
                            mensajeResultado.error = "no encontro controladores en la tabla ISR por periodicidad";
                            return mensajeResultado;
                        }
                        tablaIsr = construyeTablaXmlPorTipoNomina(ClavesParametrosModulos.claveTipoTablaISRPeriodicidad.ToString(), controladorPeriodicidad, tipoControladores, fechaFinal, ejercicio, dbContextMaster);
                        mensajeResultado = evaluaContenidoObject(tablaIsr, "No se encontro datos en la tabla ISR por periodicidad", 8, mensajeResultado);

                        if (mensajeResultado.noError != 0)
                        {
                            return mensajeResultado;
                        }
                        #endregion

                        #region tabla subsidio por periodicidad
                        tipoControladores = obtieneTipoControladorTablaBase(ClavesParametrosModulos.claveTipoTablaSubsidioPeriodicidad.ToString(), tablaBaseSistema);

                        if (mensajeResultado.noError != 0)
                        {
                            mensajeResultado.noError = 9;
                            mensajeResultado.error = "No encontro controladores en la tabla Subsidio por periodicidad";
                            return mensajeResultado;
                        }
                        tablaSubsidio = construyeTablaXmlPorTipoNomina(ClavesParametrosModulos.claveTipoTablaSubsidioPeriodicidad.ToString(), controladorPeriodicidad, tipoControladores, fechaFinal, ejercicio, dbContextMaster);
                        mensajeResultado = evaluaContenidoObject(tablaSubsidio, "No se encontro datos en la tabla Subsidio por periodicidad", 10, mensajeResultado);

                        if (mensajeResultado.noError != 0)
                        {
                            return mensajeResultado;
                        }
                        #endregion
                        
                    }
                }
                else
                {
                    string tipoTabla = ClavesParametrosModulos.claveTipoTablaISR.ToString();
                    bool isAnual = false;
                    if (modoAjustarIngresosMes == ProporcionaPeriodoConTablaAnual)
                    {
                        tipoTabla = ClavesParametrosModulos.claveTipoTablaISRAnual.ToString();
                        isAnual = true;
                    }

                    #region tabla isr
                    tipoControladores = obtieneTipoControladorTablaBase(tipoTabla, tablaBaseSistema);

                    if (mensajeResultado.noError != 0)
                    {
                        mensajeResultado.noError = isAnual ? 3 : 11;
                        mensajeResultado.error = isAnual ? "No encontro controladores en la tabla ISR anual" : "No encontro controladores en la tabla ISR";
                        return mensajeResultado;
                    }
                    tablaIsr = construyeTablaXmlPorTipoNomina(tipoTabla, controlador, tipoControladores, fechaFinal, ejercicio, dbContextMaster);
                    mensajeResultado = evaluaContenidoObject(tablaIsr, isAnual ? "No encontro datos en la tabla ISR anual" : "No encontro datos en la tabla ISR",
                        isAnual ? 4 : 12, mensajeResultado);

                    if (mensajeResultado.noError != 0)
                    {
                        return mensajeResultado;
                    }
                    #endregion

                    #region tabla subsidio
                    tipoTabla = ClavesParametrosModulos.claveTipoTablaSubsidio.ToString();
                    if (modoAjustarIngresosMes == ProporcionaPeriodoConTablaAnual)
                    {
                        tipoTabla = ClavesParametrosModulos.claveTipoTablaSUBSIDIOAnual.ToString();
                    }
                    tipoControladores = obtieneTipoControladorTablaBase(tipoTabla, tablaBaseSistema);

                    if (mensajeResultado.noError != 0)
                    {
                        mensajeResultado.noError = isAnual ? 5 : 13;
                        mensajeResultado.error = isAnual ? "No encontro controladores en la tabla Subsidio anual" : "No encontro controladores en la tabla Subsidio";
                        return mensajeResultado;
                    }
                    tablaSubsidio = construyeTablaXmlPorTipoNomina(tipoTabla, controlador, tipoControladores, fechaFinal, ejercicio, dbContextMaster);
                    mensajeResultado = evaluaContenidoObject(tablaSubsidio, isAnual ? "No encontro datos en la tabla Subsidio anual" : "No encontro datos en la tabla Subsidio",
                        isAnual ? 6 : 14, mensajeResultado);

                    if (mensajeResultado.noError != 0)
                    {
                        return mensajeResultado;
                    }
                    #endregion

                  
                }
                #region tabla zona salarial
                tipoControladores = obtieneTipoControladorTablaBase(ClavesParametrosModulos.claveTipoTablaZonaZalarial.ToString(), tablaBaseSistema);

                if (mensajeResultado.noError != 0)
                {
                    mensajeResultado.noError = 15;
                    mensajeResultado.error = "No encontro controladores en la tabla zona salarial";
                    return mensajeResultado;
                }
                tablaZonaSalarial = construyeTablaXmlPorTipoNomina(ClavesParametrosModulos.claveTipoTablaZonaZalarial.ToString(), controlador, tipoControladores, fechaFinal, ejercicio, dbContextMaster);
                mensajeResultado = evaluaContenidoObject(tablaZonaSalarial, "No encontro datos en la tabla zona salarial", 16, mensajeResultado);

                if (mensajeResultado.noError != 0)
                {
                    return mensajeResultado;
                }
                #endregion
                #region tabla UMA
                if (isUMA)
                {
                    tipoControladores = obtieneTipoControladorTablaBase(ClavesParametrosModulos.claveTipoTablaUMA.ToString(), tablaBaseSistema);
                    object[,] tablaUMA = construyeTablaXmlPorTipoNomina(ClavesParametrosModulos.claveTipoTablaUMA.ToString(), controlador, tipoControladores, fechaFinal, ejercicio, dbContextMaster);
                    mensajeResultado = evaluaContenidoObject(tablaUMA, "No encontro datos en la tabla UMA", 17, mensajeResultado);

                    if (mensajeResultado.noError != 0)
                    {
                        return mensajeResultado;
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
                            //for (int i = 0; i < tablaUMA.Length; i++)
                            //{
                            //    if (String.IsNullOrEmpty(tablaUMA[i, 0].ToString().Trim()))
                            //    {
                            //        valorUMA = null;
                            //    }
                            //    else
                            //    {
                            //        valorUMA = Convert.ToDouble(tablaUMA[i, 1].ToString());
                            //       // valorUMA = Double.Parse(tablaUMA[i, 0].ToString());
                            //    }
                            //    break;
                            //}
                            valorUMA = Convert.ToDouble(tablaUMA[0,1].ToString());
                            valorUMI = Convert.ToDouble(tablaUMA[0,3].ToString());

                            if (valorUMA == null)
                            {
                                mensajeResultado.noError = 17;
                                mensajeResultado.error = "No encontro datos en la tabla UMA";
                            }
                        }
                    }
                }
                #endregion
                #region tabla factor de integracion
                tipoControladores = obtieneTipoControladorTablaBase(ClavesParametrosModulos.claveTipoTablaFactorIntegracion.ToString(), tablaBaseSistema);
                if (mensajeResultado.noError != 0)
                {
                    mensajeResultado.noError = 18;
                    mensajeResultado.error = "no encontro controladores en la tabla factor de integracion";
                    return mensajeResultado;
                }
                tablaFactorIntegracion = construyeTablaXmlPorTipoNomina(ClavesParametrosModulos.claveTipoTablaFactorIntegracion.ToString(), controlador, tipoControladores, fechaFinal, ejercicio, dbContextMaster);
                mensajeResultado = evaluaContenidoObject(tablaFactorIntegracion, "No encontro datos en la tabla factor de integracion", 19, mensajeResultado);

                if (mensajeResultado.noError != 0)
                {
                    return mensajeResultado;
                }
                #endregion
                
                tipoControladores = obtieneTipoControladorTablaBase(ClavesParametrosModulos.claveTipoTablaISR.ToString(), tablaBaseSistema);
                tablaIsrMes = construyeTablaXmlPorTipoNomina(ClavesParametrosModulos.claveTipoTablaISR.ToString(), controlador, tipoControladores, fechaFinal, ejercicio, dbContextMaster);
                tipoControladores = obtieneTipoControladorTablaBase(ClavesParametrosModulos.claveTipoTablaSubsidio.ToString(), tablaBaseSistema);
                tablaSubsidioMes= construyeTablaXmlPorTipoNomina(ClavesParametrosModulos.claveTipoTablaSubsidio.ToString(), controlador, tipoControladores, fechaFinal, ejercicio, dbContextMaster);

                tablaDatosXml = (from t in dbContextMaster.Set<TablaDatos>() select t).ToList();

                dbContextMaster.Database.Connection.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("generaTablasXml()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                dbContextMaster.Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        private Mensaje evaluaContenidoObject(object[,] valor, string mensajeError, int noError, Mensaje mensaje)
        {
            if (valor == null & mensaje.noError == 0)
            {
                mensajeResultado.noError = noError;
                mensajeResultado.error = mensajeError;
                mensajeResultado.resultado = null;
            }
            return mensaje;
        }
        #region carga tablas isr, subsidio, factor integracion, zona salarial xml de sistema
        private List<TablaBase> getCargarTablaBaseSistema(DbContext dbContextMaster)
        {
            List<TablaBase> listTablaBase = null;
            DbContext context = dbContextMaster;
            context.Database.BeginTransaction();
            try
            {
                listTablaBase = (from tb in context.Set<TablaBase>()
                                 where tb.tipoTabla.sistema == true
                                 select tb).ToList();
                context.Database.Connection.Close();
            }
            catch (Exception ex)
            {
                //mensajeResultado.noError = 2;
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                // mensajeResultado.error = "Error al cargar Tablas base del sistema";
                mensajeResultado.resultado = 0;
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
                tipoControladores.Add((TipoControlador)ManejadorEnum.GetValue(item, typeof(TipoControlador)));
            }
            return tipoControladores;
        }

        private object[,] construyeTablaXmlPorTipoNomina(string claveTablaBase, string controlador, List<TipoControlador> tipoControladores, DateTime fechaFinal, int ejercicio, DBContextMaster dbContext)
        {
            object[,] valores = null;
            controlador = (controlador == null) ? "" : controlador;
            DbContext Context = dbContext;
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
                    //  int maxId = (from t_query in subQueryMaxID select new { t_query.t.id }).Max(p => p.id);
                    var queryPrincipal = (from t_quey in subQueryMaxID orderby t_quey.t.id descending select t_quey.t.fileXml).Skip(0).Take(1).ToArray();
                   // var queryPrincipal = (from t in Context.Set<TablaDatos>() where t.id == maxId select t.fileXml);
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
                    valores = construyeTablaXmlPorTipoNomina(claveTablaBase, controlador, tipoControladores, fechaActual, fechaActual.Year, dbContext);
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("construyeTablaXmlPorTipoNomina()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return valores;
        }
        #endregion

        #region Metodos busqueda de parametros
        public void obtenerFactores(String claveRazonSocial, Periodicidad periodicidadTipoNomina, DbContext dbContext)
        {
            try
            {
                tipoTablaISR = TipoTablaISR.NORMAL;
                decimal[] valores = new decimal[]{
                     (decimal)   ClavesParametrosModulos.claveParametroFactorAplicaciónTablaAnual,
                      (decimal)  ClavesParametrosModulos.claveParametroPagosPorHora,
                      (decimal)  ClavesParametrosModulos.claveParametroManejarHorasPor,
                      (decimal)  ClavesParametrosModulos.claveParametroManejarSalarioDiarioPor,
                      (decimal)  ClavesParametrosModulos.claveParametroFactorAplicaciónTablaMensual,
                      (decimal)  ClavesParametrosModulos.claveParametroModoAjustarIngresosAlMes,
                       (decimal) ClavesParametrosModulos.claveParametroDesgloseInternoISR,
                       (decimal) ClavesParametrosModulos.clavePagarNominaDiasNaturales,
                       (decimal) ClavesParametrosModulos.claveParametroVersionCalculoPrestamoAhorro,
                      (decimal)  ClavesParametrosModulos.claveParametroPagarIMSSDiasNaturales,
                       (decimal) ClavesParametrosModulos.claveParametroDescontarFaltasModoAjusteMes,
                      (decimal)  ClavesParametrosModulos.claveParametroPagarPrimayVacacionesAuto,
                      (decimal)  ClavesParametrosModulos.claveParametroSalarioUtilizarVacaciones,
                       (decimal) ClavesParametrosModulos.claveParametroUsaUMA};
                List<object[]> listParametros;
               
                mensajeResultado = getParametrosYListCrucePorModuloYClaves((String)ClavesParametrosModulos.claveModuloGlobal, valores, dbContext);
            
                if (mensajeResultado.noError == 0)
                {
                    listParametros = (List<object[]>)mensajeResultado.resultado;
                }
                else
                {
                    return;
                }
                Object[] parametroManejarSalarioDiarioPor = null;
                DesgloseInternoISR desgloseInternoISR = DesgloseInternoISR.DESGLOSEISRNORMALANUAL;
                for (int i = 0; i < listParametros.Count; i++)
                {
                    if (((Parametros)(listParametros[i])[0]).clave == (decimal)ClavesParametrosModulos.claveParametroFactorAplicaciónTablaAnual)
                    {
                        if (factorAnual == null ? true : factorAnual == 0)
                        {
                       
                            factorAnual = parametroFactorAplicacionTablaAnual((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
                            
                        }
                    }
                    else if (((Parametros)(listParametros[i])[0]).clave == (decimal)ClavesParametrosModulos.claveParametroPagosPorHora)
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerFactores()1_Error: ").Append(ex));
                mensajeResultado.resultado = null;
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
        }

        private Mensaje getParametrosYListCrucePorModuloYClaves(String claveModulo, decimal[] clavesParametros, DbContext dbContextMaster)
        {
            List<object[]> listParametrosYListCruce = new List<object[]>();
            DbContext dbContext = dbContextMaster;
            try
            {
                var query = (from pr in dbContext.Set<Parametros>()
                             //join m in dbContext.Set<Modulo>() on pr.modulo_ID equals m.id
                             where clavesParametros.Contains(pr.clave) && pr.modulo.clave.Equals(claveModulo)
                             select pr
                     ).ToList();
                List<Parametros> listParametros = query.ToList<Parametros>();
                List<Cruce> values;
                if ((listParametros != null) && (listParametros.Any()))
                {
                    
                    for (int i = 0; i < listParametros.Count; i++)
                    {
                        if ((listParametros[i].elementosAplicacion != null) && (listParametros[i].elementosAplicacion.Any()))
                        {
                            int conta = listParametros[i].elementosAplicacion.Select(p => p.cruce).Select(a => a.Count).FirstOrDefault();
                            if (conta > 0)
                            {
                                List<string> idsElementos = (from ele in listParametros[i].elementosAplicacion select ele.clave).ToList();
                                decimal clave = listParametros[i].clave;
                                var queryCruce = (from cr in dbContext.Set<Cruce>()

                                                  where cr.parametros.clave == clave && idsElementos.Contains(cr.elementosAplicacion.clave)
                                                  select cr
                                );
                                values = queryCruce.ToList<Cruce>();
                            }
                            else {
                                values = new List<Cruce>();
                            }
                        }
                        else
                        {
                            values = new List<Cruce>();
                        }
                        object[] objects = new object[2];

                        objects[0] = listParametros[i];
                        objects[1] = values;
                        listParametrosYListCruce.Add(objects);
                    }
                }
                mensajeResultado.resultado = listParametrosYListCruce;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                dbContext.Database.Connection.Close();
            }
            catch (Exception ex)
            {
                mensajeResultado.resultado = null;
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return mensajeResultado;
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

        public double parametroFactorAplicacionTablaAnual(Parametros parametros, List<Cruce> listCruces, List<ValoresElementosAplicacion> listValoresElementosAplicacion)
        {
            double topeHorasDoblesDiario = 3.0;
            string valorParametros = (string)valorParametroCruce(parametros, listCruces, true, listValoresElementosAplicacion);
            topeHorasDoblesDiario = Double.Parse(valorParametros);
            return topeHorasDoblesDiario;
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
        #endregion

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