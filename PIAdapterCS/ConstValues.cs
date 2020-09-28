namespace PIAdapterCS
{
	static public class ConstValues
	{
		static public uint ATSPI_IF_GetPIVersion { get; } = 0x00020000;

		static public int PanelArrSize { get; } = 256;
		static public int SoundArrSize { get; } = 256;


		static public int PISyncer_Tick { get; } = 10;
		static public int PISyncer_TimeoutCount { get; } = 1000;
		static public int PISyncer_MMFFileBufSize { get; } = (sizeof(int) * 514) + 512;
		static public string PISyncer_Name_Flags { get; } = "FLG";
		static public string PISyncer_Name_PanelD { get; } = "PNL";
		static public string PISyncer_Name_SoundD { get; } = "SND";
		static public string PISyncer_Name_ArgData { get; } = "ARG";
		static public string PISyncer_Name_Handle { get; } = "HND";
	}
}
