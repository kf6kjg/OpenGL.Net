
// Copyright (C) 2011-2017 Luca Piccioni
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

// Field is never assigned to, and will always have its default value null
// Note: fields in Memory class will be assigned by means of reflections, indeed disable the warning on this module

#pragma warning disable 649

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Reflection.Emit;

namespace OpenGL
{
	/// <summary>
	/// Main interface for executing operations on memory.
	/// </summary>
	public unsafe class Memory
	{
		#region Constructors

		/// <summary>
		/// Static constructor.
		/// </summary>
		static Memory()
		{
			// Try to load optional library
			//LoadSimdExtensions();

			// Ensure MemoryCopy functionality
			EnsureMemoryCopy();
		}

		#endregion

		#region Set

		public static void Set(IntPtr addr, byte value, uint count)
		{
			_MemsetDelegate(addr, value, count);
		}

		/// <summary>
		/// Utility route for generating 'memset' delegate.
		/// </summary>
		/// <returns></returns>
		private static Action<IntPtr, byte, uint> GenerateMemsetDelegate()
		{
#if !NETCORE && !NETSTANDARD1_4 && !NETSTANDARD2_0
			DynamicMethod dynamicMethod = new DynamicMethod(
				"Memset", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard,
				null,
				new [] { typeof(IntPtr), typeof(byte), typeof(uint) },
				typeof(Memory), true
			);

			ILGenerator generator = dynamicMethod.GetILGenerator();
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldarg_2);
			generator.Emit(OpCodes.Initblk);
			generator.Emit(OpCodes.Ret);

			return (Action<IntPtr, byte, uint>)dynamicMethod.CreateDelegate(typeof(Action<IntPtr, byte, uint>));
#else
			return (new Action<IntPtr, byte, uint>(delegate(IntPtr addr, byte value, uint count) { throw new NotImplementedException(); }));
#endif
		}

		/// <summary>
		/// Delegate executing a memory set operation.
		/// </summary>
		private static Action<IntPtr, byte, uint> _MemsetDelegate = GenerateMemsetDelegate();

		#endregion

		#region Copy

		/// <summary>
		/// Delegate used for copy memory.
		/// </summary>
		/// <param name="dst"></param>
		/// <param name="src"></param>
		/// <param name="bytes"></param>
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#if !NETCORE && !NETSTANDARD1_4
		[System.Security.SuppressUnmanagedCodeSecurity()]
#endif
		private delegate void MemoryCopyDelegate(void *dst, void* src, ulong bytes);

		/// <summary>
		/// Cached delegate for copy memory.
		/// </summary>
		private static MemoryCopyDelegate MemoryCopyPointer;

		/// <summary>
		/// Copy memory.
		/// </summary>
		/// <param name="dst"></param>
		/// <param name="src"></param>
		/// <param name="bytes"></param>
		public static unsafe void MemoryCopy(void* dst, void* src, ulong bytes)
		{
			MemoryCopyPointer(dst, src, bytes);
#if false
			byte *dstPointer = (byte *)dst, srcPointer = (byte *)src;
			byte *dstEnd = dstPointer + bytes;

			while (dstPointer < dstEnd) {
				*dstPointer = *srcPointer;
				dstPointer++; srcPointer++;
			}
#endif
		}

		/// <summary>
		/// Copy memory.
		/// </summary>
		/// <param name="dst"></param>
		/// <param name="src"></param>
		/// <param name="bytes"></param>
		public static void MemoryCopy(IntPtr dst, IntPtr src, ulong bytes)
		{
			MemoryCopy(dst.ToPointer(), src.ToPointer(), bytes);
		}

		/// <summary>
		/// Copy memory.
		/// </summary>
		/// <param name="dst"></param>
		/// <param name="src"></param>
		/// <param name="bytes"></param>
		public static void MemoryCopy(IntPtr dst, Array src, ulong bytes)
		{
			MemoryCopy(dst, src, 0, bytes);
		}

		/// <summary>
		/// Copy memory from array to unmanaged memory.
		/// </summary>
		/// <param name="dst">
		/// A <see cref="IntPtr"/> that specify the address of the destination unmanaged memory.
		/// </param>
		/// <param name="src">
		/// A <see cref="Array"/> that specify the source array object.
		/// </param>
		/// <param name="srcOffset">
		/// A <see cref="UInt32"/> that specify the offset to apply to memory copied from <paramref name="src"/>. This
		/// value is expressed in bytes.
		/// </param>
		/// <param name="bytes">
		/// A <see cref="UInt64"/> that specify the number of bytes to copy.
		/// </param>
		public static void MemoryCopy(IntPtr dst, Array src, uint srcOffset, ulong bytes)
		{
			if (dst == IntPtr.Zero)
				throw new ArgumentNullException("dst");
			if (src == null)
				throw new ArgumentNullException("src");
			if (src.Rank > 1)
				throw new ArgumentException("multidimensional array", "src");

			GCHandle srcArray = GCHandle.Alloc(src, GCHandleType.Pinned);
			try {
				IntPtr srcArrayPtr = new IntPtr(srcArray.AddrOfPinnedObject().ToInt64() + (long)srcOffset);

				// Copy from array to aligned buffer
				MemoryCopyPointer(dst.ToPointer(), srcArrayPtr.ToPointer(), bytes);
			} finally {
				srcArray.Free();
			}
		}

		/// <summary>
		/// Copy memory.
		/// </summary>
		/// <param name="dst"></param>
		/// <param name="src"></param>
		/// <param name="bytes"></param>
		public static void MemoryCopy(Array dst, IntPtr src, ulong bytes)
		{
			GCHandle dstArray = GCHandle.Alloc(dst, GCHandleType.Pinned);

			try {
				// Copy from array to aligned buffer
				MemoryCopyPointer(
					dstArray.AddrOfPinnedObject().ToPointer(), 
					src.ToPointer(),
					bytes
					);
			} finally {
				dstArray.Free();
			}
		}

		/// <summary>
		/// Copy memory.
		/// </summary>
		/// <param name="dst"></param>
		/// <param name="src"></param>
		/// <param name="bytes"></param>
		public static void MemoryCopy(Array dst, Array src, ulong bytes)
		{
			GCHandle dstArray = GCHandle.Alloc(dst, GCHandleType.Pinned);
			GCHandle srcArray = GCHandle.Alloc(src, GCHandleType.Pinned);

			try {
				// Copy from array to aligned buffer
				MemoryCopyPointer(
					dstArray.AddrOfPinnedObject().ToPointer(), 
					srcArray.AddrOfPinnedObject().ToPointer(),
					bytes
					);
			} finally {
				dstArray.Free();
				srcArray.Free();
			}
		}

		/// <summary>
		/// Ensure <see cref="MemoryCopy"/> functionality.
		/// </summary>
		private static void EnsureMemoryCopy()
		{
			if (MemoryCopyPointer == null) {
				IntPtr memoryCopyPtr;

				switch (Platform.CurrentPlatformId) {
					case Platform.Id.WindowsNT:
						memoryCopyPtr = GetProcAddressOS.GetProcAddress("msvcrt.dll", "memcpy");
						if (memoryCopyPtr != IntPtr.Zero) {
							MemoryCopyPointer = (MemoryCopyDelegate)Marshal.GetDelegateForFunctionPointer(memoryCopyPtr, typeof(MemoryCopyDelegate));
							return;
						}
						
						throw new NotSupportedException("no suitable memcpy support");
					case Platform.Id.Linux:
						memoryCopyPtr = GetProcAddressOS.GetProcAddress("libc.so.6", "memcpy");
						if (memoryCopyPtr != IntPtr.Zero) {
							MemoryCopyPointer = (MemoryCopyDelegate)Marshal.GetDelegateForFunctionPointer(memoryCopyPtr, typeof(MemoryCopyDelegate));
							return;
						}
						
						throw new NotSupportedException("no suitable memcpy support");
					default:
						throw new NotSupportedException("no suitable memcpy support");
				}
			}
		}

		#endregion
	}
}
