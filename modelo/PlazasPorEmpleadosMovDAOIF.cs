/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface PlazasPorEmpleadosMovDAOIF para llamados a metodos de Entity
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
    public interface PlazasPorEmpleadosMovDAOIF : IGenericRepository<PlazasPorEmpleadosMov>
    {
        Mensaje agregar(PlazasPorEmpleadosMov entity, SalariosIntegrados salariosIntegrados, DBContextAdapter dbContext);

        Mensaje actualizar(PlazasPorEmpleadosMov entity, SalariosIntegrados salariosIntegrados, DBContextAdapter dbContext);

        Mensaje eliminar(PlazasPorEmpleadosMov entity, DBContextAdapter dbContext);

        Mensaje eliminarMovimientosPorPlaza(PlazasPorEmpleadosMov entity, List<int> movimientos, DBContextAdapter dbContext);

        Mensaje eliminarPlazasMovimientos(PlazasPorEmpleadosMov entity, DBContextAdapter dbContext);

        Mensaje getPlazasPorEmpleadosMovMaxPorEmpleado(String claveEmpleado, String razonSocial, DBContextAdapter dbContext);

        Mensaje getPlazasPorEmpleadosMovMaxPorEmpleadoYRegPatronal(decimal idEmpleado, decimal idRegPat, decimal idRazonSocial, DBContextAdapter dbContext);

        Mensaje getPlazasPorEmpleadosMovMaxPorClave(String clave, String razonSocial, DBContextAdapter dbContext);

        Mensaje getPlazasPorEmpleadosMovPorRazonSocial(String clave, DBContextAdapter dbContext);

        Mensaje getPlazasPorEmpleadosMovPorReferencia(String referencia, String claveRazonesSociales, int result, DBContextAdapter dbContext);

        Mensaje getPlazasPorEmpleadosMovPorReferenciaYRazonsocial(String referencia, decimal? idRazonSocial, decimal idPlazaPorEmpleMov, DBContextAdapter dbContext);

        Mensaje getPlazasPorEmpleadosMovAnterior(decimal id, String referencia, String claveRazonesSociales, int result, DBContextAdapter dbContext);

        Mensaje getPorEmpleadoYRazonSocialVigente(String claveEmpleado, String claveRazonSocial, DBContextAdapter dbContext);

        Mensaje getPorEmpleadoYRazonSocialFiniquitoVigente(String claveEmpleado, String claveRazonSocial, String claveFiniquito, DBContextAdapter dbContext);

        Mensaje getPorEmpleadoYRazonSocial(String claveEmpleado, String claveRazonSocial, DBContextAdapter dbContext);

        Mensaje getPorEmpleadoYRazonSocialYFecha(String claveEmpleado, String claveRazonSocial, DateTime fecha, DBContextAdapter dbContext);

        Mensaje getCantidadPlazasPorEmpleado(String claveEmpleado, String claveRazonSocial, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosPlazasPorEmpleadosMov(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosPlazasPorEmpleadosMov(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje agregarListaPlazasPorEmpleadosMovs(List<PlazasPorEmpleadosMov> entitys, int rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje deleteListQuerys(String tabla, String campo, Object[] valores, DBContextAdapter dbContext);

        Mensaje getEmpleadosManejaPagoPorHoras(String claveTipoNomina, String claveRazonSocial, DateTime fechaInicial, DateTime fechaFinal, DBContextAdapter dbContext);

        Mensaje getPorEmpleYRazonSocialVigente(decimal? idEmpleado, String claveRazonSocial, DBContextAdapter dbContext);

        Mensaje getPorClaveEmpleYRazonSocialVigente(String claveEmpleado, String claveRazonSocial, DBContextAdapter dbContext);

        Mensaje getPorEmpleadoYRazonSocialYFechaJS(String claveEmpleado, String claveRazonSocial, DateTime? fecha, DBContextAdapter dbContext);

    }
}
