using Exitosw.Payroll.Entity.entidad;
/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface FiniquitosLiquidacionesDAOIF para llamados a metodos de Entity
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
using Exitosw.Payroll.Core.genericos.campos;
using System.Data.Entity;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public interface FiniquitosLiquidacionesDAOIF
    {
        Mensaje agregar(FiniquitosLiquida entity, DBContextAdapter dbContext);

        Mensaje actualizar(FiniquitosLiquida entity, DBContextAdapter dbContext);

        Mensaje eliminar(FiniquitosLiquida entity, DBContextAdapter dbContext);


        Mensaje guardarFiniquitos(FiniquitosLiquida entity, List<FiniqLiquidPlazas> agregaModFiniqLiquidPlazas, object[] eliminadosFiniqLiquidPlazas,
            List<FiniqLiquidCncNom> agregaModFiniqLiquidCncNom, object[] eliminadosFiniqLiquidCncNom, DBContextAdapter dbContext, DBContextAdapter dbContextMaster);

        Mensaje getFiniquitosLiquidacionesAll(String claveRazonesSociales, TipoBaja tipoBaja, ModoBaja modoBaja, DBContextAdapter dbContext);

        Mensaje getFiniquitosLiquidacionesPorCamposClave(String referencia, RazonesSociales razonSocial, ModoBaja modoBaja, TipoBaja tipoBaja, DBContextAdapter dbContext);

        Mensaje getPorEmpleado(String claveEmpleado, String claveRazonSocial, ModoBaja modoBaja, TipoBaja tipoBaja, DBContextAdapter dbContext);

        Mensaje getFiniquitosLiquidacionesGuardarModificar(FiniquitosLiquida finiquitosLiquidaciones, Object[] clavesDeleteMovimientos,
                List<MovNomConcep> AgreModifMovimientos, List<FiniqLiquidPlazas> finiqLiquidPlazas, Object[] eliminadosfiniqLiquidPlazas,
                List<FiniqLiquidCncNom> listFiniqLiquidCncNom, List<FiniqLiquidCncNom> deleteFiniqLiquidCncNom,
                int cantPlazasFiniquitadas, int cantPlazasEmpleado, IngresosBajas ingresosReingresosBajas, List<PlazasPorEmpleado> cerrarPlazaEmpleado, SalariosIntegrados salariosIntegrado, int rango, DBContextAdapter dbContext);

        Mensaje getCancelarFiniquito(Object[] eliminadoMovNomConceps, List<PlazasPorEmpleado> listPlazasPorEmpleado, IngresosBajas ingresosReingresosBajas,
                SalariosIntegrados salariosIntegrado, FiniquitosLiquida finiquitosLiquidaciones, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosFiniquitosLiquida(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosFiniquitosLiquida(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje EliminarFiniquitosLiquidacion(FiniquitosLiquida finiquitosLiquidaciones, List<PlazasPorEmpleadosMov> plazasPorEmpleados, DBContextAdapter dbContext);

        Mensaje validaFiniquitosLiquidacionTimbrados(DateTime fechaCalculo, List<PlazasPorEmpleadosMov> plazasPorEmpleados, DBContextAdapter dbContext);

        Mensaje getSaveFinLiqModifConceptos(FiniquitosLiquida finiquitosLiquidaciones, Object[] clavesDeleteMovimientos, List<MovNomConcep> agreModifMovimientos, List<FiniqLiquidCncNom> listFiniqLiquidCncNom, List<FiniqLiquidCncNom> deleteFiniqLiquidCncNom, int rango, DBContextAdapter dbContext);
    }

}

