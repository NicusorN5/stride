// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Mono.Cecil;
using Mono.Options;

namespace Stride.Core.AssemblyProcessor;

public class AssemblyProcessorProgram
{
    public static readonly string ExeName = Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);

    public int Run(string[] args, TextWriter? logger = null)
    {
        logger ??= Console.Out;

        var app = CreateAssemblyProcessorApp(args, logger, out var p, out var showHelp, out var outputFilePath, out var inputFiles);
        if (showHelp)
        {
            p.WriteOptionDescriptions(logger);
            return 1;
        }

        if (inputFiles.Count != 1)
        {
            p.WriteOptionDescriptions(logger);
            return ExitWithError("This tool requires one input file.", logger);
        }

        var inputFile = inputFiles[0];

        // Add search path from input file
        //app.SearchDirectories.Add(Path.GetDirectoryName(inputFile));

        // Load symbol file if it exists
        var symbolFile = Path.ChangeExtension(inputFile, "pdb");
        if (File.Exists(symbolFile))
        {
            app.UseSymbols = true;
        }

        // Setup output filestream
        outputFilePath ??= inputFile;

        if (!app.Run(inputFile, outputFilePath))
        {
            return ExitWithError("Unexpected error", logger);
        }

        return 0;
    }

    public static int Main(string[] args)
    {
        var program = new AssemblyProcessorProgram();
        return program.Run(args);
    }

    public static AssemblyProcessorApp CreateAssemblyProcessorApp(string[] args, TextWriter? logger = null)
    {
        logger ??= Console.Out;
        return CreateAssemblyProcessorApp(args, logger, out _, out _, out _, out _);
    }

    public static AssemblyProcessorApp CreateAssemblyProcessorApp(string[] args, TextWriter logger, out OptionSet p, out bool showHelp, out string? outputFilePath, out List<string> inputFiles)
    {
        if (logger == null) throw new ArgumentNullException(nameof(logger));

        bool localShowHelp = false;
        string? localOutputFilePath = null;

        var app = new AssemblyProcessorApp(logger);
        p = new OptionSet()
        {
            "Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp) All Rights Reserved",
            "Stride Assembly Processor tool - Version: "
            +
            string.Format(
                "{0}.{1}.{2}",
                typeof(AssemblyProcessorProgram).Assembly.GetName().Version.Major,
                typeof(AssemblyProcessorProgram).Assembly.GetName().Version.Minor,
                typeof(AssemblyProcessorProgram).Assembly.GetName().Version.Build) + string.Empty,
            string.Format("Usage: {0} [options]* inputfile -o [outputfile]", ExeName),
            string.Empty,
            "=== Options ===",
            string.Empty,
            { "h|help", "Show this message and exit", v => localShowHelp = v != null },
            { "o|output=", "Output file name", v => localOutputFilePath = v },
            { "p|platform=", "The platform (Windows, Android, iOS)", v => app.Platform = (PlatformType)Enum.Parse(typeof(PlatformType), v) },
            { "parameter-key", "Automatically initialize parameter keys in module static constructor", _ => app.ParameterKey = true },
            { "rename-assembly=", "Rename assembly", v => app.NewAssemblyName = v },
            { "auto-module-initializer", "Execute function tagged with [ModuleInitializer] at module initialization (automatically enabled)", _ => app.ModuleInitializer = true },
            { "serialization", "Generate serialiation assembly", _ => app.SerializationAssembly = true },
            { "docfile=", "Generate user documentation from XML file", v => app.DocumentationFile = v },
            { "d|directory=", "Additional search directory for assemblies", app.SearchDirectories.Add },
            { "a|assembly=", "Additional assembly (for now, it will add the assembly directory to search path)", v => app.SearchDirectories.Add(Path.GetDirectoryName(v)) },
            { "references-file=", "Project reference stored in a path", v => app.References.AddRange(File.ReadAllLines(v)) },
            { "add-reference=", "References to explicitely add", app.ReferencesToAdd.Add },
            { "delete-output-on-error", "Delete output file if an error happened", _ => app.DeleteOutputOnError = true },
            { "keep-original", "Keep copy of the original assembly", _ => app.KeepOriginal = true },
        };

        inputFiles = p.Parse(args);
        showHelp = localShowHelp;
        outputFilePath = localOutputFilePath;
        return app;
    }

    private int ExitWithError(string message, TextWriter logger)
    {
        logger ??= Console.Out;
        if (message != null)
            logger.WriteLine(message);
        return 1;
    }
}
