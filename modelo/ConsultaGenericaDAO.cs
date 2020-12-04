/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Clase ConsultaGenericaDAOIF para llamados a metodos de Entity
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
using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Core.genericos.campos;
using System.Text;
using Exitosw.Payroll.Core.servicios.extras;
using System.Reflection;
using System.Data.Entity;
using Exitosw.Payroll.Core.campos;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public class ConsultaGenericaDAO : GenericRepository<Object>, ConsultaGenericaDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");

        private ConectorQuerysGenericos conectorQuerysGenericos = new ConectorQuerysGenericos();

        public Mensaje consultaAllConOrdenado(string tabla, CamposOrden camposOrden, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {

            int tipoConexion = eligeConexionBDPorTabla(tabla);
            DBContextAdapter conexionUsada = null;
            if (tipoConexion == 1)
            {
                conexionUsada = dbContext;
            }
            else if (tipoConexion == 2)
            {
                conexionUsada = dbContextMaestra;
            }

            List<CamposSelect> camposSelect = new List<CamposSelect>() { new CamposSelect(tabla, TipoFuncion.NINGUNO) };
            OperadorSelect select = new OperadorSelect(camposSelect);
            List<CamposOrden> listCamposOrden = new List<CamposOrden>();
            if (camposOrden != null)
            {
                listCamposOrden.Add(camposOrden);
            }
            select.todosDatos = true;
            mensajeResultado = conectorQuerysGenericos.consultaGenerica(conexionUsada, TipoResultado.Lista, TipoOperacion.SELECT,
                tabla, select, null, null, null, listCamposOrden, null);
            return mensajeResultado;
        }

        public Mensaje consultaAllConOrdenado(string tabla, List<CamposWhere> camposWhere, CamposOrden camposOrden, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {

            int tipoConexion = eligeConexionBDPorTabla(tabla);
            DBContextAdapter conexionUsada = null;
            if (tipoConexion == 1)
            {
                conexionUsada = dbContext;
            }
            else if (tipoConexion == 2)
            {
                conexionUsada = dbContextMaestra;
            }

            List<CamposSelect> camposSelect = new List<CamposSelect>() { new CamposSelect(tabla, TipoFuncion.NINGUNO) };
            OperadorSelect select = new OperadorSelect(camposSelect);
            List<CamposOrden> listCamposOrden = new List<CamposOrden>();
            if (camposOrden != null)
            {
                listCamposOrden.Add(camposOrden);
            }
            select.todosDatos = true;
            mensajeResultado = conectorQuerysGenericos.consultaGenerica(conexionUsada, TipoResultado.Lista, TipoOperacion.SELECT,
                tabla, select, null, camposWhere, null, listCamposOrden, null);
            return mensajeResultado;
        }

        public Mensaje consultaPorFiltrosGenerico(string tabla, List<CamposWhere> camposWhere, ValoresRango valoresRango, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            int tipoConexion = eligeConexionBDPorTabla(tabla);
            DBContextAdapter conexionUsada = null;
            if (tipoConexion == 1)
            {
                conexionUsada = dbContext;
            }
            else if (tipoConexion == 2)
            {
                conexionUsada = dbContextMaestra;
            }

            List<CamposSelect> camposSelect = new List<CamposSelect>() { new CamposSelect(tabla, TipoFuncion.NINGUNO) };
            OperadorSelect select = new OperadorSelect(camposSelect);
            select.todosDatos = true;
            mensajeResultado = conectorQuerysGenericos.consultaGenerica(conexionUsada, TipoResultado.Lista, TipoOperacion.SELECT,
                tabla, select, null, camposWhere, null, null, valoresRango);
            return mensajeResultado;
        }

        public Mensaje consultaPorRangoConFiltroYOrdenado(string tabla, ValoresRango valoresRango, List<CamposWhere> camposWhere, CamposOrden camposOrden, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            int tipoConexion = eligeConexionBDPorTabla(tabla);
            DBContextAdapter conexionUsada = null;
            if (tipoConexion == 1)
            {
                conexionUsada = dbContext;
            }
            else if (tipoConexion == 2)
            {
                conexionUsada = dbContextMaestra;
            }

            List<CamposSelect> camposSelect = new List<CamposSelect>() { new CamposSelect(tabla, TipoFuncion.NINGUNO) };
            OperadorSelect select = new OperadorSelect(camposSelect);
            List<CamposOrden> listCamposOrden = new List<CamposOrden>();
            if (camposOrden != null)
            {
                listCamposOrden.Add(camposOrden);
            }
            select.todosDatos = true;
            mensajeResultado = conectorQuerysGenericos.consultaGenerica(conexionUsada, TipoResultado.Lista, TipoOperacion.SELECT,
                tabla, select, null, camposWhere, null, listCamposOrden, valoresRango);
            return mensajeResultado;
        }

        public Mensaje consultaPorRangos(string tabla, ValoresRango valoresRango, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            int tipoConexion = eligeConexionBDPorTabla(tabla);
            DBContextAdapter conexionUsada = null;
            if (tipoConexion == 1)
            {
                conexionUsada = dbContext;
            }
            else if (tipoConexion == 2)
            {
                conexionUsada = dbContextMaestra;
            }

            List<CamposSelect> camposSelect = new List<CamposSelect>() { new CamposSelect(tabla, TipoFuncion.NINGUNO) };
            OperadorSelect select = new OperadorSelect(camposSelect);

            ///select.todosDatos = true;
            mensajeResultado = conectorQuerysGenericos.consultaGenerica(conexionUsada, TipoResultado.Lista, TipoOperacion.SELECT,
                tabla, select, null, null, null, null, valoresRango);
            return mensajeResultado;
        }

        public Mensaje consultaPorRangosConOrdenado(string tabla, ValoresRango valoresRango, CamposOrden camposOrden, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            int tipoConexion = eligeConexionBDPorTabla(tabla);
            DBContextAdapter conexionUsada = null;
            if (tipoConexion == 1)
            {
                conexionUsada = dbContext;
            }
            else if (tipoConexion == 2)
            {
                conexionUsada = dbContextMaestra;
            }
            List<CamposSelect> camposSelect = new List<CamposSelect>() { new CamposSelect(tabla, TipoFuncion.NINGUNO) };
            OperadorSelect select = new OperadorSelect(camposSelect);
            List<CamposOrden> listCamposOrden = new List<CamposOrden>();
            if (camposOrden != null)
            {
                listCamposOrden.Add(camposOrden);
            }
            select.todosDatos = true;
            mensajeResultado = conectorQuerysGenericos.consultaGenerica(conexionUsada, TipoResultado.Lista, TipoOperacion.SELECT,
                tabla, select, null, null, null, listCamposOrden, valoresRango);
            return mensajeResultado;
        }

        public Mensaje consultaPorRangosFiltro(string tabla, ValoresRango valoresRango, List<CamposWhere> camposWhere, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            int tipoConexion = eligeConexionBDPorTabla(tabla);
            DBContextAdapter conexionUsada = null;
            if (tipoConexion == 1)
            {
                conexionUsada = dbContext;
            }
            else if (tipoConexion == 2)
            {
                conexionUsada = dbContextMaestra;
            }

            List<CamposSelect> camposSelect = new List<CamposSelect>() { new CamposSelect(tabla, TipoFuncion.NINGUNO) };
            OperadorSelect select = new OperadorSelect(camposSelect);
            select.todosDatos = true;
            mensajeResultado = conectorQuerysGenericos.consultaGenerica(conexionUsada, TipoResultado.Lista, TipoOperacion.SELECT,
                tabla, select, null, camposWhere, null, null, valoresRango);
            return mensajeResultado;
        }

        public Mensaje consultaPorRangosFiltros(string tabla, ValoresRango valoresRango, List<CamposWhere> camposWhere, CamposSelect campoSelect, CamposOrden camposOrden, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            int tipoConexion = eligeConexionBDPorTabla(tabla);
            DBContextAdapter conexionUsada = null;
            if (tipoConexion == 1)
            {
                conexionUsada = dbContext;
            }
            else if (tipoConexion == 2)
            {
                conexionUsada = dbContextMaestra;
            }

            List<CamposSelect> camposSelect;
            if (campoSelect == null)
            {
                camposSelect = new List<CamposSelect>() { new CamposSelect(tabla, TipoFuncion.NINGUNO) };
            }
            else {
                camposSelect = new List<CamposSelect>() { campoSelect };
            }

            OperadorSelect select = new OperadorSelect(camposSelect);
            List<CamposOrden> listCamposOrden = new List<CamposOrden>();
            if (camposOrden != null)
            {
                listCamposOrden.Add(camposOrden);
            }
            select.todosDatos = true;
            mensajeResultado = conectorQuerysGenericos.consultaGenerica(conexionUsada, TipoResultado.Lista, TipoOperacion.SELECT,
                tabla, select, null, camposWhere, null, listCamposOrden, valoresRango);
            return mensajeResultado;
        }

        public Mensaje existeClaveGenerico(string tabla, List<CamposWhere> camposWhere, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            int tipoConexion = eligeConexionBDPorTabla(tabla);
            DBContextAdapter conexionUsada = null;
            if (tipoConexion == 1)
            {
                conexionUsada = dbContext;
            }
            else if (tipoConexion == 2)
            {
                conexionUsada = dbContextMaestra;
            }

            OperadorSelect select = new OperadorSelect();

            select.todosDatos = true;
            mensajeResultado = conectorQuerysGenericos.consultaGenerica(conexionUsada, TipoResultado.Unico, TipoOperacion.SELECT,
                tabla, select, null, camposWhere, null, null, null);
            return mensajeResultado;
        }

        public Mensaje existeDatoGenerico(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            int tipoConexion = eligeConexionBDPorTabla(tabla);
            DBContextAdapter conexionUsada = null;
            if (tipoConexion == 1)
            {
                conexionUsada = dbContext;
            }
            else if (tipoConexion == 2)
            {
                conexionUsada = dbContextMaestra;
            }

            List<CamposSelect> camposSelect = new List<CamposSelect>() { new CamposSelect(tabla, TipoFuncion.NINGUNO) };
            OperadorSelect select = new OperadorSelect(camposSelect);
            List<CamposWhere> listCamposWhere = new List<CamposWhere>();
            if (campoWhere != null)
            {
                listCamposWhere.Add(campoWhere);
            }
            select.todosDatos = true;
            mensajeResultado = conectorQuerysGenericos.consultaGenerica(conexionUsada, TipoResultado.Lista, TipoOperacion.SELECT,
                tabla, select, null, listCamposWhere, null, null, null);
            return mensajeResultado;
        }

        public Mensaje existeDatoList(string[] tabla, List<CamposWhere> camposWhere, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            object[] listDatos = new object[tabla.Length];
            object[] datos = null;
            for (int i = 0; i < tabla.Length; i++)
            {
                datos = new object[2];
                int tipoConexion = eligeConexionBDPorTabla(tabla[i]);
                DBContextAdapter conexionUsada = null;
                if (tipoConexion == 1)
                {
                    conexionUsada = dbContext;
                }
                else if (tipoConexion == 2)
                {
                    conexionUsada = dbContextMaestra;
                }

                datos[0] = tabla[i];
                List<CamposSelect> camposSelect = new List<CamposSelect>() { new CamposSelect(tabla[i], TipoFuncion.CONTAR) };
                OperadorSelect select = new OperadorSelect(camposSelect);
                List<CamposWhere> listCamposWhere = new List<CamposWhere>();
                listCamposWhere.Add(camposWhere[i]);
                select.todosDatos = true;
                mensajeResultado = conectorQuerysGenericos.consultaGenerica(conexionUsada, TipoResultado.Lista, TipoOperacion.SELECT,
                    tabla[i], select, null, listCamposWhere, null, null, null);
                if (mensajeResultado.noError == 0)
                {
                    if ((int)mensajeResultado.resultado > 0)
                    {
                        datos[0] = true;
                    }
                    else
                    {
                        datos[0] = false;
                    }

                }
                else
                {
                    break;
                }
            }

            return mensajeResultado;
        }

        public Mensaje existenClavesConOrden(string tabla, List<CamposWhere> camposWhere, CamposOrden campoOrden, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            int tipoConexion = eligeConexionBDPorTabla(tabla);
            DBContextAdapter conexionUsada = null;
            if (tipoConexion == 1)
            {
                conexionUsada = dbContext;
            }
            else if (tipoConexion == 2)
            {
                conexionUsada = dbContextMaestra;
            }

            OperadorSelect select = new OperadorSelect();
            List<CamposOrden> listCamposOrden = new List<CamposOrden>();
            if (campoOrden != null)
            {
                listCamposOrden.Add(campoOrden);
            }
            select.todosDatos = true;
            mensajeResultado = conectorQuerysGenericos.consultaGenerica(conexionUsada, TipoResultado.Lista, TipoOperacion.SELECT,
                tabla, select, null, camposWhere, null, listCamposOrden, null);
            return mensajeResultado;
        }

        public Mensaje existenClavesGenerico(string tabla, List<CamposWhere> camposWhere, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            int tipoConexion = eligeConexionBDPorTabla(tabla);
            DBContextAdapter conexionUsada = null;
            if (tipoConexion == 1)
            {
                conexionUsada = dbContext;
            }
            else if (tipoConexion == 2)
            {
                conexionUsada = dbContextMaestra;
            }

            List<CamposSelect> camposSelect = new List<CamposSelect>() { new CamposSelect(tabla, TipoFuncion.NINGUNO) };
            OperadorSelect select = new OperadorSelect();
            mensajeResultado = conectorQuerysGenericos.consultaGenerica(conexionUsada, TipoResultado.Unico, TipoOperacion.SELECT,
                tabla, select, null, camposWhere, null, null, null);
            return mensajeResultado;
        }

        public Mensaje existeValoresEntidad(string tabla, List<CamposWhere> camposWhere, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            int tipoConexion = eligeConexionBDPorTabla(tabla);
            DBContextAdapter conexionUsada = null;
            if (tipoConexion == 1)
            {
                conexionUsada = dbContext;
            }
            else if (tipoConexion == 2)
            {
                conexionUsada = dbContextMaestra;
            }

            OperadorSelect select = new OperadorSelect();
            select.todosDatos = true;
            select.tipoFuncion = TipoFuncion.CONTAR;
            mensajeResultado = conectorQuerysGenericos.consultaGenerica(conexionUsada, TipoResultado.Unico, TipoOperacion.SELECT,
                tabla, select, null, camposWhere, null, null, null);
            if (mensajeResultado.noError > 0)
            {
                mensajeResultado.resultado = (int)mensajeResultado.resultado > 0 ? true : false;
            }
            return mensajeResultado;
        }

        public Mensaje getDataAll(string tabla, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            int tipoConexion = eligeConexionBDPorTabla(tabla);
            DBContextAdapter conexionUsada = null;
            if (tipoConexion == 1)
            {
                conexionUsada = dbContext;
            }
            else if (tipoConexion == 2)
            {
                conexionUsada = dbContextMaestra;
            }

            List<CamposFrom> camposFrom = new List<CamposFrom>() { new CamposFrom(tabla) };
            OperadorSelect select = new OperadorSelect();
            select.todosDatos = true;

            mensajeResultado = conectorQuerysGenericos.consultaGenerica(conexionUsada, TipoResultado.Lista, TipoOperacion.SELECT,
                tabla, select, camposFrom, null, null, null, null);
            return mensajeResultado;
        }

        public Mensaje getDataAllFiltro(string tabla, List<CamposWhere> camposWhere, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            int tipoConexion = eligeConexionBDPorTabla(tabla);
            DBContextAdapter conexionUsada = null;
            if (tipoConexion == 1)
            {
                conexionUsada = dbContext;
            }
            else if (tipoConexion == 2)
            {
                conexionUsada = dbContextMaestra;
            }

            OperadorSelect select = new OperadorSelect();

            select.todosDatos = true;
            mensajeResultado = conectorQuerysGenericos.consultaGenerica(conexionUsada, TipoResultado.Lista, TipoOperacion.SELECT,
                tabla, select, null, camposWhere, null, null, null);
            return mensajeResultado;
        }

        public DateTime getFechaSistema()
        {
            DateTime now = DateTime.Now;
            return now;
        }

        public Mensaje getObject(string tabla, CamposSelect campoSelect, List<CamposWhere> camposWhere, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {

            int tipoConexion = eligeConexionBDPorTabla(tabla);
            DBContextAdapter conexionUsada = null;
            if (tipoConexion == 1)
            {
                conexionUsada = dbContext;
            }
            else if (tipoConexion == 2)
            {
                conexionUsada = dbContextMaestra;
            }

            List<CamposSelect> camposSelect = new List<CamposSelect>() { campoSelect };
            OperadorSelect select = new OperadorSelect(camposSelect);

            mensajeResultado = conectorQuerysGenericos.consultaGenerica(conexionUsada, TipoResultado.Unico, TipoOperacion.SELECT,
                tabla, select, null, camposWhere, null, null, null);
            return mensajeResultado;
        }

        public Mensaje obtenerClaveNumericaMax(string tabla, string campoEvaluar, List<CamposWhere> camposWhere, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            int tipoConexion = eligeConexionBDPorTabla(tabla);
            DBContextAdapter conexionUsada = null;
            if (tipoConexion == 1)
            {
                conexionUsada = dbContext;
            }
            else if (tipoConexion == 2)
            {
                conexionUsada = dbContextMaestra;
            }

            List<CamposSelect> camposSelect = new List<CamposSelect>() { new CamposSelect(campoEvaluar, TipoFuncion.MAXIMO) };
            OperadorSelect select = new OperadorSelect(camposSelect);

            mensajeResultado = conectorQuerysGenericos.consultaGenerica(conexionUsada, TipoResultado.Unico, TipoOperacion.SELECT,
                tabla, select, null, camposWhere, null, null, null);
            return mensajeResultado;
        }

        public Mensaje obtenerClaveStringMax(string tabla, string campoEvaluar, List<CamposWhere> camposWhere, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            int tipoConexion = eligeConexionBDPorTabla(tabla);
            DBContextAdapter conexionUsada = null;
            if (tipoConexion == 1)
            {
                conexionUsada = dbContext;
            }
            else if (tipoConexion == 2)
            {
                conexionUsada = dbContextMaestra;
            }

            List<CamposSelect> camposSelect = new List<CamposSelect>() { new CamposSelect(campoEvaluar, TipoFuncion.MAXIMO) };
            OperadorSelect select = new OperadorSelect(camposSelect);

            mensajeResultado = conectorQuerysGenericos.consultaGenerica(conexionUsada, TipoResultado.Unico, TipoOperacion.SELECT,
                tabla, select, null, camposWhere, null, null, null);
            return mensajeResultado;
        }

        public Mensaje selectExisteClave(string tabla, CamposSelect campoSelect, List<CamposWhere> camposWhere, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            int tipoConexion = eligeConexionBDPorTabla(tabla);
            DBContextAdapter conexionUsada = null;
            if (tipoConexion == 1)
            {
                conexionUsada = dbContext;
            }
            else if (tipoConexion == 2)
            {
                conexionUsada = dbContextMaestra;
            }

            List<CamposSelect> camposSelect = new List<CamposSelect>() { campoSelect };
            OperadorSelect select = new OperadorSelect(camposSelect);

            mensajeResultado = conectorQuerysGenericos.consultaGenerica(conexionUsada, TipoResultado.Unico, TipoOperacion.SELECT,
                tabla, select, null, camposWhere, null, null, null);
            return mensajeResultado;
        }

        private int eligeConexionBDPorTabla(string tabla)
        {
            int dbToUse = -1;
            if (existeTablaDBContextMaster(tabla))
            {
                //conexionUsada master;
                dbToUse = 1;
            }
            else
            {
                //conexionUsada simple
                dbToUse = 2;
            }
            return dbToUse;
        }
    }
}