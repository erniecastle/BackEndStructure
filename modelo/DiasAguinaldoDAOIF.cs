/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface DiasAguinaldoDAOIF para llamados a metodos de Entity
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
    public interface DiasAguinaldoDAOIF : IGenericRepository<DiasAguinaldo>
    {
        Mensaje agregar(DiasAguinaldo entity, DBContextAdapter dbContext);

        Mensaje modificar(DiasAguinaldo entity, DBContextAdapter dbContext);

        Mensaje eliminar(DiasAguinaldo entity, DBContextAdapter dbContext);

        Mensaje getAllDiasAguinaldo(DBContextAdapter dbContext);

        Mensaje SaveDiasAguinaldo(List<DiasAguinaldo> agrega, Object[] eliminados, DBContextAdapter dbContext);

        Mensaje getPorClaveDiasAguinaldo(String claveRazonsocial, DBContextAdapter dbContext);
    }
}
