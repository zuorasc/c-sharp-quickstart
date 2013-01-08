<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestingUI.aspx.cs" Inherits="Zuora.Services.TestingUI.TestingUI" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Testing UI</title>
</head>
<script type="text/javascript" src="http://code.jquery.com/jquery-latest.min.js" charset="utf-8"></script>

<script type="text/javascript">
    
    $(document).ready( function(){
        hideAll();
    });

    function displayCorrectForm() {
        hideAll();
        var output = $("#ddl1 option:selected").val();
        if (output == "Create Child Account And Increase Credit Balance") {
            $("#amountrow").show();
            $("#ParentAccountRow").show();
            showAccountFields();
            showContactFields();
        }
        else if (output == "Create Account") {
            showAccountFields();
            showContactFields();
        }
        else if (output == "Cancel Credit Balance Adjustment On Invoice") {
            $("#InvoiceIdRow").show();
        }
        else if (output == "Get Invoices For Account") {
            $("#AccountIdRow").show();
        }
        else if (output == "Get Refunds For Account") {
            $("#AccountIdRow").show();
        }
        else if (output == "Get Credit Balance Adjustments For Account") {
            $("#AccountIdRow").show();
        }
        else if (output == "Get Credit Cards For Account") {
            $("#AccountIdRow").show();
        }
        else if (output == "Get PO For Account") {
            $("#AccountIdRow").show();
        }
        else if (output == "Get Account") {
            $("#AccountIdRow").show();
        }
        else if (output == "Get Contact") {
            $("#AccountIdRow").show();
        }
        else if (output == "Change Invoice Template") {
            $("#AccountIdRow").show();
            $("#InvoiceTemplateIdRow").show();
        }
        else if (output == "Does Account Name Exist In Zuora") {
            $("#AccountNameRow").show();
        }
        else if (output == "Get Child Account") {
            $("#AccountIdRow").show();
        }
        else if (output == "Set Parent Account") {
            $("#AccountIdRow").show();
            $("#ParentAccountRow").show();
        }
        else if (output == "Get Single Invoice") {
            $("#InvoiceIdRow").show();
        }
        else if (output == "Increase Credit Balance") {
            $("#AccountIdRow").show();
            $("#amountrow").show();
        }
        else if (output == "Apply Credit Balance To Invoice") {
            $("#InvoiceIdRow").show();
            $("#amountrow").show();
        }
        else if (output == "Refund Electronic Payment") {
            $("#PaymentIdRow").show();
            $("#RefundAmountRow").show();
        }
        else if (output == "Decrease Credit Balance") {
            $("#AccountIdRow").show();
            $("#amountrow").show();
        }
        else if (output == "Apply Payment To Invoice") {
            $("#AccountIdRow").show();
            $("#PaymentMethodIdRow").show();
            $("#PaymentAmountRow").show();
            $("#PaymentTypeRow").show();
            $("#InvoiceIdRow").show();
        }
        else if (output == "Refund External Payment") {
            $("#PaymentIdRow").show();
            $("#RefundAmountRow").show();
            $("#RefundDateRow").show();
            $("#PaymentMethodTypeRow").show();
        }
        else if (output == "Get Rate Plan By Name") {
            $("#NameRow").show();
        }
        else if (output == "Disable Rate Plan") {
            $("#ProductRatePlanIdRow").show();
            $("#DisableDateRow").show();
        }
        else if (output == "Get Product By Name") {
            $("#NameRow").show();
        }
        else if (output == "Create Product"){
            $("#NameRow").show();
            $("#DescriptionRow").show();
            $("#SKURow").show();
            $("#ProductTypeRow").show();
        }
        else if (output == "Create Rate Plan With One Time Charge") {
            $("#NameRow").show();
            $("#ProductIdRow").show();
            $("#PriceRow").show();
        }
        else if (output == "Delete Product") {
            $("#IdToDeleteRow").show();
        }
        else if (output == "Delete Rate Plan") {
            $("#IdToDeleteRow").show();
        }
        else if (output == "Change Price For Product") {
            $("#ProductRatePlanChargeIdRow").show();
            $("#PriceRow").show();
        }
        else if (output == "Subscribe") {
            showAccountFields();
            showContactFields();
            showSubscriptionFields();
        }
        else if (output == "Subscribe With Existing Account") {
            showSubscriptionFields();
            $("#AccountIdRow").show();
        }
        else if (output == "Do Add Product Amendment") {
            $("#SubscriptionIdRow").show();
            $("#AmendmentDateRow").show();
            $("#ProductRatePlanIdRow").show();
        }
        else if (output == "Do Renewal Amendment") {
            $("#SubscriptionIdRow").show();
            $("#AmendmentDateRow").show();
        }
        else if (output == "Do Terms And Conditions Amendment") {
            $("#SubscriptionIdRow").show();
            $("#AmendmentDateRow").show();
            $("#SubscriptionTermTypeRow").show();
            $("#InitalTermRow").show();
            $("#RenewalTermRow").show();
        }
        else if (output == "Get Subscription And Charge Info") {
            $("#SubscriptionIdRow").show();
        }
    }
    function showSubscriptionFields() {
        $("#ProductRatePlanNameRow").show();
        $("#SubscriptionDateRow").show();
        $("#SubscriptionTermTypeRow").show();
        $("#InitalTermRow").show();
        $("#RenewalTermRow").show();
        $("#QuantityRow").show();

    }
    function hideAll() {
        $("#ProductRatePlanNameRow").hide();
        $("#QuantityRow").hide();
        $("#amountrow").hide();
        $("#InvoiceIdRow").hide();

        $("#AccountNameRow").hide();
        $("#BillCycleDayRow").hide();
        $("#TermRow").hide();
        $("#CurrencyRow").hide();
        $("#BatchRow").hide();
        $("#PaymentTermRow").hide();
        $("#FirstNameRow").hide();
        $("#LastNameRow").hide();
        $("#Address1Row").hide();
        $("#Address2Row").hide();
        $("#CityRow").hide();
        $("#StateRow").hide();
        $("#ZipRow").hide();
        $("#CountryRow").hide();
        $("#EmailRow").hide();
        $("#ParentAccountRow").hide();
        $("#AccountIdRow").hide();
        $("#InvoiceTemplateIdRow").hide();
        $("#RefundAmountRow").hide();
        
        $("#PaymentIdRow").hide();
        $("#PaymentMethodIdRow").hide();
        $("#PaymentAmountRow").hide();
        $("#PaymentTypeRow").hide();
        
        $("#RefundDateRow").hide();
        $("#PaymentMethodTypeRow").hide();

        $("#NameRow").hide();

        $("#ProductRatePlanIdRow").hide();
        $("#DisableDateRow").hide();

        $("#DescriptionRow").hide();
        $("#SKURow").hide();
        $("#ProductTypeRow").hide();
        $("#ProductIdRow").hide();
        $("#PriceRow").hide();
        $("#IdToDeleteRow").hide();
        $("#ProductRatePlanChargeIdRow").hide();
        
        $("#SubscriptionDateRow").hide();
        $("#SubscriptionTermTypeRow").hide();
        $("#InitalTermRow").hide();
        $("#RenewalTermRow").hide();

        $("#SubscriptionIdRow").hide();
        $("#AmendmentDateRow").hide();
        $("#ProductRatePlanIdRow").hide();

    }
    function showContactFields() {
        $("#FirstNameRow").show();
        $("#LastNameRow").show();
        $("#Address1Row").show();
        $("#Address2Row").show();
        $("#CityRow").show();
        $("#StateRow").show();
        $("#ZipRow").show();
        $("#CountryRow").show();
        $("#EmailRow").show();
    }

    function showAccountFields() {
        $("#AccountNameRow").show();
        $("#BillCycleDayRow").show();
        $("#PaymentTermRow").show();
        $("#CurrencyRow").show();
        $("#BatchRow").show();
    }
</script>


<body>
    <form id="form1" runat="server">
        <asp:Table ID="Table1" runat="server">
            <asp:TableRow ID="TableRow1" runat="server">
                <asp:TableCell>Enter user name:</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="uname" runat="server"></asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>Enter password:</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="pass" runat="server"></asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>Endpoint:</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="endpoint" runat="server">https://apisandbox.zuora.com/apps/services/a/42.0</asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>Pick Operation:</asp:TableCell>
                <asp:TableCell><asp:DropDownList onChange="displayCorrectForm();" ID="ddl1" runat="server"></asp:DropDownList></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="ParentAccountRow">
                <asp:TableCell>Parent Account Id:</asp:TableCell>   
                <asp:TableCell><asp:TextBox ID="ParentAccountId" runat="server"></asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="amountrow">
                <asp:TableCell>Credit Balance Amount:</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="AmountBox" runat="server"></asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="AccountNameRow">
                <asp:TableCell>Account Name:</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="AccountName" runat="server">Test</asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="BillCycleDayRow">
                <asp:TableCell>Bill Cycle Day</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="BillCycleDay" runat="server">1</asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="BatchRow">
                <asp:TableCell>Batch:</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="Batch" runat="server">Batch1</asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="PaymentTermRow">
                <asp:TableCell>Payment Term:</asp:TableCell>
                <asp:TableCell>
                    <asp:DropDownList ID="PaymentTermDropDown" runat="server">
                        <asp:ListItem Text="Net 30" Value="Net 30"></asp:ListItem>
                        <asp:ListItem Text="Due Upon Reciept" Value="Due Upon Reciept"></asp:ListItem>
                    </asp:DropDownList>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="CurrencyRow">
                <asp:TableCell>Currency:</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="Currency" runat="server">USD</asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="FirstNameRow">
                <asp:TableCell>FirstName:</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="FirstName" runat="server">Donald</asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="LastNameRow">
                <asp:TableCell>LastName:</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="LastName" runat="server">Duck</asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="Address1Row">
                <asp:TableCell>Address1:</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="Address1" runat="server">Fun Lane</asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="Address2Row">
                <asp:TableCell>Address2:</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="Address2" runat="server">1</asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="CityRow">
                <asp:TableCell>City:</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="City" runat="server">Somewhere</asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="StateRow">
                <asp:TableCell>State:</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="State" runat="server">CA</asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="ZipRow">
                <asp:TableCell>Zip:</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="Zip" runat="server">12345</asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="CountryRow">
                <asp:TableCell>Country:</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="Country" runat="server">USA</asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="EmailRow">
                <asp:TableCell>Email:</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="Email" runat="server">test@test.com</asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="InvoiceIdRow">
                <asp:TableCell>InvoiceId:</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="InvoiceId" runat="server"></asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="AccountIdRow">
                <asp:TableCell>AccountId:</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="AccountId" runat="server"></asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="InvoiceTemplateIdRow">
                <asp:TableCell>Invoice Template Id:</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="InvoiceTemplateId" runat="server"></asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="RefundAmountRow">
                <asp:TableCell>Refund Amount:</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="RefundAmount" runat="server"></asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="PaymentIdRow">
                <asp:TableCell>Payment Id:</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="PaymentId" runat="server"></asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="PaymentAmountRow">
                <asp:TableCell>Payment Amount:</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="PaymentAmount" runat="server"></asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="PaymentTypeRow">
                <asp:TableCell>Payment Type:</asp:TableCell>
                <asp:TableCell>
                    <asp:DropDownList ID="PaymentType" runat="server">
                        <asp:ListItem Text="Electronic" Value="Electronic"></asp:ListItem>
                        <asp:ListItem Text="External" Value="External"></asp:ListItem>
                    </asp:DropDownList>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="PaymentMethodIdRow">
                <asp:TableCell>Payment Method Id:</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="PaymentMethodId" runat="server"></asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="PaymentMethodTypeRow">
                <asp:TableCell>External Payment Method Type:</asp:TableCell>
                <asp:TableCell>
                    <asp:DropDownList ID="PaymentMethodType" runat="server">
                        <asp:ListItem Text="Check" Value="Check"></asp:ListItem>
                        <asp:ListItem Text="Other" Value="Other"></asp:ListItem>
                        <asp:ListItem Text="ACH" Value="ACH"></asp:ListItem>
                        <asp:ListItem Text="Cash" Value="Cash"></asp:ListItem>
                        <asp:ListItem Text="Credit Card" Value="Credit Card"></asp:ListItem>
                        <asp:ListItem Text="Debit Card" Value="Debit Card"></asp:ListItem>
                        <asp:ListItem Text="PayPal" Value="PayPal"></asp:ListItem>
                    </asp:DropDownList>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="RefundDateRow">
                <asp:TableCell>Refund Date ( i.e. 1/1/2012 ):</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="RefundDate" runat="server"></asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="NameRow">
                <asp:TableCell>Name:</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="Name" runat="server"></asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="ProductRatePlanIdRow">
                <asp:TableCell>Product RatePlan Id:</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="ProductRatePlanId" runat="server"></asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="DisableDateRow">
                <asp:TableCell>Disable Date:</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="DisableDate" runat="server"></asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="DescriptionRow">
                <asp:TableCell>Description: </asp:TableCell>
                <asp:TableCell><asp:TextBox ID="Description" runat="server"></asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="SKURow">
                <asp:TableCell>SKU: </asp:TableCell>
                <asp:TableCell><asp:TextBox ID="SKU" runat="server"></asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="ProductIdRow">
                <asp:TableCell>Product Id: </asp:TableCell>
                <asp:TableCell><asp:TextBox ID="ProductId" runat="server"></asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="PriceRow">
                <asp:TableCell>Price: </asp:TableCell>
                <asp:TableCell><asp:TextBox ID="Price" runat="server"></asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="ProductTypeRow">
                <asp:TableCell>Product Type:</asp:TableCell>
                <asp:TableCell>
                    <asp:DropDownList ID="ProductType" runat="server">
                        <asp:ListItem Text="Application" Value="Application"></asp:ListItem>
                        <asp:ListItem Text="iCredits" Value="iCredits"></asp:ListItem>
                        <asp:ListItem Text="Storage" Value="Storage"></asp:ListItem>
                    </asp:DropDownList>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="IdToDeleteRow">
                <asp:TableCell>Id to Delete: </asp:TableCell>
                <asp:TableCell><asp:TextBox ID="IdToDelete" runat="server"></asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="ProductRatePlanChargeIdRow">
                <asp:TableCell>Product Rate Plan Charge Id: </asp:TableCell>
                <asp:TableCell><asp:TextBox ID="ProductRatePlanChargeId" runat="server"></asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="SubscriptionDateRow">
                <asp:TableCell>Subscription Start Date: </asp:TableCell>
                <asp:TableCell><asp:TextBox ID="SubscriptionDate" runat="server">1/8/2013</asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="InitalTermRow">
                <asp:TableCell>Initial Term: </asp:TableCell>
                <asp:TableCell><asp:TextBox ID="InitialTerm" runat="server">12</asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="RenewalTermRow">
                <asp:TableCell>Renewal Term: </asp:TableCell>
                <asp:TableCell><asp:TextBox ID="RenewalTerm" runat="server">12</asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="ProductRatePlanNameRow">
                <asp:TableCell>Flat Fee, Product Rate Plan Name to subscribe: </asp:TableCell>
                <asp:TableCell><asp:TextBox ID="ProductRatePlanName" runat="server"></asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="SubscriptionTermTypeRow">
                <asp:TableCell>Subscription Term Type :</asp:TableCell>
                <asp:TableCell>
                    <asp:DropDownList ID="SubscriptionTermType" runat="server">
                        <asp:ListItem Text="Termed" Value="TERMED"></asp:ListItem>
                        <asp:ListItem Text="Evergreen" Value="EVERGREEN"></asp:ListItem>
                    </asp:DropDownList>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="SubscriptionIdRow">
                <asp:TableCell>SubscriptionId: </asp:TableCell>
                <asp:TableCell><asp:TextBox ID="SubscriptionId" runat="server"></asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="AmendmentDateRow">
                <asp:TableCell>Amendment Start Date: </asp:TableCell>
                <asp:TableCell><asp:TextBox ID="AmendmentStartDate" runat="server">1/8/2013</asp:TextBox></asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <asp:Button ID="Button1" runat="server" onclick="Login" Text="Login!" />
        <asp:Button ID="Button2" runat="server" onclick="DoAction" Text="Do Action!" />
        <br/>
        <asp:Label ID="result" runat="server"></asp:Label>
    </form>
</body>
</html>
