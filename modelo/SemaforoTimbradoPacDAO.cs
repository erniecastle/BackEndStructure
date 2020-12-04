/**
* @author: Daniel Ruelas 
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Clase SemaforoTimbradoPacDAO para llamados a metodos de Entity
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
using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Core.util;
using System.Text;
using System.Reflection;
using Exitosw.Payroll.Core.genericos.campos;
using Exitosw.Payroll.Entity.util;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public class SemaforoTimbradoPacDAO : GenericRepository<SemaforoTimbradoPac>, SemaforoTimbradoPacDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        public Mensaje agregar(SemaforoTimbradoPac entity, DBContextAdapter dbContext)
        {
            Mensaje semaforo = new Mensaje();
            try
            {
                
                //inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                SemaforoTimbradoPac smf = null;
                List<CamposWhere> camposwhere = new List<CamposWhere>();
                camposwhere.Add(new CamposWhere(string.Concat(typeof(SemaforoTimbradoPac).Name, "tipoNomina.clave"),entity.tipoNomina.clave,OperadorComparacion.IGUAL,OperadorLogico.AND));
                camposwhere.Add(new CamposWhere(string.Concat(typeof(SemaforoTimbradoPac).Name, "periodoNomina.id"), entity.periodosNomina.id, OperadorComparacion.IGUAL, OperadorLogico.AND));
                mensajeResultado = existeClave(typeof(SemaforoTimbradoPac).Name, camposwhere, dbContext);
                smf = (SemaforoTimbradoPac)mensajeResultado.resultado;
                //smf=(SemaforoTimbradoPac) existeClave(SemaforoTimbradoPac.class.getSimpleName(), new String[]{"tipoNomina.clave", "periodoNomina.id"}, new Object[]{entity.getTipoNomina().getClave(), entity.getPeriodoNomina().getId()}, null);
                if (smf == null)
                {
                    semaforo = addValoresSemaforo(semaforo, entity);
                }
                else {
                    DateTime cal = DateTime.Now;
                    DateTime calNueva = DateTime.Now;
                    cal = smf.tiempoInicio;
                    calNueva = entity.tiempoInicio;
                    if (!calNueva.ToShortDateString().Equals(cal.ToShortDateString()))
                    {
                        getSession().Set<SemaforoTimbradoPac>().Add(smf);
                        getSession().SaveChanges();
                        semaforo = addValoresSemaforo(semaforo, entity);
                    }
                    else if (Convert.ToInt32(entity.tiempoInicio - smf.tiempoInicio) > 10800000)
                    {
                        getSession().Set<SemaforoTimbradoPac>().Add(smf);
                        getSession().SaveChanges();
                        semaforo = addValoresSemaforo(semaforo, entity);

                    }
                    else if (entity.usuario.Equals(smf.usuario))
                    {
                        smf.tiempoInicio = entity.tiempoInicio;
                        semaforo = addValoresSemaforo(semaforo, smf);

                    }
                    else {
                        if (entity.tipoTimbrado==SemaforoTimbradoPac.TipoTimbrado.ABRIENDO_PERIODO) {
                            if (smf.tipoTimbrado == SemaforoTimbradoPac.TipoTimbrado.GENERANDO_DATOS_TIMBRADO)
                            {
                                semaforo.noError = 2;
                            }
                            else if (smf.tipoTimbrado == SemaforoTimbradoPac.TipoTimbrado.TIMBRANDO)
                            {
                                semaforo.noError = 3;

                            }
                            else if (smf.tipoTimbrado == SemaforoTimbradoPac.TipoTimbrado.CANCELANDO_TIMBRADOS)
                            {
                                semaforo.noError = 4;
                            }
                            else {
                                semaforo.noError = 5;
                            }

                        } else if (entity.tipoTimbrado==SemaforoTimbradoPac.TipoTimbrado.GENERANDO_DATOS_TIMBRADO) {
                            if (smf.tipoTimbrado == SemaforoTimbradoPac.TipoTimbrado.TIMBRANDO)
                            {
                                semaforo.noError = 3;
                            }
                            else if (smf.tipoTimbrado == SemaforoTimbradoPac.TipoTimbrado.CANCELANDO_TIMBRADOS)
                            {
                                semaforo.noError = 4;

                            }
                            else if (smf.tipoTimbrado == SemaforoTimbradoPac.TipoTimbrado.ABRIENDO_PERIODO)
                            {
                                semaforo.noError = 5;
                            }
                            else
                            {
                                semaforo.noError = 2;
                            }

                        } else if (entity.tipoTimbrado==SemaforoTimbradoPac.TipoTimbrado.TIMBRANDO) {
                            if (smf.tipoTimbrado == SemaforoTimbradoPac.TipoTimbrado.GENERANDO_DATOS_TIMBRADO)
                            {
                                semaforo.noError = 2;
                            }
                            else if (smf.tipoTimbrado == SemaforoTimbradoPac.TipoTimbrado.CANCELANDO_TIMBRADOS)
                            {
                                semaforo.noError = 4;

                            }
                            else if (smf.tipoTimbrado == SemaforoTimbradoPac.TipoTimbrado.ABRIENDO_PERIODO)
                            {
                                semaforo.noError = 5;
                            }
                            else
                            {
                                semaforo.noError = 3;
                            }

                        } else if (entity.tipoTimbrado == SemaforoTimbradoPac.TipoTimbrado.CANCELANDO_TIMBRADOS) {
                            if (smf.tipoTimbrado == SemaforoTimbradoPac.TipoTimbrado.GENERANDO_DATOS_TIMBRADO)
                            {
                                semaforo.noError = 2;
                            }
                            else if (smf.tipoTimbrado == SemaforoTimbradoPac.TipoTimbrado.TIMBRANDO)
                            {
                                semaforo.noError = 3;

                            }
                            else if (smf.tipoTimbrado == SemaforoTimbradoPac.TipoTimbrado.ABRIENDO_PERIODO)
                            {
                                semaforo.noError = 5;
                            }
                            else
                            {
                                semaforo.noError = 4;
                            }
                        } else {
                            semaforo.noError = 1;
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
        private Mensaje addValoresSemaforo(Mensaje mensaje, SemaforoTimbradoPac entity)
        {
            entity = getSession().Set<SemaforoTimbradoPac>().Add(entity);
            mensaje.resultado = entity;
            mensaje.noError = entity == null ? 1 : 0;
            
            return mensaje;
        }
        public Mensaje eliminar(SemaforoTimbradoPac entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                //var sentencia = (from g in getSession().Set<Genero>()
                //                 where g.id == entity.id
                //                 select g).FirstOrDefault();
                getSession().Set<SemaforoTimbradoPac>().Attach(entity);
                getSession().Set<SemaforoTimbradoPac>().Remove(entity);
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
    }
}