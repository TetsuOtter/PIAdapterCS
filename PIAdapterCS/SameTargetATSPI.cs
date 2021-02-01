using System;
using System.IO;
using System.Reflection;

namespace PIAdapterCS
{
	/// <summary>TargetPlatformが同じATSプラグインを操作します.</summary>
	public class SameTargetATSPI : IAtsPI
	{
		DllManager DM;

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

		readonly d_Dispose PI_Dispose;
		readonly d_DoorClose PI_DoorClose;
		readonly d_DoorOpen PI_DoorOpen;
		readonly d_Elapse PI_Elapse;
		readonly d_GetPluginVersion PI_GetPluginVersion;
		readonly d_HornBlow PI_HornBlow;
		readonly d_Initialize PI_Initialize;
		readonly d_KeyDown PI_KeyDown;
		readonly d_KeyUp PI_KeyUp;
		readonly d_Load PI_Load;
		readonly d_SetBeaconData PI_SetBeaconData;
		readonly d_SetBrake PI_SetBrake;
		readonly d_SetPower PI_SetPower;
		readonly d_SetReverser PI_SetReverser;
		readonly d_SetSignal PI_SetSignal;
		readonly d_SetVehicleSpec PI_SetVehicleSpec;

		/// <summary>SameTargetATSPIインスタンスを初期化する</summary>
		/// <param name="PIPath">PIへのパス(絶対 or 相対)</param>
		public SameTargetATSPI(string PIPath)
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

		~SameTargetATSPI()//DllManagerは確実に解放する
		{
			DM.Dispose();
		}

		public void Dispose()
		{
			PI_Dispose();

			DM.Dispose();
		}

		public void DoorClose() => PI_DoorClose();
		public void DoorOpen() => PI_DoorOpen();
		public Hand Elapse(State s, IntPtr Pa, IntPtr So) => PI_Elapse(s, Pa, So);
		public uint GetPluginVersion() => PI_GetPluginVersion();
		public void HornBlow(int k) => PI_HornBlow(k);
		public void Initialize(int s) => PI_Initialize(s);
		public void KeyDown(int k) => PI_KeyDown(k);
		public void KeyUp(int k) => PI_KeyUp(k);
		public void Load() => PI_Load();
		public void SetBeaconData(Beacon b) => PI_SetBeaconData(b);
		public void SetBrake(int b) => PI_SetBrake(b);
		public void SetPower(int p) => PI_SetPower(p);
		public void SetReverser(int r) => PI_SetReverser(r);
		public void SetSignal(int s) => PI_SetSignal(s);
		public void SetVehicleSpec(Spec s) => PI_SetVehicleSpec(s);
	}
}
