using CommunityToolkit.Mvvm.ComponentModel;
using my_shop.Domain.Entities;
using System;

namespace my_shop.App.Models
{
    public partial class EditableOrderItem : ObservableObject
    {
        private readonly Action _onChanged;

        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private int productId;

        [ObservableProperty]
        private string productName = string.Empty;

        [ObservableProperty]
        private string productSku = string.Empty;

        [ObservableProperty]
        private int price;

        [ObservableProperty]
        private int quantity = 1;

        [ObservableProperty]
        private Product? product;

        public string FormattedPrice => $"{Price:N0} ₫";
        public int Subtotal => Price * Quantity;
        public string FormattedSubtotal => $"{Subtotal:N0} ₫";

        public EditableOrderItem(OrderItem original, Action onChanged)
        {
            _onChanged = onChanged;

            Id = original.Id;
            ProductId = original.ProductId;
            ProductName = original.Product?.Name ?? "";
            ProductSku = original.Product?.Sku ?? "";
            Price = original.Price;
            Quantity = original.Quantity;
            Product = original.Product;
        }

        public EditableOrderItem(Product product, Action onChanged)
        {
            _onChanged = onChanged;

            ProductId = product.Id;
            ProductName = product.Name;
            ProductSku = product.Sku;
            Price = product.ImportPrice;
            Quantity = 1;
            Product = product;
        }

        partial void OnQuantityChanged(int value)
        {
            OnPropertyChanged(nameof(Subtotal));
            OnPropertyChanged(nameof(FormattedSubtotal));
            _onChanged?.Invoke();
        }

        partial void OnPriceChanged(int value)
        {
            OnPropertyChanged(nameof(FormattedPrice));
            OnPropertyChanged(nameof(Subtotal));
            OnPropertyChanged(nameof(FormattedSubtotal));
            _onChanged?.Invoke();
        }

        public void UpdateProduct(Product newProduct)
        {
            Product = newProduct;
            ProductId = newProduct.Id;
            ProductName = newProduct.Name;
            ProductSku = newProduct.Sku;
            Price = newProduct.ImportPrice;

            OnPropertyChanged(nameof(ProductName));
            OnPropertyChanged(nameof(ProductSku));
            OnPropertyChanged(nameof(Price));
            OnPropertyChanged(nameof(FormattedPrice));
            OnPropertyChanged(nameof(Subtotal));
            OnPropertyChanged(nameof(FormattedSubtotal));

            _onChanged?.Invoke();
        }
    }
}