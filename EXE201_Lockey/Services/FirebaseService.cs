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
        private readonly string _bucketName = "lockey-exe.appspot.com"; // Thay thế bằng tên bucket của bạn
        private readonly StorageClient _storageClient;

        public FirebaseService()
        {
            // Xác định đường dẫn trực tiếp đến tệp serviceAccountKey.json trong thư mục Credentials
            var serviceAccountPath = Path.Combine(Directory.GetCurrentDirectory(), "Credentials", "serviceAccountKey.json");

            // Kiểm tra xem tệp có tồn tại không
            if (!File.Exists(serviceAccountPath))
            {
                throw new FileNotFoundException("Firebase service account key file not found.", serviceAccountPath);
            }

            // Cài đặt biến môi trường GOOGLE_APPLICATION_CREDENTIALS với đường dẫn của tệp
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", serviceAccountPath);

            // Khởi tạo FirebaseApp nếu chưa được khởi tạo
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(serviceAccountPath),
                });
            }

            // Tạo StorageClient để thao tác với Firebase Storage
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
