

using Exitosw.Payroll.Core.util;
/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface ConceptosEspecialesDAOIF para llamados a metodos de Entity
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
    public interface ConceptosEspecialesDAOIF : IGenericRepository<ConceptosEspeciales>
    {
        Mensaje agregar(ConceptosEspeciales entity, DBContextAdapter dbContext);

        Mensaje actualizar(ConceptosEspeciales entity, DBContextAdapter dbContext);

        Mensaje eliminar(ConceptosEspeciales entity, DBContextAdapter dbContext);

        Mensaje getConceptosEspecialesPorTipo(TipoConceptosEspeciales claveTipo, DBContextAdapter dbContext);

        Mensaje getExisteConceptosEspecialesPorTipoYconcepto(String claveConceptoDeNomina, TipoConceptosEspeciales claveTipo, DBContextAdapter dbContext);

        Mensaje saveDeleteConceptosEspeciales(List<ConceptosEspeciales> AgreModif, Object[] clavesDelete, DBContextAdapter dbContext);

        Mensaje getConceptosNominaClavesPorTipo(TipoConceptosEspeciales claveTipo, DBContextAdapter dbContext);

        Mensaje getAllConceptosEspeciales(DBContextAdapter dbContext);
    }
}
