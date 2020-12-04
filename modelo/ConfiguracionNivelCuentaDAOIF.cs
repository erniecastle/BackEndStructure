

using Exitosw.Payroll.Core.util;
/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface ConfiguracionNivelCuentaDAOIF para llamados a metodos de Entity
* -----------------------------------------------------------------------------
* MODIFICACIONES:
* -----------------------------------------------------------------------------
* Clave: 
* Autor: 
* Fecha:
* Descripción: 
* -----------------------------------------------------------------------------
*/
using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Entity.entidad.contabilidad;
using System.Collections.Generic;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public interface ConfiguracionNivelCuentaDAOIF : IGenericRepository<ConfiguracionNivelCuenta>
    {
        Mensaje agregar(ConfiguracionNivelCuenta entity, DBContextAdapter dbContext);

        Mensaje modificar(ConfiguracionNivelCuenta entity, DBContextAdapter dbContext);

        Mensaje eliminar(ConfiguracionNivelCuenta entity, DBContextAdapter dbContext);

        Mensaje saveDeleteConfiguracionNivelCuenta(ConfiguracionNivelCuenta entity, List<decimal> eliminados, DBContextAdapter dbContext);

        Mensaje getConfiguracionNivelCuentaUnico(DBContextAdapter dbContext);
    }
}
