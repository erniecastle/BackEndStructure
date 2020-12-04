using System;
using Exitosw.Payroll.Hibernate.entidad;
using Exitosw.Payroll.Hibernate.modelo;
using NHibernate;
using Exitosw.Payroll.Core.util;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Net.Mail;
using System.Web.Script.Serialization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using Stimulsoft.Report;
using Stimulsoft.Report.Export;
using System.Data;
using Stimulsoft.Base;
using System.Xml.Xsl;
using System.Data.Entity.Migrations;
using System.Collections;
using Stimulsoft.Report.Dictionary;

namespace Exitosw.Payroll.Core.modeloHB
{
    public class EnvioCorreoHDAO : NHibernateRepository<EnvioCorreoHDAO>, EnvioCorreoHDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        List<Object> listaCursos = new List<Object>();
        IQuery query;

        public Mensaje getCorreoEmpleadosPorFiltros(Dictionary<string, object> filtros, ISession uuidCxn)
        {
            StringBuilder builder = new StringBuilder();
            try
            {
                inicializaVariableMensaje();
                setSession(uuidCxn);
                getSession().BeginTransaction();
                String query = null;
                IQuery q = null;
                int tipoConsulta = -1;
                int tipoReporte = 0;
                string action = "";

                if (filtros.ContainsKey("tipoArchivo"))
                {
                    tipoConsulta = Convert.ToInt32(filtros["tipoArchivo"]);
                }

                if (filtros.ContainsKey("action"))
                {
                    action = (string)filtros["action"];
                }

                #region TypeOthers Files --
                if (tipoConsulta == 4)
                {
                    builder.Append("Select ");
                    builder.Append("o.id as id, o.plazasPorEmpleado.empleados.clave as clave, ");
                    builder.Append(" concat(o.plazasPorEmpleado.empleados.nombre, '  ', o.plazasPorEmpleado.empleados.apellidoPaterno, ' ', o.plazasPorEmpleado.empleados.apellidoMaterno) as nombre, ");
                    builder.Append("o.plazasPorEmpleado.empleados.correoElectronico as correo ");
                    builder.Append("from PlazasPorEmpleadosMov o WHERE 1=1 ");

                    if (filtros.ContainsKey("claveTipoNomina"))
                    {
                        builder.Append("AND o.tipoNomina.clave = :claveTipoNomina ");
                    }

                    if (filtros.ContainsKey("claveRegistroPatronal"))
                    {
                        builder.Append("AND o.plazasPorEmpleado.registroPatronal.clave = :claveRegistroPatronal ");
                    }

                    if (filtros.ContainsKey("claveCentroDeCostos"))
                    {
                        builder.Append("AND o.centroDeCosto.clave = :claveCentroDeCostos ");
                    }

                    builder.Append("AND o.id IN (Select MAX(m.id) ");
                    builder.Append("from PlazasPorEmpleadosMov m WHERE (m.plazasPorEmpleado.empleados.clave = o.plazasPorEmpleado.empleados.clave) ");

                    if (filtros.ContainsKey("fechaInicialPeriodo") & filtros.ContainsKey("fechaFinalPeriodo"))
                    {
                        builder.Append("AND ((m.fechaInicial <= :fechaInicialPeriodo) OR (m.fechaInicial Between :fechaInicialPeriodo AND :fechaFinalPeriodo) ");
                        builder.Append("AND (m.plazasPorEmpleado.fechaFinal >= :fechaFinalPeriodo) OR (m.plazasPorEmpleado.fechaFinal between :fechaInicialPeriodo AND :fechaFinalPeriodo)) ");
                    }

                    builder.Append(") ");

                    if (filtros.ContainsKey("claveDelEmpleado") && filtros.ContainsKey("claveAlEmpleado"))
                    {
                        builder.Append("AND (o.plazasPorEmpleado.empleados.clave BETWEEN :claveDelEmpleado AND :claveAlEmpleado) ");
                    }
                    else
                    {
                        if (filtros.ContainsKey("claveDelEmpleado"))
                        {
                            builder.Append("AND o.plazasPorEmpleado.empleados.clave = :claveDelEmpleado ");
                        }

                        if (filtros.ContainsKey("claveAlEmpleado"))
                        {
                            builder.Append("AND o.plazasPorEmpleado.empleados.clave = :claveAlEmpleado ");
                        }
                    }

                    query = builder.ToString();
                    q = getSession().CreateQuery(query);

                    if (filtros.ContainsKey("claveTipoNomina"))
                    {
                        q.SetParameter("claveTipoNomina", filtros["claveTipoNomina"].ToString());
                    }
                    if (filtros.ContainsKey("claveRegistroPatronal"))
                    {
                        q.SetParameter("claveRegistroPatronal", filtros["claveRegistroPatronal"].ToString());
                    }
                    if (filtros.ContainsKey("claveCentroDeCostos"))
                    {
                        q.SetParameter("claveCentroDeCostos", filtros["claveCentroDeCostos"].ToString());
                    }
                    if (filtros.ContainsKey("fechaInicialPeriodo"))
                    {

                        q.SetParameter("fechaInicialPeriodo", filtros["fechaInicialPeriodo"]);
                    }
                    if (filtros.ContainsKey("fechaFinalPeriodo"))
                    {
                        q.SetParameter("fechaFinalPeriodo", filtros["fechaFinalPeriodo"]);
                    }

                    if (filtros.ContainsKey("claveDelEmpleado"))
                    {
                        q.SetParameter("claveDelEmpleado", filtros["claveDelEmpleado"].ToString());
                    }

                    if (filtros.ContainsKey("claveAlEmpleado"))
                    {
                        q.SetParameter("claveAlEmpleado", filtros["claveAlEmpleado"].ToString());
                    }

                    //var values = q.SetResultTransformer(new DictionaryResultTransformer()).List();
                    //mensajeResultado.resultado = (values);

                }
                #endregion

                #region Type XML & PDF --
                if (tipoConsulta == 1 || tipoConsulta == 2 || tipoConsulta == 3 || tipoConsulta == 5)
                {
                    builder.Clear();
                    builder.Append("Select ");
                    builder.Append("o.plazasPorEmpleadosMov.id as id,o.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave as clave, ");
                    builder.Append(" concat(o.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.nombre, '  ', o.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.apellidoPaterno, ' ', o.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.apellidoMaterno) as nombre, ");
                    builder.Append("o.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.correoElectronico as correo, ");
                    builder.Append("o.plazasPorEmpleadosMov.centroDeCosto.descripcion as centroDeCosto, ");
                    builder.Append("o.plazasPorEmpleadosMov.departamentos.descripcion as departamento, ");
                    builder.Append("c.statusCorreo as statusCorreo, ");
                    builder.Append("c.xmlTimbrado as xmlTimbre, ");
                    if (tipoConsulta == 5)
                    {
                        builder.Append("c.acuse as acuse, ");
                    }
                    builder.Append("c.id as idCFDIRecibo ");
                    builder.Append("from CFDIEmpleado o INNER JOIN o.cfdiRecibo c WHERE 1=1 ");
                    if (tipoConsulta == 5)
                    {
                        builder.Append("AND c.statusTimbrado = 2 ");
                    }
                    else
                    {

                        builder.Append("AND c.statusTimbrado = 1 ");
                    }

                    if (filtros.ContainsKey("idTipoCorrida"))
                    {
                        builder.Append("AND o.tipoCorrida.id = :idTipoCorrida ");
                    }

                    if (filtros.ContainsKey("idTipoNomina"))
                    {
                        builder.Append("AND o.tipoNomina.id = :idTipoNomina ");
                    }

                    if (filtros.ContainsKey("keySerie"))
                    {
                        builder.Append("AND c.serie = :keySerie ");
                    }

                    if (filtros.ContainsKey("idPeriodosNomina"))
                    {
                        builder.Append("AND o.periodosNomina.id = :idPeriodosNomina ");
                    }

                    if (filtros.ContainsKey("idRegistroPatronal"))
                    {
                        builder.Append("AND o.plazasPorEmpleadosMov.plazasPorEmpleado.registroPatronal.id = :idRegistroPatronal ");
                    }

                    if (filtros.ContainsKey("idCentroDeCostos"))
                    {
                        builder.Append("AND o.plazasPorEmpleadosMov.centroDeCosto.id = :idCentroDeCostos ");
                    }

                    if (filtros.ContainsKey("iniClaveEmpleCf") && filtros.ContainsKey("finClaveEmpleCf"))
                    {
                        builder.Append("AND (o.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave BETWEEN :claveDelEmpleadoCf AND :claveAlEmpleadoCf) ");
                    }
                    else
                    {
                        if (filtros.ContainsKey("iniClaveEmpleCf"))
                        {
                            builder.Append("AND o.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave = :claveDelEmpleadoCf ");
                        }

                        if (filtros.ContainsKey("finClaveEmpleCf"))
                        {
                            builder.Append("AND o.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave = :claveAlEmpleadoCf ");
                        }
                    }

                    if (filtros.ContainsKey("statusCorreo"))
                    {
                        builder.Append("AND c.statusCorreo = :statusCorreo ");
                    }

                    query = builder.ToString();
                    q = getSession().CreateQuery(query);

                    if (filtros.ContainsKey("idTipoCorrida"))
                    {
                        q.SetParameter("idTipoCorrida", filtros["idTipoCorrida"]);
                    }

                    if (filtros.ContainsKey("idTipoNomina"))
                    {
                        q.SetParameter("idTipoNomina", filtros["idTipoNomina"]);
                    }

                    if (filtros.ContainsKey("keySerie"))
                    {
                        q.SetParameter("keySerie", filtros["keySerie"]);
                    }

                    if (filtros.ContainsKey("idPeriodosNomina"))
                    {
                        q.SetParameter("idPeriodosNomina", filtros["idPeriodosNomina"]);
                    }

                    if (filtros.ContainsKey("idRegistroPatronal"))
                    {
                        q.SetParameter("idRegistroPatronal", filtros["idRegistroPatronal"]);
                    }


                    if (filtros.ContainsKey("idCentroDeCostos"))
                    {
                        q.SetParameter("idCentroDeCostos", filtros["idCentroDeCostos"]);
                    }

                    if (filtros.ContainsKey("iniClaveEmpleCf"))
                    {
                        q.SetParameter("claveDelEmpleadoCf", filtros["iniClaveEmpleCf"]);
                    }

                    if (filtros.ContainsKey("finClaveEmpleCf"))
                    {
                        q.SetParameter("claveAlEmpleadoCf", filtros["finClaveEmpleCf"]);
                    }

                    if (filtros.ContainsKey("statusCorreo"))
                    {
                        q.SetParameter("statusCorreo", filtros["statusCorreo"]);
                    }

                    if (filtros.ContainsKey("tipoReporte"))
                    {
                        tipoReporte = Convert.ToInt32(filtros["tipoReporte"]);
                    }
                    #endregion
                }


                IList<object> listResult = (IList<object>)q.SetResultTransformer(new DictionaryResultTransformer()).List();

                if (tipoConsulta == 4)
                {
                    mensajeResultado.resultado = listResult;
                }
                else if (tipoConsulta == 5)
                {
                    mensajeResultado.resultado = decopileAcuseXML(listResult);
                }
                else
                {
                    q = null;
                    builder.Clear();
                    builder.Append("from RegimenFiscal");
                    query = builder.ToString();
                    q = getSession().CreateQuery(query);
                    var listResultRegFis = q.List();

                    Dictionary<string, object> configs = new Dictionary<string, object>();
                    configs.Add("tipoConsulta", tipoConsulta);
                    configs.Add("tipoReporte", tipoReporte);
                    configs.Add("listRegimenFiscal", listResultRegFis);

                    mensajeResultado.resultado = crumbleXML(listResult, configs);

                }
                // values = q.List<Object>();
                //.ToDictionary(grp => grp.Key, grp => grp.ToList());
                mensajeResultado.noError = (0);
                mensajeResultado.error = ("");
                getSession().Transaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getCorreoEmpleadosPorFiltros()1_Error: ").Append(ex));
                if (getSession().Transaction.IsActive)
                {
                    getSession().Transaction.Rollback();
                }
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return mensajeResultado;
        }

        public Mensaje enviarCorreoMasivo(Dictionary<string, object> filtros, ISession uuidCxn, DBContextAdapter dbContextSimple)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(uuidCxn);
                getSession().BeginTransaction();

                //Personalized mail
                StringBuilder queryMail = new StringBuilder();
                queryMail.Append("select c.SMTP, c.puerto, c.usuario,c.password,c.SSL,c.correoRemitente,c.texto,c.activo,p.asunto,p.texto from PersonalizacionCorreo p inner join p.configuracionCorreo c where c.razonesSociales.clave = :claveRazonSocial ");
                String query = queryMail.ToString() + "and p.tipoArchivo = :tipoArchivo ";
                IQuery q = getSession().CreateQuery(query);
                int tipoConsulta = -1;
                if (filtros.ContainsKey("tipoArchivo"))
                {
                    tipoConsulta = Convert.ToInt32(filtros["tipoArchivo"]);
                    q.SetParameter("tipoArchivo", tipoConsulta);
                }

                if (filtros.ContainsKey("claveRazonSocial"))
                {
                    string claveRazonSocial = (string)filtros["claveRazonSocial"];
                    q.SetString("claveRazonSocial", claveRazonSocial);
                }

                Object res = q.UniqueResult<object>();

                //General Mail
                if (res == null)
                {
                    queryMail.Remove(0, queryMail.Length);
                    queryMail.Append("select c.SMTP, c.puerto, c.usuario,c.password,c.SSL,c.correoRemitente,c.texto,c.activo,'',c.texto from ConfiguracionCorreo c where c.razonesSociales.clave = :claveRazonSocial");
                    q = getSession().CreateQuery(queryMail.ToString());
                    if (filtros.ContainsKey("claveRazonSocial"))
                    {
                        string claveRazonSocial = (string)filtros["claveRazonSocial"];
                        q.SetString("claveRazonSocial", claveRazonSocial);
                    }

                    res = q.UniqueResult<object>();
                }

                object[] mailCnf = res as object[];
                MailMessage mail = null;
                SmtpClient SmtpServer = null;

                if (filtros.ContainsKey("toSendMails"))
                {
                    List<object> toSendMails = ((Newtonsoft.Json.Linq.JArray)filtros["toSendMails"]).ToObject<List<object>>();

                    for (int i = 0; i < toSendMails.Count; i++)
                    {
                        mail = new MailMessage();
                        SmtpServer = new SmtpClient(mailCnf[0].ToString());
                        mail.From = new MailAddress(mailCnf[5].ToString());
                        mail.IsBodyHtml = true;

                        JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                        var JSONObj = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(toSendMails[i].ToString());

                        mail.To.Add(JSONObj["correo"].ToString());

                        if (tipoConsulta == 1 || tipoConsulta == 2 || tipoConsulta == 3)
                        {
                            if (tipoConsulta == 1 || tipoConsulta == 2)
                            {
                                byte[] bytesXml = Convert.FromBase64String(JSONObj["xmlTimbre"].ToString());
                                Attachment attXml = new Attachment(new MemoryStream(bytesXml), "Comprobante.xml", "application/xml");
                                mail.Attachments.Add(attXml);
                            }

                            if (tipoConsulta == 1 || tipoConsulta == 3)
                            {
                                byte[] bytePDF = Convert.FromBase64String(JSONObj["archivo"].ToString());
                                Attachment attPDF = new Attachment(new MemoryStream(bytePDF), "Comprobante.pdf", "application/pdf");
                                mail.Attachments.Add(attPDF);
                            }
                        }
                        else
                        {
                            byte[] bytes = Convert.FromBase64String(JSONObj["archivo"].ToString());
                            Attachment att = new Attachment(new MemoryStream(bytes), JSONObj["documento"].ToString());
                            mail.Attachments.Add(att);
                        }

                        string bodyMailString = "";
                        if (mailCnf[8].ToString() == "")
                        {
                            mail.Subject = "Entrega de archivo";
                            bodyMailString = mailCnf[6].ToString();
                        }
                        else
                        {
                            mail.Subject = mailCnf[8].ToString();
                            if (mailCnf[9].ToString() == "")
                            {
                                bodyMailString = mailCnf[6].ToString();
                            }
                            else
                            {
                                bodyMailString = mailCnf[9].ToString();
                            }
                        }

                        bodyMailString = bodyMailString.Replace("&amp;claveEmpleado", JSONObj["clave"].ToString());
                        bodyMailString = bodyMailString.Replace("&amp;nombreEmpleado", JSONObj["nombre"].ToString());

                        if (JSONObj.ContainsKey("periodoNomina"))
                        {
                            bodyMailString = bodyMailString.Replace("&amp;periodoNomina", JSONObj["periodoNomina"].ToString());
                            bodyMailString = bodyMailString.Replace("&amp;deLaFecha", JSONObj["deLaFecha"].ToString());
                            bodyMailString = bodyMailString.Replace("&amp;aLaFecha", JSONObj["aLaFecha"].ToString());
                        }

                        bodyMailString = bodyMailString.Replace("&amp;nombreEmpresa", JSONObj["nombreEmpresa"].ToString());

                        if (JSONObj.ContainsKey("tipoNomina"))
                        {
                            bodyMailString = bodyMailString.Replace("&amp;tipoNomina", JSONObj["tipoNomina"].ToString());
                        }
                        if (JSONObj.ContainsKey("centroDeCosto"))
                        {
                            bodyMailString = bodyMailString.Replace("&amp;centroCostos", JSONObj["centroDeCosto"].ToString());
                        }

                        if (JSONObj.ContainsKey("tipoComprobante"))
                        {
                            bodyMailString = bodyMailString.Replace("&amp;tipoComprobante", JSONObj["tipoComprobante"].ToString());
                        }

                        if (JSONObj.ContainsKey("folio"))
                        {
                            bodyMailString = bodyMailString.Replace("&amp;folioComprobante", JSONObj["folio"].ToString());
                        }

                        if (JSONObj.ContainsKey("serie"))
                        {
                            bodyMailString = bodyMailString.Replace("&amp;serieComprobante", JSONObj["serie"].ToString());
                        }

                        mail.Body = bodyMailString;
                        SmtpServer.Port = (int)mailCnf[1];
                        SmtpServer.Credentials = new System.Net.NetworkCredential(mailCnf[2].ToString(), mailCnf[3].ToString());
                        SmtpServer.EnableSsl = (bool)mailCnf[4];
                        SmtpServer.Send(mail);

                        //Update was send mail
                        int idCFDIRecibo = (int)JSONObj["idCFDIRecibo"];
                        var cfdiRecibo = new Exitosw.Payroll.Entity.entidad.cfdi.CFDIRecibo() { id = idCFDIRecibo, statusCorreo = 1 };
                        var db = dbContextSimple.context;
                        db.Database.BeginTransaction();
                        db.Set<Exitosw.Payroll.Entity.entidad.cfdi.CFDIRecibo>().Attach(cfdiRecibo);
                        db.Entry(cfdiRecibo).Property(x => x.statusCorreo).IsModified = true;
                        db.SaveChanges();
                        db.Database.CurrentTransaction.Commit();
                    }

                }
                mail.Dispose();
                mensajeResultado.resultado = true;
                mensajeResultado.noError = (0);
                mensajeResultado.error = ("");
                getSession().Transaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("enviarCorreoMasivo()1_Error: ").Append(ex));
                if (getSession().Transaction.IsActive)
                {
                    getSession().Transaction.Rollback();
                }
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return mensajeResultado;
        }

        private Object decopileAcuseXML(IList<object> cfdiList)
        {
            Comprobante comprobante;
            for (int i = 0; i < cfdiList.Count; i++)
            {
                Dictionary<string, object> reportAcuseValues = new Dictionary<string, object>();
                Dictionary<string, object> values = (Dictionary<string, object>)cfdiList[i];
                XmlDocument doc = new XmlDocument();
                var xmlDc = Encoding.UTF8.GetString((byte[])values["xmlTimbre"]);
                doc.LoadXml(xmlDc);


                XmlSerializer oXmlSerializar = new XmlSerializer(typeof(Comprobante));
                using (XmlReader writer = new XmlNodeReader(doc))
                {
                    comprobante = (Comprobante)oXmlSerializar.Deserialize(writer);
                }

                XmlDocument docAcuse = new XmlDocument();
                //Validate if acuse is null
                byte[] xmlAcuse = (byte[])values["acuse"];
                String uuid = "";
                String statusCFDI = "";
                String fechaHoraCancelacion = "";
                if (xmlAcuse != null)
                {
                    var xmlAcuseDc = Encoding.UTF8.GetString(xmlAcuse);
                    docAcuse.LoadXml(xmlAcuseDc);

                    XmlNodeList foliosNode = docAcuse.GetElementsByTagName("Folios");
                    XmlNodeList cancelCFDNode = docAcuse.GetElementsByTagName("CancelaCFDResponse");

                    /*Fecha y hora de solicitud*/

                    /*  fechaCancelacion  */
                    XmlNode nodeCancelCFDResult = FindNode(cancelCFDNode, "CancelaCFDResult");
                    XmlAttributeCollection childsCancelaCFDResult = nodeCancelCFDResult.Attributes;
                    fechaHoraCancelacion = childsCancelaCFDResult["Fecha"].InnerText;

                    XmlNode nodeUuid = FindNode(foliosNode, "UUID");
                    XmlNode nodeStatusUUID = FindNode(foliosNode, "EstatusUUID");

                    /*Foliofiscal*/
                    uuid = nodeUuid.InnerText;

                    /*EstatusCFDI*/
                    statusCFDI = nodeStatusUUID.InnerText;

                    if (statusCFDI == "201")
                    {
                        statusCFDI = "Cancelado";
                    }
                }

                /*  RFCEmisor  */
                var rfcEmisor = comprobante.Emisor.Rfc;

                /*  NombreEmisor  */
                var nombreEmisor = comprobante.Emisor.Nombre;

                /*  RFCReceptor  */
                var rfcReceptor = comprobante.Receptor.Rfc;

                /*  NombreReceptor  */
                var nombreReceptor = comprobante.Receptor.Nombre;

                var complemento = comprobante.Complemento[0];
                XmlElement[] eleComple = complemento.Any;
                var nodeTfd = eleComple.Where(item => item.Name == "tfd:TimbreFiscalDigital");

                /* NODE: tfd:TimbreFiscalDigital  */
                XmlAttributeCollection childstimbreFiscalDigital = nodeTfd.Select(item => item.Attributes).SingleOrDefault();

                /*  SelloDigitalSAT  */
                var selloDigitalSAT = childstimbreFiscalDigital["SelloSAT"].InnerText;

                reportAcuseValues.Add("RFCEmisor", rfcEmisor);
                reportAcuseValues.Add("NombreEmisor", nombreEmisor);
                reportAcuseValues.Add("RFCReceptor", rfcReceptor);
                reportAcuseValues.Add("NombreReceptor", nombreReceptor);
                reportAcuseValues.Add("FolioFiscal", uuid);
                reportAcuseValues.Add("EstatusCFDI", statusCFDI);
                reportAcuseValues.Add("SelloDigitalSAT", selloDigitalSAT);
                reportAcuseValues.Add("FechaHoraSolicitud", fechaHoraCancelacion);
                reportAcuseValues.Add("FechaHoraCancelacion", fechaHoraCancelacion);

                //Only for testing
                //return reportAcuseValues;

                Dictionary<string, object> dicAcuse = (Dictionary<string, object>)cfdiList[i];

                //GET DATA REPORT IN PDF
                StiReport reportAcuse = new StiReport();
                reportAcuse.Dictionary.Databases.Clear();
                string jsonStr = (new JavaScriptSerializer()).Serialize(reportAcuseValues);
                var ds = StiJsonToDataSetConverter.GetDataSet(jsonStr);
                ds.DataSetName = "JSON";
                DataTableCollection tables = ds.Tables;
                tables[0].TableName = "root";

                reportAcuse.RegData(ds);
                reportAcuse.Dictionary.Synchronize();
                var codebase = new Uri(Assembly.GetExecutingAssembly().CodeBase);
                var pathReport = Path.GetDirectoryName(codebase.LocalPath) + "\\Reports";
                var fullReport = pathReport + "\\ReportAcuse.mrt";
                reportAcuse.Load(fullReport);
                reportAcuse.Compile();
                reportAcuse.Render(false);
                var settings = new StiPdfExportSettings();
                var service = new StiPdfExportService();
                var stream = new MemoryStream();
                service.ExportTo(reportAcuse, stream, settings);
                var data = stream.ToArray();
                string bytesAcuseStr = Convert.ToBase64String(data);
                var pdfBase = bytesAcuseStr;

                dicAcuse.Add("documento", "Ver.pdf");
                dicAcuse.Add("archivo", bytesAcuseStr);
                cfdiList[i] = dicAcuse;
            }

            return cfdiList;
        }
        private Object crumbleXML(IList<object> cfdiList, Dictionary<string, object> configs)
        {
            #region Building XML
            Comprobante comprobante;
            for (int i = 0; i < cfdiList.Count; i++)
            {
                Dictionary<string, object> reportValues = new Dictionary<string, object>();
                var cfdiValues = cfdiList[i];
                Dictionary<string, object> values = (Dictionary<string, object>)cfdiList[i];
                XmlDocument doc = new XmlDocument();
                var xmlDc = Encoding.UTF8.GetString((byte[])values["xmlTimbre"]);
                doc.LoadXml(xmlDc);
                // doc.PreserveWhitespace = false;
                // var xmlDc = (byte[])values["xmlTimbre"];
                //MemoryStream ms = new MemoryStream((byte[])values["xmlTimbre"]);
                //doc.Load(ms);
                int tipoConsulta = -1;
                if (configs.ContainsKey("tipoConsulta"))
                {
                    tipoConsulta = (int)configs["tipoConsulta"];
                }

                if (tipoConsulta == 1 || tipoConsulta == 3)
                {
                    XmlSerializer oXmlSerializar = new XmlSerializer(typeof(Comprobante));
                    using (XmlReader writer = new XmlNodeReader(doc))
                    {
                        comprobante = (Comprobante)oXmlSerializar.Deserialize(writer);
                    }
                    /***** First part *****/
                    #region Extracted data XML
                    /*  CertificadoSAT  */
                    var complemento = comprobante.Complemento[0];
                    XmlElement[] eleComple = complemento.Any;
                    var nodeTfd = eleComple.Where(item => item.Name == "tfd:TimbreFiscalDigital");

                    /* NODE: tfd:TimbreFiscalDigital  */
                    XmlAttributeCollection childstimbreFiscalDigital = nodeTfd.Select(item => item.Attributes).SingleOrDefault();

                    /*  Versión  */
                    var version = childstimbreFiscalDigital["Version"].InnerText;

                    /*  UUID  */
                    var uuid = childstimbreFiscalDigital["UUID"].InnerText;

                    /*  NoCertificadoSAT  */
                    var noCertificadoSAT = childstimbreFiscalDigital["NoCertificadoSAT"].InnerText;

                    /*  NoCertificado  */
                    var numberCertified = comprobante.NoCertificado;

                    /*  FechaTimbrado  */
                    var fechaTimbrado = childstimbreFiscalDigital["FechaTimbrado"].InnerText;

                    /* RfcProvCertif */
                    var rfcProvCertif = childstimbreFiscalDigital["RfcProvCertif"].InnerText;

                    /* Leyenda ???????????????????????????????*/
                    var leyenda = "";

                    /* SelloCFD */
                    var selloCFD = childstimbreFiscalDigital["SelloCFD"].InnerText;

                    /*  RegimenFiscal  */
                    var regFiscal = comprobante.Emisor.RegimenFiscal;
                    /*Get Brief Regimen fiscal*/
                    if (configs.ContainsKey("listRegimenFiscal"))
                    {
                        IList listRegFiscal = (IList)configs["listRegimenFiscal"];
                        var getRegFiscal = listRegFiscal.Cast<RegimenFiscal>().FirstOrDefault(l => l.clave.Equals(regFiscal));

                        regFiscal = regFiscal + " " + getRegFiscal.descripcion;
                    }

                    /*  TipoDeComprobante  */
                    var tipComprobante = comprobante.TipoDeComprobante;

                    /*  Folio  */
                    var folio = comprobante.Folio;

                    /*  Serie  */
                    var serie = comprobante.Serie;

                    /*  FechaHoraEmision  */
                    var fechaHoraEm = comprobante.Fecha;

                    /*  MetodoPago  */
                    var metodoPago = comprobante.MetodoPago;

                    /***** Second part *****/

                    var nomina12Nomina = eleComple.Where(item => item.Name == "nomina12:Nomina");

                    /* NODE: nomina12:Nomina  */
                    XmlAttributeCollection childsNomina12Nomina = nomina12Nomina.Select(item => item.Attributes).SingleOrDefault();

                    var listChildsnodeNom12 = nomina12Nomina
                        .Select(item => item.ChildNodes).SingleOrDefault();

                    XmlNodeList xmlList = listChildsnodeNom12;
                    /* Emisor */
                    XmlNode nomina12Emisor = FindNode(xmlList, "nomina12:Emisor");
                    /* Receptor */
                    XmlNode nomina12Receptor = FindNode(xmlList, "nomina12:Receptor");

                    /*  RegistroPatronal  */
                    var regPatronal = nomina12Emisor.Attributes["RegistroPatronal"].InnerText;

                    /*  NumEmpleado  */
                    var numEmpleado = nomina12Receptor.Attributes["NumEmpleado"].InnerText;

                    /*  TipoRegimen  */
                    var tipoRegimen = nomina12Receptor.Attributes["TipoRegimen"].InnerText;

                    /*  Departamento ??????????????????????????????????????  */
                    //var departamento = nomina12Receptor.Attributes[""].InnerText;

                    /*  Puesto  */
                    var puesto = nomina12Receptor.Attributes["Puesto"].InnerText;

                    /*  RiesgoPuesto  */
                    var riesgoPuesto = nomina12Receptor.Attributes["RiesgoPuesto"].InnerText;

                    /*  TipoContrato  */
                    var tipoContrato = nomina12Receptor.Attributes["TipoContrato"].InnerText;

                    /*  Antigüedad   */
                    var antiguedad = nomina12Receptor.Attributes["Antigüedad"].InnerText;

                    /*  FechaInicioRelLaboral  */
                    var fechaInicioRelLaboral = nomina12Receptor.Attributes["FechaInicioRelLaboral"].InnerText;

                    /*  PeriodicidadPago  */
                    var periodicidadPago = nomina12Receptor.Attributes["PeriodicidadPago"].InnerText;

                    string salarioBaseCotApor = "";

                    /*  SalarioBaseCotApor  */
                    if (nomina12Receptor.Attributes != null && nomina12Receptor.Attributes["split"] != null)
                    {
                        salarioBaseCotApor = nomina12Receptor.Attributes["SalarioBaseCotApor"].InnerText;
                    }

                    /*  SalarioDiarioIntegrado  */
                    var salarioDiarioIntegrado = nomina12Receptor.Attributes["SalarioDiarioIntegrado"].InnerText;

                    /*  FechaPago  */
                    var fechaPago = childsNomina12Nomina["FechaPago"].InnerText;

                    /*  FechaInicialPago  */
                    var fechaInicialPago = childsNomina12Nomina["FechaInicialPago"].InnerText;

                    /*  FechaFinalPago  */
                    var fechaFinalPago = childsNomina12Nomina["FechaFinalPago"].InnerText;

                    /*  NumDiasPagados  */
                    var numDiasPagados = childsNomina12Nomina["NumDiasPagados"].InnerText;

                    /***** Third part *****/

                    /*  RFCEmisor  */
                    var rfcEmisor = comprobante.Emisor.Rfc;

                    /*  NombreEmisor  */
                    var nombreEmisor = comprobante.Emisor.Nombre;

                    /*  LugarExpedicion  */
                    var lugarExpedicion = comprobante.LugarExpedicion;

                    /***** Fourth part *****/

                    /*  RFCReceptor  */
                    var rfcReceptor = comprobante.Receptor.Rfc;

                    /*  NombreReceptor  */
                    var nombreReceptor = comprobante.Receptor.Nombre;

                    /*  CurpReceptor  */
                    var curpReceptor = nomina12Receptor.Attributes["Curp"].InnerText;

                    /*  NumSeguridadSocial  */
                    var numSeguridadSocial = nomina12Receptor.Attributes["NumSeguridadSocial"].InnerText;

                    /***** Percepciones *****/
                    //float totalSueldos = float.Parse(nomina12Percepciones.Attributes["TotalSueldos"].InnerText);
                    //var totalPercepciones = totalSueldos;
                    var totalPercepciones = float.Parse(childsNomina12Nomina["TotalPercepciones"].InnerText);

                    XmlNode nomina12Percepciones = FindNode(xmlList, "nomina12:Percepciones");
                    XmlNodeList listPercepciones = nomina12Percepciones.ChildNodes;

                    List<Dictionary<string, object>> listPercep = new List<Dictionary<string, object>>();
                    foreach (XmlNode node in listPercepciones)
                    {
                        XmlAttributeCollection percepData = node.Attributes;

                        Dictionary<string, object> percepValues = new Dictionary<string, object>();
                        percepValues.Add("idMaster", values["id"]);
                        percepValues.Add("TipoPercepcion", percepData["TipoPercepcion"].InnerText);
                        percepValues.Add("Clave", percepData["Clave"].InnerText);
                        percepValues.Add("Concepto", percepData["Concepto"].InnerText);
                        percepValues.Add("ImporteGravado", percepData["ImporteGravado"].InnerText);
                        percepValues.Add("ImporteExento", percepData["ImporteExento"].InnerText);
                        var impGra = float.Parse(percepData["ImporteGravado"].InnerText);
                        var impExe = float.Parse(percepData["ImporteExento"].InnerText);
                        percepValues.Add("ImporteGryExce", impGra + impExe);
                        listPercep.Add(percepValues);
                    }

                    /***** HorasExtras *****/
                    XmlNode nomina12HorasExtra = FindNode(listPercepciones, "nomina12:HorasExtra");
                    List<Dictionary<string, object>> listHorasExtra = new List<Dictionary<string, object>>();
                    if (nomina12HorasExtra != null)
                    {
                        XmlAttributeCollection hourExtraData = nomina12HorasExtra.Attributes;
                        Dictionary<string, object> hourExtraValues = new Dictionary<string, object>();
                        hourExtraValues.Add("idMaster", values["id"]);
                        hourExtraValues.Add("TipoHoras", hourExtraData["TipoHoras"].InnerText);
                        hourExtraValues.Add("Dias", hourExtraData["Dias"].InnerText);
                        hourExtraValues.Add("HorasExtra", hourExtraData["HorasExtra"].InnerText);
                        hourExtraValues.Add("ImportePagado", hourExtraData["ImportePagado"].InnerText);
                        listHorasExtra.Add(hourExtraValues);
                    }

                    /***** Deducciones *****/
                    //float totalOtrasDeducc = float.Parse(nomina12Deducciones.Attributes["TotalOtrasDeducciones"].InnerText);
                    //float totalImpuestosRetenidos = float.Parse(nomina12Deducciones.Attributes["TotalImpuestosRetenidos"].InnerText);
                    //var totalDeducciones = totalOtrasDeducc + totalImpuestosRetenidos;
                    var totalDeducciones = float.Parse(childsNomina12Nomina["TotalDeducciones"].InnerText);

                    XmlNode nomina12Deducciones = FindNode(xmlList, "nomina12:Deducciones");
                    XmlNodeList listDeducciones = nomina12Deducciones.ChildNodes;

                    List<Dictionary<string, object>> listDeducc = new List<Dictionary<string, object>>();
                    foreach (XmlNode node in listDeducciones)
                    {
                        XmlAttributeCollection deduccpData = node.Attributes;
                        Dictionary<string, object> deduccValues = new Dictionary<string, object>();
                        deduccValues.Add("idMaster", values["id"]);
                        deduccValues.Add("TipoDeduccion", deduccpData["TipoDeduccion"].InnerText);
                        deduccValues.Add("Clave", deduccpData["Clave"].InnerText);
                        deduccValues.Add("Concepto", deduccpData["Concepto"].InnerText);
                        deduccValues.Add("Importe", deduccpData["Importe"].InnerText);
                        listDeducc.Add(deduccValues);
                    }


                    /***** Otros Pagos *****/
                    XmlNode nomina12OtrosPagos = FindNode(xmlList, "nomina12:OtrosPagos");
                    List<Dictionary<string, object>> listOtrosPagos = new List<Dictionary<string, object>>();
                    float totalImporteOtrosPagos = 0;
                    if (nomina12OtrosPagos != null)
                    {
                        XmlNodeList listOtPag = nomina12OtrosPagos.ChildNodes;

                        foreach (XmlNode node in listOtPag)
                        {
                            XmlAttributeCollection otrosPagos = node.Attributes;
                            Dictionary<string, object> otrosPagosValues = new Dictionary<string, object>();
                            otrosPagosValues.Add("idMaster", values["id"]);
                            otrosPagosValues.Add("TipoOtroPago", otrosPagos["TipoOtroPago"].InnerText);
                            otrosPagosValues.Add("Clave", otrosPagos["Clave"].InnerText);
                            otrosPagosValues.Add("Concepto", otrosPagos["Concepto"].InnerText);
                            otrosPagosValues.Add("Importe", otrosPagos["Importe"].InnerText);
                            totalImporteOtrosPagos += float.Parse(otrosPagos["Importe"].InnerText);
                            listOtrosPagos.Add(otrosPagosValues);
                        }

                        totalPercepciones += totalImporteOtrosPagos;
                    }

                    /***** Incapacidades *****/
                    XmlNode nomina12Incapacidades = FindNode(xmlList, "nomina12:Incapacidades");
                    List<Dictionary<string, object>> listIncapacidades = new List<Dictionary<string, object>>();
                    float totalImporteIncapacidades = 0;
                    if (nomina12Incapacidades != null)
                    {
                        XmlNodeList listIncap = nomina12Incapacidades.ChildNodes;
                        foreach (XmlNode node in listIncap)
                        {
                            XmlAttributeCollection incapacidades = node.Attributes;
                            Dictionary<string, object> otrosPagosValues = new Dictionary<string, object>();
                            otrosPagosValues.Add("idMaster", values["id"]);
                            otrosPagosValues.Add("TipoIncapacidad", incapacidades["TipoIncapacidad"].InnerText);
                            otrosPagosValues.Add("DiasIncapacidad", incapacidades["DiasIncapacidad"].InnerText);
                            otrosPagosValues.Add("ImporteMonetario", incapacidades["ImporteMonetario"].InnerText);
                            totalImporteIncapacidades += float.Parse(incapacidades["ImporteMonetario"].InnerText);
                            listIncapacidades.Add(otrosPagosValues);
                        }

                        // totalDeducciones += totalImporteIncapacidades;
                    }

                    /*  NetoAPagar  */
                    var netoAPagar = comprobante.Total;

                    /*  ImporteALetras  */
                    var importeALetras = Utilerias.convertirNumerosALetras(netoAPagar.ToString(), false);

                    /*  CadenaOriginalSAT  */  //Leyenda??
                    //Leyenda
                    string cadenaOriginalSAT = "||" + version + "|" + uuid + "|" + fechaTimbrado + "|" + rfcProvCertif;
                    if (leyenda != "")
                    {
                        cadenaOriginalSAT += "|" + leyenda + "|" + selloCFD + "|" + noCertificadoSAT + "||";
                    }
                    else
                    {
                        cadenaOriginalSAT += "|" + selloCFD + "|" + noCertificadoSAT + "||";
                    }

                    /*  SelloDigitalCFDI  */
                    var selloDigitalCFDI = comprobante.Sello;

                    /*  SelloDigitalSAT  */
                    var selloDigitalSAT = childstimbreFiscalDigital["SelloSAT"].InnerText;

                    /*  QRCode  */
                    var shortSelloCFD = selloCFD.Substring(selloCFD.Length - 8);
                    var dataQr = "https://verificacfdi.facturaelectronica.sat.gob.mx/default.aspx?" + "id=" + uuid + "&re=" + rfcEmisor + "&rr=" + rfcReceptor + "&tt=" + netoAPagar + "&fe=" + shortSelloCFD;

                    #endregion

                    reportValues.Add("id", values["id"]);
                    reportValues.Add("clave", values["clave"]);
                    reportValues.Add("NoCertificadoSAT", "\u200B" + noCertificadoSAT);
                    reportValues.Add("NoCertificado", "\u200B" + numberCertified);
                    reportValues.Add("FechaTimbrado", fechaTimbrado);
                    reportValues.Add("RegimenFiscal", regFiscal);
                    reportValues.Add("TipoDeComprobante", tipComprobante);
                    reportValues.Add("Folio", folio);
                    reportValues.Add("Serie", serie);
                    reportValues.Add("FechaHoraEmision", fechaHoraEm);
                    reportValues.Add("MetodoPago", metodoPago);
                    reportValues.Add("RegistroPatronal", regPatronal);
                    reportValues.Add("NumEmpleado", numEmpleado);
                    reportValues.Add("TipoRegimen", tipoRegimen);
                    reportValues.Add("Departamento", values["departamento"]);
                    reportValues.Add("Puesto", puesto);
                    reportValues.Add("RiesgoPuesto", riesgoPuesto);
                    reportValues.Add("TipoContrato", tipoContrato);
                    reportValues.Add("Antiguedad", antiguedad);
                    reportValues.Add("FechaInicioRelLaboral", fechaInicioRelLaboral);
                    reportValues.Add("PeriodicidadPago", periodicidadPago);
                    reportValues.Add("SalarioBaseCotApor", salarioBaseCotApor);
                    reportValues.Add("SalarioDiarioIntegrado", salarioDiarioIntegrado);
                    reportValues.Add("FechaPago", fechaPago);
                    reportValues.Add("FechaInicialPago", fechaInicialPago);
                    reportValues.Add("FechaFinalPago", fechaFinalPago);
                    reportValues.Add("NumDiasPagados", numDiasPagados);
                    reportValues.Add("RFCEmisor", rfcEmisor);
                    reportValues.Add("NombreEmisor", nombreEmisor);
                    reportValues.Add("LugarExpedicion", lugarExpedicion);
                    reportValues.Add("RFCReceptor", rfcReceptor);
                    reportValues.Add("NombreReceptor", nombreReceptor);
                    reportValues.Add("CurpReceptor", curpReceptor);
                    reportValues.Add("NumSeguridadSocial", numSeguridadSocial);
                    reportValues.Add("Percepciones", listPercep);
                    reportValues.Add("TotalPercepciones", totalPercepciones);
                    reportValues.Add("HorasExtras", listHorasExtra);
                    reportValues.Add("Deducciones", listDeducc);
                    reportValues.Add("TotalDeducciones", totalDeducciones);
                    reportValues.Add("OtrosPagos", listOtrosPagos);
                    reportValues.Add("TotalOtrosPagos", totalImporteOtrosPagos);
                    reportValues.Add("Incapacidades", listIncapacidades);
                    reportValues.Add("TotalIncapacidades", totalImporteIncapacidades);
                    reportValues.Add("NetoAPagar", netoAPagar);
                    reportValues.Add("ImporteALetras", importeALetras);
                    reportValues.Add("CadenaOriginalSAT", cadenaOriginalSAT);
                    reportValues.Add("SelloDigitalCFDI", selloDigitalCFDI);
                    reportValues.Add("SelloDigitalSAT", selloDigitalSAT);
                    reportValues.Add("UUID", uuid);
                    reportValues.Add("DataQr", dataQr);

                    //Only for testing
                    //return reportValues;
                }
                #endregion

                Dictionary<string, object> dic = (Dictionary<string, object>)cfdiList[i];
                #region AddPDF
                if (tipoConsulta == 1 || tipoConsulta == 3)
                {
                    //GET DATA REPORT IN PDF
                    StiReport report = new StiReport();
                    report.Dictionary.Databases.Clear();
                    //report["isOtrosPagos"] = "1";

                    // GetByName("tituloEmpresa").valueObject = titulosRep.tituloEmpresa;


                    string jsonStr = (new JavaScriptSerializer()).Serialize(reportValues);
                    var ds = StiJsonToDataSetConverter.GetDataSet(jsonStr);
                    ds.DataSetName = "JSON";
                    DataTableCollection tables = ds.Tables;
                    tables[0].TableName = "root";

                    if (tables.Contains("Percepciones"))
                    {
                        var pos = tables.IndexOf("Percepciones");
                        tables[pos].TableName = "root_Percepciones";
                    }

                    if (tables.Contains("Deducciones"))
                    {
                        var pos = tables.IndexOf("Deducciones");
                        tables[pos].TableName = "root_Deducciones";
                    }

                    if (tables.Contains("OtrosPagos"))
                    {
                        var pos = tables.IndexOf("OtrosPagos");
                        tables[pos].TableName = "root_OtrosPagos";

                    }

                    if (tables.Contains("Incapacidades"))
                    {
                        var pos = tables.IndexOf("Incapacidades");
                        tables[pos].TableName = "root_Incapacidades";
                    }

                    if (tables.Contains("HorasExtras"))
                    {
                        var pos = tables.IndexOf("HorasExtras");
                        tables[pos].TableName = "root_HorasExtras";
                    }

                    report.RegData(ds);
                    report.Dictionary.Synchronize();
                    //var deee = report.Dictionary.GetDataSet("JSON");
                    var codebase = new Uri(Assembly.GetExecutingAssembly().CodeBase);
                    var pathReport = Path.GetDirectoryName(codebase.LocalPath) + "\\Reports";
                    var fullReport = pathReport + "\\Report.mrt";

                    int typeReport = -1;
                    if (configs.ContainsKey("tipoReporte"))
                    {
                        typeReport = (int)configs["tipoReporte"];

                    }
                    if (typeReport == 1)
                    {
                        fullReport = pathReport + "\\Report.mrt";
                    }
                    else if (typeReport == 2)
                    {
                        fullReport = pathReport + "\\ReportAsimilados.mrt";
                    }

                    report.Load(fullReport);

                    if (reportValues.ContainsKey("TotalOtrosPagos"))
                    {
                        StiVariable variableOtPag = new StiVariable();
                        variableOtPag.Name = "isOtrosPagos";
                        variableOtPag.Alias = "isOtrosPagos";
                        variableOtPag.Type = typeof(String);

                        float totalOtrosPagos = (float)reportValues["TotalOtrosPagos"];
                        if (totalOtrosPagos > 0)
                        {
                            variableOtPag.Value = "1";
                        }
                        else
                        {
                            variableOtPag.Value = "0";
                        }
                        //report.Dictionary.Variables.Add(variable);
                        report.Dictionary.Variables["isOtrosPagos"] = variableOtPag;
                    }

                    if (reportValues.ContainsKey("TotalIncapacidades"))
                    {
                        StiVariable variableInc = new StiVariable();
                        variableInc.Name = "isIncapacidades";
                        variableInc.Alias = "isIncapacidades";
                        variableInc.Type = typeof(String);

                        float totalIncapacidades = (float)reportValues["TotalIncapacidades"];
                        if (totalIncapacidades > 0)
                        {
                            variableInc.Value = "1";
                        }
                        else
                        {
                            variableInc.Value = "0";
                        }
                        report.Dictionary.Variables["isIncapacidades"] = variableInc;
                    }

                    if (reportValues.ContainsKey("HorasExtras"))
                    {
                        List<Dictionary<string, object>> listHorasExtra = (List<Dictionary<string, object>>)reportValues["HorasExtras"];

                        StiVariable variableHorExtra = new StiVariable();
                        variableHorExtra.Name = "isHorasExtras";
                        variableHorExtra.Alias = "isHorasExtras";
                        variableHorExtra.Type = typeof(String);

                        if (listHorasExtra.Count > 0)
                        {
                            variableHorExtra.Value = "1";
                        }
                        else
                        {
                            variableHorExtra.Value = "0";
                        }
                        report.Dictionary.Variables["isHorasExtras"] = variableHorExtra;
                    }

                    report.Compile();
                    report.Render(false);
                    var settings = new StiPdfExportSettings();
                    var service = new StiPdfExportService();
                    var stream = new MemoryStream();
                    service.ExportTo(report, stream, settings);
                    var data = stream.ToArray();
                    string bytesStr = Convert.ToBase64String(data);
                    var pdfBase = bytesStr;
                    dic.Add("tipoComprobante", reportValues["TipoDeComprobante"]);
                    dic.Add("folio", reportValues["Folio"]);
                    dic.Add("serie", reportValues["Serie"]);
                    dic.Add("documento", "Ver.pdf");
                    dic.Add("archivo", bytesStr);
                    cfdiList[i] = dic;
                }
                #endregion

                #region AddXML
                if (tipoConsulta == 1 || tipoConsulta == 2)
                {
                    StringWriter sw = new StringWriter();
                    XmlTextWriter xw = new XmlTextWriter(sw);
                    xw.Formatting = Formatting.Indented;
                    doc.WriteTo(xw);
                    String XmlString = sw.ToString();

                    byte[] bytes = null;
                    using (var ms = new MemoryStream())
                    {
                        TextWriter tw = new StreamWriter(ms, System.Text.Encoding.UTF8);
                        tw.Write(XmlString);
                        tw.Flush();
                        ms.Position = 0;
                        bytes = ms.ToArray();
                    }

                    string base64 = Convert.ToBase64String(bytes);
                    dic.Add("documentoXML", "Ver.xml");
                    dic.Add("archivoXML", base64);
                }
                #endregion

            }

            return cfdiList;

        }
        private XmlNode FindNode(XmlNodeList list, string nodeName)
        {
            if (list.Count > 0)
            {
                foreach (XmlNode node in list)
                {
                    if (node.Name.Equals(nodeName)) return node;
                    if (node.HasChildNodes)
                    {
                        XmlNode nodeFound = FindNode(node.ChildNodes, nodeName);
                        if (nodeFound != null)
                            return nodeFound;
                    }
                }
            }
            return null;
        }

    }
}
