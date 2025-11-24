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
        public static readonly Random _random = new();
        public CourseService(AppDbContext context)
        {
            _context = context;
        }
        //Add new course as draft
        public async Task<string> AddCourseAsync(Course course)
        {
            bool exists = await CourseIdExistsAsync(course.CourseID);
            if (exists)
            {
                throw new Exception("CourseID already exists");
            }

            _context.Courses.Add(course);
    
            await _context.SaveChangesAsync();
    
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

        public static string GenerateRandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var buffer = new char[length];
            for (int i = 0; i < length; i++)
            {
                buffer[i] = chars[_random.Next(chars.Length)];
            }
            return new string(buffer);
        }

        public async Task<bool> CourseIdExistsAsync(string courseId)
        {
            return await _context.Courses.AsNoTracking().AnyAsync(c => c.CourseID == courseId);
        }
    }
}