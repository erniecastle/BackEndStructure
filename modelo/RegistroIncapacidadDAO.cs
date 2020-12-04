/**
* @author: Daniel Ruelas 
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Clase RegistroIncapacidadDAO para llamados a metodos de Entity
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
using Exitosw.Payroll.TestCompilador.funciones;
using System.Data.Entity;
using Exitosw.Payroll.Core.campos;
using Exitosw.Payroll.Core.genericos.campos;

namespace Exitosw.Payroll.Core.modelo
{
    public class RegistroIncapacidadDAO : GenericRepository<Object>, RegistroIncapacidadDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        private List<RegistroIncapacidad> listEsp = new List<RegistroIncapacidad>();
        private bool commit = false;
        public Mensaje agregar(RegistroIncapacidad entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<RegistroIncapacidad>().Add(entity);
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
        public Mensaje modificar(RegistroIncapacidad entity, DBContextAdapter dbContext)
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
                getSession().Set<RegistroIncapacidad>().AddOrUpdate(entity);
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

        public Mensaje eliminar(RegistroIncapacidad entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                //var sentencia = (from g in getSession().Set<Genero>()
                //                 where g.id == entity.id
                //                 select g).FirstOrDefault();
                getSession().Set<RegistroIncapacidad>().Attach(entity);
                getSession().Set<RegistroIncapacidad>().Remove(entity);
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
        public Mensaje consultaPorFiltrosRegistroIncapacidad(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
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
                        campo.campo = "Paises." + item.Key.ToString();
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorFiltrosIncapacidad()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje consultaPorRangosRegistroIncapacidad(Int64 inicio, Int64 rango, DBContextAdapter dbContext)
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorRangosIncapacidad()1_Error: ").Append(ex));
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


        //        mensajeResultado.resultado = existe;
        //        mensajeResultado.noError = 0;
        //        mensajeResultado.error = "";
        //        getSession().Database.CurrentTransaction.Commit();

        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorRangosIncapacidad()1_Error: ").Append(ex));
        //        mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
        //        mensajeResultado.error = ex.GetBaseException().ToString();
        //        mensajeResultado.resultado = null;
        //        getSession().Database.CurrentTransaction.Rollback();
        //    }
        //    return mensajeResultado;
        //}

        public Mensaje getIncapacidadPorEmpleadoYFecha(string claveEmpleado, DateTime fechaInicial, DateTime fechaFinal, DBContextAdapter dbContext)
        {
            List<RegistroIncapacidad> registrosIncapacidades = new List<RegistroIncapacidad>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var query = from p in getSession().Set<RegistroIncapacidad>()
                            where (p.fechaInicial >= fechaInicial && p.fechaInicial <= fechaFinal) ||
                                  (p.fechaFinal >= fechaInicial && p.fechaFinal <= fechaFinal)
                            select p;
                if (claveEmpleado != null)
                {
                    query = from sub in query
                            where sub.empleados.clave == claveEmpleado
                            select sub;
                }
                registrosIncapacidades = query.ToList();
                mensajeResultado.resultado = registrosIncapacidades;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getIncapacidadPorEmpleadoYFecha()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getAllRegistroIncapacidad(string claveRazonesSocial, DBContextAdapter dbContext)
        {
            List<RegistroIncapacidad> registrosIncapacidades = new List<RegistroIncapacidad>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                registrosIncapacidades = (from p in getSession().Set<RegistroIncapacidad>()
                                          select p).ToList();
                mensajeResultado.resultado = registrosIncapacidades;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getRegistroIncapacidadAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getRegistroIncapacidadPorClave(string clave, string claveRazonesSocial, DBContextAdapter dbContext)
        {
            throw new NotImplementedException();//registro de incapacidades no contiene razon social
        }

        public Mensaje getRegistroIncapacidadPorClaveYRazon(string clave, string claveRazon, DBContextAdapter dbContext)
        {
            throw new NotImplementedException();//registro de incapacidades no contiene razon social
        }

        public Mensaje getRegistroIncapacidadPorEmpleado(Empleados empleado, DBContextAdapter dbContext)
        {
            List<RegistroIncapacidad> registrosIncapacidades = new List<RegistroIncapacidad>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                registrosIncapacidades = (from p in getSession().Set<RegistroIncapacidad>()
                                          where p.empleados.id == empleado.id
                                          select p).ToList();
                mensajeResultado.resultado = registrosIncapacidades;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getRegistroIncapacidadPorEmpleado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje saveDeleteRegistroIncapacidad(RegistroIncapacidad incapacidad, object[] clavesDeleteIncapacidad, int rango, Empleados empleados, RazonesSociales razonesSociales, DateTime fechaInicial, DateTime fechaFinal, DateTime fechaInicialAnterior, DateTime fechaFinalAnterior, object claveExcepcion, string formatoFecha, DateTime fechaInicEmpalme, DateTime fechaFinEmpalme, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            try
            {
                commit = true;
                inicializaVariableMensaje();
                RegistroIncapacidad registro = null;
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                List<TablaDatos> organized = (from o in getSession().Set<TablaDatos>()
                                              where o.tablaBase.clave == ClavesParametrosModulos.claveTipoTablaDiasFestivos.ToString() &&
                                              o.id == (from t in getSession().Set<TablaDatos>()
                                                       where t.tablaBase.id == o.tablaBase.id &&
                                                     t.controlPorFecha <= DateTime.Now
                                                       select new
                                                       {
                                                           t.id
                                                       }).Max(p => p.id)
                                              select o).ToList();
                organized = organized == null ? new List<TablaDatos>() : organized;
                List<DateTime> diasFestivos = new List<DateTime>();
                if (organized.Count > 0)
                {
                    byte[] convert = organized[0].fileXml;
                    Object[,] dias = UtilidadesXML.extraeValoresNodos(UtilidadesXML.convierteBytesToXML(convert));
                    DateTime fecha;
                    int i;
                    for (i = 0; i < dias.Length; i++)
                    {
                        fecha = Convert.ToDateTime(dias[i, 0].ToString());
                        diasFestivos.Add(fecha);
                    }
                }
                getSession().Database.CurrentTransaction.Commit();

                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (clavesDeleteIncapacidad.Length > 0)
                {
                    commit = deleteListQuerys("RegistroIncapacidad", new CamposWhere("RegistroIncapacidad.id", clavesDeleteIncapacidad, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                    //deleteListQuerys("RegistroIncapacidad", "id", clavesDeleteIncapacidad);
                    registro = new RegistroIncapacidad();

                }

                if (commit)
                {
                    Object[] clavesDetallesAsistencia = getDetalleAsistencia(empleados, fechaInicialAnterior, fechaFinalAnterior, razonesSociales);
                    if (clavesDetallesAsistencia != null)
                    {
                        commit = deleteListQuerys("DetalleAsistencia", new CamposWhere("DetalleAsistencia.id", clavesDetallesAsistencia, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                        //deleteListQuerys("DetalleAsistencia", "id", clavesDetallesAsistencia);
                    }

                }
                if (commit)
                {
                    List<Asistencias> asistencias = getAsistenciasPorRangoFechas(empleados.clave, fechaInicial, fechaFinal, razonesSociales.clave);
                    if (mensajeResultado.noError == 0)
                    {
                        if (asistencias != null)
                        {
                            if (asistencias.Count > 0)
                            {
                                Object[] clavesDetallesAsistencia = new Object[asistencias.Count];
                                for (int i = 0; i < asistencias.Count; i++)
                                {
                                    clavesDetallesAsistencia[i] = asistencias[i].id;
                                }
                                commit = deleteListQuerys("Asistencias", new CamposWhere("Asistencias.id", clavesDetallesAsistencia, OperadorComparacion.IN, OperadorLogico.AND), dbContext);

                                //deleteListQuerys("Asistencias", "id", clavesDetallesAsistencia);
                            }
                        }
                    }
                }

                if (commit & incapacidad != null)
                {
                    DatosGlobales datosGlobales = cargaDatosGlobales(empleados, razonesSociales, fechaInicial, fechaFinal, claveExcepcion);
                    List<Asistencias> asistencias = obtenerAsistencias(incapacidad, empleados, fechaInicialAnterior, fechaFinalAnterior, fechaInicial, fechaFinal, razonesSociales, datosGlobales, diasFestivos, fechaInicEmpalme, fechaFinEmpalme);
                    commit = agregarListaAsistencias(asistencias, rango);
                }
                if (commit & incapacidad != null)
                {
                    registro = getSession().Set<RegistroIncapacidad>().Add(incapacidad);
                }
                if (commit)
                {
                    mensajeResultado.resultado = registro;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }
                else
                {
                    mensajeResultado.resultado = null;
                    getSession().Database.CurrentTransaction.Rollback();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteRegistroIncapacidad()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        public class DatosGlobales
        {

            public DatosGlobales()
            {
                plazas = new List<PlazasPorEmpleadosMov>();
                mapTipoNomina = new Dictionary<string, List<PeriodosNomina>>();
            }

            public List<PlazasPorEmpleadosMov> plazas { get; set; }
            public Object claveExcepcion { get; set; } = "";
            public Excepciones excepIncapacidad { get; set; }
            public Excepciones excepDefault { get; set; }
            public Excepciones excepFestivo { get; set; }
            public Dictionary<string, List<PeriodosNomina>> mapTipoNomina { get; set; }

        }
        private DatosGlobales cargaDatosGlobales(Empleados empleados, RazonesSociales razonesSociales, DateTime fechaInicial, DateTime fechaFinal, Object claveExcepcion)
        {
            DatosGlobales dg = new DatosGlobales();
            int i;
            List<PlazasPorEmpleadosMov> plazas = getPorEmpleadoYRazonSocialVigente(empleados.clave, razonesSociales.clave, fechaFinal);
            if (mensajeResultado.noError != 0)
            {
                return null;
            }
            Excepciones excep, excepLaborado, excepFestivo;
            excep = getExcepcionPorClave(claveExcepcion);
            if (mensajeResultado.noError != 0)
            {
                return null;
            }
            excepLaborado = getExcepcionPorClave(ClavesParametrosModulos.claveExcepcionLaborado);
            if (mensajeResultado.noError != 0)
            {
                return null;
            }
            excepFestivo = getExcepcionPorClave(ClavesParametrosModulos.claveExcepcionFestivo);
            if (mensajeResultado.noError != 0)
            {
                return null;
            }
            DateTime inicioPeriodo = DateTime.Now;
            DateTime finPeriodo = DateTime.Now;
            Dictionary<string, List<PeriodosNomina>> mapTipoNomina = new Dictionary<string, List<PeriodosNomina>>();
            for (i = 0; i < plazas.Count; i++)
            {
                List<PeriodosNomina> periodos = getPeriodosNominaPorTipoNominaYRangoDeFechas(inicioPeriodo, finPeriodo, plazas[i].tipoNomina.clave);
                if (mensajeResultado.noError != 0)
                {
                    return null;
                }
                periodos = periodos == null ? new List<PeriodosNomina>() : periodos;
                mapTipoNomina.Add(plazas[i].tipoNomina.clave, periodos);
            }
            dg.claveExcepcion = claveExcepcion;
            dg.excepDefault = excepLaborado;
            dg.excepFestivo = excepFestivo;
            dg.mapTipoNomina = mapTipoNomina;
            dg.plazas = plazas;

            return dg;
        }

        private List<PlazasPorEmpleadosMov> getPorEmpleadoYRazonSocialVigente(String claveEmpleado, String claveRazonSocial, DateTime fechaFinal)
        {
            List<PlazasPorEmpleadosMov> values = null;
            try
            {
                values = (from o in getSession().Set<PlazasPorEmpleadosMov>()
                          where (from m in getSession().Set<PlazasPorEmpleadosMov>()
                                 where m.plazasPorEmpleado.empleados.clave == claveEmpleado &&
                                     m.plazasPorEmpleado.razonesSociales.clave == claveRazonSocial &&
                                     fechaFinal >= m.fechaInicial && fechaFinal <= m.plazasPorEmpleado.fechaFinal
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

                mensajeResultado.resultado = values;
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
            return values;

        }
        private Excepciones getExcepcionPorClave(Object clave)
        {
            Excepciones e = null;
            try
            {
                e = (from ex in getSession().Set<Excepciones>()
                     where ex.clave == clave.ToString()
                     select ex).SingleOrDefault();
                mensajeResultado.resultado = e;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getExcepcionPorClave()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return e;

        }
        private List<PeriodosNomina> getPeriodosNominaPorTipoNominaYRangoDeFechas(DateTime fechaInicial, DateTime fechaFinal, String claveTipoNomina)
        {
            List<PeriodosNomina> values = null;
            try
            {
                values = (from p in getSession().Set<PeriodosNomina>()
                          join cor in getSession().Set<TipoCorrida>()
                          on p.tipoCorrida.id equals cor.id
                          where p.tipoNomina.clave == claveTipoNomina &&
                          cor.clave == "PER" &&
                          p.fechaFinal >= fechaInicial && p.fechaInicial <= fechaFinal
                          select p).ToList();
                mensajeResultado.resultado = values;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPeriodosNominaPorTipoNominaYRangoDeFechas()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;

            }
            return values;
        }

        private List<DetalleAsistencia> getDetalleAsistenciaPorRangoFechas(String claveEmpleado, DateTime fechaInicio, DateTime fechaFinal, String claveRazonesSociales)
        {
            List<DetalleAsistencia> values = null;
            try
            {
                inicializaVariableMensaje();
                setSession(null);
                getSession().Database.BeginTransaction();
                values = (from a in getSession().Set<DetalleAsistencia>()
                          where a.empleados.clave == claveEmpleado &&
                          a.dia >= fechaInicio && a.dia <= fechaFinal &&
                          a.razonesSociales.clave == claveRazonesSociales
                          select a).ToList();
                mensajeResultado.resultado = values;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getDetalleAsistenciaPorRangoFechas()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return values;

        }

        private Object[] getDetalleAsistencia(Empleados empleados, DateTime fechaInicial, DateTime fechaFinal, RazonesSociales razonesSociales)
        {
            Object[] clavesDetallesAsistencia = null;
            List<DetalleAsistencia> detalleAsis = getDetalleAsistenciaPorRangoFechas(empleados.clave, fechaInicial, fechaFinal, razonesSociales.clave);
            if (mensajeResultado.noError != 0)
            {
                return clavesDetallesAsistencia;
            }
            detalleAsis = (detalleAsis == null) ? new List<DetalleAsistencia>() : detalleAsis;
            if (detalleAsis.Count > 0)
            {
                clavesDetallesAsistencia = new Object[detalleAsis.Count];
                for (int i = 0; i < detalleAsis.Count; i++)
                {
                    clavesDetallesAsistencia[i] = detalleAsis[i].id;
                }
            }
            return clavesDetallesAsistencia;
        }
        private List<Asistencias> getAsistenciasPorRangoFechas(String claveEmpleado, DateTime fechaInicio, DateTime fechaFinal, String claveRazonesSociales)
        {
            List<Asistencias> values = null;
            try
            {
                inicializaVariableMensaje();
                setSession(null);
                getSession().Database.BeginTransaction();
                values = (from a in getSession().Set<Asistencias>()
                          where a.empleados.clave == claveEmpleado &&
                          a.fecha >= fechaInicio && a.fecha <= fechaFinal &&
                          a.razonesSociales.clave == claveRazonesSociales &&
                          a.empleados.razonesSociales.clave == claveRazonesSociales
                          select a).ToList();
                mensajeResultado.resultado = values;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getAsistenciasPorRangoFechas()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;

            }
            return values;

        }

        private List<Asistencias> obtenerAsistencias(RegistroIncapacidad registroIncapacidad, Empleados empleados, DateTime fechaInicialAnterior, DateTime fechaFinalAnterior, DateTime fechaInicial, DateTime fechaFinal, RazonesSociales razonesSociales, DatosGlobales datoGlobal, List<DateTime> diasFestivos, DateTime fechaIniEmpalme, DateTime fechaFinEmpalme)
        {
            List<Asistencias> asistencias;
            if (fechaInicialAnterior == null | fechaFinalAnterior == null)
            {
                asistencias = getAsistenciasPorRangoFechas(empleados.clave, fechaInicial, fechaFinal, razonesSociales.clave);
            }
            else
            {
                asistencias = getAsistenciasPorRangoFechas(empleados.clave, fechaInicialAnterior, fechaFinalAnterior, razonesSociales.clave);

            }
            if (mensajeResultado.noError != 0)
            {
                return null;
            }
            asistencias = asistencias == null ? new List<Asistencias>() : asistencias;

            int i;
            bool usaTiempoExtra = false;
            for (i = 0; i < asistencias.Count; i++)
            {
                if (asistencias[i].excepciones.clave.Equals(ClavesParametrosModulos.claveExcepcionTiempoExtra.ToString())
                    || asistencias[i].excepciones.clave.Equals(ClavesParametrosModulos.claveExcepcionExtraDoble.ToString())
                    || asistencias[i].excepciones.clave.Equals(ClavesParametrosModulos.claveExcepcionExtraTriple.ToString()))
                {

                    usaTiempoExtra = true;
                    asistencias[i].cantidad = null;
                }
            }
            asistencias = creaListaAsistenciasIncapacidad(asistencias, registroIncapacidad, datoGlobal, razonesSociales, diasFestivos, fechaIniEmpalme, fechaFinEmpalme, fechaInicial, fechaFinal);
            return asistencias;

        }

        private List<Asistencias> creaListaAsistenciasIncapacidad(List<Asistencias> asistencias, RegistroIncapacidad registroIncapacidad, DatosGlobales datoGlobal, RazonesSociales razonesSociales, List<DateTime> diasFestivos, DateTime fechaIniEmpalme, DateTime fechaFinEmpalme, DateTime fechaInicial, DateTime fechaFinal)
        {
            int i, j, k, cont = 0, orden = 0;
            TipoNomina nomina;
            List<PeriodosNomina> periodos;
            PeriodosNomina periodo = null;
            Asistencias asis;
            try
            {
                DateTime fechainicio = DateTime.Now;
                if (asistencias.Count == 0)
                {
                    fechainicio = Convert.ToDateTime(registroIncapacidad.fechaInicial);
                    for (i = 0; i < registroIncapacidad.diasIncapacidad; i++)
                    {
                        fechainicio = fechainicio.AddDays(cont);
                        cont = 1;
                        for (k = 0; k < datoGlobal.plazas.Count; k++)
                        {
                            nomina = datoGlobal.plazas[k].tipoNomina;
                            periodos = (List<PeriodosNomina>)datoGlobal.mapTipoNomina[nomina.clave];
                            for (j = 0; j < periodos.Count; j++)
                            {
                                if (fechainicio.CompareTo(periodos[j].fechaInicial) > 0 && fechainicio.CompareTo(periodos[j].fechaFinal) < 0 || fechainicio.CompareTo(periodos[j].fechaInicial) == 0 || fechainicio.CompareTo(periodos[j].fechaFinal) == 0)
                                {
                                    periodo = periodos[j];
                                    break;
                                }
                            }
                            asis = creaAsistenciaIncapacidad(registroIncapacidad.empleados, nomina, fechainicio, datoGlobal.excepIncapacidad, periodo, razonesSociales);
                            asis.ordenId = orden;
                            orden++;
                            asistencias.Add(asis);
                        }
                    }
                }
                else
                {
                    List<Asistencias> cambiosAsistencias = new List<Asistencias>();
                    List<DateTime> listFechas = new List<DateTime>();
                    for (i = 0; i < registroIncapacidad.diasIncapacidad; i++)
                    {
                        fechainicio = Convert.ToDateTime(registroIncapacidad.fechaInicial);
                        fechainicio = fechainicio.AddDays(i);
                        listFechas.Add(fechainicio);

                    }
                    cont = 0;
                    j = 0;
                    bool existe;
                    fechainicio = DateTime.Now;
                    while (j < asistencias.Count)
                    {
                        existe = false;
                        cont = 0;
                        fechainicio = Convert.ToDateTime(registroIncapacidad.fechaInicial);
                        for (i = 0; i < registroIncapacidad.diasIncapacidad; i++)
                        {
                            fechainicio = fechainicio.AddDays(cont);
                            cont = 1;
                            if (fechainicio.CompareTo(asistencias[j].fecha) == 0)
                            {
                                existe = true;
                                listFechas.Remove(fechainicio);
                                break;
                            }
                        }

                        if (existe)
                        {
                            asistencias[j].excepciones = datoGlobal.excepIncapacidad;
                            cambiosAsistencias.Add(asistencias[j]);
                            asistencias.RemoveAt(j);
                        }
                        else
                        {
                            j++;
                        }
                    }
                    for (i = 0; i < listFechas.Count; i++)
                    {
                        fechainicio = listFechas[i];
                        for (k = 0; k < datoGlobal.plazas.Count; k++)
                        {
                            nomina = datoGlobal.plazas[k].tipoNomina;
                            periodos = (List<PeriodosNomina>)datoGlobal.mapTipoNomina[nomina.clave];
                            for (j = 0; j < periodos.Count; j++)
                            {
                                if (fechainicio.CompareTo(periodos[j].fechaInicial) > 0 && fechainicio.CompareTo(periodos[j].fechaFinal) < 0 || fechainicio.CompareTo(periodos[j].fechaInicial) == 0 || fechainicio.CompareTo(periodos[j].fechaFinal) == 0)
                                {
                                    periodo = periodos[j];
                                    break;
                                }
                                asis = creaAsistenciaIncapacidad(registroIncapacidad.empleados, nomina, fechainicio, datoGlobal.excepIncapacidad, periodo, razonesSociales);
                                cambiosAsistencias.Add(asis);
                            }
                        }
                    }
                    bool isdiasFestivos;
                    for (i = 0; i < asistencias.Count; i++)
                    {
                        isdiasFestivos = false;
                        for (j = 0; j < diasFestivos.Count; j++)
                        {
                            if (diasFestivos[j].CompareTo(asistencias[i].fecha) == 0)
                            {
                                isdiasFestivos = true;
                                break;
                            }
                        }
                        if (isdiasFestivos)
                        {
                            asistencias[i].excepciones = datoGlobal.excepFestivo;
                            asistencias[i].jornada = 0.0;
                        }
                        else
                        {
                            if (fechaIniEmpalme != null && fechaFinEmpalme != null)
                            {
                                if (!(Convert.ToDateTime(asistencias[i].fecha).CompareTo(fechaIniEmpalme) >= 0 && (Convert.ToDateTime(asistencias[i].fecha).CompareTo(fechaFinEmpalme) <= 0)))
                                {
                                    asistencias[i].excepciones = datoGlobal.excepDefault;
                                    asistencias[i].jornada = 8.0;
                                }
                            }
                            else
                            {
                                if (!(Convert.ToDateTime(asistencias[i].fecha).CompareTo(fechaIniEmpalme) >= 0 && (Convert.ToDateTime(asistencias[i].fecha).CompareTo(fechaFinEmpalme) <= 0)))
                                {
                                    asistencias[i].excepciones = datoGlobal.excepDefault;
                                    asistencias[i].jornada = 8.0;
                                }
                            }
                        }
                    }
                    //Utilerias.ordena(cambiosAsistencias, "Fecha");
                    for (i = 0; i < cambiosAsistencias.Count; i++)
                    {
                        cambiosAsistencias[i].ordenId = i;
                    }
                    asistencias = cambiosAsistencias;
                }

            }
            catch (Exception ex)
            {
                commit = false;
                mensajeResultado.noError = 200;
                mensajeResultado.error = ex.Message;

            }
            return asistencias;
        }
        private Asistencias creaAsistenciaIncapacidad(Empleados emp, TipoNomina nomina, DateTime fecha, Excepciones excepcion, PeriodosNomina periodo, RazonesSociales razonesSociales)
        {
            List<Asistencias> listAsistencia = getAsistenciasPorRangoFechas(emp.clave, fecha, fecha, razonesSociales.clave);//JSA02
            Asistencias asist = new Asistencias();
            if (listAsistencia == null)
            {
                listAsistencia = new List<Asistencias>();
            }
            if (listAsistencia.Count > 0)
            {
                asist = listAsistencia[0];
            }
            asist.razonesSociales = razonesSociales;
            asist.empleados = emp;
            asist.tipoNomina = nomina;
            asist.periodosNomina = periodo;
            asist.excepciones = excepcion;
            asist.fecha = fecha;
            asist.jornada = 8.0;
            asist.tipoPantalla = 100;

            return asist;
        }

        private bool agregarListaAsistencias(List<Asistencias> asistencias, int rango)
        {
            bool exito = true;
            try
            {
                int i;
                for (i = 0; i < asistencias.Count; i++)
                {
                    getSession().Set<Asistencias>().Add(asistencias[i]);
                    if (i % rango == 0 & i > 0)
                    {
                        getSession().SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("agregarListaAsistencias()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                exito = false;
            }
            return exito;
        }
        private bool deleteListQuerys(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext)
        {
            bool exito = true;
            try
            {
                //deleteListQuery(tabla, campo, valores);
                deleteListQuery(tabla, campoWhere, dbContext);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("deleteListQuerys()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                exito = false;
            }
            return exito;
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

        public Mensaje getRegistroIncapacidadPorID(decimal id, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var registrosIncapacidades = (from p in getSession().Set<RegistroIncapacidad>()
                                              where p.id == id
                                              select new
                                              {
                                                  p.clave,
                                                  p.controlIncapacidad,
                                                  p.diasIncapacidad,
                                                  empleados = new
                                                  {
                                                      p.empleados.clave,
                                                      p.empleados.nombre,
                                                      p.empleados.apellidoPaterno,
                                                      p.empleados.apellidoMaterno,
                                                      claveRazon = p.empleados.razonesSociales.clave
                                                  },
                                                  p.empleados_ID,
                                                  p.fechaFinal,
                                                  p.fechaInicial,
                                                  p.id,
                                                  incapacidadAnterior=p.incapacidadAnterior==null ? null : new {p.incapacidadAnterior.clave,p.incapacidadAnterior.ramoSeguro },
                                                  p.incapacidadAnterior_ID,
                                                  p.pagarTresPrimeroDias,
                                                  p.porcentaje,
                                                  p.ramoSeguro,
                                                  p.secuelaConsecuencia,
                                                  p.tipoRiesgo
                                              }).SingleOrDefault();
                mensajeResultado.resultado = registrosIncapacidades;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getRegistroIncapacidadPorID()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getRegistroIncapacidadPorEmpleadoID(decimal idempleado,decimal idRazon, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
               var registrosIncapacidades = (from p in getSession().Set<RegistroIncapacidad>()
                                          where p.empleados.id == idempleado && p.empleados.razonesSociales.id==idRazon
                                          select new {
                                              p.clave,
                                              p.fechaInicial,
                                              p.fechaFinal,
                                              p.diasIncapacidad,
                                              p.ramoSeguro,
                                              p.secuelaConsecuencia
                                          }).ToList();
                mensajeResultado.resultado = registrosIncapacidades;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getRegistroIncapacidadPorEmpleadoID()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
    }
}