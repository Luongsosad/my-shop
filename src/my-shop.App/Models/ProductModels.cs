using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace my_shop.App.Models
{
    public class ProductModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("sku")]
        public string Sku { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("importPrice")]
        public int ImportPrice { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("categoryId")]
        public int? CategoryId { get; set; }

        [JsonProperty("categoryName")]
        public string? CategoryName { get; set; }

        [JsonProperty("images")]
        public List<string>? Images { get; set; }
    }

    public class CreateProductModel
    {
        public string Sku { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int ImportPrice { get; set; }
        public int Count { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public List<string>? Images { get; set; }
    }

    public class UpdateProductModel
    {
        public int Id { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int ImportPrice { get; set; }
        public int Count { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public List<string>? Images { get; set; }
    }

    public class CategoryModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("productCount")]
        public int ProductCount { get; set; }
    }

    public class CustomerModel
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
        public string CreatedAt { get; set; } = string.Empty;

        public string DisplayText => $"{Name} - {Phone}";
    }

    public class CartItemModel : ObservableObject
    {
        public int ProductId
        {
            get => _productId;
            set => SetProperty(ref _productId, value);
        }
        private int _productId;
        
        public string ProductName
        {
            get => _productName;
            set => SetProperty(ref _productName, value);
        }
        private string _productName = string.Empty;
        
        public int Price
        {
            get => _price;
            set
            {
                if (SetProperty(ref _price, value))
                    OnPropertyChanged(nameof(Subtotal));
            }
        }
        private int _price;
        
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (SetProperty(ref _quantity, value))
                    OnPropertyChanged(nameof(Subtotal));
            }
        }
        private int _quantity;
        
        public string? ImageUrl
        {
            get => _imageUrl;
            set => SetProperty(ref _imageUrl, value);
        }
        private string? _imageUrl;
        
        public int Subtotal => Quantity * Price;
    }

    public class OrderItemModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int Price { get; set; }
    }

    public class CreateOrderModel
    {
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public List<OrderItemModel> Items { get; set; } = new();
        public int FinalPrice { get; set; }
    }

    public class OrderModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("customerId")]
        public int? CustomerId { get; set; }

        [JsonProperty("createdTime")]
        public string CreatedTime { get; set; } = string.Empty;

        [JsonProperty("finalPrice")]
        public int FinalPrice { get; set; }

        [JsonProperty("items")]
        public List<OrderItemModel>? Items { get; set; }
    }
}
