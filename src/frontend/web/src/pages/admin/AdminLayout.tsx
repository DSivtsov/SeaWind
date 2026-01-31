import { AppHeaderDefault } from "@/shared/layout/AppHeaderDefault";
import { AppFrame } from "@/shared/layout/AppFrame";
import { Stack, Center, NavLink, Text } from "@mantine/core";
import { Outlet, useNavigate } from "react-router-dom";
import { NavLink as RouterNavLink } from "react-router-dom";

export function AdminLayout() {
    const navigate = useNavigate();
    const headerDefaultForActiveTab =
        <AppHeaderDefault
            headerTitle={"Admin menu"}
            headerDescription="Выбери раздел"
            allCoursesOnClick={() => {
                navigate("/courses");
            }}
        />;

    const navbarDefaultForAdmin =
        <Center className="layout-publicHeader" h="100%">
            <Stack align="center" gap={30} justify="center" p="md">
                <RouterNavLink to="users-roles" style={{ textDecoration: "none" }}>
                    {({ isActive }) => (
                        <NavLink
                            label={<Text ta="center">Управления ролями пользователей</Text>}// "Set User Role"
                            active={isActive}
                        />
                    )}
                </RouterNavLink>
                <RouterNavLink to="support-inbox" style={{ textDecoration: "none" }}>
                    {({ isActive }) => (
                        <NavLink
                            label={<Text ta="center">Входящие сообщения пользователей</Text>}//"SupportInbox"
                            active={isActive}
                        />
                    )}
                </RouterNavLink>
                <RouterNavLink to="hours-added" style={{ textDecoration: "none" }}>
                    {({ isActive }) => (
                        <NavLink
                            label={<Text ta="center">Редактирование учебных часов</Text>}
                            active={isActive}
                        />
                    )}
                </RouterNavLink>
            </Stack>
        </Center>;

    return (
        <div className="layout-publicBg">
            <AppFrame header={headerDefaultForActiveTab} navbar={navbarDefaultForAdmin}>
                <Outlet />
            </AppFrame >
        </div >
    );
}

