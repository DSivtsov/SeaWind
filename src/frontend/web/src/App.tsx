import { BrowserRouter } from 'react-router-dom';
import { AppRoutes } from "./AppRoutes";

//Mantine Provider & styles
import '@mantine/core/styles.css';
import '@mantine/notifications/styles.css';
import { MantineProvider, createTheme } from '@mantine/core';
import { Notifications } from '@mantine/notifications';
import { AuthProvider } from '@/shared/auth/AuthProvider';
import { BootstrapGuard } from '@/shared/components/BootstrapGuard';

/**
 * Brand color palette (Mantine scale 0–9)
 *
 * Используется Mantine автоматически для состояний компонентов:
 * - base / hover / active / light / subtle
 *
 * ВАЖНО:
 * - основной акцент — brand[5]
 * - остальные значения — его светлые и тёмные производные
 * - индексы вручную НЕ использовать
 *
 * Диапазоны:
 * - 0–1: очень светлые (backgrounds, light / subtle)
 * - 2–4: вторичные, мягкие состояния
 * - 5: основной brand-цвет (filled, active)
 * - 6–7: hover / pressed
 * - 8–9: тёмные края шкалы, редко используются
 */
const theme = createTheme({
  primaryColor: "brand",
  colors: {
    brand: [
      "#E9FFF0", "#CFF9DD",
      "#A3F0BE", "#73E59D", "#4FDB80",
      "#35C46A",
      "#2FB55F", "#278E4B",
      "#1F7C41", "#0F3B1F",
    ],
  },
  defaultRadius: "md",
  fontFamily: "system-ui, -apple-system, Segoe UI, Roboto, Arial",
});

export default function App() {
  return (
    <MantineProvider theme={theme} defaultColorScheme="dark">
      <Notifications />
      <BrowserRouter>
        <AuthProvider>
          <BootstrapGuard>
            <AppRoutes />
          </BootstrapGuard>
        </AuthProvider>
      </BrowserRouter>
    </MantineProvider>
  );
}
