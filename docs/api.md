# API

Base URL local: `http://localhost:5058`. Em ambiente `Development`, a documentação interativa (Scalar) está em `/docs` e o schema OpenAPI em `/openapi/v1.json`.

## Convenções

### Formato de resposta

Todas as respostas dos endpoints de negócio seguem o envelope `ApiResponse<T>`:

```json
{
  "success": true,
  "data": { },
  "errors": []
}
```

Em caso de erro, `data` é omitido e `errors` traz a lista de problemas:

```json
{
  "success": false,
  "errors": [
    { "message": "Workspace not found.", "type": "notFound" }
  ]
}
```

| `type` | HTTP |
|---|---|
| `validationError` | 400 |
| `forbidden` | 403 |
| `notFound` | 404 |
| `conflict` | 409 |
| `internalServerError` | 500 |

Indisponibilidade do Redis retorna **503** com `Retry-After: 10`.

### Serialização

- Propriedades em camelCase; campos nulos são omitidos.
- Enums trafegam como strings camelCase (`available`, `daily`, `monday`). Na entrada, a comparação ignora maiúsculas/minúsculas.
- `type` (WorkspaceType) aceita o nome (`"MeetingRoom"`, `"meeting-room"`, `"meeting room"`), ignorando caixa e pontuação, ou o código numérico (`3`). Na saída é sempre o nome (`"MeetingRoom"`).
- Datas são strings ISO 8601.
- Coordenadas usam `{ "lat": number, "lgn": number }`.

Valores possíveis dos enums em [domain.md](domain.md#enums).

## Endpoints

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/workspace` | Cria um workspace |
| `GET` | `/api/workspace/{slug}` | Detalhes de um workspace |
| `PATCH` | `/api/workspace/{slug}/availability` | Define a disponibilidade recorrente |
| `PATCH` | `/api/workspace/{slug}/status` | Altera o status |
| `POST` | `/api/workspaceCalendar/{calendarId}/booking` | Cria uma reserva em um slot de calendário |
| `GET` | `/health/live` | Liveness (sem dependências) |
| `GET` | `/health/ready` | Readiness (verifica MongoDB e Redis) |
| `GET` | `/jobs` | Dashboard do Hangfire |

---

### `POST /api/workspace`

Cria um workspace com status `draft`. O slug é gerado a partir do nome; nome que gere slug já existente retorna `409`.

**Request**

```json
{
  "name": "Sala Reunião Paulista",
  "description": "Sala para até 8 pessoas com TV e quadro branco",
  "type": "MeetingRoom",
  "coordinates": { "lat": -23.561, "lgn": -46.656 },
  "pricePerHour": 80,
  "resources": ["tv", "whiteboard", "wifi"]
}
```

**Response `201 Created`** (header `Location` aponta para `GET /api/workspace/{slug}`)

```json
{
  "success": true,
  "data": {
    "slug": "sala-reuniao-paulista",
    "name": "Sala Reunião Paulista",
    "description": "Sala para até 8 pessoas com TV e quadro branco",
    "status": "draft",
    "type": "MeetingRoom",
    "coordinates": { "lat": -23.561, "lgn": -46.656 },
    "pricePerHour": 80,
    "resources": ["tv", "whiteboard", "wifi"]
  },
  "errors": []
}
```

---

### `GET /api/workspace/{slug}`

Retorna um workspace ativo. A leitura passa pelo cache Redis (`workspace:{slug}`, TTL 1h).

**Response `200 OK`**

```json
{
  "success": true,
  "data": {
    "name": "Sala Reunião Paulista",
    "description": "Sala para até 8 pessoas com TV e quadro branco",
    "slug": "sala-reuniao-paulista",
    "status": "available",
    "type": "MeetingRoom",
    "coordinates": { "lat": -23.561, "lgn": -46.656 },
    "pricePerHour": 80,
    "isInactive": false,
    "resources": ["tv", "whiteboard", "wifi"],
    "createdAt": "2026-10-01T12:00:00Z",
    "updatedAt": "2026-10-01T12:30:00Z"
  },
  "errors": []
}
```

Erros: `404` se o slug não existir ou o workspace estiver inativo.

---

### `PATCH /api/workspace/{slug}/availability`

Define a janela de disponibilidade e sua recorrência. O workspace passa para `available`, o cache é invalidado e a geração dos slots de calendário é enviada para a fila `workspace-availability` (processada pelo Worker).

| Campo | Tipo | Descrição |
|---|---|---|
| `startAt` | string ISO 8601 | Início da primeira ocorrência |
| `endAt` | string ISO 8601 | Fim da primeira ocorrência |
| `until` | string ISO 8601 | Data limite da recorrência |
| `frequency` | `daily` \| `weekly` | Frequência |
| `byDay` | `DayOfWeek[]` | Obrigatório em `weekly`; deve ser omitido em `daily` |
| `byMonth` | `int[]` | Deve ser omitido em `daily` |
| `timezone` | string | Timezone IANA usado no cálculo das ocorrências |

**Request (semanal)**

```json
{
  "startAt": "2026-10-05T09:00:00",
  "endAt": "2026-10-05T18:00:00",
  "until": "2026-12-31T23:59:59",
  "frequency": "weekly",
  "byDay": ["monday", "wednesday", "friday"],
  "timezone": "America/Sao_Paulo"
}
```

**Response `200 OK`**: ecoa a disponibilidade salva, no mesmo formato da requisição.

Erros: `400` em validação, se já existem slots cobrindo o mesmo período ou se algum slot afetado já tem reserva; `404` se o workspace não existir.

---

### `PATCH /api/workspace/{slug}/status`

**Request**

```json
{ "status": "maintenance" }
```

**Response `200 OK`**: mesmo formato de `GET /api/workspace/{slug}`, com o status atualizado.

Erros: `400` se a transição for inválida (por exemplo, `available` sem disponibilidade definida); `404` se o workspace não existir.

---

### `POST /api/workspaceCalendar/{calendarId}/booking`

Cria uma reserva dentro de um slot de calendário. O intervalo deve estar dentro do slot e não pode sobrepor outra reserva. Se o slot ficar totalmente ocupado, ele é marcado como cheio.

**Request**

```json
{
  "startAt": "2026-10-05T12:00:00Z",
  "endAt": "2026-10-05T14:00:00Z"
}
```

**Response `201 Created`**

```json
{
  "success": true,
  "data": {
    "startAt": "2026-10-05T12:00:00Z",
    "endAt": "2026-10-05T14:00:00Z",
    "totalPrice": 160
  },
  "errors": []
}
```

`totalPrice` = horas reservadas × `pricePerHour` do workspace.

Erros: `400` em validação, sobreposição de reservas ou slot cheio; `404` se o slot, o workspace ou a disponibilidade do workspace não existir.
