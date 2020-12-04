/**
* @author: Daniel Ruelas
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface TipoCentroCostosDAOIF para llamados a metodos de Entity
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
using Exitosw.Payroll.Core.genericos.campos;
using System.Data.Entity;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
   public interface TipoCentroCostosDAOIF:IGenericRepository<TipoCentroCostos>
    {
        Mensaje agregar(TipoCentroCostos entity, DBContextAdapter dbContext);

        Mensaje modificar(TipoCentroCostos entity, DBContextAdapter dbContext);

        Mensaje eliminar(TipoCentroCostos entity, DBContextAdapter dbContext);

        Mensaje getAllTipoCentroCostos(String claveRazonesSocial, DBContextAdapter dbContext);

        Mensaje getTipoCentroCostosPorClave(String clave, String claveRazonesSocial, DBContextAdapter dbContext);

        Mensaje getPorIdTipoCentroCostos(decimal? id ,DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosTipoCentroCostos(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosTipoCentroCostos(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje saveDeleteTipoCentroCostos(List<TipoCentroCostos> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);
    }
}
