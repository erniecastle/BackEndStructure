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
    public class VacacionesDisfrutadasHDAO : NHibernateRepository<VacacionesDisfrutadas>, VacacionesDisfrutadasHDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Hibernate").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        private IList<VacacionesDisfrutadas> listEsp = new List<VacacionesDisfrutadas>();
        private Boolean commit = false;

        public Mensaje actualizar(VacacionesDisfrutadas entity, ISession uuidCxn)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(uuidCxn);
                getSession().BeginTransaction();
                makePersistent(entity);
                mensajeResultado.resultado=true;
                mensajeResultado.noError=0;
                mensajeResultado.error="";
                getSession().Transaction.Commit();
            }
            catch (HibernateException ex)
            {
               // System.err.println(concatena.delete(0, concatena.length()).append(msgError).append("actualizar()1_Error: ").append(ex));
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

        public Mensaje agregar(VacacionesDisfrutadas entity, ISession uuidCxn)
        {
            return agregar(entity, uuidCxn, true);
        }

        public Mensaje agregar(VacacionesDisfrutadas entity, ISession uuidCxn, bool usacommit)
        {
            VacacionesDisfrutadas vacacionesDisfrutadas;
            try
            {
                inicializaVariableMensaje();
                setSession(uuidCxn);
                getSession().BeginTransaction();
                vacacionesDisfrutadas = makePersistent(entity);
                mensajeResultado.resultado=vacacionesDisfrutadas;
                mensajeResultado.noError=0;
                mensajeResultado.error="";
                if (usacommit)
                {
                    getSession().Transaction.Commit();
                }
            }
            catch (HibernateException ex)
            {
                //System.err.println(concatena.delete(0, concatena.length()).append(msgError).append("agregar()1_Error: ").append(ex));
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

        public Mensaje eliminar(VacacionesDisfrutadas entity, ISession uuidCxn)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(uuidCxn);
                getSession().BeginTransaction();
                makeTransient(entity);
                mensajeResultado.resultado=true;
                mensajeResultado.noError=0;
                mensajeResultado.error="";
                getSession().Transaction.Commit();
            }
            catch (HibernateException ex)
            {
                //System.err.println(concatena.delete(0, concatena.length()).append(msgError).append("eliminar()1_Error: ").append(ex));
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

        public Mensaje EliminarVacacionesDisfrutadas(List<VacacionesAplicacion> vacAplicacion, ISession uuidCxn)
        {
            try
            {
                VacacionesDisfrutadas disfrutadas = null;
                inicializaVariableMensaje();
                setSession(uuidCxn);
                getSession().BeginTransaction();
                for (int i = 0; i < vacAplicacion.Count(); i++)
                {
                    vacAplicacion[i].vacacionesDevengadas=null;

                    if (disfrutadas == null)
                    {
                        disfrutadas = vacAplicacion[i].vacacionesDisfrutadas;
                    }
                    getSession().Delete(vacAplicacion[i]);
                }
                getSession().Delete(disfrutadas);
                mensajeResultado.resultado=true;
                mensajeResultado.noError=0;
                mensajeResultado.error="";
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

        public Mensaje getVacacionesDisfrutadasAll(string claveRazonesSocial, ISession uuidCxn)
        {
            IList<VacacionesDisfrutadas> values;
            try
            {
                inicializaVariableMensaje();
                setSession(uuidCxn);
                getSession().BeginTransaction();
                IQuery q = getSession().CreateQuery("from VacacionesDisfrutadas c where c.razonesSociales.clave=:clave");
                q.SetParameter("clave", claveRazonesSocial);
                values = q.List<VacacionesDisfrutadas>();
                mensajeResultado.resultado=values;
                mensajeResultado.noError=0;
                mensajeResultado.error="";
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

        public Mensaje getVacacionesPorEmpleado(string claveEmpleado, string claveRazonSocial, ISession uuidCxn)
        {
            IList<VacacionesDisfrutadas> values;
            try
            {
                inicializaVariableMensaje();
                setSession(uuidCxn);
                getSession().BeginTransaction();
                String query = "from VacacionesDisfrutadas r where r.empleados.clave = :claveEmpleado and r.razonesSociales.clave = :claveRazonSocial";
                IQuery q = getSession().CreateQuery(query);
                q.SetString("claveEmpleado", claveEmpleado);
                q.SetParameter("claveRazonSocial", claveRazonSocial);
                values = q.List<VacacionesDisfrutadas>();
                mensajeResultado.resultado = values;
                mensajeResultado.noError=0;
                mensajeResultado.error="";
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

        public Mensaje ObtenerVacacionesDisfruradasMaxima(string claveEmpleado, string claveRazonSocial, ISession uuidCxn)
        {
            VacacionesDisfrutadas values;
            try
            {
                inicializaVariableMensaje();
                setSession(uuidCxn);
                getSession().BeginTransaction();
                String query = "from VacacionesDisfrutadas r where r.empleados.clave = :claveEmpleado and r.razonesSociales.clave = :claveRazonSocial and r.salidaVacac IN(Select MAX(a.salidaVacac) from VacacionesDisfrutadas a where a.empleados.clave = :claveEmpleado and a.razonesSociales.clave = :claveRazonSocial )";
                IQuery q = getSession().CreateQuery(query);
                q.SetString("claveEmpleado", claveEmpleado);
                q.SetParameter("claveRazonSocial", claveRazonSocial);
                values = (VacacionesDisfrutadas)q.UniqueResult();
                mensajeResultado.resultado=values;
                mensajeResultado.noError=0;
                mensajeResultado.error="";
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

        public Mensaje saveDeleteVacacionesDisfrutadas(List<VacacionesDisfrutadas> entitysCambios, object[] clavesDelete, int rango, ISession uuidCxn)
        {
            listEsp = new List<VacacionesDisfrutadas>();
            try
            {
                commit = true;
                inicializaVariableMensaje();
                setSession(uuidCxn);
                getSession().BeginTransaction();
                if (clavesDelete != null)
                {
                    commit = deleteListQuerys("VacacionesDisfrutadas", "Id", clavesDelete);
                    if (commit)
                    {
                        getSession().Flush();
                        getSession().Clear();
                    }
                }
                entitysCambios = (entitysCambios == null ? new List<VacacionesDisfrutadas>() : entitysCambios);
                if (commit && !entitysCambios.Any())
                {
                    listEsp = agregarListaVacacionesDisfrutadas(entitysCambios, rango);

                }
                if (commit)
                {
                    mensajeResultado.resultado=listEsp;
                    mensajeResultado.noError=0;
                    mensajeResultado.error="";
                    getSession().Transaction.Commit();
                }
                else
                {
                    getSession().Transaction.Rollback();
                }
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

        private IList<VacacionesDisfrutadas> agregarListaVacacionesDisfrutadas(List<VacacionesDisfrutadas> entitys, int rango)
        {
            listEsp.Clear();
            try
            {
                int i = 0;
                for (i = 0; i < entitys.Count(); i++)
                {
                    if (entitys[i].id == 0)
                    {
                        listEsp.Add(makePersistent(entitys[i]));
                    }
                    else
                    {
                        makePersistent(entitys[i]);
                    }
                    if (i % rango == 0 & i > 0)
                    {
                        flush();
                        clear();
                    }
                }
            }
            catch (HibernateException ex)
            {
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().Message;
                mensajeResultado.resultado = null;
                commit = false;
            }
            return listEsp;
        }

        private bool deleteListQuerys(String tabla, String campo, Object[] valores)
        {
            bool exito = true;
            try
            {
                deleteListQuery(tabla, campo, valores);
            }
            catch (HibernateException ex)
            {
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().Message;
                mensajeResultado.resultado = null;
                exito = false;
            }
            return exito;
        }
    }
}
