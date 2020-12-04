/**
 * @author: Daniel Ruelas
 * Fecha de Creación: 17/01/2018
 * Compañía: Exito Software
 * Descripción del programa: Interface GruposDAOIF para llamados a metodos de Entity
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
  public  interface GruposDAOIF : IGenericRepository<Grupo>
    {
        Mensaje agregar(Grupo entity, DBContextAdapter dbContext);

        Mensaje modificar(Grupo entity, DBContextAdapter dbContext);

        Mensaje eliminar(Grupo entity, DBContextAdapter dbContext);

        Mensaje getAllGrupos(DBContextAdapter dbContext);

        Mensaje getPorClaveGrupos(String clave, DBContextAdapter dbContext);

        Mensaje getPorIdGrupos(decimal? id, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosGrupo(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosGrupo(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje agregarListaGrupos(List<Grupo> entitys, int rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje deleteListQuerys(String tabla, String campo, Object[] valores, DBContextAdapter dbContext);

        Mensaje agregaGruposBaseAfectadas(Grupo entity, List<BaseAfectadaGrupo> eliminadasAfectadaGrupos, DBContextAdapter dbContext);
    }
}
