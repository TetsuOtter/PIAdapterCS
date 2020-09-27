using System;
using System.Runtime.InteropServices;

namespace PIAdapterCS
{
	public static class ATSPI_IF
	{
		static SameTargetATSPI STPI;
		static ATSPI_IF()
		{
			STPI = new SameTargetATSPI("TR.BIDSSMemLib.bve5.x64.dll");
		}

		[DllImport("kernel32", CharSet = CharSet.Unicode)]
		static extern void AllocConsole();

		[DllExport]
		static public void Dispose()
		{
			STPI.Dispose();
		}

		[DllExport]
		static public void DoorClose()
		{
			STPI.DoorClose();
		}

		[DllExport]
		static public void DoorOpen()
		{
			STPI.DoorOpen();
		}

		[DllExport]
		static public Hand Elapse(State s, IntPtr Pa, IntPtr So)
		{
			return STPI.Elapse(s, Pa, So);
		}

		[DllExport]
		static public uint GetPluginVersion()
		{
			_ = STPI.GetPluginVersion();
			return ConstValues.ATSPI_IF_GetPIVersion;
		}

		[DllExport]
		static public void HornBlow(int k)
		{
			STPI.HornBlow(k);
		}

		[DllExport]
		static public void Initialize(int s)
		{
			Console.WriteLine("ATSPI_IF Initialize");
			STPI.Initialize(s);
		}

		[DllExport]
		static public void KeyDown(int k)
		{
			STPI.KeyDown(k);
		}

		[DllExport]
		static public void KeyUp(int k)
		{
			STPI.KeyUp(k);
		}

		[DllExport]
		static public void Load()
		{
			AllocConsole();
			Console.WriteLine("ATSPI_IF Load");

			STPI.Load();
		}

		[DllExport]
		static public void SetBeaconData(Beacon b)
		{
			Console.WriteLine("ATSPI_IF SetBeaconData (Z:{0}, Num:{1}, Sig:{2}, Data:{3})", b.Z, b.Num, b.Sig, b.Data);
			STPI.SetBeaconData(b);
		}

		[DllExport]
		static public void SetBrake(int b)
		{
			Console.WriteLine("ATSPI_IF SetBrake ({0})", b);
			STPI.SetBrake(b);
		}

		[DllExport]
		static public void SetPower(int p)
		{
			Console.WriteLine("ATSPI_IF SetPower ({0})", p);
			STPI.SetPower(p);
		}

		[DllExport]
		static public void SetReverser(int r)
		{
			Console.WriteLine("ATSPI_IF SetReverser ({0})", r);
			STPI.SetReverser(r);
		}

		[DllExport]
		static public void SetSignal(int s)
		{
			Console.WriteLine("ATSPI_IF SetSignal ({0})", s);
			STPI.SetSignal(s);
		}

		[DllExport]
		static public void SetVehicleSpec(Spec s)
		{
			STPI.SetVehicleSpec(s);
		}
	}
}
