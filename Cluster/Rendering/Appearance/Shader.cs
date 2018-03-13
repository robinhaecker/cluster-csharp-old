﻿using System;
using OpenTK.Graphics.OpenGL;

namespace Cluster.Rendering.Appearance
{
    class Shader
    {
        int program;
        int vert, frag, geom;

        public Shader(string vertexShaderSource, string fragmentShaderSource, string geometryShaderSource = "")
        {
            program = GL.CreateProgram();

            vert = GL.CreateShader(ShaderType.VertexShader);
            frag = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vert, vertexShaderSource);
            GL.ShaderSource(frag, fragmentShaderSource);

            GL.CompileShader(vert);
            GL.CompileShader(frag);

            string log = GL.GetShaderInfoLog(vert);
            if (log != "") Console.Write("Compilation Error in vertex shader:\n{0}\n", log);
            log = GL.GetShaderInfoLog(frag);
            if (log != "") Console.Write("Compilation Error in fragment shader:\n{0}\n", log);

            if (geometryShaderSource != "")
            {
                geom = GL.CreateShader(ShaderType.GeometryShader);
                GL.ShaderSource(geom, geometryShaderSource);
                GL.CompileShader(geom);
                log = GL.GetShaderInfoLog(geom);
                if (log != "") Console.Write("Compilation Error in geometry shader:\n{0}\n", log);

                GL.AttachShader(program, geom);
            }

            GL.AttachShader(program, vert);
            GL.AttachShader(program, frag);

            GL.LinkProgram(program);

            GL.DetachShader(program, vert);
            GL.DetachShader(program, frag);

            if (geom != 0)
            {
                GL.DetachShader(program, geom);
            }
        }


        public void bind()
        {
            GL.UseProgram(program);
        }

        public static void unbind()
        {
            GL.UseProgram(0);
        }

        public int getHandle()
        {
            return program;
        }

        public int getUniformLocation(string uniform)
        {
            return GL.GetUniformLocation(program, uniform);
        }

        public void cleanUp()
        {
            GL.DeleteProgram(program);
            GL.DeleteShader(vert);
            GL.DeleteShader(frag);
            if (geom != 0) GL.DeleteShader(geom);
        }
    }
}