/**
* @author: Daniel Ruelas
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface UsuarioDAOIF para llamados a metodos de Entity
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
using System.Collections.Generic;

namespace Exitosw.Payroll.Core.modelo
{
    public interface UsuarioDAOIF : IGenericRepository<Usuario>
    {
        Mensaje getAllUsuario(DBContextAdapter dbContextMaestra);

        Mensaje agregar(Usuario entity, DBContextAdapter dbContextMaestra);

        Mensaje modificar(Usuario entity, DBContextAdapter dbContextMaestra);

        Mensaje eliminar(Usuario entity, DBContextAdapter dbContextMaestra);

        Mensaje getPorClaveUsuario(String clave, DBContextAdapter dbContextMaestra);

        Mensaje getPorIdUsuario(decimal? id, DBContextAdapter dbContextMaestra);

        Mensaje getAccesoCorrecto(String apodo, String password, DBContextAdapter dbContextMaestra);

        Mensaje getAccesoCorrectoConRazonSocialYRazonesSociales(String apodo, String password, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje setLastConfiguraciones(decimal? idiUsuario, Dictionary<string, int> lastAcces, DBContextAdapter dbContextMaestra);


        Mensaje getLastConfiguraciones(decimal? idiUsuario, DBContextAdapter dbContextMaestra);

        Mensaje getClaveUser(decimal? id, DBContextAdapter dbContextMaestra);

    }
}
