/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface CalculaNominaDAOIF para llamados a metodos de Entity
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
    public interface CalculaNominaDAOIF : IGenericRepository<CalculoISR>
    {
        Mensaje calculaNomina(String claveEmpIni, String claveEmpFin, String claveTipoNomina, String claveTipoCorrida, decimal? idPeriodoNomina,
         String clavePuesto, String claveCategoriasPuestos, String claveTurno, String claveRazonSocial, String claveRegPatronal, String claveFormaDePago,
         String claveDepto, String claveCtrCosto, int? tipoSalario, String tipoContrato, Boolean? status, String controlador, int uso, 
         ParametrosExtra parametrosExtra, int ejercicioActivo, DatosTablaXml datosTablaXml, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje calculaSalarioDiarioIntegerado(String claveEmpIni, String claveEmpFin, String claveTipoNomina, String claveTipoCorrida, decimal? idPeriodoNomina,
                String clavePuesto, String claveCategoriasPuestos, String claveTurno, String claveRazonSocial, String claveRegPatronal, String claveFormaDePago,
                String claveDepto, String claveCtrCosto, int? tipoSalario, String tipoContrato, bool? status, String controlador, int uso, ParametrosExtra parametrosExtra, bool soloCalculo, bool peticionModuloCalculoSalarioDiarioIntegrado, DBContextAdapter dbContext, DBContextAdapter dbContextMaster);

        Mensaje calculaSDIPorEmpleado(PlazasPorEmpleadosMov plazasPorEmpleadosMov, String controlador, ParametrosExtra parametrosExtra, bool soloCalculo, bool peticionModuloCalculoSalarioDiarioIntegrado, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje busquedaQueryConsultaEmpleados(String[] tablas, String[] camposMostrar, String[] camposWhere, Object[] valoresWhere, String[] camposOrden, String[] valoresDatosEspeciales, String[] camposWhereExtras, String nombreFuenteDatos, DateTime[] rangoFechas, String ordenado, String claveRazonSocial, String controladores, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);

        Mensaje busquedaQueryConsultaEmpleados2(TipoBD tipoBD, TipoOperacion tipoOperacion, string tabla, OperadorSelect operadorSelect, List<CamposFrom> listCamposFrom, List<CamposWhere> listCamposWhere, String[] valoresDatosEspeciales, List<CamposWhere> listCamposWhereExtras, List<CamposGrupo> listCamposGrupo, List<CamposOrden> listCamposOrden, DateTime[] rangoFechas, String ordenado, String claveRazonSocial, String controladores, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra);
    }
}
