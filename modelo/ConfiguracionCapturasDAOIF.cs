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
    public interface ConfiguracionCapturasDAOIF : IGenericRepository<ConfiguracionCapturas>
    {
        Mensaje agregar(ConfiguracionCapturas entity, DBContextAdapter dbContextMaster);

        Mensaje actualizar(ConfiguracionCapturas entity, DBContextAdapter dbContextMaster);

        Mensaje eliminar(ConfiguracionCapturas entity, DBContextAdapter dbContextMaster);
        Mensaje getAllCapturas(DBContextAdapter dbContextMaster);
        Mensaje getConfiguracionCapturasPorClave(string clave, DBContextAdapter dbContextMaster);
        Mensaje getMaxConfiguracionCapturas(DBContextAdapter dbContext);
        Mensaje getConfiguracionCapturasPorRango(int[] values, DBContextAdapter dbContextMaster);

        Mensaje getTotalRegistros(DBContextAdapter dbContextMaster);

    }
}
