/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface FirmasDAOIF para llamados a metodos de Entity
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
    public interface FirmasDAOIF : IGenericRepository<Firmas>
    {
        Mensaje getFirmasPorFechaYRazonSocial(String razonSocial, DateTime fecha, DBContextAdapter dbContext);

        Mensaje agregar(Firmas entity, DBContextAdapter dbContext);

        Mensaje modificar(Firmas entity, DBContextAdapter dbContext);

        Mensaje eliminar(Firmas entity, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosFirmas(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosFirmas(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje saveDeleteFirmas(List<Firmas> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);

        Mensaje getPorIdFirmas(decimal? idFirmas, DBContextAdapter dbContext);
    }
}
