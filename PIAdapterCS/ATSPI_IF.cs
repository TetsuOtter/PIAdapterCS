using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace PIAdapterCS
{
	public static class ATSPI_IF
	{
		static public readonly string SyncerSMemFileName = "SMem";
		static public readonly int WaitCountMS = 1000;
		static public readonly bool UseLastHandleOutput = true;

		static readonly List<IAtsPI> STPI = new List<IAtsPI>();//IAtsPIの各関数は, 完了するまで返らないことを想定
		static ATSPI_IF()
		{
			if (AtsPIClassSelector.GetAtsPluginInstance("path", STPI.Count) is IAtsPI pi)
				STPI.Add(pi);

#if DEBUG
			if (!System.Diagnostics.Debugger.IsAttached)
				System.Diagnostics.Debugger.Launch();
#endif
		}

		static private void ExecFuncParallel(Action<int> FuncToExec)
		{
			var result = Parallel.For(0, STPI.Count, FuncToExec.Invoke);

			for (int Count = 0; !result.IsCompleted && Count < WaitCountMS; Count++)
				Task.Delay(1);//タスクの終了待機
		}

		[DllExport]
		static public void Dispose() => ExecFuncParallel((i) => STPI[i].Dispose());

		[DllExport]
		static public void DoorClose() => ExecFuncParallel((i) => STPI[i].DoorClose());

		[DllExport]
		static public void DoorOpen() => ExecFuncParallel((i) => STPI[i].DoorOpen());

		[DllExport]
		static public Hand Elapse(State s, IntPtr Pa, IntPtr So)
		{
			Hand retH = new Hand();
			List<Hand> retHandArr = new List<Hand>();

			for (int i = 0; i < STPI.Count; i++)
				retHandArr.Add(STPI[i].Elapse(s, Pa, So));


			if (UseLastHandleOutput)
				return retHandArr.FindLast((_) => true);

			for(int i = 0; i < retHandArr.Count; i++)
			{
				retH.B = Math.Max(retH.B, retHandArr[i].B); //ブレーキは最大出力を採用する
				retH.P = Math.Max(retH.P, retHandArr[i].P); //力行は最大出力を採用する
				retH.R = retHandArr.FindLast((_) => true).R; //レバーサーは最後の出力を採用する
				retH.C = retHandArr.FindLast((_) => true).C; //定速制御は最後の出力を採用する
			}
			return retH;
		}

		[DllExport]
		static public uint GetPluginVersion()
		{
			ExecFuncParallel((i) => STPI[i].GetPluginVersion());
			return ConstValues.ATSPI_IF_GetPIVersion;
		}

		[DllExport]
		static public void HornBlow(int k) => ExecFuncParallel((i) => STPI[i].HornBlow(k));

		[DllExport]
		static public void Initialize(int s)
		{
			System.Diagnostics.Debug.WriteLine("PIAdapterCS ATSPI_IF Initialize");
			ExecFuncParallel((i) => STPI[i].Initialize(s));
		}

		[DllExport]
		static public void KeyDown(int k) => ExecFuncParallel((i) => STPI[i].KeyDown(k));

		[DllExport]
		static public void KeyUp(int k) => ExecFuncParallel((i) => STPI[i].KeyUp(k));

		[DllExport]
		static public void Load()
		{
			System.Diagnostics.Debug.WriteLine("PIAdapterCS ATSPI_IF Load");

			ExecFuncParallel((i) => STPI[i].Load());
		}

		[DllExport]
		static public void SetBeaconData(Beacon b)
		{
			System.Diagnostics.Debug.WriteLine("PIAdapterCS ATSPI_IF SetBeaconData (Z:{0}, Num:{1}, Sig:{2}, Data:{3})", b.Z, b.Num, b.Sig, b.Data);
			
			ExecFuncParallel((i) => STPI[i].SetBeaconData(b));
		}

		[DllExport]
		static public void SetBrake(int b) => ExecFuncParallel((i) => STPI[i].SetBrake(b));
		

		[DllExport]
		static public void SetPower(int p) => ExecFuncParallel((i) => STPI[i].SetPower(p));

		[DllExport]
		static public void SetReverser(int r) => ExecFuncParallel((i) => STPI[i].SetReverser(r));

		[DllExport]
		static public void SetSignal(int s) => ExecFuncParallel((i) => STPI[i].SetSignal(s));

		[DllExport]
		static public void SetVehicleSpec(Spec s) => ExecFuncParallel((i) => STPI[i].SetVehicleSpec(s));
	}
}
