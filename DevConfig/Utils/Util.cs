using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Reflection;

namespace DevConfig.Utils
{
    internal class Util
    {
        ///////////////////////////////////////////////////////////////////////////////////////////
        internal static object? FromFile(string file_name)
        {
            if (!File.Exists(file_name))
                return null;

            string assembly_path = Path.ChangeExtension(Path.GetFileNameWithoutExtension(file_name), "DLL");
            string temp_path_dll = Path.Combine(Path.GetTempPath(), assembly_path);

            if (File.Exists(temp_path_dll) && File.GetLastWriteTime(temp_path_dll) > File.GetLastWriteTime(file_name))
            {
                var assembly = Assembly.LoadFile(temp_path_dll);
                var class_type = assembly.ExportedTypes.First();
                if (class_type != null)
                {
                    object? class_obj = Activator.CreateInstance(class_type);
                    return class_obj;
                }
            }

            string source_code;
            string all_text_file = File.ReadAllText(file_name);
            if (all_text_file.Contains("namespace"))
            {
                source_code = all_text_file;
            }
            else
            {
                string str_prolog =
                    "using System;" + Environment.NewLine +
                    "using System.Linq;" + Environment.NewLine +
                    "using System.Drawing;" + Environment.NewLine +
                    "using System.ComponentModel;" + Environment.NewLine +
                    "using System.Collections.Generic;" + Environment.NewLine +
                    "using Message = CanDiagSupport.Message;" + Environment.NewLine +
                    "namespace CanDiag{" + Environment.NewLine;
                string str_epilog = Environment.NewLine + "}" + Environment.NewLine;
                source_code = str_prolog + all_text_file + str_epilog;
            }

            var code_string = SourceText.From(source_code);
            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp10);
            var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(code_string, options);

            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location)
            };

            Assembly.GetEntryAssembly()?.GetReferencedAssemblies().ToList()
                .ForEach(a => references.Add(MetadataReference.CreateFromFile(Assembly.Load(a).Location)));

            string fullName = Assembly.GetEntryAssembly()!.Location;
            references.Add(MetadataReference.CreateFromFile(fullName));

            var csCompilation = CSharpCompilation.Create(assembly_path,
                new[] { parsedSyntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Release,
                    nullableContextOptions: NullableContextOptions.Annotations,
                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));

            using var peStream = new MemoryStream();
            var result = csCompilation.Emit(peStream);

            if (result.Success)
            {
                Debug.WriteLine("Compilation done without any error.");
                //var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Warning);
                //foreach (var diagnostic in failures)
                //    Debug.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());

                if (File.Exists(temp_path_dll))
                    File.Delete(temp_path_dll);
                peStream.Seek(0, SeekOrigin.Begin);
                using FileStream file = new FileStream(temp_path_dll, FileMode.Create, FileAccess.Write);
                peStream.CopyTo(file);

                peStream.Seek(0, SeekOrigin.Begin);
                var compiledAssembly = peStream.ToArray();
                var asm = new MemoryStream(compiledAssembly);
                var assemblyLoadContext = new SimpleUnloadableAssemblyLoadContext();
                var assembly = assemblyLoadContext.LoadFromStream(asm);
                var class_type = assembly.ExportedTypes.First();
                if (class_type != null)
                    return Activator.CreateInstance(class_type);
            }
            else
            {
                string catption = $"Error in {Path.GetFileName(file_name)}";
                string message = string.Empty;
                var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);
                foreach (var diagnostic in failures)
                {
                    //int line = diagnostic.Location.GetLineSpan().StartLinePosition.Line;
                    string line = diagnostic.Location.GetLineSpan().StartLinePosition.ToString();
                    message += $"{line}: {diagnostic.Id}: {diagnostic.GetMessage()}{Environment.NewLine}";
                }
                Debug.WriteLine(message);
                MessageBox.Show(message.Trim(), catption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

    }
}
