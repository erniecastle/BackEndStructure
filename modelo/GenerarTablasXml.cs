using Exitosw.Payroll.Core.metodosCalculoNomina;
using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Entity.util;
using Exitosw.Payroll.TestCompilador.funciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exitosw.Payroll.Core.modelo
{
    public class GenerarTablasXml
    {
        private Mensaje mensajeResultado = new Mensaje();
        private string claveTipoNomina;
        private string claveTipoCorrida;
        private decimal? idPeriodoNomina;
        private static int ProporcionaPeriodoIndependiente = 1, ProporcionaPeriodoAjustadoFinalMes = 2, ProporcionaPeriodoAjustadoCadaVez = 3,
               ProporcionaPeriodoConTablaAnual = 4, ProporcionaCadaPeriodoUtilizandoTablaPeriodo = 5, PropPeriodoIndepAjustadoAlUltimoPeriodoMes = 6, UltimoPeriodoSinAjustarMes = 7,
               ProporcionaTablaAnual = 8, PropPeriodoIndepDiasNaturales = 9;
        private Periodicidad periodicidadTipoNomina;
        private DBContextAdapter dbContextSimple;
        private DBContextAdapter dbContextMaestra;
        public GenerarTablasXml()
        {

        }
        public GenerarTablasXml(string claveTipoNomina, string claveTipoCorrida, decimal? idPeriodoNomina, DBContextAdapter dbContextSimple, DBContextAdapter dbContextMaestra)
        {
            this.idPeriodoNomina = idPeriodoNomina;
            this.claveTipoCorrida = claveTipoCorrida;
            this.claveTipoNomina = claveTipoNomina;
            this.dbContextSimple = dbContextSimple;
            this.dbContextMaestra = dbContextMaestra;
        }

        public Mensaje generaTablasXml(String controlador, String claveRazonSocial, DateTime fechaFinal, int ejercicio)
        {
            try
            {
                DatosTablaXml datosTablaXml = new DatosTablaXml();
                mensajeResultado = buscaPeriodicidadesOrPeriodosNomina();
                if (mensajeResultado.noError == 0)
                {
                    periodicidadTipoNomina = new Periodicidad();
                    periodicidadTipoNomina = (Periodicidad)mensajeResultado.resultado;
                }
                else
                {
                    return mensajeResultado;
                }

                MetodosBDMaestra metodos = new MetodosBDMaestra(DateTime.Now);
                metodos.generaTablasXml(controlador, periodicidadTipoNomina, claveRazonSocial, fechaFinal, ejercicio, (DBContextMaster)dbContextMaestra.context);
                if (metodos.mensajeResultado.noError == 0)
                {
                    datosTablaXml.calculoSeparadoISR = metodos.calculoSeparadoISR;
                    datosTablaXml.descontarFaltasModoAjustaMes = metodos.descontarFaltasModoAjustaMes;
                    datosTablaXml.factorAnual = metodos.factorAnual;
                    datosTablaXml.factorMensual = metodos.factorMensual;
                    datosTablaXml.isUMA = metodos.isUMA;
                    datosTablaXml.manejaPagoDiasNaturales = metodos.manejaPagoDiasNaturales;
                    datosTablaXml.manejaPagoIMSSDiasNaturales = metodos.manejaPagoIMSSDiasNaturales;
                    datosTablaXml.manejaPagosPorHora = metodos.manejaPagosPorHora;
                    datosTablaXml.manejoHorasPor = metodos.manejoHorasPor;
                    datosTablaXml.manejoSalarioDiario = metodos.manejoSalarioDiario;
                    datosTablaXml.modoAjustarIngresosMes = metodos.modoAjustarIngresosMes;
                    datosTablaXml.pagarVacaAuto = metodos.pagarVacaAuto;
                    datosTablaXml.salarioVacaciones = metodos.salarioVacaciones;
                    datosTablaXml.tablaDatosXml = metodos.tablaDatosXml;
                    datosTablaXml.tablaFactorIntegracion = metodos.tablaFactorIntegracion;
                    datosTablaXml.tablaIsr = metodos.tablaIsr;
                    datosTablaXml.tablaSubsidio = metodos.tablaSubsidio;
                    datosTablaXml.tablaZonaSalarial = metodos.tablaZonaSalarial;
                    datosTablaXml.tipoTablaISR = metodos.tipoTablaISR;
                    datosTablaXml.tablaIsrMes = metodos.tablaIsrMes;
                    datosTablaXml.tablaSubsidioMes = metodos.tablaSubsidioMes;
                    datosTablaXml.valorUMA = metodos.valorUMA;
                    datosTablaXml.valorUMI = metodos.valorUMI;
                    datosTablaXml.versionCalculoPrestamoAhorro = metodos.versionCalculoPrestamoAhorro;
                    datosTablaXml.filtroConceptosNomina = buscaConceptosTipoAutomatico();
                }
                else {
                    return mensajeResultado;
                }
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                mensajeResultado.resultado = datosTablaXml;
            }
            catch (Exception ex)
            {

                mensajeResultado.error = ex.GetBaseException().Message;
                mensajeResultado.noError = 1000;
                mensajeResultado.resultado = null;
            }
            return mensajeResultado;
        }
        private List<ConcepNomDefi> buscaConceptosTipoAutomatico()
        {
            List<ConcepNomDefi> listConceptos = null;
            try
            {
                listConceptos = (from cdn in dbContextSimple.context.Set<ConcepNomDefi>()
                                 join ctc in dbContextSimple.context.Set<ConceptoPorTipoCorrida>() on cdn.id equals ctc.concepNomDefi_ID
                                 where ctc.tipoCorrida.clave == claveTipoCorrida && cdn.activado == true && cdn.tipo == Tipo.AUTOMATICO &&
                                    cdn.fecha == (from c in dbContextSimple.context.Set<ConcepNomDefi>() where c.clave == cdn.clave select new { c.fecha }).Max(f => f.fecha)
                                 select cdn).ToList();
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscaConceptosTipoAutomatico()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return listConceptos == null ? new List<ConcepNomDefi>() : listConceptos;
        }


        //public Mensaje obtenerFactores(string claveRazonSocial)
        //{
        //    try
        //    {
        //        DatosTablaXml datosTablaXml = new DatosTablaXml();
        //        datosTablaXml.tipoTablaISR = TipoTablaISR.NORMAL;
        //        decimal[] valores = new decimal[]{
        //             (decimal)   ClavesParametrosModulos.claveParametroFactorAplicaciónTablaAnual,
        //              (decimal)  ClavesParametrosModulos.claveParametroPagosPorHora,
        //              (decimal)  ClavesParametrosModulos.claveParametroManejarHorasPor,
        //              (decimal)  ClavesParametrosModulos.claveParametroManejarSalarioDiarioPor,
        //              (decimal)  ClavesParametrosModulos.claveParametroFactorAplicaciónTablaMensual,
        //              (decimal)  ClavesParametrosModulos.claveParametroModoAjustarIngresosAlMes,
        //               (decimal) ClavesParametrosModulos.claveParametroDesgloseInternoISR,
        //               (decimal) ClavesParametrosModulos.clavePagarNominaDiasNaturales,
        //               (decimal) ClavesParametrosModulos.claveParametroVersionCalculoPrestamoAhorro,
        //              (decimal)  ClavesParametrosModulos.claveParametroPagarIMSSDiasNaturales,
        //               (decimal) ClavesParametrosModulos.claveParametroDescontarFaltasModoAjusteMes,
        //              (decimal)  ClavesParametrosModulos.claveParametroPagarPrimayVacacionesAuto,
        //              (decimal)  ClavesParametrosModulos.claveParametroSalarioUtilizarVacaciones,
        //               (decimal) ClavesParametrosModulos.claveParametroUsaUMA};
        //        List<object[]> listParametros;

        //        mensajeResultado = getParametrosYListCrucePorModuloYClaves((String)ClavesParametrosModulos.claveModuloGlobal, valores);

        //        if (mensajeResultado.noError == 0)
        //        {
        //            listParametros = (List<object[]>)mensajeResultado.resultado;
        //        }
        //        else
        //        {
        //            return mensajeResultado;
        //        }

        //        mensajeResultado = buscaPeriodicidadesOrPeriodosNomina();
        //        if (mensajeResultado.noError == 0)
        //        {
        //            periodicidadTipoNomina = new Periodicidad();
        //            periodicidadTipoNomina = (Periodicidad)mensajeResultado.resultado;
        //        }
        //        else
        //        {
        //            return mensajeResultado;
        //        }
        //        Object[] parametroManejarSalarioDiarioPor = null;
        //        DesgloseInternoISR desgloseInternoISR = DesgloseInternoISR.DESGLOSEISRNORMALANUAL;
        //        for (int i = 0; i < listParametros.Count; i++)
        //        {
        //            if (((Parametros)(listParametros[i])[0]).clave == (decimal)ClavesParametrosModulos.claveParametroFactorAplicaciónTablaAnual)
        //            {
        //                if (datosTablaXml.factorAnual == null ? true : datosTablaXml.factorAnual == 0)
        //                {

        //                    datosTablaXml.factorAnual = parametroFactorAplicacionTablaAnual((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });

        //                }
        //            }
        //            else if (((Parametros)(listParametros[i])[0]).clave == (decimal)ClavesParametrosModulos.claveParametroPagosPorHora)
        //            {
        //                if (datosTablaXml.manejaPagosPorHora == null)
        //                {
        //                    Object[] objects = parametroPagosPorHora((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
        //                    datosTablaXml.manejaPagosPorHora = (bool)objects[0];
        //                }
        //            }
        //            else if (((Parametros)(listParametros[i])[0]).clave == (decimal)ClavesParametrosModulos.claveParametroManejarHorasPor)
        //            {
        //                if (datosTablaXml.manejoHorasPor == null)
        //                {
        //                    datosTablaXml.manejoHorasPor = parametroManejarHorasPor((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
        //                }
        //            }
        //            else if (((Parametros)(listParametros[i])[0]).clave == (decimal)ClavesParametrosModulos.claveParametroManejarSalarioDiarioPor)
        //            {
        //                parametroManejarSalarioDiarioPor = listParametros[i];
        //            }
        //            else if (((Parametros)(listParametros[i])[0]).clave == (decimal)ClavesParametrosModulos.claveParametroFactorAplicaciónTablaMensual)
        //            {
        //                datosTablaXml.factorMensual = parametroFactorAplicacionTablaMensual((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
        //            }
        //            else if (((Parametros)(listParametros[i])[0]).clave == (decimal)ClavesParametrosModulos.claveParametroModoAjustarIngresosAlMes)
        //            {
        //                modoAjustarIngresosMes = parametroModoAjustarIngresosAlMes((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
        //                if (modoAjustarIngresosMes == ProporcionaCadaPeriodoUtilizandoTablaPeriodo)
        //                {
        //                    tipoTablaISR = TipoTablaISR.PERIODICIDAD;
        //                }
        //            }
        //            else if (((Parametros)(listParametros[i])[0]).clave == (decimal)ClavesParametrosModulos.claveParametroDesgloseInternoISR)
        //            {
        //                desgloseInternoISR = parametroDesgloseInternoISR((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
        //            }
        //            else if (((Parametros)(listParametros[i])[0]).clave == (decimal)ClavesParametrosModulos.clavePagarNominaDiasNaturales)
        //            {
        //                if (periodicidadTipoNomina != null)
        //                {
        //                    manejaPagoDiasNaturales = pagarNominaDiasNaturales((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial), new ValoresElementosAplicacion(typeof(Periodicidad), periodicidadTipoNomina.clave) });
        //                }
        //                else
        //                {
        //                    manejaPagoDiasNaturales = pagarNominaDiasNaturales((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
        //                }
        //            }
        //            else if (((Parametros)(listParametros[i])[0]).clave == (decimal)ClavesParametrosModulos.claveParametroVersionCalculoPrestamoAhorro)
        //            {
        //                versionCalculoPrestamoAhorro = parametroVersionCalculoPrestamoAhorro((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
        //            }
        //            else if (((Parametros)(listParametros[i])[0]).clave == (decimal)ClavesParametrosModulos.claveParametroPagarIMSSDiasNaturales)
        //            {
        //                manejaPagoIMSSDiasNaturales = pagarIMSSDiasNaturales((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
        //            }
        //            else if (((Parametros)(listParametros[i])[0]).clave == (decimal)ClavesParametrosModulos.claveParametroDescontarFaltasModoAjusteMes)
        //            {
        //                descontarFaltasModoAjustaMes = descuentoFaltasModoAjustaMes((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
        //            }
        //            else if (((Parametros)(listParametros[i])[0]).clave == (decimal)ClavesParametrosModulos.claveParametroPagarPrimayVacacionesAuto)
        //            {
        //                pagarVacaAuto = parametroPagarPrimayVacacionesAuto((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
        //            }
        //            else if (((Parametros)(listParametros[i])[0]).clave == (decimal)ClavesParametrosModulos.claveParametroSalarioUtilizarVacaciones)
        //            {
        //                salarioVacaciones = parametroSalarioVacaciones((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
        //            }
        //            else if (((Parametros)(listParametros[i])[0]).clave == (decimal)ClavesParametrosModulos.claveParametroUsaUMA)
        //            {
        //                isUMA = activarUMA((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
        //            }
        //        }
        //        manejoSalarioDiario = parametroManejadorSalarioDiarioPor((Parametros)parametroManejarSalarioDiarioPor[0], (List<Cruce>)parametroManejarSalarioDiarioPor[1], manejaPagoDiasNaturales, new List<ValoresElementosAplicacion>() { new ValoresElementosAplicacion(typeof(RazonesSociales), claveRazonSocial) });
        //        if (modoAjustarIngresosMes != ProporcionaTablaAnual)
        //        {
        //            if (desgloseInternoISR == DesgloseInternoISR.DESGLOSEISRNORMALANUAL)
        //            {
        //                calculoSeparadoISR = true;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        mensajeResultado.error = "";
        //        mensajeResultado.noError = 0;
        //        mensajeResultado.resultado = null;
        //    }

        //    return mensajeResultado;
        //}

        private Mensaje buscaPeriodicidadesOrPeriodosNomina()
        {
            Periodicidad periodicidad = new Periodicidad();
            try
            {
             
                periodicidad = (from t in dbContextSimple.context.Set<TipoNomina>()
                                where t.clave == claveTipoNomina
                                select t.periodicidad).SingleOrDefault();
                mensajeResultado.error = "";
                mensajeResultado.noError = 0;
                mensajeResultado.resultado = periodicidad;
            }
            catch (Exception ex)
            {

                mensajeResultado.error = ex.GetBaseException().Message;
                mensajeResultado.noError = 1002;
                mensajeResultado.resultado = null;
            }
            return mensajeResultado;
        }

        //private Mensaje getParametrosYListCrucePorModuloYClaves(String claveModulo, decimal[] clavesParametros)
        //{
        //    List<object[]> listParametrosYListCruce = new List<object[]>();
        //    try
        //    {
        //        var query = (from pr in dbContextMaestra.context.Set<Parametros>()
        //                         //join m in dbContext.Set<Modulo>() on pr.modulo_ID equals m.id
        //                     where clavesParametros.Contains(pr.clave) && pr.modulo.clave.Equals(claveModulo)
        //                     select pr
        //             ).ToList();
        //        List<Parametros> listParametros = query.ToList<Parametros>();
        //        List<Cruce> values;
        //        if (listParametros != null && listParametros.Any())
        //        {
        //            for (int i = 0; i < listParametros.Count; i++)
        //            {
        //                if ((listParametros[i].elementosAplicacion != null) && (listParametros[i].elementosAplicacion.Any()))
        //                {
        //                    int conta = listParametros[i].elementosAplicacion.Select(p => p.cruce).Select(c => c.Count).FirstOrDefault();
        //                    if (conta > 0)
        //                    {
        //                        List<string> idsElementos = (from ele in listParametros[i].elementosAplicacion select ele.clave).ToList();
        //                        decimal clave = listParametros[i].clave;
        //                        var queryCruce = (from cr in dbContextMaestra.context.Set<Cruce>()

        //                                          where cr.parametros.clave == clave && idsElementos.Contains(cr.elementosAplicacion.clave)
        //                                          select cr
        //                        );
        //                        values = queryCruce.ToList<Cruce>();
        //                    }
        //                    else
        //                    {
        //                        values = new List<Cruce>();
        //                    }
        //                }
        //                else
        //                {
        //                    values = new List<Cruce>();
        //                }
        //                object[] objects = new object[2];

        //                objects[0] = listParametros[i];
        //                objects[1] = values;
        //                listParametrosYListCruce.Add(objects);
        //            }
        //        }
        //        mensajeResultado.error = "";
        //        mensajeResultado.noError = 0;
        //        mensajeResultado.resultado = listParametrosYListCruce;
        //    }
        //    catch (Exception ex)
        //    {

        //        mensajeResultado.noError = 1002;
        //        mensajeResultado.error = "";
        //        mensajeResultado.resultado = null;

        //    }

        //    return mensajeResultado;
        //}
    }
}
