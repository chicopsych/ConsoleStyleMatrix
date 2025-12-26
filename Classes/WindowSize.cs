using System;
using System.Threading;

namespace ConsoleStyleMatrix.Classes
{
	/// <summary>
	/// Evento disparado quando o tamanho da janela do console é alterado
	/// </summary>
	public class WindowResizeEventArgs : EventArgs
	{
		public short OldWidth { get; set; }
		public short OldHeight { get; set; }
		public short NewWidth { get; set; }
		public short NewHeight { get; set; }

		public bool HasChanged
		{
			get { return OldWidth != NewWidth || OldHeight != NewHeight; }
		}
	}

	/// <summary>
	/// Monitora e detecta mudanças no tamanho da janela do console de forma dinâmica
	/// </summary>
	internal class WindowSize : IDisposable
	{
		private readonly ConsoleManager _consoleManager;
		private Thread _monitorThread;
		private volatile bool _isMonitoring;
		private short _currentWidth;
		private short _currentHeight;
		private readonly object _lockObject = new object();

		/// <summary>
		/// Intervalo de verificação em milissegundos (padrão: 500ms)
		/// </summary>
		public int CheckInterval { get; set; }

		/// <summary>
		/// Indica se o monitoramento está ativo
		/// </summary>
		public bool IsMonitoring
		{
			get { return _isMonitoring; }
		}

		/// <summary>
		/// Largura atual da janela
		/// </summary>
		public short CurrentWidth
		{
			get
			{
				lock (_lockObject)
				{
					return _currentWidth;
				}
			}
		}

		/// <summary>
		/// Altura atual da janela
		/// </summary>
		public short CurrentHeight
		{
			get
			{
				lock (_lockObject)
				{
					return _currentHeight;
				}
			}
		}

		/// <summary>
		/// Evento disparado quando o tamanho da janela muda
		/// </summary>
		public event EventHandler<WindowResizeEventArgs> WindowResized;

		public WindowSize(ConsoleManager consoleManager)
		{
			_consoleManager = consoleManager;
			CheckInterval = 500; // Verifica a cada 500ms
			
			// Obtém o tamanho inicial
			_consoleManager.GetBufferSize(out _currentWidth, out _currentHeight);
		}

		/// <summary>
		/// Inicia o monitoramento do tamanho da janela
		/// </summary>
		public void StartMonitoring()
		{
			if (_isMonitoring)
				return;

			_isMonitoring = true;
			_monitorThread = new Thread(MonitorLoop);
			_monitorThread.IsBackground = true;
			_monitorThread.Name = "WindowSizeMonitor";
			_monitorThread.Start();
		}

		/// <summary>
		/// Para o monitoramento do tamanho da janela
		/// </summary>
		public void StopMonitoring()
		{
			if (!_isMonitoring)
				return;

			_isMonitoring = false;

			// Aguarda a thread terminar (com timeout de 1 segundo)
			if (_monitorThread != null && _monitorThread.IsAlive)
			{
				_monitorThread.Join(1000);
			}
		}

		/// <summary>
		/// Loop de monitoramento executado em thread separada
		/// </summary>
		private void MonitorLoop()
		{
			while (_isMonitoring)
			{
				try
				{
					CheckForResize();
					Thread.Sleep(CheckInterval);
				}
				catch (ThreadInterruptedException)
				{
					// Thread foi interrompida, sair do loop
					break;
				}
				catch (Exception)
				{
					// Ignora erros e continua monitorando
				}
			}
		}

		/// <summary>
		/// Verifica se houve mudança no tamanho da janela
		/// </summary>
		private void CheckForResize()
		{
			short newWidth, newHeight;
			_consoleManager.GetBufferSize(out newWidth, out newHeight);

			bool hasChanged = false;
			WindowResizeEventArgs eventArgs = null;

			lock (_lockObject)
			{
				if (newWidth != _currentWidth || newHeight != _currentHeight)
				{
					// Detectou mudança
					eventArgs = new WindowResizeEventArgs
					{
						OldWidth = _currentWidth,
						OldHeight = _currentHeight,
						NewWidth = newWidth,
						NewHeight = newHeight
					};

					_currentWidth = newWidth;
					_currentHeight = newHeight;
					hasChanged = true;
				}
			}

			// Dispara o evento fora do lock para evitar deadlocks
			if (hasChanged && WindowResized != null)
			{
				WindowResized(this, eventArgs);
			}
		}

		/// <summary>
		/// Força uma verificação imediata de redimensionamento
		/// </summary>
		public void ForceCheck()
		{
			CheckForResize();
		}

		/// <summary>
		/// Obtém o tamanho atual da janela sem esperar pelo próximo ciclo de monitoramento
		/// </summary>
		public void GetCurrentSize(out short width, out short height)
		{
			lock (_lockObject)
			{
				width = _currentWidth;
				height = _currentHeight;
			}
		}

		/// <summary>
		/// Libera recursos e para o monitoramento
		/// </summary>
		public void Dispose()
		{
			StopMonitoring();
		}
	}
}
