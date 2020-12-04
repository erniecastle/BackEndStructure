using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Entity.entidad.cfdi;
using Exitosw.Payroll.Core.genericos.campos;
using Exitosw.Payroll.Core.servicios.extras;
using Exitosw.Payroll.Core.util;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using Exitosw.Payroll.Entity.util;
using System.Data.Entity;
using System.Diagnostics;

namespace Exitosw.Payroll.Core.metodosCalculoNomina
{
    public class MetodosParaMovimientosNomina
    {
        private Mensaje mensajeResultado = new Mensaje();
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        private ConectorQuerysGenericos conectorQuerysGenericos = new ConectorQuerysGenericos();
        private List<ConcepNomDefi> filtroConceptosNomina;
        private List<MovNomConcep> filtroMovimientosNominas;
        public int tipoPantallaSistema = 100;
        Stopwatch tiempo = new Stopwatch();

        public MetodosParaMovimientosNomina()
        {

        }
        public MetodosParaMovimientosNomina(List<ConcepNomDefi> filtroConceptosNomina)
        {
            this.filtroConceptosNomina = new List<ConcepNomDefi>();
            this.filtroConceptosNomina = filtroConceptosNomina;
        }
        public Mensaje obtenerMovimientosPlazasFiniquitos(string claveTipoCorrida, String claveTipoNomina, decimal idPeriodoNomina, PlazasPorEmpleado plazaPorEmpleado,
            string claveCtrCosto, string claveRazonSocial, int uso, FiniquitosLiquida finiquitosLiquidaciones, DBContextAdapter dbContextSimple)
        {
            inicializaVariableMensaje();
            List<MovNomConcep> movNomConceptos = null;
            try
            {
                if (finiquitosLiquidaciones == null)
                {
                    var queryMov = from mn in dbContextSimple.context.Set<MovNomConcep>()
                                   where mn.uso == uso && mn.periodosNomina.id == idPeriodoNomina && mn.tipoCorrida.clave == claveTipoCorrida && mn.tipoNomina.clave == claveTipoNomina &&
                                         mn.empleados.clave == plazaPorEmpleado.empleados.clave && mn.razonesSociales.clave == claveRazonSocial && mn.plazasPorEmpleado.referencia == plazaPorEmpleado.referencia
                                   select new { mov = mn };
                    if (!string.IsNullOrEmpty(claveCtrCosto))
                    {
                        queryMov = from sub in queryMov where sub.mov.centroDeCosto.clave == claveCtrCosto select sub;
                    }

                    movNomConceptos = (from sub in queryMov orderby sub.mov.concepNomDefi.prioridadDeCalculo select sub.mov).ToList();
                }
                else
                {
                    movNomConceptos = (from mn in dbContextSimple.context.Set<MovNomConcep>() where mn.finiqLiquidCncNom.finiquitosLiquida.id == finiquitosLiquidaciones.id orderby mn.concepNomDefi.prioridadDeCalculo select mn).ToList();
                }
                mensajeResultado.noError = 0;
                mensajeResultado.resultado = movNomConceptos == null ? new List<MovNomConcep>() : movNomConceptos;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerMovimientosPlazasFiniquitos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = new List<MovNomConcep>();
            }
            return mensajeResultado;
        }

        public Mensaje obtenerMovimientosNominaPorPlaza(TipoCorrida tipoCorrida, string claveTipoNomina, decimal idPeriodoNomina, PlazasPorEmpleado plazaPorEmpleado,
            string claveCtrCosto, string claveRazonSocial, PeriodosNomina periodoNomina, DateTime fechaActual,
             CentroDeCosto centroDeCostoMovimiento, DBContextAdapter dbContextSimple)
        {
            inicializaVariableMensaje();
            filtroMovimientosNominas = new List<MovNomConcep>();
            filtroConceptosNomina = new List<ConcepNomDefi>();
            try
            {
                
                mensajeResultado = buscaMovimientosPlazasPorTipoYBaseAfecta(tipoCorrida, claveTipoNomina, idPeriodoNomina, plazaPorEmpleado, claveCtrCosto, Tipo.AUTOMATICO, claveRazonSocial, null, -1, null, periodoNomina, fechaActual, centroDeCostoMovimiento, (DBContextSimple)dbContextSimple.context);
               
                if (mensajeResultado.noError == 0)
                {
                    filtroMovimientosNominas.AddRange((List<MovNomConcep>)mensajeResultado.resultado);
                }
                else
                {
                    return mensajeResultado;
                }

                filtroConceptosNomina = buscaConceptosTipoAutomatico(tipoCorrida.clave, (DBContextSimple)dbContextSimple.context);

                if (mensajeResultado.noError != 0)
                {
                    return mensajeResultado;
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
                   
                   
                    filtroMovimientosNominas.AddRange(creaMovimientosPlazasConceptosAutomaticos(plazaPorEmpleado, periodoNomina, tipoCorrida, plazaPorEmpleado.razonesSociales, centroDeCostoMovimiento, (DBContextSimple)dbContextSimple.context));
                    if (mensajeResultado.noError != 0)
                    {
                        return mensajeResultado;
                    }
                    
                }
                
                //busca conceptos del periodo en los movimientos
                mensajeResultado = buscaMovimientosPlazasPorTipoYBaseAfecta(tipoCorrida, claveTipoNomina, idPeriodoNomina, plazaPorEmpleado, claveCtrCosto, Tipo.PERIODO, claveRazonSocial, null, -1, null, periodoNomina, fechaActual, centroDeCostoMovimiento, (DBContextSimple)dbContextSimple.context);
                if (mensajeResultado.noError == 0)
                {
                    filtroMovimientosNominas.AddRange((List<MovNomConcep>)mensajeResultado.resultado);
                }
                else
                {
                    return mensajeResultado;
                }
                //busca conceptos repetitivos en los movimientos.
                mensajeResultado = buscaMovimientosPlazasPorTipoYBaseAfecta(tipoCorrida, claveTipoNomina, idPeriodoNomina, plazaPorEmpleado, claveCtrCosto, Tipo.REPETITIVO, claveRazonSocial, null, -1, null, periodoNomina, fechaActual, centroDeCostoMovimiento, (DBContextSimple)dbContextSimple.context);
                if (mensajeResultado.noError == 0)
                {
                    filtroMovimientosNominas.AddRange((List<MovNomConcep>)mensajeResultado.resultado);
                }
                else
                {
                    return mensajeResultado;
                }
                ordenarMovimientosNomina();
                generarMovimientosAbarca2Meses(dbContextSimple);
                filtroConceptosNomina = null;
                mensajeResultado.noError = 0;
                mensajeResultado.resultado = filtroMovimientosNominas;
                
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerMovimientosNominaPorPlaza()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = new List<MovNomConcep>();
            }
            return mensajeResultado;
        }

        public Mensaje obtenerMovimientosNominaCreditoAhorro(List<CreditoAhorro> listCreditoAhorro, List<MovNomConcep> filtroMovimientosNominas)
        {
            try
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
                        if (String.Equals(listCreditoAhorro[i].concepNomiDefin.clave, filtroMovimientosNominas[z].concepNomDefi.clave, StringComparison.OrdinalIgnoreCase))
                        {
                            listMovNomConcepCreditosAhorro.Add(filtroMovimientosNominas[z]);
                            filtroMovimientosNominas.Remove(filtroMovimientosNominas[z]);
                        }
                        else if (listCreditoAhorro[i].activarManejoDescuento)
                        {
                            if (String.Equals(listCreditoAhorro[i].cNDescuento.clave, filtroMovimientosNominas[z].concepNomDefi.clave, StringComparison.OrdinalIgnoreCase))
                            {
                                if (!String.Equals(listCreditoAhorro[i].cNDescuento.clave, listCreditoAhorro[i].concepNomiDefin.clave, StringComparison.OrdinalIgnoreCase))
                                {
                                    listMovNomConcepCreditosAhorroDescuentoActivo.Add(filtroMovimientosNominas[z]);
                                    filtroMovimientosNominas.Remove(filtroMovimientosNominas[z]);
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
                                for (j = 0; j < listCreditoAhorro.Count; j++)
                                {
                                    if (String.Equals(listCreditoAhorro[j].concepNomiDefin.clave, filtroMovimientosNominas[z].concepNomDefi.clave, StringComparison.OrdinalIgnoreCase))
                                    {
                                        asignadoACredito = true;
                                        break;
                                    }
                                }
                                if (!asignadoACredito)
                                {
                                    listMovNomConcepFormulaDeducCreditos.Add(filtroMovimientosNominas[z]);
                                    filtroMovimientosNominas.Remove(filtroMovimientosNominas[z]);
                                }
                                else
                                {
                                    z++;
                                }
                            }
                            else if (filtroMovimientosNominas[z].concepNomDefi.formulaConcepto.ToUpper().StartsWith("DEDUCAHORROS"))
                            {
                                asignadoACredito = false;
                                for (j = 0; j < listCreditoAhorro.Count; j++)
                                {
                                    if (String.Equals(listCreditoAhorro[j].concepNomiDefin.clave, filtroMovimientosNominas[z].concepNomDefi.clave, StringComparison.OrdinalIgnoreCase))
                                    {
                                        asignadoACredito = true;
                                        break;
                                    }
                                }
                                if (!asignadoACredito)
                                {
                                    listMovNomConcepFormulaDeducAhorros.Add(filtroMovimientosNominas[z]);
                                    filtroMovimientosNominas.Remove(filtroMovimientosNominas[z]);
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
                listMovTmp[4] = filtroMovimientosNominas;
                mensajeResultado.noError = 0;
                mensajeResultado.resultado = listMovTmp;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerMovimientosNominaCreditoAhorro()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return mensajeResultado;
        }

        private void ordenarMovimientosNomina()
        {
            filtroMovimientosNominas = (from list in filtroMovimientosNominas orderby list.concepNomDefi.prioridadDeCalculo select list).ToList();
        }

        public Mensaje buscaMovimientosPlazasPorTipoYBaseAfecta(TipoCorrida tipoCorrida, string claveTipoNomina, decimal idPeriodoNomina, PlazasPorEmpleado plazaPorEmpleado,
            string claveCtrCosto, Tipo tipo, string claveRazonSocial, string claveBaseNomina, int tipoBaseAfecta, string claveConcepto, PeriodosNomina periodoNomina, DateTime fechaActual,
            CentroDeCosto centroDeCostoMovimiento, DBContextSimple dbContextSimple)
        {
            
            List<MovNomConcep> movNomConceptos = null;
            try
            {
                string claveTipoCorrida = tipoCorrida.clave;
                var subQuerypMov = from mn in dbContextSimple.Set<MovNomConcep>()
                                   where mn.uso == 0 && mn.tipoCorrida.clave == claveTipoCorrida && mn.tipoNomina.clave == claveTipoNomina && mn.empleados.clave == plazaPorEmpleado.empleados.clave &&
                                       mn.razonesSociales.clave == claveRazonSocial && mn.plazasPorEmpleado.referencia == plazaPorEmpleado.referencia
                                   select new { mn };
                //si no usa tipo de concepto
                if (tipo == Tipo.NINGUNO)
                {
                    subQuerypMov = (from sub in subQuerypMov
                                    where sub.mn.periodosNomina.id == idPeriodoNomina && sub.mn.periodosNomina.tipoCorrida.clave == claveTipoCorrida
                                    select sub
                    );
                }
                else if (tipo == Tipo.REPETITIVO)
                {
                    subQuerypMov = (from sub in subQuerypMov
                                    where sub.mn.periodosNomina.tipoCorrida.clave == claveTipoCorrida && sub.mn.concepNomDefi.tipo == tipo
                                    select sub
                    );
                }
                else
                {
                    subQuerypMov = (from sub in subQuerypMov
                                    where sub.mn.periodosNomina.id == idPeriodoNomina && sub.mn.periodosNomina.tipoCorrida.clave == claveTipoCorrida && sub.mn.concepNomDefi.tipo == tipo
                                    select sub
                    );
                }

                if (!String.IsNullOrEmpty(claveCtrCosto))
                {
                    subQuerypMov = (from sub in subQuerypMov
                                    join cc in dbContextSimple.Set<CentroDeCosto>() on sub.mn.centroDeCosto_ID equals cc.id into cc_join
                                    from cc in cc_join.DefaultIfEmpty()
                                    where cc.clave == claveCtrCosto
                                    select sub
                    );
                }

                if (!String.IsNullOrEmpty(claveConcepto))
                {
                    subQuerypMov = (from sub in subQuerypMov
                                    where sub.mn.concepNomDefi.clave == claveConcepto
                                    select sub
                    );
                }


                if (tipo == Tipo.REPETITIVO)
                {
                    subQuerypMov = (from sub in subQuerypMov
                                    where (sub.mn.periodosNomina.id <= idPeriodoNomina && sub.mn.periodosNomina.año == periodoNomina.año && sub.mn.periodosNomina.tipoCorrida.clave == claveTipoCorrida)
                                    orderby sub.mn.concepNomDefi.id, sub.mn.periodosNomina.id
                                    select sub
                    );
                }

                if (claveBaseNomina != null && tipoBaseAfecta > -1)
                {
                    subQuerypMov = (from sub in subQuerypMov
                                    join mna in dbContextSimple.Set<MovNomBaseAfecta>() on sub.mn.id equals mna.movNomConcep.id
                                    join bac in dbContextSimple.Set<BaseAfecConcepNom>() on mna.baseAfecConcepNom_ID equals bac.id
                                    join bn in dbContextSimple.Set<BaseNomina>() on bac.baseNomina_ID equals bn.id
                                    where bn.clave == claveBaseNomina && bac.tipoAfecta == tipoBaseAfecta
                                    select sub

                            );
                }
                
                movNomConceptos = (from sub in subQuerypMov select sub.mn).ToList<MovNomConcep>();
                
                movNomConceptos = movNomConceptos == null ? new List<MovNomConcep>() : movNomConceptos;
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
                                            if (movRepetitivo.periodosNomina.id == periodoNomina.id)
                                            {
                                                if (movimientosRepetitivos[x].periodosNomina.id == periodoNomina.id)
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
                                            if (movRepetitivo.periodosNomina.id == periodoNomina.id)
                                            {
                                                if (movimientosRepetitivos[x].periodosNomina.id == periodoNomina.id)
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
                            if (movNomConceptos[i].periodosNomina.id != periodoNomina.id)
                            {
                                MovNomConcep mnc = movNomConceptos[i];
                                movNomConceptos[i] = creaMovNomConcep(movNomConceptos[i].concepNomDefi, plazaPorEmpleado, periodoNomina, tipoCorrida, plazaPorEmpleado.razonesSociales, centroDeCostoMovimiento, dbContextSimple);
                                if (mensajeResultado.noError != 0)
                                {
                                    return mensajeResultado;
                                }
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
                mensajeResultado.noError = 0;
                mensajeResultado.resultado = movNomConceptos;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscaMovimientosPlazasPorTipoYBaseAfecta()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return mensajeResultado;
        }

        private MovNomConcep creaMovNomConcep(ConcepNomDefi concepNomDefi, PlazasPorEmpleado plazaPorEmpleado, PeriodosNomina periodosNominas, TipoCorrida tipoCorrida, RazonesSociales razonesSociales, CentroDeCosto centroDeCostoMovimiento, DBContextSimple dbContextSimple)
        {
            MovNomConcep movNomConcep = new MovNomConcep();
            try
            {
                DateTime fechaPeriodo = DateTime.Now;
                movNomConcep.empleados = plazaPorEmpleado.empleados;
                movNomConcep.plazasPorEmpleado_ID = plazaPorEmpleado.id;
                movNomConcep.plazasPorEmpleado = plazaPorEmpleado;
                movNomConcep.periodosNomina_ID = periodosNominas.id;
                movNomConcep.periodosNomina = periodosNominas;
                movNomConcep.concepNomDefi = concepNomDefi;
                movNomConcep.concepNomDefi_ID = concepNomDefi.id;
                movNomConcep.tipoCorrida = tipoCorrida;
                movNomConcep.tipoCorrida_ID = tipoCorrida.id;
                movNomConcep.tipoNomina = periodosNominas.tipoNomina;
                movNomConcep.tipoNomina_ID = periodosNominas.tipoNomina.id;
                movNomConcep.centroDeCosto = centroDeCostoMovimiento;
                movNomConcep.centroDeCosto_ID = centroDeCostoMovimiento.id;
                movNomConcep.razonesSociales = razonesSociales;
                movNomConcep.razonesSociales_ID = razonesSociales.id;
                if (concepNomDefi.baseAfecConcepNom != null)
                {
                    mensajeResultado = creaMovimBaseAfectar(concepNomDefi.baseAfecConcepNom, movNomConcep, dbContextSimple);
                    if (mensajeResultado.noError == 0)
                    {
                        movNomConcep.movNomBaseAfecta = (List<MovNomBaseAfecta>)mensajeResultado.resultado;
                    }
                    else
                    {
                        return movNomConcep;
                    }
                }

                if (movNomConcep.concepNomDefi.paraConcepDeNom == null ? false : movNomConcep.concepNomDefi.paraConcepDeNom.Count > 0)
                {
                    mensajeResultado = creaMovNomConceParam(movNomConcep.concepNomDefi, movNomConcep, dbContextSimple);
                    if (mensajeResultado.noError == 0)
                    {
                        movNomConcep.movNomConceParam = (List<MovNomConceParam>)mensajeResultado.resultado;
                    }
                    else
                    {
                        return movNomConcep;
                    }
                }
                movNomConcep.fechaCierr = periodosNominas.fechaCierre;
                movNomConcep.fechaIni = periodosNominas.fechaInicial;
                movNomConcep.tipoPantalla = tipoPantallaSistema;
                movNomConcep.ordenId = 0;
                movNomConcep.resultado = 0.0;
                movNomConcep.numero = 1;
                movNomConcep.calculado = 0.0;
                fechaPeriodo = movNomConcep.periodosNomina.fechaInicial.GetValueOrDefault();
                movNomConcep.ejercicio = periodosNominas.año.GetValueOrDefault();
                movNomConcep.mes = fechaPeriodo.Month;
                movNomConcep.numMovParticion = 1;
                movNomConcep.uso = 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("creaMovNomConcep()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }

            return movNomConcep;
        }

        //List<MovNomBaseAfecta>
        public Mensaje creaMovimBaseAfectar(List<BaseAfecConcepNom> baseAfecConcepNominas, MovNomConcep mnc, DBContextSimple dbContextSimple)
        {
            List<MovNomBaseAfecta> movNominaBaseAfectas = new List<MovNomBaseAfecta>();
            try
            {
                MovNomBaseAfecta m;
                if (mnc.movNomBaseAfecta == null ? true : mnc.movNomBaseAfecta.Count == 0)
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
                else if (baseAfecConcepNominas.Count == 0)
                {
                    if (mnc.movNomBaseAfecta.Count > 0)
                    {
                        for (int j = 0; j < mnc.movNomBaseAfecta.Count; j++)
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
                    for (int i = 0; i < baseAfecConcepNominas.Count; i++)
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
                    if (movNominaBaseAfectasTmp.Count > 0)
                    {
                        for (int j = 0; j < movNominaBaseAfectasTmp.Count; j++)
                        {
                            dbContextSimple.Set<MovNomBaseAfecta>().Attach(movNominaBaseAfectasTmp[j]);
                            dbContextSimple.Set<MovNomBaseAfecta>().Remove(movNominaBaseAfectasTmp[j]);
                        }
                    }
                }
                mensajeResultado.noError = 0;
                mensajeResultado.resultado = movNominaBaseAfectas;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("creaMovimBaseAfectar()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return mensajeResultado;
        }

        //List<MovNomConceParam>
        public Mensaje creaMovNomConceParam(ConcepNomDefi concepNomDefi, MovNomConcep mnc, DBContextSimple dbContextSimple)
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


        private List<ConcepNomDefi> buscaConceptosTipoAutomatico(String claveTipoCorrida, DBContextSimple dbContextSimple)
        {
            List<ConcepNomDefi> listConceptos = null;
            try
            {
                listConceptos = (from cdn in dbContextSimple.Set<ConcepNomDefi>()
                                 join ctc in dbContextSimple.Set<ConceptoPorTipoCorrida>() on cdn.id equals ctc.concepNomDefi_ID
                                 where ctc.tipoCorrida.clave == claveTipoCorrida && cdn.activado == true && cdn.tipo == Tipo.AUTOMATICO &&
                                    cdn.fecha == (from c in dbContextSimple.Set<ConcepNomDefi>() where c.clave == cdn.clave select new { c.fecha }).Max(f => f.fecha)
                                 select cdn).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscaConceptosTipoAutomatico()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return listConceptos == null ? new List<ConcepNomDefi>() : listConceptos;
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

        //Modificado usando plazas y razon social
        private List<MovNomConcep> creaMovimientosPlazasConceptosAutomaticos(PlazasPorEmpleado plazaPorEmpleado, PeriodosNomina periodosNominas, TipoCorrida tipoCorrida, RazonesSociales razonesSociales, CentroDeCosto centroDeCostoMovimiento, DBContextSimple dbContextSimple)
        {
            List<MovNomConcep> movNomConceptos = new List<MovNomConcep>(0);
            try
            {

                if (filtroConceptosNomina != null)
                {
                    int j;
                    
                    for (j = 0; j < filtroConceptosNomina.Count; j++)
                    {
                        
                        MovNomConcep movNomConcep;
                        DateTime fechaPeriodo;
                        
                        movNomConcep = creaMovNomConcep(filtroConceptosNomina[j], plazaPorEmpleado, periodosNominas, tipoCorrida, razonesSociales, centroDeCostoMovimiento, dbContextSimple);
                        
                        movNomConceptos.Add(movNomConcep);
                        if (evaluaPeriodoMovAbarca2Meses(periodosNominas))
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("creaMovimientosPlazasConceptosAutomaticos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return movNomConceptos;
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

        private void generarMovimientosAbarca2Meses(DBContextAdapter dbContextSimple)
        {
            try
            {
                if (filtroMovimientosNominas.Count > 0)
                {
                    if (evaluaPeriodoMovAbarca2Meses(filtroMovimientosNominas[0].periodosNomina))
                    {
                        //Este codigo vuelve a generar los movimientos de nomina que no tengan su 2do movimiento ya que el periodo
                        //abarca 2 meses y se tienen que calcular la informacion por cada mes.
                        List<MovNomConcep> listTmp = new List<MovNomConcep>();
                        int mesUno;
                        DateTime fechaInicio, fechaFinal;
                        fechaInicio = filtroMovimientosNominas[0].periodosNomina.fechaInicial.GetValueOrDefault();
                        fechaFinal = filtroMovimientosNominas[0].periodosNomina.fechaFinal.GetValueOrDefault();
                        mesUno = fechaInicio.Month;
                        int i = 0;
                        while (i < filtroMovimientosNominas.Count)
                        {
                            if (!String.Equals(filtroMovimientosNominas[i].concepNomDefi.clave, filtroMovimientosNominas[i + 1].concepNomDefi.clave, StringComparison.OrdinalIgnoreCase))
                            {
                                mensajeResultado = duplicarMovNomConcep(filtroMovimientosNominas[i], filtroMovimientosNominas[i].numero.GetValueOrDefault(), filtroMovimientosNominas[i].plazasPorEmpleado, (DBContextSimple)dbContextSimple.context);
                                MovNomConcep newMov = null;
                                if (mensajeResultado.noError == 0)
                                {
                                    newMov = (MovNomConcep)mensajeResultado.resultado;
                                }
                                else
                                {
                                    break;
                                }
                                if (filtroMovimientosNominas[i].mes == mesUno)
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
                                if (i + 1 >= filtroMovimientosNominas.Count)
                                {
                                    mensajeResultado = duplicarMovNomConcep(filtroMovimientosNominas[i], filtroMovimientosNominas[i].numero.GetValueOrDefault(), filtroMovimientosNominas[i].plazasPorEmpleado, (DBContextSimple)dbContextSimple.context);
                                    if (mensajeResultado.noError == 0)
                                    {
                                        newMov = (MovNomConcep)mensajeResultado.resultado;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    if (filtroMovimientosNominas[i].mes == mesUno)
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
                                        clavesMovEliminados.Add(filtroMovimientosNominas[x].id);
                                        filtroMovimientosNominas.Remove(filtroMovimientosNominas[x]);
                                    }
                                }
                                if (clavesMovEliminados.Count > 0)
                                {
                                    eliminarMovimientosNominaBasura(clavesMovEliminados.ToArray(), dbContextSimple);
                                    if (mensajeResultado.noError != 0)
                                    {
                                        break;
                                    }
                                    //////dbContextSimple.flush();
                                    //////dbContextSimple.clear();
                                }
                                i = x;//+ 1;
                            }
                        }
                        if (mensajeResultado.noError != 0)
                        {
                            return;
                        }
                        filtroMovimientosNominas.AddRange(listTmp);
                        ordenarMovimientosNomina();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("generarMovimientosAbarca2Meses()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
        }

        public Mensaje duplicarMovNomConcep(MovNomConcep movNomConcepTmp, int numero, PlazasPorEmpleado plazasPorEmpleado, DBContextSimple dbContextSimple)
        {
            inicializaVariableMensaje();
            MovNomConcep movNomConcep = new MovNomConcep();
            try
            {
                movNomConcep.empleados = movNomConcepTmp.plazasPorEmpleado.empleados;
                movNomConcep.plazasPorEmpleado = (plazasPorEmpleado == null ? movNomConcepTmp.plazasPorEmpleado : plazasPorEmpleado);
                movNomConcep.periodosNomina = movNomConcepTmp.periodosNomina;
                movNomConcep.concepNomDefi = movNomConcepTmp.concepNomDefi;
                movNomConcep.tipoCorrida = movNomConcepTmp.tipoCorrida;
                movNomConcep.tipoNomina = movNomConcepTmp.tipoNomina;
                movNomConcep.centroDeCosto = movNomConcepTmp.centroDeCosto;
                movNomConcep.razonesSociales = movNomConcepTmp.razonesSociales;
                if (movNomConcepTmp.concepNomDefi.baseAfecConcepNom != null)
                {
                    mensajeResultado = creaMovimBaseAfectar(movNomConcepTmp.concepNomDefi.baseAfecConcepNom, movNomConcep, dbContextSimple);
                    if (mensajeResultado.noError == 0)
                    {
                        movNomConcep.movNomBaseAfecta = (List<MovNomBaseAfecta>)mensajeResultado.resultado;
                    }
                    else
                    {
                        return mensajeResultado;
                    }
                }

                if (movNomConcepTmp.concepNomDefi.paraConcepDeNom == null ? false : movNomConcepTmp.concepNomDefi.paraConcepDeNom.Count > 0)
                {
                    mensajeResultado = creaMovNomConceParam(movNomConcepTmp.concepNomDefi, movNomConcep, dbContextSimple);
                    if (mensajeResultado.noError == 0)
                    {
                        movNomConcep.movNomConceParam = (List<MovNomConceParam>)mensajeResultado.resultado;
                    }
                    else
                    {
                        return mensajeResultado;
                    }
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
                mensajeResultado.resultado = movNomConcep;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("duplicarMovNomConcep()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }

            return mensajeResultado;
        }

        public Mensaje eliminarMovimientosNominaBasura(Object[] valores, DBContextAdapter dbContext)
        {
            try
            {
                //Elimina Bases Afectadas de Movimientos por Conceptos
                mensajeResultado = deleteListQuery(typeof(MovNomBaseAfecta).Name, new CamposWhere(string.Concat(typeof(MovNomBaseAfecta).Name, ".movNomConcep.id"), valores, OperadorComparacion.IN, OperadorLogico.AND), dbContext); //pendiente
                if (mensajeResultado.noError != 0)
                {
                    return mensajeResultado;
                }
                //Elimina Movimientos Por parametros de Movimientos por Conceptos
                mensajeResultado = deleteListQuery(typeof(MovNomConceParam).Name, new CamposWhere(string.Concat(typeof(MovNomConceParam).Name, ".movNomConcep.id"), valores, OperadorComparacion.IN, OperadorLogico.AND), dbContext); //pendiente
                if (mensajeResultado.noError != 0)
                {
                    return mensajeResultado;
                }
                //Elimina Movimientos ISRRetenidos
                mensajeResultado = deleteListQuery(typeof(CalculoISR).Name, new CamposWhere(string.Concat(typeof(CalculoISR).Name, ".movNomConcep.id"), valores, OperadorComparacion.IN, OperadorLogico.AND), dbContext); //pendiente
                if (mensajeResultado.noError != 0)
                {
                    return mensajeResultado;
                }
                //Elimina Movimientos CalculoIMSS
                mensajeResultado = deleteListQuery(typeof(CalculoIMSS).Name, new CamposWhere(string.Concat(typeof(CalculoIMSS).Name, ".movNomConcep.id"), valores, OperadorComparacion.IN, OperadorLogico.AND), dbContext); //pendiente
                if (mensajeResultado.noError != 0)
                {
                    return mensajeResultado;
                }
                //Elimina Movimientos CalculoIMSSPatron
                mensajeResultado = deleteListQuery(typeof(CalculoIMSSPatron).Name, new CamposWhere(string.Concat(typeof(CalculoIMSSPatron).Name, ".movNomConcep.id"), valores, OperadorComparacion.IN, OperadorLogico.AND), dbContext); //pendiente
                if (mensajeResultado.noError != 0)
                {
                    return mensajeResultado;
                }
                //Elimina Movimientos por Conceptos
                mensajeResultado = deleteListQuery(typeof(MovNomConcep).Name, new CamposWhere(string.Concat(typeof(MovNomConcep).Name, ".id"), valores, OperadorComparacion.IN, OperadorLogico.AND), dbContext); //pendiente
                if (mensajeResultado.noError != 0)
                {
                    return mensajeResultado;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("eliminarMovimientosNominaBasura()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return mensajeResultado;
        }

        private Mensaje deleteListQuery(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext)
        {
            try
            {
                List<CamposWhere> listaCamposWhere = new List<CamposWhere>();
                if (campoWhere != null)
                {
                    listaCamposWhere.Add(campoWhere);
                }
                mensajeResultado = conectorQuerysGenericos.consultaGenerica(dbContext, TipoResultado.Unico, TipoOperacion.DELETE,
                     tabla, null, null, listaCamposWhere, null, null, null);
                if (mensajeResultado.noError == 0)
                {
                    mensajeResultado.resultado = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("deleteListQuery()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }

            return mensajeResultado;

        }

        public Mensaje creaMovNomConceptoSubsidio(MovNomConcep movNomi, ConcepNomDefi concepNomDefi, PeriodosNomina periodosNomina, TipoCorrida tipoCorrida, RazonesSociales razonesSociales, CentroDeCosto centroDeCostoMovimiento, DBContextSimple dbContextSimple)
        {
            inicializaVariableMensaje();
            MovNomConcep movNomConcepSubsidio = new MovNomConcep();
            try
            {
                movNomConcepSubsidio.empleados = (movNomi.plazasPorEmpleado.empleados);
                movNomConcepSubsidio.plazasPorEmpleado = (movNomi.plazasPorEmpleado);
                movNomConcepSubsidio.periodosNomina = (periodosNomina);
                movNomConcepSubsidio.concepNomDefi = (concepNomDefi);
                movNomConcepSubsidio.tipoCorrida = (tipoCorrida);
                movNomConcepSubsidio.tipoNomina = (periodosNomina.tipoNomina);
                movNomConcepSubsidio.centroDeCosto = (centroDeCostoMovimiento);
                movNomConcepSubsidio.razonesSociales = (razonesSociales);
                if (concepNomDefi.baseAfecConcepNom != null)
                {
                    mensajeResultado = creaMovimBaseAfectar(concepNomDefi.baseAfecConcepNom, movNomConcepSubsidio, dbContextSimple);
                    if (mensajeResultado.noError == 0)
                    {
                        movNomConcepSubsidio.movNomBaseAfecta = (List<MovNomBaseAfecta>)mensajeResultado.resultado;
                    }
                    else
                    {
                        return mensajeResultado;
                    }

                }

                movNomConcepSubsidio.fechaCierr = (periodosNomina.fechaCierre);
                movNomConcepSubsidio.fechaIni = (periodosNomina.fechaInicial);
                movNomConcepSubsidio.tipoPantalla = (tipoPantallaSistema);
                movNomConcepSubsidio.ordenId = (movNomi.ordenId + 1);
                movNomConcepSubsidio.resultado = (0.0);
                movNomConcepSubsidio.numero = (movNomi.numero == null ? 1 : movNomi.numero + 1);
                movNomConcepSubsidio.calculado = (0.0);
                movNomConcepSubsidio.mes = (movNomi.mes);
                movNomConcepSubsidio.ejercicio = (movNomi.ejercicio);
                mensajeResultado.noError = 0;
                mensajeResultado.resultado = movNomConcepSubsidio;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("creaMovNomConceptoSubsidio()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return mensajeResultado;
        }

        /*
        * fechaPeriodo esta fecha se lee extrae mes para calcular mes a regresar
        * suma de sus resultados tipo que tipo a afectar es si isr, imsss, etc.
        * Modificado
        */
        //double
        public Mensaje calcularMovimientosPorMesTipoAfecta(List<PeriodosNomina> periodos, string claveEmpleado, string tipoNomina, string tipoCorrida, string claveBaseNomina, string claveRazonSocial, string clavePlazaEmpleado, DBContextSimple dbContextSimple)
        {
            inicializaVariableMensaje();
            try
            {
                var query = from mba in dbContextSimple.Set<MovNomBaseAfecta>()
                            where mba.movNomConcep.uso == 0 && mba.movNomConcep.tipoNomina.clave == tipoNomina && mba.movNomConcep.empleados.clave == claveEmpleado && mba.movNomConcep.tipoCorrida.clave == tipoCorrida &&
                               mba.baseAfecConcepNom.baseNomina.clave == claveBaseNomina && mba.movNomConcep.razonesSociales.clave == claveRazonSocial && mba.movNomConcep.plazasPorEmpleado.referencia == clavePlazaEmpleado &&
                               mba.movNomConcep.periodosNomina.tipoCorrida.clave == tipoCorrida
                            select new { mba };

                if (periodos != null)
                {
                    if (periodos.Count > 0)
                    {
                        decimal[] idsPeriodos = new decimal[periodos.Count];
                        for (int i = 0; i < periodos.Count; i++)
                        {
                            idsPeriodos[i] = periodos[i].id;
                        }
                        query = from sub in query where idsPeriodos.Contains(sub.mba.movNomConcep.periodosNomina.id) select sub;
                    }
                }

                var queryRes = from sub in query
                               select new
                               {
                                   resultado = sub.mba.resultado
                               };
                double baseGravable;
                if (queryRes.Count() == 0)
                {
                    baseGravable = 0.0;
                }
                else
                {
                    baseGravable = queryRes.Sum(s => s.resultado.Value);
                }
                mensajeResultado.noError = 0;
                mensajeResultado.resultado = baseGravable;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("calcularMovimientosPorMesTipoAfecta()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return mensajeResultado;
        }

        public int deleteListQueryMov(String tabla, String campo, Object[] valores, List<CFDIEmpleado> valoresCFDI, Object[] valoresCalculoUnidades, Object[] valoresReestablecer, bool incluirEliminadoDiferenteTipoPantalla100, DBContextAdapter dbContextSimple, DBContextAdapter dbContextMaestra)
        {
            int exito = 0;
            List<Object> valoresx = new List<Object>();
            try
            {
                if (valoresReestablecer == null ? false : valoresReestablecer.Length > 0)
                {
                    valoresx.AddRange(valoresReestablecer);
                    valoresx.AddRange(valores);
                }
                else
                {

                    valoresx.AddRange(valores);
                }
                List<CreditoMovimientos> listCreditoMovimientos = (from o in dbContextSimple.context.Set<MovNomConcep>()
                                                                   join credMov in dbContextSimple.context.Set<CreditoMovimientos>()
                                                                   on o.creditoMovimientos.id equals credMov.id
                                                                   where valoresx.Contains(o.id)
                                                                   select o.creditoMovimientos
                                                                 ).Distinct().ToList();
                //Elimina CreditoMovimientos y reestablece los importe.
                if (listCreditoMovimientos == null ? false : listCreditoMovimientos.Count > 0 ? false : true)
                {
                    int i, j, k;
                    for (i = 0; i < listCreditoMovimientos.Count; i++)
                    {
                        dbContextSimple.context.Set<CreditoPorEmpleado>().AddOrUpdate(listCreditoMovimientos[i].creditoPorEmpleado);
                        if (listCreditoMovimientos[i].movNomConcep != null)
                        {
                            int movimientosEliminados = 0;
                            for (j = 0; j < listCreditoMovimientos[i].movNomConcep.Count; j++)
                            {
                                listCreditoMovimientos[i].movNomConcep[j].calculado = 0.0;
                                listCreditoMovimientos[i].movNomConcep[j].resultado = 0.0;
                                listCreditoMovimientos[i].movNomConcep[j].creditoMovimientos = null;
                                if (listCreditoMovimientos[i].movNomConcep[j].movNomBaseAfecta != null)
                                {
                                    for (k = 0; k < listCreditoMovimientos[i].movNomConcep[j].movNomBaseAfecta.Count; k++)
                                    {
                                        listCreditoMovimientos[i].movNomConcep[j].movNomBaseAfecta[k].resultado = 0.0;
                                        listCreditoMovimientos[i].movNomConcep[j].movNomBaseAfecta[k].resultadoExento = 0.0;
                                    }
                                }
                                dbContextSimple.context.Set<MovNomConcep>().AddOrUpdate(listCreditoMovimientos[i].movNomConcep[j]);
                                if (listCreditoMovimientos[i].movNomConcep[j].tipoPantalla.Equals(100) || incluirEliminadoDiferenteTipoPantalla100)
                                {
                                    dbContextSimple.context.Set<MovNomConcep>().Attach(listCreditoMovimientos[i].movNomConcep[j]);
                                    dbContextSimple.context.Set<MovNomConcep>().Remove(listCreditoMovimientos[i].movNomConcep[j]);
                                    movimientosEliminados++;
                                }

                            }
                            List<MovNomConcep> m = listCreditoMovimientos[i].movNomConcep;
                            listCreditoMovimientos[i].movNomConcep.RemoveAll(mov => mov.id == Convert.ToDecimal(listCreditoMovimientos[i].movNomConcep.Select(p => p.id)));
                            dbContextSimple.context.Set<CreditoMovimientos>().Attach(listCreditoMovimientos[i]);
                            dbContextSimple.context.Set<CreditoMovimientos>().Remove(listCreditoMovimientos[i]);
                        }
                    }
                }

                if (valores.Length > 0)
                {
                    dbContextSimple.context.Database.ExecuteSqlCommand("delete from MovNomBaseAfecta o  where o.movNomConcep.id in(@valores)", new SqlParameter(" @valores", valores));

                    dbContextSimple.context.Database.ExecuteSqlCommand("delete from MovNomConceParam o  where o.movNomConcep.id in(@valores)", new SqlParameter(" @valores", valores));

                    dbContextSimple.context.Database.ExecuteSqlCommand("delete from CalculoISR o  where o.movNomConcep.id in(@valores)", new SqlParameter(" @valores", valores));

                    dbContextSimple.context.Database.ExecuteSqlCommand("delete from CalculoIMSS o  where o.movNomConcep.id in(@valores)", new SqlParameter(" @valores", valores));

                    dbContextSimple.context.Database.ExecuteSqlCommand("delete from CalculoIMSSPatron o  where o.movNomConcep.id in(@valores)", new SqlParameter(" @valores", valores));

                    dbContextSimple.context.Database.ExecuteSqlCommand("delete from " + tabla + " o  where  " + campo + "  in(@valores)", new SqlParameter(" @valores", valores));

                    dbContextSimple.context.SaveChanges();
                }
                if (valoresReestablecer == null ? false : valoresReestablecer.Length > 0)
                {
                    List<CamposSelect> camposSelect = new List<CamposSelect>() { new CamposSelect(String.Concat(typeof(MovNomConcep).Name), TipoFuncion.NINGUNO) };
                    OperadorSelect select = new OperadorSelect(camposSelect);
                    List<CamposWhere> camposwhere = new List<CamposWhere>() { new CamposWhere(String.Concat(typeof(MovNomConcep).Name, campo), valoresReestablecer, OperadorComparacion.IN, OperadorLogico.AND) };
                    mensajeResultado = conectorQuerysGenericos.consultaGenerica(dbContextSimple, TipoResultado.Unico, TipoOperacion.SELECT,
                        typeof(MovNomConcep).Name, select, null, camposwhere, null, null, null);
                    List<MovNomConcep> listMovNomConcepReestablecer = (List<MovNomConcep>)mensajeResultado.resultado;
                    if (listMovNomConcepReestablecer != null)
                    {
                        int j, k;
                        for (j = 0; j < listMovNomConcepReestablecer.Count; j++)
                        {
                            listMovNomConcepReestablecer[j].calculado = 0.0;
                            listMovNomConcepReestablecer[j].resultado = 0.0;
                            listMovNomConcepReestablecer[j].creditoMovimientos = null;
                            if (listMovNomConcepReestablecer[j].movNomBaseAfecta != null)
                            {
                                for (k = 0; k < listMovNomConcepReestablecer[j].movNomBaseAfecta.Count; k++)
                                {
                                    listMovNomConcepReestablecer[j].movNomBaseAfecta[k].resultado = 0.0;
                                    listMovNomConcepReestablecer[j].movNomBaseAfecta[k].resultadoExento = 0.0;
                                }
                            }



                            dbContextSimple.context.Set<MovNomConcep>().AddOrUpdate(listMovNomConcepReestablecer[j]);
                            dbContextSimple.context.SaveChanges();
                            if (listMovNomConcepReestablecer[j].tipoPantalla.Equals(100) || incluirEliminadoDiferenteTipoPantalla100)
                            {
                                dbContextSimple.context.Set<MovNomConcep>().Attach(listMovNomConcepReestablecer[j]);
                                dbContextSimple.context.Set<MovNomConcep>().Remove(listMovNomConcepReestablecer[j]);
                                dbContextSimple.context.SaveChanges();
                            }
                        }


                    }

                }
                if (valoresCalculoUnidades != null ? valoresCalculoUnidades.Length > 0 : false)
                {

                    dbContextSimple.context.Database.ExecuteSqlCommand("delete from CalculoUnidades o  where " + campo + " in(@valores)", new SqlParameter(" @valores", valores));
                    dbContextSimple.context.SaveChanges();
                }

                if (valoresCFDI == null ? false : valoresCFDI.Count > 0)
                {
                    Mensaje mensaje;
                    foreach (CFDIEmpleado cfdiEmpl in valoresCFDI)
                    {
                        mensaje = deleteListQuery("CFDIEmpleado", new CamposWhere("CFDIEmpleado.id", cfdiEmpl.id, OperadorComparacion.IGUAL, OperadorLogico.AND), dbContextSimple);
                        exito = (int)mensaje.resultado;
                        if (cfdiEmpl.id != 0)
                        {
                            decimal idrecibo = cfdiEmpl.cfdiRecibo.id;
                            List<decimal> cfdiCnc = (from o in dbContextSimple.context.Set<CFDIReciboConcepto>()
                                                     join cfd in dbContextSimple.context.Set<CFDIRecibo>() on
                                                     o.cfdiRecibo.id equals cfd.id
                                                     where cfd.id == 1
                                                     select o.id).ToList();
                            if (!cfdiCnc.Any())
                            {
                                mensaje = deleteListQuery("CFDIReciboConcepto", new CamposWhere("CFDIReciboConcepto.id", cfdiCnc.ToArray(), OperadorComparacion.IN, OperadorLogico.AND), dbContextSimple);
                                exito = (int)mensaje.resultado;
                            }

                            List<decimal> cfdiInc = (from o in dbContextSimple.context.Set<CFDIReciboIncapacidad>()
                                                     join cfd in dbContextSimple.context.Set<CFDIRecibo>() on
                                                     o.cfdiRecibo.id equals cfd.id
                                                     where cfd.id == 1
                                                     select o.id).ToList();
                            if (!cfdiInc.Any())
                            {
                                mensaje = deleteListQuery("CFDIReciboIncapacidad", new CamposWhere("CFDIReciboIncapacidad.id", cfdiInc.ToArray(), OperadorComparacion.IN, OperadorLogico.AND), dbContextSimple);
                                exito = (int)mensaje.resultado;
                            }
                            List<decimal> cfdiHrs = (from o in dbContextSimple.context.Set<CFDIReciboHrsExtras>()
                                                     join cfd in dbContextSimple.context.Set<CFDIRecibo>() on
                                                     o.cfdiRecibo.id equals cfd.id
                                                     where cfd.id == 1
                                                     select o.id).ToList();
                            if (!cfdiInc.Any())
                            {
                                mensaje = deleteListQuery("CFDIReciboHrsExtras", new CamposWhere("CFDIReciboHrsExtras.id", cfdiHrs.ToArray(), OperadorComparacion.IN, OperadorLogico.AND), dbContextSimple);
                                exito = (int)mensaje.resultado;
                            }
                            mensaje = deleteListQuery("CFDIRecibo", new CamposWhere("CFDIRecibo.id", idrecibo, OperadorComparacion.IGUAL, OperadorLogico.AND), dbContextSimple);
                            exito = (int)mensaje.resultado;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("deleteListQueryMov()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                dbContextSimple.context.Database.CurrentTransaction.Rollback();
            }
            return exito;
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