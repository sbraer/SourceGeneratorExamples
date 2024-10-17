using GeneratorFromAttributeExample;
namespace GeneratorFromAttributeConsoleApp;

[GeneratedAttribute(typeof(MyObject), GeneratorNotTypeRecognized.ThrowException)]
[GeneratedAttribute(typeof(OtherObject))]
internal static partial class ClassHelper { }