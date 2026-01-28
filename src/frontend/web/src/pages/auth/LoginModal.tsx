import { useState, type FormEvent } from "react";
import { Modal, TextInput, PasswordInput, Button, Stack, Text, Box } from "@mantine/core";
import { isApiError } from "@/shared/api/apiError";
import { loginRequest } from "@/pages/auth/AuthApi";
import { useAuthContext } from "@/shared/auth/authContext";

type LoginModalProps = {
  opened: boolean;
  onClose: () => void;
  /**
   * Optional: if backend returns JWT on register, you can handle it here.
   * Keep MVP: parent decides what to do (save token, close modal, open login, etc.).
   */
  onLogon?: () => void;
};

export function LoginModal({ opened, onClose, onLogon }: LoginModalProps) {
  const authCtx = useAuthContext();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const [submitting, setSubmitting] = useState(false);
  const [errorText, setErrorText] = useState("");

  const clearError = () => {
    if (errorText) setErrorText("");
  };

  const onChangeEmail = (value: string) => {
    setEmail(value);
    clearError();
  };

  const onChangePassword = (value: string) => {
    setPassword(value);
    clearError();
  };

  async function handleSubmit(event: FormEvent<HTMLFormElement>): Promise<void> {
    event.preventDefault();

    setSubmitting(true);
    setErrorText("");

    try {
      const rez = await loginRequest(email.trim(), password);
      authCtx.login(rez.accessToken, email);
      onLogon?.();
      onClose();
    } catch (e: unknown) {
      console.log(e);
      if (isApiError(e) && e.kind === "abort") return; // не показываем ошибку

      if (isApiError(e) && e.kind === "http" && e.status === 401) {
        setErrorText("Неверный email или пароль.");
        return;
      }

      setErrorText("Проблема с сервером. Попробуйте позже.");
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <Modal opened={opened} onClose={onClose} centered size="xs"
      title={<Text fw={700} c="gray.2">Login</Text>}
      overlayProps={{
        backgroundOpacity: 0.55,
        blur: 3,
      }}
      classNames={{
        header: "auth-modal-header",
      }}>
      <form onSubmit={handleSubmit}>
        <Stack gap="lg" pt="sm" >
          <TextInput
            className="input-field"
            label={<Text fw={500} c="gray.5">Email</Text>}
            value={email}
            onChange={(e) => onChangeEmail(e.currentTarget.value)}
            required
            autoComplete="email"
          />

          <PasswordInput
            className="input-field"
            label={<Text fw={500} c="gray.5">Password</Text>}
            value={password}
            onChange={(e) => onChangePassword(e.currentTarget.value)}
            required
            autoComplete="current-password"
          />

          <Text c="red.6" size="sm" mih="lg">
            {errorText}
          </Text>
        </Stack>

        <Box pt="sm" style={{ textAlign: "center" }}>
          <Button type="submit" loading={submitting} color="green">Login</Button>
        </Box>
      </form>
    </Modal >
  );
}


/*const onSubmit = async () => {
  console.log("Login: onSubmit");
  return;

  if (!canSubmit) return;

  setSubmitting(true);
  setErrorText("");
  /*
      try {
        await registerRequest(email.trim(), password);
        onRegistered?.({});
        onClose();
      } catch (e: unknown) {
        console.log(e);
        if (isApiError(e) && e.kind === "abort") return; // не показываем ошибку

        if (isApiError(e) && e.kind === "http" && e.status === 409) {
          setErrorText("Пользователь с таким email уже существует.");
          return;
        }

        if (isApiError(e) && e.kind === "http" && e.status === 400) {
          setErrorText("Проверьте введённые данные.");
          return;
        }

        setErrorText("Проблема с сервером. Попробуйте позже.");
      } finally {
        setSubmitting(false);
      }

};
*/
