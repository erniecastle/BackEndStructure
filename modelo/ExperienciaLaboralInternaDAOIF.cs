using Exitosw.Payroll.Core.util;
/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface ExperienciaLaboralInternaDAOIF para llamados a metodos de Entity
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


namespace Exitosw.Payroll.Core.modelo
{
    public interface ExperienciaLaboralInternaDAOIF
    {
        Mensaje agregar(ExperienciaLaboralInterna entity, DBContextAdapter dbContext);

        Mensaje actualizar(ExperienciaLaboralInterna entity, DBContextAdapter dbContext);

        Mensaje eliminar(ExperienciaLaboralInterna entity, DBContextAdapter dbContext);

        Mensaje getExperienciaLaboralInternaPorIDEmpleado(decimal id_empleado, DBContextAdapter dbContext);

    }
}
