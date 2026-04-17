# Ticket 08 — Frontend: Scanner com API Real

**Sprint:** 4 — Frontend
**Prioridade:** Alta
**Depende de:** Ticket 03

## Descrição

Substituir os dados mock do ScannerPage por chamadas reais à API de check-in.

## Tarefas

- [ ] Remover `MockEnrollment`, `MOCK_ENROLLMENTS`, `getMockEnrollment()` do ScannerPage.tsx
- [ ] Criar tipo `CheckinResponse` (clienteNome, campanhaNome, campanhaDescricao, checkinsAtuais, checkinsNecessarios, rewardReached)
- [ ] Atualizar `handleCodeScanned(code)`:
  - Extrair token do QR (se for URL, extrair a parte final; se for token direto, usar direto)
  - Chamar `POST /api/checkins` com `{ enrollmentToken: token }`
  - Usar resposta real para popular a tela de confirmação
- [ ] Atualizar `handleConfirmCheckin()`:
  - Remover `setTimeout` mock
  - O check-in já foi feito no `handleCodeScanned` — ou mover a chamada API para cá (confirmar primeiro, depois chamar API)
  - Decisão: melhor UX é escanear → mostrar dados do cliente → confirmar → chamar API → mostrar sucesso
- [ ] Atualizar botão "Marcar como resgatado" → chamar `POST /api/checkins/{enrollmentId}/redeem`
- [ ] Tratamento de erros: token inválido, enrollment já resgatado, erro de rede

## Arquivos

| Ação | Arquivo |
|------|---------|
| Modificar | `client/src/pages/scanner/ScannerPage.tsx` |

## Critérios de Aceite

- Escanear QR válido → mostra dados reais do cliente (nome, progresso)
- Confirmar check-in → incrementa contador via API
- Prêmio atingido → exibe mensagem e botão de resgate
- Botão "Marcar como resgatado" funciona
- Token inválido → mensagem de erro amigável
- Zero dados mock restantes no código
