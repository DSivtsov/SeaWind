import { useEffect, useMemo, useState } from "react";
import { Badge, Button, Card, Checkbox, Divider, Group, Radio, ScrollArea, Stack, Table, Text, TextInput, } from "@mantine/core";
import { useAuthContext } from "@/shared/auth/authContext";
import { useDebouncedValue } from "@/shared/hooks/useDebouncedValue";
import { PageShell } from "@/shared/components/PageShell";
import { getMentorExerciseChatInbox, type StudentExerciseDto } from "@/pages/mentor/mentorLayoutApi";
import { type ApiError } from "@/shared/api/apiError";
import type { UiZoneState } from "@/shared/components/ZoneShell";
import type { StudentExerciseStatus } from "@/pages/courses/exerciseChat/courseExerciseChatApi";
import { useNavigate } from "react-router-dom";

function statusToBadgeColor(status: StudentExerciseStatus, mark: number | null): string {
  if (status === "OnStudent" && mark !== null && mark > 0) return mark === 1 ? "blue" : "gold";
  if (status === "OnStudent") return "green";
  if (status === "OnMentor") return "red";
  return "gray";
}

const selectedRowBackground = "var(--mantine-color-blue-light)";

export function MentorExerciseChatInbox() {
  const navigate = useNavigate();
  const authCtx = useAuthContext();
  const token = authCtx.state.token;

  const [checkedOnlyOnCheck, setCheckedOnlyOnCheck] = useState(false);

  const [emailFilter, setEmailFilterFilter] = useState("");
  const debouncedEmail = useDebouncedValue(emailFilter, 500);

  const [studentExercise, setStudentExercise] = useState<UiZoneState<StudentExerciseDto[]>>({ kind: "loading" });
  const [selectedStudentExerciseId, setSelectedStudentExerciseId] = useState<string | null>(null);

  const selectedStudentExerciseData = useMemo<StudentExerciseDto | null>(() => {
    if (studentExercise.kind !== "ready" || !selectedStudentExerciseId) return null;
    return studentExercise.data.find((rec) => rec.studentExerciseId === selectedStudentExerciseId) ?? null;
  }, [studentExercise, selectedStudentExerciseId]);

  async function loadStudentExercises(ac: AbortController) {
    if (!token) {
      setStudentExercise({ kind: "error", message: "Missing data or token" });
      return;
    }

    setStudentExercise({ kind: "loading" });
    try {
      const data = await getMentorExerciseChatInbox(debouncedEmail, checkedOnlyOnCheck, token, ac.signal);

      if (data.length === 0) {
        setStudentExercise({ kind: "empty" });
        return;
      }

      setStudentExercise({ kind: "ready", data });

      // Drop selection if user disappeared from filtered result
      setSelectedStudentExerciseId((prev) => (prev && data.some(u => u.studentExerciseId === prev) ? prev : null));

    } catch (err) {
      if (ac.signal.aborted) return;
      console.error(err);
      setStudentExercise({ kind: "error", message: (err as ApiError).message });
    }
  }

  useEffect(() => {
    const ac = new AbortController();

    loadStudentExercises(ac);

    return () => ac.abort();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [token, debouncedEmail, checkedOnlyOnCheck]);

  async function onOpenStudentExercise() {
    if (!selectedStudentExerciseData) return;

    navigate(`/exercises/${selectedStudentExerciseData.exerciseId}/dashboard?studentId=${selectedStudentExerciseData.studentId}`,
      {
        state: { from: "/mentor/exercises-chats-inbox" },
      });
  }

  function renderRowStudentExercise({ studentExerciseId: id, email, exerciseTitle, status, mark }: StudentExerciseDto) {
    const isSelected = selectedStudentExerciseId === id;

    return (
      <Table.Tr
        key={id}
        onClick={() => setSelectedStudentExerciseId((prev) => (prev === id ? null : id))}
        style={{
          cursor: "pointer",
          background: isSelected ? selectedRowBackground : undefined,
        }}
      >
        <Table.Td onClick={(e) => e.stopPropagation()}>
          <Radio
            name="student-exercise-select"
            checked={isSelected}
            onChange={() => setSelectedStudentExerciseId((prev) => (prev === id ? null : id))}
          />
        </Table.Td>
        <Table.Td>{email}</Table.Td>
        <Table.Td>{exerciseTitle}</Table.Td>
        <Table.Td>
          <Badge variant="light" color={statusToBadgeColor(status, mark)}>
            {status}
          </Badge>
        </Table.Td>
      </Table.Tr>
    );
  }

  function renderTableStudentExercises(studentExercises: StudentExerciseDto[]) {
    return (
      <ScrollArea h={420} type="auto">
        <Table withTableBorder highlightOnHover stickyHeader stickyHeaderOffset={1}>
          <Table.Thead>
            <Table.Tr>
              <Table.Th w={44} />
              <Table.Th>Email</Table.Th>
              <Table.Th>Название</Table.Th>
              <Table.Th w={140}>Статус</Table.Th>
            </Table.Tr>
          </Table.Thead>
          <Table.Tbody>
            {studentExercises.map((row) => { return renderRowStudentExercise(row); })}
          </Table.Tbody>
        </Table>
      </ScrollArea>
    );
  };

  const selectedStudentExercise = (
    <Group justify="center">
      <Card withBorder>
        <Stack gap="sm">
          <Text fw={600}>Студент и упражнение</Text>
          <Divider />
          {!selectedStudentExerciseData ? (
            <Text c="dimmed">Выбери упражнение студента в таблице</Text>
          ) : (
            <>
              <Group>
                <Stack>
                  <Group gap="xs" justify="space-between">
                    <Text size="sm" c="dimmed">
                      Email студента
                    </Text>
                    <Text size="sm">{selectedStudentExerciseData.email}</Text>
                  </Group>
                  <Stack gap="2" >
                    <Text size="sm" c="dimmed">
                      Название
                    </Text>
                    <Text>{selectedStudentExerciseData.exerciseTitle}</Text>
                  </Stack>
                </Stack>
                <Divider orientation="vertical" />
                <Stack
                  align="center"
                  justify="center"
                  gap="xs">
                  <Button variant="filled" color="green"
                    onClick={onOpenStudentExercise}
                    size="md"
                  >
                    Открыть чат
                    <br />
                    со студентом
                  </Button>
                </Stack>
              </Group>
              <Divider />
              <Stack gap="2" >
                <Text size="sm" c="dimmed">
                  Краткое описание
                </Text>
                <Text maw={500}>
                  {selectedStudentExerciseData.exerciseShortDescription}
                </Text>
              </Stack >
            </>
          )}
        </Stack>
      </Card>
    </Group>
  );

  return (
    <PageShell
      state={studentExercise.kind}
      errorText={"Проблема с сервером. Попробуйте позже."}
    >
      {/* Filter zone */}
      <Stack gap="md">
        <Card withBorder>
          <Group align="end" justify="space-between" wrap="wrap">
            <TextInput
              label="Email студента"
              placeholder="Фильтрация по email студента"
              value={emailFilter}
              onChange={(e) => setEmailFilterFilter(e.currentTarget.value)}
              w={340}
            />
            <Checkbox
              label="Показать только ожидающие проверки"
              checked={checkedOnlyOnCheck}
              onChange={(event) => setCheckedOnlyOnCheck(event.currentTarget.checked)}
            />
            <Button
              variant="default"
              onClick={() => {
                setEmailFilterFilter("");
                setCheckedOnlyOnCheck(false);
                setSelectedStudentExerciseId(null);
              }}
            >
              Очистить
            </Button>
          </Group>
        </Card>

        {/* TableStudentExercise zone */}
        <Card withBorder>
          <Stack gap="sm">
            <Text fw={600}>Упражнения студентов</Text>
            {studentExercise.kind === "ready" ? renderTableStudentExercises(studentExercise.data) : null}
          </Stack>
        </Card>

        {/* Selected StudentExercise */}
        {selectedStudentExercise}
      </Stack>

    </PageShell>
  );
}
