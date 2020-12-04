/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface FiniqLiquidPlazasDAOIF para llamados a metodos de Entity
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
    public interface FiniqLiquidPlazasDAOIF
    {
        Mensaje agregar(FiniqLiquidPlazas entity, DBContextAdapter dbContext);

        Mensaje actualizar(FiniqLiquidPlazas entity, DBContextAdapter dbContext);

        Mensaje eliminar(FiniqLiquidPlazas entity, DBContextAdapter dbContext);

        Mensaje getCantidadPlazasFiniquitadaPorEmpleado(String claveEmpleado, String razonSocial, DBContextAdapter dbContext);

        Mensaje getPlazasFiniquitadaPorEmpleado(String claveEmpleado, String razonSocial, DBContextAdapter dbContext);

        //    List<FiniqLiquidPlazas> getFiniqLiquidPlazasPorRazonSocial(RazonesSociales razonSocial);

        Mensaje consultaPorFiltrosFiniqLiquidPlazas(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosFiniqLiquidPlazas(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje saveDeleteFiniqLiquidPlazas(List<FiniqLiquidPlazas> entitysCambios, Object[] clavesDelete, DBContextAdapter dbContext);
    }
}
