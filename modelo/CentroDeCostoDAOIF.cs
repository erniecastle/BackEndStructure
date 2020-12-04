/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface CentroDeCostosIF para llamados a metodos de Entity
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
using System.Data.Entity;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public interface CentroDeCostoDAOIF
    {
        Mensaje agregar(CentroDeCosto entity, DBContextAdapter dbContext);

        Mensaje modificar(CentroDeCosto entity, DBContextAdapter dbContext);

        Mensaje eliminar(CentroDeCosto entity, DBContextAdapter dbContext);

        Mensaje getAllCentroDeCosto(String claveRazonesSocial, DBContextAdapter dbContext);//JSA01

        Mensaje getCentroDeCostoPorClaveYRazon(String clave, String claveRazon, DBContextAdapter dbContext);

        Mensaje getPorIdCentroDeCosto(decimal? id,string claveRazon, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosCentroDeCosto(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosCentroDeCosto(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DbContext dbContext);

        Mensaje saveDeleteCentroDeCosto(List<CentroDeCosto> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);
    }
}
