using System;
using System.Collections.Generic;
using System.Text;

namespace FileGeneratorWithConfiguration;

public class Config(string nameSpace, string extension)
{
    public string Namespace { get; set; } = nameSpace;
    public string Extension { get; set; } = extension;
}

public class CsvFile
{
    public string ClassName { get; set; } = null!;
    public string[] Rows { get; set; } = [];
}