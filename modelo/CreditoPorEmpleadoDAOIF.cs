/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface CreditoPorEmpleadoDAOIF para llamados a metodos de Entity
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
using System.Data.Entity;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public interface CreditoPorEmpleadoDAOIF
    {
        Mensaje agregar(CreditoPorEmpleado entity, DBContextAdapter dbContext);

        Mensaje actualizar(CreditoPorEmpleado entity, DBContextAdapter dbContext);

        Mensaje eliminar(CreditoPorEmpleado entity, DBContextAdapter dbContext);

        Mensaje getCreditosAll(String claveTipoCredito, String tipoConfiguracion, DBContextAdapter dbContext);

        Mensaje getCreditosPorClave(String numeroCredito, String claveTipoCredito, String tipoConfiguracion, DBContextAdapter dbContext);

        Mensaje getCreditosPorTipoCredito(String claveTipoCredito, String tipoConfiguracion, DBContextAdapter dbContext);

        Mensaje existenMovimientosEnCreditos(String numeroDeCredito, String claveCreditoAhorro, String tipoConfiguracion, DBContextAdapter dbContext);

        Mensaje getCreditosPorTipoCreditoYFecha(DateTime fecha, String tipoCredito, String claveRazonsocial, String tipoConfiguracion, DBContextAdapter dbContext);

        Mensaje saveDeleteCreditos(List<CreditoPorEmpleado> entitysCambios, Object[] eliminados, DBContextAdapter dbContext);

        Mensaje consultaPorRangosCreditoPorEmpleado(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje getCredPorTipoCreditoYFecha(DateTime fecha, String tipoCredito, String claveRazonsocial, String tipoConfiguracion, DBContextAdapter dbContext);
    }
}
