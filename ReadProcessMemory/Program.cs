using System;
using System.Collections.Generic;
using System.Linq;
using static PInvoke.Kernel32;

namespace ReadProcessMemory
{
	class Program
	{
		static void Main(string[] args)
		{
			FindHearthstoneProcess();
			Console.WriteLine("Press any key to exit");
			Console.ReadKey(true);
		}

		static void FindHearthstoneProcess()
		{
			int[] pids = GetPids("Hearthstone.exe").ToArray();

			if (pids.Length == 0)
			{
				Console.WriteLine("Couldn't find Hearthstone.exe");
				return;
			}

			int pid = pids[0];
			ACCESS_MASK accessMask = ProcessAccess.PROCESS_QUERY_INFORMATION | ProcessAccess.PROCESS_VM_READ;

			using (SafeObjectHandle hProcess = OpenProcess(accessMask, false, pid))
			{
				if (hProcess.IsInvalid)
				{
					Console.WriteLine($"Hearthstone process handle is valid: {!hProcess.IsInvalid}");
					return;
				}
				Console.WriteLine($"Hearthstone process handle is valid: {hProcess.DangerousGetHandle()}");
			}
		}

		static IEnumerable<int> GetPids(string processName)
		{
			using (SafeObjectHandle hSnapshot = CreateToolhelp32Snapshot(CreateToolhelp32SnapshotFlags.TH32CS_SNAPPROCESS, 0))
			{
				foreach (int id in Process32Enumerate(hSnapshot).Where(process => process.ExeFile == processName).Select(process => process.th32ProcessID))
				{
					yield return id;
				}
			}
		}
	}
}
