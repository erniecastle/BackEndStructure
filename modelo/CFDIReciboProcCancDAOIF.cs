using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Core.genericos.campos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.Entity.entidad.cfdi;
namespace Exitosw.Payroll.Core.modelo
{
     

    public interface CFDIReciboProcCancDAOIF : IGenericRepository<CFDIReciboProcCanc>
    {
        Mensaje agregar(CFDIReciboProcCanc entity, DBContextAdapter dbContext);

        Mensaje modificar(CFDIReciboProcCanc entity, DBContextAdapter dbContext);

        Mensaje eliminar(CFDIReciboProcCanc entity, DBContextAdapter dbContext);
        Mensaje saveDeleteCFDIRecibo_Proc_Canc(List<CFDIReciboProcCanc> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);
    }
}
