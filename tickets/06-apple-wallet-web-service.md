# Ticket 06 — Apple Wallet Web Service (Atualização de Passes)

**Sprint:** 3 — Apple Wallet
**Prioridade:** Média
**Depende de:** Ticket 05

## Descrição

Implementar os endpoints REST obrigatórios pela Apple para que o dispositivo iOS possa registrar-se para receber atualizações de passes e buscar versões atualizadas.

## Tarefas

- [ ] Criar `Controllers/AppleWalletController.cs` com os 5 endpoints exigidos:

| Método | Rota | Propósito |
|--------|------|-----------|
| POST | `/api/apple-wallet/v1/devices/{deviceId}/registrations/{passTypeId}/{serialNumber}` | Device se registra para push updates |
| DELETE | mesma rota | Device cancela registro |
| GET | `/api/apple-wallet/v1/devices/{deviceId}/registrations/{passTypeId}` | Listar serials de passes atualizados (query: `passesUpdatedSince`) |
| GET | `/api/apple-wallet/v1/passes/{passTypeId}/{serialNumber}` | Retornar .pkpass atualizado |
| POST | `/api/apple-wallet/v1/log` | Receber logs de erro do device (salvar em log do servidor) |

- [ ] Autenticação via header `Authorization: ApplePass {authenticationToken}` — validar contra `Enrollment.ApplePassAuthToken`
- [ ] No POST de registration: salvar `AppleDeviceRegistration` e atualizar `Enrollment.ApplePushToken`
- [ ] No GET de passes: gerar .pkpass fresco via `AppleWalletService.GeneratePassAsync` com dados atuais
- [ ] No GET de registrations: retornar serials de passes modificados desde `passesUpdatedSince`

## Arquivos

| Ação | Arquivo |
|------|---------|
| Criar | `server/Voltei.Api/Controllers/AppleWalletController.cs` |
| Modificar | `server/Voltei.Api/Data/AppDbContext.cs` (se não feito no Ticket 01 — garantir DbSet de AppleDeviceRegistration) |

## Critérios de Aceite

- Endpoints seguem exatamente o contrato da Apple PassKit Web Service Reference
- POST registration salva device e push token
- DELETE registration remove o registro
- GET passes retorna .pkpass com `Last-Modified` header
- GET registrations filtra por `passesUpdatedSince`
- Auth token inválido retorna 401
- Logs recebidos são escritos no log do servidor
