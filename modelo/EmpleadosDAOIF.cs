

using Exitosw.Payroll.Core.util;
/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface EmpleadosDAOIF para llamados a metodos de Entity
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
    public interface EmpleadosDAOIF
    {
        Mensaje agregar(Empleados entity, DBContextAdapter dbContext);

        Mensaje actualizar(Empleados entity, DBContextAdapter dbContext);

        Mensaje eliminar(Empleados entity, DBContextAdapter dbContext);

        Mensaje getAllEmpleados(String claveRazonesSocial, DBContextAdapter dbContext);

        Mensaje guardarEmpleado(Empleados entity, List<Familiares> agregaModFam, Object[] eliminadosFam,
            List<FormacionAcademica> agregaModForAc, object[] eliminadosForAc,
            List<Capacitaciones> agregaModCap, object[] eliminadosCap,
            List<ExperienciaLaboralExterna> agregaModExLbExt, object[] eliminadosExLbExt,
            List<ExperienciaLaboralInterna> agregaModExLbInt, object[] eliminadosExLbInt,
            List<Documentacion> agregaModDoc, object[] eliminadosDoc,
            DBContextAdapter dbContext);

        Mensaje eliminarEmpleado(Empleados entity, DBContextAdapter dbContext);

        Mensaje SaveEmpleado(List<Familiares> agrega, Object[] eliminados,
                List<FormacionAcademica> agregaFE, Object[] eliminadosFE,
                List<Capacitaciones> agregaCap, Object[] eliminadosCap,
                List<ExperienciaLaboralExterna> agregaELE, Object[] eliminadosELE,
                List<ExperienciaLaboralInterna> agregaELI, Object[] eliminadosELI,
                List<Documentacion> agregaDoc, Object[] eliminadosDoc, List<VacacionesAplicacion> agregaVac,
                Empleados empleados, List<PlazasPorEmpleado> listPlazasPorEmpleados, List<PlazasPorEmpleadosMov> listPlazasPorEmpleadoMov, IngresosBajas ingresosReingresosBajas, SalariosIntegrados salariosIntegrados, Object[] eliminadosVac, Object[] eliminadosVacDis, Object[] eliminadosVacDev, DBContextAdapter dbContext);

        Mensaje DeleteEmpleado(Empleados entity, DBContextAdapter dbContext);

        Mensaje UpdateEmpleado(List<Familiares> agrega, Object[] eliminados,
                List<FormacionAcademica> agregaFE, Object[] eliminadosFE,
                List<Capacitaciones> agregaCap, Object[] eliminadosCap,
                List<ExperienciaLaboralExterna> agregaELE, Object[] eliminadosELE,
                List<ExperienciaLaboralInterna> agregaELI, Object[] eliminadosELI,
                List<Documentacion> agregaDoc, Object[] eliminadosDoc, List<VacacionesAplicacion> agregaVac, Empleados empleados,
                List<PlazasPorEmpleadosMov> listPlazasPorEmpleadoMov, IngresosBajas ingresosReingresosBajas, bool calcularSDI, SalariosIntegrados salariosIntegrados, List<VacacionesAplicacion> deleteVacaciones, DBContextAdapter dbContext);

        Mensaje existeRFC(String rfc, String claveEmpleado, DBContextAdapter dbContext);

        Mensaje getPorIdEmpleado(decimal? idEmpl, string claveRazon, DBContextAdapter dbContext);

        Mensaje getPorIdEmpleadoYComplementos(decimal? idEmpl, string claveRazon, DBContextAdapter dbContext);

        Mensaje getEmpleadoPorClave(string claveEmpleado, string claveRazon, DBContextAdapter dbContext);

    }
}
