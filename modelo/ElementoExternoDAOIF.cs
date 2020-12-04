/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface ElementoExternoDAOIF para llamados a metodos de Entity
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
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public interface ElementoExternoDAOIF : IGenericRepository<ElementoExterno>
    {
        Mensaje getElementoExAll(DBContextAdapter dbContext);

        Mensaje agregar(ElementoExterno entity, DBContextAdapter dbContext);

        Mensaje actualizar(ElementoExterno entity, DBContextAdapter dbContext);

        Mensaje eliminar(ElementoExterno entity, DBContextAdapter dbContext);

        Mensaje getElementoExPorContenedor(Contenedor c, DBContextAdapter dbContext);

        Mensaje SaveElementoExterno(List<ElementoExterno> listelem, DBContextAdapter dbContext);

        Mensaje DeleteElementoExterno(List<ElementoExterno> e, DBContextAdapter dbContext);
    }
}
