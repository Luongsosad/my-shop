using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using my_shop.App.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace my_shop.App.ViewModels
{
    public partial class ProductEditViewModel : ObservableObject
    {
        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private string sku = string.Empty;

        [ObservableProperty]
        private int importPrice;

        [ObservableProperty]
        private int count;

        [ObservableProperty]
        private string description = string.Empty;

        [ObservableProperty]
        private CategoryModel? selectedCategory;

        [ObservableProperty]
        private ObservableCollection<string> images = new();

        [ObservableProperty]
        private ObservableCollection<CategoryModel> categories = new();

        [ObservableProperty]
        private bool isEditMode;

        [ObservableProperty]
        private string? errorMessage;

        [ObservableProperty]
        private bool hasError;

        public ProductEditViewModel()
        {
        }

        public void LoadProduct(ProductModel? product, List<CategoryModel> availableCategories)
        {
            // Load categories
            this.Categories.Clear();
            foreach (var category in availableCategories)
            {
                this.Categories.Add(category);
            }

            if (product != null)
            {
                // Edit mode
                this.IsEditMode = true;
                this.Id = product.Id;
                this.Name = product.Name;
                this.Sku = product.Sku;
                this.ImportPrice = product.ImportPrice;
                this.Count = product.Count;
                this.Description = product.Description ?? string.Empty;

                // Load images
                this.Images.Clear();
                if (product.Images != null)
                {
                    foreach (var img in product.Images)
                    {
                        this.Images.Add(img);
                    }
                }

                // Select category
                this.SelectedCategory = this.Categories.FirstOrDefault(c => c.Id == product.CategoryId);
            }
            else
            {
                // Create mode
                this.IsEditMode = false;
                this.Id = 0;
                this.Name = string.Empty;
                this.Sku = string.Empty;
                this.ImportPrice = 0;
                this.Count = 0;
                this.Description = string.Empty;
                this.Images.Clear();
                this.SelectedCategory = this.Categories.FirstOrDefault();
            }
        }

        public bool Validate()
        {
            this.HasError = false;
            this.ErrorMessage = null;

            if (string.IsNullOrWhiteSpace(this.Name))
            {
                this.ErrorMessage = "Tên s?n ph?m không ???c ?? tr?ng";
                this.HasError = true;
                return false;
            }

            if (string.IsNullOrWhiteSpace(this.Sku))
            {
                this.ErrorMessage = "SKU không ???c ?? tr?ng";
                this.HasError = true;
                return false;
            }

            if (this.ImportPrice <= 0)
            {
                this.ErrorMessage = "Giá nh?p ph?i l?n h?n 0";
                this.HasError = true;
                return false;
            }

            if (this.Count < 0)
            {
                this.ErrorMessage = "T?n kho không ???c âm";
                this.HasError = true;
                return false;
            }

            if (this.SelectedCategory == null)
            {
                this.ErrorMessage = "Vui lòng ch?n danh m?c";
                this.HasError = true;
                return false;
            }

            return true;
        }

        public CreateProductModel ToCreateModel()
        {
            return new CreateProductModel
            {
                Name = this.Name,
                Sku = this.Sku,
                ImportPrice = this.ImportPrice,
                Count = this.Count,
                Description = string.IsNullOrWhiteSpace(this.Description) ? null : this.Description,
                CategoryId = this.SelectedCategory?.Id,
                Images = this.Images.ToList()
            };
        }

        public UpdateProductModel ToUpdateModel()
        {
            return new UpdateProductModel
            {
                Id = this.Id,
                Name = this.Name,
                Sku = this.Sku,
                ImportPrice = this.ImportPrice,
                Count = this.Count,
                Description = string.IsNullOrWhiteSpace(this.Description) ? null : this.Description,
                CategoryId = this.SelectedCategory?.Id,
                Images = this.Images.ToList()
            };
        }

        [RelayCommand]
        public void AddImage(string imageUrl)
        {
            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                Images.Add(imageUrl);
            }
        }

        [RelayCommand]
        public void RemoveImage(string imageUrl)
        {
            Images.Remove(imageUrl);
        }
    }
}
