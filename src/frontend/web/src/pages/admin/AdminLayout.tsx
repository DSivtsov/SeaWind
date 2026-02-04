import { AppHeaderDefault } from "@/shared/layout/AppHeaderDefault";
import { AppFrame } from "@/shared/layout/AppFrame";
import { Stack, Center } from "@mantine/core";
import { Outlet, useNavigate } from "react-router-dom";
import { NavItem } from "@/shared/components/NavItem";

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
        // className="layout-publicHeader" set directly at AppFrame level for NavBar & Drawer
        //  (Drawer header issue it bg not set by other)
        <Center h="100%">
            <Stack align="center" gap={30} justify="center" p="md">
                <NavItem linkTo={"users-roles"} linkLabel={"Управление ролями пользователей"} />
                <NavItem linkTo={"support-inbox"} linkLabel={"Входящие сообщения пользователей"} />
                <NavItem linkTo={"hours-added"} linkLabel={"Управление учебными часами студентов"} />
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
