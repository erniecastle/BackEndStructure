
using Exitosw.Payroll.Core.util;
/**
* @author: Daniel Ruelas
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface TipoElementoDAOIF para llamados a metodos de Entity
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
  public  interface TipoHerramientaDAOIF:IGenericRepository<TipoHerramienta>
    {
        
        Mensaje agregar(TipoHerramienta entity, DBContextAdapter dbContextMaestra);

        Mensaje modificar(TipoHerramienta entity, DBContextAdapter dbContextMaestra);

        Mensaje eliminar(TipoHerramienta entity, DBContextAdapter dbContextMaestra);

        Mensaje getTipoHerramienta(int id, DBContextAdapter dbContextMaestra);

        Mensaje getTipoHerramientaYHerramienta(DBContextAdapter dbContextMaestra);

        Mensaje getAllTipoHerramienta(DBContextAdapter dbContextMaestra);

    }
}
