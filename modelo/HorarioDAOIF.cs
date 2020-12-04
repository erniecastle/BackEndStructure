/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface HorarioDAOIF para llamados a metodos de Entity
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
   public interface HorarioDAOIF:IGenericRepository<Horario>
    {
        Mensaje agregar(Horario entity, DBContextAdapter dbContext);

        Mensaje modificar(Horario entity, DBContextAdapter dbContext);

        Mensaje eliminar(Horario entity, DBContextAdapter dbContext);

        Mensaje getAllHorario(DBContextAdapter dbContext);

        Mensaje getPorClaveHorario(String clave, DBContextAdapter dbContext);

        Mensaje getPorIdHorario(decimal? id ,DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosHorario(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosHorario(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje getHorariosPorRazonSocial(RazonesSociales razonSocial, DBContextAdapter dbContext);
    }
}
