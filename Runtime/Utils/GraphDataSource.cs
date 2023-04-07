#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using System;
using System.Diagnostics;
#endif

namespace Plugins.Repositories.GraphEditor.Runtime.Utils
{
	public static class GraphDataSource
	{
		public static string GetSourcePath()
		{
#if UNITY_EDITOR
			StackTrace stackTrace = new StackTrace(1, true);
			if(stackTrace.FrameCount == 0)
				return null;

			string path = stackTrace.GetFrame(0).GetFileName();
			if(!string.IsNullOrEmpty(path))
				path = path.Substring(path.LastIndexOf("Assets", StringComparison.Ordinal));
			return path;
#else
			return null;
#endif
		}
		
#if UNITY_EDITOR
		///Get MonoScript reference from type if able
		public static MonoScript MonoScriptFromType(System.Type targetType) {
			if ( targetType == null ) return null;
			var typeName = targetType.Name;
			if ( targetType.IsGenericType ) {
				targetType = targetType.GetGenericTypeDefinition();
				typeName = typeName.Substring(0, typeName.IndexOf('`'));
			}
			return AssetDatabase.FindAssets(string.Format("{0} t:MonoScript", typeName))
								.Select(AssetDatabase.GUIDToAssetPath)
								.Select(AssetDatabase.LoadAssetAtPath<MonoScript>)
								.FirstOrDefault(m => m != null && m.GetClass() == targetType);
		}
		
		public static bool OpenScriptOfType(System.Type type) {
			var mono = MonoScriptFromType(type);
			if ( mono != null ) {
				AssetDatabase.OpenAsset(mono);
				return true;
			}
			return false;
		}
#endif
	}
}