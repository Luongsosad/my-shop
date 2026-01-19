using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using my_shop.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace my_shop.App.Services;

public class VoucherService
{
    private readonly GraphQLHttpClient _graphQLClient;

    public VoucherService()
    {
        var endpoint = "http://localhost:5000/graphql";
        var serializerSettings = new Newtonsoft.Json.JsonSerializerSettings
        {
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
        };
        _graphQLClient = new GraphQLHttpClient(endpoint, new NewtonsoftJsonSerializer(serializerSettings));
    }

    public async Task<List<VoucherModel>> GetVouchersAsync()
    {
        try
        {
            var query = new GraphQLRequest
            {
                Query = @"
                    query {
                        vouchers {
                            id
                            code
                            description
                            discountPercent
                            maxDiscountAmount
                            minOrderAmount
                            totalQuantity
                            remainingQuantity
                            startDate
                            endDate
                            isDeleted
                            createdAt
                        }
                    }"
            };

            var response = await _graphQLClient.SendQueryAsync<VouchersResponse>(query);

            if (response.Errors != null && response.Errors.Length > 0)
            {
                return new List<VoucherModel>();
            }

            var vouchers = response.Data?.Vouchers ?? new List<VoucherResponse>();
            return vouchers.Select(v => new VoucherModel
            {
                Id = v.Id,
                Code = v.Code,
                Description = v.Description,
                DiscountPercent = v.DiscountPercent,
                MaxDiscountAmount = v.MaxDiscountAmount,
                MinOrderAmount = v.MinOrderAmount,
                TotalQuantity = v.TotalQuantity,
                RemainingQuantity = v.RemainingQuantity,
                StartDate = new DateTimeOffset(v.StartDate),
                EndDate = new DateTimeOffset(v.EndDate),
                IsDeleted = v.IsDeleted,
                CreatedAt = new DateTimeOffset(v.CreatedAt)
            }).ToList();
        }
        catch (Exception)
        {
            return new List<VoucherModel>();
        }
    }

    public async Task<VoucherModel?> GetVoucherByCodeAsync(string code)
    {
        try
        {
            var query = new GraphQLRequest
            {
                Query = @"
                    query($code: String!) {
                        voucherByCode(code: $code) {
                            id
                            code
                            description
                            discountPercent
                            maxDiscountAmount
                            minOrderAmount
                            totalQuantity
                            remainingQuantity
                            startDate
                            endDate
                            isDeleted
                            createdAt
                        }
                    }",
                Variables = new { code }
            };

            var response = await _graphQLClient.SendQueryAsync<VoucherByCodeResponse>(query);

            if (response.Errors != null && response.Errors.Length > 0)
            {
                return null;
            }

            var v = response.Data?.VoucherByCode;
            if (v == null) return null;

            return new VoucherModel
            {
                Id = v.Id,
                Code = v.Code,
                Description = v.Description,
                DiscountPercent = v.DiscountPercent,
                MaxDiscountAmount = v.MaxDiscountAmount,
                MinOrderAmount = v.MinOrderAmount,
                TotalQuantity = v.TotalQuantity,
                RemainingQuantity = v.RemainingQuantity,
                StartDate = new DateTimeOffset(v.StartDate),
                EndDate = new DateTimeOffset(v.EndDate),
                IsDeleted = v.IsDeleted,
                CreatedAt = new DateTimeOffset(v.CreatedAt)
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<VoucherDiscountResult> CheckVoucherAsync(string code, int orderAmount)
    {
        try
        {
            var query = new GraphQLRequest
            {
                Query = @"
                    query($code: String!, $orderAmount: Int!) {
                        checkVoucher(code: $code, orderAmount: $orderAmount) {
                            success
                            message
                            voucherId
                            discountAmount
                            finalAmount
                        }
                    }",
                Variables = new { code, orderAmount }
            };

            var response = await _graphQLClient.SendQueryAsync<CheckVoucherResponse>(query);

            if (response.Errors != null && response.Errors.Length > 0)
            {
                return new VoucherDiscountResult
                {
                    Success = false,
                    Message = response.Errors[0].Message
                };
            }

            var result = response.Data?.CheckVoucher;
            return new VoucherDiscountResult
            {
                Success = result?.Success ?? false,
                Message = result?.Message,
                VoucherId = result?.VoucherId,
                DiscountAmount = result?.DiscountAmount ?? 0,
                FinalAmount = result?.FinalAmount ?? orderAmount
            };
        }
        catch (Exception ex)
        {
            return new VoucherDiscountResult
            {
                Success = false,
                Message = $"Lỗi: {ex.Message}"
            };
        }
    }

    public async Task<VoucherModel> CreateVoucherAsync(VoucherModel model)
    {
        try
        {
            var mutation = new GraphQLRequest
            {
                Query = @"
                    mutation($input: CreateVoucherDtoInput!) {
                        createVoucher(input: $input) {
                            id
                            code
                            description
                            discountPercent
                            maxDiscountAmount
                            minOrderAmount
                            totalQuantity
                            remainingQuantity
                            startDate
                            endDate
                            isDeleted
                            createdAt
                        }
                    }",
                Variables = new
                {
                    input = new
                    {
                        code = model.Code,
                        description = model.Description,
                        discountPercent = model.DiscountPercent,
                        maxDiscountAmount = model.MaxDiscountAmount,
                        minOrderAmount = model.MinOrderAmount,
                        totalQuantity = model.TotalQuantity,
                        startDate = model.StartDate.UtcDateTime,
                        endDate = model.EndDate.UtcDateTime
                    }
                }
            };

            System.Diagnostics.Debug.WriteLine($"🔵 Sending GraphQL mutation with:");
            System.Diagnostics.Debug.WriteLine($"   Code: {model.Code}");
            System.Diagnostics.Debug.WriteLine($"   StartDate: {model.StartDate.UtcDateTime}");
            System.Diagnostics.Debug.WriteLine($"   EndDate: {model.EndDate.UtcDateTime}");

            var response = await _graphQLClient.SendMutationAsync<CreateVoucherResponse>(mutation);

            if (response.Errors != null && response.Errors.Length > 0)
            {
                var errorMsg = response.Errors[0].Message;
                System.Diagnostics.Debug.WriteLine($"❌ GraphQL Error: {errorMsg}");
                throw new Exception(errorMsg);
            }

            var v = response.Data?.CreateVoucher;
            if (v == null)
                throw new Exception("Failed to create voucher");

            return new VoucherModel
            {
                Id = v.Id,
                Code = v.Code,
                Description = v.Description,
                DiscountPercent = v.DiscountPercent,
                MaxDiscountAmount = v.MaxDiscountAmount,
                MinOrderAmount = v.MinOrderAmount,
                TotalQuantity = v.TotalQuantity,
                RemainingQuantity = v.RemainingQuantity,
                StartDate = new DateTimeOffset(v.StartDate),
                EndDate = new DateTimeOffset(v.EndDate),
                IsDeleted = v.IsDeleted,
                CreatedAt = new DateTimeOffset(v.CreatedAt)
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Create voucher failed: {ex.Message}");
        }
    }

    public async Task<VoucherModel> UpdateVoucherAsync(VoucherModel model)
    {
        try
        {
            var mutation = new GraphQLRequest
            {
                Query = @"
                    mutation($input: UpdateVoucherDtoInput!) {
                        updateVoucher(input: $input) {
                            id
                            code
                            description
                            discountPercent
                            maxDiscountAmount
                            minOrderAmount
                            totalQuantity
                            remainingQuantity
                            startDate
                            endDate
                            isDeleted
                            createdAt
                        }
                    }",
                Variables = new
                {
                    input = new
                    {
                        id = model.Id,
                        code = model.Code,
                        description = model.Description,
                        discountPercent = model.DiscountPercent,
                        maxDiscountAmount = model.MaxDiscountAmount,
                        minOrderAmount = model.MinOrderAmount,
                        totalQuantity = model.TotalQuantity,
                        remainingQuantity = model.RemainingQuantity,
                        startDate = model.StartDate.UtcDateTime,
                        endDate = model.EndDate.UtcDateTime
                    }
                }
            };

            var response = await _graphQLClient.SendMutationAsync<UpdateVoucherResponse>(mutation);

            if (response.Errors != null && response.Errors.Length > 0)
            {
                throw new Exception(response.Errors[0].Message);
            }

            var v = response.Data?.UpdateVoucher;
            if (v == null)
                throw new Exception("Failed to update voucher");

            return new VoucherModel
            {
                Id = v.Id,
                Code = v.Code,
                Description = v.Description,
                DiscountPercent = v.DiscountPercent,
                MaxDiscountAmount = v.MaxDiscountAmount,
                MinOrderAmount = v.MinOrderAmount,
                TotalQuantity = v.TotalQuantity,
                RemainingQuantity = v.RemainingQuantity,
                StartDate = new DateTimeOffset(v.StartDate),
                EndDate = new DateTimeOffset(v.EndDate),
                IsDeleted = v.IsDeleted,
                CreatedAt = new DateTimeOffset(v.CreatedAt)
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Update voucher failed: {ex.Message}");
        }
    }

    public async Task<bool> DeleteVoucherAsync(int id)
    {
        try
        {
            var mutation = new GraphQLRequest
            {
                Query = @"
                    mutation($id: Int!) {
                        deleteVoucher(id: $id)
                    }",
                Variables = new { id }
            };

            var response = await _graphQLClient.SendMutationAsync<DeleteVoucherResponse>(mutation);

            if (response.Errors != null && response.Errors.Length > 0)
            {
                return false;
            }

            return response.Data?.DeleteVoucher ?? false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    // Response classes
    private class VouchersResponse
    {
        public List<VoucherResponse> Vouchers { get; set; } = new();
    }

    private class VoucherByCodeResponse
    {
        public VoucherResponse? VoucherByCode { get; set; }
    }

    private class CheckVoucherResponse
    {
        public VoucherDiscountResultResponse? CheckVoucher { get; set; }
    }

    private class CreateVoucherResponse
    {
        public VoucherResponse? CreateVoucher { get; set; }
    }

    private class UpdateVoucherResponse
    {
        public VoucherResponse? UpdateVoucher { get; set; }
    }

    private class DeleteVoucherResponse
    {
        public bool DeleteVoucher { get; set; }
    }

    private class VoucherResponse
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DiscountPercent { get; set; }
        public int MaxDiscountAmount { get; set; }
        public int MinOrderAmount { get; set; }
        public int TotalQuantity { get; set; }
        public int RemainingQuantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    private class VoucherDiscountResultResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int? VoucherId { get; set; }
        public int DiscountAmount { get; set; }
        public int FinalAmount { get; set; }
    }
}

public class VoucherDiscountResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public int? VoucherId { get; set; }
    public int DiscountAmount { get; set; }
    public int FinalAmount { get; set; }
}
