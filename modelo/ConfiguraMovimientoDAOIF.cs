/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface ConfiguraMovimientoDAOIF para llamados a metodos de Entity
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
    public interface ConfiguraMovimientoDAOIF
    {
        Mensaje agregar(ConfiguraMovimiento entity, DBContextAdapter dbContext);

        Mensaje actualizar(ConfiguraMovimiento entity, DBContextAdapter dbContext);

        Mensaje eliminar(ConfiguraMovimiento entity, DBContextAdapter dbContext);

        Mensaje getConfiguraMovimientoAll(String claveRazonesSocial, DBContextAdapter dbContext);

        Mensaje consultaPorRangosConfiguraMovimiento(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DbContext dbContext);

        Mensaje saveDeleteConfiguraMovimiento(List<ConfiguraMovimiento> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);

        Mensaje PorGrupoMenu(String idContenedor, String claveRazonSocial, DBContextAdapter dbContext);

        Mensaje buscaPorIdYRazonSocial(decimal id, String claveRazonSocial, DBContextAdapter dbContext);

        Mensaje buscaConfiguracionMovimSistema(decimal id, DBContextAdapter dbContext);

        Mensaje getAllConfiguracionMovimSistema(DBContextAdapter dbContext);

        Mensaje getConfiguracionMovimientoPorRango(int[] values, DBContextAdapter dbContext);
    }
}
