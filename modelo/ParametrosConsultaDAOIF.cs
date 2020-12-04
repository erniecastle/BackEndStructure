/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface ParametrosConsultaDAOIF para llamados a metodos de Entity
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
   public interface ParametrosConsultaDAOIF:IGenericRepository<ParametrosConsulta>
    {
        Mensaje agregar(ParametrosConsulta entity, Contenedor contenedorGrupoMenu, DBContextAdapter dbContextMaestra);

        Mensaje actualizar(ParametrosConsulta entity, Contenedor contenedorGrupoMenu, DBContextAdapter dbContextMaestra);

        Mensaje eliminar(ParametrosConsulta entity, DBContextAdapter dbContextMaestra);

        Mensaje getAllParametrosConsulta(DBContextAdapter dbContextMaestra);

        Mensaje getParametrosConsultaAllEspecifico(DBContextAdapter dbContextMaestra);

        Mensaje getParametrosConsultaPorID(decimal idParametrosConsulta, DBContextAdapter dbContextMaestra);

        Mensaje consultaPorFiltrosParametrosConsulta(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosParametrosConsulta(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContextMaestra);

        Mensaje PorGrupoMenuFuenteDatos(String fuenteDatos, Int64 idContenedor, DBContextAdapter dbContextMaestra);

        Mensaje eliminarEspecifico(Int64 idReporte, DBContextAdapter dbContextMaestra);
    }
}
