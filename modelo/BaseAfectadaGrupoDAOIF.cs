

using Exitosw.Payroll.Core.util;
/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface BaseAfectadaGrupoDAOIF para llamados a metodos de Entity
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
    public interface BaseAfectadaGrupoDAOIF
    {

        Mensaje agregar(BaseAfectadaGrupo entity, DBContextAdapter dbContext);

        Mensaje modificar(BaseAfectadaGrupo entity, DBContextAdapter dbContext);

        Mensaje eliminar(BaseAfectadaGrupo entity, DBContextAdapter dbContext);

        Mensaje getAllBaseAfectadaGrupo(DBContextAdapter dbContext);

        Mensaje getPorClaveBaseAfectadaGrupo(String clave, DBContextAdapter dbContext);

        Mensaje consultaPorRangosBaseAfectadaGrupo(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje agregarListaBaseAfectadaGrupo(List<BaseAfectadaGrupo> entitys, int rango, DBContextAdapter dbContext);

        //Mensaje deleteListQuerys(String tabla, String campo, Object[] valores, DBContextAdapter dbContext);
    }
}
