/**
* @author: Daniel Ruelas
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface TipoTablaDAOIF para llamados a metodos de Entity
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
   public interface TipoTablaDAOIF:IGenericRepository<TipoTabla>
    {
        Mensaje getAllTipoTabla(DBContextAdapter dbContext);

        Mensaje getPorClaveTipoTabla(String clave, DBContextAdapter dbContext);

        Mensaje getPorIdTipoTabla(decimal? id,DBContextAdapter dbContext);

        Mensaje agregar(TipoTabla entity, DBContextAdapter dbContext);

        Mensaje modificar(TipoTabla entity, DBContextAdapter dbContext);

        Mensaje eliminar(TipoTabla tipoTabla, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosTipoTabla(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosTipoTabla(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje saveDeleteTipoTabla(List<TipoTabla> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);
    }
}
