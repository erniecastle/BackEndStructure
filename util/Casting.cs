using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Entity.util;
using System;
using System.Collections.Generic;

namespace Exitosw.Payroll.Core.util
{
    public class Casting
    {
        private int vInt;
        private long vLong;
        private double vDouble;
        private bool vBoolean;
        private char vCharacter;
        private byte vByte;
        private short vShort;
        private float vFloat;
        private string vString;
        private DateTime vDate;
        private List<object> vList;
        private ModoBaja modoBaja;
        private TipoBaja tipoBaja;

        public int getInt()
        {
            return vInt;
        }
        public void setInt(string value)
        {
            vInt = Convert.ToInt32(value);
        }

        public long getLong()
        {
            return vLong;
        }

        public void setLong(string value)
        {
            vLong = long.Parse(value);

        }

        public double getDouble()
        {
            return vDouble;
        }

        public void setDouble(string value)
        {
            vDouble = Convert.ToDouble(value);
        }

        public bool getBoolean()
        {
            return vBoolean;
        }

        public void setBoolean(string value)
        {
            vBoolean = Convert.ToBoolean(value);
        }

        public char getCharacter()
        {
            return vCharacter;
        }

        public void setCharacter(string value)
        {
            vCharacter = value[0];
        }

        public byte getByte()
        {
            return vByte;
        }

        public void setByte(string value)
        {
            vByte = Convert.ToByte(value);
        }

        public short getShort()
        {
            return vShort;
        }

        public void setShort(string value)
        {
            vShort = short.Parse(value);
        }

        public float getFloat()
        {
            return vFloat;
        }

        public void setFloat(string value)
        {
            vFloat = float.Parse(value);
        }

        public string getString()
        {
            return vString;
        }

        public void setString(string value)
        {
            vString = value;
        }

        public DateTime getDate()
        {
            return vDate;
        }

        public void setDate(String value)
        {
            vDate = DateTime.Parse(value);
        }

        public List<object> getList()
        {
            return vList;
        }

        public void setList(string value)
        {
            if (value.Equals("null") | value.Equals("[]"))
            {
                vList = new List<object>() { };
            }
        }

        public ModoBaja getModoBaja()
        {
            return modoBaja;
        }

        public void setModoBaja(String modoBaja)
        {
            //this.modoBaja = ModoBaja.getEnumString(modoBaja);
            this.modoBaja =(ModoBaja) ManejadorEnum.GetValue(modoBaja,typeof(ModoBaja));
        }

        public TipoBaja getTipoBaja()
        {
            return tipoBaja;
        }
        public void setTipoBaja(String modoBaja)
        {
            this.tipoBaja = (TipoBaja)ManejadorEnum.GetValue(modoBaja, typeof(TipoBaja));
        }
    }
}