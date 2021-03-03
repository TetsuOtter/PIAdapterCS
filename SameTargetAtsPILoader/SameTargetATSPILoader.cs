using System;
using System.Reflection;

namespace TR
{
	/// <summary>TargetPlatformが同じATSプラグインを操作します.</summary>
	public static class SameTargetATSPILoader
	{
		static public IAtsPI LoadNativeOrClrPI(in string PIPath) => CreateInstanceFromILDll(PIPath) ?? new SameTargetNativeATSPI(PIPath);
		static public IAtsPI? CreateInstanceFromILDll(in string path)
		{
			try
			{
				//ReflectionOnlyLoadFrom ref : https://docs.microsoft.com/ja-jp/dotnet/framework/reflection-and-codedom/how-to-load-assemblies-into-the-reflection-only-context
				//AppDomain ref : https://devlights.hatenablog.com/entry/20120808/p1
				Assembly asm = Assembly.ReflectionOnlyLoadFrom(path);
				foreach (var m in asm.GetModules())
				{
					try
					{
						foreach (var t in m.GetTypes())
						{
							System.Diagnostics.Debug.WriteLine(t);
							if (typeof(IAtsPI).IsAssignableFrom(t)) //IsAssignableFrom ref : https://smdn.jp/programming/netfx/tips/check_type_isassignablefrom_issubclassof/#section.2.1
							{
								System.Diagnostics.Debug.WriteLine($"IAtsPI : {t} (fullname:{asm.FullName}, typename:{t.FullName})");
								if (asm.FullName is string asm_fn && t.FullName is string t_fn)//NULL Check
									return new SameTargetIAtsPIPlugin(asm_fn, t_fn);
							}
						}
					}
					catch (Exception ex)
					{
						System.Diagnostics.Debug.WriteLine(ex);
					}
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
			}

			return null;
		}

	}
}
