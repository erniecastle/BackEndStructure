/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface IngresosReingresosBajasDAOIF para llamados a metodos de Entity
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
using System.Data.Entity;
using Exitosw.Payroll.Core.genericos.campos;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public interface IngresosBajasDAOIF
    {
        Mensaje agregar(IngresosBajas entity, DBContextAdapter dbContext);

        Mensaje actualizar(IngresosBajas entity, DBContextAdapter dbContext);

        Mensaje eliminar(IngresosBajas entity, DBContextAdapter dbContext);

        Mensaje getAllIngresosReingresosBajas(DBContextAdapter dbContext);

        Mensaje getIngresosBajasPorEmpRegPatyRazonSoc(decimal idEmpleado, decimal idRegPat, decimal idRazonSocial, DBContextAdapter dbContext);

        Mensaje getIngresosReingresosBajasPorRegPatronal(RegistroPatronal registroPatronal, DBContextAdapter dbContext);

        Mensaje getIngresosReingresosBajasPorRazonSocial(RazonesSociales razonSocial, DBContextAdapter dbContext);

        Mensaje getPorEmpleadoActivo(String claveEmpleado, String claveRegPatronal, String claveRazonSocial, DBContextAdapter dbContext);

        Mensaje getPorEmpleadoInactivo(String claveEmpleado, String claveRegPatronal, String claveRazonSocial, DateTime fechaActual, DBContextAdapter dbContext);

        Mensaje getPorReferenciaPlazaEmpActivo(String claveReferenciaPlazaEmp, String claveRegPatronal, String claveRazonSocial, DBContextAdapter dbContext);

        Mensaje getPorClaveEmpleado(String claveEmp, String claveRegPatronal, String claveRazonSocial, DBContextAdapter dbContext);

        Mensaje getPorReferenciaPlazaEmpInactiva(String claveReferenciaPlazaEmp, String claveRegPatronal, String claveRazonSocial, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosIngReingresosBajas(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosIngReingresosBajas(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje getIngresosReingresosBajasPorIdEmpleado(Empleados empleados, DBContextAdapter dbContext);

        Mensaje getIngresosReingresosBajasPorIdEmpleadoJS(decimal empleados, DBContextAdapter dbContext);


    }
}
