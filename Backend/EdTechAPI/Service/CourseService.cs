using Domains;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using DTOs;
using Services;
using System;
using System.IO;
using System.Text.Json;
using Google.Cloud.Firestore;
using System.Collections.Generic;

namespace Services
{
    public class CourseService
    {
        private readonly AppDbContext _context;
        private readonly FirestoreDb db;        
        public static readonly Random _random = new();
        public CourseService(AppDbContext context, FirestoreDb firestoredb)
        {
            _context = context;
            db = firestoredb;        }
        //Add new course as draft
        public async Task<string> AddCourseAsync(AddCourseDto addCourseDto, string creatorId)
        {
            Course course = new Course();

            course.CourseTitle = addCourseDto.Title;
            course.CourseDescription = addCourseDto.Description;
            course.CreatorID = creatorId;
            course.CourseID = await GenerateUniqueCourseIdAsync();

            var doc = db.Collection("courses").Document(course.CourseID);
            await doc.SetAsync(new Dictionary<string, object>
            {
                { "coursetitle", addCourseDto.Title },
                { "coursedescription", addCourseDto.Description }
            });

            course.CourseContentURL = doc.ToString();

            course.IsPublished = false;
            course.CourseType = "private";

            string defImageLink = Path.Combine(StorageContext.DefaultsPath, "DefaultCourseImage.jpeg");
            course.CourseImageURL = defImageLink;

            var data = new Dictionary<string, object>
            {
                { "coursetitle", addCourseDto.Title },
                { "coursedescription", addCourseDto.Description }
            };

            //DocumentReference docRef = _firestoreService.Db.Collection("courses").Document(course.CourseID);
            //await docRef.SetAsync(data);

            //course.CourseContentURL = docRef.ToString();
            course.CourseSizeBytes = 0;

            _context.Courses.Add(course);
    
            await _context.SaveChangesAsync(); 
            return course.CourseID;
        }

        public async Task<string> GenerateUniqueCourseIdAsync()
        {
            string courseId;
            bool exists;
            int attempts = 0;
            const int maxAttempts = 5;

            do
            {
                if (attempts >= maxAttempts)
                    throw new InvalidOperationException("Failed to generate a unique CourseID after 5 attempts.");

                courseId = GenerateRandomString(10); // adjust length if needed
                exists = await _context.Courses.AnyAsync(c => c.CourseID == courseId);
                attempts++;
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

        
    }
}