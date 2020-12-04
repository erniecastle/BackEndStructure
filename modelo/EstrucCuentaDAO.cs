/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Clase EstrucCuentaDAO para llamados a metodos de Entity
* -----------------------------------------------------------------------------
* MODIFICACIONES:
* -----------------------------------------------------------------------------
* Clave: 
* Autor: 
* Fecha:
* Descripción: 
* -----------------------------------------------------------------------------
*/

using Exitosw.Payroll.Entity.entidad.contabilidad;
using System;
using System.Collections.Generic;
using System.Linq;
using Exitosw.Payroll.Entity.entidad;
using System.Text;
using System.Reflection;
using Exitosw.Payroll.Core.util;
using System.Data.Entity.Migrations;
using Exitosw.Payroll.Entity.util;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public class EstrucCuentaDAO : GenericRepository<EstrucCuenta>, EstrucCuentaDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");

        public Mensaje agregar(List<EstrucCuenta> entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                for (int i = 0; i <= entity.Count(); i++)
                {
                    getSession().Set<EstrucCuenta>().Add(entity[i]);
                    getSession().SaveChanges();
                    mensajeResultado.resultado = entity;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
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
            return mensajeResultado;

        }

        public Mensaje actualizar(List<EstrucCuenta> entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                for (int i = 0; i <= entity.Count(); i++)
                {
                    getSession().Set<EstrucCuenta>().AddOrUpdate(entity[i]);
                    getSession().SaveChanges();
                    mensajeResultado.resultado = true;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                }
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

        public Mensaje eliminar(List<EstrucCuenta> entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                for (int i = 0; i <= entity.Count(); i++)
                {
                    getSession().Set<EstrucCuenta>().Attach(entity[i]);
                    getSession().Set<EstrucCuenta>().Remove(entity[i]);
                    getSession().SaveChanges();
                    mensajeResultado.resultado = true;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                }
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