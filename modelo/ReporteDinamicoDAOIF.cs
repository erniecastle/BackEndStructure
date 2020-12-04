/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface ReporteDinamicoDAOIF para llamados a metodos de Entity
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
   public interface ReporteDinamicoDAOIF
    {
        Mensaje agregar(ReporteDinamico entity, DBContextAdapter dbContextMaestra);

        Mensaje actualizar(ReporteDinamico entity, DBContextAdapter dbContextMaestra);

        Mensaje eliminar(ReporteDinamico entity, DBContextAdapter dbContextMaestra);

        Mensaje getAllReporteDinamico(DBContextAdapter dbContextMaestra);

        Mensaje getReporteDinamicoAllEspecificos(DBContextAdapter dbContextMaestra);

        Mensaje getReporteDinamicoPorFuenteYGrupo(String fuenteDatos, int idContenedor, DBContextAdapter dbContextMaestra);

        Mensaje getReporteDinamicoPorContenedor(int contenedorID, DBContextAdapter dbContextMaestra);

        Mensaje getPorClaveReporteDinamico(String clave, DBContextAdapter dbContextMaestra);

        Mensaje getReporteDinamicoPorID(decimal idReporte, DBContextAdapter dbContextMaestra);

        Mensaje saveDeleteReporteDinamico(ReporteDinamico entity, Object[] eliminarDatosConsulta, Object[] eliminarDatosIncluir, Object[] eliminarDatosRepOpcGrupo,
                Object[] eliminarDatosOrdenGrupo, Object[] eliminarCamposWhere, Object[] eliminarCamposEncabezados, Object[] eliminarDatosResumen, Object[] eliminarReporteEstilos, Contenedor contenedorGrupoMenu, DBContextAdapter dbContextMaestra);

        Mensaje consultaPorFiltrosReporteDinamico(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosReporteDinamico(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContextMaestra);

        Mensaje actualizaListaPorCampos(String[] campoModificar, Object[] valoresModificado, String[] camposWhere, Object[] valoresWhere, DBContextAdapter dbContextMaestra);

        Mensaje eliminarEspecifico(decimal idReporte, DBContextAdapter dbContextMaestra);
    }
}
