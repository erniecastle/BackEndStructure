/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Clase ConfiguracionIMSSDAO para llamados a metodos de Entity
* -----------------------------------------------------------------------------
* MODIFICACIONES:
* -----------------------------------------------------------------------------
* Clave: 
* Autor: 
* Fecha:
* Descripción: 
* -----------------------------------------------------------------------------
*/

using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Core.util;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Text;
using Exitosw.Payroll.Entity.util;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public class ConfiguracionIMSSDAO : GenericRepository<ConfiguracionIMSS>, ConfiguracionIMSSDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");

        public Mensaje agregar(ConfiguracionIMSS entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<ConfiguracionIMSS>().Add(entity);
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
        public Mensaje modificar(ConfiguracionIMSS entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<ConfiguracionIMSS>().AddOrUpdate(entity);
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
        public Mensaje eliminar(ConfiguracionIMSS entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<ConfiguracionIMSS>().Attach(entity);
                getSession().Set<ConfiguracionIMSS>().Remove(entity);
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
        public Mensaje getPorIdConfiguracionIMSS(decimal? idIMSS, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var existeconfiguracionIMSS = (from cim in getSession().Set<ConfiguracionIMSS>()
                                               where cim.id == idIMSS

                                               select new
                                               {
                                                   cim.aportacionInfonavitPatron,
                                                   cim.cuotaFijaPatron,
                                                   cim.excedenteEspecie,
                                                   cim.fechaAplica,
                                                   cim.id,
                                                   cim.tasaAportacionInfonavitPatron,
                                                   cim.tasaAportacionRetiroPatron,
                                                   cim.tasaCesantiaVejez,
                                                   cim.tasaCesanVejezPatron,
                                                   cim.tasaDineEnfermeMater,
                                                   cim.tasaEspecieEnfermeMater,
                                                   cim.tasaExcedentePatron,
                                                   cim.tasaFijaPatron,
                                                   cim.tasaGastosPension,
                                                   cim.tasaGastosPensPatron,
                                                   cim.tasaGuarderiaPatron,
                                                   cim.tasaInvalidezVida,
                                                   cim.tasaInvaliVidaPatron,
                                                   cim.tasaPrestDinePatron,
                                                   cim.tasaRiesgosPatron,
                                                   cim.topeCesanVejez,
                                                   cim.topeEnfermedadMaternidad,
                                                   cim.topeInvaliVida,
                                                   cim.topeInfonavit,
                                                   cim.topeRetiro,
                                                   cim.topeRiesgoTrabajoGuarderias
                                               }).SingleOrDefault();
                //existeconfiguracionIMSS = query;
                mensajeResultado.resultado = existeconfiguracionIMSS;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorIdConfiguracionIMSS()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje exiteConfiguracionIMSS(DateTime date, DBContextAdapter dbContext)
        {
            //ConfiguracionIMSS existeconfiguracionIMSS = new ConfiguracionIMSS();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var existeconfiguracionIMSS = (from cim in getSession().Set<ConfiguracionIMSS>()
                                               where cim.fechaAplica == date
                                               orderby cim.fechaAplica descending
                                               select new
                                               {
                                                   cim.id,
                                                   cim.aportacionInfonavitPatron,
                                                   cim.cuotaFijaPatron,
                                                   cim.excedenteEspecie,
                                                   cim.fechaAplica,
                                                   cim.tasaAportacionInfonavitPatron,
                                                   cim.tasaAportacionRetiroPatron,
                                                   cim.tasaCesantiaVejez,
                                                   cim.tasaCesanVejezPatron,
                                                   cim.tasaDineEnfermeMater,
                                                   cim.tasaEspecieEnfermeMater,
                                                   cim.tasaExcedentePatron,
                                                   cim.tasaFijaPatron,
                                                   cim.tasaGastosPension,
                                                   cim.tasaGastosPensPatron,
                                                   cim.tasaGuarderiaPatron,
                                                   cim.tasaInvalidezVida,
                                                   cim.tasaInvaliVidaPatron,
                                                   cim.tasaPrestDinePatron,
                                                   cim.tasaRiesgosPatron,
                                                   cim.topeCesanVejez,
                                                   cim.topeEnfermedadMaternidad,
                                                   cim.topeInvaliVida,
                                                   cim.topeInfonavit,
                                                   cim.topeRetiro,
                                                   cim.topeRiesgoTrabajoGuarderias
                                               }).SingleOrDefault();
                //existeconfiguracionIMSS = query;
                mensajeResultado.resultado = existeconfiguracionIMSS;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("exiteConfiguracionIMSS()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getConfiguracionIMSSActual(DateTime date, DBContextAdapter dbContext)
        {
            //ConfiguracionIMSS configuracionIMSS = new ConfiguracionIMSS();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var configuracionIMSS = (from cim in getSession().Set<ConfiguracionIMSS>()
                                         where cim.fechaAplica <= date
                                         orderby cim.fechaAplica descending
                                         select new
                                         {
                                             cim.id,
                                             cim.aportacionInfonavitPatron,
                                             cim.cuotaFijaPatron,
                                             cim.excedenteEspecie,
                                             cim.fechaAplica,
                                             cim.tasaAportacionInfonavitPatron,
                                             cim.tasaAportacionRetiroPatron,
                                             cim.tasaCesantiaVejez,
                                             cim.tasaCesanVejezPatron,
                                             cim.tasaDineEnfermeMater,
                                             cim.tasaEspecieEnfermeMater,
                                             cim.tasaExcedentePatron,
                                             cim.tasaFijaPatron,
                                             cim.tasaGastosPension,
                                             cim.tasaGastosPensPatron,
                                             cim.tasaGuarderiaPatron,
                                             cim.tasaInvalidezVida,
                                             cim.tasaInvaliVidaPatron,
                                             cim.tasaPrestDinePatron,
                                             cim.tasaRiesgosPatron,
                                             cim.topeCesanVejez,
                                             cim.topeEnfermedadMaternidad,
                                             cim.topeInfonavit,
                                             cim.topeRetiro,
                                             cim.topeRiesgoTrabajoGuarderias
                                         }).SingleOrDefault();
                // configuracionIMSS = query.Take(1).SingleOrDefault();
                mensajeResultado.resultado = configuracionIMSS;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getConfiguracionIMSSActual()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getUltimaConfiguracionIMSS(DateTime date, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var configuracionIMSS = (from cim in getSession().Set<ConfiguracionIMSS>().OrderByDescending(p => p.fechaAplica)

                                         select new
                                         {
                                             cim.id,
                                             cim.aportacionInfonavitPatron,
                                             cim.cuotaFijaPatron,
                                             cim.excedenteEspecie,
                                             cim.fechaAplica,
                                             cim.tasaAportacionInfonavitPatron,
                                             cim.tasaAportacionRetiroPatron,
                                             cim.tasaCesantiaVejez,
                                             cim.tasaCesanVejezPatron,
                                             cim.tasaDineEnfermeMater,
                                             cim.tasaEspecieEnfermeMater,
                                             cim.tasaExcedentePatron,
                                             cim.tasaFijaPatron,
                                             cim.tasaGastosPension,
                                             cim.tasaGastosPensPatron,
                                             cim.tasaGuarderiaPatron,
                                             cim.tasaInvalidezVida,
                                             cim.tasaInvaliVidaPatron,
                                             cim.tasaPrestDinePatron,
                                             cim.tasaRiesgosPatron,
                                             cim.topeCesanVejez,
                                             cim.topeEnfermedadMaternidad,
                                             cim.topeInvaliVida,
                                             cim.topeInfonavit,
                                             cim.topeRetiro,
                                             cim.topeRiesgoTrabajoGuarderias
                                         }
                                         ).FirstOrDefault();
                mensajeResultado.resultado = configuracionIMSS;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getUltimaConfiguracionIMSSActual()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getAllConfiguracionIMSS(DBContextAdapter dbContext)
        {
            List<ConfiguracionIMSS> listConfiguracionIMSSALL = new List<ConfiguracionIMSS>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listConfiguracionIMSSALL = (from a in getSession().Set<ConfiguracionIMSS>() select a).ToList();
                mensajeResultado.resultado = listConfiguracionIMSSALL;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ConfiguracionIMSSAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje obtenerRelacionConfIMSS(decimal id_configuracionIMSS, DBContextAdapter dbContext)
        {
            decimal cantidad = 0;
            bool existe = false;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                cantidad = (from ci in getSession().Set<CalculoIMSS>()
                            where ci.configuracionIMSS.id == id_configuracionIMSS
                            select ci).Count();
                if (cantidad > 0)
                {
                    existe = true;
                }
                else
                {
                    cantidad = (from cip in getSession().Set<CalculoIMSSPatron>()
                                where cip.configuracionIMSS.id == id_configuracionIMSS
                                select cip).Count();
                    if (cantidad > 0)
                    {
                        existe = true;
                    }
                    else
                    {
                        existe = false;
                    }
                }
                mensajeResultado.resultado = existe;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerRelacionConfIMSS()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        
    }
}