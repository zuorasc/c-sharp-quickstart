using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Zuora.Services
{
    public class ProductCatalogManager
    {
        ZuoraService zs;
        private string cachePath;

        public ProductCatalogManager(ZuoraService zs, string cachePath)
        {
            this.zs = zs;
            this.cachePath = cachePath;
        }

        //Get product rate plan by name
        public ProductRatePlanHolder GetProductRatePlanByName(String name)
        {
            // NOTE (maxm): this may not work as is as there's no uniqueness requirement for product rate plan names

            List<ProductHolder> products = ReadCache();

            foreach (ProductHolder prod in products)
            {
                foreach (ProductRatePlanHolder prh in prod.ProductRatePlans)
                {
                    if (name == prh.ProductRatePlan.Name)
                    {
                        return prh;
                    }
                }
            }
            return null;
        
        }

        public ResponseHolder DisableRatePlan(String productRatePlanId, DateTime date)
        {
            ProductRatePlan prp = new ProductRatePlan();
            prp.Id = productRatePlanId;
            prp.EffectiveEndDateSpecified = true;
            prp.EffectiveEndDate = date;

            return zs.Update(new List<zObject> { prp })[0];
        }

        public ProductRatePlanHolder GetProductRatePlanByName(string productId, string name)
        {
            var products = ReadCache();
            var product = products.FirstOrDefault(p => p.Product.Id == productId);
            return product == null ? null : product.ProductRatePlans.FirstOrDefault(prp => prp.ProductRatePlan.Name == name);
        }

        public ProductHolder GetProductById(string productId)
            {
            var products = ReadCache();
            return products.FirstOrDefault(p => p.Product.Id == productId);
        }

        //Get a product by name from the cache
        public ProductHolder GetProductByName(String name)
                {
            List<ProductHolder> products = ReadCache();
            return products.FirstOrDefault(p => p.Product.Name == name);
                }


        //Get the product catalog from Zuora
        public List<ProductHolder> GetProductCatalog()
        {
            DateTime time = DateTime.Now.ToZuoraTime().Date;
            string format = "yyyy-MM-ddTHH:mm:ss";
            string curDate = time.ToString(format);

            String productQueryString = "SELECT Id, Name, Description FROM Product WHERE EffectiveStartDate <= " + curDate + " AND EffectiveEndDate >" + curDate;
            ResponseHolder productQRes = zs.Query(productQueryString);
            String productRatePlanQueryString = "SELECT Id, Name, Description, ProductId FROM ProductRatePlan WHERE EffectiveStartDate <= " + curDate + " AND EffectiveEndDate >" + curDate;
            ResponseHolder productRatePlanQRes = zs.Query(productRatePlanQueryString);
            String productRatePlanChargeQueryString = "SELECT Id, Name, ProductRatePlanId, ChargeModel, ChargeType FROM ProductRatePlanCharge";
            ResponseHolder productRatePlanChargeQRes = zs.Query(productRatePlanChargeQueryString);
            String productRatePlanChargeTierQueryString = "SELECT Id, Price, ProductRatePlanChargeId FROM ProductRatePlanChargeTier";
            ResponseHolder productRatePlanChargeTierQRes = zs.Query(productRatePlanChargeTierQueryString);

            List<ProductHolder> products = new List<ProductHolder>();
            Dictionary<String, List<ProductRatePlan>> productRatePlans = new Dictionary<String, List<ProductRatePlan>>();
            Dictionary<String, List<ProductRatePlanCharge>> productRatePlanCharges = new Dictionary<String, List<ProductRatePlanCharge>>();
            Dictionary<String, List<ProductRatePlanChargeTier>> productRatePlanChargeTiers = new Dictionary<String, List<ProductRatePlanChargeTier>>();

            foreach (zObject t in productRatePlanChargeTierQRes.Objects)
            {
                ProductRatePlanChargeTier prpct = (ProductRatePlanChargeTier)t;
                List<ProductRatePlanChargeTier> tempList;
                productRatePlanChargeTiers.TryGetValue(prpct.ProductRatePlanChargeId, out tempList);
                if(tempList == null)
                {
                    productRatePlanChargeTiers.Add(prpct.ProductRatePlanChargeId, new List<ProductRatePlanChargeTier>{prpct});
                }
                else{
                    tempList.Add(prpct);
                    productRatePlanChargeTiers.Remove(prpct.ProductRatePlanChargeId);
                    productRatePlanChargeTiers.Add(prpct.ProductRatePlanChargeId, tempList);
                }
            }

            foreach(zObject c in productRatePlanChargeQRes.Objects)
            {
                ProductRatePlanCharge prpc = (ProductRatePlanCharge)c;
                List<ProductRatePlanCharge> tempList;
                productRatePlanCharges.TryGetValue(prpc.ProductRatePlanId, out tempList);
                if(tempList == null)
                {
                    productRatePlanCharges.Add(prpc.ProductRatePlanId, new List<ProductRatePlanCharge>{prpc});
                }
                else{
                    tempList.Add(prpc);
                    productRatePlanCharges.Remove(prpc.ProductRatePlanId);
                    productRatePlanCharges.Add(prpc.ProductRatePlanId, tempList);
                }
            }

            foreach(zObject r in productRatePlanQRes.Objects)
            {
                ProductRatePlan prp = (ProductRatePlan)r;
                List<ProductRatePlan> tempList;
                productRatePlans.TryGetValue(prp.ProductId, out tempList);
                if(tempList == null)
                {
                    productRatePlans.Add(prp.ProductId, new List<ProductRatePlan>{prp});
                }
                else{
                    tempList.Add(prp);
                    productRatePlans.Remove(prp.ProductId);
                    productRatePlans.Add(prp.ProductId, tempList);
                }
            }

            foreach (zObject z in productQRes.Objects)
            {
                Product prod = (Product) z;
                ProductHolder prodHolder = new ProductHolder();
                prodHolder.Product = prod;
                //get the rate plans for each product
                List<ProductRatePlan> ratePlanList = new List<ProductRatePlan>();
                List<ProductRatePlanHolder> ratePlanHolders = new List<ProductRatePlanHolder>();
                if (productRatePlans.TryGetValue(prod.Id, out ratePlanList))
                {
                    foreach (ProductRatePlan prp in ratePlanList)
                {
                    ProductRatePlanHolder prph = new ProductRatePlanHolder();
                    prph.ProductRatePlan = prp;
                    //get the charges for each rate plan
                    List<ProductRatePlanCharge> prpcList = new List<ProductRatePlanCharge>();
                    List<ProductRatePlanChargeHolder> chargeHolders = new List<ProductRatePlanChargeHolder>();
                        if (productRatePlanCharges.TryGetValue(prp.Id, out prpcList))
                        {
                            foreach (ProductRatePlanCharge prpc in prpcList)
                            {
                        ProductRatePlanChargeHolder prpch = new ProductRatePlanChargeHolder();
                        prpch.ProductRatePlanCharge = prpc;
                        //get the tiers for each charge
                        List<ProductRatePlanChargeTier> prpctList = new List<ProductRatePlanChargeTier>();
                        productRatePlanChargeTiers.TryGetValue(prpc.Id, out prpctList);
                        prpch.ProductRatePlanChargeTiers = prpctList;
                        chargeHolders.Add(prpch);
                    }
                        }

                    prph.ProductRatePlanCharges = chargeHolders;
                    ratePlanHolders.Add(prph);
                }
                }
                prodHolder.ProductRatePlans = ratePlanHolders;
                products.Add(prodHolder);

            }
            var productsJSON = new JavaScriptSerializer().Serialize(products);

            string myFile = cachePath;
            System.IO.StreamWriter fh = new System.IO.StreamWriter(myFile);
            fh.WriteLine(productsJSON);
            fh.Close();

            return ReadCache();
        }

        private List<ProductHolder> ReadCache()
        {
            if (!System.IO.File.Exists(cachePath))
            {
                return GetProductCatalog();
            }

            string myFile = cachePath;
            System.IO.StreamReader fh = new System.IO.StreamReader(myFile);
            string catalogJSon = fh.ReadToEnd();
            fh.Close();

            List<ProductHolder> products = (List<ProductHolder>)new JavaScriptSerializer().Deserialize<List<ProductHolder>>(catalogJSon);

            return products;
        }

        private void ClearCache()
        {
            if (System.IO.File.Exists(cachePath))
            {
                try
                {
                    System.IO.File.Delete(cachePath);
                }
                catch
                {
                }

            }
        }

        public ProductRatePlanHolder GetRatePlan(string ratePlanId)
        {
            var products = ReadCache();
            
            return (from p in products
                    from prp in p.ProductRatePlans
                    where prp.ProductRatePlan.Id == ratePlanId
                    select prp).FirstOrDefault();

        }

        public ProductHolder CreateProduct(string name, Constants.ProductType productType, string sku = null, string description = null, bool dupeNameCheck = true)
        {
            // NOTE (maxm): Zuora seems to allow duplicate product names. This is attempting to avoid duplicates

            if (dupeNameCheck)
            {
                var product = GetProductByName(name); // TODO (maxm): need to replace this with non cached call
                if (product != null)
                {
                    throw ZuoraException.ProductAlreadyExists(name);
                }
            }
            var r = zs.Create(new Product()
                                  {
                                      Name = name,
                                      SKU = sku,
                                      Description = description,
                                      EffectiveStartDate = DateTime.Now.ToZuoraTime(),
                                      EffectiveEndDate = Constants.DATETIME_FARFUTURE,
                                      ProductType__c = productType.ToString(),
                                      EffectiveStartDateSpecified = true, // BUGBUGBUG (maxm): shouldn't need to be included
                                      EffectiveEndDateSpecified = true, // BUGBUGBUG (maxm): shouldn't need to be included
                                  });
            if (!r.Success)
            {
                throw ZuoraException.ProductCreationError(name, sku).WithZuoraError(r);
            }
            ClearCache();
            var q = zs.Query(string.Format("SELECT Id, Name, Description, EffectiveStartDate, EffectiveEndDate FROM Product WHERE Id = '{0}' ", r.Id));
            var p = GetProductById(r.Id);
            if (p == null)
            {
                throw ZuoraException.ProductCreationError(name, sku).WithCustomError("Couldn't retrieve created product: " + r.Id);
            }
            return p;
        }

        public ProductRatePlanHolder CreateRatePlanWithOneTimeCharge(Product product, string ratePlanName, decimal price)
        {
            var existingRatePlan = GetProductRatePlanByName(product.Id, ratePlanName);
            if (existingRatePlan != null)
            {
                throw ZuoraException.ProductPlanAlreadyExists(product, ratePlanName);
            }
            var prp = new ProductRatePlan()
                          {
                              ProductId = product.Id,
                              Name = ratePlanName,
                              EffectiveStartDate = DateTime.Now
                          };
            var r = zs.Create(prp);
            if (!r.Success)
        {
                throw ZuoraException.RatePlanCreationError(product, ratePlanName).WithZuoraError(r);
            }
            var prpId = r.Id;

            var prpc = new ProductRatePlanCharge()
                           {
                               ProductRatePlanId = prpId,
                               ChargeModel = Constants.PRODUCT_RATE_PLAN_CHARGE_MODEL_FLAT_FEE,
                               ChargeType = Constants.PRODUCT_RATE_PLAN_CHARGE_TYPE_ONE_TIME,
                               Name = Constants.PRODUCT_RATE_PLAN_CHARGE_DEFAULT_NAME,
                               TriggerEvent = Constants.PRODUCT_RATE_PLAN_CHARGE_TRIGGER_EVENT_CONTRACT_EFFECTIVE,
                               ProductRatePlanChargeTierData = new ProductRatePlanChargeTier[]
            {
                                                                       new ProductRatePlanChargeTier()
                {
                                                                               Active = true,
                                                                               Currency = Constants.CURRENCY_USD,
                                                                               Price = price,
                                                                               PriceSpecified = true, // BUGBUGBUG (maxm): shouldn't need to be included
                                                                           }
                                                                   }
                           };

            r = zs.Create(prpc);
            if (!r.Success)
                    {
                DeleteRatePlan(prpId);
                throw ZuoraException.RatePlanCreationError(product, ratePlanName).WithZuoraError(r);
                    }
            var prpcId = r.Id;
            ClearCache();
            return GetRatePlan(prpId);
                }

        public void DeleteRatePlan(string ratePlanId)
        {
            zs.Delete(new ProductRatePlan() {Id = ratePlanId});
            ClearCache();
            }

        public void DeleteProduct(string productId)
        {
            zs.Delete(new Product() { Id = productId });
            ClearCache();
        }

        //Change the price of a charge
        public List<ResponseHolder> ChangePriceForProduct(String productRatePlanChargeId, Decimal[] newPrices, String currency)
        {
            String chargeQueryString = "SELECT id, ChargeModel FROM ProductRatePlanCharge WHERE Id ='" + productRatePlanChargeId + "'";
            ResponseHolder cQRes = zs.Query(chargeQueryString);
            ProductRatePlanCharge prpc = (ProductRatePlanCharge)cQRes.Objects[0];
            String tierQueryString = "SELECT id, price, productrateplanchargeid FROM ProductRatePlanChargeTier WHERE ProductRatePlanChargeId = '" + productRatePlanChargeId + "' AND Active = true AND Currency = '" + currency + "'";
            ResponseHolder tierqRes = zs.Query(tierQueryString);
            
            if (newPrices.Length == tierqRes.Objects.Count)
            {
                List<zObject> prpctList = new List<zObject>();
                for (int i = 0; i < tierqRes.Objects.Count; i++)
                {
                    ProductRatePlanChargeTier temp = (ProductRatePlanChargeTier)tierqRes.Objects[i];
                    temp.Price = newPrices[i];
                    prpctList.Add(temp);
                }
                return zs.Update(prpctList);
            }
            else
            {
                ResponseHolder error  = new ResponseHolder();
                error.Success = false;
                error.Message = "New Prices Count does not match # of tieres in Zuora";
                return new List<ResponseHolder> { error };
            }
        }
    }
}
