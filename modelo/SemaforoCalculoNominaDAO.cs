/**
* @author: Daniel Ruelas 
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Clase SemaforoCalculoNominaDAO para llamados a metodos de Entity
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
using Exitosw.Payroll.Entity.entidad;
using System.Reflection;
using System.Text;
using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.Entity.util;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    
    public class SemaforoCalculoNominaDAO : GenericRepository<SemaforoCalculoNomina>, SemaforoCalculoNominaDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        public Mensaje agregar(SemaforoCalculoNomina entity, DBContextAdapter dbContext)
        {
            Mensaje semaforo = new Mensaje();
            try
            {
               
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                SemaforoCalculoNomina smf = null;
                //smf = (SemaforoCalculoNomina)existeClave(SemaforoCalculoNomina.class.getSimpleName(), new String[]{"tipoNomina.clave", "periodoNomina.id"}, new Object[]{entity.getTipoNomina().getClave(), entity.getPeriodoNomina().getId()}, null);
               
                if (smf == null)
                {
                    semaforo = addValoresSemaforo(semaforo, entity);
                    //                if (entity.getPeriodoNomina().isStatus()) {
                    //                    PeriodosNomina periodo = (PeriodosNomina) existeClave(PeriodosNomina.class.getSimpleName(), new String[]{"id", "status"}, new Object[]{entity.getPeriodoNomina().getId(), false}, null);
                    //                    if (periodo == null) {
                    //                        entity = makePersistent(entity);
                    //                        semaforo.setResultado(entity);
                    //                        semaforo.setNoError(entity == null ? 1 : 0);
                    //                    } else {
                    //                        semaforo.setResultado(null);
                    //                        semaforo.setNoError(2);
                    //                    }
                    //                }
                }
                else {
                    if (smf.periodosNomina.status == false)
                    {
                        semaforo.noError = 2;
                        semaforo.resultado = null;
                    }
                    else if (Convert.ToInt32(entity.tiempoInicio - smf.tiempoInicio) > 10800000)
                    {
                        getSession().Set<SemaforoCalculoNomina>().Add(smf);
                        getSession().SaveChanges();
                        semaforo = addValoresSemaforo(semaforo, entity);
                    }
                    else if (entity.usuario.Equals(smf.usuario) && entity.tipoCalculo == smf.tipoCalculo)
                    {
                        if (entity.periodosNomina.status)
                        {
                            smf.tiempoInicio = entity.tiempoInicio;
                            semaforo = addValoresSemaforo(semaforo, entity);

                        }
                        else
                        {
                            semaforo.noError = 2;
                            semaforo.resultado = null;
                        }


                    }
                    else {

                        if (entity.tipoCalculo == SemaforoCalculoNomina.TipoCalculo.AGREGA_MOVIMIENTOS)
                        {
                            if (smf.tipoCalculo == SemaforoCalculoNomina.TipoCalculo.CIERRE_PERIODO)
                            {
                                semaforo.noError = 7;
                            }
                            else
                            {
                                semaforo.noError = 8;
                            }
                        }
                        else if (entity.tipoCalculo == SemaforoCalculoNomina.TipoCalculo.CIERRE_PERIODO)
                        {
                            if (smf.tipoCalculo == SemaforoCalculoNomina.TipoCalculo.CALCULO_NOMINA)
                            {
                                semaforo.noError = 3;
                            }
                            else
                            {

                                semaforo.noError = 4;
                            }
                        }
                        else if (entity.tipoCalculo == SemaforoCalculoNomina.TipoCalculo.CALCULO_NOMINA)
                        {
                            if (smf.tipoCalculo == SemaforoCalculoNomina.TipoCalculo.CIERRE_PERIODO)
                            {
                                semaforo.noError = 2;

                            }
                            else if (smf.tipoCalculo == SemaforoCalculoNomina.TipoCalculo.AGREGA_MOVIMIENTOS)
                            {

                                semaforo.noError = 5;
                            }
                            else
                            {
                                semaforo.noError = 1;
                            }
                        }
                        else if (entity.periodosNomina.status)
                        {
                            semaforo.noError = 1;
                        }
                        else {
                            semaforo.noError = 2;
                        }
                        semaforo.resultado = null;
                    }

                }
                
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
            return semaforo;
        }

        public Mensaje eliminar(SemaforoCalculoNomina entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                //var sentencia = (from g in getSession().Set<Genero>()
                //                 where g.id == entity.id
                //                 select g).FirstOrDefault();
                getSession().Set<SemaforoCalculoNomina>().Attach(entity);
                getSession().Set<SemaforoCalculoNomina>().Remove(entity);
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

        private Mensaje addValoresSemaforo(Mensaje mensaje, SemaforoCalculoNomina entity)
        {
            if (entity.periodosNomina.status == true)
            {
                PeriodosNomina periodo = null;
                //PeriodosNomina periodo = (PeriodosNomina)existeClave(PeriodosNomina.class.getSimpleName(), new String[]{"id", "status"}, new Object[]{entity.getPeriodoNomina().getId(), false}, null);
                if (periodo == null)
                {
                    getSession().Set<SemaforoCalculoNomina>().Add(entity);
                    mensaje.resultado = entity;
                    mensaje.noError = entity == null ? 1 : 0;
                }
                else
                {
                    mensaje.resultado = null;
                    mensaje.noError = 2;
                }
            }
            else {
                getSession().Set<SemaforoCalculoNomina>().Add(entity);
                mensaje.resultado = entity;
                mensaje.noError = entity == null ? 1 : 0;
            }
            return mensaje;
        }
        }
}