/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface CiudadesDAOIF para llamados a metodos de Entity
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
    public interface CiudadesDAOIF : IGenericRepository<Ciudades>
    {
        
        Mensaje agregar(Ciudades entity, DBContextAdapter dbContext);
  
        Mensaje modificar(Ciudades entity, DBContextAdapter dbContext);
       
        Mensaje eliminar(Ciudades entity, DBContextAdapter dbContext);

        Mensaje getAllCiudades(DBContextAdapter dbContext);

        Mensaje getPorClaveCiudades(String clave, DBContextAdapter dbContext);

        Mensaje getPorIdCiudades(decimal? idCiudad,DBContextAdapter dbContext);

        Mensaje getCiudadesPorMunicipio(String claveMunicipio, DBContextAdapter dbContext);

        Mensaje getCiudadesPorEstado(String claveEstado, DBContextAdapter dbContext);

        Mensaje getCiudadesPorPais(String clavePais, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosCiudades(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosCiudades(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje saveDeleteCiudades(List<Ciudades> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);
    }
}
