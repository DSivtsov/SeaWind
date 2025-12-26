import { Card, Text, Group, ActionIcon, Tooltip } from "@mantine/core";
import { IconMovie, IconTools } from "@tabler/icons-react";
import { useNavigate } from "react-router-dom";

export type Course = {
    id: string;
    title: string;
    description: string;
};

type CourseCardProps = {
    course: Course;
};

export function CourseCard({ course }: CourseCardProps) {
    const { id, title, description } = course;
    const navigate = useNavigate();

    return (
        <Card shadow="sm" padding="lg" radius="md" withBorder>
            <Group justify="space-between" mb="xs">
                <Text fw={700}>{id}</Text>
                <Group gap={"md"}>
                    <Tooltip label="Open video lectures">
                        <ActionIcon variant="filled" size="xl" radius="md" aria-label="Open video lectures"
                            onClick={() => navigate(`/courses/${id}/lectures`)} >
                            <IconMovie />
                        </ActionIcon>
                    </Tooltip>
                    <Tooltip label="Open exercises">
                        <ActionIcon variant="filled" size="xl" radius="md" aria-label="Open exercises"
                            onClick={() => navigate(`/courses/${id}/exercises`)} >
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
