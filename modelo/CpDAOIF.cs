/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface CpDAOIF para llamados a metodos de Entity
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
    public interface CpDAOIF : IGenericRepository<Cp>
    {
        Mensaje agregar(Cp entity, DBContextAdapter dbContext);

        Mensaje modificar(Cp entity, DBContextAdapter dbContext);

        Mensaje eliminar(Cp entity, DBContextAdapter dbContext);

        Mensaje getAllCp(DBContextAdapter dbContext);

        Mensaje getPorClaveCp(string clave, DBContextAdapter dbContext);

        Mensaje getPorIdCp(decimal? idCp,DBContextAdapter dbContext);

        Mensaje getCpPorCiudades(String claveCiudad, DBContextAdapter dbContext);

        Mensaje getCpPorMunicipio(String claveMunicipio, DBContextAdapter dbContext);

        Mensaje getCpPorEstado(String claveEstado, DBContextAdapter dbContext);

        Mensaje getCpPorPais(String clavePais, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosCp(Dictionary<string, object> campos, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosCp(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje saveDeleteCp(List<Cp> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);
    }
}
