/**
* @author: Daniel Ruelas
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface SistemasDAOIF para llamados a metodos de Entity
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
   public interface SistemasDAOIF:IGenericRepository<Sistemas>
    {
        Mensaje agregar(Sistemas entity, DBContextAdapter dbContextMaestra);

        Mensaje modificar(Sistemas entity, DBContextAdapter dbContextMaestra);

        Mensaje eliminar(Sistemas entity, DBContextAdapter dbContextMaestra);

        Mensaje getAllSistemas(DBContextAdapter dbContextMaestra);

        Mensaje getPorClaveSistemas(String clave, DBContextAdapter dbContextMaestra);

        Mensaje getPorIdSistemas(decimal? id ,DBContextAdapter dbContextMaestra);

        Mensaje consultaPorFiltrosSistemas(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosSistemas(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContextMaestra);

        Mensaje saveDeleteSistemas(List<Sistemas> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContextMaestra);
    }
}
