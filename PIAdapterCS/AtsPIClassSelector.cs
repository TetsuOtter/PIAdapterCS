using System;
using System.IO;

using TR.ATSPISyncer;

namespace PIAdapterCS
{
	static public class AtsPIClassSelector
	{
		static public readonly string PIAdapter_Loader_X86_FName = "PIAdapterCS.Loader.x86.exe";
		static public readonly string PIAdapter_Loader_X64_FName = "PIAdapterCS.Loader.x64.exe";
		static public IAtsPI? GetAtsPluginInstance(in string path, in int index, in PISyncer syncer)
		{
			if (!File.Exists(path))//ファイルが存在しないなら
				return null;//nullを返す

			//拡張子(ファイル名末尾)がdllなら
			if (path.EndsWith(".dll"))
				return CreateInstanceFromDll(path, index, syncer);

			//任意のアダプタ対応はとりあえず未実装
			return null;
		}

		private static IAtsPI? CreateInstanceFromDll(in string path,in int index,in PISyncer syncer)
		{
			ExecutableFileTargetChecker target = ExecutableFileTargetChecker.Check(path);
			if (!target.LoadSuccessed)
				return null;//読み込みに失敗した

			return target.TargetPlatform switch
			{
				ExecutableFileTargetChecker.TargetPlatformTypes.AnyCPU => TR.SameTargetATSPILoader.CreateInstanceFromILDll(path),//AnyCPUなら, 直で読み込む(CLRでないはずがないため)
				ExecutableFileTargetChecker.TargetPlatformTypes.X86 => CreateInstanceForX86Dll(path, index, syncer),//X86なら, それ用の判定を挟んで読み込む
				ExecutableFileTargetChecker.TargetPlatformTypes.X64 => CreateInstanceForX64Dll(path, index, syncer),//X64なら, それ用の判定を挟んで読み込む
				_ => null
			};
		}


		private static IAtsPI? CreateInstanceForX86Dll(in string path, in int index, in PISyncer syncer)
			=> Environment.Is64BitProcess ? //X86 dllは, 
			new DifferentTargetATSPI(PIAdapter_Loader_X64_FName, GetDifferentT_ArgString(path, index), syncer, index) //x64プロセスではDifferentTarget
			: TR.SameTargetATSPILoader.LoadNativeOrClrPI(path); //x86プロセスではSameTarget

		private static IAtsPI? CreateInstanceForX64Dll(in string path, in int index, in PISyncer syncer)
			=> Environment.Is64BitProcess
			? TR.SameTargetATSPILoader.LoadNativeOrClrPI(path) //x64 dllは, x64プロセスではSameTarget
			: new DifferentTargetATSPI(PIAdapter_Loader_X64_FName, GetDifferentT_ArgString(path, index), syncer, index);//x86プロセスではDifferentTarget

		private static string GetDifferentT_ArgString(in string path, in int index)
			=> $"-module \"{path}\" -mmf \"{ConstValues.SyncerSMemFileName}\" -adpv {ConstValues.AdapterVersion} -index {index}";
	}
}
