/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Clase CreditoAhorroDAO para llamados a metodos de Entity
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
using System.Data.Entity.Validation;

namespace Exitosw.Payroll.Core.modelo
{
    public class CreditoAhorroDAO : GenericRepository<CreditoAhorro>, CreditoAhorroDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");

        public Mensaje agregar(CreditoAhorro entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<CreditoAhorro>().Add(entity);
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

        public Mensaje actualizar(CreditoAhorro entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<CreditoAhorro>().AddOrUpdate(entity);
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

        public Mensaje eliminar(CreditoAhorro entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<CreditoAhorro>().Attach(entity);
                getSession().Set<CreditoAhorro>().Remove(entity);
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

        public Mensaje getAllCreditoAhorro(string claveRazonesSociales, string tipoConfiguracion, DBContextAdapter dbContext)
        {
            List<CreditoAhorro> listaCreditoAll = new List<CreditoAhorro>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listaCreditoAll = (from ca in getSession().Set<CreditoAhorro>()
                                   where ca.razonesSociales.clave.Equals(claveRazonesSociales) &&
                                   ca.tipoConfiguracion.Equals(tipoConfiguracion)
                                   select ca).ToList();
                mensajeResultado.resultado = listaCreditoAll;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("CreditoAhorroAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorClaveCreditoAhorro(string clave, string claveRazonesSociales, string tipoConfiguracion, DBContextAdapter dbContext)
        {
            CreditoAhorro creditoXclave = new CreditoAhorro();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                creditoXclave = (from ca in getSession().Set<CreditoAhorro>()
                                 where ca.clave.Equals(clave) &&
                                 ca.razonesSociales.clave.Equals(claveRazonesSociales)
                                 select ca).SingleOrDefault();
                mensajeResultado.resultado = creditoXclave;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("CreditoAhorroPorClave()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorIDCreditoAhorro(decimal idCreditoAhorro, DBContextAdapter dbContext)
        {
            //  CreditoAhorro creditoXclave = new CreditoAhorro();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var creditoXclave = (from ca in getSession().Set<CreditoAhorro>()
                                     where ca.id == idCreditoAhorro
                                     select new
                                     {
                                         ca.activarManejoDescuento,
                                         ca.asignaAutoNumCredAho,
                                         ca.clave,
                                         cNDescuento = ca.cNDescuento == null ? null : new { ca.cNDescuento.clave },
                                         ca.cNDescuento_ID,
                                         cNDInteresMensual = ca.cNDInteresMensual ==null ? null : new {ca.cNDInteresMensual.clave },
                                         ca.cNDInteresMensual_ID,
                                         concepNomiDefin = ca.concepNomiDefin == null ? null : new { ca.concepNomiDefin.clave},
                                         ca.concepNomiDefin_ID,
                                         ca.conceptoDcto,
                                         ca.corteMesDia,
                                         ca.cuandoDescontar,
                                         ca.cuotaFija,
                                         ca.definirNumEmp,
                                         ca.descPropDiasPer,
                                         ca.descripcion,
                                         ca.descripcionAbrev,
                                         ca.factorProporcional,
                                         ca.fondoAhorro,
                                         ca.id,
                                         ca.importeDescuento,
                                         ca.inicioDescuento,
                                         ca.longitudNumCredAho,
                                         ca.mascaraNumCredAho,
                                         ca.modoAgregarCredAhoIngEmp,
                                         ca.modoCapturaDescuento,
                                         ca.modoCapturaDescuentoPorc,
                                         ca.modoCapturaDescuentoVSMG,
                                         ca.modoDescontarCredAhoFini,
                                         ca.modoDescontarCredAhoLiqu,
                                         ca.modoDescuento,
                                         ca.modoManejoDescuento,
                                         ca.numDecimalesDescuento,
                                         ca.numDecimalesDescuentoPorc,
                                         ca.numDecimalesDescuentoVSMG,
                                         ca.periodicidadDescuento,
                                         ca.porcentaje,
                                         ca.razonesSociales_ID,
                                         ca.solicitarFecVen,
                                         ca.tasaIntMens,
                                         ca.tipoConfiguracion,
                                         ca.vsmg,
                                         ca.longitudNumEmp,
                                         ca.capturarCreditoTotal,
                                         ca.versionCalculoPrestamoAhorro,
                                         ca.valorVSMG

                                     }).SingleOrDefault();
                mensajeResultado.resultado = creditoXclave;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorIDCreditoAhorro()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getAllTipoCreditoAhorro(string claveRazonesSociales, string tipoConfiguracion, DBContextAdapter dbContext)
        {
         ///   List<CreditoAhorro> listaCreditoAll = new List<CreditoAhorro>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
              var  listaCreditoAll = (from ca in getSession().Set<CreditoAhorro>()
                                   where ca.razonesSociales.clave.Equals(claveRazonesSociales) &&
                                   ca.tipoConfiguracion.Equals(tipoConfiguracion)
                                   select new {
                                       ca.activarManejoDescuento,
                                       ca.asignaAutoNumCredAho,
                                       ca.clave,
                                       cNDescuento = ca.cNDescuento == null ? null : new { ca.cNDescuento.clave },
                                       ca.cNDescuento_ID,
                                       cNDInteresMensual = ca.cNDInteresMensual == null ? null : new { ca.cNDInteresMensual.clave },
                                       ca.cNDInteresMensual_ID,
                                       concepNomiDefin = ca.concepNomiDefin == null ? null : new { ca.concepNomiDefin.clave },
                                       ca.concepNomiDefin_ID,
                                       ca.conceptoDcto,
                                       ca.corteMesDia,
                                       ca.cuandoDescontar,
                                       ca.cuotaFija,
                                       ca.definirNumEmp,
                                       ca.descPropDiasPer,
                                       ca.descripcion,
                                       ca.descripcionAbrev,
                                       ca.factorProporcional,
                                       ca.fondoAhorro,
                                       ca.id,
                                       ca.importeDescuento,
                                       ca.inicioDescuento,
                                       ca.longitudNumCredAho,
                                       ca.mascaraNumCredAho,
                                       ca.modoAgregarCredAhoIngEmp,
                                       ca.modoCapturaDescuento,
                                       ca.modoCapturaDescuentoPorc,
                                       ca.modoCapturaDescuentoVSMG,
                                       ca.modoDescontarCredAhoFini,
                                       ca.modoDescontarCredAhoLiqu,
                                       ca.modoDescuento,
                                       ca.modoManejoDescuento,
                                       ca.numDecimalesDescuento,
                                       ca.numDecimalesDescuentoPorc,
                                       ca.numDecimalesDescuentoVSMG,
                                       ca.periodicidadDescuento,
                                       ca.porcentaje,
                                       ca.razonesSociales_ID,
                                       ca.solicitarFecVen,
                                       ca.tasaIntMens,
                                       ca.tipoConfiguracion,
                                       ca.vsmg,
                                       ca.longitudNumEmp,
                                       ca.capturarCreditoTotal,
                                       ca.versionCalculoPrestamoAhorro,
                                       ca.valorVSMG
                                   }).ToList();
                mensajeResultado.resultado = listaCreditoAll;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("CreditoAhorroAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje saveCreditoAhorroContenedor(CreditoAhorro entity, Contenedor contenedor, DBContextAdapter dbContextSimple, DBContextAdapter dbContextMaster)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextSimple.context);
                getSession().Database.BeginTransaction();

                if (entity.id == 0)
                {
                    getSession().Set<CreditoAhorro>().Add(entity);
                }
                else {
                    getSession().Set<CreditoAhorro>().AddOrUpdate(entity);
                }
                //getSession().Set<CreditoAhorro>().Add(entity);
                getSession().SaveChanges();
                dbContextMaster.context.Database.BeginTransaction();
                if (contenedor != null) {
                    if (entity.tipoConfiguracion.Equals("1"))
                    {
                        contenedor.accion = "{" + '"' + "tipoCaptura" + '"' + ":" + '"' + "1010" + '"' + "," + '"' + "IdScreen" + '"' + ":" + '"' + "registrosCreditos" + '"' + "," + '"' + "size" + '"' + ":" + '"' + "8" + '"' + "," + '"' + "valorIni" + '"' + ":" + '"'+ entity.id.ToString() + '"' +"}";
                    }
                    else {
                        contenedor.accion = "{" + '"' + "tipoCaptura" + '"' + ":" + '"' + "1010" + '"' + "," + '"' + "IdScreen" + '"' + ":" + '"' + "registroAhorros" + '"' + "," + '"' + "size" + '"' + ":" + '"' + "8" + '"' + "," + '"' + "valorIni" + '"' + ":" + '"' + entity.id.ToString() + '"' + "}";
                    }
                        
                    contenedor.controlPorForma = "CreditoAhorro" + entity.id;
                    dbContextMaster.context.Set<Contenedor>().AddOrUpdate(contenedor);
                }
                dbContextMaster.context.SaveChanges();
                dbContextMaster.context.Database.CurrentTransaction.Commit();

                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveCreditoAhorroContenedor()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje DeleteCreditoAhorroContenedor(CreditoAhorro entity, DBContextAdapter dbContextSimple, DBContextAdapter dbContextMaster)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextSimple.context);
                getSession().Database.BeginTransaction();
                string claveControl = "CreditoAhorro" + entity.id;
                Contenedor con = (from c in dbContextMaster.context.Set<Contenedor>()
                                  where c.controlPorForma == claveControl
                                  select c).SingleOrDefault();
                if (con != null) {
                    dbContextMaster.context.Database.BeginTransaction();
                    dbContextMaster.context.Set<Contenedor>().Attach(con);
                    dbContextMaster.context.Set<Contenedor>().Remove(con);
                    dbContextMaster.context.SaveChanges();
                    dbContextMaster.context.Database.CurrentTransaction.Commit();
                }
                getSession().Set<CreditoAhorro>().Attach(entity);
                getSession().Set<CreditoAhorro>().Remove(entity);
                getSession().SaveChanges();
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("DeleteCreditoAhorroContenedor()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
                dbContextMaster.context.Database.CurrentTransaction.Commit();
            }
            return mensajeResultado;
        }
    }
}