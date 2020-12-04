/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface CuentasContablesDAOIF para llamados a metodos de Entity
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
using Exitosw.Payroll.Core.genericos.campos;
using System;
using System.Collections.Generic;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public interface CuentasContablesDAOIF : IGenericRepository<CuentasContables>
    {
        Mensaje agregar(CuentasContables entity, DBContextAdapter dbContext);

        Mensaje modificar(CuentasContables entity, DBContextAdapter dbContext);

        Mensaje eliminar(CuentasContables entity, DBContextAdapter dbContext);

        Mensaje getAllCuentasContables(DBContextAdapter dbContext);

        Mensaje getPorClaveCuentasContables(String clave, DBContextAdapter dbContext);

        Mensaje getPorIdCuentasContables(decimal? id ,DBContextAdapter dbContext);

        Mensaje consultaPorRangosCuentasContables(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje saveDeleteCuentasContables(List<CuentasContables> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);
    }
}
