/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface PermisosDAOIF para llamados a metodos de Entity
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
using System.Data.Entity;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
   public interface PermisosDAOIF:IGenericRepository<Permisos>
    {
        Mensaje getAllPermisos(DBContextAdapter dbContextMaestra);

        Mensaje agregar(Permisos entity, DBContextAdapter dbContextMaestra);

        Mensaje modificar(Permisos entity, DBContextAdapter dbContextMaestra);

        Mensaje eliminar(Permisos entity, DBContextAdapter dbContextMaestra);

        Mensaje getPermisosPorContenedor(Contenedor c, DBContextAdapter dbContextMaestra);

        Mensaje getPermisosPorPerfil(Perfiles perfil, DBContextAdapter dbContextMaestra);

        Mensaje getPermisosPorUsuario(Usuario user, DBContextAdapter dbContextMaestra);

        Mensaje getPermisosTipoAccesoyModulo(Usuario usuario, String nombreModulo, DBContextAdapter dbContextMaestra);

        Mensaje agregarListaPermisos(List<Permisos> entitys, int rango, DBContextAdapter dbContextMaestra);

        Mensaje getPermisosPorTipoVentanaySeccion(Object secion, TipoVentana[] tipoVentanas, DBContextAdapter dbContextMaestra);
    }
}
