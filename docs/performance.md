# Teste de desempenho

O teste atual usa [k6](https://grafana.com/docs/k6/latest/) para verificar concorrência ao reservar um mesmo slot: dois usuários virtuais enviam simultaneamente a mesma reserva. O resultado esperado é uma resposta de sucesso e uma resposta `409 Conflict`, sem outras respostas.

Esse é um teste focado na disputa concorrente, não um benchmark de throughput ou latência. Ele executa duas iterações (uma por usuário virtual), com duração máxima de 30 segundos, e valida as contagens por thresholds do k6.

## Pré-requisitos

- Docker Compose local iniciado, incluindo MongoDB, Redis e Keycloak (instruções em [infrastructure.md](infrastructure.md)).
- API rodando em `Development` em `http://localhost:5058`.
- k6 instalado na máquina. Node.js e Yarn não são necessários para executar o arquivo diretamente.
- Um slot de calendário ativo, sem reservas conflitantes, com intervalo que contenha o período solicitado pelo teste.

O script usa por padrão o calendário `6ac3c4740608853bc9c535b0` e o intervalo `2026-09-07T16:00:00Z`–`2026-09-07T18:00:00Z`. Atualize esses valores em `tests/performance/index.js` para corresponder a um slot existente no seu banco local. O ID pode ser consultado pelo endpoint de recorrências do calendário, documentado em [api.md](api.md).

## Executar

Com a API, o Keycloak e os serviços do Compose iniciados:

```bash
cd tests/performance
k6 run \
  -e AUTH_BASE_URL=http://localhost:7080 \
  -e API_BASE_URL=http://localhost:5058 \
  index.js
```

O cenário autentica como o usuário local `admin` / `admin123` para obter um token no Keycloak e então envia as duas reservas concorrentes. O código do teste está em `index.js`; a obtenção do token fica em `utils/authUtils.js`.

## Resultado esperado

Ao final, o k6 deve indicar que os thresholds passaram:

- `successful_reservations`: exatamente `1`.
- `conflict_responses`: exatamente `1`.
- `unexpected_responses`: `0`.

Se houver falha de autenticação, verifique se o realm local `coworking-booking` foi importado e se o Keycloak está acessível. Se aparecer `404`, `400` ou outra resposta inesperada da API, confira o ID do calendário e se o intervalo solicitado é válido e ainda está disponível.
