/**
* @author: Daniel Ruelas
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface VacacionesDisfrutadasDAOIF para llamados a metodos de Entity
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
    public interface VacacionesDisfrutadasDAOIF
    {
        Mensaje agregar(VacacionesDisfrutadas entity, DBContextAdapter dbContext);

        Mensaje modificar(VacacionesDisfrutadas entity, DBContextAdapter dbContext);

        Mensaje eliminar(VacacionesDisfrutadas entity, DBContextAdapter dbContext);

        Mensaje getVacacionesDisfrutadasAll(String claveRazonesSocial, DBContextAdapter dbContext);

        Mensaje getVacacionesPorEmpleado(String claveEmpleado, String claveRazonSocial, DBContextAdapter dbContext);

        Mensaje saveDeleteVacacionesDisfrutadas(List<VacacionesDisfrutadas> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);

        Mensaje ObtenerVacacionesDisfruradasMaxima(String claveEmpleado, String claveRazonSocial, DBContextAdapter dbContext);

        Mensaje EliminarVacacionesDisfrutadas(List<VacacionesAplicacion> vacAplicacion, DBContextAdapter dbContext);

        Mensaje getVacacionesPorEmpleadoJS(String claveEmpleado, String claveRazonSocial, DBContextAdapter dbContext);

        Mensaje ObtenerVacacionesDisfruradasMaximaJS(decimal claveEmpleado, String claveRazonSocial, DBContextAdapter dbContext);
    }
}
