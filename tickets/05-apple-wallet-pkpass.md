# Ticket 05 — Apple Wallet (.pkpass)

**Sprint:** 3 — Apple Wallet
**Prioridade:** Alta
**Depende de:** Tickets 01, 02, 03

## Descrição

Implementar geração de .pkpass (Apple Wallet) usando `System.Security.Cryptography.Pkcs` built-in do .NET. Sem pacote terceiro. Para desenvolvimento, usar certificado auto-assinado — quando tiver Apple Developer Account, basta trocar o .p12 na config.

## Tarefas

### Configuração
- [ ] Criar `Configuration/AppleWalletOptions.cs` (PassTypeIdentifier, TeamIdentifier, CertificatePath, CertificatePassword, AppleCACertPath, WebServiceUrl)
- [ ] Adicionar seção `AppleWallet` no `appsettings.json`
- [ ] Criar script `server/certs/generate-dev-certs.sh` para gerar certificados auto-assinados via openssl
- [ ] Registrar `AppleWalletOptions` e `AppleWalletService` no DI

### Service
- [ ] Criar `Services/Wallet/AppleWalletService.cs`:
  - `GeneratePassAsync(enrollment, campaign, business, customer)`:
    1. Montar `pass.json` (tipo storeCard):
       - `passTypeIdentifier`, `teamIdentifier`, `serialNumber` (UUID)
       - `authenticationToken` (hex 32 chars aleatório)
       - `webServiceURL` (para atualizações)
       - `organizationName` (nome do negócio)
       - `barcode` / `barcodes` (QR com enrollment.Token)
       - `foregroundColor`, `backgroundColor` (cores do negócio)
       - `headerFields`: [{key: "balance", label: "Check-ins", value: "0/N"}]
       - `primaryFields`: [{key: "name", label: "Programa", value: campanha.Nome}]
       - `secondaryFields`: [{key: "reward", label: "Prêmio", value: campanha.Descricao}]
    2. Incluir imagens placeholder: `icon.png`, `icon@2x.png`, `logo.png`, `logo@2x.png`
    3. Gerar `manifest.json` — SHA256 hash de cada arquivo
    4. Assinar manifest com CMS/PKCS#7 (SignedCms detached signature)
    5. Empacotar tudo em ZIP → retornar bytes com content-type `application/vnd.apple.pkpass`
  - `UpdatePassAsync(enrollment, campaign, business, customer)`:
    - Gerar .pkpass atualizado (novo balance)
    - Enviar push APNs vazio para `enrollment.ApplePushToken` (device busca .pkpass novo)

### Integração
- [ ] Atualizar `EnrollmentService.cs` — ao inscrever, gerar .pkpass e salvar ApplePassSerial + ApplePassAuthToken no Enrollment, incluir URL de download no EnrollResponse
- [ ] Atualizar `CheckinService.cs` — ao registrar check-in, chamar `UpdatePassAsync`
- [ ] Atualizar endpoint `GET /api/enroll/{enrollmentId}/pass.pkpass` no EnrollController para servir o .pkpass gerado

## Arquivos

| Ação | Arquivo |
|------|---------|
| Criar | `server/Voltei.Api/Configuration/AppleWalletOptions.cs` |
| Criar | `server/Voltei.Api/Services/Wallet/AppleWalletService.cs` |
| Criar | `server/certs/generate-dev-certs.sh` |
| Modificar | `server/Voltei.Api/appsettings.json` |
| Modificar | `server/Voltei.Api/Program.cs` |
| Modificar | `server/Voltei.Api/Services/EnrollmentService.cs` |
| Modificar | `server/Voltei.Api/Services/CheckinService.cs` |
| Modificar | `server/Voltei.Api/Controllers/EnrollController.cs` |

## Critérios de Aceite

- .pkpass gerado é um ZIP válido com pass.json, manifest.json, signature, e imagens
- Hashes no manifest.json batem com os arquivos incluídos
- pass.json segue o schema da Apple (passTypeIdentifier, serialNumber, etc.)
- Barcode no pass contém o Token do enrollment
- Download do .pkpass funciona via endpoint
- Com certificado auto-assinado: arquivo gera mas iOS rejeita (esperado)
- Com certificado Apple real (futuro): trocar .p12 e funciona sem mudança de código
