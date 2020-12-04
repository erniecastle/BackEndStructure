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
    public interface CamposOrigenDatosDAOIF
    {
        Mensaje agregar(CamposOrigenDatos entity, List<DetalleOrigenDatos> detalles, int[] deleteDetalles, DBContextAdapter dbContextMaster);

        Mensaje actualizar(CamposOrigenDatos entity, DBContextAdapter dbContextMaster);

        Mensaje eliminar(CamposOrigenDatos entity, DBContextAdapter dbContextMaster);

        Mensaje getCamposPorOrigen(string fuente, DBContextAdapter dbContextMaster);

        Mensaje getOrigen(string fuente, DBContextAdapter dbContextMaster);

        Mensaje getDetallesOrigen(string fuente, DBContextAdapter dbContextMaster);

        Mensaje getCampoPorID(decimal? idCampo, DBContextAdapter dbContextMaster);

    }
}
