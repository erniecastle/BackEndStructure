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
using System;
using Exitosw.Payroll.Entity.entidad;
using System.Data.Entity;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
   public interface TipoElementoDAOIF:IGenericRepository<TipoElemento>
    {
        Mensaje getAllTipoElemento(DBContextAdapter dbContextMaestra);

        Mensaje getTipoElemento(String nombre, DBContextAdapter dbContextMaestra);

        Mensaje agregar(TipoElemento entity, DBContextAdapter dbContextMaestra);

        Mensaje modificar(TipoElemento entity, DBContextAdapter dbContextMaestra);

        Mensaje eliminar(TipoElemento entity, DBContextAdapter dbContextMaestra);
    }
}
