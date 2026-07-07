# {ProjectName} Frontend

Scaffold following the conventions in `docs/ARCHITECTURE.md`. Includes an example `Item` domain (`types.ts` → `api.ts` → `store/useItemStore.ts` → `components/Items.tsx`) to illustrate the layering — replace it with the project's real domain(s).

## Run locally

**Prerequisites:** Node.js

1. Copy `.env.example` to `.env` and set `VITE_API_URL`.
2. Set the real OIDC `authority`/`client_id` in `src/main.tsx`.
3. Install dependencies: `npm install`
4. Run the app: `npm run dev`
