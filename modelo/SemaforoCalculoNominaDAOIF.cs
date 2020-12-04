
using Exitosw.Payroll.Core.util;
/**
* @author: Daniel Ruelas
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface SemaforoCalculoNominaDAOIF para llamados a metodos de Entity
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
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public interface SemaforoCalculoNominaDAOIF : IGenericRepository<SemaforoCalculoNomina>
    {
        Mensaje agregar(SemaforoCalculoNomina entity, DBContextAdapter dbContext);

        Mensaje eliminar(SemaforoCalculoNomina entity, DBContextAdapter dbContext);
    }
}
