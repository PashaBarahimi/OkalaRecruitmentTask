# Okala Recruitment Task

## Introduction

This repository contains the source code for the Okala recruitment task. The task is to create a simple API that allows users to fetch a cryptocurrency quote by entering the cryptocurrency symbol.

This project uses the following APIs:

- [CoinMarketCap](https://coinmarketcap.com/api/) for fetching cryptocurrency quotes.
- [ExchangeRate-API](https://exchangeratesapi.io/) for fetching exchange rates.

The project is implemented using ASP.NET Core Web API and C#.

## Secrets Initialization

Before running the project, you need to set the following secrets:

- CoinMarketCap API Key
- ExchangeRate-API Access Key

To set the secrets, run the following commands:

```bash
dotnet user-secrets --project OkalaRecruitmentTask set "Quotes:APIs:CoinMarketCap:APIKey" "<CoinMarketCap API Key>"
dotnet user-secrets --project OkalaRecruitmentTask set "Quotes:APIs:ExchangeRates:APIKey" "<ExchangeRate-API Access Key>"
```

## Running the Project

To run the project, execute the following command:

```bash
dotnet run --project OkalaRecruitmentTask
```

The project will be hosted on `http://localhost:5173`. You can use Postman or any other API client to test the API. The API endpoint is `/api/quote/{symbol}`, replace `{symbol}` with the cryptocurrency symbol you want to fetch the quote for. An example request would be:

```http
GET http://localhost:5173/api/quote/BTC
```

Alternatively, you can use the Swagger UI to test the API. The Swagger UI is available at `http://localhost:5173/swagger/index.html`.

## Running the Tests

To run the tests, execute the following command:

```bash
dotnet test
```

The tests are implemented using xUnit and Moq.
