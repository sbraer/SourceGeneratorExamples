namespace GeneratorFromAttributeConsoleApp;

public sealed class MyObject
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public object? Value { get; set; }
}

public sealed class OtherObject
{
    public DateTime? DateTime { get; set; }
    public string? Comment { get; set; } = null!;
}