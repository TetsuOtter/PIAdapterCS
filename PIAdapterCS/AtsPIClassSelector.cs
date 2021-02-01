using System;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Reflection;

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
			Machine machine = Machine.Unknown;
			bool IsIL = false;

			using (var reader = new StreamReader(path))
			{
				if (new PEHeaders(reader.BaseStream) is PEHeaders h)//ここの処理は直ぐだろうからこのまま
				{
					machine = h.CoffHeader.Machine;

					if (h.CorHeader?.Flags is CorFlags f)
						IsIL = (f & CorFlags.ILOnly) == CorFlags.ILOnly; //Corヘッダが存在し, かつILOnlyの場合のみILである.
				}
			}//ファイルは早めに閉じておく

			if (IsIL)
				return CreateInstanceFromILDll(path);
			else
				return machine switch
				{
					Machine.Amd64 => CreateInstanceForX64Dll(path, index, syncer),//x64のDLLであった場合
					Machine.I386 => CreateInstanceForX86Dll(path, index, syncer),//x86のDLLであった場合
					_ => null//上記以外は未対応
				};
		}

		private static IAtsPI? CreateInstanceFromILDll(in string path)
		{
			try
			{
				Assembly asm = Assembly.LoadFrom(path);
				foreach(var m in asm.GetModules())
				{
					try
					{
						foreach (var t in m.GetTypes())
							if (typeof(IAtsPI).IsAssignableFrom(t)) //IsAssignableFrom ref : https://smdn.jp/programming/netfx/tips/check_type_isassignablefrom_issubclassof/#section.2.1
								return asm.CreateInstance(t.Name) as IAtsPI;
					}catch(Exception ex)
					{
						System.Diagnostics.Debug.WriteLine(ex);
					}
				}	
			}catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
			}

			return null;
		}

		private static IAtsPI? CreateInstanceForX86Dll(in string path, in int index, in PISyncer syncer)
			=> Environment.Is64BitProcess ? //X86 dllは, 
			new DifferentTargetATSPI(PIAdapter_Loader_X64_FName, GetDifferentT_ArgString(path, index), syncer, index) //x64プロセスではDifferentTarget
			: new SameTargetATSPI(path); //x86プロセスではSameTarget

		private static IAtsPI? CreateInstanceForX64Dll(in string path, in int index, in PISyncer syncer)
			=> Environment.Is64BitProcess
			? new SameTargetATSPI(path) //x64 dllは, x64プロセスではSameTarget
			: new DifferentTargetATSPI(PIAdapter_Loader_X64_FName, GetDifferentT_ArgString(path, index), syncer, index);//x86プロセスではDifferentTarget

		private static string GetDifferentT_ArgString(in string path, in int index)
			=> $"-module \"{path}\" -mmf \"{ConstValues.SyncerSMemFileName}\" -adpv {ConstValues.AdapterVersion} -index {index}";
	}
}
