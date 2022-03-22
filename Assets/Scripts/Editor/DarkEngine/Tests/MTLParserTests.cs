using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Editor.DarkEngine.Files;
using Assets.Scripts.Editor.DarkEngine.Materials;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    class MTLParserTests
    {
        const string exampleMTL = @"// Part of the following texture sets:
terrain_scale 128
render_pass
	{
	texture FAM\_ACC\M1
	}
.ifdef abc
terrain_scale 128
.else
terrain_scale 54
.endif
// Part of the following texture sets:";

        const string exampleMTL2 = "// Part of the following texture sets:\r//\tErt_2\tERT028\rterrain_scale 64\rinclude ../_ACC/GFX_ENVERT.INC\rrender_pass\r\t{\r\ttexture FAM\\_ACC\\ERT028\r\tshaded 1\r\talpha .78\r\t}\r";
        const string exampleMTL3 = @"// Part of the following texture sets:
//	Ert_1	Ert024
terrain_scale 128
render_material_only 1
render_pass
	{
	texture fam\_Grosnus\ERT024
	shaded
	}";

        const string exampleMTL5 = "// Part of the following texture sets:\nterrain_scale 128";

        private TextureFileRepository textureFileRepo;

        [SetUp]
        public void Setup()
        {
            textureFileRepo = new TextureFileRepository();
        }
        
        [Test]
        public void ParsesExample1WithoutError()
        {
            Assert.AreEqual(2, MTLParser.Parse(exampleMTL, "abs/path", "rel/path", textureFileRepo).ReferencedTexturePaths().Count());
        }

        [Test]
        public void ParsesExample2WithIncludeError()
        {
            Assert.Throws(typeof(DirectoryNotFoundException), () => MTLParser.Parse(exampleMTL2, "abs/path", "rel/path", textureFileRepo), "Didn't throw. Probably did not attempt to include");
        }

        [Test]
        public void ParsesExample3WithoutError()
        {
            Assert.AreEqual(1, MTLParser.Parse(exampleMTL3, "abs/path", "rel/path", textureFileRepo).ReferencedTexturePaths().Count());
        }

        [Test]
        public void ParseExample5()
        {
            Assert.AreEqual(1, MTLParser.Parse(exampleMTL5, "abs/path", "rel/path", textureFileRepo).ReferencedTexturePaths().Count());
        }

        [Test]
        public void ParsesTexturePathsCorrectly()
        {
            string path = Path.Combine(Application.dataPath, "Scripts", "Editor", "DarkEngine", "Tests", "test_mtl.inc");
            var dm = MTLParser.Parse(File.ReadAllText(path), path, "rel", textureFileRepo);

            Assert.AreEqual(8, dm.ReferencedTexturePaths().Count());
        }
    }
}
