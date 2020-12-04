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
    public interface ProcesosOrigenDAOIF: IGenericRepository<ProcesoOrigen>
    {
        Mensaje agregar(ProcesoOrigen entity, DBContextAdapter uuidCxn);

        Mensaje actualizar(ProcesoOrigen entity, DBContextAdapter uuidCxn);

        Mensaje eliminar(ProcesoOrigen entity, DBContextAdapter uuidCxn);

        Mensaje getAllProcesoOrigen(DBContextAdapter uuidCxn);

        Mensaje getPorClaveProcesoOrigen(string clave, DBContextAdapter uuidCxn);

        Mensaje getPorIdProcesoOrigen(decimal clave, DBContextAdapter uuidCxn);

        Mensaje saveDetallesProcesoOrigens(ProcesoOrigen entity, decimal[] deleteParam, decimal[] deleteAcciones, DBContextAdapter uuidCxn);

        Mensaje getProcesoOrigenPorRango(int[] values, DBContextAdapter uuidCxn);

        Mensaje getMaxProcesoOrigen(DBContextAdapter uuidCxn);

        Mensaje getParametrosProcesoOrigen(decimal idProceso, bool toDictionary, DBContextAdapter uuidCxn);

        Mensaje getParametrosProcesoOrigenID(decimal idProceso, bool toDictionary, DBContextAdapter uuidCxn);

        Mensaje getAccionesProcesoOrigenID(decimal idProceso, DBContextAdapter uuidCxn);
    }
}
