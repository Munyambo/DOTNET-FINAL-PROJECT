# Frontend (React + Vite)

->How to run it

1. Open the `frontend` folder in VS Code
2. First time only: `npm install`
3. `npm run dev` → open http://localhost:5173
4. The API must be running too (see backend/README.md)

->Rules

- Pages in `src/pages/`, API calls in `src/services/` (one service per topic)
- Never call `fetch` directly in a page, always use a service