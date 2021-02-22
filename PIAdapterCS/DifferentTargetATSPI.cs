using System;
using System.Diagnostics;
using System.Threading.Tasks;

using TR.ATSPISyncer;

namespace PIAdapterCS
{
	public class DifferentTargetATSPI : IAtsPI
	{
		readonly PISyncer PIS;
		readonly int ind;
		readonly Process process;
		private void ExecFunc(SyncerFlags flag)
		{
			PIS.SetSyncerFlag(ind, flag);

			for (int Count = 0; Count < ConstValues.WaitCountMS && PIS.IsSyncerFlagRaised(ind,flag); Count++)
				Task.Delay(1);//タスクの終了待機
		}

		public DifferentTargetATSPI(in string ExecuteFile, in string Args, in PISyncer pis, in int syncer_index)
		{
			PIS = pis;
			ind = syncer_index;
			process = Process.Start(new ProcessStartInfo() { FileName = ExecuteFile, Arguments = Args }) ?? throw new InvalidOperationException();
		}

		public void Dispose()
		{
			ExecFunc(SyncerFlags.Dispose);
			if (!process.HasExited)//まだ終了してないなら
				process.WaitForExit(ConstValues.WaitCountMS);

			process.Dispose();
		}

		public void DoorClose() => ExecFunc(SyncerFlags.DoorClose);

		public void DoorOpen() => ExecFunc(SyncerFlags.DoorOpen);

		public Hand Elapse(State s, IntPtr Pa, IntPtr So)
		{
			ExecFunc(SyncerFlags.Elapse);
			return PIS.GetHandle();
		}

		public uint GetPluginVersion()
		{
			ExecFunc(SyncerFlags.GetPluginVersion);
			return ConstValues.ATSPI_IF_GetPIVersion;
		}

		public void HornBlow(int k) => ExecFunc(SyncerFlags.HornBlow);

		public void Initialize(int s) => ExecFunc(SyncerFlags.Initialize);

		public void KeyDown(int k) => ExecFunc(SyncerFlags.KeyDown);

		public void KeyUp(int k) => ExecFunc(SyncerFlags.KeyUp);

		public void Load() => ExecFunc(SyncerFlags.Load);

		public void SetBeaconData(Beacon b) => ExecFunc(SyncerFlags.SetBeaconData);

		public void SetBrake(int b) => ExecFunc(SyncerFlags.SetBrake);

		public void SetPower(int p) => ExecFunc(SyncerFlags.SetPower);

		public void SetReverser(int r) => ExecFunc(SyncerFlags.SetReverser);

		public void SetSignal(int s) => ExecFunc(SyncerFlags.SetSignal);

		public void SetVehicleSpec(Spec s) => ExecFunc(SyncerFlags.SetVehicleSpec);
	}
}
