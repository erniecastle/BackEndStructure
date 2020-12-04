using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Entity.entidad.cfdi;
using Exitosw.Payroll.Core.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Exitosw.Payroll.Entity.util;

namespace Exitosw.Payroll.Core.metodosCalculoNomina
{
    public class MetodosDatosEmpleados
    {
        public Mensaje mensajeResultado = new Mensaje();
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");

        public Mensaje obtenerPlazasPorEmpleados(string claveEmpIni, string claveEmpFin, string claveTipoNomina, string clavePuesto,
            string claveCategoriasPuestos, string claveTurno, string claveRazonSocial, string claveRegPatronal, string claveDepto, string claveCtrCosto,
            int? tipoSalario, string tipoContrato, bool? status, string claveTipoCorrida, string claveFormaPago, DateTime fechaInicioPeriodo,
            DateTime fechaFinPeriodo, DateTime? fechaBajaFinq, decimal idPeriodoNomina, StatusTimbrado statusTimbrado, DBContextAdapter dbContextSimple, bool plazaPrincipal)
        {
            inicializaVariableMensaje();
            IQueryable<PlazasPorEmpleadosMov> filtroPlazasPorEmpleadosMov = null;
            try
            {
                object[] idsPPMAux = idsPlazasPorEmpleadoMovimientos(claveEmpIni, claveEmpFin, claveTipoNomina, clavePuesto, claveCategoriasPuestos, claveTurno, claveRazonSocial,
                   claveRegPatronal, claveDepto, claveCtrCosto, tipoSalario, tipoContrato, status, claveTipoCorrida, claveFormaPago, fechaInicioPeriodo, fechaFinPeriodo,
                   fechaBajaFinq, (DBContextSimple)dbContextSimple.context);
                if (mensajeResultado.noError != 0)
                {
                    return mensajeResultado;
                }
                object[] idsEmpAux = buscaIdEmpleadoConCFDITimbrado(claveEmpIni, claveEmpFin, claveRazonSocial, claveTipoNomina, claveTipoCorrida, idPeriodoNomina, statusTimbrado, (DBContextSimple)dbContextSimple.context);
                bool activa = false;
                if (plazaPrincipal)
                {
                    activa = true;
                }
                List<decimal?> idsPPM = new List<decimal?>();
                List<decimal?> idsEmp = new List<decimal?>();
                for (int i = 0; i < idsPPMAux.Length; i++)
                {
                    idsPPM.Add(Convert.ToDecimal(idsPPMAux[i]));
                }
                for (int k = 0; k < idsEmpAux.Length; k++)
                {
                    idsEmp.Add(Convert.ToDecimal(idsEmpAux[k]));
                }

                filtroPlazasPorEmpleadosMov = (from pMov in dbContextSimple.context.Set<PlazasPorEmpleadosMov>()
                                                   // join pemp in dbContextSimple.context.Set<PlazasPorEmpleado>() on pMov.plazasPorEmpleado_ID equals pemp.id
                                               where idsPPM.Contains(pMov.id) && !idsEmp.Contains(pMov.plazasPorEmpleado.empleados.id)
                                               && !(from px in dbContextSimple.context.Set<PlazasPorEmpleado>()
                                                    where px.razonesSociales.clave == claveRazonSocial && px.plazaReIngreso != null
                                                    select new { px.plazaReIngreso.id }).Contains(new { pMov.plazasPorEmpleado.id }) && pMov.plazasPorEmpleado.plazaPrincipal == activa
                                               orderby pMov.plazasPorEmpleado.empleados.clave, pMov.plazasPorEmpleado.referencia
                                               select pMov
                                    );
                mensajeResultado.noError = 0;
                mensajeResultado.resultado = filtroPlazasPorEmpleadosMov;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerPlazasPorEmpleados()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                dbContextSimple.context.Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        private object[] idsPlazasPorEmpleadoMovimientos(string claveEmpIni, string claveEmpFin, string claveTipoNomina, string clavePuesto,
            string claveCategoriasPuestos, string claveTurno, string claveRazonSocial, string claveRegPatronal, string claveDepto, string claveCtrCosto,
            int? tipoSalario, string tipoContrato, bool? status, string claveTipoCorrida, string claveFormaPago, DateTime fechaInicioPeriodo,
            DateTime fechaFinPeriodo, DateTime? fechaBajaFinq, DBContextSimple dbontextSimple)
        {
            object[] idsPPM = null;
            try
            {
                var subQuerypMovIdIN = (from pMovX in dbontextSimple.Set<PlazasPorEmpleadosMov>()
                                        //join pe in dbontextSimple.Set<PlazasPorEmpleado>() on pMovX.plazasPorEmpleado_ID equals pe.id
                                        //join em in dbontextSimple.Set<Empleados>() on pe.empleados_ID equals em.id
                                        select new
                                        {
                                            pMovX,
                                            pe= pMovX.plazasPorEmpleado,
                                            em= pMovX.plazasPorEmpleado.empleados
                                        }
    );

                if (!String.IsNullOrEmpty(claveRazonSocial))
                {
                    subQuerypMovIdIN = (from sub in subQuerypMovIdIN
                                        //join rs in dbontextSimple.Set<RazonesSociales>() on sub.pe.razonesSociales_ID equals rs.id
                                        where sub.pe.razonesSociales.clave == claveRazonSocial
                                        select sub
                    );
                }

                if (!String.IsNullOrEmpty(claveTurno))
                {
                    subQuerypMovIdIN = (from sub in subQuerypMovIdIN
                                        join tu in dbontextSimple.Set<Turnos>() on sub.pMovX.turnos_ID equals tu.id into tu_join
                                        from tu in tu_join.DefaultIfEmpty()
                                        where tu.clave == claveTurno
                                        select sub
                    );
                }

                if (!String.IsNullOrEmpty(claveTipoNomina))
                {
                    subQuerypMovIdIN = (from sub in subQuerypMovIdIN
                                        //join t in dbontextSimple.Set<TipoNomina>() on sub.pMovX.tipoNomina_ID equals t.id into t_join
                                        //from t in t_join.DefaultIfEmpty()
                                        where sub.pMovX.tipoNomina.clave == claveTipoNomina
                                        select sub
                    );
                }

                if (!String.IsNullOrEmpty(claveRegPatronal))
                {
                    subQuerypMovIdIN = (from sub in subQuerypMovIdIN
                                        join rp in dbontextSimple.Set<RegistroPatronal>() on sub.pe.registroPatronal_ID equals rp.id into rp_join
                                        from rp in rp_join.DefaultIfEmpty()
                                        where rp.clave == claveRegPatronal
                                        select sub
                    );
                }

                if (!String.IsNullOrEmpty(claveDepto))
                {
                    subQuerypMovIdIN = (from sub in subQuerypMovIdIN
                                        join dp in dbontextSimple.Set<Departamentos>() on sub.pMovX.departamentos_ID equals dp.id into dp_join
                                        from dp in dp_join.DefaultIfEmpty()
                                        where dp.clave == claveDepto
                                        select sub
                    );
                }

                if (!String.IsNullOrEmpty(claveCtrCosto))
                {
                    subQuerypMovIdIN = (from sub in subQuerypMovIdIN
                                        join cc in dbontextSimple.Set<CentroDeCosto>() on sub.pMovX.centroDeCosto_ID equals cc.id into cc_join
                                        from cc in cc_join.DefaultIfEmpty()
                                        where cc.clave == claveCtrCosto
                                        select sub
                    );
                }

                if (!String.IsNullOrEmpty(clavePuesto))
                {
                    if (String.IsNullOrEmpty(claveCategoriasPuestos))
                    {
                        subQuerypMovIdIN = (from sub in subQuerypMovIdIN
                                            join pu in dbontextSimple.Set<Puestos>() on sub.pMovX.puestos_ID equals pu.id into pu_join
                                            from pu in pu_join.DefaultIfEmpty()
                                            where pu.clave == clavePuesto
                                            select sub
                        );
                    }
                    else //categoriaPuesto No es nulo
                    {
                        subQuerypMovIdIN = (from sub in subQuerypMovIdIN
                                            join pu in dbontextSimple.Set<Puestos>() on sub.pMovX.puestos_ID equals pu.id into pu_join
                                            from pu in pu_join.DefaultIfEmpty()
                                            join cp in dbontextSimple.Set<CategoriasPuestos>() on pu.categoriasPuestos_ID equals cp.id into cp_join
                                            from cp in cp_join.DefaultIfEmpty()
                                            where pu.clave == clavePuesto && cp.clave == claveCategoriasPuestos
                                            select sub
                        );
                    }
                }

                if (!String.IsNullOrEmpty(claveCategoriasPuestos))
                {
                    if (String.IsNullOrEmpty(clavePuesto))
                    {
                        subQuerypMovIdIN = (from sub in subQuerypMovIdIN
                                            join pu in dbontextSimple.Set<Puestos>() on sub.pMovX.puestos_ID equals pu.id into pu_join
                                            from pu in pu_join.DefaultIfEmpty()
                                            join cp in dbontextSimple.Set<CategoriasPuestos>() on pu.categoriasPuestos_ID equals cp.id into cp_join
                                            from cp in cp_join.DefaultIfEmpty()
                                            where cp.clave == claveCategoriasPuestos
                                            select sub
                        );
                    }
                }

                //if (!String.IsNullOrEmpty(claveFormaPago))
                //{
                //    subQuerypMovIdIN = (from sub in subQuerypMovIdIN
                //                        join fp in dbontextSimple.Set<FormasDePago>() on sub.pMovX.formasDePago_ID equals fp.id into fp_join
                //                        from fp in fp_join.DefaultIfEmpty()
                //                        where fp.clave == claveFormaPago
                //                        select sub
                //    );
                //}

                if (!String.IsNullOrEmpty(tipoContrato))
                {
                    subQuerypMovIdIN = (from sub in subQuerypMovIdIN
                                        join tc in dbontextSimple.Set<TipoContrato>() on sub.pMovX.tipoContrato_ID equals tc.id into tc_join
                                        from tc in tc_join.DefaultIfEmpty()
                                        where tc.clave == tipoContrato
                                        select sub
                    );
                }

                if (status != null)
                {
                    subQuerypMovIdIN = (from sub in subQuerypMovIdIN
                                        where sub.em.status == status
                                        select sub
                    );
                }

                if (tipoSalario != null)
                {
                    DateTime fechaActual;
                    if (string.Equals(claveTipoCorrida, "FIN", StringComparison.OrdinalIgnoreCase))
                    {
                        fechaActual = fechaBajaFinq.Value;
                    }
                    else
                    {
                        fechaActual = fechaFinPeriodo;
                    }
                    subQuerypMovIdIN = (
                        from sub in subQuerypMovIdIN
                        from si in dbontextSimple.Set<SalariosIntegrados>()
                        join em2 in dbontextSimple.Set<Empleados>() on si.empleados_ID equals em2.id
                        where si.fecha == (from s in dbontextSimple.Set<SalariosIntegrados>()
                                           join em3 in dbontextSimple.Set<Empleados>() on s.empleados_ID equals em3.id
                                           where s.fecha <= fechaActual.Date && em3.id == em2.id && em3.id == sub.em.id
                                           select new { s.fecha }
                                        ).Max(f => f.fecha) && si.tipoDeSalario == tipoSalario
                        select sub
                    );
                }

                if (string.Equals(claveTipoCorrida, "FIN", StringComparison.OrdinalIgnoreCase))
                {
                    fechaFinPeriodo = fechaBajaFinq.Value;
                }
                subQuerypMovIdIN = (from sub in subQuerypMovIdIN
                                    where ((sub.pMovX.fechaInicial <= fechaInicioPeriodo) || (sub.pMovX.fechaInicial >= fechaInicioPeriodo.Date && sub.pMovX.fechaInicial <= fechaFinPeriodo.Date))
                                        && ((sub.pe.fechaFinal >= fechaFinPeriodo.Date) || (sub.pe.fechaFinal >= fechaInicioPeriodo.Date && sub.pe.fechaFinal <= fechaFinPeriodo.Date))
                                    select sub
                );

                if (!String.IsNullOrEmpty(claveEmpIni) && !String.IsNullOrEmpty(claveEmpFin))
                {
                  
                        subQuerypMovIdIN = (from sub in subQuerypMovIdIN
                                            where (sub.em.clave.CompareTo(claveEmpIni) >= 0 && sub.em.clave.CompareTo(claveEmpFin) <= 0)
                                            select sub
                        );
                   
                }
                else if (!String.IsNullOrEmpty(claveEmpIni))
                {
                    subQuerypMovIdIN = (from sub in subQuerypMovIdIN
                                        where sub.em.clave.CompareTo(claveEmpIni) >= 0
                                        select sub
                    );
                }
                else if (!String.IsNullOrEmpty(claveEmpFin))
                {
                    subQuerypMovIdIN = (from sub in subQuerypMovIdIN
                                        where sub.em.clave.CompareTo(claveEmpFin) <= 0
                                        select sub
                    );
                }
                var finQuerypMovIdIN = (from sub in subQuerypMovIdIN
                                        group new { sub.pe, sub.pMovX } by new
                                        {
                                            sub.pe.referencia
                                        } into grupo
                                        select new
                                        {
                                            idPPMMax = grupo.Max(p => p.pMovX.id)
                                        }
                );
                decimal[] prueba = finQuerypMovIdIN.Select(a => a.idPPMMax).ToArray();
                idsPPM = new object[prueba.Length];
                for (int i = 0; i < prueba.Length; i++)
                {
                    idsPPM[i] = prueba[i];
                }
                //idsPPM = finQuerypMovIdIN.ToArray();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("idsPlazasPorEmpleadoMovimientos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                dbontextSimple.Database.CurrentTransaction.Rollback();
            }
            if (idsPPM == null)
            {
                idsPPM = new object[] { };
            }
            return idsPPM;
        }

        private object[] buscaIdEmpleadoConCFDITimbrado(string claveEmpIni, string claveEmpFin, string claveRazonSocial, string claveTipoNomina, string claveTipoCorrida,
            decimal idPeriodoNomina, StatusTimbrado statusTimbrado, DBContextSimple dbContextSimple)
        {
            object[] idsEmp = null;
            try
            {
                var queryIdEmpleados = (from o in dbContextSimple.Set<CFDIEmpleado>()
                                        where
                                          o.razonesSociales.clave == claveRazonSocial &&
                                          o.tipoNomina.clave == claveTipoNomina &&
                                          o.tipoCorrida.clave == claveTipoCorrida &&
                                          o.periodosNomina.id == idPeriodoNomina &&
                                          o.cfdiRecibo.statusTimbrado == statusTimbrado &&
                                          (o.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave.CompareTo(claveEmpIni) >= 0 && o.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave.CompareTo(claveEmpFin) <= 0)
                                        select new
                                        {
                                            o.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.id
                                        });

                idsEmp = queryIdEmpleados.ToArray();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscaIdEmpleadoConCFDITimbrado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                dbContextSimple.Database.CurrentTransaction.Rollback();
            }
            if (idsEmp == null)
            {
                idsEmp = new object[] { };
            }
            return idsEmp;
        }

        public Mensaje obtenerIngresosReIngresosBajas(PlazasPorEmpleadosMov plazaPorEmpleadoMov, DateTime? fechaFinalPeriodo, DBContextSimple dbContextSimple)
        {
            inicializaVariableMensaje();
            IngresosBajas ingReingresosBajas = null;
            try
            {
                var queryIngBajas = (from ing in dbContextSimple.Set<IngresosBajas>()
                                     where
                                       ing.razonesSociales.clave == plazaPorEmpleadoMov.plazasPorEmpleado.razonesSociales.clave &&
                                       ing.empleados.id == plazaPorEmpleadoMov.plazasPorEmpleado.empleados.id
                                     select ing);
              //  if (plazaPorEmpleadoMov.plazasPorEmpleado != null)
              //  {
                    if (plazaPorEmpleadoMov.plazasPorEmpleado.registroPatronal != null)
                    {
                        string claveRegPatronal = plazaPorEmpleadoMov.plazasPorEmpleado.registroPatronal.clave;
                        if (!String.IsNullOrEmpty(claveRegPatronal))
                        {
                            queryIngBajas = (from sub in queryIngBajas
                                             where sub.registroPatronal.clave == claveRegPatronal
                                             select sub
                            );
                        }
                    }
              //  }
                List<IngresosBajas> listIngReingresosBajas = queryIngBajas.ToList();

                if (listIngReingresosBajas != null)
                {
                    DateTime fechaActual = DateTime.Now;
                    if (fechaFinalPeriodo != null)
                    {
                        fechaActual = fechaFinalPeriodo.Value;
                    }
                    int i, j;
                    for (i = 0; i < listIngReingresosBajas.Count; i++)
                    {
                        if (listIngReingresosBajas[i].fechaBaja.Value.CompareTo(fechaActual) <= 0)
                        {
                            ingReingresosBajas = listIngReingresosBajas[i];
                            for (j = 0; j < listIngReingresosBajas.Count; j++)
                            {
                                //if (listIngReingresosBajas[j].tipoMovimiento == IngReingresosBajas.TipoMovimiento.R)
                                //{
                                //    if (listIngReingresosBajas[j].fechaBaja.Value.CompareTo(listIngReingresosBajas[i].fechaBaja) >= 0)
                                //    {
                                //        ingReingresosBajas = listIngReingresosBajas[j];
                                //    }
                                //}
                            }
                        }
                        else
                        {
                            ingReingresosBajas = listIngReingresosBajas[i];
                        }
                    }
                    mensajeResultado.noError = 0;
                    mensajeResultado.resultado = ingReingresosBajas;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerIngresosReIngresosBajas()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                dbContextSimple.Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje obtenerMinimoPlazasPorEmpleadosMovDentroPeriodo(string claveTipoCorrida, DateTime fechaInicioPeriodo, DateTime fechaFinPeriodo, DateTime? fechaBajaFiniq, PlazasPorEmpleadosMov plazasPorEmpleadosMov, DBContextSimple dbContextSimple)
        {
            try
            {
                inicializaVariableMensaje();
                if (string.Equals(claveTipoCorrida, "FIN", StringComparison.OrdinalIgnoreCase))
                {
                    fechaInicioPeriodo = fechaBajaFiniq.GetValueOrDefault();
                }
                List<PlazasPorEmpleadosMov> filtroPlazasPorEmpleadosMov = (from pmov in dbContextSimple.Set<PlazasPorEmpleadosMov>()
                                                                           where pmov.id != plazasPorEmpleadosMov.id && pmov.plazasPorEmpleado.id == plazasPorEmpleadosMov.plazasPorEmpleado.id &&
                                                                               pmov.fechaInicial == (from pmovIn in dbContextSimple.Set<PlazasPorEmpleadosMov>()
                                                                                                     where (pmovIn.fechaInicial >= fechaInicioPeriodo && pmovIn.fechaInicial <= fechaFinPeriodo) && pmovIn.plazasPorEmpleado.fechaFinal >= fechaInicioPeriodo &&
                                                                                                            pmovIn.fechaInicial <= fechaFinPeriodo && pmovIn.id != plazasPorEmpleadosMov.id &&
                                                                                                            pmovIn.plazasPorEmpleado.id == plazasPorEmpleadosMov.plazasPorEmpleado.id
                                                                                                     select new { fechaMin = pmovIn.fechaInicial }).Min(f => f.fechaMin)
                                                                           /*  && pmov.cambioSalarioPor == true*/
                                                                           select pmov).ToList();
                mensajeResultado.noError = 0;
                mensajeResultado.resultado = filtroPlazasPorEmpleadosMov;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerMinimoPlazasPorEmpleadosMovDentroPeriodo()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return mensajeResultado;
        }

        //List<Object>
        public Mensaje obtenerModificacionesDePlazasPorEmpleadoMov(bool configuracionSueldoDiarioVigente, bool configuracionPercepcion_Plaza, bool configuracionPercepcion_Plaza_Vigente, MovNomConcep movimientosNomina, PlazasPorEmpleadosMov plazasPorEmpleadosMov,
            List<MovNomConcep> filtroMovimientosNominas, PeriodosNomina periodosNomina, DateTime fechaIni, DateTime fechaFin, DateTime fechaActual, DateTime? fechaBajaFiniq, CentroDeCosto centroDeCostoMovimiento, TipoCorrida tipoCorrida, DBContextAdapter dbContextSimple)
        {
            try
            {
                List<Object> listObject = new List<Object>();
                List<MovNomConcep> listMovNomConcepPromocional = new List<MovNomConcep>();
                List<PlazasPorEmpleadosMov> listPlazasPorEmpleadosMovOficial = new List<PlazasPorEmpleadosMov>();
                MetodosParaMovimientosNomina metodosMovimientosNomina = new MetodosParaMovimientosNomina();
                int i;
                bool continuar = false;
                if (configuracionSueldoDiarioVigente)
                {
                    //<editor-fold defaultstate="collapsed" desc="Programacion para cuando se aplica modificaciones salariales, aqui se obtienen los movimientos del empleado dentro del periodo">
                    listMovNomConcepPromocional.Add(movimientosNomina);
                    if (periodosNomina != null)
                    {
                        fechaIni = periodosNomina.fechaInicial.GetValueOrDefault();
                        fechaFin = periodosNomina.fechaFinal.GetValueOrDefault();
                    }
                    List<PlazasPorEmpleadosMov> listPlazasPorEmpleadosMovTmp = new List<PlazasPorEmpleadosMov>();
                    listPlazasPorEmpleadosMovTmp.AddRange(obtenerPlazasPorEmpleadosMovDentroPeriodo(tipoCorrida.clave, periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault(), new Object[] { plazasPorEmpleadosMov }, fechaBajaFiniq, (DBContextSimple)dbContextSimple.context));
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
                            mensajeResultado = metodosMovimientosNomina.duplicarMovNomConcep(listMovNomConcepPromocional[0], numero, listPlazasPorEmpleadosMovTmp[i].plazasPorEmpleado, (DBContextSimple)dbContextSimple.context);
                            if (mensajeResultado.noError == 0)
                            {
                                listMovNomConcepPromocional.Add((MovNomConcep)mensajeResultado.resultado);
                            }
                            else
                            {
                                break;
                            }
                            if (metodosMovimientosNomina.evaluaPeriodoMovAbarca2Meses(periodosNomina))
                            {
                                int mesUno;
                                DateTime fechaPromocion, fechaInicio, fechaFinal;
                                fechaInicio = filtroMovimientosNominas[0].periodosNomina.fechaInicial.GetValueOrDefault();
                                fechaFinal = filtroMovimientosNominas[0].periodosNomina.fechaFinal.GetValueOrDefault();
                                fechaPromocion = listPlazasPorEmpleadosMovTmp[i].fechaInicial.GetValueOrDefault();
                                mesUno = fechaInicio.Month;
                                listMovNomConcepPromocional[listMovNomConcepPromocional.Count - 1].ejercicio = periodosNomina.año.GetValueOrDefault();
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
                        if (mensajeResultado.noError != 0)
                        {
                            return mensajeResultado;
                        }
                    }
                    listPlazasPorEmpleadosMovOficial.AddRange(listPlazasPorEmpleadosMovTmp);
                    listPlazasPorEmpleadosMovOficial.Add(plazasPorEmpleadosMov);
                    if (listPlazasPorEmpleadosMovOficial[0].fechaInicial.GetValueOrDefault().CompareTo(fechaIni) > 0)
                    {
                        bool agregarMovNomConcepPromocional = true;
                        if (metodosMovimientosNomina.evaluaPeriodoMovAbarca2Meses(periodosNomina))
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

                        listPlazasPorEmpleadosMovOficial.InsertRange(0, obtenerAnteriorPlazasPorEmpleadosMov(listPlazasPorEmpleadosMovOficial[0], (DBContextSimple)dbContextSimple.context));
                        if (agregarMovNomConcepPromocional)
                        {
                            mensajeResultado = metodosMovimientosNomina.duplicarMovNomConcep(listMovNomConcepPromocional[0], listMovNomConcepPromocional[listMovNomConcepPromocional.Count - 1].numero.GetValueOrDefault() + 1, listPlazasPorEmpleadosMovOficial[0].plazasPorEmpleado, (DBContextSimple)dbContextSimple.context);
                            if (mensajeResultado.noError == 0)
                            {
                                listMovNomConcepPromocional.Add((MovNomConcep)mensajeResultado.resultado);
                            }
                            else
                            {
                                return mensajeResultado;
                            }
                        }
                    }
                    //</editor-fold>
                }
                else if (configuracionPercepcion_Plaza)
                {
                    //<editor-fold defaultstate="collapsed" desc="Programacion para cuando se aplica modificaciones salariales, aqui se obtienen los movimientos del empleado dentro del periodo">
                    if (periodosNomina != null)
                    {
                        fechaIni = periodosNomina.fechaInicial.GetValueOrDefault();
                        fechaFin = periodosNomina.fechaFinal.GetValueOrDefault();
                    }
                    List<PlazasPorEmpleadosMov> listPlazasPorEmpleadosMovTmp = new List<PlazasPorEmpleadosMov>();
                    listPlazasPorEmpleadosMovTmp.AddRange(obtenerPlazasPorEmpleadosMovRestantes(plazasPorEmpleadosMov.plazasPorEmpleado.razonesSociales.clave, plazasPorEmpleadosMov.plazasPorEmpleado.registroPatronal.id, periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault(), fechaBajaFiniq, plazasPorEmpleadosMov, periodosNomina, tipoCorrida, (DBContextSimple)dbContextSimple.context));
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
                        mensajeResultado = metodosMovimientosNomina.buscaMovimientosPlazasPorTipoYBaseAfecta(movimientosNomina.tipoCorrida, movimientosNomina.tipoNomina.clave, movimientosNomina.periodosNomina.id, listPlazasPorEmpleadosMovTmp[i].plazasPorEmpleado, movimientosNomina.centroDeCosto == null ? null : movimientosNomina.centroDeCosto.clave, Tipo.AUTOMATICO,
                            movimientosNomina.razonesSociales.clave, null, -1, movimientosNomina.concepNomDefi.clave, periodosNomina, fechaActual, centroDeCostoMovimiento, (DBContextSimple)dbContextSimple.context);
                        if (mensajeResultado.noError == 0)
                        {
                            listMovNomTmp.AddRange((List<MovNomConcep>)mensajeResultado.resultado);
                        }
                        else
                        {
                            break;
                        }
                        mensajeResultado = metodosMovimientosNomina.buscaMovimientosPlazasPorTipoYBaseAfecta(movimientosNomina.tipoCorrida, movimientosNomina.tipoNomina.clave, movimientosNomina.periodosNomina.id, listPlazasPorEmpleadosMovTmp[i].plazasPorEmpleado, movimientosNomina.centroDeCosto == null ? null : movimientosNomina.centroDeCosto.clave, Tipo.PERIODO,
                            movimientosNomina.razonesSociales.clave, null, -1, movimientosNomina.concepNomDefi.clave, periodosNomina, fechaActual, centroDeCostoMovimiento, (DBContextSimple)dbContextSimple.context);
                        if (mensajeResultado.noError == 0)
                        {
                            listMovNomTmp.AddRange((List<MovNomConcep>)mensajeResultado.resultado);
                        }
                        else
                        {
                            break;
                        }
                        mensajeResultado = metodosMovimientosNomina.buscaMovimientosPlazasPorTipoYBaseAfecta(movimientosNomina.tipoCorrida, movimientosNomina.tipoNomina.clave, movimientosNomina.periodosNomina.id, listPlazasPorEmpleadosMovTmp[i].plazasPorEmpleado, movimientosNomina.centroDeCosto == null ? null : movimientosNomina.centroDeCosto.clave, Tipo.REPETITIVO,
                            movimientosNomina.razonesSociales.clave, null, -1, movimientosNomina.concepNomDefi.clave, periodosNomina, fechaActual, centroDeCostoMovimiento, (DBContextSimple)dbContextSimple.context);
                        if (mensajeResultado.noError == 0)
                        {
                            listMovNomTmp.AddRange((List<MovNomConcep>)mensajeResultado.resultado);
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (mensajeResultado.noError != 0)
                    {
                        return mensajeResultado;
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
                            mensajeResultado = metodosMovimientosNomina.duplicarMovNomConcep(movimientosNomina, numero, listPlazasPorEmpleadosMovTmp[i].plazasPorEmpleado, (DBContextSimple)dbContextSimple.context);
                            if (mensajeResultado.noError == 0)
                            {
                                listMovNomConcepPromocional.Add((MovNomConcep)mensajeResultado.resultado);
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (mensajeResultado.noError != 0)
                        {
                            return mensajeResultado;
                        }
                    }
                    listMovNomConcepPromocional.Add(movimientosNomina);
                    listPlazasPorEmpleadosMovOficial.AddRange(listPlazasPorEmpleadosMovTmp);
                    listPlazasPorEmpleadosMovOficial.Add(plazasPorEmpleadosMov);
                    //</editor-fold>
                }
                else if (configuracionPercepcion_Plaza_Vigente)
                {
                    //<editor-fold defaultstate="collapsed" desc="Programacion para cuando se aplica modificaciones salariales, aqui se obtienen los movimientos del empleado dentro del periodo">
                    listMovNomConcepPromocional.Add(movimientosNomina);
                    if (periodosNomina != null)
                    {
                        fechaIni = periodosNomina.fechaInicial.GetValueOrDefault();
                        fechaFin = periodosNomina.fechaFinal.GetValueOrDefault();
                    }
                    List<PlazasPorEmpleadosMov> listPlazasPorEmpleadosMovTmp = new List<PlazasPorEmpleadosMov>();
                    listPlazasPorEmpleadosMovTmp.AddRange(obtenerPlazasPorEmpleadosMovRestantes(plazasPorEmpleadosMov.plazasPorEmpleado.razonesSociales.clave, plazasPorEmpleadosMov.plazasPorEmpleado.registroPatronal.id, periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault(), fechaBajaFiniq, plazasPorEmpleadosMov, periodosNomina, tipoCorrida, (DBContextSimple)dbContextSimple.context));
                    List<MovNomConcep> listMovNomTmp = new List<MovNomConcep>();
                    for (i = 0; i < listPlazasPorEmpleadosMovTmp.Count; i++)
                    {

                        mensajeResultado = metodosMovimientosNomina.buscaMovimientosPlazasPorTipoYBaseAfecta(movimientosNomina.tipoCorrida, movimientosNomina.tipoNomina.clave, movimientosNomina.periodosNomina.id, listPlazasPorEmpleadosMovTmp[i].plazasPorEmpleado, movimientosNomina.centroDeCosto == null ? null : movimientosNomina.centroDeCosto.clave, Tipo.AUTOMATICO,
                           movimientosNomina.razonesSociales.clave, null, -1, movimientosNomina.concepNomDefi.clave, periodosNomina, fechaActual, centroDeCostoMovimiento, (DBContextSimple)dbContextSimple.context);
                        if (mensajeResultado.noError == 0)
                        {
                            listMovNomTmp.AddRange((List<MovNomConcep>)mensajeResultado.resultado);
                        }
                        else
                        {
                            break;
                        }
                        mensajeResultado = metodosMovimientosNomina.buscaMovimientosPlazasPorTipoYBaseAfecta(movimientosNomina.tipoCorrida, movimientosNomina.tipoNomina.clave, movimientosNomina.periodosNomina.id, listPlazasPorEmpleadosMovTmp[i].plazasPorEmpleado, movimientosNomina.centroDeCosto == null ? null : movimientosNomina.centroDeCosto.clave, Tipo.PERIODO,
                            movimientosNomina.razonesSociales.clave, null, -1, movimientosNomina.concepNomDefi.clave, periodosNomina, fechaActual, centroDeCostoMovimiento, (DBContextSimple)dbContextSimple.context);
                        if (mensajeResultado.noError == 0)
                        {
                            listMovNomTmp.AddRange((List<MovNomConcep>)mensajeResultado.resultado);
                        }
                        else
                        {
                            break;
                        }
                        mensajeResultado = metodosMovimientosNomina.buscaMovimientosPlazasPorTipoYBaseAfecta(movimientosNomina.tipoCorrida, movimientosNomina.tipoNomina.clave, movimientosNomina.periodosNomina.id, listPlazasPorEmpleadosMovTmp[i].plazasPorEmpleado, movimientosNomina.centroDeCosto == null ? null : movimientosNomina.centroDeCosto.clave, Tipo.REPETITIVO,
                            movimientosNomina.razonesSociales.clave, null, -1, movimientosNomina.concepNomDefi.clave, periodosNomina, fechaActual, centroDeCostoMovimiento, (DBContextSimple)dbContextSimple.context);
                        if (mensajeResultado.noError == 0)
                        {
                            listMovNomTmp.AddRange((List<MovNomConcep>)mensajeResultado.resultado);
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (mensajeResultado.noError != 0)
                    {
                        return mensajeResultado;
                    }
                    listPlazasPorEmpleadosMovTmp.Add(plazasPorEmpleadosMov);
                    listPlazasPorEmpleadosMovTmp.AddRange(obtenerPlazasPorEmpleadosMovDentroPeriodo(tipoCorrida.clave, periodosNomina.fechaInicial.GetValueOrDefault(), periodosNomina.fechaFinal.GetValueOrDefault(), listPlazasPorEmpleadosMovTmp.ToArray(), fechaBajaFiniq, (DBContextSimple)dbContextSimple.context));
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
                    if (clavesMovEliminados.Count > 0)
                    {
                        metodosMovimientosNomina.eliminarMovimientosNominaBasura(clavesMovEliminados.ToArray(), dbContextSimple);
                        //getSession().flush();
                    }
                    listMovNomTmp = listMovNomTmp.Except(listMovNomTmp).ToList();
                    if (listPlazasPorEmpleadosMovTmp.Count > 1)
                    {
                        listPlazasPorEmpleadosMovTmp.Sort(new rowComparator());
                    }
                    listMovNomConcepPromocional[0].numero = 1;
                    int numero = 1;
                    if (listMovNomConcepPromocional.Count < listPlazasPorEmpleadosMovTmp.Count)
                    {
                        for (i = 1; i < listPlazasPorEmpleadosMovTmp.Count; i++)
                        {
                            numero++;
                            mensajeResultado = metodosMovimientosNomina.duplicarMovNomConcep(listMovNomConcepPromocional[0], numero, listPlazasPorEmpleadosMovTmp[i].plazasPorEmpleado, (DBContextSimple)dbContextSimple.context);
                            if (mensajeResultado.noError == 0)
                            {
                                listMovNomConcepPromocional.Add((MovNomConcep)mensajeResultado.resultado);
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (mensajeResultado.noError != 0)
                        {
                            return mensajeResultado;
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
                                    listPlazasPorEmpleadosMovOficial.InsertRange(0, obtenerAnteriorPlazasPorEmpleadosMov(listPlazasPorEmpleadosMovOficial[i], (DBContextSimple)dbContextSimple.context));
                                }
                                else
                                {
                                    listPlazasPorEmpleadosMovOficial.InsertRange(i - 1, obtenerAnteriorPlazasPorEmpleadosMov(listPlazasPorEmpleadosMovOficial[i], (DBContextSimple)dbContextSimple.context));
                                }
                                if (listPlazasPorEmpleadosMovOficial.Count != listMovNomConcepPromocional.Count)
                                {
                                    mensajeResultado = metodosMovimientosNomina.duplicarMovNomConcep(listMovNomConcepPromocional[0], listMovNomConcepPromocional[listMovNomConcepPromocional.Count - 1].numero.GetValueOrDefault() + 1, listPlazasPorEmpleadosMovOficial[i].plazasPorEmpleado, (DBContextSimple)dbContextSimple.context);
                                    if (mensajeResultado.noError == 0)
                                    {
                                        listMovNomConcepPromocional.Add((MovNomConcep)mensajeResultado.resultado);
                                    }
                                    else
                                    {
                                        return mensajeResultado;
                                    }
                                }
                            }
                        }
                    }
                    //            listPlazasPorEmpleadosMovOficial.add(plazasPorEmpleadosMov);
                    //            plazasPorEmpleadosMov.addAll(0, listPlazasPorEmpleadosMovTmp);
                    //</editor-fold>
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
                    if (metodosMovimientosNomina.evaluaPeriodoMovAbarca2Meses(periodosNomina))
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
                        if (metodosMovimientosNomina.evaluaPeriodoMovAbarca2Meses(periodosNomina))
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
                        metodosMovimientosNomina.eliminarMovimientosNominaBasura(clavesMovEliminados.ToArray(), dbContextSimple);  // pendiente
                        ////getSession().flush();
                        ////getSession().clear();
                    }
                }
                if (listPlazasPorEmpleadosMovOficial.Count > 1)
                {
                    if (metodosMovimientosNomina.evaluaPeriodoMovAbarca2Meses(periodosNomina))
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
                                listMovNomConcepPromocional[indiceMovimientoNomina].ejercicio = periodosNomina.año.GetValueOrDefault();
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
                                            listMovNomConcepPromocional[indiceMovimientoNomina].ejercicio = periodosNomina.año.GetValueOrDefault();
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
                                listMovNomConcepPromocional[indiceMovimientoNomina].ejercicio = periodosNomina.año.GetValueOrDefault();
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
                mensajeResultado.noError = 0;
                mensajeResultado.resultado = listObject;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerModificacionesDePlazasPorEmpleadoMov()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return mensajeResultado;
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

        private List<PlazasPorEmpleadosMov> obtenerAnteriorPlazasPorEmpleadosMov(PlazasPorEmpleadosMov plazasPorEmpleadosMov, DBContextSimple dbContextSimple)
        {
            List<PlazasPorEmpleadosMov> filtroPlazasPorEmpleadosMov = null;
            try
            {
                filtroPlazasPorEmpleadosMov = (from pmov in dbContextSimple.Set<PlazasPorEmpleadosMov>()
                                               where pmov.id != plazasPorEmpleadosMov.id && pmov.plazasPorEmpleado.id == plazasPorEmpleadosMov.plazasPorEmpleado.id &&
                                               pmov.fechaInicial == (from pmovx in dbContextSimple.Set<PlazasPorEmpleadosMov>()
                                                                     where pmovx.plazasPorEmpleado.fechaFinal >= plazasPorEmpleadosMov.fechaInicial && pmovx.fechaInicial <= plazasPorEmpleadosMov.fechaInicial &&
                                                                     pmovx.id != plazasPorEmpleadosMov.id && pmovx.plazasPorEmpleado.id == plazasPorEmpleadosMov.plazasPorEmpleado.id
                                                                     select new { pmovx.fechaInicial }).Max(f => f.fechaInicial)
                                               select pmov).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerAnteriorPlazasPorEmpleadosMov()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return filtroPlazasPorEmpleadosMov == null ? new List<PlazasPorEmpleadosMov>() : filtroPlazasPorEmpleadosMov;
        }

        private List<PlazasPorEmpleadosMov> obtenerPlazasPorEmpleadosMovDentroPeriodo(string claveTipoCorrida, DateTime fechaInicioPeriodo, DateTime fechaFinPeriodo, Object[] plazasPorEmpleadosMov, DateTime? fechaBajaFiniq, DBContextSimple dbContextSimple)
        {
            List<PlazasPorEmpleadosMov> filtroPlazasPorEmpleadosMov = null;
            try
            {
                List<Object> listIdPlazaPorEmpleadoMov = new List<Object>(), listIdPlazasPorEmpleado = new List<Object>();
                int i;
                for (i = 0; i < plazasPorEmpleadosMov.Length; i++)
                {
                    listIdPlazaPorEmpleadoMov.Add(((PlazasPorEmpleadosMov)plazasPorEmpleadosMov[i]).id);
                    listIdPlazasPorEmpleado.Add(((PlazasPorEmpleadosMov)plazasPorEmpleadosMov[i]).plazasPorEmpleado.id);
                }
                if (string.Equals(claveTipoCorrida, "FIN", StringComparison.OrdinalIgnoreCase))
                {
                    fechaInicioPeriodo = fechaBajaFiniq.GetValueOrDefault();
                }

                filtroPlazasPorEmpleadosMov = (from pmov in dbContextSimple.Set<PlazasPorEmpleadosMov>()
                                               where (pmov.fechaInicial >= fechaInicioPeriodo && pmov.fechaInicial <= fechaFinPeriodo) && pmov.plazasPorEmpleado.fechaFinal >= fechaInicioPeriodo &&
                                                      pmov.fechaInicial <= fechaFinPeriodo && !listIdPlazaPorEmpleadoMov.Contains(pmov.id) && listIdPlazasPorEmpleado.Contains(pmov.plazasPorEmpleado.id)
                                               /* && pmov.cambioSalarioPor == true*/
                                               select pmov).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerPlazasPorEmpleadosMovDentroPeriodo()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return filtroPlazasPorEmpleadosMov == null ? new List<PlazasPorEmpleadosMov>() : filtroPlazasPorEmpleadosMov;
        }

        //Usando PlazasPorEmpleado
        private List<PlazasPorEmpleadosMov> obtenerPlazasPorEmpleadosMovRestantes(string claveRazonSocial, decimal idRegPatronal, DateTime fechaInicioPeriodo, DateTime fechaFinPeriodo, DateTime? fechaBajaFiniq, PlazasPorEmpleadosMov plazasPorEmpleadosMovEjecutandose, PeriodosNomina periodosNomina,
            TipoCorrida tipoCorrida, DBContextSimple dbContextSimple)
        {
            List<PlazasPorEmpleadosMov> filtroPlazasPorEmpleadosMov = null;
            try
            {
                var queryC34lavesEmp = (from cfdi in dbContextSimple.Set<CFDIEmpleado>()
                                        where cfdi.razonesSociales.clave == claveRazonSocial &&
                                        cfdi.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.id == plazasPorEmpleadosMovEjecutandose.plazasPorEmpleado.empleados.id && cfdi.cfdiRecibo.statusTimbrado == StatusTimbrado.TIMBRADO
                                        select new { cfdi });

                if (tipoCorrida != null)
                {
                    queryC34lavesEmp = (from subQ in queryC34lavesEmp where subQ.cfdi.tipoCorrida.clave == tipoCorrida.clave select subQ);
                }

                if (periodosNomina != null)
                {
                    queryC34lavesEmp = (from subQ in queryC34lavesEmp where subQ.cfdi.periodosNomina.id == periodosNomina.id select subQ);
                }

                object[] claveEmpleado = (from subQ in queryC34lavesEmp
                                          select subQ.cfdi.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave).ToArray();

                string claveTipoCorrida = tipoCorrida == null ? "" : tipoCorrida.clave;


                if (string.Equals(claveTipoCorrida, "FIN", StringComparison.OrdinalIgnoreCase))
                {
                    fechaInicioPeriodo = fechaBajaFiniq.GetValueOrDefault();
                }

                var query = from pMov in dbContextSimple.Set<PlazasPorEmpleadosMov>()
                            where (from pMovX in dbContextSimple.Set<PlazasPorEmpleadosMov>()
                                   where pMovX.plazasPorEmpleado.razonesSociales.clave == claveRazonSocial && pMovX.plazasPorEmpleado.registroPatronal.id == idRegPatronal &&
                                        ((pMovX.fechaInicial <= fechaInicioPeriodo) || (pMovX.fechaInicial >= fechaInicioPeriodo && pMovX.fechaInicial <= fechaInicioPeriodo)) &&
                                        ((pMovX.plazasPorEmpleado.fechaFinal >= fechaFinPeriodo) || (pMovX.plazasPorEmpleado.fechaFinal >= fechaInicioPeriodo && pMovX.plazasPorEmpleado.fechaFinal <= fechaInicioPeriodo)) &&
                                        pMovX.plazasPorEmpleado.empleados.id == plazasPorEmpleadosMovEjecutandose.plazasPorEmpleado.empleados.id &&
                                        !(from px in dbContextSimple.Set<PlazasPorEmpleado>()
                                          where px.plazaReIngreso != null && px.razonesSociales.clave == claveRazonSocial
                                          select new { px.plazaReIngreso.id }).Contains(new { pMov.plazasPorEmpleado.id }) && !claveEmpleado.Contains(pMov.plazasPorEmpleado.empleados.clave)
                                   group new { pMovX } by new
                                   {
                                       pMovX.plazasPorEmpleado.referencia
                                   } into grupo
                                   select new { idPPMMax = grupo.Max(p => p.pMovX.id) }
                                    ).Contains(new { idPPMMax = pMov.id })
                            select new { pMov = pMov }
                ;

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerPlazasPorEmpleadosMovRestantes()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }
            return filtroPlazasPorEmpleadosMov == null ? new List<PlazasPorEmpleadosMov>() : filtroPlazasPorEmpleadosMov;
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