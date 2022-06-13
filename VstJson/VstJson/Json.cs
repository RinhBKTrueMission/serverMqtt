using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using JC = Newtonsoft.Json.JsonConvert;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace System
{
    public class Json
    {
        public static T Convert<T>(object value)
        {
            var json = JObject.FromObject(value);
            return json.ToObject<T>();
            //return GetObject<T>(GetString(value));
        }
        public static string GetString(object value)
        {
            return JC.SerializeObject(value);
        }
        public static object GetObject(string text)
        {
            return JC.DeserializeObject(text);
        }
        public static T GetObject<T>(string text)
        {
            return JC.DeserializeObject<T>(text);
        }
        public static void Save(string fileName, object value)
        {
            using (var sw = new System.IO.StreamWriter(fileName))
            {
                sw.Write(JC.SerializeObject(value));
            }
        }
        public static T Read<T>(string fileName)
        {
            using (var sr = new System.IO.StreamReader(fileName))
            {
                return JC.DeserializeObject<T>(sr.ReadToEnd());
            }
        }
    }
}

namespace System
{
    public static class JObjectExtension
    {
        public static JObject Append(this JObject o, string key, object value)
        {
            o.Add(key, JToken.FromObject(value));
            return o;
        }
        public static object Get(this JObject o, string path)
        {
            return o.SelectToken(path)?.ToObject<object>();
        }
        public static T Get<T>(this JObject o, string path)
        {
            var token = o.SelectToken(path);
            if (token == null) { return default(T); }
            return token.ToObject<T>();
        }
        public static T CreateObject<T>(this JObject o, object objectId)
        {
            var token = o.SelectToken("Id");
            if (token == null)
            {
                return o.Append("Id", objectId).ToObject<T>();
            }

            token.Replace(JToken.FromObject(objectId));
            return o.ToObject<T>();
        }    

        public static JObject SetObject(this JObject o, string path, object value)
        {
            var token = o.SelectToken(path);
            token.Replace(JToken.FromObject(value));

            return o;
        }
        public static JObject Set<T>(this JObject o, string path, string value)
        {
            var token = o.SelectToken(path);
            token.Replace(JToken.Parse(value));

            return o;
        }
        public static JObject UpdateObject(this JObject o, string path, object value)
        {
            var token = o.SelectToken(path);
            var v = JToken.FromObject(value);
            if (token == null) { o.Add(path, v); }
            else { token.Replace(v); }

            return o;
        }
    }

}

namespace System
{
    public static class StringExtensions
    {
        public static byte[] ToBytes(this string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }
        public static string ToMD5(this string input)
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
        public static string GetSecurity(this string input, string postfix)
        {
            return ToMD5(string.Format("{0}{1}", input.ToLower(), postfix));
        }
    }
}

namespace System
{
    public static class DocumentExtensions
    {
        public static byte[] ToBytes(this object obj)
        {
            return Json.GetString(obj).ToBytes();
        }
        public static T ToObject<T>(this byte[] bytes)
        {
            return ToObject<T>(bytes, 0, bytes.Length);
        }
        public static T ToObject<T>(this byte[] bytes, int offset, int length)
        {
            var s = Encoding.UTF8.GetString(bytes, offset, length);
            return Json.GetObject<T>(s);
        }
    }

}
