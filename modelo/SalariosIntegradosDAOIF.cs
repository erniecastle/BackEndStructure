/**
* @author: Daniel Ruelas
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface SalariosIntegradosDAOIF para llamados a metodos de Entity
* -----------------------------------------------------------------------------
* MODIFICACIONES:
* -----------------------------------------------------------------------------
* Clave: 
* Autor: 
* Fecha:
* Descripción: 
* -----------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Core.genericos.campos;
using System.Data.Entity;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public interface SalariosIntegradosDAOIF
    {
        Mensaje agregar(SalariosIntegrados entity, DBContextAdapter dbContext);

        Mensaje actualizar(SalariosIntegrados entity, DBContextAdapter dbContext);

        Mensaje eliminar(SalariosIntegrados entity, DBContextAdapter dbContext);

        Mensaje getAllSalariosIntegrados(RazonesSociales razonesSociales, DBContextAdapter dbContext);

        Mensaje getSalariosIntegradosPorEmpleadoyRegPat(decimal idEmpleados, decimal idRegistroPatronal, DateTime fecha, DBContextAdapter dbContext);

        Mensaje getSDIActualPorEmpleadoyRegPatActual(String claveEmpleados, String claveRegistroPatronal, DateTime fecha, RazonesSociales razonesSociales, DBContextAdapter dbContext);

        Mensaje getSalariosIntegradosPorRegPatronal(RegistroPatronal registroPatronal, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosSalariosIntegrados(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosSalariosIntegrados(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        Mensaje getSalarioIDEmpleadoIDReg(decimal? idEmpleado, decimal? idRegistroPatronal,DateTime fecha, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);
    }
}
