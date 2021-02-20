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
				Assembly asm = Assembly.LoadFrom(path);
				foreach (var m in asm.GetModules())
				{
					try
					{
						foreach (var t in m.GetTypes())
							if (typeof(IAtsPI).IsAssignableFrom(t)) //IsAssignableFrom ref : https://smdn.jp/programming/netfx/tips/check_type_isassignablefrom_issubclassof/#section.2.1
								return asm.CreateInstance(t.Name) as IAtsPI;
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
