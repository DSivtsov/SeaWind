import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import path from "path";

// Адрес backend
const target = process.env.API_URL || 'http://localhost:5000';

export default defineConfig({
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
      }
    }
  }
});
