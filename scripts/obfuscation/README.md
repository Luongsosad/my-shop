# 🔒 Obfuscation cho my-shop Application

Hệ thống obfuscation (làm rối mã nguồn) để bảo vệ ứng dụng khỏi reverse engineering và decompile.

## 📋 Mục đích

Bảo vệ mã nguồn .NET khỏi các công cụ decompile như:
- ILSpy
- dotPeek
- dnSpy
- JustDecompile

## 🛠️ Công cụ sử dụng

**ConfuserEx 2** - Free, open-source obfuscator cho .NET
- GitHub: https://github.com/mkaring/ConfuserEx
- Hỗ trợ: .NET Framework, .NET Core, .NET 5+
- Nhiều tính năng bảo vệ mạnh mẽ

## 📦 Cài đặt

### Bước 1: Tải ConfuserEx

```bash
# Chạy script setup (sẽ hướng dẫn chi tiết)
setup-confuserex.bat
```

Hoặc tải thủ công:
1. Truy cập: https://github.com/mkaring/ConfuserEx/releases
2. Tải phiên bản mới nhất (ConfuserEx 2.x)
3. Giải nén vào thư mục: `scripts\obfuscation\tools\ConfuserEx\`
4. Đảm bảo có file: `tools\ConfuserEx\Confuser.CLI.exe`

## 🚀 Sử dụng

### Cách 1: Build và Obfuscate (Khuyên dùng)

```bash
# Build Release và obfuscate tự động
build-and-obfuscate.bat
```

### Cách 2: Obfuscate riêng

```bash
# Nếu đã build Release rồi, chỉ cần obfuscate
obfuscate.bat
```

## 📁 Cấu trúc files

```
scripts/obfuscation/
├── ConfuserEx.crproj           # Cấu hình obfuscation
├── setup-confuserex.bat        # Script cài đặt ConfuserEx
├── obfuscate.bat               # Script chạy obfuscation
├── build-and-obfuscate.bat     # Build + Obfuscate
├── restore-backup.bat          # Khôi phục từ backup
├── README.md                   # File này
└── tools/
    └── ConfuserEx/             # ConfuserEx binaries (tự tải)
```

## 🔐 Các bảo vệ được áp dụng

### 1. **Anti Debug** 
Phát hiện và ngăn chặn debugger

### 2. **Anti Tamper**
Phát hiện nếu file bị chỉnh sửa

### 3. **Constants Protection**
Mã hóa các chuỗi và hằng số
- Strings → Encrypted
- Numbers → Obfuscated

### 4. **Control Flow**
Làm rối logic điều khiển
- Switch statements
- Predicates
- Intensity: 60%

### 5. **Reference Proxy**
Ẩn các method/field references

### 6. **Rename**
Đổi tên symbols
- Classes: `ProductService` → `A`
- Methods: `GetProducts` → `a`
- Variables: `productId` → `b`

### 7. **Resources Protection**
Mã hóa embedded resources

## ⚠️ Exclusions (Không obfuscate)

Các thành phần sau được loại trừ để tránh lỗi:

### ✋ ViewModels
```
my_shop.App.ViewModels.*
```
**Lý do:** Có XAML bindings, không thể rename

### ✋ Models
```
my_shop.App.Models.*
```
**Lý do:** Dùng trong data binding

### ✋ Converters
```
my_shop.App.Converters.*
```
**Lý do:** Được reference trong XAML

### ✋ DTOs
```
my_shop.Application.DTOs.*
```
**Lý do:** Dùng cho serialization (JSON/GraphQL)

### ✋ Domain Entities
```
my_shop.Domain.Entities.*
```
**Lý do:** Dùng với Entity Framework Core

### ✋ Public Interfaces
```
*.Interfaces.*
```
**Lý do:** Public APIs, không nên rename

### ✋ Entry Points
```
- my_shop.App.App
- my_shop.App.MainWindow
```
**Lý do:** Entry points của ứng dụng

## 📂 Output

Sau khi obfuscate thành công:

```
../../Obfuscated/
└── net8.0-windows10.0.19041.0/
    └── win-x64/
        ├── my-shop.App.dll           # ✅ Obfuscated
        ├── my-shop.Application.dll   # ✅ Obfuscated
        ├── my-shop.Domain.dll        # ✅ Obfuscated
        ├── my-shop.Infrastructure.dll # ✅ Obfuscated
        ├── my-shop.Shared.dll        # ✅ Obfuscated
        └── ...                       # Các DLL dependencies khác
```

### 2. API (Release/my-shop.API/)
- `my-shop.API.dll`           # ✅ Obfuscated
- `my-shop.Application.dll`   # ✅ Obfuscated
- `my-shop.Domain.dll`        # ✅ Obfuscated
- `my-shop.Infrastructure.dll` # ✅ Obfuscated
```

## 🔄 Backup & Restore

### Backup tự động
Mỗi lần obfuscate, files gốc được backup tự động vào:
```
../../Backup/YYYYMMDD_HHMMSS/
```

### Khôi phục từ backup
```bash
# Nếu có vấn đề, restore lại files gốc
restore-backup.bat
```

## ✅ Workflow khuyên dùng

### Development
```bash
# Build Debug bình thường
dotnet build
```

### Release/Distribution
```bash
# 1. Build và obfuscate
cd scripts\obfuscation
build-and-obfuscate.bat

# 2. Test obfuscated build
# Chạy thử ứng dụng từ thư mục Obfuscated

# 3. Nếu OK → Copy sang thư mục publish/release
# Nếu lỗi → restore-backup.bat
```

## 🧪 Testing

Sau khi obfuscate, **bắt buộc** phải test:

### ✅ Checklist
- [ ] Ứng dụng khởi động bình thường
- [ ] Đăng nhập hoạt động
- [ ] XAML bindings hoạt động (UI hiển thị đúng)
- [ ] Navigation giữa các màn hình
- [ ] CRUD operations (Create, Read, Update, Delete)
- [ ] GraphQL API calls
- [ ] Database connections

### 🔍 Kiểm tra obfuscation
Dùng decompiler để kiểm tra:
```bash
# Tải ILSpy: https://github.com/icsharpcode/ILSpy
# Mở file: Obfuscated/net8.0-windows10.0.19041.0/win-x64/my-shop.App.dll
# → Code phải khó đọc, nhiều ký tự lạ
```

## 🎯 Kết quả mong đợi

### Trước obfuscation
```csharp
public class ProductService
{
    public List<Product> GetProducts()
    {
        return _repository.GetAll();
    }
}
```

### Sau obfuscation (decompiled)
```csharp
public class A
{
    public List<B> a()
    {
        return this.b.c();
    }
}
```

## ⚡ Performance

Obfuscation có thể ảnh hưởng hiệu năng:
- Startup time: +5-10%
- Runtime performance: +2-5%
- File size: +10-20%

**Trade-off:** Bảo mật > Hiệu năng nhỏ

## 🐛 Troubleshooting

### Lỗi: "ConfuserEx not found"
```bash
# Chạy setup
setup-confuserex.bat

# Hoặc tải thủ công từ GitHub
```

### Lỗi: "Release build not found"
```bash
# Build Release trước
dotnet build -c Release src\my-shop.App\my-shop.App.csproj
```

### Lỗi: App không khởi động sau obfuscate
```bash
# 1. Restore backup
restore-backup.bat

# 2. Kiểm tra ConfuserEx.crproj
# 3. Thêm exclusions cho các namespace bị lỗi
```

### Lỗi: XAML binding không hoạt động
Thêm exclusion trong `ConfuserEx.crproj`:
```xml
<rule pattern="namespace('YourNamespace')" inherit="false">
  <protection id="rename" exclude="true" />
</rule>
```

## 📚 Tài liệu tham khảo

- ConfuserEx Wiki: https://github.com/mkaring/ConfuserEx/wiki
- ConfuserEx Protections: https://github.com/mkaring/ConfuserEx/wiki/Protections

## 📝 Notes

- ⚠️ **KHÔNG** commit thư mục `tools/` và `Obfuscated/` vào Git
- ⚠️ **KHÔNG** obfuscate khi debug
- ✅ Chỉ obfuscate bản Release cuối cùng trước khi deploy
- ✅ Luôn test kỹ sau khi obfuscate
- ✅ Giữ backup của bản gốc

## 🔐 Security Notes

Obfuscation **KHÔNG** phải là mã hóa 100%:
- Vẫn có thể decompile, nhưng rất khó đọc
- Tăng thời gian và độ khó cho attacker
- Kết hợp với các biện pháp bảo mật khác:
  - Code signing
  - License validation
  - Runtime checks
  - Server-side validation

## 📄 License

ConfuserEx: MIT License
my-shop: [Your License]

---

**Tạo bởi:** Obfuscation Setup Script
**Ngày:** 2026-01-20
**Version:** 1.0
