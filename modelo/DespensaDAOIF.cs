/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface DespensaDAOIF para llamados a metodos de Entity
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
using System;
using System.Collections.Generic;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public interface DespensaDAOIF : IGenericRepository<Despensa>
    {
        Mensaje agregar(Despensa entity, DBContextAdapter dbContext);

        Mensaje modificar(Despensa entity, DBContextAdapter dbContext);

        Mensaje eliminar(Despensa entity, DBContextAdapter dbContext);

        Mensaje getAllDespensa(DBContextAdapter dbContext);

        Mensaje getPorClaveDespensa(DateTime clave, DBContextAdapter dbContext);

        Mensaje SaveDespensa(List<Incidencias> agrega, Object[] eliminados, Despensa entity, DBContextAdapter dbContext);

    }
}
