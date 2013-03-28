using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using TranslationAPI_Test.Controllers;
using System.Media;

namespace TranslationAPI.Controllers
{
    public class TranslateController : ApiController
    {
        AdmAuthentication admAuth = new AdmAuthentication
        ("vernaculatetranslate", "LqLdkBdQ+I/DasFZ6EKBRKvlxmaTlBQPcWKV4Srs3eQ=");

        // GET returns access token in case the client needs to make a call to MicrosoftTranslator
        public AdmAccessToken Get()
        {
            return admAuth.GetAccessToken();
        }

        //POST receives the 'untranslated' from client, returns a translated object
        public translated Post([FromBody]translated u)
        {
            AdmAccessToken admToken = admAuth.GetAccessToken();
            string text = u.value;
            string from = "en";
            string to = u.lang;
            string translation;
            GlosbeObject glosbe;

            string uri = "http://api.microsofttranslator.com/v2/Http.svc/Translate?text=" + System.Web.HttpUtility.UrlEncode(text) +
                         "&from=" + from + "&to=" + to;
            string authToken = "Bearer " + admToken.access_token;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.Headers.Add("Authorization", authToken);

            WebResponse response = null;
            try
            {
                response = httpWebRequest.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    System.Runtime.Serialization.DataContractSerializer dcs = new System.Runtime.Serialization.DataContractSerializer(Type.GetType("System.String"));
                    translation = (string)dcs.ReadObject(stream);
                    Console.WriteLine("Translation for source text '{0}' from {1} to {2} is", text, "en", "de");
                    Console.WriteLine(translation);
                }
            }
            catch (InvalidCastException e)
            {
                Console.WriteLine(e.ToString());
                translation = "not found";
            }
            int numOfWords = translation.Split(' ').Length;
            translated t = new translated(translation, to, numOfWords);
            //t.value = translation;
            //t.lang = to;
            string[] words = translation.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                word temp;
                //string textToDetect = "Hola";
                //Keep appId parameter blank as we are sending access token in authorization header.
                //strong contender: http://www.google.com/dictionary/json?callback=dict_api.callbacks.id100&q=die&sl=en&tl=en&client=en
                uri = "http://glosbe.com/gapi/translate?from=" + to + "&dest=eng&format=json&phrase=" + words[i] + "&page=1&pretty=true"+
                      "&apiKey=2b8dgjkaeu5u57ebmfcbjwu5ior97ubq";
                httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
                httpWebRequest.Headers.Add("Authorization", authToken);
                response = null;
                try
                {
                     response = httpWebRequest.GetResponse();
                     using (Stream stream = response.GetResponseStream())
                     {
                         DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(GlosbeObject));
                         //Get deserialized object from JSON stream
                         glosbe = (GlosbeObject)serializer.ReadObject(response.GetResponseStream());    
                     }
                }
                catch(FileNotFoundException e)
                {
                  throw e;
                }
                if (glosbe.tuc == null)
                {
                    temp = new word(words[i], "no definition");
                }
                else
                {
                    temp = new word(words[i], glosbe.from);
                }

                t.words[i] = temp;
            }
            //temp for language names//

            ///////////////////////////
            return t;
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }

    public class translated
    {
        public translated(string translation, string to, int numOfWords)
        {
            value = translation;
            lang = to;
            words = new word[numOfWords];
            setLangName(to);
        }
        public string value { get; set; }
        public string lang { get; set; }
        public string langName { get; set;}
        public word[] words { get; set; }
        //public Stream speak { get; set; }
        void setLangName(string t)
        {
            switch (t)
            {
                case "ar":langName = "Arabic"; break;
                case "bg":langName ="Bulgarian"; break;
                case "da":langName ="Danish"; break;
                case "nl":langName ="Dutch"; break;
                case "en":langName ="English"; break;
                case "fr":langName ="French"; break;
                case "de":langName ="German"; break;
                case "hi":langName ="Hindi"; break;
                case "it":langName ="Italian"; break;
                case "ja":langName ="Japanese"; break;
                case "ko":langName ="Korean"; break;
                case "pt":langName ="Portuguese"; break;
                case "ru":langName ="Russian"; break;
                case "es":langName ="Spanish"; break;
                case "sv":langName ="Swedish"; break;
                case "th":langName ="Thai"; break;
                case "tr":langName ="Turkish"; break;
            }
        }
    }
    public class word
    {
        public word(string _theWord, string lang)
        {
            theWord = _theWord;
            definition = lang;
        }

        public string theWord;
        public string definition;

    }
    [DataContract]
    public class GlosbeObject
    {
        [DataMember]
        public GlosbePhraseArr[] tuc = new GlosbePhraseArr[10];
        [DataMember]
        public string from { get; set; }
    }
    [DataContract]
    public class GlosbePhraseArr
    {
        [DataMember]
        public GlosbePhrase phrase { get; set; }
        [DataMember]
        public string meaningID { get; set; }
    }
    [DataContract]
    public class GlosbePhrase
    {
        public string text { get; set; }
    }

    /// <summary>
    /// microsoft translator access token class below
    /// </summary>
    class Program
    {
        private static void DetectMethod(string authToken)
        {
            Console.WriteLine("Enter Text to detect language:");
            string textToDetect = "Hola";
            //Keep appId parameter blank as we are sending access token in authorization header.
            string uri = "http://api.microsofttranslator.com/v2/Http.svc/Detect?text=" + textToDetect;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.Headers.Add("Authorization", authToken);
            WebResponse response = null;
            try
            {
                response = httpWebRequest.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    System.Runtime.Serialization.DataContractSerializer dcs = new System.Runtime.Serialization.DataContractSerializer(Type.GetType("System.String"));
                    string languageDetected = (string)dcs.ReadObject(stream);
                    Console.WriteLine(string.Format("Language detected:{0}", languageDetected));
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey(true);
                }
            }

            catch
            {
                throw;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }
        }
        private static void ProcessWebException(WebException e)
        {
            Console.WriteLine("{0}", e.ToString());
            // Obtain detailed error information
            string strResponse = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)e.Response)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(responseStream, System.Text.Encoding.ASCII))
                    {
                        strResponse = sr.ReadToEnd();
                    }
                }
            }
            Console.WriteLine("Http status code={0}, error message={1}", e.Status, strResponse);
        }
    }
    [DataContract]
    public class AdmAccessToken
    {
        [DataMember]
        public string access_token { get; set; }
        [DataMember]
        public string token_type { get; set; }
        [DataMember]
        public string expires_in { get; set; }
        [DataMember]
        public string scope { get; set; }
    }

    public class AdmAuthentication
    {
        public static readonly string DatamarketAccessUri = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
        private string clientId;
        private string clientSecret;
        private string request;
        private AdmAccessToken token;
        private Timer accessTokenRenewer;

        //Access token expires every 10 minutes. Renew it every 9 minutes only.
        private const int RefreshTokenDuration = 9;

        public AdmAuthentication(string clientId, string clientSecret)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            //If clientid or client secret has special characters, encode before sending request
            this.request = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com", HttpUtility.UrlEncode(clientId), HttpUtility.UrlEncode(clientSecret));
            this.token = HttpPost(DatamarketAccessUri, this.request);
            //renew the token every specfied minutes
            accessTokenRenewer = new Timer(new TimerCallback(OnTokenExpiredCallback), this, TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
        }

        public AdmAccessToken GetAccessToken()
        {
            return this.token;
        }


        private void RenewAccessToken()
        {
            AdmAccessToken newAccessToken = HttpPost(DatamarketAccessUri, this.request);
            //swap the new token with old one
            //Note: the swap is thread unsafe
            this.token = newAccessToken;
            Console.WriteLine(string.Format("Renewed token for user: {0} is: {1}", this.clientId, this.token.access_token));
        }

        private void OnTokenExpiredCallback(object stateInfo)
        {
            try
            {
                RenewAccessToken();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Failed renewing access token. Details: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    accessTokenRenewer.Change(TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Failed to reschedule the timer to renew access token. Details: {0}", ex.Message));
                }
            }
        }


        private AdmAccessToken HttpPost(string DatamarketAccessUri, string requestDetails)
        {
            //Prepare OAuth request 
            WebRequest webRequest = WebRequest.Create(DatamarketAccessUri);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
            webRequest.ContentLength = bytes.Length;
            using (Stream outputStream = webRequest.GetRequestStream())
            {
                outputStream.Write(bytes, 0, bytes.Length);
            }
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AdmAccessToken));
                //Get deserialized object from JSON stream
                AdmAccessToken token = (AdmAccessToken)serializer.ReadObject(webResponse.GetResponseStream());
                return token;
            }
        }
    }
}
