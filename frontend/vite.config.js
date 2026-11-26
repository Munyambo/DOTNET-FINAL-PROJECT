import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
  plugins: [react()],
  server: {
    // Any call to /api/... is forwarded to our C# API
    proxy: {
      "/api": "http://localhost:5103",
    },
  },
});