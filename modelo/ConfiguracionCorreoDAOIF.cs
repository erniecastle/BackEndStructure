/**
* @author: Ernesto Castillo
* Fecha de Creación: 29/04/2020
* Compañía: Exito
* Descripción del programa: Interface ConfiguracionCorreoDAOIF para llamados a metodos de Entity
* -----------------------------------------------------------------------------
* MODIFICACIONES:
* -----------------------------------------------------------------------------
* Clave: 
* Autor: 
* Fecha:
* Descripción: 
* -----------------------------------------------------------------------------
*/

using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.Entity.entidad;

namespace Exitosw.Payroll.Core.modelo
{
    public interface ConfiguracionCorreoDAOIF : IGenericRepository<ConfiguracionCorreo>
    {
        Mensaje agregar(ConfiguracionCorreo entity, DBContextAdapter dbContext);

        Mensaje modificar(ConfiguracionCorreo entity, DBContextAdapter dbContext);

        Mensaje eliminar(ConfiguracionCorreo entity, DBContextAdapter dbContext);

        Mensaje enviarCoreo(ConfiguracionCorreo entity, DBContextAdapter dbContext);

        Mensaje getConfiguracionCorreo(decimal? idRazonSocial, DBContextAdapter dbContext);
    }
}
