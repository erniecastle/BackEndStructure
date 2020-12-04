/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface ConsultaGenericaEspecialesDAOIF para llamados a metodos de Entity
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
using Exitosw.Payroll.Core.campos;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public interface ConsultaGenericaEspecialesDAOIF : IGenericRepository<Object>
    {
        Mensaje existeClave(String identificador, List<CamposWhere> listCamposWhere, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje consultaPorRangosFiltro(String identificador, ValoresRango valoresRango, List<CamposWhere> listCamposWhere, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje obtenerDatosCriterioConsulta(String[] tablas, List<CamposSelect> listCamposSelect, List<CamposWhere> listCamposWhere, List<CamposOrden> listCamposOrden, DBContextAdapter dbContext);

       
    }
}
