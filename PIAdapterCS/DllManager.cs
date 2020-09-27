﻿using System;
using System.IO;
using System.Runtime.InteropServices;

namespace PIAdapterCS
{
	/// <summary>指定のdllを読み込み, 扱えるようにする.</summary>
	public class DllManager : IDisposable
	{
		//ref : https://anis774.net/codevault/loadlibrary.html

		/// <summary>モジュールを読み込む</summary>
		/// <param name="lpLibFileName">モジュールへのパス</param>
		/// <returns>モジュールハンドル</returns>
		[DllImport("kernel32")]
		static extern IntPtr LoadLibrary(string lpLibFileName);

		/// <summary>関数へのポインタを取得する.</summary>
		/// <param name="hModule">モジュールハンドル</param>
		/// <param name="lpProcName">関数名</param>
		/// <returns>関数ポインタ</returns>
		[DllImport("kernel32")]
		static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

		/// <summary>LoadLibraryで取得したモジュールリソースを解放する</summary>
		/// <param name="hLibModule">モジュールハンドル</param>
		/// <returns>解放に成功したかどうか</returns>
		[DllImport("kernel32")]
		static extern bool FreeLibrary(IntPtr hLibModule);

		/// <summary>読み込んだモジュールに割り当てられたハンドル</summary>
		public IntPtr ModuleHandle{ get; }

		/// <summary>delegateを, 初期化時に指定したdllから設定する</summary>
		/// <typeparam name="T">delegate</typeparam>
		/// <param name="method">関数の表記名</param>
		/// <returns>取得したdelegate</returns>
		public T GetProcDelegate<T>(string method) where T : class
			=> Marshal.GetDelegateForFunctionPointer(GetProcAddress(ModuleHandle, method), typeof(T)) as T;
		
		/// <summary>確保しているリソースを解放する</summary>
		public void Dispose() => FreeLibrary(ModuleHandle);

		/// <summary>指定のモジュールを読み込むことで, インスタンスを初期化する.</summary>
		/// <param name="path">モジュールへのパス</param>
		public DllManager(string path)
		{
			if (!File.Exists(path))
				throw new DllNotFoundException();//存在しないファイルへの参照は許容しない.

			ModuleHandle = LoadLibrary(path);
		}
		
	}
}
