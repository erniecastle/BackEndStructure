/**
* @author: Daniel Ruelas
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface ReporteOrdenGrupoDAOIF para llamados a metodos de Entity
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
   public  interface ReporteOrdenGrupoDAOIF
    {
        Mensaje agregar(ReporteOrdenGrupo entity, DBContextAdapter dbContextMaestra);

        Mensaje actualizar(ReporteOrdenGrupo entity, DBContextAdapter dbContextMaestra);

        Mensaje eliminar(ReporteOrdenGrupo entity, DBContextAdapter dbContextMaestra);

        Mensaje getAllReporteOrdenGrupo(DBContextAdapter dbContextMaestra);

        Mensaje getPorClaveReporteOrdenGrupo(String clave, DBContextAdapter dbContextMaestra);

        Mensaje saveDeleteReporteOrdenGrupo(List<ReporteOrdenGrupo> entitysCambios, Object[] eliminados, DBContextAdapter dbContextMaestra);

        Mensaje consultaPorFiltrosReporteOrdenGrupo(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosReporteOrdenGrupo(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContextMaestra);

        Mensaje actualizaListaPorCampos(String[] campoModificar, Object[] valoresModificado, String[] camposWhere, Object[] valoresWhere, DBContextAdapter dbContextMaestra);
    }
}
