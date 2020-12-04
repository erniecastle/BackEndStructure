using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Exitosw.Payroll.Core.util
{
    public class UtileriasSat
    {
        public static String VERSION_XML_DEFAULT = "1.0";
        public static String ENCODING_XML_DEFAULT = "UTF-8";
        public static String XML_HEADER = "<?xml version=\"" + VERSION_XML_DEFAULT + "\" encoding=\"" + ENCODING_XML_DEFAULT + "\"?>";  //"<?xml version=\"".Concat(VERSION_XML_DEFAULT).concat("\" encoding=\"").concat(ENCODING_XML_DEFAULT).concat("\"?>");
        public static String FORMATO_FECHA_HORA_SAT = "yyyy-MM-dd'T'HH:mm:ss";


        public static decimal castNumerosToBigDecimal(Object valor)
        {
            Type t = valor.GetType();
            if (valor.GetType() == typeof(float?) | valor.GetType() == typeof(Double?) | valor.GetType() == typeof(double?))
            {
                //if (valor.GetType() == typeof(float?) | valor.GetType() == typeof(Double?) | valor.GetType() == typeof(double?)) {
                String[] numeros = valor.ToString().Split('.');
                if (numeros.Length > 1)
                {
                    if (numeros[1].Length > 6)
                    {
                        numeros[1] = numeros[1].Substring(0, 6);
                    }
                    valor = numeros[0] + "." + numeros[1];
                }


                // }
                return Convert.ToDecimal(valor);
            }
            else if (valor.GetType() == typeof(float) | valor.GetType() == typeof(Double) | valor.GetType() == typeof(double))
            {
                // if (valor.GetType() == typeof(float) | valor.GetType() == typeof(Double) | valor.GetType() == typeof(double))
                //{
                String[] numeros = valor.ToString().Split('.');
                if (numeros.Length > 1)
                {
                    if (numeros[1].Length > 6)
                    {
                        numeros[1] = numeros[1].Substring(0, 6);
                    }
                    valor = numeros[0] + "." + numeros[1];
                }
                else {
                    valor = numeros[0] + ".00" ;
                }


                // }
                return Convert.ToDecimal(valor);
            }
            else if (valor.GetType() == typeof(int) | valor.GetType() == typeof(Int16) | valor.GetType() == typeof(Int32) | valor.GetType() == typeof(Int64) |
                valor.GetType() == typeof(int?) | valor.GetType() == typeof(Int16?) | valor.GetType() == typeof(Int32?) | valor.GetType() == typeof(Int64?))
            {
                return Convert.ToDecimal(valor);
            }

            return Convert.ToDecimal("-100");
        }

        public static BigInteger? castNumerosToBigInteger(Object valor)
        {
          
            if (valor == null)
            {
                return null;
            }
            if (valor.GetType().IsPrimitive)
            {
                return BigInteger.Parse(valor.ToString());
            }
            else if (valor.GetType() == typeof(string) | valor.GetType() == typeof(String))
            {
                if (!valor.ToString().Replace("0", "").Any())
                {
                    return null;
                }
                else
                {
                    return BigInteger.Parse(valor.ToString());
                }
            }
            else {
                return BigInteger.Parse(valor.ToString());

            }
           
        }

        public static decimal castNumerosToImporteMX(Object valor)
        {
            if (valor.GetType() == typeof(float) || valor.GetType() == typeof(float?)
                || valor.GetType() == typeof(double) || valor.GetType() == typeof(double?)
                || valor.GetType() == typeof(Double) || valor.GetType() == typeof(Double?))
            {
                valor = redondearDecimales((double)valor, 2);
            }
            else {

                valor = 0.00;
            }

            return Convert.ToDecimal(valor);
        }

        private static double redondearDecimales(double valorInicial, int numeroDecimales)
        {
            double parteEntera, resultado;
            resultado = valorInicial;
            parteEntera = Math.Floor(resultado);
            resultado = (resultado - parteEntera) * Math.Pow(10, numeroDecimales);
            resultado = Math.Round(resultado);
            resultado = (resultado / Math.Pow(10, numeroDecimales)) + parteEntera;
            return resultado;
        }

        public static string castXmlFechaFormatoIso8601(DateTime fecha)
        {
            return castFechatoXmlFechaFormato(fecha, "yyyy-MM-dd");
        }
        public static string castFechatoXmlFechaFormato(DateTime fecha, String formato)
        {
            string fec = "";
            try
            {
                fec = fecha.ToString(formato);

            }
            catch (Exception ex)
            {
                //bitacoraStatic(ex.getMessage());
                //fec = null;
            }
            return fec;
        }
        /*antiguedad formato mes dia*/
        public static String getAntiguedadYMD(DateTime fechaInicio, DateTime fechaActual)
        {
            DateTime ini = DateTime.Now;
            ini = fechaInicio; ;
            DateTime fin = DateTime.Now;
            fin = fechaActual;
            int diaInicio = ini.Day;
            int mesInicio = ini.Month + 1;
            int anioInicio = ini.Year;
            //int diaInicio = ini.get(Calendar.DAY_OF_MONTH);
            //int mesInicio = ini.get(Calendar.MONTH) + 1;
            //int anioInicio = ini.get(Calendar.YEAR);

            int diaActual = fin.Day;
            int mesActual = fin.Month + 1;
            int anioActual = fin.Year;

            //int diaActual = fin.get(Calendar.DAY_OF_MONTH);
            //int mesActual = fin.get(Calendar.MONTH) + 1;
            //int anioActual = fin.get(Calendar.YEAR);

            int b = 0, dias = 0, mes = 0, anios = 0, meses = 0;
            mes = mesInicio - 1;
            if (mes == 2)
            {
                if ((anioActual % 4 == 0) && ((anioActual % 100 != 0) || (anioActual % 400 == 0)))
                {
                    b = 29;
                }
                else
                {
                    b = 28;
                }
            }
            else if (mes <= 7)
            {
                if (mes == 0)
                {
                    b = 31;
                }
                else if (mes % 2 == 0)
                {
                    b = 30;
                }
                else
                {
                    b = 31;
                }
            }
            else if (mes > 7)
            {
                if (mes % 2 == 0)
                {
                    b = 31;
                }
                else
                {
                    b = 30;
                }
            }
            if ((anioInicio > anioActual) || (anioInicio == anioActual && mesInicio > mesActual)
                    || (anioInicio == anioActual && mesInicio == mesActual && diaInicio > diaActual))
            {
                return "La fecha de inicio debe ser anterior a la fecha Actual";
            }
            else if (mesInicio <= mesActual)
            {
                anios = anioActual - anioInicio;
                if (diaInicio <= diaActual)
                {
                    meses = mesActual - mesInicio;
                    dias = (diaActual - diaInicio);
                }
                else
                {
                    if (mesActual == mesInicio)
                    {
                        anios = anios - 1;
                    }
                    meses = (mesActual - mesInicio - 1 + 12) % 12;
                    dias = b - (diaInicio - diaActual);
                }
            }
            else
            {
                anios = anioActual - anioInicio - 1;
                if (diaInicio > diaActual)
                {
                    meses = mesActual - mesInicio + 12;
                    dias = b - (diaInicio - diaActual);
                }
                else
                {
                    meses = mesActual - mesInicio + 12;
                    dias = diaActual - diaInicio;
                }
            }

            StringBuilder antiguedad = new StringBuilder("P");
            bool year = false;
            if (anios > 0)
            {
                antiguedad.Append(anios.ToString()).Append("Y");
                year = true;
            }
            if (meses > 0 | year)
            {
                antiguedad.Append(meses.ToString()).Append("M");
            }
            antiguedad.Append(dias.ToString()).Append("D");
            return antiguedad.ToString();
        }

        public static int getAniosServicio(DateTime fechaInicio, DateTime fechaActual)
        {
            DateTime ini = DateTime.Now;
            ini = fechaInicio;
            DateTime fin = DateTime.Now;
            fin = fechaActual;

            int diaInicio = ini.Day;
            int mesInicio = ini.Month + 1;
            int anioInicio = ini.Year;

            int diaActual = fin.Day;
            int mesActual = fin.Month + 1;
            int anioActual = fin.Year;

            int b = 0, mes = 0, anios = 0, meses = 0;
            mes = mesInicio - 1;

            if (mes == 2)
            {
                if ((anioActual % 4 == 0) && ((anioActual % 100 != 0) || (anioActual % 400 == 0)))
                {
                    b = 29;
                }
                else
                {
                    b = 28;
                }
            }
            else if (mes <= 7)
            {
                if (mes == 0)
                {
                    b = 31;
                }
                else if (mes % 2 == 0)
                {
                    b = 30;
                }
                else
                {
                    b = 31;
                }
            }
            else if (mes > 7)
            {
                if (mes % 2 == 0)
                {
                    b = 31;
                }
                else
                {
                    b = 30;
                }
            }
            if ((anioInicio > anioActual) || (anioInicio == anioActual && mesInicio > mesActual)
                    || (anioInicio == anioActual && mesInicio == mesActual && diaInicio > diaActual))
            {
                return 0;
            }
            else if (mesInicio <= mesActual)
            {
                anios = anioActual - anioInicio;
                if (diaInicio <= diaActual)
                {
                    meses = mesActual - mesInicio;
                }
                else
                {
                    if (mesActual == mesInicio)
                    {
                        anios = anios - 1;
                    }
                    meses = (mesActual - mesInicio - 1 + 12) % 12;
                }
            }
            else
            {
                anios = anioActual - anioInicio - 1;
                if (diaInicio > diaActual)
                {
                    meses = mesActual - mesInicio + 12;
                }
                else
                {
                    meses = mesActual - mesInicio + 12;
                }
            }
            if (meses >= 6)
            {
                anios = anios + 1;
            }

            return anios;
        }
    }
}
