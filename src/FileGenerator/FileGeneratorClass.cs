﻿using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;
using System.Globalization;
using System.IO;
using System.Threading;

namespace FileGenerator;

[Generator(LanguageNames.CSharp)]
public class FileGeneratorClass : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<FileConfig> txtFiles =
            context.AdditionalTextsProvider
                .Where(text => text.Path.EndsWith(".txt"))
                .Select(GetDocument)
                .Where((document) => document is not null)!;

        context.RegisterSourceOutput(txtFiles, Execute);
    }

    private FileConfig? GetDocument(AdditionalText additionalText, CancellationToken cancellationToken)
    {
        //if (!Debugger.IsAttached) Debugger.Launch();
        SourceText? text = additionalText.GetText(cancellationToken);
        if (text is null)
        {
            return null;
        }

        return new FileConfig(Path.GetFileNameWithoutExtension(additionalText.Path), text.ToString());
    }

    private void Execute(SourceProductionContext context, FileConfig fileConfig)
    {
        using StringWriter writer = new(CultureInfo.InvariantCulture);
        using IndentedTextWriter text = new(writer);

        text.WriteLine("// <auto-generated/>");
        text.WriteLine("#nullable enable");
        text.WriteLine();
        text.WriteLine($@"[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""{typeof(FileGeneratorClass).Assembly.GetName().Name}"", ""{typeof(FileGeneratorClass).Assembly.GetName().Version}"")]");
        text.WriteLine($"internal static class {fileConfig.ClassName}");
        text.WriteLine('{');
        text.Indent++;

        text.WriteLine(""""
            internal static string Content() =>"""
            """");
        text.Indent--;
        text.WriteLine(fileConfig.Content);
        text.WriteLine(""""
            """;
            """");

        text.WriteLine('}');

        context.AddSource($"{fileConfig.ClassName}.g.cs", writer.ToString());
    }
}
