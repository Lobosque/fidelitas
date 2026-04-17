# Ticket 03 — Endpoints de Check-in

**Sprint:** 1 — Fundação
**Prioridade:** Alta
**Depende de:** Ticket 01, 02

## Descrição

Criar o controller autenticado para registrar check-ins e resgatar prêmios. O dono do negócio escaneia o QR do cartão do cliente, confirma, e o sistema incrementa o contador.

## Tarefas

- [ ] Criar `Dto/CheckinRequest.cs` (EnrollmentToken [Required])
- [ ] Criar `Dto/CheckinResponse.cs` (ClienteNome, CampanhaNome, CampanhaDescricao, CheckinsAtuais, CheckinsNecessarios, RewardReached)
- [ ] Criar `Services/CheckinService.cs` para orquestrar:
  - Buscar Enrollment pelo Token
  - Validar que a campanha está ativa e enrollment não resgatado
  - Incrementar CheckinsAtuais
  - Criar CheckinLog com RegistradoPor = userId autenticado
  - (Chamadas de wallet update serão adicionadas nos Tickets 04 e 06)
  - Retornar CheckinResponse
- [ ] Criar `Controllers/CheckinController.cs`:
  - `POST /api/checkins` — registrar check-in (body: EnrollmentToken)
  - `POST /api/checkins/{enrollmentId}/redeem` — marcar prêmio como resgatado
- [ ] Registrar CheckinService no DI (Program.cs)

## Arquivos

| Ação | Arquivo |
|------|---------|
| Criar | `server/Voltei.Api/Dto/CheckinRequest.cs` |
| Criar | `server/Voltei.Api/Dto/CheckinResponse.cs` |
| Criar | `server/Voltei.Api/Services/CheckinService.cs` |
| Criar | `server/Voltei.Api/Controllers/CheckinController.cs` |
| Modificar | `server/Voltei.Api/Program.cs` |

## Critérios de Aceite

- `POST /api/checkins` com token válido incrementa contador e retorna dados atualizados
- Check-in no último ponto retorna `RewardReached=true`
- Tentar check-in em enrollment já resgatado retorna erro 400
- `POST /api/checkins/{id}/redeem` marca `Resgatou=true`
- Cada check-in cria um CheckinLog com staff e timestamp
- Endpoints exigem JWT auth (401 sem token)
