**This library allows you to easily use the Magnet API using C# via .NET.**

# Table of Contents

* [About](#about)
* [Installation](#installation)
* [Usage](#usage)

<a name="about"></a>
# About

Klangoo NLP API is a natural language processing (NLP) service that uses the rule-based paradigm and machine learning to recognize the aboutness of text. The service recognizes the category of the text, extracts key disambiguated topics, places, people, brands, events, and 41 other types of names; analyzes text using tokenization, parts of speech, parsing, word sense disambiguation, named entity recognition; and automatically finds the relatedness score between documents.

[Read More](https://klangoosupport.zendesk.com/hc/en-us/categories/360000812171-Klangoo-Natural-Language-API).

[Signup for a free trail](https://connect.klangoo.com/pub/Signup/)

<a name="installation"></a>
# Installation

## Prerequisites

- This library is compatible with .Net Framework 4.x, .Net Core 2.x, .Net Core 3.x, .NET 5 and .NET 6
- An API Key Provided by [Klangoo](https://klangoosupport.zendesk.com/hc/en-us/articles/360015236872-Step-2-Registering-to-Klangoo-NLP-API)
- An API Secret Provided by [Klangoo](https://klangoosupport.zendesk.com/hc/en-us/articles/360015236872-Step-2-Registering-to-Klangoo-NLP-API)


## Install

### From nuget using package manager (Recommended)

```
PM> Install-Package Klangoo.Magnet.ApiClient
```
### Manually
To use MagnetApiClient in your C# .NET project, you can <a href="https://github.com/Klangoo/MagnetApiClient.CSharp">download the Magnet API Library directly from our Github repository</a> and reference it in your project.


<a name="usage"></a>
# Usage

This quick start tutorial will show you how to process a text.

## Initialize the client

To begin, you will need to initialize the client. In order to do this you will need your API Key **CALK** and **Secret Key**.
You can find both on [your Klangoo account](https://connect.klangoo.com/).

```C#
using Klangoo.Client;


static void ProcessDocument()
{
	string ENDPOINT = "https://nlp.klangoo.com/Service.svc";
    string CALK = "enter your calk here";
    string SECRET_KEY = "enter your secret key here";

	MagnetAPIClient client = new MagnetAPIClient(ENDPOINT, CALK, SECRET_KEY);

	string json = client.CallWebMethod("ProcessDocument",
		new MagnetAPIClient.Params { 
			{ "text", "The United States of America (USA), commonly known as the United States (U.S.) or America, is a federal republic composed of 50 states, a federal district, five major self-governing territories, and various possessions." },
			{ "lang", "en" },
			{ "format", "json" } }, "POST");

	Console.WriteLine(json);
}
```