using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using Google.Apis.Auth.OAuth2;

namespace Services
{
    public class FirebaseService
    {
        public readonly FirestoreDb _firestore;
        public FirebaseService()
        {
            string path = "../practise-d5653-firebase-adminsdk-fbsvc-ae3dbd49f7.json";
            var credential = GoogleCredential.FromFile(path);
            _firestore = new FirestoreDbBuilder
            {
                ProjectId = "practise-d5653",
                Credential = credential
            }.Build();
        }
        
        public async Task<(string UserId, string Role)> GetUserIdAndRoleFromTokenAsync(string idToken)
        {
            try
        {
            // Verify Firebase token
            FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
            string uid = decodedToken.Uid;

            // Fetch user data from Firestore
            DocumentReference userDoc = _firestore.Collection("users").Document(uid);
            DocumentSnapshot snapshot = await userDoc.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                string role = snapshot.ContainsField("role") ? snapshot.GetValue<string>("role") : "unknown";
                return (uid, role);
            }
            else
            {
                return (uid, "no-role-found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token verification failed: {ex.Message}");
            throw new UnauthorizedAccessException("Invalid or expired token.");
        }
        }
    }
}