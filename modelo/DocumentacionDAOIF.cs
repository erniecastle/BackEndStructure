/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface DocumentacionDAOIF para llamados a metodos de Entity
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
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public interface DocumentacionDAOIF
    {
        Mensaje agregar(Documentacion entity, DBContextAdapter dbContext);

        Mensaje modificar(Documentacion entity, DBContextAdapter dbContext);

        Mensaje eliminar(Documentacion entity, DBContextAdapter dbContext);

        Mensaje getDocumentacionPorIDEmpleado(decimal id_empleados, DBContextAdapter dbContext);
    }
}
