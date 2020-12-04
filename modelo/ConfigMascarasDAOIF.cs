/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface ConfigMascarasDAOIF para llamados a metodos de Entity
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
    public interface ConfigMascarasDAOIF
    {
        Mensaje agregar(Mascaras entity, DBContextAdapter dbContext);

        Mensaje actualizar(Mascaras entity, DBContextAdapter dbContext);

        Mensaje eliminar(Mascaras entity, DBContextAdapter dbContext);

        Mensaje getAllConfigMascaras(DBContextAdapter dbContext);

        Mensaje getPorClaveConfigMascaras(String clave, DBContextAdapter dbContext);

        Mensaje getSaveMascarasAfectaClaves(List<Mascaras> listMascaras, bool soloAplicarReEstructuracion, byte[] propertiesMascara, String ubicacion, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje consultaPorRangosMascaras(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje getFilePropertiesMascara(String directorioReportesDelSistema, DBContextAdapter dbContext);

        Mensaje getAllConfigMascarasJS(DBContextAdapter dbContext);

        Mensaje saveConfigMascara(List<Mascaras> entitys, DBContextAdapter dbContext);

        Mensaje getConfigMascaraPorClaveJS(string claveMascara, DBContextAdapter dbContext);

    }
}
