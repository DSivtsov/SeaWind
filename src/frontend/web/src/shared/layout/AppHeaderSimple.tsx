import { AvatarMenu } from "@/pages/avatar/AvatarMenu";
import { Text, Flex, Stack, Button } from "@mantine/core";

type AppHeaderSimpleProps =
    {
        headerTitle: string;
        headerDescription: string;
        exitBackOnClick: () => void;
    }

export function AppHeaderSimple({ headerTitle, headerDescription, exitBackOnClick }: AppHeaderSimpleProps) {
    return (
        <div className="layout-publicHeader" style={{ height: "100%" }}>
            <Flex h="100%" align="center" justify="space-between" px="xl">
                <Stack gap={2}>
                    <Text c="gray.2" size="xl" fw={700}>{headerTitle}</Text>
                    <Text c="gray.4" size="sm" fw={500}>{headerDescription}</Text>
                </Stack>
                <Flex h="100%" justify="flex-end" align="center" gap="xl" pr="xl">
                    <Button variant="filled" color="orange" size="md"
                        onClick={exitBackOnClick}>EXIT</Button>

                    <AvatarMenu />
                </Flex>
            </Flex>
        </div>
    );
}
