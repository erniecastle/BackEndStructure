/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface MovimientosNominaDAOIF para llamados a metodos de Entity
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
using Exitosw.Payroll.Entity.entidad.cfdi;
using System.Data.Entity;
using Exitosw.Payroll.Core.genericos.campos;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
  public interface MovimientosNominaDAOIF :IGenericRepository<MovNomConcep>
    {
        Mensaje getAllMovimientosNomina(DBContextAdapter dbContext);

        Mensaje getMovimientosNominaAsc(DBContextAdapter dbContext);

        Mensaje getMaxNumeroMovimientoPorTipoNominaYPeriodo(String claveTipoNomina, decimal idPeriodo, DBContextAdapter dbContext);

        Mensaje saveDeleteMovimientosNomina(List<MovNomConcep> AgreModif, Object[] clavesDelete, Object[] valoresReestablecer, bool incluirEliminadoDiferenteTipoPantalla100, DBContextAdapter dbContext);

        Mensaje getMovimientosPorPlazaEmpleado(Object[] clavesPlazasPorEmpleado, String claveTipoCorrida, String claveRazonSocial, DBContextAdapter dbContext);

        Mensaje eliminaListaMovimientos(String campo, Object[] valores, List<CFDIEmpleado> valoresCFDI, Object[] valoresCalculoUnidades, Object[] valoresReestablecer, bool incluirEliminadoDiferenteTipoPantalla100, DBContextAdapter dbContext);

        Mensaje getMovimientosPorFiltro(List<CamposWhere> camposwhere, DBContextAdapter dbContext);

        Mensaje getMovimientosPorFiltroEspecifico(List<CamposWhere> camposwhere, DBContextAdapter dbContext);

        Mensaje getCalculosUnidadesPorFiltroEspecifico(List<CamposWhere> camposwhere, List<CFDIEmpleado> listCFDIEmpleado, DBContextAdapter dbContext);//JSA02

        Mensaje buscaMovimientosNominaFiltrado(List<Object> valoresDeFiltrado, DBContextAdapter dbContext);

        Mensaje saveDeleteMovNomConcep(List<object> entitysCambios, List<object> clavesDelete, Dictionary<string, object> listaParametros, int rango, DBContextAdapter dbContext);

        Mensaje getMaxNumeroMovimientoPorTipoNominaYPeriodoID(decimal? claveTipoNomina, decimal? idPeriodo, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosMovNomConcep(List<CamposWhere> camposwhere, long inicio, long rango, DBContextAdapter dbContext);

        Mensaje saveDeleteMovNomina(List<MovNomConcep> AgreModif, Object[] clavesDelete, Object[] valoresReestablecer, bool incluirEliminadoDiferenteTipoPantalla100, DBContextAdapter dbContext);

        Mensaje getIsMovPeriodoNomina(decimal idPeriodo, decimal IdtipoNomina, DBContextAdapter dbContext);

       

        Mensaje eliminarMovNomina(decimal? idTipoCorrida, decimal? idTipoNomina, decimal? idPeriodoNomina, decimal? idRegPatronal,
       decimal? idCenCosto, decimal? idDepartamento, decimal? idEmplIni, decimal? idEmplFin, decimal? idRazonSocial, DBContextAdapter dbContext,  DBContextAdapter dbContextMaster);


  }
}
