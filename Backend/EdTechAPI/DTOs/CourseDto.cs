using System.Text.Json.Serialization;

namespace DTOs;

public class AddCourseDto
{
    [JsonPropertyName("coursetitle")]
    public string Title { get; set; }

    [JsonPropertyName("coursedescription")]
    public string Description { get; set; }
}