namespace my_shop.Application.Interfaces;

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(Stream imageStream, string fileName);
    Task<List<string>> UploadImagesAsync(List<Stream> imageStreams, List<string> fileNames);
    Task<bool> DeleteImageAsync(string publicId);
    Task<bool> DeleteImagesAsync(List<string> publicIds);
}
