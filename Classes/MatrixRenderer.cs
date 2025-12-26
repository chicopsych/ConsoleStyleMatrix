using System;

namespace ConsoleStyleMatrix.Classes
{
	/// <summary>
	/// Responsável por renderizar o buffer da animação Matrix na tela
	/// </summary>
	internal class MatrixRenderer
	{
		private readonly IntPtr _consoleHandle;
		private readonly ConsoleManager.CharInfo[] _screenBuffer;
		private readonly short _width;
		private readonly short _height;
		private readonly Random _random;

		// Caracteres usados na animação (ASCII e símbolos)
		private static readonly char[] _chars = "$%#@!*abcdefghijklmnopqrstuvwxyz1234567890?;:ABCDEFGHIJKLMNOPQRSTUVWXYZ^&".ToCharArray();

		public MatrixRenderer(IntPtr consoleHandle, short width, short height)
		{
			_consoleHandle = consoleHandle;
			_width = width;
			_height = height;
			_screenBuffer = new ConsoleManager.CharInfo[width * height];
			_random = new Random();

			InitializeBuffer();
		}

		/// <summary>
		/// Inicializa o buffer com espaços pretos
		/// </summary>
		private void InitializeBuffer()
		{
			for (int i = 0; i < _screenBuffer.Length; i++)
			{
				_screenBuffer[i].Char.UnicodeChar = ' ';
				_screenBuffer[i].Attributes = 0;
			}
		}

		/// <summary>
		/// Desenha um caractere na posição especificada
		/// </summary>
		public void DrawChar(int x, int y, char character, short color)
		{
			if (x < 0 || x >= _width || y < 0 || y >= _height)
				return;

			int index = y * _width + x;
			if (index >= 0 && index < _screenBuffer.Length)
			{
				_screenBuffer[index].Char.UnicodeChar = character;
				_screenBuffer[index].Attributes = color;
			}
		}

		/// <summary>
		/// Limpa (apaga) um caractere na posição especificada
		/// </summary>
		public void ClearChar(int x, int y)
		{
			if (x < 0 || x >= _width || y < 0 || y >= _height)
				return;

			int index = y * _width + x;
			if (index >= 0 && index < _screenBuffer.Length)
			{
				_screenBuffer[index].Char.UnicodeChar = ' ';
				_screenBuffer[index].Attributes = 0;
			}
		}

		/// <summary>
		/// Atualiza um caractere existente (mantém a cor)
		/// </summary>
		public void UpdateChar(int x, int y, char character)
		{
			if (x < 0 || x >= _width || y < 0 || y >= _height)
				return;

			int index = y * _width + x;
			if (index >= 0 && index < _screenBuffer.Length)
			{
				_screenBuffer[index].Char.UnicodeChar = character;
			}
		}

		/// <summary>
		/// Retorna um caractere aleatório para a animação
		/// </summary>
		public char GetRandomChar()
		{
			return _chars[_random.Next(_chars.Length)];
		}

		/// <summary>
		/// Renderiza o buffer inteiro na tela usando WriteConsoleOutput
		/// </summary>
		public void Render()
		{
			ConsoleManager.SmallRect writeRegion = new ConsoleManager.SmallRect
			{
				Left = 0,
				Top = 0,
				Right = (short)(_width - 1),
				Bottom = (short)(_height - 1)
			};

			ConsoleManager.WriteConsoleOutput(
				_consoleHandle,
				_screenBuffer,
				new ConsoleManager.Coord(_width, _height),
				new ConsoleManager.Coord(0, 0),
				ref writeRegion);
		}
	}
}
