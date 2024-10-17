using CodeGenerated;

string[] properties = Helper.GetProperties<MyObject>();
foreach (var property in properties)
{
    Console.WriteLine(property);
}