using Domains;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using FirebaseAdmin;
using FirebaseAdmin.Auth;

namespace Services
{
    public class CourseService
    {
        private readonly AppDbContext _context;
        private static readonly Random _random = new();
        public CourseService(AppDbContext context)
        {
            _context = context;
        }
        //Add new course as draft
        public async Task<string> AddCourseAsync(Course course)
{
    
    course.CourseID = await GenerateUniqueCourseIdAsync();

    _context.Courses.Add(course);
    
    var changes = await _context.SaveChangesAsync();
    
    return course.CourseID;
}

        public async Task<string> GenerateUniqueCourseIdAsync()
        {
            string courseId;
            bool exists;
            do
            {
                courseId = GenerateRandomString(10);
                exists = await _context.Courses.AnyAsync(c => c.CourseID == courseId);
            } while (exists);
            return courseId;
        }

        private static string GenerateRandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var buffer = new char[length];
            for (int i = 0; i < length; i++)
            {
                buffer[i] = chars[_random.Next(chars.Length)];
            }
            return new string(buffer);
        }

// ... setup and initialization of FirebaseApp.DefaultInstance ...

public async Task<string> GetUidFromIdToken(string idToken)
{
    try
    {
        // VerifyIdTokenAsync checks the token's signature, issuer, and expiration time.
        // It returns a decoded token object.
        FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance
            .VerifyIdTokenAsync(idToken);
            
        // The UID is available in the decoded token object.
        string uid = decodedToken.Uid;
        return uid; 
    }
    catch (FirebaseAuthException e)
    {
        // Handle token errors (e.g., token expired, invalid signature)
        Console.WriteLine($"Token verification error: {e.Message}");
        return null;
    }
}
    }
}