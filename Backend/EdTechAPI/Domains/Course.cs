using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Domains
{
    [Table("course_list")]
    public class Course
    {
        [Key]
        [Column("course_id")]
        [JsonPropertyName("courseid")]
        public required string CourseID { get; set; }

        [Column("creator_id")]
        [JsonPropertyName("creatorid")]
        public string CreatorID { get; set; }

        [Column("course_title")]
        [JsonPropertyName("coursetitle")]
        public string CourseTitle { get; set; }

        [Column("course_image_url")]
        [JsonPropertyName("courseimageurl")]
        public string CourseImageURL { get; set; }

        [Column("course_description")]
        [JsonPropertyName("coursedescription")]
        public string CourseDescription { get; set; }

        [Column("course_content_url")]
        [JsonPropertyName("coursecontenturl")]
        public string CourseContentURL { get; set; }

        [Column("is_published")]
        [JsonPropertyName("ispublished")]
        public Boolean IsPublished { get; set; }

        [Column("course_size_bytes")]
        [JsonPropertyName("coursesizebytes")]
        public int CourseSizeBytes { get; set; }

        [Column("course_type")]
        [JsonPropertyName("coursetype")]
        public string CourseType { get; set; }
    }
    
}