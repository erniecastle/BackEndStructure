/**
* @author: Ernesto Castillo 
* Fecha de Creación: 29/07/2020
* Compañía: Exito
* Descripción del programa: Interface AparienciaDAOIF para llamados a metodos de Entity
* -----------------------------------------------------------------------------
* MODIFICACIONES:
* -----------------------------------------------------------------------------
* Clave: 
* Autor: 
* Fecha:
* Descripción: 
* -----------------------------------------------------------------------------
*/

using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.Entity.entidad;
using System;

namespace Exitosw.Payroll.Core.modelo
{

    public interface AparienciaDAOIF : IGenericRepository<Apariencia>
    {
        Mensaje agregar(Apariencia entity, DBContextAdapter dbContext);

        Mensaje modificar(Apariencia entity, string keyUser, int idRazonSocial, DBContextAdapter dbContextMaster, DBContextAdapter dbContextSimple);

        Mensaje eliminar(Apariencia entity, DBContextAdapter dbContext);

        Mensaje getActualApariencia(string keyUser, int idRazonSocial, DBContextAdapter dbContextMaster, DBContextAdapter dbContextSimple);

    }
}
