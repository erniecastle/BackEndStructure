/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface PerfilDAOIF para llamados a metodos de Entity
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
    public interface PerfilDAOIF:IGenericRepository<Perfiles>
    {
        Mensaje getAllPerfiles(DBContextAdapter dbContextMaestra);

        Mensaje getPorClavePerfiles(String clave, DBContextAdapter dbContextMaestra);

        Mensaje getPorIdPerfiles(decimal? id ,DBContextAdapter dbContextMaestra);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContextMaestra);

        Mensaje savePerfilMenusPermisos(Perfiles entity, List<Object> menus, List<Permisos> permisos, DBContextAdapter dbContextMaestra);

        Mensaje deletePerfilMenusPermisos(Perfiles entity, List<Permisos> permisos, DBContextAdapter dbContextMaestra);
    }
}
