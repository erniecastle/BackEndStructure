/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface ContactosDAOIF para llamados a metodos de Entity
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
    public interface ContactosDAOIF
    {
        Mensaje agregar(Contactos entity, DBContextAdapter dbContext);

        Mensaje modificar(Contactos entity, DBContextAdapter dbContext);

        Mensaje eliminar(Contactos entity, DBContextAdapter dbContext);

        Mensaje getAllContactos(DBContextAdapter dbContext);

        Mensaje getContactosPorBancos(Bancos b, DBContextAdapter dbContext);

        Mensaje eliminaContactosPorBanco(Bancos banco, DBContextAdapter dbContext);
    }
}
