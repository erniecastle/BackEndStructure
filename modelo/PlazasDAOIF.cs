/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface PlazasDAOIF para llamados a metodos de Entity
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
  public interface PlazasDAOIF:IGenericRepository<Plazas>
    {
        Mensaje agregar(Plazas entity, DBContextAdapter dbContext);

        Mensaje actualizar(Plazas entity, DBContextAdapter dbContext);

        Mensaje eliminar(Plazas entity, DBContextAdapter dbContext);

        //    List<Plazas> getPlazasAll(, DBContextAdapter dbContext);

        Mensaje getPlazasPorClave(String clave, String razonSocial, DBContextAdapter dbContext);

        Mensaje getPlazasPorRazonSocial(String clave, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosPlazas(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosPlazas(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje agregarListaPlazas(List<Plazas> entitys, int rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje deleteListQuerys(String tabla, String campo, Object[] valores, DBContextAdapter dbContext);
    }
}
