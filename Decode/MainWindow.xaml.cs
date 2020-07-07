using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Decode
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.OutputFile.Text = @"D:\Code.txt";
        }

        private void Button_Click_InputFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Assemble (.dll)|*.dll|Executable (.exe)|*.exe";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;
            try
            {
                Nullable<bool> result = openFileDialog.ShowDialog();

                if (result == true)
                    this.InputFile.Text = openFileDialog.FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Button_Click_OutputFile(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "Archivo texto (*.txt)|*.txt";
            saveFileDialog.FilterIndex = 1;

            try
            {
                Nullable<bool> result = saveFileDialog.ShowDialog();

                if (result == true)
                {
                    char[] charsToTrim = { ' ' };
                    this.OutputFile.Text = saveFileDialog.FileName.TrimEnd(charsToTrim);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Button_Click_Read(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(this.InputFile.Text) ||
                String.IsNullOrWhiteSpace(this.OutputFile.Text))
                return;

            this.Cursor = Cursors.Wait;

            if (DecompileAssembly(this.InputFile.Text, this.OutputFile.Text))
                MessageBox.Show("Se genero!");
        }

        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public bool DecompileAssembly(string pathToAssembly, string path)
        {
            ///http://stackoverflow.com/questions/8689505/ilspy-how-to-resolve-dependencies
            try
            {
                Assembly assembly = Assembly.ReflectionOnlyLoadFrom(pathToAssembly);

                var resolver = new Mono.Cecil.DefaultAssemblyResolver();
                resolver.AddSearchDirectory(new FileInfo(pathToAssembly).DirectoryName);

                var parameters = new Mono.Cecil.ReaderParameters
                {
                    AssemblyResolver = resolver,
                };

                Mono.Cecil.AssemblyDefinition assemblyDefinition =
                    Mono.Cecil.AssemblyDefinition.ReadAssembly(pathToAssembly, parameters);

                ICSharpCode.Decompiler.Ast.AstBuilder astBuilder =
                    new ICSharpCode.Decompiler.Ast.AstBuilder(
                        new ICSharpCode.Decompiler.DecompilerContext(assemblyDefinition.MainModule));

                astBuilder.AddAssembly(assemblyDefinition);

                StringBuilder sb = new StringBuilder();

                using (StringWriter output = new StringWriter(sb))
                {
                    astBuilder.GenerateCode(new ICSharpCode.Decompiler.PlainTextOutput(output));
                }

                using (StreamWriter outfile = new StreamWriter(path, true))
                {
                    outfile.Write(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Arrow;
                MessageBox.Show(ex.Message);

                return false;
            }

            this.Cursor = Cursors.Arrow;

            return true;
        }
    }

    //Assembly assembly = Assembly.LoadFrom(pathToAssembly);
    //assembly.GetReferencedAssemblies();
    //assembly.GetReferencedAssemblies(assembly);
    //Mono.Cecil.AssemblyDefinition assemblyDefinition = Mono.Cecil.AssemblyDefinition.ReadAssembly(pathToAssembly);
    //var parameters = new Mono.Cecil.ReaderParameters() { AssemblyResolver = new MyAssemblyResolver() };
    //Mono.Cecil.AssemblyDefinition assemblyDefinition =
    //new Mono.Cecil.DefaultAssemblyResolver().Resolve(System.Reflection.AssemblyName.GetAssemblyName(pathToAssembly).ToString());
    //new Helpers.RemoveCompilerAttribute().Run(decompiler.CompilationUnit);
}
