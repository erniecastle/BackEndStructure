using Exitosw.Payroll.Core.util;
/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface JornadaDAOIF para llamados a metodos de Entity
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
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
   public interface JornadaDAOIF:IGenericRepository<Jornada>
    {
        Mensaje getAllJornada(DBContextAdapter dbContext);
    }
}
