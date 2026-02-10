# Voltei — MVP

## Stack

**Backend:**
- .NET 10 (Web API)
- Hangfire (jobs em background — escopo a definir)
- PostgreSQL

**Frontend:**
- React (Vite)
- Tailwind UI (componentes pagos) — repositório local em `tailwind_ui/` com 362 componentes organizados por categoria (ver `carai.md` para catálogo completo)
- Chart.js (gráficos do painel)

**Integrações:**
- Google Wallet API (JWT + REST API)
- Apple Wallet (passkit — .pkpass assinado)

**Infraestrutura (sugestão inicial):**
- Hospedagem: a definir
- Storage de imagens: necessário para logos dos negócios (Google Wallet exige URL pública acessível)
- Banco: PostgreSQL gerenciado

---

## Premissas de Design

- O design deve ser limpo, colorido e intuitivo.
- **Mobile first:** toda a interface deve ser projetada primeiro para celulares, depois adaptada para tablets e desktops. A maioria dos donos de negócio e todos os clientes finais acessarão pelo celular.
- A aplicação deve funcionar bem em **celulares, tablets e PCs** — layout responsivo em todos os breakpoints.
- A plataforma deve adaptar seu esquema de cores à paleta do cliente (dono do negócio). Isso se aplica principalmente ao cartão gerado na wallet, mas também aos elementos da interface quando possível.
- O visual do cartão na wallet deve parecer profissional — é a "vitrine" do negócio dentro do celular do cliente final.

### Tailwind UI — Repositório Local

Os componentes do Tailwind UI (licença paga) estão disponíveis localmente em `tailwind_ui/`, organizados por categoria e subcategoria:

```
tailwind_ui/
├── application_shells/     # Stacked, Sidebar, Multi-column layouts
├── headings/               # Page, Card, Section headings
├── data_display/           # Description Lists, Stats, Calendars
├── lists/                  # Stacked Lists, Tables, Grid Lists, Feeds
├── forms/                  # Form Layouts, Input Groups, Select Menus, etc.
├── feedback/               # Alerts, Empty States
├── navigation/             # Navbars, Pagination, Tabs, Breadcrumbs, etc.
├── overlays/               # Modal Dialogs, Drawers, Notifications
├── elements/               # Avatars, Badges, Dropdowns, Buttons, Button Groups
├── layout/                 # Containers, Cards, List Containers, Media Objects, Dividers
└── page_examples/          # Home, Detail, Settings Screens
```

Cada arquivo segue o padrão `N_nome_do_componente.jsx` (ex: `1_simple.jsx`, `2_with_icons.jsx`). O catálogo completo com todos os nomes está em `carai.md` na raiz do projeto.

---

## Premissas de Experiência do Usuário

- Deve ficar sempre claro ao usuário qual a próxima ação a ser feita, e quantas ações faltam para acabar um dado fluxo (progress indicators / steppers).
- Quando ocorrer um erro, deve ficar claro o que aconteceu e como resolver o problema (mensagens de erro acionáveis, não genéricas).
- Deve ser possível cancelar qualquer ação, e deve ficar claro que a ação foi cancelada.
- O onboarding do dono do negócio deve ser guiado passo a passo (wizard), resultando em uma campanha pronta e um totem para imprimir ao final.
- O onboarding do cliente final deve ser o mais curto possível — idealmente, escaneou o QR e em 2 toques já tem o cartão na wallet.

---

## Modelo de Dados (principal)

```
Negocio (Business)
├── id
├── nome
├── email
├── senha (hash)
├── logo_url
├── cores (primária, secundária)
├── plano (gratis | profissional | negocio)
├── criado_em
│
├── Campanha[] (Campaign)
│   ├── id
│   ├── negocio_id
│   ├── nome (ex: "Corte Fidelidade")
│   ├── descricao / regra (ex: "10 cortes → 1 grátis")
│   ├── checkins_necessarios
│   ├── ativa (bool)
│   ├── criada_em
│   ├── wallet_class_id (Google Wallet Loyalty Class ID)
│   │
│   └── Participacao[] (Enrollment)
│       ├── id
│       ├── campanha_id
│       ├── cliente_id
│       ├── checkins_atuais
│       ├── resgatou (bool)
│       ├── wallet_object_id (Google Wallet Loyalty Object ID)
│       └── criada_em
│
└── Usuario[] (Staff / Atendentes)
    ├── id
    ├── negocio_id
    ├── nome
    └── papel (dono | atendente)

Cliente (Customer)
├── id
├── nome
├── telefone
├── email (opcional)
├── criado_em
│
└── Participacao[] (via campanhas)

CheckinLog
├── id
├── participacao_id
├── registrado_por (staff_id)
├── criado_em
```

---

## Fluxos Principais

### 1. Dono do Negócio — Signup

**Telas:** Cadastro → Confirmação de email → Onboarding wizard

1. Dono acessa o site do Voltei
2. Preenche: nome do negócio, email, senha
3. Confirma o email
4. É direcionado ao onboarding wizard (próximo fluxo)

---

### 2. Dono do Negócio — Criação de Campanha (Onboarding Wizard)

**Telas:** Wizard com steps (1/4, 2/4, 3/4, 4/4)

**Step 1 — Identidade visual:**
- Upload de logo
- Escolha de cores (primária + secundária)
- Preview em tempo real de como o cartão ficará na wallet

**Step 2 — Regra da campanha:**
- Nome da campanha (ex: "Corte Fidelidade")
- Número de check-ins para o prêmio (ex: 10)
- Descrição do prêmio (ex: "1 corte grátis")

**Step 3 — Preview e confirmação:**
- Preview do cartão na wallet (mockup visual)
- Preview do totem com QR code
- Botão "Criar campanha"

**Step 4 — Totem pronto:**
- Exibe o totem gerado (QR code + nome + regra)
- Botão "Imprimir totem" (gera PDF otimizado para impressão)
- Botão "Ir para o painel"

---

### 3. Dono do Negócio — Acompanhamento de Campanha (Painel)

**Telas:** Dashboard principal

**Métricas exibidas:**
- Total de clientes cadastrados na campanha
- Check-ins registrados (hoje / semana / mês)
- Clientes próximos do prêmio (ex: 8/10 ou mais)
- Clientes inativos (sem check-in há 30+ dias)
- Resgates realizados
- Gráfico de check-ins ao longo do tempo (Chart.js)

**Ações disponíveis:**
- Ver lista de clientes (com filtros: ativos, inativos, próximos do prêmio)
- Criar nova campanha
- Editar campanha existente (nome, cores — não altera regra de campanha ativa)
- Reimprimir totem

---

### 4. Dono do Negócio — Registrar Check-in (Escanear cartão)

**Telas:** Tela de scan (acessível via botão destacado no painel)

1. Dono/atendente abre a tela de check-in no celular ou computador
2. Escaneia o QR code do cartão de fidelidade do cliente (exibido na wallet)
3. O sistema exibe: nome do cliente, campanha, progresso atual (ex: "5 de 10")
4. Dono confirma o check-in
5. Sistema atualiza o contador e faz PATCH no Google Wallet / atualiza Apple Wallet
6. Tela de confirmação: "Check-in registrado! Maria agora tem 6 de 10."

**Caso especial — prêmio atingido:**
- Se o check-in completou os 10, exibe: "Maria completou a campanha! Prêmio disponível."
- Dono pode marcar o prêmio como resgatado

---

### 5. Cliente — Signup e Adição do Cartão

**Telas:** Landing page (após scan do QR code do totem)

1. Cliente escaneia o QR code do totem com a câmera do celular
2. Abre a landing page do Voltei com os dados da campanha (nome do negócio, regra, cores)
3. Preenche cadastro rápido: nome e telefone (mínimo possível)
4. Sistema detecta o dispositivo:
   - **Android:** Botão "Adicionar à Carteira Google" → gera JWT → redireciona ao Google Wallet
   - **iOS:** Botão "Adicionar à Carteira Apple" → gera .pkpass → download automático abre o Wallet
5. Cliente salva o cartão na wallet
6. Tela de confirmação: "Pronto! Seu cartão de fidelidade está na sua carteira."

**Visitas seguintes:**
- Se o cliente escanear o QR code do totem novamente e já estiver cadastrado (identificado pelo telefone), é redirecionado diretamente para ver seu progresso — sem cadastro repetido.

---

## Integrações com Wallet

### Google Wallet
- **Criação de campanha** → Cria Loyalty Class via REST API
- **Cadastro de cliente** → Cria Loyalty Object via JWT (save URL)
- **Check-in** → PATCH no Loyalty Object (atualiza loyaltyPoints.balance)
- **Cartão atualiza automaticamente** no celular do cliente

### Apple Wallet
- **Criação de campanha** → Define template do .pkpass
- **Cadastro de cliente** → Gera .pkpass assinado com certificado do Voltei
- **Check-in** → Push notification (APNs) → dispositivo busca .pkpass atualizado no servidor
- **Requer:** Apple Developer Account ($99/ano), certificado de Pass Type ID

### Issuer único
- O Voltei é o único emissor (Issuer) de passes em ambas as plataformas
- Cada negócio = uma Loyalty Class (Google) / Pass Type (Apple)
- Cada cliente por campanha = um Loyalty Object (Google) / .pkpass (Apple)

---

## Fora do Escopo do MVP

- Push notifications para clientes (ex: "faltam 2 check-ins!")
- Múltiplos atendentes por negócio (MVP: apenas o dono)
- Plano Negócio / marca branca
- Marketplace de descoberta de negócios
- Integrações com WhatsApp
- Cobrança / billing (Stripe, etc.) — MVP será gratuito para os primeiros clientes
- Relatórios exportáveis (PDF, CSV)
- Multi-idioma

---

## Segurança

- Autenticação do dono: email + senha (hash bcrypt), JWT para sessão
- O QR code do cartão de fidelidade deve conter um token único e não previsível (UUID v4 ou similar), não dados pessoais do cliente
- Credenciais do Google/Apple Wallet armazenadas em variáveis de ambiente, nunca no código
- HTTPS obrigatório em produção
- Rate limiting nas APIs públicas (especialmente no endpoint de criação de pass)
