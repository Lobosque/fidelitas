# Ticket 02 — DTOs e Endpoints de Enrollment

**Sprint:** 1 — Fundação
**Prioridade:** Alta
**Depende de:** Ticket 01

## Descrição

Criar os DTOs e o controller público para o fluxo de inscrição do cliente na campanha. Esses endpoints são acessados pela landing page (sem autenticação).

## Tarefas

- [ ] Criar `Dto/EnrollRequest.cs` (Nome [Required], Telefone [Required])
- [ ] Criar `Dto/EnrollResponse.cs` (EnrollmentId, GoogleWalletSaveUrl?, ApplePassUrl?, CheckinsAtuais, CheckinsNecessarios, AlreadyEnrolled)
- [ ] Criar `Dto/CampaignPublicResponse.cs` (Id, Nome, Descricao, CheckinsNecessarios, NegocioNome, LogoUrl?, CoresPrimaria, CoresSecundaria)
- [ ] Criar `Controllers/EnrollController.cs` com os endpoints:
  - `GET /api/enroll/{campaignId}` — retorna info pública da campanha
  - `POST /api/enroll/{campaignId}` — cadastra cliente (nome + telefone), cria Enrollment com Token (UUID), retorna EnrollResponse
  - `GET /api/enroll/{campaignId}/status?telefone=` — checa se telefone já está inscrito, retorna progresso
  - `GET /api/enroll/{enrollmentId}/pass.pkpass` — download do .pkpass (placeholder por enquanto, implementar de verdade no Ticket 06)
- [ ] Criar `Services/EnrollmentService.cs` para orquestrar:
  - Find or create Customer por telefone
  - Checar enrollment existente
  - Criar novo Enrollment com Token aleatório
  - (Chamadas de wallet serão adicionadas nos Tickets 04 e 06)

## Arquivos

| Ação | Arquivo |
|------|---------|
| Criar | `server/Voltei.Api/Dto/EnrollRequest.cs` |
| Criar | `server/Voltei.Api/Dto/EnrollResponse.cs` |
| Criar | `server/Voltei.Api/Dto/CampaignPublicResponse.cs` |
| Criar | `server/Voltei.Api/Controllers/EnrollController.cs` |
| Criar | `server/Voltei.Api/Services/EnrollmentService.cs` |
| Modificar | `server/Voltei.Api/Program.cs` (registrar EnrollmentService no DI) |

## Critérios de Aceite

- `GET /api/enroll/{id}` retorna dados da campanha com nome do negócio e cores
- `POST /api/enroll/{id}` cria customer + enrollment e retorna EnrollResponse
- Telefone duplicado na mesma campanha retorna enrollment existente (AlreadyEnrolled=true)
- `GET /api/enroll/{id}/status?telefone=` retorna progresso ou 404
- `dotnet build` e `dotnet test` passando
