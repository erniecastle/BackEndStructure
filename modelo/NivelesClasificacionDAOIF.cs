/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface NivelesClasificacionDAOIF para llamados a metodos de Entity
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
   public interface NivelesClasificacionDAOIF:IGenericRepository<NivelesClasificacion>
    {
        Mensaje agregar(NivelesClasificacion entity, DBContextAdapter dbContext);

        Mensaje modificar(NivelesClasificacion entity, DBContextAdapter dbContext);

        Mensaje eliminar(NivelesClasificacion entity, DBContextAdapter dbContext);

        Mensaje getAllNivelesClasificacion(DBContextAdapter dbContext);

        Mensaje getPorClaveNivelesClasificacion(String clave, DBContextAdapter dbContext);

        Mensaje getPorIdNivelesClasificacion(decimal? id ,DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosNivelesClasificacion(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosNivelesClasificacion(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje saveDeleteNivelesClasificacion(List<NivelesClasificacion> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);
    }
}
