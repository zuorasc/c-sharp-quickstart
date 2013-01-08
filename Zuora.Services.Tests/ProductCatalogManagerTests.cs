using System;
using System.Collections.Generic;
using Zuora.Services;
using Xunit;

namespace Zuora.Services.Tests
{
    public class ProductCatalogManagerTests : IDisposable
    {
        ProductCatalogManager pcm;
        ZuoraService zs;
        ZuoraTestHelper zth;
        public ProductCatalogManagerTests()
        {
            zs = new ZuoraService(Settings.API_USERNAME, Settings.API_PASSWORD, Settings.API_ENDPOINT);
            pcm = new ProductCatalogManager(zs, Settings.CATALOG_CACHE_PATH);
            zth = new ZuoraTestHelper();
        }

        [Fact]
        public void CanWeDisableAProductRatePlan()
        {
            DateTime today = DateTime.Now;
            String productRatePlanId = zth.CreateRatePlanToSubscribe();
            ResponseHolder updateRes = pcm.DisableRatePlan(productRatePlanId, today);

            Assert.True(updateRes.Success);
            
        }

        [Fact]
        public void CanWeGetAProductRatePlanByName()
        {
            List<ProductHolder> products = pcm.GetProductCatalog();
            String productRatePlanName = products[0].ProductRatePlans[0].ProductRatePlan.Name;
            ProductRatePlanHolder productRatePlan = pcm.GetProductRatePlanByName(productRatePlanName);
            Assert.Equal(productRatePlanName, productRatePlan.ProductRatePlan.Name);
        }

        [Fact]
        public void CanWeGetAProductByName()
        {
            List<ProductHolder> products = pcm.GetProductCatalog();
            String productName = products[0].Product.Name;
            ProductHolder product = pcm.GetProductByName(productName);
            Assert.Equal(productName, product.Product.Name);

        }

        [Fact]
        public void CanWeGetProductsFromLocalCache()
        {
            List<ProductHolder> products = pcm.GetProductCatalog();
            Assert.NotNull(products);
            Assert.True(products.Count > 0);
        }

        [Fact]
        public void CanWeGetSingleRatePlanFromTheCache()
        {
            List<ProductHolder> products = pcm.GetProductCatalog();
            ProductRatePlanHolder prph = new ProductRatePlanHolder();
            foreach (ProductHolder product in products)
            {
                for (int i = 0; i < product.ProductRatePlans.Count; i++)
                {
                    prph = pcm.GetRatePlan(product.ProductRatePlans[i].ProductRatePlan.Id);
                }
                if (prph != null)
                    break;
            }
            

            Assert.NotNull(prph);
            Assert.True(prph.ProductRatePlanCharges.Count > 0);
        }

        [Fact]
        public void CanWeGetTheProductCatalog()
        {
            List<ProductHolder> products = pcm.GetProductCatalog();
            Assert.NotNull(products);
            Assert.True(products.Count > 0);
        }

        [Fact]
        public void CanWeChangeThePriceForAChargeWithTiers()
        {
            //create a volume product
            String chargeId = zth.CreateTieredProductRatePlanChargeToUpdate();
            //update the prices
            List<ResponseHolder> updateRes = pcm.ChangePriceForProduct(chargeId, new Decimal[] { 2, 3 }, "USD");
            foreach (ResponseHolder rh in updateRes)
            {
                Assert.True(rh.Success);
            }
        }

        [Fact]
        public void CanWeChangeThePriceForAChargeWithNoTiers()
        {
            String chargeId = zth.CreateProductRatePlanChargeToUpdate();
            Decimal newPrice = (Decimal)12.0;
            List<ResponseHolder> updateRes = pcm.ChangePriceForProduct(chargeId, new Decimal[] { newPrice }, "USD");
            Assert.True(updateRes[0].Success);
        }

        public void Dispose()
        {
            ZuoraTestHelper zth = new ZuoraTestHelper();
            zth.CleanUp();
        }
    }
}
