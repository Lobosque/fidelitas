# Plano de Negócios — Voltei

> Sistema de fidelidade digital sem aplicativo para pequenos negócios brasileiros.

---

## 1. O Problema

Milhões de pequenos negócios no Brasil usam cartões de fidelidade de papel (ex: "10 cortes → 1 grátis"). Esse modelo tem falhas claras:

**Para o cliente:**
- Perde o cartão na carteira, bolsa ou lavagem de roupa
- Esquece quantos pontos acumulou
- Não sabe quando está perto do prêmio

**Para o dono do negócio:**
- Nenhum dado sobre frequência ou perfil dos clientes
- Não sabe quem parou de vir ou por quê
- Não tem como se comunicar com clientes de forma proativa
- Sem inteligência para tomar decisões

---

## 2. A Solução

O **Voltei** é um sistema digital de fidelidade que substitui o cartão de papel — sem que o cliente precise baixar nenhum aplicativo.

### Como funciona

1. O dono do negócio faz seu cadastro no Voltei
2. Cria uma campanha (ex: "Compre 9 sorvetes e ganhe o décimo")
3. O sistema gera um totem com QR code para imprimir e colocar no balcão
4. O cliente escaneia o QR code com a câmera do celular
5. Se for a primeira vez, faz um cadastro rápido (nome, telefone)
6. Recebe um cartão digital para guardar na carteira do celular (Apple Wallet / Google Wallet)
7. A cada compra, o atendente escaneia o QR code da carteira digital → check-in registrado
8. Ao completar os check-ins necessários, o cliente resgata o prêmio

### Loop principal

```
Cliente escaneia QR (totem) → Cadastro rápido → Cartão na carteira digital
                                                         ↓
              Prêmio resgatado ← Check-ins acumulados ← Atendente escaneia carteira
```

### Para o dono do negócio

- Painel com dados de frequência, retenção e perfil de clientes
- Múltiplas campanhas simultâneas
- Comunicação direta via push notification
- Relatórios de desempenho das campanhas

---

## 3. Proposta de Valor e Posicionamento

### Proposta de valor

> "Transforme seu cartão fidelidade de papel em um sistema digital inteligente — sem que seu cliente precise baixar nenhum app. Em 5 minutos você cria sua campanha, imprime um QR code, e seus clientes acumulam check-ins direto na carteira digital do celular."

### Pilares de diferenciação

1. **Zero atrito** — sem app para o cliente, usa carteira digital nativa (Apple/Google Wallet)
2. **Simplicidade** — o dono configura em minutos, não em dias
3. **Inteligência** — dados reais de frequência, perfil e comunicação com o cliente
4. **Acessível** — preço justo para MEI e micro empresas

### Posicionamento competitivo

- **vs. concorrentes brasileiros** (Fidel, FidelizeApp, Fidelizi): Sem app para o cliente — zero atrito
- **vs. concorrentes internacionais** (Loopy Loyalty, PassKit): Feito no Brasil, em português, com preço em real
- **vs. cartão de papel**: Nunca mais perde o cartão. O dono ganha dados reais.

---

## 4. Análise de Concorrência

### Concorrentes brasileiros

| Plataforma | Modelo | Diferencial | Fraqueza |
|---|---|---|---|
| Fidel.com.br | Freemium (30 dias) | Cartão digital, WhatsApp, push | Requer app próprio do cliente |
| Fidelizi Mini | Gratuito | Simples, focado em food service | Sem carteira digital nativa |
| FidelizeApp | Assinatura | App próprio para o cliente | Atrito — cliente precisa baixar app |
| Fidelimax | Assinatura | Cashback + automação | Mais complexo, voltado a e-commerce |
| Smartbis | Assinatura | Cashback + referral | Mais voltado para médias empresas |

### Concorrentes internacionais

| Plataforma | Preço | Diferencial | Fraqueza |
|---|---|---|---|
| Loopy Loyalty | US$25-95/mês | Apple/Google Wallet nativo | Sem foco no Brasil, preço em dólar |
| Boomerangme | Assinatura | Wallet + múltiplos tipos de cartão | Sem localização BR |
| PassKit | Assinatura | Plataforma robusta de wallet | Enterprise, complexo demais para PME |

### Gap de mercado (oportunidade do Voltei)

> Sistema de fidelidade brasileiro, sem app, com carteira digital nativa, acessível para micro e pequenos negócios.

### Dados de mercado

- 88,3% dos brasileiros participam de pelo menos um programa de fidelidade
- 76% interagem semanalmente com esses programas
- 60% dos consumidores já usam cartões em carteira digital no checkout
- Sebrae destaca QR Codes dinâmicos como tendência para pequenos negócios em 2026

---

## 5. Modelo de Receita

### Estrutura de planos

| | Grátis | Profissional | Negócio |
|---|---|---|---|
| **Preço** | R$ 0 | R$ 49/mês | R$ 149/mês |
| **Campanhas ativas** | 1 | 3 | Ilimitadas |
| **Clientes cadastrados** | Até 50 | Até 500 | Ilimitados |
| **Usuários (atendentes)** | 1 | 3 | 10 |
| **Relatórios** | Básico | Completo (frequência, retenção) | Completo + exportação |
| **Push notifications** | Não | Sim (até 500/mês) | Ilimitadas |
| **Personalização do cartão** | Padrão (logo + cores) | Completa | Completa |
| **Suporte** | FAQ | Chat | Chat prioritário |
| **Branding** | "Powered by Voltei" | "Powered by Voltei" | Marca branca |

### Gatilhos de upgrade (grátis → pago)

- Atinge 50 clientes cadastrados
- Quer criar uma segunda campanha
- Quer enviar push notifications
- Quer ver relatórios de retenção

### Receita futura (pós-MVP)

- Plano Franquias — gestão multi-unidade, dashboard consolidado
- Marketplace de promoções — clientes descobrem negócios locais
- Integrações pagas — WhatsApp Business API, sistemas de PDV

---

## 6. Personas e Jornadas

### Persona 1: O Dono do Negócio — "Seu Carlos"

- 38 anos, dono de barbearia no bairro há 6 anos
- Trabalha sozinho ou com 1 ajudante
- Já tentou cartão de papel, mas clientes perdiam
- Usa Instagram e WhatsApp no dia a dia
- Não é técnico — se for complicado, desiste

**Dores:** Clientes somem sem ele saber por quê. Sem dados de frequência. Concorrência crescente. Não sabe se promoções funcionam.

**Jornada:**
1. Descobre o Voltei (indicação, Instagram, Google)
2. Acessa o site, vê que é simples e gratuito para começar
3. Cadastra o negócio em 5 minutos
4. Cria campanha: "10 cortes → 1 grátis"
5. Imprime o QR code e cola no espelho do balcão
6. Começa a ver clientes se cadastrando
7. Após 2 meses, percebe valor nos dados e faz upgrade

### Persona 2: O Cliente Final — "Rafaela"

- 27 anos, usa o celular para tudo
- Não quer baixar app de cada loja que frequenta
- Já perdeu cartão de fidelidade de papel várias vezes
- Gosta de sentir que está "ganhando algo" por ser fiel

**Dores:** Cartão de papel some. Esquece quantos pontos tem. Não sabe quando está perto do prêmio.

**Jornada:**
1. Vai à barbearia/sorveteria, vê o QR code no balcão
2. Escaneia com a câmera do celular
3. Faz cadastro rápido (nome, telefone)
4. Adiciona o cartão na carteira digital
5. A cada compra, o atendente escaneia → check-in registrado
6. Recebe notificação: "Faltam 3 para o seu corte grátis!"
7. Resgata o prêmio — experiência positiva, volta mais

### Persona 3 (futura): Dona de Franquia — "Fernanda"

- Dona de 4 unidades de açaiteria
- Precisa de visão consolidada de todas as lojas
- Quer campanhas padronizadas nas unidades
- Entra num segundo momento, não no MVP

---

## 7. Go-to-Market

### Fase 1: Validação (Mês 1-3) — Meta: 20 negócios ativos

**Estratégia: Corpo a corpo local**

- Escolher um bairro ou cidade pequena e ir pessoalmente nos negócios
- Oferecer o plano gratuito e ajudar o dono a configurar na hora
- Focar em um segmento só (ex: barbearias) para ter casos comparáveis
- Pedir feedback semanal — o produto vai mudar muito nessa fase

**Por que um segmento só?**
- Facilita o discurso de venda ("feito para barbearias")
- Permite criar templates prontos de campanha
- Gera prova social concentrada ("12 barbearias do bairro X já usam")

**Critério de sucesso:**
- Pelo menos 10 dos 20 negócios com clientes finais usando ativamente
- Entender a taxa de cadastro (quantos clientes escaneiam o QR code)
- Identificar os motivos de quem não adota

### Fase 2: Tração Local (Mês 4-8) — Meta: 100 negócios

**Estratégia: Prova social + indicação**

- Cases de sucesso: depoimentos em vídeo dos melhores resultados da Fase 1
- Programa de indicação: "Indique outro negócio e ganhe 1 mês grátis do plano Profissional"
- Parcerias com contadores e associações comerciais
- Instagram e Google Meu Negócio: conteúdo educativo
- Expandir para 2-3 segmentos (barbearias + sorveterias + cafeterias)

**Critério de sucesso:**
- Taxa de conversão grátis → pago acima de 10%
- CAC sustentável
- Primeiros upgrades orgânicos para o plano Profissional

### Fase 3: Escala (Mês 9-18) — Meta: 500+ negócios

**Estratégia: Canais escaláveis**

- Marketing de conteúdo / SEO: blog e vídeos sobre fidelização para pequenos negócios
- Anúncios segmentados: Instagram/Facebook Ads para donos de pequenos negócios
- Parcerias com fornecedores: distribuidoras que oferecem o Voltei como benefício
- Marketplace de descoberta: clientes encontram negócios participantes próximos

### Resumo das fases

```
Fase 1 (1-3m)     Fase 2 (4-8m)        Fase 3 (9-18m)
Corpo a corpo  →   Indicação/Cases  →   SEO + Ads + Parcerias
20 negócios        100 negócios         500+ negócios
1 segmento         2-3 segmentos        Multi-segmento
Validar            Monetizar            Escalar
```

---

## 8. Métricas-Chave (KPIs)

### North Star Metric

> **Check-ins registrados por semana**

Se esse número cresce, significa que novos negócios estão entrando, os existentes estão ativos, os clientes finais estão usando, e o loop principal está funcionando.

### KPIs da Plataforma

| Métrica | O que mede | Meta inicial |
|---|---|---|
| Negócios cadastrados | Adoção geral | 20 → 100 → 500 |
| Negócios ativos | Com check-in nos últimos 30 dias | >60% dos cadastrados |
| Conversão free → pago | Monetização | >10% |
| MRR | Saúde financeira | Acompanhar mês a mês |
| Churn mensal | Cancelamentos | <5% |
| CAC | Custo de aquisição por negócio | Fase 1: ~R$0 |
| LTV | Valor ao longo do tempo | LTV > 3x CAC |

### KPIs do Dono do Negócio (visíveis no painel)

| Métrica | Por que importa |
|---|---|
| Clientes cadastrados | Mostra adoção do sistema |
| Taxa de escaneamento | Mede visibilidade do totem |
| Frequência média de visita | A cada quantos dias o cliente volta |
| Clientes próximos do prêmio | Oportunidade de push para reengajamento |
| Clientes inativos (30+ dias) | Alerta para ação |
| Resgates realizados | Prova de que o programa funciona |

### KPIs do Cliente Final (internos)

| Métrica | O que revela |
|---|---|
| Taxa de cadastro após scan | O onboarding está simples o bastante? |
| Taxa de adição à carteira | O cliente realmente guarda o cartão? |
| Tempo médio entre check-ins | Frequência de consumo real |
| Taxa de resgate | Clientes chegam ao prêmio ou desistem? |
| Clientes multi-negócio | Efeito de rede — usam em mais de um local |

---

## 9. Projeção Financeira

### Premissas

| Premissa | Valor |
|---|---|
| Período de projeção | 18 meses |
| Início de receita | Mês 4 |
| Conversão free → pago | 10% |
| Distribuição dos pagantes | 70% Profissional (R$49) / 30% Negócio (R$149) |
| Churn mensal | 5% |
| Ticket médio ponderado | ~R$79/mês |

### Projeção de crescimento

| Mês | Negócios cadastrados | Pagantes | MRR |
|---|---|---|---|
| 3 | 20 | 0 | R$ 0 |
| 6 | 60 | 6 | ~R$ 470 |
| 9 | 120 | 12 | ~R$ 950 |
| 12 | 250 | 25 | ~R$ 1.975 |
| 15 | 400 | 40 | ~R$ 3.160 |
| 18 | 600 | 60 | ~R$ 4.740 |

### Custos operacionais (fase inicial)

| Item | Custo mensal |
|---|---|
| Infraestrutura (servidor, domínio, email) | R$ 200-500 |
| Apple Developer Account | ~R$ 50/mês (US$99/ano) |
| Google Wallet API | Gratuito |
| Marketing/Ads (a partir do mês 4) | R$ 500-1.500 |
| **Total mensal** | **R$ 750 - 2.000** |

### Break-even

Com custo operacional de ~R$1.500/mês e ticket médio de R$79:
- **~19 clientes pagantes** para atingir equilíbrio
- Estimativa: entre o **mês 10 e 12**

---

## 10. Próximos Passos Imediatos

1. **Registrar o nome "Voltei"**
   - Verificar domínio em registro.br (voltei.com.br e variações)
   - Consultar base do INPI na classe 42 (software/SaaS)
   - Registrar a marca (~R$355 via e-Marcas)

2. **Definir MVP técnico**
   - Escopo mínimo para a Fase 1 de validação
   - Stack tecnológica
   - Integrações com Apple Wallet e Google Wallet

3. **Iniciar validação qualitativa**
   - Conversar com 10 donos de pequenos negócios sobre o conceito
   - Validar se o fluxo do QR code + carteira digital faz sentido na prática
   - Identificar objeções antes de desenvolver

---

*Documento gerado em fevereiro de 2026.*
*Codenome do projeto: Fidelitas → Nome definido: Voltei*
