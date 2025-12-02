# ?? MY SHOP - H? th?ng qu?n lý c?a hàng

D? án qu?n lý c?a hàng s? d?ng WinUI 3, .NET 8, Entity Framework Core, GraphQL API v?i ki?n trúc Clean Architecture.

---

## ?? M?C L?C

1. [T?ng quan ki?n trúc](#-t?ng-quan-ki?n-trúc)
2. [Công ngh? s? d?ng](#-công-ngh?-s?-d?ng)
3. [C?u trúc th? m?c](#-c?u-trúc-th?-m?c)
4. [Cài ??t và ch?y](#-cài-??t-và-ch?y)
5. [GraphQL API](#-graphql-api)
6. [Dependency Injection](#-dependency-injection)
7. [MVVM Pattern](#-mvvm-pattern)
8. [Responsive Layout](#-responsive-layout)
9. [Plugin Architecture](#-plugin-architecture)
10. [H??ng d?n phát tri?n](#-h??ng-d?n-phát-tri?n)

---

## ??? T?NG QUAN KI?N TRÚC

D? án ???c xây d?ng theo **Clean Architecture** v?i các layer:

```
???????????????????????????????????????????
?         my-shop.App (WinUI 3)          ?  ? Presentation Layer
?  Views + ViewModels + DI Container     ?
???????????????????????????????????????????
                  ?
???????????????????????????????????????????
?       my-shop.Application              ?  ? Application Layer
?     Business Logic / Use Cases         ?
???????????????????????????????????????????
                  ?
???????????????????????????????????????????
?         my-shop.Domain                 ?  ? Domain Layer
?    Entities + Interfaces (Core)        ?
???????????????????????????????????????????
                  ?
???????????????????????????????????????????
?      my-shop.Infrastructure            ?  ? Infrastructure Layer
?   DbContext + EF Core + Repositories   ?
???????????????????????????????????????????
                  ?
???????????????????????????????????????????
?          my-shop.API                   ?  ? API Layer
?      GraphQL Server (HotChocolate)     ?
???????????????????????????????????????????
```

---

## ??? CÔNG NGH? S? D?NG

### **Backend & API**
| Công ngh? | Version | M?c ?ích |
|-----------|---------|----------|
| .NET | 8.0 | Framework chính |
| HotChocolate.AspNetCore | 15.1.11 | GraphQL Server |
| Entity Framework Core | 8.0.10 | ORM |
| SQLite | 8.0.10 | Database |

### **Frontend (WinUI 3)**
| Công ngh? | Version | M?c ?ích |
|-----------|---------|----------|
| WinUI 3 | 1.8.x | UI Framework |
| CommunityToolkit.Mvvm | 8.4.0 | MVVM Pattern |
| Microsoft.Extensions.Hosting | 9.0.10 | Dependency Injection |

### **Architecture Patterns**
- ? **Clean Architecture** - Tách bi?t các layer
- ? **MVVM** - Model-View-ViewModel cho UI
- ? **Dependency Injection** - IoC Container
- ? **Repository Pattern** - Data access abstraction
- ? **Plugin Architecture** - Extensible via JSON

---

## ?? C?U TRÚC TH? M?C

```
my-shop/
??? src/
?   ??? my-shop.API/                    # GraphQL API Server
?   ?   ??? Program.cs                  # API startup & configuration
?   ?   ??? appsettings.json            # API settings
?   ?   ??? my-shop.API.csproj          # API project file
?   ?
?   ??? my-shop.App/                    # WinUI 3 Application
?   ?   ??? App.xaml.cs                 # DI Container setup
?   ?   ??? MainWindow.xaml             # Main window
?   ?   ??? Views/                      # XAML UI Pages
?   ?   ?   ??? DashboardView.xaml
?   ?   ?   ??? ProductsScreenView.xaml
?   ?   ?   ??? OrdersScreenView.xaml
?   ?   ?   ??? ...
?   ?   ??? ViewModels/                 # MVVM ViewModels
?   ?   ?   ??? DashboardViewModel.cs
?   ?   ?   ??? ProductsScreenViewModel.cs
?   ?   ?   ??? ...
?   ?   ??? Services/                   # Business services
?   ?       ??? IDataService.cs
?   ?       ??? SQLiteService.cs
?   ?       ??? PostgresService.cs
?   ?
?   ??? my-shop.Domain/                 # Domain Entities
?   ?   ??? Entities/
?   ?       ??? Product.cs              # Product entity
?   ?       ??? Category.cs             # Category entity
?   ?       ??? Order.cs                # Order entity
?   ?       ??? OrderItem.cs            # Order item entity
?   ?
?   ??? my-shop.Infrastructure/         # Data Access Layer
?   ?   ??? Db/
?   ?   ?   ??? MyShopDbContext.cs      # EF Core DbContext
?   ?   ??? Repositories/               # Repository implementations
?   ?
?   ??? my-shop.Application/            # Business Logic
?   ?   ??? Services/                   # Application services
?   ?
?   ??? my-shop.Shared/                 # Shared code
?
??? tests/
    ??? my-shop.UnitTests/              # Unit tests
```

---

## ?? CÀI ??T VÀ CH?Y

### **Yêu c?u h? th?ng**
- Windows 10/11 (version 1809+)
- .NET 8.0 SDK
- Visual Studio 2022 (recommended) ho?c VS Code

### **1. Clone Repository**
```bash
git clone https://github.com/Luongsosad/my-shop
cd my-shop
```

### **2. Restore packages**
```bash
dotnet restore
```

### **3. Ch?y GraphQL API**

#### Option 1: Command Line
```bash
cd src/my-shop.API
dotnet run
```

API s? ch?y t?i: `http://localhost:5000/graphql`

#### Option 2: Visual Studio
- M? solution `my-shop.sln`
- Set `my-shop.API` làm Startup Project
- Press F5 ho?c Debug ? Start Debugging

### **4. Ch?y WinUI App**

```bash
cd src/my-shop.App
dotnet build -c Debug /p:Platform=x64
```

> ?? **L?u ý**: WinUI App yêu c?u ch? ??nh Platform (x64, x86, ho?c ARM64)

Ho?c trong Visual Studio:
- Set `my-shop.App` làm Startup Project
- Ch?n Platform: x64
- Press F5

### **5. Build toàn b? solution**

```bash
# Build t?t c? projects
dotnet build

# Build specific configuration
dotnet build -c Release
```

---

## ?? GRAPHQL API

### **Endpoint**
```
http://localhost:5000/graphql
```

### **GraphQL Playground**
Truy c?p endpoint trên b?ng browser ?? m? GraphQL IDE t??ng tác.

### **Schema Overview**

```graphql
type Query {
  products(
    where: ProductFilterInput
    order: [ProductSortInput!]
  ): [Product!]!
  
  categories(
    where: CategoryFilterInput
    order: [CategorySortInput!]
  ): [Category!]!
}

type Product {
  id: Int!
  sku: String!
  name: String!
  importPrice: Int!
  count: Int!
  description: String
  categoryId: Int
  extensionData: String
  category: Category
}

type Category {
  id: Int!
  name: String!
  description: String
  extensionData: String
  products: [Product!]!
}
```

### **Query Examples**

#### 1?? L?y t?t c? Products
```graphql
query {
  products {
    id
    name
    sku
    importPrice
    count
    category {
      id
      name
    }
  }
}
```

**Response:**
```json
{
  "data": {
    "products": [
      {
        "id": 1,
        "name": "Smartphone X",
        "sku": "SPX-001",
        "importPrice": 500,
        "count": 10,
        "category": {
          "id": 1,
          "name": "Electronics"
        }
      }
    ]
  }
}
```

#### 2?? L?y Products v?i filtering
```graphql
query {
  products(
    where: { 
      importPrice: { gte: 100 }
      name: { contains: "Phone" }
    }
  ) {
    id
    name
    importPrice
  }
}
```

#### 3?? L?y Categories v?i Products
```graphql
query {
  categories {
    id
    name
    description
    products {
      id
      name
      sku
      importPrice
    }
  }
}
```

#### 4?? Sorting và Pagination
```graphql
query {
  products(
    order: { importPrice: DESC }
    take: 10
    skip: 0
  ) {
    id
    name
    importPrice
  }
}
```

### **S? d?ng GraphQL trong code C#**

#### V?i HttpClient:
```csharp
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class GraphQLClient
{
    private readonly HttpClient _httpClient;
    
    public GraphQLClient()
    {
        _httpClient = new HttpClient 
        { 
            BaseAddress = new Uri("http://localhost:5000") 
        };
    }
    
    public async Task<T> QueryAsync<T>(string query)
    {
        var request = new
        {
            query = query
        };
        
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync("/graphql", content);
        var result = await response.Content.ReadAsStringAsync();
        
        var data = JsonSerializer.Deserialize<GraphQLResponse<T>>(result);
        return data.Data;
    }
}

// Usage
var client = new GraphQLClient();
var products = await client.QueryAsync<ProductsResponse>(@"
{
  products {
    id
    name
    sku
    importPrice
  }
}
");
```

### **Test API v?i PowerShell**

```powershell
$query = @{
    query = @"
{
  products {
    id
    name
    sku
    importPrice
  }
}
"@
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/graphql" `
    -Method Post `
    -Body $query `
    -ContentType "application/json"
```

### **Test API v?i cURL**

```bash
curl -X POST http://localhost:5000/graphql \
  -H "Content-Type: application/json" \
  -d '{"query":"{ products { id name sku } }"}'
```

---

## ?? DEPENDENCY INJECTION

### **Setup trong App.xaml.cs**

```csharp
public partial class App : Application
{
    public IHost AppHost { get; private set; }

    public App()
    {
        InitializeComponent();

        // C?u hình DI Container
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Register Window
                services.AddTransient<MainWindow>();

                // Register Views
                services.AddTransient<DashboardView>();
                services.AddTransient<ProductsScreenView>();
                services.AddTransient<OrdersScreenView>();
                services.AddTransient<ConfigScreenView>();
                services.AddTransient<LoginScreenView>();
                services.AddTransient<ReportScreenView>();
                services.AddTransient<SettingsScreenView>();

                // Register ViewModels
                services.AddTransient<DashboardViewModel>();
                services.AddTransient<ProductsScreenViewModel>();
                services.AddTransient<OrdersScreenViewModel>();
                services.AddTransient<ConfigScreenViewModel>();
                services.AddTransient<LoginScreenViewModel>();
                services.AddTransient<ReportScreenViewModel>();
                services.AddTransient<SettingsScreenViewModel>();

                // Register Services
                services.AddTransient<IDataService, SQLiteService>();
                services.AddTransient<IDataService, PostgresService>();
                // Thêm services khác...
            })
            .Build();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = AppHost.Services.GetRequiredService<MainWindow>();
        _window.Activate();
    }
    
    // Helper method ?? l?y service t? b?t k? ?âu
    public static T GetService<T>() where T : class
    {
        var app = (App)Current;
        return app.AppHost.Services.GetRequiredService<T>();
    }
}
```

### **S? d?ng DI trong MainWindow**

```csharp
public sealed partial class MainWindow : Window
{
    private readonly LoginScreenView _loginScreenView;

    // Constructor injection
    public MainWindow(IHost appHost)
    {
        InitializeComponent();
        
        // L?y View t? DI container
        _loginScreenView = appHost.Services.GetRequiredService<LoginScreenView>();
        RootGrid.Children.Add(_loginScreenView);
    }
}
```

### **L?y Service t? b?t k? ?âu**

```csharp
// Trong ViewModel ho?c Code-behind
var dataService = App.GetService<IDataService>();
var products = await dataService.GetProductsAsync();
```

---

## ?? MVVM PATTERN

### **C?u trúc MVVM**

```
View (XAML) ?? ViewModel ?? Model/Service
```

### **1. View (XAML)**

**File:** `Views/ProductsScreenView.xaml`

```xaml
<Page
    x:Class="my_shop.App.Views.ProductsScreenView"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:vm="using:my_shop.App.ViewModels">
    
    <!-- DataContext binding -->
    <Page.DataContext>
        <vm:ProductsScreenViewModel />
    </Page.DataContext>
    
    <Grid>
        <!-- Binding to ViewModel properties -->
        <ListView ItemsSource="{Binding Products}">
            <ListView.ItemTemplate>
                <DataTemplate>
                    <StackPanel>
                        <TextBlock Text="{Binding Name}" />
                        <TextBlock Text="{Binding Sku}" />
                    </StackPanel>
                </DataTemplate>
            </ListView.ItemTemplate>
        </ListView>
    </Grid>
</Page>
```

### **2. ViewModel**

**File:** `ViewModels/ProductsScreenViewModel.cs`

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace my_shop.App.ViewModels
{
    public partial class ProductsScreenViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        // Observable property - t? ??ng notify khi thay ??i
        [ObservableProperty]
        private ObservableCollection<Product> _products;

        [ObservableProperty]
        private Product _selectedProduct;

        [ObservableProperty]
        private bool _isLoading;

        // Constructor injection
        public ProductsScreenViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Products = new ObservableCollection<Product>();
            
            // Load data khi kh?i t?o
            _ = LoadProductsAsync();
        }

        // Command - t? ??ng t?o LoadProductsCommand
        [RelayCommand]
        private async Task LoadProductsAsync()
        {
            IsLoading = true;
            try
            {
                var products = await _dataService.GetProductsAsync();
                Products.Clear();
                foreach (var product in products)
                {
                    Products.Add(product);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task AddProductAsync(Product product)
        {
            await _dataService.AddProductAsync(product);
            await LoadProductsAsync();
        }

        [RelayCommand]
        private async Task DeleteProductAsync(int productId)
        {
            await _dataService.DeleteProductAsync(productId);
            Products.Remove(Products.First(p => p.Id == productId));
        }
    }
}
```

### **3. Code-behind (View)**

**File:** `Views/ProductsScreenView.xaml.cs`

```csharp
using Microsoft.UI.Xaml.Controls;
using my_shop.App.ViewModels;

namespace my_shop.App.Views
{
    public sealed partial class ProductsScreenView : Page
    {
        public ProductsScreenViewModel ViewModel { get; }

        public ProductsScreenView(ProductsScreenViewModel viewModel)
        {
            this.InitializeComponent();
            
            // Inject ViewModel qua constructor
            ViewModel = viewModel;
            DataContext = ViewModel;
        }
    }
}
```

### **S? d?ng trong XAML**

```xaml
<!-- Binding to property -->
<TextBlock Text="{Binding SelectedProduct.Name}" />

<!-- Binding to command -->
<Button Content="Load Products" 
        Command="{Binding LoadProductsCommand}" />

<!-- Binding with loading state -->
<ProgressRing IsActive="{Binding IsLoading}" />

<!-- Two-way binding -->
<TextBox Text="{Binding SearchText, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
```

---

## ?? RESPONSIVE LAYOUT

WinUI 3 h? tr? responsive layout thông qua **AdaptiveTrigger** và **VisualStateManager**.

### **Cách ho?t ??ng**

```xaml
<Page>
    <Grid>
        <!-- VisualStateManager qu?n lý các tr?ng thái UI -->
        <VisualStateManager.VisualStateGroups>
            <VisualStateGroup x:Name="WindowSizeStates">
                
                <!-- Wide layout: >= 1024px -->
                <VisualState x:Name="WideState">
                    <VisualState.StateTriggers>
                        <AdaptiveTrigger MinWindowWidth="1024" />
                    </VisualState.StateTriggers>
                    <VisualState.Setters>
                        <!-- 2 c?t layout -->
                        <Setter Target="MainGrid.ColumnDefinitions[1].Width" Value="*" />
                        <Setter Target="ProductDetail.Visibility" Value="Visible" />
                    </VisualState.Setters>
                </VisualState>

                <!-- Medium layout: 641-1023px -->
                <VisualState x:Name="MediumState">
                    <VisualState.StateTriggers>
                        <AdaptiveTrigger MinWindowWidth="641" />
                    </VisualState.StateTriggers>
                    <VisualState.Setters>
                        <!-- 1 c?t, ?n detail -->
                        <Setter Target="MainGrid.ColumnDefinitions[1].Width" Value="0" />
                        <Setter Target="ProductDetail.Visibility" Value="Collapsed" />
                    </VisualState.Setters>
                </VisualState>

                <!-- Narrow layout: < 641px -->
                <VisualState x:Name="NarrowState">
                    <VisualState.StateTriggers>
                        <AdaptiveTrigger MinWindowWidth="0" />
                    </VisualState.StateTriggers>
                    <VisualState.Setters>
                        <!-- Mobile layout -->
                        <Setter Target="HeaderText.FontSize" Value="20" />
                        <Setter Target="MainGrid.Padding" Value="10" />
                    </VisualState.Setters>
                </VisualState>
                
            </VisualStateGroup>
        </VisualStateManager.VisualStateGroups>

        <Grid x:Name="MainGrid">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="*"/>
            </Grid.ColumnDefinitions>

            <!-- Content ? ?ây -->
            <StackPanel x:Name="ProductList" Grid.Column="0" />
            <StackPanel x:Name="ProductDetail" Grid.Column="1" />
        </Grid>
    </Grid>
</Page>
```

### **Breakpoints tiêu chu?n**

| Kích th??c | MinWidth | Layout |
|------------|----------|--------|
| Mobile | 0-640px | 1 c?t, compact |
| Tablet | 641-1023px | 1-2 c?t |
| Desktop | 1024+px | 2-3 c?t, sidebar |

### **Tips**
- S? d?ng `Grid` thay vì `StackPanel` cho responsive t?t h?n
- K?t h?p v?i `RelativePanel` cho layout linh ho?t
- Test trên nhi?u kích th??c màn hình

---

## ?? PLUGIN ARCHITECTURE

### **Concept**

D? án s? d?ng **ExtensionData** (JSON column) ?? cho phép m? r?ng ??ng mà không c?n thay ??i schema database.

### **Schema trong Database**

```sql
CREATE TABLE products (
    product_id INTEGER PRIMARY KEY,
    sku TEXT NOT NULL,
    name TEXT NOT NULL,
    import_price INTEGER DEFAULT 0,
    count INTEGER DEFAULT 0,
    category_id INTEGER,
    extension_data TEXT,  -- JSON column for plugin data
    FOREIGN KEY (category_id) REFERENCES categories(category_id)
);
```

### **Entity Definition**

```csharp
[Table("products")]
public class Product
{
    [Key]
    [Column("product_id")]
    public int Id { get; set; }

    [Required]
    [Column("name")]
    public string Name { get; set; }

    // Plugin extensible column
    [Column("extension_data", TypeName = "jsonb")]
    public string? ExtensionData { get; set; }
    
    // Other properties...
}
```

### **S? d?ng Plugin Data**

#### 1?? L?u plugin data
```csharp
// T?o product v?i extension data
var product = new Product
{
    Name = "Smartphone X",
    Sku = "SPX-001",
    ImportPrice = 500,
    Count = 10,
    // Plugin data d??i d?ng JSON
    ExtensionData = JsonSerializer.Serialize(new
    {
        Screen = "OLED",
        Battery = "5000mAh",
        Camera = "48MP",
        CustomField1 = "Value1"
    })
};

await dbContext.Products.AddAsync(product);
await dbContext.SaveChangesAsync();
```

#### 2?? ??c plugin data
```csharp
var product = await dbContext.Products.FindAsync(1);

if (!string.IsNullOrEmpty(product.ExtensionData))
{
    var extraData = JsonSerializer.Deserialize<Dictionary<string, object>>(
        product.ExtensionData
    );
    
    var screen = extraData["Screen"].ToString(); // "OLED"
    var battery = extraData["Battery"].ToString(); // "5000mAh"
}
```

#### 3?? Query qua GraphQL
```graphql
query {
  products {
    id
    name
    extensionData
  }
}
```

Response:
```json
{
  "data": {
    "products": [
      {
        "id": 1,
        "name": "Smartphone X",
        "extensionData": "{\"Screen\":\"OLED\",\"Battery\":\"5000mAh\"}"
      }
    ]
  }
}
```

### **L?i ích**

? **No schema migration needed** - Không c?n alter table khi thêm field m?i  
? **Flexible** - M?i product có th? có properties khác nhau  
? **Plugin-friendly** - Plugin có th? thêm data riêng không ?nh h??ng core  
? **Backward compatible** - Data c? v?n ho?t ??ng bình th??ng  

### **Use Cases**

1. **E-commerce variants**: Size, Color, Material cho t?ng lo?i s?n ph?m
2. **Custom attributes**: Thu?c tính riêng c?a t?ng category
3. **Plugin metadata**: Data t? plugin bên th? 3
4. **Dynamic forms**: Form fields ??ng t? admin config

---

## ????? H??NG D?N PHÁT TRI?N

### **1. T?o View và ViewModel m?i**

#### B??c 1: T?o ViewModel

```csharp
// File: ViewModels/NewFeatureViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace my_shop.App.ViewModels
{
    public partial class NewFeatureViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title = "New Feature";

        [RelayCommand]
        private void DoSomething()
        {
            // Logic here
        }
    }
}
```

#### B??c 2: T?o View

```xaml
<!-- File: Views/NewFeatureView.xaml -->
<Page
    x:Class="my_shop.App.Views.NewFeatureView"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:vm="using:my_shop.App.ViewModels">
    
    <Grid>
        <TextBlock Text="{Binding Title}" />
        <Button Content="Do Something" Command="{Binding DoSomethingCommand}" />
    </Grid>
</Page>
```

#### B??c 3: Register trong DI

```csharp
// File: App.xaml.cs
AppHost = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        // ... existing services
        
        services.AddTransient<NewFeatureView>();
        services.AddTransient<NewFeatureViewModel>();
    })
    .Build();
```

### **2. G?i GraphQL API t? ViewModel**

```csharp
public partial class ProductsScreenViewModel : ObservableObject
{
    private readonly HttpClient _httpClient;

    public ProductsScreenViewModel()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };
    }

    [RelayCommand]
    private async Task LoadProductsAsync()
    {
        var query = new
        {
            query = @"
            {
              products {
                id
                name
                sku
                importPrice
                category {
                  name
                }
              }
            }"
        };

        var json = JsonSerializer.Serialize(query);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync("/graphql", content);
        var result = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<GraphQLResponse<List<Product>>>(result);
        
        Products.Clear();
        foreach (var product in data.Data)
        {
            Products.Add(product);
        }
    }
}

// Response model
public class GraphQLResponse<T>
{
    public T Data { get; set; }
}
```

### **3. Thêm Entity m?i**

#### B??c 1: T?o Entity

```csharp
// File: Domain/Entities/Customer.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace my_shop.Domain.Entities
{
    [Table("customers")]
    public class Customer
    {
        [Key]
        [Column("customer_id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; }

        [Column("email")]
        public string Email { get; set; }

        // Plugin support
        [Column("extension_data", TypeName = "jsonb")]
        public string? ExtensionData { get; set; }
    }
}
```

#### B??c 2: Thêm vào DbContext

```csharp
// File: Infrastructure/Db/MyShopDbContext.cs
public class MyShopDbContext : DbContext
{
    // Existing DbSets...
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // C?u hình cho Customer
        modelBuilder.Entity<Customer>()
            .Property(c => c.Name)
            .IsRequired();
    }
}
```

#### B??c 3: T?o Migration (n?u dùng migrations)

```bash
cd src/my-shop.Infrastructure
dotnet ef migrations add AddCustomerTable
dotnet ef database update
```

#### B??c 4: Thêm vào GraphQL Query

```csharp
// File: API/Program.cs
public class Query
{
    // Existing queries...
    
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Customer> GetCustomers([Service] MyShopDbContext context) =>
        context.Customers;
}
```

### **4. Thêm Service m?i**

```csharp
// File: Services/ICustomerService.cs
public interface ICustomerService
{
    Task<List<Customer>> GetAllCustomersAsync();
    Task<Customer> GetCustomerByIdAsync(int id);
    Task AddCustomerAsync(Customer customer);
}

// File: Services/CustomerService.cs
public class CustomerService : ICustomerService
{
    private readonly HttpClient _httpClient;

    public CustomerService()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };
    }

    public async Task<List<Customer>> GetAllCustomersAsync()
    {
        var query = new { query = "{ customers { id name email } }" };
        var json = JsonSerializer.Serialize(query);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync("/graphql", content);
        var result = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<GraphQLResponse<List<Customer>>>(result);
        
        return data.Data;
    }
    
    // Implement other methods...
}

// Register trong DI
services.AddTransient<ICustomerService, CustomerService>();
```

### **5. Debug và Troubleshooting**

#### Ki?m tra DI registration
```csharp
// Trong App.xaml.cs
var services = AppHost.Services;
var viewModel = services.GetService<ProductsScreenViewModel>();
if (viewModel == null)
{
    Debug.WriteLine("ViewModel not registered!");
}
```

#### Test GraphQL query
```powershell
# Test nhanh query
$query = '{"query":"{ products { id name } }"}'
Invoke-RestMethod -Uri http://localhost:5000/graphql -Method Post -Body $query -ContentType "application/json"
```

#### Ki?m tra database
```bash
# Xem tables
sqlite3 src/my-shop.API/myshop.db ".tables"

# Xem data
sqlite3 src/my-shop.API/myshop.db "SELECT * FROM products;"
```

---

## ?? TÀI LI?U THAM KH?O

### **Official Documentation**
- [WinUI 3 Documentation](https://learn.microsoft.com/en-us/windows/apps/winui/winui3/)
- [.NET MAUI & WinUI](https://learn.microsoft.com/en-us/dotnet/maui/)
- [HotChocolate GraphQL](https://chillicream.com/docs/hotchocolate/v13)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)

### **Tutorials**
- [Building WinUI 3 Apps](https://learn.microsoft.com/en-us/windows/apps/get-started/)
- [GraphQL Best Practices](https://graphql.org/learn/best-practices/)
- [Clean Architecture in .NET](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)

---

## ? FAQ

### **Q: Làm sao ?? thay ??i database t? SQLite sang PostgreSQL?**
A: Thay ??i trong `Program.cs` c?a API:
```csharp
// Thay vì UseSqlite
builder.Services.AddDbContext<MyShopDbContext>(options =>
    options.UseNpgsql("Host=localhost;Database=myshop;Username=user;Password=pass"));
```

### **Q: Làm sao ?? thêm Authentication?**
A: Thêm vào API:
```csharp
builder.Services
    .AddAuthentication()
    .AddJwtBearer();

builder.Services
    .AddGraphQLServer()
    .AddAuthorization();
```

### **Q: App không build ???c do l?i Platform?**
A: Ch? ??nh platform khi build:
```bash
dotnet build /p:Platform=x64
```

### **Q: GraphQL không tr? v? data?**
A: Ki?m tra:
1. API có ?ang ch?y không?
2. Database có data không?
3. Query syntax ?úng ch?a?
4. Xem logs trong console API

---

## ?? LICENSE

MIT License - Free to use and modify.

---

## ?? CONTRIBUTORS

D? án ???c phát tri?n b?i team MyShop.

---

## ?? GETTING STARTED QUICK

```bash
# 1. Clone repo
git clone https://github.com/Luongsosad/my-shop
cd my-shop

# 2. Start API
cd src/my-shop.API
dotnet run

# 3. Trong terminal khác, test API
curl -X POST http://localhost:5000/graphql \
  -H "Content-Type: application/json" \
  -d '{"query":"{ products { name } }"}'

# 4. Build và ch?y App
cd ../my-shop.App
dotnet build /p:Platform=x64
```

**Enjoy coding! ??**