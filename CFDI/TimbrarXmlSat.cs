using Exitosw.Payroll.Core.CFDI.PACS;
using Exitosw.Payroll.Core.CFDI.Timbrado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.CFDI
{
    public class TimbrarXmlSat
    {
        List<object> datosTimbrados = null;
        public string error { get; set; }
        public List<object> generaTimbres(InfoATimbrar infoATimbrar , DBContextAdapter dbContext)
        {
            datosTimbrados = null;
            error = "";
            try
            {
                if (infoATimbrar.urlWebServices.Contains("solucionfactible.com"))
                {
                    SolucionFactible solucionFact = new SolucionFactible();
                    if (infoATimbrar.tipoOperacion == TipoOperacionWS.TIMBRAR)
                    {
                        datosTimbrados = solucionFact.Timbrar(infoATimbrar);
                    }
                    else if (infoATimbrar.tipoOperacion == TipoOperacionWS.CANCELAR)
                    {
                        datosTimbrados = solucionFact.cancelarAcuse(infoATimbrar, dbContext);
                    }
                    else if (infoATimbrar.tipoOperacion == TipoOperacionWS.CANCELARSTATUS)
                    {
                        datosTimbrados = solucionFact.CancelarAcuseStatus(infoATimbrar,  dbContext);
                    }
                    error = solucionFact.error;
                }
            }
            catch (Exception e)
            {
                if (error != "")
                { 
                    error = e.Message; 
                }
                
                // System.out.println(e.getMessage());
                // Utilerias.bitacora(e.getMessage());
            }
            return datosTimbrados == null ? new List<object>() : datosTimbrados;
        }
    }
}
