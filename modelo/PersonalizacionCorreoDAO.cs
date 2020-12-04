/**
* @author: Ernesto Castillo
* Fecha de Creación: 30/04/2020
* Compañía: Macropro
* Descripción del programa: Clase PersonalizacionCorreoDAO para llamados a metodos de Entity
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

namespace Exitosw.Payroll.Core.modelo
{
    public class PersonalizacionCorreoDAO : GenericRepository<PersonalizacionCorreo>, PersonalizacionCorreoDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        public Mensaje agregar(PersonalizacionCorreo entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<PersonalizacionCorreo>().Add(entity);
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

        public Mensaje modificar(PersonalizacionCorreo entity, decimal? idRazonSocial, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                dbContext.context.Database.BeginTransaction();
                var getCnf = (from cnMail in dbContext.context.Set<ConfiguracionCorreo>()
                              where cnMail.razonesSociales_ID == idRazonSocial
                              select new
                              {
                                  id = cnMail.id
                              }).SingleOrDefault();

                if (getCnf != null)
                {
                    entity.configuracionCorreo_ID = getCnf.id;
                }

                //Check if exitst key
                var getKeyPerCor = (from perMail in dbContext.context.Set<PersonalizacionCorreo>()
                                    where perMail.tipoArchivo == entity.tipoArchivo && perMail.configuracionCorreo_ID == getCnf.id
                                    select new
                                    {
                                        id = perMail.id
                                    }).SingleOrDefault();

                if (getKeyPerCor == null)
                {
                    entity.id = 0;
                }
                else {
                    entity.id = getKeyPerCor.id;
                }

                dbContext.context.Set<PersonalizacionCorreo>().AddOrUpdate(entity);
                dbContext.context.SaveChanges();
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                dbContext.context.Database.CurrentTransaction.Commit();
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

        public Mensaje eliminar(PersonalizacionCorreo entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<PersonalizacionCorreo>().Attach(entity);
                getSession().Set<PersonalizacionCorreo>().Remove(entity);
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

        public Mensaje getPersonalizacionCorreo(decimal? idRazonSocial, int tipoDeArchivo, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();

                var getCnf = (from cnMail in dbContext.context.Set<ConfiguracionCorreo>()
                              where cnMail.razonesSociales_ID == idRazonSocial
                              select new
                              {
                                  id = cnMail.id
                              }).SingleOrDefault();

                if (getCnf == null)
                {
                    mensajeResultado.resultado = null;
                }
                else
                {
                    var listaPerCorreo = (from perMail in dbContext.context.Set<PersonalizacionCorreo>()
                                          where perMail.configuracionCorreo_ID == getCnf.id && perMail.tipoArchivo == tipoDeArchivo
                                          select new
                                          {
                                              id = perMail.id,
                                              tipoArchivo = perMail.tipoArchivo,
                                              asunto = perMail.asunto,
                                              texto = perMail.texto,
                                              configuracionCorreo_ID = perMail.configuracionCorreo_ID
                                          }).ToList();
                    mensajeResultado.resultado = listaPerCorreo.Count == 0 ? null : listaPerCorreo;
                }
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPersonalizacionCorreo()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
    }
}
