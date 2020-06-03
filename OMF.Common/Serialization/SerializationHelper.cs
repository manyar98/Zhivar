using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;

namespace OMF.Common.Serialization
{
    public static class SerializationHelper
    {
        public static byte[] Serialize<T>(T obj)
        {
            MemoryStream memoryStream = new MemoryStream();
            new BinaryFormatter().Serialize((Stream)memoryStream, (object)obj);
            byte[] array = memoryStream.ToArray();
            memoryStream.Close();
            return array;
        }

        public static T Deserialize<T>(byte[] buffer)
        {
            MemoryStream memoryStream = new MemoryStream(buffer);
            T obj = (T)new BinaryFormatter().Deserialize((Stream)memoryStream);
            memoryStream.Close();
            return obj;
        }

        public static byte[] SerializeJson<T>(T obj, IEnumerable<Type> knownTypes = null)
        {
            MemoryStream memoryStream = new MemoryStream();
            new DataContractJsonSerializer(typeof(T), knownTypes).WriteObject((Stream)memoryStream, (object)obj);
            byte[] array = memoryStream.ToArray();
            memoryStream.Close();
            return array;
        }

        public static T DeserializeJson<T>(byte[] buffer, IEnumerable<Type> knownTypes = null)
        {
            MemoryStream memoryStream = new MemoryStream(buffer);
            T obj = (T)new DataContractJsonSerializer(typeof(T), knownTypes).ReadObject((Stream)memoryStream);
            memoryStream.Close();
            return obj;
        }

        public static byte[] SerializeDataContact<T>(T obj, IEnumerable<Type> knownTypes = null)
        {
            MemoryStream memoryStream = new MemoryStream();
            new DataContractSerializer(typeof(T), knownTypes).WriteObject((Stream)memoryStream, (object)obj);
            byte[] array = memoryStream.ToArray();
            memoryStream.Close();
            return array;
        }

        public static T DeserializeDataContact<T>(byte[] buffer, IEnumerable<Type> knownTypes = null)
        {
            MemoryStream memoryStream = new MemoryStream(buffer);
            T obj = (T)new DataContractSerializer(typeof(T), knownTypes).ReadObject((Stream)memoryStream);
            memoryStream.Close();
            return obj;
        }

        public static byte[] SerializeCustom(object obj)
        {
            return new CustomSerializer().Serialize(obj);
        }

        public static List<Tuple<string, string>> DeserializeCustom(byte[] buffer)
        {
            return new CustomSerializer().Deserialize(buffer);
        }
    }
}
