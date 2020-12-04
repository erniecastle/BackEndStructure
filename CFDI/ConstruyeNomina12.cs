using Exitosw.Payroll.Core.CFDI.Sat.Catalogos;
using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Entity.entidad.cfdi;
using Exitosw.Payroll.Entity.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace Exitosw.Payroll.Core.CFDI
{
    public class ConstruyeNomina12
    {
        private int anioServicio = 0;
        private bool existeIncapacidades;
        private ConcepNomDefi conceptoNominaSubsidio;
        public Mensaje mensajeNomina { get; set; }
        private double totalDeducciones = 0.0, totalPercepciones = 0.0, totalOtrosPagos = 0.0;

        public ConstruyeNomina12()
        {
            mensajeNomina = new Mensaje();
            mensajeNomina.error=("");
            mensajeNomina.noError=(0);
           
        }
        public Nomina generaComplementoNomina(CFDIEmpleado cfdiEmpleado, ConcepNomDefi conceptoNominaSubsidio1)
        {
            conceptoNominaSubsidio = conceptoNominaSubsidio1;
            Nomina nomina = contruyeNomina12(cfdiEmpleado);
            return nomina;
        }

        private Nomina contruyeNomina12(CFDIEmpleado cfdiEmpleado)
        {
            Nomina nomina = new Nomina();
            try
            {
                nomina.Version = ("1.2");  
                nomina.FechaFinalPago = (UtileriasSat.castXmlFechaFormatoIso8601(cfdiEmpleado.fechaFinalPago));  
                nomina.FechaInicialPago = (UtileriasSat.castXmlFechaFormatoIso8601(cfdiEmpleado.fechaInicioPago));  
                nomina.FechaPago = (UtileriasSat.castXmlFechaFormatoIso8601(cfdiEmpleado.fechaPago));  
                nomina.NumDiasPagados = (UtileriasSat.castNumerosToBigDecimal(cfdiEmpleado.numeroDiasPago));  
                nomina.Emisor = (construyeNominaEmisor(cfdiEmpleado));
                nomina.Receptor = (construyeNominaReceptor(cfdiEmpleado));
                anioServicio = UtileriasSat.getAniosServicio(cfdiEmpleado.fechaInicioRelLaboral.GetValueOrDefault(), cfdiEmpleado.fechaFinalPago);
                if (string.Equals(cfdiEmpleado.tipoCorrida.clave, "ASI", StringComparison.OrdinalIgnoreCase))
                {// duda, no solo una corrida asimilados es tipo de nomina especial, tambien finiquitos, vacaciones 
                    nomina.NumDiasPagados = (UtileriasSat.castNumerosToBigDecimal(1));
                    nomina.TipoNomina =ManejadorEnum.GetDescription(CTipoNomina.E);
                    if (nomina.Receptor != null)
                    {
                        nomina.Receptor.PeriodicidadPago = ("99");
                    }
                    cargaConceptosANomina(nomina, cfdiEmpleado.cfdiRecibo.cfdiReciboConcepto, cfdiEmpleado.cfdiRecibo.cfdiReciboHrsExtras);
                    if (existeIncapacidades)
                    {
                        nomina.Incapacidades = (contruyeNominaListaIncapacidades(cfdiEmpleado.cfdiRecibo.cfdiReciboIncapacidad).ToArray());
                    }
                }
                else
                {
                    nomina.TipoNomina = ManejadorEnum.GetDescription(CTipoNomina.O);
                    cargaConceptosANomina(nomina, cfdiEmpleado.cfdiRecibo.cfdiReciboConcepto, cfdiEmpleado.cfdiRecibo.cfdiReciboHrsExtras);
                    if (existeIncapacidades)
                    {
                        NominaIncapacidad[] incapacidades = contruyeNominaListaIncapacidades(cfdiEmpleado.cfdiRecibo.cfdiReciboIncapacidad).ToArray();
                        if (incapacidades.Length>0) {
                            nomina.Incapacidades = incapacidades;
                        }
                      
                    }
                }
            }
            catch (Exception ex)
            {
                mensajeNomina.error = (ex.Message);
                mensajeNomina.noError = (1);
                //utilSat.bitacora(ex.Message);
                ///nomina = null;
            }
            return nomina;
        }

        private NominaEmisor construyeNominaEmisor(CFDIEmpleado cfdiEmpleado)
        {
            NominaEmisor emisor = new NominaEmisor();
            /// emisor.setCurp("");   ///Condicional cuando es persona fisica 
            ///emisor.setEntidadSNCF(creaEntidadSNCF());  ////Condicional pendiente
            ///!cfdiEmpleado.tipoContrato.equalsIgnoreCase("09") | !cfdiEmpleado.tipoContrato.equalsIgnoreCase("10")
                //    | !cfdiEmpleado.getTipoContrato().equalsIgnoreCase("99")

            //checar si la empresa es persona fisica, de donde saco la curp
            /*if (cfdiEmpleado.CURP == null ? false : cfdiEmpleado.CURP.Trim().Length > 0)
            {
                emisor.Curp = cfdiEmpleado.CURP;

            }*/

            if (!string.Equals(cfdiEmpleado.tipoContrato, "09", StringComparison.OrdinalIgnoreCase) || !string.Equals(cfdiEmpleado.tipoContrato, "10", StringComparison.OrdinalIgnoreCase)
                | !string.Equals(cfdiEmpleado.tipoContrato, "99", StringComparison.OrdinalIgnoreCase))
            {
                if (cfdiEmpleado.noRegistroPatronal == null ? false : cfdiEmpleado.noRegistroPatronal.Trim().Length > 0)
                {
                    emisor.RegistroPatronal = (cfdiEmpleado.noRegistroPatronal); //Condicional  20 caract maximo
                }
            }
            /////emisor.setRfcPatronOrigen("");  ///pendiente opcional
            return emisor;
        }

        private NominaReceptor construyeNominaReceptor(CFDIEmpleado cfdiEmpleado)
        {
            NominaReceptor receptor = new NominaReceptor();
            receptor.Curp = (cfdiEmpleado.CURP);
            if (cfdiEmpleado.noSeguroSocial == null ? false : cfdiEmpleado.noSeguroSocial.Trim().Length > 0)
            {
                receptor.NumSeguridadSocial = (cfdiEmpleado.noSeguroSocial);
            }

            if (cfdiEmpleado.fechaInicioRelLaboral != null)
            {
                receptor.FechaInicioRelLaboral = (UtileriasSat.castXmlFechaFormatoIso8601(cfdiEmpleado.fechaInicioRelLaboral.GetValueOrDefault()));
            }
            if (cfdiEmpleado.antiguedad != null)
            {
                //"P".concat(String.valueOf(cfdiEmpleado.getAntiguedad())).concat("W")
                receptor.Antigüedad = (string.Concat("P", string.Concat(cfdiEmpleado.antiguedad, "W")));
            }
            ////////        if (cfdiEmpleado.getAntiguedadYMD().trim().length() > 0) {
            ////////            receptor.setAntiguedad(cfdiEmpleado.getAntiguedadYMD());
            ////////        }
            receptor.TipoContrato = (cfdiEmpleado.tipoContrato);
            if (cfdiEmpleado.jornada == null ? false : cfdiEmpleado.jornada.Trim().Length > 0)
            {
                receptor.TipoJornada = (cfdiEmpleado.jornada);
            }
            receptor.TipoRegimen = (cfdiEmpleado.regimenContratacion); //@
            receptor.NumEmpleado = (cfdiEmpleado.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave);

            if (cfdiEmpleado.departamento == null ? false : cfdiEmpleado.departamento.Trim().Length > 0)
            {
                receptor.Departamento = (cfdiEmpleado.departamento);
            }

            if(cfdiEmpleado.puesto == null ? false : cfdiEmpleado.puesto.Trim().Length > 0)
            {
                receptor.Puesto = (cfdiEmpleado.puesto);
            }
            if (cfdiEmpleado.riesgoPuesto != null)
            {
                receptor.RiesgoPuesto = (cfdiEmpleado.riesgoPuesto);
            }

            receptor.PeriodicidadPago = (cfdiEmpleado.periodiciadadPago);  

            if (cfdiEmpleado.claveBancoSat == null ? false : cfdiEmpleado.claveBancoSat.Trim().Length > 0)
            {
                receptor.Banco = (cfdiEmpleado.claveBancoSat);
            }
            if(cfdiEmpleado.cuentaBancaria == null ? false : cfdiEmpleado.cuentaBancaria.Trim().Length > 0)
            {
                receptor.CuentaBancaria = (UtileriasSat.castNumerosToBigInteger(cfdiEmpleado.cuentaBancaria).ToString());
            }

            if (cfdiEmpleado.salBaseCotAport == null ? false: cfdiEmpleado.salBaseCotAport>0)
            { 
                receptor.SalarioBaseCotApor = (UtileriasSat.castNumerosToImporteMX(cfdiEmpleado.salBaseCotAport));
            }
            if (cfdiEmpleado.salIntIMSS == null ? false: cfdiEmpleado.salIntIMSS>0)
            { 
                receptor.SalarioDiarioIntegrado = (UtileriasSat.castNumerosToImporteMX(cfdiEmpleado.salIntIMSS));
            }
            try
            {
                //receptor.ClaveEntFed=(CEstado.fromValue(cfdiEmpleado.estado));
                receptor.ClaveEntFed = (cfdiEmpleado.estado);
            }
            catch (Exception e)
            {
                receptor.ClaveEntFed = (null);
            }

            ///subcontratacion
            
            ///receptor.setSindicalizado(SCHEMALOCATION_NOMINA);    //opcional   pendiente
            
            return receptor;
        }

        private Nomina cargaConceptosANomina(Nomina nomina, List<CFDIReciboConcepto> listConceptos, List<CFDIReciboHrsExtras> listReciboHorasExtras)
        {
            totalDeducciones = 0.0;
            totalPercepciones = 0.0;
            totalOtrosPagos = 0.0;
            listConceptos = listConceptos == null ? new List<CFDIReciboConcepto>() : listConceptos;
            if (listConceptos.Count() > 0)
            {
                if (nomina == null)
                {
                    nomina = new Nomina();
                }
                List<CFDIReciboConcepto> listPercepciones = new List<CFDIReciboConcepto>();
                List<CFDIReciboConcepto> listOtrosPagos = new List<CFDIReciboConcepto>();
                List<CFDIReciboConcepto> listDeducciones = new List<CFDIReciboConcepto>();
                int i;
                for (i = 0; i < listConceptos.Count(); i++)
                {
                    if (listConceptos[i].otroPago)
                    {
                        listOtrosPagos.Add(listConceptos[i]);
                    }
                    else if (string.Equals(listConceptos[i].tipoNaturaleza, ManejadorEnum.GetDescription(Naturaleza.PERCEPCION)))
                    {
                        listPercepciones.Add(listConceptos[i]);
                    }
                    else if (string.Equals(listConceptos[i].tipoNaturaleza, ManejadorEnum.GetDescription(Naturaleza.DEDUCCION)))
                    {
                        listDeducciones.Add(listConceptos[i]);
                    }
                }
                nomina.Percepciones=(contruyeNominaListaPercepciones(listPercepciones, listReciboHorasExtras));
                if (listOtrosPagos.Any())
                {
                    nomina.OtrosPagos = (contruyeNominaListaOtrosPagos(listOtrosPagos).ToArray());
                }
                else {
                    nomina.OtrosPagos = (construyeSinSubsidio().ToArray());
                }
                
                nomina.Deducciones=(contruyeNominaListaDeducciones(listDeducciones));
                nomina.TotalPercepciones=(UtileriasSat.castNumerosToImporteMX(totalPercepciones));
                nomina.TotalOtrosPagos=(UtileriasSat.castNumerosToImporteMX(totalOtrosPagos));
                nomina.TotalDeducciones=(UtileriasSat.castNumerosToImporteMX(totalDeducciones));
            }
            return nomina;
        }

        private NominaPercepciones contruyeNominaListaPercepciones(List<CFDIReciboConcepto> listConceptos, List<CFDIReciboHrsExtras> listReciboHorasExtras)
        {
            NominaPercepciones percepciones = null;
            existeIncapacidades = false;
            listConceptos = listConceptos == null ? new List<CFDIReciboConcepto>() : listConceptos;
            if (listConceptos.Count() > 0)
            {
                double ultSueldo = 0.0;
                percepciones = new NominaPercepciones();
                List<NominaPercepcionesPercepcion> listPercepciones = new List<NominaPercepcionesPercepcion>();
                int i;
                bool jubilacion = false, separacionIndeminizacion = false;
                Double totalExento = 0.0, totalGravable = 0.0, totalSueldo = 0.0, totalSeparacionIndeminizacion = 0.0, totalJubilacionPension = 0.0;
                ConceptosJubilacion conceptosJubilacion = ConceptosJubilacion.NINGUNA;
                for (i = 0; i < listConceptos.Count(); i++)
                {
                    totalExento = totalExento + listConceptos[i].importeExento;
                    totalGravable = totalGravable + listConceptos[i].importeGravable;
                    if (string.Equals(listConceptos[i].claveSAT,"014",StringComparison.OrdinalIgnoreCase))
                    {
                        existeIncapacidades = true;
                    }
                    if (string.Equals(listConceptos[i].claveSAT, "001", StringComparison.OrdinalIgnoreCase))
                    {
                        existeIncapacidades = true;
                        ultSueldo = listConceptos[i].importeExento + listConceptos[i].importeGravable;
                    }

                    if (string.Equals(listConceptos[i].claveSAT, "022", StringComparison.OrdinalIgnoreCase) | string.Equals(listConceptos[i].claveSAT, "023", StringComparison.OrdinalIgnoreCase)
                            | string.Equals(listConceptos[i].claveSAT, "025", StringComparison.OrdinalIgnoreCase))
                    {  /////SeparacionIndeminizacion
                        totalSeparacionIndeminizacion = totalSeparacionIndeminizacion + listConceptos[i].importeExento + listConceptos[i].importeGravable;
                        separacionIndeminizacion = true;
                    }
                    else if (string.Equals(listConceptos[i].claveSAT, "039", StringComparison.OrdinalIgnoreCase) | string.Equals(listConceptos[i].claveSAT, "044", StringComparison.OrdinalIgnoreCase))
                    {  ///JubilacionPension
                        totalJubilacionPension = totalJubilacionPension + listConceptos[i].importeExento + listConceptos[i].importeGravable;
                        jubilacion = true;
                        if (string.Equals(listConceptos[i].claveSAT, "039", StringComparison.OrdinalIgnoreCase))
                        {
                            conceptosJubilacion = ConceptosJubilacion.TOTAL;
                        }
                        else if (string.Equals(listConceptos[i].claveSAT, "044", StringComparison.OrdinalIgnoreCase))
                        {
                            conceptosJubilacion = ConceptosJubilacion.PARCIAL;
                        }
                    }
                    else
                    {
                        totalSueldo = totalSueldo + listConceptos[i].importeExento + listConceptos[i].importeGravable;
                    }
                    listPercepciones.Add(contruyeNominaPercepcion(listConceptos[i], listReciboHorasExtras));
                }
                percepciones.Percepcion = listPercepciones.ToArray();

                totalPercepciones = totalExento + totalGravable;
                percepciones.TotalExento=(UtileriasSat.castNumerosToBigDecimal(totalExento));
                percepciones.TotalGravado=(UtileriasSat.castNumerosToBigDecimal(totalGravable));
                percepciones.TotalSueldos=(UtileriasSat.castNumerosToBigDecimal(totalSueldo));
               

                if (jubilacion)
                {
                    percepciones.JubilacionPensionRetiro=(createJubilacionRetiro(conceptosJubilacion, totalJubilacionPension, null, ultSueldo));   //pendiente
                    percepciones.TotalJubilacionPensionRetiro=(UtileriasSat.castNumerosToBigDecimal(totalJubilacionPension));
                  
                }
                if (separacionIndeminizacion)
                {
                    percepciones.TotalSeparacionIndemnizacion=(UtileriasSat.castNumerosToBigDecimal(totalSeparacionIndeminizacion));
                    percepciones.SeparacionIndemnizacion=(createSeparacionIndemnizacion(totalSeparacionIndeminizacion, ultSueldo, anioServicio));
                   
                }
            }
            return percepciones;
        }

        private NominaPercepcionesPercepcion contruyeNominaPercepcion(CFDIReciboConcepto cfdiConceptoPercepcion, List<CFDIReciboHrsExtras> listReciboHorasExtras)
        {
            NominaPercepcionesPercepcion percepcion = new NominaPercepcionesPercepcion();
            percepcion.Clave=(cfdiConceptoPercepcion.claveConcepto);
            percepcion.Concepto=(cfdiConceptoPercepcion.descripcionConcepto);
            percepcion.ImporteExento=(UtileriasSat.castNumerosToBigDecimal(cfdiConceptoPercepcion.importeExento));
            percepcion.ImporteGravado=(UtileriasSat.castNumerosToBigDecimal(cfdiConceptoPercepcion.importeGravable));
            percepcion.TipoPercepcion=(cfdiConceptoPercepcion.claveSAT);
            if (string.Equals(cfdiConceptoPercepcion.claveSAT,"045"))
            { // accciones y titulos
                percepcion.AccionesOTitulos=(createAccionesTitulos());
            }
            else if (string.Equals(cfdiConceptoPercepcion.claveSAT, "019"))
            {  /// horas extras
                contruyeNominaListaHorasExtras(percepcion.HorasExtra.ToList(), listReciboHorasExtras);
            }
            return percepcion;
        }

        private NominaPercepcionesPercepcionAccionesOTitulos createAccionesTitulos()
        {
            NominaPercepcionesPercepcionAccionesOTitulos accionesOTitulos = new NominaPercepcionesPercepcionAccionesOTitulos();
            accionesOTitulos.PrecioAlOtorgarse=(Decimal.Zero);
            accionesOTitulos.ValorMercado=(Decimal.Zero);
            return accionesOTitulos;
        }

        private List<NominaPercepcionesPercepcionHorasExtra> contruyeNominaListaHorasExtras(List<NominaPercepcionesPercepcionHorasExtra> horasExtras, List<CFDIReciboHrsExtras> listHrsExtras)
        {
            listHrsExtras = listHrsExtras == null ? new List<CFDIReciboHrsExtras>() : listHrsExtras;
            if (listHrsExtras.Count() > 0)
            {
                int i;
                for (i = 0; i < listHrsExtras.Count(); i++)
                {
                    horasExtras.Add(contruyeNominaHoraExtra(listHrsExtras[i]));
                }
            }
            return horasExtras;
        }

        private NominaPercepcionesPercepcionHorasExtra contruyeNominaHoraExtra(CFDIReciboHrsExtras cfdiReciboHrsExtras)
        {
            NominaPercepcionesPercepcionHorasExtra horaExtra = new NominaPercepcionesPercepcionHorasExtra();
            horaExtra.Dias=(cfdiReciboHrsExtras.dias);
            horaExtra.HorasExtra=(cfdiReciboHrsExtras.horasExtras);
            Double importePagado = cfdiReciboHrsExtras.importeExento + cfdiReciboHrsExtras.importeGravable;
            horaExtra.ImportePagado=(UtileriasSat.castNumerosToImporteMX(importePagado)); ///pendiente
            horaExtra.TipoHoras=(cfdiReciboHrsExtras.tipoHoras);
            return horaExtra;
        }

        private NominaPercepcionesJubilacionPensionRetiro createJubilacionRetiro(ConceptosJubilacion conceptosJubilacion, Double total, Double? montoDiario, Double ultSueldoMen)
        {
            NominaPercepcionesJubilacionPensionRetiro jubilacionPensionRetiro = new NominaPercepcionesJubilacionPensionRetiro();
            if (conceptosJubilacion == ConceptosJubilacion.TOTAL)
            {
                jubilacionPensionRetiro.TotalUnaExhibicion=(UtileriasSat.castNumerosToBigDecimal(total));
            }
            if (conceptosJubilacion == ConceptosJubilacion.PARCIAL)
            {
                jubilacionPensionRetiro.MontoDiario=(UtileriasSat.castNumerosToBigDecimal(total));
                jubilacionPensionRetiro.TotalParcialidad=(UtileriasSat.castNumerosToBigDecimal(montoDiario));
            }
            if (ultSueldoMen.CompareTo(total) > 0)
            {
                jubilacionPensionRetiro.IngresoAcumulable=(UtileriasSat.castNumerosToBigDecimal(total));
            }
            else
            {
                jubilacionPensionRetiro.IngresoAcumulable=(UtileriasSat.castNumerosToBigDecimal(ultSueldoMen));
            }
            jubilacionPensionRetiro.IngresoNoAcumulable=(UtileriasSat.castNumerosToBigDecimal(total - ultSueldoMen));

            return jubilacionPensionRetiro;
        }

        /*finiquito*/
        private NominaPercepcionesSeparacionIndemnizacion createSeparacionIndemnizacion(Double totalSeparacionIndeminizacion, Double ultSueldoMes, int anioServ)
        {
            NominaPercepcionesSeparacionIndemnizacion separacionIndemnizacion = new NominaPercepcionesSeparacionIndemnizacion();

            separacionIndemnizacion.NumAñosServicio=(anioServ);
            separacionIndemnizacion.TotalPagado=(UtileriasSat.castNumerosToBigDecimal(totalSeparacionIndeminizacion));
            separacionIndemnizacion.UltimoSueldoMensOrd=(UtileriasSat.castNumerosToBigDecimal(ultSueldoMes));
            if (ultSueldoMes.CompareTo(totalSeparacionIndeminizacion) > 0)
            {
                separacionIndemnizacion.IngresoAcumulable=(UtileriasSat.castNumerosToBigDecimal(totalSeparacionIndeminizacion));
            }
            else
            {
                separacionIndemnizacion.IngresoAcumulable=(UtileriasSat.castNumerosToBigDecimal(ultSueldoMes));
            }
            separacionIndemnizacion.IngresoNoAcumulable=(UtileriasSat.castNumerosToBigDecimal(totalSeparacionIndeminizacion - ultSueldoMes));
            return separacionIndemnizacion;
        }

        private List<NominaOtroPago> contruyeNominaListaOtrosPagos(List<CFDIReciboConcepto> listConceptosOtros)
        {
            NominaOtroPago otrosPagos = null;
            List<NominaOtroPago> listOtrosPagos = new List<NominaOtroPago>();
            listConceptosOtros = listConceptosOtros == null ? new List<CFDIReciboConcepto>() : listConceptosOtros;
            if (listConceptosOtros.Count() > 0)
            {
                otrosPagos = new NominaOtroPago();
              //  List<NominaOtroPago> listOtrosPagos = new List<NominaOtroPago>();
                int i;
                Double totalExento = 0.0, totalGravable = 0.0;
                for (i = 0; i < listConceptosOtros.Count(); i++)
                {
                    if (listConceptosOtros[i].tipoNaturaleza != "CALCULO") {
                        totalExento = totalExento + listConceptosOtros[i].importeExento;
                        totalGravable = totalGravable + listConceptosOtros[i].importeGravable;
                    }
                    
                   
                }
                
                listOtrosPagos.Add(contruyeNominaOtroPago(listConceptosOtros));
                totalOtrosPagos = totalExento + totalGravable;
                
               // otrosPagos = listOtrosPagos;
            }
            return listOtrosPagos;
        }

        private NominaOtroPago contruyeNominaOtroPago(List<CFDIReciboConcepto> listConceptosOtros)
        {
            NominaOtroPago otroPago = new NominaOtroPago();

            int i;
            Double total1 = 0.0, total2 = 0.0;
            for (i = 0; i < listConceptosOtros.Count(); i++)
            {
                if (i == 0) {
                    total1 = listConceptosOtros[i].importeGravable + listConceptosOtros[i].importeExento;
                }
                if (i == 1)
                {
                    total2 = listConceptosOtros[i].importeExento + listConceptosOtros[i].importeGravable;
                }
            }

            if (total1 < total2)
            {
                otroPago.Clave = (listConceptosOtros[0].claveConcepto);
                otroPago.Concepto = (listConceptosOtros[0].descripcionConcepto);
                otroPago.TipoOtroPago = (listConceptosOtros[0].claveSAT);
                otroPago.Importe = (UtileriasSat.castNumerosToBigDecimal(listConceptosOtros[0].importeExento));

                otroPago.SubsidioAlEmpleo=(creaSubsidioAlEmpleo(total2));

            }

            if (total1 > total2)
            {
                if (listConceptosOtros.Count() == 1)
                {
                    otroPago.Clave = (listConceptosOtros[0].claveConcepto);
                    otroPago.Concepto = (listConceptosOtros[0].descripcionConcepto);
                    otroPago.TipoOtroPago = (listConceptosOtros[0].claveSAT);
                    otroPago.Importe = (UtileriasSat.castNumerosToBigDecimal(listConceptosOtros[0].importeExento));
                }
                else
                {
                    otroPago.Clave = (listConceptosOtros[1].claveConcepto);
                    otroPago.Concepto = (listConceptosOtros[1].descripcionConcepto);
                    otroPago.TipoOtroPago = (listConceptosOtros[1].claveSAT);
                    otroPago.Importe = (UtileriasSat.castNumerosToBigDecimal(listConceptosOtros[1].importeExento));
                }

                otroPago.SubsidioAlEmpleo = (creaSubsidioAlEmpleo(total1));

            }

            /*  otroPago.Clave=(cfdiConceptoPercepcion.claveConcepto);
              otroPago.Concepto=(cfdiConceptoPercepcion.descripcionConcepto);
              otroPago.TipoOtroPago=(cfdiConceptoPercepcion.claveSAT);
              otroPago.Importe=(UtileriasSat.castNumerosToBigDecimal(cfdiConceptoPercepcion.importeExento));

              otroPago.SubsidioAlEmpleo=(creaSubsidioAlEmpleo(cfdiConceptoPercepcion.importeExento));  ///pendiente
              ////////        otroPago.setCompensacionSaldosAFavor(creaCompensacionSaldosAFavor());////pendiente
              */
            return otroPago;
        }


        private List<NominaOtroPago> construyeSinSubsidio()
        {
            //---
            List<NominaOtroPago> listOtrosPagos = new List<NominaOtroPago>();
            NominaOtroPago otroPago = new NominaOtroPago();
            otroPago.Clave = conceptoNominaSubsidio.clave;//(listConceptosOtros[0].claveConcepto);
            otroPago.Concepto = conceptoNominaSubsidio.descripcion;//(listConceptosOtros[0].descripcionConcepto);
            otroPago.TipoOtroPago = "002";//(listConceptosOtros[0].claveSAT);
            otroPago.Importe = (UtileriasSat.castNumerosToBigDecimal(0.00));

            otroPago.SubsidioAlEmpleo = (creaSubsidioAlEmpleo(0));
            listOtrosPagos.Add(otroPago);

            return listOtrosPagos;
        }


        private NominaOtroPagoSubsidioAlEmpleo creaSubsidioAlEmpleo(Double subsidioCausado)
        {
            NominaOtroPagoSubsidioAlEmpleo subsidioAlEmpleo = new NominaOtroPagoSubsidioAlEmpleo();
            subsidioAlEmpleo.SubsidioCausado=(UtileriasSat.castNumerosToBigDecimal(subsidioCausado));
            
            return subsidioAlEmpleo;
        }

        private NominaDeducciones contruyeNominaListaDeducciones(List<CFDIReciboConcepto> listConceptos)
        {
            NominaDeducciones deducciones = null;
            listConceptos = listConceptos == null ? new List<CFDIReciboConcepto>() : listConceptos;
            if (listConceptos.Count() > 0)
            {
                deducciones = new NominaDeducciones();
                List<NominaDeduccionesDeduccion> listDeducciones = new List<NominaDeduccionesDeduccion>();
                int i;
                Double totalImpuestosRetenidos = 0.0, totalOtrasDeducciones = 0.0;
                for (i = 0; i < listConceptos.Count(); i++)
                {
                    if (string.Equals(listConceptos[i].claveSAT,"002",StringComparison.OrdinalIgnoreCase))
                    {
                        totalImpuestosRetenidos = totalImpuestosRetenidos + listConceptos[i].importeExento + listConceptos[i].importeGravable;
                    }
                    else
                    {
                        totalOtrasDeducciones = totalOtrasDeducciones + listConceptos[i].importeGravable + listConceptos[i].importeExento;
                    }
                    listDeducciones.Add(contruyeNominaDeduccion(listConceptos[i]));
                }
                totalDeducciones = totalImpuestosRetenidos + totalOtrasDeducciones;
                deducciones.Deduccion = listDeducciones.ToArray();
                if (totalImpuestosRetenidos > 0.0)
                {
                    deducciones.TotalImpuestosRetenidos=(UtileriasSat.castNumerosToBigDecimal(totalImpuestosRetenidos));
                }
                deducciones.TotalOtrasDeducciones=(UtileriasSat.castNumerosToBigDecimal(totalOtrasDeducciones));
            }
            return deducciones;
        }

        private NominaDeduccionesDeduccion contruyeNominaDeduccion(CFDIReciboConcepto cfdiConceptoDeduccion)
        {
            NominaDeduccionesDeduccion deduccion = new NominaDeduccionesDeduccion();
            deduccion.Clave=(cfdiConceptoDeduccion.claveConcepto);
            deduccion.Concepto=(cfdiConceptoDeduccion.descripcionConcepto);
            Double importe = cfdiConceptoDeduccion.importeExento + cfdiConceptoDeduccion.importeGravable;
            deduccion.Importe=(UtileriasSat.castNumerosToBigDecimal(importe));
            deduccion.TipoDeduccion=(cfdiConceptoDeduccion.claveSAT);
            return deduccion;
        }

        private List<NominaIncapacidad> contruyeNominaListaIncapacidades(List<CFDIReciboIncapacidad> listIncapacidades)
        {
            listIncapacidades = listIncapacidades == null ? new List<CFDIReciboIncapacidad>() : listIncapacidades;
            // NominaIncapacidad incapacidades = null;
            List<NominaIncapacidad> listaIncapacidades = new List<NominaIncapacidad>();
            if (listIncapacidades.Count() > 0)
            {
               // incapacidades = new NominaIncapacidad();
                
                int i;
                for (i = 0; i < listIncapacidades.Count(); i++)
                {
                    listaIncapacidades.Add(contruyeNominaIncapacidad(listIncapacidades[i]));
                }
                //incapacidades.incapacidad = listaIncapacidades;
            }
            return listaIncapacidades;
        }

        private NominaIncapacidad contruyeNominaIncapacidad(CFDIReciboIncapacidad cfdiReciboIncapacidad)
        {
            NominaIncapacidad incapacidad = new NominaIncapacidad();
            incapacidad.ImporteMonetario=(UtileriasSat.castNumerosToBigDecimal(cfdiReciboIncapacidad.importeMonetario));
            incapacidad.DiasIncapacidad=(cfdiReciboIncapacidad.diasIncapacidad);
            incapacidad.TipoIncapacidad=(cfdiReciboIncapacidad.tipoIncapacidad);
            return incapacidad;
        }

        private enum ConceptosJubilacion
        {

            TOTAL, PARCIAL, NINGUNA,
        }

    }
}
