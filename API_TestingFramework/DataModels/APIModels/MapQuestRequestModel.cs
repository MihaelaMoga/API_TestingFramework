using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace API_TestingFramework.DataModels.APIModels
{
    //adaug nodul "route" conform xpath
    [XmlRoot(ElementName = "route")]


    public class MapQuestRequestModel
    {
        //adaug nodul "locations"
        [XmlArray(ElementName = "locations")]
        [XmlArrayItem(ElementName = "location")]

        //definim lista cu locations din request body
        public List<string> locations { get; set; }

        //adaug nodul "options"
        [XmlElement(ElementName = "options")]

        //definim optiunile care vor fi de tipul clasei MapQuestOptionsModel => pasul urmator e sa definim clasa MapQuestOptionsModel
        public MapQuestOptionsModel options { get; set; }
    }
}