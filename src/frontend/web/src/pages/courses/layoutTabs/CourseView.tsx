import { COURSE_TAB_ORDER, COURSE_TABS, type CourseTab } from "@/pages/courses/layoutTabs/COURSE_TABS";
import type { CourseDto } from "@/pages/courses/list/CoursesApi";
import { TabsNavLinkTab } from "@/shared/components/TabsNavLinkTab";
import { useAppCtx } from "@/shared/layout/appCtx";
import { Group, Box, Stack, Grid, Select, Tabs, Text } from "@mantine/core";
import { useNavigate } from "react-router-dom";

function TabLabel(props: { tab: CourseTab; activeTab: CourseTab }) {
    const isActive = props.activeTab === props.tab;
    const text = COURSE_TABS[props.tab].text;
    return (
        <Text fw={isActive ? 600 : undefined} c={isActive ? undefined : "dimmed"} size="sm">
            {text}
        </Text>
    );
}

type ReadyCourseViewProps = {
    course: CourseDto,
    activeTab: CourseTab,
};

export function CourseView({ course, activeTab }: ReadyCourseViewProps) {
    const { id, title, description } = course;
    const { isNarrow } = useAppCtx();
    const navigate = useNavigate();
    return (
        <Group justify="space-between" wrap="nowrap">
            <Box flex={1} miw={0}>
                <Stack gap={"xs"}>
                    <Grid gutter="xs">
                        <Grid.Col span={{ base: 12, sm: 4 }}>
                            <Group gap="sm">
                                <Text size="xs" c="dimmed">Course code</Text>
                                <Text fw={500}>{id}</Text>
                            </Group>
                        </Grid.Col>
                        <Grid.Col span={{ base: 12, sm: 8 }}>
                            <Group gap="sm">
                                <Text size="xs" c="dimmed">Course Title</Text>
                                <Text fw={500}>{title}</Text>
                            </Group>
                        </Grid.Col>
                        <Grid.Col span={12}>
                            <Group gap="sm">
                                <Text size="xs" c="dimmed">Course description</Text>
                                <Text fw={500}>{description ?? ""}</Text>
                            </Group>
                        </Grid.Col>
                    </Grid>
                    {isNarrow &&
                        <Box mx="auto" maw={600}>
                            <Group align="center" gap="sm" wrap="nowrap">
                                <Text size="sm" c="dimmed" style={{ whiteSpace: "nowrap" }}>
                                    Выбери контент
                                </Text>
                                <Select classNames={{ input: 'course-tab-select' }} chevronColor="indigo"
                                    value={activeTab} onChange={(v) => v && navigate(v)}
                                    data={COURSE_TAB_ORDER.map(t => ({ value: t, label: COURSE_TABS[t].text }))}
                                    radius="xs"
                                    allowDeselect={false}
                                />
                            </Group>
                        </Box>
                    }
                </Stack>
            </Box>
            {!isNarrow &&
                <Tabs variant="pills" radius="xs" color="indigo" value={activeTab} orientation="vertical" placement="right">
                    <Tabs.List>
                        {COURSE_TAB_ORDER.map((item) => (
                            <TabsNavLinkTab key={item} tabsTabPropsValue={item} navLinkPropsTo={item}>
                                <TabLabel tab={item} activeTab={activeTab} />
                            </TabsNavLinkTab>
                        ))}
                    </Tabs.List>
                </Tabs>}
        </Group>
    );
}
