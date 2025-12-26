using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleStyleMatrix.Classes
{
	/// <summary>
	/// Gerencia a animação de derretimento do banner CYBERSHELL no estilo Matrix
	/// </summary>
	internal class BannerMeltingAnimation
	{
		private readonly MatrixRenderer _renderer;
		private readonly short _width;
		private readonly short _height;
		private readonly Random _random;
		private readonly List<BannerParticle> _particles;
		private int _frameCount;

		// Caracteres Matrix para efeito de transformação
		private static readonly char[] _matrixChars = "$%#@!*abcdefghijklmnopqrstuvwxyz1234567890?;:ABCDEFGHIJKLMNOPQRSTUVWXYZ^&".ToCharArray();

		public BannerMeltingAnimation(MatrixRenderer renderer, short width, short height)
		{
			_renderer = renderer;
			_width = width;
			_height = height;
			_random = new Random();
			_particles = new List<BannerParticle>();
			_frameCount = 0;
		}

		/// <summary>
		/// Executa a animação completa de derretimento
		/// </summary>
		public void Animate()
		{
			// 1. Converter banner em partículas
			ConvertBannerToParticles();

			// 2. Loop principal de animação
			bool allDone = false;
			while (!allDone)
			{
				_frameCount++;
				allDone = true;

				// Atualizar cada partícula
				foreach (var particle in _particles)
				{
					UpdateParticle(particle);
					if (particle.State != ParticleState.Done)
					{
						allDone = false;
					}
				}

				// Renderizar frame
				_renderer.Render();

				// Controle de velocidade
				Thread.Sleep(30);

				// Permitir saída com ESC
				if (Console.KeyAvailable)
				{
					var key = Console.ReadKey(true).Key;
					if (key == ConsoleKey.Escape)
					{
						break;
					}
				}
			}

			// Pequena pausa antes de iniciar Matrix Rain
			Thread.Sleep(500);
		}

		/// <summary>
		/// Converte o banner em partículas individuais
		/// </summary>
		private void ConvertBannerToParticles()
		{
			var bannerLines = MeltingName.GetBannerLines();
			int bannerWidth = bannerLines[0].Length;
			int startX = Math.Max(0, (_width - bannerWidth) / 2);
			int startY = 2; // Posição vertical inicial

			for (int y = 0; y < bannerLines.Length; y++)
			{
				string line = bannerLines[y];
				for (int x = 0; x < line.Length; x++)
				{
					char c = line[x];
					
					// Ignora espaços em branco
					if (c != ' ')
					{
						_particles.Add(new BannerParticle
						{
							OriginalX = startX + x,
							OriginalY = startY + y,
							CurrentY = startY + y,
							Character = c,
							FallSpeed = _random.Next(1, 4),
							DelayFrames = y * 3 + _random.Next(0, 5), // Cascata com variação
							State = ParticleState.Waiting,
							HorizontalOffset = 0
						});
					}
				}
			}
		}

		/// <summary>
		/// Atualiza o estado de uma partícula individual
		/// </summary>
		private void UpdateParticle(BannerParticle particle)
		{
			switch (particle.State)
			{
				case ParticleState.Waiting:
					// Renderiza na posição original
					_renderer.DrawChar(
						particle.OriginalX,
						particle.OriginalY,
						particle.Character,
						ConsoleManager.FOREGROUND_GREEN
					);

					// Decrementa delay
					if (particle.DelayFrames > 0)
					{
						particle.DelayFrames--;
					}
					else
					{
						particle.State = ParticleState.Falling;
					}
					break;

				case ParticleState.Falling:
					// Só atualiza baseado na velocidade
					if (_frameCount % particle.FallSpeed == 0)
					{
						// Apaga posição anterior
						_renderer.ClearChar(
							particle.OriginalX + particle.HorizontalOffset,
							particle.CurrentY
						);

						// Move para baixo
						particle.CurrentY++;

						// Efeito de ondulação horizontal (30% chance)
						if (_random.Next(100) < 30)
						{
							int newOffset = _random.Next(-1, 2);
							if (Math.Abs(newOffset) <= 2) // Limita oscilação
							{
								particle.HorizontalOffset += newOffset;
							}
						}

						// Verifica se saiu da tela
						if (particle.CurrentY >= _height)
						{
							particle.State = ParticleState.Done;
							return;
						}

						// Efeito de glitch - muda caractere (40% chance)
						if (_random.Next(100) < 40)
						{
							particle.Character = GetRandomMatrixChar();
						}

						// Calcula cor baseada na distância caída
						short color = CalculateColorGradient(particle);

						// Desenha na nova posição
						_renderer.DrawChar(
							particle.OriginalX + particle.HorizontalOffset,
							particle.CurrentY,
							particle.Character,
							color
						);

						// Transição para Morphing quando está próximo do fim
						int distanceFallen = particle.CurrentY - particle.OriginalY;
						if (distanceFallen > 20)
						{
							particle.State = ParticleState.Morphing;
						}
					}
					break;

				case ParticleState.Morphing:
					// Fase final - transformação acelerada em caracteres Matrix
					if (_frameCount % 2 == 0)
					{
						// Apaga posição anterior
						_renderer.ClearChar(
							particle.OriginalX + particle.HorizontalOffset,
							particle.CurrentY
						);

						particle.CurrentY++;

						// Verifica se saiu da tela
						if (particle.CurrentY >= _height)
						{
							particle.State = ParticleState.Done;
							return;
						}

						// Muda caractere toda vez
						particle.Character = GetRandomMatrixChar();

						// Desenha com cor branca brilhante
						_renderer.DrawChar(
							particle.OriginalX + particle.HorizontalOffset,
							particle.CurrentY,
							particle.Character,
							ConsoleManager.FOREGROUND_WHITE
						);
					}
					break;

				case ParticleState.Done:
					// Nada a fazer
					break;
			}
		}

		/// <summary>
		/// Calcula a cor baseada na distância caída (gradiente)
		/// </summary>
		private short CalculateColorGradient(BannerParticle particle)
		{
			int distanceFallen = particle.CurrentY - particle.OriginalY;

			if (distanceFallen < 3)
			{
				// Verde escuro inicial
				return ConsoleManager.FOREGROUND_GREEN;
			}
			else if (distanceFallen < 8)
			{
				// Verde claro
				return 0x000A;
			}
			else if (distanceFallen < 15)
			{
				// Verde brilhante
				return (short)(0x000A | ConsoleManager.FOREGROUND_INTENSITY);
			}
			else
			{
				// Branco brilhante
				return ConsoleManager.FOREGROUND_WHITE;
			}
		}

		/// <summary>
		/// Retorna um caractere aleatório do conjunto Matrix
		/// </summary>
		private char GetRandomMatrixChar()
		{
			return _matrixChars[_random.Next(_matrixChars.Length)];
		}
	}
}
