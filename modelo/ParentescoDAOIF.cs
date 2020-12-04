/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface ParentescoDAOIF para llamados a metodos de Entity
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
    public interface ParentescoDAOIF : IGenericRepository<Parentesco>
    {
        Mensaje agregar(Parentesco entity, DBContextAdapter dbContext);

        Mensaje modificar(Parentesco entity, DBContextAdapter dbContext);

        Mensaje eliminar(Parentesco entity, DBContextAdapter dbContext);

        Mensaje getAllParentesco(DBContextAdapter dbContext);

        Mensaje getPorClaveParentescos(String clave, DBContextAdapter dbContext);

        Mensaje getPorIdParentesco(decimal? id, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosParentesco(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosParentesco(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje saveDeleteParentesco(List<Parentesco> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);
    }
}
