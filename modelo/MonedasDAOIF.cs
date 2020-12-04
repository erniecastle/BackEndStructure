/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface MonedasDAOIF para llamados a metodos de Entity
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
 public interface MonedasDAOIF:IGenericRepository<Monedas>
    {
        Mensaje agregar(Monedas entity, DBContextAdapter dbContext);

        Mensaje modificar(Monedas entity, DBContextAdapter dbContext);

        Mensaje eliminar(Monedas entity, DBContextAdapter dbContext);

        Mensaje getAllMonedas(DBContextAdapter dbContext);

        Mensaje getPorClaveMonedas(String clave, DBContextAdapter dbContext);


        Mensaje getPorIdMonedas(decimal? id, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosMonedas(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosMonedas(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje saveDeleteMonedas(List<Monedas> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);
    }
}
