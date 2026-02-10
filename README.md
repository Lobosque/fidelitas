# Voltei — Prova de Conceito

Sistema de fidelidade digital sem aplicativo. Esta PoC valida o fluxo: **QR code → cartão na carteira digital do celular (Google Wallet)**.

---

## Pré-requisitos

- Node.js 18+
- Conta Google Cloud (gratuita)
- Celular Android com Google Wallet (para teste)

---

## Setup do Google Cloud

### 1. Criar projeto

1. Acesse [Google Cloud Console](https://console.cloud.google.com)
2. Crie um novo projeto (ex: "voltei-poc")

### 2. Ativar a Google Wallet API

1. No projeto criado, vá em **APIs & Services → Library**
2. Busque por **"Google Wallet API"**
3. Clique em **Enable**

### 3. Criar Service Account

1. Vá em **IAM & Admin → Service Accounts**
2. Clique em **Create Service Account**
3. Dê um nome (ex: "voltei-wallet")
4. Clique em **Done**
5. Clique na service account criada → aba **Keys**
6. **Add Key → Create new key → JSON**
7. Salve o arquivo JSON baixado (contém as credenciais)

### 4. Criar Issuer Account (Google Wallet)

1. Acesse [Google Pay & Wallet Console](https://pay.google.com/business/console)
2. Crie uma conta de emissor (Issuer)
3. Anote o **Issuer ID** (número exibido no console)
4. Em **Users**, adicione o email da Service Account criada no passo anterior
   - O email está no arquivo JSON baixado, campo `client_email`
   - Dê permissão de **Developer** ou **Admin**

### 5. Configurar variáveis de ambiente

Copie o arquivo de exemplo e preencha com seus dados:

```bash
cp server/.env.example server/.env
```

Edite `server/.env` com os dados do arquivo JSON da Service Account:

```env
GOOGLE_SERVICE_ACCOUNT_EMAIL=<client_email do JSON>
GOOGLE_SERVICE_ACCOUNT_PRIVATE_KEY=<private_key do JSON>
GOOGLE_WALLET_ISSUER_ID=<Issuer ID do Google Wallet Console>
PORT=3001
BASE_URL=http://localhost:5173
```

> **Dica:** A `private_key` no JSON vem com `\n` literal. Cole a string inteira entre aspas duplas no `.env`.

---

## Rodar o projeto

Em dois terminais separados:

**Terminal 1 — Backend:**
```bash
cd server
npm install
npm run dev
```

**Terminal 2 — Frontend:**
```bash
cd client
npm install
npm run dev
```

O frontend estará em `http://localhost:5173`.

---

## Testar no celular

Para testar o QR code com o celular, o frontend precisa estar acessível na rede local. Opções:

### Opção A: Mesma rede Wi-Fi
```bash
cd client
npx vite --host
```
Acesse pelo IP da máquina (ex: `http://192.168.1.100:5173`).

### Opção B: Túnel (ngrok, localtunnel)
```bash
npx localtunnel --port 5173
```

---

## Estrutura do projeto

```
fidelitas/
├── client/                  # React (Vite)
│   └── src/
│       ├── pages/
│       │   ├── TotemPage.jsx    # Tela do totem (QR code)
│       │   └── PassPage.jsx     # Landing page do cliente
│       ├── App.jsx              # Rotas
│       └── main.jsx
├── server/                  # Express.js
│   ├── index.js                 # Server principal
│   ├── routes/pass.js           # Endpoints da API
│   ├── services/googleWallet.js # Geração do Google Wallet pass
│   └── .env.example
├── PLANO_DE_NEGOCIOS.md
└── README.md
```

---

## Fluxo da PoC

1. Abra `http://localhost:5173` no navegador — verá a tela do **totem** com o QR code
2. Escaneie o QR code com a câmera de um celular Android
3. A **landing page** abrirá mostrando os dados da campanha
4. Clique em **"Adicionar à Carteira Google"**
5. O Google Wallet abrirá com o cartão de fidelidade para salvar

---

## Próximos passos

- [ ] Apple Wallet (requer Apple Developer Account — $99/ano)
- [ ] Cadastro real do cliente
- [ ] Banco de dados
- [ ] Check-in / acúmulo de pontos
- [ ] Painel do dono do negócio
