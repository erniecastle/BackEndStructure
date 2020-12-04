

using Exitosw.Payroll.Core.util;
/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface CalculoPtuDAOIF para llamados a metodos de Entity
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
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public interface CalculoPtuDAOIF
    {
        Mensaje guardarCargaAcumulados(PtuDatosGenerales ptuDatosGenerales, List<PtuEmpleados> ptuEmpleados, DBContextAdapter dbContext);

        Mensaje cargaDeAcumulados(int ejercicio, String claveRazonsocial, int diasPorDerechoPTU, bool cumplenPtu, DBContextAdapter dbContext);

        Mensaje ptuDatosGeneralesPorEjercioyEmpresa(int ejercicio, String claveRazonsocial, DBContextAdapter dbContext);

        Mensaje ptuEmpleadosPorEjercioyEmpresa(int ejercicio, String claveRazonsocial, DBContextAdapter dbContext);

        Mensaje calculoPtu(PtuDatosGenerales ptuDatosGenerales, List<PtuEmpleados> ptuEmpleados, Double cantidadRepartir, Object[] totales, DBContextAdapter dbContext);

    }
}
