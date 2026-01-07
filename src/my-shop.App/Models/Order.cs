using CommunityToolkit.Mvvm.ComponentModel;
using my_shop.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace my_shop.App.Models
{
    public partial class Order : ObservableObject
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("createdTime")]
        public DateTime CreatedTime { get; set; }

        [JsonProperty("finalPrice")]
        public int FinalPrice { get; set; }

        [JsonProperty("extensionData")]
        public string? ExtensionData { get; set; }

        [ObservableProperty]
        [JsonIgnore]
        private string status = "created";

        partial void OnStatusChanged(string value)
        {
            OnPropertyChanged(nameof(IsCreated));
            OnPropertyChanged(nameof(IsPaid));
            OnPropertyChanged(nameof(IsCancelled));
            OnPropertyChanged(nameof(StatusDisplay));
        }

        [JsonProperty("orderItems")]
        public List<OrderItem>? OrderItems { get; set; }

        [JsonProperty("customerId")]
        public int? CustomerId { get; set; }

        [JsonProperty("customer")]
        public Customer? Customer { get; set; }

        // UI properties
        public string FormattedPrice => FinalPrice.ToString("N0") + " ₫";
        public string FormattedDate => CreatedTime.ToString("dd/MM/yyyy HH:mm");
        public int TotalItems => OrderItems?.Count ?? 0;

        public string StatusDisplay => Status switch
        {
            "created" => "Created",
            "paid" => "Paid",
            "cancelled" => "Cancelled",
            _ => Status
        };

        public bool IsCreated => Status == "created";
        public bool IsPaid => Status == "paid";
        public bool IsCancelled => Status == "cancelled";

        [ObservableProperty]
        private bool isExpanded;
    }


    public class OrderItem
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("orderId")]
        public int OrderId { get; set; }

        [JsonProperty("productId")]
        public int ProductId { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("price")]
        public int Price { get; set; }

        [JsonProperty("product")]
        public Product? Product { get; set; }

        // UI properties
        public string FormattedPrice => Price.ToString("N0") + " ₫";
        public int Subtotal => Price * Quantity;
        public string FormattedSubtotal => Subtotal.ToString("N0") + " ₫";
    }

    public class Product : ObservableObject
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("sku")]
        public string Sku { get; set; } = string.Empty;

        [JsonProperty("importPrice")]
        public int ImportPrice { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("categoryId")]
        public int? CategoryId { get; set; }

        [JsonProperty("category")]
        public Category? Category { get; set; }

        [JsonProperty("extensionData")]
        public string? ExtensionData { get; set; }

        private List<string>? _images;

        [JsonProperty("images")]
        public List<string>? Images
        {
            get => _images;
            set
            {
                // Filter out null or whitespace values when setting
                _images = value?
                    .Where(img => !string.IsNullOrWhiteSpace(img))
                    .ToList();

                OnPropertyChanged();
                OnPropertyChanged(nameof(HasValidImages));
            }
        }

        [JsonIgnore]
        public string FormattedPrice => $"{ImportPrice:N0} đ";

        [JsonIgnore]
        public bool HasValidImages => Images?.Any() == true;

        [JsonIgnore]
        public string StockStatus => Count > 0 ? $"{Count} units" : "Out of stock";

        [JsonIgnore]
        public bool IsInStock => Count > 0;
    }

    public class Customer : ObservableObject
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("phone")]
        public string Phone { get; set; } = string.Empty;

        [JsonProperty("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("orders")]
        public List<Order>? Orders { get; set; }

        // UI Properties
        [JsonIgnore]
        public string DisplayName => $"{Name} - {Phone}";

        [JsonIgnore]
        public string FormattedCreatedAt => CreatedAt.ToString("dd/MM/yyyy HH:mm");

        [JsonIgnore]
        public string StatusText => IsDeleted ? "Inactive" : "Active";

        [JsonIgnore]
        public int TotalOrders => Orders?.Count ?? 0;
    }
}
