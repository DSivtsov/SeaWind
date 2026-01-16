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
        <Card padding="lg" radius="md" bg="dark.8">
            <Group justify="space-between" mb="xs">
                <Text fw={700} size="xs" c="gray.7">
                    {id}
                </Text>

                <Group gap="sm">
                    <Tooltip label="Open video lectures">
                        <ActionIcon variant="subtle" color="green" size="lg" aria-label="Open video lectures"
                            onClick={() => navigate(`/courses/${id}/lectures`)} >
                            <IconMovie />
                        </ActionIcon>
                    </Tooltip>
                    <Tooltip label="Open exercises">
                        <ActionIcon variant="subtle" color="green" size="lg" aria-label="Open video exercises"
                            onClick={() => navigate(`/courses/${id}/exercises`)} >
                            <IconTools />
                        </ActionIcon>
                    </Tooltip>
                </Group>
            </Group>

            <Text fw={600} c="gray.4">
                {title}
            </Text>

            <Text c="gray.6">
                {description}
            </Text>
        </Card>

    );
}
