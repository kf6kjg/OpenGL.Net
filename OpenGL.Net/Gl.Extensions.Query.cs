﻿
// Copyright (C) 2015 Luca Piccioni
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
// USA

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OpenGL
{
	public partial class Gl
	{
		/// <summary>
		/// Extension support listing.
		/// </summary>
		public partial class Extensions
		{
			/// <summary>
			/// Query the extensions supported by current platform.
			/// </summary>
			/// <remarks>
			/// An OpenGL context must be current on the calling thread.
			/// </remarks>
			public void Query()
			{
				string glVersion = GetString(StringName.Version);
				if (glVersion == null)
					throw new InvalidOperationException("unable to determine OpenGL version");

				Match glVersionMatch = Regex.Match(glVersion, @"(?<Major>\d+)\..*");
				if (glVersionMatch.Success == false)
					throw new InvalidOperationException("unable to match OpenGL version pattern");

				int glVersionMajor = Int32.Parse(glVersionMatch.Groups["Major"].Value, System.Globalization.NumberFormatInfo.InvariantInfo);
				bool indexedExtensions = (glVersionMajor >= 3) && (Delegates.pglGetStringi != null);

				if (indexedExtensions) {
					int extensionCount;

					Get(GetPName.NumExtensions, out extensionCount);

					List<string> extensions = new List<string>();
					for (uint i = 0; i < (uint)extensionCount; i++)
						extensions.Add(GetString((int)StringName.Extensions, i));

					Query(extensions.ToArray());
				} else {
					Query(GetString(StringName.Extensions));
				}
			}
		}
	}
}