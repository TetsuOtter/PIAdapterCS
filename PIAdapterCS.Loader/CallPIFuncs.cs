using System;
using System.Threading.Tasks;

using TR;
using TR.ATSPISyncer;

namespace PIAdapterCS.Loader
{
	class CallPIFuncs
	{
		public bool IsDisposed { get; private set; }
		readonly PISyncer pis;
		readonly IAtsPI pi;
		readonly int SyncerIndex = -1;

		public CallPIFuncs(in string a_MMFPath, in string a_ModPath, in int a_SyncerIndex)
		{
			SyncerIndex = a_SyncerIndex;
			pis = new PISyncer(a_MMFPath);
			pi = SameTargetATSPILoader.LoadNativeOrClrPI(a_ModPath);
		}

		/// <summary>関数実行の必要性を確認し, 必要なら実行する</summary>
		public void CheckAndExecFuncs()
		{
			if (pis.IsSyncerFlagRaised(SyncerIndex, SyncerFlags.Dispose))
			{
				pi.Dispose();
				pis.SetSyncerFlagToLower(SyncerIndex, SyncerFlags.Dispose);
				pis.Dispose();
				IsDisposed = true;
				return;//Disposeで終了
			}

			CheckExecFunc(SyncerFlags.DoorClose, pi.DoorClose);
			CheckExecFunc(SyncerFlags.DoorOpen, pi.DoorOpen);
			CheckExecFunc(SyncerFlags.Elapse, () => pis.DoElapseFunc(pi.Elapse, ConstValues.PanelArrSize, ConstValues.SoundArrSize));
			CheckExecFunc(SyncerFlags.GetPluginVersion, () => pi.GetPluginVersion());
			CheckExecFuncI(SyncerFlags.HornBlow, pi.HornBlow);
			CheckExecFuncI(SyncerFlags.Initialize, pi.Initialize);
			CheckExecFuncI(SyncerFlags.KeyDown, pi.KeyDown);
			CheckExecFuncI(SyncerFlags.KeyUp, pi.KeyUp);
			CheckExecFunc(SyncerFlags.Load, pi.Load);
			CheckExecFuncA(SyncerFlags.SetBeaconData, (a) => pi.SetBeaconData(a.beacon));
			CheckExecFuncI(SyncerFlags.SetBrake, pi.SetBrake);
			CheckExecFuncI(SyncerFlags.SetPower, pi.SetPower);
			CheckExecFuncI(SyncerFlags.SetReverser, pi.SetReverser);
			CheckExecFuncI(SyncerFlags.SetSignal, pi.SetSignal);
			CheckExecFuncA(SyncerFlags.SetVehicleSpec, (a) => pi.SetVehicleSpec(a.spec));

			Task.Delay(1);
		}

		void CheckExecFunc(in SyncerFlags f, in Action act)//引数のない処理を確認/実行
		{
			if (pis.IsSyncerFlagRaised(SyncerIndex, f))
			{
				act.Invoke();
				pis.SetSyncerFlagToLower(SyncerIndex, f);
			}
		}
		void CheckExecFuncI(in SyncerFlags f, Action<int> act) => CheckExecFuncA(f, (a) => act.Invoke(a.intValue));//int型引数の処理を確認/実行
		void CheckExecFuncA(in SyncerFlags f, in Action<ArgData> act)//引数をもつメソッドを確認/実行
		{
			if (pis.IsSyncerFlagRaised(SyncerIndex, f))
			{
				act.Invoke(pis.TouchArgData());
				pis.SetSyncerFlagToLower(SyncerIndex, f);
			}
		}

	}
}
