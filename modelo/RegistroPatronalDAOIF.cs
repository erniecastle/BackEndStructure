/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface RegistroPatronalDAOIF para llamados a metodos de Entity
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
    public interface RegistroPatronalDAOIF
    {
        Mensaje agregar(RegistroPatronal entity, DBContextAdapter dbContext);

        Mensaje modificar(RegistroPatronal entity, DBContextAdapter dbContext);
        Mensaje eliminar(RegistroPatronal entity, DBContextAdapter dbContext);

        Mensaje getAllRegistroPatronal(String claveRazonesSocial, DBContextAdapter dbContext);

        Mensaje getSeriesPorRegistroPatronal(String claveRazonesSocial, DBContextAdapter dbContext);

        Mensaje getPorClaveRegistroPatronal(String clave, String claveRazonesSocial, DBContextAdapter dbContext);

        Mensaje getPorIdRegistroPatronal(decimal? id, string claveRazonesSocial, DBContextAdapter dbContext);

        Mensaje SaveRegistroPatronal(List<Primas> agrega, Object[] eliminados, RegistroPatronal entity, DBContextAdapter dbContext);

        Mensaje UpdateRegistroPatronal(List<Primas> agrega, Object[] eliminados, RegistroPatronal entity, DBContextAdapter dbContext);

        Mensaje DeleteRegistroPatronal(RegistroPatronal entity, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosRegistroPatronal(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosRegistroPatronal(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje agregarListaRegistrosPatronales(List<RegistroPatronal> cambios, List<RegistroPatronal> temporales, List<Primas> cambioprima,
                Object[] clavesDelete, Object[] clavesPrimasDelete, int rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje deleteListClavesRegistroPatronal(Object[] valores, DBContextAdapter dbContext);
    }
}
