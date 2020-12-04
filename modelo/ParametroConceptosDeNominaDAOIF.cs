/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface ParametroConceptosDeNominaDAOIF para llamados a metodos de Entity
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
  public  interface ParametroConceptosDeNominaDAOIF
    {
        Mensaje agregar(ParaConcepDeNom entity, DBContextAdapter dbContext);

        Mensaje actualizar(ParaConcepDeNom entity, DBContextAdapter dbContext);

        Mensaje eliminar(ParaConcepDeNom entity, DBContextAdapter dbContext);

        Mensaje getAllParametroConceptosDeNomina(DBContextAdapter dbContext);

        Mensaje getPorClaveParametroConceptosDeNomina(String clave, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosParaConcepDeNom(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosParaConcepDeNom(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje agregarListaParametroConceptosDeNomina(List<ParaConcepDeNom> entitys, int rango, DBContextAdapter dbContext);

        Mensaje deleteListQuerys(String tabla, String campo, Object[] valores, DBContextAdapter dbContext);

        Mensaje getPorClaveParametroConceptosDeNominaID(decimal? clave, DBContextAdapter dbContext);
    }
}
