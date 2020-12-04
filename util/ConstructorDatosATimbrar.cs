using Exitosw.Payroll.Core.modelo;
using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Entity.entidad.cfdi;
using Exitosw.Payroll.Entity.util;
using Exitosw.Payroll.TestCompilador.funciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Exitosw.Payroll.Core.util
{
    public class ConstructorDatosATimbrar
    {
        private StringBuilder camposNecesariosMsg;
        private Dictionary<String, ConfigConceptosSat> mapConceptosSAT;
        private Dictionary<String, MovNomConcep> mapConceptosIncapacidades;
        private Dictionary<String, MovNomConcep> mapConceptosHrsExtras;
        public Mensaje mensaje { get; set; }
        private bool nombreCompletoPeriodicidad;

        public ConstructorDatosATimbrar(Dictionary<String, ConfigConceptosSat> mapConceptosSAT, Dictionary<String, MovNomConcep> mapConceptosIncapacidades, Dictionary<String, MovNomConcep> mapConceptosHrsExtras, bool nombreCompletoPeriodicidad)
        {
            this.mapConceptosSAT = mapConceptosSAT;
            this.mapConceptosIncapacidades = mapConceptosIncapacidades;
            this.mapConceptosHrsExtras = mapConceptosHrsExtras;
            this.nombreCompletoPeriodicidad = nombreCompletoPeriodicidad;
            this.camposNecesariosMsg = new StringBuilder();
            this.mensaje = new Mensaje();
        }


        public Mensaje validarDatosRazonSocial(RazonesSociales razonSocial) 
        {
            
            mensaje.resultado = "";
            mensaje.noError = 0;
            mensaje.error = "";

            validaDatosRazonSocial(razonSocial);
            if (camposNecesariosMsg.Length > 0)
            {

                mensaje.resultado = null;
                mensaje.noError = 800;
                mensaje.error = "Faltan datos necesarios :" + (camposNecesariosMsg.ToString());

            }
            return mensaje;
        }


        public List<CFDIEmpleado> generaDatosATimbrar(List<DatosParaTimbrar> datosParaTimbrados, PeriodosNomina periodoNomina, TipoNomina nomina, TipoCorrida tipoCorrida, DateTime fechaActual, RazonesSociales razonesSocialesActual)
        {
            if (datosParaTimbrados == null)
            {
                return null;
            }
            

           // camposNecesariosMsg = new StringBuilder();


            int i;
            CFDIEmpleado cfdiEmpleado;
            CFDIRecibo cfdiRecibo;
            List<CFDIEmpleado> listCFDIEmpleado = new List<CFDIEmpleado>();

            for (i = 0; i < datosParaTimbrados.Count(); i++)
            {
                if (camposNecesariosMsg.Length > 0)
                {
                    camposNecesariosMsg.Remove(0, camposNecesariosMsg.Length);
                }
                cfdiEmpleado = costruyeCFDIEmpleado(datosParaTimbrados[i].datosPorEmpleado, periodoNomina, tipoCorrida, nomina, fechaActual, razonesSocialesActual);
                cfdiRecibo = construyeCFDIRecibo(datosParaTimbrados[i].movimientos, datosParaTimbrados[i].datosHorasExtras, datosParaTimbrados[i].datosIncapacidades, fechaActual );
                cfdiEmpleado.mensaje = (camposNecesariosMsg.ToString());
                cfdiEmpleado.cfdiRecibo = (cfdiRecibo);
                                
                if (camposNecesariosMsg.Length == 0)
                {
                    cfdiEmpleado.statusGeneraInfo = (true);
                }
                else
                {
                    cfdiEmpleado.statusGeneraInfo = (false);
                }
                listCFDIEmpleado.Add(cfdiEmpleado);
            }

            
            return listCFDIEmpleado;
        }

        private CFDIEmpleado costruyeCFDIEmpleado(DatosPorEmpleado datosPorEmpleado, PeriodosNomina periodoNomina, TipoCorrida tipoCorrida, TipoNomina nomina, DateTime fechaActual,RazonesSociales razonesSocialesActual)
        {
            CFDIEmpleado cfdiEmpleado = new CFDIEmpleado();

            PlazasPorEmpleadosMov ppem = (PlazasPorEmpleadosMov)datosPorEmpleado.plazasPorEmpleadosMov;
            Empleados empleado = ppem.plazasPorEmpleado.empleados;
            cfdiEmpleado.razonesSociales = (razonesSocialesActual);//MainPrincipal.getRazonesSocialesActual());
            //validaDatosRazonSocial(razonesSocialesActual);//MainPrincipal.getRazonesSocialesActual());
            cfdiEmpleado.tipoCorrida = (tipoCorrida);
            cfdiEmpleado.tipoNomina = (nomina);
            cfdiEmpleado.periodosNomina = (periodoNomina);
            cfdiEmpleado.plazasPorEmpleadosMov = (ppem);
            cfdiEmpleado.nombre = (empleado.nombre);
            cfdiEmpleado.apellidoMaterno = (empleado.apellidoMaterno);
            cfdiEmpleado.apellidoPaterno = (empleado.apellidoPaterno);
            camposNecesariosMsg.Append(empleado.CURP == null ? "Empleado_CURP|" : !empleado.CURP.Any() ? "Empleado_CURP|" : "");
            cfdiEmpleado.CURP = (empleado.CURP);
            camposNecesariosMsg.Append(empleado.RFC == null ? "Empleado_RFC|" : !empleado.RFC.Any() ? "Empleado_RFC|" : "");
            cfdiEmpleado.RFC = (empleado.RFC);
            cfdiEmpleado.calle = (empleado.domicilio);
            cfdiEmpleado.noExterior = (empleado.numeroExt);
            cfdiEmpleado.noInterior = (empleado.numeroInt);
            cfdiEmpleado.colonia = (empleado.colonia);

            if (empleado.cp != null)
            {
                cfdiEmpleado.codigoPostal = (empleado.cp.clave);
            }
            if (empleado.ciudades != null)
            {
                cfdiEmpleado.ciudad = (empleado.ciudades.descripcion);
            }
            if (empleado.municipios != null)
            {
                cfdiEmpleado.municipio = (empleado.municipios.descripcion);
            }
            if (empleado.estados != null)
            {
                cfdiEmpleado.estado = (empleado.estados.clave);
            }
            if (empleado.paises == null)
            {
                camposNecesariosMsg.Append("Empleado_Pais|");
            }
            else
            {
                String pais = empleado.paises.descripcion;
                camposNecesariosMsg.Append(pais == null ? "Empleado_Pais|" : !pais.Any() ? "Empleado_Pais|" : "");
                cfdiEmpleado.pais = (pais);
            }
            cfdiEmpleado.correoElectronico = (empleado.correoElectronico);
            cfdiEmpleado.noSeguroSocial = (empleado.IMSS);
           
            cfdiEmpleado.formaPago = "99"; //dato fijo segun la guia de llenado
            cfdiEmpleado.noRegistroPatronal = (ppem.plazasPorEmpleado.registroPatronal == null ? null : ppem.plazasPorEmpleado.registroPatronal.registroPatronal.Replace(" ", "-"));
            cfdiEmpleado.tipoContrato = (ppem.tipoContrato == null ? null : ppem.tipoContrato.clave);
            cfdiEmpleado.riesgoPuesto = (ppem.plazasPorEmpleado.registroPatronal == null ? null : ppem.plazasPorEmpleado.registroPatronal.riesgoPuesto);
            camposNecesariosMsg.Append(ppem.plazasPorEmpleado.registroPatronal == null ? "RegistroPatronal_RiesgoPuesto|" : ppem.plazasPorEmpleado.registroPatronal.riesgoPuesto == null ? "RegistroPatronal_RiesgoPuesto|" : !ppem.plazasPorEmpleado.registroPatronal.riesgoPuesto.Any() ? "RegistroPatronal_RiesgoPuesto|" : "");
            cfdiEmpleado.puesto = (ppem.puestos == null ? null : ppem.puestos.descripcion);
            cfdiEmpleado.departamento = (ppem.departamentos == null ? null : ppem.departamentos.descripcion);

            cfdiEmpleado.fechaInicioRelLaboral = (ppem.fechaInicial); /// fecha ingreso esta en ingresos y reingresos

            if (tipoCorrida == null ? false : string.Equals(tipoCorrida.clave, "ASI", StringComparison.OrdinalIgnoreCase))
            {//
              //  camposNecesariosMsg.Append(ppem.clabe == null ? "PlazasPorEmpleadosMov_CLABE|" : !ppem.clabe.Trim().Any() ? "PlazasPorEmpleadosMov_CLABE" : "");
            }
            //cfdiEmpleado.CLABE = (ppem.clabe);
            //camposNecesariosMsg.Append(ppem.bancos == null ? "PlazasPorEmpleadosMov_Bancos|" : "");
            //cfdiEmpleado.claveBancoSat = (ppem.bancos == null ? null : ppem.bancos.clave);

            //if (ppem.cuentaBancaria != null)
            //{
            //    cfdiEmpleado.cuentaBancaria = (ppem.cuentaBancaria.Replace("-", ""));
            //}
            camposNecesariosMsg.Append(ppem.regimenContratacion == null ? "PlazasPorEmpleadosMov_RegimenContratacion|" : "");
            cfdiEmpleado.regimenContratacion = (ppem.regimenContratacion);
            cfdiEmpleado.jornada = (ppem.turnos == null ? null : ppem.turnos.Jornada == null ? null : ppem.turnos.Jornada.clave);

            camposNecesariosMsg.Append(periodoNomina.fechaPago == null ? "PeriodosNomina_FechaPago|" : "");
            cfdiEmpleado.fechaPago = (periodoNomina.fechaPago.GetValueOrDefault());
            camposNecesariosMsg.Append(periodoNomina.fechaFinal == null ? "PeriodosNomina_FechaFinal|" : "");
            cfdiEmpleado.fechaFinalPago = (periodoNomina.fechaFinal.GetValueOrDefault());
            camposNecesariosMsg.Append(periodoNomina.fechaInicial == null ? "PeriodosNomina_FechaInicial|" : "");
            cfdiEmpleado.fechaInicioPago = (periodoNomina.fechaInicial.GetValueOrDefault());
            camposNecesariosMsg.Append(periodoNomina.diasPago == null ? "PeriodosNomina_DiasPago|" : "");
            cfdiEmpleado.numeroDiasPago = (periodoNomina.diasPago.GetValueOrDefault());

            cfdiEmpleado.salIntIMSS = (datosPorEmpleado.salarioDiarioIntegrado);
            cfdiEmpleado.salBaseCotAport = (ppem.sueldoDiario);
            if (datosPorEmpleado.fechaIngreso == null || fechaActual == null)
            {
                System.Diagnostics.Debug.WriteLine("Las fechas del empleado estan vacías");
            }                                                                      ////datosPorEmpleado.getFechaIngreso()   
            cfdiEmpleado.antiguedad = (Utilerias.cantidadSemanasEntreDosFechasStatic(ppem.fechaInicial.GetValueOrDefault(), periodoNomina.fechaFinal.GetValueOrDefault())); //calculado
            cfdiEmpleado.antiguedadYMD = (UtileriasSat.getAntiguedadYMD(ppem.fechaInicial.GetValueOrDefault(), periodoNomina.fechaFinal.GetValueOrDefault())); //calculado
                                                                                                                                                               
            cfdiEmpleado.periodiciadadPago = (nomina.periodicidad.clave); 

            return cfdiEmpleado;
        }

        private void validaDatosRazonSocial(RazonesSociales razonSocial)
        {
            camposNecesariosMsg.Append(razonSocial.regimenFiscal == null ? "RazonesSociales_RegimenFiscal|" : !razonSocial.regimenFiscal.Any() ? "RazonesSociales_RegimenFiscal|" : "");
            camposNecesariosMsg.Append(razonSocial.certificadoSAT == null ? "RazonesSociales_CertificadoSAT|" : "");
            camposNecesariosMsg.Append(razonSocial.llaveSAT == null ? "RazonesSociales_LlaveSAT|" : "");
            camposNecesariosMsg.Append(razonSocial.password == null ? "RazonesSociales_Password|" : !razonSocial.password.Any() ? "RazonesSociales_Password|" : "");
            camposNecesariosMsg.Append(razonSocial.rfc == null ? "RazonesSociales_Rfc|" : !razonSocial.rfc.Any() ? "RazonesSociales_Rfc|" : "");
            camposNecesariosMsg.Append(razonSocial.calle == null ? "RazonesSociales_Calle|" : !razonSocial.calle.Any() ? "RazonesSociales_Calle|" : "");
            camposNecesariosMsg.Append(razonSocial.folio == null ? "RazonesSociales_|" : !razonSocial.folio.Any() ? "RazonesSociales_Folio|" : "");
            String valor;
            if (razonSocial.cp == null)
            {
                camposNecesariosMsg.Append("RazonesSociales_Cp|");
            }
            else
            {
                camposNecesariosMsg.Append(razonSocial.cp.descripcion == null ? "RazonesSociales_Cp|" : !razonSocial.cp.descripcion.Any() ? "RazonesSociales_Cp|" : "");
            }
            if (razonSocial.municipios == null)
            {
                camposNecesariosMsg.Append("RazonesSociales_Municipios|");
            }
            else
            {
                valor = razonSocial.municipios.descripcion;
                camposNecesariosMsg.Append(valor == null ? "RazonesSociales_Municipios|" : !valor.Any() ? "RazonesSociales_Municipios|" : "");
            }
            if (razonSocial.estados == null)
            {
                camposNecesariosMsg.Append("RazonesSociales_Estados|");
            }
            else
            {
                valor = razonSocial.estados.descripcion;
                camposNecesariosMsg.Append(valor == null ? "RazonesSociales_Estados|" : !valor.Any() ? "RazonesSociales_Estados|" : "");
            }
            if (razonSocial.paises == null)
            {
                camposNecesariosMsg.Append("RazonesSociales_Pais|");
            }
            else
            {
                valor = razonSocial.paises.descripcion;
                camposNecesariosMsg.Append(valor == null ? "RazonesSociales_Pais|" : !valor.Any() ? "RazonesSociales_Pais|" : "");
            }
        }

        private CFDIRecibo construyeCFDIRecibo(List<MovNomConcep> listMovNom, List<DatosHorasExtras> listHrsExtras, List<DatosIncapacidades> listIncapacidades, DateTime fechaActual)
        {
            CFDIRecibo cfdiRecibo = new CFDIRecibo();
            ////////        cfdiRecibo.setCadenaCertificado(nombreTablaBDs);
            ////////        cfdiRecibo.setCadenaOriginalTimbrado(nombreTablaBDs);
            ////////        cfdiRecibo.setCertificadoTimbrado(nombreTablaBDs);
            cfdiRecibo.fechaGeneraInfo=(fechaActual);
            ////////        cfdiRecibo.setFechaHoraTimCancelado(null);
            ////////        cfdiRecibo.setFechaHoraTimbrado(null);
            ////////        cfdiRecibo.setFolioCFDI(msgError);
            ////////        cfdiRecibo.setMensaje(msgError);
            ////////        cfdiRecibo.setMotivoCancelacion(nombreTablaBDs);
            ////////        cfdiRecibo.setNoCertificado(nombreTablaBDs);
            ////////        cfdiRecibo.setSello(msgError);
            ////////        cfdiRecibo.setSelloTimbrado(nombreTablaBDs);
            ////////        cfdiRecibo.setSerieCFDI(msgError);
            cfdiRecibo.statusTimbrado=(StatusTimbrado.EN_PROCESO);
            //////        cfdiRecibo.setUUID(msgError);
            //////        cfdiRecibo.setVersion(msgError);
            //////        cfdiRecibo.setXmlTimbrado(xmlTimbrado);
            //***genera datos conceptos***/ 
            listMovNom = listMovNom == null ? new List<MovNomConcep>() : listMovNom;
            double totalPercepcion = 0.0, totalDeduccion = 0.0, totalOtrosPagos = 0.0, totalSeparacionIndeminizacion = 0.0;
            if (listMovNom.Count() > 0)
            {
                int i, j;
                List<MovNomBaseAfecta> baseAfectas;
                List<CFDIReciboConcepto> listReciboConceptos = new List<CFDIReciboConcepto>();

                CFDIReciboConcepto reciboConcepto;

                ConcepNomDefi cnd;
                Naturaleza naturaleza;
                ConfigConceptosSat claveConSat;
                for (i = 0; i < listMovNom.Count(); i++)
                {
                    cnd = listMovNom[i].concepNomDefi;
                    naturaleza = cnd.naturaleza;
                    if (listMovNom[i].resultado == null ? true : listMovNom[i].resultado == 0.0)
                    {
                        camposNecesariosMsg.Append(cnd.clave).Append(".- ").Append(cnd.descripcion).Append(" No ha sido calculado|");
                    }
                    else
                    {
                        if (
                            (listMovNom[i].concepNomDefi.naturaleza == Naturaleza.PERCEPCION | listMovNom[i].concepNomDefi.naturaleza == Naturaleza.DEDUCCION | 
                            (
                            (listMovNom[i].concepNomDefi.naturaleza == Naturaleza.CALCULO) && 
                            ((listMovNom[i].concepNomDefi.formulaConcepto == "SubsEmpleoCalculado") | (listMovNom[i].concepNomDefi.formulaConcepto == "ISRSubsidio"))
                            )
                            )
                            & mapConceptosSAT.ContainsKey(string.Concat(cnd.clave,ManejadorEnum.GetDescription(naturaleza)))
                            )
                        {
                            if (mapConceptosIncapacidades.ContainsKey(cnd.clave))
                            {
                                claveConSat = mapConceptosSAT[string.Concat(cnd.clave, ManejadorEnum.GetDescription(naturaleza))];//.get(cnd.getClave().concat(naturaleza.name()));
                                if (string.Equals(claveConSat.conceptoSatClave, ClavesParametrosModulos.claveConceptoIncapacidadesSAT.ToString(),StringComparison.OrdinalIgnoreCase)    )
                                {
                                    mapConceptosIncapacidades[cnd.clave] = listMovNom[i];//.put(cnd.getClave(), listMovNom.get(i));
                                }
                            }
                            else if (mapConceptosHrsExtras.ContainsKey(cnd.clave))
                            {
                                //claveConSat = mapConceptosSAT.get(cnd.getClave().concat(naturaleza.name()));
                                claveConSat = mapConceptosSAT[string.Concat(cnd.clave, ManejadorEnum.GetDescription(naturaleza))];
                                if (string.Equals(claveConSat.conceptoSatClave,ClavesParametrosModulos.claveConceptoHrsExtrasSAT.ToString(),StringComparison.OrdinalIgnoreCase))
                                {
                                   // mapConceptosHrsExtras.put(cnd.getClave(), listMovNom.get(i));
                                    mapConceptosHrsExtras[cnd.clave] = listMovNom[i];
                                }
                            }
                            reciboConcepto = new CFDIReciboConcepto();
                            reciboConcepto.id=0;
                            reciboConcepto.claveConcepto=(cnd.clave);
                           // claveConSat = mapConceptosSAT.get(cnd.getClave().concat(naturaleza.name()));
                            claveConSat= mapConceptosSAT[string.Concat(cnd.clave, ManejadorEnum.GetDescription(naturaleza))];
                            reciboConcepto.claveSAT=(claveConSat.conceptoSatClave);
                            reciboConcepto.descripcionConcepto=(cnd.descripcion);
                            reciboConcepto.tipoNaturaleza=(ManejadorEnum.GetDescription(naturaleza));
                            reciboConcepto.otroPago=(claveConSat.otroPago);
                            if (listMovNom[i].movNomBaseAfecta == null)
                            {
                                reciboConcepto.importeExento=(0.0);
                                reciboConcepto.importeGravable=(0.0);
                            }
                            else
                            {
                                baseAfectas = listMovNom[i].movNomBaseAfecta;
                                if (!baseAfectas.Any())
                                {
                                    if (claveConSat.otroPago & naturaleza == Naturaleza.PERCEPCION)
                                    {
                                        totalOtrosPagos = totalOtrosPagos + listMovNom[i].resultado.GetValueOrDefault();
                                    }
                                    else if (naturaleza == Naturaleza.PERCEPCION)
                                    {
                                         
                                            totalPercepcion = totalPercepcion + listMovNom[i].resultado.GetValueOrDefault(); 
                                        
                                        
                                    }
                                    else if (naturaleza == Naturaleza.DEDUCCION)
                                    {
                                        totalDeduccion = totalDeduccion + listMovNom[i].resultado.GetValueOrDefault();
                                    }
                                   
                                        reciboConcepto.importeExento = (listMovNom[i].resultado == null ? 0.0 : listMovNom[i].resultado.GetValueOrDefault());
                                        reciboConcepto.importeGravable = (0.0);
                                   
                                        
                                }
                                else
                                {
                                    for (j = 0; j < baseAfectas.Count(); j++)
                                    {
                                        //string.Equals(baseAfectas[j].baseAfecConcepNom.baseNomina.clave, (String)ClavesParametrosModulos.claveBaseNominaISR, StringComparison.OrdinalIgnoreCase)
                                        if (string.Equals(baseAfectas[j].baseAfecConcepNom.baseNomina.clave, (String)ClavesParametrosModulos.claveBaseNominaISR, StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (cnd.formulaConcepto.ToUpper().Contains("ISRSubsidio".ToUpper()))
                                            {
                                                reciboConcepto.importeExento=(listMovNom[i].resultado == null ? 0.0 : listMovNom[i].resultado.GetValueOrDefault());
                                                reciboConcepto.importeGravable=(0.0);
                                            }
                                            else if (cnd.formulaConcepto.ToUpper().Contains("CalculoISR".ToUpper())
                                                  | cnd.formulaConcepto.ToUpper().Contains("CalculoIMSS".ToUpper())
                                                  | cnd.formulaConcepto.ToUpper().Contains("CalculoIMSSPatronal".ToUpper()))
                                            {
                                                reciboConcepto.importeExento=(0.0);
                                                reciboConcepto.importeGravable=(listMovNom[i].resultado == null ? 0.0 : listMovNom[i].resultado.GetValueOrDefault());
                                            }
                                            else
                                            {
                                                reciboConcepto.importeExento=(baseAfectas[j].resultadoExento == null ? 0.0 : baseAfectas[j].resultadoExento.GetValueOrDefault());
                                                reciboConcepto.importeGravable=(baseAfectas[j].resultado == null ? 0.0 : baseAfectas[j].resultado.GetValueOrDefault());
                                            }
                                            if (claveConSat.otroPago & naturaleza == Naturaleza.PERCEPCION)
                                            {
                                                totalOtrosPagos = totalOtrosPagos + reciboConcepto.importeExento + reciboConcepto.importeGravable;
                                            }
                                            else if (naturaleza == Naturaleza.PERCEPCION)
                                            {
                                                totalPercepcion = totalPercepcion + reciboConcepto.importeExento + reciboConcepto.importeGravable;
                                            }
                                            else if (naturaleza == Naturaleza.DEDUCCION)
                                            {
                                                totalDeduccion = totalDeduccion + reciboConcepto.importeExento + reciboConcepto.importeGravable;
                                            }
                                            break;
                                        }
                                        else
                                        {
                                            if (claveConSat.otroPago & naturaleza == Naturaleza.PERCEPCION)
                                            {
                                                totalOtrosPagos = totalOtrosPagos + listMovNom[i].resultado.GetValueOrDefault();
                                            }
                                            else if (naturaleza == Naturaleza.PERCEPCION)
                                            {
                                                totalPercepcion = totalPercepcion + listMovNom[i].resultado.GetValueOrDefault();
                                            }
                                            else if (naturaleza == Naturaleza.DEDUCCION)
                                            {
                                                totalDeduccion = totalDeduccion + listMovNom[i].resultado.GetValueOrDefault();
                                            }
                                            
                                            
                                                reciboConcepto.importeExento = (listMovNom[i].resultado.GetValueOrDefault());
                                                reciboConcepto.importeGravable = (0.0);
                                            
                                        }
                                    }
                                }
                            }

                            if (naturaleza == Naturaleza.PERCEPCION)
                            {
                                if (string.Equals(claveConSat.conceptoSatClave, "022", StringComparison.OrdinalIgnoreCase) | string.Equals(claveConSat.conceptoSatClave, "023", StringComparison.OrdinalIgnoreCase)
                            | string.Equals(claveConSat.conceptoSatClave, "025", StringComparison.OrdinalIgnoreCase))
                                {
                                    totalSeparacionIndeminizacion = totalSeparacionIndeminizacion + reciboConcepto.importeExento + reciboConcepto.importeGravable;
                                        }

                            }

                            listReciboConceptos.Add(reciboConcepto);
                        }
                        else
                        {
                            camposNecesariosMsg.Append(cnd.clave).Append(".- ").Append(cnd.descripcion).Append(" ").Append("");//Utilerias.ObtenerMensaje.getString("ConcepNomDefiMsgClaveSat")).append("|");Pendiente
                        }
                    }
                }
                listHrsExtras = listHrsExtras == null ? new List<DatosHorasExtras>() : listHrsExtras;
                if (listHrsExtras.Count() > 0)
                {
                    List<CFDIReciboHrsExtras> cfdiReciboHrsExtras = new List<CFDIReciboHrsExtras>();
                    CFDIReciboHrsExtras cfdiReciboHrsExtra = null;
                    for (i = 0; i < listHrsExtras.Count(); i++)
                    {
                        cfdiReciboHrsExtra = construyeCFDIReciboHrsExtras(cfdiReciboHrsExtra, listHrsExtras[i]);
                        if (cfdiReciboHrsExtra != null)
                        {
                            cfdiReciboHrsExtras.Add(cfdiReciboHrsExtra);
                        }
                    }
                    cfdiRecibo.cfdiReciboHrsExtras=(!cfdiReciboHrsExtras.Any() ? null : cfdiReciboHrsExtras);
                }
                listIncapacidades = listIncapacidades == null ? new List<DatosIncapacidades>() : listIncapacidades;
                if (listIncapacidades.Count() > 0)
                {
                    List<CFDIReciboIncapacidad> cfdiReciboIncapacidades = new List<CFDIReciboIncapacidad>();
                    CFDIReciboIncapacidad cfdiIncapacidad = null;
                    for (i = 0; i < listIncapacidades.Count(); i++)
                    {
                        cfdiIncapacidad = construyeCFDIReciboIncapacidad(cfdiIncapacidad, listIncapacidades[i]);
                        if (cfdiIncapacidad != null)
                        {
                            cfdiReciboIncapacidades.Add(cfdiIncapacidad);
                        }
                    }
                    cfdiRecibo.cfdiReciboIncapacidad=(!cfdiReciboIncapacidades.Any() ? null : cfdiReciboIncapacidades);
                }
                cfdiRecibo.cfdiReciboConcepto=(listReciboConceptos);
            }
            cfdiRecibo.totalPercepcion=(totalPercepcion);
            cfdiRecibo.totalOtroPagos=(totalOtrosPagos);
            cfdiRecibo.totalDeduccion=(totalDeduccion);
            cfdiRecibo.total = (totalPercepcion + totalOtrosPagos+ totalSeparacionIndeminizacion) - totalDeduccion;

            cfdiRecibo.mensajeRec = (camposNecesariosMsg.ToString()); ;
            return cfdiRecibo;
        }

        private CFDIReciboHrsExtras construyeCFDIReciboHrsExtras(CFDIReciboHrsExtras cfdiReciboHrsExtra, DatosHorasExtras horasExtra)
        {
            if (cfdiReciboHrsExtra == null)
            {
                cfdiReciboHrsExtra = new CFDIReciboHrsExtras();
            }
            if (mapConceptosHrsExtras.ContainsKey(horasExtra.asistencia.excepciones.concepNomDefi.clave))
            {
                MovNomConcep mnc = mapConceptosHrsExtras[horasExtra.asistencia.excepciones.concepNomDefi.clave];//.get(horasExtra.getAsistencia().getExcepciones().getConcepNomDefi().getClave());
                if (mnc != null)
                {
                    cfdiReciboHrsExtra.dias=(horasExtra.dias);
                    cfdiReciboHrsExtra.horasExtras=(horasExtra.hrsExtas);
                    if (string.Equals(horasExtra.asistencia.excepciones.clave, ClavesParametrosModulos.claveExcepcionExtraDoble.ToString(),StringComparison.OrdinalIgnoreCase))
                    {
                        cfdiReciboHrsExtra.tipoHoras=("01");
                    }
                    else if (string.Equals(horasExtra.asistencia.excepciones.clave, ClavesParametrosModulos.claveExcepcionExtraTriple.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        cfdiReciboHrsExtra.tipoHoras=("02");
                    }
                    else
                    {
                        cfdiReciboHrsExtra.tipoHoras=("03");
                    }
                    if (mnc.movNomBaseAfecta == null)
                    {
                        cfdiReciboHrsExtra.importeExento=(0.0);
                        cfdiReciboHrsExtra.importeGravable=(0.0);
                    }
                    else
                    {
                        List<MovNomBaseAfecta> baseAfectas = mnc.movNomBaseAfecta;
                        if (!baseAfectas.Any())
                        {
                            cfdiReciboHrsExtra.importeExento=(0.0);
                            cfdiReciboHrsExtra.importeGravable=(0.0);
                        }
                        else
                        {
                            int j;
                            for (j = 0; j < baseAfectas.Count(); j++)
                            {
                                if (string.Equals(baseAfectas[j].baseAfecConcepNom.baseNomina.clave,(String)ClavesParametrosModulos.claveBaseNominaISR,StringComparison.OrdinalIgnoreCase))
                                {
                                    cfdiReciboHrsExtra.importeExento=(baseAfectas[j].resultadoExento.GetValueOrDefault());
                                    cfdiReciboHrsExtra.importeGravable=(baseAfectas[j].resultado.GetValueOrDefault());
                                    break;
                                }
                                else
                                {
                                    cfdiReciboHrsExtra.importeExento=(0.0);
                                    cfdiReciboHrsExtra.importeGravable=(0.0);
                                }
                            }
                        }
                    }
                }
                else
                {
                    cfdiReciboHrsExtra = null;
                }
            }
            else
            {
                cfdiReciboHrsExtra = null;
            }
            return cfdiReciboHrsExtra;
        }

        private CFDIReciboIncapacidad construyeCFDIReciboIncapacidad(CFDIReciboIncapacidad cfdiReciboIncapacidad, DatosIncapacidades datosIncapacidad)
        {
            if (cfdiReciboIncapacidad == null)
            {
                cfdiReciboIncapacidad = new CFDIReciboIncapacidad();
            }
            if (mapConceptosIncapacidades.ContainsKey(datosIncapacidad.asistencia.excepciones.concepNomDefi.clave))
            {
                MovNomConcep mnc = mapConceptosIncapacidades[datosIncapacidad.asistencia.excepciones.concepNomDefi.clave];//.get(datosIncapacidad.getAsistencia().getExcepciones().getConcepNomDefi().getClave());
                if (mnc != null)
                {
                    cfdiReciboIncapacidad.importeMonetario=(mnc.resultado.GetValueOrDefault());
                    cfdiReciboIncapacidad.diasIncapacidad=(datosIncapacidad.dias);
                    if (String.Equals(datosIncapacidad.asistencia.excepciones.clave,ClavesParametrosModulos.claveExcepcionIncapacidadPorAccidente.ToString(),StringComparison.OrdinalIgnoreCase))
                    {
                        cfdiReciboIncapacidad.tipoIncapacidad=("01");
                    }
                    else if (string.Equals(datosIncapacidad.asistencia.excepciones.clave,ClavesParametrosModulos.claveExcepcionIncapacidadPorEnfermedad.ToString(),StringComparison.OrdinalIgnoreCase))
                    {
                        cfdiReciboIncapacidad.tipoIncapacidad=("02");
                    }
                    else if (string.Equals(datosIncapacidad.asistencia.excepciones.clave,ClavesParametrosModulos.claveExcepcionIncapacidadPorMaternidad.ToString(),StringComparison.OrdinalIgnoreCase))
                    {
                        cfdiReciboIncapacidad.tipoIncapacidad=("03");
                    }
                }
                else
                {
                    cfdiReciboIncapacidad = null;
                }
            }
            else
            {
                cfdiReciboIncapacidad = null;
            }
            return cfdiReciboIncapacidad;
        }

    }
}
