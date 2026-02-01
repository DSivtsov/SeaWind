import { NavLink, Text } from "@mantine/core";
import { NavLink as RouterNavLink } from "react-router-dom";

export type NavItemProps = {
    linkTo: string;
    linkLabel: string;
};

export function NavItem({ linkTo, linkLabel }: NavItemProps) {
    return (
        <RouterNavLink to={linkTo} style={{ textDecoration: "none" }}>
            {({ isActive }) => (
                <NavLink
                    label={<Text ta="center">{linkLabel}</Text>}
                    active={isActive}
                />
            )}
        </RouterNavLink>
    );
}
