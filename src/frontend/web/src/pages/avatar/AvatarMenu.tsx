import { useMemo } from "react";
import { useNavigate } from "react-router-dom";
import { ActionIcon, Avatar, Badge, Divider, Group, Loader, Menu, Stack, Text, } from "@mantine/core";
import { IconDoorExit, IconLayoutDashboard, IconUser } from "@tabler/icons-react";
import { useAuth } from "@/shared/auth/useAuth";

type WorkspaceTarget = "/admin" | "/mentor";

function getWorkspaceTarget(role: string): WorkspaceTarget | null {
  if (role === "Admin") return "/admin";
  if (role === "Mentor") return "/mentor";
  return null;
}

function toRoleLabel(role: string): string {
  return role;
}

export function AvatarMenu() {
  const auth = useAuth();
  const navigate = useNavigate();

  const isAuthed = auth.isAuthenticated;
  const me = auth.me;

  const ui = useMemo(() => {
    if (me.kind === "loading") return { kind: "loading" as const };

    if (me.kind === "error") return { kind: "error" as const };

    if (me.kind === "ready") {
      const { email, role } = me.user;
      const workspaceTarget = getWorkspaceTarget(role);
      return {
        kind: "ready" as const,
        email,
        role,
        roleLabel: toRoleLabel(role),
        workspaceTarget,
      };
    }

    // MVP: should not happen when authed, but keep it safe.
    return { kind: "empty" as const };
  }, [me]);

  const menuDisabled = ui.kind === "loading" || ui.kind === "empty";

  const onOpenProfile = () => navigate("/profile");
  const onOpenWorkspace = () => {
    if (ui.kind !== "ready") return;
    if (!ui.workspaceTarget) return;
    navigate(ui.workspaceTarget);
  };
  const onLogout = () => {
    auth.logout();
  };
  console.log(`{disabled}=[${!isAuthed}]`);
  console.log(`{kind}=[${ui.kind}]`);
  console.log(`{role}=[${ui.role}]`);
  console.log(`{role}=[${ui.email}]`);
  console.log(`{roleLabel}=[${ui.roleLabel}]`);

  const avatarVariant = isAuthed ? "filled" : "transparent";
  const avatarSize = isAuthed ? "lg" : "md";
  const avatarColor = isAuthed ? "green" : undefined;
  const avatarText = isAuthed ? auth.state.initials : "";
  return (
    <Menu width={280} position="bottom-end" shadow="md" withArrow>
      <Menu.Target>
        <ActionIcon variant="transparent" size="xl" radius="xl" aria-label="Open user menu" disabled={!isAuthed}>
          <Avatar variant={avatarVariant} size={avatarSize} color={avatarColor} radius="xl">{avatarText}</Avatar>
        </ActionIcon>
      </Menu.Target>

      <Menu.Dropdown>
        {ui.kind === "loading" && (
          <Group gap="sm" px="sm" py="xs">
            <Loader size="sm" />
            <Text size="sm">Loading…</Text>
          </Group>
        )}

        {ui.kind === "error" && (
          <Stack gap={4} px="sm" py="xs">
            <Text size="sm" fw={600}>
              Problem with server
            </Text>
            <Text size="sm" c="dimmed">
              Please try again later.
            </Text>
          </Stack>
        )}

        {ui.kind === "ready" && (
          <Stack gap={6} px="sm" py="xs">
            <Group justify="space-between" align="center">
              <Text size="sm" fw={600} lineClamp={1}>
                {ui.email}
              </Text>
              <Badge variant="light">{ui.roleLabel}</Badge>
            </Group>
          </Stack>
        )}

        <Divider my="xs" />

        <Menu.Item
          leftSection={<IconUser size={16} />}
          disabled={menuDisabled}
          onClick={onOpenProfile}
        >
          Profile
        </Menu.Item>

        <Menu.Item
          leftSection={<IconLayoutDashboard size={16} />}
          disabled={menuDisabled || ui.kind !== "ready" || !ui.workspaceTarget}
          onClick={onOpenWorkspace}
        >
          Workspace
        </Menu.Item>

        <Divider my="xs" />

        <Menu.Item
          color="red"
          leftSection={<IconDoorExit size={16} />}
          disabled={menuDisabled}
          onClick={onLogout}
        >
          Logout
        </Menu.Item>
      </Menu.Dropdown>
    </Menu>
  );
}
