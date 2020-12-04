/**
* @author: Daniel Ruelas 
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Clase PeriodosNominaDAO para llamados a metodos de Entity
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
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using Exitosw.Payroll.Core.genericos.campos;
using System.Data.Entity;
using Exitosw.Payroll.Core.campos;

namespace Exitosw.Payroll.Core.modelo
{
    public class PeriodosNominaDAO : GenericRepository<PeriodosNomina>, PeriodosNominaDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        private bool commit;
        public Mensaje actualizaListaPorCampos(string[] campoModificar, object[] valoresModificado, string[] camposWhere, object[] valoresWhere, DBContextAdapter dbContext)
        {
            int resultado = 0;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();

                mensajeResultado.resultado = resultado;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("actualizaListaPorCampos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        public Mensaje agregar(PeriodosNomina entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<PeriodosNomina>().Add(entity);
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
        public Mensaje modificar(PeriodosNomina entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                //var genero = getSession().Set<Genero>().FirstOrDefault(g => g.id == entity.id);
                //genero.clave = entity.clave;
                //genero.descripcion = entity.descripcion;
                //genero.empleados = entity.empleados;
                getSession().Set<PeriodosNomina>().AddOrUpdate(entity);
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

        public Mensaje eliminar(PeriodosNomina entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                //var sentencia = (from g in getSession().Set<Genero>()
                //                 where g.id == entity.id
                //                 select g).FirstOrDefault();
                getSession().Set<PeriodosNomina>().Attach(entity);
                getSession().Set<PeriodosNomina>().Remove(entity);
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

        public Mensaje consultaPorFiltrosPeriodosNomina(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            List<PeriodosNomina> periodosNomina = new List<PeriodosNomina>();
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
                        campo.campo = "PeriodosNomina." + item.Key.ToString();
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorFiltrosPeriodos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje consultaPorRangosPeriodosNomina(Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            List<PeriodosNomina> periodosNomina = new List<PeriodosNomina>();
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

        public Mensaje getPeriodosNominaActualPorFecha(DateTime fecha, string claveTipoNomina, string claveTipoCorrida, bool status, DBContextAdapter dbContext)
        {
            PeriodosNomina periodosNomina;
            try
            {//dudas sobre estos valores en java q.setFirstResult(0);q.setMaxResults(1);

                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                periodosNomina = (from p in getSession().Set<PeriodosNomina>()
                                  join cor in getSession().Set<TipoCorrida>()
                                  on p.tipoCorrida.id equals cor.id
                                  where (p.fechaInicial >= fecha || p.fechaFinal >= fecha) &&
                                  p.tipoNomina.clave == claveTipoNomina &&
                                  p.status == status &&
                                  cor.clave == claveTipoCorrida
                                  select p).SingleOrDefault();
                mensajeResultado.resultado = periodosNomina;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPeriodosNominaActualPorFecha()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getAllPeriodosNomina(DBContextAdapter dbContext)
        {
            // List<PeriodosNomina> periodosNomina = new List<PeriodosNomina>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var periodosNomina = (from p in getSession().Set<PeriodosNomina>()
                                      select new
                                      {

                                          acumularAMes = p.acumularAMes,
                                          //aguinaldoPagos = p.aguinaldoPagos,
                                          //asistencias = p.asistencias,
                                          año = p.año,
                                          bloquear = p.bloquear,
                                          //calculoUnidades = p.calculoUnidades,
                                          //cfdiEmpleado = p.cfdiEmpleado,
                                          cierreMes = p.cierreMes,
                                          clave = p.clave,
                                          claveUsuario = p.claveUsuario,
                                          //creditoMovimientos = p.creditoMovimientos,
                                          //creditoPorEmpleado = p.creditoPorEmpleado,
                                          descontarAhorro = p.descontarAhorro,
                                          descontarPrestamo = p.descontarPrestamo,
                                          descripcion = p.descripcion,
                                          //detalleAsistencia = p.detalleAsistencia,
                                          detalleConceptoRecibo = p.detalleConceptoRecibo,
                                          diasIMSS = p.diasIMSS,
                                          diasPago = p.diasPago,
                                          fechaAsistenciaFinal = p.fechaAsistenciaFinal,
                                          fechaAsistenciInicial = p.fechaAsistenciInicial,
                                          fechaCierre = p.fechaCierre,
                                          fechaFinal = p.fechaFinal,
                                          fechaInicial = p.fechaInicial,
                                          fechaModificado = p.fechaModificado,
                                          fechaPago = p.fechaPago,
                                          id = p.id,
                                          // inasistenciaPorHora = p.inasistenciaPorHora,
                                          incluirBajas = p.incluirBajas,
                                          //listaperiodoPtuDias = p.listaperiodoPtuDias,
                                          //listaperiodoPtuPercep = p.listaperiodoPtuPercep,
                                          // movNomConcep = p.movNomConcep,
                                          origen = p.origen,
                                          //  semaforoCalculoNomina = p.semaforoCalculoNomina,
                                          //semaforoTimbradoPac = p.semaforoTimbradoPac,
                                          soloPercepciones = p.soloPercepciones,
                                          status = p.status,
                                          // tipoCorrida = p.tipoCorrida,
                                          tipoCorrida_ID = p.tipoCorrida_ID,
                                          // tipoNomina = p.tipoNomina,
                                          tipoNomina_ID = p.tipoNomina_ID,
                                          tipoUso = p.tipoUso
                                          //vacDisfr_perpriVaca = p.vacDisfr_perpriVaca,
                                          //vacDisf_perApli = p.vacDisf_perApli,
                                          //vacDisf_perVac = p.vacDisf_perVac


                                      }).ToList();
                mensajeResultado.resultado = periodosNomina;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPeriodosNominaAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPeriodosNominaPorAño(int year, DBContextAdapter dbContext)
        {

            try
            {
                int res;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                res = (from p in getSession().Set<PeriodosNomina>()
                       where p.año == year
                       select p).Count();
                if (res > 0)
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPeriodosNominaPorAño()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPeriodosNominaPorAñoYTipoCorrida(int year, string claveTipoCorrida, DBContextAdapter dbContext)
        {
            List<PeriodosNomina> periodosNomina = new List<PeriodosNomina>();
            try
            {

                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                periodosNomina = (from p in getSession().Set<PeriodosNomina>()
                                  where p.año == year &&
                                  p.tipoCorrida.clave == claveTipoCorrida
                                  orderby p.tipoNomina.clave
                                  select p).ToList();
                mensajeResultado.resultado = periodosNomina;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPeriodosNominaPorAñoYTipoCorrida()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPeriodosNominaPorAñoYTipoNominaYTipoCorrida(int año, string tipoNomina, string tipoCorrida, DBContextAdapter dbContext)
        {
            int? anio = año;

            //  List<PeriodosNomina> periodosNomina = new List<PeriodosNomina>();
            try
            {

                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var periodosNomina = (from p in getSession().Set<PeriodosNomina>()
                                      join tc in getSession().Set<TipoCorrida>() on p.tipoCorrida_ID equals tc.id
                                      join tn in getSession().Set<TipoNomina>() on p.tipoNomina_ID equals tn.id
                                      where p.año == año
                                      && tn.clave == tipoNomina &&
                                      tc.clave == tipoCorrida

                                      select new
                                      {
                                          acumularAMes = p.acumularAMes,
                                          año = p.año,
                                          bloquear = p.bloquear,
                                          cierreMes = p.cierreMes,
                                          clave = p.clave,
                                          claveUsuario = p.claveUsuario,
                                          descripcion = p.descripcion,
                                          detalleConceptoRecibo = p.detalleConceptoRecibo,
                                          diasIMSS = p.diasIMSS,
                                          diasPago = p.diasPago,
                                          fechaAsistenciaFinal = p.fechaAsistenciaFinal,
                                          fechaAsistenciInicial = p.fechaAsistenciInicial,
                                          fechaCierre = p.fechaCierre,
                                          fechaFinal = p.fechaFinal,
                                          fechaInicial = p.fechaInicial,
                                          fechaModificado = p.fechaModificado,
                                          fechaPago = p.fechaPago,
                                          id = p.id,
                                          incluirBajas = p.incluirBajas,
                                          origen = p.origen,
                                          soloPercepciones = p.soloPercepciones,
                                          status = p.status,
                                          tipoCorrida_ID = p.tipoCorrida_ID,
                                          tipoNomina_ID = p.tipoNomina_ID,
                                          tipoNomina = new { 
                                              Id=p.tipoNomina.id, 
                                              Clave=p.tipoNomina.clave, 
                                              Descripcion=p.tipoNomina.descripcion,
                                              Periodicidaddias = p.tipoNomina.periodicidad.dias,
                                              Periodicidaddescripcion =p.tipoNomina.periodicidad.descripcion },
                                          tipoUso = p.tipoUso,
                                      }).ToList();
                mensajeResultado.resultado = periodosNomina;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPeriodosNominaPorAñoYTipoNominaYTipoCorrida()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPeriodosNominaPorClave(string clave, int year, string claveTipoCorrida, DBContextAdapter dbContext)
        {
            //PeriodosNomina periodosNomina;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var periodosNomina = (from p in getSession().Set<PeriodosNomina>()
                                      join cor in getSession().Set<TipoCorrida>()
                                      on p.tipoCorrida.id equals cor.id
                                      where p.clave == clave && cor.clave == claveTipoCorrida && p.año == year
                                      select new
                                      {
                                          p.acumularAMes,
                                          p.año,
                                          p.bloquear,
                                          p.cierreMes,
                                          p.clave,
                                          p.claveUsuario,
                                          p.descontarAhorro,
                                          p.descontarPrestamo,
                                          p.descripcion,
                                          p.detalleConceptoRecibo,
                                          p.diasIMSS,
                                          p.diasPago,
                                          p.fechaAsistenciaFinal,
                                          p.fechaAsistenciInicial,
                                          p.fechaCierre,
                                          p.fechaFinal,
                                          p.fechaInicial,
                                          p.fechaModificado,
                                          p.fechaPago,
                                          p.id,
                                          p.incluirBajas,
                                          p.origen,
                                          p.soloPercepciones,
                                          p.status,
                                          p.tipoCorrida_ID,
                                          p.tipoNomina_ID,
                                          p.tipoUso

                                      }).SingleOrDefault();
                mensajeResultado.resultado = periodosNomina;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPeriodosNominaPorClave()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPeriodosNominaPorClaveYTipoDeNominaCorrida(string clave, string claveTipoNomina, string claveTipoCorrida, DBContextAdapter dbContext)
        {
            PeriodosNomina periodosNomina;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                periodosNomina = (from p in getSession().Set<PeriodosNomina>()
                                  join cor in getSession().Set<TipoCorrida>()
                                  on p.tipoCorrida.id equals cor.id
                                  where p.clave == clave && p.tipoNomina.clave == claveTipoNomina
                                  && cor.clave == claveTipoCorrida
                                  select p).SingleOrDefault();
                mensajeResultado.resultado = periodosNomina;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPeriodosNominaPorClaveYTipoDeNominaCorrida()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPeriodosNominaPorClaveYTipoDeNominaECorrida(string clave, TipoNomina tipoNomina, string claveTipoCorrida, DBContextAdapter dbContext)
        {
            PeriodosNomina periodosNomina;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                periodosNomina = (from p in getSession().Set<PeriodosNomina>()
                                  join cor in getSession().Set<TipoCorrida>()
                                  on p.tipoCorrida.id equals cor.id
                                  where p.clave == clave && p.tipoNomina.id == tipoNomina.id
                                  && cor.clave == claveTipoCorrida
                                  select p).SingleOrDefault();
                mensajeResultado.resultado = periodosNomina;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPeriodosNominaPorClaveYTipoDeNominaECorrida()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPeriodosNominaPorFechasYTipoNominaCorrida(DateTime inicio, DateTime fin, TipoNomina tipoNomina, string claveTipoCorrida, DBContextAdapter dbContext)
        {
            List<PeriodosNomina> periodosNominaTmp, periodosNomina;
            try
            {
                DateTime fechaInicial = DateTime.Now;
                DateTime fechaFinal = DateTime.Now;
                fechaInicial = inicio;
                fechaFinal = fin;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                DateTime? fechaini;
                fechaini = (from p in getSession().Set<PeriodosNomina>()
                            where p.tipoNomina.clave == tipoNomina.clave &&
                            p.tipoCorrida.clave == claveTipoCorrida && p.fechaInicial <= inicio
                            orderby p.fechaInicial descending
                            select p.fechaInicial).FirstOrDefault();
                if (fechaini == null)
                {
                    fechaInicial = inicio;
                }
                else
                {
                    fechaInicial = Convert.ToDateTime(fechaini);
                }


                DateTime? fechafin;
                fechafin = (from p in getSession().Set<PeriodosNomina>()
                            where p.tipoNomina.clave == tipoNomina.clave &&
                            p.tipoCorrida.clave == claveTipoCorrida && p.fechaFinal >= fin
                            orderby p.fechaFinal ascending
                            select p.fechaFinal).FirstOrDefault();
                if (fechafin == null)
                {
                    fechaFinal = fechaInicial;
                }
                else
                {
                    fechaFinal = Convert.ToDateTime(fechaInicial);
                }

                periodosNominaTmp = (from p in getSession().Set<PeriodosNomina>()
                                     where p.tipoNomina.clave == tipoNomina.clave &&
                                     p.tipoCorrida.clave == claveTipoCorrida && p.fechaInicial >= fechaini
                                     && p.fechaFinal <= fechafin && p.status == true && p.bloquear == false
                                     select p).ToList();

                if (periodosNominaTmp.Count > 1)
                {
                    if (fechaini.Equals(periodosNominaTmp[0].fechaCierre))
                    {
                        periodosNominaTmp.RemoveAt(0);
                    }
                    else
                    {
                        periodosNominaTmp.RemoveAt(1);
                    }
                }
                periodosNomina = new List<PeriodosNomina>();


                int anioOEjercicio = fechaInicial.Year, anioOEjercicio2 = fechaFinal.Year;
                for (int i = 0; i < periodosNominaTmp.Count; i++)
                {
                    if (periodosNominaTmp[i].año.Equals(anioOEjercicio) || periodosNominaTmp[i].año.Equals(anioOEjercicio2))
                    {
                        periodosNomina.Add(periodosNominaTmp[i]);
                    }
                }

                mensajeResultado.resultado = periodosNomina;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPeriodosNominaPorFechasYTipoNominaCorrida()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPeriodosNominaPorFechaTipoNominaCorrida(DateTime fecha, string claveTipoNomina, string claveTipoCorrida, DBContextAdapter dbContext)
        {
            PeriodosNomina periodosNomina;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                periodosNomina = (from p in getSession().Set<PeriodosNomina>()
                                  where (fecha >= p.fechaInicial && fecha <= p.fechaFinal) && p.tipoNomina.clave == claveTipoNomina
                                  && p.tipoCorrida.clave == claveTipoCorrida
                                  select p).SingleOrDefault();
                mensajeResultado.resultado = periodosNomina;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPeriodosNominaPorFechaTipoNominaCorrida()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPeriodosNominaPorFechayTipoCorridaSinStatus(DateTime fecha, string claveTipoNomina, string claveTipoCorrida, DBContextAdapter dbContext)
        {
            PeriodosNomina periodosNomina;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                periodosNomina = (from p in getSession().Set<PeriodosNomina>()
                                  join cor in getSession().Set<TipoCorrida>()
                                  on p.tipoCorrida.id equals cor.id
                                  where (p.fechaInicial >= fecha || p.fechaFinal >= fecha) &&
                                  p.tipoNomina.clave == claveTipoNomina && cor.clave == claveTipoCorrida
                                  orderby p.año, p.clave
                                  select p).SingleOrDefault();
                mensajeResultado.resultado = periodosNomina;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPeriodosNominaPorFechayTipoCorridaSinStatus()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPeriodosNominaPorTipoNominaYRangoDeFechas(DateTime fechaInicial, DateTime fechaFinal, string claveTipoNomina, string claveTipoCorrida, DBContextAdapter dbContext)
        {
            List<PeriodosNomina> periodosNomina = new List<PeriodosNomina>();
            try
            {

                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                periodosNomina = (from p in getSession().Set<PeriodosNomina>()
                                  join cor in getSession().Set<TipoCorrida>()
                                  on p.tipoCorrida.id equals cor.id
                                  where p.tipoNomina.clave == claveTipoNomina &&
                                  cor.clave == claveTipoCorrida &&
                                  p.fechaFinal >= fechaInicial && p.fechaInicial <= fechaFinal
                                  select p).ToList();
                mensajeResultado.resultado = periodosNomina;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPeriodosNominaPorTipoNominaYRangoDeFechas()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPrimerPeriodo(int año, string claveTipoNomina, string claveTipoCorrida, DBContextAdapter dbContext)
        {
            PeriodosNomina periodosNomina;
            DateTime fechaActual = DateTime.Now;
            int? año2 = año;
            try
            {//dudas sobre estos valores en java q.setFirstResult(0);q.setMaxResults(1);

                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                periodosNomina = (from p in getSession().Set<PeriodosNomina>()
                                  where p.año == año2 && p.tipoNomina.clave == claveTipoNomina
                                  && p.tipoCorrida.clave == claveTipoCorrida
                                  orderby p.clave ascending
                                  select p).FirstOrDefault();
                mensajeResultado.resultado = periodosNomina;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPrimerPeriodo()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getStatusPeriodo(decimal idPeriodo, DBContextAdapter dbContext)
        {
            bool existe;
            try
            {

                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                existe = (from p in getSession().Set<PeriodosNomina>()
                          join cor in getSession().Set<TipoCorrida>()
                          on p.tipoCorrida.id equals cor.id
                          where p.id == idPeriodo
                          select p.status).SingleOrDefault();
                mensajeResultado.resultado = existe;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getStatusPeriodo()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getUltimoPeriodo(int año, string claveTipoNomina, string claveTipoCorrida, DBContextAdapter dbContext)
        {
            PeriodosNomina periodosNomina;
            DateTime fechaActual = DateTime.Now;
            int? año2 = año;
            try
            {//dudas sobre estos valores en java q.setFirstResult(0);q.setMaxResults(1);

                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                periodosNomina = (from p in getSession().Set<PeriodosNomina>()
                                  where p.año == año2 && p.tipoNomina.clave == claveTipoNomina
                                  && p.tipoCorrida.clave == claveTipoCorrida
                                  orderby p.clave descending
                                  select p).FirstOrDefault();
                mensajeResultado.resultado = periodosNomina;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getUltimoPeriodo()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getUltimoPeriodoCerradoPorFecha(DateTime fecha, string claveTipoNomina, string claveTipoCorrida, DBContextAdapter dbContext)
        {
            PeriodosNomina periodosNomina;
            DateTime fechaActual = DateTime.Now;
            try
            {//dudas sobre estos valores en java q.setFirstResult(0);q.setMaxResults(1);

                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                periodosNomina = (from p in getSession().Set<PeriodosNomina>()
                                  join cor in getSession().Set<TipoCorrida>()
                                  on p.tipoCorrida.id equals cor.id
                                  where (p.fechaInicial >= fecha || p.fechaFinal >= fecha) &&
                                  p.tipoNomina.clave == claveTipoNomina &&
                                  cor.clave == claveTipoCorrida &&
                                  p.status == false && p.año == fechaActual.Year
                                  orderby p.clave descending
                                  select p).SingleOrDefault();
                mensajeResultado.resultado = periodosNomina;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPeriodosNominaActualPorFecha()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje ObtenerFechaFinalMax(string claveTipoNomina, string claveTipoCorrida, int año, DBContextAdapter dbContext)
        {
            DateTime fecha;
            DateTime fechafin = Convert.ToDateTime("01/01/1900");
            try
            {

                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (año > 0)
                {
                    fecha = (DateTime)getSession().Set<PeriodosNomina>().Where(p => p.tipoNomina.clave == claveTipoNomina && p.tipoCorrida.clave == claveTipoCorrida && p.año == año).DefaultIfEmpty().Max(p => (p == null ? fechafin : p.fechaFinal));
                }
                else
                {
                    fecha = (DateTime)getSession().Set<PeriodosNomina>().Where(p => p.tipoNomina.clave == claveTipoNomina && p.tipoCorrida.clave == claveTipoCorrida).DefaultIfEmpty().Max(p => (p == null ? fechafin : p.fechaFinal));
                }

                if (fecha.Year == año)
                {
                    mensajeResultado.resultado = fecha;
                }
                else
                {
                    mensajeResultado.resultado = null;
                }


                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ObtenerFechaFinalMax()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje ObtenerFechaFinalMin(string claveTipoNomina, string claveTipoCorrida, int año, DBContextAdapter dbContext)
        {
            DateTime fecha;
            DateTime fechafin = Convert.ToDateTime("01/01/1900");
            try
            {

                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();

                fecha = (DateTime)getSession().Set<PeriodosNomina>().Where(p => p.tipoNomina.clave == claveTipoNomina && p.tipoCorrida.clave == claveTipoCorrida && p.año == año).DefaultIfEmpty().Min(p => (p == null ? fechafin : p.fechaInicial));


                if (fecha.Year == año)
                {
                    mensajeResultado.resultado = fecha;
                }
                else
                {
                    mensajeResultado.resultado = null;
                }
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ObtenerFechaFinalMin()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje saveDeletePeriodosNomina(List<PeriodosNomina> entitysCambios, List<PeriodosNomina> eliminados, TipoCorrida tCorrida, DBContextAdapter dbContext)
        {
            List<PeriodosNomina> listEsp = new List<PeriodosNomina>();
            bool exitoRegistro = true;
            PeriodosNomina p = null;
            DateTime fecha = DateTime.Now;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (eliminados != null && eliminados.Count > 0)
                {
                    Object[] eliminados2 = new Object[eliminados.Count];
                    String[] claveEliminar = new String[eliminados.Count];
                    String claveTipoNomina = eliminados[0].tipoNomina.clave;
                    for (int i = 0; i < eliminados.Count; i++)
                    {
                        eliminados2[i] = eliminados[i].id;
                        claveEliminar[i] = eliminados[i].clave;

                    }
                    exitoRegistro = deleteListQuery(eliminados2, claveEliminar, claveTipoNomina, tCorrida, dbContext);

                }
                entitysCambios = (entitysCambios == null ? new List<PeriodosNomina>() : entitysCambios);
                int cantidadSaveUpdate = 0, cantidadFlush = 50;
                if (exitoRegistro && entitysCambios.Count > 0)
                {
                    for (int i = 0; i < entitysCambios.Count; i++)
                    {
                        fecha = Convert.ToDateTime(entitysCambios[i].fechaFinal);
                        if (entitysCambios[i].año >= fecha.Year || entitysCambios[i].cierreMes == true)
                        {
                            bool existeId = entitysCambios[i].id > 0;
                            getSession().Set<PeriodosNomina>().AddOrUpdate(entitysCambios[i]);

                            if (existeId == true && tCorrida.clave.Equals("PER"))
                            {

                                int noOfRowDeleted = getSession().Database.ExecuteSqlCommand("UPDATE PeriodosNomina o set o.descripcion=@descripcion,"
                                    + "o.detalleConceptoRecibo =@detalleConceptoRecibo,"
                                    + "o.fechaInicial=@fechaInicial,o.fechaFinal=@fechaFinal,o.fechaPago=@fechaPago,"
                                    + "o.fechaAsistenciInicial=@fechaAsistenciInicial,"
                                    + "o.fechaAsistenciaFinal=@fechaAsistenciaFinal,o.descontarAhorro=@descontarAhorro,"
                                    + "o.descontarPrestamo=@descontarPrestamo,o.soloPercepciones=@soloPercepciones,"
                                    + "o.incluirBajas=@incluirBajas,o.bloquear=@bloquear,o.cierreMes=@cierreMes,o.fechaModificado=@fechaModificado "
                                    + "WHERE o.id IN(select p.id from PeriodosNomina p where p.clave=@clavesEliminar "
                                    + "AND p.tipoNomina.clave=@claveTipoNomina AND p.tipoCorrida.usaCorrPeriodica=1 AND p.origen=1)",
                                    new SqlParameter(" @descripcion", entitysCambios[i].descripcion),
                                    new SqlParameter(" @detalleConceptoRecibo", entitysCambios[i].detalleConceptoRecibo),
                                    new SqlParameter(" @fechaInicial", entitysCambios[i].fechaInicial),
                                    new SqlParameter(" @fechaFinal", entitysCambios[i].fechaFinal),
                                    new SqlParameter(" @fechaPago", entitysCambios[i].fechaPago),
                                    new SqlParameter(" @fechaAsistenciInicial", entitysCambios[i].fechaAsistenciInicial),
                                    new SqlParameter(" @fechaAsistenciaFinal", entitysCambios[i].fechaAsistenciaFinal),
                                    new SqlParameter(" @descontarAhorro", entitysCambios[i].descontarAhorro),
                                    new SqlParameter(" @descontarPrestamo", entitysCambios[i].descontarPrestamo),
                                    new SqlParameter(" @soloPercepciones", entitysCambios[i].soloPercepciones),
                                    new SqlParameter(" @incluirBajas", entitysCambios[i].incluirBajas),
                                    new SqlParameter(" @bloquear", entitysCambios[i].bloquear),
                                    new SqlParameter(" @cierreMes", entitysCambios[i].cierreMes),
                                    new SqlParameter(" @fechaModificado", entitysCambios[i].fechaModificado),
                                    new SqlParameter(" @clavesEliminar", entitysCambios[i].clave),
                                    new SqlParameter(" @claveTipoNomina", entitysCambios[i].tipoNomina.clave));
                            }
                            listEsp.Add(entitysCambios[i]);
                        }
                        //cantidadSaveUpdate++;
                        //if (cantidadSaveUpdate % cantidadFlush == 0 & cantidadSaveUpdate > 0)
                        //{
                        getSession().SaveChanges();
                        //}
                    }
                }

                if (exitoRegistro)
                {
                    getSession().SaveChanges();
                    mensajeResultado.resultado = true;
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ObtenerFechaFinalMin()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        private bool deleteListQuery(Object[] eliminados, String[] clavesEliminar, String claveTipoNomina, TipoCorrida tCorrida, DBContextAdapter dbContext)
        {
            bool existe = true;
            try
            {
                // deleteListQuery("PeriodosNomina", "Id", eliminados);
                deleteListQuery("PeriodosNomina", new CamposWhere("PeriodosNomina.id", eliminados, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                if (tCorrida.clave.Equals("PER"))
                {
                    int result = getSession().Database.ExecuteSqlCommand("DELETE FROM PeriodosNomina o  WHERE o.id IN(select p.id from PeriodosNomina p WHERE p.clave IN(@clavesEliminar) AND p.tipoNomina.clave=@claveTipoNomina AND p.tipoCorrida.usaCorrPeriodica=1 AND p.origen=1", new SqlParameter(" @clavesEliminar", clavesEliminar), new SqlParameter("@claveTipoNomina", claveTipoNomina));
                    ////////////System.out.println("Rows affected: " + result);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ObtenerFechaFinalMin()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                existe = false;
            }
            return existe;
        }

        public Mensaje getPeriodosNominaPorID(decimal? id, DBContextAdapter dbContext)
        {
            // PeriodosNomina per = new PeriodosNomina();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var per = (from p in getSession().Set<PeriodosNomina>()
                           where p.id == id
                           select new
                           {
                               acumularAMes = p.acumularAMes,
                               //    aguinaldoPagos = p.aguinaldoPagos,
                               // asistencias = p.asistencias,
                               año = p.año,
                               bloquear = p.bloquear,
                               // calculoUnidades = p.calculoUnidades,
                               // cfdiEmpleado = p.cfdiEmpleado,
                               cierreMes = p.cierreMes,
                               clave = p.clave,
                               claveUsuario = p.claveUsuario,
                               //creditoMovimientos = p.creditoMovimientos,
                               //creditoPorEmpleado = p.creditoPorEmpleado,
                               //descontarAhorro = p.descontarAhorro,
                               //descontarPrestamo = p.descontarPrestamo,
                               descripcion = p.descripcion,
                               // detalleAsistencia = p.detalleAsistencia,
                               //detalleConceptoRecibo = p.detalleConceptoRecibo,
                               diasIMSS = p.diasIMSS,
                               diasPago = p.diasPago,
                               fechaAsistenciaFinal = p.fechaAsistenciaFinal,
                               fechaAsistenciInicial = p.fechaAsistenciInicial,
                               fechaCierre = p.fechaCierre,
                               fechaFinal = p.fechaFinal,
                               fechaInicial = p.fechaInicial,
                               fechaModificado = p.fechaModificado,
                               fechaPago = p.fechaPago,
                               id = p.id,
                               // inasistenciaPorHora = p.inasistenciaPorHora,
                               incluirBajas = p.incluirBajas,
                               //listaperiodoPtuDias = p.listaperiodoPtuDias,
                               //listaperiodoPtuPercep = p.listaperiodoPtuPercep,
                               //movNomConcep = p.movNomConcep,
                               origen = p.origen,
                               // semaforoCalculoNomina = p.semaforoCalculoNomina,
                               //semaforoTimbradoPac = p.semaforoTimbradoPac,
                               soloPercepciones = p.soloPercepciones,
                               status = p.status,
                               // tipoCorrida = p.tipoCorrida,
                               tipoCorrida_ID = p.tipoCorrida_ID,
                               //tipoNomina = p.tipoNomina,
                               tipoNomina_ID = p.tipoNomina_ID,
                               tipoUso = p.tipoUso,
                               //vacDisfr_perpriVaca = p.vacDisfr_perpriVaca,
                               //vacDisf_perApli = p.vacDisf_perApli,
                               //vacDisf_perVac = p.vacDisf_perVac
                           }).Single();
                mensajeResultado.resultado = per;
                mensajeResultado.error = "";
                mensajeResultado.noError = 0;
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPeriodosNominaPorID()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPeriodosNominaPorAñoYTipoNominaYTipoCorridaID(int año, string tipoNomina, decimal? tipoCorrida, DBContextAdapter dbContext)
        {
            int? anio = año;

            //  List<PeriodosNomina> periodosNomina = new List<PeriodosNomina>();
            try
            {

                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                decimal? clve = Convert.ToDecimal(tipoNomina);
                //string clave = (from t in getSession().Set<TipoCorrida>()
                //                where t.id == tipoCorrida
                //                select t.clave).SingleOrDefault();
                var periodosNomina = (from p in getSession().Set<PeriodosNomina>()
                                      where p.año == año && p.tipoNomina.id == clve &&
                                      p.tipoCorrida.id == tipoCorrida
                                      orderby p.clave
                                      select new
                                      {
                                          acumularAMes = p.acumularAMes,
                                          //aguinaldoPagos = p.aguinaldoPagos,
                                          // asistencias = p.asistencias,
                                          año = p.año,
                                          bloquear = p.bloquear,
                                          // calculoUnidades = p.calculoUnidades,
                                          //cfdiEmpleado = p.cfdiEmpleado,
                                          cierreMes = p.cierreMes,
                                          clave = p.clave,
                                          claveUsuario = p.claveUsuario,
                                          //creditoMovimientos = p.creditoMovimientos,
                                          //creditoPorEmpleado = p.creditoPorEmpleado,
                                          descontarAhorro = p.descontarAhorro,
                                          descontarPrestamo = p.descontarPrestamo,
                                          descripcion = p.descripcion,
                                          //detalleAsistencia = p.detalleAsistencia,
                                          detalleConceptoRecibo = p.detalleConceptoRecibo,
                                          diasIMSS = p.diasIMSS,
                                          diasPago = p.diasPago,
                                          fechaAsistenciaFinal = p.fechaAsistenciaFinal,
                                          fechaAsistenciInicial = p.fechaAsistenciInicial,
                                          fechaCierre = p.fechaCierre,
                                          fechaFinal = p.fechaFinal,
                                          fechaInicial = p.fechaInicial,
                                          fechaModificado = p.fechaModificado,
                                          fechaPago = p.fechaPago,
                                          id = p.id,
                                          //inasistenciaPorHora = p.inasistenciaPorHora,
                                          incluirBajas = p.incluirBajas,
                                          // listaperiodoPtuDias = p.listaperiodoPtuDias,
                                          // listaperiodoPtuPercep = p.listaperiodoPtuPercep,
                                          //movNomConcep = p.movNomConcep,
                                          origen = p.origen,
                                          // semaforoCalculoNomina = p.semaforoCalculoNomina,
                                          //semaforoTimbradoPac = p.semaforoTimbradoPac,
                                          soloPercepciones = p.soloPercepciones,
                                          status = p.status,
                                          // tipoCorrida = p.tipoCorrida,
                                          tipoCorrida_ID = p.tipoCorrida_ID,
                                          //tipoNomina = p.tipoNomina,
                                          tipoNomina_ID = p.tipoNomina_ID,
                                          tipoUso = p.tipoUso,
                                          //vacDisfr_perpriVaca = p.vacDisfr_perpriVaca,
                                          //vacDisf_perApli = p.vacDisf_perApli,
                                          //vacDisf_perVac = p.vacDisf_perVac
                                      }).ToList();
                mensajeResultado.resultado = periodosNomina;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPeriodosNominaPorAñoYTipoNominaYTipoCorridaID()1_Error: ").Append(ex));
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
        public Mensaje getPeriodosNominaPorAñoYIDTipoNominaYIDTipoCorrida(int año, decimal? tipoNomina, decimal? tipoCorrida, DBContextAdapter dbContext)
        {
            int? anio = año;

            //  List<PeriodosNomina> periodosNomina = new List<PeriodosNomina>();
            try
            {

                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var periodosNomina = (from p in getSession().Set<PeriodosNomina>()
                                      where p.año == año && p.tipoNomina_ID == tipoNomina &&
                                      p.tipoCorrida_ID == tipoCorrida
                                      orderby p.clave
                                      select new
                                      {
                                          acumularAMes = p.acumularAMes,
                                          //aguinaldoPagos = p.aguinaldoPagos,
                                          // asistencias = p.asistencias,
                                          año = p.año,
                                          bloquear = p.bloquear,
                                          // calculoUnidades = p.calculoUnidades,
                                          //cfdiEmpleado = p.cfdiEmpleado,
                                          cierreMes = p.cierreMes,
                                          clave = p.clave,
                                          claveUsuario = p.claveUsuario,
                                          //creditoMovimientos = p.creditoMovimientos,
                                          //creditoPorEmpleado = p.creditoPorEmpleado,
                                          descontarAhorro = p.descontarAhorro,
                                          descontarPrestamo = p.descontarPrestamo,
                                          descripcion = p.descripcion,
                                          //detalleAsistencia = p.detalleAsistencia,
                                          detalleConceptoRecibo = p.detalleConceptoRecibo,
                                          diasIMSS = p.diasIMSS,
                                          diasPago = p.diasPago,
                                          fechaAsistenciaFinal = p.fechaAsistenciaFinal,
                                          fechaAsistenciInicial = p.fechaAsistenciInicial,
                                          fechaCierre = p.fechaCierre,
                                          fechaFinal = p.fechaFinal,
                                          fechaInicial = p.fechaInicial,
                                          fechaModificado = p.fechaModificado,
                                          fechaPago = p.fechaPago,
                                          id = p.id,
                                          //inasistenciaPorHora = p.inasistenciaPorHora,
                                          incluirBajas = p.incluirBajas,
                                          // listaperiodoPtuDias = p.listaperiodoPtuDias,
                                          // listaperiodoPtuPercep = p.listaperiodoPtuPercep,
                                          //movNomConcep = p.movNomConcep,
                                          origen = p.origen,
                                          // semaforoCalculoNomina = p.semaforoCalculoNomina,
                                          //semaforoTimbradoPac = p.semaforoTimbradoPac,
                                          soloPercepciones = p.soloPercepciones,
                                          status = p.status,
                                          // tipoCorrida = p.tipoCorrida,
                                          tipoCorrida_ID = p.tipoCorrida_ID,
                                          //tipoNomina = p.tipoNomina,
                                          tipoNomina_ID = p.tipoNomina_ID,
                                          tipoUso = p.tipoUso,
                                          //vacDisfr_perpriVaca = p.vacDisfr_perpriVaca,
                                          //vacDisf_perApli = p.vacDisf_perApli,
                                          //vacDisf_perVac = p.vacDisf_perVac
                                      }).ToList();
                mensajeResultado.resultado = periodosNomina;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPeriodosNominaPorAñoYTipoNominaYTipoCorrida()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPeriodosNominaPorFechaTipoNominaCorridaEnti(DateTime fecha, string claveTipoNomina, string claveTipoCorrida, DBContextAdapter dbContext)
        {
            // PeriodosNomina periodosNomina;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var periodosNomina = (from p in getSession().Set<PeriodosNomina>()
                                      where (fecha.Date >= p.fechaInicial && fecha.Date <= p.fechaFinal) && p.tipoNomina.clave == claveTipoNomina
                                      && p.tipoCorrida.clave == claveTipoCorrida
                                      select new
                                      {
                                          p.acumularAMes,
                                          p.año,
                                          p.bloquear,
                                          p.cierreMes,
                                          p.clave,
                                          p.claveUsuario,
                                          p.descontarAhorro,
                                          p.descontarPrestamo,
                                          p.descripcion,
                                          p.detalleConceptoRecibo,
                                          p.fechaCierre,
                                          p.fechaFinal,
                                          p.fechaInicial,
                                          p.fechaModificado,
                                          p.fechaPago,
                                          p.id,
                                          p.status

                                      }).SingleOrDefault();
                mensajeResultado.resultado = periodosNomina;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPeriodosNominaPorFechaTipoNominaCorridaEnti()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPeriodosNominaPorClaveYTipoDeNominaCorridaAñoEnti(string clave, string claveTipoNomina, string claveTipoCorrida, int ejercicio, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var periodosNomina = (from p in getSession().Set<PeriodosNomina>()
                                      join cor in getSession().Set<TipoCorrida>()
                                      on p.tipoCorrida.id equals cor.id
                                      where p.clave == clave && p.tipoNomina.clave == claveTipoNomina
                                      && cor.clave == claveTipoCorrida & p.año == ejercicio
                                      select new
                                      {
                                          p.acumularAMes,
                                          p.año,
                                          p.bloquear,
                                          p.cierreMes,
                                          p.clave,
                                          p.claveUsuario,
                                          p.descontarAhorro,
                                          p.descontarPrestamo,
                                          p.descripcion,
                                          p.detalleConceptoRecibo,
                                          p.diasIMSS,
                                          p.diasPago,
                                          p.fechaAsistenciaFinal,
                                          p.fechaAsistenciInicial,
                                          p.fechaCierre,
                                          p.fechaFinal,
                                          p.fechaInicial,
                                          p.fechaModificado,
                                          p.fechaPago,
                                          p.id,
                                          p.incluirBajas,
                                          p.origen,
                                          p.soloPercepciones,
                                          p.status,
                                          p.tipoCorrida_ID,
                                          p.tipoNomina_ID,
                                          p.tipoUso
                                      }).SingleOrDefault();
                mensajeResultado.resultado = periodosNomina;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPeriodosNominaPorClaveYTipoDeNominaCorridaEnti()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPeriodosNominaPorFechaTipoNominaCorridaEntiJS(DateTime fecha, decimal claveTipoNomina, string claveTipoCorrida, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var periodosNomina = (from p in getSession().Set<PeriodosNomina>()
                                      where (fecha.Date >= p.fechaInicial && fecha.Date <= p.fechaFinal) && p.tipoNomina.id == claveTipoNomina
                                      && p.tipoCorrida.clave == claveTipoCorrida
                                      select new
                                      {
                                          p.acumularAMes,
                                          p.año,
                                          p.bloquear,
                                          p.cierreMes,
                                          p.clave,
                                          p.claveUsuario,
                                          p.descontarAhorro,
                                          p.descontarPrestamo,
                                          p.descripcion,
                                          p.detalleConceptoRecibo,
                                          p.fechaCierre,
                                          p.fechaFinal,
                                          p.fechaInicial,
                                          p.fechaModificado,
                                          p.fechaPago,
                                          p.id,
                                          p.status,
                                          p.tipoNomina_ID,
                                          p.tipoCorrida_ID

                                      }).SingleOrDefault();
                mensajeResultado.resultado = periodosNomina;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPeriodosNominaPorFechaTipoNominaCorridaEnti()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
    }
}