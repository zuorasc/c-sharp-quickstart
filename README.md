# SimpleTestUI
* This is a simple UI that uses the Zuora Service C# wrapper to do various calls to Zuora

## Install
* Git clone https://github.com/zuorasc/SimpleTestUI.git
* Open solution in Visual Studio 2012 for Web
* Run TestingUI project

## Tests
* xUnit unit tests are included, however these don't run natively in the VS Express

## Generating WSDL
* Run GenProxy.bat from the folder
* Search/Replace in ApiProxy.cs: 'api.zuora.com.ErrorCode' to 'ErrorCode'
