import { BrowserRouter } from 'react-router-dom';
import { AppRoutes } from "./AppRoutes";

//Mantine Provider & styles
import '@mantine/core/styles.css';
import { MantineProvider, createTheme } from '@mantine/core';

const theme = createTheme({
  primaryColor: "brand",
  colors: {
    brand: [
      "#E9FFF0", "#CFF9DD", "#A3F0BE", "#73E59D", "#4FDB80",
      "#35C46A", "#2FB55F", "#278E4B", "#1F7C41", "#0F3B1F",
    ],
  },
  defaultRadius: "md",
  fontFamily: "system-ui, -apple-system, Segoe UI, Roboto, Arial",
});

export default function App() {
  return (
    <MantineProvider theme={theme}>
      <BrowserRouter>
        <AppRoutes />
      </BrowserRouter>
    </MantineProvider>
  );
}
