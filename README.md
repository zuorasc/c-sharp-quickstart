# SimpleTestUI
* This is a simple UI that uses the Zuora Service C# wrapper to do various calls to Zuora

## Install
* Git clone https://github.com/zuorasc/SimpleTestUI.git
* Open solution in Visual Studio 2012 for Web
* Run TestingUI project

## Tests
* xUnit unit tests are included, however these don't run natively in the VS Express

## Generating WSDL
* Add the custom fields to the wsdl in the zuora services project folder
* Open up GenProxy.bat in Visual Studio and make sure the wsdl name is correct
* Open up the folder in explorer/finder
* Run GenProxy.bat from the folder
* Search/Replace in ApiProxy.cs: 'api.zuora.com.ErrorCode' to 'ErrorCode'

#Legal Notice
      Copyright (c) 2013 Zuora, Inc.
	  
      Permission is hereby granted, free of charge, to any person obtaining a copy of 
	  this software and associated documentation files (the "Software"), to use copy, 
	  modify, merge, publish the Software and to distribute, and sublicense copies of 
	  the Software, provided no fee is charged for the Software.  In addition the
	  rights specified above are conditioned upon the following:
	
	  The above copyright notice and this permission notice shall be included in all
	  copies or substantial portions of the Software.
	
	  Zuora, Inc. or any other trademarks of Zuora, Inc.  may not be used to endorse
	  or promote products derived from this Software without specific prior written
	  permission from Zuora, Inc.
	
	  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	  FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL
	  ZUORA, INC. BE LIABLE FOR ANY DIRECT, INDIRECT OR CONSEQUENTIAL DAMAGES
	  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
	  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
	  ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
	  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
	  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.  
	
	  IN THE EVENT YOU ARE AN EXISTING ZUORA CUSTOMER, USE OF THIS SOFTWARE IS GOVERNED
	  BY THIS AGREEMENT AND NOT YOUR MASTER SUBSCRIPTION AGREEMENT WITH ZUORA.
