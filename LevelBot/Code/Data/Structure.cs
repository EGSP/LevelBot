using Automata.IO;
using Directory = Automata.IO.Directory;

namespace LevelBot.Code.Data;

public static class Structure
{
    public static IDirectory Root { get; }
    
    public static IDirectory Logs { get; }

    static Structure()
    {
        Root = new Directory(Environment.CurrentDirectory+"/content/");
        Logs = Root.Directory("logs");
    }
}