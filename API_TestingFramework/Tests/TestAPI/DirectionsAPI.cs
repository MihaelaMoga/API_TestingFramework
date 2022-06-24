using API_Testing_Framework.Utilities;
using API_TestingFramework.DataModels.APIModels;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace API_Testing_Framework.Tests.TestAPI
{
    class DirectionsAPI
    {
        //pt APi, RestClient e ca un browser
        RestClient client = new RestClient();


        //API-ul are un URL cu parametrii =>parametrii care NU se schimba niciodata + parametrii variabili din directionsData.csv ii numesc queryParams
        //string url = "https://www.mapquestapi.com/directions/v2/route?key=5yYBkyjdGHmBPyVOSYVwVhijOS2LhRBA&from=Bucharest&to=Mangalia&outFormat=xml";
        //metoda care returneaza url-ul de API
        private string GetUrl(string queryParams)
        {
            //returnam url-ul API-ului cu: parametrii care NU se schimba niciodata + parametrii variabili din directionsData.csv ii numesc queryParams
            //queryParams e un dictionar care contine datele din directionsData.csv
            return String.Format("{0}{1}", Utilities.FrameworkConstants.GetApiUrl(), queryParams);
        }





        //datele de test (parametrii url-ului de API numiti paramsList si expected results numiti resultsList) sunt in fisier csv (in TestData\TestDataAPI)  
        //metoda pt a citi datele de test 
        private static IEnumerable<TestCaseData> GetRequestData()
        {
            var paramsList = Utils.ConvertCsvToDictionary("TestData\\TestDataAPI\\directionsData.csv");
            var resultsList = Utils.ConvertCsvToDictionary("TestData\\TestDataAPI\\directionsValidations.csv");

            //trebuie ca nr de linii din directionsData.csv sa fie egal cu nr de linii din directionsValidations.csv
            if (paramsList.Count == resultsList.Count)
            {

                //pt fiecare dictionar din paramList (adica din directionsData.csv)
                for (int i = 0; i < paramsList.Count; i++)
                {
                    //returnam TestCaseData cu 2 dictionare: paramList (adica parametrii URL-ului din directionsData.csv) si resultsList (adica expected results din directionsValidations.cs)
                    yield return new TestCaseData(paramsList[i], resultsList[i]);
                }

            }
            else
            {
                Console.WriteLine("There is a test data/test validation count consistency");
            }
        }




        //testam un request GET catre API
        //test data e CITIT cu metoda de mai sus (numita GetRequestData)
        [Test, TestCaseSource("GetRequestData")]

        //Test01 are 2 parametrii: dictionarul queryParams (unde sunt parametrii API-ului) si dictionarul results
        public void Test01(Dictionary<string, string> queryParams, Dictionary<string, string> results)
        {
            //queryParams e un dictionar care contine datele din directionsData.csv
            //API-ul are un link cu parametrii=> construim Url-ul API-ului
            //Dictionary<string, string> queryParams = new Dictionary<string, string>();
            //queryParams.Add("to", to);
            //queryParams.Add("from", from);
            //queryParams.Add("outFormat", "json");
            //queryParams.Add("unit", "k");
            string url = GetUrl(Utils.ConvertDictionaryToQuerry(queryParams));


            //construim requestul GET de trimis catre server
            //in REQUEST voi indica cu ce METODA ne conectam la API: GET/POST/PUT/DELETE, etc
            var request = new RestRequest(url, Method.Get);

            //trimite requestul catre server
            var response = client.GetAsync(request);

            //salvez raspunsul serverului in responseContent
            var responseContent = response.Result.Content;
            Console.WriteLine(responseContent);

            //dupa ce rulez linia de mai jos=> apikey criptat =>in apiconfig.properties: valorea cheii necriptate o inlocuiesc cu valoarea apikey-ului criptat
            //Console.WriteLine(Utils.Encrypt(apiKey,"btauto2022"));         
            //Console.WriteLine(Utils.Decrypt(Utilities.FrameworkConstants.apikeyEncrypt + "==","btauto2022"));

            Console.WriteLine(url);


            //in stringul outFormat voi salva outFormat din directionsData.csv
            string outFormat = queryParams["outFormat"];
            switch (outFormat)
            {
                //daca in directionsData.csv, outFormat=json
                case "json":
                    {
                        //responseContent il parsam ca JObject (obiect de tip json)
                        var json = JObject.Parse(responseContent);

                        //comparam results (de exemplu expected distance si expected formattedTime) cu "$..route.distance" (actual distance) respectiv "$..route.formattedTime" (adica actual formattedTime)
                        //folosim variabila json ca sa cautam "$..route.distance" in responseContent parsat  
                        //SelectToken e un metoda ca si FindElement (dar intr-un json) si vrem sa returneze un string (=> folosim ToString)

                        Assert.AreEqual(results["distance"], json.SelectToken("$..route.distance").ToString().Replace(",", "."));
                        Console.WriteLine("Expected distance is {0}", results["distance"]);
                        Console.WriteLine("Actual distance is {0}", json.SelectToken("$..route.distance").ToString().Replace(",", "."));

                        Console.WriteLine();

                        Assert.AreEqual(results["formattedTime"], json.SelectToken("$..route.formattedTime").ToString().Replace(",", "."));
                        Console.WriteLine("Expected formattedTime is {0}", results["formattedTime"]);
                        Console.WriteLine("Axpected formattedTime is {0}", json.SelectToken("$..route.formattedTime").ToString().Replace(",", "."));


                        break;
                    }

                //daca in directionsData.csv, outFormat=xml (Cluj - Iasi)
                case "xml":
                    {
                        //click pe bec galben si click pe using.System.Xml
                        XmlDocument doc = new XmlDocument();
                        //luam raspunsul (adica responseContent) din doc
                        doc.LoadXml(responseContent);

                        //afisam responseContent;
                        //de ce? din responseContent afisat la consola, in "Open result log": luam response-ul pt Cluj Iasi (pt care outFormat=xml) si il punem in xpather.com ca sa luam xpath
                        Console.WriteLine(responseContent);
                        //pt distance fac un XmlNode distance in care salvez xpath-ul gasit cu xpather.com 
                        XmlNode distance = doc.SelectSingleNode("/response/route/distance");
                        XmlNode formattedTime = doc.SelectSingleNode("/response/route/formattedTime");

                        //compar expected distance cu actual distance din fisierul xml
                        Assert.AreEqual(results["distance"], distance.InnerText);
                        //compar expected firmattedTime cu actual formattedTime din fisierul xml
                        Assert.AreEqual(results["formattedTime"], formattedTime.InnerText);

                        break;
                    }

                default:
                    {
                        Console.WriteLine("In directionsData.csv, OutFormat should be only xml or json");
                        break;
                    }
            }

        }


//metoda prin care CITIM datele de test dintr-un request POST catre API
        private MapQuestRequestModel GetPostData(Dictionary<string, string> kvp)
        {
            //cream obiectul mapOptions (pt options)
            MapQuestOptionsModel mapOptions = new MapQuestOptionsModel
            {
                //valorile de dupa egal sunt luate din documentatia API-ului (de pe site mapquest.com)

                avoids = new List<string>(),
                avoidTimedConditions = false,
                doReverseGeocode = true,
                shapeFormat = "raw",
                generalize = 0,
                routeType = "fastest",
                timeType = 1,
                locale = "en_US",
                unit = kvp["unit"],
                enhancedNarrative = false,
                drivingStyle = 2,
                //float
                highwayEfficiency = 21.0f,
            };

            //cream obiectul request care are 2 parametrii: lista de locations si lista de options
            MapQuestRequestModel request = new MapQuestRequestModel
            {
                locations = new List<string>
                                {
                                    kvp["to"],
                                    kvp["from"]
                                },
                options = mapOptions
            };

            //returnam request-ul POST
            return request;
        }






        //testam un POST Request catre server
        [Test, TestCaseSource("GetRequestData")]
        public void Test02(Dictionary<string, string> queryParams, Dictionary<string, string> results)
        {
            //pas 1:API-ul are un link cu parametrii=> construim Url-ul API-ului => cream url-ul (adica HEADER-ul) pt requestul de tip POST
            //cream un obiect de tip dictionary
            //postQueryParams e un dictionar care contine datele din directionsData.csv care vor fi folosite in url-ul de POST
            var postQueryParams = new Dictionary<string, string>();
            //in dictionar adaugam cheia outFormat si valoarea outFormat
            postQueryParams.Add("outFormat", queryParams["outFormat"]);
            //key nu e necesara pt ca e deja adaugata in metoda GetUrl()
            //postQueryParams.Add("key", queryParams["key"]);
            postQueryParams.Add("to", queryParams["to"]);
            postQueryParams.Add("from", queryParams["from"]);
            postQueryParams.Add("unit", queryParams["unit"]);
            string url = GetUrl(Utils.ConvertDictionaryToQuerry(postQueryParams));

            //pas 2: cream request de tip POST    
            //pt APi, RestClient e ca un browser
            var client = new RestClient(url);
            //cream requestul cu 2 parametrii: url-ul catre API si tipul request-ului care e POST
            var request = new RestRequest(url, Method.Post);


            //pas 3: pe langa HEADER, tb sa construim si BODY-ul requestului (unde punem parametrii locations si options)
            //spre deosebire de request-ul GET, la POST requestul tb sa aiba info in BODY
            //BODY-ul requestului de tip POST: prin serializare obtin un fisier json in care vars parametrii: locations si options 
            string jsonBody = System.Text.Json.JsonSerializer.Serialize(GetPostData(queryParams));


            //pas 4: BODY-ul creat la pas 3 il adaug la request
            request.AddParameter(jsonBody, ParameterType.RequestBody);
            //indic ca request-ul POST e de tip json
            request.RequestFormat = DataFormat.Json;


            //pas 5: salvez response-ul serverului in variabila response
            var response = client.PostAsync(request);
            Console.WriteLine(response.Result.Content.ToString());


            //PAS 6: in continuare scriem acelasi ASSERTII ca in Test01

            //in stringul outFormat voi salva outFormat din directionsData.csv
            string outFormat = queryParams["outFormat"];
            switch (outFormat)
            {
                //daca in directionsData.csv, outFormat=json
                case "json":
                    {
                        //responseContent il parsam ca JObject (obiect de tip json)
                        var json = JObject.Parse(response.Result.Content);

                        //comparam results (de exemplu expected distance si expected formattedTime) cu "$..route.distance" (actual distance) respectiv "$..route.formattedTime" (adica actual formattedTime)
                        //folosim variabila json ca sa cautam "$..route.distance" in responseContent parsat  
                        //SelectToken e un metoda ca si FindElement (dar intr-un json) si vrem sa returneze un string (=> folosim ToString)

                        Assert.AreEqual(results["distance"], json.SelectToken("$..route.distance").ToString().Replace(",", "."));
                        Console.WriteLine("Expected distance is {0}", results["distance"]);
                        Console.WriteLine("Actual distance is {0}", json.SelectToken("$..route.distance").ToString().Replace(",", "."));

                        Console.WriteLine();

                        Assert.AreEqual(results["formattedTime"], json.SelectToken("$..route.formattedTime").ToString().Replace(",", "."));
                        Console.WriteLine("Expected formattedTime is {0}", results["formattedTime"]);
                        Console.WriteLine("Axpected formattedTime is {0}", json.SelectToken("$..route.formattedTime").ToString().Replace(",", "."));

                        break;
                    }

                //daca in directionsData.csv, outFormat=xml (Cluj - Iasi)
                case "xml":
                    {
                        //click pe bec galben si click pe using.System.Xml
                        XmlDocument doc = new XmlDocument();
                        //luam raspunsul (adica responseContent) din doc
                        doc.LoadXml(response.Result.Content);

                        //afisam responseContent;
                        //de ce? din responseContent afisat la consola, in "Open result log": luam response-ul pt Cluj Iasi (pt care outFormat=xml) si il punem in xpather.com ca sa luam xpath
                        Console.WriteLine(response.Result.Content);
                        //pt distance fac un XmlNode distance in care salvez xpath-ul gasit cu xpather.com 
                        XmlNode distance = doc.SelectSingleNode("/response/route/distance");
                        XmlNode formattedTime = doc.SelectSingleNode("/response/route/formattedTime");

                        //compar expected distance cu actual distance din fisierul xml
                        Assert.AreEqual(results["distance"], distance.InnerText);
                        //compar expected firmattedTime cu actual formattedTime din fisierul xml
                        Assert.AreEqual(results["formattedTime"], formattedTime.InnerText);

                        break;
                    }

                default:
                    {
                        Console.WriteLine("In directionsData.csv, OutFormat should be only xml or json");
                        break;
                    }

            }

        }


    }
}
