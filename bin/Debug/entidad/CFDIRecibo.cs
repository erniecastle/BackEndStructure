/**
 * @author: Ernesto Castillo
 * Fecha de Creación: 21/02/2018
 * Compañía: Exito
 * Descripción del programa: Entidad para HBRequest
 * -----------------------------------------------------------------------------
 */

using System.Collections.Generic;

namespace Exitosw.Payroll.Hibernate.entidad
{

    public partial class CFDIRecibo
    {

        public CFDIRecibo()
        {
            this.cfdiEmpleado = new List<CFDIEmpleado>();
            this.cfdiReciboConcepto = new List<CFDIReciboConcepto>();
            this.cfdiReciboHrsExtras = new List<CFDIReciboHrsExtras>();
            this.cfdiReciboIncapacidad = new List<CFDIReciboIncapacidad>();

        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual string UUID
        {
            get;
            set;
        }

        public virtual string serie
        {
            get;
            set;
        }

        public virtual byte[] acuse
        {
            get;
            set;
        }

        public virtual string UUIDRelacionado
        {
            get;
            set;
        }

        public virtual string cadenaCertificado
        {
            get;
            set;
        }

        public virtual string cadenaOriginalTimbrado
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaGeneraInfo
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaHoraTimCancelado
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaEmision
        {
            get;
            set;
        }

        public virtual string folioCFDI
        {
            get;
            set;
        }

        public virtual string leyenda
        {
            get;
            set;
        }

        public virtual string motivoCancelacion
        {
            get;
            set;
        }

        public virtual string noCertificado
        {
            get;
            set;
        }

        public virtual string noCertificadoSAT
        {
            get;
            set;
        }

        public virtual string rfcProvCertif
        {
            get;
            set;
        }

        public virtual string sello
        {
            get;
            set;
        }

        public virtual string selloCFD
        {
            get;
            set;
        }

        public virtual string selloSAT
        {
            get;
            set;
        }

        public virtual string serieCFDI
        {
            get;
            set;
        }

        public virtual int statusTimbrado
        {
            get;
            set;
        }

        public virtual int statusXmlSat
        {
            get;
            set;
        }

        public virtual bool statusCorreo
        {
            get;
            set;
        }

        public virtual double? total
        {
            get;
            set;
        }

        public virtual string version
        {
            get;
            set;
        }

        public virtual byte[] xmlTimbrado
        {
            get;
            set;
        }

        public virtual string certificadoTimbrado
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaHoraTimbrado
        {
            get;
            set;
        }

        public virtual string selloTimbrado
        {
            get;
            set;
        }

        public virtual IList<CFDIEmpleado> cfdiEmpleado
        {
            get;
            set;
        }

        public virtual IList<CFDIReciboConcepto> cfdiReciboConcepto
        {
            get;
            set;
        }

        public virtual IList<CFDIReciboHrsExtras> cfdiReciboHrsExtras
        {
            get;
            set;
        }

        public virtual IList<CFDIReciboIncapacidad> cfdiReciboIncapacidad
        {
            get;
            set;
        }
    }

}
