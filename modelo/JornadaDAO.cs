/**
* @author: Daniel Ruelas 
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Clase JornadaDAO para llamados a metodos de Entity
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
using Exitosw.Payroll.Entity.util;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public class JornadaDAO : GenericRepository<Jornada>, JornadaDAOIF
    {

        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        public Mensaje getAllJornada(DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var listJornadas = (from j in getSession().Set<Jornada>()
                                    select new
                                    {
                                        j.id,
                                        j.clave,
                                        j.descripcion,
                                    }).ToList();
                mensajeResultado.resultado = listJornadas;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getJornadaAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
    }
}