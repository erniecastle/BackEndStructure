/**
* @author: Daniel Ruelas
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface SemaforoTimbradoPacDAOIF para llamados a metodos de Entity
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exitosw.Payroll.Entity.entidad;
using System.Data.Entity;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public interface SemaforoTimbradoPacDAOIF : IGenericRepository<SemaforoTimbradoPac>
    {
        Mensaje agregar(SemaforoTimbradoPac entity, DBContextAdapter dbContext);

        Mensaje eliminar(SemaforoTimbradoPac entity, DBContextAdapter dbContext);
    }
}
