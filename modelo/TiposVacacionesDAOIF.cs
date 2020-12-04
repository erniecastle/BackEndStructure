/**
* @author: Daniel Ruelas
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface TiposVacacionesDAOIF para llamados a metodos de Entity
* -----------------------------------------------------------------------------
* MODIFICACIONES:
* -----------------------------------------------------------------------------
* Clave: 
* Autor: 
* Fecha:
* Descripción: 
* -----------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Core.genericos.campos;
using System.Data.Entity;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public interface TiposVacacionesDAOIF
    {
        Mensaje getAllTiposVacaciones(DBContextAdapter dbContext);

        Mensaje getPorClaveTiposVacaciones(String clave, DBContextAdapter dbContext);

        Mensaje getPorIdTiposVacaciones(decimal? id ,DBContextAdapter dbContext);

        Mensaje agregar(TiposVacaciones entity, DBContextAdapter dbContext);

        Mensaje modificar(TiposVacaciones entity, DBContextAdapter dbContext);

        Mensaje eliminar(TiposVacaciones entity, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosTiposVacaciones(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosTiposVacaciones(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje saveDeleteTiposVacaciones(List<TiposVacaciones> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);

        //Mensaje existeClave(String tabla, String[] campo, Object[] valores, String queryAntesDeFrom, DBContextAdapter dbContext);

        Mensaje getAllTiposVacacionesJS(DBContextAdapter dbContext);
    }
}
