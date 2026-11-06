import { defineConfig, loadEnv } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig(({ mode }) => {
  console.log("CWD:", process.cwd());
  console.log("ENV carregado:", loadEnv(mode, process.cwd(), "VITE_"));

  return {
    plugins: [react()],
  };
});
