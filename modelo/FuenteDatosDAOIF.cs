using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.Entity.entidad;
using System;


namespace Exitosw.Payroll.Core.modelo
{
    public interface FuenteDatosDAOIF
    {
        Mensaje getOrigenDatosTablas();

        Mensaje getDatosTabla(String fuente);

        Mensaje getOrigenDatos(DBContextAdapter dbContextMaster);



    }
}