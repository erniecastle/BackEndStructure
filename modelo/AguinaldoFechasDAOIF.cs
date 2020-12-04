

using Exitosw.Payroll.Core.util;
/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface AguinaldoFechasDAOIF para llamados a metodos de Entity
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
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public interface AguinaldoFechasDAOIF : IGenericRepository<AguinaldoFechas>
    {
        Mensaje agregar(AguinaldoFechas entity, DBContextAdapter dbContext);

        Mensaje modificar(AguinaldoFechas entity, DBContextAdapter dbContext);

        Mensaje eliminar(AguinaldoFechas entity, DBContextAdapter dbContext);

        Mensaje getAllAguinaldoFechas(DBContextAdapter dbContext);

        Mensaje SaveAguinaldoFechas(List<AguinaldoFechas> agrega, Object[] eliminados, DBContextAdapter dbContext);

        /*List<AguinaldoFechas>*/
        Mensaje getPorClaveAguinaldoFechas(String claveRazonsocial, DBContextAdapter dbContext);
    }
}
