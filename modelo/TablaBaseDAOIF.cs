/**
* @author: Daniel Ruelas
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface TablaBaseDAOIF para llamados a metodos de Entity
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
    public interface TablaBaseDAOIF:IGenericRepository<TablaBase>
    {
        Mensaje getAllTablaBase(DBContextAdapter dbContextMaestra);

        Mensaje getPorClaveTablaBase(String clave, DBContextAdapter dbContextMaestra);

        Mensaje getPorIdTablaBase(decimal? id,DBContextAdapter dbContextMaestra);

        Mensaje agregar(TablaBase entity, DBContextAdapter dbContextMaestra);

        Mensaje modificar(TablaBase entity, DBContextAdapter dbContextMaestra);

        Mensaje eliminar(TablaBase tablaPersonalizada, DBContextAdapter dbContextMaestra);

        Mensaje getTablaBasePorTipoTabla(TipoTabla tipoTabla, DBContextAdapter dbContextMaestra);

        Mensaje getTablaBaseSistema(DBContextAdapter dbContextMaestra);
    }
}
