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
    public class VacacionesAplicacionHDAO : NHibernateRepository<VacacionesAplicacion>, VacacionesAplicacionHDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Hibernate").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        private List<VacacionesAplicacion> listEsp = new List<VacacionesAplicacion>();
        private bool commit = false;

        public Mensaje agregar(VacacionesAplicacion entity, ISession uuidCxn)
        {
            VacacionesAplicacion vacacionesAplicacion;
            try
            {
                inicializaVariableMensaje();
                setSession(uuidCxn);
                getSession().BeginTransaction();
                vacacionesAplicacion = makePersistent(entity);
                mensajeResultado.resultado = true;
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

        public Mensaje actualizar(VacacionesAplicacion entity, ISession uuidCxn)
        {
            VacacionesAplicacion vacacionesAplicacion;
            try
            {
                inicializaVariableMensaje();
                setSession(uuidCxn);
                getSession().BeginTransaction();
                vacacionesAplicacion = makePersistent(entity);
                mensajeResultado.resultado = true;
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


        public Mensaje eliminar(VacacionesAplicacion entity, ISession uuidCxn)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(uuidCxn);
                getSession().BeginTransaction();
                makeTransient(entity);
                mensajeResultado.resultado = true;
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

        public Mensaje getVacacionesAplicacionAll(string claveRazonesSocial, ISession uuidCxn)
        {
            IList<VacacionesAplicacion> values;
            try
            {
                inicializaVariableMensaje();
                setSession(uuidCxn);
                getSession().BeginTransaction();
               IQuery q = getSession().CreateQuery("from VacacionesAplicacion");
                // q.setParameter("clave", claveRazonesSocial);
                values = q.List<VacacionesAplicacion>();
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

        public Mensaje getVacacionesPorEmpleado(string claveEmpleado, string claveRazonSocial, ISession uuidCxn)
        {
            IList<VacacionesAplicacion> values;
            try
            {
                inicializaVariableMensaje();
                setSession(uuidCxn);
                getSession().BeginTransaction();
                String query = "Select v from VacacionesAplicacion v inner join v.vacacionesDisfrutadas vd where vd.empleados.clave = :claveEmpleado and vd.razonesSociales.clave = :claveRazonSocial";
                IQuery q = getSession().CreateQuery(query);
                q.SetString("claveEmpleado", claveEmpleado);
                q.SetParameter("claveRazonSocial", claveRazonSocial);
                values = q.List<VacacionesAplicacion>();
                mensajeResultado.resultado=(values);
                mensajeResultado.noError=(0);
                mensajeResultado.error=("");
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

        public Mensaje saveDeleteVacacionesAplicacion(List<VacacionesAplicacion> entitysCambios, object[] clavesDelete, int rango, ISession uuidCxn)
        {
            listEsp = new List<VacacionesAplicacion>();
            try
            {
                commit = true;
                inicializaVariableMensaje();
                setSession(uuidCxn);
                getSession().BeginTransaction();
                if (clavesDelete != null) {
                    commit = deleteListQuerys("VacacionesAplicacion", "Id", clavesDelete);
                    if (commit)
                    {
                        getSession().Flush();
                        getSession().Clear();
                    }
                }
                entitysCambios = (entitysCambios == null ? new List<VacacionesAplicacion>() : entitysCambios);
                if (commit && entitysCambios.Any())
                {
                    listEsp = agregarListaVacacionesAplicacion(entitysCambios, rango);

                }
                if (commit)
                {
                    mensajeResultado.resultado= listEsp;
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

        private List<VacacionesAplicacion> agregarListaVacacionesAplicacion(List<VacacionesAplicacion> entitys, int rango)
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
