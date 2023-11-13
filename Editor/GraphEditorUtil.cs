using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace WhiteSparrow.Shared.GraphEditor
{
	public class GraphEditorUtil
	{
		public static string FindAbsolutePathToCallingScript(string relativePath = null, int skipFrames = 0)
		{
			StackTrace st = new StackTrace(1 + skipFrames, true);
			var frame = st.GetFrame(0);
			string path = frame.GetFileName();
			if (path == null)
				return relativePath;

			if (!string.IsNullOrWhiteSpace(relativePath))
			{
				if (relativePath.StartsWith("./") || relativePath.StartsWith(".\\"))
					relativePath = relativePath.Remove(0, 2); 
				
				FileInfo fi = new FileInfo(path);
				path = Path.Combine(fi.Directory?.FullName ?? "", relativePath);
			}				


			#if UNITY_EDITOR_WIN
				path = path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			#endif

			return path;

		}

		public static string FindAssetPathToCallingScript(string relativePath = null)
		{
			string path = FindAbsolutePathToCallingScript(relativePath, 1);
			if (path.StartsWith(Application.dataPath))
				return "Assets" + path.Substring(Application.dataPath.Length);
			if (path.Contains("PackageCache"))
				return "Packages" + path.Substring(path.IndexOf("PackageCache", StringComparison.Ordinal) + "PackageCache".Length);
			return path;
		}
		
		
	}
}