using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace EXE201_Lockey
{
    public static class FirebaseServiceExtensions
    {
        public static void AddFirebase(this IServiceCollection services)
        {
            var serviceAccountPath = Path.Combine(Directory.GetCurrentDirectory(), "Credentials", "serviceAccountKey.json");

            if (!File.Exists(serviceAccountPath))
            {
                throw new FileNotFoundException("Firebase service account key file not found.", serviceAccountPath);
            }

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", serviceAccountPath);
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(serviceAccountPath),
            });
        }

    }
}
