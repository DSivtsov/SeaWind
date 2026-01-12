import type { LoginReason } from "@/shared/auth/authStorage";
import { Alert, Button, Group, Text } from "@mantine/core";
import { IconInfoCircle } from "@tabler/icons-react";

//export type LoginReason = "unauthorized" | "forbidden";

export function CoursesAccessInfo({
  reason,
  onClose,
  onLoginClick,
}: {
  reason: LoginReason;
  onClose: () => void;
  onLoginClick: () => void;
}) {
  const message =
    reason === "forbidden"
      ? "У вас нет доступа к этому разделу"
      : "Чтобы продолжить, войдите в систему";

  return (
    <Alert
      icon={<IconInfoCircle size={18} />}
      title="Доступ ограничен"
      withCloseButton
      onClose={onClose}
      mb="md"
    >
      <Group justify="space-between" align="center" wrap="wrap">
        <Text size="sm">{message}</Text>
        {reason === "unauthorized" && (
          <Button variant="default" onClick={onLoginClick}>
            Login
          </Button>
        )}
      </Group>
    </Alert>
  );
}
