/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface BancosDAOIF para llamados a metodos de Entity
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
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public interface BancosDAOIF : IGenericRepository<Bancos>
    {
        Mensaje agregar(Bancos entity, DBContextAdapter dbContext);

        Mensaje modificar(Bancos entity, DBContextAdapter dbContext);

        Mensaje eliminar(Bancos entity, DBContextAdapter dbContext);

        Mensaje getAllBancos(DBContextAdapter dbContext);

        Mensaje getPorClaveBancos(String clave, DBContextAdapter dbContext);

        Mensaje getPorIdBancos(decimal? idBancos,DBContextAdapter dbContext);

        Mensaje SaveBanco(List<Contactos> agrega, Object[] eliminados, Bancos entity, DBContextAdapter dbContext);

        Mensaje saveDetallesBancos(Bancos entity, Dictionary<string, object> Detalles, DBContextAdapter dbContext);

        Mensaje deleteDetallesBancos(Bancos entity, Dictionary<string, object> Detalles, DBContextAdapter dbContext);

        Mensaje DeleteBanco(Bancos entity, DBContextAdapter dbContext);

        Mensaje UpdateBanco(List<Contactos> agrega, Object[] eliminados, Bancos entity, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosBancos(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosBancos(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        //Mensaje existeDato(string tabla, CamposWhere campoWhere, Conexion conexion);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        Mensaje agregarListaBancos(List<Bancos> cambios, List<Bancos> temporales, List<Contactos> cambioContactos, Object[] clavesContactosDelete, int rango, DBContextAdapter dbContext);

        Mensaje existeRFC(String rfc, String claveBancos, DBContextAdapter dbContext);
    }
}
