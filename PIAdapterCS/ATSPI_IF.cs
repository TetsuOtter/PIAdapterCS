using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using TR.ATSPISyncer;

namespace PIAdapterCS
{
	public static class ATSPI_IF
	{
		static readonly List<IAtsPI> STPI = new List<IAtsPI>();//IAtsPIの各関数は, 完了するまで返らないことを想定
		static PISyncer syncer;

		static ATSPI_IF()//TypeInitializationException
		{
#if DEBUG
			if (!System.Diagnostics.Debugger.IsAttached)
				System.Diagnostics.Debugger.Launch();//Not Called
#endif
			try
			{
				syncer = new PISyncer(new Random().Next().ToString());
			}catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
				throw;
			}
		}

		static private void ExecFuncParallel(Action<int> FuncToExec)
		{
			if (STPI.Count <= 0)
				return;

			var result = Parallel.For(0, STPI.Count, FuncToExec.Invoke);

			for (int Count = 0; !result.IsCompleted && Count < ConstValues.WaitCountMS; Count++)
				Task.Delay(1);//タスクの終了待機
		}

		[DllExport(CallingConvention= CallingConvention.StdCall)]
		static public void Load()
		{
			System.Diagnostics.Debug.WriteLine("PIAdapterCS ATSPI_IF Load");
			try
			{
				if (AtsPIClassSelector.GetAtsPluginInstance(System.IO.Path.Combine(ConstValues.DllDirectory, "TR.BIDSSMemLib.bve5.x86.dll"), STPI.Count, syncer) is IAtsPI pi)//////////TO DEBUG
					STPI.Add(pi);
			}catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
				return;
			}
			ExecFuncParallel((i) => STPI[i].Load());
		}


		[DllExport(CallingConvention= CallingConvention.StdCall)]
		static public void Dispose()
		{
			ExecFuncParallel((i) => STPI[i].Dispose());
			STPI.Clear();
		}

		[DllExport(CallingConvention= CallingConvention.StdCall)]
		static public void DoorClose() => ExecFuncParallel((i) => STPI[i].DoorClose());

		[DllExport(CallingConvention= CallingConvention.StdCall)]
		static public void DoorOpen() => ExecFuncParallel((i) => STPI[i].DoorOpen());

		static Hand myHand = new Hand();
		[DllExport(CallingConvention= CallingConvention.StdCall)]
		static public Hand Elapse(State s, IntPtr Pa, IntPtr So)
		{
			Hand retH = new Hand();
			List<Hand> retHandArr = new List<Hand>();

			if (STPI.Count <= 0)
				return myHand;

			for (int i = 0; i < STPI.Count; i++)
				retHandArr.Add(STPI[i].Elapse(s, Pa, So));


			if (ConstValues.UseLastHandleOutput)
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

		[DllExport(CallingConvention= CallingConvention.StdCall)]
		static public uint GetPluginVersion()
		{
			ExecFuncParallel((i) => STPI[i].GetPluginVersion());
			return ConstValues.ATSPI_IF_GetPIVersion;
		}

		[DllExport(CallingConvention= CallingConvention.StdCall)]
		static public void HornBlow(int k) => ExecFuncParallel((i) => STPI[i].HornBlow(k));

		[DllExport(CallingConvention= CallingConvention.StdCall)]
		static public void Initialize(int s)
		{
			System.Diagnostics.Debug.WriteLine("PIAdapterCS ATSPI_IF Initialize");
			ExecFuncParallel((i) => STPI[i].Initialize(s));
		}

		[DllExport(CallingConvention= CallingConvention.StdCall)]
		static public void KeyDown(int k) => ExecFuncParallel((i) => STPI[i].KeyDown(k));

		[DllExport(CallingConvention= CallingConvention.StdCall)]
		static public void KeyUp(int k) => ExecFuncParallel((i) => STPI[i].KeyUp(k));

		[DllExport(CallingConvention= CallingConvention.StdCall)]
		static public void SetBeaconData(Beacon b)
		{
			System.Diagnostics.Debug.WriteLine("PIAdapterCS ATSPI_IF SetBeaconData (Z:{0}, Num:{1}, Sig:{2}, Data:{3})", b.Z, b.Num, b.Sig, b.Data);
			
			ExecFuncParallel((i) => STPI[i].SetBeaconData(b));
		}

		[DllExport(CallingConvention = CallingConvention.StdCall)]
		static public void SetBrake(int b)
		{
			myHand.B = b;
			ExecFuncParallel((i) => STPI[i].SetBrake(b));
		}


		[DllExport(CallingConvention = CallingConvention.StdCall)]
		static public void SetPower(int p)
		{
			myHand.P = p;
			ExecFuncParallel((i) => STPI[i].SetPower(p));
		}

		[DllExport(CallingConvention = CallingConvention.StdCall)]
		static public void SetReverser(int r)
		{
			myHand.R = r;
			ExecFuncParallel((i) => STPI[i].SetReverser(r));
		}

		[DllExport(CallingConvention= CallingConvention.StdCall)]
		static public void SetSignal(int s) => ExecFuncParallel((i) => STPI[i].SetSignal(s));

		[DllExport(CallingConvention= CallingConvention.StdCall)]
		static public void SetVehicleSpec(Spec s) => ExecFuncParallel((i) => STPI[i].SetVehicleSpec(s));
	}
}
