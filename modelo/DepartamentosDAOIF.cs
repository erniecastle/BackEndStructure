/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface DepartamentosDAOIF para llamados a metodos de Entity
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
    public interface DepartamentosDAOIF
    {
        Mensaje getAllDepartamentos(String claveRazonesSocial, DBContextAdapter dbContext);

        Mensaje getPorClaveDepartamentos(String clave, String claveRazonesSocial, DBContextAdapter dbContext);

        Mensaje getPorIdDepartamentos(decimal? idDepartamento, String claveRazonesSocial, DBContextAdapter dbContext);

        Mensaje agregar(Departamentos entity, DBContextAdapter dbContext);

        Mensaje modificar(Departamentos entity, DBContextAdapter dbContext);

        Mensaje eliminar(Departamentos entity, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosDepartamentos(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosDepartamentos(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje saveDeleteDepartamentos(List<Departamentos> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);
    }
}
