using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using my_shop.App.Models;
using my_shop.Domain.Entities;
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
    }
}
