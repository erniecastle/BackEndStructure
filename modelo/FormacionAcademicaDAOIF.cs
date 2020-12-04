using Exitosw.Payroll.Core.util;
/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface FormacionEconomicaDAOIF para llamados a metodos de Entity
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
    public interface FormacionAcademicaDAOIF
    {
        Mensaje agregar(FormacionAcademica entity, DBContextAdapter dbContext);

        Mensaje modificar(FormacionAcademica entity, DBContextAdapter dbContext);

        Mensaje eliminar(FormacionAcademica entity, DBContextAdapter dbContext);

        Mensaje getFormacionAcademicaPorIDEmpleado(decimal id_empleado, DBContextAdapter dbContext);
    }
}
