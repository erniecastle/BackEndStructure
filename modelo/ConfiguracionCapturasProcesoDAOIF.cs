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
    public interface ConfiguracionCapturasProcesoDAOIF : IGenericRepository<ConfiguracionCapturasProceso>
    {
        Mensaje agregar(ConfiguracionCapturasProceso entity, DBContextAdapter uuidCxn);

        Mensaje actualizar(ConfiguracionCapturasProceso entity, DBContextAdapter uuidCxn);

        Mensaje eliminar(ConfiguracionCapturasProceso entity, DBContextAdapter uuidCxn);
        Mensaje getAllCapturasProceso(DBContextAdapter uuidCxn);
        Mensaje getConfiguracionCapturasProcesoPorClave(string clave, DBContextAdapter uuidCxn);
        Mensaje getMaxConfiguracionCapturasProceso(DBContextAdapter uuidCxn);
        Mensaje getConfiguracionCapturasProcesoPorRango(int[] values, DBContextAdapter uuidCxn);

        Mensaje getTotalRegistrosProceso(DBContextAdapter uuidCxn);
    }
}
