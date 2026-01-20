# 🔒 Obfuscation cho my-shop - Quick Start

## 🚀 Bắt đầu nhanh (3 bước)

### 1️⃣ Cài đặt ConfuserEx

```bash
cd scripts\obfuscation
setup-confuserex.bat
```

Làm theo hướng dẫn:
- Tải ConfuserEx 2 từ GitHub
- Giải nén vào `scripts\obfuscation\tools\ConfuserEx\`

### 2️⃣ Build và Obfuscate

```bash
build-and-obfuscate.bat
```

# Run in Antigravity
```bash 
cd 'd:\Windows\my-shop\scripts\obfuscation'
cmd /c obfuscate.bat
```

### 3️⃣ Kiểm tra kết quả

Files đã obfuscate ở: `../../Obfuscated/`

## ✅ Done!

Xem [README.md](README.md) để biết chi tiết.

## 📊 Kết quả

**Trước:**
```csharp
public class ProductService {
    public List<Product> GetProducts() { ... }
}
```

**Sau (decompiled):**
```csharp
public class A {
    public List<B> a() { ... }
}
```

---

Có vấn đề? Xem [Troubleshooting](README.md#-troubleshooting)
