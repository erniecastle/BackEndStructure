/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface RegistroIncapacidadDAOIF para llamados a metodos de Entity
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
using Exitosw.Payroll.Core.genericos.campos;
using System.Data.Entity;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
   public interface RegistroIncapacidadDAOIF
    {
        Mensaje agregar(RegistroIncapacidad entity, DBContextAdapter dbContext);

        Mensaje modificar(RegistroIncapacidad entity, DBContextAdapter dbContext);

        Mensaje eliminar(RegistroIncapacidad entity, DBContextAdapter dbContext);

        Mensaje getAllRegistroIncapacidad(String claveRazonesSocial, DBContextAdapter dbContext);

        Mensaje getRegistroIncapacidadPorClave(String clave, String claveRazonesSocial, DBContextAdapter dbContext);

        Mensaje getRegistroIncapacidadPorClaveYRazon(String clave, String claveRazon, DBContextAdapter dbContext);

        Mensaje getRegistroIncapacidadPorEmpleado(Empleados empleado, DBContextAdapter dbContext);

        Mensaje getIncapacidadPorEmpleadoYFecha(String claveEmpleado, DateTime fechaInicial, DateTime fechaFinal, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosRegistroIncapacidad(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosRegistroIncapacidad(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje saveDeleteRegistroIncapacidad(RegistroIncapacidad incapacidad, Object[] clavesDeleteIncapacidad, int rango, Empleados empleados, RazonesSociales razonesSociales, DateTime fechaInicial, DateTime fechaFinal, DateTime fechaInicialAnterior, DateTime fechaFinalAnterior, Object claveExcepcion, String formatoFecha, DateTime fechaInicEmpalme, DateTime fechaFinEmpalme, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje getRegistroIncapacidadPorID(decimal id,  DBContextAdapter dbContext);

        Mensaje getRegistroIncapacidadPorEmpleadoID(decimal idempleado, decimal idRazon, DBContextAdapter dbContext);
    }
}
