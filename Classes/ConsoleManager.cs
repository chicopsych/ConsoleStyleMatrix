using System;
using System.Runtime.InteropServices;

namespace ConsoleStyleMatrix.Classes
{
	/// <summary>
	/// Gerencia operações de baixo nível do console através de P/Invoke
	/// </summary>
	internal class ConsoleManager
	{
		// Estruturas para P/Invoke
		[StructLayout(LayoutKind.Sequential)]
		public struct Coord
		{
			public short X;
			public short Y;
			public Coord(short x, short y) { X = x; Y = y; }
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct CharUnion
		{
			[FieldOffset(0)] public char UnicodeChar;
			[FieldOffset(0)] public byte AsciiChar;
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct CharInfo
		{
			[FieldOffset(0)] public CharUnion Char;
			[FieldOffset(2)] public short Attributes;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SmallRect
		{
			public short Left;
			public short Top;
			public short Right;
			public short Bottom;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ConsoleScreenBufferInfo
		{
			public Coord dwSize;
			public Coord dwCursorPosition;
			public short wAttributes;
			public SmallRect srWindow;
			public Coord dwMaximumWindowSize;
		}

		// Constantes de cor para o console
		public const short FOREGROUND_GREEN = 0x0002;
		public const short FOREGROUND_INTENSITY = 0x0008;
		public const short FOREGROUND_WHITE = 0x000F;
		public const short FOREGROUND_DARK_GREEN = FOREGROUND_GREEN;

		// P/Invoke declarations
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetStdHandle(int nStdHandle);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool WriteConsoleOutput(
			IntPtr hConsoleOutput,
			CharInfo[] lpBuffer,
			Coord dwBufferSize,
			Coord dwBufferCoord,
			ref SmallRect lpWriteRegion);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool GetConsoleScreenBufferInfo(
			IntPtr hConsoleOutput,
			out ConsoleScreenBufferInfo lpConsoleScreenBufferInfo);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool SetConsoleScreenBufferSize(
			IntPtr hConsoleOutput,
			Coord dwSize);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool SetConsoleWindowInfo(
			IntPtr hConsoleOutput,
			bool bAbsolute,
			ref SmallRect lpConsoleWindow);

		public const int STD_OUTPUT_HANDLE = -11;

		private IntPtr _consoleHandle;

		public IntPtr ConsoleHandle
		{
			get { return _consoleHandle; }
		}

		public ConsoleManager()
		{
			_consoleHandle = GetStdHandle(STD_OUTPUT_HANDLE);
		}

		/// <summary>
		/// Maximiza a janela do console para o tamanho máximo possível
		/// </summary>
		public void MaximizeConsoleWindow()
		{
			// Obtém informações do buffer atual
			ConsoleScreenBufferInfo csbi;
			GetConsoleScreenBufferInfo(_consoleHandle, out csbi);

			// Calcula o tamanho máximo da janela
			short maxWidth = csbi.dwMaximumWindowSize.X;
			short maxHeight = csbi.dwMaximumWindowSize.Y;

			// Primeiro, reduz a janela para o mínimo para poder redimensionar o buffer
			SmallRect minWindow = new SmallRect { Left = 0, Top = 0, Right = 1, Bottom = 1 };
			SetConsoleWindowInfo(_consoleHandle, true, ref minWindow);

			// Define o tamanho do buffer igual ao tamanho máximo da janela
			SetConsoleScreenBufferSize(_consoleHandle, new Coord(maxWidth, maxHeight));

			// Agora expande a janela para o tamanho máximo
			SmallRect maxWindow = new SmallRect
			{
				Left = 0,
				Top = 0,
				Right = (short)(maxWidth - 1),
				Bottom = (short)(maxHeight - 1)
			};
			SetConsoleWindowInfo(_consoleHandle, true, ref maxWindow);
		}

		/// <summary>
		/// Obtém o tamanho atual do buffer do console
		/// </summary>
		public void GetBufferSize(out short width, out short height)
		{
			ConsoleScreenBufferInfo csbi;
			GetConsoleScreenBufferInfo(_consoleHandle, out csbi);
			width = csbi.dwSize.X;
			height = csbi.dwSize.Y;
		}

		/// <summary>
		/// Configura o console para o modo Matrix
		/// </summary>
		public void ConfigureForMatrix()
		{
			Console.Title = "The Matrix - CYBERSHELL Terminal";
			Console.CursorVisible = false;
			MaximizeConsoleWindow();
		}

		/// <summary>
		/// Restaura as configurações padrão do console
		/// </summary>
		public void RestoreDefaults()
		{
			Console.ResetColor();
			Console.CursorVisible = true;
			Console.Clear();
		}
	}
}
