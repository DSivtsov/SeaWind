import { useEffect, useMemo, useState } from "react";
import {
  Badge, Button, Card, Divider, Group, Radio, ScrollArea, Select, Stack, Table,
  Text, TextInput, Tooltip,
} from "@mantine/core";
import { notifications } from "@mantine/notifications";
import { useAuthContext } from "@/shared/auth/authContext";
import type { Role } from "@/shared/auth/meApi";
import { useDebouncedValue } from "@/shared/hooks/useDebouncedValue";
import { PageShell } from "@/shared/components/PageShell";
import { fetchUsers, putUserRole, type UserRowDto } from "@/pages/admin/AdminLayoutApi";
import { isAbortError } from "@/shared/api/apiError";

type UsersState =
  | { kind: "loading" }
  | { kind: "empty" }
  | { kind: "error" }
  | { kind: "ready"; users: UserRowDto[] };

const ROLE_OPTIONS: Array<{ value: Role; label: string }> = [
  { value: "FreeStudent", label: "FreeStudent" },
  { value: "Student", label: "Student" },
  { value: "Mentor", label: "Mentor" },
  { value: "Admin", label: "Admin" },
];

const ALL_ROLES_VALUE = "ALL_ROLES" as const;
type RoleFilter = Role | typeof ALL_ROLES_VALUE;

function roleToBadgeColor(role: Role): string {
  if (role === "Admin") return "grape";
  if (role === "Mentor") return "cyan";
  if (role === "Student") return "blue";
  return "gray";
}

const ROLE_FILTER_DATA = [
  { value: ALL_ROLES_VALUE, label: "All Roles" },
  ...ROLE_OPTIONS.map((x) => ({ value: x.value, label: x.label })),
];

const selectedRowBackground = "var(--mantine-color-blue-light)";


export function AdminManageUserRoles() {
  const authCtx = useAuthContext();
  const token = authCtx.state.token ?? null;

  const [roleFilter, setRoleFilter] = useState<RoleFilter>(ALL_ROLES_VALUE);

  const [userNameFilter, setUserNameFilter] = useState("");
  const debouncedUserName = useDebouncedValue(userNameFilter, 750);

  const apiRoleFilter: Role | null = roleFilter === ALL_ROLES_VALUE ? null : roleFilter;

  const [state, setState] = useState<UsersState>({ kind: "loading" });

  const [selectedUserID, setSelectedUserID] = useState<string | null>(null);

  const selectedUserData = useMemo<UserRowDto | null>(() => {
    if (state.kind !== "ready" || !selectedUserID) return null;
    return state.users.find((u) => u.id === selectedUserID) ?? null;
  }, [state, selectedUserID]);

  const [newRole, setNewRole] = useState<Role | null>(null);
  const [isChanging, setIsChanging] = useState(false);

  async function loadUsers(signal?: AbortSignal) {
    setState({ kind: "loading" });
    try {
      const users = await fetchUsers({
        token, userName: debouncedUserName,
        role: apiRoleFilter, signal: signal,
      });

      if (users.length === 0) {
        setState({ kind: "empty" });
        return;
      }

      setState({ kind: "ready", users });

      // Drop selection if user disappeared from filtered result
      setSelectedUserID((prev) => (prev && users.some(u => u.id === prev) ? prev : null));

    } catch (err) {
      if (isAbortError(err)) return;
      setState({ kind: "error" });
    }
  }

  // Keep newRole in sync with selection (default = current role)
  useEffect(() => {
    if (!selectedUserData) {
      setNewRole(null);
      return;
    }
    setNewRole(selectedUserData.role);
  }, [selectedUserData]);

  useEffect(() => {
    const ac = new AbortController();

    loadUsers(ac.signal);

    return () => ac.abort();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [token, debouncedUserName, roleFilter]);

  const canSubmit =
    !isChanging &&
    selectedUserData !== null &&
    newRole !== null &&
    newRole !== selectedUserData.role;

  async function onSubmit() {
    if (!selectedUserData || !newRole) return;

    setIsChanging(true);
    try {
      await putUserRole({ token, userId: selectedUserData.id, role: newRole });

      notifications.show({
        title: "Success",
        message: "Role updated",
        color: "green",
      });

      await loadUsers();

    } catch {
      notifications.show({
        title: "Error",
        message: "Проблема с сервером. Попробуйте позже.",
        color: "red",
      });
    } finally {
      setIsChanging(false);
    }
  }

  function renderRowTableUsers({ id, userName, role }: UserRowDto) {
    const isSelected = selectedUserID === id;

    return (
      <Table.Tr
        key={id}
        onClick={() => setSelectedUserID((prev) => (prev === id ? null : id))}
        style={{
          cursor: "pointer",
          background: isSelected ? selectedRowBackground : undefined,
        }}
      >
        <Table.Td onClick={(e) => e.stopPropagation()}>
          <Radio
            name="admin-user-role-select"
            checked={isSelected}
            onChange={() => setSelectedUserID((prev) => (prev === id ? null : id))}
          />
        </Table.Td>
        <Table.Td>{userName}</Table.Td>
        <Table.Td>
          <Badge variant="light" color={roleToBadgeColor(role)}>
            {role}
          </Badge>
        </Table.Td>
      </Table.Tr>
    );
  }

  function renderTableUsers(users: UserRowDto[]) {
    return (
      <ScrollArea h={420} type="auto">
        <Table withTableBorder highlightOnHover>
          <Table.Thead>
            <Table.Tr>
              <Table.Th w={44} />
              <Table.Th>Email</Table.Th>
              <Table.Th w={140}>Role</Table.Th>
            </Table.Tr>
          </Table.Thead>
          <Table.Tbody>
            {users.map((userRow) => { return renderRowTableUsers(userRow); })}
          </Table.Tbody>
        </Table>
      </ScrollArea>
    );
  };


  const selectedPanel = (
    <Group justify="center">
      <Card withBorder>
        <Stack gap="sm">
          <Text fw={600}>Выбранный пользователь:</Text>
          <Divider />
          {!selectedUserData ? (
            <Text c="dimmed">Выбери пользователя в таблице</Text>
          ) : (
            <Group>
              <Stack>
                <Group gap="xs" justify="space-between">
                  <Text size="sm" c="dimmed">
                    Имя пользователя
                  </Text>
                  <Text size="sm">{selectedUserData.userName}</Text>
                </Group>
                <Group gap="xs" justify="space-between">
                  <Text size="sm" c="dimmed">
                    Текущая роль
                  </Text>
                  <Badge variant="light" color={roleToBadgeColor(selectedUserData.role)}>
                    {selectedUserData.role}
                  </Badge>
                </Group>
              </Stack>

              <Divider orientation="vertical" />

              <Stack
                align="center"
                justify="center"
                gap="xs">
                <Tooltip label="Выбери другую роль для активации кнопки" disabled={canSubmit}>
                  <Button variant="filled" color="green" className="layout-publicDrawer"
                    onClick={onSubmit}
                    disabled={!canSubmit}
                    loading={isChanging}
                  >
                    Изменить роль
                  </Button>
                </Tooltip>
                <Select
                  label="Новая роль"
                  data={ROLE_OPTIONS.map((x) => ({ value: x.value, label: x.label }))}
                  value={newRole ?? selectedUserData.role}
                  onChange={(v) => setNewRole((v as Role) ?? selectedUserData.role)}
                  allowDeselect={false}
                  disabled={isChanging}
                />
              </Stack>
            </Group>
          )}
        </Stack>
      </Card>
    </Group>
  );

  return (
    <PageShell
      state={state.kind}
      errorText={"Проблема с сервером. Попробуйте позже."}
    >
      {/* Filter zone */}
      <Stack gap="md">
        <Card withBorder>
          <Group align="end" justify="space-between" wrap="wrap">
            <TextInput
              label="Имя пользователя"
              placeholder="Фильтрация по имени пользователя"
              value={userNameFilter}
              onChange={(e) => setUserNameFilter(e.currentTarget.value)}
              w={340}
            />

            <Select
              label="Роль"
              data={ROLE_FILTER_DATA}
              value={roleFilter}
              onChange={(v) => setRoleFilter((v as RoleFilter))}
              allowDeselect={false}
              w={220}
            />

            <Button
              variant="default"
              onClick={() => {
                setUserNameFilter("");
                setRoleFilter(ALL_ROLES_VALUE);
                setSelectedUserID(null);
              }}
            >
              Очистить
            </Button>
          </Group>
        </Card>

        {/* TableUsers zone */}
        <Card withBorder>
          <Stack gap="sm">
            <Text fw={600}>Users</Text>
            {state.kind === "ready" ? renderTableUsers(state.users) : null}
          </Stack>
        </Card>

        {/* Selected / change zone */}
        {selectedPanel}
      </Stack>

    </PageShell>
  );
}
