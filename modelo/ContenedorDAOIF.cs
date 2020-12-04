/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface ContenedorDAOIF para llamados a metodos de Entity
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
    public interface ContenedorDAOIF : IGenericRepository<Contenedor>
    {
        Mensaje getContenedor(int parentId, Herramienta herramienta, List<String> ventanasAOmitir, DBContextAdapter dbContext);

        Mensaje getContenedorAll(List<String> ventanasAOmitir, DBContextAdapter dbContext);

        Mensaje getContenedorPorHerramienta(int herramientaId, DBContextAdapter dbContext);

        Mensaje getContenedorHert(Herramienta herramienta, List<String> ventanasAOmitir, Usuario usuario, RazonSocial razonSocial, DBContextAdapter dbContext);

        Mensaje getContenedorHertCompartida(Herramienta herramienta, List<String> ventanasAOmitir, Usuario usuario, RazonSocial razonSocial, DBContextAdapter dbContext);

        Mensaje getContenedorMax(DBContextAdapter dbContext);

        Mensaje agregar(Contenedor entity, DBContextAdapter dbContext);

        Mensaje actualizar(Contenedor entity, DBContextAdapter dbContext);

        Mensaje eliminar(List<Contenedor> entity, DBContextAdapter dbContext);

        Mensaje getContenedorPorId(int id, DBContextAdapter dbContext);

        Mensaje getContenedorOrder(List<String> ventanasAOmitir, DBContextAdapter dbContext);

        Mensaje SaveContenedor(List<Contenedor> c, DBContextAdapter dbContext);

        Mensaje DeleteContenedor(List<Contenedor> c, DBContextAdapter dbContext);

        Mensaje getContenedorPorNombreVentana(String nombreVentana, DBContextAdapter dbContext);

        Mensaje getContenedoresPorTipoAcciones(TipoAcciones[] tipoAcciones, DBContextAdapter dbContext);

        Mensaje buscaPorTipoAccionesYidMultiUsos(TipoAcciones[] tipoAcciones, decimal?[] idMultiUsos, String claveRazonSocial, DBContextAdapter dbContext);

        Mensaje getContenedorPorTipoHerramienta(int tipoherramientaId, DBContextAdapter dbContext);

        Mensaje getContenedorIDMax(DBContextAdapter dbContext);

        Mensaje getContenedorPorControlForma(string claveControl,DBContextAdapter dbContext);

    }
}
