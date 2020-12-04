/**
* @author: Daniel Ruelas 
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Clase RazonSocialConfiguracionDAO para llamados a metodos de Entity
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
using System.Collections.Generic;
using System.Linq;
using Exitosw.Payroll.Entity.entidad;
using System.Text;
using System.Reflection;
using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.Entity.util;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public class RazonSocialConfiguracionDAO : GenericRepository<RazonSocialConfiguracion>, RazonSocialConfiguracionDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        public Mensaje getAllRazonSocialConfiguracion(DBContextAdapter dbContext)
        {
            List<RazonSocialConfiguracion> razoneSocialesConfiguracion = new List<RazonSocialConfiguracion>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                razoneSocialesConfiguracion = (from p in getSession().Set<RazonSocialConfiguracion>()
                                               select p).ToList();
                mensajeResultado.resultado = razoneSocialesConfiguracion;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getRazonSocialConfiguracionAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getRazonSocialConfiguracionPorClave(string clavesRazonSocial, string claveUsuario, DBContextAdapter dbContext)
        {
            RazonSocialConfiguracion razonSocialConfiguracion;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                razonSocialConfiguracion = (from p in getSession().Set<RazonSocialConfiguracion>()
                                            join r in getSession().Set<RazonSocial>()
                                            on p.razonSocial.id equals r.id
                                            where r.claveRazonSocial == clavesRazonSocial && p.usuario.clave == claveUsuario
                                            select p).SingleOrDefault();
                mensajeResultado.resultado = razonSocialConfiguracion;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getRazonSocialConfiguracionPorClave()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getRazonSocialConfiguracionPorRazonSocial(string claveRazonSocial, DBContextAdapter dbContext)
        {
            List<RazonSocialConfiguracion> razoneSocialesConfiguracion = new List<RazonSocialConfiguracion>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                razoneSocialesConfiguracion = (from p in getSession().Set<RazonSocialConfiguracion>()
                                               where p.razonSocial.claveRazonSocial == claveRazonSocial
                                               select p).ToList();
                mensajeResultado.resultado = razoneSocialesConfiguracion;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getRazonSocialConfiguracionPorRazonSocial()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getRazonSocialConfiguracionPorUsuario(int idUsuario, DBContextAdapter dbContext)
        {
            //List<RazonSocialConfiguracion> razoneSocialesConfiguracion = new List<RazonSocialConfiguracion>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var razoneSocialesConfiguracion = (from p in getSession().Set<RazonSocialConfiguracion>()
                                                   where p.usuario.id == idUsuario
                                                   select new
                                                   {
                                                       id = p.id,
                                                       permitido = p.permitido,
                                                       razonSocial =
                                                       new
                                                       {
                                                           p.razonSocial.id,
                                                           p.razonSocial.claveRazonSocial,
                                                           p.razonSocial.nombreRazonSocial
                                                       }
                                                   }
                                          ).ToList();

                mensajeResultado.resultado = razoneSocialesConfiguracion;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getRazonSocialConfiguracionPorUsuario()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
    }
}