# ?? Guia de Customização - Animação de Derretimento

## Ajustes Rápidos

### 1. Velocidade da Animação

#### Derretimento Mais Rápido
```csharp
// Em BannerMeltingAnimation.cs, método Animate()
Thread.Sleep(15);  // Padrão: 30ms
```

#### Derretimento Mais Lento
```csharp
Thread.Sleep(50);  // Mais dramático
```

### 2. Efeito de Cascata

#### Cascata Mais Gradual
```csharp
// Em ConvertBannerToParticles()
DelayFrames = y * 5 + _random.Next(0, 10)
```

#### Cascata Mais Sincronizada
```csharp
DelayFrames = y * 1 + _random.Next(0, 2)
```

#### Sem Cascata (Tudo Cai Junto)
```csharp
DelayFrames = 0
```

### 3. Intensidade do Glitch

#### Mais Glitch
```csharp
// Em UpdateParticle(), estado Falling
if (_random.Next(100) < 70)  // Padrão: 40
```

#### Menos Glitch (Mais Suave)
```csharp
if (_random.Next(100) < 15)
```

#### Sem Glitch
```csharp
// Comente a seção de glitch
// if (_random.Next(100) < 40) { ... }
```

### 4. Ondulação Horizontal

#### Mais Ondulação
```csharp
// Em UpdateParticle(), estado Falling
if (_random.Next(100) < 60)  // Padrão: 30
```

#### Sem Ondulação
```csharp
// Comente a seção de ondulação
// if (_random.Next(100) < 30) { ... }
```

### 5. Gradiente de Cores

#### Personalizar Cores
```csharp
// Em CalculateColorGradient()
private short CalculateColorGradient(BannerParticle particle)
{
    int distanceFallen = particle.CurrentY - particle.OriginalY;

    if (distanceFallen < 5)
        return 0x0009;  // Azul escuro
    else if (distanceFallen < 10)
        return 0x000B;  // Ciano
    else if (distanceFallen < 15)
        return 0x000D;  // Magenta
    else
        return 0x000F;  // Branco
}
```

#### Cores Disponíveis (Windows Console)
```csharp
0x0001  // Azul escuro
0x0002  // Verde escuro
0x0003  // Ciano escuro
0x0004  // Vermelho escuro
0x0005  // Magenta escuro
0x0006  // Amarelo escuro
0x0007  // Cinza claro
0x0008  // Cinza escuro
0x0009  // Azul
0x000A  // Verde
0x000B  // Ciano
0x000C  // Vermelho
0x000D  // Magenta
0x000E  // Amarelo
0x000F  // Branco
```

### 6. Velocidade de Queda Individual

#### Queda Mais Uniforme
```csharp
// Em ConvertBannerToParticles()
FallSpeed = 2  // Fixo (padrão: random 1-4)
```

#### Queda Mais Variada
```csharp
FallSpeed = _random.Next(1, 6)  // Range maior
```

### 7. Transformação Final (Morphing)

#### Morphing Mais Cedo
```csharp
// Em UpdateParticle(), estado Falling
if (distanceFallen > 10)  // Padrão: 20
```

#### Morphing Mais Tarde
```csharp
if (distanceFallen > 30)
```

#### Sem Morphing
```csharp
// Remova a transição para Morphing
// particle.State = ParticleState.Morphing;
```

## Exemplos de Configurações Prontas

### ?? Modo "Apocalipse" (Rápido e Caótico)
```csharp
// BannerMeltingAnimation.cs

// Velocidade
Thread.Sleep(10);

// Cascata
DelayFrames = y * 1 + _random.Next(0, 2)

// Glitch intenso
if (_random.Next(100) < 80)

// Ondulação máxima
if (_random.Next(100) < 70)

// Velocidade variada
FallSpeed = _random.Next(1, 8)
```

### ?? Modo "Onda Suave" (Lento e Elegante)
```csharp
// Velocidade
Thread.Sleep(50)

// Cascata gradual
DelayFrames = y * 8 + _random.Next(0, 5)

// Pouco glitch
if (_random.Next(100) < 10)

// Sem ondulação
// (comentar seção)

// Velocidade uniforme
FallSpeed = 3
```

### ? Modo "Digital Rain" (Estilo Matrix Clássico)
```csharp
// Velocidade média
Thread.Sleep(25)

// Cascata média
DelayFrames = y * 3 + _random.Next(0, 5)

// Glitch moderado
if (_random.Next(100) < 40)

// Pouca ondulação
if (_random.Next(100) < 15)

// Morphing rápido
if (distanceFallen > 12)
```

### ?? Modo "Precisão" (Sem Randomização)
```csharp
// Velocidade fixa
Thread.Sleep(30)

// Cascata uniforme
DelayFrames = y * 4  // Sem random

// Sem glitch
// (comentar)

// Sem ondulação
// (comentar)

// Velocidade fixa
FallSpeed = 2

// Cores fixas
return ConsoleManager.FOREGROUND_GREEN  // Sempre verde
```

## Criando Temas Personalizados

### Tema "Hacker Verde Clássico"
```csharp
// Todas partículas em verde
private short CalculateColorGradient(BannerParticle particle)
{
    return 0x000A;  // Verde sempre
}

// Sem transformação
// (remover estado Morphing)
```

### Tema "Cyberpunk Rosa/Azul"
```csharp
private short CalculateColorGradient(BannerParticle particle)
{
    int distanceFallen = particle.CurrentY - particle.OriginalY;
    
    if (distanceFallen < 10)
        return 0x000D;  // Magenta
    else
        return 0x000B;  // Ciano
}
```

### Tema "Fogo Digital"
```csharp
private short CalculateColorGradient(BannerParticle particle)
{
    int distanceFallen = particle.CurrentY - particle.OriginalY;
    
    if (distanceFallen < 5)
        return 0x000E;  // Amarelo
    else if (distanceFallen < 12)
        return 0x000C;  // Vermelho
    else
        return 0x0008;  // Cinza (desaparecendo)
}
```

## Adicionando Som (Opcional)

### Beep no Início
```csharp
public void Animate()
{
    // Sinal sonoro de início
    Console.Beep(800, 100);
    
    ConvertBannerToParticles();
    // ...
}
```

### Beep por Linha Derretida
```csharp
private void ConvertBannerToParticles()
{
    for (int y = 0; y < bannerLines.Length; y++)
    {
        Console.Beep(1000 - (y * 50), 50);
        // ...
    }
}
```

### Beep Contínuo Durante Queda
```csharp
// No loop principal
if (_frameCount % 10 == 0)
{
    Console.Beep(400 + (_frameCount % 200), 30);
}
```

## Combinando com Outras Funcionalidades

### Exibir Mensagem Durante Derretimento
```csharp
public void Animate()
{
    ConvertBannerToParticles();
    
    // Mensagem no rodapé
    Console.SetCursorPosition(0, _height - 2);
    Console.ForegroundColor = ConsoleColor.DarkGreen;
    Console.WriteLine("    [SYSTEM MELTDOWN INITIATED]");
    Console.ResetColor();
    
    // Loop principal...
}
```

### Pausa Configurável Antes de Matrix Rain
```csharp
// Em MatrixRain.RunWithMeltingBanner()
meltingAnimation.Animate();

// Pausa ajustável
Thread.Sleep(1500);  // Padrão: 500ms

Console.Clear();
```

## Performance Tips

### Para Consoles Grandes (>150x50)
```csharp
// Reduzir número de partículas
if (_random.Next(100) < 50)  // Apenas 50% dos caracteres
{
    _particles.Add(new BannerParticle { ... });
}
```

### Para Animação Mais Fluida
```csharp
// Reduzir sleep
Thread.Sleep(16);  // ~60 FPS

// Simplificar cálculos
// Usar lookup table para cores
```

## Troubleshooting

### Problema: Animação muito rápida
**Solução**: Aumentar `Thread.Sleep(30)` ? `Thread.Sleep(50)`

### Problema: Caracteres "pulam" ao invés de deslizar
**Solução**: Reduzir `FallSpeed` ou aumentar taxa de atualização

### Problema: Muita variação de cores
**Solução**: Simplificar `CalculateColorGradient()`

### Problema: Banner desalinhado
**Solução**: Verificar `GetCenteredStartX()` ou ajustar `startY`

---

**Experimente e crie sua própria variação única!** ??
