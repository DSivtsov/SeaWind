import { Card, Text, Group, ActionIcon, Tooltip } from "@mantine/core";
import { IconMovie, IconTools } from "@tabler/icons-react";

export type Course = {
    title: string;
    code: string;
    description: string;
};

type CourseCardProps = {
    course: Course;
};

export function CourseCard({ course }: CourseCardProps) {
    const { title, code, description } = course;
    return (
        <Card shadow="sm" padding="lg" radius="md" withBorder>
            <Group justify="space-between" mb="xs">
                <Text fw={700}>{code}</Text>
                <Group gap={"md"}>
                    <Tooltip label="Open video lectures">
                        <ActionIcon variant="filled" size="xl" radius="md" aria-label="Open video lectures">
                            <IconMovie />
                        </ActionIcon>
                    </Tooltip>
                    <Tooltip label="Open exercises">
                        <ActionIcon variant="filled" size="xl" radius="md" aria-label="Open exercises">
                            <IconTools />
                        </ActionIcon>
                    </Tooltip>
                </Group>
            </Group>

            <Text fw={500}>
                {title}
            </Text>

            <Text>
                {description}
            </Text>
        </Card>
    );
}
