/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface MovNominaBaseAfectaDAOIF para llamados a metodos de Entity
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
    public interface MovNominaBaseAfectaDAOIF:IGenericRepository<MovNomBaseAfecta>
    {
        Mensaje getAllMovNominaBaseAfecta(DBContextAdapter dbContext);

        Mensaje getMovNominaBaseAfectaAsc(DBContextAdapter dbContext);

        Mensaje saveDeleteMovNominaBaseAfecta(List<MovNomBaseAfecta> AgreModif, List<MovNomBaseAfecta> Ordenados, Object[] clavesDelete, DBContextAdapter dbContext);
    }
}
