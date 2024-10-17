namespace FileGenerator;

internal class FileConfig(string className, string content)
{
    public string ClassName { get; set; } = className;
    public string Content { get; set; } = content;
}
