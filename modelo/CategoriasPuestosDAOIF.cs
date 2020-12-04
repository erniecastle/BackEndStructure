/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface CategoriasPuestosDAOIF para llamados a metodos de Entity
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
    public interface CategoriasPuestosDAOIF : IGenericRepository<CategoriasPuestos>
    {
        Mensaje agregar(CategoriasPuestos entity, DBContextAdapter dbContext);

        Mensaje modificar(CategoriasPuestos entity, DBContextAdapter dbContext);

        Mensaje eliminar(CategoriasPuestos entity, DBContextAdapter dbContext);

        Mensaje getAllCategoriasPuestos(DBContextAdapter dbContext);

        Mensaje getPorIdCategoriasPuestos(decimal? idCategoriasPuestos,DBContextAdapter dbContext);

        Mensaje SaveCategoriaPuesto(List<PercepcionesFijas> agrega, Object[] eliminados, CategoriasPuestos entity, DBContextAdapter dbContext);

        Mensaje DeleteCategoriaPuesto(CategoriasPuestos entity, DBContextAdapter dbContext);

        Mensaje UpdateCategoriaPuesto(List<PercepcionesFijas> agrega, Object[] eliminados, CategoriasPuestos entity, DBContextAdapter dbContext);

        Mensaje consultaPorRangosCategoriasPuestos(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        Mensaje saveDeleteCategoriasPuestos(List<CategoriasPuestos> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);
    }
}
