import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import path from "path";

// Адрес для Vite Proxy при запросах к local backend (API/CDN)
const target = 'http://localhost:5000';

export default defineConfig({
  build: {
    emptyOutDir: true,
    chunkSizeWarningLimit: 800, // или 1000
  },

  plugins: [react()],

  resolve: {
    alias: {
      "@": path.resolve(__dirname, "src"),
    },
  },

  server: {
    warmup: {
      clientFiles: [
        "./src/main.tsx",
        "./src/App.tsx",
        "./src/AppRoutes.tsx",
        "./src/common/app.css",
        "./src/pages/landing/landing.css"
      ]
    },
    proxy: {
      '/api': {
        target,
        changeOrigin: true,
        secure: false
      },
      "/contentExercises": {
        target,
        changeOrigin: true,
        secure: false,
      },
      "/chatsExercise-attachments": {
        target,
        changeOrigin: true,
        secure: false,
      }
    }
  }
});
