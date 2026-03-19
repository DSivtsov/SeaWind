import { assertNever } from "@/shared/functions/assertNever";
import type { RedirectReason } from "@/shared/auth/authListeners";
import { Code, Group, Modal, Text, ThemeIcon } from "@mantine/core";
import { IconAlertTriangle, IconInfoCircle } from "@tabler/icons-react";

export type RedirectInfo = {
  reason: RedirectReason;
  fromLocation?: string
};

type AccessDeniedModalProps = {
  info: RedirectInfo;
  onClose: () => void;
};

export function CoursesAccessDeniedModal({ info, onClose }: AccessDeniedModalProps) {
  const { reason, fromLocation } = info;
  const { iconColor, icon, titleText, message } = getTitleAndText(reason);

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

function getTitleAndText(reason: "unauthorized" | "forbidden" | "invalid_state" | "exercise_chat_mentor_error") {
  switch (reason) {
    case "forbidden":
      return {
        iconColor: "yellow",
        icon: <IconAlertTriangle size={18} />,
        titleText: "Доступ запрещён",
        message: "Недостаточно прав для выполнения действия",
      };

    case "unauthorized":
      return {
        iconColor: "blue",
        icon: <IconInfoCircle size={18} />,
        titleText: "Требуется вход",
        message: "Чтобы продолжить, войдите в систему",
      };

    case "invalid_state":
      return {
        iconColor: "red",
        icon: <IconAlertTriangle size={18} />,
        titleText: "Ошибка состояния",
        message:
          "Страница оказалась в некорректном состоянии. Выполнен возврат на список курсов.",
      };

    case "exercise_chat_mentor_error":
      return {
        iconColor: "red",
        icon: <IconAlertTriangle size={18} />,
        titleText: "Ошибка при попытке доступа",
        message:
          "Произошла ошибка при попытке открыть чат упражнения. Доступ запрещен.",
      };
  }

  return assertNever(reason);
}


