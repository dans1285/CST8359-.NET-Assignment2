namespace EventSignup.Services
{
    public interface IBlobService
    {
        Task<string> UploadFileAsync(IFormFile file, string containerName);
    }
}
