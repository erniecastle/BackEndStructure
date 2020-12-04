/**
* @author: Daniel Ruelas
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface TiposDeCambioDAOIF para llamados a metodos de Entity
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
using System.Data.Entity;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public interface TiposDeCambioDAOIF:IGenericRepository<TiposDeCambio>
    {
        Mensaje agregar(TiposDeCambio entity, DBContextAdapter dbContext);

        Mensaje modificar(TiposDeCambio entity, DBContextAdapter dbContext);

        Mensaje eliminar(TiposDeCambio entity, DBContextAdapter dbContext);

        Mensaje getAllTiposDeCambio(DBContextAdapter dbContext);

        Mensaje getPorClaveTiposDeCambio(String clave, DBContextAdapter dbContext);

        Mensaje getPorIdTiposDeCambio(decimal? id ,DBContextAdapter dbContext);

        Mensaje getTiposDeCambioPorMoneda(Monedas m, DBContextAdapter dbContext);

        Mensaje getTiposDeCambioPorFecha(DateTime fecha, DBContextAdapter dbContext);

        Mensaje guardaTiposDeCambio(List<TiposDeCambio> agrega, Object[] eliminados, DBContextAdapter dbContext);

        Mensaje actualizaTiposDeCambio(List<TiposDeCambio> agrega, Object[] eliminados, DBContextAdapter dbContext);

        Mensaje eliminaTiposDeCambio(Object[] eliminados, DBContextAdapter dbContext);
    }
}
