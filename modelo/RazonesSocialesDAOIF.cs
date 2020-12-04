/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface RazonesSocialesDAOIF para llamados a metodos de Entity
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
    public interface RazonesSocialesDAOIF : IGenericRepository<RazonesSociales>
    {
        Mensaje agregar(RazonesSociales entity, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje modificar(RazonesSociales entity, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje eliminar(RazonesSociales entity, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje getAllRazonesSociales(DBContextAdapter dbContext);

        Mensaje getPorClaveRazonesSociales(String clave, DBContextAdapter dbContext);

        Mensaje getSeriePorRazonesSociales(string clave, DBContextAdapter dbContext);

        Mensaje getPorIdRazonesSociales(decimal? id, DBContextAdapter dbContext);

        Mensaje getRazonesSocialesPorClaves(String[] claveRazonesSociales, DBContextAdapter dbContext);

        Mensaje existeRFC(String rfc, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosRazonesSociales(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosRazonesSociales(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje saveDeleteRazonesSociales(List<RazonesSociales> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje consultaPorFiltroIN(String query, Object[] campos, Object[] valores, DBContextAdapter dbContext);

    }
}
