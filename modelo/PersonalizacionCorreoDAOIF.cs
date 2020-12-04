/**
* @author: Ernesto Castillo
* Fecha de Creación: 30/04/2020
* Compañía: Exito
* Descripción del programa: Interface PersonalizacionCorreoDAOIF para llamados a metodos de Entity
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
    public interface PersonalizacionCorreoDAOIF : IGenericRepository<PersonalizacionCorreo>
    {
        Mensaje agregar(PersonalizacionCorreo entity, DBContextAdapter dbContext);

        Mensaje modificar(PersonalizacionCorreo entity, decimal? idRazonSocial, DBContextAdapter dbContext);

        Mensaje eliminar(PersonalizacionCorreo entity, DBContextAdapter dbContext);

        Mensaje getPersonalizacionCorreo(decimal? idRazonSocial, int tipoDeArchivo, DBContextAdapter dbContext);
    }
}
