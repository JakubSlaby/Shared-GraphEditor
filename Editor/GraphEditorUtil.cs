using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

namespace WhiteSparrow.Shared.GraphEditor
{
	public static class GraphEditorUtil
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

		private static Regex s_PackageVersionRegex = new Regex("PackageCache/[^@]*(?<rev>@[^/]+)");
		
		public static string FindAssetPathToCallingScript(string relativePath = null)
		{
			string path = FindAbsolutePathToCallingScript(relativePath, 1);
			if (path.StartsWith(Application.dataPath))
				return "Assets" + path.Substring(Application.dataPath.Length);
			var match = s_PackageVersionRegex.Match(path);
			if (match.Success)
				path = path.Remove(match.Groups["rev"].Index, match.Groups["rev"].Length);
			if (path.Contains("PackageCache"))
				path = "Packages" + path.Substring(path.IndexOf("PackageCache", StringComparison.Ordinal) + "PackageCache".Length);
			if (!path.Contains("Packages") && Path.IsPathRooted(path))
			{
				if (!FindPackageName(path, out string packageName, out string packageRoot))
				{
					throw new Exception("Unable to resolve package root");
				}
				
				if (path.StartsWith(packageRoot))
					return $"Packages/{packageName}" + path.Substring(packageRoot.Length);
				
			}
			Debug.Log(path);
			return path;
		}

		private static bool FindPackageName(string path, out string packageName, out string packageRoot)
		{
			packageName = packageRoot = null;
			string packagePath = FindPackageRoot(path);
			if (packagePath == null)
				return false;
			FileInfo packageFile = new FileInfo(packagePath);
			packageRoot = packageFile.Directory?.FullName;

			string json = File.ReadAllText(packagePath);
			try
			{
				var jObject = JObject.Parse(json);
				packageName = jObject["name"]?.Value<string>() ?? null;
				return !string.IsNullOrWhiteSpace(packageName);
			}
			catch (Exception e)
			{
				return false;
			}
			
		}
		private static string FindPackageRoot(string path)
		{
			FileInfo file = new FileInfo(path);
			DirectoryInfo directory = file.Directory;

			while (directory != null)
			{
				string candidate = Path.Combine(directory.FullName, "package.json");
				if (File.Exists(candidate))
					return candidate;
                
				directory = directory.Parent;
			}

			return null;
		}

		public static T FindInParent<T>(this VisualElement instance)
		{
			var iterator = instance;
			while (iterator != null)
			{
				if (iterator is T iT)
					return iT;
				
				iterator = iterator.parent;
			}

			return default(T);
		}
		
		
	}
}