/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface ConceptoDeNominaDAOIF para llamados a metodos de Entity
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
using System.Data.Entity;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public interface ConceptoDeNominaDAOIF
    {
        Mensaje agregar(ConceptoDeNomina entity, DBContextAdapter dbContext);

        Mensaje modificar(ConceptoDeNomina entity, DBContextAdapter dbContext);

        Mensaje eliminar(ConceptoDeNomina entity, DBContextAdapter dbContext);

        Mensaje getAllConceptoDeNomina(DBContextAdapter dbContext);

        Mensaje getPorClaveConceptoDeNomina(String clave, DBContextAdapter dbContext);

        Mensaje getPorIdConceptoDeNomina(decimal? id,DBContextAdapter dbContext);

        Mensaje consultaPorRangosConceptoDeNomina(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);
    }
}
