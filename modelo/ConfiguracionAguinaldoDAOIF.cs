

using Exitosw.Payroll.Core.util;
/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface ConfiguracionAguinaldoDAOIF para llamados a metodos de Entity
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
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public interface ConfiguracionAguinaldoDAOIF : IGenericRepository<AguinaldoConfiguracion>
    {
        Mensaje agregar(AguinaldoConfiguracion entity, DBContextAdapter dbContext);

        Mensaje modificar(AguinaldoConfiguracion entity, DBContextAdapter dbContext);

        Mensaje eliminar(AguinaldoConfiguracion entity, DBContextAdapter dbContext);

        Mensaje getAllConfiguracionAguinaldo(DBContextAdapter dbContext);

        Mensaje SaveConfiguracionAguinaldo(List<AguinaldoConfiguracion> agrega, Object[] eliminados, DBContextAdapter dbContext);

        Mensaje getPorClaveConfiguracionAguinaldo(String claveRazonsocial, DBContextAdapter dbContext);
    }
}
