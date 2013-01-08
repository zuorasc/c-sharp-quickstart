using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zuora.Services
{
    public static class Constants
    {
        public static readonly DateTime DATETIME_FARFUTURE = new DateTime(2199, 1, 1);

        //http://knowledgecenter.zuora.com/D_Using_the_Zuora_API/C_API_Reference/C_API_Use_Cases_and_Examples/B_Working_with_the_Product_Catalog/Creating_a_Product_Catalog

        public const string CURRENCY_USD = "USD";

        public const string PRODUCT_RATE_PLAN_CHARGE_MODEL_FLAT_FEE = "FlatFee";
        public const string PRODUCT_RATE_PLAN_CHARGE_TYPE_ONE_TIME = "OneTime";
        public const string PRODUCT_RATE_PLAN_CHARGE_DEFAULT_NAME = "Default charge";

        // http://knowledgecenter.zuora.com/C_Zuora_User_Guides/A_Billing/A_Z-Billing/D_Product_Catalog/A_Product_Catalog_Concepts/Triggering_Conditions
        public const string PRODUCT_RATE_PLAN_CHARGE_TRIGGER_EVENT_CUSTOMER_ACCEPTANCE = "CustomerAcceptance";
        public const string PRODUCT_RATE_PLAN_CHARGE_TRIGGER_EVENT_CONTRACT_EFFECTIVE = "ContractEffective";
        public const string PRODUCT_RATE_PLAN_CHARGE_TRIGGER_EVENT_SERVICE_ACTIVATION = "ServiceActivation";

        public enum ProductType // Custom field for Product object
        {
            Undefined,
            Application,
            iCredits,
            Storage
        };

        public static DateTime ToZuoraTime(this DateTime dt)
        {
            return TimeZoneInfo.ConvertTime(dt, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
        }

    }
}
