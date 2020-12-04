/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface CreditoMovimientosDAOIF para llamados a metodos de Entity
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
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public interface CreditoMovimientosDAOIF
    {
        Mensaje getCreditoMovimientos(DateTime fecha, String tipoCredito, TiposMovimiento tipoMovimiento, String razonesSociales, String tipoConfiguracion, DBContextAdapter dbContext);

        Mensaje getMaxNumeroCreditoMovimiento(CreditoPorEmpleado credito, TiposMovimiento tiposMovimiento, DateTime fecha, DBContextAdapter dbContext);

        Mensaje saveDeleteCreditosMov(List<CreditoMovimientos> entitysCambios, object[] deleteCreditos, TiposMovimiento tiposMovimiento, DBContextAdapter dbContext);

        Mensaje getCreditoMovimientosXEntity(DateTime fecha, decimal tipoCredito, TiposMovimiento tipoMovimiento, decimal razonesSociales, String tipoConfiguracion, DBContextAdapter dbContext);
    }
}
