using Infrastructure;
using Domains;
using DTOs;
using Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using FirebaseAdmin;
using Google.Cloud.Firestore;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly FirebaseAuth _firebaseAuth;
        public readonly CourseService _courseService;
        public CourseController(CourseService courseService, FirebaseAuth firebaseAuth)
        {
            _courseService = courseService;
            _firebaseAuth = firebaseAuth;
        }

        [HttpPost("addcourse")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddCourse([FromBody] AddCourseDto addCourseDto)
        {
            try
            {
                var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new { Message = "Authorization header is missing or invalid" });
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();
                FirebaseToken decodedToken = await _firebaseAuth.VerifyIdTokenAsync(token);
                string uid = decodedToken.Uid;

        
                string CourseID = await _courseService.AddCourseAsync(addCourseDto, uid);
                return Ok(new { Message = "Course added successfully"});
            }
    
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { Message = "An error occurred while adding the course", Error = ex.Message });
            }
        }

        [HttpGet("getcourses")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<CourseCardDto>>> GetCourses()
        {
            try
            {
                var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new { Message = "Authorization header is missing or invalid" });
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();
                FirebaseToken decodedToken = await _firebaseAuth.VerifyIdTokenAsync(token);
                string uid = decodedToken.Uid;

                var courses = await _courseService.GetCoursesByCreatorIdAsync(uid);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { Message = "An error occurred while retrieving courses", Error = ex.Message });
            }
        }
    }
}