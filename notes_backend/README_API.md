# Notes Backend API

- Swagger docs: /docs
- OpenAPI JSON: /openapi/v1.json
- Health: GET /

Auth:
- POST /api/auth/register { email, displayName, password }
- POST /api/auth/login { email, password }

Notes (requires Bearer token):
- GET /api/notes?page=1&pageSize=20
- GET /api/notes/{id}
- POST /api/notes { title, content }
- PUT /api/notes/{id} { title?, content? }
- DELETE /api/notes/{id}

Environment:
- JWT_SECRET (required in production)
- DB_CONNECTION_STRING (optional; uses in-memory DB if not set). Supports Postgres (Npgsql) or SQL Server based on the string.
