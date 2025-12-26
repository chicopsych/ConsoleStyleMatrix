using System;
using ConsoleStyleMatrix.Classes;

namespace ConsoleStyleMatrix
{
	/// <summary>
	/// Ponto de entrada da aplicação Console Style Matrix
	/// </summary>
	internal class Program
	{
		static void Main(string[] args)
		{
			MatrixRain matrixRain = null;

			try
			{
				// Cria e executa a animação Matrix com banner inicial
				matrixRain = new MatrixRain();
				matrixRain.RunWithBanner();
			}
			catch (Exception ex)
			{
				Console.Clear();
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Erro ao executar a animação: " + ex.Message);
				Console.WriteLine("\nDetalhes: " + ex.StackTrace);
				Console.ResetColor();
				Console.WriteLine("\nPressione qualquer tecla para sair...");
				Console.ReadKey();
			}
			finally
			{
				// Garante limpeza de recursos
				if (matrixRain != null)
				{
					matrixRain.Dispose();
				}
			}
		}
	}
}
