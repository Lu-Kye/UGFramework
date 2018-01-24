using System.Diagnostics;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UGFramework.UGEditor
{
	/**
	 * --- DOC BEGIN ---
	 * An executor for external command tools like python, shell, bat and so on.
	 * --- DOC END ---
	 */ 
	public class CommandTool 
	{
		public static string Root = Application.dataPath + "/..";

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
#if UNITY_EDITOR
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			processStartInfo.FileName = command;
			processStartInfo.Arguments = string.IsNullOrEmpty(arguments) ? "" : arguments;
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

			if (string.IsNullOrEmpty(output.Trim()) == false)
				UnityEngine.Debug.Log(output);
			if (string.IsNullOrEmpty(error.Trim()) == false) 
				UnityEngine.Debug.LogError(error);

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
#else
			return new Result();
#endif
		}

		/**
		 * --- DOC BEGIN ---
		 * ## Note
		 * Give the execute access to the shell file.
		 * --- DOC END ---
		 */ 
		public static Result ExecuteShell(string shellFile, string arguments = null)
		{
			Execute("chmod", "+x " + Root + "/" + shellFile);
			return Execute(Root + "/" + shellFile, arguments);  	
		}
	}
}