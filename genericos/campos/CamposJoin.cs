using System;


namespace Exitosw.Payroll.Core.genericos.campos
{
    public class CamposJoin
    {

        public CamposJoin()
        {

        }

        public CamposJoin(Type campo, string alias, string nombreCampoBD, string campoId)
        {
            this.campo = campo;
            this.alias = alias;
            this.nombreCampoBD = nombreCampoBD;
            this.campoId = campoId;
            this.tipoJoin = TipoJoin.LEFT_JOIN;
        }

        public string alias { get; set; }

        public Type campo { get; set; }

        public string nombreCampoBD { get; set; }

        public string campoId { get; set; }

        public TipoJoin? tipoJoin { get; set; } = TipoJoin.LEFT_JOIN;

    }
}
