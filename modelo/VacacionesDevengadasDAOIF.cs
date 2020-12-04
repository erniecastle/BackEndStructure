/**
* @author: Daniel Ruelas
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface VacacionesDevengadasDAOIF para llamados a metodos de Entity
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
    public interface VacacionesDevengadasDAOIF:IGenericRepository<VacacionesDevengadas>
    {
        Mensaje getAllVacacionesDevengadas(DBContextAdapter dbContext);

        Mensaje calcularVacacionesDevengadasEmpleados(RazonesSociales razonesSociales, object[,] factorIntegracion, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje getVacacionesDenvengadasPorEmpleado(String claveEmpleado, String claveRazonSocial, DBContextAdapter dbContext);

        Mensaje saveDeleteVacacionesDevengadas(List<VacacionesDevengadas> entitysCambios, int rango, DBContextAdapter dbContext);

        Mensaje getVacacionesDenvengadasPorEmpleadoJS(String claveEmpleado, String claveRazonSocial, DBContextAdapter dbContext);

        Mensaje agregar(VacacionesDevengadas entity, DBContextAdapter dbContext);
    }
}
