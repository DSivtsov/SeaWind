import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Badge, Box, Card, Grid, Group, Skeleton, Stack, Text } from "@mantine/core";
import { useAuthContext } from "@/shared/auth/authContext";
import { getAllExercisesByCourseIdOrdered, type CourseExerciseDto }
  from "@/pages/courses/layoutTabs/courseLayoutTabsApi";
import { PageShell } from "@/shared/components/PageShell";
import { httpError, type ApiError } from "@/shared/api/apiError";

type ExercisesPageState =
  | { kind: "loading" }
  | { kind: "error"; error: ApiError }
  | { kind: "empty" }
  | { kind: "ready"; exercises: CourseExerciseDto[] };

export function CourseExercisesPage() {
  const { courseId } = useParams<{ courseId: string }>();
  const authCtx = useAuthContext();
  const navigate = useNavigate();

  const token = authCtx.state.token ?? null;

  const [exercisesPageState, setExercisesPageState] = useState<ExercisesPageState>({ kind: "loading" });

  useEffect(() => {
    if (!courseId || !token) {
      // Defensive: RouteGuard should prevent entering here without token.
      setExercisesPageState({ kind: "error", error: httpError("parse", "Missing courseId or token"), });
      return;
    }
    const abortController = new AbortController();

    (async function () {
      try {
        setExercisesPageState({ kind: "loading" });

        const exercises: CourseExerciseDto[] = await getAllExercisesByCourseIdOrdered(courseId, token, abortController.signal);

        if (!exercises || exercises.length === 0) {
          setExercisesPageState({ kind: "empty" });
          return;
        }

        setExercisesPageState({ kind: "ready", exercises: exercises });
      } catch (e) {
        if (abortController.signal.aborted) return;
        setExercisesPageState({ kind: "error", error: e as ApiError });
      }
    })();

    return () => abortController.abort();
  }, [courseId, token]);


  const openExerciseChat = (exercise: CourseExerciseDto) => {
    //navigate(`${exercise.id}/chat`);
    navigate(`/exercises/${exercise.id}/chat`, {
      state: { from: `/courses/${courseId}/exercises` }
    });
    return;
  };

  return (
    <Box p="md">
      <PageShell
        state={exercisesPageState.kind}
        loadingView={loadingView}
        emptyView={emptyView}
        errorText="Проблема с сервером. Не могу получить информацию об упражнениях курса."
        error={exercisesPageState.kind === "error" ? exercisesPageState.error : undefined}
      >
        {exercisesPageState.kind === "ready" && readyView(exercisesPageState.exercises, openExerciseChat)}
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

const emptyView = <Text>В этом курсе ещё нет упражнений</Text>;

function readyView(
  exercises: CourseExerciseDto[],
  openExerciseChat: (lecture: CourseExerciseDto) => void) {
  return <Grid gutter="md">
    {exercises.map((lec) => (
      <Grid.Col key={lec.id} span={{ base: 12, sm: 6, lg: 4 }}>
        <Card
          withBorder
          radius="md"
          padding="md"
          role="button"
          tabIndex={0}
          onClick={() => openExerciseChat(lec)}
          onKeyDown={(e) => {
            if (e.key === "Enter" || e.key === " ") openExerciseChat(lec);
          }}
        >
          <Stack gap="xs">
            <Group justify="space-between" align="center">
              <Badge variant="light">Exercise № {lec.orderNo}</Badge>
              <Badge variant="outline">▶</Badge>
            </Group>
            <Text fw={600} lineClamp={2}>
              {lec.title}
            </Text>
            <Text size="sm" c="dimmed" lineClamp={3}>
              {lec.shortDescription ?? ""}
            </Text>
            <Text size="xs" c="dimmed">
              Нажмите ▶ для перехода к детальному описанию и сдаче упражнения.
            </Text>
          </Stack>
        </Card>
      </Grid.Col>
    ))}
  </Grid>;
}
