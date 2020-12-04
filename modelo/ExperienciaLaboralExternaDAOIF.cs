using Exitosw.Payroll.Core.util;
/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface ExperienciaLaboralExternaDAOIF para llamados a metodos de Entity
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
    public interface ExperienciaLaboralExternaDAOIF
    {
        Mensaje agregar(ExperienciaLaboralExterna entity, DBContextAdapter dbContext);

        Mensaje actualizar(ExperienciaLaboralExterna entity, DBContextAdapter dbContext);

        Mensaje eliminar(ExperienciaLaboralExterna entity, DBContextAdapter dbContext);

        Mensaje getExperienciaLaboralExternaPorIDEmpleado(decimal id_empleado, DBContextAdapter dbContext);
    }
}

