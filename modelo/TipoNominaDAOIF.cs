/**
* @author: Daniel Ruelas
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface TipoNominaDAOIF para llamados a metodos de Entity
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
    public interface TipoNominaDAOIF
    {
        Mensaje getAllTipoNomina(DBContextAdapter dbContext);

        Mensaje getPorClaveTipoNomina(String clave, DBContextAdapter dbContext);

        Mensaje getSeriePorTipoNomina(string clave, DBContextAdapter dbContext);

        Mensaje getPorIdTipoNomina(decimal? id, DBContextAdapter dbContext);

        Mensaje agregar(TipoNomina entity, DBContextAdapter dbContext);

        Mensaje modificar(TipoNomina entity, DBContextAdapter dbContext);

        Mensaje eliminar(TipoNomina entity, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosTipoNomina(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosTipoNomina(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DbContext dbContext);

        Mensaje saveDeleteTipoNomina(List<TipoNomina> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);

        //Mensaje existeClave(String tabla, String[] campo, Object[] valores, String queryAntesDeFrom, DbContext dbContext);
    }
}
