# SimpleTestUI
* This is a simple UI that uses the Zuora Service to do various calls to Zuora

## Install
* git clone
* open solution in Visual Studio 2012
* Run TestingUI project

## Tests
* xUnit unit tests are included, however these don't run natively in the VS Express

## Generating WSDL
* Run GenProxy.bat from the folder
* Search/Replace in ApiProxy.cs: 'api.zuora.com.ErrorCode' to 'ErrorCode'
