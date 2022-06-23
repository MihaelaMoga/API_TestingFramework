using System;
using System.Collections.Generic;
using System.Text;

namespace API_Testing_Framework.Utilities
{
    public class FrameworkConstants
    {
        //citesc apiconfig.properties de pe disk refolosind codul de la metoda ReadConfig din Utils
        static Dictionary<string, string> apiConfigData = Utils.ReadConfig("apiconfig.properties");
        static string apiprotocol = apiConfigData["protocol"];
        static string apihostname = apiConfigData["apihost"];
        static string directionsApi = apiConfigData["directionsapi"];
        //static string apiKey = Utils.Decrypt(apiConfigData["apikey"], "btauto2022");
        //"btauto2022" e cheia cu care criptam cheia API-ului
        // static string apiKey = Utils.Decrypt(apiConfigData["apikey"],"btauto2022");
        static string apiKey = apiConfigData["apikey"];
        //am comentat linia de mai jos pt ca nu am mai encriptat apikey
        // public static string apikeyEncrypt = apiConfigData["apikeyencrypt"];



        public static string GetApiUrl()
        {
            //apiKey e cheia necriptata
            //url avand ca parametru doar apiKey (fara to si from care pot varia)
            return String.Format("{0}://{1}{2}?key={3}", apiprotocol, apihostname, directionsApi, apiKey);
        }





    }
}
