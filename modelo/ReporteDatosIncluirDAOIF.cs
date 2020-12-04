/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface ReporteDatosIncluirDAOIF para llamados a metodos de Entity
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
    public interface ReporteDatosIncluirDAOIF
    {
        Mensaje agregar(ReporteDatosIncluir entity, DBContextAdapter dbContextMaestra);

        Mensaje actualizar(ReporteDatosIncluir entity, DBContextAdapter dbContextMaestra);

        Mensaje eliminar(ReporteDatosIncluir entity, DBContextAdapter dbContextMaestra);

        Mensaje getAllReporteDatosIncluir(DBContextAdapter dbContextMaestra);

        Mensaje getPorClaveReporteDatosIncluir(String clave, DBContextAdapter dbContextMaestra);

        Mensaje saveDeleteReporteDatosIncluir(List<ReporteDatosIncluir> entitysCambios, Object[] eliminados, DBContextAdapter dbContextMaestra);

        Mensaje consultaPorFiltrosReporteDatosIncluir(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosReporteDatosIncluir(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContextMaestra);

        Mensaje actualizaListaPorCampos(String[] campoModificar, Object[] valoresModificado, String[] camposWhere, Object[] valoresWhere, DBContextAdapter dbContextMaestra);
    }
}
