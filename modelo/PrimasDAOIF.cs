/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface PrimasDAOIF para llamados a metodos de Entity
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
    public interface PrimasDAOIF
    {
        Mensaje agregar(Primas entity, DBContextAdapter dbContext);

        Mensaje modificar(Primas entity, DBContextAdapter dbContext);

        Mensaje eliminar(Primas entity, DBContextAdapter dbContext);

        Mensaje getAllPrimas(DBContextAdapter dbContext);

        Mensaje getPrimasPorClaveRegistroPatronal(decimal clave, DBContextAdapter dbContext);

        Mensaje DeletePrimasByRegistroPatronal(decimal registroPatronal, DBContextAdapter dbContext);

        Mensaje SavePrimas(List<Primas> agrega, List<Primas> elimina, RegistroPatronal r, DBContextAdapter dbContext);

        Mensaje DeletePrimas(List<Primas> p, DBContextAdapter dbContext);

        Mensaje deleteListClavesPrimas(Object[] valores, DBContextAdapter dbContext);
    }
}
