using System;
using System.Threading;

namespace ConsoleStyleMatrix.Classes
{
	/// <summary>
	/// Gerencia a animação completa do efeito Matrix Rain
	/// </summary>
	internal class MatrixRain : IDisposable
	{
		private ConsoleManager _consoleManager;
		private MatrixRenderer _renderer;
		private MatrixColumn[] _columns;
		private short _width;
		private short _height;
		private int _frameCount;
		private int _animationSpeed;
		private WindowSize _windowMonitor;
		private volatile bool _needsResize;
		private volatile bool _isRunning;
		private readonly object _resizeLock = new object();

		/// <summary>
		/// Velocidade da animação em milissegundos
		/// </summary>
		public int AnimationSpeed
		{
			get { return _animationSpeed; }
			set { _animationSpeed = value; }
		}

		public MatrixRain()
		{
			_animationSpeed = 30;
			_consoleManager = new ConsoleManager();
			_consoleManager.ConfigureForMatrix();

			InitializeResources();

			// Configura monitoramento de redimensionamento
			_windowMonitor = new WindowSize(_consoleManager);
			_windowMonitor.WindowResized += OnWindowResized;
			_windowMonitor.StartMonitoring();
		}

		/// <summary>
		/// Inicializa ou reinicializa os recursos da animação
		/// </summary>
		private void InitializeResources()
		{
			short width, height;
			_consoleManager.GetBufferSize(out width, out height);
			_width = width;
			_height = height;

			_renderer = new MatrixRenderer(_consoleManager.ConsoleHandle, _width, _height);
			_columns = new MatrixColumn[_width];

			InitializeColumns();
		}

		/// <summary>
		/// Handler para evento de redimensionamento da janela
		/// </summary>
		private void OnWindowResized(object sender, WindowResizeEventArgs e)
		{
			if (!_isRunning)
				return;

			// Marca que precisa redimensionar
			_needsResize = true;
		}

		/// <summary>
		/// Reinicializa a animação com o novo tamanho da janela
		/// </summary>
		private void ReinitializeForNewSize()
		{
			lock (_resizeLock)
			{
				try
				{
					// Limpa a tela
					Console.Clear();

					// Reconfigura o console
					_consoleManager.MaximizeConsoleWindow();

					// Reinicializa recursos com novo tamanho
					InitializeResources();

					// Reseta contador de frames
					_frameCount = 0;
				}
				catch (Exception)
				{
					// Se falhar, continua com o tamanho anterior
				}
			}
		}

		/// <summary>
		/// Inicializa todas as colunas com valores aleatórios
		/// </summary>
		private void InitializeColumns()
		{
			for (int x = 0; x < _width; x++)
			{
				_columns[x] = new MatrixColumn(x, _height);
			}
		}

		/// <summary>
		/// Atualiza o estado de uma coluna específica
		/// </summary>
		private void UpdateColumn(MatrixColumn column)
		{
			// Só atualiza baseado na velocidade da coluna
			if (_frameCount % column.Speed != 0)
				return;

			int x = column.X;
			int y = column.Y;
			int length = column.Length;

			// 1. Desenha a "cabeça" da gota (Branco Brilhante)
			_renderer.DrawChar(x, y, _renderer.GetRandomChar(), ConsoleManager.FOREGROUND_WHITE);

			// 2. Desenha o "corpo" da gota (Verde Escuro)
			if (y - 1 >= 0)
			{
				_renderer.DrawChar(x, y - 1, _renderer.GetRandomChar(), ConsoleManager.FOREGROUND_DARK_GREEN);
			}

			// 3. Adiciona variação de intensidade no rastro
			if (y - 2 >= 0 && new Random().Next(10) < 3)
			{
				_renderer.UpdateChar(x, y - 2, _renderer.GetRandomChar());
			}

			// 4. Apaga o final do rastro (Preto/Espaço)
			int tailY = column.GetTailY();
			_renderer.ClearChar(x, tailY);

			// Move a gota para baixo
			column.MoveDown();

			// Se o rastro inteiro já saiu da tela, reinicia a coluna no topo
			if (column.ShouldReset())
			{
				column.Reset();
			}
		}

		/// <summary>
		/// Executa o loop principal da animação
		/// </summary>
		public void Run()
		{
			_isRunning = true;

			while (_isRunning)
			{
				// Verifica se o usuário quer sair
				if (Console.KeyAvailable)
				{
					var key = Console.ReadKey(true).Key;
					if (key == ConsoleKey.Escape)
					{
						_isRunning = false;
						break;
					}
				}

				// Verifica se precisa redimensionar
				if (_needsResize)
				{
					ReinitializeForNewSize();
					_needsResize = false;
				}

				_frameCount++;

				// Atualiza todas as colunas
				lock (_resizeLock)
				{
					foreach (var column in _columns)
					{
						UpdateColumn(column);
					}

					// Renderiza o frame
					_renderer.Render();
				}

				// Pausa para controlar a velocidade da animação
				Thread.Sleep(_animationSpeed);
			}

			// Restaura configurações ao sair
			Cleanup();
		}

		/// <summary>
		/// Executa a animação com um banner inicial
		/// </summary>
		public void RunWithBanner()
		{
			// Exibe o banner antes de iniciar a animação
			Console.Clear();
			MeltingName.DisplayBannerCentered(ConsoleColor.Green);
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine("                    Pressione ESC para sair...");
			Console.ResetColor();
			Thread.Sleep(3000);

			// Reconfigura o console para maximizado
			_consoleManager.MaximizeConsoleWindow();

			// Inicia a animação
			Run();
		}

		/// <summary>
		/// Executa a animação com efeito de derretimento do banner no estilo Matrix
		/// </summary>
		public void RunWithMeltingBanner()
		{
			// 1. Exibe o banner estático por 2 segundos
			Console.Clear();
			MeltingName.DisplayBannerCentered(ConsoleColor.Green);
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine("                    Pressione ESC para sair...");
			Console.ResetColor();
			Thread.Sleep(2000);

			// 2. Reconfigura o console para maximizado e limpa
			_consoleManager.MaximizeConsoleWindow();
			Console.Clear();

			// 3. Reinicializa recursos com o tamanho correto
			InitializeResources();

			// 4. Executa animação de derretimento
			BannerMeltingAnimation meltingAnimation = new BannerMeltingAnimation(
				_renderer,
				_width,
				_height
			);
			meltingAnimation.Animate();

			// 5. Limpa tela e reinicializa colunas para Matrix Rain
			Console.Clear();
			InitializeResources();

			// 6. Inicia a animação Matrix Rain normal
			Run();
		}

		/// <summary>
		/// Limpa recursos e restaura console
		/// </summary>
		private void Cleanup()
		{
			_windowMonitor.StopMonitoring();
			_consoleManager.RestoreDefaults();
		}

		/// <summary>
		/// Libera recursos
		/// </summary>
		public void Dispose()
		{
			_isRunning = false;
			
			if (_windowMonitor != null)
			{
				_windowMonitor.Dispose();
			}

			Cleanup();
		}
	}
}
