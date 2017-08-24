using UnityEditor;
using UnityEngine;

namespace UGFramework.UGEditor
{
	public class CommandToolxample : ScriptableObject 
	{
		[MenuItem(MenuConfig.EXAMPLES + "/CommandTool/Shell")]
		public static void Shell()
		{
			var filePath = "UGEditor/CommandTool/CommandToolExample/Shell.sh";
			CommandTool.ExecuteShell(filePath);
		}
	}
}