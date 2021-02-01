using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIAdapterCS
{
	static public class AtsPIClassSelector
	{
		static public IAtsPI? GetAtsPluginInstance(in string path, in int index)
		{
			if (!File.Exists(path))//ファイルが存在しないなら
				return null;//nullを返す

			//拡張子(ファイル名末尾)がdllなら
			if (path.EndsWith(".dll"))
			{

			}

			//任意のアダプタ対応はとりあえず未実装
			return null;
		}

		enum DllType
		{
			Unknown,
			x86,
			x64,
			TR_IAtsPI,
			OpenBve
		}
	}
}
