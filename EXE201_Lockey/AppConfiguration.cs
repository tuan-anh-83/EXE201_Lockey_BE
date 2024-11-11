using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace EXE201_Lockey
{
    public static class AppConfiguration
    {
        public static void AddFirebase(this IServiceCollection services, IConfiguration configuration)
        {
            // Read the service account path from appsettings.json
            var serviceAccountPath = configuration["Firebase:ServiceAccountPath"];

            if (string.IsNullOrEmpty(serviceAccountPath))
            {
                throw new Exception("FIREBASE_CREDENTIALS_PATH is not set in the configuration.");
            }

            var firebaseOptions = new AppOptions
            {
                Credential = GoogleCredential.FromFile(serviceAccountPath),
            };

            FirebaseApp.Create(firebaseOptions);
        }
    }
}
