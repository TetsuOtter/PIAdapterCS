﻿using System;
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

						IsModPathSet = true;
						break;

					//MemoryMappedFileへのパス
					case "-f":
					case "-mmf":
					case "-memorymappedfile":

						IsMMFPathSet = true;
						break;

					//PIAdapterバージョン
					case "-adpv":
					case "-adapterversion":

						IsAdapterVersionSet = true;
						break;

					//Syncerでのindex
					case "-i":
					case "-in":
					case "-ind":
					case "-inde":
					case "-index":

						IsIndexSet = true;
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
		}
	}
}
