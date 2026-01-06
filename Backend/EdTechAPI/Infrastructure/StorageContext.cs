using System.IO;
using System;
using Domains;

namespace Infrastructure;

public static class StorageContext
{
    public static readonly string AppStoragePath = @"C:\Users\Admin\OneDrive\Desktop\Универ\EdTechSTRTP\Backend\AppStorage";
    public static readonly string CoursesPath = Path.Combine(AppStoragePath, "Courses");
    public static readonly string DefaultsPath = Path.Combine(AppStoragePath, "Defaults");
    public static readonly string CoursesImagesPath = Path.Combine(CoursesPath, "Images");
}