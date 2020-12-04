/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface ConfigFoliacionDAOIF para llamados a metodos de Entity
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
using Exitosw.Payroll.Core.genericos.campos;
using System;
using System.Collections.Generic;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public interface ConfigFoliacionDAOIF : IGenericRepository<ConfigFoliacion>
    {
        Mensaje agregar(ConfigFoliacion entity, DBContextAdapter dbContext);

        Mensaje actualizar(ConfigFoliacion entity, DBContextAdapter dbContext);

        Mensaje eliminar(ConfigFoliacion entity, DBContextAdapter dbContext);

        Mensaje getAllConfigFoliacion(DBContextAdapter dbContext);

        Mensaje getPorClaveConfigFoliacion(String tabla, String campoClave, DBContextAdapter dbContext);

        Mensaje consultaPorRangosConfigFoliacion(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje saveDeleteConfigFoliacion(List<ConfigFoliacion> entitysCambios, Object[] idDelete, int rango, DBContextAdapter dbContext);
    }
}
