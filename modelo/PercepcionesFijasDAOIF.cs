/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface PercepcionesFijasDAOIF para llamados a metodos de Entity
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
   public interface PercepcionesFijasDAOIF:IGenericRepository<PercepcionesFijas>
    {
        Mensaje agregar(PercepcionesFijas entity, DBContextAdapter dbContext);

        Mensaje actualizar(PercepcionesFijas entity, DBContextAdapter dbContext);

        Mensaje eliminar(PercepcionesFijas entity, DBContextAdapter dbContext);

        Mensaje getAllPercepcionesFijas(DBContextAdapter dbContext);

        Mensaje getPercepcionesFijasPorCategoriasPuestos(CategoriasPuestos c, DBContextAdapter dbContext);

        Mensaje getPercepcionesFijasPorPuestos(Puestos puestos, DBContextAdapter dbContext);

        Mensaje getPercepcionesFijasPorIDCategoriaPuesto(decimal clave, DBContextAdapter dbContext);

        Mensaje getPercepcionesFijasPorIDPuesto(decimal clave, DBContextAdapter dbContext);

        Mensaje getPercepcionesFijasPorId(decimal id, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosPercepcionesFijas(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosPercepcionesFijas(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje agregarListaPercepcionesFijas(List<PercepcionesFijas> entitys, int rango, DBContextAdapter dbContext);

        Mensaje deleteListQuerys(String tabla, String campo, Object[] valores, DBContextAdapter dbContext);
    }
}
