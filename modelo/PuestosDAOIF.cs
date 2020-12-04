/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface PuestosDAOIF para llamados a metodos de Entity
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
    public interface PuestosDAOIF:IGenericRepository<Puestos>
    {
        Mensaje agregar(Puestos entity, DBContextAdapter dbContext);

        Mensaje modificar(Puestos entity, DBContextAdapter dbContext);

        Mensaje eliminar(Puestos entity, DBContextAdapter dbContext);

        Mensaje getAllPuestos(DBContextAdapter dbContext);

        Mensaje getPorClavePuestos(String clave, DBContextAdapter dbContext);

        Mensaje getPorIdPuestos(decimal? id,DBContextAdapter dbContext);

        Mensaje SavePuesto(List<PercepcionesFijas> agrega, Object[] eliminados, Puestos entity, DBContextAdapter dbContext);

        Mensaje DeletePuesto(Puestos entity, DBContextAdapter dbContext);

        Mensaje UpdatePuesto(List<PercepcionesFijas> agrega, Object[] eliminados, Puestos entity, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosPuestos(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosPuestos(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje agregarListaPuestos(List<Puestos> entitys, int rango, DBContextAdapter dbContext);

        Mensaje deleteListQuerys(String tabla, String campo, Object[] valores, DBContextAdapter dbContext);
    }
}
