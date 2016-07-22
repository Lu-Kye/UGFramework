using UnityEngine;
using System.Collections;
using UnityEditor;

namespace UGFramework
{
	public class CommandUtilExample 
	{
		[MenuItem("UGFrameworkExamples/CommandUtil/Test")]
		public static void Test()
		{
			CommandUtil.ExecuteShell("Tools/ShellTools/test.sh");			
		}
	}
}