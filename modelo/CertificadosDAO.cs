using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Entity.entidad.cfdi;
using Exitosw.Payroll.Core.genericos.campos;
using System.Reflection;
namespace Exitosw.Payroll.Core.modelo
{
    public class CertificadosDAO : GenericRepository<Certificados>, CertificadosDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        List<Certificados> listaCertificados = new List<Certificados>();
        DateTime fechaActual;
        bool commit = false;

        public Mensaje agregar(Certificados entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<Certificados>().Add(entity);
                getSession().SaveChanges();
                mensajeResultado.resultado = entity;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("agregar()1_Error_Certificados: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje eliminar(Certificados entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<Certificados>().Attach(entity);
                getSession().Set<Certificados>().Remove(entity);
                getSession().SaveChanges();
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("eliminar()1_Error_Certificados: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje modificar(Certificados entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<Certificados>().AddOrUpdate(entity);
                getSession().SaveChanges();
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("actualizar()1_Error_Certificados: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        private List<Certificados> agregarListaCertificados(List<Certificados> entitys, int rango)
        {
            List<Certificados> listCertificados = new List<Certificados>();
            commit = true;
            try
            {
                int i = 0;
                for (i = 0; i < entitys.Count(); i++)
                {
                    if (entitys[i].id > 0)
                    {
                        listCertificados.Add(getSession().Set<Certificados>().Add(entitys[i]));
                    }
                    else
                    {
                        getSession().Set<Certificados>().AddOrUpdate(entitys[i]);
                    }
                    if (i % rango == 0 & i > 0)
                    {
                        getSession().SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("agregarListaCertificados()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return listCertificados;
        }

        public Mensaje saveDeleteBITCancelacion(List<Certificados> entitysCambios, object[] clavesDelete, int rango, DBContextAdapter dbContext)
        {
            listaCertificados = new List<Certificados>();
            try
            {
                commit = true;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (clavesDelete != null)
                {

                    Mensaje mensaje = deleteListQuery("Certificados", new CamposWhere("Certificados.id", clavesDelete, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                    commit = (bool)mensaje.resultado;
                }
                entitysCambios = (entitysCambios == null ? new List<Certificados>() : entitysCambios);
                if (commit && entitysCambios.Count() > 0)
                {
                    listaCertificados = agregarListaCertificados(entitysCambios, rango);
                }
                if (commit)
                {
                    mensajeResultado.resultado = listaCertificados;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }
                else
                {
                    getSession().Database.CurrentTransaction.Rollback();

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteBITCancelacion()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }


        public Mensaje getPorIdRazonesSociales(decimal? id, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var razonSocial = (from rs in getSession().Set<RazonesSociales>()
                                   where rs.id == id
                                   select new
                                   {
                                       rs.clave,
                                       rs.id,
                                       rs.razonsocial
                                   }).SingleOrDefault();
                mensajeResultado.resultado = razonSocial;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorIdRazonesSociales()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }


        public Mensaje getAllCertificados(decimal? idRazonSocial, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var certificado = (from cert in getSession().Set<Certificados>()
                                   where cert.razonesSociales_ID == idRazonSocial
                                   select new
                                   {
                                       cert.id,
                                       cert.clave,
                                       cert.certificado,
                                       cert.llavePrivada,
                                       cert.password,
                                       cert.vigenciaDesde,
                                       cert.vigenciaHasa,
                                       cert.noCertificado,
                                       cert.razonesSociales_ID,
                                   }).SingleOrDefault();
                mensajeResultado.resultado = certificado;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getAllCertificados()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
            
        }

        public Mensaje getPorIdCertificados(decimal? id, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var certificado = (from cert in getSession().Set<Certificados>()
                                   where cert.id == id
                                   select new
                                   {
                                       cert.id,
                                       cert.clave,
                                       cert.certificado,
                                       cert.llavePrivada,
                                       cert.password,
                                       cert.vigenciaDesde,
                                       cert.vigenciaHasa,
                                       cert.noCertificado,
                                       cert.razonesSociales_ID,
                                   }).SingleOrDefault();
                mensajeResultado.resultado = certificado;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getAllCertificados()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;

        }

        public Mensaje getDatosCertificados(byte[] ArchivoCER, byte[] ArchivoKEY, string ClavePrivada, DBContextAdapter dbContext)
        {
            try
            {
                

                
                inicializaVariableMensaje();
                SelloDigital oSelloDigital = new SelloDigital();
                string numeroCertificado,  c;
                DateTime aa, b;
                List<object> listResultado = new List<object>();

                bool rest =  SelloDigital.leerCertDate(ArchivoCER, out aa, out b, out c, out numeroCertificado);
                bool coincide = SelloDigital.validarCERKEYARCHIVO(ArchivoKEY, ClavePrivada);
                                 
                listResultado.Add(aa);
                listResultado.Add(b);
                listResultado.Add(c);
                listResultado.Add(numeroCertificado);
                listResultado.Add(coincide);
                listResultado.Add(rest);
                


                mensajeResultado.resultado = listResultado;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";

            }

            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getDatosCertificados()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;

            }



            return mensajeResultado;
        }

        public Mensaje saveCertificados(Certificados entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();


                if (entity.id == -1)
                {
                    getSession().Set<Certificados>().Add(entity);
                }
                else
                {
                    getSession().Set<Certificados>().AddOrUpdate(entity);
                }                  



                getSession().SaveChanges();
                mensajeResultado.resultado = entity;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveCertificados()1_Error_Certificados: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje eliminarIDCertificados(decimal? id, DBContextAdapter dbContext)
        {
            try
            {
                commit = true;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                List<object> datos1 = new List<object>();
                datos1.Add(id);


                Mensaje mensaje = deleteListQuery("Certificados", new CamposWhere("Certificados.id", datos1.ToArray(), OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                                  
                commit = (bool)mensaje.resultado;
                if (commit)
                {
                    mensajeResultado.resultado = "OK";
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }
                else
                {
                    getSession().Database.CurrentTransaction.Rollback();

                }


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("eliminarIDCertificados()1_Error_Certificados: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje certificadoActual(decimal? idRazonSocial, DBContextAdapter dbContext)
        {
            try
            {
                //List<Certificados> certificadoActual = new List<Certificados>();
                fechaActual = DateTime.Now;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var certificado = (from cert in getSession().Set<Certificados>()
                                   where (cert.razonesSociales_ID == idRazonSocial) &&
                                   (cert.vigenciaDesde <= fechaActual) &&
                                   (cert.vigenciaHasa >= fechaActual)
                                   orderby cert.id
                                   select new
                                   {
                                       cert.id,
                                       cert.clave,
                                       cert.certificado,
                                       cert.llavePrivada,
                                       cert.password,
                                       cert.vigenciaDesde,
                                       cert.vigenciaHasa,
                                       cert.noCertificado,
                                       cert.razonesSociales_ID,
                                   }).First();//SingleOrDefault();


                if (certificado != null)
                {
                    mensajeResultado.resultado = certificado;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }
                else
                {
                    mensajeResultado.resultado = null;
                    mensajeResultado.noError = 10;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }
                
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("certificadoActual()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Certificados certificadoActualId(decimal? idRazonSocial, DBContextAdapter dbContext)
        {
            fechaActual = DateTime.Now;
            inicializaVariableMensaje();
            setSession(dbContext.context);
            //  getSession().Database.BeginTransaction();
            Certificados certResultado = new Certificados();
            List<Certificados> Listcert = new List<Certificados>();
            var query = (from cert in getSession().Set<Certificados>()
                         where (cert.razonesSociales_ID == idRazonSocial) &&
                         (cert.vigenciaDesde <= fechaActual) &&
                         (cert.vigenciaHasa >= fechaActual)
                         orderby cert.id
                         select new
                         {
                             cert
                         });//.First();//SingleOrDefault();
            try
            {
                query = (from subquery in query
                         where (subquery.cert.razonesSociales_ID == idRazonSocial) &&
                         (subquery.cert.vigenciaDesde <= fechaActual) &&
                         (subquery.cert.vigenciaHasa >= fechaActual)
                         select subquery);

                var query2 = (from subquery in query
                              select subquery.cert);
                inicializaVariableMensaje();
                Listcert = query2.ToList<Certificados>();
                if (Listcert != null) {
                    certResultado = Listcert[0];
                }





            }
            catch (Exception ex)
            {
                certResultado = null;
                /*System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("certificadoActual()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;*/
                getSession().Database.CurrentTransaction.Rollback();
            }

            return certResultado;

        }

    }
}
