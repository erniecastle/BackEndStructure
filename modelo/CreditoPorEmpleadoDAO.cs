/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Clase CreditoPorEmpleadoDAO para llamados a metodos de Entity
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
using Exitosw.Payroll.Core.genericos.campos;
using Exitosw.Payroll.Core.util;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Data.Entity;
using Exitosw.Payroll.Core.campos;

namespace Exitosw.Payroll.Core.modelo
{
    public class CreditoPorEmpleadoDAO : GenericRepository<CreditoPorEmpleado>, CreditoPorEmpleadoDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        private List<CreditoPorEmpleado> listaCreditoMovimientos = new List<CreditoPorEmpleado>();
        private String max = null;

        public Mensaje agregar(CreditoPorEmpleado entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<CreditoPorEmpleado>().Add(entity);
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

        public Mensaje actualizar(CreditoPorEmpleado entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<CreditoPorEmpleado>().AddOrUpdate(entity);
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

        public Mensaje eliminar(CreditoPorEmpleado entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<CreditoPorEmpleado>().Attach(entity);
                getSession().Set<CreditoPorEmpleado>().Remove(entity);
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

        public Mensaje consultaPorRangosCreditoPorEmpleado(Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            listaCreditoMovimientos = new List<CreditoPorEmpleado>();

            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                ValoresRango rangos = new ValoresRango(Convert.ToInt32(inicio), Convert.ToInt32(rango));

                mensajeResultado = consultaPorRangos(rangos, null, null);
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorRangos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        //public Mensaje existeDato(string campo, object valor, DBContextAdapter dbContext)
        //{
        //    bool existe = false;
        //    try
        //    {
        //        inicializaVariableMensaje();
        //        setSession(dbContext);
        //        getSession().Database.BeginTransaction();
        //        //existe = existeDato("CreditoPorEmpleado", campo, valor);
        //        mensajeResultado.resultado = existe;
        //        mensajeResultado.noError = 0;
        //        mensajeResultado.error = "";
        //        getSession().Database.CurrentTransaction.Commit();
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("existeDato()1_Error: ").Append(ex));
        //        mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
        //        mensajeResultado.error = ex.GetBaseException().ToString();
        //        mensajeResultado.resultado = null;
        //        getSession().Database.CurrentTransaction.Rollback();
        //    }
        //    return mensajeResultado;
        //}

        public Mensaje existenMovimientosEnCreditos(string numeroDeCredito, string claveCreditoAhorro, string tipoConfiguracion, DBContextAdapter dbContext)
        {
            int tam = 0;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                tam = (from cm in getSession().Set<CreditoMovimientos>()
                       where cm.creditoPorEmpleado.numeroCredito.Equals(numeroDeCredito) &&
                       cm.creditoPorEmpleado.creditoAhorro.clave.Equals(claveCreditoAhorro) &&
                       cm.creditoPorEmpleado.creditoAhorro.tipoConfiguracion.Equals(tipoConfiguracion)
                       select cm).Count();
                if (tam > 0)
                {
                    mensajeResultado.resultado = true;
                }
                else
                {
                    mensajeResultado.resultado = false;
                }
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

        public Mensaje getCreditosAll(string claveTipoCredito, string tipoConfiguracion, DBContextAdapter dbContext)
        {
            listaCreditoMovimientos = new List<CreditoPorEmpleado>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listaCreditoMovimientos = (from a in getSession().Set<CreditoPorEmpleado>()
                                           where a.creditoAhorro.clave.Equals(claveTipoCredito) &&
                                           a.creditoAhorro.tipoConfiguracion.Equals(tipoConfiguracion)
                                           select a).ToList();
                mensajeResultado.resultado = listaCreditoMovimientos;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("CreditosAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getCreditosPorClave(string numeroCredito, string claveTipoCredito, string tipoConfiguracion, DBContextAdapter dbContext)
        {
            CreditoPorEmpleado creditoMovimientos = new CreditoPorEmpleado();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                creditoMovimientos = (from a in getSession().Set<CreditoPorEmpleado>()
                                      where a.numeroCredito.Equals(numeroCredito) &&
                                      a.creditoAhorro.clave.Equals(claveTipoCredito) &&
                                      a.creditoAhorro.tipoConfiguracion.Equals(tipoConfiguracion)
                                      select a).SingleOrDefault();
                mensajeResultado.resultado = creditoMovimientos;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("CreditosPorClave()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getCreditosPorTipoCredito(string claveTipoCredito, string tipoConfiguracion, DBContextAdapter dbContext)
        {
            listaCreditoMovimientos = new List<CreditoPorEmpleado>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listaCreditoMovimientos = (from a in getSession().Set<CreditoPorEmpleado>()
                                           where a.creditoAhorro.clave.Equals(claveTipoCredito) &&
                                           a.creditoAhorro.tipoConfiguracion.Equals(tipoConfiguracion)
                                           orderby a.numeroCredito descending
                                           select a).ToList();
                mensajeResultado.resultado = listaCreditoMovimientos;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("CreditosPorTipoCredito()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getCreditosPorTipoCreditoYFecha(DateTime fecha, string tipoCredito, string claveRazonsocial, string tipoConfiguracion, DBContextAdapter dbContext)
        {
            listaCreditoMovimientos = new List<CreditoPorEmpleado>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listaCreditoMovimientos = (from a in getSession().Set<CreditoPorEmpleado>()
                                           where a.fechaCredito == fecha &&
                                           a.creditoAhorro.clave.Equals(tipoCredito) &&
                                           a.razonesSociales.clave.Equals(claveRazonsocial) &&
                                           a.creditoAhorro.tipoConfiguracion.Equals(tipoConfiguracion)
                                           select a).ToList();
                mensajeResultado.resultado = listaCreditoMovimientos;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("CreditosPorTipoCreditoYFecha()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje saveDeleteCreditos(List<CreditoPorEmpleado> entitysCambios, object[] eliminados, DBContextAdapter dbContext)
        {
            listaCreditoMovimientos = new List<CreditoPorEmpleado>();
            entitysCambios = (entitysCambios == null ? new List<CreditoPorEmpleado>() : entitysCambios);
            eliminados = eliminados == null ? new Object[] { } : eliminados;
            inicializaVariableMensaje();
            setSession(dbContext.context);
            CreditoPorEmpleado cred = null;
            max = null;
            try
            {
                bool commit = true;
                getSession().Database.BeginTransaction();
                if (!entitysCambios.Any())
                {
                    /* max = obtenerClaveStringMax("CreditoPorEmpleado",
                       new String[] { "razonesSociales.clave", "creditoAhorro.clave", "creditoAhorro.tipoConfiguracion" },
                       new Object[]{entitysCambios[0].razonesSociales.clave,
                             entitysCambios[0].creditoAhorro.clave, entitysCambios[0].creditoAhorro.tipoConfiguracion}, "numeroCredito");*/
                }
                if (eliminados != null && eliminados.Count() > 0)
                {
                    commit = deleteListQuery(eliminados, dbContext);
                    //clear();
                }
                if (commit && entitysCambios.Any())
                {
                    for (int i = 0; i < entitysCambios.Count(); i++)
                    {
                        cred = entitysCambios[i];
                        //if (entitysCambios[i].id == 0)
                        //{
                        //    getSession().Set<CreditoPorEmpleado>().Add(entitysCambios[i]);
                        //}
                        //else
                        //{
                            getSession().Set<CreditoPorEmpleado>().AddOrUpdate(entitysCambios[i]);
                        //}

                    }
                    getSession().SaveChanges();
                }
                if (commit)
                {
                    getSession().Database.CurrentTransaction.Commit();
                    mensajeResultado.resultado = true;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                }
                else
                {
                    getSession().Database.CurrentTransaction.Rollback();
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteCreditos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = cred;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        private bool deleteListQuery(Object[] eliminados, DBContextAdapter dbContext)
        {
            bool commit = true;
            try
            {
                //deleteListQuery("CreditoPorEmpleado", "Id", eliminados);
                deleteListQuery("CreditoPorEmpleado", new CamposWhere("CreditoPorEmpleado.id", eliminados, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("deleteListQuery()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                commit = false;
            }
            return commit;
        }

        public Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                mensajeResultado = existeClave(tabla, campoWhere, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getExisteClave()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                getSession().Database.CurrentTransaction.Rollback();
                mensajeResultado.resultado = null;
            }
            return mensajeResultado;
        }

        public Mensaje getCredPorTipoCreditoYFecha(DateTime fecha, string tipoCredito, string claveRazonsocial, string tipoConfiguracion, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var listaCreditoMovimientos = (from a in getSession().Set<CreditoPorEmpleado>()
                                               where a.fechaCredito == fecha &&
                                               a.creditoAhorro.clave.Equals(tipoCredito) &&
                                               a.razonesSociales.clave.Equals(claveRazonsocial) &&
                                               a.creditoAhorro.tipoConfiguracion.Equals(tipoConfiguracion)
                                               select new
                                               {

                                                   a.creditoAhorro_ID,
                                                   a.cuentaContable,
                                                   empleados = new
                                                   {
                                                       a.empleados.clave,
                                                       a.empleados.nombre,
                                                       a.empleados.apellidoPaterno,
                                                       a.empleados.apellidoMaterno
                                                   },
                                                   a.empleados_ID,
                                                   a.fechaAutorizacion,
                                                   a.fechaCredito,
                                                   a.fechaVence,
                                                   a.id,
                                                   a.inicioDescuento,
                                                   a.modoDescuentoCredito,
                                                   a.montoDescuento,
                                                   a.numeroCredito,
                                                   a.numeroEmpleadoExtra,
                                                   a.numeroParcialidades,
                                                   periodosNomina=new { a.periodosNomina.clave,a.periodosNomina.descripcion,a.periodosNomina.año },
                                                   a.periodosNomina_ID,
                                                   a.razonesSociales_ID,

                                                   a.totalCredito
                                               }).ToList();
                mensajeResultado.resultado = listaCreditoMovimientos;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getCredPorTipoCreditoYFecha()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
    }
}