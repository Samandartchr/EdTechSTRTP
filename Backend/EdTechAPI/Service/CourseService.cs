using Domains;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

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
            //Save to database
            course.CourseID = await GenerateUniqueCourseIdAsync();
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
    }
}