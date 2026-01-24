import { useEffect, useMemo, useState } from "react";
import { useParams } from "react-router-dom";
import { Badge, Box, Card, Grid, Group, Modal, Skeleton, Stack, Text, TextInput, Title } from "@mantine/core";

import { useAuth } from "@/shared/auth/useAuth";

type CourseDto = {
  id: string;
  code: string;
  title: string;
  description?: string | null;
};

type CourseLectureDto = {
  id: string;
  title: string;
  description?: string | null;
  videoUrl?: string | null;
};

type UiState =
  | { kind: "loading" }
  | { kind: "error" }
  | { kind: "empty"; course: CourseDto }
  | { kind: "ready"; course: CourseDto; lectures: CourseLectureDto[] };

function isValidHttpUrl(url: string): boolean {
  try {
    const u = new URL(url);
    return u.protocol === "http:" || u.protocol === "https:";
  } catch {
    return false;
  }
}

async function apiGetJson<T>(path: string, token: string, signal: AbortSignal): Promise<T> {
  const res = await fetch(path, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    signal,
  });

  if (!res.ok) {
    // Contract: 401/403 are handled centrally (AuthProvider + accessDenied event).
    // Here we treat any non-OK as a screen-level error.
    throw new Error(`HTTP_${res.status}`);
  }

  return (await res.json()) as T;
}

export function CourseLecturesPage() {
  const { courseId } = useParams<{ courseId: string }>();
  const auth = useAuth();

  const token = auth.state.accessToken ?? null;

  const [ui, setUi] = useState<UiState>({ kind: "loading" });
  const [badLinkOpened, setBadLinkOpened] = useState(false);

  const courseTitle = useMemo(() => {
    if (ui.kind === "ready" || ui.kind === "empty") return ui.course.title;
    return "";
  }, [ui]);

  useEffect(() => {
    if (!courseId || !token) {
      // Defensive: RouteGuard should prevent entering here without token.
      setUi({ kind: "error" });
      return;
    }

    const ac = new AbortController();

    (async () => {
      try {
        setUi({ kind: "loading" });

        const [course, lectures] = await Promise.all([
          apiGetJson<CourseDto>(`/api/course/${courseId}`, token, ac.signal),
          apiGetJson<CourseLectureDto[]>(`/api/courses/${courseId}/lectures`, token, ac.signal),
        ]);

        if (!lectures || lectures.length === 0) {
          setUi({ kind: "empty", course });
          return;
        }

        setUi({ kind: "ready", course, lectures });
      } catch {
        if (ac.signal.aborted) return;
        setUi({ kind: "error" });
      }
    })();

    return () => ac.abort();
  }, [courseId, token]);

  const openLectureVideo = (lecture: CourseLectureDto) => {
    const url = (lecture.videoUrl ?? "").trim();
    if (!url || !isValidHttpUrl(url)) {
      setBadLinkOpened(true);
      return;
    }

    window.open(url, "_blank", "noopener,noreferrer");
  };

  return (
    <Box>
      <Modal
        opened={badLinkOpened}
        onClose={() => setBadLinkOpened(false)}
        title="Ссылка недоступна"
        centered
      >
        <Text size="sm">Похоже, у этой лекции нет корректной ссылки на видео.</Text>
      </Modal>

      <Stack gap="md">
        {/* Extra header: active course info */}
        <Box>
          {ui.kind === "loading" ? (
            <Stack gap="xs">
              <Skeleton h={28} w={260} />
              <Skeleton h={36} />
              <Skeleton h={36} />
              <Skeleton h={52} />
            </Stack>
          ) : ui.kind === "error" ? (
            <Stack gap={4}>
              <Title order={3}>Course Lectures</Title>
              <Text c="dimmed">Проблема с сервером. Попробуйте позже.</Text>
            </Stack>
          ) : (
            <Stack gap="xs">
              <Group justify="space-between" align="center">
                <Stack gap={0}>
                  <Title order={3}>Lectures</Title>
                  <Text c="dimmed" size="sm">
                    {courseTitle}
                  </Text>
                </Stack>
                <Badge variant="light">List Lectures</Badge>
              </Group>

              <Grid gutter="md">
                <Grid.Col span={{ base: 12, md: 4 }}>
                  <TextInput label="Course Code" value={ui.course.code} readOnly />
                </Grid.Col>
                <Grid.Col span={{ base: 12, md: 8 }}>
                  <TextInput label="Course Title" value={ui.course.title} readOnly />
                </Grid.Col>
                <Grid.Col span={12}>
                  <TextInput
                    label="Course description"
                    value={ui.course.description ?? ""}
                    readOnly
                  />
                </Grid.Col>
              </Grid>
            </Stack>
          )}
        </Box>

        {/* Content */}
        {ui.kind === "loading" ? (
          <Grid gutter="md">
            {Array.from({ length: 6 }).map((_, i) => (
              <Grid.Col key={i} span={{ base: 12, sm: 6, lg: 4 }}>
                <Card withBorder radius="md" padding="md">
                  <Stack gap="xs">
                    <Skeleton h={16} w={120} />
                    <Skeleton h={16} w="80%" />
                    <Skeleton h={14} w="90%" />
                    <Skeleton h={14} w="70%" />
                    <Skeleton h={32} />
                  </Stack>
                </Card>
              </Grid.Col>
            ))}
          </Grid>
        ) : ui.kind === "error" ? null : ui.kind === "empty" ? (
          <Text>В этом курсе ещё нет лекций</Text>
        ) : (
          <Grid gutter="md">
            {ui.lectures.map((lec, idx) => (
              <Grid.Col key={lec.id ?? String(idx)} span={{ base: 12, sm: 6, lg: 4 }}>
                <Card
                  withBorder
                  radius="md"
                  padding="md"
                  role="button"
                  tabIndex={0}
                  onClick={() => openLectureVideo(lec)}
                  onKeyDown={(e) => {
                    if (e.key === "Enter" || e.key === " ") openLectureVideo(lec);
                  }}
                >
                  <Stack gap="xs">
                    <Group justify="space-between" align="center">
                      <Badge variant="light">Lecture</Badge>
                      <Badge variant="outline">▶</Badge>
                    </Group>
                    <Text fw={600} lineClamp={2}>
                      {lec.title}
                    </Text>
                    <Text size="sm" c="dimmed" lineClamp={3}>
                      {lec.description ?? ""}
                    </Text>
                    <Text size="xs" c="dimmed">
                      Click to open video
                    </Text>
                  </Stack>
                </Card>
              </Grid.Col>
            ))}
          </Grid>
        )}
      </Stack>
    </Box>
  );
}
