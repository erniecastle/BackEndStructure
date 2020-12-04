/**
* @author: Ernesto Castillo
* Fecha de Creación: 07/10/2020
* Compañía: Macropro
* Descripción del programa: Clase CausaDeBajaDAOIF para llamados a metodos de Entity
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
    public interface CausaDeBajaDAOIF : IGenericRepository<CausaDeBaja>
    {
        Mensaje agregar(CausaDeBaja entity, DBContextAdapter dbContext);

        Mensaje modificar(CausaDeBaja entity, DBContextAdapter dbContext);

        Mensaje eliminar(CausaDeBaja entity, DBContextAdapter dbContext);

        Mensaje getAllCausaDeBaja(DBContextAdapter dbContext);
    }
}
