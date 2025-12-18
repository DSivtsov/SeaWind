import { BrowserRouter } from 'react-router-dom';
import { AppRoutes } from "./AppRoutes";

//Mantine Provider & styles
import '@mantine/core/styles.css';
import { MantineProvider } from '@mantine/core';

export default function App() {
  return (
    <MantineProvider theme={{ fontFamily: "system-ui, -apple-system, Segoe UI, Roboto, Arial" }}>
      <BrowserRouter>
        <AppRoutes />
      </BrowserRouter>
    </MantineProvider>
  );
}
