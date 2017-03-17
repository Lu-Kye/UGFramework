using UnityEditor;
using UnityEngine;

namespace UGFramework.Editor
{
	public class CommandUtilExample : ScriptableObject 
	{
		[MenuItem(TopbarConfig.EXAMPLES + "/CommandUtil/TestShell")]
		public static void TestShell()
		{
			var filePath = "UGEditor/Command/CommandUtilExample/CommandUtilExample.sh";
			CommandUtil.ExecuteShell(filePath);
		}
	}
}