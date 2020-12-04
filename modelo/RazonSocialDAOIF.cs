/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface RazonSocialDAOIF para llamados a metodos de Entity
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
  public interface RazonSocialDAOIF:IGenericRepository<RazonSocial>
    {
        Mensaje agregar(RazonSocial entity, DBContextAdapter dbContextMaestra);

        Mensaje modificar(RazonSocial entity, DBContextAdapter dbContextMaestra);

        Mensaje eliminar(RazonSocial entity, DBContextAdapter dbContextMaestra);

        Mensaje getAllRazonSocial(DBContextAdapter dbContextMaestra);

        Mensaje getPorClaveRazonSocial(String claves, DBContextAdapter dbContextMaestra);

        Mensaje getPorIdRazonSocial(decimal? id , DBContextAdapter dbContextMaestra);
    }
}
