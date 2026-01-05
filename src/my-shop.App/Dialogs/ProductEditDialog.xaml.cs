using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using my_shop.App.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace my_shop.App.Dialogs
{
    public sealed partial class ProductEditDialog : ContentDialog
    {
        private ProductModel? _product;
        private ObservableCollection<string> _images = new();

        public ProductEditDialog()
        {
            this.InitializeComponent();
            ImagesListView.ItemsSource = _images;
            
            // Update NoImagesText visibility when images change
            _images.CollectionChanged += (s, e) => UpdateNoImagesVisibility();
            UpdateNoImagesVisibility();
        }

        private void UpdateNoImagesVisibility()
        {
            try
            {
                if (NoImagesText != null)
                {
                    NoImagesText.Visibility = _images.Count == 0 
                        ? Visibility.Visible 
                        : Visibility.Collapsed;
                }
            }
            catch
            {
                // Ignore if control not yet loaded
            }
        }

        public void LoadProduct(ProductModel? product, List<CategoryModel> categories)
        {
            _product = product;

            // Load categories
            CategoryComboBox.ItemsSource = categories;

            if (product != null)
            {
                // Edit mode
                this.Title = $"Sửa sản phẩm: {product.Name}";
                NameTextBox.Text = product.Name;
                SkuTextBox.Text = product.Sku;
                PriceNumberBox.Value = product.ImportPrice;
                StockNumberBox.Value = product.Count;
                DescriptionTextBox.Text = product.Description ?? "";

                // Load images
                _images.Clear();
                if (product.Images != null)
                {
                    foreach (var img in product.Images)
                    {
                        _images.Add(img);
                    }
                }

                // Select category
                CategoryComboBox.SelectedItem = categories.FirstOrDefault(c => c.Id == product.CategoryId);
            }
            else
            {
                // Create mode
                this.Title = "Thêm sản phẩm mới";
                CategoryComboBox.SelectedIndex = 0;
            }
        }

        public bool Validate(out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                errorMessage = "Tên sản phẩm không được để trống";
                return false;
            }

            if (string.IsNullOrWhiteSpace(SkuTextBox.Text))
            {
                errorMessage = "SKU không được để trống";
                return false;
            }

            if (PriceNumberBox.Value <= 0)
            {
                errorMessage = "Giá nhập phải lớn hơn 0";
                return false;
            }

            if (StockNumberBox.Value < 0)
            {
                errorMessage = "Tồn kho không được âm";
                return false;
            }

            if (CategoryComboBox.SelectedItem == null)
            {
                errorMessage = "Vui lòng chọn danh mục";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        public CreateProductModel GetCreateModel()
        {
            var category = CategoryComboBox.SelectedItem as CategoryModel;
            return new CreateProductModel
            {
                Name = NameTextBox.Text,
                Sku = SkuTextBox.Text,
                ImportPrice = (int)PriceNumberBox.Value,
                Count = (int)StockNumberBox.Value,
                Description = string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ? null : DescriptionTextBox.Text,
                CategoryId = category?.Id,
                Images = _images.ToList()
            };
        }

        public UpdateProductModel GetUpdateModel()
        {
            var category = CategoryComboBox.SelectedItem as CategoryModel;
            return new UpdateProductModel
            {
                Id = _product!.Id,
                Name = NameTextBox.Text,
                Sku = SkuTextBox.Text,
                ImportPrice = (int)PriceNumberBox.Value,
                Count = (int)StockNumberBox.Value,
                Description = string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ? null : DescriptionTextBox.Text,
                CategoryId = category?.Id,
                Images = _images.ToList()
            };
        }

        private void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            var url = ImageUrlTextBox.Text?.Trim();
            if (!string.IsNullOrWhiteSpace(url))
            {
                _images.Add(url);
                ImageUrlTextBox.Text = string.Empty;
            }
        }

        private void RemoveImageButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var imageUrl = button?.Tag as string;
            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                _images.Remove(imageUrl);
            }
        }

        private async void UploadImageButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Hide previous status
                if (UploadStatusText != null)
                {
                    UploadStatusText.Visibility = Visibility.Collapsed;
                }

                // Disable button while uploading
                if (UploadImageButton != null)
                {
                    UploadImageButton.IsEnabled = false;
                    
                    // Use StackPanel for loading state
                    var loadingPanel = new StackPanel { Orientation = Orientation.Horizontal };
                    loadingPanel.Children.Add(new ProgressRing { Width = 16, Height = 16, IsActive = true, Margin = new Thickness(0, 0, 8, 0) });
                    loadingPanel.Children.Add(new TextBlock { Text = "?ang upload...", VerticalAlignment = VerticalAlignment.Center });
                    UploadImageButton.Content = loadingPanel;
                }

                // Create file picker
                var picker = new FileOpenPicker();
                
                // Get window handle for WinUI 3
                var app = Microsoft.UI.Xaml.Application.Current as App;
                var window = app?.AppHost.Services.GetRequiredService<MainWindow>();
                if (window != null)
                {
                    var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
                    WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
                }
                
                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".jpeg");
                picker.FileTypeFilter.Add(".png");
                picker.FileTypeFilter.Add(".gif");
                picker.FileTypeFilter.Add(".webp");
                picker.ViewMode = PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

                var file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    // Upload to Cloudinary
                    var cloudinaryService = App.GetService<my_shop.Application.Interfaces.ICloudinaryService>();
                    
                    // Convert WinRT stream to .NET stream
                    using var randomAccessStream = await file.OpenReadAsync();
                    using var stream = randomAccessStream.AsStream();
                    using var memoryStream = new System.IO.MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    
                    var url = await cloudinaryService.UploadImageAsync(memoryStream, file.Name);
                    
                    System.Diagnostics.Debug.WriteLine($"✅ Cloudinary returned URL: {url}");
                    System.Diagnostics.Debug.WriteLine($"🔵 Current _images count: {_images.Count}");
                    
                    if (!string.IsNullOrEmpty(url))
                    {
                        _images.Add(url);
                        System.Diagnostics.Debug.WriteLine($"✅ Added to _images. New count: {_images.Count}");
                        System.Diagnostics.Debug.WriteLine($"✅ Uploaded image: {url}");
                        
                        // Show success message
                        if (UploadStatusText != null)
                        {
                            UploadStatusText.Text = $"✅ Upload thành công: {file.Name}";
                            UploadStatusText.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Green);
                            UploadStatusText.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ URL is empty or null!");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Upload error: {ex.Message}");
                
                // Show error in status text instead of dialog
                if (UploadStatusText != null)
                {
                    var errorMsg = ex.Message.Contains("401") 
                        ? "⚠ Lỗi: Chưa cấu hình Cloudinary API key. Vui lòng kiểm tra CLOUDINARY_URL trong environment variables."
                        : $"⚠ Lỗi upload: {ex.Message}";
                    
                    UploadStatusText.Text = errorMsg;
                    UploadStatusText.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);
                    UploadStatusText.Visibility = Visibility.Visible;
                }
            }
            finally
            {
                // Re-enable button
                if (UploadImageButton != null)
                {
                    UploadImageButton.IsEnabled = true;
                    
                    // Restore original content
                    var originalPanel = new StackPanel { Orientation = Orientation.Horizontal };
                    originalPanel.Children.Add(new FontIcon { Glyph = "\uE8B7", FontSize = 16, Margin = new Thickness(0, 0, 8, 0) });
                    originalPanel.Children.Add(new TextBlock { Text = "Chọn ảnh từ máy tính" });
                    UploadImageButton.Content = originalPanel;
                }
            }
        }
    }
}

