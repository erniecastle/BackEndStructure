/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface ElementosAplicacionDAOIF para llamados a metodos de Entity
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
    public interface ElementosAplicacionDAOIF : IGenericRepository<ElementosAplicacion>
    {
        Mensaje getAllElementosAplicacion(DBContextAdapter dbContext);

        Mensaje getElementosAplicacionHert(DBContextAdapter dbContext, long nodoPadre);

        Mensaje getElementosAplicacionPorClave(DBContextAdapter dbContext, String clave, long parentID);

        Mensaje guardarElementosAplicacion(DBContextAdapter dbContext, List<ElementosAplicacion> agrega, Object[] eliminados);

        Mensaje getElementosAplicacionMaximo(DBContextAdapter dbContext);
    }
}
