using System.IO;
using System;
using Domains;

namespace Infrastructure;

public static class StorageContext
{
    public static readonly string AppStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "AppStorage");
    public static readonly string CoursesPath = Path.Combine(AppStoragePath, "Courses");
    public static readonly string DefaultsPath = Path.Combine(AppStoragePath, "Defaults");
}