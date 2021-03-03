using System;
using System.IO;
using System.Xml.Serialization;

namespace PIAdapterCS
{
	public class Settings
	{
		public string PluginListFilePath = string.Empty;

		[XmlIgnore]
		public string PluginListFilePath_Formated => SettingManager.FilePathFormatter(PluginListFilePath);
	}

	public static class SettingManager
	{
		const string ReplaceTo_X86_or_X64 = "*/X86X64*";
		const string ReplaceTo_x86_or_x64 = "*/x86x64*";
		static readonly XmlSerializer SettingsXMLSerializer = new XmlSerializer(typeof(Settings));

		public static Settings? LoadFromXMLFile(in string Path)
		{
			if (!File.Exists(Path))
				throw new FileNotFoundException();

			using StreamReader sr = new StreamReader(Path);
			return LoadFromStream(sr.BaseStream);
		}
		public static Settings? LoadFromStream(in Stream sr)
			=> SettingsXMLSerializer.Deserialize(sr) as Settings;

		public static void WriteSettingXML(in Stream stream, in Settings settings)
		{
			if (!stream.CanWrite)
				throw new AccessViolationException("Cannot Write to stream");

			SettingsXMLSerializer.Serialize(stream, settings);
		}

		static public string FilePathFormatter(in string src)
			=> src//Source Text
			.Replace(ReplaceTo_x86_or_x64, Environment.Is64BitProcess ? "x64" : "x86")//Small 'x'
			.Replace(ReplaceTo_X86_or_X64, Environment.Is64BitProcess ? "X64" : "X86");//Big 'X'
		
	}
}
