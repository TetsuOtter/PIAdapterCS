using System;
using System.IO;

namespace PIAdapterCS.Loader
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			if (!(args?.Length > 0))
				throw new ArgumentException("実行時引数は必ずセットする必要があります.");

			bool IsModPathSet = false;
			bool IsMMFPathSet = false;
			bool IsAdapterVersionSet = false;
			bool IsIndexSet = false;

			string ModPath = string.Empty;
			string MMFPath = string.Empty;
			int PIAdapterVersion = 0;
			int SyncerIndex = -1;

			for(int i = 0; i < args.Length; i++)
			{
				//すべて必須要素
				switch (args[i].ToLower())
				{
					//DLLファイルへのパス
					case "-m":
					case "-mo":
					case "-mod":
					case "-modu":
					case "-modul":
					case "-module":
						if ((i + 1) < args.Length)
						{
							ModPath = args[++i];
							IsModPathSet = true;
						}
						break;

					//MemoryMappedFileへのパス
					case "-f":
					case "-mmf":
					case "-memorymappedfile":
						if ((i + 1) < args.Length)
						{
							MMFPath = args[++i];
							IsMMFPathSet = true;
						}
						break;

					//PIAdapterバージョン
					case "-adpv":
					case "-adapterversion":
						if ((i + 1) < args.Length && int.TryParse(args[i + 1], out PIAdapterVersion))
						{
							i++;
							IsAdapterVersionSet = true;
						}
						break;

					//Syncerでのindex
					case "-i":
					case "-in":
					case "-ind":
					case "-inde":
					case "-index":
						if ((i + 1) < args.Length && int.TryParse(args[i + 1], out SyncerIndex) && SyncerIndex >= 0)
						{
							i++;
							IsIndexSet = true;
						}
						break;
				}
			}

			#region 入力の正当性検証
			string err = string.Empty;

			if (!IsModPathSet)
				err += "DLLファイルへのパスが指定されていません.  \"-module\"オプション識別子で指定してください.\n";
			if (!IsMMFPathSet)
				err += "MemoryMappedPathへのパスが指定されていません.  \"-memorymappedfile\"オプション識別子で指定してください.\n";
			if (!IsAdapterVersionSet)
				err += "AdapterVersionが指定されていません.  \"-adapterversion\"オプション識別子で指定してください.\n";
			if (!IsIndexSet)
				err += "SyncerIndexが指定されていません.  \"-index\"オプション識別子で指定してください.";

			if (!string.IsNullOrWhiteSpace(ModPath) && !File.Exists(ModPath))
				err += ("指定されたDLLファイルが見つかりません.  指定されたファイルへのパス : " + ModPath + "\n");
			if (!string.IsNullOrWhiteSpace(MMFPath) && !File.Exists(MMFPath))
				err += ("指定されたMemoryMappedFileが見つかりません.  指定されたファイルへのパス : " + MMFPath + "\n");
			#endregion //入力の正当性検証

			if (err!=string.Empty)
			{
				Console.WriteLine(err);

				return;//エラー発生時は終了させる.
			}

			PISyncer pis = new PISyncer(MMFPath);
			SameTargetATSPI pi = new SameTargetATSPI(ModPath);

			void CheckExecFunc(in SyncerFlags f, in Action act)//引数のない処理を確認/実行
			{
				if (pis.IsSyncerFlagRaised(SyncerIndex, f)){
					act.Invoke();
					pis.SetSyncerFlagToLower(SyncerIndex, f);
				}
			}
			void CheckExecFuncI(in SyncerFlags f, Action<int> act) => CheckExecFuncA(f, (a) => act.Invoke(a.intValue));//int型引数の処理を確認/実行
			void CheckExecFuncA(in SyncerFlags f, in Action<ArgData> act)//引数をもつメソッドを確認/実行
			{
				if (pis.IsSyncerFlagRaised(SyncerIndex, f)){
					act.Invoke(pis.TouchArgData());
					pis.SetSyncerFlagToLower(SyncerIndex, f);
				}
			}

			while (true)
			{
				if (pis.IsSyncerFlagRaised(SyncerIndex, SyncerFlags.Dispose))
				{
					pi.Dispose();
					pis.SetSyncerFlagToLower(SyncerIndex, SyncerFlags.Dispose);
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

				System.Threading.Tasks.Task.Delay(1);
			}
		}

	}
}
