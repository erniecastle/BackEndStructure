

using Exitosw.Payroll.Core.util;
/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface CFDIReciboDAOIF para llamados a metodos de Entity
* -----------------------------------------------------------------------------
* MODIFICACIONES:
* -----------------------------------------------------------------------------
* Clave: 
* Autor: 
* Fecha:
* Descripción: 
* -----------------------------------------------------------------------------
*/
using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Entity.entidad.cfdi;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public interface CFDIReciboDAOIF
    {
        Mensaje agregar(CFDIRecibo entity, DBContextAdapter dbContext);

        Mensaje actualizar(CFDIRecibo entity, DBContextAdapter dbContext);

        Mensaje eliminar(CFDIRecibo entity, DBContextAdapter dbContext);

        Mensaje getAllCFDIRecibo(DBContextAdapter dbContext);

        Mensaje saveDeleteCFDIRecibo(List<CFDIRecibo> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosCFDIRecibo(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);
    }
}
