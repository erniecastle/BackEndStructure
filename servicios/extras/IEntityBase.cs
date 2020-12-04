using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exitosw.Payroll.Core.servicios.extras
{
    public interface IEntityBase
    {
        decimal id { get; set; }
        string  clave { get; set; }
    }
}
