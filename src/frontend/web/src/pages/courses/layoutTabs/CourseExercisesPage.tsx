import { useCallback, useEffect, useRef, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Badge, Box, Card, Grid, Group, Skeleton, Stack, Text, Tooltip } from "@mantine/core";
import { useAuthContext } from "@/shared/auth/authContext";
import { getAllExercisesByCourseIdOrdered, type CourseExerciseDto }
  from "@/pages/courses/layoutTabs/courseLayoutTabsApi";
import { PageShell } from "@/shared/components/PageShell";
import { httpError, type ApiError } from "@/shared/api/apiError";
import { IconSquare, IconSquareCheck } from "@tabler/icons-react";

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
  const refController = useRef<AbortController>(null);

  const userRole = authCtx.me.kind === "ready" ? authCtx.me.user.role : null;
  const isStudent = userRole === "Student";

  const load = useCallback(async () => {
    if (!courseId || !token) {
      // Defensive: RouteGuard should prevent entering here without token.
      setExercisesPageState({ kind: "error", error: httpError("parse", "Missing courseId or token"), });
      return;
    }

    refController.current?.abort();
    const abortController = new AbortController();
    refController.current = abortController;

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
  }, [courseId, token]);

  useEffect(() => {

    load();

    return () => refController.current?.abort();
  }, [load]);

  const retry = load;

  const openExerciseDashboard = (exerciseId: string) => {
    if (isStudent) {
      navigate(`/exercises/${exerciseId}/dashboard`, {
        state: { from: `/courses/${courseId}/exercises` }
      });
    }
  };

  return (
    <Box p="md">
      <PageShell
        state={exercisesPageState.kind}
        loadingView={loadingView}
        emptyView={emptyView}
        errorText="Проблема с сервером. Не могу получить информацию об упражнениях курса."
        error={exercisesPageState.kind === "error" ? exercisesPageState.error : undefined}
        onRetry={retry}
      >
        {exercisesPageState.kind === "ready" && readyView(exercisesPageState.exercises, isStudent, openExerciseDashboard)}
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

function readyView(exercises: CourseExerciseDto[], isStudent: boolean, openExerciseDashboard: (exerciseId: string) => void) {

  const showMark = (mark: number | null) => {
    if (!isStudent || mark === null) return null;

    const completed = mark > 0;

    const icon = completed
      ? <IconSquareCheck color="green" />
      : <IconSquare color="gray" />;

    const label = completed
      ? "Упражнение зачтено"
      : "Упражнение не начато";

    const color = completed ? "green" : "gray";

    return (
      <Tooltip label={label} color={color}>
        {icon}
      </Tooltip>
    );
  };

  return <Grid gutter="md">
    {exercises.map((exercise) => (
      <Grid.Col key={exercise.id} span={{ base: 12, sm: 6, lg: 4 }}>
        <Card
          withBorder
          radius="md"
          padding="md"
          role="button"
          tabIndex={0}
          onClick={() => openExerciseDashboard(exercise.id)}
          onKeyDown={(e) => {
            if (e.key === "Enter" || e.key === " ") openExerciseDashboard(exercise.id);
          }}
        >
          <Stack gap="xs">
            <Group justify="space-between" align="center">
              <Group>
                <Badge variant="light">Exercise № {exercise.orderNo}</Badge>
                {showMark(exercise.mark)}
              </Group>
              <Badge variant="outline">▶</Badge>
            </Group>
            <Text fw={600} lineClamp={2}>
              {exercise.title}
            </Text>
            <Text size="sm" c="dimmed" lineClamp={3}>
              {exercise.shortDescription ?? ""}
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
