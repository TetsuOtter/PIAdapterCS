using System;

namespace TR
{
	public class SameTargetIAtsPIPlugin : IAtsPI
	{
		private const uint SupportPIVersion = 0x00020000;
		IAtsPI? PI { get; set; }

		//AppDomain ref : https://devlights.hatenablog.com/entry/20120808/p1
		AppDomain AnotherDomain { get; }

		public SameTargetIAtsPIPlugin(in string assembly_fullname, in string typename)
		{
			AnotherDomain = AppDomain.CreateDomain(assembly_fullname + typename);

			PI = AnotherDomain.CreateInstanceAndUnwrap(assembly_fullname, typename) as IAtsPI;//IAtsPIにのみ対応
		}
		Hand handRec = new Hand();
		public void Dispose()
		{
			PI?.Dispose();
			PI = null;
			AppDomain.Unload(AnotherDomain);
		}

		public void DoorClose() => PI?.DoorClose();

		public void DoorOpen() => PI?.DoorOpen();

		public Hand Elapse(State s, IntPtr Pa, IntPtr So) => PI?.Elapse(s, Pa, So) ?? handRec;

		public uint GetPluginVersion() => PI?.GetPluginVersion() ?? SupportPIVersion;

		public void HornBlow(int k) => PI?.HornBlow(k);

		public void Initialize(int s) => PI?.Initialize(s);

		public void KeyDown(int k) => PI?.KeyDown(k);

		public void KeyUp(int k) => PI?.KeyUp(k);

		public void Load() => PI?.Load();

		public void SetBeaconData(Beacon b) => PI?.SetBeaconData(b);

		public void SetBrake(int b)
		{
			handRec.B = b;
			PI?.SetBrake(b);
		}

		public void SetPower(int p)
		{
			handRec.P = p;
			PI?.SetPower(p);
		}

		public void SetReverser(int r)
		{
			handRec.R = r;
			PI?.SetReverser(r);
		}

		public void SetSignal(int s) => PI?.SetSignal(s);

		public void SetVehicleSpec(Spec s) => PI?.SetVehicleSpec(s);
	}
}
