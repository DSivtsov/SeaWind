import { FOOTER_HEIGHT, HEADER_HEIGHT } from "@/common/constants";
import { CoursesListPage } from "@/pages/courses/list/CoursesListPage";
import { ActionIcon, AppShell, Avatar, Text, Button, Flex, Stack, Anchor } from "@mantine/core";


export function CoursesLayout() {
    return (
        <AppShell
            padding="md"
            header={{ height: HEADER_HEIGHT }}
            footer={{ height: FOOTER_HEIGHT }}
        >
            <AppShell.Header>
                <Flex h="100%" justify="flex-end" align="center" gap="md" pr="md">
                    <Button variant="default">Registration</Button>
                    <Button variant="default">Login</Button>
                    <ActionIcon variant="transparent" size="xl" radius="xl" disabled={true}
                        onClick={() => console.log("OnClick Avatar")}
                    >
                        <Avatar variant="outline" size="lg" radius="xl" />
                    </ActionIcon>
                </Flex>
            </AppShell.Header>

            <AppShell.Main className="layout-appMain">
                <CoursesListPage />
            </AppShell.Main>

            <AppShell.Footer>
                <Flex h="100%" align="center" justify="space-between" px="md">
                    <Stack gap={2}>
                        <Text>© WorkshopCode 2025</Text>
                        <Text>
                            Связаться с администратором:{' '}
                            <Anchor href="mailto:admin@workshopcode.app">
                                admin@workshopcode.app
                            </Anchor>
                        </Text>
                    </Stack>
                    <Button variant="default">Support Chat</Button>
                </Flex>
            </AppShell.Footer>
        </AppShell>
    );
}
