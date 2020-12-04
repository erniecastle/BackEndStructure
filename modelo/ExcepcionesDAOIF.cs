using Exitosw.Payroll.Core.util;
/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface ExcepcionesDAOIF para llamados a metodos de Entity
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
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public interface ExcepcionesDAOIF
    {
        Mensaje agregar(Excepciones entity, DBContextAdapter dbContext);

        Mensaje agregarExcepciones(List<Excepciones> entitysCambios, DBContextAdapter dbContext);

        Mensaje getAllExcepciones(DBContextAdapter dbContext);

        Mensaje getExcepcionPorId(decimal? idExcepcion, DBContextAdapter dbContext);
    }
}
