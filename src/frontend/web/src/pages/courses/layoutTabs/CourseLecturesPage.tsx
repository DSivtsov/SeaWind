import { useCallback, useEffect, useRef, useState } from "react";
import { useParams } from "react-router-dom";
import { Badge, Box, Card, Grid, Group, Modal, Skeleton, Stack, Text } from "@mantine/core";
import { useAuthContext } from "@/shared/auth/authContext";
import { getAllLecturesByCourseIdOrdered, type CourseLectureDto }
  from "@/pages/courses/layoutTabs/courseLayoutTabsApi";
import { PageShell } from "@/shared/components/PageShell";
import { httpError, type ApiError } from "@/shared/api/apiError";

type LecturesPageState =
  | { kind: "loading" }
  | { kind: "error"; error: ApiError }
  | { kind: "empty" }
  | { kind: "ready"; lectures: CourseLectureDto[] };

function isValidHttpUrl(url: string | undefined): boolean {
  if (!url) return false;
  try {
    const u = new URL(url);
    return u.protocol === "http:" || u.protocol === "https:";
  } catch {
    return false;
  }
}

export function CourseLecturesPage() {
  const { courseId } = useParams<{ courseId: string }>();
  const authCtx = useAuthContext();
  const token = authCtx.state.token ?? null;
  const [lecturesPageState, setLecturesPageState] = useState<LecturesPageState>({ kind: "loading" });
  const [badLinkOpened, setBadLinkOpened] = useState(false);
  const refController = useRef<AbortController>(null);

  const load = useCallback(async () => {
    if (!courseId || !token) {
      // Defensive: RouteGuard should prevent entering here without token.
      setLecturesPageState({ kind: "error", error: httpError("parse", "Missing courseId or token"), });
      return;
    }

    refController.current?.abort();
    const abortController = new AbortController();
    refController.current = abortController;

    try {
      setLecturesPageState({ kind: "loading" });

      const lectures: CourseLectureDto[] = await getAllLecturesByCourseIdOrdered(courseId, token, abortController.signal);

      if (!lectures || lectures.length === 0) {
        setLecturesPageState({ kind: "empty" });
        return;
      }

      setLecturesPageState({ kind: "ready", lectures });
    } catch (e) {
      if (abortController.signal.aborted) return;
      setLecturesPageState({ kind: "error", error: e as ApiError });
    }

  }, [courseId, token]);

  useEffect(() => {
    load();

    return () => refController.current?.abort();
  }, [load]);

  const retry = load;

  return (
    <Box p="md">
      <Modal
        opened={badLinkOpened}
        onClose={() => setBadLinkOpened(false)}
        title="Ссылка недоступна"
        centered
      >
        <Text size="sm">Похоже, у этой лекции нет корректной ссылки на видео.</Text>
      </Modal>

      <PageShell
        state={lecturesPageState.kind}
        loadingView={loadingView}
        emptyView={emptyView}
        errorText="Проблема с сервером. Не могу получить информацию о лекциях курса."
        error={lecturesPageState.kind === "error" ? lecturesPageState.error : undefined}
        onRetry={retry}
      >
        {lecturesPageState.kind === "ready" && readyView(lecturesPageState.lectures, setBadLinkOpened)}
      </PageShell>
    </Box>
  );
}

const loadingView = <Grid gutter="md">
  {Array.from({ length: 6 }).map((_, i) => (
    <Grid.Col key={i} span={{ base: 12, sm: 6, lg: 4 }}>
      <Card withBorder radius="md" padding="md">
        <Stack gap="xs">
          <Skeleton h={16} w={120} />
          <Skeleton h={16} w="80%" />
          <Skeleton h={16} w="70%" />
          <Skeleton h={32} />
        </Stack>
      </Card>
    </Grid.Col>
  ))}
</Grid>;

const emptyView = <Text>В этом курсе ещё нет лекций</Text>;

function readyView(lectures: CourseLectureDto[], setBadLinkOpened: (state: boolean) => void) {
  return (
    <Grid gutter="md">
      {lectures.map((lec) => {
        const ref = (lec.videoUrl ?? "").trim() || undefined;
        const isValid = isValidHttpUrl(ref);

        return (
          <Grid.Col key={lec.id} span={{ base: 12, sm: 6, lg: 4 }}>
            <a
              href={isValid ? ref : undefined}
              target="_blank"
              rel="noopener noreferrer"
              style={{ textDecoration: "none" }}
              onClick={(e) => {
                if (!isValid) {
                  e.preventDefault();
                  setBadLinkOpened(true);
                }
              }}
            >
              <Card
                withBorder
                radius="md"
                padding="md"
                role="button"
                tabIndex={0}
              >
                <Stack gap="xs">
                  <Group justify="space-between" align="center">
                    <Badge variant="light">Lecture № {lec.orderNo}</Badge>
                    <Badge variant="outline">▶</Badge>
                  </Group>
                  <Text fw={600} lineClamp={2}>
                    {lec.title}
                  </Text>
                  <Text size="sm" c="dimmed" lineClamp={3}>
                    {lec.description ?? ""}
                  </Text>
                  <Text size="xs" c="dimmed">
                    Нажмите ▶ для открытия видеоурока.
                  </Text>
                </Stack>
              </Card>
            </a>
          </Grid.Col>
        );
      })}
    </Grid>
  );
}
