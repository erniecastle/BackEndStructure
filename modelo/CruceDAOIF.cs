/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface CruceDAOIF para llamados a metodos de Entity
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
    public interface CruceDAOIF : IGenericRepository<Cruce>
    {
        Mensaje agregar(DBContextAdapter dbContext, Cruce entity);

        Mensaje actualizar(DBContextAdapter dbContext, Cruce entity);

        Mensaje eliminar(DBContextAdapter dbContext, Cruce entity);

        Mensaje getAllCruce(DBContextAdapter dbContext);

        Mensaje existeClaveElemento(DBContextAdapter dbContext, String claveelemento, String elemento, decimal parametro);

        Mensaje getCrucePorParametros(DBContextAdapter dbContext, decimal claveParametro);

        Mensaje getCrucePorElemento(DBContextAdapter dbContext, String elemento);

        Mensaje getCrucePorParamElemento(DBContextAdapter dbContext, decimal claveParametro, String elemento);

        Mensaje getCrucePorElemeYClave(DBContextAdapter dbContext, String elemento, String claveelemento);

        Mensaje getCrucePorParaElemeYClave(DBContextAdapter dbContext, decimal claveParametro, String elemento, String claveelemento);

        Mensaje SaveDeleteCruces(DBContextAdapter dbContext, List<Cruce> AgreModif, List<Cruce> Ordenados, Object[] clavesDelete);

        Mensaje SaveCruces(DBContextAdapter dbContext, List<Cruce> c);

        Mensaje DeleteCruces(DBContextAdapter dbContext, List<Cruce> c);
    }
}
