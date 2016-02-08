/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
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
        public static byte[] BinarySerialize<T>(T objectToSerialize)
        {
            using (var ms = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(ms, objectToSerialize);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Return deserialized object from serializedBytes serialized by Serializtion.BinarySerialize()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializedBytes">buffer serialized by Serializtion.BinarySerialize()</param>
        /// <returns>T</returns>
        public static T BinaryDeserialize<T>(byte[] serializedBytes)
        {
            using (var ms = new MemoryStream(serializedBytes))
            {
                var binaryFormatter = new BinaryFormatter();
                return (T) binaryFormatter.Deserialize(ms);
            }
        }
    }
}
