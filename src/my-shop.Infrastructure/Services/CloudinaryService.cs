using my_shop.Application.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;

namespace my_shop.Infrastructure.Services;

public class CloudinaryService : ICloudinaryService
{
    private readonly string _cloudName;
    private readonly string _apiKey;
    private readonly string _apiSecret;
    private readonly HttpClient _httpClient;

    public CloudinaryService(string cloudinaryUrl)
    {
        // Parse cloudinary://api_key:api_secret@cloud_name
        var uri = new Uri(cloudinaryUrl);
        _cloudName = uri.Host;
        _apiKey = uri.UserInfo.Split(':')[0];
        _apiSecret = uri.UserInfo.Split(':')[1];
        _httpClient = new HttpClient();
    }

    public async Task<string> UploadImageAsync(Stream imageStream, string fileName)
    {
        System.Diagnostics.Debug.WriteLine($"🔵 CloudinaryService: Uploading to cloud: {_cloudName}");
        System.Diagnostics.Debug.WriteLine($"🔵 CloudinaryService: File name: {fileName}");
        
        var boundary = "----CloudinaryBoundary" + Guid.NewGuid().ToString("N");
        using var content = new MultipartFormDataContent(boundary);
        
        var streamContent = new StreamContent(imageStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = "\"file\"",
            FileName = $"\"{fileName}\""
        };
        content.Add(streamContent);
        
        var presetContent = new StringContent("ml_default");
        presetContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = "\"upload_preset\""
        };
        content.Add(presetContent);

        System.Diagnostics.Debug.WriteLine($"🔵 CloudinaryService: Posting to https://api.cloudinary.com/v1_1/{_cloudName}/image/upload");
        var response = await _httpClient.PostAsync(
            $"https://api.cloudinary.com/v1_1/{_cloudName}/image/upload",
            content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"❌ CloudinaryService: Upload failed - {errorContent}");
            throw new InvalidOperationException($"Cloudinary upload failed: {response.StatusCode} - {errorContent}");
        }

        var result = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(result);
        
        return jsonDoc.RootElement.GetProperty("secure_url").GetString() 
            ?? throw new InvalidOperationException("Failed to get image URL from Cloudinary");
    }

    public async Task<List<string>> UploadImagesAsync(List<Stream> imageStreams, List<string> fileNames)
    {
        if (imageStreams.Count != fileNames.Count)
            throw new ArgumentException("Number of streams must match number of file names");

        var uploadTasks = imageStreams
            .Select((stream, index) => UploadImageAsync(stream, fileNames[index]))
            .ToList();

        return (await Task.WhenAll(uploadTasks)).ToList();
    }

    public async Task<bool> DeleteImageAsync(string publicId)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        
        var parameters = new SortedDictionary<string, string>
        {
            { "public_id", publicId },
            { "timestamp", timestamp }
        };
        
        var signature = GenerateSignature(parameters);
        
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(publicId), "public_id");
        content.Add(new StringContent(_apiKey), "api_key");
        content.Add(new StringContent(timestamp), "timestamp");
        content.Add(new StringContent(signature), "signature");

        var response = await _httpClient.PostAsync(
            $"https://api.cloudinary.com/v1_1/{_cloudName}/image/destroy",
            content);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteImagesAsync(List<string> publicIds)
    {
        var deleteTasks = publicIds.Select(DeleteImageAsync).ToList();
        var results = await Task.WhenAll(deleteTasks);
        return results.All(r => r);
    }

    private string GenerateSignature(SortedDictionary<string, string> parameters)
    {
        // Build string to sign: param1=value1&param2=value2 (sorted alphabetically) + API_SECRET
        var paramString = string.Join("&", parameters.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        var stringToSign = paramString + _apiSecret;
        
        // Generate SHA1 hash
        using var sha1 = System.Security.Cryptography.SHA1.Create();
        var hash = sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(stringToSign));
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}
