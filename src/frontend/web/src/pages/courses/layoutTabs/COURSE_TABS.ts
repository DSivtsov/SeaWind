
export type CourseTab = "lectures" | "exercises" | "workshops";

export const COURSE_TAB_ORDER: CourseTab[] = ["lectures", "exercises", "workshops"];


export const COURSE_TABS: Record<CourseTab, { text: string; }> = {
    lectures: { text: "Лекции" },
    exercises: { text: "Упражнения" },
    workshops: { text: "Семинары" },
};

export function isCourseTab(value: string): value is CourseTab {
    return value in COURSE_TABS;
}


export function getActiveTabText(activeTab: CourseTab): string {
    return COURSE_TABS[activeTab].text;
}

