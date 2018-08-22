using System;

using Klangoo.Client;

namespace Klangoo.Example.NLP
{
    class Program
    {
        const string ENDPOINT = "https://nlp.klangoo.com/Service.svc";
        const string CALK = "enter your calk here";
        const string SECRET_KEY = "enter your secret key here";

        static void Main(string[] args)
        {
            ProcessDocument();
            GetSummary();
            GetEntities();
            GetCategories();
            GetKeyTopics();
        }

        static void ProcessDocument()
        {
            MagnetAPIClient client = new MagnetAPIClient(ENDPOINT, CALK, SECRET_KEY);

            string json = client.CallWebMethod("ProcessDocument",
                new MagnetAPIClient.Params { 
                    { "text", "The United States of America (USA), commonly known as the United States (U.S.) or America, is a federal republic composed of 50 states, a federal district, five major self-governing territories, and various possessions." },
                    { "lang", "en" },
                    { "format", "json" } }, "POST");

            Console.WriteLine(json);
        }

        static void GetSummary()
        {
            MagnetAPIClient client = new MagnetAPIClient(ENDPOINT, CALK, SECRET_KEY);

            string json = client.CallWebMethod("GetSummary",
                new MagnetAPIClient.Params { 
                    { "text", "The United States of America (USA), commonly known as the United States (U.S.) or America, is a federal republic composed of 50 states, a federal district, five major self-governing territories, and various possessions." },
                    { "lang", "en" },
                    { "format", "json" } }, "POST");

            Console.WriteLine(json);
        }

        static void GetEntities()
        {
            MagnetAPIClient client = new MagnetAPIClient(ENDPOINT, CALK, SECRET_KEY);

            string json = client.CallWebMethod("GetEntities",
                new MagnetAPIClient.Params { 
                    { "text", "The United States of America (USA), commonly known as the United States (U.S.) or America, is a federal republic composed of 50 states, a federal district, five major self-governing territories, and various possessions." },
                    { "lang", "en" },
                    { "format", "json" } }, "POST");

            Console.WriteLine(json);
        }

        static void GetCategories()
        {
            MagnetAPIClient client = new MagnetAPIClient(ENDPOINT, CALK, SECRET_KEY);

            string json = client.CallWebMethod("GetCategories",
                new MagnetAPIClient.Params { 
                    { "text", "The United States of America (USA), commonly known as the United States (U.S.) or America, is a federal republic composed of 50 states, a federal district, five major self-governing territories, and various possessions." },
                    { "lang", "en" },
                    { "format", "json" } }, "POST");

            Console.WriteLine(json);
        }

        static void GetKeyTopics()
        {
            MagnetAPIClient client = new MagnetAPIClient(ENDPOINT, CALK, SECRET_KEY);

            string json = client.CallWebMethod("GetKeyTopics",
                new MagnetAPIClient.Params { 
                    { "text", "The United States of America (USA), commonly known as the United States (U.S.) or America, is a federal republic composed of 50 states, a federal district, five major self-governing territories, and various possessions." },
                    { "lang", "en" },
                    { "format", "json" } }, "POST");

            Console.WriteLine(json);
        }
    }
}
