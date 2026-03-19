import { Stack, Card, Button, TextInput, Title, Badge } from "@mantine/core";

export function MentorTimer() {
    const isSessionRunning = false;
    return (
        <Card withBorder>
            <Stack gap="sm">
                <Stack align="flex-start" gap="xs">
                    <Title order={4}>Время ментора</Title>
                    <Badge variant="light" color={isSessionRunning ? "red" : "green"}>
                        {isSessionRunning ? "Идёт проверка" : "На паузе"}
                    </Badge>
                </Stack>

                <TextInput
                    label="Текущая сессия"
                    value="00:12"
                    readOnly
                    description="Время за текущую проверку" />

                <TextInput
                    label="Всего"
                    value="01:45"
                    readOnly
                    description="Суммарное время по упражнению" />

                <Button color={isSessionRunning ? "red" : "green"} variant="light" fullWidth>
                    {isSessionRunning ? "Пауза" : "Продолжить"}
                </Button>
            </Stack>
        </Card>);
}
