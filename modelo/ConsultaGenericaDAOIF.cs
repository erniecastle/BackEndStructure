/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface ConsultaGenericaDAOIF para llamados a metodos de Entity
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
using Exitosw.Payroll.Core.genericos.campos;
using System;
using System.Collections.Generic;
using Exitosw.Payroll.Core.campos;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public interface ConsultaGenericaDAOIF : IGenericRepository<Object>
    {
        Mensaje getDataAll(String tabla, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje getDataAllFiltro(String tabla, List<CamposWhere> camposWhere, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje consultaPorFiltrosGenerico(string tabla, List<CamposWhere> camposWhere, ValoresRango valoresRango, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje consultaPorRangos(String tabla, ValoresRango valoresRango, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje consultaPorRangosFiltro(String tabla, ValoresRango valoresRango, List<CamposWhere> camposWhere, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje consultaPorRangosFiltros(String tabla, ValoresRango valoresRango, List<CamposWhere> camposWhere, CamposSelect campoSelect, CamposOrden camposOrden, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);//JEVC

        Mensaje existeDatoGenerico(String tabla, CamposWhere campoWhere, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje existeDatoList(String[] tabla, List<CamposWhere> camposWhere, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje existeValoresEntidad(String tabla, List<CamposWhere> camposWhere, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje existeClaveGenerico(String tabla, List<CamposWhere> camposWhere, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje selectExisteClave(String tabla, CamposSelect campoSelect, List<CamposWhere> camposWhere, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje existenClavesGenerico(String tabla, List<CamposWhere> camposWhere, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje existenClavesConOrden(String tabla, List<CamposWhere> camposWhere, CamposOrden campoOrden, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje obtenerClaveStringMax(String tabla, String campoEvaluar, List<CamposWhere> camposWhere, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje obtenerClaveNumericaMax(String tabla, String campoEvaluar, List<CamposWhere> camposWhere, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        DateTime getFechaSistema();

        Mensaje consultaPorRangosConOrdenado(String tabla, ValoresRango valoresRango, CamposOrden camposOrden, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje consultaAllConOrdenado(String tabla, List<CamposWhere> camposWhere, CamposOrden camposOrden, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje consultaAllConOrdenado(String tabla, CamposOrden camposOrden, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje consultaPorRangoConFiltroYOrdenado(String tabla, ValoresRango valoresRango, List<CamposWhere> camposWhere, CamposOrden camposOrden, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje getObject(String tabla, CamposSelect campoSelect, List<CamposWhere> camposWhere, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);
    }
}
