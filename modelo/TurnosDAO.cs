/**
* @author: Daniel Ruelas 
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Clase TurnosDAO para llamados a metodos de Entity
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
using Exitosw.Payroll.Core.genericos.campos;
using Exitosw.Payroll.Entity.util;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public class TurnosDAO : GenericRepository<Turnos>, TurnosDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");

        public Mensaje agregar(Turnos entity, DBContextAdapter dbContext)
        {
            Turnos turnos;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                turnos = getSession().Set<Turnos>().Add(entity);
                if (turnos.turnosHorariosFijos_turnos != null)
                {
                    for (int i = 0; i < turnos.turnosHorariosFijos_turnos.Count; i++)
                    {
                        turnos.turnosHorariosFijos_turnos[i].turnos = turnos;
                        getSession().Set<TurnosHorariosFijos>().AddOrUpdate(turnos.turnosHorariosFijos_turnos[i]);
                        getSession().SaveChanges();
                    }
                }
                getSession().SaveChanges();
                mensajeResultado.resultado = turnos;
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
        public Mensaje modificar(Turnos entity, DBContextAdapter dbContext)
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
                getSession().Set<Turnos>().AddOrUpdate(entity);
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

        public Mensaje eliminar(Turnos entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                //var sentencia = (from g in getSession().Set<Genero>()
                //                 where g.id == entity.id
                //                 select g).FirstOrDefault();
                getSession().Set<Turnos>().Attach(entity);
                getSession().Set<Turnos>().Remove(entity);
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

        public Mensaje getAllTurnos(string claveRazonesSocial, DBContextAdapter dbContext)
        {
            //List<Turnos> turnos = new List<Turnos>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
               var turnos = (from t in getSession().Set<Turnos>()
                          where t.razonesSociales.clave == claveRazonesSocial
                          select new {
                              t.clave,
                              t.descripcion,
                              t.diasJornada,
                              t.horaJornada,
                              t.id,
                              t.Jornada_ID,
                              t.primerDiaSemana,
                              t.razonesSociales_ID,
                              t.tipoDeJornadaIMSS,
                              t.tipoDeTurno,
                              t.topeHorasDoblesSemanal
                          }).ToList();
                mensajeResultado.resultado = turnos;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getTurnosAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorClaveTurnos(string clave, DBContextAdapter dbContext)
        {
            Turnos turno;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                turno = (from t in getSession().Set<Turnos>()
                         where t.clave == clave
                         select t).SingleOrDefault();
                mensajeResultado.resultado = turno;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getTurnosAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje UpdateTurnos(Turnos entity, object[] eliminados, DBContextAdapter dbContext)
        {
            bool exito = true;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (eliminados.Length > 0)
                {
                    ///exito = deleteListQueryEn(getSession(), "TurnosHorariosFijos", "id", eliminados);
                    Mensaje mensaje = deleteListQuery("TurnosHorariosFijos", new CamposWhere("TurnosHorariosFijos.id", eliminados, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                    exito = (bool)mensaje.resultado;
                }

                if (exito)
                {
                    getSession().Set<Turnos>().AddOrUpdate(entity);
                    mensajeResultado.resultado = exito;
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("UpdateTurnos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorIdTurnos(decimal? id, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var turnos = (from t in getSession().Set<Turnos>()
                              where t.id == id
                              select new
                              {
                                  t.clave,
                                  t.descripcion,
                                  t.diasJornada,
                                  t.horaJornada,
                                  t.id,
                                  t.Jornada_ID,
                                  t.primerDiaSemana,
                                  t.razonesSociales_ID,
                                  t.tipoDeJornadaIMSS,
                                  t.tipoDeTurno,
                                  t.topeHorasDoblesSemanal,
                                  turnosHorariosFijos=t.turnosHorariosFijos_turnos.Select(a=>new {
                                      a.diaSemana,
                                      a.Horario_ID,
                                      a.id,
                                      a.ordenDia,
                                      a.razonesSociales_ID,
                                      a.statusDia,
                                      a.turnos_ID
                                  })
                              }).SingleOrDefault();
                mensajeResultado.resultado = turnos;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorIdTurnos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        //private boolean deleteListQueryEn(Session session, String tabla, String campo, Object[] valores)
        //{
        //    boolean continuar = true;
        //    try
        //    {
        //        consulta = new StringBuilder("delete ");
        //        consulta.append(tabla).append(" where ").append(campo).append(" in(:valores)");
        //        q = session.createQuery(consulta.toString());
        //        q.setParameterList("valores", valores);
        //        q.executeUpdate();
        //    }
        //    catch (HibernateException ex)
        //    {
        //        System.err.println(concatena.delete(0, concatena.length()).append(msgError).append("deleteListQueryEn()1_Error: ").append(ex));
        //        continuar = false;
        //    }
        //    return continuar;
        //}
    }
}