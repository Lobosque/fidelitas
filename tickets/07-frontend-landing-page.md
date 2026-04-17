# Ticket 07 — Frontend: Landing Page do Cliente

**Sprint:** 4 — Frontend
**Prioridade:** Alta
**Depende de:** Tickets 02, 04, 05

## Descrição

Reescrever a landing page do cliente (acessada via QR do totem) com o fluxo real de inscrição, detecção de dispositivo e botões de wallet.

## Tarefas

- [ ] Adicionar tipos TypeScript em `client/src/types/` ou inline:
  - `CampaignPublicInfo` (id, nome, descricao, checkinsNecessarios, negocioNome, logoUrl, coresPrimaria, coresSecundaria)
  - `EnrollResponse` (enrollmentId, googleWalletSaveUrl, applePassUrl, checkinsAtuais, checkinsNecessarios, alreadyEnrolled)

- [ ] Reescrever `client/src/pages/customer/LandingPage.tsx`:
  1. **Estado inicial**: Fetch `GET /api/enroll/{campaignId}` → exibir branding (logo, cores, nome do negócio, regra)
  2. **Input de telefone**: Campo para digitar telefone → checar `GET /api/enroll/{campaignId}/status?telefone=`
  3. **Se já inscrito**: Mostrar progresso (X de Y check-ins), sem botão de wallet
  4. **Se novo**: Form com nome + telefone → `POST /api/enroll/{campaignId}`
  5. **Detecção de device**:
     ```ts
     const isIOS = /iPad|iPhone|iPod/.test(navigator.userAgent)
     const isAndroid = /Android/.test(navigator.userAgent)
     ```
  6. **Botões de wallet**:
     - Android: "Adicionar ao Google Wallet" → abre `googleWalletSaveUrl` em nova aba
     - iOS: "Adicionar ao Apple Wallet" → download do .pkpass via `applePassUrl`
     - Desktop: mostrar ambos os botões
  7. **Tela de confirmação**: "Pronto! Seu cartão de fidelidade está na sua carteira."

- [ ] Aplicar cores do negócio dinamicamente (primary/secondary via CSS variables ou inline styles)
- [ ] Design mobile-first, responsivo

## Arquivos

| Ação | Arquivo |
|------|---------|
| Modificar | `client/src/pages/customer/LandingPage.tsx` (rewrite completo) |
| Criar/Modificar | `client/src/types/index.ts` ou arquivo dedicado (novos tipos) |

## Critérios de Aceite

- Landing page carrega e exibe branding da campanha (cores, logo, nome)
- Telefone já cadastrado → mostra progresso sem re-inscrição
- Novo telefone → form rápido (nome + telefone) → inscrição
- Botão correto aparece baseado no device (Google para Android, Apple para iOS)
- Design limpo, mobile-first, com cores do negócio
- Funciona sem JS errors no console
