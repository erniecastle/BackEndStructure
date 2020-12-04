using System.Collections.Generic;


namespace Exitosw.Payroll.Core.genericos.campos
{
    public class CamposSelect
    {

        public CamposSelect()
        {
            subCampos = new List<CamposSelect>();
        }

        public CamposSelect(string campoMostrar, TipoFuncion tipoFuncion)
        {
            this.campoMostrar = campoMostrar;
            this.tipoFuncion = tipoFuncion;
            subCampos = new List<CamposSelect>();
        }

        public string campoMostrar { get; set; }

        /*
            funcion Count funciona con entidad  COUNT(p.*) o COUNT(p.clave)
            los demas deben ser con campos      SUM(p.*) invalido como todo los demas
            SUM(p.clave) valido tomar encuenta tipo dato del campo
        */
        public TipoFuncion tipoFuncion { get; set; }

        ///public Type tipoDato { get; set; }

        public bool usaCaseWhen { get; set; } = false;

        public List<CamposSelect> subCampos { get; set; }

        //Usado para actualizar campos de una tabla
        public object valor { get; set; } = false;

        public bool isFormula { get; set; } = false;

    }
}
