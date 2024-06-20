using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Runtime.Serialization;
using Microsoft.VisualBasic;
namespace abc;
public static class XOREncryption
{
    static void Main(string[] args)
    {

        Console.OutputEncoding = System.Text.Encoding.Unicode;
        Console.InputEncoding = System.Text.Encoding.Unicode;
        // string projectPath = @"D:\dotnetcore\test dll";
        //string projectPath = "";
        if (args.Length > 0)
        {
            projectPath = args[0]; // Thay đổi đường dẫn này đến thư mục gốc của dự án

        }
        else
        {
            Console.WriteLine("nhập folder project");
            projectPath = Console.ReadLine()!;
        }
        if (File.Exists(projectPath + "\\xor_atribuite.cs"))
        {
            bb = File.ReadAllText(projectPath + "\\xor_atribuite.cs");

        }
        try
        {
            var file = Directory.GetFiles(projectPath, "*.csproj");
            foreach (var item in file)
            {
                try
                {
                    string c = File.ReadAllText(item);
                    string a = $"""<PackageReference Include="CompileTimeObfuscator" Version="1.0.0">{Environment.NewLine}      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>{Environment.NewLine}      <PrivateAssets>all</PrivateAssets>{Environment.NewLine}    </PackageReference>""";
                    if (!c.Contains(a))
                    {
                        File.WriteAllText(item, c.Replace("</Project>", $"""  <ItemGroup>{Environment.NewLine}    <PackageReference Include="CompileTimeObfuscator" Version="1.0.0">{Environment.NewLine}      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>{Environment.NewLine}      <PrivateAssets>all</PrivateAssets>{Environment.NewLine}    </PackageReference>{Environment.NewLine}  </ItemGroup>{Environment.NewLine}</Project>"""));
                    }
                }
                catch { }
            }
        }
        catch { }
        try
        {
            XOREncryption.setup(projectPath);
            abc.XOREncryption.EncryptStringsInProject(projectPath);
            XOREncryption.end();
            XOREncryption.coppy(projectPath);
        }
        catch
        {

        }
    }
    static bool ClearBufferWhenDispose = true;
    static int KeyLength = 10;
    static string? bb = "";
    static StringBuilder s = new StringBuilder();
    static string projectPath="";
    public static void coppy(string projectPath)
    {
        if (!File.Exists(projectPath + "\\xor_atribuite.cs"))
        {
            File.WriteAllText(projectPath + "\\xor_atribuite.cs", s.ToString());
        }
        else
        {
            string cc = File.ReadAllText(projectPath + "\\xor_atribuite.cs");
            File.WriteAllText(projectPath + "\\xor_atribuite.cs", cc.Replace("}}//", "") + s.ToString());
        }
    }
    public static void setup(string projectPath)
    {
        if (File.Exists(projectPath + "\\xor_atribuite.cs"))
        {
            return;
            //  string cc = File.ReadAllText(projectPath + "\\xor_atribuite.cs");
            //  File.WriteAllText(projectPath + "\\xor_atribuite.cs", cc.Replace("}//",""));
        }
        else
        {

            s.Append(@"using CompileTimeObfuscator;
namespace A{
    public partial class B
    {");
        }
    }
    public static void end()
    {
        s.Append("}}//");
    }
    public static void Creat(string string_need, string name_var)
    {
        if (s.ToString().Contains(name_var))
        {
            return;
        }
        if (string_need.Length <= 1)
        {
            return;
        }
        if (!string.IsNullOrEmpty(bb))
        {
            if (bb!.Contains(name_var + "()"))
            {
                return;
            }
        }
        File.AppendAllText(projectPath+"\\map.txt", name_var + "|" + KeyLength.ToString() + "|" + ClearBufferWhenDispose.ToString().ToLower() + "|" + string_need+ Environment.NewLine);
        s.Append(@$"[ObfuscatedString({string_need}, KeyLength = {KeyLength.ToString()}, ClearBufferWhenDispose = {ClearBufferWhenDispose.ToString().ToLower()})]
        public static partial string {name_var}();{Environment.NewLine}");
    }
    public static void EncryptStringsInProject(string projectPath)
    {
        //    var files = Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories);
        var files = GetFilesExcludingObj(projectPath, "*.cs");
        foreach (var file in files)
        {
            if (file.Contains("\\xor_atribuite.cs"))
            {
                continue;
            }
            Console.WriteLine(file);
            File.WriteAllText(file, get(File.ReadAllText(file)));

        }
    }
    private static List<string> GetFilesExcludingObj(string root, string searchPattern)
    {
        var files = new List<string>();
        var directories = Directory.GetDirectories(root, "*", SearchOption.AllDirectories);

        foreach (var directory in directories)
        {
            if (directory.Contains(Path.DirectorySeparatorChar + "obj") || directory.Contains(Path.DirectorySeparatorChar + "bin"))
            {
                continue;
            }

            files.AddRange(Directory.GetFiles(directory, searchPattern));
        }
        files.AddRange(Directory.GetFiles(root, searchPattern));
        return files;
    }


    static Random random = new Random();
    public static string ComputeMD5Hash(string input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            return "S" + sb.ToString();
        }
    }
    public static SyntaxNode FindParentOfType(SyntaxNode node, SyntaxKind syntaxKind)
    {
        return FindParentOfType(node, new[] { syntaxKind });
    }

    public static SyntaxNode FindParentOfType(SyntaxNode node, SyntaxKind[] syntaxKinds)
    {
        SyntaxNode pNode = node.Parent;
        SyntaxNode pParent = null;

        do
        {
            if (pNode == null)
            {
                break;
            }

            if (syntaxKinds.Any(s => pNode.IsKind(s)))
            {
                pParent = pNode;
                break;
            }

            pNode = pNode.Parent;
        } while (pParent == null && pNode != null);

        return pParent;
    }

    public static bool IsConstStatement(SyntaxNode node)
    {
        SyntaxNode pField = FindParentOfType(node, new[] { SyntaxKind.FieldDeclaration, SyntaxKind.LocalDeclarationStatement });

        if (pField == null)
        {
            return false;
        }

        bool isConst = false;
        foreach (SyntaxToken t in pField.ChildTokens())
        {
            if (t.IsKind(SyntaxKind.ConstKeyword))
            {
                isConst = true;
                break;
            }
        }

        return isConst;
    }

    public static bool CanReplaceString(SyntaxNode node)
    {
        bool canReplace = true;
        SyntaxNode pNode = node.Parent!;
        do
        {
            if (pNode == null)
            {
                break;
            }

            // The SyntaxKind below is required to be a constant, therefore we cannot replace it with dynamic code (like a b64 decode).
            switch (pNode.Kind())
            {
                case SyntaxKind.Attribute:
                    canReplace = false;
                    break;
                case SyntaxKind.CaseSwitchLabel:
                    canReplace = false;
                    break;
                case SyntaxKind.Parameter:
                    canReplace = false;
                    break;
                case SyntaxKind.VariableDeclaration:
                    if (IsConstStatement(pNode))
                    {
                        canReplace = false;
                    }
                    break;
            }
            pNode = pNode.Parent!;
        } while (true);

        return canReplace;
    }
    public static SyntaxNode FindChildOfType(SyntaxNode node, SyntaxKind syntaxKind)
    {
        return FindChildOfType(node, new[] { syntaxKind });
    }

    public static SyntaxNode FindChildOfType(SyntaxNode node, SyntaxKind[] syntaxKinds)
    {
        SyntaxNode pChild = null!;

        foreach (SyntaxNode pNode in node.ChildNodes())
        {
            if (syntaxKinds.Any(s => pNode.IsKind(s)))
            {
                pChild = pNode;
                break;
            }
        }

        return pChild;
    }
    public static bool HasInterpolationFormat(SyntaxNode node)
    {
        SyntaxNode interpolation = FindChildOfType(node, SyntaxKind.Interpolation);
        if (interpolation == null)
        {
            return false;
        }

        SyntaxNode interpolationFormatClause = FindChildOfType(interpolation, SyntaxKind.InterpolationFormatClause);
        return (interpolationFormatClause != null);
    }
    public static string get(string input)
    {
        //  string sourceCode = File.ReadAllText(path);
        SyntaxTree tree = CSharpSyntaxTree.ParseText(input);
        var root = tree.GetRoot();

        // var root = (CompilationUnitSyntax)tree.GetRoot();
        var interpolatedStrings = root.DescendantNodes().OfType<InterpolatedStringExpressionSyntax>().ToList();
        List<InterpolatedStringExpressionSyntax>? interpolatedStrings2 = new List<InterpolatedStringExpressionSyntax>();

        foreach (var interpolatedString in interpolatedStrings)
        {
            // if (!HasInterpolationFormat(interpolatedString))
            // {
            //     continue;
            // }
            if (!CanReplaceString(interpolatedString))
            {
                continue;
            }
            interpolatedStrings2.Add(interpolatedString);
            //  input = input.Replace(interpolatedString.ToFullString(), BuildRegularString(interpolatedString));
            // Console.WriteLine(interpolatedString);
        }
        root = root.ReplaceNodes(
        interpolatedStrings2,
        (originalNode, rewrittenNode) =>
        {
            return SyntaxFactory.ParseExpression(BuildRegularString(originalNode));
        });
        var stringLiterals = root.DescendantNodes().OfType<LiteralExpressionSyntax>().ToList();
        List<LiteralExpressionSyntax>? stringLiterals2 = new List<LiteralExpressionSyntax>();
        foreach (var literal in stringLiterals)
        {
            //literal.ReplaceToken((SyntaxToken)literal,literal);
            if (!literal.IsKind(SyntaxKind.StringLiteralExpression))
            {
                continue;
            }
            if (literal.Token.ValueText.Length <= 1)
            {
                continue;
            }
            if (!CanReplaceString(literal))
            {
                continue;
            }
            stringLiterals2.Add(literal);
            Creat(literal.ToFullString(), ComputeMD5Hash(literal.ToFullString()));
            // input = input.Replace(literal.ToFullString(), "A.B." + ComputeMD5Hash(literal.ToFullString()) + "()");
            Console.WriteLine(literal);
        }

        root = root.ReplaceNodes(
                   stringLiterals2,
                   (originalNode, rewrittenNode) =>
                   {
                       return SyntaxFactory.ParseExpression("A.B." + ComputeMD5Hash(originalNode.ToFullString()) + "()");

                   });
        return root.ToFullString();
    }
    private static string BuildRegularString(InterpolatedStringExpressionSyntax interpolatedString)
    {
        // Initialize an empty string builder
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        bool a = true;
        var d = interpolatedString.ToFullString().Replace(interpolatedString.Contents.ToFullString(), "RP").Replace("$", "");
        // Traverse each content of the interpolated string
        foreach (var content in interpolatedString.Contents)
        {

            if (content is InterpolatedStringTextSyntax text)
            {
                if (text.TextToken.ValueText.Length < 1)
                {
                    continue;
                }
                else if (text.TextToken.ValueText.Length <= 1)
                {
                    if (a)
                    {
                        sb.Append(d.Replace("RP", text.ToFullString()));
                        a = false;
                    }
                    else
                    {
                        sb.Append("+" + d.Replace("RP", text.ToFullString()));
                    }
                    continue;
                }
                else
                {
                    if (a)
                    {
                        sb.Append("A.B." + ComputeMD5Hash(d.Replace("RP", text.ToFullString())) + "()");
                        a = false;
                    }
                    else
                    {
                        sb.Append("+ A.B." + ComputeMD5Hash(d.Replace("RP", text.ToFullString())) + "()");
                    }
                }
                Creat(d.Replace("RP", text.ToFullString()), ComputeMD5Hash(d.Replace("RP", text.ToFullString())));

                // Append the plain text part
            }
            else if (content is InterpolationSyntax interpolation)
            {
                if (a)
                {
                    sb.Append(interpolation.Expression.ToFullString());
                    a = false;
                }
                else
                {
                    sb.Append("+" + interpolation.Expression.ToFullString());
                }
                // Append the interpolated expression enclosed in braces
            }
        }

        // Return the constructed regular string
        return sb.ToString();
    }
}
