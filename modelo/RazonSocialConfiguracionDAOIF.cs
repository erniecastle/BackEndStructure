/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface RazonSocialConfiguracionDAOIF para llamados a metodos de Entity
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
    public interface RazonSocialConfiguracionDAOIF:IGenericRepository<RazonSocialConfiguracion>
    {
        Mensaje getAllRazonSocialConfiguracion(DBContextAdapter dbContextMaestra);

        Mensaje getRazonSocialConfiguracionPorUsuario(int idUsuario, DBContextAdapter dbContextMaestra);

        Mensaje getRazonSocialConfiguracionPorClave(String clavesRazonSocial, String claveUsuario, DBContextAdapter dbContextMaestra);

        Mensaje getRazonSocialConfiguracionPorRazonSocial(String claveRazonSocial, DBContextAdapter dbContextMaestra);
    }
}
