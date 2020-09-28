using System;
using System.Collections.Generic;
using System.Text;

namespace PIAdapterCS
{
	public class DifferentTargetATSPI : IAtsPI
	{
		PISyncer PIS;
		public DifferentTargetATSPI(string ExecuteFile, string Args, PISyncer pis, int syncer_index)
		{

		}
		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public void DoorClose()
		{
			throw new NotImplementedException();
		}

		public void DoorOpen()
		{
			throw new NotImplementedException();
		}

		public Hand Elapse(State s, IntPtr Pa, IntPtr So)
		{
			throw new NotImplementedException();
		}

		public uint GetPluginVersion()
		{
			throw new NotImplementedException();
		}

		public void HornBlow(int k)
		{
			throw new NotImplementedException();
		}

		public void Initialize(int s)
		{
			throw new NotImplementedException();
		}

		public void KeyDown(int k)
		{
			throw new NotImplementedException();
		}

		public void KeyUp(int k)
		{
			throw new NotImplementedException();
		}

		public void Load()
		{
			throw new NotImplementedException();
		}

		public void SetBeaconData(Beacon b)
		{
			throw new NotImplementedException();
		}

		public void SetBrake(int b)
		{
			throw new NotImplementedException();
		}

		public void SetPower(int p)
		{
			throw new NotImplementedException();
		}

		public void SetReverser(int r)
		{
			throw new NotImplementedException();
		}

		public void SetSignal(int s)
		{
			throw new NotImplementedException();
		}

		public void SetVehicleSpec(Spec s)
		{
			throw new NotImplementedException();
		}
	}
}
