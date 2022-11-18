using LevelBot.Code.Files;

namespace LevelBot.Code.Data;

public static class Structure
{
    public static IDirectory Root { get; }
    
    public static IDirectory Logs { get; }

    static Structure()
    {
        Root = new DirectoryContainer(Environment.CurrentDirectory+"/content/");
        Logs = Root.Directory("logs");
    }
}