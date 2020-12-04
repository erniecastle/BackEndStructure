

using Exitosw.Payroll.Core.genericos.campos;
using Exitosw.Payroll.Core.util;
/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface ConceptoDeNominaDefinicionDAOIF para llamados a metodos de Entity
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
    public interface ConcepNomDefiDAOIF
    {
        Mensaje agregar(ConcepNomDefi entity, DBContextAdapter dbContext);

        Mensaje modificar(ConcepNomDefi entity, DBContextAdapter dbContext);

        Mensaje eliminar(ConcepNomDefi entity, DBContextAdapter dbContext);

        Mensaje guardarConceptosNomina(ConcepNomDefi entity,
           List<ParaConcepDeNom> agregaModparametrosCn, object[] eliminadosParametros,
           List<ConceptoPorTipoCorrida> agregaModTiposCorrida, object[] eliminadosTiposCorrida,
           List<BaseAfecConcepNom> agregaModBasesAfecta, object[] eliminadosBasesAfecta,
            List<BaseAfecConcepNom> agregaModOtrasBasesAfecta, object[] eliminadosOtrasBasesAfecta,
           DBContextAdapter dbContext);

        Mensaje eliminarConceptosNomina(ConcepNomDefi entity, DBContextAdapter dbContext);

        Mensaje getPorIdConceptosYComplementos(decimal? idConcep, String claveCnc, DateTime? fecha, DBContextAdapter dbContext);

        Mensaje getVersionesConceptos(string claveConcep, DBContextAdapter dbContext);

        Mensaje getLastVersionConcepto(string claveConcep, DBContextAdapter dbContext);

        Mensaje getAllConcepNomDefi(DBContextAdapter dbContext);

        Mensaje getConceptoDeNominaDefinicionPorClave(String clave, DBContextAdapter dbContext);

        Mensaje getPorTipoCorridaConcepNomDefi(string claveTipoCorrida, DBContextAdapter dbContext);

        Mensaje getConceptoDeNominaDefinicionConCuentaContable(DBContextAdapter dbContext);

        Mensaje getConceptoDeNominaDefinicionPorClaves(Object[] claves, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosConceptoDeNominaDefinicion(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosConceptoDeNominaDefinicion(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DbContext dbContext);

        Mensaje agregaConceptoNominaBaseAfectadas(ConcepNomDefi entity, List<BaseAfecConcepNom> eliminadasAfectadaConceptoNominas, DBContextAdapter dbContext);

        Mensaje claveDescripcionConceptos(DBContextAdapter dbContext);

        Mensaje getPorTipoCorridaIDConcepNomDefi(decimal? claveTipoCorrida, DBContextAdapter dbContext);

        Mensaje getConceptoDeNominaDefinicionPorClaveID(decimal? clave, decimal? claveTipoCorrida, DBContextAdapter dbContext);

        Mensaje getConceptoDeNominaDefinicionPorID(decimal? idConcep, DBContextAdapter dbContext);
    }
}
