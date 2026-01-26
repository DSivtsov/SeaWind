import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { Badge, Box, Card, Grid, Group, Modal, Skeleton, Stack, Text } from "@mantine/core";
import { useAuthContext } from "@/shared/auth/authContext";
import { getAllLecturesByCourseIdOrdered, type CourseLectureDto } from "@/pages/courses/layoutTabs/CourseLayoutTabsApi";
import { PageShell } from "@/shared/components/PageShell";

type LecturesPageState =
  | { kind: "loading" }
  | { kind: "error" }
  | { kind: "empty" }
  | { kind: "ready"; lectures: CourseLectureDto[] };

function isValidHttpUrl(url: string): boolean {
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

  useEffect(() => {
    if (!courseId || !token) {
      // Defensive: RouteGuard should prevent entering here without token.
      setLecturesPageState({ kind: "error" });
      return;
    }
    const abortController = new AbortController();

    (async function () {
      try {
        setLecturesPageState({ kind: "loading" });

        const lectures: CourseLectureDto[] = await getAllLecturesByCourseIdOrdered(courseId, token, abortController.signal);
        //lectures = (() => [...lectures].sort((a, b) => a.orderNo - b.orderNo))();

        if (!lectures || lectures.length === 0) {
          setLecturesPageState({ kind: "empty" });
          return;
        }

        setLecturesPageState({ kind: "ready", lectures });
      } catch {
        if (abortController.signal.aborted) return;
        setLecturesPageState({ kind: "error" });
      }
    })();

    return () => abortController.abort();
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
        errorText="Проблема с сервером. Попробуйте позже."
      >
        {lecturesPageState.kind === "ready" && readyView(lecturesPageState.lectures, openLectureVideo)}
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

function readyView(
  lectures: CourseLectureDto[],
  openLectureVideo: (lecture: CourseLectureDto) => void) {
  return <Grid gutter="md">
    {lectures.map((lec) => (
      <Grid.Col key={lec.id} span={{ base: 12, sm: 6, lg: 4 }}>
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
              Click to open video
            </Text>
          </Stack>
        </Card>
      </Grid.Col>
    ))}
  </Grid>;
}
