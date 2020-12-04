/**
* @author: Daniel Ruelas
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface TipoContratoDAOIF para llamados a metodos de Entity
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
    public interface TipoContratoDAOIF : IGenericRepository<TipoContrato>
    {
        Mensaje agregar(TipoContrato entity, DBContextAdapter dbContext);

        Mensaje modificar(TipoContrato entity, DBContextAdapter dbContext);

        Mensaje eliminar(TipoContrato entity, DBContextAdapter dbContext);

        Mensaje getAllTipoContrato(DBContextAdapter dbContext);

        Mensaje getPorClaveTipoContrato(String clave, DBContextAdapter dbContext);

        Mensaje getPorIdTipoContrato(decimal? id, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosTipoContrato(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosTipoContrato(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje saveDeleteTipoContrato(List<TipoContrato> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);
    }
}
