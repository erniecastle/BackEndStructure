using Exitosw.Payroll.Hibernate.entidad;
using Exitosw.Payroll.Hibernate.modelo;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exitosw.Payroll.Core.modelo
{
   public  interface CalculoNominaHIF: IRepository<object>
    {
        Mensaje calculaNomina(String claveEmpIni, String claveEmpFin, String claveTipoNomina, String claveTipoCorrida, decimal? idPeriodoNomina,
        String clavePuesto, String claveCategoriasPuestos, String claveTurno, String claveRazonSocial, String claveRegPatronal, String claveFormaDePago,
        String claveDepto, String claveCtrCosto, int? tipoSalario, String tipoContrato, Boolean? status, String controlador, int uso, ParametrosExtra parametrosExtra,
        int ejercicioActivo, Object factoresCalculo, ISession sessionSimple,ISession sessionMaster);
    }
}
