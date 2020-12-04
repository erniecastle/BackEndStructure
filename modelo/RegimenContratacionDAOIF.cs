using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.Entity.entidad;

namespace Exitosw.Payroll.Core.modelo
{
    public interface RegimenContratacionDAOIF
    {
        Mensaje agregar(RegimenContratacion entity, DBContextAdapter dbContext);

        Mensaje modificar(RegimenContratacion entity, DBContextAdapter dbContext);

        Mensaje eliminar(RegimenContratacion entity, DBContextAdapter dbContext);

        Mensaje getAllRegimenContratacion(DBContextAdapter dbContext);
    }
}
