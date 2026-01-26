import { Code, Group, Modal, Text, ThemeIcon } from "@mantine/core";
import { IconAlertTriangle, IconInfoCircle } from "@tabler/icons-react";

export type AccessDeniedInfo = {
  reason: LoginReason;
  fromLocation?: string
};

type AccessDeniedModalProps = {
  info: AccessDeniedInfo;
  onClose: () => void;
};

export function CoursesAccessDeniedModal({ info, onClose }: AccessDeniedModalProps) {
  const { reason, fromLocation } = info;
  const isForbidden = reason === "forbidden";
  const titleText = isForbidden ? "Доступ запрещён" : "Требуется вход";
  const message = isForbidden ? "Недостаточно прав для выполнения действия" : "Чтобы продолжить, войдите в систему";

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

      {fromLocation ? (
        <Text size="xs" c="dimmed" mt="sm">
          Запрошенный адрес: <Code>{fromLocation}</Code>
        </Text>
      ) : null}
    </Modal>
  );
}
