import { Text, Button, Flex, Stack, Anchor } from "@mantine/core";

export function AppFooterDefault() {
    return (
        <div className="layout-publicFooter" style={{ height: "100%" }}>
            <Flex h="100%" align="center" justify="space-between" px="xl">
                <Stack gap={2}>
                    <Text c="gray.2">© WorkshopCode 2025</Text>
                    <Text c="gray.6">
                        Связаться с администратором:{' '}
                        <Anchor c="green.5" href="mailto:admin@workshopcode.app">
                            admin@workshopcode.app
                        </Anchor>
                    </Text>
                </Stack>
                <Button variant="filled" color="green">Support Chat</Button>
            </Flex>
        </div>
    );
}
