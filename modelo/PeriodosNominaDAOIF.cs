/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface PeriodosNominaDAOIF para llamados a metodos de Entity
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
    public interface PeriodosNominaDAOIF
    {

        Mensaje agregar(PeriodosNomina entity, DBContextAdapter dbContext);

        Mensaje modificar(PeriodosNomina entity, DBContextAdapter dbContext);

        Mensaje eliminar(PeriodosNomina entity, DBContextAdapter dbContext);

        Mensaje getAllPeriodosNomina(DBContextAdapter dbContext);

        Mensaje getPeriodosNominaPorClave(String clave, int year, String claveTipoCorrida, DBContextAdapter dbContext);

        Mensaje getPeriodosNominaPorClaveYTipoDeNominaCorrida(String clave, String claveTipoNomina, String claveTipoCorrida, DBContextAdapter dbContext);

        Mensaje getPeriodosNominaPorClaveYTipoDeNominaECorrida(String clave, TipoNomina tipoNomina, String claveTipoCorrida, DBContextAdapter dbContext);

        Mensaje getPeriodosNominaActualPorFecha(DateTime fecha, String claveTipoNomina, String claveTipoCorrida, bool status, DBContextAdapter dbContext);

        Mensaje getPeriodosNominaPorTipoNominaYRangoDeFechas(DateTime fechaInicial, DateTime fechaFinal, String claveTipoNomina, String claveTipoCorrida, DBContextAdapter dbContext);

        Mensaje getUltimoPeriodoCerradoPorFecha(DateTime fecha, String claveTipoNomina, String claveTipoCorrida, DBContextAdapter dbContext);

        Mensaje getPeriodosNominaPorAño(int year, DBContextAdapter dbContext);

        Mensaje getPeriodosNominaPorAñoYTipoNominaYTipoCorrida(int año, String tipoNomina, String tipoCorrida, DBContextAdapter dbContext);

        Mensaje getPeriodosNominaPorFechasYTipoNominaCorrida(DateTime inicio, DateTime fin, TipoNomina tipoNomina, String claveTipoCorrida, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosPeriodosNomina(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosPeriodosNomina(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DbContext dbContext);

        Mensaje saveDeletePeriodosNomina(List<PeriodosNomina> entitysCambios, List<PeriodosNomina> eliminados, TipoCorrida tCorrida, DBContextAdapter dbContext);

        Mensaje ObtenerFechaFinalMax(String claveTipoNomina, String claveTipoCorrida, int año, DBContextAdapter dbContext);

        Mensaje ObtenerFechaFinalMin(String claveTipoNomina, String claveTipoCorrida, int año, DBContextAdapter dbContext);

        Mensaje getUltimoPeriodo(int año, String claveTipoNomina, String claveTipoCorrida, DBContextAdapter dbContext);

        Mensaje getPrimerPeriodo(int año, String claveTipoNomina, String claveTipoCorrida, DBContextAdapter dbContext);

        Mensaje getPeriodosNominaPorFechaTipoNominaCorrida(DateTime fecha, String claveTipoNomina, String claveTipoCorrida, DBContextAdapter dbContext);

        Mensaje actualizaListaPorCampos(String[] campoModificar, Object[] valoresModificado, String[] camposWhere, Object[] valoresWhere, DBContextAdapter dbContext);

        /*boolean*/
        Mensaje getStatusPeriodo(decimal idPeriodo, DBContextAdapter dbContext);

        /*List<PeriodosNomina>*/
        Mensaje getPeriodosNominaPorAñoYTipoCorrida(int year, String claveTipoCorrida, DBContextAdapter dbContext);

        Mensaje getPeriodosNominaPorFechayTipoCorridaSinStatus(DateTime fecha, String claveTipoNomina, String claveTipoCorrida, DBContextAdapter dbContext);

        Mensaje getPeriodosNominaPorID(decimal? id, DBContextAdapter dbContext);

        Mensaje getPeriodosNominaPorAñoYTipoNominaYTipoCorridaID(int año, String tipoNomina, decimal? tipoCorrida, DBContextAdapter dbContext);

        Mensaje getPeriodosNominaPorAñoYIDTipoNominaYIDTipoCorrida(int año, decimal? tipoNomina, decimal? tipoCorrida, DBContextAdapter dbContext);

        Mensaje getPeriodosNominaPorFechaTipoNominaCorridaEnti(DateTime fecha, String claveTipoNomina, String claveTipoCorrida, DBContextAdapter dbContext);

        Mensaje getPeriodosNominaPorClaveYTipoDeNominaCorridaAñoEnti(String clave, String claveTipoNomina, String claveTipoCorrida,int ejercicio , DBContextAdapter dbContext);

        Mensaje getPeriodosNominaPorFechaTipoNominaCorridaEntiJS(DateTime fecha, decimal claveTipoNomina, String claveTipoCorrida, DBContextAdapter dbContext);

    }
}
