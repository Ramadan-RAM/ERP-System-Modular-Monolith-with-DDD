using Microsoft.AspNetCore.Http;

namespace ERPSys.SharedKernel.Utils
{
    public class UploadFile
    {
        // Upluade Image
        public static async Task<byte[]> ProcessUploadedFile(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    return ms.ToArray();
                }
            }

            return null;
        }
    }
}
