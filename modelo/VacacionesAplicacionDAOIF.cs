/**
* @author: Daniel Ruelas
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface VacacionesAplicacionDAOIF para llamados a metodos de Entity
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
    public interface VacacionesAplicacionDAOIF
    {
        Mensaje agregar(VacacionesAplicacion entity, DBContextAdapter dbContext);

        Mensaje modificar(VacacionesAplicacion entity, DBContextAdapter dbContext);

        Mensaje eliminar(VacacionesAplicacion entity, DBContextAdapter dbContext);

        Mensaje getVacacionesAplicacionAll(String claveRazonesSocial, DBContextAdapter dbContext);

        Mensaje getVacacionesPorEmpleado(String claveEmpleado, String claveRazonSocial, DBContextAdapter dbContext);

        Mensaje saveDeleteVacacionesAplicacion(List<VacacionesAplicacion> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);

        Mensaje getVacacionesPorEmpleadoJS(String claveEmpleado, String claveRazonSocial, DBContextAdapter dbContext);

        Mensaje SaveDeleteVacAplicacionJS(List<VacacionesAplicacion> entitysCambios, Object[] clavesDelete, List<VacacionesDevengadas> entytysDevengada, DBContextAdapter dbContext);

        Mensaje SaveDeleteVacAplicacionJSP(List<VacacionesAplicacion> entitysCambios, List<VacacionesAplicacion> entitysDelete, DBContextAdapter dbContext);

        Mensaje getVacacionesAplicacionAllJS(DBContextAdapter dbContext);
    }
}
