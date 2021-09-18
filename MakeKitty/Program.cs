using System ;
using System . Collections . Generic ;
using System . IO ;
using System . Linq ;
using System . Text ;

using DreamRecorder . ToolBox . CommandLine ;
using DreamRecorder . ToolBox . General ;

using Microsoft . Extensions . CommandLineUtils ;
using Microsoft . Extensions . Logging ;

using WenceyWang . FIGlet ;

namespace DreamRecorder . MakeKitty
{

	public class Program
		: ProgramBase <Program , Program . ProgramExitCode , Program . ProgramSetting , Program . ProgramSettingCatalog>
	{

		public enum ProgramSettingCatalog
		{

		}

		public override bool WaitForExit => false ;

		public override string License => "https://www.gnu.org/licenses/agpl-3.0.en.html" ;

		public override bool CheckLicense => false ;

		public override bool WriteLicenseFile => false ;

		public override bool CanExit => true ;

		public override bool HandleInput => false ;

		public override bool LoadSetting => false ;

		public override bool AutoSaveSetting => false ;

		public override bool LoadPlugin => false ;

		public override bool ThrowOnUnexpectedArg => true ;

		public CommandOption NameOption { get ; private set ; }

		public static void Main ( string [ ] args ) { new Program ( ) . RunMain ( args ) ; }


		public override void RegisterArgument ( CommandLineApplication application )
		{
			NameOption = application . Option ( "-name" , "Name in CatalogHeader" , CommandOptionType . SingleValue ) ;
		}

		public override void Start ( string [ ] args )
		{
			string resultName = NameOption . Value ( ) ;

			if ( string . IsNullOrWhiteSpace ( resultName ) )
			{
				DirectoryInfo directory = new DirectoryInfo ( Environment . CurrentDirectory ) ;

				resultName = directory . Name ;
			}

			List <string> fileList = Directory .
									EnumerateFiles (
													Environment . CurrentDirectory ,
													"*" ,
													SearchOption . AllDirectories ) .
									Select (
											fileName
												=> fileName . TrimStartPattern ( Environment . CurrentDirectory ) .
															TrimStart ( '\\' ) ) .
									SkipWhile ( fileName => fileName == ( $"{resultName}.cat" ) ) .
									ToList ( ) ;

			FileStream resultFile = File . OpenWrite ( resultName + ".cdf" ) ;

			StreamWriter fileWriter = new StreamWriter ( resultFile ) ;

			fileWriter . WriteLine (
									$"[CatalogHeader]\r\nName={resultName}.cat\r\nResultDir={Environment . CurrentDirectory}\r\nCatalogVersion=2\r\nHashAlgorithms=SHA256\r\nPageHashes=true" ) ;
			fileWriter . WriteLine ( ) ;
			fileWriter . WriteLine ( "[CatalogFiles]" ) ;

			foreach ( string s in fileList )
			{
				fileWriter . WriteLine ( $"<HASH>{s}={s}" ) ;
			}

			fileWriter . WriteLine ( ) ;
			fileWriter . Flush ( ) ;

			Exit ( ) ;
		}


		public override void ConfigureLogger ( ILoggingBuilder builder ) { builder . AddConsole ( ) ; }

		public override void ShowLogo ( )
		{
			StringBuilder logo = new StringBuilder ( ) ;
			logo . AppendLine (
								"           __..--''``---....___   _..._    __\r\n /// //_.-'    .-/\";  `        ``<._  ``.''_ `. / // /\r\n///_.-' _..--.'_    \\                    `( ) ) // //\r\n/ (_..-' // (< _     ;_..__               ; `' / ///\r\n / // // //  `-._,_)' // / ``--...____..-' /// / //" ) ;

			logo . AppendLine ( new AsciiArt ( "MakeKitty" , width : CharacterWidth . Smush ) . ToString ( ) ) ;
			Console . WriteLine ( logo . ToString ( ) ) ;
		}

		public override void ShowCopyright ( )
		{
			Console . WriteLine ( $"MakeKitty Copyright (C) 2021 - {DateTime . Now . Year} Wencey Wang" ) ;

			Console . WriteLine ( @"This program comes with ABSOLUTELY NO WARRANTY." ) ;

			Console . WriteLine (
								@"This is free software, and you are welcome to redistribute it under certain conditions; read https://www.gnu.org/licenses/agpl-3.0.en.html for details." ) ;
		}

		public override void OnExit ( ProgramExitCode code ) { }

		public class ProgramExitCode : ProgramExitCode <ProgramExitCode>
		{

		}

		public class ProgramSetting : SettingBase <ProgramSetting , ProgramSettingCatalog>
		{

		}

	}

}
