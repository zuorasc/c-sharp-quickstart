@echo off
svcutil /t:code /out:Proxy/ApiProxy.cs /NoConfig /internal  /ct:System.Collections.Generic.List`1  zuora.a.42.0CUSTOM.wsdl

echo !!! Search/Replace in ApiProxy.cs: 'api.zuora.com.ErrorCode' to 'ErrorCode'
pause