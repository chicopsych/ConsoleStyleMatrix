using System;

namespace ConsoleStyleMatrix.Classes
{
	/// <summary>
	/// Representa um caractere individual do banner durante a animação de derretimento
	/// </summary>
	internal class BannerParticle
	{
		/// <summary>
		/// Posição X original no banner
		/// </summary>
		public int OriginalX { get; set; }

		/// <summary>
		/// Posição Y original no banner
		/// </summary>
		public int OriginalY { get; set; }

		/// <summary>
		/// Posição Y atual durante a queda
		/// </summary>
		public int CurrentY { get; set; }

		/// <summary>
		/// Caractere original do banner
		/// </summary>
		public char Character { get; set; }

		/// <summary>
		/// Velocidade de queda (frames para pular)
		/// </summary>
		public int FallSpeed { get; set; }

		/// <summary>
		/// Número de frames para aguardar antes de começar a cair
		/// </summary>
		public int DelayFrames { get; set; }

		/// <summary>
		/// Estado atual da partícula
		/// </summary>
		public ParticleState State { get; set; }

		/// <summary>
		/// Deslocamento horizontal para efeito de ondulação
		/// </summary>
		public int HorizontalOffset { get; set; }

		public BannerParticle()
		{
			State = ParticleState.Waiting;
			HorizontalOffset = 0;
		}
	}

	/// <summary>
	/// Estados possíveis de uma partícula do banner
	/// </summary>
	internal enum ParticleState
	{
		/// <summary>
		/// Aguardando para começar a cair
		/// </summary>
		Waiting,

		/// <summary>
		/// Caindo pela tela
		/// </summary>
		Falling,

		/// <summary>
		/// Transformando-se em caractere Matrix
		/// </summary>
		Morphing,

		/// <summary>
		/// Terminou a animação
		/// </summary>
		Done
	}
}
