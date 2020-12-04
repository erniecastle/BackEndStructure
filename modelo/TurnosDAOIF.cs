/**
* @author: Daniel Ruelas
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface TurnosDAOIF para llamados a metodos de Entity
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
    public interface TurnosDAOIF
    {
        Mensaje agregar(Turnos entity, DBContextAdapter dbContext);

        Mensaje modificar(Turnos entity, DBContextAdapter dbContext);

        Mensaje eliminar(Turnos entity, DBContextAdapter dbContext);

        Mensaje UpdateTurnos(Turnos entity, Object[] eliminados, DBContextAdapter dbContext);

        Mensaje getAllTurnos(String claveRazonesSocial, DBContextAdapter dbContext);

        Mensaje getPorClaveTurnos(String clave, DBContextAdapter dbContext);

        Mensaje getPorIdTurnos(decimal? id,DBContextAdapter dbContext);
    }
}
