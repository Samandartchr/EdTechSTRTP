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
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                    return Unauthorized(new { Message = "Missing or invalid authorization header" });

                var token = authHeader.Substring("Bearer ".Length).Trim();

                var decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
                var uid = decoded.Uid;

                DocumentReference userDoc = _firebaseService._firestore.Collection("users").Document(uid);
                DocumentSnapshot snapshot = await userDoc.GetSnapshotAsync();

                if (!snapshot.Exists)
                    return Unauthorized(new { Message = "User not found in Firestore" });

                string role = snapshot.GetValue<string>("role");

                if (role != "creator") return Forbid("You are not creator or unautenticated");

                course.CreatorID = uid;
                course.CourseSizeBytes = 0;
                course.CourseType = "private";
                string courseId = await _courseService.AddCourseAsync(course);
                //Return message with new course ID
                return Ok(new { Message = "Course added successfully", CourseID = courseId });
            }
            catch (FirebaseAuthException ex)
            {
                return Unauthorized(new { Message = "Invalid Firebase token", Error = ex.Message });
            }
            catch (Exception)
            {
                //Return error message
                return StatusCode(500, new { Message = "An error occurred while adding the course" });
            }
        }
    }
}