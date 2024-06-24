# CurrencyConverter

## Description
This Solution consists of two projects; the CurrencyConverter API Project and the Unit Tests project. 
The API Converts an amount from one currency to another, caches exchange rates for 15 minutes and "archives" exchange rates in a database table after the 15 minutes have elapsed.

### CurrencyConverter API
The API follows a Controller-Service-Repository Pattern and has two Get endpoints: The "Convert" endpoint and the "GetFiltered" endpoint. The API stores and retrieves data from a MySQL Database and uses In-Memory caching. 

Note: Technical Issues were faced in attempts to implement Redis Cache locally. In-Memory caching was used to fulfil the caching functional requirement within the stipulated delivery date.

#### Convert Endpoint

The Convert Endpoint is a Get endpoint that takes two parameters: "base", "target" and "amount".  
Param - Amount: This is the amount that is to be converted from one currency to another based on the current Exchange Rate.  
Param - Base: This parameter is the ISO representation of the currency that the "amount" is currently in. Examples are ZAR - South African Rand; USD - American Dollar; GBP - Great Britain Pound; etc.  
Param - Currency: This parameter is the ISO representation of the currency into which the "amount" is supposed to be converted based on the exchange rate. Examples are ZAR - South African Rand; USD - American Dollar; GBP - Great Britain Pound; etc.

This Endpoint checks the system cache to see if there is a previous exchange rate with the same base currency - target currency pair that has not yet expired.
If it finds an entry in the cache that matches the base currency-target currency pair, it uses the found entry to convert the amount into the converted amount in the target currency.

If a matching value is not found in the cache, then a call is made to the CurrencyExchange API, a third-party API [documentation found here](https://exchangeratesapi.io/documentation/).

The exchange rate is cached for 15 minutes, after which, it is saved in the ConversionHistory database table.

This endpoint returns the converted amount in terms of the target currency.

#### GetFiltered Endpoint

The GetFiltered Endpoint gets the exchange rates that have been saved in the database table based on several search terms.
Param - rateKey: The string that represents a Base Currency - Target Currency pair. E.g, ZARUSD, "USDGBP"  
Param - dateFrom: The earliest date the conversion could have occurred. Human readable dates such as "1 Jan 2024", "1 January 2024" and system dates such as "2024-03-12 20:39:06.3133333" and "2024-03-12" are valid inputs. If dateFrom is not provided as part of the date range, it will return entries recorded on or after exactly a month before the day the call is made (e.g. if the call is made on 1 Feb 2024, dateFrom will default to 1 Jan 2024).  
Param - dateTo: The latest date that the conversion could have occurred. Human readable dates such as "1 Jan 2024", "1 January 2024" and system dates such as "2024-03-12 20:39:06.3133333" and "2024-03-12" are valid inputs. If dateTo is not provided as part of the date range, it will default dateTo to the date the call is made (e.g. if the call is made on 2 Feb 2024, dateTo will default to 2 Feb 2024.  
Param - pageNumber: The number of the page to be returned in a paginated UI. If pageNumber is not provided, the first page of the results is returned.
Param - pageSize: Defines the number of results to return per page in a paginated UI. If pageSize is not provided, the default page size is 20 records per page.  

### Unit Tests

This project uses X-Unit and Moq to cover both the happy and bad paths of the controller and service layer code.
