/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface EstudiosDAOIF para llamados a metodos de Entity
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
    public interface EstudiosDAOIF : IGenericRepository<Estudios>
    {
        Mensaje agregar(Estudios entity, DBContextAdapter dbContext);

        Mensaje actualizar(Estudios entity, DBContextAdapter dbContext);

        Mensaje eliminar(Estudios entity, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosEstudios(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosEstudios(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje saveDeleteEstudios(List<Estudios> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);

        Mensaje getPorIdEstudios(decimal? idEstudios, DBContextAdapter dbContext);

    }
}
