/**
* @author: Claudia Zavala 
* Fecha de Creación: 08/08/2020
* Compañía: Exito
* Descripción del programa: Interface BITCanxelacionDAOIF para llamados a metodos de Entity
* -----------------------------------------------------------------------------

*/

using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Core.genericos.campos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.Entity.entidad.cfdi;

namespace Exitosw.Payroll.Core.modelo
{
    public interface BITCancelacionDAOIF : IGenericRepository<BITCancelacion>
    {
        Mensaje agregar(BITCancelacion entity, DBContextAdapter dbContext);

        Mensaje modificar(BITCancelacion entity, DBContextAdapter dbContext);

        Mensaje eliminar(BITCancelacion entity, DBContextAdapter dbContext);
        Mensaje saveDeleteBITCancelacion(List<BITCancelacion> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);
    }
}
