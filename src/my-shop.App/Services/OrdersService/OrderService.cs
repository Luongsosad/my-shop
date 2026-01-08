using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using my_shop.App.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace my_shop.App.Services.OrdersService
{
    public class OrderService
    {
        private readonly GraphQLHttpClient _graphQLClient;

        public OrderService()
        {
            _graphQLClient = new GraphQLHttpClient(
                "http://localhost:5000/graphql",
                new NewtonsoftJsonSerializer()
            );
        }

        public async Task<(List<Order> Orders, int TotalCount)> GetOrdersAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var skip = (page - 1) * pageSize;
                var query = new GraphQLRequest
                {
                    Query = @"
                        query {
                            orders(order: { createdTime: DESC }) {
                                id
                                createdTime
                                finalPrice
                                status
                                customerId
                                customer {
                                    id
                                    name
                                    phone
                                    isDeleted
                                    createdAt
                                }    
                                extensionData
                                orderItems {
                                    id
                                    productId
                                    quantity
                                    price
                                    product {
                                        id
                                        name
                                        sku
                                        importPrice
                                        count
                                        description
                                        categoryId
                                        category {
                                            id
                                            name
                                        }
                                        extensionData
                                        images
                                    }
                                }
                            }
                        }"
                };

                var response = await _graphQLClient.SendQueryAsync<OrdersResponse>(query);

                if (response.Errors != null && response.Errors.Any())
                {
                    foreach (var error in response.Errors)
                    {
                        Debug.WriteLine($"GraphQL Error: {error.Message}");
                    }
                }

                var allOrders = response.Data?.Orders ?? new List<Order>();
                var orders = allOrders.Skip(skip).Take(pageSize).ToList();
                var totalCount = allOrders.Count;

                Debug.WriteLine($"Total orders: {totalCount}, Page: {page}, Showing: {orders.Count}");

                return (orders, totalCount);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetOrdersAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return (new List<Order>(), 0);
            }
        }

        public async Task<Order?> UpdateOrderStatusAsync(int orderId, string status)
        {
            try
            {
                var mutation = new GraphQLRequest
                {
                    Query = @"
                        mutation($id: Int!, $status: String!) {
                            updateOrderStatus(id: $id, status: $status) {
                                id
                                status
                                finalPrice
                                createdTime
                            }
                        }",
                    Variables = new { id = orderId, status }
                };

                var response = await _graphQLClient.SendMutationAsync<UpdateOrderResponse>(mutation);

                if (response.Errors != null && response.Errors.Any())
                {
                    foreach (var error in response.Errors)
                    {
                        Debug.WriteLine($"GraphQL Error: {error.Message}");
                    }
                    throw new Exception(response.Errors.First().Message);
                }

                return response.Data?.UpdateOrderStatus;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in UpdateOrderStatusAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<Order?> UpdateOrderAsync(int orderId, List<OrderItem> items)
        {
            try
            {
                var orderItemsInput = items.Select(item => new
                {
                    productId = item.ProductId,
                    quantity = item.Quantity,
                    price = item.Price
                }).ToList();

                var mutation = new GraphQLRequest
                {
                    Query = @"
                        mutation($id: Int!, $items: [OrderItemInput!]!) {
                            updateOrder(id: $id, items: $items) {
                                id
                                finalPrice
                                status
                                orderItems {
                                    id
                                    productId
                                    quantity
                                    price
                                    product {
                                        id
                                        name
                                        sku
                                        importPrice
                                        count
                                        description
                                        categoryId
                                        category {
                                            id
                                            name
                                        }
                                        images
                                    }
                                }
                            }
                        }",
                    Variables = new { id = orderId, items = orderItemsInput }
                };

                var response = await _graphQLClient.SendMutationAsync<UpdateOrderFullResponse>(mutation);

                if (response.Errors != null && response.Errors.Any())
                {
                    foreach (var error in response.Errors)
                    {
                        Debug.WriteLine($"GraphQL Error: {error.Message}");
                    }
                    throw new Exception(response.Errors.First().Message);
                }

                return response.Data?.UpdateOrder;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in UpdateOrderAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Models.Product>> SearchProductsAsync(string searchText)
        {
            try
            {
                var query = new GraphQLRequest
                {
                    Query = @"
                        query($search: String!) {
                            products(
                                where: {
                                    or: [
                                        { name: { contains: $search } }
                                        { sku: { contains: $search } }
                                    ]
                                }
                                order: { name: ASC }
                            ) {
                                id
                                name
                                sku
                                importPrice
                                count
                                description
                                categoryId
                                category {
                                    id
                                    name
                                }
                                extensionData
                                images
                            }
                        }",
                    Variables = new { search = searchText }
                };

                var response = await _graphQLClient.SendQueryAsync<ProductSearchResponse>(query);

                if (response.Errors != null && response.Errors.Any())
                {
                    foreach (var error in response.Errors)
                    {
                        Debug.WriteLine($"GraphQL Error: {error.Message}");
                    }
                    return new List<Product>();
                }

                var products = response.Data?.Products ?? new List<Product>();
                Debug.WriteLine($"Found {products.Count} products for search: {searchText}");

                return products;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in SearchProductsAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return new List<Product>();
            }
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            try
            {
                var query = new GraphQLRequest
                {
                    Query = @"
                        query($id: Int!) {
                            products(where: { id: { eq: $id } }) {
                                id
                                name
                                sku
                                importPrice
                                count
                                description
                                categoryId
                                category {
                                    id
                                    name
                                }
                                extensionData
                                images
                            }
                        }",
                    Variables = new { id = productId }
                };

                var response = await _graphQLClient.SendQueryAsync<ProductSearchResponse>(query);

                if (response.Errors != null && response.Errors.Any())
                {
                    foreach (var error in response.Errors)
                    {
                        Debug.WriteLine($"GraphQL Error: {error.Message}");
                    }
                    return null;
                }

                return response.Data?.Products?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetProductByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<(List<Models.Order> Orders, int TotalCount)> SearchOrdersAsync(
            int page = 1,
            int pageSize = 10,
            DateTimeOffset? fromDate = null,
            DateTimeOffset? toDate = null,
            int? customerId = null,
            int? productId = null,
            double? minPrice = null,
            double? maxPrice = null,
            string? status = null)
        {
            try
            {
                var skip = (page - 1) * pageSize;

                // Build where conditions dynamically
                var whereConditions = new List<string>();
                var variables = new Dictionary<string, object>();

                if (fromDate.HasValue)
                {
                    whereConditions.Add("{ createdTime: { gte: $fromDate } }");
                    variables["fromDate"] = fromDate.Value.UtcDateTime;
                }

                if (toDate.HasValue)
                {
                    whereConditions.Add("{ createdTime: { lte: $toDate } }");
                    variables["toDate"] = toDate.Value.UtcDateTime;
                }

                if (customerId.HasValue)
                {
                    whereConditions.Add("{ customerId: { eq: $customerId } }");
                    variables["customerId"] = customerId.Value;
                }

                if (productId.HasValue)
                {
                    whereConditions.Add("{ orderItems: { some: { productId: { eq: $productId } } } }");
                    variables["productId"] = productId.Value;
                }

                if (minPrice.HasValue && !double.IsNaN(minPrice.Value))
                {
                    whereConditions.Add("{ finalPrice: { gte: $minPrice } }");
                    variables["minPrice"] = (int)minPrice.Value;
                }

                if (maxPrice.HasValue && !double.IsNaN(maxPrice.Value))
                {
                    whereConditions.Add("{ finalPrice: { lte: $maxPrice } }");
                    variables["maxPrice"] = (int)maxPrice.Value;
                }

                if (!string.IsNullOrEmpty(status))
                {
                    whereConditions.Add("{ status: { eq: $status } }");
                    variables["status"] = status;
                }

                // Build query with proper variable declarations
                var variableDeclarations = new List<string>();
                if (variables.ContainsKey("fromDate")) variableDeclarations.Add("$fromDate: DateTime");
                if (variables.ContainsKey("toDate")) variableDeclarations.Add("$toDate: DateTime");
                if (variables.ContainsKey("customerId")) variableDeclarations.Add("$customerId: Int");
                if (variables.ContainsKey("productId")) variableDeclarations.Add("$productId: Int");
                if (variables.ContainsKey("minPrice")) variableDeclarations.Add("$minPrice: Int");
                if (variables.ContainsKey("maxPrice")) variableDeclarations.Add("$maxPrice: Int");
                if (variables.ContainsKey("status")) variableDeclarations.Add("$status: String");

                var whereClause = whereConditions.Any()
                    ? $"where: {{ and: [{string.Join(", ", whereConditions)}] }}"
                    : "";

                var queryStr = variableDeclarations.Any()
                    ? $"query({string.Join(", ", variableDeclarations)})"
                    : "query";

                var query = new GraphQLRequest
                {
                    Query = $@"
                {queryStr} {{
                    orders({whereClause} order: {{ createdTime: DESC }}) {{
                        id
                        createdTime
                        finalPrice
                        status
                        customerId
                        customer {{
                            id
                            name
                            phone
                            isDeleted
                            createdAt
                        }}
                        extensionData
                        orderItems {{
                            id
                            productId
                            quantity
                            price
                            product {{
                                id
                                name
                                sku
                                importPrice
                                count
                                description
                                categoryId
                                category {{
                                    id
                                    name
                                }}
                                extensionData
                                images
                            }}
                        }}
                    }}
                }}",
                    Variables = variables.Any() ? variables : null
                };

                Debug.WriteLine($"GraphQL Query: {query.Query}");
                Debug.WriteLine($"Variables: {JsonConvert.SerializeObject(variables)}");

                var response = await _graphQLClient.SendQueryAsync<OrdersResponse>(query);

                if (response.Errors != null && response.Errors.Any())
                {
                    foreach (var error in response.Errors)
                    {
                        Debug.WriteLine($"GraphQL Error: {error.Message}");
                    }
                    // Return empty if there are errors
                    return (new List<Order>(), 0);
                }

                var allOrders = response.Data?.Orders ?? new List<Order>();
                var orders = allOrders.Skip(skip).Take(pageSize).ToList();
                var totalCount = allOrders.Count;

                Debug.WriteLine($"Search results: {totalCount}, Page: {page}, Showing: {orders.Count}");

                return (orders, totalCount);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in SearchOrdersAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return (new List<Order>(), 0);
            }
        }

        public async Task<List<Customer>> SearchCustomersAsync(string searchText)
        {
            try
            {
                var query = new GraphQLRequest
                {
                    Query = @"
                query($search: String!) {
                    customers(
                        where: {
                            and: [
                                { isDeleted: { eq: false } }
                                {
                                    or: [
                                        { name: { contains: $search } }
                                        { phone: { contains: $search } }
                                    ]
                                }
                            ]
                        }
                        order: { name: ASC }
                    ) {
                        id
                        name
                        phone
                        createdAt
                    }
                }",
                    Variables = new { search = searchText }
                };

                var response = await _graphQLClient.SendQueryAsync<CustomerSearchResponse>(query);

                if (response.Errors != null && response.Errors.Any())
                {
                    foreach (var error in response.Errors)
                    {
                        Debug.WriteLine($"GraphQL Error: {error.Message}");
                    }
                    return new List<Customer>();
                }

                return response.Data?.Customers ?? new List<Customer>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in SearchCustomersAsync: {ex.Message}");
                return new List<Customer>();
            }
        }

        private class OrdersResponse
        {
            [JsonProperty("orders")]
            public List<Order>? Orders { get; set; }
        }

        private class UpdateOrderResponse
        {
            [JsonProperty("updateOrderStatus")]
            public Order? UpdateOrderStatus { get; set; }
        }

        private class UpdateOrderFullResponse
        {
            [JsonProperty("updateOrder")]
            public Order? UpdateOrder { get; set; }
        }

        private class ProductSearchResponse
        {
            [JsonProperty("products")]
            public List<Product>? Products { get; set; }
        }

        private class CustomerSearchResponse
        {
            [JsonProperty("customers")]
            public List<Customer>? Customers { get; set; }
        }
    }
}