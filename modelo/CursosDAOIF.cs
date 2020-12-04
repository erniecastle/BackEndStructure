using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Core.genericos.campos;
/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface CursosDAOIF para llamados a metodos de Entity
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
using System.Data.Entity;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public interface CursosDAOIF : IGenericRepository<Cursos>
    {

        Mensaje agregar(Cursos entity, DBContextAdapter dbContext);

        Mensaje actualizar(Cursos entity, DBContextAdapter dbContext);

        Mensaje eliminar(Cursos entity, DBContextAdapter dbContext);

        Mensaje getAllCurso(DBContextAdapter dbContext);

        Mensaje getPorClaveCurso(String clave, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosCursos(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosCursos(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje saveDeleteCursos(List<Cursos> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);

        Mensaje getPorIdCursos(decimal? idCursos, DBContextAdapter dbContext);
    }
}
