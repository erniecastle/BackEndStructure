/**
* @author: Daniel Ruelas 
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Clase PlazasPorEmpleadosMovDAO para llamados a metodos de Entity
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
using Exitosw.Payroll.Entity.entidad.cfdi;
using Exitosw.Payroll.Core.genericos.campos;
using System.Data.Entity;
using Exitosw.Payroll.Core.campos;
using System.Data.Entity.Validation;

namespace Exitosw.Payroll.Core.modelo
{
    public class PlazasPorEmpleadosMovDAO : GenericRepository<PlazasPorEmpleadosMov>, PlazasPorEmpleadosMovDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");

        public Mensaje agregar(PlazasPorEmpleadosMov entity, SalariosIntegrados salariosIntegrados, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                IngresosBajas ing = null;
                DateTime fechaSistema = getFechaDelSistema();// fechaSistema.AddDays();
                decimal idRazonSocial = entity.plazasPorEmpleado.razonesSociales_ID,
                    idRegistroPatronal = entity.plazasPorEmpleado.registroPatronal_ID,
                    idEmpleado = entity.plazasPorEmpleado.empleados_ID.GetValueOrDefault();

                //Búsqueda de Ingresos Bajas
                ing = (from o in dbContext.context.Set<IngresosBajas>()
                       where o.razonesSociales_ID == idRazonSocial &&
                       o.registroPatronal_ID == idRegistroPatronal &&
                       o.empleados_ID == idEmpleado &&
                       o.fechaIngreso == (from max in dbContext.context.Set<IngresosBajas>()
                                          where max.id == o.id
                                       && max.razonesSociales_ID == idRazonSocial &&
                       max.registroPatronal_ID == idRegistroPatronal &&
                       max.empleados_ID == idEmpleado
                                          select new { max.fechaIngreso }).Max(p => p.fechaIngreso)
                                          && o.fechaBaja >= fechaSistema.Date
                       select o).SingleOrDefault();

                //Ingresos y Bajas
                if (ing == null)
                {
                    IngresosBajas ingresosBajas = new IngresosBajas();
                    ingresosBajas.fechaBaja = entity.plazasPorEmpleado.fechaFinal;
                    ingresosBajas.fechaIngreso = entity.fechaIMSS;
                    //New Campo
                    ingresosBajas.fechaPrestaciones = entity.fechaInicial;
                    //
                    ingresosBajas.empleados_ID = entity.plazasPorEmpleado.empleados_ID;
                    ingresosBajas.plazasPorEmpleado = new List<PlazasPorEmpleado> { entity.plazasPorEmpleado };
                    ingresosBajas.registroPatronal_ID = entity.plazasPorEmpleado.registroPatronal_ID;
                    ingresosBajas.razonesSociales_ID = entity.plazasPorEmpleado.razonesSociales_ID;
                    ingresosBajas.aplicar = false;
                    ingresosBajas.causaBaja = null;
                    ingresosBajas.tipoSeparacion = "";
                    ingresosBajas.fechaCalculo = null;
                    ingresosBajas.calculado = false;
                    ingresosBajas.complementaria = false;
                    ingresosBajas.fechaComplementaria = null;
                    ingresosBajas.previa = false;
                    ingresosBajas.procesado = false;
                    getSession().Set<IngresosBajas>().Add(ingresosBajas);
                }
                else
                {
                    entity.plazasPorEmpleado.ingresosBajas_ID = ing.id;
                    if (ing.fechaBaja < entity.plazasPorEmpleado.fechaFinal ||
                        ing.fechaBaja == entity.fechaIMSS)
                    {
                        ing.fechaBaja = entity.plazasPorEmpleado.fechaFinal;
                    }

                    //if (ing.fechaBaja == entity.fechaIMSS)
                    //{
                    //    ing.fechaBaja = entity.plazasPorEmpleado.fechaFinal;
                    //}
                }
                if (entity.isPrm)
                {
                    entity.plazasPorEmpleado = null;
                }
                getSession().Set<PlazasPorEmpleadosMov>().Add(entity);
                getSession().SaveChanges();

                if (entity.isPrm)
                {
                    mensajeResultado.resultado = true;
                }
                else
                {
                    var plazasPorEmpleadosMov = new PlazasPorEmpleadosMov();
                    plazasPorEmpleadosMov.id = entity.id;
                    plazasPorEmpleadosMov.plazasPorEmpleado_ID = entity.plazasPorEmpleado.id;
                    mensajeResultado.resultado = plazasPorEmpleadosMov;
                }

                mensajeResultado.noError = 0;
                mensajeResultado.error = "";

                if (salariosIntegrados != null)
                {
                    //Leer el Salario integrado traerse el ID y sustiruirlo por los nuevo valores calculados
                    getSession().Set<SalariosIntegrados>().Add(salariosIntegrados);
                    getSession().SaveChanges();
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
            //catch (DbEntityValidationException e)
            //{
            //    foreach (var eve in e.EntityValidationErrors)
            //    {
            //        System.Diagnostics.Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
            //            eve.Entry.Entity.GetType().Name, eve.Entry.State);

            //        //Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
            //        //    eve.Entry.Entity.GetType().Name, eve.Entry.State);

            //        foreach (var ve in eve.ValidationErrors)
            //        {
            //            System.Diagnostics.Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
            //                ve.PropertyName, ve.ErrorMessage);

            //            //Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
            //            //     ve.PropertyName, ve.ErrorMessage);
            //        }
            //    }
            //    throw;
            //}
            return mensajeResultado;
        }

        public Mensaje actualizar(PlazasPorEmpleadosMov entity, SalariosIntegrados salariosIntegrados, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                PlazasPorEmpleadosMov plazasPorEmpleadosMovAnt = null;
                var actualiza = false;

                if (entity.isPrm)//Se actualiza una promoción
                {
                    actualiza = true;
                }
                else
                {
                    var query = from o in dbContext.context.Set<PlazasPorEmpleadosMov>()
                                where o.plazasPorEmpleado_ID == entity.plazasPorEmpleado_ID
                                && o.plazasPorEmpleado.razonesSociales_ID == entity.plazasPorEmpleado.razonesSociales_ID
                                orderby o.id descending
                                select o;

                    plazasPorEmpleadosMovAnt = query.Skip(1).Take(1).FirstOrDefault();

                    if (plazasPorEmpleadosMovAnt == null)
                    {
                        actualiza = true;

                    }
                    else
                    {
                        if (entity.fechaIMSS >= plazasPorEmpleadosMovAnt.fechaIMSS)
                        {
                            if (entity.fechaIMSS > plazasPorEmpleadosMovAnt.plazasPorEmpleado.ingresosBajas.fechaBaja)
                            {
                                mensajeResultado.resultado = 1;
                            }
                            else if (entity.fechaIMSS == plazasPorEmpleadosMovAnt.plazasPorEmpleado.ingresosBajas.fechaBaja)
                            {
                                actualiza = true;
                                IngresosBajas ingresosBajas = entity.plazasPorEmpleado.ingresosBajas;
                                ingresosBajas.fechaBaja = entity.plazasPorEmpleado.fechaFinal;
                                getSession().Set<IngresosBajas>().AddOrUpdate(ingresosBajas);
                            }
                        }
                        else
                        {
                            mensajeResultado.resultado = 0;
                        }
                    }
                }
                if (actualiza)
                {
                    getSession().Set<PlazasPorEmpleadosMov>().AddOrUpdate(entity);

                    if (!entity.isPrm)
                    {
                        getSession().Set<PlazasPorEmpleado>().AddOrUpdate(entity.plazasPorEmpleado);
                        getSession().Set<IngresosBajas>().AddOrUpdate(entity.plazasPorEmpleado.ingresosBajas);
                    }

                    getSession().SaveChanges();

                }

                if (salariosIntegrados != null)
                {

                    SalariosIntegrados salariosIntegradoAnt = null;
                    salariosIntegradoAnt = (from si in dbContext.context.Set<SalariosIntegrados>()
                                            where si.fecha < entity.fechaIMSS && si.empleados.id == entity.plazasPorEmpleado.empleados_ID
                                            orderby si.fecha descending
                                            select si).Take(1).SingleOrDefault();

                    if (salariosIntegradoAnt == null)
                    {
                        getSession().Set<SalariosIntegrados>().AddOrUpdate(salariosIntegrados);
                    }
                    else
                    {
                        SalariosIntegrados salariosIntegradoAFecha = null;
                        salariosIntegradoAFecha = (from si in dbContext.context.Set<SalariosIntegrados>()
                                                   where si.fecha == entity.fechaIMSS && si.empleados.id == entity.plazasPorEmpleado.empleados_ID
                                                   orderby si.fecha descending
                                                   select si).Take(1).SingleOrDefault();

                        if (salariosIntegradoAnt.salarioDiarioIntegrado != salariosIntegrados.salarioDiarioIntegrado)
                        {
                            if (salariosIntegradoAFecha == null)
                            {
                                //Add Salario
                                getSession().Set<SalariosIntegrados>().AddOrUpdate(salariosIntegrados);
                            }
                            else
                            {
                                //Actualizar Salario
                                salariosIntegradoAFecha.factorIntegracion = salariosIntegrados.factorIntegracion;
                                salariosIntegradoAFecha.salarioDiarioFijo = salariosIntegrados.salarioDiarioFijo;
                                salariosIntegradoAFecha.salarioDiarioIntegrado = salariosIntegrados.salarioDiarioIntegrado;
                                salariosIntegradoAFecha.salarioDiarioVariable = salariosIntegrados.salarioDiarioVariable;
                                salariosIntegradoAFecha.tipoDeSalario = salariosIntegrados.tipoDeSalario;
                                getSession().Set<SalariosIntegrados>().AddOrUpdate(salariosIntegradoAFecha);
                            }
                        }
                        else
                        {
                            if (salariosIntegradoAFecha != null)
                            {   //Delete Salario
                                getSession().Set<SalariosIntegrados>().Attach(salariosIntegradoAFecha);
                                getSession().Set<SalariosIntegrados>().Remove(salariosIntegradoAFecha);
                            }
                        }
                    }

                    getSession().SaveChanges();
                }
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

        public Mensaje eliminar(PlazasPorEmpleadosMov entity, DBContextAdapter dbContext)
        {
            try
            {
                //List<StatusTimbrado> status = new List<StatusTimbrado>();
                //status.Add(StatusTimbrado.NINGUNO);
                //status.Add(StatusTimbrado.TIMBRADO);
                // int countTimbres;
                //PlazasPorEmpleado plaza = entity.plazasPorEmpleado;
                //countTimbres = (from o in getSession().Set<CFDIEmpleado>()
                //                where o.razonesSociales.clave == plaza.razonesSociales.clave &&
                //                o.plazasPorEmpleadosMov.plazasPorEmpleado.id == plaza.id &&
                //                (new List<StatusTimbrado> { StatusTimbrado.NINGUNO, StatusTimbrado.TIMBRADO }).Contains(o.cfdiRecibo.statusTimbrado)
                //                select o).Count();
                //if (countTimbres > 0)
                //{//Verificamos si esta plaza tiene timbres cancelados o timbrados
                //    mensajeResultado.resultado = "timbre";
                //    getSession().Database.CurrentTransaction.Rollback();
                //}
                //else
                //{
                //}//pendiente de terminar lo de timbrado

                int plazaPorEmMovs = 0;
                int uniquePlazaEm = 0;
                inicializaVariableMensaje();
                bool elimina = true;
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (elimina)
                {
                    //Ingreso unico por plaza
                    plazaPorEmMovs = (from o in getSession().Set<PlazasPorEmpleadosMov>()
                                      where o.plazasPorEmpleado_ID == entity.plazasPorEmpleado_ID
                                      select o).Count();

                    if (plazaPorEmMovs == 1)
                    {
                        if (entity.isPrm)
                        {
                            elimina = false;//Es una alta notificar
                            mensajeResultado.resultado = 0;//Tiene promociones
                        }
                        else
                        {
                            elimina = true;//No tiene pomociones
                        }
                    }
                    else if (plazaPorEmMovs > 1)
                    {
                        elimina = false;
                        mensajeResultado.resultado = 0;//Tiene promociones
                        if (entity.isPrm)
                        {
                            elimina = true;
                        }
                    }
                }

                if (elimina)
                {
                    //Delete Salario from Promo
                    SalariosIntegrados salarioDePromo = null;
                    salarioDePromo = (from si in dbContext.context.Set<SalariosIntegrados>()
                                      where si.fecha == entity.fechaIMSS && si.empleados.id == entity.plazasPorEmpleado.empleados_ID
                                      && si.registroPatronal_ID == entity.plazasPorEmpleado.registroPatronal_ID
                                      select si).FirstOrDefault();

                    if (entity.isPrm)
                    {

                        if (salarioDePromo != null)
                        {
                            getSession().Set<SalariosIntegrados>().Attach(salarioDePromo);
                            getSession().Set<SalariosIntegrados>().Remove(salarioDePromo);
                        }
                        var plazasPorEmpleadosMov = new PlazasPorEmpleadosMov { id = entity.id };
                        getSession().Set<PlazasPorEmpleadosMov>().Attach(plazasPorEmpleadosMov);
                        getSession().Set<PlazasPorEmpleadosMov>().Remove(plazasPorEmpleadosMov);
                        getSession().SaveChanges();
                    }
                    else
                    {
                        uniquePlazaEm = (from o in dbContext.context.Set<PlazasPorEmpleado>()
                                         where o.ingresosBajas_ID == entity.plazasPorEmpleado.ingresosBajas.id
                                         select o).Count();

                        if (salarioDePromo != null)
                        {
                            getSession().Set<SalariosIntegrados>().Attach(salarioDePromo);
                            getSession().Set<SalariosIntegrados>().Remove(salarioDePromo);
                        }

                        var plazasPorEmpleadosMov = new PlazasPorEmpleadosMov { id = entity.id };
                        getSession().Set<PlazasPorEmpleadosMov>().Attach(plazasPorEmpleadosMov);
                        getSession().Set<PlazasPorEmpleadosMov>().Remove(plazasPorEmpleadosMov);
                        getSession().SaveChanges();

                        var plazasPorEmpleado = new PlazasPorEmpleado { id = entity.plazasPorEmpleado.id };
                        getSession().Set<PlazasPorEmpleado>().Attach(plazasPorEmpleado);
                        getSession().Set<PlazasPorEmpleado>().Remove(plazasPorEmpleado);
                        getSession().SaveChanges();

                        if (uniquePlazaEm == 1)
                        {
                            var ingresosBajas = new IngresosBajas { id = entity.plazasPorEmpleado.ingresosBajas.id };
                            getSession().Set<IngresosBajas>().Attach(ingresosBajas);
                            getSession().Set<IngresosBajas>().Remove(ingresosBajas);
                            getSession().SaveChanges();
                        }
                    }

                    mensajeResultado.resultado = true;
                    mensajeResultado.noError = 0;
                }

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

        public Mensaje agregarListaPlazasPorEmpleadosMovs(List<PlazasPorEmpleadosMov> entitys, int rango, DBContextAdapter dbContext)
        {
            List<PlazasPorEmpleadosMov> plazasPorEmpleadosMov = new List<PlazasPorEmpleadosMov>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                int i;
                for (i = 0; i < entitys.Count; i++)
                {
                    if (entitys[i].id == 0)
                    {
                        plazasPorEmpleadosMov.Add(getSession().Set<PlazasPorEmpleadosMov>().Add(entitys[i]));
                    }
                    else
                    {
                        getSession().Set<PlazasPorEmpleadosMov>().AddOrUpdate(entitys[i]);
                    }
                }

                getSession().SaveChanges();
                mensajeResultado.resultado = plazasPorEmpleadosMov;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("agregarListaPlazasPorEmpleadosMovs()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje consultaPorFiltrosPlazasPorEmpleadosMov(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            List<PlazasPorEmpleadosMov> plazasPorEmpleadosMov = null;
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
                        campo.campo = "PlazasPorEmpleadosMov." + item.Key.ToString();
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorFiltrosEmpleado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje consultaPorRangosPlazasPorEmpleadosMov(Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            List<PlazasPorEmpleadosMov> plazasPorEmpleadosMov = null;
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

        public Mensaje deleteListQuerys(string tabla, string campo, object[] valores, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                deleteListQuery(tabla, new CamposWhere(tabla + "." + campo, valores, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("deleteListQuerys()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje eliminarMovimientosPorPlaza(PlazasPorEmpleadosMov entity, List<int> movimientos, DBContextAdapter dbContext)
        {
            throw new NotImplementedException();
        }

        public Mensaje eliminarPlazasMovimientos(PlazasPorEmpleadosMov entity, DBContextAdapter dbContext)
        {
            throw new NotImplementedException();
        }

        public Mensaje getCantidadPlazasPorEmpleado(string claveEmpleado, string claveRazonSocial, DBContextAdapter dbContext)
        {
            int cantidad;
            try
            {
                DateTime fechaActual = DateTime.Now;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                cantidad = (from pm in
                                    (from pm in getSession().Set<PlazasPorEmpleadosMov>()
                                     where
                                           (from m in getSession().Set<PlazasPorEmpleadosMov>()
                                            where
                                             m.plazasPorEmpleado.empleados.clave == claveEmpleado &&
                                             m.plazasPorEmpleado.razonesSociales.clave == claveRazonSocial
                                            group new { m.plazasPorEmpleado, m } by new
                                            {
                                                m.plazasPorEmpleado.referencia
                                            } into g
                                            select new
                                            {
                                                Column1 = (decimal?)g.Max(p => p.m.id)
                                            }).Contains(new { Column1 = (System.Decimal?)pm.id })
                                     select new
                                     {
                                         Dummy = "x"
                                     })
                            group pm by new { pm.Dummy } into g
                            select g
                           ).Count();
                mensajeResultado.resultado = cantidad;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getCantidadPlazasPorEmpleado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getEmpleadosManejaPagoPorHoras(string claveTipoNomina, string claveRazonSocial, DateTime fechaInicial, DateTime fechaFinal, DBContextAdapter dbContext)
        {
            List<PlazasPorEmpleadosMov> plazasPorEmpleadosMov = null;
            try
            {
                DateTime fechaActual = DateTime.Now;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                plazasPorEmpleadosMov = (from o in getSession().Set<PlazasPorEmpleadosMov>()
                                         where
                                               (from pem in getSession().Set<PlazasPorEmpleadosMov>()
                                                join pu in getSession().Set<Puestos>() on pem.puestos.id equals pu.id into pu_join
                                                from pu in pu_join.DefaultIfEmpty()
                                                join cp in getSession().Set<CategoriasPuestos>() on pu.categoriasPuestos.id equals cp.id into cp_join
                                                from cp in cp_join.DefaultIfEmpty()
                                                where
                                                 pem.plazasPorEmpleado.razonesSociales.clave == claveRazonSocial &&
                                                 pem.tipoNomina.clave == claveTipoNomina &&
                                                 cp.pagarPorHoras == true &&
                                                 (pem.fechaInicial <= fechaInicial ||
                                                 pem.fechaInicial >= fechaInicial && pem.fechaInicial <= fechaFinal) &&
                                                 (pem.plazasPorEmpleado.fechaFinal >= fechaFinal ||
                                                 pem.plazasPorEmpleado.fechaFinal >= fechaInicial && pem.plazasPorEmpleado.fechaFinal <= fechaFinal)
                                                group new { pem.plazasPorEmpleado, pem } by new
                                                {
                                                    pem.plazasPorEmpleado.referencia
                                                } into g
                                                select new
                                                {
                                                    Column1 = g.Max(p => p.pem.id)
                                                }).Contains(new { Column1 = o.id })
                                         orderby
                                           o.plazasPorEmpleado.empleados.clave
                                         select o).ToList();
                mensajeResultado.resultado = plazasPorEmpleadosMov;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getEmpleadosManejaPagoPorHoras()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPlazasPorEmpleadosMovAnterior(decimal idPlazaPorEmpleMov, string referencia, string claveRazonesSociales, int result, DBContextAdapter dbContext)
        {
            List<PlazasPorEmpleadosMov> plazasPorEmpleadosMov = null;
            try
            {
                DateTime fechaActual = DateTime.Now;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var query = from o in getSession().Set<PlazasPorEmpleadosMov>()
                            where o.plazasPorEmpleado.referencia == referencia
                            && o.id < idPlazaPorEmpleMov && o.plazasPorEmpleado.razonesSociales.clave == claveRazonesSociales
                            orderby o.id descending
                            select o;
                if (result > 0)
                {
                    plazasPorEmpleadosMov = query.Take(result).ToList();
                }
                else
                {
                    plazasPorEmpleadosMov = query.ToList();
                }
                mensajeResultado.resultado = plazasPorEmpleadosMov;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPlazasPorEmpleadosMovAnterior()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPlazasPorEmpleadosMovMaxPorClave(string clave, string razonSocial, DBContextAdapter dbContext)
        {
            PlazasPorEmpleadosMov plazaPorEmpleadoMov;
            try
            {
                DateTime fechaActual = DateTime.Now;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                plazaPorEmpleadoMov = (from pm in getSession().Set<PlazasPorEmpleadosMov>()
                                       where
                                         pm.plazasPorEmpleado.referencia == clave &&
                                         pm.plazasPorEmpleado.razonesSociales.clave == razonSocial &&
                                         pm.fechaInicial ==
                                           (from o in getSession().Set<PlazasPorEmpleadosMov>()
                                            where
                                             o.plazasPorEmpleado.referencia == clave &&
                                             o.plazasPorEmpleado.razonesSociales.clave == razonSocial &&
                                             o.fechaInicial <= fechaActual
                                            select new
                                            {
                                                o.fechaInicial
                                            }).Max(p => p.fechaInicial)
                                       select pm).SingleOrDefault();
                mensajeResultado.resultado = plazaPorEmpleadoMov;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPlazasPorEmpleadosMovMaxPorClave()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPlazasPorEmpleadosMovMaxPorEmpleado(string claveEmpleado, string razonSocial, DBContextAdapter dbContext)
        {
            PlazasPorEmpleadosMov plazaPorEmpleadoMov;
            try
            {
                DateTime fechaActual = DateTime.Now;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                plazaPorEmpleadoMov = (from pm in getSession().Set<PlazasPorEmpleadosMov>()
                                       where
                                         pm.plazasPorEmpleado.empleados.clave == claveEmpleado &&
                                         pm.plazasPorEmpleado.razonesSociales.clave == razonSocial &&
                                         pm.fechaInicial ==
                                           (from o in getSession().Set<PlazasPorEmpleadosMov>()
                                            where
                                             o.plazasPorEmpleado.empleados.clave == claveEmpleado &&
                                             o.plazasPorEmpleado.razonesSociales.clave == razonSocial &&
                                             o.fechaInicial <= fechaActual
                                            select new
                                            {
                                                o.fechaInicial
                                            }).Max(p => p.fechaInicial)
                                       select pm).SingleOrDefault();
                mensajeResultado.resultado = plazaPorEmpleadoMov;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPlazasPorEmpleadosMovMaxPorEmpleado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPlazasPorEmpleadosMovMaxPorEmpleadoYRegPatronal(decimal idEmpleado, decimal idRegPat, decimal idRazonSocial, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                DateTime fechaSistema = getFechaDelSistema();
                var query = (from pm in dbContext.context.Set<PlazasPorEmpleadosMov>()
                             join pe in dbContext.context.Set<PlazasPorEmpleado>()
                             on pm.plazasPorEmpleado.id equals pe.id
                             join ib in dbContext.context.Set<IngresosBajas>()
                             on pe.ingresosBajas.id equals ib.id
                             where pe.razonesSociales_ID == idRazonSocial &&
                             pe.registroPatronal_ID == idRegPat &&
                             pe.empleados_ID == idEmpleado
                             && ib.fechaBaja >= DateTime.Today
                             && pm.fechaIMSS == (from pem in dbContext.context.Set<PlazasPorEmpleadosMov>()
                                                 join ppe in dbContext.context.Set<PlazasPorEmpleado>()
                                                 on pem.plazasPorEmpleado.id equals ppe.id
                                                 join inb in dbContext.context.Set<IngresosBajas>()
                                                 on ppe.ingresosBajas.id equals inb.id
                                                 where ppe.razonesSociales_ID == idRazonSocial &&
                                                 ppe.registroPatronal_ID == idRegPat &&
                                                 ppe.empleados_ID == idEmpleado &&
                                                 inb.fechaBaja >= DateTime.Today
                                                 select new { pem.fechaIMSS }).Max(p => p.fechaIMSS)
                             select pm).Select(PXEM => new
                             {
                                 PXEM.fechaInicial,
                                 PXEM.fechaIMSS,
                                 PXEM.plazas.clave,
                                 plazasPorEmpleado =
                    new
                    {
                        PXEM.plazasPorEmpleado.referencia,
                        empleadoClave = PXEM.plazasPorEmpleado.empleados.clave,
                        registroPatronalClave = PXEM.plazasPorEmpleado.registroPatronal.clave
                        /* PXEM.plazasPorEmpleado.*/


                    },
                                 ingresosBajas = new
                                 {
                                     PXEM.plazasPorEmpleado.ingresosBajas.fechaIngreso,
                                     PXEM.plazasPorEmpleado.ingresosBajas.fechaBaja
                                 }

                             }).SingleOrDefault();

                mensajeResultado.resultado = query;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPlazasPorEmpleadosMovMaxPorEmpleadoYRegPatronal()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPlazasPorEmpleadosMovPorRazonSocial(string clave, DBContextAdapter dbContext)
        {
            List<PlazasPorEmpleadosMov> plazaPorEmpleadoMov;
            try
            {
                DateTime fechaActual = DateTime.Now;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                plazaPorEmpleadoMov = (from pm in getSession().Set<PlazasPorEmpleadosMov>()
                                       join pe in getSession().Set<PlazasPorEmpleado>()
                                       on pm.plazasPorEmpleado.id equals pe.id
                                       join rs in getSession().Set<RazonesSociales>()
                                       on pe.razonesSociales_ID equals rs.id
                                       where rs.clave == clave

                                       select pm).ToList();
                mensajeResultado.resultado = plazaPorEmpleadoMov;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPlazasPorEmpleadosMovPorRazonSocial()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPlazasPorEmpleadosMovPorReferencia(string referencia, string claveRazonesSociales, int result, DBContextAdapter dbContext)
        {
            List<PlazasPorEmpleadosMov> plazasPorEmpleadosMov = null;
            try
            {
                DateTime fechaActual = DateTime.Now;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                Object o = obtenerMovimientosDePlaza(referencia, claveRazonesSociales, false, result);
                if (o.GetType() == typeof(List<>))
                {
                    plazasPorEmpleadosMov = (List<PlazasPorEmpleadosMov>)o;
                    mensajeResultado.resultado = plazasPorEmpleadosMov;
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPlazasPorEmpleadosMovPorReferencia()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPlazasPorEmpleadosMovPorReferenciaYRazonsocial(string referencia, decimal? idRazonSocial, decimal idPlazaPorEmpleMov, DBContextAdapter dbContext)
        {
            /* referencia = "4";
             idRazonSocial = 1;*/
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();

                var query = (from ppm in dbContext.context.Set<PlazasPorEmpleadosMov>()
                             where ppm.plazasPorEmpleado.razonesSociales_ID == idRazonSocial
                             select ppm);

                if (idPlazaPorEmpleMov > -1)
                {
                    query = from subq in query
                            where subq.id == idPlazaPorEmpleMov
                                            && subq.id == (from pxe in dbContext.context.Set<PlazasPorEmpleadosMov>()
                                                           where pxe.id == idPlazaPorEmpleMov
                                                           && pxe.plazasPorEmpleado.razonesSociales_ID == idRazonSocial
                                                           select new { pxe.id }).Min(l => l.id)
                            select subq;

                }

                else if (!string.IsNullOrEmpty(referencia))
                {
                    query = from subq in query
                            where subq.plazasPorEmpleado.referencia == referencia
                                            && subq.id == (from pxe in dbContext.context.Set<PlazasPorEmpleadosMov>()
                                                           where pxe.plazasPorEmpleado.referencia == referencia
                                                           && pxe.plazasPorEmpleado.razonesSociales_ID == idRazonSocial
                                                           select new { pxe.id }).Min(l => l.id)
                            select subq;
                }


                var plazasPorEmpleadosMov = query.Select(PXMOV => new
                {
                    PXMOV.id,
                    clavePlazas = PXMOV.plazas.clave,
                    PXMOV.tipoRelacionLaboral,
                    /*is sindicalizado*/
                    PXMOV.fechaInicial,
                    PXMOV.fechaIMSS,
                    idTipoContrato = PXMOV.tipoContrato.id,
                    idRegimenContratacion = PXMOV.regimenContratacion,
                    claveTipoNomina = PXMOV.tipoNomina.clave,
                    claveCentroDeCosto = PXMOV.centroDeCosto.clave,
                    claveDepartamentos = PXMOV.departamentos.clave,
                    clavePuestos = PXMOV.puestos.clave,
                    claveTurnos = PXMOV.turnos.clave,
                    PXMOV.horas,
                    PXMOV.importe,
                    PXMOV.sindicalizado,
                    PXMOV.salarioPor,
                    PXMOV.jornada_ID,
                    PXMOV.cambioCentroDeCostos,
                    PXMOV.cambioDepartamento,
                    PXMOV.cambioHoras,
                    PXMOV.cambioPlazasPosOrganigrama,
                    PXMOV.cambioPuestos,
                    PXMOV.cambioRegimenContratacion,
                    PXMOV.cambioTipoContrato,
                    PXMOV.cambioTipoDeNomina,
                    PXMOV.cambioTipoRelacionLaboral,
                    PXMOV.cambioTurno,
                    PXMOV.cambioSindicalizado,
                    PXMOV.cambioJornada,
                    PXMOV.cambioTipoSalario,
                    PXMOV.cambioSalario,
                    plazasPorEmpleado = new
                    {
                        PXMOV.plazasPorEmpleado.id,
                        PXMOV.plazasPorEmpleado.referencia,
                        PXMOV.plazasPorEmpleado.fechaFinal,
                        PXMOV.plazasPorEmpleado.fechaPrestaciones,
                        idRegistroPatronal = PXMOV.plazasPorEmpleado.registroPatronal.id,
                        claveRegistroPatronal = PXMOV.plazasPorEmpleado.registroPatronal.clave,
                        claveRazonesSociales = PXMOV.plazasPorEmpleado.razonesSociales.clave,
                        registroPatronal = new
                        {
                            PXMOV.plazasPorEmpleado.registroPatronal.clave,
                            PXMOV.plazasPorEmpleado.registroPatronal.delegacion,
                            PXMOV.plazasPorEmpleado.registroPatronal.subdelegacion,
                        },
                        empleados = new
                        {
                            PXMOV.plazasPorEmpleado.empleados.id,
                            PXMOV.plazasPorEmpleado.empleados.clave,
                            PXMOV.plazasPorEmpleado.empleados.apellidoPaterno,
                            PXMOV.plazasPorEmpleado.empleados.apellidoMaterno,
                            PXMOV.plazasPorEmpleado.empleados.nombre,
                            PXMOV.plazasPorEmpleado.empleados.lugarNacimiento,
                            PXMOV.plazasPorEmpleado.empleados.fechaNacimiento,
                            PXMOV.plazasPorEmpleado.empleados.genero.descripcion,
                            PXMOV.plazasPorEmpleado.empleados.nacionalidad,
                            PXMOV.plazasPorEmpleado.empleados.status,
                        },
                        ingresosBajas = new
                        {
                            PXMOV.plazasPorEmpleado.ingresosBajas.id,
                            PXMOV.plazasPorEmpleado.ingresosBajas.fechaIngreso,
                            PXMOV.plazasPorEmpleado.ingresosBajas.fechaBaja,
                            PXMOV.plazasPorEmpleado.ingresosBajas.fechaPrestaciones,
                            idEmpleado = PXMOV.plazasPorEmpleado.ingresosBajas.empleados_ID,
                            idRazonesSociales = PXMOV.plazasPorEmpleado.ingresosBajas.razonesSociales_ID,
                            idRegistroPatronal = PXMOV.plazasPorEmpleado.ingresosBajas.registroPatronal_ID,
                            PXMOV.plazasPorEmpleado.ingresosBajas.aplicar,
                            PXMOV.plazasPorEmpleado.ingresosBajas.causaBaja,
                            PXMOV.plazasPorEmpleado.ingresosBajas.tipoSeparacion,
                            PXMOV.plazasPorEmpleado.ingresosBajas.fechaCalculo,
                            PXMOV.plazasPorEmpleado.ingresosBajas.calculado,
                            PXMOV.plazasPorEmpleado.ingresosBajas.complementaria,
                            PXMOV.plazasPorEmpleado.ingresosBajas.fechaComplementaria,
                            PXMOV.plazasPorEmpleado.ingresosBajas.previa,
                            PXMOV.plazasPorEmpleado.ingresosBajas.procesado,
                        }
                    },
                }).SingleOrDefault();

                mensajeResultado.resultado = plazasPorEmpleadosMov;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPlazasPorEmpleadosActivos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorEmpleadoYRazonSocial(string claveEmpleado, string claveRazonSocial, DBContextAdapter dbContext)
        {
            List<PlazasPorEmpleadosMov> plazasPorEmpleadosMov = null;
            try
            {
                DateTime fechaActual = DateTime.Now;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                plazasPorEmpleadosMov = (from o in getSession().Set<PlazasPorEmpleadosMov>()
                                         where (from m in getSession().Set<PlazasPorEmpleadosMov>()
                                                where m.plazasPorEmpleado.empleados.clave == claveEmpleado &&
                                                    m.plazasPorEmpleado.razonesSociales.clave == claveRazonSocial
                                                group new { m.plazasPorEmpleado, m } by new
                                                {
                                                    m.plazasPorEmpleado.referencia
                                                } into g
                                                select new
                                                {
                                                    Column1 = g.Max(p => p.m.id)
                                                }).Contains(new { Column1 = o.id })
                                         orderby o.plazasPorEmpleado.referencia
                                         select o).ToList();
                mensajeResultado.resultado = plazasPorEmpleadosMov;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorEmpleadoYRazonSocial()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorEmpleadoYRazonSocialFiniquitoVigente(string claveEmpleado, string claveRazonSocial, string claveFiniquito, DBContextAdapter dbContext)
        {
            throw new NotImplementedException();//no se usa en el cliente 
        }

        public Mensaje getPorEmpleadoYRazonSocialVigente(string claveEmpleado, string claveRazonSocial, DBContextAdapter dbContext)
        {
            List<PlazasPorEmpleadosMov> plazasPorEmpleadosMov = null;
            try
            {
                DateTime fechaActual = DateTime.Now;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                plazasPorEmpleadosMov = (from o in getSession().Set<PlazasPorEmpleadosMov>()
                                         where (from m in getSession().Set<PlazasPorEmpleadosMov>()
                                                where m.plazasPorEmpleado.empleados.clave == claveEmpleado &&
                                                    m.plazasPorEmpleado.razonesSociales.clave == claveRazonSocial &&
                                                    fechaActual >= m.fechaInicial && fechaActual <= m.plazasPorEmpleado.fechaFinal
                                                group new { m.plazasPorEmpleado, m } by new
                                                {
                                                    m.plazasPorEmpleado.referencia
                                                } into g
                                                select new
                                                {
                                                    Column1 = g.Max(p => p.m.id)
                                                }).Contains(new { Column1 = o.id })
                                         orderby o.plazasPorEmpleado.referencia
                                         select o).ToList();
                mensajeResultado.resultado = plazasPorEmpleadosMov;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorEmpleadoYRazonSocialVigente()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorEmpleadoYRazonSocialYFecha(string claveEmpleado, string claveRazonSocial, DateTime fecha, DBContextAdapter dbContext)
        {
            List<PlazasPorEmpleadosMov> plazasPorEmpleadosMov = null;
            try
            {
                DateTime fechaActual = DateTime.Now;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                plazasPorEmpleadosMov = (from o in getSession().Set<PlazasPorEmpleadosMov>()
                                         where
                                               (from o0 in getSession().Set<PlazasPorEmpleadosMov>()
                                                where
                                                  (from o012 in getSession().Set<PlazasPorEmpleado>()
                                                   where
                                                      o012.empleados.clave == claveEmpleado &&
                                                      o012.razonesSociales.clave == claveRazonSocial &&
                                                      o012.fechaFinal >= fecha
                                                   select new
                                                   {
                                                       o012.id
                                                   }).Contains(new { id = o0.plazasPorEmpleado.id })
                                                group new { o0.plazasPorEmpleado, o0 } by new
                                                {
                                                    o0.plazasPorEmpleado.id
                                                } into g
                                                select new
                                                {
                                                    Column1 = g.Max(p => p.o0.id)
                                                }).Contains(new { Column1 = o.id })
                                         select o).ToList();
                mensajeResultado.resultado = plazasPorEmpleadosMov;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorEmpleadoYRazonSocialYFecha()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        private Object obtenerMovimientosDePlaza(String referencia, String claveRazonesSociales, bool ordenAsc, int result)
        {
            List<PlazasPorEmpleadosMov> plazasPorEmpleadosMov = null;
            try
            {
                var query = from o in getSession().Set<PlazasPorEmpleadosMov>()
                            where o.plazasPorEmpleado.referencia == referencia &&
                            o.plazasPorEmpleado.razonesSociales.clave == claveRazonesSociales
                            select o;
                if (ordenAsc == true)
                {
                    query = from sub in query
                            orderby sub.id ascending
                            select sub;
                }
                else
                {
                    query = from sub in query
                            orderby sub.id descending
                            select sub;
                }
                if (result > 0)
                {
                    plazasPorEmpleadosMov = query.Take(result).ToList();
                }
                else
                {
                    plazasPorEmpleadosMov = query.ToList();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPlazasPorEmpleadosMovMaxPorEmpleado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return plazasPorEmpleadosMov;
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

        public Mensaje getPorEmpleYRazonSocialVigente(decimal? idEmpleado, string claveRazonSocial, DBContextAdapter dbContext)
        {
            //7 List<PlazasPorEmpleadosMov> plazasPorEmpleadosMov = null;
            try
            {
                DateTime fechaActual = DateTime.Now;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var plazasPorEmpleadosMov = (from o in dbContext.context.Set<PlazasPorEmpleadosMov>()
                                             where (from m in dbContext.context.Set<PlazasPorEmpleadosMov>()
                                                    where m.plazasPorEmpleado.empleados.id == idEmpleado &&
                                                        m.plazasPorEmpleado.razonesSociales.clave == claveRazonSocial &&
                                                        fechaActual >= m.fechaInicial && fechaActual <= m.plazasPorEmpleado.fechaFinal
                                                    group new { m.plazasPorEmpleado, m } by new
                                                    {
                                                        m.plazasPorEmpleado.referencia
                                                    } into g
                                                    select new
                                                    {
                                                        Column1 = g.Max(p => p.m.id)
                                                    }).Contains(new { Column1 = o.id })
                                             orderby o.plazasPorEmpleado.referencia
                                             select new
                                             {
                                                 o.centroDeCosto_ID,
                                                 o.departamentos_ID,
                                                 o.fechaIMSS,
                                                 o.fechaInicial,
                                                 o.id,
                                                 plazasPorEmpleado = new
                                                 {
                                                     o.plazasPorEmpleado.plazaPrincipal
                                                 },
                                                 o.plazasPorEmpleado_ID,
                                                 o.plazas_ID,
                                                 o.puestos_ID,
                                                 o.regimenContratacion,
                                                 o.tipoContrato_ID,
                                                 o.tipoNomina_ID,
                                                 o.turnos_ID
                                             }).ToList();
                mensajeResultado.resultado = plazasPorEmpleadosMov;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorEmpleadoYRazonSocialVigente()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorClaveEmpleYRazonSocialVigente(string claveEmpleado, string claveRazonSocial, DBContextAdapter dbContext)
        {

            try
            {
                DateTime fechaActual = DateTime.Now;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var plazasPorEmpleadosMov = (from o in dbContext.context.Set<PlazasPorEmpleadosMov>()
                                             where (from m in dbContext.context.Set<PlazasPorEmpleadosMov>()
                                                    where m.plazasPorEmpleado.empleados.clave == claveEmpleado &&
                                                        m.plazasPorEmpleado.razonesSociales.clave == claveRazonSocial &&
                                                        fechaActual >= m.fechaInicial && fechaActual <= m.plazasPorEmpleado.fechaFinal
                                                    group new { m.plazasPorEmpleado, m } by new
                                                    {
                                                        m.plazasPorEmpleado.referencia
                                                    } into g
                                                    select new
                                                    {
                                                        Column1 = g.Max(p => p.m.id)
                                                    }).Contains(new { Column1 = o.id })
                                             orderby o.plazasPorEmpleado.referencia
                                             select new
                                             {
                                                 o.id,
                                                 idPlazaEmpleado = o.plazasPorEmpleado.id,
                                                 o.plazasPorEmpleado.referencia,
                                                 descripcionPuesto = o.puestos.descripcion,
                                                 o.horas,
                                                 o.importe,
                                                 o.fechaInicial

                                             }).ToList();
                mensajeResultado.resultado = plazasPorEmpleadosMov;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorEmpleadoYRazonSocialVigente()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorEmpleadoYRazonSocialYFechaJS(string claveEmpleado, string claveRazonSocial, DateTime? fecha, DBContextAdapter dbContext)
        {
            //List<PlazasPorEmpleadosMov> plazasPorEmpleadosMov = null; 
            try
            {
                DateTime fechaActual = DateTime.Now;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                List<decimal> ids = (from pl in getSession().Set<PlazasPorEmpleado>()
                                     where pl.empleados.clave == claveEmpleado && pl.razonesSociales.clave == claveRazonSocial && pl.fechaFinal >= fechaActual.Date
                                     select pl.id
                    ).ToList();

                List<decimal> idsM = (from plM in getSession().Set<PlazasPorEmpleadosMov>()
                                      where ids.Contains(plM.id)
                                      select plM.id).ToList();

                var plazasPorEmpleadosMov = (from o in getSession().Set<PlazasPorEmpleadosMov>()
                                             where idsM.Contains(o.id)
                                             select new
                                             {
                                                 o.centroDeCosto_ID,
                                                 o.departamentos_ID,
                                                 o.fechaIMSS,
                                                 o.fechaInicial,
                                                 o.id,
                                                 o.importe,
                                                 o.jornada_ID,
                                                 o.plazasPorEmpleado_ID,
                                                 o.plazas_ID,
                                                 o.puestos_ID,
                                                 puestos = new
                                                 {
                                                     o.puestos.salarioTabular
                                                 },
                                                 o.salarioPor,
                                                 o.sueldoDiario,
                                                 o.tipoContrato_ID,
                                                 o.tipoNomina_ID,
                                                 o.turnos_ID

                                             }).ToList();

                mensajeResultado.resultado = plazasPorEmpleadosMov;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorEmpleadoYRazonSocialYFecha()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
    }
}


