using Infrastructure;
using Domains;
using Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        public readonly CourseService _courseService;
        public readonly FirebaseService _firebaseService;
        public CourseController(CourseService courseService, FirebaseService firebaseService)
        {
            _courseService = courseService;
            _firebaseService = firebaseService;
        }

        [HttpPost("addcourse")]
public async Task<IActionResult> AddCourse([FromBody] Course course)
{
    try
    {
        /*
        Console.WriteLine("=== AddCourse endpoint hit ===");
        
        var authHeader = Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            Console.WriteLine("ERROR: Missing or invalid authorization header");
            return Unauthorized(new { Message = "Missing or invalid authorization header" });
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();
        Console.WriteLine($"Token received: {token.Substring(0, 20)}...");

        var decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        var uid = decoded.Uid;
        Console.WriteLine($"User ID: {uid}");

        DocumentReference userDoc = _firebaseService._firestore.Collection("users").Document(uid);
        DocumentSnapshot snapshot = await userDoc.GetSnapshotAsync();

        if (!snapshot.Exists)
        {
            Console.WriteLine("ERROR: User not found in Firestore");
            return Unauthorized(new { Message = "User not found in Firestore" });
        }

        string role = snapshot.GetValue<string>("role");
        Console.WriteLine($"User role: {role}");

        if (role != "creator")
        {
            Console.WriteLine("ERROR: User is not a creator");
            return Forbid("You are not creator or unauthenticated");
        }

        // Log received course data
        Console.WriteLine($"Course Title: {course.CourseTitle}");
        Console.WriteLine($"Course Description: {course.CourseDescription}");
        
        course.CreatorID = uid;
        //course.CourseSizeBytes = 0;
               // course.CourseType = "private";
              //  course.CourseContentURL = "fbvd";
       // course.IsPublished = 2;
        
        Console.WriteLine("Calling AddCourseAsync...");*/
        string courseid = await _courseService.AddCourseAsync(course);

        
        return Ok(new { Message = "Course added successfully" });
    }
    catch (FirebaseAuthException ex)
    {
        Console.WriteLine($"Firebase Auth Error: {ex.Message}");
        return Unauthorized(new { Message = "Invalid Firebase token", Error = ex.Message });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        return StatusCode(500, new { Message = "An error occurred while adding the course", Error = ex.Message });
    }
}
    }
}