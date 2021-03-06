﻿using Exitosw.Payroll.Core.util;
/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface FamiliaresDAOIF para llamados a metodos de Entity
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
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public interface FamiliaresDAOIF
    {
        Mensaje agregar(Familiares entity, DBContextAdapter dbContext);

        Mensaje modificar(Familiares entity, DBContextAdapter dbContext);

        Mensaje eliminar(Familiares entity, DBContextAdapter dbContext);

        Mensaje getFamiliaresPorIDEmpleado(decimal id_empleado, DBContextAdapter dbContext);
    }
}
