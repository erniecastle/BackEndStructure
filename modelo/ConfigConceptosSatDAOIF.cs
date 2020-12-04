/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface ConfigConceptosSatDAOIF para llamados a metodos de Entity
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
    public interface ConfigConceptosSatDAOIF
    {
        Mensaje agregar(ConfigConceptosSat entity, DBContextAdapter dbContext);

        Mensaje modificar(ConfigConceptosSat entity, DBContextAdapter dbContext);

        Mensaje eliminar(ConfigConceptosSat entity, DBContextAdapter dbContext);

        Mensaje getAllConfigConceptosSat(DBContextAdapter dbContext);

        Mensaje getPorClaveConfigConceptosSat(String clave, DBContextAdapter dbContext);

        Mensaje getPorIdConfigConceptosSat(decimal? id , DBContextAdapter dbContext);

        Mensaje saveDeleteConfigConceptosSat(List<ConfigConceptosSat> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosConfigConceptosSat(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje eliminaPorClaveConceptoNomina(String claveConcepto, DBContextAdapter dbContext);
    }
}
