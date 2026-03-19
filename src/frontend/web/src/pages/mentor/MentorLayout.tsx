import { AppHeaderDefault } from "@/shared/layout/AppHeaderDefault";
import { AppFrame } from "@/shared/layout/AppFrame";
import { Stack, Center } from "@mantine/core";
import { Outlet, useNavigate } from "react-router-dom";
import { NavItem } from "@/shared/components/NavItem";

export function MentorLayout() {
    const navigate = useNavigate();
    const headerDefaultForActiveTab =
        <AppHeaderDefault
            headerTitle={"Mentor menu"}
            headerDescription="Выбери раздел"
            allCoursesOnClick={() => {
                navigate("/courses");
            }}
        />;

    const navbarMentor =
        // className="layout-publicHeader" set directly at AppFrame level for NavBar & Drawer
        //  (Drawer header issue it bg not set by other)
        <Center h="100%">
            <Stack align="center" gap={30} justify="center" p="md">
                <NavItem linkTo={"exercises-chats-inbox"} linkLabel={"Чаты упражнений"} />
                <NavItem linkTo={"workshop-manage"} linkLabel={"Управление рабочими сессиями"} />
                <NavItem linkTo={"hours-spent"} linkLabel={"Управление учётом трудозатрат"} />
            </Stack>
        </Center>;

    return (
        <div className="layout-publicBg">
            <AppFrame header={headerDefaultForActiveTab} navbar={navbarMentor}>
                <Outlet />
            </AppFrame >
        </div >
    );
}
