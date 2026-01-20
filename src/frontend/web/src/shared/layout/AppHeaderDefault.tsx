import { AvatarMenu } from "@/pages/avatar/AvatarMenu";
import { Text, Button, Flex, Stack } from "@mantine/core";

type AppHeaderDefaultProps =
    {
        headerTitle: string;
        headerDescription: string;
        allCoursesOnClick: () => void;
    }

export function AppHeaderDefault({ headerTitle, headerDescription, allCoursesOnClick }: AppHeaderDefaultProps) {
    return (
        <div className="layout-publicHeader" style={{ height: "100%" }}>
            <Flex h="100%" align="center" justify="space-between" px="xl">
                <Stack gap={2}>
                    <Text c="gray.2" size="xl" fw={700}>{headerTitle}</Text>
                    <Text c="gray.4" size="sm" fw={500}>{headerDescription}</Text>
                </Stack>
                <Flex h="100%" justify="flex-end" align="center" gap="md" pr="xl">
                    <Button variant="filled" color="green"
                        onClick={allCoursesOnClick}>Все курсы</Button>

                    <AvatarMenu />
                </Flex>
            </Flex>
        </div>
    );
}
