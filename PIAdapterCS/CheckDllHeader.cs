using System;
using System.IO;

namespace PIAdapterCS
{
	public class ExecutableFileTargetChecker
	{
		public bool LoadSuccessed { get; } = false;

		/// <summary>Whether the analyzed file is a Dll file</summary>
		public bool IsDllFile { get; } = false;
		
		/// <summary>Plauform Type</summary>
		public TargetPlatformTypes TargetPlatform { get; } = TargetPlatformTypes.Unknown;
		static public ExecutableFileTargetChecker Check(StreamReader sr) => new ExecutableFileTargetChecker(sr);
		static public ExecutableFileTargetChecker Check(string path)
		{
			using StreamReader sr = new StreamReader(path);
			return new ExecutableFileTargetChecker(sr);
		}

		public enum TargetPlatformTypes
		{
			Unknown,
			X86,
			X64,
			AnyCPU,
			Arm,
			Arm64,
		}

		#region Settings
		//ref : http://shopping2.gmobb.jp/htdmnr/www08/mcc/doc/pe.html

		/// <summary>Minimum file size that's required to be parsed as a Dll file</summary>
		static private readonly ushort DOS_HEADER_MAGIC = 0x5A4D;//MZ
		static private readonly uint NT_HEADER_Signature = 0x00004550;//PE\0\0

		static private readonly int POS_DOS_HEADER_MAGIC = 0;
		static private readonly int DOS_HEADER_MAGIC_SIZE = sizeof(ushort);

		static private readonly int POS_DOS_HEADER_LFANEW = 0x3C;
		static private readonly int DOS_HEADER_LFANEW_SIZE = sizeof(uint);

		static private readonly int NT_HEADER_SIGN_SIZE = sizeof(uint);
		static private readonly int FILE_HEADER_MACHINE_SIZE = sizeof(uint);

		//ref : https://docs.microsoft.com/en-us/windows/win32/api/winnt/ns-winnt-image_file_header
		/// <summary>x86 or CLR-AnyCPU</summary>
		static private readonly ushort IMAGE_FILE_MACHINE_I386 = 0x014c;
		/// <summary>AMD64 (x86_64)</summary>
		static private readonly ushort IMAGE_FILE_MACHINE_AMD64 = 0x8664;

		static private readonly ushort IMAGE_FILE_CHARACTERISTICS_EXECUTABLE_IMAGE = 0x0002;
		static private readonly ushort IMAGE_FILE_CHARACTERISTICS_32BIT = 0x0100;
		static private readonly ushort IMAGE_FILE_CHARACTERISTICS_DLL = 0x2000;
		#endregion Settings


		StreamReader _file;
		private ExecutableFileTargetChecker(StreamReader sr)
		{
			_file = sr;
			if (!sr.BaseStream.CanRead || !sr.BaseStream.CanSeek)
				return;

			if (BitConverter.ToUInt16(ReadBytes(POS_DOS_HEADER_MAGIC, DOS_HEADER_MAGIC_SIZE), 0) != DOS_HEADER_MAGIC)
				return;//マジックナンバー不一致は拒否

			uint POS_NT_HEADER = BitConverter.ToUInt32(ReadBytes(POS_DOS_HEADER_LFANEW, DOS_HEADER_LFANEW_SIZE), 0);
			if (POS_NT_HEADER <= POS_DOS_HEADER_LFANEW)
				return;//MS-DOSヘッダとNTヘッダが被ることはあり得ないため, 被るなら除外

			if (BitConverter.ToUInt32(ReadBytes(POS_NT_HEADER, NT_HEADER_SIGN_SIZE), 0) != NT_HEADER_Signature)
				return;//NTヘッダのSignが違えば除外

			ushort Machine = BitConverter.ToUInt16(ReadBytes(FILE_HEADER_MACHINE_SIZE), 0);
			ushort Characteristics = BitConverter.ToUInt16(ReadBytes(16, sizeof(ushort), SeekOrigin.Current), 0);

			if ((Characteristics & IMAGE_FILE_CHARACTERISTICS_EXECUTABLE_IMAGE) == 0)
				return;//必ずセットされていなければならないフラグが立ってないなら, 除外

			IsDllFile = (Characteristics & IMAGE_FILE_CHARACTERISTICS_DLL) != 0;//0じゃないならDLLフラグが立っている

			TargetPlatform = CheckTargetType(Machine, Characteristics);

			LoadSuccessed = true;
		}

		byte[] ReadBytes(in long offset, in int len, in SeekOrigin origin = SeekOrigin.Begin)
		{
			_file.BaseStream.Seek(offset, origin);
			return ReadBytes(len);
		}
		byte[] ReadBytes(in int len)
		{
			byte[] ba = new byte[len];
			_file.BaseStream.Read(ba, 0, ba.Length);
			return ba;
		}

		static TargetPlatformTypes CheckTargetType(in ushort Machine, in ushort Characteristics)
		{
			if (Machine == IMAGE_FILE_MACHINE_AMD64)
				return TargetPlatformTypes.X64;

			if (Machine == IMAGE_FILE_MACHINE_I386)
				return ((Characteristics & IMAGE_FILE_CHARACTERISTICS_32BIT) != 0) ? TargetPlatformTypes.X86 : TargetPlatformTypes.AnyCPU;

			return TargetPlatformTypes.Unknown;
		}
	}
}
