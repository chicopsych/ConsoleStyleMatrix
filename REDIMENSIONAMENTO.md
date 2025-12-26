# ?? Redimensionamento Dinâmico da Janela

## Visão Geral

A aplicação **Console Style Matrix** agora suporta **redimensionamento dinâmico** da janela do console durante a execução da animação.

## Como Funciona

### Componentes

#### 1. **WindowSize** (`Classes\WindowSize.cs`)
- Monitora continuamente o tamanho da janela do console
- Executa em thread separada (background)
- Verifica mudanças a cada 500ms
- Dispara evento `WindowResized` quando detecta alteração

#### 2. **MatrixRain** (`Classes\Drops.cs`)
- Assina o evento de redimensionamento
- Reinicializa recursos automaticamente quando o tamanho muda
- Mantém thread-safety durante a reinicialização

### Fluxo de Execução

```
????????????????
? Usuário      ?
? redimensiona ?
? janela       ?
????????????????
       ?
       ?
???????????????????????
? WindowSize          ?
? detecta mudança     ? (thread de monitoramento)
???????????????????????
       ?
       ?
???????????????????????
? Evento              ?
? WindowResized       ?
? é disparado         ?
???????????????????????
       ?
       ?
???????????????????????
? MatrixRain          ?
? marca _needsResize  ?
???????????????????????
       ?
       ?
???????????????????????
? Próximo frame       ?
? detecta flag        ?
???????????????????????
       ?
       ?
???????????????????????
? ReinitializeForNewSize ?
? - Limpa tela        ?
? - Maximiza janela   ?
? - Recria renderer   ?
? - Recria colunas    ?
???????????????????????
       ?
       ?
???????????????????????
? Animação continua   ?
? com novo tamanho    ?
???????????????????????
```

## Recursos Implementados

### ? Thread-Safety
- Uso de `lock` para sincronizar acesso aos recursos compartilhados
- Flags `volatile` para comunicação entre threads
- Separação de responsabilidades entre threads

### ? Performance
- Verificação a cada 500ms (configurável via `CheckInterval`)
- Reinicialização apenas quando realmente necessário
- Thread em background (não bloqueia aplicação)

### ? Robustez
- Tratamento de exceções durante redimensionamento
- Implementação de `IDisposable` para limpeza adequada
- Timeout ao parar thread de monitoramento

### ? Flexibilidade
- Intervalo de verificação configurável
- Evento público para extensibilidade
- Propriedades para consultar estado atual

## API Pública

### WindowSize

```csharp
// Propriedades
public int CheckInterval { get; set; }           // Padrão: 500ms
public bool IsMonitoring { get; }                // Status do monitoramento
public short CurrentWidth { get; }               // Largura atual
public short CurrentHeight { get; }              // Altura atual

// Evento
public event EventHandler<WindowResizeEventArgs> WindowResized;

// Métodos
public void StartMonitoring()                    // Inicia monitoramento
public void StopMonitoring()                     // Para monitoramento
public void ForceCheck()                         // Força verificação imediata
public void GetCurrentSize(out short w, out short h)  // Obtém tamanho atual
public void Dispose()                            // Libera recursos
```

### WindowResizeEventArgs

```csharp
public short OldWidth { get; set; }
public short OldHeight { get; set; }
public short NewWidth { get; set; }
public short NewHeight { get; set; }
public bool HasChanged { get; }                  // Indica se houve mudança real
```

## Exemplo de Uso

```csharp
// O redimensionamento é automático!
// Basta executar a aplicação normalmente:

MatrixRain matrixRain = new MatrixRain();
matrixRain.RunWithBanner();

// A janela pode ser redimensionada manualmente pelo usuário
// e a animação se ajustará automaticamente
```

## Configuração Personalizada

```csharp
// Se quiser ajustar o intervalo de verificação:
MatrixRain matrixRain = new MatrixRain();
matrixRain._windowMonitor.CheckInterval = 1000; // Verifica a cada 1 segundo

// Se quiser receber notificações de redimensionamento:
matrixRain._windowMonitor.WindowResized += (sender, e) =>
{
    Console.WriteLine($"Janela redimensionada de {e.OldWidth}x{e.OldHeight} para {e.NewWidth}x{e.NewHeight}");
};
```

## Notas Técnicas

### Quando a Reinicialização Ocorre?
- Somente quando o tamanho do buffer do console realmente muda
- Verificação dupla para evitar falsos positivos
- Reinicialização ocorre entre frames (não interrompe frame atual)

### O que é Reinicializado?
1. **MatrixRenderer** - Buffer de tela recriado com novo tamanho
2. **MatrixColumn[]** - Array de colunas redimensionado
3. **Posições e propriedades** - Todas as colunas resetadas

### O que NÃO é Reinicializado?
- **ConsoleManager** - Reutilizado
- **Velocidade de animação** - Mantida
- **Estado do monitoramento** - Continua ativo

## Limitações Conhecidas

1. **Apenas Windows** - P/Invoke usa API do Windows (kernel32.dll)
2. **Breve interrupção visual** - Durante redimensionamento, tela é limpa
3. **Console legado** - Funciona melhor em Windows Console, não Windows Terminal

## Troubleshooting

### Problema: Animação não se ajusta ao redimensionar
**Solução**: Certifique-se de que está usando o console tradicional do Windows, não o Windows Terminal (ainda em desenvolvimento para Terminal)

### Problema: Performance degradada após múltiplos redimensionamentos
**Solução**: Isso é normal - cada redimensionamento recria recursos. Minimize redimensionamentos frequentes.

### Problema: Thread de monitoramento não para
**Solução**: Sempre use `Dispose()` ou `using` para garantir limpeza adequada.

## Futuras Melhorias

- [ ] Suporte para Windows Terminal
- [ ] Transição suave durante redimensionamento
- [ ] Cache de recursos para redimensionamentos frequentes
- [ ] Detecção de maximização/restauração de janela
- [ ] Configuração de debounce para evitar múltiplas reinicializações
