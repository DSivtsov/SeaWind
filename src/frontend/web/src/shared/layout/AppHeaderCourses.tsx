import { AvatarMenu } from "@/pages/avatar/AvatarMenu";
import { Text, Button, Flex, Stack } from "@mantine/core";

type AppHeaderCoursesProps =
    {
        registrationOnClick: () => void,
        loginOnClick: () => void
    }

export function AppHeaderCourses({ registrationOnClick, loginOnClick }: AppHeaderCoursesProps) {
    return (
        <div className="layout-publicHeader" style={{ height: "100%" }}>
            <Flex h="100%" align="center" justify="space-between" px="xl">
                <Stack gap={2}>
                    <Text c="gray.2" size="xl" fw={700}>Доступные лекции и упражнения</Text>
                    <Text c="gray.4" size="sm" fw={500}>Переходи к интересующим материалам курса</Text>
                </Stack>
                <Flex h="100%" justify="flex-end" align="center" gap="md" pr="xl">
                    <Button variant="filled" color="green" onClick={registrationOnClick}>Registration</Button>

                    <Button variant="filled" color="green" onClick={loginOnClick}>Login</Button>

                    <AvatarMenu />
                </Flex>
            </Flex>
        </div>
    );
}
