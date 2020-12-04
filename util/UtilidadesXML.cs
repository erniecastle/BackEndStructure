using Exitosw.Payroll.Entity.entidad;
using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Exitosw.Payroll.Core.util
{
    public class UtilidadesXML
    {
        public static int ERROR_XML = 0;
        public static Mensaje mensajeError;

        public static XDocument convierteBytesToXML(byte[] xmlString)
        {
            inicializaVariableMensaje();
            XDocument doc = new XDocument();
            try
            {
                ERROR_XML = 0;
                string s_unicode2 = Encoding.UTF8.GetString(xmlString);
                doc= XDocument.Parse(s_unicode2);

            }
            catch (XmlSchemaValidationException ex)
            {
                ERROR_XML = 1;
                mensajeError.noError = ERROR_XML;
                mensajeError.error = ex.GetBaseException().ToString();
                mensajeError.resultado = null;
                System.Diagnostics.Debug.WriteLine(ex);
            }
            catch (Exception ex)
            {
                ERROR_XML = 2;
                mensajeError.noError = ERROR_XML;
                mensajeError.error = ex.GetBaseException().ToString();
                mensajeError.resultado = null;
                System.Diagnostics.Debug.WriteLine(ex);
            }

            return doc;
        }

        public static Object[,] extraeValoresNodos(XDocument doc)
        {
            Object[,] valores = null;
            try
            {
                ERROR_XML = 0;
                var valoresDatos = from a in doc.Descendants("dato")
                                   select a;
                XElement valorDato = valoresDatos.FirstOrDefault();
                int fila = valoresDatos.Count();
                int columna = valorDato.Elements().Count();
                valores = new Object[fila, columna];
                for (int i = 0; i < fila; i++)
                {

                    XElement itemDato = valoresDatos.ElementAt(i);

                    for (int j = 0; j < columna; j++)
                    {
                        valores[i, j] = itemDato.Elements().ToList()[j].Value;

                    }

                }

            }
            catch (Exception ex)
            {
                ERROR_XML = 2;
                mensajeError.noError = ERROR_XML;
                mensajeError.error = ex.GetBaseException().ToString();
                mensajeError.resultado = null;
                System.Diagnostics.Debug.WriteLine(ex);
            }
            return valores;
        }

        private static void inicializaVariableMensaje()
        {
            if (mensajeError == null)
            {
                mensajeError = new Mensaje();

            }
            mensajeError.resultado = null;
            mensajeError.noError = 0;
            mensajeError.error = "";
        }
    }
}