using Exitosw.Payroll.Core.genericos.campos;
using System;
using System.Collections.Generic;

namespace Exitosw.Payroll.Core.genericos
{
    

        public class DatosQuery
        {
            public DatosQuery()
            {
                aliasTablas = new Dictionary<string, CamposJoin>();
                parametrosCampos = new Dictionary<string, Object>();
                listParametros = new List<String>();
            }

            public List<String> listParametros { get; set; }

            public string query { get; set; }

            public Dictionary<string, CamposJoin> aliasTablas { get; set; }

            public Dictionary<string, Object> parametrosCampos { get; set; }

            public bool conParametros { get; set; } = false;
        }
    
}