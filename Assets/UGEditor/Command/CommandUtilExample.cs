using UnityEditor;

namespace UGFramework
{
	public class CommandUtilExample 
	{
		static string TEST_FILE_PATH = "Tools/test/test.sh";

		[MenuItem(UGFramework.EditorMenu.TopbarConfig.EXAMPLES + "/CommandUtil/TestShell")]
		public static void TestShell()
		{
			CommandUtil.ExecuteShell(TEST_FILE_PATH);
		}
	}
}