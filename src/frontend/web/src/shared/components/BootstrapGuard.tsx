import { useAuthContext } from "@/shared/auth/authContext";
import { getHealth } from "@/shared/auth/healthApi";
import { Button, Center, Flex, Loader, Stack, Text, Title } from "@mantine/core";
import { IconExclamationCircleFilled } from "@tabler/icons-react";
import { useEffect, useMemo, useState } from "react";
import { useLocation } from "react-router-dom";

type ApiHealth =
    "loading"        // ждем ответа /health
    | "error"            // где-то app-level error
    | "ready";           // получили Healthy и после не было ошибок


type Bootstrap = "loading" | "pass" | "error";

function FullPageLoading() {
    return (
        <Center mih="80vh">
            <Stack align="center" gap="xs">
                <Loader size="lg" type="dots" />
                <Text size="lg">Загрузка…</Text>
            </Stack>
        </Center>
    );
}

function FullPageError({ text }: { text: string }) {
    return (
        <div className="layout-publicBg">
            <Flex mih="100dvh" gap="md" justify="center" align="center" direction="column">
                <Flex flex={1} />
                <Flex direction="column" align="center" gap="md" mb="xl">
                    <IconExclamationCircleFilled color="red" size={150} />
                    <Title order={1}>Проблема с сервером</Title>
                    <Text size="xl">{text}</Text>
                </Flex>
                <Flex flex={3} direction="column" align="center" gap="sm">
                    <Text c="dimmed">Попробуйте обновить страницу или повторить позже.</Text>
                    <Button size="lg" onClick={() => window.location.reload()}>
                        Попробовать снова
                    </Button>
                </Flex>
            </Flex>
        </div>
    );
}

export function BootstrapGuard({ children }: { children: React.ReactNode }) {
    const authCtx = useAuthContext();
    const location = useLocation();

    const [apiHealth, setApiHealth] = useState<ApiHealth>("loading");

    const isLanding = location.pathname === "/";
    const isAuthed = authCtx.isAuthenticated;

    const guestMode = !isAuthed && authCtx.me.kind === "empty";

    const guard: Bootstrap = useMemo(() => {
        if (isLanding) return "pass";

        if (isAuthed) {
            if (authCtx.me.kind === "ready") return "pass";
            if (authCtx.me.kind === "loading") return "loading";
            return "error";
        }

        // !isAuthed only
        if (!guestMode) return "loading"; // на всякий случай (переходные кадры)
        if (apiHealth === "ready") return "pass";
        if (apiHealth === "loading") return "loading";
        return "error";
    }, [apiHealth, authCtx.me.kind, guestMode, isAuthed, isLanding]);

    useEffect(() => {
        if (isLanding) return;
        if (!guestMode) return;
        if (apiHealth !== "loading") return;

        const ctrl = new AbortController();

        (async () => {
            try {
                await getHealth(ctrl.signal);
                setApiHealth("ready");
            } catch {
                if (ctrl.signal.aborted) return;
                setApiHealth("error");
            }
        })();

        return () => ctrl.abort();
    }, [apiHealth, guestMode, isLanding]);

    if (guard === "loading") return <FullPageLoading />;

    if (guard === "error") {
        const text = isAuthed ? "Не удалось загрузить профиль пользователя."
            : "Не могу связаться с сервером.";

        return <FullPageError text={text} />;
    }

    return <>{children}</>;
}
