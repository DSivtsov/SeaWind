import { Tabs } from "@mantine/core";
import { NavLink, type NavLinkProps } from "react-router-dom";

type TabsNavLinkTabProps = {
    tabsTabPropsValue: string;
    navLinkPropsTo: NavLinkProps["to"];
    children: React.ReactNode;
};

export function TabsNavLinkTab({
    tabsTabPropsValue: value,
    navLinkPropsTo: to,
    children,
}: TabsNavLinkTabProps) {
    return (
        <Tabs.Tab
            value={value}
            renderRoot={(props) => <NavLink {...props} to={to} />}
        >
            {children}
        </Tabs.Tab>
    );
}
