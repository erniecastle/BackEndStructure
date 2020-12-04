/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Clase ConceptoDeNominaDefinicionDAO para llamados a metodos de Entity
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
using System.Data.Entity;
using Exitosw.Payroll.Core.genericos.campos;
using Exitosw.Payroll.Core.campos;

namespace Exitosw.Payroll.Core.modelo
{

    public class ConcepNomDefiDAO : GenericRepository<ConcepNomDefi>, ConcepNomDefiDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        List<ConcepNomDefi> listconcepNomDefi = new List<ConcepNomDefi>();

        public Mensaje agregar(ConcepNomDefi entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<ConcepNomDefi>().Add(entity);
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

        public Mensaje modificar(ConcepNomDefi entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<ConcepNomDefi>().AddOrUpdate(entity);
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

        public Mensaje eliminar(ConcepNomDefi entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<ConcepNomDefi>().Attach(entity);
                getSession().Set<ConcepNomDefi>().Remove(entity);
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

        public Mensaje guardarConceptosNomina(ConcepNomDefi entity,
            List<ParaConcepDeNom> agregaModparametrosCn, object[] eliminadosParametros,
            List<ConceptoPorTipoCorrida> agregaModTiposCorrida, object[] eliminadosTiposCorrida,
            List<BaseAfecConcepNom> agregaModBasesAfecta, object[] eliminadosBasesAfecta,
            List<BaseAfecConcepNom> agregaModOtrasBasesAfecta, object[] eliminadosOtrasBasesAfecta,
            DBContextAdapter dbContext)
        {
            DbContext dbContextSimple;
            DbContextTransaction transacion;
            using (dbContextSimple = dbContext.context)
            {
                using (transacion = dbContextSimple.Database.BeginTransaction())
                {
                    try
                    {
                        ConcepNomDefi exitsCnc = null;
                        if (entity.id == 0)
                        {
                            //Exits date with this cnc
                            exitsCnc = (from cnDate in dbContextSimple.Set<ConcepNomDefi>()
                                        where cnDate.clave == entity.clave &&
                                        cnDate.fecha == entity.fecha
                                        select cnDate).SingleOrDefault();
                        }

                        if (exitsCnc == null)
                        {
                            //Validate superior date
                            var getFirstVersionCnc = (from cSup in dbContextSimple.Set<ConcepNomDefi>()
                                                      where cSup.clave == entity.clave
                                                      orderby cSup.clave descending
                                                      select cSup).FirstOrDefault();
                            bool isValid = false;
                            if (getFirstVersionCnc == null)
                            {
                                isValid = true;
                            }
                            else
                            {
                                var firstVersion = getFirstVersionCnc.fecha.Value.Date;
                                if (entity.fecha.Value.Date >= firstVersion)
                                {
                                    isValid = true;
                                }
                            }

                            if (isValid)
                            {
                                inicializaVariableMensaje();
                                dbContextSimple.Set<ConcepNomDefi>().AddOrUpdate(entity);
                                dbContextSimple.SaveChanges();
                                int i;

                                //Parametros
                                for (i = 0; i < agregaModparametrosCn.Count(); i++)
                                {
                                    agregaModparametrosCn[i].concepNomDefi_ID = entity.id;
                                    dbContextSimple.Set<ParaConcepDeNom>().AddOrUpdate(agregaModparametrosCn[i]);
                                }
                                dbContextSimple.SaveChanges();

                                foreach (object addPar in eliminadosParametros)
                                {
                                    decimal idGet = Convert.ToDecimal(addPar);
                                    var getObj = new ParaConcepDeNom { id = idGet };
                                    dbContextSimple.Set<ParaConcepDeNom>().Attach(getObj);
                                    dbContextSimple.Entry(getObj).State = EntityState.Deleted;
                                }
                                dbContextSimple.SaveChanges();

                                //Tipos de corrida
                                for (i = 0; i < agregaModTiposCorrida.Count(); i++)
                                {
                                    agregaModTiposCorrida[i].concepNomDefi_ID = entity.id;
                                    dbContextSimple.Set<ConceptoPorTipoCorrida>().AddOrUpdate(agregaModTiposCorrida[i]);
                                }
                                dbContextSimple.SaveChanges();

                                foreach (object addTipCorr in eliminadosTiposCorrida)
                                {
                                    decimal idGet = Convert.ToDecimal(addTipCorr);
                                    var getObj = new ConceptoPorTipoCorrida { id = idGet };
                                    dbContextSimple.Set<ConceptoPorTipoCorrida>().Attach(getObj);
                                    dbContextSimple.Entry(getObj).State = EntityState.Deleted;
                                }
                                dbContextSimple.SaveChanges();

                                //Bases afectadas
                                for (i = 0; i < agregaModBasesAfecta.Count(); i++)
                                {
                                    agregaModBasesAfecta[i].concepNomDefin_ID = entity.id;
                                    dbContextSimple.Set<BaseAfecConcepNom>().AddOrUpdate(agregaModBasesAfecta[i]);
                                }
                                dbContextSimple.SaveChanges();

                                foreach (object addTipBasesAfec in eliminadosBasesAfecta)
                                {
                                    decimal idGet = Convert.ToDecimal(addTipBasesAfec);
                                    var getObj = new BaseAfecConcepNom { id = idGet };
                                    dbContextSimple.Set<BaseAfecConcepNom>().Attach(getObj);
                                    dbContextSimple.Entry(getObj).State = EntityState.Deleted;
                                }
                                dbContextSimple.SaveChanges();

                                //Otras Bases afectadas
                                for (i = 0; i < agregaModOtrasBasesAfecta.Count(); i++)
                                {
                                    agregaModOtrasBasesAfecta[i].concepNomDefin_ID = entity.id;
                                    dbContextSimple.Set<BaseAfecConcepNom>().AddOrUpdate(agregaModOtrasBasesAfecta[i]);
                                }
                                dbContextSimple.SaveChanges();

                                foreach (object addTipOtrasBasesAfec in eliminadosOtrasBasesAfecta)
                                {
                                    decimal idGet = Convert.ToDecimal(addTipOtrasBasesAfec);
                                    var getObj = new BaseAfecConcepNom { id = idGet };
                                    dbContextSimple.Set<BaseAfecConcepNom>().Attach(getObj);
                                    dbContextSimple.Entry(getObj).State = EntityState.Deleted;
                                }
                                dbContextSimple.SaveChanges();

                                if (mensajeResultado.noError == 0)
                                {
                                    mensajeResultado.resultado = true;
                                    mensajeResultado.noError = 0;
                                }
                                transacion.Commit();

                            }
                            else
                            {
                                mensajeResultado.resultado = -2;//Date exits
                                mensajeResultado.noError = 0;
                            }

                        }
                        else
                        {
                            if (mensajeResultado.noError == 0)
                            {
                                mensajeResultado.resultado = -1;//Date exits
                                mensajeResultado.noError = 0;
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("guardarConceptosNomina()1_Error: ").Append(ex));
                        mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                        mensajeResultado.error = ex.GetBaseException().ToString();
                        mensajeResultado.resultado = null;
                        transacion.Rollback();
                    }
                }
            }

            return mensajeResultado;
        }

        public Mensaje eliminarConceptosNomina(ConcepNomDefi entity, DBContextAdapter dbContext)
        {
            DbContext dbContextSimple;
            DbContextTransaction transacion;
            using (dbContextSimple = dbContext.context)
            {
                using (transacion = dbContextSimple.Database.BeginTransaction())
                {
                    try
                    {
                        inicializaVariableMensaje();
                        //Delete Parametros Cnc 
                        dbContextSimple.Set<ParaConcepDeNom>().
                            RemoveRange(dbContextSimple.Set<ParaConcepDeNom>().Where(x => x.concepNomDefi_ID == entity.id));
                        dbContextSimple.SaveChanges();

                        //Delete Parametros Cnc 
                        dbContextSimple.Set<ConceptoPorTipoCorrida>().
                            RemoveRange(dbContextSimple.Set<ConceptoPorTipoCorrida>().Where(x => x.concepNomDefi_ID == entity.id));
                        dbContextSimple.SaveChanges();

                        //Delete Bases Afectas y Otras Bases
                        dbContextSimple.Set<BaseAfecConcepNom>().
                            RemoveRange(dbContextSimple.Set<BaseAfecConcepNom>().Where(x => x.concepNomDefin_ID == entity.id));
                        dbContextSimple.SaveChanges();

                        //Select conceptos nomina
                        var countCnc = (from cncDef in dbContext.context.Set<ConcepNomDefi>()
                                        where cncDef.clave == entity.clave
                                        select cncDef).Count();

                        var getCncNom = entity.conceptoDeNomina_ID;
                        //Delete Conceptos nomina definicion
                        dbContextSimple.Set<ConcepNomDefi>().Attach(entity);
                        dbContextSimple.Set<ConcepNomDefi>().Remove(entity);
                        dbContextSimple.SaveChanges();

                        if (countCnc == 1)
                        {
                            //Delete conceptos nomina
                            dbContextSimple.Set<ConceptoDeNomina>().
                                RemoveRange(dbContextSimple.Set<ConceptoDeNomina>().Where(x => x.id == getCncNom));
                            dbContextSimple.SaveChanges();
                        }


                        if (mensajeResultado.noError == 0)
                        {
                            mensajeResultado.resultado = true;
                            mensajeResultado.noError = 0;
                        }
                        transacion.Commit();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("eliminarEmpleado()1_Error: ").Append(ex));
                        mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                        mensajeResultado.error = ex.GetBaseException().ToString();
                        mensajeResultado.resultado = null;
                        transacion.Rollback();
                    }
                }
            }

            return mensajeResultado;
        }

        public Mensaje getPorIdConceptosYComplementos(decimal? idConcep, String claveCnc, DateTime? fecha, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                object[] complementos = new object[7];

                var queryConcepto = (from c in dbContext.context.Set<ConcepNomDefi>() select c);

                if (idConcep != null)
                {
                    queryConcepto = from subq in queryConcepto where subq.id == idConcep select subq;
                }

                if (claveCnc != null)
                {
                    queryConcepto = from subq in queryConcepto where subq.clave == claveCnc select subq;
                }

                if (fecha != null)
                {
                    queryConcepto = from subq in queryConcepto where subq.fecha == fecha select subq;

                }

                var getConcepto = queryConcepto.Select(c => new
                {
                    id = c.id,
                    clave = c.clave,
                    fecha = c.fecha,
                    descripcion = c.descripcion,
                    descripcionAbreviada = c.descripcionAbreviada,
                    naturaleza = c.naturaleza,
                    tipo = c.tipo,
                    formulaConcepto = c.formulaConcepto,
                    condicionConcepto = c.condicionConcepto,
                    mascara = c.mascara,
                    activarPlaza = c.activarPlaza,
                    activarDesglose = c.activarDesglose,
                    activado = c.activado,
                    imprimirEnListadoNomina = c.imprimirEnListadoNomina,
                    imprimirEnReciboNomina = c.imprimirEnReciboNomina,
                    tipoAccionMascaras = c.tipoAccionMascaras,
                    subCuenta = c.subCuenta,
                    comportamiento = c.comportamiento,
                    conceptoDeNomina = new
                    {
                        c.conceptoDeNomina.id,
                        c.conceptoDeNomina.clave,
                    }
                }).SingleOrDefault();

                complementos[0] = getConcepto;

                if (getConcepto != null)
                {
                    var getParametros = (from p in dbContext.context.Set<ParaConcepDeNom>()
                                         where p.concepNomDefi_ID == getConcepto.id
                                         select new
                                         {
                                             id = p.id,
                                             clasificadorParametro =
                                             new
                                             {
                                                 id = p.clasificadorParametro,
                                                 clave = p.clasificadorParametro == 0 ? "Entrada" : "Especial",
                                                 descripcion = p.clasificadorParametro == 0 ? "Entrada" : "Especial",
                                             },
                                             descripcion = p.descripcion,
                                             mascara = p.mascara,
                                             numero = p.numero,
                                             tipo =
                                             new
                                             {
                                                 id = p.tipo,
                                                 clave = p.tipo == "INTEGER" ? "Numérico" : "Alfanumérico",
                                                 descripcion = p.tipo == "INTEGER" ? "Numérico" : "Alfanumérico",
                                             },
                                             unidad =
                                             new
                                             {
                                                 id = p.unidad,
                                                 clave = p.unidad,
                                                 descripcion = p.unidad == "HORAS" ? "Horas" :
                                                 p.unidad == "DIAS" ? "Días" : p.unidad == "PIEZAS" ? "Piezas" :
                                                 p.unidad == "IMPORTE" ? "Importe" : p.unidad == "OTROS" ? "Otros" : "",
                                             },
                                             concepNomDefi_ID = p.concepNomDefi_ID
                                         }).ToList();

                    complementos[1] = getParametros;

                    var getConceptosCorrida = (from cc in dbContext.context.Set<ConceptoPorTipoCorrida>()
                                               where cc.concepNomDefi_ID == getConcepto.id
                                               select new
                                               {
                                                   id = cc.id,
                                                   cantidad = cc.cantidad,
                                                   descuentoCreditos = cc.descuentoCreditos,
                                                   incluir = cc.incluir,
                                                   modificarCantidad = cc.modificarCantidad,
                                                   modificarImporte = cc.modificarImporte,
                                                   mostrar = cc.opcional,
                                                   opcional = cc.opcional,
                                                   concepNomDefi_ID = cc.concepNomDefi_ID,
                                                   tipoCorrida_ID = cc.tipoCorrida_ID,
                                                   tipoCorrida =
                                             new
                                             {
                                                 id = cc.tipoCorrida_ID,
                                                 clave = cc.tipoCorrida.descripcion,
                                                 descripcion = cc.tipoCorrida.clave,

                                             },
                                                   nombreTipocorrida = cc.tipoCorrida.descripcion
                                               }).ToList();
                    complementos[2] = getConceptosCorrida;

                    var getBasesAfecta = (from ba in dbContext.context.Set<BaseAfecConcepNom>()
                                          where ba.concepNomDefin_ID == getConcepto.id
                                          && ba.baseNomina.reservado == true
                                          select new
                                          {
                                              id = ba.id,
                                              formulaExenta = ba.formulaExenta,
                                              periodoExentoISR = ba.periodoExentoISR,
                                              tipoAfecta = ba.tipoAfecta,
                                              baseNomina_ID = ba.baseNomina_ID,
                                              concepNomDefin_ID = ba.concepNomDefin_ID
                                          }).ToList();

                    complementos[3] = getBasesAfecta;

                    var getOtrasBasesAfecta = (from ba in dbContext.context.Set<BaseAfecConcepNom>()
                                               where ba.concepNomDefin_ID == getConcepto.id
                                               && ba.baseNomina.reservado == false
                                               select new
                                               {
                                                   id = ba.id,
                                                   otrasBases =
                                                  new
                                                  {
                                                      id = ba.id,
                                                      clave = ba.baseNomina.clave,
                                                      descripcion = ba.baseNomina.descripcion
                                                  },
                                                   formulaExenta = ba.formulaExenta,
                                                   periodoExentoISR = ba.periodoExentoISR,
                                                   tipoAfecta = ba.tipoAfecta,
                                                   baseNomina_ID = ba.baseNomina_ID,
                                                   concepNomDefin_ID = ba.concepNomDefin_ID
                                               }).ToList();

                    complementos[4] = getOtrasBasesAfecta;
                }

                mensajeResultado.resultado = getConcepto == null ? null : complementos;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorIdConceptosYComplementos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getVersionesConceptos(string claveConcep, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var getVersionesCnc = (from c in dbContext.context.Set<ConcepNomDefi>()
                                       where c.clave == claveConcep
                                       select new
                                       {
                                           id = c.id,
                                           clave = c.clave,
                                           fecha = c.fecha
                                       }).ToList();
                mensajeResultado.resultado = getVersionesCnc;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getVersionesConceptos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;

        }

        public Mensaje getLastVersionConcepto(string claveConcep, DBContextAdapter dbContext)
        {
            {
                DbContext dbContextSimple;
                DbContextTransaction transacion;
                using (dbContextSimple = dbContext.context)
                {
                    using (transacion = dbContextSimple.Database.BeginTransaction())
                    {
                        try
                        {
                            inicializaVariableMensaje();
                            var getLastVersionCnc = dbContextSimple.Set<ConcepNomDefi>().Where(c => c.clave == claveConcep)
                           .OrderByDescending(x => x.fecha).FirstOrDefault();

                            if (getLastVersionCnc != null)
                            {
                                mensajeResultado.resultado = getLastVersionCnc.id;
                                mensajeResultado.noError = 0;
                                mensajeResultado.error = "";
                            }

                            transacion.Commit();
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getLastVersionConcepto()1_Error: ").Append(ex));
                            mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                            mensajeResultado.error = ex.GetBaseException().ToString();
                            mensajeResultado.resultado = null;
                            transacion.Rollback();
                        }
                    }
                }

                return mensajeResultado;
            }
        }
        public Mensaje agregaConceptoNominaBaseAfectadas(ConcepNomDefi entity, List<BaseAfecConcepNom> eliminadasAfectadaConceptoNominas, DBContextAdapter dbContext)
        {
            bool exitoRegistro = true;

            inicializaVariableMensaje();
            setSession(dbContext.context);
            object valor;
            BaseAfecConcepNom afectadaConceptoNomina;
            try
            {
                getSession().Database.BeginTransaction();
                int i = 0;
                List<BaseAfecConcepNom> noEliminadasBasesConceptoNominas = new List<BaseAfecConcepNom>();
                if (eliminadasAfectadaConceptoNominas != null)
                {
                    noEliminadasBasesConceptoNominas = new List<BaseAfecConcepNom>();
                    for (i = 0; i < eliminadasAfectadaConceptoNominas.Count(); i++)
                    {
                        /* if (existeDato(typeof(MovNomBaseAfecta).Name, "baseAfecConcepNom", eliminadasAfectadaConceptoNominas[i]))
                         {
                             noEliminadasBasesConceptoNominas.Add(eliminadasAfectadaConceptoNominas[i]);
                         }else
                         {
                             afectadaConceptoNomina = eliminadasAfectadaConceptoNominas[i];
                             afectadaConceptoNomina.concepNomDefi=null;
                             getSession().Set<BaseAfecConcepNom>().Attach(afectadaConceptoNomina);
                             getSession().Set<BaseAfecConcepNom>().Remove(afectadaConceptoNomina);
                         }*/
                    }
                }
                getSession().Set<ConcepNomDefi>().AddOrUpdate(entity);
                getSession().SaveChanges();
                if (noEliminadasBasesConceptoNominas != null)
                {
                    entity.baseAfecConcepNom = noEliminadasBasesConceptoNominas;
                }
                if (exitoRegistro)
                {
                    valor = entity;
                    mensajeResultado.resultado = valor;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("agregaConceptoNominaBaseAfectadas()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje claveDescripcionConceptos(DBContextAdapter dbContext)
        {
            object valores = null;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                //valores = getSession().createQuery(CONSULTA_CONCEPTO_CON_NOMENCLATURA);
                mensajeResultado.resultado = valores;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("claveDescripcionConceptos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }


            throw new NotImplementedException();
        }

        public Mensaje consultaPorFiltrosConceptoDeNominaDefinicion(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            listconcepNomDefi = new List<ConcepNomDefi>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                List<CamposWhere> camposwheres = new List<CamposWhere>();
                foreach (var item in campos)
                {
                    if (!item.Value.ToString().Equals("") && item.Value != null)
                    {
                        CamposWhere campo = new CamposWhere();
                        campo.campo = "ConcepNomDefi." + item.Key.ToString();
                        campo.valor = item.Value;
                        if (operador == "=")
                        {
                            campo.operadorComparacion = OperadorComparacion.IGUAL;
                        }
                        else if (operador == "like")
                        {
                            campo.operadorComparacion = OperadorComparacion.LIKE;
                        }
                        campo.operadorLogico = OperadorLogico.AND;
                        camposwheres.Add(campo);
                    }


                }
                ValoresRango rangos = new ValoresRango(Convert.ToInt32(inicio), Convert.ToInt32(rango));
                mensajeResultado.resultado = consultaPorRangos(rangos, camposwheres, null);
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorFiltrosConceptos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje consultaPorRangosConceptoDeNominaDefinicion(Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            listconcepNomDefi = new List<ConcepNomDefi>();
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

        //public Mensaje existeDato(string campo, object valor, DbContext dbContext)
        //{
        //    bool existe = false;
        //    try
        //    {
        //        inicializaVariableMensaje();
        //        setSession(dbContext);
        //        getSession().Database.BeginTransaction();
        //        //existe = existeDato("ConcepNomDefi", campo, valor);
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

        public Mensaje getAllConcepNomDefi(DBContextAdapter dbContext)
        {
            //listconcepNomDefi = new List<ConcepNomDefi>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var listconcepNomDefi = (from a in getSession().Set<ConcepNomDefi>()
                                         select new
                                         {

                                             activado = a.activado,
                                             agregarSubcuentasPor = a.agregarSubcuentasPor,
                                             //  baseAfecConcepNom=a.baseAfecConcepNom,
                                             // camposDimConceptos=a.camposDimConceptos,
                                             clave = a.clave,
                                             comportamiento = a.comportamiento,
                                             // conceptoDeNomina=a.conceptoDeNomina,
                                             conceptoDeNomina_ID = a.conceptoDeNomina_ID,
                                             // conceptoPorTipoCorrida=a.conceptoPorTipoCorrida,
                                             condicionConcepto = a.condicionConcepto,
                                             // configConceptosSat=a.configConceptosSat,
                                             //  creditoAhorro_cNDescuento=a.creditoAhorro_cNDescuento,
                                             // creditoAhorro_cNDInteresMensual=a.creditoAhorro_cNDInteresMensual,
                                             //creditoAhorro_concepNomiDefin=a.creditoAhorro_concepNomiDefin,
                                             cuentaContable = a.cuentaContable,
                                             descripcion = a.descripcion,
                                             descripcionAbreviada = a.descripcionAbreviada,
                                             //excepciones=a.excepciones,
                                             fecha = a.fecha,
                                             formulaConcepto = a.formulaConcepto,
                                             // grupo=a.grupo,
                                             grupo_ID = a.grupo_ID,
                                             id = a.id,
                                             imprimirEnListadoNomina = a.imprimirEnListadoNomina,
                                             imprimirEnReciboNomina = a.imprimirEnReciboNomina,
                                             mascara = a.mascara,
                                             // movNomConcep=a.movNomConcep,
                                             naturaleza = a.naturaleza,
                                             //  paraConcepDeNom=a.paraConcepDeNom,
                                             prioridadDeCalculo = a.prioridadDeCalculo,
                                             //  res=a.resultado,
                                             //result=a.resultado,
                                             // salariosIntegradosDet=a.salariosIntegradosDet,
                                             subCuenta = a.subCuenta,
                                             tipo = a.tipo,
                                             tipoAccionMascaras = a.tipoAccionMascaras,
                                             tipoMovto = a.tipoMovto,


                                         }).ToList();
                mensajeResultado.resultado = listconcepNomDefi;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ConceptoDeNominaDefinicionAll_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getConceptoDeNominaDefinicionConCuentaContable(DBContextAdapter dbContext)
        {
            listconcepNomDefi = new List<ConcepNomDefi>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listconcepNomDefi = (from a in getSession().Set<ConcepNomDefi>()
                                     where a.cuentaContable != null
                                     select a).ToList();
                mensajeResultado.resultado = listconcepNomDefi;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ConceptoDeNominaDefinicionConCuentaContable_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getConceptoDeNominaDefinicionPorClave(string clave, DBContextAdapter dbContext)
        {
            ConcepNomDefi concepNomDefi = new ConcepNomDefi();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                concepNomDefi = (from c in getSession().Set<ConcepNomDefi>()
                                 where c.clave.Equals(clave) &&
                                  c.activado == true
                                 select c).SingleOrDefault();
                mensajeResultado.resultado = concepNomDefi;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getConceptoDeNominaDefinicionPorClave_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getConceptoDeNominaDefinicionPorClaves(object[] claves, DBContextAdapter dbContext)
        {
            listconcepNomDefi = new List<ConcepNomDefi>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listconcepNomDefi = (from o in getSession().Set<ConcepNomDefi>()
                                     where claves.Contains(o.conceptoDeNomina.clave) &&
                                     o.fecha ==
                                     (from c in getSession().Set<ConcepNomDefi>()
                                      where c.clave.Equals(o.clave)
                                      select new { c.fecha }).Max(p => p.fecha) &&
                                      o.activado == true
                                     select o).ToList();
                mensajeResultado.resultado = listconcepNomDefi;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ConceptoDeNominaDefinicionPorTipoCorrida_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorTipoCorridaConcepNomDefi(string claveTipoCorrida, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var listconcepNomDefi = (from o in getSession().Set<ConceptoPorTipoCorrida>()
                                         join ct in getSession().Set<ConcepNomDefi>() on o.concepNomDefi.id equals ct.id
                                         join t in getSession().Set<TipoCorrida>() on o.tipoCorrida.id equals t.id
                                         where o.tipoCorrida.clave == claveTipoCorrida && ct.activado == true
                                         select o).ToList();

                var lis = listconcepNomDefi.Select(x => new
                {
                    //id = x.id,
                    conceptoPorTipoCorrida_ID = x.id,
                    clave = x.concepNomDefi.clave,
                    descripcion = x.concepNomDefi.descripcion,
                    descripcionAbreviada = x.concepNomDefi.descripcionAbreviada,
                    importe = 0,
                    cantidad = 0
                }).ToList();
                mensajeResultado.resultado = lis;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ConceptoDeNominaDefinicionPorTipoCorrida_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorTipoCorridaIDConcepNomDefi(decimal? claveTipoCorrida, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var listconcepNomDefi = (from o in getSession().Set<ConceptoPorTipoCorrida>()
                                         join ct in getSession().Set<ConcepNomDefi>() on o.concepNomDefi.id equals ct.id
                                         join t in getSession().Set<TipoCorrida>() on o.tipoCorrida.id equals t.id
                                         where o.tipoCorrida.id == claveTipoCorrida &&
                                          /* ct.fecha ==
                                           (from c in getSession().Set<ConcepNomDefi>()
                                            where ct.id==c.id
                                            select new { c.fecha }).Max(p => p.fecha) &&*/
                                          ct.activado == true && ct.tipo != 0
                                         select new
                                         {
                                             ct.activado,
                                             ct.activarDesglose,
                                             ct.activarPlaza,
                                             ct.agregarSubcuentasPor,
                                             baseAfecConcepNom = ct.baseAfecConcepNom.Select(b => new
                                             {
                                                 b.baseNomina_ID,
                                                 b.concepNomDefin_ID,
                                                 b.formulaExenta,
                                                 b.id,
                                                 b.periodoExentoISR,
                                                 b.tipoAfecta
                                             }).ToList(),
                                             ct.categoriaPuestos_ID,
                                             ct.clave,
                                             ct.comportamiento,
                                             ct.conceptoDeNomina_ID,
                                             ct.condicionConcepto,
                                             ct.cuentaContable,
                                             ct.descripcion,
                                             ct.descripcionAbreviada,
                                             ct.fecha,
                                             ct.formulaConcepto,
                                             ct.grupo_ID,
                                             ct.id,
                                             ct.imprimirEnListadoNomina,
                                             ct.imprimirEnReciboNomina,
                                             ct.mascara,
                                             ct.naturaleza,
                                             paraConcepDeNom = ct.paraConcepDeNom.Select(p => new
                                             {
                                                 p.clasificadorParametro,
                                                 p.descripcion,
                                                 p.id,
                                                 p.mascara,
                                                 p.numero,
                                                 p.tipo,
                                                 p.unidad
                                             }).ToList(),
                                             ct.subCuenta,
                                             ct.tipo,
                                             ct.tipoAccionMascaras,
                                             ct.tipoMovto
                                         }).ToList();

                mensajeResultado.resultado = listconcepNomDefi;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorTipoCorridaIDConcepNomDefi_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getConceptoDeNominaDefinicionPorClaveID(decimal? clave, decimal? claveTipoCorrida, DBContextAdapter dbContext)
        {
            //  var concepNomDefi;// = new ConcepNomDefi();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var concepNom = (from c in getSession().Set<ConceptoPorTipoCorrida>()
                                 where c.concepNomDefi.id == clave &&
                                  /* c.concepNomDefi.fecha ==
                                   (from cnd in getSession().Set<ConcepNomDefi>()
                                    where c.concepNomDefi.clave == cnd.clave
                                    select new { cnd.fecha }).Max(p => p.fecha) &&*/
                                  c.concepNomDefi.activado == true &&
                                  c.tipoCorrida.id == claveTipoCorrida
                                 select c.concepNomDefi);

                var param = concepNom.Select(p => new
                {
                    p.activado,
                    p.activarDesglose,
                    p.activarPlaza,
                    p.agregarSubcuentasPor,
                    p.clave,
                    p.condicionConcepto,
                    p.cuentaContable,
                    p.descripcion,
                    p.descripcionAbreviada,
                    p.fecha,
                    p.formulaConcepto,
                    p.id,
                    p.imprimirEnListadoNomina,
                    p.imprimirEnReciboNomina,
                    p.mascara,
                    p.naturaleza,
                    paraConcepDeNom = p.paraConcepDeNom.Select(pa => new
                    {
                        pa.clasificadorParametro,

                        pa.descripcion,
                        pa.id,
                        pa.mascara,
                        // pa.movNomConceParam,
                        pa.numero,
                        pa.tipo,
                        pa.unidad
                    }).ToList()


                }).SingleOrDefault();
                mensajeResultado.resultado = param;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getConceptoDeNominaDefinicionPorClaveID_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
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

        public Mensaje getConceptoDeNominaDefinicionPorID(decimal? idConcep, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var concepNom = (from c in getSession().Set<ConcepNomDefi>()
                                 where
                                  c.activado == true &&
                                  c.id == idConcep
                                 select new
                                 {
                                     c.clave,
                                     c.descripcion,
                                     c.descripcionAbreviada,
                                     c.id
                                 }).SingleOrDefault();


                mensajeResultado.resultado = concepNom;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getConceptoDeNominaDefinicionPorID_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
    }
}