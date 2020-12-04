/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Clase CFDIEmpleadoDAO para llamados a metodos de Entity
* -----------------------------------------------------------------------------
* MODIFICACIONES:
* -----------------------------------------------------------------------------
* Clave: 
* Autor: 
* Fecha:
* Descripción: 
* -----------------------------------------------------------------------------
*/

using Exitosw.Payroll.Entity.entidad.cfdi;
using System;
using System.Collections.Generic;
using System.Linq;
using Exitosw.Payroll.Entity.entidad;
using System.Text;
using System.Reflection;
using Exitosw.Payroll.Core.util;
using System.Data.Entity.Migrations;
using Exitosw.Payroll.Core.genericos.campos;
using Exitosw.Payroll.TestCompilador.funciones;
using System.Data.Entity;
using Exitosw.Payroll.Core.campos;
using NHibernate;
using Newtonsoft.Json;
//using Exitosw.Payroll.Hibernate.entidad;

namespace Exitosw.Payroll.Core.modelo
{
    public class CFDIEmpleadoDAO : GenericRepository<CFDIEmpleado>, CFDIEmpleadoDAOIF
    {
        private ConcepNomDefi conceptoNominaSubsidio;
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        List<CFDIEmpleado> listCFDIEmpleado = new List<CFDIEmpleado>();
        private bool manejaPagosPorHora = false;
        private ManejoHorasPor manejoHorasPor = ManejoHorasPor.NINGUNO;
        private ManejoSalarioDiario manejoSalarioDiario = ManejoSalarioDiario.NINGUNO;
        private bool manejaPagoDiasNaturales = false;
        private ConcepNomDefi concepto;

        public Mensaje agregar(CFDIEmpleado entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<CFDIEmpleado>().Add(entity);
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

        public Mensaje actualizar(CFDIEmpleado entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<CFDIEmpleado>().AddOrUpdate(entity);
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

        public Mensaje eliminar(CFDIEmpleado entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<CFDIEmpleado>().Attach(entity);
                getSession().Set<CFDIEmpleado>().Remove(entity);
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

        public Mensaje buscaCFDIEmpleadosFiltrado(List<object> valoresDeFiltrado, List<decimal?> listIdEmpleados, DBContextAdapter dbContext)
        {
            List<CFDIEmpleado> cFDIEmpleados = new List<CFDIEmpleado>();
            valoresDeFiltrado = valoresDeFiltrado == null ? new List<Object>() : valoresDeFiltrado;
            int i;
            List<String> camposWhere = new List<String>();
            List<Object> valoresWhere = new List<Object>();
            bool empleadoInicio = true;
            /**
             * ******************************Carga datos para
             * filtrado******************************************************
             */
            setSession(dbContext.context);
            var query = (from cem in getSession().Set<CFDIEmpleado>()
                         join rs in getSession().Set<RazonesSociales>() on cem.razonesSociales_ID equals rs.id into rs_join
                         from rs in rs_join.DefaultIfEmpty()
                         join ppm in getSession().Set<PlazasPorEmpleadosMov>() on cem.plazaPorEmpleadoMov_ID equals ppm.id into ppm_join
                         from ppm in ppm_join.DefaultIfEmpty()
                         join pp in getSession().Set<PlazasPorEmpleado>() on ppm.plazasPorEmpleado_ID equals pp.id into pp_join
                         from pp in pp_join.DefaultIfEmpty()
                         join em in getSession().Set<Empleados>() on pp.empleados_ID equals em.id into em_join
                         from em in em_join.DefaultIfEmpty()
                         select new
                         {
                             rs,
                             cem,
                             pp,
                             ppm,
                             em.clave,
                             pp.empleados_ID
                         }
                       );
            mensajeResultado.resultado = cFDIEmpleados;
            mensajeResultado.noError = 0;
            mensajeResultado.error = "";
            //getSession().Database.CurrentTransaction.Commit();
            if (valoresDeFiltrado.Count() > 0)
            {
                Empleados empIni = null;
                Empleados empFin = null;
                try
                {
                    List<StatusTimbrado> listStatusTimbrado = new List<StatusTimbrado>();
                    for (i = 0; i < valoresDeFiltrado.Count(); i++)
                    {
                        if (valoresDeFiltrado[i].GetType().BaseType == typeof(RazonesSociales))
                        {
                            string claveRazonSocial = ((RazonesSociales)valoresDeFiltrado[i]).clave;
                            query = (from subquery in query
                                     where subquery.rs.clave == claveRazonSocial
                                     select subquery);
                        }
                        else if (valoresDeFiltrado[i].GetType().BaseType == typeof(TipoNomina))
                        {
                            string claveTipoNomina = ((TipoNomina)valoresDeFiltrado[i]).clave;
                            query = (from subquery in query
                                     join tn in getSession().Set<TipoNomina>() on subquery.cem.tipoNomina_ID equals tn.id into tn_join
                                     from tn in tn_join.DefaultIfEmpty()
                                     where tn.clave == claveTipoNomina
                                     select subquery);
                        }
                        else if (valoresDeFiltrado[i].GetType().BaseType == typeof(TipoCorrida))
                        {
                            string claveTipoCorrida = ((TipoCorrida)valoresDeFiltrado[i]).clave;
                            query = (from subquery in query
                                     join tc in getSession().Set<TipoCorrida>() on subquery.cem.tipoCorrida_ID equals tc.id into tc_join
                                     from tc in tc_join.DefaultIfEmpty()
                                     where tc.clave == claveTipoCorrida
                                     select subquery);
                        }
                        else if (valoresDeFiltrado[i].GetType().BaseType == typeof(PeriodosNomina))
                        {
                            decimal? idPeriodoNomina = ((PeriodosNomina)valoresDeFiltrado[i]).id;
                            query = (from subquery in query
                                     join pn in getSession().Set<PeriodosNomina>() on subquery.cem.periodosNomina_ID equals pn.id into pn_join
                                     from pn in pn_join.DefaultIfEmpty()
                                     where pn.id == idPeriodoNomina
                                     select subquery);
                        }
                        else if (valoresDeFiltrado[i].GetType().BaseType == typeof(RegistroPatronal))
                        {
                            string claveRegistroPatronal = ((RegistroPatronal)valoresDeFiltrado[i]).clave;
                            query = (from subquery in query
                                     join rp in getSession().Set<RegistroPatronal>() on subquery.pp.registroPatronal_ID equals rp.id into rp_join
                                     from rp in rp_join.DefaultIfEmpty()
                                     where rp.clave == claveRegistroPatronal
                                     select subquery);
                        }
                        else if (valoresDeFiltrado[i].GetType().BaseType == typeof(CentroDeCosto))
                        {
                            string claveCentroCostos = ((CentroDeCosto)valoresDeFiltrado[i]).clave;
                            query = (from subquery in query
                                     join cc in getSession().Set<CentroDeCosto>() on subquery.ppm.centroDeCosto_ID equals cc.id into cc_join
                                     from cc in cc_join.DefaultIfEmpty()
                                     where cc.clave == claveCentroCostos
                                     select subquery);
                        }
                        else if (valoresDeFiltrado[i].GetType().BaseType == typeof(Departamentos))
                        {
                            string claveDepartamentos = ((Departamentos)valoresDeFiltrado[i]).clave;
                            query = (from subquery in query
                                     join dp in getSession().Set<Departamentos>() on subquery.ppm.departamentos_ID equals dp.id into dp_join
                                     from dp in dp_join.DefaultIfEmpty()
                                     where dp.clave == claveDepartamentos
                                     select subquery);
                        }
                        else if (valoresDeFiltrado[i].GetType() == typeof(StatusTimbrado))
                        {

                            listStatusTimbrado.Add((StatusTimbrado)valoresDeFiltrado[i]);

                        }
                        else if (valoresDeFiltrado[i].GetType().BaseType == typeof(Empleados) && empleadoInicio)
                        {
                            empleadoInicio = false;
                            empIni = (Empleados)valoresDeFiltrado[i];
                            if (empIni.id == 0)
                            {
                                empIni = null;
                            }
                        }
                        else if (valoresDeFiltrado[i].GetType().BaseType == typeof(Empleados))
                        {
                            empFin = (Empleados)valoresDeFiltrado[i];
                            if (empFin.id == 0)
                            {
                                empFin = null;
                            }
                        }

                    }
                    if (listStatusTimbrado.Any())
                    {
                        query = (from subquery in query
                                 join cfdiR in getSession().Set<CFDIRecibo>() on subquery.cem.cfdiRecibo_ID equals cfdiR.id into cfdiR_join
                                 from cfdiR in cfdiR_join.DefaultIfEmpty()
                                 where listStatusTimbrado.Contains(cfdiR.statusTimbrado)
                                 select subquery);
                    }
                    if (listIdEmpleados != null) {

                        query = (from subquery in query

                                 where (listIdEmpleados.Contains(subquery.empleados_ID) )
                                 select subquery);
                    }
                    else {
                        if (empIni != null && empFin != null)
                        {
                            query = (from subquery in query
                                     where ((subquery.clave.CompareTo(empIni.clave) >= 0) &&
                                     (subquery.clave.CompareTo(empFin.clave) <= 0))
                                     select subquery);

                        }
                        else if (empIni != null)
                        {
                            query = (from subquery in query
                                     where (subquery.clave.CompareTo(empIni.clave) >= 0)
                                     select subquery);
                        }
                        else if (empFin != null)
                        {
                            query = (from subquery in query
                                     where (subquery.clave.CompareTo(empFin.clave) <= 0)
                                     select subquery);
                        }
                    }
                    
                    var query2 = (from subquery in query
                                  select subquery.cem);
                    cFDIEmpleados = query2.ToList<CFDIEmpleado>();
                    mensajeResultado.resultado = cFDIEmpleados;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscaCFDIEmpleadosFiltrado()1_Error: ").Append(ex));
                    mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                    mensajeResultado.error = ex.GetBaseException().ToString();
                    mensajeResultado.resultado = null;
                    getSession().Database.CurrentTransaction.Rollback();
                }
            }

            return mensajeResultado;
        }

        public Mensaje consultaPorRangosCFDIEmpleado(Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
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

        public Mensaje buscarCFDIEmpleadosEnProceso(List<object> valoresDeFiltrado, DBContextAdapter dbContext)
        {
            List<CFDIEmpleado> cFDIEmpleados = new List<CFDIEmpleado>();
            valoresDeFiltrado = valoresDeFiltrado == null ? new List<Object>() : valoresDeFiltrado;
            int i;
            List<String> camposWhere = new List<String>();
            List<Object> valoresWhere = new List<Object>();
            bool empleadoInicio = true;
            /* **********************Carga datos para filtrado********************** */
            setSession(dbContext.context);
            var query = (from cepc in getSession().Set<CFDIReciboProcCanc>()
                         join cem in getSession().Set<CFDIEmpleado>() on cepc.cfdiRecibo_ID equals cem.cfdiRecibo_ID into cem_join
                         from cem in cem_join.DefaultIfEmpty()
                         join rs in getSession().Set<RazonesSociales>() on cem.razonesSociales_ID equals rs.id into rs_join
                         from rs in rs_join.DefaultIfEmpty()
                         join ppm in getSession().Set<PlazasPorEmpleadosMov>() on cem.plazaPorEmpleadoMov_ID equals ppm.id into ppm_join
                         from ppm in ppm_join.DefaultIfEmpty()
                         join pp in getSession().Set<PlazasPorEmpleado>() on ppm.plazasPorEmpleado_ID equals pp.id into pp_join
                         from pp in pp_join.DefaultIfEmpty()
                         join em in getSession().Set<Empleados>() on pp.empleados_ID equals em.id into em_join
                         from em in em_join.DefaultIfEmpty()
                         select new
                         {
                             cepc,
                             rs,
                             cem,
                             pp,
                             ppm,
                             em.clave
                         }
                       );
            mensajeResultado.resultado = cFDIEmpleados;
            mensajeResultado.noError = 0;
            mensajeResultado.error = "";
           
            if (valoresDeFiltrado.Count() > 0)
            {
                Empleados empIni = null;
                Empleados empFin = null;
                try
                {
                    
                    for (i = 0; i < valoresDeFiltrado.Count(); i++)
                    {
                        if (valoresDeFiltrado[i].GetType().BaseType == typeof(RazonesSociales))
                        {
                            string claveRazonSocial = ((RazonesSociales)valoresDeFiltrado[i]).clave;
                            query = (from subquery in query
                                     where subquery.rs.clave == claveRazonSocial
                                     select subquery);
                        }
                        else if (valoresDeFiltrado[i].GetType().BaseType == typeof(Empleados) && empleadoInicio)
                        {
                            empleadoInicio = false;
                            empIni = (Empleados)valoresDeFiltrado[i];
                            if (empIni.id == 0)
                            {
                                empIni = null;
                            }
                        }
                        else if (valoresDeFiltrado[i].GetType().BaseType == typeof(Empleados))
                        {
                            empFin = (Empleados)valoresDeFiltrado[i];
                            if (empFin.id == 0)
                            {
                                empFin = null;
                            }
                        }

                    }
                    if(empIni != null && empFin != null)
                    {
                        query = (from subquery in query
                                 where ((subquery.clave.CompareTo(empIni.clave) >= 0) &&
                                 (subquery.clave.CompareTo(empFin.clave) <= 0))
                                 select subquery);

                    }
                    else if (empIni != null)
                    {
                        query = (from subquery in query
                                 where (subquery.clave.CompareTo(empIni.clave) >= 0)
                                 select subquery);
                    }
                    else if (empFin != null)
                    {
                        query = (from subquery in query
                                 where (subquery.clave.CompareTo(empFin.clave) <= 0)
                                 select subquery);
                    }
                    var query2 = (from subquery in query
                                  select subquery.cem);
                    cFDIEmpleados = query2.ToList<CFDIEmpleado>();
                    mensajeResultado.resultado = cFDIEmpleados;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscarCFDIEmpleadosEnProceso()1_Error: ").Append(ex));
                    mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                    mensajeResultado.error = ex.GetBaseException().ToString();
                    mensajeResultado.resultado = null;
                    getSession().Database.CurrentTransaction.Rollback();
                }
            }

            return mensajeResultado;
        }

        public Mensaje buscarCFDIReciboProcCanc(decimal idCFDIReciboProcCanc, DBContextAdapter dbContext)
        { //--
            List<CFDIReciboProcCanc> cFDIRecProcCanc = new List<CFDIReciboProcCanc>();
            

            mensajeResultado.resultado = cFDIRecProcCanc;
            mensajeResultado.noError = 0;
            mensajeResultado.error = "";

            setSession(dbContext.context);
            var query = (from cepc in getSession().Set<CFDIReciboProcCanc>()
                         where cepc.id == idCFDIReciboProcCanc
                         select new
                         {
                             cepc
                         }
                       );


            try
            {
                var query2 = (from subquery in query
                              select subquery.cepc);
                cFDIRecProcCanc = query2.ToList<CFDIReciboProcCanc>();
                mensajeResultado.resultado = cFDIRecProcCanc;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscarCFDIReciboProcCanc()1_Error: ").Append(ex));
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
        //        //existe = existeDato(CFDIEmpleado.class.getSimpleName(), campo, valor);
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
        private List<Object> construyeQueryDatosGlobalesEmpleados(DatosFiltradoEmpleados datosFiltradoEmpleado, DBContextAdapter dBContextSimple)
        {
            List<Object> datosEmpleado = new List<object>();
            List<Object> datosEmpleadoAux = new List<object>();



            DateTime? fechaInicial = datosFiltradoEmpleado.getFechaInicio();
            DateTime? fechaFinal = datosFiltradoEmpleado.getFechaFin();
            DateTime? fechaIngresoAux = new DateTime(1900, 1, 1);
            string claveTipoCorrida = datosFiltradoEmpleado.getClaveTipoCorrida();
            string claveTipoNomina = datosFiltradoEmpleado.getClaveTipoNomina();
            string claveRazonSocial = datosFiltradoEmpleado.getClaveRazonSocial();
            string claveCentroCosto = datosFiltradoEmpleado.getClaveCentroCosto();
            string claveDepartamento = datosFiltradoEmpleado.getClaveDepartamento();
            string claveInicioEmpleado = datosFiltradoEmpleado.getClaveInicioEmpleado();
            string claveFinEmpleado = datosFiltradoEmpleado.getClaveFinEmpleado();
            decimal idEmpIni = datosFiltradoEmpleado.getidEmpleadoInicio();
            decimal idEmpFin = datosFiltradoEmpleado.getidEmpleadoFin();

            string claveRegistroPatronal = datosFiltradoEmpleado.getClaveRegistroPatronal();
            var querycorrida = (from cfdiEmp in dBContextSimple.context.Set<CFDIEmpleado>()
                                join cfdiPPM in dBContextSimple.context.Set<PlazasPorEmpleadosMov>() on cfdiEmp.plazaPorEmpleadoMov_ID equals cfdiPPM.id into cfdiPPM_join
                                from cfdiPPM in cfdiPPM_join.DefaultIfEmpty()
                                join recibo in dBContextSimple.context.Set<CFDIRecibo>() on cfdiEmp.cfdiRecibo_ID equals recibo.id into recibo_join
                                from recibo in recibo_join.DefaultIfEmpty()
                                join cfdiPeriodo in dBContextSimple.context.Set<PeriodosNomina>() on cfdiEmp.periodosNomina_ID equals cfdiPeriodo.id into cfdiPeriodo_join
                                from cfdiPeriodo in cfdiPeriodo_join.DefaultIfEmpty()
                                join cfdiCorrida in dBContextSimple.context.Set<TipoCorrida>() on cfdiPeriodo.tipoCorrida_ID equals cfdiCorrida.id into cfdiCorrida_join
                                from cfdiCorrida in cfdiCorrida_join.DefaultIfEmpty()
                                join cfdiPP in dBContextSimple.context.Set<PlazasPorEmpleado>() on cfdiPPM.plazasPorEmpleado_ID equals cfdiPP.id into cfdiPP_join
                                from cfdiPP in cfdiPP_join.DefaultIfEmpty()
                                join em in dBContextSimple.context.Set<Empleados>() on cfdiPP.empleados_ID equals em.id into em_join
                                from em in em_join.DefaultIfEmpty()
                                join razonSocial in dBContextSimple.context.Set<RazonesSociales>() on cfdiPP.razonesSociales_ID equals razonSocial.id into razonSocial_join
                                from razonSocial in razonSocial_join.DefaultIfEmpty()
                                where
                               recibo.statusTimbrado == StatusTimbrado.TIMBRADO &&
                                 ((cfdiPeriodo.fechaInicial >= fechaInicial &&
                                  cfdiPeriodo.fechaInicial <= fechaFinal) ||
                                  (cfdiPeriodo.fechaFinal >= fechaInicial &&
                                   cfdiPeriodo.fechaFinal <= fechaFinal))
                                &&
                                  em.razonesSociales_ID == razonSocial.id
                                select new { em, cfdiCorrida });
            if (datosFiltradoEmpleado.getClaveTipoCorrida().Any())
            {
                querycorrida = (from subquery in querycorrida
                                where subquery.cfdiCorrida.clave.Equals(claveTipoCorrida)
                                select subquery);
            }

            var query = (from ppm in dBContextSimple.context.Set<PlazasPorEmpleadosMov>()
                         join pp in dBContextSimple.context.Set<PlazasPorEmpleado>() on ppm.plazasPorEmpleado_ID equals pp.id into pp_join
                         from pp in pp_join.DefaultIfEmpty()
                         join empleado in dBContextSimple.context.Set<Empleados>() on pp.empleados_ID equals empleado.id into empleado_join
                         from empleado in empleado_join.DefaultIfEmpty()
                         join nomina in dBContextSimple.context.Set<TipoNomina>() on ppm.tipoNomina_ID equals nomina.id into nomina_join
                         from nomina in nomina_join.DefaultIfEmpty()
                         join razonSocial in dBContextSimple.context.Set<RazonesSociales>() on pp.razonesSociales_ID equals razonSocial.id into razonSocial_join
                         from razonSocial in razonSocial_join.DefaultIfEmpty()
                         from sdi in dBContextSimple.context.Set<SalariosIntegrados>()
                         join sdiEmp in dBContextSimple.context.Set<Empleados>() on sdi.empleados_ID equals sdiEmp.id into sdiEmp_join
                         from sdiEmp in sdiEmp_join.DefaultIfEmpty()
                         from mvn in dBContextSimple.context.Set<MovNomConcep>()
                         join periodo in dBContextSimple.context.Set<PeriodosNomina>() on mvn.periodosNomina_ID equals periodo.id into periodo_join
                         from periodo in periodo_join.DefaultIfEmpty()
                         join corrida in dBContextSimple.context.Set<TipoCorrida>() on mvn.tipoCorrida_ID equals corrida.id into corrida_join
                         from corrida in corrida_join.DefaultIfEmpty()
                         join mvmEmp in dBContextSimple.context.Set<Empleados>() on mvn.empleado_ID equals mvmEmp.id into mvmEmp_join
                         from mvmEmp in mvmEmp_join.DefaultIfEmpty()
                         join periodo2 in dBContextSimple.context.Set<PeriodosNomina>() on mvn.periodosNomina_ID equals periodo2.id into periodo2_join
                         from periodo2 in periodo2_join.DefaultIfEmpty()
                         where mvmEmp.id == empleado.id &&
                         sdiEmp.id == empleado.id &&
                          !
                           (from sub in querycorrida.AsEnumerable()
                            group sub by new
                            {
                                sub.em.id
                            } into g
                            select new
                            {
                                Column1 = g.Count() == 0 ? 0 : g.Key.id
                            }).Contains(new { Column1 = empleado.id }) &&
                              sdi.fecha == (from s0 in dBContextSimple.context.Set<SalariosIntegrados>()
                                            join s0Emp in dBContextSimple.context.Set<Empleados>() on s0.empleados_ID equals s0Emp.id into s0Emp_join
                                            from s0Emp in s0Emp_join.DefaultIfEmpty()
                                            where
                                              s0.fecha <= fechaFinal &&
                                              s0Emp.id == empleado.id
                                            select new
                                            {
                                                s0.fecha
                                            }).Max(p => p.fecha) &&
                         (new List<decimal> { ppm.id }).Contains((from pem in dBContextSimple.context.Set<PlazasPorEmpleadosMov>()
                                                                  where
                                                                 pem.plazasPorEmpleado.empleados.id == empleado.id &&
                                                                 fechaFinal >= pem.fechaInicial &&
                                                                 pem.plazasPorEmpleado.fechaFinal > fechaFinal
                                                                  select new
                                                                  {
                                                                      pem.id

                                                                  }).Max(p => p.id))
                         //select
                         select new
                         {

                             empleado,
                             sdi,
                             nomina,
                             razonSocial,
                             corrida,
                             periodo,
                             pp,
                             ppm

                         });

            if (datosFiltradoEmpleado.getClaveTipoNomina().Any())
            {
                query = (from subquery in query
                         where subquery.nomina.clave == claveTipoNomina
                         && subquery.nomina.clave == claveTipoNomina
                         select subquery);
            }

            if (datosFiltradoEmpleado.getClaveRazonSocial().Any())
            {
                query = (from subquery in query
                         where subquery.razonSocial.clave.Equals(claveRazonSocial)
                         select subquery);
            }
            if (datosFiltradoEmpleado.getClaveTipoCorrida().Any())
            {
                query = (from subquery in query
                         where subquery.corrida.clave.Equals(claveTipoCorrida) &&
                         subquery.periodo.tipoCorrida.clave.Equals(claveTipoCorrida)
                         select subquery);

            }
            if (datosFiltradoEmpleado.getFechaFin() != null && datosFiltradoEmpleado.getFechaInicio() != null)
            {
                query = (from subquery in query
                         where subquery.periodo.fechaFinal <= fechaFinal &&
                         subquery.periodo.fechaInicial >= fechaInicial
                         select subquery);
            }
            if (datosFiltradoEmpleado.getClaveCentroCosto().Any())
            {
                query = (from subquery in query
                         join cc in dBContextSimple.context.Set<CentroDeCosto>() on subquery.ppm.centroDeCosto.id equals cc.id into cc_join
                         from cc in cc_join.DefaultIfEmpty()
                         where cc.clave.Equals(claveCentroCosto)
                         select subquery);
            }
            if (datosFiltradoEmpleado.getClaveDepartamento().Any())
            {
                query = (from subquery in query
                         join depto in dBContextSimple.context.Set<Departamentos>() on subquery.ppm.departamentos.id equals depto.id into depto_join
                         from depto in depto_join.DefaultIfEmpty()
                         where depto.clave.Equals(claveDepartamento)
                         select subquery);
            }
            if (datosFiltradoEmpleado.getClaveRegistroPatronal().Any())
            {
                query = (from subquery in query
                         join rp in dBContextSimple.context.Set<RegistroPatronal>() on subquery.pp.registroPatronal.id equals rp.id into rp_join
                         from rp in rp_join.DefaultIfEmpty()
                         where rp.clave == claveRegistroPatronal
                         select subquery);
            }
            if (datosFiltradoEmpleado.getClaveInicioEmpleado().Any() && datosFiltradoEmpleado.getClaveFinEmpleado().Any())
            {
                /* query = (from subquery in query
                          where Convert.ToInt32(subquery.empleado.clave) >= Convert.ToInt32(claveInicioEmpleado) &&
                          Convert.ToInt32(subquery.empleado.clave) <= Convert.ToInt32(claveFinEmpleado)
                          select subquery);*/

                query = (from subquery in query
                         where subquery.empleado.id >= idEmpIni   &&
                         subquery.empleado.id <= idEmpFin
                         select subquery);
            }
            else if (datosFiltradoEmpleado.getClaveInicioEmpleado().Any())
            {
                query = (from subquery in query
                         where subquery.empleado.id >= idEmpIni
                         select subquery);
            }
            else if (datosFiltradoEmpleado.getClaveFinEmpleado().Any())
            {
                query = (from subquery in query
                         where subquery.empleado.id <= idEmpFin
                         select subquery);
            }

           

            var query2 = (from subquery in query
                          orderby (subquery.empleado.clave == null ? "" : subquery.empleado.clave)
                          select new
                          {
                              empleado_clave = subquery.empleado.clave == null ? "" : subquery.empleado.clave,
                              ppm2 = new
                              {
                                  // subquery.ppm.bancos_ID,
                                  // subquery.ppm.cambioBanco,
                                  subquery.ppm.cambioCentroDeCostos,
                                  // subquery.ppm.cambioClabe,
                                  //  subquery.ppm.cambioCuentaBancaria,
                                  subquery.ppm.cambioDepartamento,
                                 /* subquery.ppm.cambioFormaDePago,*/
                                  subquery.ppm.cambioHoras,
                                  subquery.ppm.cambioPlazasPosOrganigrama,
                                  subquery.ppm.cambioPuestos,
                                  subquery.ppm.cambioRegimenContratacion,
                                  // subquery.ppm.cambioSalarioPor,
                                  subquery.ppm.cambioTipoContrato,
                                  subquery.ppm.cambioTipoDeNomina,
                                  subquery.ppm.cambioTipoRelacionLaboral,
                                  subquery.ppm.cambioTurno,
                                  //subquery.ppm.cambioZonaGeografica,
                                  subquery.ppm.centroDeCosto_ID,
                                  // subquery.ppm.clabe,
                                  // subquery.ppm.cuentaBancaria,
                                  subquery.ppm.departamentos_ID,
                                  subquery.ppm.fechaIMSS,
                                  subquery.ppm.fechaInicial,
                                /*  subquery.ppm.formasDePago_ID,*/
                                  subquery.ppm.horas,
                                  subquery.ppm.id,
                                  subquery.ppm.importe,
                                  subquery.ppm.plazasPorEmpleado_ID,
                                  subquery.ppm.plazas_ID,
                                  subquery.ppm.puestos_ID,
                                  subquery.ppm.regimenContratacion,
                                  //   subquery.ppm.salarioPor,
                                  //subquery.ppm.sueldoDiario,
                                  subquery.ppm.tipoContrato_ID,
                                  subquery.ppm.tipoNomina_ID,
                                  subquery.ppm.tipoRelacionLaboral,
                                  subquery.ppm.turnos_ID,
                                  //  subquery.ppm.zonaGeografica
                              },
                              sdisalarioDiarioIntegrado = subquery.sdi.salarioDiarioIntegrado == null ? 0.0 : subquery.sdi.salarioDiarioIntegrado,
                              Column3 = (DateTime?)
                                                         ((from ing in dBContextSimple.context.Set<IngresosBajas>()
                                                           join emp in dBContextSimple.context.Set<Empleados>() on ing.empleados_ID equals emp.id into emp_join
                                                           from emp in emp_join.DefaultIfEmpty()
                                                           join ingPP in dBContextSimple.context.Set<PlazasPorEmpleado>()
                                                           on ing.id equals ingPP.ingresosBajas_ID into ingPP_join
                                                           from ingPP in ingPP_join.DefaultIfEmpty()
                                                           where
                                                             emp.id == subquery.empleado.id &&
                                                             ingPP.id == subquery.pp.id
                                                             /*&&
                                                             ing.fechaBaja >= fechaInicial &&
                                                             ing.fechaIngreso <= fechaFinal*/
                                                           select new
                                                           {
                                                               Column1 =
                                                             ing.fechaIngreso == null ? fechaIngresoAux : ing.fechaIngreso
                                                           }).FirstOrDefault().Column1),
                              periododetalleConceptoRecibo = subquery.periodo.detalleConceptoRecibo == null ? "" : subquery.periodo.detalleConceptoRecibo,
                              corridadetalleConceptoRecibo = subquery.corrida.detalleConceptoRecibo == null ? "" : subquery.corrida.detalleConceptoRecibo,
                              nominadetalleConceptoRecibo = subquery.nomina.detalleConceptoRecibo == null ? "" : subquery.nomina.detalleConceptoRecibo,
                          }).Distinct();
            datosEmpleadoAux = query2.ToList<object>();


            for (int i = 0; i < datosEmpleadoAux.Count; i++)
            {
                var json = JsonConvert.SerializeObject(datosEmpleadoAux[i]);
                Dictionary<string, object> valores = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                int cont = 0;
                object[] datos = new object[valores.Count()];
                foreach (var item in valores)
                {
                    datos[cont] = item.Value;
                    cont++;
                }
                datosEmpleado.Add(datos);
            }






            return datosEmpleado;
        }
        public Mensaje generaDatosParaTimbrado(List<object> valoresDeFiltrado, string claveRazonSocial, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            List<DatosParaTimbrar> datosParaTimbrar = new List<DatosParaTimbrar>();
            valoresDeFiltrado = valoresDeFiltrado == null ? new List<object>() : valoresDeFiltrado;
            try
            {
                /**
                 * ******************************Carga datos para
                 * filtrado******************************************************
                 */
                Periodicidad periodicidad = null;
                int i, j;
                DatosFiltradoEmpleados datosFiltradoEmpleado = new DatosFiltradoEmpleados();
                DatosFiltradoMovNom datosFiltradoMovNom = new DatosFiltradoMovNom();
                DatosFiltradoAsistencias datosFiltradoAsistencia = new DatosFiltradoAsistencias();
                datosFiltradoEmpleado.setClaveRazonSocial(claveRazonSocial);
                datosFiltradoMovNom.setClaveRazonSocial(claveRazonSocial);
                datosFiltradoAsistencia.setClaveRazonSocial(claveRazonSocial);
                bool empleadoInicio = true;
                
                if (valoresDeFiltrado.Count() > 0)
                {
                    Empleados empIni = null;
                    Empleados empFin = null;

                    for (i = 0; i < valoresDeFiltrado.Count(); i++)
                    {
                        //if (valoresDeFiltrado[i]  instanceof TipoNomina);
                        Type type = valoresDeFiltrado[i].GetType();
                        if (valoresDeFiltrado[i].GetType().BaseType == typeof(TipoNomina))
                        {
                            periodicidad = ((TipoNomina)valoresDeFiltrado[i]).periodicidad;
                            datosFiltradoEmpleado.setClaveTipoNomina(((TipoNomina)valoresDeFiltrado[i]).clave);
                            datosFiltradoMovNom.setClaveTipoNomina(((TipoNomina)valoresDeFiltrado[i]).clave);
                            datosFiltradoAsistencia.setClaveTipoNomina(((TipoNomina)valoresDeFiltrado[i]).clave);
                        }
                        else if (valoresDeFiltrado[i].GetType().BaseType == typeof(TipoCorrida))
                        {
                            datosFiltradoMovNom.setClaveTipoCorrida(((TipoCorrida)valoresDeFiltrado[i]).clave);
                            datosFiltradoEmpleado.setClaveTipoCorrida(((TipoCorrida)valoresDeFiltrado[i]).clave);
                        }
                        else if (valoresDeFiltrado[i].GetType().BaseType == typeof(PeriodosNomina))
                        {
                            datosFiltradoEmpleado.setFechaInicio(((PeriodosNomina)valoresDeFiltrado[i]).fechaInicial);
                            datosFiltradoEmpleado.setFechaFin(((PeriodosNomina)valoresDeFiltrado[i]).fechaFinal);

                            datosFiltradoMovNom.setFechaInicio(((PeriodosNomina)valoresDeFiltrado[i]).fechaInicial);
                            datosFiltradoMovNom.setFechaFin(((PeriodosNomina)valoresDeFiltrado[i]).fechaFinal);

                            datosFiltradoAsistencia.setFechaInicio(((PeriodosNomina)valoresDeFiltrado[i]).fechaInicial);
                            datosFiltradoAsistencia.setFechaFin(((PeriodosNomina)valoresDeFiltrado[i]).fechaFinal);
                        }
                        else if (valoresDeFiltrado[i].GetType().BaseType == typeof(RegistroPatronal))
                        {
                            datosFiltradoEmpleado.setClaveRegistroPatronal(((RegistroPatronal)valoresDeFiltrado[i]).clave);
                        }
                        else if (valoresDeFiltrado[i].GetType().BaseType == typeof(CentroDeCosto))
                        {
                            datosFiltradoEmpleado.setClaveCentroCosto(((CentroDeCosto)valoresDeFiltrado[i]).clave);
                            datosFiltradoMovNom.setClaveCentroCosto(((CentroDeCosto)valoresDeFiltrado[i]).clave);
                            datosFiltradoAsistencia.setClaveCentroCosto(((CentroDeCosto)valoresDeFiltrado[i]).clave);
                        }
                        else if (valoresDeFiltrado[i].GetType().BaseType == typeof(Departamentos))
                        {
                            datosFiltradoEmpleado.setClaveDepartamento(((Departamentos)valoresDeFiltrado[i]).clave);
                        }
                        else if ((valoresDeFiltrado[i].GetType().BaseType == typeof(Empleados) == empleadoInicio))
                        {
                            empleadoInicio = false;
                            empIni = (Empleados)valoresDeFiltrado[i];
                            if (empIni.id == 0)
                            {
                                empIni = null;
                            }
                        }
                        else if (valoresDeFiltrado[i].GetType().BaseType == typeof(Empleados))
                        {
                            empFin = (Empleados)valoresDeFiltrado[i];
                            if (empFin.id == 0)
                            {
                                empFin = null;
                            }
                        }
                    }
                    if (empIni != null && empFin != null)
                    {
                        datosFiltradoEmpleado.setClaveInicioEmpleado(empIni.clave);
                        datosFiltradoEmpleado.setClaveFinEmpleado(empFin.clave);
                        datosFiltradoEmpleado.setidEmpleadoInicio(empIni.id);
                        datosFiltradoEmpleado.setidEmpleadoFin(empFin.id);
                    }
                    else if (empIni != null)
                    {
                        datosFiltradoEmpleado.setClaveInicioEmpleado(empIni.clave);
                        datosFiltradoEmpleado.setidEmpleadoInicio(empIni.id);
                    }
                    else if (empFin != null)
                    {
                        datosFiltradoEmpleado.setClaveFinEmpleado(empFin.clave);
                        datosFiltradoEmpleado.setidEmpleadoFin(empFin.id);
                    }
                }
                
                inicializaVariableMensaje();
                //setSession(dbContextMaestra.context);
                obtenerFactores(claveRazonSocial, periodicidad, dbContextMaestra);

                //conceptoNominaSubsidio = getConceptoNominaSubsidio(dbContextMaestra);
                //getSession().Database.BeginTransaction();

                //setSession(dbContext.context);
                //getSession().Database.BeginTransaction();

                List<object> datosEmpleado = construyeQueryDatosGlobalesEmpleados(datosFiltradoEmpleado, dbContext);
                if (datosEmpleado == null)
                {
                    return mensajeResultado;
                }
                if (datosEmpleado.Count() > 0)
                {
                    object[] valores;
                    string claveEmpleado;
                    for (i = 0; i < datosEmpleado.Count(); i++)
                    {
                        
                        valores = (object[])datosEmpleado[i];
                        valores[1] = obtenerPlazaEmpleaMovFull(JsonConvert.DeserializeObject<PlazasPorEmpleadosMov>(valores[1].ToString()), dbContext);
                        //valores[1] = calculaSueldoDiario(JsonConvert.DeserializeObject<PlazasPorEmpleadosMov>(valores[1].ToString()));
                        valores[1] = calculaSueldoDiario((PlazasPorEmpleadosMov)valores[1]);
                        datosEmpleado[i] = valores;
                    }
                    List<object> datosMovNomina = (List<Object>)construyeQueryDatosGlobalesMovNom(datosFiltradoMovNom, datosFiltradoEmpleado, dbContext);
                    if (datosMovNomina == null)
                    {
                        return mensajeResultado;
                    }
                    List<Object> datosAsistencias = (List<Object>)construyeQueryDatosGlobalesAsistencias(datosFiltradoAsistencia, datosFiltradoEmpleado, dbContext);
                    //****************************eliminar movimientos repetidos y suma bases afecta y resultado de movimiento*********************************************************/
                    if (datosMovNomina.Count() > 1)
                    {
                        i = 0;
                        int k, l;
                        int contEx = 0, total = datosMovNomina.Count();
                        object[] tmp;
                        Dictionary<string, object> val;
                        MovNomConcep mov1, mov2 = new MovNomConcep();
                        List<MovNomBaseAfecta> basesAfectaMov1 = new List<MovNomBaseAfecta>();
                        List<MovNomBaseAfecta> basesAfectaMov2 = new List<MovNomBaseAfecta>();
                        String claveEmp, claveEmpTemp;
                        while (i < datosMovNomina.Count())
                        {
                            valores = (object[])datosMovNomina[i];
                            MovNomConcep movTemp = obtenerMovNomConcepFull(JsonConvert.DeserializeObject<MovNomConcep>(valores[1].ToString()), dbContext);
                            //movTemp.movNomBaseAfecta = obtenerMovNomBaseAfecta(movTemp.id, dbContext);
                            //movTemp.movNomConceParam = obtenerMovNomConceParam(movTemp.id, dbContext);
                            mov1 = nuevaInstanciaMovNomina(movTemp);
                            claveEmp = (String)valores[0];
                            j = i + 1;
                            if (j < datosMovNomina.Count())
                            {
                                while (j < datosMovNomina.Count())
                                {
                                    tmp = (Object[])datosMovNomina[j];
                                    MovNomConcep movTemp2 = obtenerMovNomConcepFull(JsonConvert.DeserializeObject<MovNomConcep>(tmp[1].ToString()), dbContext);

                                    //movTemp2.movNomBaseAfecta = obtenerMovNomBaseAfecta(movTemp.id, dbContext);
                                    //movTemp2.movNomConceParam = obtenerMovNomConceParam(movTemp.id, dbContext);
                                    claveEmpTemp = (String)tmp[0];


                                    var json = JsonConvert.SerializeObject(tmp[1]);
                                    val = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                                    mov2 = movTemp2;
                                    if (claveEmpTemp.Equals(claveEmp, StringComparison.InvariantCultureIgnoreCase) & mov1.concepNomDefi.clave.Equals(mov2.concepNomDefi.clave, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        val["calculado"] = (mov1.calculado == null ? 0.0 : mov1.calculado) + (mov2.calculado == null ? 0.0 : mov2.calculado);
                                        val["resultado"] = (mov1.resultado == null ? 0.0 : mov1.resultado) + (mov2.resultado == null ? 0.0 : mov2.resultado);
                                        mov1.resultado = (mov1.resultado == null ? 0.0 : mov1.resultado) + (mov2.resultado == null ? 0.0 : mov2.resultado);
                                        mov1.calculado = (mov1.calculado == null ? 0.0 : mov1.calculado) + (mov2.calculado == null ? 0.0 : mov2.calculado);
                                        basesAfectaMov1 = mov1.movNomBaseAfecta == null ? new List<MovNomBaseAfecta>() : mov1.movNomBaseAfecta;
                                        basesAfectaMov2 = mov2.movNomBaseAfecta == null ? new List<MovNomBaseAfecta>() : mov2.movNomBaseAfecta;
                                        /**
                                         * *suma bases afecta*
                                        */
                                        if (basesAfectaMov1.Any())
                                        {
                                            if (basesAfectaMov2.Count() > 0)
                                            {
                                                mov1.movNomBaseAfecta = creaMovimBaseAfectar(basesAfectaMov2, mov1);
                                            }
                                        }
                                        else
                                        {
                                            if (basesAfectaMov2.Count() > 0)
                                            {
                                                for (l = 0; l < basesAfectaMov1.Count(); l++)
                                                {
                                                    for (k = 0; k < basesAfectaMov2.Count(); k++)
                                                    {
                                                        if (basesAfectaMov1[l].baseAfecConcepNom.id == basesAfectaMov2[k].baseAfecConcepNom.id)
                                                        {
                                                            basesAfectaMov1[l].resultado = (basesAfectaMov1[l].resultado == null ? 0.0 : basesAfectaMov1[l].resultado) + (basesAfectaMov2[k].resultado == null ? 0.0 : basesAfectaMov2[k].resultado);
                                                            basesAfectaMov1[l].resultadoExento = (basesAfectaMov1[l].resultadoExento == null ? 0.0 : basesAfectaMov1[l].resultadoExento) + (basesAfectaMov2[k].resultadoExento == null ? 0.0 : basesAfectaMov2[k].resultadoExento);
                                                            break;
                                                        }
                                                    }
                                                }
                                                mov1.movNomBaseAfecta = basesAfectaMov1;
                                            }
                                        }
                                        

                                        datosMovNomina.Remove(j);

                                        json = JsonConvert.SerializeObject(val);
                                       // val = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

                                        valores[1] = json;
                                        //datosMovNomina.Insert(i, valores);
                                        j++;
                                    }
                                    else
                                    {
                                        j++;
                                    }
                                }
                            }
                            i++;
                        }
                    }
                    //****************************agrupa informacion por empleado*********************************************************/
                    DatosParaTimbrar datoPorTimbrar = new DatosParaTimbrar(); ;
                    List<Asistencias> listAsistencias = new List<Asistencias>();
                    List<MovNomConcep> listMovNom = new List<MovNomConcep>();
                    Object[] valoresComp;
                    DatosPorEmpleado dpe = new DatosPorEmpleado();
                    for (i = 0; i < datosEmpleado.Count(); i++)
                    {
                        datoPorTimbrar = new DatosParaTimbrar();
                        valores = (Object[])datosEmpleado[i];
                        dpe = new DatosPorEmpleado();
                        dpe.plazasPorEmpleadosMov = (PlazasPorEmpleadosMov)valores[1];
                        dpe.salarioDiarioIntegrado = (Double)valores[2];
                        dpe.fechaIngreso = (DateTime)valores[3];
                        dpe.detalleReciboPeriodo = (String)valores[4];
                        dpe.detalleReciboCorrida = (String)valores[5];

                       /* dpe.detalleReciboPeriodo = (String)valores[3];
                        dpe.detalleReciboCorrida = (String)valores[4];
                        dpe.fechaIngreso = (DateTime)valores[5];*/
                        dpe.detalleReciboNomina = (String)valores[6];
                        datoPorTimbrar.datosPorEmpleado = dpe;
                        claveEmpleado = (String)valores[0];
                        if (datosMovNomina.Count() > 0)
                        {
                            j = 0;
                            listMovNom = new List<MovNomConcep>();
                            while (j < datosMovNomina.Count())
                            {
                                valoresComp = (Object[])datosMovNomina[j];
                                if (claveEmpleado.Equals(valoresComp[0].ToString(), StringComparison.InvariantCultureIgnoreCase))
                                {
                                    listMovNom.Add(JsonConvert.DeserializeObject<MovNomConcep>(valoresComp[1].ToString()));
                                    datosMovNomina.RemoveAt(j);
                                }
                                else
                                {
                                    j++;
                                }
                            }
                            datoPorTimbrar.movimientos = listMovNom;
                        }
                        if (datosAsistencias.Count() > 0)
                        {
                            j = 0;
                            listAsistencias = new List<Asistencias>();
                            while (j < datosAsistencias.Count())
                            {
                                valoresComp = (Object[])datosAsistencias[j];
                                if (claveEmpleado.Equals(valoresComp[0].ToString(), StringComparison.InvariantCultureIgnoreCase))
                                {
                                    Asistencias asistencia = obtenerAsistenciaFull(JsonConvert.DeserializeObject<Asistencias>(valoresComp[1].ToString()), dbContext);
                                    listAsistencias.Add(asistencia);
                                    datosAsistencias.RemoveAt(j);
                                }
                                else
                                {
                                    j++;
                                }
                            }
                            datoPorTimbrar = agregaIncapacidadesHorasExtras(listAsistencias, datoPorTimbrar);
                        }
                        if (datoPorTimbrar.movimientos == null ? false : datoPorTimbrar.movimientos.Any())
                        {
                            datosParaTimbrar.Add(datoPorTimbrar);
                        }
                    }
                }
                nullVariablesGlobales();
                mensajeResultado.resultado = datosParaTimbrar;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                //getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("generaDatosParaTimbrado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        private DatosParaTimbrar agregaIncapacidadesHorasExtras(List<Asistencias> listAsistencias, DatosParaTimbrar datosParaTimbrar)
        {
            if (datosParaTimbrar == null)
            {
                datosParaTimbrar = new DatosParaTimbrar();
            }
            int diasIncapacidadEnfermedad = 0, diasIncapacidadAccidente = 0, diasIncapacidadMaternidad = 0, diasHorasDobles = 0, diasHorasTriples = 0, diasTiempoExtra = 0;
            Double? hrsExtraDoble = 0.0, hrsExtraTriple = 0.0;
            ///tiempoExtra = 0.0;
            int x;
            Asistencias asistenciaEnfermedad = null, asistenciaAccidente = null, asistenciaMaternidad = null, asistenciaHrsDobles = null, asistenciaHrsTriples = null;
            if (listAsistencias != null)
            {
                for (x = 0; x < listAsistencias.Count(); x++)
                {
                    switch (Convert.ToInt32(listAsistencias[x].excepciones.clave))
                    {
                        case 6://IncapacidadPorEnfermedad = "6";
                            asistenciaEnfermedad = listAsistencias[x];
                            diasIncapacidadEnfermedad++;
                            break;
                        case 7://IncapacidadPorAccidente = "7";
                            asistenciaAccidente = listAsistencias[x];
                            diasIncapacidadAccidente++;
                            break;
                        case 8://IncapacidadPorMaternidad = "8";
                            asistenciaMaternidad = listAsistencias[x];
                            diasIncapacidadMaternidad++;
                            asistenciaEnfermedad = listAsistencias[x];
                            break;
                        case 14://ExtraDoble = "14";
                            asistenciaHrsDobles = listAsistencias[x];
                            diasHorasDobles++;
                            hrsExtraDoble += listAsistencias[x].cantidad;
                            break;
                        case 15://ExtraTriple = "15";
                            asistenciaHrsTriples = listAsistencias[x];
                            diasHorasTriples++;
                            hrsExtraTriple += listAsistencias[x].cantidad;
                            break;
                    }
                }
                List<DatosHorasExtras> listHrsExtras = new List<DatosHorasExtras>();
                DatosHorasExtras extra;
                if (asistenciaHrsDobles != null)
                {
                    extra = new DatosHorasExtras();
                    extra.asistencia = asistenciaHrsDobles;
                    extra.dias = diasHorasDobles;
                    extra.hrsExtas = Convert.ToInt32(hrsExtraDoble);
                    listHrsExtras.Add(extra);
                }
                if (asistenciaHrsTriples != null)
                {
                    extra = new DatosHorasExtras();
                    extra.asistencia = asistenciaHrsTriples;
                    extra.dias = diasHorasTriples;
                    extra.hrsExtas = Convert.ToInt32(hrsExtraTriple);
                    listHrsExtras.Add(extra);
                }
                List<DatosIncapacidades> listIncapacidades = new List<DatosIncapacidades>();
                DatosIncapacidades incapacidad;
                if (asistenciaAccidente != null)
                {
                    incapacidad = new DatosIncapacidades();
                    incapacidad.asistencia = asistenciaAccidente;
                    incapacidad.dias = diasIncapacidadAccidente;
                    listIncapacidades.Add(incapacidad);
                }
                if (asistenciaEnfermedad != null)
                {
                    incapacidad = new DatosIncapacidades();
                    incapacidad.asistencia = asistenciaEnfermedad;
                    incapacidad.dias = diasIncapacidadEnfermedad;
                    listIncapacidades.Add(incapacidad);
                }
                if (asistenciaMaternidad != null)
                {
                    incapacidad = new DatosIncapacidades();
                    incapacidad.asistencia = asistenciaMaternidad;
                    incapacidad.dias = diasIncapacidadMaternidad;
                    listIncapacidades.Add(incapacidad);
                }
                datosParaTimbrar.datosHorasExtras = listHrsExtras;
                datosParaTimbrar.datosIncapacidades = listIncapacidades;
            }
            return datosParaTimbrar;
        }
        private MovNomConcep nuevaInstanciaMovNomina(MovNomConcep movNomConcep)
        {
            MovNomConcep nueva = new MovNomConcep();
            nueva.calculado = movNomConcep.calculado;
            nueva.centroDeCosto = movNomConcep.centroDeCosto;
            nueva.centroDeCosto_ID = movNomConcep.centroDeCosto_ID;
            nueva.concepNomDefi = movNomConcep.concepNomDefi;
            nueva.concepNomDefi_ID = movNomConcep.concepNomDefi_ID;
            nueva.ejercicio = movNomConcep.ejercicio;
            nueva.empleados = movNomConcep.empleados;
            nueva.empleado_ID = movNomConcep.empleado_ID;
            nueva.fechaCierr = movNomConcep.fechaCierr;
            nueva.fechaIni = movNomConcep.fechaIni;
            nueva.finiqLiquidCncNom = movNomConcep.finiqLiquidCncNom;
            nueva.finiqLiquidCncNom_ID = movNomConcep.finiqLiquidCncNom_ID;
            nueva.id = 0;
            nueva.IsEnBD = movNomConcep.IsEnBD;
            nueva.mes = movNomConcep.mes;
            nueva.movNomBaseAfecta = creaMovimBaseAfectar(movNomConcep.movNomBaseAfecta, nueva);
            nueva.movNomConceParam = creaMovNomConceParam(nueva, movNomConcep.movNomConceParam);
            nueva.numMovParticion = movNomConcep.numMovParticion;
            nueva.numero = movNomConcep.numero;
            nueva.ordenId = movNomConcep.ordenId;
            nueva.periodosNomina = movNomConcep.periodosNomina;
            nueva.periodosNomina_ID = movNomConcep.periodosNomina_ID;
            nueva.plazasPorEmpleado = movNomConcep.plazasPorEmpleado;
            nueva.plazasPorEmpleado_ID = movNomConcep.plazasPorEmpleado_ID;
            nueva.razonesSociales = movNomConcep.razonesSociales;
            nueva.razonesSociales_ID = movNomConcep.razonesSociales_ID;
            nueva.resultado = movNomConcep.resultado;
            nueva.tipoCorrida = movNomConcep.tipoCorrida;
            nueva.tipoCorrida_ID = movNomConcep.tipoCorrida_ID;
            nueva.tipoNomina = movNomConcep.tipoNomina;
            nueva.tipoNomina_ID = movNomConcep.tipoNomina_ID;
            nueva.uso = movNomConcep.uso;
            return nueva;
        }
        private List<MovNomConceParam> creaMovNomConceParam(MovNomConcep mnc, List<MovNomConceParam> parametros)
        {
            if (parametros == null)
            {
                return null;
            }
            List<MovNomConceParam> movparametros = new List<MovNomConceParam>();
            MovNomConceParam m;
            foreach (MovNomConceParam movParam in parametros)
            {
                m = new MovNomConceParam();
                m.movNomConcep = mnc;
                m.movNomConcep_ID = mnc.id;
                m.paraConcepDeNom = movParam.paraConcepDeNom;
                m.paraConcepDeNom_ID = movParam.paraConcepDeNom_ID;
                m.valor = movParam.valor;
                movparametros.Add(m);
            }
            return movparametros;
        }
        private List<MovNomBaseAfecta> creaMovimBaseAfectar(List<MovNomBaseAfecta> baseAfecConcepNominas, MovNomConcep mnc)
        {
            if (baseAfecConcepNominas == null)
            {
                return null;
            }
            List<MovNomBaseAfecta> movNominaBaseAfectas = new List<MovNomBaseAfecta>(0);
            MovNomBaseAfecta m;
            foreach (MovNomBaseAfecta afecConcepNom in baseAfecConcepNominas)
            {
                m = new MovNomBaseAfecta();
                m.baseAfecConcepNom = afecConcepNom.baseAfecConcepNom;
                m.baseAfecConcepNom_ID = afecConcepNom.baseAfecConcepNom_ID;
                m.movNomConcep = mnc;
                m.movNomConcep_ID = mnc.id;
                m.uso = afecConcepNom.uso;
                m.resultado = afecConcepNom.resultado;
                m.resultadoExento = afecConcepNom.resultadoExento;
                movNominaBaseAfectas.Add(m);
            }
            return movNominaBaseAfectas;
        }
        private List<Object> construyeQueryDatosGlobalesAsistencias(DatosFiltradoAsistencias datosFiltradoAsistencias, DatosFiltradoEmpleados datosFiltradoEmpleado, DBContextAdapter dBContextSimple)
        {
            List<Object> datosAsistencias = new List<object>();
            List<Object> datosAsistenciasAux = new List<object>();
            DateTime fechaInicio = datosFiltradoEmpleado.getFechaInicio().GetValueOrDefault();
            DateTime fechaFin = datosFiltradoEmpleado.getFechaFin().GetValueOrDefault();
            string claveTipoCorrida = datosFiltradoEmpleado.getClaveTipoCorrida();
            string claveTipoNomina = datosFiltradoEmpleado.getClaveTipoNomina();
            string claveCentroCosto = datosFiltradoEmpleado.getClaveCentroCosto();
            string claveDepartamento = datosFiltradoEmpleado.getClaveDepartamento();
            string claveRazonSocial = datosFiltradoEmpleado.getClaveRazonSocial();
            string claveRegistroPatronal = datosFiltradoEmpleado.getClaveRegistroPatronal();
            string claveFinEmpleado = datosFiltradoEmpleado.getClaveFinEmpleado();
            string claveInicioEmpleado = datosFiltradoEmpleado.getClaveInicioEmpleado();
            string claveRazonSocialAsis = datosFiltradoAsistencias.getClaveRazonSocial();
            // string claveTipoCorridaAsis = datosFiltradoAsistencias.getClaveTipoCorrida();
            string claveTipoNominaAsis = datosFiltradoAsistencias.getClaveTipoNomina();
            string claveCentroCostoAsis = datosFiltradoAsistencias.getClaveCentroCosto();
            DateTime fechaInicioAsis = datosFiltradoAsistencias.getFechaInicio().GetValueOrDefault();
            DateTime fechaFinAsis = datosFiltradoAsistencias.getFechaFin().GetValueOrDefault();
            decimal idEmpIni = datosFiltradoEmpleado.getidEmpleadoInicio();
            decimal idEmpFin = datosFiltradoEmpleado.getidEmpleadoFin();

            string claveExcepcionExtraDoble = ClavesParametrosModulos.claveExcepcionExtraDoble.ToString();
            string claveExcepcionExtraTriple = ClavesParametrosModulos.claveExcepcionExtraTriple.ToString();
            string claveExcepcionIncapacidadPorAccidente = ClavesParametrosModulos.claveExcepcionIncapacidadPorAccidente.ToString();
            string claveExcepcionIncapacidadPorEnfermedad = ClavesParametrosModulos.claveExcepcionIncapacidadPorEnfermedad.ToString();
            string claveExcepcionIncapacidadPorMaternidad = ClavesParametrosModulos.claveExcepcionIncapacidadPorMaternidad.ToString();
            var querytipocorrida = (from cfdiEmp in dBContextSimple.context.Set<CFDIEmpleado>()
                                    join cfdiPPM in dBContextSimple.context.Set<PlazasPorEmpleadosMov>() on cfdiEmp.plazaPorEmpleadoMov_ID equals cfdiPPM.id into cfdiPPM_join
                                    from cfdiPPM in cfdiPPM_join.DefaultIfEmpty()
                                    join recibo in dBContextSimple.context.Set<CFDIRecibo>() on cfdiEmp.cfdiRecibo_ID equals recibo.id into recibo_join
                                    from recibo in recibo_join.DefaultIfEmpty()
                                    join cfdiPeriodo in dBContextSimple.context.Set<PeriodosNomina>() on cfdiEmp.periodosNomina_ID equals cfdiPeriodo.id into cfdiPeriodo_join
                                    from cfdiPeriodo in cfdiPeriodo_join.DefaultIfEmpty()
                                    join cfdiPP in dBContextSimple.context.Set<PlazasPorEmpleado>() on cfdiPPM.plazasPorEmpleado_ID equals cfdiPP.id into cfdiPP_join
                                    from cfdiPP in cfdiPP_join.DefaultIfEmpty()
                                    join em in dBContextSimple.context.Set<Empleados>() on cfdiPP.empleados_ID equals em.id into em_join
                                    from em in em_join.DefaultIfEmpty()
                                    join razonSocial in dBContextSimple.context.Set<RazonesSociales>() on cfdiPP.razonesSociales_ID equals razonSocial.id into razonSocial_join
                                    from razonSocial in razonSocial_join.DefaultIfEmpty()
                                    where
                                    recibo.statusTimbrado == StatusTimbrado.TIMBRADO &&
                                    ((cfdiPeriodo.fechaInicial >= fechaInicioAsis && cfdiPeriodo.fechaInicial <= fechaFinAsis) ||
                                    (cfdiPeriodo.fechaFinal >= fechaInicioAsis && cfdiPeriodo.fechaFinal <= fechaFinAsis)) &&
                                    em.razonesSociales_ID == razonSocial.id
                                    select new
                                    {
                                        em,
                                        cfdiPeriodo
                                    });
            if (datosFiltradoEmpleado.getClaveTipoCorrida().Any())
            {
                querytipocorrida = (from subquery in querytipocorrida
                                    where subquery.cfdiPeriodo.tipoCorrida.clave == claveTipoCorrida
                                    select subquery);
            }

            var query = (from x0 in dBContextSimple.context.Set<Asistencias>()
                         join x1 in dBContextSimple.context.Set<Empleados>() on x0.empleados_ID equals x1.id into x1_join
                         from x1 in x1_join.DefaultIfEmpty()
                         join x2 in dBContextSimple.context.Set<RazonesSociales>() on x0.razonesSociales_ID equals x2.id into x2_join
                         from x2 in x2_join.DefaultIfEmpty()
                         join x3 in dBContextSimple.context.Set<TipoNomina>() on x0.tipoNomina_ID equals x3.id into x3_join
                         from x3 in x3_join.DefaultIfEmpty()
                         join x4 in dBContextSimple.context.Set<Excepciones>() on x0.excepciones_ID equals x4.id into x4_join
                         from x4 in x4_join.DefaultIfEmpty()
                         join x5 in dBContextSimple.context.Set<PeriodosNomina>() on x0.periodosNomina_ID equals x5.id into x5_join
                         from x5 in x5_join.DefaultIfEmpty()
                         from ppm in dBContextSimple.context.Set<PlazasPorEmpleadosMov>()
                         join nomina in dBContextSimple.context.Set<TipoNomina>() on ppm.tipoNomina_ID equals nomina.id into nomina_join
                         from nomina in nomina_join.DefaultIfEmpty()
                         join pp in dBContextSimple.context.Set<PlazasPorEmpleado>() on ppm.plazasPorEmpleado_ID equals pp.id into pp_join
                         from pp in pp_join.DefaultIfEmpty()
                         join razonSocial in dBContextSimple.context.Set<RazonesSociales>() on pp.razonesSociales_ID equals razonSocial.id into razonSocial_join
                         from razonSocial in razonSocial_join.DefaultIfEmpty()
                         join empleado in dBContextSimple.context.Set<Empleados>() on pp.empleados_ID equals empleado.id into empleado_join
                         from empleado in empleado_join.DefaultIfEmpty()
                         from sdi in dBContextSimple.context.Set<SalariosIntegrados>()
                         join sdiEmp in dBContextSimple.context.Set<Empleados>() on sdi.empleados_ID equals sdiEmp.id into sdiEmp_join
                         from sdiEmp in sdiEmp_join.DefaultIfEmpty()
                         where
                            x1.id == empleado.id &&
                            sdiEmp.id == empleado.id &&
                              !
                            (from sub in querytipocorrida.AsEnumerable()
                             group sub.em by new
                             {
                                 sub.em.id
                             } into g
                             select new
                             {
                                 Column1 =
                               g.Count() == 0 ? 0 : g.Key.id
                             }).Contains(new { Column1 = x1.id }) &&
                             sdi.fecha == (from s0 in dBContextSimple.context.Set<SalariosIntegrados>()
                                           join s0Emp in dBContextSimple.context.Set<Empleados>() on s0.empleados_ID equals s0Emp.id into s0Emp_join
                                           from s0Emp in s0Emp_join.DefaultIfEmpty()
                                           where
                                             s0.fecha <= fechaFin &&
                                             s0Emp.id == empleado.id
                                           select new
                                           {
                                               s0.fecha
                                           }).Max(p => p.fecha) &&
                             (new decimal[] { ppm.id }).Contains((from pem in dBContextSimple.context.Set<PlazasPorEmpleadosMov>()
                                                                  where
                                                                  pem.plazasPorEmpleado.empleados.id == empleado.id &&
                                                                  fechaFin >= pem.fechaInicial &&
                                                                  pem.plazasPorEmpleado.fechaFinal > fechaFin
                                                                  select new
                                                                  {
                                                                      pem.id
                                                                  }).Max(p => p.id))
                         select new
                         {
                             nomina,
                             razonSocial,
                             ppm,
                             pp,
                             empleado,
                             x0,
                             x4,
                             x5
                         });
            if (datosFiltradoEmpleado.getClaveTipoNomina().Any())
            {
                query = (from subquery in query
                         where subquery.nomina.clave == claveTipoNomina
                         select subquery);
            }
            if (datosFiltradoEmpleado.getClaveRazonSocial().Any())
            {
                query = (from subquery in query
                         where subquery.razonSocial.clave == claveRazonSocial
                         select subquery);
            }
            if (datosFiltradoEmpleado.getClaveCentroCosto().Any())
            {
                query = (from subquery in query
                         join cc in dBContextSimple.context.Set<CentroDeCosto>() on subquery.ppm.centroDeCosto.id equals cc.id into cc_join
                         from cc in cc_join.DefaultIfEmpty()
                         where cc.clave == claveCentroCosto
                         select subquery);
            }
            if (datosFiltradoEmpleado.getClaveDepartamento().Any())
            {
                query = (from subquery in query
                         join depto in dBContextSimple.context.Set<Departamentos>() on subquery.ppm.departamentos.id equals depto.id into depto_join
                         from depto in depto_join.DefaultIfEmpty()
                         where depto.clave == claveDepartamento
                         select subquery);
            }
            if (datosFiltradoEmpleado.getClaveRegistroPatronal().Any())
            {
                query = (from subquery in query
                         join rp in dBContextSimple.context.Set<RegistroPatronal>() on subquery.pp.registroPatronal.id equals rp.id into rp_join
                         from rp in rp_join.DefaultIfEmpty()
                         where rp.clave == claveRegistroPatronal
                         select subquery);
            }
            if (datosFiltradoEmpleado.getClaveInicioEmpleado().Any() && datosFiltradoEmpleado.getClaveFinEmpleado().Any())
            {
                query = (from subquery in query
                         where(subquery.empleado.id) >= (idEmpIni) &&
                         (subquery.empleado.id) <= (idEmpFin)
                         select subquery);
            }
            else if (datosFiltradoEmpleado.getClaveInicioEmpleado().Any())
            {
                query = (from subquery in query
                         where (subquery.empleado.id) >= (idEmpFin)
                         select subquery);
            }
            else if (datosFiltradoEmpleado.getClaveFinEmpleado().Any())
            {
                query = (from subquery in query
                         where (subquery.empleado.id) <= (idEmpFin)
                         select subquery);
            }

            query = (from subquery in query
                     where subquery.x4.clave == claveExcepcionExtraDoble ||
                     subquery.x4.clave == claveExcepcionExtraTriple ||
                     subquery.x4.clave == claveExcepcionIncapacidadPorAccidente ||
                     subquery.x4.clave == claveExcepcionIncapacidadPorEnfermedad ||
                     subquery.x4.clave == claveExcepcionIncapacidadPorMaternidad
                     select subquery);
            if (datosFiltradoAsistencias.getClaveRazonSocial().Any())
            {
                query = (from subquery in query
                         where subquery.razonSocial.clave == claveRazonSocialAsis
                         select subquery);
            }
            if (datosFiltradoAsistencias.getClaveTipoNomina().Any())
            {
                query = (from subquery in query
                         where subquery.nomina.clave == claveTipoNominaAsis
                         select subquery);
            }
            if (datosFiltradoAsistencias.getFechaFin() != null & datosFiltradoAsistencias.getFechaInicio() != null)
            {
                query = (from subquery in query
                         where subquery.x5.fechaInicial >= fechaInicioAsis &&
                         subquery.x5.fechaFinal <= fechaFinAsis
                         select subquery);
                if (datosFiltradoEmpleado.getClaveTipoCorrida().Any())
                {
                    query = (from subquery in query
                             where subquery.x5.tipoCorrida.clave == "PER"
                             select subquery);
                }
            }
            if (datosFiltradoAsistencias.getClaveCentroCosto().Any())
            {
                query = (from subquery in query
                         join x8 in dBContextSimple.context.Set<CentroDeCosto>() on subquery.x0.centroDeCosto.id equals x8.id into x8_join
                         from x8 in x8_join.DefaultIfEmpty()
                         where x8.clave == claveCentroCostoAsis
                         select subquery);
            }

            var query2 = (from subquery in query
                          orderby (subquery.empleado.clave == null ? "" : subquery.empleado.clave)
                          select new
                          {
                              empleado_clave = subquery.empleado.clave == null ? "" : subquery.empleado.clave,
                              x0 = new
                              {
                                  subquery.x0.cantidad,
                                  subquery.x0.centroDeCosto_ID,
                                  subquery.x0.empleados_ID,
                                  excepciones = new
                                  {
                                      subquery.x0.excepciones.clave,
                                      subquery.x0.excepciones.concepNomDefi_ID,
                                      subquery.x0.excepciones.excepcion,
                                      subquery.x0.excepciones.id,
                                      subquery.x0.excepciones.naturaleza,
                                      subquery.x0.excepciones.orden,
                                      subquery.x0.excepciones.tipoDatoExcepcion,
                                      subquery.x0.excepciones.unico
                                  },
                                  subquery.x0.excepciones_ID,
                                  subquery.x0.fecha,
                                  subquery.x0.id,
                                  subquery.x0.jornada,
                                  subquery.x0.ordenId,
                                  subquery.x0.periodosNomina_ID,
                                  subquery.x0.razonesSociales_ID,
                                  subquery.x0.tipoNomina_ID,
                                  subquery.x0.tipoPantalla
                              }
                          }).Distinct();
            datosAsistenciasAux = query2.ToList<object>();
            for (int i = 0; i < datosAsistenciasAux.Count; i++)
            {
                var json = JsonConvert.SerializeObject(datosAsistenciasAux[i]);
                Dictionary<string, object> valores = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                int cont = 0;
                object[] datos = new object[valores.Count()];
                foreach (var item in valores)
                {
                    datos[cont] = item.Value;
                    cont++;
                }
                datosAsistencias.Add(datos);
            }
            return datosAsistencias;
        }
        private List<Object> construyeQueryDatosGlobalesMovNom(DatosFiltradoMovNom datosFiltradoMovNom, DatosFiltradoEmpleados datosFiltradoEmpleado, DBContextAdapter dBContextSimple)
        {
            List<Object> datosMovimientos = new List<object>();
            List<Object> datosMovimientosAux = new List<object>();
            DateTime fechaInicio = datosFiltradoEmpleado.getFechaInicio().GetValueOrDefault();
            DateTime fechaFin = datosFiltradoEmpleado.getFechaFin().GetValueOrDefault();
            string claveTipoCorrida = datosFiltradoEmpleado.getClaveTipoCorrida();
            string claveTipoNomina = datosFiltradoEmpleado.getClaveTipoNomina();
            string claveCentroCosto = datosFiltradoEmpleado.getClaveCentroCosto();
            string claveDepartamento = datosFiltradoEmpleado.getClaveDepartamento();
            string claveRazonSocial = datosFiltradoEmpleado.getClaveRazonSocial();
            string claveRegistroPatronal = datosFiltradoEmpleado.getClaveRegistroPatronal();
            string claveFinEmpleado = datosFiltradoEmpleado.getClaveFinEmpleado();
            string claveInicioEmpleado = datosFiltradoEmpleado.getClaveInicioEmpleado();
            string claveRazonSocialMovNom = datosFiltradoMovNom.getClaveRazonSocial();
            string claveTipoCorridaMovNom = datosFiltradoMovNom.getClaveTipoCorrida();
            string claveTipoNominaMovNom = datosFiltradoMovNom.getClaveTipoNomina();
            string claveCentroCostoMovNom = datosFiltradoMovNom.getClaveCentroCosto();
            decimal idEmpIni = datosFiltradoEmpleado.getidEmpleadoInicio();
            decimal idEmpFin = datosFiltradoEmpleado.getidEmpleadoFin();
            DateTime fechaInicioMovNom = datosFiltradoMovNom.getFechaInicio().GetValueOrDefault();
            DateTime fechaFinMovNom = datosFiltradoMovNom.getFechaFin().GetValueOrDefault();
            var querytipocorrida = (from cfdiEmp in dBContextSimple.context.Set<CFDIEmpleado>()
                                    join cfdiPPM in dBContextSimple.context.Set<PlazasPorEmpleadosMov>() on cfdiEmp.plazaPorEmpleadoMov_ID equals cfdiPPM.id into cfdiPPM_join
                                    from cfdiPPM in cfdiPPM_join.DefaultIfEmpty()
                                    join recibo in dBContextSimple.context.Set<CFDIRecibo>() on cfdiEmp.cfdiRecibo_ID equals recibo.id into recibo_join
                                    from recibo in recibo_join.DefaultIfEmpty()
                                    join cfdiPeriodo in dBContextSimple.context.Set<PeriodosNomina>() on cfdiEmp.periodosNomina_ID equals cfdiPeriodo.id into cfdiPeriodo_join
                                    from cfdiPeriodo in cfdiPeriodo_join.DefaultIfEmpty()
                                    join cfdiPP in dBContextSimple.context.Set<PlazasPorEmpleado>() on cfdiPPM.plazasPorEmpleado_ID equals cfdiPP.id into cfdiPP_join
                                    from cfdiPP in cfdiPP_join.DefaultIfEmpty()
                                    join em in dBContextSimple.context.Set<Empleados>() on cfdiPP.empleados_ID equals em.id into em_join
                                    from em in em_join.DefaultIfEmpty()
                                    join razonSocial in dBContextSimple.context.Set<RazonesSociales>() on cfdiPP.razonesSociales_ID equals razonSocial.id into razonSocial_join
                                    from razonSocial in razonSocial_join.DefaultIfEmpty()
                                    where
                                      recibo.statusTimbrado == StatusTimbrado.TIMBRADO &&
                                      ((cfdiPeriodo.fechaInicial >= fechaInicioMovNom &&
                                       cfdiPeriodo.fechaInicial <= fechaFinMovNom) ||
                                      (cfdiPeriodo.fechaFinal >= fechaInicioMovNom &&
                                       cfdiPeriodo.fechaFinal <= fechaFinMovNom)) &&
                                      em.razonesSociales_ID == razonSocial.id
                                    select new
                                    {
                                        em,
                                        cfdiPeriodo
                                    });
            if (!datosFiltradoEmpleado.getClaveTipoCorrida().Any())
            {
                querytipocorrida = (from subquery in querytipocorrida
                                    where subquery.cfdiPeriodo.tipoCorrida.clave == claveTipoCorrida
                                    select subquery);
            }

            var query = (from x0 in dBContextSimple.context.Set<MovNomConcep>()
                         join x1 in dBContextSimple.context.Set<Empleados>() on x0.empleado_ID equals x1.id into x1_join
                         from x1 in x1_join.DefaultIfEmpty()
                         join x2 in dBContextSimple.context.Set<RazonesSociales>() on x0.razonesSociales_ID equals x2.id into x2_join
                         from x2 in x2_join.DefaultIfEmpty()
                         join x3 in dBContextSimple.context.Set<TipoNomina>() on x0.tipoNomina_ID equals x3.id into x3_join
                         from x3 in x3_join.DefaultIfEmpty()
                         join x4 in dBContextSimple.context.Set<TipoCorrida>() on x0.tipoCorrida_ID equals x4.id into x4_join
                         from x4 in x4_join.DefaultIfEmpty()
                         join x5 in dBContextSimple.context.Set<PeriodosNomina>() on x0.periodosNomina_ID equals x5.id into x5_join
                         from x5 in x5_join.DefaultIfEmpty()
                         join x6 in dBContextSimple.context.Set<ConcepNomDefi>() on x0.concepNomDefi_ID equals x6.id into x6_join
                         from x6 in x6_join.DefaultIfEmpty()
                         join x11 in dBContextSimple.context.Set<Empleados>() on x0.empleado_ID equals x11.id into x11_join
                         from x11 in x11_join.DefaultIfEmpty()
                         join x7 in dBContextSimple.context.Set<PeriodosNomina>() on x0.periodosNomina_ID equals x7.id into x7_join
                         from x7 in x7_join.DefaultIfEmpty()
                         from ppm in dBContextSimple.context.Set<PlazasPorEmpleadosMov>()
                         join nomina in dBContextSimple.context.Set<TipoNomina>() on ppm.tipoNomina_ID equals nomina.id into nomina_join
                         from nomina in nomina_join.DefaultIfEmpty()
                         join pp in dBContextSimple.context.Set<PlazasPorEmpleado>() on ppm.plazasPorEmpleado_ID equals pp.id into pp_join
                         from pp in pp_join.DefaultIfEmpty()
                         join razonSocial in dBContextSimple.context.Set<RazonesSociales>() on pp.razonesSociales_ID equals razonSocial.id into razonSocial_join
                         from razonSocial in razonSocial_join.DefaultIfEmpty()
                         join empleado in dBContextSimple.context.Set<Empleados>() on pp.empleados_ID equals empleado.id into empleado_join
                         from empleado in empleado_join.DefaultIfEmpty()
                         from sdi in dBContextSimple.context.Set<SalariosIntegrados>()
                         join sdiEmp in dBContextSimple.context.Set<Empleados>() on sdi.empleados_ID equals sdiEmp.id into sdiEmp_join
                         from sdiEmp in sdiEmp_join.DefaultIfEmpty()
                         where
                           x1.id == empleado.id &&
                           sdiEmp.id == empleado.id &&
                           !
                           (from sub in querytipocorrida.AsEnumerable()
                            group sub.em by new
                            {
                                sub.em.id
                            } into g

                            select new
                            {
                                Column1 =
                              g.Count() == 0 ? 0 : g.Key.id
                            }).Contains(new { Column1 = x1.id }) &&
                            sdi.fecha == (from s0 in dBContextSimple.context.Set<SalariosIntegrados>()
                                          join s0emp in dBContextSimple.context.Set<Empleados>() on s0.empleados_ID equals s0emp.id into s0emp_join
                                          from s0emp in s0emp_join.DefaultIfEmpty()
                                          where
                                            s0.fecha <= fechaFin &&
                                            s0emp.id == empleado.id
                                          select new
                                          {
                                              s0.fecha
                                          }).Max(p => p.fecha) &&
                           (new List<decimal> { ppm.id }).Contains((from pem in dBContextSimple.context.Set<PlazasPorEmpleadosMov>()
                                                                    where
                                                                      pem.plazasPorEmpleado.empleados.id == empleado.id &&
                                                                      fechaFin >= pem.fechaInicial &&
                                                                      pem.plazasPorEmpleado.fechaFinal > fechaFin
                                                                    select new
                                                                    {
                                                                        pem.id
                                                                    }).Max(p => p.id))
                         select new
                         {
                             nomina,
                             razonSocial,
                             ppm,
                             pp,
                             empleado,
                             x0,
                             x4,
                             x5,
                             x6,
                         });
            if (datosFiltradoEmpleado.getClaveTipoNomina().Any())
            {
                query = (from subquery in query
                         where subquery.nomina.clave == claveTipoNomina
                         select subquery);
            }
            if (datosFiltradoEmpleado.getClaveRazonSocial().Any())
            {
                query = (from subquery in query
                         where subquery.razonSocial.clave == claveRazonSocial
                         select subquery);
            }
            if (datosFiltradoEmpleado.getClaveCentroCosto().Any())
            {
                query = (from subquery in query
                         join cc in dBContextSimple.context.Set<CentroDeCosto>() on subquery.ppm.centroDeCosto.id equals cc.id into cc_join
                         from cc in cc_join.DefaultIfEmpty()
                         where cc.clave == claveCentroCosto
                         select subquery);
            }
            if (datosFiltradoEmpleado.getClaveDepartamento().Any())
            {
                query = (from subquery in query
                         join depto in dBContextSimple.context.Set<Departamentos>() on subquery.ppm.departamentos.id equals depto.id into depto_join
                         from depto in depto_join.DefaultIfEmpty()
                         where depto.clave == claveDepartamento
                         select subquery);
            }
            if (datosFiltradoEmpleado.getClaveRegistroPatronal().Any())
            {
                query = (from subquery in query
                         join rp in dBContextSimple.context.Set<RegistroPatronal>() on subquery.pp.registroPatronal.id equals rp.id into rp_join
                         from rp in rp_join.DefaultIfEmpty()
                         where rp.clave == claveRegistroPatronal
                         select subquery);
            }
            if (datosFiltradoEmpleado.getClaveInicioEmpleado().Any() && datosFiltradoEmpleado.getClaveFinEmpleado().Any())
            {
                query = (from subquery in query
                         where (subquery.empleado.id) >= (idEmpIni) &&
                         (subquery.empleado.id) <= (idEmpFin)
                         select subquery);
            }
            else if (datosFiltradoEmpleado.getClaveInicioEmpleado().Any())
            {
                query = (from subquery in query
                         where(subquery.empleado.id) >= (idEmpIni)
                         select subquery);
            }
            else if (datosFiltradoEmpleado.getClaveFinEmpleado().Any())
            {
                query = (from subquery in query
                         where (subquery.empleado.id) <= (idEmpFin)
                         select subquery);
            }
            query = (from subquery in query
                     where subquery.x6.naturaleza == Naturaleza.PERCEPCION ||
                     subquery.x6.naturaleza == Naturaleza.DEDUCCION ||
                     subquery.x6.formulaConcepto == "SubsEmpleoCalculado" ||
                     subquery.x6.formulaConcepto == "ISRSubsidio"


                     select subquery);
            if (datosFiltradoMovNom.getClaveRazonSocial().Any())
            {
                query = (from subquery in query
                         where subquery.razonSocial.clave == claveRazonSocialMovNom
                         select subquery);
            }
            if (datosFiltradoMovNom.getClaveTipoNomina().Any())
            {
                query = (from subquery in query
                         where subquery.nomina.clave == claveTipoNominaMovNom
                         select subquery);
            }
            if (datosFiltradoMovNom.getFechaFin() != null & datosFiltradoMovNom.getFechaInicio() != null)
            {
                query = (from subquery in query
                         where ((subquery.x5.fechaInicial >= fechaInicioMovNom &&
                         subquery.x5.fechaInicial <= fechaFinMovNom) || (subquery.x5.fechaFinal >= fechaInicioMovNom &&
                                       subquery.x5.fechaFinal <= fechaFinMovNom))
                         select subquery);
                if (datosFiltradoEmpleado.getClaveTipoCorrida().Any())
                {
                    query = (from subquery in query
                             where subquery.x5.tipoCorrida.clave == claveTipoCorridaMovNom
                             select subquery);
                }
            }
            if (datosFiltradoMovNom.getClaveCentroCosto().Any())
            {
                query = (from subquery in query
                         join x8 in dBContextSimple.context.Set<CentroDeCosto>() on subquery.x0.centroDeCosto.id equals x8.id into x8_join
                         from x8 in x8_join.DefaultIfEmpty()
                         where x8.clave == claveCentroCostoMovNom
                         select subquery);
            }

            var query2 = (from subquery in query
                          orderby (subquery.empleado.clave == null ? "" : subquery.empleado.clave)
                          select new
                          {
                              empleado_clave = subquery.empleado.clave == null ? "" : subquery.empleado.clave,

                              x0 = new
                              {
                                  subquery.x0.calculado,
                                  subquery.x0.centroDeCosto_ID,
                                  concepNomDefi = new
                                  {
                                      subquery.x0.concepNomDefi.activado,
                                      subquery.x0.concepNomDefi.activarDesglose,
                                      subquery.x0.concepNomDefi.activarPlaza,
                                      subquery.x0.concepNomDefi.agregarSubcuentasPor,
                                      subquery.x0.concepNomDefi.categoriaPuestos_ID,
                                      subquery.x0.concepNomDefi.clave,
                                      subquery.x0.concepNomDefi.comportamiento,
                                      subquery.x0.concepNomDefi.conceptoDeNomina_ID,
                                      subquery.x0.concepNomDefi.condicionConcepto,
                                      subquery.x0.concepNomDefi.cuentaContable,
                                      subquery.x0.concepNomDefi.descripcion,
                                      subquery.x0.concepNomDefi.descripcionAbreviada,
                                      subquery.x0.concepNomDefi.fecha,
                                      subquery.x0.concepNomDefi.formulaConcepto,
                                      subquery.x0.concepNomDefi.grupo_ID,
                                      subquery.x0.concepNomDefi.id,
                                      subquery.x0.concepNomDefi.imprimirEnListadoNomina,
                                      subquery.x0.concepNomDefi.imprimirEnReciboNomina,
                                      subquery.x0.concepNomDefi.mascara,
                                      subquery.x0.concepNomDefi.naturaleza,
                                      subquery.x0.concepNomDefi.prioridadDeCalculo,
                                      subquery.x0.concepNomDefi.subCuenta,
                                      subquery.x0.concepNomDefi.tipo,
                                      subquery.x0.concepNomDefi.tipoAccionMascaras,
                                      subquery.x0.concepNomDefi.tipoMovto
                                  },
                                  subquery.x0.concepNomDefi_ID,
                                  subquery.x0.creditoMovimientos_ID,
                                  subquery.x0.ejercicio,
                                  subquery.x0.empleado_ID,
                                  subquery.x0.fechaCierr,
                                  subquery.x0.fechaIni,
                                  subquery.x0.finiqLiquidCncNom_ID,
                                  subquery.x0.id,
                                  subquery.x0.mes,
                                  subquery.x0.numero,
                                  subquery.x0.numMovParticion,
                                  subquery.x0.ordenId,
                                  subquery.x0.periodosNomina_ID,
                                  subquery.x0.plazasPorEmpleado_ID,
                                  subquery.x0.razonesSociales_ID,
                                  subquery.x0.resultado,
                                  subquery.x0.tipoCorrida_ID,
                                  subquery.x0.tipoNomina_ID,
                                  subquery.x0.tipoPantalla,
                                  subquery.x0.uso,
                                  //movNomBaseAfecta=subquery.x0.movNomBaseAfecta.Select(p => new {
                                  //p.baseAfecConcepNom_ID,
                                  //p.id,
                                  //p.movNomConcep_ID,
                                  //p.resultado,
                                  //p.resultadoExento,
                                  //p.uso
                                  //}).ToList()
                              },
                              x6 = subquery.x6.clave
                          }).Distinct();
            datosMovimientosAux = query2.ToList<object>();
            for (int i = 0; i < datosMovimientosAux.Count; i++)
            {
                var json = JsonConvert.SerializeObject(datosMovimientosAux[i]);
                Dictionary<string, object> valores = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                int cont = 0;
                object[] datos = new object[valores.Count()];
                foreach (var item in valores)
                {
                    datos[cont] = item.Value;
                    cont++;
                }
                datosMovimientos.Add(datos);
            }
            return datosMovimientos;
        }
        private PlazasPorEmpleadosMov calculaSueldoDiario(PlazasPorEmpleadosMov ppem)
        {
            double? sueldoDiario = 0;
            //if (ppem.salarioPor == 2)
            //{
            //    sueldoDiario = ppem.importe;
            //}
            //else
            //{
            //    sueldoDiario = ppem.puestos.salarioTabular;
            //}
            ppem.sueldoDiario = Convert.ToDouble(sueldoDiario);
            return ppem;
        }
        private void nullVariablesGlobales()
        {
            manejaPagosPorHora = false;
            manejoHorasPor = ManejoHorasPor.NINGUNO;
            manejoSalarioDiario = ManejoSalarioDiario.NINGUNO;
        }
        private void obtenerFactores(Object clavesElementoAplicacion, Periodicidad periodicidadTipoNomina, DBContextAdapter dbContextMaestra)
        {
            try
            {
                string valor = null;
                #region Maneja pagos por hora 
                decimal claveParametroPagosPorHora = Convert.ToDecimal(ClavesParametrosModulos.claveParametroPagosPorHora);
                string claveElementoAplicacionRazonSocial = ClavesParametrosModulos.claveElementoAplicacionRazonSocial.ToString();
                string claveModuloGlobal = ClavesParametrosModulos.claveModuloGlobal.ToString();
                decimal claveParametroManejarHorasPor = Convert.ToDecimal(ClavesParametrosModulos.claveParametroManejarHorasPor);
                decimal clavePagarNominaDiasNaturales = Convert.ToDecimal(ClavesParametrosModulos.clavePagarNominaDiasNaturales);
                string claveElementoAplicacionPeriodicidad = ClavesParametrosModulos.claveElementoAplicacionPeriodicidad.ToString();
                decimal claveParametroManejarSalarioDiarioPor = Convert.ToDecimal(ClavesParametrosModulos.claveParametroManejarSalarioDiarioPor);
                if (manejaPagosPorHora == false)
                {
                    valor = (from cr in dbContextMaestra.context.Set<Cruce>()
                             join pr in dbContextMaestra.context.Set<Parametros>() on cr.parametros.id equals pr.id
                             join ea in dbContextMaestra.context.Set<ElementosAplicacion>() on cr.elementosAplicacion.id equals ea.id
                             where pr.clave == claveParametroPagosPorHora &&
                             ea.clave.Equals(claveElementoAplicacionRazonSocial) &&
                             cr.claveElemento.Equals(clavesElementoAplicacion.ToString())
                             select cr.valor).Take(1).SingleOrDefault();
                    valor = (valor == null ? "" : valor);
                    if (String.IsNullOrEmpty(valor))
                    {

                        valor = (from pr in dbContextMaestra.context.Set<Parametros>()
                                 join m in dbContextMaestra.context.Set<Modulo>() on pr.modulo.id equals m.id
                                 where pr.clave == claveParametroPagosPorHora &&
                                 m.clave.Equals(claveModuloGlobal)
                                 select pr.valor).Take(1).SingleOrDefault();
                    }
                    manejaPagosPorHora = valor.ToString().Equals(ClavesParametrosModulos.opcionParametroPagarPorHoras.ToString(), StringComparison.InvariantCultureIgnoreCase);//equalsIgnoreCase

                }
                #endregion
                #region Manejo horas Por
                if (manejaPagosPorHora == false)
                {
                    valor = (from cr in dbContextMaestra.context.Set<Cruce>()
                             join pr in dbContextMaestra.context.Set<Parametros>() on cr.parametros.id equals pr.id
                             join ea in dbContextMaestra.context.Set<ElementosAplicacion>() on cr.elementosAplicacion.id equals ea.id
                             where pr.clave == claveParametroManejarHorasPor &&
                             ea.clave.Equals(claveElementoAplicacionRazonSocial) &&
                             cr.claveElemento.Equals(clavesElementoAplicacion.ToString())
                             select cr.valor).Take(1).SingleOrDefault();
                    valor = (valor == null ? "" : valor);
                    if (String.IsNullOrEmpty(valor))
                    {

                        valor = (from pr in dbContextMaestra.context.Set<Parametros>()
                                 join m in dbContextMaestra.context.Set<Modulo>() on pr.modulo.id equals m.id
                                 where pr.clave == claveParametroManejarHorasPor &&
                                 m.clave.Equals(claveModuloGlobal)
                                 select pr.valor).Take(1).SingleOrDefault();
                    }
                    if (valor.Equals(ClavesParametrosModulos.opcionParametroHorasNaturales))
                    {
                        manejoHorasPor = ManejoHorasPor.HORASNATURALES;
                    }
                    else if (valor.Equals(ClavesParametrosModulos.opcionParametroHorasHSM))
                    {
                        manejoHorasPor = ManejoHorasPor.HSM;
                    }
                }
                #endregion
                #region Manejo de pagos dias naturales 
                if (manejaPagosPorHora == false) //BUSQUEDA POR PERIODICIDAD//
                {
                    if (periodicidadTipoNomina != null)
                    {
                        valor = (from cr in dbContextMaestra.context.Set<Cruce>()
                                 join pr in dbContextMaestra.context.Set<Parametros>() on cr.parametros.id equals pr.id
                                 join ea in dbContextMaestra.context.Set<ElementosAplicacion>() on cr.elementosAplicacion.id equals ea.id
                                 where pr.clave == clavePagarNominaDiasNaturales &&
                                 ea.clave.Equals(claveElementoAplicacionPeriodicidad) &&
                                 cr.claveElemento.Equals(periodicidadTipoNomina.clave)
                                 select cr.valor).Take(1).SingleOrDefault();
                    }
                    valor = (valor == null ? "" : valor);
                    if (String.IsNullOrEmpty(valor)) // BUSQUEDA POR RAZON SOCIAL
                    {

                        valor = (from cr in dbContextMaestra.context.Set<Cruce>()
                                 join pr in dbContextMaestra.context.Set<Parametros>() on cr.parametros.id equals pr.id
                                 join ea in dbContextMaestra.context.Set<ElementosAplicacion>() on cr.elementosAplicacion.id equals ea.id
                                 where pr.clave == clavePagarNominaDiasNaturales &&
                                 ea.clave.Equals(claveElementoAplicacionPeriodicidad) &&
                                 cr.claveElemento.Equals(clavesElementoAplicacion.ToString())
                                 select cr.valor).Take(1).SingleOrDefault();
                        valor = (valor == null ? "" : valor);

                    }
                    if (String.IsNullOrEmpty(valor)) //BUSQUEDA GLOBAL
                    {

                        valor = (from pr in dbContextMaestra.context.Set<Parametros>()
                                 join m in dbContextMaestra.context.Set<Modulo>() on pr.modulo.id equals m.id
                                 where pr.clave == clavePagarNominaDiasNaturales &&
                                 m.clave.Equals(claveModuloGlobal)
                                 select pr.valor).Take(1).SingleOrDefault();
                    }
                    if (valor.Equals(ClavesParametrosModulos.opcionParametroPagarPorDiaNatural))
                    {
                        manejaPagoDiasNaturales = true;
                    }
                    else
                    {
                        manejaPagoDiasNaturales = true;
                    }

                }
                #endregion
                #region Manejo de Salario Diario
                if (manejaPagoDiasNaturales)
                {
                    manejoSalarioDiario = ManejoSalarioDiario.DIARIO;
                }
                else
                {
                    if (manejaPagosPorHora == false)
                    {
                        valor = (from cr in dbContextMaestra.context.Set<Cruce>()
                                 join pr in dbContextMaestra.context.Set<Parametros>() on cr.parametros.id equals pr.id
                                 join ea in dbContextMaestra.context.Set<ElementosAplicacion>() on cr.elementosAplicacion.id equals ea.id
                                 where pr.clave == claveParametroManejarSalarioDiarioPor &&
                                 ea.clave.Equals(claveElementoAplicacionRazonSocial) &&
                                 cr.claveElemento.Equals(clavesElementoAplicacion.ToString())
                                 select cr.valor).Take(1).SingleOrDefault();
                        valor = (valor == null ? "" : valor);
                        if (String.IsNullOrEmpty(valor))
                        {

                            valor = (from pr in dbContextMaestra.context.Set<Parametros>()
                                     join m in dbContextMaestra.context.Set<Modulo>() on pr.modulo.id equals m.id
                                     where pr.clave == claveParametroManejarSalarioDiarioPor &&
                                     m.clave.Equals(claveModuloGlobal)
                                     select pr.valor).Take(1).SingleOrDefault();
                        }
                        if (valor.Equals(ClavesParametrosModulos.opcionParametroSalarioDiario))
                        {
                            manejoSalarioDiario = ManejoSalarioDiario.DIARIO;
                        }
                        else if (valor.Equals(ClavesParametrosModulos.opcionParametroSalarioSemanal))
                        {
                            manejoSalarioDiario = ManejoSalarioDiario.SEMANAL;
                        }
                        else if (valor.Equals(ClavesParametrosModulos.opcionParametroSalarioQuincenal))
                        {
                            manejoSalarioDiario = ManejoSalarioDiario.QUINCENAL;
                        }
                        else
                        {
                            manejoSalarioDiario = ManejoSalarioDiario.MENSUAL;
                        }
                    }
                    #endregion
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerFactores()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
        }
        private class DatosFiltradoEmpleados
        {

            private String claveRazonSocial;
            private String claveTipoNomina;
            private String claveRegistroPatronal;
            private String claveCentroCosto;
            private String claveDepartamento;
            private String claveInicioEmpleado;
            private String claveFinEmpleado;
            private String claveTipoCorrida;
            private DateTime? fechaInicio;
            private DateTime? fechaFin;
            private decimal idEmpleadoInicio;
            private decimal idEmpleadoFin;

            public String getClaveRazonSocial()
            {
                return claveRazonSocial == null ? "" : claveRazonSocial;
            }

            public void setClaveRazonSocial(String claveRazonSocial)
            {
                this.claveRazonSocial = claveRazonSocial;
            }

            public String getClaveTipoNomina()
            {
                return claveTipoNomina == null ? "" : claveTipoNomina;
            }

            public void setClaveTipoNomina(String claveTipoNomina)
            {
                this.claveTipoNomina = claveTipoNomina;
            }

            public String getClaveRegistroPatronal()
            {
                return claveRegistroPatronal == null ? "" : claveRegistroPatronal;
            }

            public void setClaveRegistroPatronal(String claveRegistroPatronal)
            {
                this.claveRegistroPatronal = claveRegistroPatronal;
            }

            public String getClaveCentroCosto()
            {
                return claveCentroCosto == null ? "" : claveCentroCosto;
            }

            public void setClaveCentroCosto(String claveCentroCosto)
            {
                this.claveCentroCosto = claveCentroCosto;
            }

            public String getClaveDepartamento()
            {
                return claveDepartamento == null ? "" : claveDepartamento;
            }

            public void setClaveDepartamento(String claveDepartamento)
            {
                this.claveDepartamento = claveDepartamento;
            }

            public String getClaveInicioEmpleado()
            {
                return claveInicioEmpleado == null ? "" : claveInicioEmpleado;
            }

            public void setClaveInicioEmpleado(String claveInicioEmpleado)
            {
                this.claveInicioEmpleado = claveInicioEmpleado;
            }

            public String getClaveFinEmpleado()
            {
                return claveFinEmpleado == null ? "" : claveFinEmpleado;
            }

            public void setClaveFinEmpleado(String claveFinEmpleado)
            {
                this.claveFinEmpleado = claveFinEmpleado;
            }

            public DateTime? getFechaInicio()
            {
                return fechaInicio;
            }

            public void setFechaInicio(DateTime? fechaInicio)
            {
                this.fechaInicio = fechaInicio;
            }

            public DateTime? getFechaFin()
            {
                return fechaFin;
            }

            public void setFechaFin(DateTime? fechaFin)
            {
                this.fechaFin = fechaFin;
            }

            public String getClaveTipoCorrida()
            {
                return claveTipoCorrida == null ? "" : claveTipoCorrida;
            }

            public void setClaveTipoCorrida(String claveTipoCorrida)
            {
                this.claveTipoCorrida = claveTipoCorrida;
            }

            public decimal getidEmpleadoInicio()
            {
                return idEmpleadoInicio;
            }

            public void setidEmpleadoInicio(decimal idEmpleadoInicio)
            {
                this.idEmpleadoInicio = idEmpleadoInicio;
            }

            public decimal getidEmpleadoFin()
            {
                return idEmpleadoFin;
            }

            public void setidEmpleadoFin(decimal idEmpleadoFin)
            {
                this.idEmpleadoFin = idEmpleadoFin;
            }


        }
        private class DatosFiltradoMovNom
        {

            private String claveRazonSocial;
            private String claveTipoNomina;
            private String claveTipoCorrida;
            private String claveCentroCosto;
            private DateTime? fechaInicio;
            private DateTime? fechaFin;
            public String getClaveRazonSocial()
            {
                return claveRazonSocial == null ? "" : claveRazonSocial;
            }

            public void setClaveRazonSocial(String claveRazonSocial)
            {
                this.claveRazonSocial = claveRazonSocial;
            }

            public String getClaveTipoNomina()
            {
                return claveTipoNomina == null ? "" : claveTipoNomina;
            }

            public void setClaveTipoNomina(String claveTipoNomina)
            {
                this.claveTipoNomina = claveTipoNomina;
            }

            public String getClaveTipoCorrida()
            {
                return claveTipoCorrida == null ? "" : claveTipoCorrida;
            }

            public void setClaveTipoCorrida(String claveTipoCorrida)
            {
                this.claveTipoCorrida = claveTipoCorrida;
            }

            public String getClaveCentroCosto()
            {
                return claveCentroCosto == null ? "" : claveCentroCosto;
            }

            public void setClaveCentroCosto(String claveCentroCosto)
            {
                this.claveCentroCosto = claveCentroCosto;
            }

            public DateTime? getFechaInicio()
            {
                return fechaInicio;
            }

            public void setFechaInicio(DateTime? fechaInicio)
            {
                this.fechaInicio = fechaInicio;
            }

            public DateTime? getFechaFin()
            {
                return fechaFin;
            }

            public void setFechaFin(DateTime? fechaFin)
            {
                this.fechaFin = fechaFin;
            }
        }

        private class DatosFiltradoAsistencias
        {

            private String claveRazonSocial;
            private String claveTipoNomina;
            private String claveCentroCosto;
            private DateTime? fechaInicio;
            private DateTime? fechaFin;
            private String[] clavesEmpleados;

            public String getClaveRazonSocial()
            {
                return claveRazonSocial == null ? "" : claveRazonSocial;
            }

            public void setClaveRazonSocial(String claveRazonSocial)
            {
                this.claveRazonSocial = claveRazonSocial;
            }

            public String getClaveTipoNomina()
            {
                return claveTipoNomina == null ? "" : claveTipoNomina;
            }

            public void setClaveTipoNomina(String claveTipoNomina)
            {
                this.claveTipoNomina = claveTipoNomina;
            }

            public String getClaveCentroCosto()
            {
                return claveCentroCosto == null ? "" : claveCentroCosto;
            }

            public void setClaveCentroCosto(String claveCentroCosto)
            {
                this.claveCentroCosto = claveCentroCosto;
            }

            public DateTime? getFechaInicio()
            {
                return fechaInicio;
            }

            public void setFechaInicio(DateTime? fechaInicio)
            {
                this.fechaInicio = fechaInicio;
            }

            public DateTime? getFechaFin()
            {
                return fechaFin;
            }

            public void setFechaFin(DateTime? fechaFin)
            {
                this.fechaFin = fechaFin;
            }

            public String[] getClavesEmpleados()
            {
                return clavesEmpleados;
            }

            public void setClavesEmpleados(String[] clavesEmpleados)
            {
                this.clavesEmpleados = clavesEmpleados;
            }
        }
        public Mensaje getAllCFDIEmpleado(DBContextAdapter dbContext)
        {
            listCFDIEmpleado = new List<CFDIEmpleado>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listCFDIEmpleado = (from a in getSession().Set<CFDIEmpleado>() select a).ToList();
                mensajeResultado.resultado = listCFDIEmpleado;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("CFDIEmpleadoAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        public Mensaje getCFDIEmpleadoPorFiltro(string claveRazonSocial, string claveTipoNomina, string claveTipoCorrida, decimal idPeriodoNomina, StatusTimbrado statusTimbre, string[] rangoEmpleados, DBContextAdapter dbContext)
        {
            listCFDIEmpleado = new List<CFDIEmpleado>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var query = (from a in getSession().Set<CFDIEmpleado>()
                             where a.razonesSociales.clave.Equals(claveRazonSocial) &&
                             a.tipoNomina.clave.Equals(claveTipoNomina) &&
                             a.tipoCorrida.clave.Equals(claveTipoCorrida) &&
                             a.periodosNomina.id.Equals(idPeriodoNomina) &&
                             rangoEmpleados.Contains(a.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave)
                             select a);
                if (statusTimbre != StatusTimbrado.NINGUNO)
                {
                    query = (from subquery in query
                             where subquery.cfdiRecibo.statusTimbrado == statusTimbre
                             select subquery);
                }
                listCFDIEmpleado = query.ToList();
                mensajeResultado.resultado = listCFDIEmpleado;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("CFDIEmpleadoPorFiltro()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        public Mensaje getCFDIEmpleadoStatusPorFiltro(string claveRazonSocial, string claveTipoNomina, string claveTipoCorrida, decimal idPeriodoNomina, List<StatusTimbrado> tiposTimbre, string[] rangoEmpleados, DBContextAdapter dbContext)
        {
            List<object[]> listCFDIEmpleadostatus = new List<object[]>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();

                var query = (from a in getSession().Set<CFDIEmpleado>()
                             where a.razonesSociales.clave.Equals(claveRazonSocial) &&
                             a.tipoNomina.clave.Equals(claveTipoNomina) &&
                             a.tipoCorrida.clave.Equals(claveTipoCorrida) &&
                             a.periodosNomina.id == (idPeriodoNomina) &&
                             rangoEmpleados.Contains(a.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave)
                             select new
                             {
                                 a.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave,
                                 a.cfdiRecibo.statusTimbrado
                             }).ToList();

                if (tiposTimbre != null)
                {
                    query = (from subquery in query
                             where tiposTimbre.Contains(subquery.statusTimbrado)
                             select subquery).ToList();
                }

                listCFDIEmpleadostatus.Add(query.ToArray());
                mensajeResultado.resultado = listCFDIEmpleadostatus;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("CFDIEmpleadoPorFiltro()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        public Mensaje getCFDIEmpleadoStatusPorFiltroPorRangoPeriodosNomina(string claveRazonSocial, string claveTipoNomina, string claveTipoCorrida, DateTime fechaInicial, DateTime fechaFinal, List<StatusTimbrado> tiposTimbre, object[] rangoEmpleados, DBContextAdapter dbContext)
        {
            List<object[]> listCFDIEmpleado = new List<object[]>();
            bool existenTimbresActivos = false;
            List<PeriodosNomina> listPeriodosNomina = new List<PeriodosNomina>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listPeriodosNomina = (from p in getSession().Set<PeriodosNomina>()
                                      where p.tipoNomina.clave.Equals(claveTipoNomina) &&
                                      p.tipoCorrida.clave.Equals(claveTipoCorrida) &&
                                      p.fechaFinal >= fechaInicial &&
                                      p.fechaInicial <= fechaFinal
                                      select p).ToList();
                if (listPeriodosNomina == null)
                {
                    listPeriodosNomina = new List<PeriodosNomina>();
                }
                if (listPeriodosNomina.Count > 0)
                {
                    decimal[] listid = new decimal[listPeriodosNomina.Count];
                    for (int i = 0; i < listPeriodosNomina.Count(); i++)
                    {
                        listid[i] = listPeriodosNomina[i].id;
                    }
                    var query = (from a in getSession().Set<CFDIEmpleado>()
                                 where a.razonesSociales.clave.Equals(claveRazonSocial) &&
                                 a.tipoNomina.clave.Equals(claveTipoNomina) &&
                                 a.tipoCorrida.clave.Equals(claveTipoCorrida) &&
                                 listid.Contains(a.periodosNomina.id) &&
                                 rangoEmpleados.Contains(a.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave)
                                 select new
                                 {
                                     a.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave,
                                     a.cfdiRecibo.statusTimbrado
                                 });
                    if (tiposTimbre != null)
                    {
                        query = (from subquery in query
                                 where tiposTimbre.Contains(subquery.statusTimbrado)
                                 select subquery);
                    }
                    listCFDIEmpleado.Add(query.ToArray());
                    if (listCFDIEmpleado.Count == 0)
                    {
                        listCFDIEmpleado = new List<object[]>();
                    }
                    if (listCFDIEmpleado.Count > 0)
                    {
                        if ((StatusTimbrado)listCFDIEmpleado[0][1] == StatusTimbrado.TIMBRADO)
                        {
                            existenTimbresActivos = true;
                        }
                    }
                }

                mensajeResultado.resultado = existenTimbresActivos;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("CFDIEmpleadoStatusPorFiltroPorRangoPeriodosNomina()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        public Mensaje getCFDIEmpleadoTimbrados(decimal[] idsCFDIEmpleado, DBContextAdapter dbContext)
        {
            listCFDIEmpleado = new List<CFDIEmpleado>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listCFDIEmpleado = (from o in getSession().Set<CFDIEmpleado>()
                                    join cr in getSession().Set<CFDIRecibo>() on o.cfdiRecibo.id equals cr.id into com
                                    from cr in com.DefaultIfEmpty()
                                    where idsCFDIEmpleado.Contains(o.id)
                                    && cr.statusTimbrado == (StatusTimbrado.TIMBRADO)
                                    select o).ToList();

                mensajeResultado.resultado = listCFDIEmpleado;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("CFDIEmpleadoPorFiltro()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        public Mensaje getLimpiaConStatusErrorOEnProceso(string claveRazonSocial, string claveTipoNomina, string claveTipoCorrida, decimal idPeriodoNomina, List<string> rangoEmpleados, DBContextAdapter dbContext)
        {
            listCFDIEmpleado = new List<CFDIEmpleado>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listCFDIEmpleado = (from o in getSession().Set<CFDIEmpleado>()
                                    join cr in getSession().Set<CFDIRecibo>() on o.cfdiRecibo.id equals cr.id into com
                                    from cr in com.DefaultIfEmpty()
                                    where o.razonesSociales.clave.Equals(claveRazonSocial) &&
                                    o.tipoNomina.clave.Equals(claveTipoNomina) &&
                                    o.periodosNomina.id == idPeriodoNomina &&
                                    rangoEmpleados.Contains(o.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave) &&
                                    cr.statusTimbrado == StatusTimbrado.ERROR || cr.statusTimbrado == StatusTimbrado.EN_PROCESO
                                    select o).ToList();
                //listCFDIEmpleado = query.Cast<CFDIEmpleado>().ToList();
                listCFDIEmpleado = listCFDIEmpleado == null ? new List<CFDIEmpleado>() : listCFDIEmpleado;
                int i = 0;
                if (listCFDIEmpleado.Count > 0)
                {
                    for (i = 0; i < listCFDIEmpleado.Count(); i++)
                    {

                        if (listCFDIEmpleado[i].cfdiRecibo_ID > 0)
                        {

                            if (listCFDIEmpleado[i].cfdiRecibo.cfdiReciboConcepto.Any())
                            {

                                int total = listCFDIEmpleado[i].cfdiRecibo.cfdiReciboConcepto.Count();
                                for (int k = 0; k < total; k++)
                                {
                                    getSession().Set<CFDIReciboConcepto>().Attach(listCFDIEmpleado[i].cfdiRecibo.cfdiReciboConcepto[0]);
                                    getSession().Set<CFDIReciboConcepto>().Remove(listCFDIEmpleado[i].cfdiRecibo.cfdiReciboConcepto[0]);
                                }
                            }

                            if (listCFDIEmpleado[i].cfdiRecibo.cfdiReciboHrsExtras.Any())
                            {
                                int total = listCFDIEmpleado[i].cfdiRecibo.cfdiReciboHrsExtras.Count();
                                for (int j = 0; j < total; j++)
                                {
                                    getSession().Set<CFDIReciboHrsExtras>().Attach(listCFDIEmpleado[i].cfdiRecibo.cfdiReciboHrsExtras[0]);
                                    getSession().Set<CFDIReciboHrsExtras>().Remove(listCFDIEmpleado[i].cfdiRecibo.cfdiReciboHrsExtras[0]);
                                }
                            }
                            if (listCFDIEmpleado[i].cfdiRecibo.cfdiReciboIncapacidad.Any())
                            {
                                int total = listCFDIEmpleado[i].cfdiRecibo.cfdiReciboIncapacidad.Count();
                                for (int l = 0; l < total; l++)
                                {
                                    getSession().Set<CFDIReciboIncapacidad>().Attach(listCFDIEmpleado[i].cfdiRecibo.cfdiReciboIncapacidad[0]);
                                    getSession().Set<CFDIReciboIncapacidad>().Remove(listCFDIEmpleado[i].cfdiRecibo.cfdiReciboIncapacidad[0]);
                                }
                            }

                            getSession().Set<CFDIRecibo>().Attach(listCFDIEmpleado[i].cfdiRecibo);
                            getSession().Set<CFDIRecibo>().Remove(listCFDIEmpleado[i].cfdiRecibo);
                        }

                        getSession().Set<CFDIEmpleado>().Attach(listCFDIEmpleado[i]);
                        getSession().Set<CFDIEmpleado>().Remove(listCFDIEmpleado[i]);
                        listCFDIEmpleado.RemoveAt(i);
                    }
                }
                                
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getCFDIEmpleadoPorFiltro()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje saveDeleteCFDIEmpleado(List<CFDIEmpleado> entitysCambios, object[] idEliminar, int rango, DBContextAdapter dbContext)
        {
            listCFDIEmpleado = new List<CFDIEmpleado>();
            int i = 0; int j = 0;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (idEliminar != null)
                {
                    // deleteListQuery( typeof(CFDIEmpleado).Name, "id", idEliminar);
                   
                    Mensaje mensaje;
                    
                    for (j = 0; j < idEliminar.Count(); j++)
                    {
                        /*
                         object[] valores;
                        valores = (object[])datosEmpleado[i];
                         */

                        decimal cfdiEmpleado_id = Convert.ToDecimal(idEliminar[j]);
                        List<decimal> cfdiRecibo = (from o in getSession().Set<CFDIEmpleado>()
                                                    where o.id == cfdiEmpleado_id
                                                    select o.cfdiRecibo_ID).ToList();
                        if (cfdiRecibo == null ? false : cfdiRecibo.Count > 0)
                        {
                            decimal cfdiRecibo_id = cfdiRecibo[0];

                           List<decimal> cfdiCnc = (from o in getSession().Set<CFDIReciboConcepto>()
                                                     join cfd in getSession().Set<CFDIRecibo>() on
                                                     o.cfdiRecibo.id equals cfd.id
                                                     where cfd.id == cfdiRecibo_id
                                                     select o.id).ToList(); ;
                            if (cfdiCnc == null ? false: cfdiCnc.Count > 0)
                            {
                                int cont = 0;
                                List<object> datos = new List<object>();
                                
                                while (cont < cfdiCnc.Count)
                                {
                                    datos.Add(cfdiCnc[cont]);
                                    
                                    cont++;
                                }
                               
                                mensaje = deleteListQuery("CFDIReciboConcepto", new CamposWhere("CFDIReciboConcepto.id", datos.ToArray(), OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                                
                            }

                            List<decimal> cfdiInc = (from o in getSession().Set<CFDIReciboIncapacidad>()
                                                     join cfd in getSession().Set<CFDIRecibo>() on
                                                     o.cfdiRecibo.id equals cfd.id
                                                     where cfd.id == cfdiRecibo_id
                                                     select o.id).ToList();
                            if (cfdiInc == null ? false : cfdiInc.Count > 0)
                            {
                                int cont = 0;
                                List<object> datos = new List<object>();

                                while (cont < cfdiInc.Count)
                                {
                                    datos.Add(cfdiInc[cont]);

                                    cont++;
                                }

                                mensaje = deleteListQuery("CFDIReciboIncapacidad", new CamposWhere("CFDIReciboIncapacidad.id", datos.ToArray(), OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                                
                            }
                            List<decimal> cfdiHrs = (from o in getSession().Set<CFDIReciboHrsExtras>()
                                                     join cfd in getSession().Set<CFDIRecibo>() on
                                                     o.cfdiRecibo.id equals cfd.id
                                                     where cfd.id == cfdiRecibo_id
                                                     select o.id).ToList();
                            if (cfdiHrs == null ? false : cfdiHrs.Count > 0)
                            {

                                int cont = 0;
                                List<object> datos = new List<object>();

                                while (cont < cfdiHrs.Count)
                                {
                                    datos.Add(cfdiHrs[cont]);

                                    cont++;
                                }
                                mensaje = deleteListQuery("CFDIReciboHrsExtras", new CamposWhere("CFDIReciboHrsExtras.id", datos.ToArray(), OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                                
                            }
                            List<object> datos1 = new List<object>();
                            datos1.Add(cfdiRecibo_id);

                            mensaje = deleteListQuery("CFDIRecibo", new CamposWhere("CFDIRecibo.id", datos1.ToArray(), OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                        }
                    }
                    mensaje = deleteListQuery(typeof(CFDIEmpleado).Name, new CamposWhere("CFDIEmpleado.id", idEliminar, OperadorComparacion.IN, OperadorLogico.AND), dbContext);



                }
                entitysCambios = (entitysCambios == null ? new List<CFDIEmpleado>() : entitysCambios);
                for (i = 0; i < entitysCambios.Count(); i++)
                {
                    if (entitysCambios[i].id == 0)
                    {
                        getSession().Set<CFDIEmpleado>().Add(entitysCambios[i]);
                    }
                    else
                    {
                        getSession().Set<CFDIEmpleado>().AddOrUpdate(entitysCambios[i]);
                    }

                    getSession().SaveChanges();

                }
                mensajeResultado.resultado = true;
                mensajeResultado.error = "";
                mensajeResultado.noError = 0;
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteCFDIEmpleado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public List<MovNomBaseAfecta> obtenerMovNomBaseAfecta(decimal idMov, DBContextAdapter dBContext)
        {

            List<MovNomBaseAfecta> movBase = new List<MovNomBaseAfecta>();

            movBase = (from mb in dBContext.context.Set<MovNomBaseAfecta>()
                       where mb.movNomConcep_ID == idMov
                       select mb).ToList();
            return movBase;

        }

        public List<MovNomConceParam> obtenerMovNomConceParam(decimal idMov, DBContextAdapter dBContext)
        {

            List<MovNomConceParam> movParam = new List<MovNomConceParam>();

            movParam = (from mb in dBContext.context.Set<MovNomConceParam>()
                        where mb.movNomConcep_ID == idMov
                        select mb).ToList();
            return movParam;

        }

        public PlazasPorEmpleadosMov obtenerPlazaEmpleaMovFull(PlazasPorEmpleadosMov plaza, DBContextAdapter dbContext)
        {

            decimal? id = plaza.id;
            plaza = (from pl in dbContext.context.Set<PlazasPorEmpleadosMov>() where pl.id == id select pl).SingleOrDefault();
            return plaza;

        }

        public MovNomConcep obtenerMovNomConcepFull(MovNomConcep mov, DBContextAdapter dbContext)
        {
            decimal? id = mov.id;
            mov = (from m in dbContext.context.Set<MovNomConcep>() where m.id == id
                   select m
                   
                       ).SingleOrDefault();
           

            return mov;
        }

        public Asistencias obtenerAsistenciaFull(Asistencias asistencia, DBContextAdapter dbContext)
        {
            decimal? id = asistencia.id;
            asistencia = (from a in dbContext.context.Set<Asistencias>() where a.id == id select a).SingleOrDefault();

            return asistencia;
        }

        public ConcepNomDefi getConceptoNominaSubsidio(DBContextAdapter dBContext)
        {

           concepto = new ConcepNomDefi();
           concepto = (from pl in dBContext.context.Set<ConcepNomDefi>() where pl.formulaConcepto== "SubsEmpleoCalculado" select pl).SingleOrDefault();
            return concepto;
        }

       


    }
}
