using System;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace PIAdapterCS
{
	public class PISyncer : IDisposable
	{
		readonly MemoryMappedFile MMF_Flags, MMF_PanelD, MMF_SoundD, MMF_ArgData, MMF_Handle;
		private bool disposedValue;

		public PISyncer(in string MMF_FileName)
		{
			if (!File.Exists(MMF_FileName))
				File.Create(MMF_FileName, ConstValues.PISyncer_MMFFileBufSize);

			MMF_Flags = MemoryMappedFile.CreateFromFile(MMF_FileName, FileMode.Open, ConstValues.PISyncer_Name_Flags);
			MMF_PanelD = MemoryMappedFile.CreateFromFile(MMF_FileName, FileMode.Open, ConstValues.PISyncer_Name_PanelD);
			MMF_SoundD = MemoryMappedFile.CreateFromFile(MMF_FileName, FileMode.Open, ConstValues.PISyncer_Name_SoundD);
			MMF_ArgData = MemoryMappedFile.CreateFromFile(MMF_FileName, FileMode.Open, ConstValues.PISyncer_Name_ArgData);
			MMF_Handle = MemoryMappedFile.CreateFromFile(MMF_FileName, FileMode.Open, ConstValues.PISyncer_Name_Handle);
		}

		public void SetSyncerFlag(in int index, in SyncerFlags value) => SetSyncerFlag(index, (uint)value);
		public void SetSyncerFlag(in int index, in uint value)
		{
			using var va = MMF_Flags.CreateViewAccessor(index, sizeof(uint));

			va.Write(0, va.ReadUInt32(0) | value);
		}

		public void SetSyncerFlagToLower(in int index, in SyncerFlags value) => SetSyncerFlagToLower(index, (uint)value);
		public void SetSyncerFlagToLower(in int index, in uint value)
		{
			using var va = MMF_Flags.CreateViewAccessor(index, sizeof(uint));

			va.Write(0, va.ReadUInt32(0) & ~value);//ビットごとの補数をAND
		}

		public bool IsSyncerFlagRaised(in int index, in SyncerFlags value) => IsSyncerFlagRaised(index, (uint)value);
		public bool IsSyncerFlagRaised(in int index, in uint value)
		{
			using var va = MMF_Flags.CreateViewAccessor(index, sizeof(uint));

			return (va.ReadUInt32(0) & value) == value;//AND演算の結果がCheckFlagsと一致するなら, CheckFlagsはすべて立ってる.
		}


		public void DoElapseFunc(Func<State,IntPtr,IntPtr,Hand> elapse, in int pnlarr_len, in int sndarr_len)
		{
			using var va_pnl = MMF_PanelD.CreateViewAccessor(sizeof(int), sizeof(int) * pnlarr_len);
			using var va_snd = MMF_SoundD.CreateViewAccessor(sizeof(int), sizeof(int) * sndarr_len);
			using var va_hnd = MMF_Handle.CreateViewAccessor();

			Hand h = elapse.Invoke(TouchArgData().state, va_pnl.SafeMemoryMappedViewHandle.DangerousGetHandle(), va_snd.SafeMemoryMappedViewHandle.DangerousGetHandle());
			va_hnd.Write(0, ref h);
		}

		/// <summary>MemoryMappedFileにあるPanelデータを取得します</summary>
		/// <param name="arr">書き込み先配列</param>
		/// <param name="length">読み込む要素数</param>
		public void GetPanelFromMMF(int[] arr, in int length)
		{
			using var va = MMF_PanelD.CreateViewAccessor();

			va.ReadArray(sizeof(int), arr, 0, length);//オフセットは将来の「可変配列長」機能用
		}
		
		/// <summary>MemoryMappedFileのPanelデータを指定値で更新します</summary>
		/// <param name="arr">書き込む値が記録された配列</param>
		/// <param name="length">書き込む要素数</param>
		public void SetPanelToMMF(in int[] arr, in int length)
		{
			using var va = MMF_PanelD.CreateViewAccessor();

			va.WriteArray(sizeof(int), arr, 0, length);//オフセットは将来の「可変配列長」機能用
		}
		
		/// <summary>MemoryMappedFileにあるSoundデータを取得します</summary>
		/// <param name="arr">書き込み先配列</param>
		/// <param name="length">読み込む要素数</param>
		public void GetSoundFromMMF(int[] arr, in int length)
		{
			using var va = MMF_SoundD.CreateViewAccessor();

			va.ReadArray(sizeof(int), arr, 0, length);//オフセットは将来の「可変配列長」機能用
		}

		/// <summary>MemoryMappedFileのSoundデータを指定値で更新します</summary>
		/// <param name="arr">書き込む値が記録された配列</param>
		/// <param name="length">書き込む要素数</param>
		public void SetSoundToMMF(in int[] arr, in int length)
		{
			using var va = MMF_SoundD.CreateViewAccessor();

			va.WriteArray(sizeof(int), arr, 0, length);//オフセットは将来の「可変配列長」機能用
		}

		public ArgData TouchArgData(Action<ArgData>? act = null)
		{
			using var va_stt = MMF_ArgData.CreateViewAccessor();

			va_stt.Read(0, out ArgData ad);

			if (act != null)
			{
				act.Invoke(ad);//適当な操作
				va_stt.Write(0, ref ad);//操作後の値を書き込み
			}

			return ad;
		}

		public Hand GetHandle()
		{
			using var va = MMF_Handle.CreateViewAccessor();
			
			va.Read(0, out Hand h);

			return h;
		}

		#region Dispose
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					MMF_Flags?.Dispose();
					MMF_PanelD?.Dispose();
					MMF_SoundD?.Dispose();
					MMF_ArgData?.Dispose();
					MMF_Handle?.Dispose();
				}

				disposedValue = true;
			}
		}

		// // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
		// ~PISyncer()
		// {
		//     // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
		//     Dispose(disposing: false);
		// }

		public void Dispose()
		{
			// このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}

	public struct ArgData
	{
		public State state;
		public Spec spec;
		public Beacon beacon;
		public int intValue;
	}

	[Flags]
	public enum SyncerFlags : uint
	{
		Load = 1 << 0,
		Dispose = 1 << 1,
		GetPluginVersion = 1 << 2,
		SetVehicleSpec = 1 << 3,
		Initialize = 1 << 4,
		Elapse = 1 << 5,
		SetPower = 1 << 6,
		SetBrake = 1 << 7,
		SetReverser = 1 << 8,
		DoorOpen = 1 << 9,
		DoorClose = 1 << 10,
		KeyDown = 1 << 11,
		KeyUp = 1 << 12,
		HornBlow = 1 << 13,
		SetSignal = 1 << 14,
		SetBeaconData = 1 << 15,
	}
}
