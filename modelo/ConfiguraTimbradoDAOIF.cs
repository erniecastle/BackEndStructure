
/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface ConfiguraTimbradoDAOIF para llamados a metodos de Entity
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
using System.Collections.Generic;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public interface ConfiguraTimbradoDAOIF : IGenericRepository<ConfiguraTimbrado>
    {
        Mensaje agregar(ConfiguraTimbrado entity, DBContextAdapter dbContext);

        Mensaje modificar(ConfiguraTimbrado entity, DBContextAdapter dbContext);

        Mensaje eliminar(ConfiguraTimbrado entity, DBContextAdapter dbContext);

        Mensaje getAllConfiguraTimbrado(DBContextAdapter dbContext);

        Mensaje getPorIdConfiguraTimbrado(decimal? id ,DBContextAdapter dbContext);

        Mensaje saveConfiguraTimbrado(List<ConfiguraTimbrado> entitysCambios, DBContextAdapter dbContext);

        Mensaje getConfiguraTimbradoPrincipal(ConfiguraTimbrado configuraTimbrado, DBContextAdapter dbContext);
    }
}
