using System;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Web.Script.Serialization;



namespace Exitosw.Payroll.Core.util
{
    /// <summary>
    /// Helper class to serialize/deserialize any [Serializable] object in a centralized way.
    /// </summary>
    public static class SerializationHelper
    {
        /// <summary>
        /// Internal utility method to convert an UTF8 Byte Array to an UTF8 string.
        /// </summary>
        /// <param name="characters"></param>
        /// <returns></returns>
        public static string UTF8ByteArrayToString(byte[] characters)
        {
            //return System.Text.Encoding.UTF8.GetString(characters);
            // return Encoding.GetEncoding(1252).GetString(characters);
            /* System.Text.ASCIIEncoding codificador = new System.Text.ASCIIEncoding();
             return codificador.GetString(characters);*/
            /*Encoding enc = Encoding.GetEncoding(1252);
            return enc.GetString(characters);*/
            //return System.Text.Encoding.Default.GetString(characters);
            return new UTF8Encoding().GetString(characters);
        }

        /// <summary>
        /// Internal utility method to convert an UTF8 string to an UTF8 Byte Array.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static byte[] StringToUTF8ByteArray(string xml)
        {
            return new UTF8Encoding().GetBytes(xml);
        }

        /// <summary>
        /// Serialize a [Serializable] object of type T into an XML/UTF8 string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string SerializeObject<T>(this T item)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (XmlTextWriter xml = new XmlTextWriter(stream, new UTF8Encoding(false)))
                    {
                        XmlSerializer xs = new XmlSerializer(typeof(T));
                        xs.Serialize(xml, item);
                        return UTF8ByteArrayToString(((MemoryStream)xml.BaseStream).ToArray());
                    }
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// De-serialize an XML/UTF8 string into an object of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string xml)
        {
            using (MemoryStream stream = new MemoryStream(StringToUTF8ByteArray(xml)))
            using (new XmlTextWriter(stream, new UTF8Encoding(false)))
            {
                return (T)new XmlSerializer(typeof(T)).Deserialize(stream);
            }
        }

        /// <summary>
        /// Serialize an object T using BinaryFormatter.
        /// </summary>
        // <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string SerializeObjectUsingBinaryFormatter<T>(this T item)
        {
            BinaryFormatter serializer = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.Serialize(ms, item);
                return UTF8ByteArrayToString(ms.ToArray());
            }
        }

        /// <summary>
        /// De-serialize a BinaryFormatter-serialized string into an object of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T DeserializeObjectUsingBinaryFormatter<T>(string str)
        {
            BinaryFormatter serializer = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(StringToUTF8ByteArray(str)))
            {
                return (T)serializer.Deserialize(ms);
            }
        }
        /// <summary>
        /// Serialize a [Serializable] object of type T into an XML-formatted string using XmlSerializer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">any T object</param>
        /// <returns>an XML-formatted string</returns>
        public static string SerializeToXML<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            try
            {
                var xmlserializer = new XmlSerializer(typeof(T));
                var stringWriter = new StringWriter();
                using (var writer = XmlWriter.Create(stringWriter))
                {
                    xmlserializer.Serialize(writer, value);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }

        /// <summary>
        /// De-serialize a [Serializable] object of type T into an XML-formatted string using XmlSerializer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">any T object</param>
        /// <returns>an XML-formatted string</returns>
        public static T DeserializeFromXML<T>(string xml)
        {
            if (String.IsNullOrEmpty(xml)) throw new NotSupportedException("ERROR: input string cannot be null.");
            try
            {
                var xmlserializer = new XmlSerializer(typeof(T));
                var stringReader = new StringReader(xml);
                using (var reader = XmlReader.Create(stringReader))
                {
                    return (T)xmlserializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }

        /// <summary>
        /// Serialize a [Serializable] object into a JSON-formatted string using System.Web.Script.Serialization.JavaScriptSerializer
        /// NOTE: you need to add a reference to System.Web.Extensions to have access to System.Web.Script.Serialization namespace.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">any T object</param>
        /// <returns>a JSON-formatted string</returns>
        public static string SerializeToJson<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            try
            {
                return new JavaScriptSerializer().Serialize(value);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }

        /// <summary>
        /// De-serialize a [Serializable] object into a JSON-formatted string using System.Web.Script.Serialization.JavaScriptSerializer
        /// NOTE: you need to add a reference to System.Web.Extensions to have access to System.Web.Script.Serialization namespace.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">any T object</param>
        /// <returns>a JSON-formatted string</returns>
        public static T DeserializeFromJson<T>(string json)
        {
            if (String.IsNullOrEmpty(json)) throw new NotSupportedException("ERROR: input string cannot be null.");
            try
            {
                return new JavaScriptSerializer().Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }
    }
}
