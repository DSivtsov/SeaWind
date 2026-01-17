import { Button, Container, Group, Paper, Stack, Text, Title } from "@mantine/core";
import { useNavigate } from "react-router-dom";

export function ForbiddenPage() {
    const navigate = useNavigate();

    return (
        <Container size="sm" py="xl">
            <Paper withBorder p="xl" radius="md">
                <Stack gap="md">
                    <Title order={2}>403 Доступ запрещён</Title>
                    <Text>У вас нет прав для доступа к этой странице.</Text>
                    <Text c="dimmed" size="sm">Этот раздел доступен только для пользователей с другой ролью.</Text>
                    <Group justify="space-between" mt="sm">
                        <Button variant="default" onClick={() => navigate(-1)}>Назад</Button>
                        <Button onClick={() => navigate("/courses", { replace: true })}>Вернуться к курсам</Button>
                    </Group>
                </Stack>
            </Paper>
        </Container>
    );
}
