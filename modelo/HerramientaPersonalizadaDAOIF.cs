using Exitosw.Payroll.Core.util;
/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface HerramientaPersonalizadaDAOIF para llamados a metodos de Entity
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
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public interface HerramientaPersonalizadaDAOIF : IGenericRepository<HerramientaPersonalizada>
    {
        Mensaje getAllHerramientaPersonal(DBContextAdapter dbContext);

        Mensaje getAllHerrPerYContPorUsuario(string keyUser, DBContextAdapter dbContext);

        Mensaje getHerramientaPersonalPrincipales(DBContextAdapter dbContext);

        Mensaje getHerramientaPersonalPerfilHerr(Perfiles perfil, Herramienta herramienta, DBContextAdapter dbContext);

        Mensaje getHerramientaPersonalUsuarioHerr(Usuario user, Herramienta herramienta, DBContextAdapter dbContext);

        Mensaje getHerramientasPersonalPerfil(Perfiles perfil, DBContextAdapter dbContext);

        Mensaje getHerramientasPersonalUsuario(Usuario user, DBContextAdapter dbContext);

        Mensaje agregar(HerramientaPersonalizada entity, DBContextAdapter dbContext);

        Mensaje modificar(HerramientaPersonalizada entity, DBContextAdapter dbContext);

        Mensaje eliminar(HerramientaPersonalizada entity, DBContextAdapter dbContext);
    }
}
