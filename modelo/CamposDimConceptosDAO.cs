/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Clase CamposDimConceptosDAO para llamados a metodos de Entity
* -----------------------------------------------------------------------------
* MODIFICACIONES:
* -----------------------------------------------------------------------------
* Clave: 
* Autor: 
* Fecha:
* Descripción: 
* -----------------------------------------------------------------------------
*/

using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Core.util;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Text;
using Exitosw.Payroll.Entity.util;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public class CamposDimConceptosDAO : GenericRepository<CamposDimConceptos>, CamposDimConceptosDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");

        public Mensaje filtradoCampoDim(int campoDim, DBContextAdapter dbContext)
        {
            List<CamposDimConceptos> listaCamposDimFilt = new List<CamposDimConceptos>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listaCamposDimFilt = (from a in getSession().Set<CamposDimConceptos>()
                                           where a.campoDIM_ID == campoDim
                                           select a).ToList();
                mensajeResultado.resultado = listaCamposDimFilt;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("filtradoCampoDim()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje filtradoCampoDimConceptos(int campoDim, int Concepto, DBContextAdapter dbContext)
        {
            List<CamposDimConceptos> listaCamposDimConcep = new List<CamposDimConceptos>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listaCamposDimConcep = (from a in getSession().Set<CamposDimConceptos>()
                                           where a.campoDIM_ID == campoDim &&
                                           a.concepnomDefi_ID == Concepto
                                           select a).ToList();
                mensajeResultado.resultado = listaCamposDimConcep;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("filtradoCampoDimConceptos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje filtradoConceptos(int Concepto, DBContextAdapter dbContext)
        {
            List<CamposDimConceptos> listaConceptosfilt = new List<CamposDimConceptos>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listaConceptosfilt = (from a in getSession().Set<CamposDimConceptos>()
                                           where a.concepnomDefi_ID == Concepto
                                           select a).ToList();
                mensajeResultado.resultado = listaConceptosfilt;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("filtradoConceptos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getCampoDimConceptoAll(DBContextAdapter dbContext)
        {
            List<CamposDimConceptos> listaCamposDimConceptos = new List<CamposDimConceptos>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listaCamposDimConceptos = (from a in getSession().Set<CamposDimConceptos>() select a).ToList();
                mensajeResultado.resultado = listaCamposDimConceptos;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("CampoDimConceptoAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje guardaryEliminar(List<CamposDimConceptos> ListaGuardar, List<CamposDimConceptos> ListaEliminar, DBContextAdapter dbContext)
        {
            CamposDimConceptos camposDimConceptos = new CamposDimConceptos();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                int i = 0,j=0;
                for(i=0; i < ListaGuardar.Count; i++)
                {
                    getSession().Set<CamposDimConceptos>().AddOrUpdate(ListaGuardar[i]);
                    getSession().SaveChanges();
                    mensajeResultado.resultado = camposDimConceptos;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                }
                for(j=0;j < ListaEliminar.Count; j++)
                {
                    getSession().Set<CamposDimConceptos>().AddOrUpdate(ListaEliminar[i]);
                    getSession().SaveChanges();
                }
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("AsignaTipoReporteAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
    }
}