using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EXE201_Lockey.Services
{
    public class FirebaseService
    {
        private readonly string _bucketName;
        private readonly StorageClient _storageClient;

        public FirebaseService(IConfiguration configuration)
        {
            // Lấy bucket name từ cấu hình
            _bucketName = configuration["Firebase:StorageBucket"];

            // Lấy thông tin ServiceAccount từ appsettings.json
            var serviceAccount = configuration.GetSection("Firebase:ServiceAccount").Get<Dictionary<string, object>>();

            if (serviceAccount == null)
            {
                throw new Exception("Firebase Service Account configuration is missing.");
            }

            // Chuyển đổi thông tin ServiceAccount thành JSON string
            string jsonCredentials = JsonConvert.SerializeObject(serviceAccount);

            // Tạo GoogleCredential từ JSON string
            var credential = GoogleCredential.FromJson(jsonCredentials);

            // Khởi tạo FirebaseApp nếu chưa được khởi tạo
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = credential,
                });
            }

            // Tạo StorageClient từ GoogleCredential
            _storageClient = StorageClient.Create(credential);
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
                return $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/{Uri.EscapeDataString(storageObject.Name)}?alt=media";
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
