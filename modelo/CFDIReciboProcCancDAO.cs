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
    public class CFDIReciboProcCancDAO :  GenericRepository<CFDIReciboProcCanc>, CFDIReciboProcCancDAOIF
    {

        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        List<CFDIReciboProcCanc> listaCFDIRecibo_Proc_Canc = new List<CFDIReciboProcCanc>();
        bool commit = false;

        public Mensaje agregar(CFDIReciboProcCanc entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<CFDIReciboProcCanc>().Add(entity);
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

        public Mensaje eliminar(CFDIReciboProcCanc entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<CFDIReciboProcCanc>().Attach(entity);
                getSession().Set<CFDIReciboProcCanc>().Remove(entity);
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

        public Mensaje modificar(CFDIReciboProcCanc entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<CFDIReciboProcCanc>().AddOrUpdate(entity);
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


        private List<CFDIReciboProcCanc> agregarListaCFDIRecibo_Proc_Canc(List<CFDIReciboProcCanc> entitys, int rango)
        {
            List<CFDIReciboProcCanc> listCFDIRecibo_Proc_Canc = new List<CFDIReciboProcCanc>();
            commit = true;
            try
            {
                int i = 0;
                for (i = 0; i < entitys.Count(); i++)
                {
                    if (entitys[i].id > 0)
                    {
                        listCFDIRecibo_Proc_Canc.Add(getSession().Set<CFDIReciboProcCanc>().Add(entitys[i]));
                    }
                    else
                    {
                        getSession().Set<CFDIReciboProcCanc>().AddOrUpdate(entitys[i]);
                    }
                    if (i % rango == 0 & i > 0)
                    {
                        getSession().SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("agregarListaCFDIRecibo_Proc_Canc()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return listCFDIRecibo_Proc_Canc;
        }

        public Mensaje saveDeleteCFDIRecibo_Proc_Canc(List<CFDIReciboProcCanc> entitysCambios, object[] clavesDelete, int rango, DBContextAdapter dbContext)
        {
            listaCFDIRecibo_Proc_Canc = new List<CFDIReciboProcCanc>();
            try
            {
                commit = true;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (clavesDelete != null)
                {

                    Mensaje mensaje = deleteListQuery("CFDIReciboProcCanc", new CamposWhere("CFDIReciboProcCanc.id", clavesDelete, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                    commit = (bool)mensaje.resultado;
                }
                entitysCambios = (entitysCambios == null ? new List<CFDIReciboProcCanc>() : entitysCambios);
                if (commit && entitysCambios.Count() > 0)
                {
                    listaCFDIRecibo_Proc_Canc = agregarListaCFDIRecibo_Proc_Canc(entitysCambios, rango);
                }
                if (commit)
                {
                    mensajeResultado.resultado = listaCFDIRecibo_Proc_Canc;
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteCFDIRecibo_Proc_Canc()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
    }
}
