using Exitosw.Payroll.Core.util;
/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface FormatoCnxContaDAOIF para llamados a metodos de Entity
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
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public interface FormatoCnxContaDAOIF : IGenericRepository<FormatoCnxConta>
    {
        Mensaje agregar(FormatoCnxConta entity, DBContextAdapter dbContext);

        Mensaje modificar(FormatoCnxConta entity, DBContextAdapter dbContext);

        Mensaje eliminar(FormatoCnxConta entity, DBContextAdapter dbContext);

        Mensaje saveDeleteConfiguracionNivelCuenta(FormatoCnxConta entity, List<decimal> eliminados, DBContextAdapter dbContext);
    }
}
