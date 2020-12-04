/**
* @author: Daniel Ruelas
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface ReporteFuenteDatosDAOIF para llamados a metodos de Entity
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
    public interface ReporteFuenteDatosDAOIF
    {
        Mensaje agregar(ReporteFuenteDatos entity, DBContextAdapter dbContextMaestra);

        Mensaje actualizar(ReporteFuenteDatos entity, DBContextAdapter dbContextMaestra);

        Mensaje eliminar(ReporteFuenteDatos entity, DBContextAdapter dbContextMaestra);

        Mensaje getAllReporteFuenteDatos(DBContextAdapter dbContextMaestra);

        Mensaje getPorClaveReporteFuenteDatos(String clave, DBContextAdapter dbContextMaestra);

        Mensaje saveDeleteReporteFuenteDatos(List<ReporteFuenteDatos> entitysCambios, Object[] eliminados, DBContextAdapter dbContextMaestra);

        Mensaje consultaPorFiltrosReporteFuenteDatos(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosReporteFuenteDatos(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContextMaestra);

        Mensaje actualizaListaPorCampos(String[] campoModificar, Object[] valoresModificado, String[] camposWhere, Object[] valoresWhere, DBContextAdapter dbContextMaestra);
    }
}
