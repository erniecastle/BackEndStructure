

using Exitosw.Payroll.Core.util;
/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface AsistenciasDAOIF para llamados a metodos de Entity
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
    public interface AsistenciasDAOIF : IGenericRepository<Object>
    {
        Mensaje getAllAsistencias(String claveRazonesSociales, DBContextAdapter dbContext);

        Mensaje saveDeleteAsistencias(List<Asistencias> AgreModif, List<Asistencias> Ordenados, Object[] clavesDelete,
                List<DetalleAsistencia> AgreModifDet, Object[] clavesDeleteDet, List<RegistroIncapacidad> incapacidades, Object[] clavesDeleteIncapacidades, DBContextAdapter dbContext);

        Mensaje getAsistenciasPorRangoFechas(String claveEmpleado, DateTime fechaInicio, DateTime fechaFinal, String claveRazonesSociales, DBContextAdapter dbContext);

        Mensaje getAsistenciaPorFiltros(string clavetipoNomina, decimal idPeriodoNomina, string claveEmpleado, string claveRazonSocial, string claveCentroCosto, string claveExepcion, DBContextAdapter dbcontext);

        Mensaje saveDeleteAsist(List<Asistencias> AgreModif, Object[] clavesDelete,
               List<DetalleAsistencia> AgreModifDet, Object[] clavesDeleteDet, DBContextAdapter dbContext);

        Mensaje getAsistenciaPorFiltrosIDS(decimal clavetipoNomina, decimal idPeriodoNomina, decimal claveEmpleado, decimal claveRazonSocial, decimal claveCentroCosto, int claveExepcion, DBContextAdapter dbcontext);

        Mensaje getDetalleAsistenciaPorFiltrosIDS(decimal clavetipoNomina, decimal idPeriodoNomina, decimal claveEmpleado, decimal claveRazonSocial, decimal claveCentroCosto,  DBContextAdapter dbcontext);

        Mensaje buscaAsistenciaExitente(DateTime fecha,string claveEmpleado,string claveTipoNomina, string clavePeriodo, string claveRazon, DBContextAdapter dbcontext);
    }
}
