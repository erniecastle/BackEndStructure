/**
* @author: Claudia Zavala 
* Fecha de Creación: 03/11/2020
* Compañía: Exito
* Descripción del programa: Interface CertificadosDAOIF para llamados a metodos de Entity
* -----------------------------------------------------------------------------

*/

using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Core.genericos.campos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.Entity.entidad.cfdi;

namespace Exitosw.Payroll.Core.modelo
{
    interface CertificadosDAOIF : IGenericRepository<Certificados>
    {
        Mensaje agregar(Certificados entity, DBContextAdapter dbContext);

        Mensaje modificar(Certificados entity, DBContextAdapter dbContext);

        Mensaje eliminar(Certificados entity, DBContextAdapter dbContext);
        Mensaje saveDeleteBITCancelacion(List<Certificados> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);

        Mensaje getPorIdRazonesSociales(decimal? id, DBContextAdapter dbContext);

        Mensaje getAllCertificados(decimal? idRazonSocial,DBContextAdapter dbContext);

        Mensaje getPorIdCertificados(decimal? id, DBContextAdapter dbContext);

        Mensaje getDatosCertificados(byte[] ArchivoCER, byte[] ArchivoKEY, string ClavePrivada, DBContextAdapter dbContext);

        Mensaje saveCertificados(Certificados entity, DBContextAdapter dbContext);

        Mensaje eliminarIDCertificados(decimal? id, DBContextAdapter dbContext);

        Mensaje certificadoActual(decimal? idRazonSocial, DBContextAdapter dbContext);

        Certificados certificadoActualId(decimal? idRazonSocial, DBContextAdapter dbContext);

    }
}
