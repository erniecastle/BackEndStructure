/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface EstadosDAOIF para llamados a metodos de Entity
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
    public interface EstadosDAOIF : IGenericRepository<Estados>
    {
        Mensaje agregar(Estados entity, DBContextAdapter dbContext);
        Mensaje modificar(Estados entity, DBContextAdapter dbContext);
        Mensaje eliminar(Estados entity, DBContextAdapter dbContext);
        Mensaje getAllEstados(DBContextAdapter dbContext);
        Mensaje getPorClaveEstados(String clave, DBContextAdapter dbContext);
        Mensaje getPorIdEstados(decimal? idEstados, DBContextAdapter dbContext);
        Mensaje getEstadosPorPais(String clavePais, DBContextAdapter dbContext);
        Mensaje getPorPaisEstados(String clavePais, DBContextAdapter dbContext);
        Mensaje consultaPorFiltrosEstados(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);
        Mensaje consultaPorRangosEstados(Int64 inicio, Int64 rango, DBContextAdapter dbContext);
        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);
        Mensaje saveDeleteEstados(List<object> entitysCambios, List<object> clavesDelete, int rango, DBContextAdapter dbContext);
    }
}
