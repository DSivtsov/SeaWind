import { publicImages } from "@/common/constants";
import "./landing.css";
import {
    Box,
    Button,
    Container,
    Flex,
    Group,
    List,
    Paper,
    Stack,
    Text,
    ThemeIcon,
    Title,
} from "@mantine/core";
import { IconCheck } from "@tabler/icons-react";
import { useNavigate } from "react-router-dom";

type BulletProps = {
    items: string[];
};

function BulletList({ items }: BulletProps) {
    const icon = (
        <ThemeIcon radius="xl" size={22} style={{ backgroundColor: "var(--pub-surface-1)" }}>
            <IconCheck size={14} />
        </ThemeIcon>
    );

    return (
        <List spacing="xs" icon={icon}>
            {items.map((x) => (
                <List.Item key={x}>
                    <Text c="var(--pub-text-1)">{x}</Text>
                </List.Item>
            ))}
        </List>
    );
}

export function LandingPage() {
    const navigate = useNavigate();

    const heroBullets = [
        "Практико-ориентированные курсы программирования с поддержкой ментора",
        "Обучение через реальные задачи, упражнения и воркшопы",
    ];

    const freeBullets = ["Полный доступ ко всем курсам, лекциям и заданиям", "Полностью самостоятельное обучение"];

    const mentorBullets = [
        "Всё, что входит в Free доступ",
        "Поддержка со стороны ментора",
        "Проверка заданий ментором (через чат)",
        "Воркшопы с ментором (совместная работа и обсуждение)",
    ];

    const onExploreCourses = () => navigate("/courses");

    return (
        <div className="layout-LandingBg">
            <Container className="layout-publicContainer" size={1200}>
                {/* HERO */}
                <Box
                    mih={360}
                    style={{
                        display: "flex",
                        alignItems: "center",
                    }}
                >
                    <Flex w="100%" gap={40} align="center" justify="space-between" wrap="wrap">
                        <Stack gap={10} style={{ flex: "1 1 520px" }}>
                            <Title order={1} style={{ color: "var(--pub-text-0)" }}>
                                WorkshopCode
                            </Title>
                            <Text size="lg" style={{ color: "var(--pub-text-1)" }}>
                                Learn C# through practice
                            </Text>

                            <Paper
                                radius="md"
                                p="md"
                                style={{
                                    background: "var(--pub-surface-0)",
                                    border: "1px solid var(--pub-border)",
                                    maxWidth: 720,
                                }}
                            >
                                <BulletList items={heroBullets} />
                            </Paper>
                        </Stack>

                        <Stack gap={10} align="center" style={{ flex: "0 1 420px" }}>
                            <Paper
                                radius="xl"
                                p={0}
                                style={{
                                    width: "100%",
                                    height: 220,          // ← вот ключ
                                    background: "var(--pub-surface-0)",
                                    border: "1px solid var(--pub-border)",
                                    overflow: "hidden",
                                }}
                            >
                                <img
                                    src={publicImages.hero}
                                    alt="Abstract technology illustration"
                                    style={{
                                        width: "100%",
                                        height: "100%",
                                        display: "block",
                                        objectFit: "cover",
                                        opacity: 0.95,
                                    }}
                                />
                            </Paper>

                            <Button
                                size="lg"
                                onClick={onExploreCourses}
                            >
                                Начать обучение
                            </Button>

                            <Text size="sm" style={{ color: "var(--pub-text-1)" }}>
                                Перейти к курсам и заданиям
                            </Text>
                        </Stack>
                    </Flex>
                </Box>

                {/* BELOW HERO */}
                <Group align="flex-start" gap={48} mt="xl" wrap="wrap">
                    <Stack gap={10} style={{ flex: "1 1 360px" }}>
                        <Title order={3} style={{ color: "var(--pub-text-0)" }}>
                            Выберите уровень доступа
                        </Title>

                        <Paper
                            radius="md"
                            p="md"
                            style={{
                                background: "var(--pub-surface-0)",
                                border: "1px solid var(--pub-border)",
                            }}
                        >
                            <Stack gap={8}>
                                <Text fw={700} style={{ color: "var(--pub-text-0)" }}>
                                    Free доступ (FreeStudent)
                                </Text>
                                <BulletList items={freeBullets} />
                            </Stack>
                        </Paper>

                        <Paper
                            radius="md"
                            p="md"
                            style={{
                                background: "var(--pub-surface-0)",
                                border: "1px solid var(--pub-border)",
                            }}
                        >
                            <Stack gap={8}>
                                <Text fw={700} style={{ color: "var(--pub-text-0)" }}>
                                    Доступ с ментором (PaidStudent)
                                </Text>
                                <BulletList items={mentorBullets} />
                            </Stack>
                        </Paper>
                    </Stack>

                    <Stack gap={10} style={{ flex: "1 1 520px" }}>
                        <Title order={3} style={{ color: "var(--pub-text-0)" }}>
                            Как это работает
                        </Title>

                        <Paper
                            radius="md"
                            p="md"
                            style={{
                                background: "var(--pub-surface-0)",
                                border: "1px solid var(--pub-border)",
                            }}
                        >
                            <List spacing="sm">
                                <List.Item>
                                    <Text c="var(--pub-text-1)">Выберите курс, посмотрите лекции и сразу переходите к практике.</Text>
                                </List.Item>
                                <List.Item>
                                    <Text c="var(--pub-text-1)">Решайте задания и отслеживайте прогресс по курсу.</Text>
                                </List.Item>
                                <List.Item>
                                    <Text c="var(--pub-text-1)">
                                        Если у вас доступ с ментором — обсуждайте решения в чате и участвуйте в воркшопах.
                                    </Text>
                                </List.Item>
                            </List>
                        </Paper>
                    </Stack>
                </Group>
            </Container>
        </div>
    );
}
