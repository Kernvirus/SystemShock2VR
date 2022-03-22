using Assets.Scripts.Editor.DarkEngine.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.Materials
{
    public class MTLParser
    {
        public static MTL Parse(string mtlContent, string absMTLPath, string relMTLPath, TextureFileRepository textureFileRepo)
        {
            // fix for really old line breaks
            mtlContent = mtlContent.Replace('\r', '\n');
            LinkedList<string> lines = new LinkedList<string>(mtlContent.Split('\n'));

            bool ignoreLines = false;
            MTL darkMaterial = new MTL(relMTLPath);
            DarkRenderPass renderpass = null;
            LinkedListNode<string> currentNode = lines.First;
            string baseTexturePath = PathUtility.FilePathWithoutExtension(relMTLPath).ToLower();
            while (currentNode != null)
            {
                string line = currentNode.Value.Trim();

                // remove comments
                int commentStart = line.IndexOf("//");
                if (commentStart != -1)
                {
                    line = line.Substring(0, commentStart);
                }
                commentStart = line.IndexOf("#");
                if (commentStart != -1)
                {
                    line = line.Substring(0, commentStart);
                }

                string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0)
                {
                    ignoreLines = ParsePreprocessorDirektive(parts, ignoreLines);
                    if (!ignoreLines)
                    {
                        if (renderpass != null)
                        {
                            if (parts[0] == "}")
                            {
                                darkMaterial.AddRenderpass(renderpass);
                                renderpass = null;
                            }
                            else
                            {
                                var mtlStatement = new MTLStatement(line);
                                ParseRenderPass(renderpass, mtlStatement, textureFileRepo, baseTexturePath);
                            }
                        }

                        switch (parts[0])
                        {
                            // render pass
                            case "render_pass":
                                renderpass = new DarkRenderPass();
                                break;
                            case "render_material_only":
                                if (parts.Length >= 2)
                                {
                                    int num;
                                    if (int.TryParse(parts[1], out num))
                                    {
                                        darkMaterial.renderMaterialOnly = num != 0;
                                    }
                                }
                                else
                                {
                                    darkMaterial.renderMaterialOnly = true;
                                }
                                break;
                            case "include":
                                string path = Path.Combine(Path.GetDirectoryName(absMTLPath), parts[1]);
                                string content;
                                try
                                {
                                    content = File.ReadAllText(path).Replace('\r', '\n');
                                }
                                catch (IOException)
                                {
                                    content = "";
                                    Debug.LogError("Couldn't find mtl incl: " + path);
                                }
                                string[] newLines = content.Split('\n');
                                foreach (var l in newLines.Reverse())
                                {
                                    lines.AddAfter(currentNode, l);
                                }
                                break;
                            case "illum_map":
                                // illum_map < intensity:float> < texture >
                                renderpass = new DarkRenderPass();
                                renderpass.texturePaths = new List<string>();
                                renderpass.texturePaths.Add(parts[2]);
                                darkMaterial.AddRenderpass(renderpass);
                                renderpass = null;
                                break;
                            case "terrain_scale":
                                if (parts.Length == 3)
                                {
                                    darkMaterial.terrainScale = new Vector2Int(int.Parse(parts[1]),
                                        int.Parse(parts[2]));
                                }
                                else
                                {
                                    darkMaterial.terrainScale = new Vector2Int(int.Parse(parts[1]),
                                           int.Parse(parts[1]));
                                }
                                break;
                            case "ani_frames":
                                darkMaterial.aniFrames = int.Parse(parts[1]);
                                break;
                            case "u_material":
                                darkMaterial.uMaterial = (MTL.UMaterial)Enum.Parse(typeof(MTL.UMaterial), parts[1], true);
                                break;
                        }
                    }
                }
                currentNode = currentNode.Next;
            }

            if (!darkMaterial.renderMaterialOnly && !textureFileRepo.DoesNameExist(baseTexturePath))
                darkMaterial.renderMaterialOnly = true;
            return darkMaterial;
        }

        private static bool ParsePreprocessorDirektive(string[] parts, bool ignoreLines)
        {
            switch (parts[0])
            {
                // preprocessor
                case ".ifdef":
                    return EvaluatePreprocessorDirektive(parts[1]);
                case ".ifndef":
                    return !EvaluatePreprocessorDirektive(parts[1]);
                case ".else":
                    return !ignoreLines;
                case ".endif":
                    return false;
            }
            return ignoreLines;
        }

        private static bool EvaluatePreprocessorDirektive(string directive)
        {
            int number = 0;
            if (int.TryParse(directive, out number))
                return number != 0;
            else if (directive == "DX6")
                return false;
            else
            {
                Debug.Log("Could not evaluate expression. " + directive);
                return false;
            }
        }

        private static void ParseRenderPass(DarkRenderPass renderpass, MTLStatement statement, TextureFileRepository textureFileRepo, string baseTextureName)
        {
            if (statement.NextPartMatches("texture"))
            {
                renderpass.texturePaths = new List<string>(ParseTexture(statement.Parts, baseTextureName, textureFileRepo));
            }
            else if (statement.NextPartMatches("uv_source"))
            {
                renderpass.uvSource = (DarkRenderPass.UVSource)Enum.Parse(typeof(DarkRenderPass.UVSource), statement.Current, true);
            }
            else if (statement.NextPartMatches("shaded"))
            {
                renderpass.shaded = !statement.NextPartMatches("0");
            }
            else if (statement.NextPartMatches("ani_rate"))
            {
                renderpass.aniRate = int.Parse(statement.Current);
            }
            else if (statement.NextPartMatches("u_type"))
            {
                renderpass.uType = (DarkRenderPass.UType)Enum.Parse(typeof(DarkRenderPass.UType), statement.Current, true);
            }
            else if (statement.NextPartMatches("alpha"))
            {
                if (statement.NextPartMatches("func"))
                {
                    if (statement.NextPartMatches("wave"))
                    {
                        renderpass.alphaFunction = WaveFunction.Parse(statement.Parts);
                    }
                    else if (statement.NextPartMatches("incidence"))
                    {
                        renderpass.alphaFunction = IncidenceFunction.Parse(statement.Parts);
                    }
                }
            }
            else if (statement.NextPartMatches("rgb"))
            {
                if (statement.NextPartMatches("func"))
                {
                    if (statement.NextPartMatches("wave"))
                    {
                        renderpass.rgbFunction = WaveFunction.Parse(statement.Parts);
                    }
                    else if (statement.NextPartMatches("incidence"))
                    {
                        renderpass.rgbFunction = IncidenceFunction.Parse(statement.Parts);
                    }
                }
            }
        }

        private static string[] ParseTexture(string[] parts, string baseTextureName, TextureFileRepository textureFileRepo)
        {
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].ToLower() == "$texture")
                    parts[i] = baseTextureName;
            }

            if (parts.Length == 2)
            {
                // single
                return new string[] { parts[1] };
            }
            else if (parts[1] == "{")
            {
                // custom sequence
                string[] tex = new string[parts.Length - 3];
                for (int i = 0; i < tex.Length; i++)
                {
                    tex[i] = parts[i + 2];
                }
                return tex;
            }
            else if (parts.Length == 4)
            {
                // numeric sequence
                return TexturePathsFromNumSeq(parts[1], int.Parse(parts[2]), parts[3], textureFileRepo).ToArray();
            }
            throw new Exception("Unknown texture format. " + parts[0]);
        }

        private static IEnumerable<string> TexturePathsFromNumSeq(string mod, int limit, string basePath, TextureFileRepository textureFileRepo)
        {
            // base path may be in ""
            basePath = basePath.Replace("\"", "");

            yield return basePath;

            var stack = new Stack<char>();
            if (mod.Length == 1)
            {
                for (var i = basePath.Length - 1; i >= 0; i--)
                {
                    if (!char.IsDigit(basePath[i]))
                    {
                        break;
                    }
                    stack.Push(basePath[i]);
                }
                mod = "";
            }
            else
            {
                mod = "_";
            }

            int startNumber = 0;
            if (stack.Count > 0)
            {
                startNumber = int.Parse(new string(stack.ToArray()));
                basePath = basePath.Substring(0, basePath.Length - stack.Count);
            }

            if (limit > 0)
            {
                for (int i = 1; i < limit; i++)
                {
                    yield return basePath + mod + (startNumber + i).ToString("D" + stack.Count);
                }
            }
            else
            {
                int i = 1;
                string path = basePath + mod + (startNumber + i).ToString("D" + stack.Count);
                while (textureFileRepo.DoesNameExist(path))
                {
                    yield return path;
                    i++;
                    path = basePath + mod + (startNumber + i).ToString("D" + stack.Count);
                }
            }
        }
    }
}
