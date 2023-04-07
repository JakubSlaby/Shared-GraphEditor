namespace Plugins.Repositories.GraphEditor.Runtime.Utils
{
	public interface IGraphDataSource
	{
#if UNITY_EDITOR
		string GetScriptPath();
#endif
	}
}