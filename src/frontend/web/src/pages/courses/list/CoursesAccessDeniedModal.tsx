import type { LoginReason } from "@/shared/auth/authStorage";
import { Group, Modal, Text, ThemeIcon } from "@mantine/core";
import { IconAlertTriangle, IconInfoCircle } from "@tabler/icons-react";

type CoursesAccessDeniedModalProps = {
  reason: LoginReason;
  onClose: () => void;
};

export function CoursesAccessDeniedModal({ reason, onClose }: CoursesAccessDeniedModalProps) {
  const isForbidden = reason === "forbidden";

  const titleText = isForbidden ? "Доступ запрещён" : "Требуется вход";
  const message = isForbidden
    ? "У вас нет доступа к этому разделу"
    : "Чтобы продолжить, войдите в систему";

  const icon = isForbidden ? <IconAlertTriangle size={18} /> : <IconInfoCircle size={18} />;
  const iconColor = isForbidden ? "yellow" : "blue";

  const title = (
    <Group gap="sm">
      <ThemeIcon color={iconColor} variant="light" radius="xl">
        {icon}
      </ThemeIcon>
      <Text fw={700}>{titleText}</Text>
    </Group>
  );

  return (
    // Modal component is always open when it is rendered
    <Modal opened onClose={onClose} centered size="sm" title={title}>
      <Text size="sm" pt="xs">
        {message}
      </Text>
    </Modal>
  );
}
