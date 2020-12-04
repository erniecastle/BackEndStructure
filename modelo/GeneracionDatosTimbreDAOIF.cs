using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Entity.entidad.cfdi;
using NHibernate;
using System.Collections.Generic;

namespace Exitosw.Payroll.Core.modelo
{
   public interface GeneracionDatosTimbreDAOIF: IGenericRepository<CFDIEmpleado>
    {
        Mensaje generacionDatosTimbre(decimal? idTipoCorrida, decimal? idTipoNomina, decimal? idPeriodoNomina, decimal? idRegPatronal,
            decimal? idCenCosto, decimal? idDepartamento, decimal? idEmplIni, decimal? idEmplFin, decimal? idRazonSocial, 
            DBContextAdapter dbContextSimple, DBContextAdapter dbContextMaster, ISession sessionSimple, ISession sessionMaster);

        Mensaje generaTimbrado(decimal? idTipoCorrida, decimal? idTipoNomina, decimal? idPeriodoNomina, decimal? idRegPatronal,
          decimal? idCenCosto, decimal? idDepartamento, decimal? idEmplIni, decimal? idEmplFin, decimal? idRazonSocial, string ruta, List<decimal?> listIdEmpleados,
          DBContextAdapter dbContextSimple, DBContextAdapter dbContextMaster);

        Mensaje buscarTimbres(decimal? idTipoCorrida, decimal? idTipoNomina, decimal? idPeriodoNomina, decimal? idRegPatronal,
        decimal? idCenCosto, decimal? idDepartamento, decimal? idEmplIni, decimal? idEmplFin, decimal? idRazonSocial, 
        DBContextAdapter dbContextSimple, DBContextAdapter dbContextMaster);

        Mensaje cancelarTimbres(decimal? idTipoCorrida, decimal? idTipoNomina, decimal? idPeriodoNomina, decimal? idRegPatronal,
        decimal? idCenCosto, decimal? idDepartamento, decimal? idEmplIni, decimal? idEmplFin, decimal? idRazonSocial, List<decimal?> listIdEmpleados,
        DBContextAdapter dbContextSimple, DBContextAdapter dbContextMaster);

        Mensaje buscarTimbresId(decimal? idTipoCorrida, decimal? idTipoNomina, decimal? idPeriodoNomina, decimal? idRegPatronal,
       decimal? idCenCosto, decimal? idDepartamento, decimal? idEmplIni, decimal? idEmplFin, decimal? idRazonSocial, int tipoNodoConsulta,
       DBContextAdapter dbContextSimple, DBContextAdapter dbContextMaster);

        Mensaje buscarEnProcesoCanc(decimal? idRazonSocial, DBContextAdapter dbContextSimple, DBContextAdapter dbContextMaster);

        Mensaje recuperarAcuse(decimal? idRazonSocial, List<decimal?> listIdEmpleados, DBContextAdapter dbContextSimple, DBContextAdapter dbContextMaster);
    }
}
