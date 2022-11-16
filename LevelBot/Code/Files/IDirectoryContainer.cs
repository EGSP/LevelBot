namespace LevelBot.Code.Files;

public interface IDataContainer
{
    public string Path { get; }
    
}

public interface IDirectory : IDataContainer
{
}

public sealed class DirectoryContainer : IDirectory
{
    public DirectoryContainer(string path)
    {
        Path = path+"/";
    }

    public string Path { get; }
}

public interface IFile : IDataContainer
{
    IDirectory Directory { get; }
    string Name { get; }
}

public sealed class FileContainer : IFile
{
    public IDirectory Directory { get; }
    public string Name { get; }

    public FileContainer(IDirectory directory,string name)
    {
        Directory = directory;
        Name = name;
    }

    public string Path => Directory.Path + "/" + Name;
}

public static class DataContainerExtensions
{
    public static void Create(this IDirectory directory) => System.IO.Directory.CreateDirectory(directory.Path);
    public static bool Exist(this IDirectory directory) => System.IO.Directory.Exists(directory.Path);
    
    public static IFile File(this IDirectory directory, string name)
        => new FileContainer(directory, name);
    
    public static IDirectory Directory(this IDirectory directory, string name)
        => new DirectoryContainer($"{directory.Path}/{name}/");

    public static void Create(this IFile file)
    {
        if(!file.Directory.Exist())
            file.Directory.Create();
        System.IO.File.Create(file.Path).Close();
    }

    public static bool Exist(this IFile file) => System.IO.File.Exists(file.Path);
    
    public static async Task<string> Read(this IFile file)
    {
        if (file.Exist())
            return await System.IO.File.ReadAllTextAsync(file.Path);;
        return string.Empty;
    }

    public static async Task<string> ReadOrCreate(this IFile file)
    {
        if (file.Exist())
            return await System.IO.File.ReadAllTextAsync(file.Path);
        file.Create();
        return await file.Read();
    }
}