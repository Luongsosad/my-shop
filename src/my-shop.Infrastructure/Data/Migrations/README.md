# 📦 Database Migrations

## 📋 Tổng quan

Thư mục này chứa tất cả các EF Core migrations để quản lý schema của database. Migrations giúp:
- ✅ Version control cho database schema
- ✅ Tự động apply changes khi deploy
- ✅ Rollback về version trước nếu cần
- ✅ Đồng bộ database giữa các môi trường (dev, staging, production)

## 📂 Cấu trúc

```
Migrations/
├── 20251212023015_InitialCreate.cs              # Migration đầu tiên - tạo toàn bộ tables
├── 20251212023015_InitialCreate.Designer.cs     # Metadata cho InitialCreate
├── 20251212024507_AddProductImages.cs           # Migration thêm trường Images
├── 20251212024507_AddProductImages.Designer.cs  # Metadata cho AddProductImages
├── MyShopDbContextModelSnapshot.cs              # Snapshot của schema hiện tại
└── README.md                                    # File này
```

## 🚀 Cách sử dụng

### 1️⃣ Tạo Migration Mới

Khi bạn thay đổi entity (thêm/sửa/xóa property), tạo migration:

```bash
# Windows
cd src\my-shop.Infrastructure
C:\Users\LUONG\.dotnet\tools\dotnet-ef.exe migrations add TenMigration --startup-project ..\my-shop.API --output-dir Data\Migrations

# Linux/Mac
cd src/my-shop.Infrastructure
dotnet ef migrations add TenMigration --startup-project ../my-shop.API --output-dir Data/Migrations
```

**Ví dụ tên migration:**
- `AddProductImages` - Thêm trường Images vào Product
- `AddOrderStatus` - Thêm trường Status vào Order
- `CreateIndexOnSKU` - Tạo index cho SKU
- `RemoveObsoleteField` - Xóa field không dùng

### 2️⃣ Apply Migration

**Tự động khi chạy app:**
```bash
cd src/my-shop.API
dotnet run
```
→ App sẽ tự động detect và apply pending migrations

**Thủ công:**
```bash
cd src/my-shop.Infrastructure
dotnet ef database update --startup-project ../my-shop.API
```

### 3️⃣ Kiểm tra Status

```bash
# Xem tất cả migrations
dotnet ef migrations list --startup-project ../my-shop.API

# Kiểm tra pending migrations
dotnet ef database update --startup-project ../my-shop.API --dry-run
```

### 4️⃣ Rollback Migration

```bash
# Rollback về migration trước đó
dotnet ef database update TenMigrationTruocDo --startup-project ../my-shop.API

# Rollback tất cả (xóa toàn bộ schema)
dotnet ef database update 0 --startup-project ../my-shop.API
```

### 5️⃣ Xóa Migration Chưa Apply

```bash
# Xóa migration cuối cùng (nếu chưa apply)
dotnet ef migrations remove --startup-project ../my-shop.API --force
```

## 📝 Migrations Hiện tại

### `20251212023015_InitialCreate`
**Tạo bởi:** System  
**Ngày:** 12/12/2025 02:30:15  
**Mô tả:** Migration đầu tiên tạo toàn bộ database schema

**Tables được tạo:**
- `categories` - Danh mục sản phẩm
  - `category_id` (PK)
  - `name` (varchar 255)
  - `description` (text)
  - `extension_data` (jsonb)

- `products` - Sản phẩm
  - `product_id` (PK)
  - `sku` (varchar 100)
  - `name` (varchar 255)
  - `import_price` (integer)
  - `count` (integer)
  - `description` (text)
  - `category_id` (FK → categories)
  - `extension_data` (jsonb)

- `orders` - Đơn hàng
  - `order_id` (PK)
  - `created_time` (timestamp with time zone)
  - `final_price` (integer)
  - `extension_data` (jsonb)

- `order_items` - Chi tiết đơn hàng
  - `order_item_id` (PK)
  - `order_id` (FK → orders)
  - `product_id` (FK → products)
  - `quantity` (integer)
  - `price` (integer)

**Indexes:**
- `IX_order_items_order_id`
- `IX_order_items_product_id`
- `IX_products_category_id`

---

### `20251212024507_AddProductImages`
**Tạo bởi:** System  
**Ngày:** 12/12/2025 02:45:07  
**Mô tả:** Thêm trường lưu danh sách ảnh sản phẩm

**Thay đổi:**
- ✅ Thêm column `images` (jsonb) vào table `products`
- Cho phép lưu mảng URL ảnh dưới dạng JSON

**SQL được execute:**
```sql
ALTER TABLE products ADD images jsonb;
```

## ⚠️ Lưu ý quan trọng

### ✅ Best Practices

1. **Tên migration rõ ràng**
   ```
   ✅ AddProductImages
   ✅ CreateUserEmailIndex
   ✅ RemoveObsoleteOrderStatus
   ❌ Update
   ❌ Fix
   ❌ Change1
   ```

2. **Kiểm tra trước khi apply**
   - Review file migration được generate
   - Test trên database local trước
   - Backup production database trước khi deploy

3. **Không chỉnh sửa migration đã apply**
   - Nếu migration đã được apply vào production, KHÔNG chỉnh sửa
   - Tạo migration mới để sửa lỗi

4. **Luôn commit migrations vào Git**
   ```bash
   git add src/my-shop.Infrastructure/Data/Migrations/
   git commit -m "feat: add product images migration"
   ```

### 🚨 Xử lý sự cố

**Migration bị lỗi khi apply:**
```bash
# Xem chi tiết lỗi
dotnet ef database update --startup-project ../my-shop.API --verbose

# Nếu cần rollback
dotnet ef database update MigrationTruocDo --startup-project ../my-shop.API
```

**Conflict giữa 2 migrations:**
```bash
# Developer A và B đều tạo migration mới
# → Cần merge và tạo lại migration
dotnet ef migrations remove --force
git pull
dotnet ef migrations add TenMigrationMoi --startup-project ../my-shop.API
```

**Database out of sync:**
```bash
# Xem migrations đã apply
dotnet ef migrations list --startup-project ../my-shop.API

# Force sync (CẢNH BÁO: có thể mất data)
dotnet ef database drop --startup-project ../my-shop.API --force
dotnet ef database update --startup-project ../my-shop.API
```

## 🔧 Workflow Development

### Khi phát triển feature mới:

1. **Thay đổi Entity**
   ```csharp
   // Product.cs
   public List<string>? Images { get; set; }
   ```

2. **Tạo Migration**
   ```bash
   dotnet ef migrations add AddProductImages
   ```

3. **Review Migration**
   - Kiểm tra file `*_AddProductImages.cs`
   - Đảm bảo logic Up/Down đúng

4. **Apply Local**
   ```bash
   dotnet run  # Tự động apply
   ```

5. **Test**
   - Kiểm tra column mới trong database
   - Test CRUD operations

6. **Commit**
   ```bash
   git add .
   git commit -m "feat: add product images field"
   git push
   ```

### Khi deploy production:

1. **Backup database trước**
2. **Deploy code mới**
3. **Restart app** → Migrations tự động apply
4. **Verify** database schema đã update

## 📚 Tài liệu tham khảo

- [EF Core Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [Migration SQL Generation](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/managing)
- [Team Environments](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/teams)

## 🆘 Cần giúp đỡ?

Liên hệ team database hoặc xem documentation tại `docs/database/README.md`
