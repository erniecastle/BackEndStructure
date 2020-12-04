/**
* @author: Daniel Ruelas
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface TablaPersonalizadaDAOIF para llamados a metodos de Entity
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
   public interface TablaPersonalizadaDAOIF:IGenericRepository<TablaPersonalizada>
    {
        Mensaje getAllTablaPersonalizada(DBContextAdapter dbContextMaestra);

        Mensaje getPorClaveTablaPersonalizada(String clave, DBContextAdapter dbContextMaestra);

        Mensaje getPorIdTablaPersonalizada(decimal? id,DBContextAdapter dbContextMaestra);

        Mensaje agregar(TablaPersonalizada entity, DBContextAdapter dbContextMaestra);

        Mensaje modificar(TablaPersonalizada entity, DBContextAdapter dbContextMaestra);

        Mensaje eliminar(TablaPersonalizada tablaPersonalizada, DBContextAdapter dbContextMaestra);

        Mensaje existeClaveTablaPersonalizada(String clave, DBContextAdapter dbContextMaestra);
    }
}
