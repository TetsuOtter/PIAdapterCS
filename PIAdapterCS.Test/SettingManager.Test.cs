using System.IO;
using System.Text;

using NUnit.Framework;

namespace PIAdapterCS.Test
{
	public class SettingManagerTests
	{
		const string XMLSetting_SampleFilePath = @"C:\abc\dec.dll";
		static readonly string XMLSetting_ExceptedOutput = @$"<?xml version=""1.0""?>
<Settings xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <PluginListFilePath>{XMLSetting_SampleFilePath}</PluginListFilePath>
</Settings>";
		[Test]
		public void ConvertToXMLTest()
		{
			Settings settings = new Settings() { PluginListFilePath = XMLSetting_SampleFilePath };
			using MemoryStream stream = new MemoryStream();
			SettingManager.WriteSettingXML(stream, settings);
			string str = Encoding.UTF8.GetString(stream.ToArray());
			Assert.AreEqual(XMLSetting_ExceptedOutput, str);
		}

		[Test]
		public void ConvertFromXMLTest()
		{
			byte[] ba = Encoding.UTF8.GetBytes(XMLSetting_ExceptedOutput);
			using MemoryStream stream = new MemoryStream();
			stream.Write(ba, 0, ba.Length);
			stream.Position = 0;
			Settings? loadedSetting = SettingManager.LoadFromStream(stream);
			
			Assert.IsNotNull(loadedSetting);//If NULL => fail

			Assert.AreEqual(XMLSetting_SampleFilePath, loadedSetting?.PluginListFilePath);
		}

		const string SamplePath_UpperCase = "ABC/def/*/X86X64*/sample.dll";
		static readonly string SamplePath_UpperCase_Excepted = $"ABC/def/{(System.Environment.Is64BitProcess ? "X64" : "X86")}/sample.dll";
		[Test]
		public void FilePathFormatterTest_UpperCase()
			=> Assert.AreEqual(SamplePath_UpperCase_Excepted, SettingManager.FilePathFormatter(SamplePath_UpperCase));
		
		const string SamplePath_LowerCase = "ABC/def/*/x86x64*/sample.dll";
		static readonly string SamplePath_LowerCase_Excepted = $"ABC/def/{(System.Environment.Is64BitProcess ? "x64" : "x86")}/sample.dll";
		[Test]
		public void FilePathFormatterTest_LowerCase()
			=> Assert.AreEqual(SamplePath_LowerCase_Excepted, SettingManager.FilePathFormatter(SamplePath_LowerCase));
		
		const string SamplePath_UpperAndLowerCase = "ABC/def/*/x86x64*/sample.*/X86X64*.dll";
		static readonly string SamplePath_UpperAndLowerCase_Excepted = $"ABC/def/{(System.Environment.Is64BitProcess ? "x64" : "x86")}/sample.{(System.Environment.Is64BitProcess ? "X64" : "X86")}.dll";
		[Test]
		public void FilePathFormatterTest_UpperAndLowerCase()
			=> Assert.AreEqual(SamplePath_UpperAndLowerCase_Excepted, SettingManager.FilePathFormatter(SamplePath_UpperAndLowerCase));
		
	}
}