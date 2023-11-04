using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using S.AddonsOverhaul.Core.Interfaces.Log;

namespace S.AddonsOverhaul.Core.Compilation
{
    internal static class Compiler
    {
        private static readonly string[] GameAssemblies =
        {
            "mscorlib.dll",
            "netstandard.dll",
            "System.dll",
            "System.Core.dll",
            "System.Data.dll",
            "System.Xml.dll",

            "Assembly-CSharp.dll",
            "Assembly-CSharp-firstpass.dll",

            "UnityEngine.dll",
            "UnityEngine.CoreModule.dll",
            "UnityEngine.AssetBundleModule.dll",
            "UnityEngine.UI.dll",
            "UnityEngine.UIModule.dll",
            "UnityEngine.ParticleSystemModule.dll",
            "UnityEngine.PhysicsModule.dll",
            "UnityEngine.StreamingModule.dll",
            "UnityEngine.SubstanceModule.dll",
            "UnityEngine.UmbraModule.dll",
            "UnityEngine.TextCoreFontEngineModule.dll",
            "UnityEngine.TextCoreTextEngineModule.dll",
            "UnityEngine.TextRenderingModule.dll",
            "UnityEngine.SharedInternalsModule.dll",
            "UnityEngine.IMGUIModule.dll",
            "UnityEngine.InputLegacyModule.dll",
            "UnityEngine.VideoModule.dll",
            "UnityEngine.JSONSerializeModule.dll",

            "Unity.TextMeshPro.dll",

            "UniTask.dll",
            "UniTask.DOTween.dll",
            "UniTask.Addressables.dll",
            "UniTask.Linq.dll",
            "UniTask.TextMeshPro.dll",

            "RG.ImGui.dll",
            "RG.ImGui.Unity.dll"
        };

        private static readonly string[] AdditionalAssemblies =
        {
            "S.AddonsOverhaul.dll",
            "0Harmony.dll",
            "System.Collections.Immutable.dll"
        };

        public static bool Compile(string addonName, string addonAuthor, string[] sourceFiles)
        {
            var assemblyName = addonName + " - AssemblyCSharp";

            foreach (var file in sourceFiles)
                if (!File.Exists(file))
                {
                    AddonsLogger.Log($"Could not find source file '{file}'.", LogLevel.Error);
                    return false;
                }

            var syntaxTrees = new List<SyntaxTree>();

            foreach (var sourceFile in sourceFiles)
            {
                AddonsLogger.Log($"Compiling file '{sourceFile}'...");

                if (!File.Exists(sourceFile))
                {
                    AddonsLogger.Log($"Could not find source file '{sourceFile}'.", LogLevel.Error);
                    return false;
                }

                syntaxTrees.Add(CSharpSyntaxTree.ParseText(Converter.ToOverhaulFormat(File.ReadAllText(sourceFile))));
            }

            var installDirectory = Directory.Exists("rocketstation_Data/Managed/")
                ? "rocketstation_Data/Managed/"
                : "rocketstation_DedicatedServer_Data/Managed/";

            var references = new List<MetadataReference>();

            foreach (var file in GameAssemblies)
            {
                var reference =
                    MetadataReference.CreateFromFile(Path.Combine(Constants.GamePath, installDirectory,
                        file));
                references.Add(reference);
            }

            foreach (var file in AdditionalAssemblies)
            {
                var reference =
                    MetadataReference.CreateFromFile(Path.Combine(Constants.ManagerPath, file));
                references.Add(reference);
            }

            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            AddonsLogger.Log($"Linking addon '{addonName}'...");

            var compilation = CSharpCompilation.Create($"{assemblyName}-{addonAuthor}-{DateTime.UtcNow.Ticks}")
                .AddSyntaxTrees(syntaxTrees)
                .WithReferences(references.ToArray())
                .WithOptions(options);

            var returned = true;

            new DisposableMemoryStream().RunThenDispose(ms =>
            {
                var result = compilation.Emit(ms);
                var output = result.Diagnostics;

                if (!result.Success)
                    foreach (var error in output)
                    {
                        var errorMessage = error.GetMessage();

                        if (errorMessage.Contains("mscorlib, Version=2.0.0.0,")) continue;

                        switch (error.Severity)
                        {
                            case DiagnosticSeverity.Hidden:
                            case DiagnosticSeverity.Info: continue;
                            case DiagnosticSeverity.Warning:
                                AddonsLogger.Log("(Plugin Compiler - WARNING) " + errorMessage, LogLevel.Warn);
                                continue;
                            case DiagnosticSeverity.Error:
                                AddonsLogger.Log("(Plugin Compiler - ERROR) " + errorMessage, LogLevel.Error);
                                returned = false;
                                continue;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                if (!Directory.Exists(Path.Combine(Constants.AddonPath, addonName)))
                    Directory.CreateDirectory(Path.Combine(Constants.AddonPath, addonName));

                var assemblyFile = Path.Combine(Constants.AddonPath, addonName, assemblyName + ".dll");

                if (File.Exists(assemblyFile))
                    File.Delete(assemblyFile);

                new DisposableFileStream(assemblyFile, FileMode.Create).WriteAndDispose(ms.GetBuffer(), 0,
                    (int)ms.Length);
            });

            return returned;
        }
    }
}