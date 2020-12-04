
using Exitosw.Payroll.Core.util;
/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface AguinaldoPagosDAOIF para llamados a metodos de Entity
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
    public interface AguinaldoPagosDAOIF : IGenericRepository<AguinaldoPagos>
    {
        Mensaje agregar(AguinaldoPagos entity, DBContextAdapter dbContext);

        Mensaje modificar(AguinaldoPagos entity, DBContextAdapter dbContext);

        Mensaje eliminar(AguinaldoPagos entity, DBContextAdapter dbContext);

        Mensaje getAllAguinaldoPagos(DBContextAdapter dbContext);
    }
}
