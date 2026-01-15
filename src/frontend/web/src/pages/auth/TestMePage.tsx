import { useState } from "react";
import { Box, Button, Code, Stack, Text } from "@mantine/core";
import { apiRequest } from "@/shared/api/apiRequests";
import { useAuth } from "@/shared/auth/useAuth";

export function TestMePage() {
    const auth = useAuth();
    const token = auth.state.accessToken;

    const [resultApiMe, setResultApiMe] = useState<string>("");
    const [resultNoAccess, setResultNoAccess] = useState<string>("");

    const onCallApiMe = async () => {
        setResultApiMe("Loading...");
        try {
            const data = await apiRequest("/api/users/me", { method: "GET" }, token);
            setResultApiMe(JSON.stringify(data, null, 2));
        } catch {
            // 401/403 уже обрабатываются глобально (redirect -> /courses)
            setResultApiMe("Request failed (see redirect / info on /courses).");
        }
    };

    const onCallApiNoAccess = async () => {
        setResultNoAccess("Loading...");
        try {
            const data = await apiRequest("/api/noAccess", { method: "GET" }, token);
            setResultNoAccess(JSON.stringify(data, null, 2));
        } catch {
            // 401/403 уже обрабатываются глобально (redirect -> /courses)
            setResultNoAccess("Request failed (see redirect / info on /courses).");
        }
    };

    return (
        <Stack p="xl">
            <Box>
                <Text fw={700}>Test: GET /api/users/me (Authorize)</Text>
                <Button onClick={onCallApiMe} color="green">Call /api/users/me</Button>
                <Code block>{resultApiMe}</Code>
            </Box>
            <Box>
                <Text fw={700}>Test: GET /api/noAccess (Forbidden)</Text>
                <Button onClick={onCallApiNoAccess} color="green">Call /api/noAccess</Button>
                <Code block>{resultNoAccess}</Code>
            </Box>
        </Stack>
    );
}
