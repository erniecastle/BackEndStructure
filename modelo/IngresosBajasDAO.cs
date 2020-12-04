/**
* @author: Daniel Ruelas 
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Clase IngresosReingresosBajasDAO para llamados a metodos de Entity
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
using Exitosw.Payroll.Core.campos;
using Exitosw.Payroll.Core.genericos.campos;
using System.Globalization;

namespace Exitosw.Payroll.Core.modelo
{
    public class IngresosBajasDAO : GenericRepository<IngresosBajas>, IngresosBajasDAOIF
    {

        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        private DateTime fechasistema = DateTime.Now;
        public Mensaje agregar(IngresosBajas entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<IngresosBajas>().Add(entity);
                getSession().SaveChanges();
                mensajeResultado.resultado = entity.id;
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
        public Mensaje actualizar(IngresosBajas entity, DBContextAdapter dbContext)
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
                getSession().Set<IngresosBajas>().AddOrUpdate(entity);
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

        public Mensaje eliminar(IngresosBajas entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                //var sentencia = (from g in getSession().Set<Genero>()
                //                 where g.id == entity.id
                //                 select g).FirstOrDefault();
                getSession().Set<IngresosBajas>().Attach(entity);
                getSession().Set<IngresosBajas>().Remove(entity);
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

        public Mensaje consultaPorFiltrosIngReingresosBajas(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            List<IngresosBajas> ingReingresosBajas = new List<IngresosBajas>();
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
                        campo.campo = "IngReingresosBajas." + item.Key.ToString();
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorFiltrosReIngresos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje consultaPorRangosIngReingresosBajas(Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            List<IngresosBajas> ingReingresosBajas = new List<IngresosBajas>();
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
        //    bool existe = true;
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

        public Mensaje getAllIngresosReingresosBajas(DBContextAdapter dbContext)
        {
            List<IngresosBajas> ingReingresosBajas = new List<IngresosBajas>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                ingReingresosBajas = (from ing in getSession().Set<IngresosBajas>() orderby ing.id select ing).ToList();
                mensajeResultado.resultado = ingReingresosBajas;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getIngresosReingresosBajasAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getIngresosBajasPorEmpRegPatyRazonSoc(decimal idEmpleado, decimal idRegPat, decimal idRazonSocial, DBContextAdapter dbContext)
        {
            List<IngresosBajas> listIngresosReingresosBajas;
            //IngReingresosBajas ingresosReingresosBajas = null;
            try
            {

                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var query = from ingre in getSession().Set<IngresosBajas>()
                            where ingre.empleados.id == idEmpleado
                            select ingre;
                if (idRegPat > -1)
                {
                    query = from subq in query
                            where subq.registroPatronal.id == idRegPat
                            select subq;
                }
                listIngresosReingresosBajas = (from sub3 in query
                                               where sub3.razonesSociales.id == idRazonSocial
                                               select sub3).ToList();
                //else
                //{

                //    listIngresosReingresosBajas = (from ingre in getSession().Set<IngReingresosBajas>()
                //                                   where ingre.empleados.clave == claveEmpleado
                //                                   && ingre.razonesSociales.clave == claveRazonSocial
                //                                   select ingre).ToList();
                //}
                if (listIngresosReingresosBajas != null)
                {
                    DateTime fechaActual = DateTime.Now;
                    fechaActual = quitaHrsDeFecha(fechaActual);
                    fechaActual.AddHours(0);
                    fechaActual.AddMilliseconds(0);
                    fechaActual.AddSeconds(0);
                    fechaActual.AddMinutes(0);
                    var ingresosReingresosBajas = listIngresosReingresosBajas.Where(p => p.fechaBaja >= fechaActual)
                        .Select(p => new { p.id }
                        ).SingleOrDefault();

                    //for (int i = 0; i < listIngresosReingresosBajas.Count; i++)
                    //{
                    //    if (DateTime.Compare((DateTime)listIngresosReingresosBajas[i].fechaBaja, fechaActual) <= 0)
                    //    {
                    //        ingresosReingresosBajas = listIngresosReingresosBajas[i];
                    //        for (int j = 0; j < listIngresosReingresosBajas.Count; j++)
                    //        {
                    //            //if (listIngresosReingresosBajas[j].tipoMovimiento == IngReingresosBajas.TipoMovimiento.R)
                    //            //{
                    //            //    if (DateTime.Compare((DateTime)listIngresosReingresosBajas[j].fechaBaja, (DateTime)listIngresosReingresosBajas[i].fechaBaja) >= 0)
                    //            //    {
                    //            //        ingresosReingresosBajas = listIngresosReingresosBajas[j];

                    //            //    }
                    //            //}
                    //        }
                    //    }
                    //    else
                    //    {
                    //        ingresosReingresosBajas = listIngresosReingresosBajas[i];
                    //    }

                    //}
                    mensajeResultado.resultado = ingresosReingresosBajas;
                }

                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getIngresosBajasPorEmpRegPatyRazonSoc()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        private DateTime quitaHrsDeFecha(DateTime fecha)
        {
            Calendar myCal = CultureInfo.InvariantCulture.Calendar;
            fecha = myCal.AddHours(fecha, 0);
            fecha = myCal.AddMinutes(fecha, 0);
            fecha = myCal.AddSeconds(fecha, 0);
            fecha = myCal.AddMilliseconds(fecha, 0);

            return fecha;
        }

        public Mensaje getIngresosReingresosBajasPorIdEmpleado(Empleados empleados, DBContextAdapter dbContext)
        {
            DateTime? fechaEmpleado;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                fechaEmpleado = (from ing in getSession().Set<IngresosBajas>()
                                 where ing.empleados == empleados
                                 select ing.fechaIngreso).SingleOrDefault();
                mensajeResultado.resultado = fechaEmpleado;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getIngresosReingresosBajasPorIdEmpleado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getIngresosReingresosBajasPorRazonSocial(RazonesSociales razonSocial, DBContextAdapter dbContext)
        {
            List<IngresosBajas> ingReingresosBajas = new List<IngresosBajas>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                ingReingresosBajas = (from ing in getSession().Set<IngresosBajas>()
                                      where ing.razonesSociales == razonSocial
                                      select ing).ToList();
                mensajeResultado.resultado = ingReingresosBajas;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getIngresosReingresosBajasPorRazonSocial()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getIngresosReingresosBajasPorRegPatronal(RegistroPatronal registroPatronal, DBContextAdapter dbContext)
        {
            List<IngresosBajas> ingReingresosBajas = new List<IngresosBajas>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                ingReingresosBajas = (from ing in getSession().Set<IngresosBajas>()
                                      where ing.registroPatronal == registroPatronal
                                      select ing).ToList();
                mensajeResultado.resultado = ingReingresosBajas;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getIngresosReingresosBajasPorRegPatronal()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorClaveEmpleado(string claveEmp, string claveRegPatronal, string claveRazonSocial, DBContextAdapter dbContext)
        {
            IngresosBajas ingReingresosBajas;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                ingReingresosBajas = (from ingre in getSession().Set<IngresosBajas>()
                                      where ingre.empleados.clave == claveEmp &&
                                      ingre.razonesSociales.clave == claveRazonSocial &&
                                      ingre.registroPatronal.clave == claveRegPatronal &&
                                      ingre.fechaBaja >= fechasistema
                                      select ingre).SingleOrDefault();
                mensajeResultado.resultado = ingReingresosBajas;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorClaveEmpleado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorEmpleadoActivo(string claveEmpleado, string claveRegPatronal, string claveRazonSocial, DBContextAdapter dbContext)
        {
            IngresosBajas ingReingresosBajas;
            try
            {

                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                ingReingresosBajas = (from ingre in getSession().Set<IngresosBajas>()
                                      where ingre.empleados.clave == claveEmpleado &&
                                      ingre.registroPatronal.clave == claveRegPatronal &&
                                      ingre.razonesSociales.clave == claveRazonSocial &&
                                      ingre.fechaBaja >= fechasistema
                                      select ingre).SingleOrDefault();
                mensajeResultado.resultado = ingReingresosBajas;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorEmpleadoActivo()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorEmpleadoInactivo(string claveEmpleado, string claveRegPatronal, string claveRazonSocial, DateTime fechaActual, DBContextAdapter dbContext)
        {
            List<IngresosBajas> listIngresosReingresosBajas;
            IngresosBajas ingReingresosBajas = new IngresosBajas();
            try
            {

                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listIngresosReingresosBajas = (from ingre in getSession().Set<IngresosBajas>()
                                               where ingre.empleados.clave == claveEmpleado &&
                                               ingre.registroPatronal.clave == claveRegPatronal &&
                                               ingre.razonesSociales.clave == claveRazonSocial
                                               orderby ingre.fechaBaja
                                               select ingre).ToList();
                for (int i = 0; i < listIngresosReingresosBajas.Count; i++)
                {
                    if (DateTime.Compare((DateTime)listIngresosReingresosBajas[i].fechaBaja, fechasistema) <= 0)
                    {
                        ingReingresosBajas = listIngresosReingresosBajas[i];
                        for (int j = 0; j < listIngresosReingresosBajas.Count; j++)
                        {
                            //if (listIngresosReingresosBajas[j].tipoMovimiento == IngReingresosBajas.TipoMovimiento.R)
                            //{
                            //    if (DateTime.Compare((DateTime)listIngresosReingresosBajas[j].fechaBaja, (DateTime)listIngresosReingresosBajas[i].fechaBaja) >= 0)
                            //    {
                            //        if (DateTime.Compare((DateTime)listIngresosReingresosBajas[j].fechaBaja, fechasistema) >= 0)
                            //        {
                            //            ingReingresosBajas = null;
                            //        }
                            //        else
                            //        {
                            //            ingReingresosBajas = listIngresosReingresosBajas[i];
                            //        }

                            //    }
                            //}
                        }
                    }
                }
                mensajeResultado.resultado = ingReingresosBajas;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorEmpleadoInactivo()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorReferenciaPlazaEmpActivo(string claveReferenciaPlazaEmp, string claveRegPatronal, string claveRazonSocial, DBContextAdapter dbContext)
        {
            IngresosBajas ingReingresosBajas;
            try
            {

                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                ingReingresosBajas = (from ingre in getSession().Set<IngresosBajas>()
                                      where /*ingre.plazasPorEmpleado.referencia == claveReferenciaPlazaEmp &&*/
                                      ingre.registroPatronal.clave == claveRegPatronal &&
                                      ingre.razonesSociales.clave == claveRazonSocial &&
                                      ingre.fechaBaja >= fechasistema
                                      select ingre).SingleOrDefault();
                mensajeResultado.resultado = ingReingresosBajas;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorReferenciaPlazaEmpActivo()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorReferenciaPlazaEmpInactiva(string claveReferenciaPlazaEmp, string claveRegPatronal, string claveRazonSocial, DBContextAdapter dbContext)
        {
            IngresosBajas ingReingresosBajas;
            try
            {

                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                ingReingresosBajas = (from ingre in getSession().Set<IngresosBajas>()
                                      where /*ingre.plazasPorEmpleado.referencia == claveReferenciaPlazaEmp &&*/
                                      ingre.registroPatronal.clave == claveRegPatronal &&
                                      ingre.razonesSociales.clave == claveRazonSocial &&
                                      ingre.fechaBaja < fechasistema
                                      select ingre).SingleOrDefault();
                mensajeResultado.resultado = ingReingresosBajas;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorReferenciaPlazaEmpInactiva()1_Error: ").Append(ex));
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

        public Mensaje getIngresosReingresosBajasPorIdEmpleadoJS(decimal empleados, DBContextAdapter dbContext)
        {
            DateTime? fechaEmpleado;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                fechaEmpleado = (from ing in getSession().Set<IngresosBajas>()
                                 where ing.empleados.id == empleados
                                 select ing).Max(p => p.fechaIngreso);
                mensajeResultado.resultado = fechaEmpleado;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getIngresosReingresosBajasPorIdEmpleado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
    }
}