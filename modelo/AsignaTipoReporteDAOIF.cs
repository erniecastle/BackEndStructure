

using Exitosw.Payroll.Core.genericos.campos;
/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface AsignaTipoReporteDAOIF para llamados a metodos de Entity
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
using System.Data.Entity;
using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public interface AsignaTipoReporteDAOIF : IGenericRepository<AsignaTipoReporte>
    {

        Mensaje agregar(AsignaTipoReporte entity, DBContextAdapter dbContext);

        Mensaje actualizar(AsignaTipoReporte entity, DBContextAdapter dbContext);

        Mensaje eliminar(AsignaTipoReporte entity, DBContextAdapter dbContext);

        Mensaje getAsignaTipoReporteAll(DBContextAdapter dbContext);

        Mensaje getAsignaPorTipoReporteDinamico(String claveReporteDinamico, DBContextAdapter dbContext);

        Mensaje getAsignaPorTipoReporte(TipoReporte tipoReporte, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        // Mensaje existeDato(string tabla, CamposWhere campoWhere, Conexion conexion);

        Mensaje saveDeleteAsignaTipoReporte(List<AsignaTipoReporte> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);
    }
}
