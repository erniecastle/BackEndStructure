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
    public class MetodosPeriodosNomina
    {
        private Mensaje mensajeResultado = new Mensaje();
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        private int ENERO = 1;

        //List<PeriodosNomina>
        public Mensaje buscarPeriodosPorRangoMeses(int rangoDeMes, DateTime fechaPeriodoNomina, string claveTipoNomina, string claveTipoCorrida, DBContextSimple dbContextSimple)
        {
            inicializaVariableMensaje();
            List<PeriodosNomina> periodos = null;
            try
            {
                DateTime fechaRango = fechaPeriodoNomina;
                fechaRango.AddMonths(fechaPeriodoNomina.Month + rangoDeMes);
                int mesIni = -1, mesFin = -1;
                int yearPeriodo = fechaPeriodoNomina.Year;
                int mesPeriodo;
                int mesPeriodoRango;
                int mesEnero = -1;


                var query = from p in dbContextSimple.Set<PeriodosNomina>()
                            select new { p };

                if (mesIni == -1 & mesFin == -1)
                {
                    mesIni = fechaPeriodoNomina.Month;
                    mesFin = fechaRango.Month;
                    mesEnero = ENERO;
                }
                mesPeriodo = mesIni;
                mesPeriodoRango = mesFin;
                if (fechaPeriodoNomina.Year > fechaRango.Year)
                {
                    query = from sub in query
                            where sub.p.tipoNomina.clave == claveTipoNomina && sub.p.tipoCorrida.clave == claveTipoCorrida && sub.p.año == yearPeriodo && sub.p.acumularAMes.Value.Month <= mesPeriodo ||
                                (sub.p.tipoNomina.clave == claveTipoNomina && sub.p.tipoCorrida.clave == claveTipoCorrida && sub.p.año == yearPeriodo - 1 && sub.p.acumularAMes.Value.Month == mesEnero && sub.p.acumularAMes.Value.Year == yearPeriodo) ||
                                (sub.p.tipoNomina.clave == claveTipoNomina && sub.p.tipoCorrida.clave == claveTipoCorrida && sub.p.año == yearPeriodo - 1 && sub.p.acumularAMes.Value.Month >= mesPeriodoRango)
                            select sub;
                }
                else if (fechaPeriodoNomina.Year < fechaRango.Year)
                {
                    query = from sub in query
                            where sub.p.tipoNomina.clave == claveTipoNomina && sub.p.tipoCorrida.clave == claveTipoCorrida && sub.p.año == yearPeriodo && sub.p.acumularAMes.Value.Month == mesPeriodo ||
                                (sub.p.tipoNomina.clave == claveTipoNomina && sub.p.tipoCorrida.clave == claveTipoCorrida && sub.p.año == yearPeriodo && sub.p.acumularAMes.Value.Month == mesEnero && sub.p.acumularAMes.Value.Year == yearPeriodo + 1) ||
                                (sub.p.tipoNomina.clave == claveTipoNomina && sub.p.tipoCorrida.clave == claveTipoCorrida && sub.p.año == yearPeriodo + 1 && sub.p.acumularAMes.Value.Month <= mesPeriodoRango && sub.p.acumularAMes.Value.Year == yearPeriodo + 2)
                            select sub;
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
                    mesPeriodo = mesIni;
                    mesPeriodoRango = mesFin;
                    if (fechaRango.Month == ENERO) //Mes Enero
                    {
                        mesPeriodo = fechaPeriodoNomina.Month;
                        mesPeriodoRango = fechaRango.Month;
                        query = from sub in query
                                where sub.p.tipoNomina.clave == claveTipoNomina && sub.p.tipoCorrida.clave == claveTipoCorrida && sub.p.año == yearPeriodo && (sub.p.acumularAMes.Value.Month <= mesPeriodo && sub.p.acumularAMes.Value.Year == yearPeriodo) ||
                                    (sub.p.tipoNomina.clave == claveTipoNomina && sub.p.tipoCorrida.clave == claveTipoCorrida && sub.p.año == yearPeriodo - 1 && sub.p.acumularAMes.Value.Month == mesPeriodoRango && sub.p.acumularAMes.Value.Year == yearPeriodo)
                                select sub;
                    }
                    else
                    {
                        query = from sub in query
                                where sub.p.tipoNomina.clave == claveTipoNomina && sub.p.tipoCorrida.clave == claveTipoCorrida && sub.p.año == yearPeriodo && (sub.p.acumularAMes.Value.Month >= mesPeriodo && sub.p.acumularAMes.Value.Month <= mesPeriodoRango)
                                select sub;
                    }
                }
                periodos = (from sub in query select sub.p).ToList();
                mensajeResultado.noError = 0;
                mensajeResultado.resultado = periodos;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscarPeriodosPorRangoMeses()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return mensajeResultado;
        }
        public Mensaje buscaPeriodoNominaActual(String claveTipoNomina, String claveTipoCorrida, decimal? idPeriodo, DateTime fechaActual, DBContextSimple dbContextSimple)
        {
            inicializaVariableMensaje();
            PeriodosNomina periodos = null;
            try {
                var query = from p in dbContextSimple.Set<PeriodosNomina>()
                            join t in dbContextSimple.Set<TipoNomina>() on p.tipoNomina.id equals t.id
                            join c in dbContextSimple.Set<TipoCorrida>() on p.tipoCorrida.id equals c.id
                            select new { p ,t,c};
                if (idPeriodo == null)
                {
                    query = from sub in query
                            where (fechaActual >= sub.p.fechaInicial && fechaActual <= sub.p.fechaFinal) && sub.t.clave == claveTipoNomina && sub.c.clave == (claveTipoCorrida == null ? "PER" : claveTipoCorrida)
                            select sub;
                }
                else {

                    query = from sub in query
                            where sub.p.id == idPeriodo
                            select sub;
                }
                periodos = query.Select(p=>p.p).Single();
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                mensajeResultado.resultado = periodos;
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscaPeriodoNominaActual()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return mensajeResultado;
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