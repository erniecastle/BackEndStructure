using Exitosw.Payroll.Core.util;
/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface InasistenciaPorHoraDAOIF para llamados a metodos de Entity
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
    public interface InasistenciaPorHoraDAOIF:IGenericRepository<InasistenciaPorHora>
    {
        Mensaje getAllInasistenciaPorHora(DBContextAdapter dbContext);

        Mensaje saveDeleteInasistenciaPorHora(List<InasistenciaPorHora> AgreModif, Object[] clavesDelete, DBContextAdapter dbContext);

        Mensaje getInasistenciaPorNominaPeriodo(String claveTipoNomina, String claveRazonSocial, Int64 idPeriodo, DBContextAdapter dbContext);
    }
}
