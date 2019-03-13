using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Reflection;

namespace TC4I
{
    enum Socket_Status { Init, Connecting, Normal, Connect_Failed, Closed };
    [Serializable]
    enum Socket_Data_Type : int { Heartbeat,Camera_Data };
    [Serializable]
    struct Socket_Data
    {
        public Socket_Data_Type Data_Type;
        public int Index;
        public string strInfo;
        public int iTest;
        public byte[] Photo;
        public byte[] Photo2;
    }

    class TC4I_Socket
    {
        public bool serializeObjToStr(Object obj, out string serializedStr)
        {
            bool serializeOk = false;
            serializedStr = "";
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, obj);
                serializedStr = System.Convert.ToBase64String(memoryStream.ToArray());

                serializeOk = true;
            }
            catch
            {
                serializeOk = false;
            }

            return serializeOk;
        }

        public bool deserializeStrToObj(string serializedStr, out object deserializedObj)
        {
            bool deserializeOk = false;
            deserializedObj = null;

            try
            {
                byte[] restoredBytes = System.Convert.FromBase64String(serializedStr);
                MemoryStream restoredMemoryStream = new MemoryStream(restoredBytes);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                deserializedObj = binaryFormatter.Deserialize(restoredMemoryStream);

                deserializeOk = true;
            }
            catch
            {
                deserializeOk = false;
            }

            return deserializeOk;
        }

        public static bool serializeObjToByte(Object obj, out byte[] serializedByte)
        {
            bool serializeOk = false;
            serializedByte = null;
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, obj);
                serializedByte = memoryStream.ToArray();

                serializeOk = true;
            }
            catch (Exception e)
            {
                TC4I_Common.PrintLog(0, string.Format("error: serializeObjToByte exception, {0}",e.Message));
                serializeOk = false;
            }

            return serializeOk;
        }

        public static bool deserializeByteToObj(Byte[] serializedByte, out object deserializedObj)
        {
            bool deserializeOk = false;
            deserializedObj = null;

            try
            {
                MemoryStream restoredMemoryStream = new MemoryStream(serializedByte);
                //BinaryFormatter binaryFormatter = new BinaryFormatter();
                IFormatter formatter = new BinaryFormatter();
                formatter.Binder = new UBinder();

                //deserializedObj = binaryFormatter.Deserialize(restoredMemoryStream);
                deserializedObj = formatter.Deserialize(restoredMemoryStream);
                deserializeOk = true;
            }
            catch
            {
                deserializeOk = false;
            }

            return deserializeOk;
        }

        public class UBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                Assembly ass = Assembly.GetExecutingAssembly();
                return ass.GetType(typeName);
            }
        }
    }
}
