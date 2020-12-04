using Exitosw.Payroll.Core.CFDI.Timbrado;
using Exitosw.Payroll.Entity.entidad.cfdi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Exitosw.Payroll.Core.modelo;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.CFDI.PACS
{
    public class SolucionFactible
    {
        //com.Wssolucionfactible.Timbrado Timbrado serviceTimbrado;
        //TimbradoPortType servicePortTimbrado;
        public String error { get; set; }
        public String descripcion { get; set; }
        DateTime fechaInicial;
        DateTime fechaFinal;
        List<object> datosTimbreFiscalDigitales = null;
        List<object> datosCancelados = null;
        private byte[] CertCSD = null;
        private byte[] KeyCSD = null;
        private string PasswordCSD = "";
        private string Usuario = "";
        private string Password = "";
        private string Url = "";
        private string RFC = "";
        private decimal total = 0;
        private string UsuarioSistema = "";
        private string folio = "";
        private string serie = "";
        private decimal cfdirecibo_id = -1;
        BITCancelacionDAO bitCancelacionDAO = new BITCancelacionDAO();



        public SolucionFactible()
        {
            //  this.serviceTimbrado = null;
            // this.servicePortTimbrado = null;
        }

        public List<object> Timbrar(InfoATimbrar infoATimbrar)
        {

            try
            {
                WsSolucionfactible.Timbrado timbrar = new WsSolucionfactible.Timbrado();
                WsSolucionfactible.CFDICertificacion certificar = new WsSolucionfactible.CFDICertificacion();
                WsSolucionfactible.CFDIResultadoCertificacion response = new WsSolucionfactible.CFDIResultadoCertificacion();

                List<InfoExtra> extras = infoATimbrar.infoExtras;
                DatosTimbreFiscalDigital datosTimbreFiscalDigital;
                datosTimbreFiscalDigitales = new List<object>();
                if (extras != null)
                {
                    List<byte[]> xmlCfdi = new List<byte[]>();
                    for (int i = 0; i < extras.Count; i++)
                    {
                        timbrar.Url = infoATimbrar.urlWebServices;
                        certificar = timbrar.timbrar(infoATimbrar.usuario, infoATimbrar.password, extras[i].archivoXML, false, true);
                        if (certificar.status == 200)
                        {

                            response = certificar.resultados[0];
                            byte[] inter_byte =  certificar.resultados[0].cfdiTimbrado;



                            


                            if (response.status == 200 | response.status == 307)
                            {
                                
                                datosTimbreFiscalDigital = new DatosTimbreFiscalDigital();
                                //datosTimbreFiscalDigital.setDescripcion("");
                                datosTimbreFiscalDigital.error = 0;
                               datosTimbreFiscalDigital.fechaTimbrado = (response.fechaTimbrado.GetValueOrDefault());
                                datosTimbreFiscalDigital.noCertificadoSAT = (response.certificadoSAT);
                                //datosTimbreFiscalDigital.setSelloCFD(cfdiCertificacion.getSelloCFD());
                                datosTimbreFiscalDigital.selloSAT = (response.selloSAT);
                                datosTimbreFiscalDigital.uuid = (response.uuid);
                                datosTimbreFiscalDigital.version = (response.versionTFD);
                                datosTimbreFiscalDigital.status = (response.status.ToString());
                                datosTimbreFiscalDigital.referenciasProveedor = ("SFE0807172W8");
                                datosTimbreFiscalDigital.folio = (extras[i].folio);
                                datosTimbreFiscalDigital.xmlTimbrado = inter_byte;
                                datosTimbreFiscalDigitales.Add(datosTimbreFiscalDigital);
                            }
                            else
                            { 
                                datosTimbreFiscalDigital = new DatosTimbreFiscalDigital();
                                datosTimbreFiscalDigital.status = Convert.ToString(response.status);
                                datosTimbreFiscalDigital.error = response.status;
                                datosTimbreFiscalDigital.descripcion = response.mensaje;
                                datosTimbreFiscalDigital.folio = (extras[i].folio);
                                datosTimbreFiscalDigitales.Add(datosTimbreFiscalDigital);
                                this.error = response.mensaje;
                            }

                        }
                        else
                        {
                            this.error = certificar.mensaje;
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return datosTimbreFiscalDigitales;
        }

        public List<object> cancelarAcuse(InfoATimbrar infoATimbrar,  DBContextAdapter dbContext)
        {
            datosCancelados = new List<object>();
            cUUIDCancelado UUIDCancelado = null;
            try
            {
                WsSolucionfactibleCancelar.Cancelacion cancelacion = new WsSolucionfactibleCancelar.Cancelacion();
                WsSolucionfactibleCancelar.CancelacionAsincronoResponse response = new WsSolucionfactibleCancelar.CancelacionAsincronoResponse();
                cancelacion.Url = infoATimbrar.urlWebServices.Substring(0, infoATimbrar.urlWebServices.LastIndexOf("/") + 1) + "Cancelacion ";
                List<InfoExtra> extras = infoATimbrar.infoExtras;
                this.Url = infoATimbrar.urlWebServices;
                this.Usuario = infoATimbrar.usuario;
                this.Password = infoATimbrar.password;
                this.PasswordCSD = infoATimbrar.passwordKey;
                this.KeyCSD = infoATimbrar.archivoKey;
                this.CertCSD = infoATimbrar.archivoPfx;
                
                


                for (int i = 0; i < extras.Count; i++)
                {
                    
                    this.RFC = extras[i].rfcEmisor;
                    this.folio = extras[i].folio;
                    this.serie = extras[i].serie;
                    this.cfdirecibo_id = extras[i].cfdirecibo_id;
                    this.UsuarioSistema = extras[i].usuario;
                    UUIDCancelado = cancelar(extras[i].UUID, dbContext);
                    datosCancelados.Add(UUIDCancelado);
                }

            }
            catch (Exception)
            {

                throw;
            }
            return datosCancelados;
        }

        public cUUIDCancelado cancelar(string UUID, DBContextAdapter dbContext) {
            cUUIDCancelado UUIDCancelado = null;
            try
            {
                WsSolucionfactibleCancelar.Cancelacion cancelacion = new WsSolucionfactibleCancelar.Cancelacion();
                WsSolucionfactibleCancelar.CancelacionAsincronoResponse response = new WsSolucionfactibleCancelar.CancelacionAsincronoResponse();
                WsSolucionfactibleCancelar.StatusCancelacionResponse status = new  WsSolucionfactibleCancelar.StatusCancelacionResponse();

                BITCancelacion bITCancelacion = new BITCancelacion();
                cancelacion.Url = this.Url.Substring(0, this.Url.LastIndexOf("/") + 1) + "Cancelacion ";
                
                response = cancelacion.cancelarAsincrono(Usuario, Password, UUID, RFC, "", CertCSD, KeyCSD, PasswordCSD, null);
                switch (response.status)
                {
                    case 200:
                        bITCancelacion.fechaYHora = DateTime.Now;
                        bITCancelacion.usuario = this.UsuarioSistema;
                        bITCancelacion.proceso = ProcesoBitcancelacion.CANCELARASINCRONO;
                        bITCancelacion.status = StatusBitcancelacion.SolicitudRegistrada;
                        bITCancelacion.cfdiRecibo_ID = this.cfdirecibo_id;
                        bITCancelacion.statusMsj = "[" + response.status.ToString() + "] " + response.mensaje; 
                        bitCancelacionDAO.agregar(bITCancelacion, dbContext);

                        UUIDCancelado = new cUUIDCancelado();
                        UUIDCancelado.UUID = UUID;
                        UUIDCancelado.statusSAT = StatusXmlSat.CANCELABLESINACEPTACION;
                        UUIDCancelado.status = "200";
                        UUIDCancelado = CancelarAcuseStatusUno(UUID, dbContext);
                        break;
                    case 201:
                    case 202:
                    case 701:

                        bITCancelacion.fechaYHora = DateTime.Now;
                        bITCancelacion.usuario = this.UsuarioSistema;
                        bITCancelacion.proceso = ProcesoBitcancelacion.CANCELARASINCRONO;
                        bITCancelacion.status = StatusBitcancelacion.SolicitudRegistrada;
                        bITCancelacion.cfdiRecibo_ID = this.cfdirecibo_id;
                        bITCancelacion.statusMsj = "[" + response.status.ToString() + "] " + response.mensaje;
                        bitCancelacionDAO.agregar(bITCancelacion, dbContext);

                        UUIDCancelado = new cUUIDCancelado();
                        UUIDCancelado.UUID = UUID;
                        UUIDCancelado.statusSAT = StatusXmlSat.SOLICITUDREGISTRADA;
                        UUIDCancelado.status = "100";
                        break;
                    case 204:

                        bITCancelacion.fechaYHora = DateTime.Now;
                        bITCancelacion.usuario = this.UsuarioSistema;
                        bITCancelacion.proceso = ProcesoBitcancelacion.CANCELARASINCRONO;
                        bITCancelacion.status = StatusBitcancelacion.NoCancelable;
                        bITCancelacion.cfdiRecibo_ID = this.cfdirecibo_id;
                        bITCancelacion.statusMsj = "[" + response.status.ToString() + "] " + response.mensaje;
                        bitCancelacionDAO.agregar(bITCancelacion, dbContext);


                        UUIDCancelado = new cUUIDCancelado();
                        UUIDCancelado.UUID = UUID;
                        UUIDCancelado.statusSAT = StatusXmlSat.NOCANCELABLE;
                        UUIDCancelado.status = "100";
                        UUIDCancelado.observaciones = "[" + response.status.ToString() + "] " + response.mensaje;
                        break;
                    case 211:

                        bITCancelacion.fechaYHora = DateTime.Now;
                        bITCancelacion.usuario = this.UsuarioSistema;
                        bITCancelacion.proceso = ProcesoBitcancelacion.CANCELARASINCRONO;
                        bITCancelacion.status = StatusBitcancelacion.EnProcesoAceptacion;
                        bITCancelacion.cfdiRecibo_ID = this.cfdirecibo_id;
                        bITCancelacion.statusMsj = "[" + response.status.ToString() + "] " + response.mensaje; ;
                        bitCancelacionDAO.agregar(bITCancelacion, dbContext);
                        UUIDCancelado = new cUUIDCancelado();
                        UUIDCancelado.UUID = UUID;
                        UUIDCancelado.statusSAT = StatusXmlSat.ENPROCESOACEPTACION;
                        UUIDCancelado.status = "100";
                        UUIDCancelado.observaciones = "[" + response.status.ToString() + "] " + response.mensaje;
                        break;
                    case 213:
                        bITCancelacion.fechaYHora = DateTime.Now;
                        bITCancelacion.usuario = this.UsuarioSistema;
                        bITCancelacion.proceso = ProcesoBitcancelacion.CANCELARASINCRONO;
                        bITCancelacion.status = StatusBitcancelacion.SolicitudRechazada;
                        bITCancelacion.cfdiRecibo_ID = this.cfdirecibo_id;
                        bITCancelacion.statusMsj = "[" + response.status.ToString() + "] " + response.mensaje;
                        bitCancelacionDAO.agregar(bITCancelacion, dbContext);

                        UUIDCancelado = new cUUIDCancelado();
                        UUIDCancelado.UUID = UUID;
                        UUIDCancelado.statusSAT = StatusXmlSat.SOLICITUDRECHAZADA;
                        UUIDCancelado.status = "100";
                        UUIDCancelado.observaciones = "[" + response.status.ToString() + "] " + response.mensaje;
                        break;

                    default:
                        bITCancelacion.fechaYHora = DateTime.Now;
                        bITCancelacion.usuario = this.UsuarioSistema;
                        bITCancelacion.proceso = ProcesoBitcancelacion.CANCELARASINCRONO;
                        bITCancelacion.status = StatusBitcancelacion.Indefinido;
                        bITCancelacion.cfdiRecibo_ID = this.cfdirecibo_id;
                        bITCancelacion.statusMsj = "Error al realizar la cancelación: [" + response.status.ToString() + "] " + response.mensaje;
                        bitCancelacionDAO.agregar(bITCancelacion, dbContext);

                        UUIDCancelado = new cUUIDCancelado();
                        UUIDCancelado.UUID = UUID;
                        UUIDCancelado.status = "160";
                        UUIDCancelado.statusSAT = StatusXmlSat.INDEFINIDO;
                        UUIDCancelado.observaciones = "Error al realizar la cancelación: [" + response.status.ToString() + "] " + response.mensaje;
                        break;
                }

            }
            catch (Exception)
            {

                throw;
            }

            return UUIDCancelado;
        }
        public List<object> CancelarAcuseStatus(InfoATimbrar infoATimbrar, DBContextAdapter dbContext) {
            datosCancelados = new List<object>();
            try
            {
                cUUIDCancelado UUIDCancelado = null;

                WsSolucionfactibleCancelar.Cancelacion Cancelacion = new WsSolucionfactibleCancelar.Cancelacion();
                WsSolucionfactibleCancelar.StatusCancelacionResponse response = new WsSolucionfactibleCancelar.StatusCancelacionResponse();
                BITCancelacion bITCancelacion = new BITCancelacion();
                Cancelacion.Url = infoATimbrar.urlWebServices.Substring(0, infoATimbrar.urlWebServices.LastIndexOf("/") + 1) + "Cancelacion ";
                XmlDocument xmlAcuse = new XmlDocument();
                List<InfoExtra> extras = infoATimbrar.infoExtras;

                this.Url = infoATimbrar.urlWebServices;
                this.Usuario = infoATimbrar.usuario;
                this.Password = infoATimbrar.password;
                this.PasswordCSD = infoATimbrar.passwordKey;
                this.KeyCSD = infoATimbrar.archivoKey;
                this.CertCSD = infoATimbrar.archivoPfx;

                
                for (int i = 0; i < extras.Count; i++)
                {

                    this.RFC = extras[i].rfcEmisor;
                    this.folio = extras[i].folio;
                    this.serie = extras[i].serie;
                    this.cfdirecibo_id = extras[i].cfdirecibo_id;
                    this.UsuarioSistema = extras[i].usuario;

                    
                    response = Cancelacion.getStatusCancelacionAsincrona(this.Usuario, this.Password, extras[i].UUID, null);
                    switch (response.status)
                    {
                        case 200:

                            bITCancelacion.fechaYHora = DateTime.Now;
                            bITCancelacion.usuario = this.UsuarioSistema;
                            bITCancelacion.proceso = ProcesoBitcancelacion.GETSTATUSCANCELACION;
                            bITCancelacion.status = StatusBitcancelacion.ObtuvoAcuse;
                            bITCancelacion.cfdiRecibo_ID = this.cfdirecibo_id;
                            bITCancelacion.statusMsj = "[" + response.status.ToString() + "] " + response.mensaje;
                            bitCancelacionDAO.agregar(bITCancelacion, dbContext);

                            MemoryStream msXMLT = new MemoryStream(response.acuseSat);
                            msXMLT.Position = 0;
                            xmlAcuse.Load(msXMLT);

                            UUIDCancelado = new cUUIDCancelado();
                            UUIDCancelado.UUID = extras[i].UUID;
                            UUIDCancelado.status = "100";
                            UUIDCancelado.statusSAT = StatusXmlSat.SOLICITUDREGISTRADA;
                            UUIDCancelado.Acuse = System.Text.Encoding.UTF8.GetBytes(xmlAcuse.OuterXml);
                            datosCancelados.Add(UUIDCancelado);
                            break;
                        case 202:
                            bITCancelacion.fechaYHora = DateTime.Now;
                            bITCancelacion.usuario = this.UsuarioSistema;
                            bITCancelacion.proceso = ProcesoBitcancelacion.GETSTATUSCANCELACION;
                            bITCancelacion.status = StatusBitcancelacion.SolicitudRegistrada;
                            bITCancelacion.cfdiRecibo_ID = this.cfdirecibo_id;
                            bITCancelacion.statusMsj = "[" + response.status.ToString() + "] " + response.mensaje;
                            bitCancelacionDAO.agregar(bITCancelacion, dbContext);

                            UUIDCancelado = cancelar(extras[i].UUID, dbContext);
                            UUIDCancelado.UUID = extras[i].UUID;
                            UUIDCancelado.statusSAT = StatusXmlSat.SOLICITUDREGISTRADA;
                            UUIDCancelado.status = "100";
                            UUIDCancelado.observaciones = "";
                            datosCancelados.Add(UUIDCancelado);
                            break;
                        case 204:

                            bITCancelacion.fechaYHora = DateTime.Now;
                            bITCancelacion.usuario = this.UsuarioSistema;
                            bITCancelacion.proceso = ProcesoBitcancelacion.GETSTATUSCANCELACION;
                            bITCancelacion.status = StatusBitcancelacion.NoCancelable;
                            bITCancelacion.cfdiRecibo_ID = this.cfdirecibo_id;
                            bITCancelacion.statusMsj = "[" + response.status.ToString() + "] " + response.mensaje;
                            bitCancelacionDAO.agregar(bITCancelacion, dbContext);

                            UUIDCancelado = new cUUIDCancelado();
                            UUIDCancelado.UUID = extras[i].UUID;
                            UUIDCancelado.statusSAT = StatusXmlSat.NOCANCELABLE;
                            UUIDCancelado.status = "100";
                            UUIDCancelado.observaciones = "[" + response.status.ToString() + "] " + response.mensaje;
                            datosCancelados.Add(UUIDCancelado);
                            break;
                        case 211:
                            bITCancelacion.fechaYHora = DateTime.Now;
                            bITCancelacion.usuario = this.UsuarioSistema;
                            bITCancelacion.proceso = ProcesoBitcancelacion.GETSTATUSCANCELACION;
                            bITCancelacion.status = StatusBitcancelacion.EnProcesoAceptacion;
                            bITCancelacion.cfdiRecibo_ID = this.cfdirecibo_id;
                            bITCancelacion.statusMsj = "[" + response.status.ToString() + "] " + response.mensaje;
                            bitCancelacionDAO.agregar(bITCancelacion, dbContext);

                            UUIDCancelado = new cUUIDCancelado();
                            UUIDCancelado.UUID = extras[i].UUID;
                            UUIDCancelado.statusSAT = StatusXmlSat.ENPROCESOACEPTACION;
                            UUIDCancelado.status = "100";
                            UUIDCancelado.observaciones = "[" + response.status.ToString() + "] " + response.mensaje;
                            datosCancelados.Add(UUIDCancelado);
                            break;
                        case 213:

                            bITCancelacion.fechaYHora = DateTime.Now;
                            bITCancelacion.usuario = this.UsuarioSistema;
                            bITCancelacion.proceso = ProcesoBitcancelacion.GETSTATUSCANCELACION;
                            bITCancelacion.status = StatusBitcancelacion.SolicitudRechazada;
                            bITCancelacion.cfdiRecibo_ID = this.cfdirecibo_id;
                            bITCancelacion.statusMsj = "[" + response.status.ToString() + "] " + response.mensaje;
                            bitCancelacionDAO.agregar(bITCancelacion, dbContext);

                            UUIDCancelado = new cUUIDCancelado();
                            UUIDCancelado.UUID = extras[i].UUID;
                            UUIDCancelado.statusSAT = StatusXmlSat.SOLICITUDRECHAZADA;
                            UUIDCancelado.status = "100";
                            UUIDCancelado.observaciones = "[" + response.status.ToString() + "] " + response.mensaje;
                            datosCancelados.Add(UUIDCancelado);
                            break;
                        default:
                            bITCancelacion.fechaYHora = DateTime.Now;
                            bITCancelacion.usuario = this.UsuarioSistema;
                            bITCancelacion.proceso = ProcesoBitcancelacion.GETSTATUSCANCELACION;
                            bITCancelacion.status = StatusBitcancelacion.SolicitudRegistrada;
                            bITCancelacion.cfdiRecibo_ID = this.cfdirecibo_id;
                            bITCancelacion.statusMsj = "[" + response.status.ToString() + "] " + response.mensaje;
                            bitCancelacionDAO.agregar(bITCancelacion, dbContext);


                            UUIDCancelado = new cUUIDCancelado();
                            UUIDCancelado.UUID = extras[i].UUID; 
                            UUIDCancelado.status = "160";
                            UUIDCancelado.statusSAT = StatusXmlSat.SOLICITUDREGISTRADA;
                            UUIDCancelado.observaciones = "Error al obtener el status de la cancelación: [" + response.status.ToString() + "] " + response.mensaje;
                            datosCancelados.Add(UUIDCancelado);
                            break;
                    }


                }
            }
            catch (Exception)
            {

                throw;
            }

            return datosCancelados;
        }

        public cUUIDCancelado CancelarAcuseStatusUno(string UUID, DBContextAdapter dbContext) {

            cUUIDCancelado UUIDCancelado = null;
            BITCancelacion bITCancelacion = new BITCancelacion();
            try {
                

                WsSolucionfactibleCancelar.Cancelacion Cancelacion = new WsSolucionfactibleCancelar.Cancelacion();
                WsSolucionfactibleCancelar.StatusCancelacionResponse response = new WsSolucionfactibleCancelar.StatusCancelacionResponse();

                Cancelacion.Url = this.Url.Substring(0, this.Url.LastIndexOf("/") + 1) + "Cancelacion ";
                XmlDocument xmlAcuse = new XmlDocument();
                response = Cancelacion.getStatusCancelacionAsincrona(this.Usuario, this.Password, UUID, null);
                switch (response.status)
                {
                    case 200:

                        bITCancelacion.fechaYHora = DateTime.Now;
                        bITCancelacion.usuario = this.UsuarioSistema;
                        bITCancelacion.proceso = ProcesoBitcancelacion.GETSTATUSCANCELACION;
                        bITCancelacion.status = StatusBitcancelacion.ObtuvoAcuse;
                        bITCancelacion.cfdiRecibo_ID = this.cfdirecibo_id;
                        bITCancelacion.statusMsj = response.mensaje;
                        bitCancelacionDAO.agregar(bITCancelacion, dbContext);


                       /* MemoryStream msXMLT = new MemoryStream(response.acuseSat);
                        msXMLT.Position = 0;
                        xmlAcuse.Load(msXMLT); */

                        UUIDCancelado = new cUUIDCancelado();
                        UUIDCancelado.UUID = UUID;
                        UUIDCancelado.status = "100";
                        UUIDCancelado.statusSAT = StatusXmlSat.SOLICITUDREGISTRADA;
                        UUIDCancelado.Acuse = System.Text.Encoding.UTF8.GetBytes(xmlAcuse.OuterXml);
                        break;
                    case 202:

                        bITCancelacion.fechaYHora = DateTime.Now;
                        bITCancelacion.usuario = this.UsuarioSistema;
                        bITCancelacion.proceso = ProcesoBitcancelacion.GETSTATUSCANCELACION;
                        bITCancelacion.status = StatusBitcancelacion.SolicitudRegistrada;
                        bITCancelacion.cfdiRecibo_ID = this.cfdirecibo_id;
                        bITCancelacion.statusMsj = response.mensaje;
                        bitCancelacionDAO.agregar(bITCancelacion, dbContext);

                        UUIDCancelado = cancelar(UUID, dbContext);
                        UUIDCancelado.UUID = UUID;
                        UUIDCancelado.statusSAT = StatusXmlSat.SOLICITUDREGISTRADA;
                        UUIDCancelado.status = "100";
                        UUIDCancelado.observaciones = "";
                        break;
                    case 204:

                        bITCancelacion.fechaYHora = DateTime.Now;
                        bITCancelacion.usuario = this.UsuarioSistema;
                        bITCancelacion.proceso = ProcesoBitcancelacion.GETSTATUSCANCELACION;
                        bITCancelacion.status = StatusBitcancelacion.NoCancelable;
                        bITCancelacion.cfdiRecibo_ID = this.cfdirecibo_id;
                        bITCancelacion.statusMsj = response.mensaje;
                        bitCancelacionDAO.agregar(bITCancelacion, dbContext);

                        UUIDCancelado = new cUUIDCancelado();
                        UUIDCancelado.UUID = UUID;
                        UUIDCancelado.statusSAT = StatusXmlSat.NOCANCELABLE;
                        UUIDCancelado.status = "100";
                        UUIDCancelado.observaciones = "[" + response.status.ToString() + "] " + response.mensaje;
                        break;
                    case 211:
                        bITCancelacion.fechaYHora = DateTime.Now;
                        bITCancelacion.usuario = this.UsuarioSistema;
                        bITCancelacion.proceso = ProcesoBitcancelacion.GETSTATUSCANCELACION;
                        bITCancelacion.status = StatusBitcancelacion.EnProcesoAceptacion;
                        bITCancelacion.cfdiRecibo_ID = this.cfdirecibo_id;
                        bITCancelacion.statusMsj = response.mensaje;
                        bitCancelacionDAO.agregar(bITCancelacion, dbContext);


                        UUIDCancelado = new cUUIDCancelado();
                        UUIDCancelado.UUID = UUID;
                        UUIDCancelado.statusSAT = StatusXmlSat.ENPROCESOACEPTACION;
                        UUIDCancelado.status = "100";
                        UUIDCancelado.observaciones = "[" + response.status.ToString() + "] " + response.mensaje;
                        break;
                    case 213:

                        bITCancelacion.fechaYHora = DateTime.Now;
                        bITCancelacion.usuario = this.UsuarioSistema;
                        bITCancelacion.proceso = ProcesoBitcancelacion.GETSTATUSCANCELACION;
                        bITCancelacion.status = StatusBitcancelacion.SolicitudRechazada;
                        bITCancelacion.cfdiRecibo_ID = this.cfdirecibo_id;
                        bITCancelacion.statusMsj = response.mensaje;
                        bitCancelacionDAO.agregar(bITCancelacion, dbContext);

                        UUIDCancelado = new cUUIDCancelado();
                        UUIDCancelado.UUID = UUID;
                        UUIDCancelado.statusSAT = StatusXmlSat.SOLICITUDRECHAZADA;
                        UUIDCancelado.status = "100";
                        UUIDCancelado.observaciones = "[" + response.status.ToString() + "] " + response.mensaje;
                        break;
                    default:

                        bITCancelacion.fechaYHora = DateTime.Now;
                        bITCancelacion.usuario = this.UsuarioSistema;
                        bITCancelacion.proceso = ProcesoBitcancelacion.GETSTATUSCANCELACION;
                        bITCancelacion.status = StatusBitcancelacion.SolicitudRegistrada;
                        bITCancelacion.cfdiRecibo_ID = this.cfdirecibo_id;
                        bITCancelacion.statusMsj = response.mensaje;
                        bitCancelacionDAO.agregar(bITCancelacion, dbContext);

                        UUIDCancelado = new cUUIDCancelado();
                        UUIDCancelado.UUID = UUID;
                        UUIDCancelado.status = "160";
                        UUIDCancelado.statusSAT = StatusXmlSat.SOLICITUDREGISTRADA;
                        UUIDCancelado.observaciones = "Error al obtener el status de la cancelación: [" + response.status.ToString() + "] " + response.mensaje;
                        break;
                }


            }

            catch (Exception)
            {

                throw;
            }
            return UUIDCancelado;
        }
    }
}
