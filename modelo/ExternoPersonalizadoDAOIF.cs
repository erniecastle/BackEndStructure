using Exitosw.Payroll.Core.util;
/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface ExternoPersonalizadoDAOIF para llamados a metodos de Entity
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


namespace Exitosw.Payroll.Core.modelo
{
    public interface ExternoPersonalizadoDAOIF : IGenericRepository<ExternoPersonalizado>
    {
        Mensaje getExternoPersonalPerfilHerr(Perfiles perfil, Herramienta herramienta, DBContextAdapter dbContext);

        Mensaje getExternoPersonalUsuarioHerr(Usuario user, Herramienta herramienta, DBContextAdapter dbContext);

        Mensaje getExternosPersonalPerfil(Perfiles perfil, DBContextAdapter dbContext);

        Mensaje getExternosPersonalPerfilSinUsuarios(Perfiles perfil, DBContextAdapter dbContext);

        Mensaje getExternosPersonalUsuario(Usuario user, DBContextAdapter dbContext);

        Mensaje agregar(ExternoPersonalizado entity, DBContextAdapter dbContext);

        Mensaje actualizar(ExternoPersonalizado entity, DBContextAdapter dbContext);

        Mensaje eliminar(ExternoPersonalizado entity, DBContextAdapter dbContext);
    }
}
