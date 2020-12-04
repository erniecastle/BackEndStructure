/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface ConceptoPorTipoCorridaDAOIF para llamados a metodos de Entity
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
    public interface ConceptoPorTipoCorridaDAOIF
    {
        Mensaje agregar(ConceptoPorTipoCorrida entity, DBContextAdapter dbContext);

        Mensaje modificar(ConceptoPorTipoCorrida entity, DBContextAdapter dbContext);

        Mensaje eliminar(ConceptoPorTipoCorrida entity, DBContextAdapter dbContext);

        Mensaje getAllConceptoPorTipoCorrida(DBContextAdapter dbContext);

        Mensaje getConceptoPorTipoCorrida(TipoCorrida tipoCorrida, DBContextAdapter dbContext);

        Mensaje getConceptoPorTipoCorridaId(decimal? idTipoCorrida, DBContextAdapter dbContext);

        Mensaje saveDeleteConceptoPorTipoCorrida(List<ConceptoPorTipoCorrida> entitysCambios, Object[] eliminados, DBContextAdapter dbContext);

        Mensaje getConceptoPorTipoCorridaMostrarActivo(String tipoCorrida, DBContextAdapter dbContext);

        Mensaje consultaPorRangosConceptoPorTipoCorrida(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        Mensaje getConceptoPorTipoCorridaIdClavesConcep(decimal? tipoCorrida, List<string> clavesconcep, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);
    }
}
