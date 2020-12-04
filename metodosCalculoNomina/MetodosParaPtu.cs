using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Core.util;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Exitosw.Payroll.Entity.util;

namespace Exitosw.Payroll.Core.metodosCalculoNomina
{
    public class MetodosParaPtu
    {
        private Mensaje mensajeResultado = new Mensaje();
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        private PtuDatosGenerales ptuDatosGenerales = null;
        public PtuEmpleados ptuEmpleado = null;
        public bool isCalculoPTU = false;

        public Mensaje buscaCalculoPTU(string claveRazonsocial, DateTime fechaInicio, DateTime fechaFin, DBContextSimple dbContextSimple)
        {
            inicializaVariableMensaje();
            isCalculoPTU = false;
            try
            {
                ptuDatosGenerales = (from ptu in dbContextSimple.Set<PtuDatosGenerales>()
                                     where (fechaInicio >= ptu.fechaCalculo && fechaFin <= ptu.fechaCalculo) && ptu.razonesSociales.clave.Equals(claveRazonsocial)
                                     select ptu).SingleOrDefault<PtuDatosGenerales>();
                if (ptuDatosGenerales != null)
                {
                    DateTime fechaDeCalculo = ptuDatosGenerales.fechaCalculo.GetValueOrDefault(),
                            fechaPeriodoInicial = fechaInicio,
                            fechaPeriodoFinal = fechaFin;
                    if ((DateTime.Compare(fechaDeCalculo, fechaPeriodoInicial) > 0
                            || DateTime.Compare(fechaDeCalculo, fechaPeriodoInicial) == 0)
                            && (DateTime.Compare(fechaDeCalculo, fechaPeriodoFinal) < 0
                            || DateTime.Compare(fechaDeCalculo, fechaPeriodoFinal) == 0))
                    {
                        isCalculoPTU = true;
                    }
                }
                mensajeResultado.noError = 0;
                mensajeResultado.resultado = ptuDatosGenerales;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscaCalculoPTU()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                dbContextSimple.Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje buscaEmpleadoPTU(string claveRazonSocial, decimal idEmpleado, DBContextSimple dbContextSimple)
        {
            try
            {
                ptuEmpleado = (from ptuEm in dbContextSimple.Set<PtuEmpleados>()
                               where ptuEm.razonesSociales.clave == claveRazonSocial && ptuEm.empleados.id == idEmpleado
                               select ptuEm).SingleOrDefault();
                mensajeResultado.noError = 0;
                mensajeResultado.resultado = ptuEmpleado;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscaEmpleadoPTU()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                dbContextSimple.Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje buscarValoresPTU(String campo, string claveEmpleado, string claveRazonSocial, int ejercicio, DBContextSimple dbContextSimple)
        {
            inicializaVariableMensaje();
            Object resultado = null;
            try
            {
                StringBuilder query = new StringBuilder(0);
                if (string.Equals(campo, "PTUDIAS", StringComparison.OrdinalIgnoreCase))
                {
                    resultado = (from ptu in dbContextSimple.Set<PtuEmpleados>()
                                 where ptu.empleados.clave == claveEmpleado && ptu.razonesSociales.clave == claveRazonSocial && ptu.ejercicio == ejercicio
                                 select ptu.ptuDias).SingleOrDefault();
                }
                else if (string.Equals(campo, "PTUPERCEPCIONES", StringComparison.OrdinalIgnoreCase))
                {
                    resultado = (from ptu in dbContextSimple.Set<PtuEmpleados>()
                                 where ptu.empleados.clave == claveEmpleado && ptu.razonesSociales.clave == claveRazonSocial && ptu.ejercicio == ejercicio
                                 select ptu.ptuPercepciones).SingleOrDefault();
                }
                else if (string.Equals(campo, "PTUTOTAL", StringComparison.OrdinalIgnoreCase))
                {
                    resultado = (from ptu in dbContextSimple.Set<PtuEmpleados>()
                                 where ptu.empleados.clave == claveEmpleado && ptu.razonesSociales.clave == claveRazonSocial && ptu.ejercicio == ejercicio
                                 select ptu.ptuDias + ptu.ptuPercepciones).SingleOrDefault();
                }
                if (resultado == null)
                {
                    mensajeResultado.resultado = 0;
                }
                else
                {
                    mensajeResultado.resultado = resultado;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscarValoresPTU()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return mensajeResultado;
        }

        private void inicializaVariableMensaje()
        {
            if (mensajeResultado == null)
            {
                mensajeResultado = new Mensaje();

            }
            mensajeResultado.resultado = null;
            mensajeResultado.noError = 0;
            mensajeResultado.error = "";
        }
    }
}