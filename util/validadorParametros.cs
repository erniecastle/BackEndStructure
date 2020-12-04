using Exitosw.Payroll.Core.modelo;
using Exitosw.Payroll.Entity.entidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Exitosw.Payroll.Core.util
{
    public class validadorParametros
    {
        private StringBuilder concatena = new StringBuilder(0);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");

        private static validadorParametros parametrosInstancia = new validadorParametros();
        //private ServicioPaisesIF servicioPaises;
        private Mensaje mensajeDB;
        public String[] parametroMascaraSalarioDiarioIntegradoDefault = new String[] { "######", "##" },
                parametroMascaraSueldoDiarioDefault = new String[] { "######", "##" },
                parametroMascaraSueldosDefault = new String[] { "######", "##" }, parametroMascarasResultadoDefault = new String[] { "######", "##" };
        public bool parametroModalidadTiempoComerDefault = false, parametroActivarTiempoComidaDefault = false,
                parametroActivarChecadaSalidaEntradaPrimerCoffeBreak = false,
                parametroActivarPrimerCoffeBreak = false, parametroActivarSegundoCoffeBreak = false,
                parametroActivarChecadaSalidaEntradaSegundoCoffeBreak = false;
        private DateTime fechaActualSistema;
        ElementosAplicacion elemenRazonSocial = null;

        private validadorParametros()
        {
        }

        public static validadorParametros getInstance()
        {
            return parametrosInstancia;
        }


        public Object valorParametroCruce(Parametros parametrosGlobal, List<Cruce> listCruces, bool regresarValor, RazonesSociales razonesSocialesActual, Usuario usuarioActual/*true=valor,false=byte[]*/)
        {
            Object valorParametros = null;
            try
            {

                if (regresarValor)
                {
                    valorParametros = parametrosGlobal.valor;
                }
                else
                {
                    valorParametros = parametrosGlobal.imagen;
                }
                for (int i = 0; i < listCruces.Count(); i++)
                {
                    if (listCruces[i].elementosAplicacion.entidad == typeof(RazonesSociales).Name)
                    {
                        if (razonesSocialesActual != null)
                        {
                            if (listCruces[i].claveElemento.Equals(razonesSocialesActual.clave))
                            {
                                if (regresarValor)
                                {
                                    valorParametros = listCruces[i].valor;
                                }
                                else
                                {
                                    valorParametros = listCruces[i].imagen;
                                }
                            }
                        }
                    }
                    else if (listCruces[i].elementosAplicacion.entidad == typeof(Usuario).Name)
                    {
                        if (usuarioActual != null)
                        {
                            if (listCruces[i].claveElemento.Equals(usuarioActual.clave))
                            {
                                if (regresarValor)
                                {
                                    valorParametros = listCruces[i].valor;
                                }
                                else
                                {
                                    valorParametros = listCruces[i].imagen;
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("valorParametroCruce()1_Error: ").Append(ex));
            }
            return valorParametros;
        }


        public bool parametroPermiteDepartamento(Parametros parametros, List<Cruce> listCruces, RazonesSociales razonesSocialesActual,Usuario usuarioActual)
        {
            bool manejarDepartamento = false;
            String valorParametros = (String)valorParametroCruce(parametros, listCruces, true,razonesSocialesActual,usuarioActual);
            manejarDepartamento = string.Equals(valorParametros, "1", StringComparison.OrdinalIgnoreCase);
            return manejarDepartamento;
        }

        public bool parametroPermiteTipoCostos(Parametros parametros, List<Cruce> listCruces, RazonesSociales razonesSocialesActual, Usuario usuarioActual)
        {
            bool manejaTipoCentroCostos = false;
            String valorParametros = (String)valorParametroCruce(parametros, listCruces, true,razonesSocialesActual,usuarioActual);
            manejaTipoCentroCostos = string.Equals(valorParametros, "1", StringComparison.OrdinalIgnoreCase);
            return manejaTipoCentroCostos;
        }

        public bool parametroNombreCompletoDetalleRecibo(Parametros parametros, List<Cruce> listCruces, RazonesSociales razonesSocialesActual, Usuario usuarioActual)
        {
            bool parametroNombreCompletoDetalleRecibo;
            String valorParametros = (String)valorParametroCruce(parametros, listCruces, true,razonesSocialesActual,usuarioActual);
            parametroNombreCompletoDetalleRecibo = string.Equals(valorParametros, "1", StringComparison.OrdinalIgnoreCase);
            return parametroNombreCompletoDetalleRecibo;
        }
    }
}
