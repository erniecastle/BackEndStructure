/**
* @author:Daniel ruelas  
* Fecha de Creación: 03-07-2018 
* Compañía: Exito Software 
* Descripción del programa: clase Utilerias
* -----------------------------------------------------------------------------
* MODIFICACIONES:
* -----------------------------------------------------------------------------
*/

using System;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Exitosw.Payroll.Core.util
{
    public class Utilerias
    {

        public static Object castStringTo(String tipoDato, String valor)
        {

            Type cls;
            MethodInfo method;
            Casting casting = new Casting();
            object o = null;
            tipoDato = tipoDato.Substring(tipoDato.LastIndexOf('.') + 1);
            cls = casting.GetType();
            try
            {
                method = cls.GetMethod(String.Concat("set", tipoDato));
                object instancia = Activator.CreateInstance(cls);
                method.Invoke(instancia, new object[] { valor });
                method = cls.GetMethod(String.Concat("get", tipoDato));
                o = method.Invoke(instancia, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al convertir este " + valor + " a este " + tipoDato + ":" + ex);
            }
            return o;
        }
        public static Type buscarTipoDatoCampo(string pathCampo)
        {
            Type tipoDato = null;
            try
            {
                string[] path = pathCampo.Split('.');
                string nameProject = Assembly.GetCallingAssembly().GetName().Name;
                //HttpContext.Current.ApplicationInstance.GetType().BaseType.Assembly.GetName().Name;
                tipoDato = Type.GetType(nameProject + ".entidad." + (path[0]));
                if (path.Length > 1)
                {
                    PropertyInfo field = tipoDato.GetProperty(path[1]);
                    if (field.PropertyType.Namespace.Equals("System.Collections.Generic"))
                    {
                        tipoDato = field.PropertyType.GenericTypeArguments[0];

                    }
                    else
                    {
                        tipoDato = field.PropertyType;
                    }

                    if (path.Length > 2)
                    {
                        int i;
                        StringBuilder ruta = new StringBuilder(tipoDato.GetType().Name);
                        for (i = 2; i < path.Length; i++)
                        {
                            ruta.Append(".").Append(path[i]);
                        }
                        tipoDato = buscarTipoDatoCampo(ruta.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Concat("buscarTipoDatoCampo()_Error", ex.Message.ToString()));
            }

            return tipoDato;
        }
        public static int diasBimestre(DateTime fechaEvaluar)
        {
            DateTime fecha = DateTime.Now;
            fecha = fechaEvaluar;
            if ((fecha.Month) % 2 == 0)
            {
                fecha.AddMonths(fecha.Month - 2);
            }
            else
            {
                fecha.AddMonths(fecha.Month - 1);
            }
            if (fecha.Month == 1 || fecha.Month == 2)
            {
                int year = fecha.Year;
                if ((year % 4 == 0) && ((year % 100 != 0) || (year % 400 == 0)))
                {
                    return 60;
                }
                else
                {
                    return 59;
                }
            }
            else if (fecha.Month == 3 || fecha.Month == 4)
            {
                return 61;
            }
            else if (fecha.Month == 5 || fecha.Month == 6)
            {
                return 61;
            }
            else if (fecha.Month == 7 || fecha.Month == 8)
            {
                return 62;
            }
            else if (fecha.Month == 9 || fecha.Month == 10)
            {
                return 61;
            }
            else if (fecha.Month == 11 || fecha.Month == 12)
            {
                return 61;
            }
            return -1;
        }
        public static double truncateDecimal(double valor, int decimales)
        {
            double numTruncado = 0.0;
            string[] param = valor.ToString("R").Split('.');
            if (valor > 0)
            {
                if (param[1].Length >= decimales)
                {
                    numTruncado = Convert.ToDouble(param[0] + "." + param[1].Substring(0, decimales));
                }
                else
                {
                    decimales = param[1].Length;
                    numTruncado = Convert.ToDouble(param[0] + "." + param[1].Substring(0, decimales));
                }
            }
            else
            {
                numTruncado = 0;
            }
            return numTruncado;
        }
        public static string createMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
        public static int cantidadSemanasEntreDosFechasStatic(DateTime fechaInicio, DateTime fechaFin)
        {
            int dias = cantidadDiasEntreDosFechasStatic(fechaInicio, fechaFin);
            if (dias > 0)
            {
                return dias / 7;
            }
            return 0;
        }
        private static int cantidadDiasEntreDosFechasStatic(DateTime fechaInicio, DateTime fechaFin)
        {
            TimeSpan tSpan = fechaFin - fechaInicio;
            int dias = tSpan.Days;
            //long fechaInicialMs = fechaInicio.getTime();
            //long fechaFinalMs = fechaFin.getTime();
            //long diferencia = fechaFinalMs - fechaInicialMs;
            //double dias = Math.floor(diferencia / (1000 * 60 * 60 * 24));
            return dias;
        }

        private static String[] UNIDADES = { "", "un ", "dos ", "tres ", "cuatro ", "cinco ", "seis ", "siete ", "ocho ", "nueve " };
        private static String[] DECENAS = {"diez ", "once ", "doce ", "trece ", "catorce ", "quince ", "dieciseis ",
        "diecisiete ", "dieciocho ", "diecinueve", "veinte ", "treinta ", "cuarenta ",
        "cincuenta ", "sesenta ", "setenta ", "ochenta ", "noventa "};
        private static String[] CENTENAS = {"", "ciento ", "doscientos ", "trecientos ", "cuatrocientos ", "quinientos ", "seiscientos ",
        "setecientos ", "ochocientos ", "novecientos "};
        private static Regex r;
        public static String convertirNumerosALetras(String numero, bool mayusculas, string moneda = "pesos")
        {

            String literal = "";
            String parte_decimal;
            //si el numero utiliza (.) en lugar de (,) -> se reemplaza
            numero = numero.Replace(".", ",");

            //si el numero no tiene parte decimal, se le agrega ,00
            if (numero.IndexOf(",") == -1)
            {
                numero = numero + ",00";
            }
            //se valida formato de entrada -> 0,00 y 999 999 999,00
            r = new Regex(@"\d{1,9},\d{1,2}");
            MatchCollection mc = r.Matches(numero);
            if (mc.Count > 0)
            {
                //se divide el numero 0000000,00 -> entero y decimal
                String[] Num = numero.Split(',');

                string MN = " M.N.";
                if (moneda != "pesos")
                    MN = "";

                //de da formato al numero decimal
                parte_decimal = moneda + " " + Num[1] + "/100" + MN;
                //se convierte el numero a literal
                if (int.Parse(Num[0]) == 0)
                {//si el valor es cero
                    literal = "cero ";
                }
                else if (int.Parse(Num[0]) > 999999)
                {//si es millon
                    literal = getMillones(Num[0]);
                }
                else if (int.Parse(Num[0]) > 999)
                {//si es miles
                    literal = getMiles(Num[0]);
                }
                else if (int.Parse(Num[0]) > 99)
                {//si es centena
                    literal = getCentenas(Num[0]);
                }
                else if (int.Parse(Num[0]) > 9)
                {//si es decena
                    literal = getDecenas(Num[0]);
                }
                else
                {//sino unidades -> 9
                    literal = getUnidades(Num[0]);
                }
                //devuelve el resultado en mayusculas o minusculas
                if (mayusculas)
                {
                    return (literal + parte_decimal).ToUpper();
                }
                else
                {
                    return (literal + parte_decimal);
                }
            }
            else
            {//error, no se puede convertir
                return literal = null;
            }
        }

        /* funciones para convertir los numeros a literales */

        private static String getUnidades(String numero)
        {   // 1 - 9
            //si tuviera algun 0 antes se lo quita -> 09 = 9 o 009=9
            String num = numero.Substring(numero.Length - 1);
            return UNIDADES[int.Parse(num)];
        }

        private static String getDecenas(String num)
        {// 99
            int n = int.Parse(num);
            if (n < 10)
            {//para casos como -> 01 - 09
                return getUnidades(num);
            }
            else if (n > 19)
            {//para 20...99
                String u = getUnidades(num);
                if (u.Equals(""))
                { //para 20,30,40,50,60,70,80,90
                    return DECENAS[int.Parse(num.Substring(0, 1)) + 8];
                }
                else
                {
                    return DECENAS[int.Parse(num.Substring(0, 1)) + 8] + "y " + u;
                }
            }
            else
            {//numeros entre 11 y 19
                return DECENAS[n - 10];
            }
        }

        private static String getCentenas(String num)
        {// 999 o 099
            if (int.Parse(num) > 99)
            {//es centena
                if (int.Parse(num) == 100)
                {//caso especial
                    return " cien ";
                }
                else
                {
                    return CENTENAS[int.Parse(num.Substring(0, 1))] + getDecenas(num.Substring(1));
                }
            }
            else
            {//por Ej. 099
                //se quita el 0 antes de convertir a decenas
                return getDecenas(int.Parse(num) + "");
            }
        }

        private static String getMiles(String numero)
        {// 999 999
            //obtiene las centenas
            String c = numero.Substring(numero.Length - 3);
            //obtiene los miles
            String m = numero.Substring(0, numero.Length - 3);
            String n = "";
            //se comprueba que miles tenga valor entero
            if (int.Parse(m) > 0)
            {
                n = getCentenas(m);
                return n + "mil " + getCentenas(c);
            }
            else
            {
                return "" + getCentenas(c);
            }

        }

        private static String getMillones(String numero)
        { //000 000 000
            //se obtiene los miles
            String miles = numero.Substring(numero.Length - 6);
            //se obtiene los millones
            String millon = numero.Substring(0, numero.Length - 6);
            String n = "";
            if (millon.Length > 1)
            {
                n = getCentenas(millon) + "millones ";
            }
            else
            {
                n = getUnidades(millon) + "millon ";
            }
            return n + getMiles(miles);
        }

    }
}