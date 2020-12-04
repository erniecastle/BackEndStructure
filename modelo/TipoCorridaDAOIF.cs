/**
* @author: Daniel Ruelas
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface TipoCorridaDAOIF para llamados a metodos de Entity
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
   public interface TipoCorridaDAOIF
    {
        Mensaje agregar(TipoCorrida entity, DBContextAdapter dbContext);

        Mensaje modificar(TipoCorrida entity, DBContextAdapter dbContext);

        Mensaje eliminar(TipoCorrida entity, DBContextAdapter dbContext);

        Mensaje getAllTipoCorrida(DBContextAdapter dbContext);

        Mensaje getPorClaveTipoCorrida(String clave, DBContextAdapter dbContext);

        Mensaje getPorIdTipoCorrida(decimal? id,DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosTipoCorrida(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosTipoCorrida(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje saveDeleteTipoCorrida(List<TipoCorrida> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);

        Mensaje saveTipoCorridaContenedor(TipoCorrida entity, Contenedor contenedor, DBContextAdapter dbContextSimple, DBContextAdapter dbContextMaster);

        Mensaje DeleteTipoCorridaContenedor(TipoCorrida entity, DBContextAdapter dbContextSimple, DBContextAdapter dbContextMaster);
    }
}
