import { LoginModal } from "@/pages/auth/LoginModal";
import { RegistrationModal } from "@/pages/auth/RegistrationModal";
import { CoursesAccessDeniedModal, type RedirectInfo } from "@/pages/courses/list/CoursesAccessDeniedModal";
import { CoursesListPage } from "@/pages/courses/list/CoursesListPage";
import { AppFrame } from "@/shared/layout/AppFrame";
import { AppHeaderCourses } from "@/shared/layout/AppHeaderCourses";
import { showSuccessWithTitle } from "@/shared/functions/toast";
import { useDisclosure } from "@mantine/hooks";
import { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { AppFooterDefault } from "@/shared/layout/AppFooterDefault";

export function CoursesLayout() {
    const [regOpened, reg] = useDisclosure(false);
    const [loginOpened, login] = useDisclosure(false);
    const location = useLocation();
    const navigate = useNavigate();
    const [accessDeniedInfo, setAccessDeniedInfo] = useState<RedirectInfo | undefined>(undefined);

    const { close: closeReg } = reg;
    const { close: closeLogin } = login;

    //Handel auto-close modal Registration & Login at route changes
    useEffect(() => {
        closeReg();
        closeLogin();
    }, [location.pathname, closeReg, closeLogin]);

    //Handel route changes with state included AccessDeniedInfo
    useEffect(() => {
        const info = location.state as RedirectInfo | undefined;
        if (!info) return;

        setAccessDeniedInfo(info);
        // "обнуляем" state, чтобы сообщение не повторялось при back/forward
        navigate("/courses", { replace: true, state: undefined });
    }, [location.state, navigate]);

    const headerCourses = <AppHeaderCourses
        registrationOnClick={() => { reg.open(); }}
        loginOnClick={() => { login.open(); }}
    />;

    return (
        <div className="layout-publicBg">
            <AppFrame header={headerCourses} footer={<AppFooterDefault />}>
                <CoursesListPage />
            </AppFrame >
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
        </div>
    );
}
