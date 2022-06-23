using Microsoft.VisualBasic.FileIO;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace API_Testing_Framework.Utilities
{
    //in clasa Utils vom pune toate metodele statice din proiect
   public class Utils
    {    


//METODA folosita pt citirea datelor de test din fisier csv
        //ReadConfig este metoda pt citirea unui fisier extern, de tip text, DE PROPRIETATI (vezi config.properties sau apiconfig.properties)
        //string configFilePath e calea catre fisierul config.properties/apiconfig.properties
        //eu ii dau fisierul de configurare, de tip PROPERTIES
        //metoda va citi config.properties + va returna un Dictionar de tip cheie valoare cu informatiile din config.properties

        public static Dictionary<string,string> ReadConfig(string configFilePath)
        {
            //cream Dictionarul de tip string,string
            var configData = new Dictionary<string, string>();

            //citeste fiecare linie din fisierul text 
            foreach(var line in File.ReadAllLines(configFilePath))
            {
                //pt fiecare linie din fisier, adaugam in Dictionar: valoarea dinainte de =, valoarea de dupa =
                //am folosit Trim() pt a elimina cazurile cand in config.properties se pune spatiu inainte sau dupa egal
                string[] values = line.Split('=');
                configData.Add(values[0].Trim(), values[1].Trim());
            }

           //metoda va citi fisierul de configurare si va returna un Dictionar de tip cheie valoare cu informatiile din acel fisier
            return configData;
        }







//metoda pentru CONVERTIRE dictionar queryParams in Querry (pt a testa API-ul folosind Selenium)
        public static string ConvertDictionaryToQuerry(Dictionary<string,string> queryParams)
        {
            StringBuilder sb = new StringBuilder();
            //pt fiecare key (adica parametrii API-ului: to, from,outFormat, unit) din queryParams
            foreach(string key in queryParams.Keys)
            {
                //adaug perechile cheie=valoare cheie
                sb.Append(String.Format("&{0}={1}",key,queryParams[key]));
            }
            //returnam un Querry de tip string
            return sb.ToString();
        }








//metoda pt a CONVERTI un fisier CSV intr-o lista de dictionare - am folosit functia asta la API
        //de ce lista de dictionare? pt ca pt fiecare linie din fisierul csv se va crea un dictionar
        public static List<Dictionary<string, string>> ConvertCsvToDictionary(string filePath)
        {
            var lines = File.ReadAllLines(filePath).Select(a => a.Split(','));
            List<Dictionary<string, string>> dictionaryList = new List<Dictionary<string, string>>();
            string[] header = lines.ElementAt(0).ToArray();

            //incepand cu linia 2 a fisierului csv
            for (int i = 1; i < lines.Count(); i++)
            {
                var currentValues = lines.ElementAt(i).ToArray();
             //pt fiecare linie in parte vom avea un dictionar => vom avea o lista de dictionare 
                //queryParams sunt from,to,outFormat,unit
                Dictionary<string, string> queryParams = new Dictionary<string, string>();
                for (int j = 0; j < currentValues.Count(); j++)
                {
                    queryParams.Add(header[j], currentValues[j]);
                }
                //adaugam queryParams in dictionaryList
                dictionaryList.Add(queryParams);
            }

            //metoda curenta returneaza o lista de dictionare
            return dictionaryList;
        }

    }

}

    

