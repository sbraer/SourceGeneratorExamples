﻿using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace FileGeneratorWithConfiguration;

[Generator(LanguageNames.CSharp)]
internal sealed class FileGeneratorWithConfigurationClass : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<Config> configProvider =
                context.AnalyzerConfigOptionsProvider.Select((provider, _) =>
                {
                    provider.GlobalOptions.TryGetValue("build_property.FieldSourceGeneratorNamespace", out var ns);
                    provider.GlobalOptions.TryGetValue("build_property.FieldSourceGeneratorExtensionFilePattern", out var pattern);
                    return new Config(ns ?? "Models", pattern ?? ".csv");
                });

        IncrementalValuesProvider<(AdditionalText, Config)> combinedProvider =
            context.AdditionalTextsProvider.Combine(configProvider);

        context.RegisterSourceOutput(combinedProvider, (spc, source) =>
            Execute(spc, source.Item1, source.Item2));
    }

    private static CsvFile? GetColumns(AdditionalText additionalText)
    {
        SourceText? text = additionalText.GetText();
        if (text is null) return null;

        var className = Path.GetFileNameWithoutExtension(additionalText.Path);
        var rows = text.ToString().Split('\n');
        if (rows.Length == 0) return null;

        return new CsvFile
        {
            ClassName = className,
            Rows = rows
        };
    }

    private static void Execute(SourceProductionContext context,
            AdditionalText file, Config config)
    {
        // if (!Debugger.IsAttached) Debugger.Launch();
        var ext = Path.GetExtension(file.Path);
        if (ext != config.Extension) return;

        var csvFile = GetColumns(file);
        if (csvFile is null) return;

        using StringWriter writer = new(CultureInfo.InvariantCulture);
        using IndentedTextWriter tx = new(writer);

        tx.WriteLine("// <auto-generated/>");
        tx.WriteLine("#nullable enable");
        tx.WriteLine();
        tx.WriteLine($"namespace {config.Namespace};");
        tx.WriteLine();
        tx.WriteLine($@"[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""{typeof(FileGeneratorWithConfigurationClass).Assembly.GetName().Name}"", ""{typeof(FileGeneratorWithConfigurationClass).Assembly.GetName().Version}"")]");
        tx.WriteLine($"public sealed class {csvFile.ClassName}");
        tx.WriteLine('{');
        tx.Indent++;

        List<string> fieldList = [];
        foreach (var row in csvFile.Rows)
        {
            var columns = row.Split(',');
            if (columns.Length != 2) continue;
            fieldList.Add(columns[0]);
            tx.WriteLine(columns switch
            {
                var c when c[1].StartsWith("varchar") => $"public string {c[0]} {{ get; set; }} = null!;",
                var c when c[1].StartsWith("int") => $"public int {c[0]} {{ get; set; }}",
                var c when c[1].StartsWith("decimal") => $"public double {c[0]} {{ get; set; }}",
                _ => string.Empty
            });
        }

        tx.WriteLine();
        tx.WriteLine("public override string ToString()");
        tx.WriteLine('{');
        tx.Indent++;
        tx.Write("return $\"");
        foreach (var field in fieldList)
        {
            tx.Write($"{{{field}}} ");
        }
        tx.WriteLine("\";");
        tx.Indent--;
        tx.WriteLine('}');
        tx.Indent--;
        tx.WriteLine('}');

        Debug.Assert(tx.Indent == 0);
        context.AddSource($"{csvFile.ClassName}.g.cs", writer.ToString());
    }
}