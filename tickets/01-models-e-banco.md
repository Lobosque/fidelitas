# Ticket 01 — Novos Models e Banco de Dados

**Sprint:** 1 — Fundação
**Prioridade:** Alta (bloqueante para todos os outros tickets)

## Descrição

Criar os models que faltam para suportar o fluxo de fidelidade: Customer, Enrollment, CheckinLog e AppleDeviceRegistration. Atualizar o Campaign existente e configurar o AppDbContext com os novos relacionamentos. Migrar de `EnsureCreated()` para EF Core Migrations.

## Tarefas

- [ ] Criar `Models/Customer.cs` (Id, Nome, Telefone, Email?, CriadoEm)
- [ ] Criar `Models/Enrollment.cs` (Id, CampanhaId, ClienteId, CheckinsAtuais, Resgatou, Token, WalletObjectId?, ApplePassSerial?, ApplePassAuthToken?, ApplePushToken?, CriadaEm)
- [ ] Criar `Models/CheckinLog.cs` (Id, ParticipacaoId, RegistradoPor, CriadoEm)
- [ ] Criar `Models/AppleDeviceRegistration.cs` (Id, DeviceLibraryIdentifier, PushToken, PassTypeIdentifier, SerialNumber, CriadoEm)
- [ ] Atualizar `Models/Campaign.cs` — adicionar `ApplePassTypeId` (string?) e nav prop `ICollection<Enrollment>`
- [ ] Atualizar `Data/AppDbContext.cs`:
  - Novos DbSets: Customers, Enrollments, CheckinLogs, AppleDeviceRegistrations
  - Index único em Customer.Telefone
  - Index composto único em Enrollment (CampanhaId + ClienteId)
  - FKs: Enrollment→Campaign, Enrollment→Customer, CheckinLog→Enrollment, CheckinLog→User
- [ ] Trocar `db.Database.EnsureCreated()` por `db.Database.Migrate()` em Program.cs
- [ ] Rodar `dotnet ef migrations add AddWalletEntities`
- [ ] `dotnet build` e `dotnet test` passando

## Arquivos

| Ação | Arquivo |
|------|---------|
| Criar | `server/Voltei.Api/Models/Customer.cs` |
| Criar | `server/Voltei.Api/Models/Enrollment.cs` |
| Criar | `server/Voltei.Api/Models/CheckinLog.cs` |
| Criar | `server/Voltei.Api/Models/AppleDeviceRegistration.cs` |
| Modificar | `server/Voltei.Api/Models/Campaign.cs` |
| Modificar | `server/Voltei.Api/Data/AppDbContext.cs` |
| Modificar | `server/Voltei.Api/Program.cs` |

## Critérios de Aceite

- `dotnet build` compila sem erros
- `dotnet test` — testes existentes continuam passando
- Migration gerada e aplicável
- Banco cria as tabelas corretamente com os índices e FKs
