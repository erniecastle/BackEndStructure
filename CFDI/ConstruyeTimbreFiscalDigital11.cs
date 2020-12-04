using Exitosw.Payroll.Core.CFDI.Timbrado;
using Exitosw.Payroll.Core.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Exitosw.Payroll.Core.CFDI
{
    public class ConstruyeTimbreFiscalDigital11
    {
        string ruta;
        public ConstruyeTimbreFiscalDigital11()
        {
            
        }
        public ConstruyeTimbreFiscalDigital11(string ruta)
        {
            this.ruta = ruta;
        }
        public String getCadenaOriginalTimbrado(XmlNode nodo)
        {
            string cadenaSalida="";
            try
            {
                string ruta2 = ruta + @"\CFDI\prueba.xml";
                System.IO.File.WriteAllText(ruta2, nodo.OuterXml);
                cadenaSalida = getCadenaoriginal(ruta2);
            }
            catch (Exception ex)
            {
                //utilSat.bitacora(ex.getMessage());
                //System.err.println(concatena.delete(0, concatena.length()).append(msgError).append("getCadenaOriginalTimbrado()1_Error: ").append(ex));
                return null;
            }
            return cadenaSalida;
        }

        private string getCadenaoriginal(string node)
        {
            string cadenaOriginal = "";

            string pathxsl = ruta + @"\recursos\xsltLocal\cadenaoriginal_3_3.xslt";
            System.Xml.Xsl.XslCompiledTransform transformador = new System.Xml.Xsl.XslCompiledTransform(true);
            transformador.Load(pathxsl);

            using (StringWriter sw = new StringWriter()) {
               
                using (XmlWriter xwo = XmlWriter.Create(sw, transformador.OutputSettings))
                {

                    transformador.Transform(node, xwo);
                    cadenaOriginal = sw.ToString();
                }
            }
            return cadenaOriginal;
        }
        public XmlElement generaNodoTimbreFiscal(DatosTimbreFiscalDigital datosTimbreFiscalDigital)
        {

            try
            {
                TimbreFiscalDigital tfc = contruyeTimbreFiscalDigital(datosTimbreFiscalDigital);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(TimbreFiscalDigital));
                XmlDocument documento = new XmlDocument();
                //XmlNode a = new XmlNode();
                string doc = "";
                using (var sww = new StringWriterWithEncoding(Encoding.UTF8))
                {

                    using (XmlWriter writter = XmlWriter.Create(sww))
                    {

                        xmlSerializer.Serialize(writter, tfc);
                        doc = sww.ToString();
                        documento.LoadXml(doc);
                    }

                }

                return documento.DocumentElement;

            }
            catch (Exception ex)
            {

                return null;
            }

        }

      
        public TimbreFiscalDigital contruyeTimbreFiscalDigital(DatosTimbreFiscalDigital datosTimbreFiscalDigital)
        {
            TimbreFiscalDigital timbreFiscalDigital = new TimbreFiscalDigital();
            timbreFiscalDigital.FechaTimbrado = (datosTimbreFiscalDigital.fechaTimbrado);
            timbreFiscalDigital.NoCertificadoSAT = (datosTimbreFiscalDigital.noCertificadoSAT);
            timbreFiscalDigital.SelloCFD = (datosTimbreFiscalDigital.selloCFD);
            timbreFiscalDigital.SelloSAT = (datosTimbreFiscalDigital.selloSAT);
            timbreFiscalDigital.UUID = (datosTimbreFiscalDigital.uuid);
            timbreFiscalDigital.Version = (datosTimbreFiscalDigital.version);
            return timbreFiscalDigital;
        }
    }
}
