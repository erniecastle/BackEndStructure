/**
* @author: Ismael Pineda
* Fecha de Creación: 24/07/2020
* Compañía: Macropro
* Descripción del programa: Interface SeriesDAOIF para llamados a metodos de Entity
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
    public interface SeriesDAOIF
    {
        Mensaje agregar(Series entity, DBContextAdapter dbContext);

        Mensaje modificar(Series entity, DBContextAdapter dbContext);

        Mensaje eliminar(Series entity, DBContextAdapter dbContext);

        Mensaje getAllSeries(DBContextAdapter dbContext);

        Mensaje getPorIdSeries(decimal? idSeries, DBContextAdapter dbContext);

    }
}
