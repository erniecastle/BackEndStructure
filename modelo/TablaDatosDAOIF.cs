/**
* @author: Daniel Ruelas
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface TablaDatosDAOIF para llamados a metodos de Entity
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
using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Core.genericos.campos;
using System.Data.Entity;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
   public  interface TablaDatosDAOIF:IGenericRepository<TablaDatos>
    {
        Mensaje getAllTablaDatos(DBContextAdapter dbContextMaestra);

        Mensaje getListTablaDatosPorTablaBase(String clave, DBContextAdapter dbContextMaestra);

        Mensaje agregar(TablaDatos entity, DBContextAdapter dbContextMaestra);

        Mensaje actualizar(TablaDatos entity, DBContextAdapter dbContextMaestra);

        Mensaje eliminar(TablaDatos entity, DBContextAdapter dbContextMaestra);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContextMaestra);

        //    public Boolean existeClave(String clave, String  ""Maestra);
        Mensaje getTablaDatosPorTablas(TablaBase tablaBase, TablaPersonalizada tablaPersonalizada, DBContextAdapter dbContextMaestra);

        Mensaje getTablaDatosPorTablasPorControlador(String controladores, TablaBase tablaBase, TablaPersonalizada tablaPersonalizada, DBContextAdapter dbContextMaestra);

        Mensaje consultaPorControladores(String controladores, DBContextAdapter dbContextMaestra);

        Mensaje consultaPorListaControladores(String[] controladores, DBContextAdapter dbContextMaestra);

        Mensaje getExisteDatosPorTablas(String tablaBaseClave, String tablaPersonalizadaClave, DBContextAdapter dbContextMaestra);

        Mensaje getTablaDatosPorTablasPorControladorPorFiltrosEspeciales(DateTime controlFecha, int controlAnio, String controladores, String tablaBaseClave, String tablaPersonalizadaClave, DBContextAdapter dbContextMaestra);

        Mensaje getTablaDatosPorTablaBaseyFechaControl(string claveTablaBase,DateTime fecha,DBContextAdapter dbContextMaestra);

        Mensaje getTablaDatosPorTablasPorControladorPorFiltrosEspecialesJS(DateTime? controlFecha, int controlAnio, String controladores, String tablaBaseClave, String tablaPersonalizadaClave, DBContextAdapter dbContextMaestra);
    }
}
