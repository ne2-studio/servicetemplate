# Service Template

Starting point for new `exeal/ne2-studio` projects: a monorepo with a `frontend/` and `backend/`
scaffolded to match [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md).

**Status: work in progress.** The pieces here were pulled together from prior projects and don't
all line up yet — expect naming inconsistencies (e.g. deploy workflows still reference a source
project's image names instead of `{ProjectName}`), and gaps between what `docs/ARCHITECTURE.md`
prescribes and what's actually scaffolded. Treat this as a reference to assemble a new project
from, not a turnkey `git clone` — check each piece against the architecture doc as you copy it
over, rather than assuming it's already correct.

## What's here

| Directory | Contents |
|-----------|----------|
| `docs/ARCHITECTURE.md` | The architecture standard both services should follow |
| `backend/` | ASP.NET Core (.NET 10) ports & adapters scaffold, with a `Widget` CRUD slice as a working reference implementation — see [`backend/README.md`](backend/README.md) |
| `frontend/` | React 19 + Vite + Zustand scaffold, with an `Item` domain as a working reference implementation — see [`frontend/README.md`](frontend/README.md) |
| `.github/workflows/` | Path-filtered CI/CD for each service (build → test → Docker image → registry → deploy webhook) |

The `Widget`/`Item` example resources exist purely to exercise every layer/convention end-to-end
so you have something to read and adapt, not to demonstrate a feature. Replace them with the new
project's actual domain — see each service's README for the specific steps.

## Using this template for a new project

1. Copy `backend/` and `frontend/` into the new project's repo.
2. Follow the "How to use this template" steps in [`backend/README.md`](backend/README.md) and
   [`frontend/README.md`](frontend/README.md) to rename `ServiceTemplate`/`{ProjectName}` throughout
   and swap in the real domain.
3. Update `.github/workflows/*.yml`: image names under `env.IMAGE_NAME` and the Coolify webhook
   secrets need to point at the new project, not whatever they were copied from.
4. Update this file and `docs/ARCHITECTURE.md` if the new project deviates from the standard
   layout — the architecture doc should stay authoritative for the project going forward.

## Architecture

Two independently deployable services, no shared code between them:

| Directory | Stack |
|-----------|-------|
| `frontend/` | React 19, TypeScript, Vite, Tailwind CSS v4, Zustand, react-oidc-context |
| `backend/` | ASP.NET Core (.NET 10), PostgreSQL, Serilog |

Auth is OIDC/JWT Bearer end-to-end: the frontend authenticates against an external OIDC provider
and attaches the access token to every API call; the backend validates it via `JwtBearer`
middleware. Full conventions and rationale are in [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md).

## Getting started

### Prerequisites

- Node.js 22+
- .NET 10 SDK
- Docker (for PostgreSQL locally, and for building images)

### Backend

```bash
cd backend
dotnet restore
dotnet run --project ServiceTemplate.Api
```

See [`backend/README.md`](backend/README.md) for running PostgreSQL locally, environment
configuration, tests, and Docker.

### Frontend

```bash
cd frontend
cp .env.example .env
# Set VITE_API_URL to the backend URL, and the OIDC authority/client_id in src/main.tsx

npm install
npm run dev
```

See [`frontend/README.md`](frontend/README.md) for details.

## Deployment

Both services are containerized and deploy independently. CI/CD runs on push to `main`
(path-filtered per service), builds a Docker image, pushes it to GitHub Container Registry, and
triggers a Coolify deploy webhook — see `.github/workflows/backend-deploy.yml` and
`frontend-deploy.yml`. **These still need the image names and webhook secrets updated per new
project** (see "Using this template for a new project" above).

## License

MIT © [Exeal](https://www.exeal.com)
