using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.Entity.entidad;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exitosw.Payroll.Core.modelo
{
    public interface FormasCapturaDAOIF
    {
        Mensaje getDetalleOrigen(string clase, DBContextAdapter dbContextMaster);

        Mensaje getOrigesDatosALL(DBContextAdapter dbContextMaster);

        Mensaje getDetallaOrigenesDatos(decimal idorigen, DBContextAdapter dbContextMaster);

        Mensaje getCamposOrigenDatos(decimal idOrigen, bool toDictionary, DBContextAdapter dbContextMaster);
    }
}
