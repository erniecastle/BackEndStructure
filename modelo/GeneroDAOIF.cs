/**
 * @author: Daniel Ruelas
 * Fecha de Creación: 17/01/2018
 * Compañía: Exito Software
 * Descripción del programa: Interface GeneroDAOIF para llamados a metodos de Entity
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
    public interface GeneroDAOIF : IGenericRepository<Genero>
    {
        Mensaje agregar(Genero entity, DBContextAdapter dbContext);

        Mensaje modificar(Genero entity, DBContextAdapter dbContext);

        Mensaje eliminar(Genero entity, DBContextAdapter dbContext);

        Mensaje getAllGenero(DBContextAdapter dbContext);

        Mensaje getPorClaveGenero(String clave, DBContextAdapter dbContext);

        Mensaje getPorIdGenero(decimal? id, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosGenero(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosGenero(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje saveDeleteGenero(List<Genero> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);
    }
}
