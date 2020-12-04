/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface CampoDIMDAOIF para llamados a metodos de Entity
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
    public interface CampoDIMDAOIF
    {
        Mensaje agregar(CampoDIM entity, DBContextAdapter dbContext);

        Mensaje modificar(CampoDIM entity, DBContextAdapter dbContext);

        Mensaje eliminar(CampoDIM entity, DBContextAdapter dbContext);

        Mensaje getAllCampoDIM(DBContextAdapter dbContext);

        Mensaje getPorClaveCampoDIM(String clave, DBContextAdapter dbContext);

        Mensaje getPorIdCampoDIM(decimal? idCampoDIM, DBContextAdapter dbContext);

        Mensaje consultaPorRangosCampoDIM(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);
    }
}
