using GeneratorFromAttributeExample;
namespace GeneratorFromAttributeConsoleApp;

[GenerateSetProperty<MyObject>(GeneratorNotTypeRecognized.ThrowException)]
[GenerateSetProperty<OtherObject>()]
internal static partial class ClassHelper { }