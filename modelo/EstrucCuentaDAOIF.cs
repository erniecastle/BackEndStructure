using Exitosw.Payroll.Core.util;
/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface EstrucCuentaDAOIF para llamados a metodos de Entity
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
using Exitosw.Payroll.Entity.entidad.contabilidad;
using System.Collections.Generic;


namespace Exitosw.Payroll.Core.modelo
{
    public interface EstrucCuentaDAOIF : IGenericRepository<EstrucCuenta>
    {
        Mensaje agregar(List<EstrucCuenta> entity, DBContextAdapter dbContext);

        Mensaje actualizar(List<EstrucCuenta> entity, DBContextAdapter dbContext);

        Mensaje eliminar(List<EstrucCuenta> entity, DBContextAdapter dbContext);
    }
}
