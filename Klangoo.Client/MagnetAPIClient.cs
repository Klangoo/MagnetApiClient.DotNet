/**************************************************************************************
 * file: MagnetAPICLient.cs
 * 
 * lastmodif: 21 AUG 2018
 * 
 * Minimum Compatibility: .Net Framework 2.0
 * 
 * Copyright 2018, Klangoo Inc.
 *  
 * ************************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using System.Collections;

namespace Klangoo.Client
{
    /// <summary>
    /// Example:
    ///        <code>
    ///        MagnetAPIClient requestHelper = new MagnetAPIClient(ENDPOINT_URI, CALK, SECRET_KEY);
    ///        Dictionary<string, string> request = new Dictionary<string, string>();
    ///        request.Add("text", "Hello World");
    ///        string response = requestHelper.CallWebMethod("ProcessDocument", request, "POST");
    ///        </code>
    /// </summary>
    public class MagnetAPIClient
    {
        private string _endpointUri = null;
        private MagnetSigner _magnetSigner = null;
        private string _calk = null;

        static MagnetAPIClient()
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
        }

        private static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="endpointUri"></param>
        /// <param name="calk">calk (API identifier)</param>
        public MagnetAPIClient(string endpointUri, string calk) :
            this(endpointUri, calk, null)
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="endpointUri"></param>
        /// <param name="calk">calk (API identifier)</param>
        /// <param name="secretKey">calk secret key (optional)</param>
        public MagnetAPIClient(string endpointUri, string calk, string secretKey)
        {
            _calk = calk;
            _endpointUri = endpointUri;

            if (secretKey != null)
                _magnetSigner = new MagnetSigner(_endpointUri, secretKey);
        }

        /// <summary>
        /// Call Web Method (Http request)
        /// sign request if secret key is provided in the constructor
        /// </summary>
        /// <param name="methodName">method name</param>
        /// <param name="request">query string</param>
        /// <param name="requestMethod">GET or POST</param>
        /// <returns>response</returns>
        public string CallWebMethod(string methodName, Params parameters, string requestMethod)
        {
            return CallWebMethod(methodName, parameters.ToDictionary(), requestMethod, true);
        }

        /// <summary>
        /// Call Web Method (Http request)
        /// sign request if secret key is provided in the constructor
        /// </summary>
        /// <param name="methodName">method name</param>
        /// <param name="request">query string</param>
        /// <param name="requestMethod">GET or POST</param>
        /// <returns>response</returns>
        public string CallWebMethod(string methodName, IDictionary<string, string> request, string requestMethod)
        {
            return CallWebMethod(methodName, request, requestMethod, true);
        }

        /// <summary>
        /// Call Web Method (Http request)
        /// </summary>
        /// <param name="methodName">method name</param>
        /// <param name="request">query string</param>
        /// <param name="requestMethod">GET or POST</param>
        /// <param name="signRequest">sign request if secret key is provided in the constructor</param>
        /// <returns></returns>
        private string CallWebMethod(string methodName, IDictionary<string, string> request, string requestMethod, bool signRequest)
        {
            if (!HasCalk(request)) { request.Add("calk", _calk); }

            string queryString;
            if (signRequest && _magnetSigner != null)
            {
                queryString = _magnetSigner.GetSignedQueryString(methodName, request, requestMethod);
            }
            else
            {
                queryString = ConstructQueryString(request);
            }
            return CallWebMethod__(methodName, queryString, requestMethod);
        }

        /// <summary>
        /// Call Web Method (Http request)
        /// sign request if secret key is provided in the constructor
        /// </summary>
        /// <param name="methodName">method name</param>
        /// <param name="request">query string</param>
        /// <param name="requestMethod">GET or POST</param>
        /// <returns>response</returns>
        private string CallWebMethod(string methodName, string queryString, string requestMethod)
        {
            return CallWebMethod(methodName, queryString, requestMethod, true);
        }

        /// <summary>
        /// Call Web Method (Http request)
        /// </summary>
        /// <param name="methodName">method name</param>
        /// <param name="request">query string</param>
        /// <param name="requestMethod">GET or POST</param>
        /// <param name="signRequest">sign request if secret key is provided in the constructor</param>
        /// <returns></returns>
        private string CallWebMethod(string methodName, string queryString, string requestMethod, bool signRequest)
        {
            if (queryString == string.Empty) { queryString = "calk=" + _calk; }
            else if (!HasCalk(queryString)) { queryString = "calk=" + _calk + "&" + queryString; }

            if (signRequest && _magnetSigner != null)
            {
                queryString = _magnetSigner.GetSignedQueryString(methodName, queryString, requestMethod);
            }
            return CallWebMethod__(methodName, queryString, requestMethod);
        }

        private string CallWebMethod__(string methodName, IDictionary<string, string> request, string requestMethod)
        {
            string queryString = ConstructQueryString(request);
            return CallWebMethod__(methodName, queryString, requestMethod);
        }

        private string CallWebMethod__(string methodName, string queryString, string requestMethod)
        {
            if (requestMethod.ToUpper() == "GET")
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_endpointUri + "/" + methodName + "?" + queryString);
                request.ContentType = "application/x-www-form-urlencoded";
                request.Method = "GET";
                WebResponse resp = request.GetResponse();
                Stream respStream = resp.GetResponseStream();
                StreamReader rdr = new StreamReader(respStream, System.Text.Encoding.UTF8);
                string response = rdr.ReadToEnd();
                respStream.Close();
                return response;
            }
            else // POST
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_endpointUri + "/" + methodName);
                request.ContentType = "application/x-www-form-urlencoded";
                //request.Timeout = 30000;
                request.Method = "POST";
                System.IO.Stream stream = request.GetRequestStream();
                byte[] arrBytes = System.Text.Encoding.UTF8.GetBytes(queryString);
                stream.Write(arrBytes, 0, arrBytes.Length);
                stream.Close();
                WebResponse resp = request.GetResponse();
                Stream respStream = resp.GetResponseStream();
                StreamReader rdr = new StreamReader(respStream, System.Text.Encoding.UTF8);
                string response = rdr.ReadToEnd();
                respStream.Close();
                return response;
            }
        }

        private string ConstructQueryString(IDictionary<string, string> request)
        {
            StringBuilder builder = new StringBuilder();

            if (request.Count == 0)
            {
                builder.Append("");
                return builder.ToString();
            }

            foreach (KeyValuePair<string, string> kvp in request)
            {
                builder.Append(this.PercentEncodeRfc3986(kvp.Key));
                builder.Append("=");
                builder.Append(this.PercentEncodeRfc3986((kvp.Value != null) ? kvp.Value : ""));
                builder.Append("&");
            }
            string queryString = builder.ToString();
            queryString = queryString.Substring(0, queryString.Length - 1);
            return queryString;
        }

        private string PercentEncodeRfc3986(string str)
        {
            str = UrlEncode(str, System.Text.Encoding.UTF8);
            str = str.Replace("'", "%27").Replace("(", "%28").Replace(")", "%29").Replace("*", "%2A").Replace("!", "%21").Replace("%7e", "~").Replace("+", "%20");

            StringBuilder sbuilder = new StringBuilder(str);
            for (int i = 0; i < sbuilder.Length; i++)
            {
                if (sbuilder[i] == '%')
                {
                    if (Char.IsLetter(sbuilder[i + 1]) || Char.IsLetter(sbuilder[i + 2]))
                    {
                        sbuilder[i + 1] = Char.ToUpper(sbuilder[i + 1]);
                        sbuilder[i + 2] = Char.ToUpper(sbuilder[i + 2]);
                    }
                }
            }
            return sbuilder.ToString();
        }

        private bool HasCalk(IDictionary<string, string> request)
        {
            foreach (KeyValuePair<string, string> param in request)
            {
                if (param.Key.ToLower() == "calk")
                    return true;
            }
            return false;
        }

        private bool HasCalk(string queryString)
        {
            string queryStringLower = queryString.ToLower();
            if (queryStringLower.StartsWith("calk=") || queryStringLower.Contains("?calk=") || queryStringLower.Contains("&calk="))
            {
                return true;
            }
            return false;
        }

        #region Url Encode & Decode

        #region Url Encode

        /// <summary>
        /// Source: https://github.com/mono/mono/blob/master/mcs/class/System.Web/System.Web/HttpUtility.cs
        /// </summary>
        private static string UrlEncode(string str, Encoding e)
        {
            if (str == null)
                return null;

            if (str == String.Empty)
                return String.Empty;

            bool needEncode = false;
            int len = str.Length;
            for (int i = 0; i < len; i++)
            {
                char c = str[i];
                if ((c < '0') || (c < 'A' && c > '9') || (c > 'Z' && c < 'a') || (c > 'z'))
                {
                    if (NotEncoded(c))
                        continue;

                    needEncode = true;
                    break;
                }
            }

            if (!needEncode)
                return str;

            // avoided GetByteCount call
            byte[] bytes = new byte[e.GetMaxByteCount(str.Length)];
            int realLen = e.GetBytes(str, 0, str.Length, bytes, 0);
            return Encoding.ASCII.GetString(UrlEncodeToBytes(bytes, 0, realLen));
        }

        private static bool NotEncoded(char c)
        {
            return (c == '!' || c == '(' || c == ')' || c == '*' || c == '-' || c == '.' || c == '_'
            );
        }

        private static byte[] UrlEncodeToBytes(byte[] bytes, int offset, int count)
        {
            if (bytes == null)
                return null;

            int blen = bytes.Length;
            if (blen == 0)
                return new byte[0];

            if (offset < 0 || offset >= blen)
                throw new ArgumentOutOfRangeException("offset");

            if (count < 0 || count > blen - offset)
                throw new ArgumentOutOfRangeException("count");

            MemoryStream result = new MemoryStream(count);
            int end = offset + count;
            for (int i = offset; i < end; i++)
                UrlEncodeChar((char)bytes[i], result, false);

            return result.ToArray();
        }

        private static char[] hexChars = "0123456789abcdef".ToCharArray();

        private static void UrlEncodeChar(char c, Stream result, bool isUnicode)
        {
            if (c > 255)
            {
                //FIXME: what happens when there is an internal error?
                //if (!isUnicode)
                //	throw new ArgumentOutOfRangeException ("c", c, "c must be less than 256");
                int idx;
                int i = (int)c;

                result.WriteByte((byte)'%');
                result.WriteByte((byte)'u');
                idx = i >> 12;
                result.WriteByte((byte)hexChars[idx]);
                idx = (i >> 8) & 0x0F;
                result.WriteByte((byte)hexChars[idx]);
                idx = (i >> 4) & 0x0F;
                result.WriteByte((byte)hexChars[idx]);
                idx = i & 0x0F;
                result.WriteByte((byte)hexChars[idx]);
                return;
            }

            if (c > ' ' && NotEncoded(c))
            {
                result.WriteByte((byte)c);
                return;
            }
            if (c == ' ')
            {
                result.WriteByte((byte)'+');
                return;
            }
            if ((c < '0') ||
                (c < 'A' && c > '9') ||
                (c > 'Z' && c < 'a') ||
                (c > 'z'))
            {
                if (isUnicode && c > 127)
                {
                    result.WriteByte((byte)'%');
                    result.WriteByte((byte)'u');
                    result.WriteByte((byte)'0');
                    result.WriteByte((byte)'0');
                }
                else
                    result.WriteByte((byte)'%');

                int idx = ((int)c) >> 4;
                result.WriteByte((byte)hexChars[idx]);
                idx = ((int)c) & 0x0F;
                result.WriteByte((byte)hexChars[idx]);
            }
            else
                result.WriteByte((byte)c);
        }

        #endregion

        #region Url Decode

        /// <summary>
        /// Source: https://github.com/mono/mono/blob/master/mcs/class/System.Web/System.Web/HttpUtility.cs
        /// </summary>
        private static string UrlDecode(string str, Encoding e)
        {
            if (null == str)
                return null;

            if (str.IndexOf('%') == -1 && str.IndexOf('+') == -1)
                return str;

            if (e == null)
                e = Encoding.UTF8;

            long len = str.Length;
            List<byte> bytes = new List<byte>();
            int xchar;
            char ch;

            for (int i = 0; i < len; i++)
            {
                ch = str[i];
                if (ch == '%' && i + 2 < len && str[i + 1] != '%')
                {
                    if (str[i + 1] == 'u' && i + 5 < len)
                    {
                        // unicode hex sequence
                        xchar = GetChar(str, i + 2, 4);
                        if (xchar != -1)
                        {
                            WriteCharBytes(bytes, (char)xchar, e);
                            i += 5;
                        }
                        else
                            WriteCharBytes(bytes, '%', e);
                    }
                    else if ((xchar = GetChar(str, i + 1, 2)) != -1)
                    {
                        WriteCharBytes(bytes, (char)xchar, e);
                        i += 2;
                    }
                    else
                    {
                        WriteCharBytes(bytes, '%', e);
                    }
                    continue;
                }

                if (ch == '+')
                    WriteCharBytes(bytes, ' ', e);
                else
                    WriteCharBytes(bytes, ch, e);
            }

            byte[] buf = bytes.ToArray();
            bytes = null;
            return e.GetString(buf);
        }

        private static void WriteCharBytes(IList buf, char ch, Encoding e)
        {
            if (ch > 255)
            {
                foreach (byte b in e.GetBytes(new char[] { ch }))
                    buf.Add(b);
            }
            else
                buf.Add((byte)ch);
        }

        private static int GetChar(string str, int offset, int length)
        {
            int val = 0;
            int end = length + offset;
            for (int i = offset; i < end; i++)
            {
                char c = str[i];
                if (c > 127)
                    return -1;

                int current = GetInt((byte)c);
                if (current == -1)
                    return -1;
                val = (val << 4) + current;
            }

            return val;
        }

        private static int GetInt(byte b)
        {
            char c = (char)b;
            if (c >= '0' && c <= '9')
                return c - '0';

            if (c >= 'a' && c <= 'f')
                return c - 'a' + 10;

            if (c >= 'A' && c <= 'F')
                return c - 'A' + 10;

            return -1;
        }

        #endregion

        #endregion

        class MagnetSigner
        {
            private string _endpointUri;
            private byte[] _secretKey;
            private HMAC _hmac;

            public MagnetSigner(string endpointUri, string secretKey)
            {
                _endpointUri = endpointUri;
                _secretKey = Encoding.UTF8.GetBytes(secretKey);
                _hmac = new HMACSHA256(_secretKey);
            }

            private string GetSignatureUsingCanonicalQueryString(string methodName, string canonicalQueryString, string requestMethod)
            {
                // Derive the bytes needs to be signed.
                StringBuilder builder = new StringBuilder();
                builder.Append(requestMethod.ToLower())
                    .Append("\n")
                    .Append(_endpointUri.ToLower())
                    .Append("\n")
                    .Append(methodName.ToLower())
                    .Append("\n")
                    .Append(canonicalQueryString);

                string stringToSign = builder.ToString();
                byte[] toSign = Encoding.UTF8.GetBytes(stringToSign);

                // Compute the signature and convert to Base64.
                byte[] sigBytes = _hmac.ComputeHash(toSign);
                string signature = Convert.ToBase64String(sigBytes);

                return this.PercentEncodeRfc3986(signature);
            }

            public string GetSignedQueryString(string methodName, string queryString, string requestMethod)
            {
                IDictionary<string, string> request = this.CreateDictionary(queryString);
                return GetSignedQueryString(methodName, request, requestMethod);
            }

            public string GetSignedQueryString(string methodName, IDictionary<string, string> request, string requestMethod)
            {
                // Use a SortedDictionary to get the parameters in natural byte order
                ParamComparer pc = new ParamComparer();
                SortedDictionary<string, string> sortedMap = new SortedDictionary<string, string>(request, pc);

                // Add Timestamp to the requests.
                sortedMap["timestamp"] = this.GetTimestamp();

                // Get the canonical query string
                string canonicalQueryString = this.ConstructCanonicalQueryString(sortedMap);

                string signature = GetSignatureUsingCanonicalQueryString(methodName, canonicalQueryString, requestMethod);

                // now construct the complete URL and return to caller.
                StringBuilder qsBuilder = new StringBuilder();
                qsBuilder.Append(canonicalQueryString).Append("&signature=").Append(signature);

                return qsBuilder.ToString();
            }

            ///
            /// Current time in IS0 8601 format
            ///
            private string GetTimestamp()
            {
                DateTime currentTime = DateTime.UtcNow;
                string timestamp = currentTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
                return timestamp;
            }

            private string PercentEncodeRfc3986(string str)
            {
                str = UrlEncode(str, System.Text.Encoding.UTF8);
                str = str.Replace("'", "%27").Replace("(", "%28").Replace(")", "%29").Replace("*", "%2A").Replace("!", "%21").Replace("%7e", "~").Replace("+", "%20");

                StringBuilder sbuilder = new StringBuilder(str);
                for (int i = 0; i < sbuilder.Length; i++)
                {
                    if (sbuilder[i] == '%')
                    {
                        if (Char.IsLetter(sbuilder[i + 1]) || Char.IsLetter(sbuilder[i + 2]))
                        {
                            sbuilder[i + 1] = Char.ToUpper(sbuilder[i + 1]);
                            sbuilder[i + 2] = Char.ToUpper(sbuilder[i + 2]);
                        }
                    }
                }
                return sbuilder.ToString();
            }

            ///
            /// Convert a query string to corresponding dictionary of name-value pairs.
            ///
            private IDictionary<string, string> CreateDictionary(string queryString)
            {
                Dictionary<string, string> map = new Dictionary<string, string>();

                string[] requestParams = queryString.Split('&');

                for (int i = 0; i < requestParams.Length; i++)
                {
                    if (requestParams[i].Length < 1)
                    {
                        continue;
                    }

                    char[] sep = { '=' };
                    string[] param = requestParams[i].Split(sep, 2);
                    for (int j = 0; j < param.Length; j++)
                    {
                        param[j] = UrlDecode(param[j], System.Text.Encoding.UTF8);
                    }
                    switch (param.Length)
                    {
                        case 1:
                            {
                                if (requestParams[i].Length >= 1)
                                {
                                    if (requestParams[i].ToCharArray()[0] == '=')
                                    {
                                        map[""] = param[0];
                                    }
                                    else
                                    {
                                        map[param[0]] = "";
                                    }
                                }
                                break;
                            }
                        case 2:
                            {
                                if (!string.IsNullOrEmpty(param[0]))
                                {
                                    map[param[0]] = param[1];
                                }
                            }
                            break;
                    }
                }

                return map;
            }

            ///
            /// Construct the canonical query string from the sorted parameter map.
            ///
            private string ConstructCanonicalQueryString(SortedDictionary<string, string> sortedParamMap)
            {
                StringBuilder builder = new StringBuilder();

                if (sortedParamMap.Count == 0)
                {
                    builder.Append("");
                    return builder.ToString();
                }

                foreach (KeyValuePair<string, string> kvp in sortedParamMap)
                {
                    builder.Append(this.PercentEncodeRfc3986(kvp.Key));
                    builder.Append("=");
                    builder.Append(this.PercentEncodeRfc3986((kvp.Value != null) ? kvp.Value : ""));
                    builder.Append("&");
                }
                string canonicalString = builder.ToString();
                canonicalString = canonicalString.Substring(0, canonicalString.Length - 1);
                return canonicalString;
            }

            class ParamComparer : IComparer<string>
            {
                public int Compare(string p1, string p2)
                {
                    return string.CompareOrdinal(p1, p2);
                }
            }
        }

        public class Params : IEnumerable
        {
            private Dictionary<string, object> _data = new Dictionary<string, object>();

            public Params()
            {
            }

            public Params(Dictionary<string, object> dictionary)
            {
                _data = dictionary;
            }

            public IEnumerator GetEnumerator()
            {
                return _data.GetEnumerator();
            }

            public virtual Params Add(string name, object value)
            {
                Params jsonDoc = new Params(_data);
                jsonDoc._data.Add(name, value);
                return jsonDoc;
            }

            public Dictionary<string, string> ToDictionary()
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                foreach (KeyValuePair<string, object> kvp in this)
                {
                    dictionary.Add(kvp.Key, kvp.Value.ToString());
                }
                return dictionary;
            }
        }
    }
}