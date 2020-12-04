/**
 * @author: Beatriz Baldenebro 
 * Fecha de Creación: 17/01/2018
 * Compañía: Macropro
 * Descripción del programa: clase CalculoPtuDAO para llamados a metodos de Entity
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
using Exitosw.Payroll.Core.util;
using System.Text;
using System.Reflection;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using Exitosw.Payroll.Entity.util;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public class CalculoPtuDAO : GenericRepository<Object>, CalculoPtuDAOIF
    {

        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");

        public Mensaje calculoPtu(PtuDatosGenerales ptuDatosGenerales, List<PtuEmpleados> ptuEmpleados, double cantidadRepartir, object[] totales, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                Double cantidadDistribuida = (cantidadRepartir / 2);
                Double totalDeDias = (Double)totales[0];
                Double totalDePercepciones = (Double)totales[1];
                Double? totalDiasPtu = 0D;
                Double? totalPercepcionesPtu = 0D;
                if (ptuEmpleados != null)
                {
                    int i = 0;
                    for (i = 0; i < ptuEmpleados.Count; i++)
                    {
                        Double? ptuDiasEmple = ptuEmpleados[i].diasLaborados / totalDeDias * cantidadDistribuida;
                        if (ptuEmpleados[i].participa)
                        {
                            ptuEmpleados[i].ptuDias = ptuDiasEmple;
                        }
                        else
                        {
                            ptuEmpleados[i].ptuDias = 0.0;
                        }
                        totalDiasPtu += ptuEmpleados[i].ptuDias;
                        Double? ptuPercepEmple;
                        if (Convert.ToDouble(ptuEmpleados[i].percepciones) > 0.0)
                        {
                            ptuPercepEmple = ptuEmpleados[i].percepciones / totalDePercepciones * cantidadDistribuida;
                        }
                        else
                        {
                            ptuPercepEmple = 0.0;
                        }
                        if (ptuEmpleados[i].participa)
                        {
                            ptuEmpleados[i].ptuPercepciones = ptuPercepEmple;
                        }
                        else
                        {
                            ptuEmpleados[i].ptuPercepciones = 0.0;
                        }
                        totalPercepcionesPtu += ptuEmpleados[i].percepciones;
                        getSession().Set<PtuEmpleados>().AddOrUpdate(ptuEmpleados[i]);
                        if (i % 50 == 0 & i > 0)
                        {
                            getSession().SaveChanges();
                        }
                    }
                }
                ptuDatosGenerales.totalDiasptu = totalDiasPtu;
                ptuDatosGenerales.totalPercepciones = totalPercepcionesPtu;
                ptuDatosGenerales.status = "CALCULADO";
                getSession().Set<PtuDatosGenerales>().AddOrUpdate(ptuDatosGenerales);
                mensajeResultado.resultado = ptuEmpleados;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("calculoPtu()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje cargaDeAcumulados(int ejercicio, string claveRazonsocial, int diasPorDerechoPTU, bool cumplenPtu, DBContextAdapter dbContext)
        {
            int resul = 0;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                resul = getSession().Database.ExecuteSqlCommand("DELETE p from PtuEmpleados AS p INNER JOIN RazonesSociales  AS r On p.razonesSociales_ID = r.id WHERE p.id IN (SELECT ptuEm.id from PtuEmpleados AS ptuEm WHERE ptuEm.ejercicio >= @@anio AND r.clave >= @@claveRazonsocial)", new SqlParameter("@anio", ejercicio), new SqlParameter("@clave", claveRazonsocial));
                Double dias = 365D;
                if (DateTime.IsLeapYear(ejercicio))
                {
                    dias = 366D;
                }
                DateTime fechaInicial = new DateTime(ejercicio, 1, 1);
                DateTime fechaFinal = new DateTime(ejercicio, 12, 31);

                var query = (from o in getSession().Set<PlazasPorEmpleadosMov>()
                             join ppe3 in getSession().Set<PlazasPorEmpleado>() on new { PlazasPorEmpleado_id = Convert.ToDecimal(o.plazasPorEmpleado_ID) } equals new { PlazasPorEmpleado_id = ppe3.id } into ppe3_join
                             from ppe3 in ppe3_join.DefaultIfEmpty()
                             join emp3 in getSession().Set<Empleados>() on new { Empleados_ID = Convert.ToDecimal(ppe3.empleados_ID) } equals new { Empleados_ID = emp3.id } into emp3_join
                             from emp3 in emp3_join.DefaultIfEmpty()
                             where
                             (from m in getSession().Set<PlazasPorEmpleadosMov>()
                              join ppe4 in getSession().Set<PlazasPorEmpleado>() on new { PlazasPorEmpleado_id = Convert.ToDecimal(m.plazasPorEmpleado_ID) } equals new { PlazasPorEmpleado_id = ppe4.id } into ppe4_join
                              from ppe4 in ppe4_join.DefaultIfEmpty()
                              join rs in getSession().Set<RazonesSociales>() on new { RazonesSociales_ID = ppe4.razonesSociales_ID } equals new { RazonesSociales_ID = rs.id } into rs_join
                              from rs in rs_join.DefaultIfEmpty()
                              join emp4 in getSession().Set<Empleados>() on new { Empleados_id = Convert.ToDecimal(ppe4.empleados_ID) } equals new { Empleados_id = emp4.id } into emp4_join
                              from emp4 in emp4_join.DefaultIfEmpty()
                              where
                              rs.clave == claveRazonsocial &&
                              (m.fechaInicial.Value.Year <= ejercicio &&
                              ppe4.fechaFinal.Value.Year >= ejercicio) ||
                              ppe4.fechaFinal.Value.Year == ejercicio
                              group new { emp4, m } by new
                              {
                                  emp4.clave
                              } into g
                              select new
                              {
                                  Column1 = (decimal?)g.Max(p => p.m.id)
                              }).Contains(new { Column1 = (System.Decimal?)o.id })
                             select new
                             {
                                 o.puestos_ID,
                                 Empleados_id = (decimal?)ppe3.empleados_ID,
                                 Column1 =
                                    o.fechaInicial == null ? fechaInicial : o.fechaInicial,
                                 Column2 =
                                    ppe3.fechaFinal.Value.Year >= fechaFinal.Year ? Convert.ToString(ppe3.fechaFinal) : "",
                                 Column3 =
                                    o.tipoRelacionLaboral == null ? "" : Convert.ToString(o.tipoRelacionLaboral),
                                 Column4 = (double?)
                                   dias - (from a in getSession().Set<Asistencias>()
                                           where
                                              a.empleados.id == ppe3.id &&
                                              a.razonesSociales.clave == claveRazonsocial &&
                                              !(new int[] { 3, 5, 6 }).Contains(Convert.ToInt32(a.excepciones.clave)) &&
                                              a.fecha >= fechaInicial && a.fecha <= fechaInicial
                                           select new
                                           {
                                               a,
                                               a.excepciones,
                                               a.empleados,
                                               a.razonesSociales,
                                               a.tipoNomina,
                                               a.periodosNomina
                                           }).Count() * 1.0,
                                 Column5 = from mov in
                                        (from mov in getSession().Set<MovNomConcep>()
                                         join per in getSession().Set<PeriodosNomina>() on new { PeriodosNomina_ID = Convert.ToDecimal(mov.periodosNomina_ID) } equals new { PeriodosNomina_ID = per.id } into per_join
                                         from per in per_join.DefaultIfEmpty()
                                         join cnc in getSession().Set<ConcepNomDefi>() on new { ConcepNomDefi_ID = Convert.ToDecimal(mov.concepNomDefi_ID) } equals new { ConcepNomDefi_ID = cnc.id } into cnc_join
                                         from cnc in cnc_join.DefaultIfEmpty()
                                         join emple in getSession().Set<Empleados>() on new { Empleado_ID = Convert.ToDecimal(mov.empleado_ID) } equals new { Empleado_ID = emple.id } into emple_join
                                         from emple in emple_join.DefaultIfEmpty()
                                         join Hx11 in getSession().Set<PlazasPorEmpleado>() on new { PlazasPorEmpleado_ID = Convert.ToDecimal(mov.plazasPorEmpleado_ID) } equals new { PlazasPorEmpleado_ID = Hx11.id } into Hx11_join
                                         from Hx11 in Hx11_join.DefaultIfEmpty()
                                         join rs in getSession().Set<RazonesSociales>() on new { RazonesSociales_ID = Convert.ToDecimal(mov.razonesSociales_ID) } equals new { RazonesSociales_ID = rs.id } into rs_join
                                         from rs in rs_join.DefaultIfEmpty()
                                         where
                                                cnc.naturaleza == 0 &&
                                                emple.id == ppe3.id &&
                                                rs.clave == claveRazonsocial &&
                                                per.año == ejercicio &&
                                                (new int?[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }).Contains(mov.mes)
                                         select new
                                         {
                                             Column1 =
                                                mov.resultado == null ? 0.0 : mov.resultado,
                                             Dummy = "x"
                                         })
                                           group mov by new { mov.Dummy } into g
                                           select new
                                           {
                                               Column1 =
                                         g.Count() == 0 ? 0.0 : g.Sum(p => p.Column1)
                                           },
                                 abase =
                                        (((from pm in getSession().Set<PlazasPorEmpleadosMov>()
                                           where
                                             pm.tipoRelacionLaboral == 1 &&
                                             pm.plazasPorEmpleado_ID ==
                                               (((from mov in getSession().Set<MovNomConcep>()
                                                  join emple in getSession().Set<Empleados>() on new { Empleado_ID = Convert.ToDecimal(mov.empleado_ID) } equals new { Empleado_ID = emple.id } into emple_join
                                                  from emple in emple_join.DefaultIfEmpty()
                                                  join rs in getSession().Set<RazonesSociales>() on new { RazonesSociales_ID = Convert.ToDecimal(mov.razonesSociales_ID) } equals new { RazonesSociales_ID = rs.id } into rs_join
                                                  from rs in rs_join.DefaultIfEmpty()
                                                  where
                                                 emple.id == ppe3.id &&
                                                 rs.clave == claveRazonsocial &&
                                                 pm.fechaInicial.Value.Year == ejercicio
                                                  select new
                                                  {
                                                      mov.plazasPorEmpleado_ID
                                                  }).Distinct()).First().plazasPorEmpleado_ID)
                                           select new
                                           {
                                               Column1 =
                                             pm.tipoRelacionLaboral == null ? "" : Convert.ToString(pm.tipoRelacionLaboral)
                                           }).Distinct()).First().Column1),
                                 eventual =
                                            (((from pm in getSession().Set<PlazasPorEmpleadosMov>()
                                               where
                                                 pm.tipoRelacionLaboral == 2 &&
                                                 pm.plazasPorEmpleado_ID ==
                                                   (((from mov in getSession().Set<MovNomConcep>()
                                                      join emple in getSession().Set<Empleados>() on new { Empleado_ID = Convert.ToDecimal(mov.empleado_ID) } equals new { Empleado_ID = emple.id } into emple_join
                                                      from emple in emple_join.DefaultIfEmpty()
                                                      join rs in getSession().Set<RazonesSociales>() on new { RazonesSociales_ID = Convert.ToDecimal(mov.razonesSociales_ID) } equals new { RazonesSociales_ID = rs.id } into rs_join
                                                      from rs in rs_join.DefaultIfEmpty()
                                                      where
                                                     emple.id == ppe3.id &&
                                                     rs.clave == claveRazonsocial &&
                                                     pm.fechaInicial.Value.Year == ejercicio
                                                      select new
                                                      {
                                                          mov.plazasPorEmpleado_ID
                                                      }).Distinct()).First().plazasPorEmpleado_ID)
                                               select new
                                               {
                                                   Column1 =
                                                 pm.tipoRelacionLaboral == null ? "" : Convert.ToString(pm.tipoRelacionLaboral)
                                               }).Distinct()).First().Column1),
                                 confianza =
                                                (((from pm in getSession().Set<PlazasPorEmpleadosMov>()
                                                   where
                                                     pm.tipoRelacionLaboral == 3 &&
                                                     pm.plazasPorEmpleado_ID ==
                                                       (((from mov in getSession().Set<MovNomConcep>()
                                                          join emple in getSession().Set<Empleados>() on new { Empleado_ID = Convert.ToDecimal(mov.empleado_ID) } equals new { Empleado_ID = emple.id } into emple_join
                                                          from emple in emple_join.DefaultIfEmpty()
                                                          join rs in getSession().Set<RazonesSociales>() on new { RazonesSociales_ID = Convert.ToDecimal(mov.razonesSociales_ID) } equals new { RazonesSociales_ID = rs.id } into rs_join
                                                          from rs in rs_join.DefaultIfEmpty()
                                                          where
                                                         emple.id == ppe3.id &&
                                                         rs.clave == claveRazonsocial &&
                                                         pm.fechaInicial.Value.Year == ejercicio
                                                          select new
                                                          {
                                                              mov.plazasPorEmpleado_ID
                                                          }).Distinct()).First().plazasPorEmpleado_ID)
                                                   select new
                                                   {
                                                       Column1 =
                                                     pm.tipoRelacionLaboral == null ? "" : Convert.ToString(pm.tipoRelacionLaboral)
                                                   }).Distinct()).First().Column1),
                                 Column6 =
                                                ((from pm in getSession().Set<PlazasPorEmpleadosMov>()
                                                  join ppe22 in getSession().Set<PlazasPorEmpleado>() on new { PlazasPorEmpleado_id = Convert.ToDecimal(pm.plazasPorEmpleado_ID) } equals new { PlazasPorEmpleado_id = ppe22.id } into ppe22_join
                                                  from ppe22 in ppe22_join.DefaultIfEmpty()
                                                  join emp22 in getSession().Set<Empleados>() on new { Empleados_ID = Convert.ToDecimal(ppe22.empleados_ID) } equals new { Empleados_ID = emp22.id } into emp22_join
                                                  from emp22 in emp22_join.DefaultIfEmpty()
                                                  join rs22 in getSession().Set<RazonesSociales>() on new { RazonesSociales_ID = ppe22.razonesSociales_ID } equals new { RazonesSociales_ID = rs22.id } into rs22_join
                                                  from rs22 in rs22_join.DefaultIfEmpty()
                                                  where
                                                   pm.fechaInicial ==
                                                        (from pm2 in getSession().Set<PlazasPorEmpleadosMov>()
                                                         join ppe2 in getSession().Set<PlazasPorEmpleado>() on new { PlazasPorEmpleado_id = Convert.ToDecimal(pm2.plazasPorEmpleado_ID) } equals new { PlazasPorEmpleado_id = ppe2.id } into ppe2_join
                                                         from ppe2 in ppe2_join.DefaultIfEmpty()
                                                         join emp2 in getSession().Set<Empleados>() on new { Empleados_ID = Convert.ToDecimal(ppe2.empleados_ID) } equals new { Empleados_ID = emp2.id } into emp2_join
                                                         from emp2 in emp2_join.DefaultIfEmpty()
                                                         join rs in getSession().Set<RazonesSociales>() on new { RazonesSociales_ID = ppe2.razonesSociales_ID } equals new { RazonesSociales_ID = rs.id } into rs_join
                                                         from rs in rs_join.DefaultIfEmpty()
                                                         where
                                                           emp2.id == emp3.id &&
                                                           rs.clave == claveRazonsocial &&
                                                           pm2.fechaInicial.Value.Year <= ejercicio
                                                         select new
                                                         {
                                                             pm2.fechaInicial
                                                         }).Max(p => p.fechaInicial) &&
                                                      emp22.id == emp3.id &&
                                                      rs22.clave == claveRazonsocial
                                                  select new
                                                  {
                                                      Column1 =
                                                    pm.tipoRelacionLaboral == null ? "" : Convert.ToString(pm.tipoRelacionLaboral)
                                                  }).First().Column1)
                             });
                // List<Plazas> plazas = query.ToList<Plazas>(); //DUDA
                List<object> plazas = query.ToList<object>();
                if (plazas != null)
                {
                    int j = 0;
                    if (!cumplenPtu)
                    {
                        while (j < plazas.Count())
                        {
                            //Object[] info = (Object[])plazas.get(j); DUDA
                            Object[] info = new Object[] { plazas[j] };
                            //Inicial
                            Puestos puesto = (Puestos)info[0];
                            bool PuestoDirectivo = false;
                            if (puesto != null)
                            {
                                if (puesto.directivo)
                                {
                                    PuestoDirectivo = true;
                                }
                            }
                            fechaInicial = (DateTime)info[2];
                            Double diasPTU = (Double)info[5];
                            if (fechaInicial.Year == fechaFinal.Year)
                            {
                                int days = daysBetween(fechaInicial, fechaFinal);
                                diasPTU = diasPTU - (diasPTU - days + 1);
                            }
                            if ((diasPTU < diasPorDerechoPTU) | PuestoDirectivo)
                            {
                                plazas.RemoveAt(j);
                            }
                            else
                            {
                                j++;
                            }
                        }
                    }
                }
                mensajeResultado.resultado = plazas;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("guardarCargaAcumulados()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            throw new NotImplementedException();
        }
        private int daysBetween(DateTime d1, DateTime d2)
        {
            return (int)(d2 - d1).TotalDays;
            //return (int)((d2 - d1) / (1000 * 60 * 60 * 24));
        }

        public Mensaje guardarCargaAcumulados(PtuDatosGenerales ptuDatosGenerales, List<PtuEmpleados> ptuEmpleados, DBContextAdapter dbContext)
        {
            List<PtuEmpleados> listPtuDatosGen = new List<PtuEmpleados>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                int i = 0;
                for (i = 0; i < ptuEmpleados.Count; i++)
                {
                    getSession().Set<PtuEmpleados>().AddOrUpdate(ptuEmpleados[i]);
                    listPtuDatosGen.Add(ptuEmpleados[i]);
                    if (i % 50 == 0 & i > 0)
                    {
                        getSession().SaveChanges();
                    }
                }
                mensajeResultado.resultado = new Object[] { listPtuDatosGen, ptuDatosGenerales };
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("guardarCargaAcumulados()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje ptuDatosGeneralesPorEjercioyEmpresa(int ejercicio, string claveRazonsocial, DBContextAdapter dbContext)
        {
            PtuDatosGenerales ptuDatosGenerales = new PtuDatosGenerales();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                ptuDatosGenerales = (from p in getSession().Set<PtuDatosGenerales>()
                                     where p.ejercicio == ejercicio &&
                                     p.razonesSociales.clave == claveRazonsocial
                                     select p).SingleOrDefault();
                mensajeResultado.resultado = ptuDatosGenerales;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ptuDatosGeneralesPorEjercioyEmpresa()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje ptuEmpleadosPorEjercioyEmpresa(int ejercicio, string claveRazonsocial, DBContextAdapter dbContext)
        {
            List<PtuEmpleados> listptuEmpleados = new List<PtuEmpleados>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listptuEmpleados = (from p in getSession().Set<PtuEmpleados>()
                                    where p.ejercicio == ejercicio &&
                                    p.razonesSociales.clave == claveRazonsocial
                                    select p).ToList();
                mensajeResultado.resultado = listptuEmpleados;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ptuEmpleadosPorEjercioyEmpresa()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
    }
}