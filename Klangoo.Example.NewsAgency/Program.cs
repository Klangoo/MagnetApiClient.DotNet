using System;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using Klangoo.Client;

namespace Klangoo.Example.NewsAgency
{
    class Program
    {
        private static string ENDPOINT = "https://magnetapi.klangoo.com/NewsAgencyService.svc";
        private static string CALK = "";// use your own CALK
        private static string SECRET_KEY = "";// use your own Secret Key

        private static MagnetAPIClient _magnetAPIClient;

        static void Main(string[] args)
        {
            _magnetAPIClient = new MagnetAPIClient(ENDPOINT, CALK, SECRET_KEY);

            string articleUID = "wiki/United_States";

            AddArticle(articleUID);
            UpdateArticle(articleUID);
            DeleteArticle(articleUID);
            GetArticle(articleUID);
            ShowIndex();
        }

        public static void AddArticle(string articleUID)
        {
            Dictionary<string, string> request = new Dictionary<string, string>();
            request.Add("text", "The United States of America (USA), commonly known as the United States (U.S.) or America, is a federal republic composed of 50 states, a federal district, five major self-governing territories, and various possessions.");
            request.Add("insertDate", "23 OCT 2015 10:12:00 +01:00"); // article date
            request.Add("url", "https://en.wikipedia.org/wiki/United_States");
            request.Add("articleUID", articleUID);
            request.Add("source", "en.wikipedia.org");
            request.Add("language", "en"); // English
            request.Add("format", "xml");

            try
            {
                string response = _magnetAPIClient.CallWebMethod("AddArticle", request, "POST");
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response);

                if (doc.DocumentElement["status"].InnerText == "OK")
                {
                    Console.WriteLine("AddArticle:");
                    Console.WriteLine(response);
                    WriteToFile(response, "AddArticle.xml");
                }
                else
                {
                    // ERROR
                    HandleApiError(doc);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured: " + ex.Message);
            }
        }

        private static void UpdateArticle(string articleUID)
        {
            Dictionary<string, string> request = new Dictionary<string, string>();
            request.Add("articleUID", articleUID);
            request.Add("text", "The United States of America is a federal republic composed of 50 states, a federal district, five major self-governing territories, and various possessions.");

            request.Add("updateDate ", "23 OCT 2015 10:12:00 +01:00"); // article date
            request.Add("language", "en"); // English
            request.Add("format", "xml");

            try
            {
                string response = _magnetAPIClient.CallWebMethod("UpdateArticle", request, "POST");
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response);

                if (doc.DocumentElement["status"].InnerText == "OK")
                {
                    Console.WriteLine("UpdateArticle:");
                    Console.WriteLine(response);
                    WriteToFile(response, "UpdateArticle.xml");
                }
                else
                {
                    // ERROR
                    HandleApiError(doc);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured: " + ex.Message);
            }
        }

        private static void HandleApiError(XmlDocument doc)
        {
            XmlElement rootElement = doc.DocumentElement;
            XmlElement errorNode = rootElement["error"];
            if (errorNode != null)
            {
                int errorNo = int.Parse(errorNode["errorNo"].InnerText);
                string errorMessage = "";

                if (errorNode["errorMessage"] != null)
                {
                    errorMessage = errorNode["errorMessage"].InnerText;
                }

                Console.WriteLine("Error occured -- errorNo: " + errorNo + " -- errorMessage: " + errorMessage);
            }
        }

        private static void DeleteArticle(string articleUID)
        {
            Dictionary<string, string> request = new Dictionary<string, string>();
            request.Add("articleUID", articleUID);
            request.Add("format", "xml");

            try
            {
                string response = _magnetAPIClient.CallWebMethod("DeleteArticle", request, "POST");
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response);

                if (doc.DocumentElement["status"].InnerText == "OK")
                {
                    Console.WriteLine("DeleteArticle:");
                    Console.WriteLine(response);
                    WriteToFile(response, "DeleteArticle.xml");
                }
                else
                {
                    // ERROR
                    HandleApiError(doc);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured: " + ex.Message);
            }
        }

        private static void GetArticle(string articleUID)
        {
            Dictionary<string, string> request = new Dictionary<string, string>();
            request.Add("articleUID", articleUID);
            request.Add("format", "xml");

            try
            {
                String response = _magnetAPIClient.CallWebMethod("GetArticle", request, "GET");
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response);

                if (doc.DocumentElement["status"].InnerText == "OK")
                {
                    Console.WriteLine("GetArticle:");
                    Console.WriteLine(response);
                    WriteToFile(response, "GetArticle.xml");
                }
                else
                {
                    // ERROR
                    HandleApiError(doc);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured: " + ex.Message);
            }
        }

        private static void ShowIndex()
        {
            Dictionary<string, string> request = new Dictionary<string, string>();
            request.Add("page", "0");
            request.Add("orderByDate", "true");
            request.Add("format", "xml");

            try
            {
                String response = _magnetAPIClient.CallWebMethod("ShowIndex", request, "GET");
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response);

                if (doc.DocumentElement["status"].InnerText == "OK")
                {
                    Console.WriteLine("ShowIndex:");
                    Console.WriteLine(response);
                    WriteToFile(response, "ShowIndex.xml");
                }
                else
                {
                    // ERROR
                    HandleApiError(doc);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured: " + ex.Message);
            }
        }

        private static void WriteToFile(string response, string filename)
        {
            try
            {
                TextWriter fWriter = new StreamWriter(filename);
                fWriter.Write(response);
                fWriter.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured: " + ex.Message);
            }
        }
    }
}
