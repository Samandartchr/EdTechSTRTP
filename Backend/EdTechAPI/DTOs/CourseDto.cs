using System.Text.Json.Serialization;

namespace DTOs;

public class AddCourseDto
{
    [JsonPropertyName("coursetitle")]
    public string Title { get; set; }

    [JsonPropertyName("coursedescription")]
    public string Description { get; set; }

}

public class CourseCardDto
{
    [JsonPropertyName("courseid")]
    public string CourseId { get; set; }

    [JsonPropertyName("coursetitle")]
    public string Title { get; set; }

    [JsonPropertyName("courseimage")]
    public string CourseImage { get; set; }

    [JsonPropertyName("ispublished")]
    public bool IsPublished { get; set; }

    //[JsonPropertyName("coursedescription")]
    //public string Description { get; set; }
}