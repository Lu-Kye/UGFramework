using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Text;
using UnityEditor;

namespace UGFramework
{
	/**
	 * --- DOC BEGIN ---
	 * # 命令行调用工具
	 * - 执行shell
	 * - 执行python
	 * --- DOC END ---
	 */ 
	public class CommandUtil 
	{
		public static string Root = Application.dataPath.Replace("Assets", "");

		public class Result
		{
			public string error;
			public string output;
			public bool IsSuccessful
			{
				get 
				{
					return string.IsNullOrEmpty(error);	
				}
			}
		}

		public static Result Execute(string command, string arguments = null)
		{
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			processStartInfo.FileName = command;
			if (string.IsNullOrEmpty(arguments))
				processStartInfo.Arguments = "";

			processStartInfo.CreateNoWindow = false;
			processStartInfo.ErrorDialog = true;
			processStartInfo.UseShellExecute = false;
			processStartInfo.StandardErrorEncoding = Encoding.UTF8;
			processStartInfo.StandardOutputEncoding = Encoding.UTF8;
			processStartInfo.RedirectStandardError = true;
			processStartInfo.RedirectStandardOutput = true;

			UnityEngine.Debug.Log(command + " " + arguments);

			Process process = Process.Start(processStartInfo);

			string output = process.StandardOutput.ReadToEnd();
			string error = process.StandardError.ReadToEnd();

			if (string.IsNullOrEmpty(output.Trim()))
				UnityEngine.Debug.Log(command + arguments + " Executed.");
			else
				UnityEngine.Debug.Log(output);

			if (!string.IsNullOrEmpty(error.Trim ())) 
			{
				UnityEngine.Debug.LogError(error);
			}

			EditorUtility.DisplayProgressBar(
				"Command Executing", 
				string.IsNullOrEmpty(arguments) ? command : command + " " + arguments, 
				1.0f);
			
			process.WaitForExit();
			process.Close();

			EditorUtility.ClearProgressBar();

			Result result = new Result();
			result.error = error;
			result.output = output;
			return result;
		}

		/**
		 * --- DOC BEGIN ---
		 * ## Note
		 * 需要给予shell脚本执行权限
		 * --- DOC END ---
		 */ 
		public static Result ExecuteShell(string shellFile, string arguments = null)
		{
			return Execute(Root + shellFile, arguments);  	
		}
	}
}