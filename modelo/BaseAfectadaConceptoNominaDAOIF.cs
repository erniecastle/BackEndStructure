

using Exitosw.Payroll.Core.util;
/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface BaseAfectadaConceptoNominaDAOIF para llamados a metodos de Entity
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
    public interface BaseAfectadaConceptoNominaDAOIF
    {
        Mensaje agregar(BaseAfecConcepNom entity, DBContextAdapter dbContext);

        Mensaje modificar(BaseAfecConcepNom entity, DBContextAdapter dbContext);

        Mensaje eliminar(BaseAfecConcepNom entity, DBContextAdapter dbContext);

        Mensaje getAllBaseAfecConcepNom(DBContextAdapter dbContext);

        Mensaje getPorClaveBaseAfecConcepNom(String clave, DBContextAdapter dbContext);

        //Mensaje consultaPorRangos(int inicio, int rango, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje agregarListaBaseAfectadaConceptoNomina(List<BaseAfecConcepNom> entitys, int rango, DBContextAdapter dbContext);

       // Mensaje deleteListQuerys(String tabla, String campo, Object[] valores, DBContextAdapter dbContext);
    }
}
