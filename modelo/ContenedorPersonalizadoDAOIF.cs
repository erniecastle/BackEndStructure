/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface ContenedorPersonalizadoDAOIF para llamados a metodos de Entity
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
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public interface ContenedorPersonalizadoDAOIF : IGenericRepository<ContenedorPersonalizado>
    {
        Mensaje getContenedorPersonal(int parentId, Herramienta herramienta, DBContextAdapter dbContext);

        Mensaje getContenedorPersonalAll(DBContextAdapter dbContext);

        Mensaje getContenedorPersonalHert(Herramienta herramienta, DBContextAdapter dbContext);

        Mensaje getContenedorPersonalPerfilHerr(Perfiles perfil, Herramienta herramienta, DBContextAdapter dbContext);

        Mensaje getContenedorPersonalUsuarioHerr(Usuario user, Herramienta herramienta, DBContextAdapter dbContext);

        Mensaje getContenedoresPersonalPerfil(Perfiles perfil, DBContextAdapter dbContext);

        Mensaje getContenedoresPersonalesSinUsuarios(Perfiles perfil, DBContextAdapter dbContext);

        Mensaje getContenedoresPersonalUsuario(Usuario user, DBContextAdapter dbContext);

        Mensaje agregar(ContenedorPersonalizado entity, DBContextAdapter dbContext);

        Mensaje actualizar(ContenedorPersonalizado entity, DBContextAdapter dbContext);

        Mensaje eliminar(ContenedorPersonalizado entity, DBContextAdapter dbContext);
    }
}
