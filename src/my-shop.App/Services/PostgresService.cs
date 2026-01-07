using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using my_shop.App.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace my_shop.App.Services
{
    public class PostgresService : IDataService
    {
        private readonly GraphQLHttpClient _graphQLClient;
        
        public PostgresService()
        {
            // GraphQL API endpoint
            var endpoint = "http://localhost:5000/graphql";
            
            // Configure serializer with camelCase settings
            var serializerSettings = new Newtonsoft.Json.JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };
            
            _graphQLClient = new GraphQLHttpClient(endpoint, new NewtonsoftJsonSerializer(serializerSettings));
        }
        
        public async Task<DashboardStats> GetDashboardStatsAsync()
        {
            try
            {
                var query = new GraphQLRequest
                {
                    Query = @"
                        query {
                            dashboardStats {
                                totalProducts
                                todayOrders
                                todayRevenue
                                lowStockCount
                            }
                        }"
                };

                var response = await _graphQLClient.SendQueryAsync<DashboardStatsResponse>(query);
                
                if (response.Errors != null && response.Errors.Length > 0)
                {
                    return GetDefaultDashboardStats();
                }

                if (response.Data == null || response.Data.DashboardStats == null)
                {
                    return GetDefaultDashboardStats();
                }

                return response.Data.DashboardStats;
            }
            catch (Exception)
            {
                return GetDefaultDashboardStats();
            }
        }

        public async Task<List<RecentOrderDto>> GetRecentOrdersAsync(int count = 3)
        {
            try
            {
                var query = new GraphQLRequest
                {
                    Query = @"
                        query($count: Int!) {
                            recentOrders(count: $count) {
                                orderId
                                createdTime
                                itemCount
                                totalPrice
                            }
                        }",
                    Variables = new { count }
                };

                var response = await _graphQLClient.SendQueryAsync<RecentOrdersResponse>(query);
                
                if (response.Errors != null && response.Errors.Length > 0)
                {
                    return new List<RecentOrderDto>();
                }

                return response.Data?.RecentOrders ?? new List<RecentOrderDto>();
            }
            catch (Exception)
            {
                return new List<RecentOrderDto>();
            }
        }

        public async Task<List<LowStockProductDto>> GetLowStockProductsAsync(int threshold = 5, int count = 5)
        {
            try
            {
                var query = new GraphQLRequest
                {
                    Query = @"
                        query($threshold: Int!, $count: Int!) {
                            lowStockProducts(threshold: $threshold, count: $count) {
                                name
                                sku
                                count
                            }
                        }",
                    Variables = new { threshold, count }
                };

                var response = await _graphQLClient.SendQueryAsync<LowStockProductsResponse>(query);
                
                if (response.Errors != null && response.Errors.Length > 0)
                {
                    return new List<LowStockProductDto>();
                }

                return response.Data?.LowStockProducts ?? new List<LowStockProductDto>();
            }
            catch (Exception)
            {
                return new List<LowStockProductDto>();
            }
        }

        public async Task<List<BestSellingProductDto>> GetBestSellingProductsAsync(int count = 5)
        {
            try
            {
                var query = new GraphQLRequest
                {
                    Query = @"
                        query($count: Int!) {
                            bestSellingProducts(count: $count) {
                                name
                                category
                                soldCount
                                soldPercentage
                            }
                        }",
                    Variables = new { count }
                };

                var response = await _graphQLClient.SendQueryAsync<BestSellingProductsResponse>(query);
                
                if (response.Errors != null && response.Errors.Length > 0)
                {
                    return new List<BestSellingProductDto>();
                }

                return response.Data?.BestSellingProducts ?? new List<BestSellingProductDto>();
            }
            catch (Exception)
            {
                return new List<BestSellingProductDto>();
            }
        }

        public async Task<List<ProductModel>> GetProductsAsync(string? searchTerm = null, int? categoryId = null)
        {
            try
            {
                // Build query dynamically based on filters
                string queryString;
                object variables;

                if (string.IsNullOrWhiteSpace(searchTerm) && categoryId == null)
                {
                    // No filters - get all products
                    queryString = @"
                        query {
                            products {
                                id
                                sku
                                name
                                importPrice
                                count
                                description
                                categoryId
                                category {
                                    name
                                }
                                images
                            }
                        }";
                    variables = new { };
                }
                else if (!string.IsNullOrWhiteSpace(searchTerm) && categoryId == null)
                {
                    // Only search term filter
                    queryString = @"
                        query($searchTerm: String) {
                            products(where: { 
                                or: [
                                    { name: { contains: $searchTerm } },
                                    { sku: { contains: $searchTerm } }
                                ]
                            }) {
                                id
                                sku
                                name
                                importPrice
                                count
                                description
                                categoryId
                                category {
                                    name
                                }
                                images
                            }
                        }";
                    variables = new { searchTerm };
                }
                else if (string.IsNullOrWhiteSpace(searchTerm) && categoryId != null)
                {
                    // Only category filter
                    queryString = @"
                        query($categoryId: Int) {
                            products(where: { 
                                categoryId: { eq: $categoryId }
                            }) {
                                id
                                sku
                                name
                                importPrice
                                count
                                description
                                categoryId
                                category {
                                    name
                                }
                                images
                            }
                        }";
                    variables = new { categoryId };
                }
                else
                {
                    // Both filters
                    queryString = @"
                        query($searchTerm: String, $categoryId: Int) {
                            products(where: { 
                                and: [
                                    {
                                        or: [
                                            { name: { contains: $searchTerm } },
                                            { sku: { contains: $searchTerm } }
                                        ]
                                    },
                                    { categoryId: { eq: $categoryId } }
                                ]
                            }) {
                                id
                                sku
                                name
                                importPrice
                                count
                                description
                                categoryId
                                category {
                                    name
                                }
                                images
                            }
                        }";
                    variables = new { searchTerm, categoryId };
                }

                var query = new GraphQLRequest
                {
                    Query = queryString,
                    Variables = variables
                };

                var response = await _graphQLClient.SendQueryAsync<ProductsResponse>(query);
                
                if (response.Errors != null && response.Errors.Length > 0)
                {
                    return new List<ProductModel>();
                }

                var products = response.Data?.Products ?? new List<ProductResponse>();
                return products.Select(p => new ProductModel
                {
                    Id = p.Id,
                    Sku = p.Sku,
                    Name = p.Name,
                    ImportPrice = p.ImportPrice,
                    Count = p.Count,
                    Description = p.Description,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category?.Name,
                    Images = p.Images
                }).ToList();
            }
            catch (Exception)
            {
                return new List<ProductModel>();
            }
        }

        public async Task<ProductModel?> GetProductByIdAsync(int id)
        {
            try
            {
                var query = new GraphQLRequest
                {
                    Query = @"
                        query($id: Int!) {
                            products(where: { id: { eq: $id } }) {
                                id
                                sku
                                name
                                importPrice
                                count
                                description
                                categoryId
                                category {
                                    name
                                }
                                images
                            }
                        }",
                    Variables = new { id }
                };

                var response = await _graphQLClient.SendQueryAsync<ProductsResponse>(query);
                
                if (response.Errors != null && response.Errors.Length > 0)
                {
                    return null;
                }

                var product = response.Data?.Products?.FirstOrDefault();
                if (product == null)
                    return null;

                return new ProductModel
                {
                    Id = product.Id,
                    Sku = product.Sku,
                    Name = product.Name,
                    ImportPrice = product.ImportPrice,
                    Count = product.Count,
                    Description = product.Description,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category?.Name,
                    Images = product.Images
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ProductModel> CreateProductAsync(CreateProductModel model)
        {
            try
            {
                var mutation = new GraphQLRequest
                {
                    Query = @"
                        mutation($input: CreateProductDtoInput!) {
                            createProduct(input: $input) {
                                id
                                sku
                                name
                                importPrice
                                count
                                description
                                categoryId
                                images
                            }
                        }",
                    Variables = new { input = model }
                };

                var response = await _graphQLClient.SendMutationAsync<CreateProductResponse>(mutation);
                
                if (response.Errors != null && response.Errors.Length > 0)
                {
                    throw new Exception(response.Errors[0].Message);
                }

                var product = response.Data?.CreateProduct;
                if (product == null)
                    throw new Exception("Failed to create product");

                return new ProductModel
                {
                    Id = product.Id,
                    Sku = product.Sku,
                    Name = product.Name,
                    ImportPrice = product.ImportPrice,
                    Count = product.Count,
                    Description = product.Description,
                    CategoryId = product.CategoryId,
                    Images = product.Images
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Create product failed: {ex.Message}");
            }
        }

        public async Task<ProductModel> UpdateProductAsync(UpdateProductModel model)
        {
            try
            {
                var mutation = new GraphQLRequest
                {
                    Query = @"
                        mutation($input: UpdateProductDtoInput!) {
                            updateProduct(input: $input) {
                                id
                                sku
                                name
                                importPrice
                                count
                                description
                                categoryId
                                images
                            }
                        }",
                    Variables = new { input = model }
                };

                var response = await _graphQLClient.SendMutationAsync<UpdateProductResponse>(mutation);
                
                if (response.Errors != null && response.Errors.Length > 0)
                {
                    throw new Exception(response.Errors[0].Message);
                }

                var product = response.Data?.UpdateProduct;
                if (product == null)
                    throw new Exception("Failed to update product");

                return new ProductModel
                {
                    Id = product.Id,
                    Sku = product.Sku,
                    Name = product.Name,
                    ImportPrice = product.ImportPrice,
                    Count = product.Count,
                    Description = product.Description,
                    CategoryId = product.CategoryId,
                    Images = product.Images
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Update product failed: {ex.Message}");
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                var mutation = new GraphQLRequest
                {
                    Query = @"
                        mutation($id: Int!) {
                            deleteProduct(id: $id)
                        }",
                    Variables = new { id }
                };

                var response = await _graphQLClient.SendMutationAsync<DeleteProductResponse>(mutation);
                
                if (response.Errors != null && response.Errors.Length > 0)
                {
                    return false;
                }

                return response.Data?.DeleteProduct ?? false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<CategoryModel>> GetCategoriesAsync()
        {
            try
            {
                var query = new GraphQLRequest
                {
                    Query = @"
                        query {
                            categories {
                                id
                                name
                                description
                                products {
                                    id
                                }
                            }
                        }"
                };

                var response = await _graphQLClient.SendQueryAsync<CategoriesResponse>(query);
                
                if (response.Errors != null && response.Errors.Length > 0)
                {
                    return new List<CategoryModel>();
                }

                var categories = response.Data?.Categories ?? new List<CategoryResponse>();
                return categories.Select(c => new CategoryModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ProductCount = c.Products?.Count ?? 0
                }).ToList();
            }
            catch (Exception)
            {
                return new List<CategoryModel>();
            }
        }

        public async Task<OrderModel> CreateOrderAsync(CreateOrderModel model)
        {
            try
            {
                var mutation = new GraphQLRequest
                {
                    Query = @"
                        mutation($customerId: Int, $customerName: String, $customerPhone: String, $items: [OrderItemInput!]!, $finalPrice: Int!) {
                            createOrder(
                                customerId: $customerId, 
                                customerName: $customerName,
                                customerPhone: $customerPhone,
                                items: $items, 
                                finalPrice: $finalPrice
                            ) {
                                id
                                customerId
                                createdTime
                                finalPrice
                            }
                        }",
                    Variables = new 
                    { 
                        customerId = model.CustomerId,
                        customerName = model.CustomerName,
                        customerPhone = model.CustomerPhone,
                        items = model.Items.Select(i => new 
                        {
                            productId = i.ProductId,
                            quantity = i.Quantity,
                            price = i.Price
                        }).ToList(),
                        finalPrice = model.FinalPrice
                    }
                };

                var response = await _graphQLClient.SendMutationAsync<CreateOrderResponse>(mutation);
                
                if (response.Errors != null && response.Errors.Length > 0)
                {
                    throw new Exception(response.Errors[0].Message);
                }

                var order = response.Data?.CreateOrder;
                if (order == null)
                    throw new Exception("Failed to create order");

                return new OrderModel
                {
                    Id = order.Id,
                    CustomerId = order.CustomerId,
                    CreatedTime = order.CreatedTime,
                    FinalPrice = order.FinalPrice
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Create order failed: {ex.Message}");
            }
        }

        public async Task<List<CustomerModel>> GetCustomersAsync()
        {
            try
            {
                var query = new GraphQLRequest
                {
                    Query = @"
                        query {
                            customers(where: { isDeleted: { eq: false } }) {
                                id
                                name
                                phone
                                isDeleted
                                createdAt
                            }
                        }"
                };

                var response = await _graphQLClient.SendQueryAsync<CustomersResponse>(query);
                
                if (response.Errors != null && response.Errors.Length > 0)
                {
                    return new List<CustomerModel>();
                }

                var customers = response.Data?.Customers ?? new List<CustomerResponse>();
                return customers.Select(c => new CustomerModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Phone = c.Phone,
                    IsDeleted = c.IsDeleted,
                    CreatedAt = c.CreatedAt
                }).ToList();
            }
            catch (Exception)
            {
                return new List<CustomerModel>();
            }
        }

        public async Task<CustomerModel?> GetCustomerByIdAsync(int id)
        {
            try
            {
                var query = new GraphQLRequest
                {
                    Query = @"
                        query($id: Int!) {
                            customers(where: { id: { eq: $id } }) {
                                id
                                name
                                phone
                                isDeleted
                            }
                        }",
                    Variables = new { id }
                };

                var response = await _graphQLClient.SendQueryAsync<CustomersResponse>(query);
                
                if (response.Errors != null && response.Errors.Length > 0)
                {
                    return null;
                }

                var customer = response.Data?.Customers?.FirstOrDefault();
                if (customer == null)
                    return null;

                return new CustomerModel
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Phone = customer.Phone,
                    IsDeleted = customer.IsDeleted
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<CustomerModel> CreateCustomerAsync(string name, string phone)
        {
            try
            {
                var mutation = new GraphQLRequest
                {
                    Query = @"
                        mutation($name: String!, $phone: String!) {
                            createCustomer(name: $name, phone: $phone) {
                                id
                                name
                                phone
                                isDeleted
                                createdAt
                            }
                        }",
                    Variables = new { name, phone }
                };

                var response = await _graphQLClient.SendMutationAsync<CreateCustomerResponse>(mutation);
                
                if (response.Errors != null && response.Errors.Length > 0)
                {
                    throw new Exception(response.Errors[0].Message);
                }

                var customer = response.Data?.CreateCustomer;
                if (customer == null)
                    throw new Exception("Failed to create customer");

                return new CustomerModel
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Phone = customer.Phone,
                    IsDeleted = customer.IsDeleted,
                    CreatedAt = customer.CreatedAt
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Create customer failed: {ex.Message}");
            }
        }

        public async Task<CustomerModel> UpdateCustomerAsync(int id, string name, string phone)
        {
            try
            {
                var mutation = new GraphQLRequest
                {
                    Query = @"
                        mutation($id: Int!, $name: String!, $phone: String!) {
                            updateCustomer(id: $id, name: $name, phone: $phone) {
                                id
                                name
                                phone
                                isDeleted
                                createdAt
                            }
                        }",
                    Variables = new { id, name, phone }
                };

                var response = await _graphQLClient.SendMutationAsync<UpdateCustomerResponse>(mutation);
                
                if (response.Errors != null && response.Errors.Length > 0)
                {
                    throw new Exception(response.Errors[0].Message);
                }

                var customer = response.Data?.UpdateCustomer;
                if (customer == null)
                    throw new Exception("Failed to update customer");

                return new CustomerModel
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Phone = customer.Phone,
                    IsDeleted = customer.IsDeleted,
                    CreatedAt = customer.CreatedAt
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Update customer failed: {ex.Message}");
            }
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            try
            {
                var mutation = new GraphQLRequest
                {
                    Query = @"
                        mutation($id: Int!) {
                            deleteCustomer(id: $id)
                        }",
                    Variables = new { id }
                };

                var response = await _graphQLClient.SendMutationAsync<DeleteCustomerResponse>(mutation);
                
                if (response.Errors != null && response.Errors.Length > 0)
                {
                    return false;
                }

                return response.Data?.DeleteCustomer ?? false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<ProductSalesReport>> GetProductSalesReportAsync(DateTime startDate, DateTime endDate, ReportPeriodType periodType, int topProducts = 10)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== GetProductSalesReportAsync Called ===");
                System.Diagnostics.Debug.WriteLine($"StartDate: {startDate:yyyy-MM-dd}");
                System.Diagnostics.Debug.WriteLine($"EndDate: {endDate:yyyy-MM-dd}");
                System.Diagnostics.Debug.WriteLine($"PeriodType: {periodType}");
                
                // Ensure dates are treated as UTC
                var startDateUtc = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
                var endDateUtc = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);
                
                var query = new GraphQLRequest
                {
                    Query = @"
                        query($startDate: DateTime!, $endDate: DateTime!, $periodType: ReportPeriodType!, $topProducts: Int!) {
                            productSalesReport(startDate: $startDate, endDate: $endDate, periodType: $periodType, topProducts: $topProducts) {
                                productId
                                productName
                                sku
                                totalQuantitySold
                                totalRevenue
                                dataPoints {
                                    period
                                    date
                                    quantitySold
                                }
                            }
                        }",
                    Variables = new { 
                        startDate = startDateUtc.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        endDate = endDateUtc.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        periodType = periodType.ToString().ToUpper(),
                        topProducts 
                    }
                };

                System.Diagnostics.Debug.WriteLine($"Sending GraphQL query to API...");
                var response = await _graphQLClient.SendQueryAsync<ProductSalesReportResponse>(query);
                
                if (response.Errors != null && response.Errors.Length > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"!!! GraphQL Errors:");
                    foreach (var error in response.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"  - {error.Message}");
                    }
                    return new List<ProductSalesReport>();
                }

                var result = response.Data?.ProductSalesReport ?? new List<ProductSalesReport>();
                System.Diagnostics.Debug.WriteLine($"Success! Got {result.Count} product sales records");
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"!!! EXCEPTION in GetProductSalesReportAsync:");
                System.Diagnostics.Debug.WriteLine($"  Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"  Type: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"  Stack: {ex.StackTrace}");
                return new List<ProductSalesReport>();
            }
        }

        public async Task<RevenueProfitReport> GetRevenueProfitReportAsync(DateTime startDate, DateTime endDate, ReportPeriodType periodType)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== GetRevenueProfitReportAsync Called ===");
                System.Diagnostics.Debug.WriteLine($"StartDate: {startDate:yyyy-MM-dd}");
                System.Diagnostics.Debug.WriteLine($"EndDate: {endDate:yyyy-MM-dd}");
                System.Diagnostics.Debug.WriteLine($"PeriodType: {periodType}");
                
                // Ensure dates are treated as UTC
                var startDateUtc = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
                var endDateUtc = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);
                
                var query = new GraphQLRequest
                {
                    Query = @"
                        query($startDate: DateTime!, $endDate: DateTime!, $periodType: ReportPeriodType!) {
                            revenueProfitReport(startDate: $startDate, endDate: $endDate, periodType: $periodType) {
                                totalRevenue
                                totalProfit
                                totalCost
                                totalOrders
                                averageOrderValue
                                profitMargin
                                dataPoints {
                                    period
                                    date
                                    revenue
                                    profit
                                    cost
                                    orderCount
                                }
                            }
                        }",
                    Variables = new { 
                        startDate = startDateUtc.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        endDate = endDateUtc.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        periodType = periodType.ToString().ToUpper()
                    }
                };

                System.Diagnostics.Debug.WriteLine($"Sending GraphQL query to API...");
                var response = await _graphQLClient.SendQueryAsync<RevenueProfitReportResponse>(query);
                
                if (response.Errors != null && response.Errors.Length > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"!!! GraphQL Errors:");
                    foreach (var error in response.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"  - {error.Message}");
                    }
                    return GetDefaultRevenueProfitReport();
                }

                var result = response.Data?.RevenueProfitReport ?? GetDefaultRevenueProfitReport();
                System.Diagnostics.Debug.WriteLine($"Success! Total Revenue: {result.TotalRevenue}, Total Profit: {result.TotalProfit}");
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"!!! EXCEPTION in GetRevenueProfitReportAsync:");
                System.Diagnostics.Debug.WriteLine($"  Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"  Type: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"  Stack: {ex.StackTrace}");
                return GetDefaultRevenueProfitReport();
            }
        }

        private DashboardStats GetDefaultDashboardStats()
        {
            return new DashboardStats
            {
                TotalProducts = 0,
                TodayOrders = 0,
                TodayRevenue = 0,
                LowStockCount = 0
            };
        }

        private RevenueProfitReport GetDefaultRevenueProfitReport()
        {
            return new RevenueProfitReport
            {
                TotalRevenue = 0,
                TotalProfit = 0,
                TotalCost = 0,
                TotalOrders = 0,
                AverageOrderValue = 0,
                ProfitMargin = 0,
                DataPoints = new List<RevenueProfitDataPoint>()
            };
        }

        private string FormatCurrency(string value)
        {
            if (int.TryParse(value, out int amount))
            {
                return amount.ToString("N0", new CultureInfo("vi-VN")) + " ₫";
            }
            return value;
        }

        // Response classes for GraphQL
        private class DashboardStatsResponse
        {
            [Newtonsoft.Json.JsonProperty("dashboardStats")]
            public DashboardStats? DashboardStats { get; set; }
        }

        private class RecentOrdersResponse
        {
            [Newtonsoft.Json.JsonProperty("recentOrders")]
            public List<RecentOrderDto>? RecentOrders { get; set; }
        }

        private class LowStockProductsResponse
        {
            [Newtonsoft.Json.JsonProperty("lowStockProducts")]
            public List<LowStockProductDto>? LowStockProducts { get; set; }
        }

        private class BestSellingProductsResponse
        {
            [Newtonsoft.Json.JsonProperty("bestSellingProducts")]
            public List<BestSellingProductDto>? BestSellingProducts { get; set; }
        }

        private class ProductsResponse
        {
            [Newtonsoft.Json.JsonProperty("products")]
            public List<ProductResponse>? Products { get; set; }
        }

        private class ProductResponse
        {
            [Newtonsoft.Json.JsonProperty("id")]
            public int Id { get; set; }

            [Newtonsoft.Json.JsonProperty("sku")]
            public string Sku { get; set; } = string.Empty;

            [Newtonsoft.Json.JsonProperty("name")]
            public string Name { get; set; } = string.Empty;

            [Newtonsoft.Json.JsonProperty("importPrice")]
            public int ImportPrice { get; set; }

            [Newtonsoft.Json.JsonProperty("count")]
            public int Count { get; set; }

            [Newtonsoft.Json.JsonProperty("description")]
            public string? Description { get; set; }

            [Newtonsoft.Json.JsonProperty("categoryId")]
            public int? CategoryId { get; set; }

            [Newtonsoft.Json.JsonProperty("category")]
            public CategoryResponse? Category { get; set; }

            [Newtonsoft.Json.JsonProperty("images")]
            public List<string>? Images { get; set; }
        }

        private class CategoriesResponse
        {
            [Newtonsoft.Json.JsonProperty("categories")]
            public List<CategoryResponse>? Categories { get; set; }
        }

        private class CategoryResponse
        {
            [Newtonsoft.Json.JsonProperty("id")]
            public int Id { get; set; }

            [Newtonsoft.Json.JsonProperty("name")]
            public string Name { get; set; } = string.Empty;

            [Newtonsoft.Json.JsonProperty("description")]
            public string? Description { get; set; }

            [Newtonsoft.Json.JsonProperty("products")]
            public List<ProductResponse>? Products { get; set; }
        }

        private class CustomersResponse
        {
            [Newtonsoft.Json.JsonProperty("customers")]
            public List<CustomerResponse>? Customers { get; set; }
        }

        private class CustomerResponse
        {
            [Newtonsoft.Json.JsonProperty("id")]
            public int Id { get; set; }

            [Newtonsoft.Json.JsonProperty("name")]
            public string Name { get; set; } = string.Empty;

            [Newtonsoft.Json.JsonProperty("phone")]
            public string Phone { get; set; } = string.Empty;

            [Newtonsoft.Json.JsonProperty("isDeleted")]
            public bool IsDeleted { get; set; }

            [Newtonsoft.Json.JsonProperty("createdAt")]
            public string CreatedAt { get; set; } = string.Empty;
        }

        private class CreateProductResponse
        {
            [Newtonsoft.Json.JsonProperty("createProduct")]
            public ProductResponse? CreateProduct { get; set; }
        }

        private class UpdateProductResponse
        {
            [Newtonsoft.Json.JsonProperty("updateProduct")]
            public ProductResponse? UpdateProduct { get; set; }
        }

        private class DeleteProductResponse
        {
            [Newtonsoft.Json.JsonProperty("deleteProduct")]
            public bool DeleteProduct { get; set; }
        }

        private class CreateOrderResponse
        {
            [Newtonsoft.Json.JsonProperty("createOrder")]
            public OrderResponse? CreateOrder { get; set; }
        }

        private class OrderResponse
        {
            [Newtonsoft.Json.JsonProperty("id")]
            public int Id { get; set; }

            [Newtonsoft.Json.JsonProperty("customerId")]
            public int? CustomerId { get; set; }

            [Newtonsoft.Json.JsonProperty("createdTime")]
            public string CreatedTime { get; set; } = string.Empty;

            [Newtonsoft.Json.JsonProperty("finalPrice")]
            public int FinalPrice { get; set; }
        }

        private class CreateCustomerResponse
        {
            [Newtonsoft.Json.JsonProperty("createCustomer")]
            public CustomerResponse? CreateCustomer { get; set; }
        }

        private class UpdateCustomerResponse
        {
            [Newtonsoft.Json.JsonProperty("updateCustomer")]
            public CustomerResponse? UpdateCustomer { get; set; }
        }

        private class DeleteCustomerResponse
        {
            [Newtonsoft.Json.JsonProperty("deleteCustomer")]
            public bool DeleteCustomer { get; set; }
        }

        private class ProductSalesReportResponse
        {
            [Newtonsoft.Json.JsonProperty("productSalesReport")]
            public List<ProductSalesReport>? ProductSalesReport { get; set; }
        }

        private class RevenueProfitReportResponse
        {
            [Newtonsoft.Json.JsonProperty("revenueProfitReport")]
            public RevenueProfitReport? RevenueProfitReport { get; set; }
        }
    }
}
