import { useMemo, useState } from "react";
import { Modal, TextInput, PasswordInput, Button, Stack, Text, Box, Group } from "@mantine/core";
import { IconCheck, IconX } from "@tabler/icons-react";
import { isApiError } from "@/shared/api/apiRequests";
import { registerRequest } from "@/pages/auth/RegistrationApi";
import { buildPasswordRules, isValidEmail, type RuleCheck } from "@/pages/auth/RegistrationRules";

type RegisterResult = {
  accessToken?: string;
};

type RegistrationModalProps = {
  opened: boolean;
  onClose: () => void;
  /**
   * Optional: if backend returns JWT on register, you can handle it here.
   * Keep MVP: parent decides what to do (save token, close modal, open login, etc.).
   */
  onRegistered?: (result: RegisterResult) => void;
};

function RuleRow({ label, ok }: RuleCheck) {
  const Icon = ok ? IconCheck : IconX;
  const color = ok ? "green.5" : "red.6";

  return (
    <Group gap="xs" justify="space-between" wrap="nowrap">
      <Text size="sm" c="gray.5">
        {label}
      </Text>
      <Icon size={18} color={`var(--mantine-color-${color.replace(".", "-")})`} />
    </Group>
  );
}

export function RegistrationModal({ opened, onClose, onRegistered }: RegistrationModalProps) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");

  const [submitting, setSubmitting] = useState(false);
  const [errorText, setErrorText] = useState("");

  //экономия вызовов buildPasswordRules (...) в случае изменения других полей - email
  const rules = useMemo(() => buildPasswordRules(password, confirmPassword), [password, confirmPassword]);

  const emailOk = isValidEmail(email);
  const canSubmit = emailOk && rules.every((r) => r.ok) && !submitting;

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

  const onChangeConfirmPassword = (value: string) => {
    setConfirmPassword(value);
    clearError();
  };

  const onSubmit = async () => {
    if (!canSubmit) return;

    setSubmitting(true);
    setErrorText("");

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
  return (
    <Modal opened={opened} onClose={onClose} centered size="sm"
      title={<Text fw={700} c="gray.2">Registration</Text>}
      overlayProps={{
        backgroundOpacity: 0.55,
        blur: 3,
      }}
      classNames={{
        header: "auth-modal-header",
      }}>
      <Stack gap="lg" pt="sm" >
        <TextInput
          className="input-field"
          label={<Text fw={500} c="gray.5">Email</Text>}
          placeholder="you@example.com"
          value={email}
          onChange={(e) => onChangeEmail(e.currentTarget.value)}
          error={email.length > 0 && !emailOk ? "Invalid email" : " "}
          autoComplete="email"
        />

        <PasswordInput
          className="input-field"
          label={<Text fw={500} c="gray.5">Password</Text>}
          value={password}
          onChange={(e) => onChangePassword(e.currentTarget.value)}
          autoComplete="new-password"
        />

        <PasswordInput
          className="input-field"
          label={<Text fw={500} c="gray.5">Confirm password</Text>}
          value={confirmPassword}
          onChange={(e) => onChangeConfirmPassword(e.currentTarget.value)}
          autoComplete="new-password"
        />

        <Stack gap={6}>
          {rules.map((r) => (
            <RuleRow key={r.label} label={r.label} ok={r.ok} />
          ))}
        </Stack>

        <Text c="red.6" size="sm" mih="lg">
          {errorText}
        </Text>

      </Stack>
      <Box pt="sm" style={{ textAlign: "center" }}>
        <Button onClick={onSubmit} disabled={!canSubmit} loading={submitting} color="green">
          Registration
        </Button>
      </Box>
    </Modal >
  );
}
