﻿
// Copyright (C) 2015-2017 Luca Piccioni
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;

using NUnit.Framework;

namespace OpenGL.Test
{
	/// <summary>
	/// Test OpenGL 2.0 API.
	/// </summary>
	[TestFixture]
	[Category("GL_VERSION_2_0")]
	class Gl_VERSION_2_0 : TestBaseGL
	{
		/// <summary>
		/// Test Gl.GetString.
		/// </summary>
		[Test]
		public void TestGetString()
		{
			if (!HasVersion(2, 0) && !HasEsVersion(2, 0))
				Assert.Inconclusive("OpenGL 2.0 or OpenGL ES 2.0");

			#region Gl.SHADING_LANGUAGE_VERSION

			string shadingLanguageVersion = Gl.GetString(StringName.ShadingLanguageVersion);

			Console.WriteLine("Shading Language version: {0}", shadingLanguageVersion);

			#endregion
		}

		/// <summary>
		/// Test Gl.CreateProgram.
		/// </summary>
		[Test]
		public void TestCreateProgram()
		{
			if (!HasVersion(2, 0) && !HasEsVersion(2, 0))
				Assert.Inconclusive("OpenGL 2.0 or OpenGL ES 2.0");

			uint program = Gl.CreateProgram();
			Assert.AreNotEqual(0, program, "Gl.CreateProgram failure");

			try {
				Assert.IsTrue(Gl.IsProgram(program));
			} finally {
				if (program != 0) {
					Gl.DeleteProgram(program);
					Assert.IsFalse(Gl.IsProgram(program), "Gl.DeleteProgram failure");
				}
			}
		}

		/// <summary>
		/// Test Gl.CreateProgram.
		/// </summary>
		[Test]
		public void TestCreateShader()
		{
			if (!HasVersion(2, 0) && !HasEsVersion(2, 0))
				Assert.Inconclusive("OpenGL 2.0 or OpenGL ES 2.0");

			uint shader = Gl.CreateShader(ShaderType.VertexShader);
			try {
				Assert.AreNotEqual(0, shader, "Gl.CreateShader failure");
				Assert.IsTrue(Gl.IsShader(shader));

				int shaderGet;

				Gl.GetShader(shader, ShaderParameterName.ShaderType, out shaderGet);
				Assert.AreEqual(Gl.VERTEX_SHADER, shaderGet);
				Gl.GetShader(shader, ShaderParameterName.DeleteStatus, out shaderGet);
				Assert.AreEqual(Gl.FALSE, shaderGet);
				Gl.GetShader(shader, ShaderParameterName.CompileStatus, out shaderGet);
				Assert.AreEqual(Gl.FALSE, shaderGet);

			} finally {
				if (shader != 0) {
					Gl.DeleteShader(shader);
					Assert.IsFalse(Gl.IsShader(shader), "Gl.DeleteShader failure");
				}
			}
		}
	}
}
