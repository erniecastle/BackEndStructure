using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exitosw.Payroll.Hibernate.entidad
{
    public class Mensaje
    {
        public String error { get; set; }
        public int noError { get; set; }
        public Object resultado { get; set; }
    }
}
