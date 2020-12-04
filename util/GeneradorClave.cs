using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exitosw.Payroll.Core.util
{
    public class GeneradorClave
    {
        private int index = -1;
        private char incrementaCaracter(char c, int pos)
        {
            if (c == '9')
            {
                index = pos - 1;
                return c = '0';
            }
            else if (c == 'Z' || c == 'z')
            {
                index = pos - 1;
                if (c == 'Z')
                {
                    return c = 'A';
                }
                else
                {
                    return c = 'a';
                }
            }
            else if ( !Char.IsDigit(c) & !isLetra(c))
            {
                index = pos - 1;
                return c;
            }
            else
            {
                return (char)((int)c + 1);
            }
        }

        private bool isLetra(char c)
        {
            if ((c >= 'a' & c <= 'z') || (c >= 'A' & c <= 'Z'))
            {
                return true;
            }
            return false;
        }

        private char[] recorre(char[] v, int pos)
        {
            v[pos] = incrementaCaracter(v[pos], pos);
            if (index < pos & index > -1)
            {
                v = recorre(v, index);
                if (index > -1)
                {
                    index = v.Length - 1;
                }
            }
            return v;
        }

        //    public String generaClaveMax(String claveMax) {
        //        char[] car = claveMax.toCharArray();
        //        int pos = car.length - 1;
        //        index = pos;
        //        car = recorre(car, pos);
        //        if (index == -1) {
        //        }
        //        return String.valueOf(car);
        //    }
        public String generaClaveMax(String claveMax, String mascara)
        {
            char[] car = claveMax.ToCharArray();
            int pos = car.Length - 1;
            index = pos;
            if (pos > -1)
            {
                car = recorre(car, pos);
            }
            if (index == -1)
            {
                if (mascara.Trim().Length == 0)
                {
                    return addValorClave(car, '1');//JSA01
                }
                else
                {
                    String simbolo = mascara.Substring(car.Length, car.Length + 1);
                    pos = car.Length + 1;
                    simbolo = mascara.Substring(mascara.Length - pos, mascara.Length - (pos - 1));
                    if (simbolo.Equals("#"))
                    {
                        return addValorClave(car, '1');
                    }
                    else if (simbolo.Equals("A"))
                    {
                        return addValorClave(car, 'A');
                    }
                }
            }
            return new string(car);
        }

        //añade nuevo caracter a la clave cuando llega a su valor final
        private String addValorClave(char[] clave, char simbolo)
        {
            char[] newClave = new char[clave.Length + 1];
            Array.Copy(clave, 0, newClave, 1, clave.Length);
            newClave[0] = simbolo;
            return new string(newClave);
        }

        public GeneradorClave()
        {
        }

        public GeneradorClave(String cadena)
        {
            char[] v = cadena.ToCharArray();
            int cant = 1, i;
            for (i = 0; i < v.Length; i++)
            {
                if (Char.IsDigit(v[i]))
                {
                    cant = cant * 10;
                }
                else if (isLetra(v[i]))
                {
                    cant = cant * 26;
                }
            }
            int pos = v.Length - 1;
            index = pos;
            for (i = 0; i < cant; i++)
            {
              //  System.out.println(String.valueOf(v));
                v = recorre(v, pos);
            }
        }

        public static void main(String[] args)
        {
            new GeneradorClave("0-0");
        }
    }
}
