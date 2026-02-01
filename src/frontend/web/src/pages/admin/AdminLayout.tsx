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
        <Center className="layout-publicHeader" h="100%">
            <Stack align="center" gap={30} justify="center" p="md">
                <NavItem linkTo={"users-roles"} linkLabel={"Управления ролями пользователей"} />
                <NavItem linkTo={"support-inbox"} linkLabel={"Входящие сообщения пользователей"} />
                <NavItem linkTo={"hours-added"} linkLabel={"Редактирование учебных часов"} />
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
