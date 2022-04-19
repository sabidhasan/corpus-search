# Introduction
Text for Semitic languages, such as Arabic and Hebrew, is written using right-to-left (RTL) character sets and is not necessarily legible to scholars or religious people who are often familiar with only a Western character set. As such, there is a need for a search tool that allows input in a Latin-based character set on a normal QWERTY keyboard and doing the search against a corpus in a Semitic character set.

This repository contains the code to CorpusSearch, which, at its core is a web application to allow phonetic searching of the sacred texts of Semitic languages as well as a tool that permits socially reading sacred Semitic texts (such as the Hebrew Bible or the Arabic Qur'an).

I made this to get my hand at several technologies:

| **Technology** | **Purpose**                                       |
|----------------|---------------------------------------------------|
| Python         | Prototyping / validating search algorithm         |
| ASP.net        | Backend framework                                 |
| C#             | Strong typing of ASP.NET backend                  |
| gRPC           | Communication between search service / web server |
| GraphQL        | Communication between web server and front end    |
| Angular 2      | Frontend Framework                                |
| Typescript     | Strong typing for Angula frontend                 |
| Redis          | Caching of search results                         |
| RabbitMQ       | For messaging related to social reading           |
 
There are several parts to this repository:
1. A quick search prototype, written in Python
2. A search microservice, written in C#, which communicates using gRPC
3. An ASP.net microservice (written in C#), handling 

# Search Prototype
## Introduction
The search prototype was written for two reasons:

1. Allow downloading of a Semitic corpus (in this case the Arabic Qur'an), and
2. Validate the methodology for the search algorithm

You can re-generate the corpus from scratch by downloading it from the web (though it _is_ provided as part of the source code in this repository, so is not strictly required). Once the corpus is generated, you can run a phonetic search on the text using.

As it is meant to be a quick prototype, the code is written in Python.

## Running the Prototype Code
To fetch the corpus data (this step is optional, as the data is shipped with the repository):

```bash
    python3 fetch_data.py
```

Next, to run the search, first build the Docker container, and then run it:

```bash
    cd Prototype
    docker build -t arabic-search  .
    # Runs the search term in `search_poc.py`
    docker run arabic-search
```
