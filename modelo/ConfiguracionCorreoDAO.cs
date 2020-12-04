/**
* @author: Ernesto Castillo
* Fecha de Creación: 29/04/2020
* Compañía: Macropro
* Descripción del programa: Clase ConfiguracionCorreoDAO para llamados a metodos de Entity
* -----------------------------------------------------------------------------
* MODIFICACIONES:
* -----------------------------------------------------------------------------
* Clave: 
* Autor: 
* Fecha:
* Descripción: 
* -----------------------------------------------------------------------------
*/

using System;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Data.Entity.Migrations;
using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Core.util;
using System.Net.Mail;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Exitosw.Payroll.Core.modelo
{
    public class ConfiguracionCorreoDAO : GenericRepository<ConfiguracionCorreo>, ConfiguracionCorreoDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        public Mensaje agregar(ConfiguracionCorreo entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<ConfiguracionCorreo>().Add(entity);
                getSession().SaveChanges();
                mensajeResultado.resultado = entity;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("agregar()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje modificar(ConfiguracionCorreo entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<ConfiguracionCorreo>().AddOrUpdate(entity);
                getSession().SaveChanges();
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("actualizar()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje eliminar(ConfiguracionCorreo entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<ConfiguracionCorreo>().Attach(entity);
                getSession().Set<ConfiguracionCorreo>().Remove(entity);
                getSession().SaveChanges();
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("eliminar()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje enviarCoreo(ConfiguracionCorreo entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();


                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(entity.SMTP);
                mail.From = new MailAddress(entity.correoRemitente);
                mail.To.Add(entity.correoPrueba);

                mail.Subject = "Entrega de archivo";
                mail.Body = entity.texto;

                mail.IsBodyHtml = true;
                SmtpServer.Port = entity.puerto;
                SmtpServer.Credentials = new System.Net.NetworkCredential(entity.usuario, entity.password);
                // Credentials are necessary if the server requires the client 
                // to authenticate before it will send email on the client's behalf.
                //client.UseDefaultCredentials = true;
                SmtpServer.EnableSsl = entity.SSL;

               //// ServicePointManager.ServerCertificateValidationCallback =

               ////delegate (object s

               ////    , X509Certificate certificate

               ////    , X509Chain chain

               ////    , SslPolicyErrors sslPolicyErrors)

               ////{ return true; };



                SmtpServer.Send(mail);

                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("agregar()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return mensajeResultado;
        }

        public Mensaje getConfiguracionCorreo(decimal? idRazonSocial, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var listaCnfCorreo = (from cnMail in getSession().Set<ConfiguracionCorreo>()
                                      where cnMail.razonesSociales_ID == idRazonSocial
                                      select new
                                      {
                                          id = cnMail.id,
                                          SMTP = cnMail.SMTP,
                                          puerto = cnMail.puerto,
                                          usuario = cnMail.usuario,
                                          password = cnMail.password,
                                          SSL = cnMail.SSL,
                                          correoRemitente = cnMail.correoRemitente,
                                          correoPrueba = cnMail.correoPrueba,
                                          texto = cnMail.texto,
                                          razonesSociales_ID = cnMail.razonesSociales_ID,
                                      }).ToList();
                mensajeResultado.resultado = listaCnfCorreo.Count() == 0 ? null : listaCnfCorreo;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getConfiguracionCorreo()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

    }
}
