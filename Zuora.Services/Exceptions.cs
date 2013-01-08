using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zuora.Services
{
    public class ZuoraException : ApplicationException
    {        
        public ZuoraException(string message)
            : base(message)
        {           
        }

        public ZuoraException(string message, int errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public int ErrorCode { get; set; }
        public string ZuoraErrorCode { get; set; }
        public string ZuoraErrorMessage { get; set; }
        public string CustomErrorMessage { get; set; }

        public ZuoraException WithZuoraError(ResponseHolder response)
        {
            if (!response.Success)
            {
                ZuoraErrorCode = response.ErrorCode;
                ZuoraErrorMessage = response.Message;
            }
            return this;
        }

        public ZuoraException WithCustomError(string message)
        {
            CustomErrorMessage = message;
            return this;
        }

        public static ZuoraException ProductPlanAlreadyExists(Product product, string name)
        {
            return new ZuoraConflictException(ErrorCodes.ToTuple(ErrorCodes.PRODUCT_CATALOG_PRODUCT_PLAN_ALREADY_EXISTS, name, product.Id, product.Name));
        }

        public static ZuoraException ProductAlreadyExists(string name)
        {
            return new ZuoraConflictException(ErrorCodes.ToTuple(ErrorCodes.PRODUCT_CATALOG_PRODUCT_ALREADY_EXISTS, name));
        }

        public static ZuoraException RatePlanCreationError(Product product, string name)
        {
            return new ZuoraUnexpectedErrorException(ErrorCodes.ToTuple(ErrorCodes.PRODUCT_CATALOG_RATE_PLAN_CREATION_ERROR, name, product.Id, product.Name));
        }
        public static ZuoraException ProductCreationError(string name, string appId)
        {
            return new ZuoraUnexpectedErrorException(ErrorCodes.ToTuple(ErrorCodes.PRODUCT_CATALOG_PRODUCT_CREATION_ERROR, name, appId));
        }

    }

    public static class ErrorCodes
    {
        private static readonly Dictionary<int, string> CodeToDescription;

        public const int PRODUCT_CATALOG_PRODUCT_PLAN_ALREADY_EXISTS = 1001;
        public const int PRODUCT_CATALOG_RATE_PLAN_CREATION_ERROR = 1002;
        public const int PRODUCT_CATALOG_PRODUCT_ALREADY_EXISTS = 1003;
        public const int PRODUCT_CATALOG_PRODUCT_CREATION_ERROR = 1004;

        static ErrorCodes()
        {
            CodeToDescription = new Dictionary<int, string>();
            CodeToDescription.Add(PRODUCT_CATALOG_PRODUCT_PLAN_ALREADY_EXISTS, "Product plan '{0}' already exists for product (Id: '{1}' Name: '{2}')");
            CodeToDescription.Add(PRODUCT_CATALOG_RATE_PLAN_CREATION_ERROR, "Failed to create rate plan name '{0}' for product (Id: '{1}' Name: '{2}')");
            CodeToDescription.Add(PRODUCT_CATALOG_PRODUCT_ALREADY_EXISTS, "Product with name '{0}' already exists");
            CodeToDescription.Add(PRODUCT_CATALOG_PRODUCT_CREATION_ERROR, "Failed to create product (name: '{0}' appId: '{1}')");
        }

        public static string ToString(int errorCode, params string[] parameters)
        {
            string errorString = null;
            if (CodeToDescription.TryGetValue(errorCode, out errorString))
            {
                return string.Format(errorString, parameters);
            }
            else
            {
                return "MISSING ERROR DESCRIPTION";
            }
        }

        public static Tuple<int, string> ToTuple(int errorCode, params string[] parameters)
        {
            var s = ToString(errorCode, parameters);
            return new Tuple<int, string>(errorCode, s);
        }            
    }

    public class ZuoraConflictException : ZuoraException
    {

        public ZuoraConflictException(string message)
            : base(message)
        {
        }

        public ZuoraConflictException(string message, int errorCode)
            : base(message, errorCode)
        {
        }

        public ZuoraConflictException(Tuple<int, string> errorCodeAndMessage)
            : base(errorCodeAndMessage.Item2, errorCodeAndMessage.Item1)
        {
        }
    }
    public class ZuoraUnexpectedErrorException : ZuoraException
    {

        public ZuoraUnexpectedErrorException(string message)
            : base(message)
        {
        }

        public ZuoraUnexpectedErrorException(string message, int errorCode)
            : base(message, errorCode)
        {
        }

        public ZuoraUnexpectedErrorException(Tuple<int, string> errorCodeAndMessage)
            : base(errorCodeAndMessage.Item2, errorCodeAndMessage.Item1)
        {
        }
    }
}
