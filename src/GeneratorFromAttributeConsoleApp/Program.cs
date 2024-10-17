using GeneratorFromAttributeConsoleApp;

MyObject obj = new();
ClassHelper.SetPropertyMyObject(obj, "Id", "12");
ClassHelper.SetPropertyMyObject(obj, "Name", "AZ");

Console.WriteLine($"Id = {obj.Id} Name = '{obj.Name}'");

Console.WriteLine();
Console.WriteLine("Try to insert wrong type...");
if (!ClassHelper.SetPropertyMyObject(obj, "Id", "aaa"))
{
    Console.WriteLine("'aaa' cannot be converted to int");
}

Console.WriteLine();
Console.WriteLine("Try to insert value in wrong property...");
if (!ClassHelper.SetPropertyMyObject(obj, "Description", "aaa"))
{
    Console.WriteLine("'Description' property does not exist in MyObject class");
}

Console.WriteLine();
Console.WriteLine("Exception for wrong property type");

try
{
    ClassHelper.SetPropertyMyObject(obj, "Value", "aaa");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Excepion message: {ex.Message}");
}