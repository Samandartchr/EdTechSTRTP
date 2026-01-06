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

    [JsonPropertyName("coursedescription")]
    public string Description { get; set; }
}

public class CourseContentDto
{
    [JsonPropertyName("id")]
    public string CourseId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("image")]
    public string CourseImage { get; set; }

    [JsonPropertyName("modules")]
    public List<ModuleCardDto> Modules { get; set; }
}

public class ModuleCardDto
{
    [JsonPropertyName("moduleid")]
    public string ModuleId { get; set; }

    [JsonPropertyName("name")]
    public string ModuleName { get; set; }
}

public class ModuleDto
{
    [JsonPropertyName("id")]
    public string ModuleId { get; set; }

    [JsonPropertyName("courseid")]
    public string CourseId { get; set; }

    [JsonPropertyName("name")]
    public string ModuleName { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("contents")]
    public List<ContentCardDto> Contents { get; set; }
}

public class ContentCardDto
{
    [JsonPropertyName("id")]
    public string ContentId { get; set; }

    [JsonPropertyName("name")]
    public string ContentName { get; set; }

    [JsonPropertyName("type")]
    public string ContentType { get; set; }
}