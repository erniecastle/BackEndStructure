/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Clase CreditoMovimientosDAO para llamados a metodos de Entity
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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace Exitosw.Payroll.Core.modelo
{
    public class CreditoMovimientosDAO : GenericRepository<CreditoMovimientos>, CreditoMovimientosDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        public Mensaje getCreditoMovimientos(DateTime fecha, string tipoCredito, TiposMovimiento tipoMovimiento, string razonesSociales, string tipoConfiguracion, DBContextAdapter dbContext)
        {
            List<CreditoMovimientos> listaCreditoMovimientos = new List<CreditoMovimientos>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listaCreditoMovimientos = (from a in getSession().Set<CreditoMovimientos>()
                                           where a.fecha == fecha &&
                                           a.creditoPorEmpleado.creditoAhorro.clave.Equals(tipoCredito) &&
                                           a.tiposMovimiento == tipoMovimiento &&
                                           a.creditoPorEmpleado.creditoAhorro.tipoConfiguracion.Equals(tipoConfiguracion) &&
                                           a.creditoPorEmpleado.razonesSociales.clave.Equals(razonesSociales)
                                           select a).ToList();
                mensajeResultado.resultado = listaCreditoMovimientos;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("CreditoMovimientos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getCreditoMovimientosXEntity(DateTime fecha, decimal tipoCredito, TiposMovimiento tipoMovimiento, decimal razonesSociales, string tipoConfiguracion, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                List<CreditoMovimientos> listaCreditoMovimientos = (from a in getSession().Set<CreditoMovimientos>()
                                                                    where a.fecha == fecha.Date &&
                                                                    a.creditoPorEmpleado.creditoAhorro.id == tipoCredito &&
                                                                    a.tiposMovimiento == tipoMovimiento &&
                                                                    a.creditoPorEmpleado.creditoAhorro.tipoConfiguracion.Equals(tipoConfiguracion) &&
                                                                    a.creditoPorEmpleado.razonesSociales.id == razonesSociales
                                                                    select a).ToList();

                var lista = listaCreditoMovimientos.Select(c => new
                {
                    creditoPorEmpleado = new { c.creditoPorEmpleado.numeroCredito, c.creditoPorEmpleado.saldo,c.creditoPorEmpleado.montoDescuento },
                    c.creditoPorEmpleado_ID,
                    c.fecha,
                    c.id,
                    empleados = new
                    {
                        c.creditoPorEmpleado.empleados.clave,
                        c.creditoPorEmpleado.empleados_ID,
                        c.creditoPorEmpleado.empleados.nombre,
                        c.creditoPorEmpleado.empleados.apellidoMaterno,
                        c.creditoPorEmpleado.empleados.apellidoPaterno
                    },
                    empleados_ID = c.creditoPorEmpleado.empleados_ID,

                    initPeriodNom = c.initPeriodNom == null ? null : new { c.initPeriodNom.clave, c.initPeriodNom.año, c.initPeriodNom.descripcion },
                    c.initPeriodNom_ID,
                    c.numeroPeriodosBloquear,
                    c.tiposMovimiento,
                    c.importe,
                    c.observaciones
                }).ToList();
                //select new
                //{
                //    creditoPorEmpleado = new { a.creditoPorEmpleado.numeroCredito },
                //    a.creditoPorEmpleado_ID,
                //    a.fecha,
                //    a.id,
                //    empleados = new
                //    {
                //        a.creditoPorEmpleado.empleados.clave,
                //        a.creditoPorEmpleado.empleados_ID,
                //        a.creditoPorEmpleado.empleados.nombre,
                //        a.creditoPorEmpleado.empleados.apellidoMaterno,
                //        a.creditoPorEmpleado.empleados.apellidoPaterno
                //    },
                //    empleados_ID = a.creditoPorEmpleado.empleados_ID,

                //    initPeriodNom = new { a.initPeriodNom.clave, a.initPeriodNom.año, a.initPeriodNom.descripcion },
                //    a.initPeriodNom_ID,
                //    a.numeroPeriodosBloquear,
                //    a.tiposMovimiento,
                //    a.importe,
                //    a.observaciones
                //}).ToList();
                mensajeResultado.resultado = lista;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("CreditoMovimientos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getMaxNumeroCreditoMovimiento(CreditoPorEmpleado credito, TiposMovimiento tiposMovimiento, DateTime fecha, DBContextAdapter dbContext)
        {
            Object maxValue;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                maxValue = (from a in getSession().Set<CreditoMovimientos>()
                            where a.creditoPorEmpleado.id == credito.id &&
                            a.tiposMovimiento == tiposMovimiento &&
                            a.fecha == fecha
                            select new { a.numeroPeriodosBloquear }).Max(p => p.numeroPeriodosBloquear);
                mensajeResultado.resultado = maxValue;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("MaxNumeroCreditoMovimiento()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje saveDeleteCreditosMov(List<CreditoMovimientos> entitysCambios, object[] deleteCreditos, TiposMovimiento tiposMovimiento, DBContextAdapter dbContext)
        {
            CreditoMovimientos credMov = null;
            try
            {
                bool commit = true;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (deleteCreditos == null ? false : deleteCreditos.Any())
                {
                    commit = deleteListQueryCreditoMovimientos(deleteCreditos, dbContext);
                    //if (commit)
                    //{
                    //    clear();
                    //}
                }
                if (commit && entitysCambios.Any())
                {
                    for (int i = 0; i < entitysCambios.Count(); i++)
                    {
                        credMov = entitysCambios[i];
                        getSession().Set<CreditoMovimientos>().AddOrUpdate(entitysCambios[i]);
                        getSession().SaveChanges();
                    }
                }
                getSession().Database.CurrentTransaction.Commit();
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteCreditosMov()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;

        }
        private bool deleteListQueryCreditoMovimientos(object[] valores, DBContextAdapter dbContext)
        {
            bool exito = true;
            try
            {
                //Object[] valores = new Object[deleteCreditos.Count()];
                //for (int i = 0; i < deleteCreditos.Count(); i++)
                //{
                //    valores[i] = deleteCreditos[i].id;
                //}
                //deleteListQuery("CreditoMovimientos", "Id", valores);
                deleteListQuery("CreditoMovimientos", new CamposWhere("CreditoMovimientos.id", valores, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("deleteListQueryCreditoMovimientos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                exito = false;
            }
            return exito;
        }

    }
}