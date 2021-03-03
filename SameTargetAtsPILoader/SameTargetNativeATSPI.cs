using System;
using System.IO;
using System.Reflection;

namespace TR
{
	//AppDomain Ref : https://devlights.hatenablog.com/entry/20120808/p1
	public class SameTargetNativeATSPI : IAtsPI
	{
		private const uint ATSPluginInterfaceVersion = 0x00020000;
		IAtsPI? PI { get; set; }
		AppDomain AnotherDomain { get; }

		public SameTargetNativeATSPI(in string PIPath)
		{
			AnotherDomain = AppDomain.CreateDomain(PIPath.GetHashCode().ToString());

			Assembly asm = Assembly.GetExecutingAssembly();

			//exec ctor with argument ref : https://www.atmarkit.co.jp/fdotnet/dotnettips/854asmcreateinstance/asmcreateinstance.html
			if (typeof(SameTargetNativeATSPI_Manager).FullName is string fn)//NULL Check
				PI = asm.CreateInstance(fn, false, BindingFlags.CreateInstance, null, new object[] { PIPath }, null, null) as IAtsPI;
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

		public uint GetPluginVersion() => PI?.GetPluginVersion() ?? ATSPluginInterfaceVersion;

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


		/// <summary>TargetPlatformが同じATSプラグインを操作します.</summary>
		private class SameTargetNativeATSPI_Manager : IAtsPI
		{
			private const uint ATSPluginInterfaceVersion = 0x00020000;
			readonly DllManager DM;

			delegate void d_Dispose();
			delegate void d_DoorClose();
			delegate void d_DoorOpen();
			delegate Hand d_Elapse(State s, IntPtr Pa, IntPtr So);
			delegate uint d_GetPluginVersion();
			delegate void d_HornBlow(int k);
			delegate void d_Initialize(int s);
			delegate void d_KeyDown(int k);
			delegate void d_KeyUp(int k);
			delegate void d_Load();
			delegate void d_SetBeaconData(Beacon b);
			delegate void d_SetBrake(int b);
			delegate void d_SetPower(int p);
			delegate void d_SetReverser(int r);
			delegate void d_SetSignal(int s);
			delegate void d_SetVehicleSpec(Spec s);

			readonly d_Dispose? PI_Dispose;
			readonly d_DoorClose? PI_DoorClose;
			readonly d_DoorOpen? PI_DoorOpen;
			readonly d_Elapse? PI_Elapse;
			readonly d_GetPluginVersion? PI_GetPluginVersion;
			readonly d_HornBlow? PI_HornBlow;
			readonly d_Initialize? PI_Initialize;
			readonly d_KeyDown? PI_KeyDown;
			readonly d_KeyUp? PI_KeyUp;
			readonly d_Load? PI_Load;
			readonly d_SetBeaconData? PI_SetBeaconData;
			readonly d_SetBrake? PI_SetBrake;
			readonly d_SetPower? PI_SetPower;
			readonly d_SetReverser? PI_SetReverser;
			readonly d_SetSignal? PI_SetSignal;
			readonly d_SetVehicleSpec? PI_SetVehicleSpec;


			/// <summary>SameTargetATSPIインスタンスを初期化する</summary>
			/// <param name="PIPath">PIへのパス(絶対 or 相対)</param>
			public SameTargetNativeATSPI_Manager(in string PIPath)
			{
				//Ref : https://dobon.net/vb/dotnet/file/pathclass.html
				//ref : https://dobon.net/vb/dotnet/file/isabsolutepath.html

				DM = new DllManager(
					Path.IsPathRooted(PIPath) ?//絶対パスかどうか
						PIPath ://絶対パスならそのまま使用
						Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, PIPath)//相対パスなら, 絶対パスに変換して使用
					);


				PI_Dispose = DM.GetProcDelegate<d_Dispose>(nameof(Dispose));
				PI_DoorClose = DM.GetProcDelegate<d_DoorClose>(nameof(DoorClose));
				PI_DoorOpen = DM.GetProcDelegate<d_DoorOpen>(nameof(DoorOpen));
				PI_Elapse = DM.GetProcDelegate<d_Elapse>(nameof(Elapse));
				PI_GetPluginVersion = DM.GetProcDelegate<d_GetPluginVersion>(nameof(GetPluginVersion));
				PI_HornBlow = DM.GetProcDelegate<d_HornBlow>(nameof(HornBlow));
				PI_Initialize = DM.GetProcDelegate<d_Initialize>(nameof(Initialize));
				PI_KeyDown = DM.GetProcDelegate<d_KeyDown>(nameof(KeyDown));
				PI_KeyUp = DM.GetProcDelegate<d_KeyUp>(nameof(KeyUp));
				PI_Load = DM.GetProcDelegate<d_Load>(nameof(Load));
				PI_SetBeaconData = DM.GetProcDelegate<d_SetBeaconData>(nameof(SetBeaconData));
				PI_SetBrake = DM.GetProcDelegate<d_SetBrake>(nameof(SetBrake));
				PI_SetPower = DM.GetProcDelegate<d_SetPower>(nameof(SetPower));
				PI_SetReverser = DM.GetProcDelegate<d_SetReverser>(nameof(SetReverser));
				PI_SetSignal = DM.GetProcDelegate<d_SetSignal>(nameof(SetSignal));
				PI_SetVehicleSpec = DM.GetProcDelegate<d_SetVehicleSpec>(nameof(SetVehicleSpec));
			}
			~SameTargetNativeATSPI_Manager() => DM.Dispose();//DllManagerは確実に解放する

			public void Dispose()
			{
				PI_Dispose?.Invoke();

				DM.Dispose();
			}

			public void DoorClose() => PI_DoorClose?.Invoke();
			public void DoorOpen() => PI_DoorOpen?.Invoke();
			public Hand Elapse(State s, IntPtr Pa, IntPtr So) => PI_Elapse?.Invoke(s, Pa, So) ?? default;
			public uint GetPluginVersion() => PI_GetPluginVersion?.Invoke() ?? ATSPluginInterfaceVersion;
			public void HornBlow(int k) => PI_HornBlow?.Invoke(k);
			public void Initialize(int s) => PI_Initialize?.Invoke(s);
			public void KeyDown(int k) => PI_KeyDown?.Invoke(k);
			public void KeyUp(int k) => PI_KeyUp?.Invoke(k);
			public void Load() => PI_Load?.Invoke();
			public void SetBeaconData(Beacon b) => PI_SetBeaconData?.Invoke(b);
			public void SetBrake(int b) => PI_SetBrake?.Invoke(b);
			public void SetPower(int p) => PI_SetPower?.Invoke(p);
			public void SetReverser(int r) => PI_SetReverser?.Invoke(r);
			public void SetSignal(int s) => PI_SetSignal?.Invoke(s);
			public void SetVehicleSpec(Spec s) => PI_SetVehicleSpec?.Invoke(s);
		}
	}

}
