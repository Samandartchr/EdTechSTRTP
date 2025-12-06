using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Domains
{
    [Table("course_list")]
    public class Course
    {
        [Column("course_id")]
        public string CourseID { get; set; }

        [Column("creator_id")]
        public string CreatorID { get; set; }

        [Column("course_title")]
        public string CourseTitle { get; set; }

        [Column("course_image_url")]
        public string CourseImageURL { get; set; }

        [Column("course_description")]
        public string CourseDescription { get; set; }

        [Column("course_content_url")]
        public string CourseContentURL { get; set; }

        [Column("is_published")]
        public Boolean IsPublished { get; set; }

        [Column("course_size_bytes")]
        public int CourseSizeBytes { get; set; }

        [Column("course_type")]
        public string CourseType { get; set; }
    }
    
}