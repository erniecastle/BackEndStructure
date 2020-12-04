/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface MunicipiosDAOIF para llamados a metodos de Entity
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
   public interface MunicipiosDAOIF:IGenericRepository<Municipios>
    {
        Mensaje getAllMunicipios(DBContextAdapter dbContext);

        Mensaje getPorClaveMunicipios(String clave, DBContextAdapter dbContext);

        Mensaje agregar(Municipios entity, DBContextAdapter dbContext);

        Mensaje modificar(Municipios entity, DBContextAdapter dbContext);

        Mensaje eliminar(Municipios entity, DBContextAdapter dbContext);

        Mensaje getMunicipiosPorEstado(String claveEstado, DBContextAdapter dbContext);

        Mensaje getMunicipiosPorPais(String clavePais, DBContextAdapter dbContext);

        Mensaje getPorIdMunicipios(decimal? idMunicipios,DBContextAdapter dbContext);

       Mensaje consultaPorFiltrosMunicipios(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosMunicipios(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje saveDeleteMunicipios(List<Municipios> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);
    }
}
