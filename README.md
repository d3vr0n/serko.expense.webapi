# Serko Expense Webapi

Serko Expense has a new requirement to import data from text received via email.
The data will either be:

* Embedded as ‘islands’ of XML-like content
* Marked up using XML style opening and closing tags

## Installation

Clone repository and then Run build (Build should restore all nuget packages)

```bash
git clone https://github.com/ronniecool/serko.expense.webapi.git
```

## Test

* Hit F5, This should build and open swagger UI
``
http://localhost:58934/swagger/index.html
``
* Click on Expense Post and click Try It Out.
* Paste your test email content on the textbox provided and hit Execute.

You should see expected result under Responses.

## Nuget Usage

This project uses the following Major nuget libraries
* Microsoft.AspNetCore.App
* Swashbuckle.AspNetCore
* Serilog.AspNetCore
* Microsoft.NET.Test.Sdk
* NUnit
* MSTest.TestFramework
* Microsoft.AspNetCore.Mvc.Testing
* Microsoft.AspNetCore.TestHost

## Unit & Integration Tests

Click on VS Test Explorer and run all tests.

## Contributing
Create branch from master and raise pull requests 

## License
[MIT](https://choosealicense.com/licenses/mit/)