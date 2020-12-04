using Exitosw.Payroll.Hibernate.entidad;
using Exitosw.Payroll.Hibernate.modelo;
using Exitosw.Payroll.Hibernate.util;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Exitosw.Payroll.Core.modeloHB
{
    public class VacacionesDevengadasHDAO : NHibernateRepository<VacacionesDevengadas>, VacacionesDevengadasHDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Hibernate").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        private StringBuilder query = new StringBuilder();
        public Dictionary<String, VacacionesDevengadas> devengadaActual { set; get; }
        public Mensaje calcularVacacionesDevengadasEmpleados(RazonesSociales razonesSociales, ISession uuidCxn, ISession uuidCxnMaestra)
        {
            return calcularVacacionesDevengadasEmpleados(razonesSociales, uuidCxn, uuidCxnMaestra, true);
        }

        public Mensaje getVacacionesDenvengadasPorEmpleado(string claveEmpleado, string claveRazonSocial, ISession uuidCxn)
        {
            IList<VacacionesDevengadas> values;
            try
            {
                inicializaVariableMensaje();
                setSession(uuidCxn);
                getSession().BeginTransaction();
                String query = "Select v from VacacionesDevengadas v   where v.plazasPorEmpleado.empleados.clave = :claveEmpleado and v.razonesSociales.clave = :claveRazonSocial";
                IQuery q = getSession().CreateQuery(query);
                q.SetString("claveEmpleado", claveEmpleado);
                q.SetParameter("claveRazonSocial", claveRazonSocial);
                values = q.List<VacacionesDevengadas>();
                mensajeResultado.resultado = (values);
                mensajeResultado.noError = (0);
                mensajeResultado.error = ("");
                getSession().Transaction.Commit();
            }
            catch (HibernateException ex)
            {
                try
                {
                    if (getSession().Transaction.IsActive)
                    {
                        getSession().Transaction.Rollback();
                    }
                    mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                    mensajeResultado.error = ex.GetBaseException().Message;
                }
                catch (HibernateException exc)
                {

                    mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(exc);
                    mensajeResultado.error = exc.GetBaseException().Message;
                }

                mensajeResultado.resultado = null;

            }

            return mensajeResultado;
        }

        public Mensaje getVacacionesDevengadasAll(ISession uuidCxn)
        {
            IList<VacacionesDevengadas> values;
            try
            {
                inicializaVariableMensaje();
                setSession(uuidCxn);
                getSession().BeginTransaction();
                IQuery q = getSession().CreateQuery("from VacacionesDevengadas");
                // q.setParameter("clave", claveRazonesSocial);
                values = q.List<VacacionesDevengadas>();
                mensajeResultado.resultado = values;
                mensajeResultado.error = "";
                mensajeResultado.noError = 0;
                getSession().Transaction.Commit();

            }
            catch (HibernateException ex)
            {
                try
                {
                    if (getSession().Transaction.IsActive)
                    {
                        getSession().Transaction.Rollback();
                    }
                    mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                    mensajeResultado.error = ex.GetBaseException().Message;
                }
                catch (HibernateException exc)
                {

                    mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(exc);
                    mensajeResultado.error = exc.GetBaseException().Message;
                }

                mensajeResultado.resultado = null;

            }

            return mensajeResultado;
        }

        public Mensaje saveDeleteVacacionesDevengadas(List<VacacionesDevengadas> entitysCambios, int rango, ISession uuidCxn)
        {
            IList<VacacionesDevengadas> listEsp = new List<VacacionesDevengadas>();
            try
            {
                inicializaVariableMensaje();
                setSession(uuidCxn);
                getSession().BeginTransaction();
                int i = 0;
                for (i = 0; i < entitysCambios.Count(); i++)
                {

                    listEsp.Add(makePersistent(entitysCambios[i]));

                }
                mensajeResultado.resultado = listEsp;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Transaction.Commit();
            }
            catch (HibernateException ex)
            {
                try
                {
                    if (getSession().Transaction.IsActive)
                    {
                        getSession().Transaction.Rollback();
                    }
                    mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                    mensajeResultado.error = ex.GetBaseException().Message;
                }
                catch (HibernateException exc)
                {

                    mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(exc);
                    mensajeResultado.error = exc.GetBaseException().Message;
                }

                mensajeResultado.resultado = null;

            }
            return mensajeResultado;
        }

        public Mensaje calcularVacacionesDevengadasEmpleados(RazonesSociales razonesSociales, ISession uuidCxn, ISession uuidCxnMaestra, bool usaCommit)
        {
            Object[,] reglaFactor = null;
            devengadaActual = new Dictionary<string, VacacionesDevengadas>();
            try
            {
                inicializaVariableMensaje();
                setSession(uuidCxn);
                getSession().BeginTransaction();
                query.Remove(0, query.Length);
                //Obtiene el control de vacaciones por día
                query.Append("from ControlVacDeveng o ");
                query.Append("where o.razonesSociales.clave =:claveRazonsocial ");
                query.Append("AND o.fecha = (select MAX(o.fecha) from ControlVacDeveng o) ");
                IQuery q = getSession().CreateQuery(query.ToString());
                q.SetParameter("claveRazonsocial", razonesSociales.clave);
                ControlVacDeveng control = (ControlVacDeveng)q.UniqueResult();
                DateTime? fechaUltimDev = null;
                List<DateTime> diasPendientes = null;
                if (control == null)
                {
                    diasPendientes = new List<DateTime>();
                    diasPendientes.Add(DateTime.Now);
                }
                else
                {
                    fechaUltimDev = control.fecha;
                    diasPendientes = getDaysBetweenDates(fechaUltimDev.GetValueOrDefault(), DateTime.Now);
                }
                query.Remove(0, query.Length);

                int d = 0;
                ControlVacDeveng controlCalculadas = null;
                for (d = 0; d < diasPendientes.Count(); d++)
                {
                    if (diasPendientes.Any()) {
                        //Obtiene empleados que cumplen aniversario en la empresa al día
                        query.Append("select o.plazasPorEmpleado from PlazasPorEmpleadosMov o ");
                        query.Append("where o.id IN (Select MAX(m.id) from PlazasPorEmpleadosMov m   ");
                        query.Append("where  m.plazasPorEmpleado.razonesSociales.clave =:claveRazonsocial ");
                        query.Append("AND  m.plazasPorEmpleado.fechaFinal >= :fechaActual ");
                        query.Append("AND month(o.plazasPorEmpleado.fechaPrestaciones)=month(:fechaActual) ");
                        query.Append("AND day(o.plazasPorEmpleado.fechaPrestaciones)=day(:fechaActual)");
                        query.Append("group by m.plazasPorEmpleado.empleados.clave)");
                        IList<PlazasPorEmpleado> plazasEmpleados = null;
                        q = getSession().CreateQuery(query.ToString());
                        q.SetParameter("claveRazonsocial", razonesSociales.clave);
                        q.SetParameter("fechaActual", diasPendientes[d]);
                        plazasEmpleados = q.List<PlazasPorEmpleado>();
                        plazasEmpleados = plazasEmpleados == null ? new List<PlazasPorEmpleado>() : plazasEmpleados;
                        query.Remove(0, query.Length);

                        if (plazasEmpleados.Any())
                        {//Obtiene  los factores de integracion
                            if (reglaFactor == null)
                            {
                                query.Append("from TablaDatos o where o.tablaBase.clave = :clave ");
                                query.Append("AND o.id = (SELECT MAX(t.id) FROM TablaDatos t WHERE t.tablaBase.id = o.tablaBase.id)");
                                setSession(uuidCxnMaestra);
                                getSession().BeginTransaction();
                                IList<Object> values;
                                q = getSession().CreateQuery(query.ToString());
                                q.SetParameter("clave", "05");
                                values = q.List<object>();
                                values = values == null ? new List<Object>() : values;
                                if (values.Count() > 0)
                                {
                                    byte[] convert = ((TablaDatos)values[0]).fileXml;
                                    reglaFactor = util.UtilidadesXML.extraeValoresNodos(util.UtilidadesXML.convierteBytesToXML(convert));
                                }
                                getSession().Transaction.Commit();
                                query.Remove(0, query.Length);
                            }
                            //Llenar tabla de Vacaciones Devengadas por día
                            VacacionesDevengadas vd = null;
                            setSession(uuidCxn);
                            getSession().BeginTransaction();
                            for (int i = 0; i < plazasEmpleados.Count(); i++)
                            {
                                Double antiguedad = (Double)calcularAntiguedadExacta(plazasEmpleados[i].fechaPrestaciones.GetValueOrDefault());
                                //antiguedad.intValue();
                                //Obtiene vacaciones devengadas por año a ese empleado
                                query.Append("select CASE WHEN (dev IS NULL) THEN 'NOCALCULADA' ELSE 'CALCULADA' END ");
                                query.Append("from VacacionesDevengadas dev where dev.plazasPorEmpleado.id =:idplaza AND dev.ejercicio =:ejercicio");
                                q = getSession().CreateQuery(query.ToString());
                                q.SetParameter("idplaza", plazasEmpleados[i].id);
                                q.SetParameter("ejercicio",Convert.ToInt32(antiguedad));
                                String calculada = (String)q.UniqueResult();
                                if (calculada == null)
                                {
                                    vd = new VacacionesDevengadas();
                                    vd.razonesSociales=plazasEmpleados[i].razonesSociales;
                                    vd.empleados=plazasEmpleados[i].empleados;
                                    vd.ejercicio=Convert.ToInt32(antiguedad);
                                    Object[] factorEmpleado = (Object[])obtieneFactorIntegracion(reglaFactor,Convert.ToInt32(antiguedad));
                                    query.Remove(0, query.Length);
                                    query.Append("select MAX(sdi.fecha),sdi.salarioDiarioFijo from SalariosIntegrados sdi WHERE ");
                                    query.Append("empleados.id= :idEmpleado AND fecha <= :fechaPrestacion GROUP BY sdi.salarioDiarioFijo");
                                    q = getSession().CreateQuery(query.ToString());
                                    q.SetParameter("idEmpleado", plazasEmpleados[i].empleados.id);
                                    q.SetParameter("fechaPrestacion", plazasEmpleados[i].fechaPrestaciones.GetValueOrDefault().Date);
                                    Object[] salarioAniv = (Object[])q.UniqueResult();
                                    if (salarioAniv == null)
                                    {
                                        vd.salarioAniversario=0.0;
                                    }
                                    else
                                    {
                                        vd.salarioAniversario=((Double)salarioAniv[1]);
                                    }
                                    vd.factorPrima=Double.Parse(factorEmpleado[4].ToString());
                                    vd.diasVacaciones=int.Parse(factorEmpleado[3].ToString());
                                    vd.registroInicial=false;
                                    double primaVac = Double.Parse(factorEmpleado[4].ToString())
                                            / 100 * int.Parse(factorEmpleado[3].ToString());
                                    vd.diasPrimaVaca=primaVac;
                                    getSession().Save(vd);
                                    devengadaActual[plazasEmpleados[i].empleados.clave]= vd;
                                }
                                query.Remove(0, query.Length);
                            }
                        }
                    }

                    controlCalculadas = new ControlVacDeveng();
                    controlCalculadas.fecha=diasPendientes[d];
                    controlCalculadas.razonesSociales=razonesSociales;//razon
                    getSession().Save(controlCalculadas);
                }




                if (usaCommit)
                {
                    getSession().Transaction.Commit();
                }
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
            }
            catch (HibernateException ex)
            {
                try
                {
                    if (getSession().Transaction.IsActive)
                    {
                        getSession().Transaction.Rollback();
                    }
                    mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                    mensajeResultado.error = ex.GetBaseException().Message;
                }
                catch (HibernateException exc)
                {

                    mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(exc);
                    mensajeResultado.error = exc.GetBaseException().Message;
                }

                mensajeResultado.resultado = null;

            }
            return mensajeResultado;
        }

        private Object obtieneFactorIntegracion(Object[,] reglaFactor, int antiguedad)
        {
            int i = 0;
            for (i = 0; i < reglaFactor.Length; i++)
            {
                int dataFact = int.Parse(reglaFactor[i,0].ToString());
                if (antiguedad <= dataFact)
                {
                    break;
                }
            }
            return reglaFactor[i,0];
        }

        private Object calcularAntiguedadExacta(DateTime fechaInicial)
        {
            try
            {
                fechaInicial = fechaInicial.Date;
                DateTime fechaFinal = DateTime.Now;

                long diferencia;
                diferencia = (long)((fechaFinal - fechaInicial).TotalMilliseconds);
                double dias, antiguedad, antiguedadDias;
                dias = (double)Math.Floor((double)(diferencia / (1000 * 60 * 60 * 24)));
                antiguedad = dias / 365;
                antiguedadDias = (dias % 365);
                return antiguedad;
            }
            catch (Exception ex)
            {
                mensajeResultado.noError = 22;
                mensajeResultado.error = String.Concat("Error al calcular antiguedad exacta", " ", ex.GetBaseException().ToString());
            }
            return 0.0;
        }

        private List<DateTime> getDaysBetweenDates(DateTime startdate, DateTime enddate)
        {
            List<DateTime> dates = new List<DateTime>();
            DateTime calendar = startdate;
            calendar = calendar.AddDays(1);


            while (calendar.Date < enddate.Date)
            {
                DateTime result = calendar;
                dates.Add(result);
                calendar = calendar.AddDays(1);
            }
            return dates;
        }
    }
}
