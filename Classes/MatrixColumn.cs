using System;

namespace ConsoleStyleMatrix.Classes
{
	/// <summary>
	/// Representa uma coluna individual na animação Matrix (uma "gota" de caracteres caindo)
	/// </summary>
	internal class MatrixColumn
	{
		private static readonly Random _random = new Random();

		/// <summary>
		/// Posição X (horizontal) da coluna
		/// </summary>
		public int X { get; private set; }

		/// <summary>
		/// Posição Y (vertical) atual da cabeça da gota
		/// </summary>
		public int Y { get; set; }

		/// <summary>
		/// Comprimento do rastro da gota
		/// </summary>
		public int Length { get; private set; }

		/// <summary>
		/// Velocidade de queda (frames para pular)
		/// </summary>
		public int Speed { get; private set; }

		private readonly int _maxHeight;

		public MatrixColumn(int x, int maxHeight)
		{
			X = x;
			_maxHeight = maxHeight;
			Reset();
		}

		/// <summary>
		/// Reinicia a coluna com valores aleatórios
		/// </summary>
		public void Reset()
		{
			Y = _random.Next(-_maxHeight, 0);
			Length = _random.Next(5, _maxHeight / 2);
			Speed = _random.Next(1, 3);
		}

		/// <summary>
		/// Move a coluna uma posição para baixo
		/// </summary>
		public void MoveDown()
		{
			Y++;
		}

		/// <summary>
		/// Verifica se a coluna completou sua trajetória e precisa ser reiniciada
		/// </summary>
		public bool ShouldReset()
		{
			return Y - Length > _maxHeight;
		}

		/// <summary>
		/// Obtém a posição Y do final do rastro
		/// </summary>
		public int GetTailY()
		{
			return Y - Length;
		}
	}
}
