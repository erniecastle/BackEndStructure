/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Clase CruceDAO para llamados a metodos de Entity
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
    public class CruceDAO : GenericRepository<Cruce>, CruceDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");

        public Mensaje agregar(DBContextAdapter dbContext, Cruce entity)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<Cruce>().Add(entity);
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

        public Mensaje actualizar(DBContextAdapter dbContext, Cruce entity)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<Cruce>().AddOrUpdate(entity);
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

        public Mensaje eliminar(DBContextAdapter dbContext, Cruce entity)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<Cruce>().Attach(entity);
                getSession().Set<Cruce>().Remove(entity);
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

        public Mensaje DeleteCruces(DBContextAdapter dbContext, List<Cruce> c)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                foreach (Cruce cu in c)
                {
                    getSession().Set<Cruce>().AddOrUpdate(cu);
                    getSession().SaveChanges();
                }
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("existenMovimientosEnCreditos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje existeClaveElemento(DBContextAdapter dbContext, string claveelemento, string elemento, decimal parametro)
        {
            bool result;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                List<Cruce> listcruce = (from c in getSession().Set<Cruce>()
                                         where c.claveElemento.Equals(claveelemento) &&
                                         c.elementosAplicacion.clave.Equals(elemento) &&
                                         c.parametros.clave.Equals(parametro)
                                         select c).ToList();
                List<Cruce>.Enumerator list = listcruce.GetEnumerator();
                result = list.MoveNext();
                mensajeResultado.resultado = result;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("existeClaveElemento()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getAllCruce(DBContextAdapter dbContext)
        {
            List<Cruce> listaCruce = new List<Cruce>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listaCruce = (from a in getSession().Set<Cruce>()
                              orderby a.ordenId ascending
                              select a).ToList();
                mensajeResultado.resultado = listaCruce;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("CruceAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getCrucePorElemento(DBContextAdapter dbContext, string elemento)
        {
            List<Cruce> listaCruceXElem = new List<Cruce>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listaCruceXElem = (from a in getSession().Set<Cruce>()
                                   where a.elementosAplicacion.clave.Equals(elemento)
                                   orderby a.ordenId ascending
                                   select a).ToList();
                mensajeResultado.resultado = listaCruceXElem;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("CrucePorElemento()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getCrucePorElemeYClave(DBContextAdapter dbContext, string elemento, string claveelemento)
        {
            List<Cruce> listaCruceXElemYClav = new List<Cruce>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listaCruceXElemYClav = (from a in getSession().Set<Cruce>()
                                   where a.elementosAplicacion.clave.Equals(elemento) &&
                                   a.claveElemento.Equals(claveelemento)
                                   orderby a.ordenId ascending
                                   select a).ToList();
                mensajeResultado.resultado = listaCruceXElemYClav;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("CrucePorElemeYClave()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getCrucePorParaElemeYClave(DBContextAdapter dbContext, decimal claveParametro, string elemento, string claveelemento)
        {
            List<Cruce> listaCruceXParaElemYClav = new List<Cruce>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listaCruceXParaElemYClav = (from a in getSession().Set<Cruce>()
                                        where a.parametros.clave.Equals(claveParametro) &&
                                        a.elementosAplicacion.clave.Equals(elemento) &&
                                        a.claveElemento.Equals(claveelemento)
                                        orderby a.ordenId ascending
                                        select a).ToList();
                mensajeResultado.resultado = listaCruceXParaElemYClav;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("CrucePorParaElemeYClave()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getCrucePorParamElemento(DBContextAdapter dbContext, decimal claveParametro, string elemento)
        {
            List<Cruce> listaCruceXParaElem = new List<Cruce>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listaCruceXParaElem = (from a in getSession().Set<Cruce>()
                                            where a.parametros.clave.Equals(claveParametro) &&
                                            a.elementosAplicacion.clave.Equals(elemento) 
                                            orderby a.ordenId ascending
                                            select a).ToList();
                mensajeResultado.resultado = listaCruceXParaElem;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("CrucePorParamElemento()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getCrucePorParametros(DBContextAdapter dbContext, decimal claveParametro)
        {
            List<Cruce> listaCruceXPara = new List<Cruce>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listaCruceXPara = (from a in getSession().Set<Cruce>()
                                       where a.parametros.clave.Equals(claveParametro)
                                       orderby a.ordenId ascending
                                       select a).ToList();
                mensajeResultado.resultado = listaCruceXPara;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("CrucePorParametros()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje SaveCruces(DBContextAdapter dbContext, List<Cruce> c)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                for(int i=0;i < c.Count(); i++)
                {
                    getSession().Set<Cruce>().AddOrUpdate(c[i]);
                    if (i % 100 == 0)
                    {
                        getSession().SaveChanges();
                    }
                }
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("SaveCruces()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje SaveDeleteCruces(DBContextAdapter dbContext, List<Cruce> AgreModif, List<Cruce> Ordenados, object[] clavesDelete)
        {
            bool exito = true;
            int order = 0;
            Cruce cruce = new Cruce();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if(clavesDelete != null)
                {
                    //deleteListQuery("Cruce", "id", clavesDelete);
                }
                ////Guardado de modificados
                foreach (Cruce cm in AgreModif)
                {
                    cruce = cm;
                    getSession().Set<Cruce>().Add(cm);
                    getSession().SaveChanges();
                }
                if(Ordenados.Count() > 0)
                {
                    order = Ordenados[0].id;
                }
                //Guardado de Ordenados
                foreach (Cruce cruc in Ordenados)
                {
                    cruc.ordenId=order;
                    cruce = cruc;
                    getSession().Set<Cruce>().Add(cruc);
                    getSession().SaveChanges();
                    order++;
                }
                if (exito)
                {
                    cruce = null;
                    getSession().Database.CurrentTransaction.Commit();
                    mensajeResultado.resultado = cruce;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("SaveDeleteCruces()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
    }
}