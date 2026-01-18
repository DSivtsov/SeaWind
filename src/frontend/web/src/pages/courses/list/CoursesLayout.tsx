import { FOOTER_HEIGHT, HEADER_HEIGHT } from "@/common/constants";
import { LoginModal } from "@/pages/auth/LoginModal";
import { RegistrationModal } from "@/pages/auth/RegistrationModal";
import { AvatarMenu } from "@/pages/avatar/AvatarMenu";
import { CoursesAccessDeniedModal, type AccessDeniedInfo } from "@/pages/courses/list/CoursesAccessDeniedModal";
import { CoursesListPage } from "@/pages/courses/list/CoursesListPage";
import { showSuccessWithTitle } from "@/shared/ui/toast";
import { AppShell, Text, Button, Flex, Stack, Anchor } from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";

export function CoursesLayout() {
    const [regOpened, reg] = useDisclosure(false);
    const [loginOpened, login] = useDisclosure(false);
    const location = useLocation();
    const navigate = useNavigate();
    const [accessDeniedInfo, setAccessDeniedInfo] = useState<AccessDeniedInfo | undefined>(undefined);

    const { close: closeReg } = reg;
    const { close: closeLogin } = login;

    useEffect(() => {
        closeReg();
        closeLogin();
    }, [location.pathname, closeReg, closeLogin]);

    useEffect(() => {
        const info = location.state as AccessDeniedInfo | undefined;
        if (!info) return;

        setAccessDeniedInfo(info);
        // "обнуляем" state, чтобы сообщение не повторялось при back/forward
        navigate("/courses", { replace: true, state: undefined });
    }, [location.state, navigate]);

    return (
        <div className="layout-publicBg">
            <AppShell
                padding={0} // отключаем дефолтные padding Mantine — все отступы контролируем вручную
                header={{ height: HEADER_HEIGHT }} // высота нужна Mantine для расчёта header offset
                footer={{ height: FOOTER_HEIGHT }} // высота нужна Mantine для расчёта footer offset
                styles={{
                    root: {
                        height: "100vh", // фиксируем layout по высоте viewport (иначе main растёт по контенту)
                        display: "flex", // делаем корень flex-контейнером
                        flexDirection: "column", // вертикальная колонка: header / main / footer
                    },
                    main: {
                        flex: 1, // main занимает всё оставшееся место между header и footer
                        minHeight: 0, // критично для flex: позволяет main сжиматься и включать overflow
                        overflowY: "auto", // скролл ТОЛЬКО внутри main

                        // Компенсация overlay header/footer. Offsets рассчитываются Mantine автоматически.
                        // Контентные отступы (pt/pb/px) задаются на уровне страницы (CoursesListPage).
                        paddingTop: "var(--app-shell-header-offset)",
                        paddingBottom: "var(--app-shell-footer-offset)",
                    },
                }}
            >
                <AppShell.Header className="layout-publicHeader" >
                    <Flex h="100%" align="center" justify="space-between" px="xl">
                        <Stack gap={2}>
                            <Text c="gray.2" size="xl" fw={700}>Доступные лекции и упражнения</Text>
                            <Text c="gray.4" size="sm" fw={500}>Переходи к интересующим материалам курса</Text>
                        </Stack>
                        <Flex h="100%" justify="flex-end" align="center" gap="md" pr="xl">
                            <Button variant="filled" color="green"
                                onClick={() => {
                                    reg.open();
                                }}>Registration</Button>

                            <Button variant="filled" color="green"
                                onClick={() => {
                                    login.open();
                                }}>Login</Button>

                            <AvatarMenu />
                        </Flex>
                    </Flex>
                </AppShell.Header>

                <AppShell.Main>
                    <CoursesListPage />
                </AppShell.Main>

                <AppShell.Footer className="layout-publicFooter" >
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
                </AppShell.Footer>
            </AppShell>
            {
                regOpened && <RegistrationModal
                    opened={regOpened}
                    onClose={() => reg.close()}
                    onRegistered={() => {
                        showSuccessWithTitle("Регистрация пользователя", 'Регистрация прошла успешно');
                        login.open();
                    }} />
            }
            {
                loginOpened && <LoginModal
                    opened={loginOpened}
                    onClose={() => login.close()}
                    onLogon={() => {
                        showSuccessWithTitle("Подключение пользователя", 'Подключение прошло успешно');
                    }} />
            }
            {
                accessDeniedInfo != null && <CoursesAccessDeniedModal
                    info={accessDeniedInfo}
                    onClose={() => {
                        setAccessDeniedInfo(undefined);
                    }}
                />
            }
        </div >
    );
}
