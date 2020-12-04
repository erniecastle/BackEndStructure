/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface BaseNominaDAOIF para llamados a metodos de Entity
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
    public interface BaseNominaDAOIF
    {
        Mensaje agregar(BaseNomina entity, DBContextAdapter dbContext);

        Mensaje modificar(BaseNomina entity, DBContextAdapter dbContext);

        Mensaje eliminar(BaseNomina entity, DBContextAdapter dbContext);

        Mensaje getAllBaseNomina(DBContextAdapter dbContext);

        Mensaje getBaseNominaReservadosONoReservados(Boolean soloReservados, DBContextAdapter dbContext);

        Mensaje getPorClaveBaseNomina(String clave, DBContextAdapter dbContext);

        Mensaje getPorIdBaseNomina(decimal? idBaseNomina,DBContextAdapter dbContext);

        Mensaje consultaPorRangosBaseNomina(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);
    }
}
