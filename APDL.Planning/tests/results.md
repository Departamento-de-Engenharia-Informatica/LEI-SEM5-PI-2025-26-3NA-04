# Análise de Complexidade Computacional
## Algoritmo de Agendamento Ótimo com Restrições de Recursos

---

## Resumo Executivo

Este relatório apresenta a análise de complexidade computacional do algoritmo de agendamento ótimo implementado para o sistema APDL Planning. O algoritmo considera restrições operacionais realistas: disponibilidade de docas, eficiência de gruas, turnos de pessoal e capacidade de armazenamento. 

**Resultado Principal**: O algoritmo consegue **zero atrasos** para até 10 navios através de paralelização eficiente, mas com complexidade factorial O(n!) que limita a escalabilidade operacional.

---

## 1. Contexto e Configuração do Sistema

### 1.1 Recursos Disponíveis

**Docas e Gruas:**
- **dock_a**: crane01 (fator 1.0 - velocidade normal)
- **dock_b**: crane02 (fator 0.8 - 20% mais rápida)
- **dock_c**: crane03 (fator 1.0 - velocidade normal)

**Pessoal Operacional:** 10 membros distribuídos em 4 turnos
- **Turno Madrugada** (00:00-06:00): staff09, staff10
- **Turno Manhã** (06:00-14:00): staff01, staff02, staff03
- **Turno Tarde** (14:00-22:00): staff04, staff05, staff06
- **Turno Noite** (22:00-06:00+): staff07, staff08

**Armazenamento:**
- Nível inicial: 100 contentores
- Capacidade máxima: 500 contentores
- Unidade: 1 contentor por unidade de tempo (10 minutos)

**Unidade Temporal:**
- 1 unidade de tempo = 10 minutos
- Exemplo: tempo 36 = 06:00, tempo 84 = 14:00

### 1.2 Navios de Teste

10 navios com características variadas:
- Chegadas distribuídas ao longo do dia (tempo 6 a 81)
- Operações de carga/descarga de 0 a 26 unidades de tempo
- Partidas desejadas entre tempo 40 e 110

---

## 2. Resultados Experimentais

### 2.1 Tabela de Performance

| Nº Navios | Permutações (n!) | Tempo de Execução | Atraso Total | Fator de Crescimento |
|-----------|------------------|-------------------|--------------|---------------------|
| 2         | 2                | 0.0026s          | 0            | -                   |
| 3         | 6                | 0.0010s          | 0            | 0.4x                |
| 4         | 24               | 0.0067s          | 0            | 6.7x                |
| 5         | 120              | 0.024s           | 0            | 3.6x                |
| 6         | 720              | 0.16s            | 0            | 6.7x                |
| 7         | 5,040            | 1.35s            | 0            | 8.4x                |
| 8         | 40,320           | 12.06s           | 0            | 8.9x                |
| 9         | 362,880          | 115.00s (1.9 min)| 0            | 9.5x                |
| 10        | 3,628,800        | 1306.07s (21.8 min)| 0          | 11.4x               |
| 11        | 39,916,800       | ~4.1 horas (est.)| -            | ~11.4x              |
| 12        | 479,001,600      | ~2 dias (est.)   | -            | ~12x                |

### 2.2 Observações Críticas

**Zero Atrasos até 10 Navios**
- O sistema consegue processar até 10 navios **sem qualquer atraso**
- Demonstra que a capacidade instalada (3 docas, 3 gruas, 10 staff) é suficiente
- Prova a eficácia da paralelização de operações

**Utilização Eficiente de Recursos:**
- **dock_b (crane02)**: Preferencial - grua mais rápida (0.8)
- **dock_c (crane03)/dock_a (crane01)**: Secundária - operações paralelas

**Crescimento Temporal Factorial:**
- 9→10 navios: tempo × 11.4 (1.9 min → 21.8 min)
- Comportamento esperado para algoritmo de força bruta
- Cada navio adicional multiplica o tempo por aproximadamente n

---

## 3. Análise de Complexidade Detalhada

### 3.1 Classe de Complexidade

**Complexidade Total: O(n! × n × k)**

Onde:
- **n** = número de navios
- **k** = combinações de recursos por navio ≈ 90
  - 3 docas × 3 gruas × 10 staff = 90 opções potenciais

### 3.2 Decomposição Algorítmica

**Nível 1: Geração de Permutações**
```prolog
permutation(LV, SeqV)
```
- **Complexidade**: O(n!)
- **Operação**: Gera todas as ordenações possíveis de n navios
- **Exemplo**: 10 navios = 3.628.800 permutações

**Nível 2: Processamento de Cada Permutação**
```prolog
sequence_temporization(SeqV, SeqTriplets)
```
- **Complexidade**: O(n)
- **Operação**: Para cada permutação, processa n navios sequencialmente

**Nível 3: Atribuição de Recursos por Navio**
```prolog
findall((EndTime, VesselDelay, ...), (...), AvailableOptions)
```
- **Complexidade**: O(d × c × s) ≈ O(90)
- **Operação**: Para cada navio, explora todas as combinações:
  - 3 docas
  - 3 gruas (1 por doca)
  - 10 turnos de pessoal
  - Verificação de armazenamento: O(1)

**Nível 4: Ordenação de Opções**
```prolog
sort(AvailableOptions, ...)
```
- **Complexidade**: O(k log k) onde k ≈ 90
- **Operação**: Ordena por tempo de conclusão (EndTime)

**Nível 5: Operações de Base de Dados**
```prolog
retract/asserta para dock_occupation, staff_occupation, storage_level
```
- **Complexidade**: O(1) amortizado
- **Operação**: 6 operações de BD por navio

### 3.3 Análise de Complexidade Final

**Fórmula Completa:**
```
T(n) = n! × n × (90 + 90×log(90) + 6)
     ≈ n! × n × 100
     = O(n! × n)
```

**Justificação da Classe O(n!):**
- O termo factorial (n!) domina completamente
- Para n=10: 3.628.800 permutações
- Todos os outros termos (n, 100) são insignificantes comparados a n!

**Crescimento Prático:**
```
T(10) ≈ 10! × 10 × 100 = 3.628.800 × 1.000 ≈ 3.6 mil milhões de operações
T(11) ≈ 11! × 11 × 100 = 39.916.800 × 1.100 ≈ 44 mil milhões de operações
```

---

## 4. Impacto das Restrições de Recursos

### 4.1 Impacto Individual por Restrição

**1. Disponibilidade de Docas e Gruas**
- Permite paralelização: até 3 navios simultâneos
- Grua mais rápida (crane02, fator 0.8) reduz tempo em 20%
- **Resultado**: Zero atrasos conseguidos através de distribuição eficiente

**2. Turnos de Pessoal**
- 10 membros de pessoal garantem cobertura 24/7
- Permite operação simultânea de todas as docas
- Transições de turno são geridas sem interrupções
- **Resultado**: Nenhuma operação bloqueada por falta de pessoal

**3. Capacidade de Armazenamento**
- Capacidade de 500 contentores com 100 iniciais é mais que suficiente
- Navios testados têm saldo líquido de -63 contentores (mais carga que descarga)
- **Resultado**: Nunca foi fator limitante nos testes

**4. Eficiência das Gruas**
- Crane02 (0.8) é consistentemente preferida nos resultados ótimos
- Redução de 20% no tempo de operação é significativa
- **Resultado**: Contribui diretamente para zero atrasos

---

## 5. Escalabilidade e Viabilidade Operacional

### 5.1 Limites Práticos por Cenário de Uso

| Cenário de Uso | Tempo Máximo Aceitável | Nº Máximo de Navios | Viabilidade |
|----------------|----------------------|---------------------|-------------|
| **Planeamento em Tempo Real** | < 1 segundo | **5-6 navios** | Limitado |
| **Planeamento Operacional Diário** | < 5 minutos | **8 navios** | Limitado |
| **Planeamento Offline/Noturno** | < 30 minutos | **10 navios** | Viável |
| **Análise e Validação** | < 4 horas | **11 navios** | Viável |
| **Operação Portuária Real** | Variável | **20-50+ navios** | Inviável |

### 5.2 Projeções de Crescimento

**Extrapolação Baseada em Dados Empíricos:**

| Navios | Tempo Estimado | Viabilidade Operacional |
|--------|----------------|------------------------|
| 11     | ~4.1 horas     | Análise offline apenas |
| 12     | ~2 dias        | Benchmarking apenas |
| 13     | ~26 dias       | Académico apenas |
| 15     | ~10 anos       | Impossível |

---

## 6. Conclusões

### 6.1 Escalabilidade
**O algoritmo NÃO é escalável** para operações portuárias reais:
- Limite prático: **10 navios em ~22 minutos**
- Cenário real: **20-30 navios por dia**
- Gap de viabilidade: **2-3× o tamanho gerível**

### 6.2 Eficiência
**Eficiência inadequada para tempo real**:
- Decisões operacionais requerem respostas em **segundos**
- Algoritmo atual necessita **minutos a horas**
- Trade-off inaceitável entre otimalidade e tempo de resposta para cenários reais

### 6.3 Qualidade das Soluções
**Soluções ótimas quando viável**:
- Zero atrasos para até 10 navios
- Utilização eficiente de recursos paralelos
- Adequado como **baseline** para validação de heurísticas

---

## 7. Recomendações

### 7.1 Desenvolvimento de Algoritmo Heurístico (Prioritário)
**Use Case 3.4.4** - Implementar algoritmo heurístico que:
- Produza soluções "boas o suficiente" em **segundos**
- Garanta tempo de resposta previsível O(n²) ou O(n log n)
- Permita operação com 20-50+ navios

**Abordagens Sugeridas:**
- **Greedy por EDD (Earliest Due Date)**: Ordenar por tempo de partida desejado
- **Greedy por SPT (Shortest Processing Time)**: Priorizar operações mais rápidas  
- **Local Search**: Hill climbing com perturbações
- **Metaheurísticas**: Simulated Annealing ou Algoritmos Genéticos

### 7.2 Estratégia Híbrida
- **Instâncias pequenas (≤8 navios)**: Usar algoritmo ótimo
- **Instâncias médias (9-15 navios)**: Usar heurística rápida
- **Instâncias grandes (>15 navios)**: Decompor problema em sub-problemas

### 7.3 Uso do Algoritmo Ótimo
Utilizar como **baseline** para:
- Medir qualidade de soluções heurísticas (gap de otimalidade)
- Validar correção de implementações alternativas
- Benchmarking de performance

---

## Resumo Final

| Métrica | Valor |
|---------|-------|
| **Classe de Complexidade** | O(n! × n) |
| **Limite Prático** | 10 navios / 22 minutos |
| **Escalabilidade** | Inadequada para operações reais |
| **Recomendação** | Implementar algoritmo heurístico (Use Case 3.4.4) |
| **Uso Apropriado** | Validação e benchmarking |

**Conclusão Final**: O algoritmo de força bruta garante soluções ótimas mas é **impraticável para uso operacional** em portos comerciais. A implementação de um algoritmo heurístico (Use Case 3.4.4) é **essencial** para viabilizar o sistema em ambiente de produção.