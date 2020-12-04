/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface PeriodicidadDAOIF para llamados a metodos de Entity
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
  public interface PeriodicidadDAOIF
    {
        Mensaje getAllPeriodicidad(DBContextAdapter dbContext);

        Mensaje getPorClavePeriodicidad(String clave, DBContextAdapter dbContext);

        Mensaje getPorIdPeriodicidad(decimal? id, DBContextAdapter dbContext);

        Mensaje agregar(Periodicidad entity, DBContextAdapter dbContext);

        Mensaje modificar(Periodicidad entity, DBContextAdapter dbContext);

        Mensaje eliminar(Periodicidad entity, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosPeriodicidad(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosPeriodicidad(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje saveDeletePeriodicidad(List<Periodicidad> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);
    }
}
