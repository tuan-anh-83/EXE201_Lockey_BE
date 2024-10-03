using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EXE201_Lockey.Services
{
    public class FirebaseService
    {
        private readonly string _bucketName = "gs://lockey-exe.appspot.com"; // Thay thế bằng bucket của bạn
        private readonly StorageClient _storageClient;

        public FirebaseService()
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("Credentials/serviceAccountKey.json"),
                });
            }

            // Tạo StorageClient để tương tác với Firebase Storage
            _storageClient = StorageClient.Create();
        }

        // Phương thức upload file lên Firebase Storage
        public async Task<string> UploadFileAsync(string localFilePath, string storageFilePath)
        {
            try
            {
                // Đọc file từ đường dẫn local
                using var fileStream = File.OpenRead(localFilePath);

                // Upload file lên Firebase Storage (Google Cloud Storage)
                var storageObject = await _storageClient.UploadObjectAsync(_bucketName, storageFilePath, null, fileStream);

                // Trả về URL của file đã upload
                return $"https://storage.googleapis.com/{_bucketName}/{storageObject.Name}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading file: {ex.Message}");
                return null;
            }
        }

        // Phương thức xóa file từ Firebase Storage
        public async Task<bool> DeleteFileAsync(string fileName)
        {
            try
            {
                await _storageClient.DeleteObjectAsync(_bucketName, fileName);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file: {ex.Message}");
                return false;
            }
        }
    }
}
