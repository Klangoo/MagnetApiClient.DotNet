**This library allows you to easily use the Magnet API using C# via .NET.**

# Table of Contents

* [About](#about)
* [Installation](#installation)
* [Usage](#usage)


<a name="about"></a>
# About

Magnet offers online publishers and digital content providers with the following features to help them better engage their users on their website and mobile App.
- Topically Related Articles
- Personalized Recommendations
- Automatically Generated Summary
- Follow a Specific Topic
- Follow a Developing Story
- Meta Tags/Description
- Highlight Named-Entities
- Entity Listing
- Entity Pages

[Read More](http://www.klangoo.com/Engagement.aspx).

[Book a demo with our sales team now!](mailto:sales@klangoo.com)

<a name="installation"></a>
# Installation

## Prerequisites

- An API Key Provided by [Klangoo](http://klangoo.com)
- An API Secret Provided by [Klangoo](http://klangoo.com)

## Install

To use MagnetApiClient in your C# .NET project, you can either <a href="https://github.com/Klangoo/MagnetApiClient.CSharp">download the Magnet API Library directly from our Github repository</a> and reference it in your project or, if you have the Nuget package manager installed, you can download it automatically by running

```
PM> Install-Package Klangoo.Magnet.ApiClient
```

Once you have the Magnet API Client properly referenced in your project, you can start sending calls to the API in your code.
For sample implementations, check the [news agency sample](https://github.com/Klangoo/MagnetApiClient.CSharp/blob/master/NewsAgencySample.cs).

<a name="usage"></a>
# Usage

## Get Article

The following is an example for reading an article from the API:

```C#
private static void GetArticle(string articleUID)
{
	MagnetAPIClient magnetAPIClient  = new MagnetAPIClient(ENDPOINT_URI, CALK, SECRET_KEY);
			
	Dictionary<string, string> request = new Dictionary<string, string>();
	request.Add("articleUID", articleUID);
	request.Add("format", "xml");

	try
	{
		String response = magnetAPIClient.CallWebMethod("GetArticle", request, "GET");
		XmlDocument doc = new XmlDocument();
		doc.LoadXml(response);

		if (doc.DocumentElement["status"].InnerText == "OK")
		{
			Console.WriteLine("GetArticle:");
			Console.WriteLine(response);
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
```

## Add Article
The same applies for posting or updating an article. following is an example for adding an article:

```C#
public static void AddArticle(string articleUID)
{
	MagnetAPIClient magnetAPIClient  = new MagnetAPIClient(ENDPOINT_URI, CALK, SECRET_KEY);

	Dictionary<string, string> request = new Dictionary<string, string>();
	request.Add("text", "SAMPLE ARTICLE TEXT");
	request.Add("title", "SAMPLE ARTICLE TITLE");
	request.Add("insertDate", "23 JAN 2017 10:12:00 +01:00"); // article date
	request.Add("url", "http://demo.klangoo.com/article-demo/api-example");
	request.Add("articleUID", articleUID);
	request.Add("source", "klangoo.com");
	request.Add("language", "en");
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
```

You can find an example implementation for all of the API calls here [here](https://github.com/Klangoo/MagnetApiClient.CSharp/blob/master/NewsAgencySample.cs).
