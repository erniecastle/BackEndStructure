/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface ModuloDAOIF para llamados a metodos de Entity
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
    public interface ModuloDAOIF:IGenericRepository<Modulo>
    {
         Mensaje getAllModulo(DBContextAdapter dbContext);

         Mensaje getModuloPorNombre(String nombreModulo, DBContextAdapter dbContext);

         Mensaje agregar(Modulo entity, DBContextAdapter dbContext);

         Mensaje actualizar(Modulo entity, DBContextAdapter dbContext);

         Mensaje eliminar(Modulo entity, DBContextAdapter dbContext);

         Mensaje getModuloPorSistemas(String id, DBContextAdapter dbContext);

         Mensaje getPorClaveModulo(String clave, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosModulo(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosModulo(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje saveDeleteModulo(List<Modulo> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);
    }
}
