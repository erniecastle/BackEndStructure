/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface PlazasPorEmpleadoDAOIF para llamados a metodos de Entity
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
   public interface PlazasPorEmpleadoDAOIF:IGenericRepository<PlazasPorEmpleado>
    {
        Mensaje agregar(PlazasPorEmpleado entity, DBContextAdapter dbContext);

        Mensaje actualizar(PlazasPorEmpleado entity, DBContextAdapter dbContext);

        Mensaje eliminar(PlazasPorEmpleado entity, DBContextAdapter dbContext);

        Mensaje getPlazasPorEmpleadoPorClave(String clave, String razonSocial, DBContextAdapter dbContext);

        Mensaje getPlazasPorEmpleadoPorRazonSocial(String clave, String razonSocial, DBContextAdapter dbContext);

        Mensaje getPlazasPorEmpleadoPorRazonSocialActivo(String claveEmpleado, String razonSocial, DateTime fecha, int result, DBContextAdapter dbContext);

        Mensaje getPlazasPorEmpleadoPorClavePorRazonSocialActivo(String clavePlaza, String claveEmpleado, String razonSocial, DateTime fecha, DBContextAdapter dbContext);

        Mensaje getPlazasPorEmpleadosActivos(String claveEmpleado, String claveRazonSocial, DateTime fechaInicial, DateTime fechaFinal, String claveTipoNomina, int result, DBContextAdapter dbContext);

        Mensaje getPlazasPorEmpleadosPorReferenciaActiva(String claveEmpleado, String claveRazonSocial, DateTime fechaInicial, DateTime fechaFinal, String claveTipoNomina, String claveReferencia, DBContextAdapter dbContext);

        Mensaje getPlazasPorEmpleadosPorReferencia(decimal idReferencia, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosPlazasPorEmpleado(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosPlazasPorEmpleado(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje agregarListaPlazasPorEmpleados(List<PlazasPorEmpleado> entitys, int rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DbContext dbContext);

        Mensaje deleteListQuerys(String tabla, String campo, Object[] valores, DBContextAdapter dbContext);

        Mensaje getPlazasPorEmpleadoReingreso(String claveReingreso, String claveRazonesSociales, DBContextAdapter dbContext);

        Mensaje getPlazasPorEmpleadosActivosId(decimal? claveEmpleado, decimal? claveRazonSocial, DateTime? fechaInicial, DateTime? fechaFinal, decimal? claveTipoNomina, int result, DBContextAdapter dbContext);

        Mensaje getPlazasPorEmpleadosActivosIdList(decimal? claveEmpleado, decimal? claveRazonSocial, DateTime? fechaInicial, DateTime? fechaFinal, decimal? claveTipoNomina, DBContextAdapter dbContext);

        Mensaje getPlazaPorID(decimal? idPlaza, DBContextAdapter dbContext); 
    }
}
