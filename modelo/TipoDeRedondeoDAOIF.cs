/**
* @author: Daniel Ruelas
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface TipoDeRedondeoDAOIF para llamados a metodos de Entity
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
   public interface TipoDeRedondeoDAOIF
    {
        Mensaje getAllTipoDeRedondeo(DBContextAdapter dbContext);

        Mensaje getPorClaveTipoDeRedondeo(String clave, DBContextAdapter dbContext);

        Mensaje getPorIdTipoDeRedondeo(decimal? id ,DBContextAdapter dbContext);

        Mensaje agregar(TipoDeRedondeo entity, DBContextAdapter dbContext);

        Mensaje modificar(TipoDeRedondeo entity, DBContextAdapter dbContext);

        Mensaje eliminar(TipoDeRedondeo entity, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosTipoDeRedondeo(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosTipoDeRedondeo(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje agregarListTipoDeRedondeo(List<TipoDeRedondeo> entitys, int rango, DBContextAdapter dbContext);

        Mensaje deleteListQuerys(String tabla, String campo, Object[] valores, DBContextAdapter dbContext);

        Mensaje getTipoDeRedondeoPorTipoValor(TipoDeValor tipoValor, DBContextAdapter dbContext);

        Mensaje updateTipoRedondeoValor(TipoDeRedondeo entity, List<DatosTipoValor> eliminaDatosTipoValores, DBContextAdapter dbContext);
    }
}
