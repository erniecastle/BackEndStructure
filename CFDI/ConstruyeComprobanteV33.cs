
using Exitosw.Payroll.Core.CFDI.Sat.Catalogos;
using Exitosw.Payroll.Core.CFDI.Timbrado;
using Exitosw.Payroll.Core.modelo;
using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Entity.entidad.cfdi;
using Exitosw.Payroll.Entity.util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Exitosw.Payroll.Core.CFDI
{
    public class ConstruyeComprobanteV33
    {
        private Dictionary<String, String> mapClaveConceptosISR;
        private CFDIEmpleadoDAO servicioCFDIEmpleado;
        private CertificadosDAO servicioCertificado;
        string ruta;
        DateTime fechaGeneraXML;
        public Mensaje mensaje { get; set; }
        public ConstruyeComprobanteV33(Dictionary<String, String> mapClaveConceptosISRs, string ruta)
        {
            this.mapClaveConceptosISR = mapClaveConceptosISRs;
            this.ruta = ruta;
            mensaje = new Mensaje();
            mensaje.error = ("");
            mensaje.noError = (0);
        }

        public void setServicioCFDIEmpleado(CFDIEmpleadoDAO servicioCFDIEmpleado)
        {
            this.servicioCFDIEmpleado = servicioCFDIEmpleado;
        }
        public void setservicioCertificado(CertificadosDAO servicioCertificado)
        {
            this.servicioCertificado = servicioCertificado;
        }

        public List<CFDIEmpleado> generaComprobanteV33(TipoNomina tipoNomina, PeriodosNomina periodosNomina, List<CFDIEmpleado> listCFDIEmpleados, ConfiguraTimbrado configuraTimbrado, System.Xml.Xsl.XslCompiledTransform transformerCadenaOriginal,  DBContextAdapter dbContextSimple)
        {

            Comprobante oComprobante = new Comprobante();
            //aqui va que agarre el certificado de donde se tiene configurado
            ConcepNomDefi conceptoNominaSubsidio = servicioCFDIEmpleado.getConceptoNominaSubsidio(dbContextSimple);

           /* string pathCer = ruta;
            string pathKey = ruta;*/
            string pathXML = ruta;
           // string clavePrivada = null;
            SelloDigital oSelloDigital = new SelloDigital();
            Certificados  certificadoActual = new Certificados();
           

            listCFDIEmpleados = listCFDIEmpleados == null ? new List<CFDIEmpleado>() : listCFDIEmpleados;
            List<object> timbreFiscalDigitales;
            try
            {
                List<DatosEmpleadoComprobate> datosEmpleadoComprobates = new List<DatosEmpleadoComprobate>();
                if (listCFDIEmpleados.Count > 0)
                {
                    RazonesSociales razonSocial = listCFDIEmpleados[0].razonesSociales;
                    int i, j;
                    String nombreFile;
                    certificadoActual = servicioCertificado.certificadoActualId(razonSocial.id, dbContextSimple);
                   
                    
                   StringBuilder pathXmlExistentes = new StringBuilder();
                    pathXmlExistentes.Append(construyeRutaXML(razonSocial, tipoNomina, periodosNomina)).Append(System.IO.Path.DirectorySeparatorChar);
                    /*******************Busca documentos ya existentes********************/
                    Comprobante comprobanteExistentes;
                    ConstruyeTimbreFiscalDigital11 timbreDigitalCadenaOrig = null;
                    List<CFDIEmpleado> listCFDIEmpleadosTimbrados = new List<CFDIEmpleado>();
                    String ruta2 = pathXmlExistentes.ToString(), nombreArchivo;
                    for (i = 0; i < listCFDIEmpleados.Count(); i++)
                    {
                        
                        nombreArchivo = nomenclaturaNombreArchivo(tipoNomina, periodosNomina, listCFDIEmpleados[i]);
                        
                        if (listCFDIEmpleados[i].cfdiRecibo.statusTimbrado == StatusTimbrado.TIMBRADO)
                        {

                            XmlDocument doc = new XmlDocument();
                            MemoryStream ms = new MemoryStream(listCFDIEmpleados[i].cfdiRecibo.xmlTimbrado);
                            doc.Load(ms);
                            XmlSerializer oXmlSerializar = new XmlSerializer(typeof(Comprobante));
                            using (XmlReader writer = new XmlNodeReader(doc))
                            {
                                comprobanteExistentes = (Comprobante)oXmlSerializar.Deserialize(writer);
                            }

                            CreateXML(comprobanteExistentes, string.Concat(ruta2, nombreArchivo));
                            listCFDIEmpleadosTimbrados.Add(listCFDIEmpleados[i]);
                        }
                    }

                    if (listCFDIEmpleadosTimbrados.Count() > 0)
                    {
                        for (i = 0; i < listCFDIEmpleadosTimbrados.Count; i++)
                        {
                            listCFDIEmpleados.Remove(listCFDIEmpleadosTimbrados[i]);
                        }

                    }

                    Comprobante comprobante = null;
                    ConstruyeNomina12 creaXmlNomina = new ConstruyeNomina12();
                    Nomina nomina12;

                    if (listCFDIEmpleados.Count > 0)
                    {
                        for (i = 0; i < listCFDIEmpleados.Count; i++)
                        {
                            if (listCFDIEmpleados[i].cfdiRecibo.statusXmlSat == StatusXmlSat.NINGUNO)
                            {
                                listCFDIEmpleados[i].certificadoAsignado = certificadoActual;
                                comprobante = construyeComprobante(listCFDIEmpleados[i]);
                                listCFDIEmpleados[i].cfdiRecibo.fechaEmision = fechaGeneraXML;
                                nomina12 = creaXmlNomina.generaComplementoNomina(listCFDIEmpleados[i], conceptoNominaSubsidio);
                                if (creaXmlNomina.mensajeNomina.noError > 0)
                                {
                                    DatosEmpleadoComprobate errorNominaDatos = new DatosEmpleadoComprobate(listCFDIEmpleados[i], comprobante);
                                    datosEmpleadoComprobates.Add(new DatosEmpleadoComprobate(listCFDIEmpleados[i], comprobante));
                                    errorNominaDatos.cfdiEmpleado.mensaje = creaXmlNomina.mensajeNomina.error;
                                }
                                else
                                {
                                    comprobante = agregarComplementoNomina(comprobante, nomina12);
                                    datosEmpleadoComprobates.Add(new DatosEmpleadoComprobate(listCFDIEmpleados[i], comprobante));
                                }
                            }
                            else if (listCFDIEmpleados[i].cfdiRecibo.statusXmlSat == StatusXmlSat.ENVIADO_SAT)
                            {
                                try
                                {
                                    XmlDocument doc = new XmlDocument();
                                    MemoryStream ms = new MemoryStream(listCFDIEmpleados[i].cfdiRecibo.xmlTimbrado);
                                    doc.Load(ms);
                                    XmlSerializer oXmlSerializar = new XmlSerializer(typeof(Comprobante));
                                    using (XmlReader writer = new XmlNodeReader(doc))
                                    {
                                        comprobante = (Comprobante)oXmlSerializar.Deserialize(writer);
                                    }
                                }
                                catch (Exception)
                                {
                                    listCFDIEmpleados[i].certificadoAsignado = certificadoActual;
                                    listCFDIEmpleados[i].cfdiRecibo.statusXmlSat = StatusXmlSat.NINGUNO;
                                    comprobante = construyeComprobante(listCFDIEmpleados[i]);
                                    listCFDIEmpleados[i].cfdiRecibo.fechaEmision = fechaGeneraXML;
                                    nomina12 = creaXmlNomina.generaComplementoNomina(listCFDIEmpleados[i], conceptoNominaSubsidio);
                                    comprobante = agregarComplementoNomina(comprobante, nomina12);

                                }
                                datosEmpleadoComprobates.Add(new DatosEmpleadoComprobate(listCFDIEmpleados[i], comprobante));
                            }

                        }
                        CFDIRecibo recibo;
                        for (i = 0; i < datosEmpleadoComprobates.Count; i++)
                        {
                            if (datosEmpleadoComprobates[i].cfdiEmpleado.cfdiRecibo.statusXmlSat == StatusXmlSat.NINGUNO)
                            {
                                nombreFile = nomenclaturaNombreArchivo(tipoNomina, periodosNomina, datosEmpleadoComprobates[i].cfdiEmpleado);
                                CreateXML(datosEmpleadoComprobates[i].comprobante, string.Concat(ruta2, nombreFile));
                                string cadenaOriginal = getCadenaoriginal(string.Concat(ruta2, nombreFile));


                                //byte[] ClavePrivada1 = razonSocial.llaveSAT;
                                byte[] ClavePrivada1 = certificadoActual.llavePrivada;
                                datosEmpleadoComprobates[i].comprobante.Sello = oSelloDigital.Sellar(cadenaOriginal, ClavePrivada1, certificadoActual.password);
                                CreateXML(datosEmpleadoComprobates[i].comprobante, string.Concat(ruta2, nombreFile));
                                recibo = datosEmpleadoComprobates[i].cfdiEmpleado.cfdiRecibo;
                                recibo.total = Convert.ToDouble(datosEmpleadoComprobates[i].comprobante.Total);
                                recibo.folioCFDI = datosEmpleadoComprobates[i].comprobante.Folio;
                                recibo.noCertificado = datosEmpleadoComprobates[i].comprobante.NoCertificado;
                                recibo.sello = (datosEmpleadoComprobates[i].comprobante.Sello);
                                recibo.serieCFDI = (datosEmpleadoComprobates[i].comprobante.Serie);
                                recibo.cadenaCertificado = (datosEmpleadoComprobates[i].comprobante.Certificado);
                                recibo.version = (datosEmpleadoComprobates[i].comprobante.Version);
                                recibo.serie = (datosEmpleadoComprobates[i].comprobante.Serie);

                                datosEmpleadoComprobates[i].cfdiEmpleado.cfdiRecibo = (recibo);

                                datosEmpleadoComprobates[i].archivoByte = System.IO.File.ReadAllBytes(string.Concat(ruta2, nombreFile));
                                datosEmpleadoComprobates[i].nombreArchivo = nombreFile;
                            }
                            else if (datosEmpleadoComprobates[i].cfdiEmpleado.cfdiRecibo.statusXmlSat == StatusXmlSat.ENVIADO_SAT)
                            {
                                nombreFile = nomenclaturaNombreArchivo(tipoNomina, periodosNomina, datosEmpleadoComprobates[i].cfdiEmpleado);
                                datosEmpleadoComprobates[i].archivoByte = datosEmpleadoComprobates[i].cfdiEmpleado.cfdiRecibo.xmlTimbrado;
                                datosEmpleadoComprobates[i].nombreArchivo = nombreFile;
                            }
                        }

                        List<InfoExtra> infoExtras = new List<InfoExtra>();
                        InfoExtra infoExtra;

                        for (i = 0; i < datosEmpleadoComprobates.Count; i++)
                        {
                            datosEmpleadoComprobates[i].statusTimbrado = (StatusTimbrado.EN_PROCESO);
                            infoExtra = new InfoExtra();

                            infoExtra.archivoXML = (datosEmpleadoComprobates[i].archivoByte);
                            infoExtra.nombreArchivo = (datosEmpleadoComprobates[i].nombreArchivo);
                            infoExtra.rfcEmisor = (datosEmpleadoComprobates[i].comprobante.Emisor.Rfc);
                            infoExtra.rfcReceptor = (datosEmpleadoComprobates[i].comprobante.Receptor.Rfc);
                            infoExtra.version = (datosEmpleadoComprobates[i].comprobante.Version);
                            infoExtra.folio = (datosEmpleadoComprobates[i].comprobante.Folio);
                            infoExtras.Add(infoExtra);
                            if (datosEmpleadoComprobates[i].cfdiEmpleado.cfdiRecibo.statusXmlSat == StatusXmlSat.NINGUNO)
                            {
                                datosEmpleadoComprobates[i].cfdiEmpleado.cfdiRecibo.statusXmlSat = (StatusXmlSat.ENVIADO_SAT);
                                datosEmpleadoComprobates[i].cfdiEmpleado.cfdiRecibo.xmlTimbrado = (datosEmpleadoComprobates[i].archivoByte);
                                if (datosEmpleadoComprobates[i].cfdiEmpleado.id != 0)
                                {
                                    servicioCFDIEmpleado.actualizar(datosEmpleadoComprobates[i].cfdiEmpleado, dbContextSimple);
                                }
                            }
                        }

                        //timbrar
                        if (infoExtras.Count > 0)
                        {
                            //datos de timbrar desde la razon social
                            TimbrarXmlSat timbrarXmlSat = new TimbrarXmlSat();
                            InfoATimbrar infoATimbrar = new InfoATimbrar();
                            infoATimbrar.tipoOperacion = (TipoOperacionWS.TIMBRAR);
                            infoATimbrar.infoExtras = (infoExtras);
                            infoATimbrar.password = configuraTimbrado.contraseña;
                            infoATimbrar.urlWebServices = configuraTimbrado.URL; 
                            infoATimbrar.usuario = configuraTimbrado.usuario; 

                            timbreFiscalDigitales = timbrarXmlSat.generaTimbres(infoATimbrar, dbContextSimple);
                            if (timbrarXmlSat.error !=null)
                            {
                                mensaje.noError=101;
                                mensaje.resultado= (timbrarXmlSat.error);
                                if (timbreFiscalDigitales.Count > 0)
                                {
                                    

                                    for (i = 0; i < timbreFiscalDigitales.Count; i++)
                                    {
                                        j = 0;
                                        DatosTimbreFiscalDigital info = (DatosTimbreFiscalDigital)timbreFiscalDigitales[i];
                                        
                                        while (j < datosEmpleadoComprobates.Count)
                                        {
                                            if (info.error != 0) {
                                                if (string.Equals((datosEmpleadoComprobates[j].folio == null ? "" : datosEmpleadoComprobates[j].folio), (info.folio == null ? "" : info.folio), StringComparison.OrdinalIgnoreCase))
                                                {

                                                    datosEmpleadoComprobates[j].cfdiEmpleado.mensaje = info.descripcion;


                                                }
                                            }
                                            
                                            j++;
                                        }
                                    }
                                        
                                }
                            }
                            timbreFiscalDigitales = timbreFiscalDigitales == null ? new List<object>() : timbreFiscalDigitales;

                            if (timbreFiscalDigitales.Count > 0)
                            {
                                XmlNode tfd;
                                ConstruyeTimbreFiscalDigital11 digital11 = new ConstruyeTimbreFiscalDigital11(ruta);
                                
                                j = 0;
                                listCFDIEmpleados.Clear();
                                for (i = 0; i < timbreFiscalDigitales.Count; i++)
                                {
                                    while (j < datosEmpleadoComprobates.Count)
                                    {
                                        DatosTimbreFiscalDigital timbre = (DatosTimbreFiscalDigital)timbreFiscalDigitales[i];
                                       

                                        if (string.Equals((datosEmpleadoComprobates[j].folio == null ? "" : datosEmpleadoComprobates[j].folio), (timbre.folio == null ? "" : timbre.folio), StringComparison.OrdinalIgnoreCase))
                                        {
                                           
                                            if (string.Equals(timbre.status, "200", StringComparison.OrdinalIgnoreCase) || string.Equals(timbre.status, "307", StringComparison.OrdinalIgnoreCase))
                                            {
                                                //CreateXMLTimbreFiscal(digital11.contruyeTimbreFiscalDigital(timbreFiscalDigitales[i]), string.Concat(ruta2, "pruebaTimbre.xml"));
                                               
                                                recibo = datosEmpleadoComprobates[j].cfdiEmpleado.cfdiRecibo;
                                                CreateXML(datosEmpleadoComprobates[j].comprobante, string.Concat(ruta2, datosEmpleadoComprobates[j].nombreArchivo));
                                                string cadenaOriginal = getCadenaoriginal(string.Concat(ruta2, datosEmpleadoComprobates[j].nombreArchivo));
                                                recibo.cadenaCertificado = cadenaOriginal;
                                                recibo.certificadoTimbrado=(timbre.noCertificadoSAT);
                                                recibo.noCertificadoSAT = (timbre.noCertificadoSAT);
                                                recibo.fechaHoraTimbrado=(timbre.fechaTimbrado);
                                                recibo.selloTimbrado=(timbre.selloSAT);
                                                recibo.selloSAT = (timbre.selloSAT);
                                                recibo.UUID=(timbre.uuid);
                                                recibo.rfcProvCertif = (timbre.referenciasProveedor);
                                                recibo.statusTimbrado=(StatusTimbrado.TIMBRADO);
                                                recibo.statusXmlSat=(StatusXmlSat.RECIBIDO_SAT);
                                                recibo.xmlTimbrado = (timbre.xmlTimbrado);


                                                string cadenaOriginalSAT1 = "||" + timbre.version + "|" + timbre.uuid + "|" + timbre.fechaTimbrado + "|" + timbre.referenciasProveedor;
                                                if (timbre.descripcion != "")
                                                {
                                                    cadenaOriginalSAT1 += "|" + timbre.descripcion + "|" + timbre.selloCFD + "|" + timbre.noCertificadoSAT + "||";
                                                }
                                                else
                                                {
                                                    cadenaOriginalSAT1 += "|" + timbre.selloCFD + "|" + timbre.noCertificadoSAT + "||";
                                                }

                                                recibo.cadenaOriginalTimbrado = cadenaOriginalSAT1;


                                                datosEmpleadoComprobates[j].cfdiEmpleado.recienTimbrado=(true);
                                                datosEmpleadoComprobates[j].cfdiEmpleado.cfdiRecibo=(recibo);

                                              

                                                if (datosEmpleadoComprobates[i].cfdiEmpleado.id != 0)
                                                {
                                                    servicioCFDIEmpleado.actualizar(datosEmpleadoComprobates[j].cfdiEmpleado, dbContextSimple);
                                                }

                                            }
                                            else
                                            {
                                                datosEmpleadoComprobates[j].statusTimbrado = StatusTimbrado.ERROR;
                                                datosEmpleadoComprobates[j].cfdiEmpleado.cfdiRecibo.mensajeRec = (timbre.descripcion);

                                                datosEmpleadoComprobates[j].cfdiEmpleado.cfdiRecibo.statusTimbrado = StatusTimbrado.ERROR;
                                                datosEmpleadoComprobates[j].cfdiEmpleado.cfdiRecibo.statusXmlSat = StatusXmlSat.NINGUNO;
                                                datosEmpleadoComprobates[j].cfdiEmpleado.cfdiRecibo.xmlTimbrado = null;
                                                if (datosEmpleadoComprobates[j].cfdiEmpleado.id != 0)
                                                {
                                                    servicioCFDIEmpleado.actualizar(datosEmpleadoComprobates[j].cfdiEmpleado, dbContextSimple);
                                                }
                                            }

                                            datosEmpleadoComprobates[j].cfdiEmpleado.mensaje = (timbre.descripcion);
                                            listCFDIEmpleados.Add(datosEmpleadoComprobates[j].cfdiEmpleado);
                                           // datosEmpleadoComprobates.RemoveAt(j);
                                            j = 0;
                                            break;
                                        }

                                        else
                                        {
                                            j++;
                                        }

                                    }
                                }
                            }
                        }

                    }

                    if (listCFDIEmpleadosTimbrados.Count() > 0)
                    {
                        listCFDIEmpleados.AddRange(listCFDIEmpleadosTimbrados);
                        //                    Utilerias.ordena(listCFDIEmpleados, uuidCxn);
                    }
                }

               

            }
            catch (Exception ex)
            {

                throw;
            }


            return listCFDIEmpleados;
        }

        private Comprobante construyeComprobante(CFDIEmpleado cfdiEmpleado)
        {
            try
            {
                Comprobante comprobante = new Comprobante();
                asignarCertificado(comprobante, cfdiEmpleado.certificadoAsignado);//@
                comprobante.Emisor = (construyeEmisor(cfdiEmpleado.razonesSociales)); //@
                comprobante.Receptor = (construyeReceptor(cfdiEmpleado)); //@
                List<ComprobanteConcepto> listConceptos = new List<ComprobanteConcepto>();
                listConceptos.Add(contruyeConceptos(cfdiEmpleado));
                // comprobante.Conceptos=(contruyeListaConceptos(listConceptos)); //@

                
                comprobante.Conceptos = listConceptos.ToArray(); 
                comprobante.Version = ("3.3"); 

                if (cfdiEmpleado.cfdiRecibo.serieCFDI != "")
                    comprobante.Serie = (cfdiEmpleado.cfdiRecibo.serieCFDI);

                if (cfdiEmpleado.cfdiRecibo.folioCFDI != "")
                    comprobante.Folio = (cfdiEmpleado.cfdiRecibo.folioCFDI);

                fechaGeneraXML = DateTime.Now;

                comprobante.Fecha = (UtileriasSat.castFechatoXmlFechaFormato(fechaGeneraXML, UtileriasSat.FORMATO_FECHA_HORA_SAT));
                comprobante.FormaPago = ("99");
                comprobante.MetodoPago = ManejadorEnum.GetDescription(CMetodoPago.PUE);

                comprobante.TipoDeComprobante = ManejadorEnum.GetDescription(CTipoDeComprobante.N);
                comprobante = calculaImportesConceptos(comprobante, cfdiEmpleado);
                
               
                String lugarExpedicion = "";
                if (cfdiEmpleado.codigoPostal == null ? false : cfdiEmpleado.codigoPostal.Trim().Length > 0)
                {
                    lugarExpedicion = string.Concat(lugarExpedicion, cfdiEmpleado.razonesSociales.cp.clave);// cfdiEmpleado.codigoPostal lugarExpedicion.concat(cfdiEmpleado.getCodigoPostal());
                }
                comprobante.LugarExpedicion = (lugarExpedicion); 
                
                
                comprobante.Moneda = ManejadorEnum.GetDescription(CMoneda.MXN);

                if (cfdiEmpleado.cfdiRecibo.UUIDRelacionado != null)
                {
                    ConstruyeCfdiRelacionado(cfdiEmpleado.cfdiRecibo.UUIDRelacionado);
                 }

                return comprobante;
            }
            catch (Exception e)
            {
                //utilSat.bitacora(e.getMessage());
                //System.err.println(concatena.delete(0, concatena.length()).append(msgError).append("contruyeComprobante()1_Error: ").append(e));
                return null;
            }
        }

        //private Comprobante asignarCertificado(Comprobante comprobante, RazonesSociales razonSocial)
        private Comprobante asignarCertificado(Comprobante comprobante, Certificados certificado)
        {
           
            string numeroCertificado, aa, b, c;
            SelloDigital oSelloDigital = new SelloDigital();
           
            if (comprobante == null)
            {
                comprobante = new Comprobante();
            }

            try
            {

                /*if (razonSocial.certificadoSAT == null)
                {
                    comprobante.Certificado = oSelloDigital.Certificado(razonSocial.rutaCert);
                    SelloDigital.leerCER(razonSocial.rutaCert, out aa, out b, out c, out numeroCertificado);
                    comprobante.NoCertificado = numeroCertificado;
                }
                else
                {
                    comprobante.Certificado = oSelloDigital.Certificado(razonSocial.certificadoSAT);
                    SelloDigital.leerCER(razonSocial.certificadoSAT, out aa, out b, out c, out numeroCertificado);
                    comprobante.NoCertificado = numeroCertificado;
                }

                comprobante.NoCertificado = numeroCertificado; */
                comprobante.Certificado = oSelloDigital.Certificado(certificado.certificado);
                SelloDigital.leerCER(certificado.certificado, out aa, out b, out c, out numeroCertificado);
                comprobante.NoCertificado = numeroCertificado;

            }
            catch (Exception ex)
            {
                //utilSat.bitacora(ex.getMessage());
                //erroresArchivos.append(ex.getMessage().concat("|"));
              //  System.err.println(concatena.delete(0, concatena.length()).append(msgError).append("asignarCertificado()1_Error: ").append(ex));
            }

            return comprobante;
        }

        private ComprobanteEmisor construyeEmisor(RazonesSociales razonSocial)
        {

            ComprobanteEmisor emisor = new ComprobanteEmisor();
            emisor.Rfc = (razonSocial.rfc);  //@
            if (razonSocial.razonsocial != "")
            emisor.Nombre = (razonSocial.razonsocial);

            emisor.RegimenFiscal = (razonSocial.regimenFiscal);//Preguntar si sera fisico o moral //JEVC01
                                                               //emisor.getRegimenFiscal().add(construyeEmisorRegimenFiscal(razonSocial.getRegimenFiscal()));
            return emisor;
        }
        
        private ComprobanteReceptor construyeReceptor(CFDIEmpleado cfdiEmpleado)
        {
            ComprobanteReceptor receptor = new ComprobanteReceptor();
            //agregar el nombre
            
            receptor.Rfc = (cfdiEmpleado.RFC); 
            if (cfdiEmpleado.nombre != "") 
            { 
                receptor.Nombre = (cfdiEmpleado.nombre); 
            }
            receptor.UsoCFDI = ManejadorEnum.GetDescription(CUsoCFDI.P_01);
            return receptor;
        }

        private ComprobanteConcepto contruyeConceptos(CFDIEmpleado cfdiEmpleado)
        {
            ComprobanteConcepto concepto = new ComprobanteConcepto();
            concepto.Cantidad = (UtileriasSat.castNumerosToBigDecimal(1)); //@pendiente el cast
            concepto.Descripcion = ("Pago de nómina"); //@
            concepto.ClaveProdServ = ("84111505");//84111505
            concepto.ClaveUnidad = ("ACT");
            //no hay necesidad de poner el nodo impuesto
            Double totalPercepcion = 0.0, totalDeduciones = 0.0, totalOtrosPagos = 0.00;
            List<CFDIReciboConcepto> conceptos = cfdiEmpleado.cfdiRecibo.cfdiReciboConcepto;
            if (conceptos != null)
            {
                int i;
                for (i = 0; i < conceptos.Count(); i++)
                {
                   
                    if (string.Equals(conceptos[i].tipoNaturaleza, ManejadorEnum.GetDescription(Naturaleza.PERCEPCION), StringComparison.OrdinalIgnoreCase))
                    {
                        if (conceptos[i].otroPago)
                        {
                            totalOtrosPagos = totalOtrosPagos + (conceptos[i].importeExento) + (conceptos[i].importeGravable);
                        }
                        else {
                            totalPercepcion = totalPercepcion + conceptos[i].importeExento + conceptos[i].importeGravable;
                        }

                        
                    }
                    else if (string.Equals(conceptos[i].tipoNaturaleza, ManejadorEnum.GetDescription(Naturaleza.DEDUCCION), StringComparison.OrdinalIgnoreCase))
                    {
                        totalDeduciones = totalDeduciones + conceptos[i].importeExento + conceptos[i].importeGravable;
                    }

                }
            }
            concepto.Descuento = UtileriasSat.castNumerosToBigDecimal(totalDeduciones);
            concepto.Importe = (UtileriasSat.castNumerosToBigDecimal(totalPercepcion + totalOtrosPagos)); //@//pendiente la conversion 
            concepto.ValorUnitario = (UtileriasSat.castNumerosToBigDecimal(totalPercepcion+ totalOtrosPagos)); //@//pendiente la conversion
            return concepto;
        }

        private Comprobante calculaImportesConceptos(Comprobante comprobante, CFDIEmpleado cfdiEmpleado)
        {
            if (comprobante == null)
            {
                comprobante = new Comprobante();
            }
            Double totalDeduccion = 0.0, subTotal = 0.0, impuestoRetenido = 0.0, descuentos = 0.0, total = 0.0;
            List<CFDIReciboConcepto> conceptos = cfdiEmpleado.cfdiRecibo.cfdiReciboConcepto;
            ////        CFDIReciboConcepto cfdiReciboConceptoISR = null;
            if (conceptos != null)
            {
                int i;
                for (i = 0; i < conceptos.Count(); i++)
                {
                    if (mapClaveConceptosISR.ContainsKey(conceptos[i].claveConcepto))
                    {
                        ////                    cfdiReciboConceptoISR = conceptos.get(i);
                        impuestoRetenido = (conceptos[i].importeExento) + (conceptos[i].importeGravable);

                    }
                    if (string.Equals(conceptos[i].tipoNaturaleza, ManejadorEnum.GetDescription(Naturaleza.PERCEPCION)) | conceptos[i].otroPago ) 
                    {
                        if (conceptos[i].tipoNaturaleza != "CALCULO")
                            subTotal = subTotal + (conceptos[i].importeExento) + (conceptos[i].importeGravable);
                    }
                    else if (string.Equals(conceptos[i].tipoNaturaleza, ManejadorEnum.GetDescription(Naturaleza.DEDUCCION), StringComparison.OrdinalIgnoreCase))
                    {
                        if (!mapClaveConceptosISR.ContainsKey(conceptos[i].claveConcepto))
                        {
                            descuentos = descuentos + (conceptos[i].importeExento) + (conceptos[i].importeGravable);
                        }
                        totalDeduccion = totalDeduccion + (conceptos[i].importeExento) + (conceptos[i].importeGravable);
                    }
                }
            }

            comprobante.Descuento = (UtileriasSat.castNumerosToBigDecimal(descuentos + impuestoRetenido));
            comprobante.SubTotal = (UtileriasSat.castNumerosToBigDecimal(subTotal)); //@
            total = subTotal - descuentos - impuestoRetenido;
            comprobante.Total = (UtileriasSat.castNumerosToBigDecimal(total)); //Pendiente
            return comprobante;
        }

        static public String construyeRutaXML(RazonesSociales razonesSociales, TipoNomina tipoNomina, PeriodosNomina periodosNomina)
        {
            StringBuilder builder = new StringBuilder();
            String ruta = razonesSociales.ubicacionXML;
            try
            {
                if (ruta == null ? true : !ruta.Any())
                {
                    string path = System.IO.Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
                    if (Environment.OSVersion.Version.Major >= 6)
                    {
                        ruta = System.IO.Directory.GetParent(path).ToString();
                    }
                }
                builder.Append(ruta).Append(System.IO.Path.DirectorySeparatorChar).Append(razonesSociales.clave).Append("-").Append(razonesSociales.rfc);


                bool continuar = true;
                if (!System.IO.Directory.Exists(builder.ToString()))
                {
                    System.IO.Directory.CreateDirectory(builder.ToString());
                }

                if (continuar)
                {
                    builder.Append(System.IO.Path.DirectorySeparatorChar);
                    builder.Append(tipoNomina.clave);
                    if (!System.IO.Directory.Exists(builder.ToString()))
                    {
                        System.IO.Directory.CreateDirectory(builder.ToString());
                    }
                }
                if (continuar)
                {
                    builder.Append(System.IO.Path.DirectorySeparatorChar);
                    builder.Append(periodosNomina.año);
                    if (!System.IO.Directory.Exists(builder.ToString()))
                    {
                        System.IO.Directory.CreateDirectory(builder.ToString());
                    }
                }
                if (continuar)
                {
                    builder.Append(System.IO.Path.DirectorySeparatorChar);
                    builder.Append(periodosNomina.clave);
                    if (!System.IO.Directory.Exists(builder.ToString()))
                    {
                        System.IO.Directory.CreateDirectory(builder.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                // System.out.println("ERROR AL CREAR LA CARPETA " + file.getAbsolutePath());
                builder.Remove(0, builder.Length).Append(ruta);
            }
            return builder.ToString();
        }

        private Comprobante agregarComplementoNomina(Comprobante comprobante, Nomina nomina)
        {
            XmlDocument docNomina = new XmlDocument();
            XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
            xmlSerializerNamespaces.Add("nomina12", "http://www.sat.gob.mx/nomina12");
            XmlSerializer oXmlSerializar = new XmlSerializer(typeof(Nomina));
            using (XmlWriter writer = docNomina.CreateNavigator().AppendChild())
            {
                oXmlSerializar.Serialize(writer, nomina, xmlSerializerNamespaces);
            }
            comprobante.Complemento = new ComprobanteComplemento[1];
            comprobante.Complemento[0] = new ComprobanteComplemento();

            comprobante.Complemento[0].Any = new XmlElement[1];
            comprobante.Complemento[0].Any[0] = docNomina.DocumentElement;

            return comprobante;
        }

        private String nomenclaturaNombreArchivo(TipoNomina tipoNomina, PeriodosNomina periodosNomina, CFDIEmpleado cFDIEmpleado)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("TIPNOM_").Append((tipoNomina).clave).Append("_");
            builder.Append("PERIODO_").Append(periodosNomina.clave).Append("_");
            builder.Append("NUMEMP_").Append(cFDIEmpleado.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave).Append("_");
            builder.Append("FOLIO_").Append(cFDIEmpleado.cfdiRecibo.serieCFDI).Append(cFDIEmpleado.cfdiRecibo.folioCFDI).Append(".xml");
            return builder.ToString();

        }

        private static void CreateXML(Comprobante oComprobante, string pathXML)
        {
            //SERIALIZAMOS.-------------------------------------------------

            XmlSerializerNamespaces xmlNameSpace = new XmlSerializerNamespaces();
            xmlNameSpace.Add("cfdi", "http://www.sat.gob.mx/cfd/3");
            xmlNameSpace.Add("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");
            xmlNameSpace.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");


            XmlSerializer oXmlSerializar = new XmlSerializer(typeof(Comprobante));

            string sXml = "";

            using (var sww = new StringWriterWithEncoding(Encoding.UTF8))
            {

                using (XmlWriter writter = XmlWriter.Create(sww))
                {

                    oXmlSerializar.Serialize(writter, oComprobante, xmlNameSpace);
                    sXml = sww.ToString();
                }

            }

            //guardamos el string en un archivo
            System.IO.File.WriteAllText(pathXML, sXml);
        }
        private static void CreateXMLTimbreFiscal(TimbreFiscalDigital oComprobante, string pathXML)
        {
            //SERIALIZAMOS.-------------------------------------------------

       


            XmlSerializer oXmlSerializar = new XmlSerializer(typeof(TimbreFiscalDigital));

            string sXml = "";

            using (var sww = new StringWriterWithEncoding(Encoding.UTF8))
            {

                using (XmlWriter writter = XmlWriter.Create(sww))
                {

                    oXmlSerializar.Serialize(writter, oComprobante);
                    sXml = sww.ToString();
                }

            }

            //guardamos el string en un archivo
            System.IO.File.WriteAllText(pathXML, sXml);
        }

        private string getCadenaoriginal(string pathXML)
        {
            string cadenaOriginal = "";

            string pathxsl = ruta + @"\recursos\xsltLocal\cadenaoriginal_3_3.xslt";
            System.Xml.Xsl.XslCompiledTransform transformador = new System.Xml.Xsl.XslCompiledTransform(true);
            transformador.Load(pathxsl);

            using (StringWriter sw = new StringWriter())
            using (XmlWriter xwo = XmlWriter.Create(sw, transformador.OutputSettings))
            {

                transformador.Transform(pathXML, xwo);
                cadenaOriginal = sw.ToString();
            }

            return cadenaOriginal;
        }

        private ComprobanteCfdiRelacionados ConstruyeCfdiRelacionado(string uuidRelacionado) 
        {
            ComprobanteCfdiRelacionados cfdiRelacionados = new ComprobanteCfdiRelacionados();

            cfdiRelacionados.TipoRelacion = "04";
            cfdiRelacionados.CfdiRelacionado[0].UUID = uuidRelacionado;

            return cfdiRelacionados;
        }


        private class DatosEmpleadoComprobate
        {

            public CFDIEmpleado cfdiEmpleado { get; set; }
            public Comprobante comprobante { get; set; }
            public byte[] archivoByte { get; set; }
            public String nombreArchivo { get; set; }
            public StatusTimbrado statusTimbrado { get; set; }
            public String folio { get; set; }

            public DatosEmpleadoComprobate(CFDIEmpleado cfdiEmpleado, Comprobante comprobante)
            {
                this.cfdiEmpleado = cfdiEmpleado;
                this.comprobante = comprobante;
                folio = comprobante.Folio;
            }

            //public CFDIEmpleado getCfdiEmpleado()
            //{
            //    return cfdiEmpleado;
            //}

            //public void setCfdiEmpleado(CFDIEmpleado cfdiEmpleado)
            //{
            //    this.cfdiEmpleado = cfdiEmpleado;
            //}

            //public Comprobante getComprobante()
            //{
            //    return comprobante;
            //}

            //public void setComprobante(Comprobante comprobante)
            //{
            //    this.comprobante = comprobante;
            //}

            //public byte[] getArchivoByte()
            //{
            //    return archivoByte;
            //}

            //public void setArchivoByte(byte[] archivoByte)
            //{
            //    this.archivoByte = archivoByte;
            //}

            //public String getNombreArchivo()
            //{
            //    return nombreArchivo;
            //}

            //public void setNombreArchivo(String nombreArchivo)
            //{
            //    this.nombreArchivo = nombreArchivo;
            //}

            //public StatusTimbrado getStatusTimbrado()
            //{
            //    return statusTimbrado;
            //}

            //public void setStatusTimbrado(StatusTimbrado statusTimbrado)
            //{
            //    this.statusTimbrado = statusTimbrado;
            //}

            //public String getFolio()
            //{
            //    return folio;
            //}

            //public void setFolio(String folio)
            //{
            //    this.folio = folio;
            //}
        }

    }
}
