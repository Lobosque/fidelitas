# Ticket 04 — Google Wallet

**Sprint:** 2 — Google Wallet
**Prioridade:** Alta
**Depende de:** Tickets 01, 02, 03

## Descrição

Implementar integração com Google Wallet para criar loyalty classes (por campanha) e loyalty objects (por enrollment), além de atualizar o saldo de check-ins via PATCH.

## Tarefas

### Configuração
- [ ] Adicionar NuGet packages: `Google.Apis.Walletobjects.v1`, `Google.Apis.Auth`
- [ ] Criar `Configuration/GoogleWalletOptions.cs` (IssuerId, ServiceAccountKeyPath, ApplicationName)
- [ ] Adicionar seção `GoogleWallet` no `appsettings.json` e `appsettings.Development.json`
- [ ] Registrar `GoogleWalletOptions` e `GoogleWalletService` no DI (Program.cs)

### Service
- [ ] Criar `Services/Wallet/GoogleWalletService.cs`:
  - `CreateLoyaltyClassAsync(campaign, business)`:
    - REST API → cria LoyaltyClass
    - Class ID: `{issuerId}.voltei-{campaignId}`
    - Configura: programName, programLogo (logo do negócio), hexBackgroundColor (cor primária), issuerName
    - Retorna classId para salvar em Campaign.WalletClassId
  - `CreateLoyaltyObjectAsync(enrollment, campaign, business, customer)`:
    - Cria payload do LoyaltyObject com: objectId `{issuerId}.voltei-{enrollmentId}`, classId, loyaltyPoints (balance=0), barcode (QR com enrollment.Token), accountName (cliente.Nome), accountId (enrollment.Token)
    - Assina JWT com chave privada do service account (RS256)
    - Retorna save URL: `https://pay.google.com/gp/v/save/{jwt}`
  - `UpdateLoyaltyObjectAsync(enrollment, campaign)`:
    - PATCH no loyalty object
    - Atualiza `loyaltyPoints.balance` para `{checkinsAtuais}`

### Integração
- [ ] Atualizar `CampaignsController.cs` — ao criar campanha, chamar `CreateLoyaltyClassAsync` e salvar WalletClassId
- [ ] Atualizar `EnrollmentService.cs` — ao inscrever cliente, chamar `CreateLoyaltyObjectAsync` e incluir saveUrl no EnrollResponse
- [ ] Atualizar `CheckinService.cs` — ao registrar check-in, chamar `UpdateLoyaltyObjectAsync`

## Arquivos

| Ação | Arquivo |
|------|---------|
| Criar | `server/Voltei.Api/Configuration/GoogleWalletOptions.cs` |
| Criar | `server/Voltei.Api/Services/Wallet/GoogleWalletService.cs` |
| Modificar | `server/Voltei.Api/Voltei.Api.csproj` (NuGet packages) |
| Modificar | `server/Voltei.Api/appsettings.json` |
| Modificar | `server/Voltei.Api/Program.cs` |
| Modificar | `server/Voltei.Api/Controllers/CampaignsController.cs` |
| Modificar | `server/Voltei.Api/Services/EnrollmentService.cs` |
| Modificar | `server/Voltei.Api/Services/CheckinService.cs` |

## Critérios de Aceite

- Criar campanha → Loyalty Class criada no Google Wallet (modo demo)
- Inscrever cliente → save URL funcional retornada
- Registrar check-in → balance atualizado no cartão
- Se credenciais Google não configuradas, log warning mas não quebra o fluxo
