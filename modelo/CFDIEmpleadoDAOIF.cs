

using Exitosw.Payroll.Core.util;
/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface CFDIEmpleadoDAOIF para llamados a metodos de Entity
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
using Exitosw.Payroll.Entity.entidad.cfdi;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public interface CFDIEmpleadoDAOIF
    {
        Mensaje agregar(CFDIEmpleado entity, DBContextAdapter dbContext);

        Mensaje actualizar(CFDIEmpleado entity, DBContextAdapter dbContext);

        Mensaje eliminar(CFDIEmpleado entity, DBContextAdapter dbContext);

        Mensaje getAllCFDIEmpleado(DBContextAdapter dbContext);

        Mensaje getCFDIEmpleadoPorFiltro(String claveRazonSocial, String claveTipoNomina, String claveTipoCorrida, decimal idPeriodoNomina, StatusTimbrado statusTimbre, String[] rangoEmpleados, DBContextAdapter dbContext);

        Mensaje getCFDIEmpleadoStatusPorFiltro(String claveRazonSocial, String claveTipoNomina, String claveTipoCorrida, decimal idPeriodoNomina, List<StatusTimbrado> tiposTimbre, String[] rangoEmpleados, DBContextAdapter dbContext);

        Mensaje getCFDIEmpleadoStatusPorFiltroPorRangoPeriodosNomina(String claveRazonSocial, String claveTipoNomina, String claveTipoCorrida, DateTime fechaInicial, DateTime fechaFinal, List<StatusTimbrado> tiposTimbre, Object[] rangoEmpleados, DBContextAdapter dbContext);

        Mensaje getLimpiaConStatusErrorOEnProceso(String claveRazonSocial, String claveTipoNomina, String claveTipoCorrida, decimal idPeriodoNomina, List<string> rangoEmpleados, DBContextAdapter dbContext);

        Mensaje saveDeleteCFDIEmpleado(List<CFDIEmpleado> entitysCambios, Object[] idEliminar, int rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosCFDIEmpleado(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getCFDIEmpleadoTimbrados(decimal[] idsCFDIEmpleado, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje generaDatosParaTimbrado(List<Object> valoresDeFiltrado, String claveRazonSocial, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje buscaCFDIEmpleadosFiltrado(List<Object> valoresDeFiltrado, List<decimal?> listIdEmpleados, DBContextAdapter dbContext);

        Mensaje buscarCFDIEmpleadosEnProceso(List<object> valoresDeFiltrado, DBContextAdapter dbContext);

        Mensaje buscarCFDIReciboProcCanc(decimal idCFDIReciboProcCanc, DBContextAdapter dbContext);

    }
}
