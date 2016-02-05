using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web;

namespace NetMQ.Zyre
{
    /// <summary>
    /// Class for "simple" serialization/deserialization of some common types we need to communicate within Zyre
    /// </summary>
    public static class Serialization
    {
        /// <summary>
        /// Serialize into serializedBytes that can be deserialized by other members of this class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToSerialize"></param>
        /// <returns></returns>
        public static byte[] Serialize<T>(T objectToSerialize)
        {
            using (var ms = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(ms, objectToSerialize);
                return ms.ToArray();
            }
        }

        private static object Deserialize(byte[] serializedBytes)
        {
            using (var ms = new MemoryStream(serializedBytes))
            {
                var binaryFormatter = new BinaryFormatter();
                return  binaryFormatter.Deserialize(ms);
            }
        }

        /// <summary>
        /// Return deserialized object from serializedBytes serialized by Serializtion.Serialize()
        /// </summary>
        /// <param name="serializedBytes">buffer serialized by Serializtion.Serialize()</param>
        /// <returns>deserialized List of string</returns>
        public static List<string> DeserializeListString(byte[] serializedBytes)
        {
            return (List<string>) Deserialize(serializedBytes);
        }

        /// <summary>
        /// Return deserialized object from serializedBytes serialized by Serializtion.Serialize()
        /// </summary>
        /// <param name="serializedBytes">buffer serialized by Serializtion.Serialize()</param>
        /// <returns></returns>
        public static Dictionary<string, string> DeserializeDictStringString(byte[] serializedBytes)
        {
            return (Dictionary<string, string>)Deserialize(serializedBytes);
        }

    }
}
