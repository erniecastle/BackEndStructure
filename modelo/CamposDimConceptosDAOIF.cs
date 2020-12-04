/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface CamposDimConceptosDAOIF para llamados a metodos de Entity
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
using System.Collections.Generic;
using System.Data.Entity;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public interface CamposDimConceptosDAOIF
    {
        Mensaje getCampoDimConceptoAll(DBContextAdapter dbContext);

        Mensaje guardaryEliminar(List<CamposDimConceptos> ListaGuardar, List<CamposDimConceptos> ListaEliminar, DBContextAdapter dbContext);

        Mensaje filtradoCampoDimConceptos(int campoDim, int Concepto, DBContextAdapter dbContext);

        Mensaje filtradoCampoDim(int campoDim, DBContextAdapter dbContext);

        Mensaje filtradoConceptos(int Concepto, DBContextAdapter dbContext);
    }
}
