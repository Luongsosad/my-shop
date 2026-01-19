using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace my_shop.App.Services.EmailService
{
    public interface IEmailService
    {
        Task<bool> SendActivationCodeAsync(string toEmail, string toName, string activationCode);
    }

    public class AutoEmailService : IEmailService
    {
        private readonly string _gmailEmail;
        private readonly string _gmailAppPassword;

        public AutoEmailService(string gmailEmail, string gmailAppPassword)
        {
            _gmailEmail = gmailEmail;
            _gmailAppPassword = gmailAppPassword;
        }

        public async Task<bool> SendActivationCodeAsync(string toEmail, string toName, string activationCode)
        {
            try
            {
                Debug.WriteLine($"🔵 [AutoEmailService] Sending email to {toEmail}...");

                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(_gmailEmail, "My Shop System");
                    message.To.Add(new MailAddress(toEmail, toName));
                    message.Subject = "My Shop - Mã Kích Hoạt Phần Mềm 🔐";
                    message.IsBodyHtml = true;
                    message.Body = CreateEmailBody(toName, activationCode);

                    using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtpClient.Credentials = new NetworkCredential(_gmailEmail, _gmailAppPassword);
                        smtpClient.EnableSsl = true;
                        smtpClient.Timeout = 10000; // 10 seconds

                        await smtpClient.SendMailAsync(message);
                        Debug.WriteLine($"✅ [AutoEmailService] Email sent successfully to {toEmail}");
                        return true;
                    }
                }
            }
            catch (SmtpException ex)
            {
                Debug.WriteLine($"❌ [AutoEmailService] SMTP Error: {ex.StatusCode}");
                Debug.WriteLine($"❌ Message: {ex.Message}");

                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"❌ Inner Exception: {ex.InnerException.Message}");
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ [AutoEmailService] Error: {ex.Message}");
                Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        private string CreateEmailBody(string toName, string activationCode)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{
            font-family: 'Segoe UI', Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            margin: 0;
            padding: 0;
            background-color: #f5f5f5;
        }}
        .email-wrapper {{
            max-width: 600px;
            margin: 20px auto;
            background: white;
        }}
        .header {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 40px 30px;
            text-align: center;
        }}
        .header h1 {{
            margin: 0;
            font-size: 28px;
            font-weight: 600;
        }}
        .header p {{
            margin: 10px 0 0 0;
            opacity: 0.9;
            font-size: 14px;
        }}
        .content {{
            padding: 40px 30px;
        }}
        .greeting {{
            font-size: 18px;
            color: #333;
            margin-bottom: 20px;
        }}
        .activation-code-box {{
            background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
            border: 3px dashed #667eea;
            border-radius: 12px;
            padding: 30px;
            text-align: center;
            margin: 30px 0;
            box-shadow: 0 4px 6px rgba(0,0,0,0.1);
        }}
        .code-label {{
            font-size: 14px;
            color: #666;
            margin-bottom: 10px;
            text-transform: uppercase;
            letter-spacing: 1px;
        }}
        .activation-code {{
            font-size: 36px;
            font-weight: bold;
            letter-spacing: 4px;
            font-family: 'Courier New', monospace;
            color: #667eea;
            margin: 10px 0;
            word-break: break-all;
        }}
        .instructions {{
            background: #f8f9fa;
            border-radius: 8px;
            padding: 25px;
            margin: 25px 0;
        }}
        .instructions h3 {{
            color: #667eea;
            margin-top: 0;
            font-size: 16px;
        }}
        .instructions ol {{
            margin: 15px 0;
            padding-left: 20px;
        }}
        .instructions li {{
            margin: 10px 0;
            line-height: 1.8;
        }}
        .warning {{
            background: #fff3cd;
            border-left: 4px solid #ffc107;
            padding: 15px 20px;
            margin: 25px 0;
            border-radius: 4px;
        }}
        .warning strong {{
            color: #856404;
        }}
        .footer {{
            background: #f8f9fa;
            text-align: center;
            padding: 30px;
            border-top: 1px solid #dee2e6;
        }}
        .footer p {{
            margin: 5px 0;
            font-size: 12px;
            color: #777;
        }}
        .footer strong {{
            color: #333;
        }}
    </style>
</head>
<body>
    <div class='email-wrapper'>
        <div class='header'>
            <h1>🔐 My Shop System</h1>
            <p>Mã Kích Hoạt Phần Mềm</p>
        </div>
        
        <div class='content'>
            <div class='greeting'>
                Xin chào <strong>{toName}</strong>,
            </div>
            
            <p>
                Cảm ơn bạn đã sử dụng <strong>My Shop Management System</strong>. 
                Dưới đây là mã kích hoạt phần mềm của bạn:
            </p>
            
            <div class='activation-code-box'>
                <div class='code-label'>Mã Kích Hoạt Của Bạn</div>
                <div class='activation-code'>{activationCode}</div>
            </div>
            
            <div class='instructions'>
                <h3>📋 Hướng Dẫn Kích Hoạt:</h3>
                <ol>
                    <li>Mở phần mềm <strong>My Shop</strong></li>
                    <li>Vào menu <strong>Settings</strong> (Cài đặt)</li>
                    <li>Nhập mã kích hoạt ở trên vào ô <strong>Activation Code</strong></li>
                    <li>Nhấn nút <strong>Activate Now</strong> để hoàn tất</li>
                </ol>
            </div>
            
            <div class='warning'>
                <strong>⚠️ Lưu Ý Quan Trọng:</strong>
                <ul style='margin: 10px 0; padding-left: 20px;'>
                    <li>Mã kích hoạt này là duy nhất cho tài khoản của bạn</li>
                    <li>Vui lòng giữ bảo mật và không chia sẻ cho người khác</li>
                    <li>Mỗi mã chỉ có thể sử dụng một lần</li>
                </ul>
            </div>
            
            <p style='margin-top: 30px; color: #666; font-size: 14px;'>
                Nếu bạn không yêu cầu mã kích hoạt này, vui lòng bỏ qua email này.
            </p>
        </div>
        
        <div class='footer'>
            <p><strong>My Shop Management System</strong></p>
            <p>Hệ Thống Quản Lý Bán Hàng Toàn Diện</p>
            <p style='margin-top: 15px;'>© 2026 My Shop. All rights reserved.</p>
            <p style='color: #999; margin-top: 10px;'>
                Email này được gửi tự động, vui lòng không trả lời.
            </p>
        </div>
    </div>
</body>
</html>";
        }
    }
}