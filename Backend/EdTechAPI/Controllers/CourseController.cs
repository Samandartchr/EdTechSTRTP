using Infrastructure;
using Domains;
using Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using FirebaseAdmin;
using Google.Cloud.Firestore;
using FirebaseAdmin.Auth;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        public readonly CourseService _courseService;
        public CourseController(CourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpPost("addcourse")]
        public async Task<IActionResult> AddCourse([FromBody] Course course)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString();
                var rawToken = token.Replace("Bearer ", "");
                string uid = await _courseService.GetUidFromIdToken(rawToken);
                course.CreatorID = uid;
        
                string courseid = await _courseService.AddCourseAsync(course);
                return Ok(new { Message = "Course added successfully" });
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