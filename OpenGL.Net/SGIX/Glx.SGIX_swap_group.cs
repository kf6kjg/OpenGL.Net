
// MIT License
// 
// Copyright (c) 2009-2017 Luca Piccioni
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
// 
// This file is automatically generated

#pragma warning disable 649, 1572, 1573

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace OpenGL
{
	public partial class Glx
	{
		/// <summary>
		/// [GLX] glXJoinSwapGroupSGIX: Binding for glXJoinSwapGroupSGIX.
		/// </summary>
		/// <param name="dpy">
		/// A <see cref="T:IntPtr"/>.
		/// </param>
		/// <param name="drawable">
		/// A <see cref="T:IntPtr"/>.
		/// </param>
		/// <param name="member">
		/// A <see cref="T:IntPtr"/>.
		/// </param>
		[RequiredByFeature("GLX_SGIX_swap_group")]
		public static void JoinSwapGroupSGIX(IntPtr dpy, IntPtr drawable, IntPtr member)
		{
			Debug.Assert(Delegates.pglXJoinSwapGroupSGIX != null, "pglXJoinSwapGroupSGIX not implemented");
			Delegates.pglXJoinSwapGroupSGIX(dpy, drawable, member);
			LogCommand("glXJoinSwapGroupSGIX", null, dpy, drawable, member			);
			DebugCheckErrors(null);
		}

		public unsafe static partial class UnsafeNativeMethods
		{
			#if !NETCORE && !NETSTANDARD1_4
			[SuppressUnmanagedCodeSecurity()]
			#endif
			[DllImport(Library, EntryPoint = "glXJoinSwapGroupSGIX", ExactSpelling = true)]
			internal extern static unsafe void glXJoinSwapGroupSGIX(IntPtr dpy, IntPtr drawable, IntPtr member);

		}

		internal unsafe static partial class Delegates
		{
			[RequiredByFeature("GLX_SGIX_swap_group")]
			#if !NETCORE && !NETSTANDARD1_4
			[SuppressUnmanagedCodeSecurity()]
			#endif
			internal unsafe delegate void glXJoinSwapGroupSGIX(IntPtr dpy, IntPtr drawable, IntPtr member);

			[RequiredByFeature("GLX_SGIX_swap_group")]
			internal static glXJoinSwapGroupSGIX pglXJoinSwapGroupSGIX;

		}
	}

}
