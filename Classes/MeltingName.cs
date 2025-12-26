using System;
using System.Threading;

namespace ConsoleStyleMatrix.Classes
{
	internal class MeltingName
	{
		private static readonly string[] bannerLines = new string[]
		{
			"╔═══════════════════════════════════════════════════════════════════╗",
			"║  ▄████▄  ▓██   ██▓ ▄▄▄▄   ▓█████  ██▀███    ██████  ██░ ██ ▓█████ ██▓     ██▓    ║",
			"║ ▒██▀ ▀█   ▒██  ██▒▓█████▄ ▓█   ▀ ▓██ ▒ ██▒▒██    ▒ ▓██░ ██▒▓█   ▀▓██▒    ▓██▒    ║",
			"║ ▒▓█    ▄   ▒██ ██░▒██▒ ▄██▒███   ▓██ ░▄█ ▒░ ▓██▄   ▒██▀▀██░▒███  ▒██░    ▒██░    ║",
			"║ ▒▓▓▄ ▄██▒  ░ ▐██▓░▒██░█▀  ▒▓█  ▄ ▒██▀▀█▄    ▒   ██▒░▓█ ░██ ▒▓█  ▄▒██░    ▒██░    ║",
			"║ ▒ ▓███▀ ░  ░ ██▒▓░░▓█  ▀█▓░▒████▒░██▓ ▒██▒▒██████▒▒░▓█▒░██▓░▒████▒░██████▒░██████▒║",
			"║ ░ ░▒ ▒  ░   ██▒▒▒ ░▒▓███▀▒░░ ▒░ ░░ ▒▓ ░▒▓░▒ ▒▓▒ ▒ ░ ▒ ░░▒░▒░░ ▒░ ░░ ▒░▓  ░░ ▒░▓  ░║",
			"║   ░  ▒    ▓██ ░▒░ ▒░▒   ░  ░ ░  ░  ░▒ ░ ▒░░ ░▒  ░ ░ ▒ ░▒░ ░ ░ ░  ░░ ░ ▒  ░░ ░ ▒  ░║",
			"╠═══════════════════════════════════════════════════════════════════╣",
			"║   ◢◤ CHICOPSYCH NEURAL TERMINAL v5.0 ◥◣   [ 2 0 7 7 ]             ║",
			"╚═══════════════════════════════════════════════════════════════════╝"
		};

		/// <summary>
		/// Exibe o banner CYBERSHELL no console
		/// </summary>
		public static void DisplayBanner()
		{
			DisplayBanner(ConsoleColor.Green);
		}

		/// <summary>
		/// Exibe o banner CYBERSHELL no console com a cor especificada
		/// </summary>
		/// <param name="color">Cor do texto do banner</param>
		public static void DisplayBanner(ConsoleColor color)
		{
			ConsoleColor originalColor = Console.ForegroundColor;
			Console.ForegroundColor = color;

			foreach (string line in bannerLines)
			{
				Console.WriteLine(line);
			}

			Console.ForegroundColor = originalColor;
		}

		/// <summary>
		/// Exibe o banner com efeito de "derretimento" linha por linha
		/// </summary>
		/// <param name="color">Cor do banner</param>
		/// <param name="delayMs">Delay entre cada linha em milissegundos</param>
		public static void DisplayBannerWithMelting(ConsoleColor color = ConsoleColor.Green, int delayMs = 100)
		{
			ConsoleColor originalColor = Console.ForegroundColor;
			Console.ForegroundColor = color;

			foreach (string line in bannerLines)
			{
				Console.WriteLine(line);
				Thread.Sleep(delayMs);
			}

			Console.ForegroundColor = originalColor;
		}

		/// <summary>
		/// Exibe o banner centralizado no console
		/// </summary>
		/// <param name="color">Cor do banner</param>
		public static void DisplayBannerCentered(ConsoleColor color = ConsoleColor.Green)
		{
			ConsoleColor originalColor = Console.ForegroundColor;
			Console.ForegroundColor = color;

			int windowWidth = Console.WindowWidth;
			int bannerWidth = bannerLines[0].Length;
			int leftPadding = Math.Max(0, (windowWidth - bannerWidth) / 2);

			foreach (string line in bannerLines)
			{
				Console.WriteLine(new string(' ', leftPadding) + line);
			}

			Console.ForegroundColor = originalColor;
		}

		/// <summary>
		/// Retorna o array com as linhas do banner
		/// </summary>
		public static string[] GetBannerLines()
		{
			return (string[])bannerLines.Clone();
		}
	}
}
