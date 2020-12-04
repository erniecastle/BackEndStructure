/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface FiniqLiquidCncNomDAOIF para llamados a metodos de Entity
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
    public interface FiniqLiquidCncNomDAOIF
    {
        Mensaje agregar(FiniqLiquidCncNom entity, DBContextAdapter dbContext);

        Mensaje actualizar(FiniqLiquidCncNom entity, DBContextAdapter dbContext);

        Mensaje eliminar(FiniqLiquidCncNom entity, DBContextAdapter dbContext);

        Mensaje getFiniqLiquidCncNomPorFiniquitosLiquidaciones(FiniquitosLiquida finiquitosLiquidacion, DBContextAdapter dbContext);

        //    FiniqLiquidCncNom getFiniqLiquidCncNomPorEmpERegPatyRazonSoc(Empleados empleados, RegistroPatronal registroPatronal, RazonesSociales razonSocial);
        //
        //    List<FiniqLiquidCncNom> getFiniqLiquidCncNomPorRegPatronal(RegistroPatronal registroPatronal);
        //
        //    List<FiniqLiquidCncNom> getFiniqLiquidCncNomPorRazonSocial(RazonesSociales razonSocial);

        Mensaje consultaPorFiltrosFiniqLiquidCncNom(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosFiniqLiquidCncNom(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje saveDeleteFiniqLiquidCncNom(List<FiniqLiquidCncNom> entitysCambios, Object[] clavesDelete, DBContextAdapter dbContext);
    }
}
