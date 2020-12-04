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
   public  interface CFDIEmpleadoDAOIFH : IRepository<Object>
    {
        Mensaje generaDatosParaTimbrado(List<Object> valoresDeFiltrado, String claveRazonSocial,ISession sessionSimple, ISession sessionMaster);
    }
}
