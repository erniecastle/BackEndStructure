/**
* @author: Daniel Ruelas
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface SalariosIntegradosDetDAOIF para llamados a metodos de Entity
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
    public interface SalariosIntegradosDetDAOIF
    {
        Mensaje agregar(SalariosIntegradosDet entity, DBContextAdapter dbContext);

        Mensaje actualizar(SalariosIntegradosDet entity, DBContextAdapter dbContext);

        Mensaje eliminar(SalariosIntegradosDet entity, DBContextAdapter dbContext);

        Mensaje getSalariosIntegradosDetAll(RazonesSociales razonesSociales, DBContextAdapter dbContext);

        Mensaje getSalariosIntegradosDetPorEmpleadoyRegPat(Empleados empleados, RegistroPatronal registroPatronal, DateTime fecha, DBContextAdapter dbContext);

        Mensaje getSalariosIntegradosDetPorRegPatronal(RegistroPatronal registroPatronal, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosSalariosIntegradosDet(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosSalariosIntegradosDet(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);
    }
}
