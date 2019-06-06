﻿using OpenTK.Graphics.OpenGL;
using SFGraphics.GLObjects.Shaders;
using System;
using System.Collections.Generic;
using System.IO;

namespace StudioSB.Rendering
{
    public class ShaderManager
    {
        private static Dictionary<string, Shader> shaderByName = new Dictionary<string, Shader>();

        /// <summary>
        /// Gets the shader by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>shader with specified name and null otherwise </returns>
        public static Shader GetShader(string name)
        {
            if (shaderByName.ContainsKey(name))
                return shaderByName[name];
            else
                return null;
        }

        public static void SetUpShaders()
        {
            shaderByName.Clear();

            Shader rModel = CreateRModelShader();
            shaderByName.Add("UltimateModel", rModel);

            Shader rModelUv = CreateRModelUvShader();
            shaderByName.Add("UltimateModelUV", rModelUv);

            // TODO: This shader can be generated by SFGraphics.
            Shader rModelDebug = CreateRModelDebugShader();
            shaderByName.Add("UltimateModelDebug", rModelDebug);

            Shader textureShader = CreateTextureShader();
            shaderByName.Add("Texture", textureShader);

            Shader boneShader = CreateBoneShader();
            shaderByName.Add("Bone", boneShader);

            Shader textShader = CreateTextShader();
            shaderByName.Add("Text", textShader);

            Shader sphShader = CreateSphereShader();
            shaderByName.Add("Sphere", sphShader);

            //Shader capShader = CreateCapsuleShader();
            //shaderByName.Add("Capsule", capShader);

            Shader primShader = CreatePrismShader();
            shaderByName.Add("Prism", primShader);

            Shader cubeShader = CreateCubeShader();
            shaderByName.Add("CubeMap", cubeShader);

            foreach (var pair in shaderByName)
            {
                if (!pair.Value.LinkStatusIsOk)
                {
                    System.Diagnostics.Debug.WriteLine(pair.Key);
                    System.Diagnostics.Debug.WriteLine(pair.Value.GetErrorLog());
                }
            }
        }

        private static Shader CreateSphereShader()
        {
            Shader shader = new Shader();
            shader.LoadShaders(File.ReadAllText("Shaders/Sphere.vert"), File.ReadAllText("Shaders/SolidColor.frag"));
            return shader;
        }

        private static Shader CreateCapsuleShader()
        {
            Shader shader = new Shader();
            shader.LoadShaders(File.ReadAllText("Shaders/Capsule.vert"), File.ReadAllText("Shaders/SolidColor.frag"));
            return shader;
        }

        private static Shader CreateTextShader()
        {
            Shader textShader = new Shader();
            textShader.LoadShaders(File.ReadAllText("Shaders/Text.vert"), File.ReadAllText("Shaders/Text.frag"));
            return textShader;
        }

        private static Shader CreateCubeShader()
        {
            Shader cubeShader = new Shader();
            cubeShader.LoadShaders(File.ReadAllText("Shaders/CubeMap.vert"), File.ReadAllText("Shaders/CubeMap.frag"));
            return cubeShader;
        }

        private static Shader CreateBoneShader()
        {
            Shader boneShader = new Shader();
            boneShader.LoadShaders(File.ReadAllText("Shaders/Bone.vert"), File.ReadAllText("Shaders/SolidColor.frag"));
            return boneShader;
        }

        private static Shader CreatePrismShader()
        {
            Shader boneShader = new Shader();
            boneShader.LoadShaders(File.ReadAllText("Shaders/RectangularPrism.vert"), File.ReadAllText("Shaders/SolidColor.frag"));
            return boneShader;
        }

        private static Shader CreateTextureShader()
        {
            Shader textureShader = new Shader();
            textureShader.LoadShaders(new List<Tuple<string, ShaderType, string>>()
            {
                new Tuple<string, ShaderType, string>(File.ReadAllText("Shaders/Texture.frag"), ShaderType.FragmentShader, ""),
                new Tuple<string, ShaderType, string>(File.ReadAllText("Shaders/Texture.vert"), ShaderType.VertexShader, ""),
                new Tuple<string, ShaderType, string>(File.ReadAllText("Shaders/Gamma.frag"), ShaderType.FragmentShader, ""),
            });
            return textureShader;
        }

        private static Shader CreateRModelDebugShader()
        {
            Shader rModelDebug = new Shader();
            rModelDebug.LoadShaders(new List<Tuple<string, ShaderType, string>>()
            {
                new Tuple<string, ShaderType, string>(File.ReadAllText("Shaders/UltimateModelDebug.frag"), ShaderType.FragmentShader, ""),
                new Tuple<string, ShaderType, string>(File.ReadAllText("Shaders/UltimateModel.geom"), ShaderType.GeometryShader, ""),
                new Tuple<string, ShaderType, string>(File.ReadAllText("Shaders/NormalMap.frag"), ShaderType.FragmentShader, ""),
                new Tuple<string, ShaderType, string>(File.ReadAllText("Shaders/UltimateModel.vert"), ShaderType.VertexShader, ""),
                new Tuple<string, ShaderType, string>(File.ReadAllText("Shaders/Gamma.frag"), ShaderType.FragmentShader, ""),
                new Tuple<string, ShaderType, string>(File.ReadAllText("Shaders/Wireframe.frag"), ShaderType.FragmentShader, ""),
                new Tuple<string, ShaderType, string>(File.ReadAllText("Shaders/TextureLayers.frag"), ShaderType.FragmentShader, "")
            });

            return rModelDebug;
        }

        private static Shader CreateRModelUvShader()
        {
            Shader rModelUv = new Shader();
            rModelUv.LoadShaders(new List<Tuple<string, ShaderType, string>>()
            {
                new Tuple<string, ShaderType, string>(File.ReadAllText("Shaders/UltimateModelUV.vert"), ShaderType.VertexShader, ""),
                new Tuple<string, ShaderType, string>(File.ReadAllText("Shaders/UltimateModel.geom"), ShaderType.GeometryShader, ""),
                new Tuple<string, ShaderType, string>(File.ReadAllText("Shaders/UltimateModelUV.frag"), ShaderType.FragmentShader, ""),
                new Tuple<string, ShaderType, string>(File.ReadAllText("Shaders/NormalMap.frag"), ShaderType.FragmentShader, ""),
                new Tuple<string, ShaderType, string>(File.ReadAllText("Shaders/Wireframe.frag"), ShaderType.FragmentShader, ""),
            });
            return rModelUv;
        }

        private static Shader CreateRModelShader()
        {
            Shader rModel = new Shader();
            rModel.LoadShaders(new List<Tuple<string, ShaderType, string>>()
            {
                new Tuple<string, ShaderType, string>(File.ReadAllText("Shaders/UltimateModel.frag"), ShaderType.FragmentShader, ""),
                new Tuple<string, ShaderType, string>(File.ReadAllText("Shaders/UltimateModel.geom"), ShaderType.GeometryShader, ""),
                new Tuple<string, ShaderType, string>(File.ReadAllText("Shaders/NormalMap.frag"), ShaderType.FragmentShader, ""),
                new Tuple<string, ShaderType, string>(File.ReadAllText("Shaders/UltimateModel.vert"), ShaderType.VertexShader, ""),
                new Tuple<string, ShaderType, string>(File.ReadAllText("Shaders/Gamma.frag"), ShaderType.FragmentShader, ""),
                new Tuple<string, ShaderType, string>(File.ReadAllText("Shaders/Wireframe.frag"), ShaderType.FragmentShader, ""),
                new Tuple<string, ShaderType, string>(File.ReadAllText("Shaders/TextureLayers.frag"), ShaderType.FragmentShader, "")
            });
            return rModel;
        }
    }
}
