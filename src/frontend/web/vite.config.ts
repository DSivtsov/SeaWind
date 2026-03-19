import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import path from "path";

// Адрес backend
const target = process.env.API_URL || 'http://localhost:5000';

export default defineConfig({
  build: {
    chunkSizeWarningLimit: 800, // или 1000
  },

  plugins: [react()],

  resolve: {
    alias: {
      "@": path.resolve(__dirname, "src"),
    },
  },

  server: {
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
