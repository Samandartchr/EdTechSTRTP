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
            //string defImageLink = "gffdds";
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

        public async Task<List<CourseCardDto>> GetCoursesByCreatorIdAsync(string creatorId)
        {
            var courses = await _context.Courses
                .AsNoTracking()
                .Where(c => c.CreatorID == creatorId)
                .Select(c => new CourseCardDto
                {
                    CourseId = c.CourseID,
                    Title = c.CourseTitle,
                    CourseImage = Convert.ToBase64String(File.ReadAllBytes(c.CourseImageURL)),
                    IsPublished = c.IsPublished,
                    Description = c.CourseDescription
                })
                .ToListAsync();

            return courses;
        }

        public async Task<CourseContentDto> GetCourseContentByIdAsync(string courseId, string uid)
        {
            DocumentSnapshot snapshot = await db.Collection("courses").Document(courseId).GetSnapshotAsync();

            var course = await _context.Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CourseID == courseId && c.CreatorID == uid);

            if (course == null)
            {
                return null;
            }

            var modules = new List<ModuleCardDto>();
            if(snapshot.ContainsField("modules"))
            {
                var modulesData = snapshot.GetValue<List<Dictionary<string, object>>>("modules");
                foreach (var moduleData in modulesData)
                {
                    modules.Add(new ModuleCardDto
                    {
                        ModuleId = moduleData["moduleid"].ToString(),
                        ModuleName = moduleData["modulename"].ToString()
                    });
                }
            }

            var courseContentDto = new CourseContentDto
            {
                CourseId = course.CourseID,
                Title = course.CourseTitle,
                Description = course.CourseDescription,
                CourseImage = Convert.ToBase64String(File.ReadAllBytes(course.CourseImageURL)),
                Modules = modules
            };

            return courseContentDto;
        }

        public async Task<bool> UpdateCourseContentAsync(CourseContentDto courseContentDto, string uid)
        {
            //check if course exists and belongs to user
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseID == courseContentDto.CourseId && c.CreatorID == uid);
            if (course == null)
            {
                return false;
            }
            //update course details
            course.CourseTitle = courseContentDto.Title;
            course.CourseDescription = courseContentDto.Description;
            File.WriteAllBytesAsync(StorageContext.CoursesImagesPath + $@"\{course.CourseID}.jpeg", Convert.FromBase64String(courseContentDto.CourseImage));
            course.CourseImageURL = Path.Combine(StorageContext.CoursesImagesPath, $"{course.CourseID}.jpeg");
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            //update firestore course content
            var docRef = db.Collection("courses").Document(course.CourseID);
            var ModulesCollection = db.Collection("modules");
            var CourseSnapshot = await docRef.GetSnapshotAsync();

            //Get documents list from modules whose names are started with courseId-
            var existingModules = new HashSet<string>();
            foreach (var module in CourseSnapshot.GetValue<List<Dictionary<string, object>>>("modules"))
            {
                existingModules.Add(course.CourseID + "-" + module["moduleid"].ToString());
            }

            var modulesData = new List<Dictionary<string, object>>();
            foreach (var module in courseContentDto.Modules)
            {
                modulesData.Add(new Dictionary<string, object>
                {
                    { "moduleid", module.ModuleId },
                    { "modulename", module.ModuleName }
                });
            }
            await docRef.UpdateAsync(new Dictionary<string, object>
            {
                { "coursetitle", courseContentDto.Title },
                { "coursedescription", courseContentDto.Description },
                { "modules", modulesData }
            });
            //create a document for each module
            foreach (var module in courseContentDto.Modules)
            {
                var moduleDoc = db.Collection("modules").Document(course.CourseID + "-" + module.ModuleId);
                await moduleDoc.UpdateAsync(new Dictionary<string, object>
                {
                    { "modulename", module.ModuleName }
                });
            }

            //delete removed modules
            foreach (var existingModuleId in existingModules)
            {
                bool moduleExists = false;
                foreach (var module in courseContentDto.Modules)
                {
                    if (existingModuleId == course.CourseID + "-" + module.ModuleId)
                    {
                        moduleExists = true;
                        break;
                    }
                }
                if (!moduleExists)
                {
                    var moduleDoc = db.Collection("modules").Document(existingModuleId);
                    await moduleDoc.DeleteAsync();
                }
            }
            
            return true;
        }

        public async Task<bool> UpdateModuleAsync(ModuleDto moduleDto, string uid)
{
    // 1. Check if course exists and belongs to user
    var course = await _context.Courses
        .FirstOrDefaultAsync(c => c.CourseID == moduleDto.CourseId && c.CreatorID == uid);

    if (course == null)
        return false;

    var moduleDocId = $"{moduleDto.CourseId}-{moduleDto.ModuleId}";
    var moduleDoc = db.Collection("modules").Document(moduleDocId);

    var moduleSnapshot = await moduleDoc.GetSnapshotAsync();
    if (!moduleSnapshot.Exists)
        return false;

    // 2. Update module metadata
    await moduleDoc.UpdateAsync(new Dictionary<string, object>
    {
        { "modulename", moduleDto.ModuleName },
        { "description", moduleDto.Description }
    });

    // 3. Load existing contents
    var existingContents = new HashSet<string>();

    if (moduleSnapshot.ContainsField("contents"))
    {
        var contents = moduleSnapshot.GetValue<List<Dictionary<string, object>>>("contents");
        foreach (var content in contents)
        {
            var id = $"{moduleDto.CourseId}-{moduleDto.ModuleId}-{content["contentid"]}";
            existingContents.Add(id);
        }
    }

    // 4. Prepare updated contents list
    var contentsData = new List<Dictionary<string, object>>();

    foreach (var content in moduleDto.Contents)
    {
        var contentDocId = $"{moduleDto.CourseId}-{moduleDto.ModuleId}-{content.ContentId}";
        contentsData.Add(new Dictionary<string, object>
        {
            { "contentid", content.ContentId },
            { "contentname", content.ContentName },
            { "contenttype", content.ContentType }
        });

        var contentDoc = db.Collection("contents").Document(contentDocId);
        await contentDoc.SetAsync(new Dictionary<string, object>
        {
            { "contentname", content.ContentName },
            { "contenttype", content.ContentType }
        });
    }

    // 5. Update module contents array
    await moduleDoc.UpdateAsync(new Dictionary<string, object>
    {
        { "contents", contentsData }
    });

    // 6. Delete removed contents
    foreach (var existingContentId in existingContents)
    {
        bool stillExists = moduleDto.Contents.Any(c =>
            existingContentId == $"{moduleDto.CourseId}-{moduleDto.ModuleId}-{c.ContentId}");

        if (!stillExists)
        {
            var contentDoc = db.Collection("contents").Document(existingContentId);
            await contentDoc.DeleteAsync();
        }
    }

    return true;
}

        public async Task<ModuleDto> GetModuleByIdAsync(string moduleId, string courseId, string uid)
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseID == courseId && c.CreatorID == uid);
            if (course == null)
            {
                return null;
            }

            var moduleDocId = $"{courseId}-{moduleId}";
            var moduleDoc = db.Collection("modules").Document(moduleDocId);
            var moduleSnapshot = await moduleDoc.GetSnapshotAsync();
            if (!moduleSnapshot.Exists)
            {
                return null;
            }

            var contents = new List<ContentCardDto>();
            if (moduleSnapshot.ContainsField("contents"))
            {
                var contentsData = moduleSnapshot.GetValue<List<Dictionary<string, object>>>("contents");
                foreach (var contentData in contentsData)
                {
                    contents.Add(new ContentCardDto
                    {
                        ContentId = contentData["contentid"].ToString(),
                        ContentName = contentData["contentname"].ToString(),
                        ContentType = contentData["contenttype"].ToString()
                    });
                }
            }
            var moduleDto = new ModuleDto
            {
                ModuleId = moduleId,
                CourseId = courseId,
                ModuleName = moduleSnapshot.GetValue<string>("modulename"),
                Description = moduleSnapshot.ContainsField("description") ? moduleSnapshot.GetValue<string>("description") : string.Empty,
                Contents = contents
            };
            return moduleDto;
        }
    }
}