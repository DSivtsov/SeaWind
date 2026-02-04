import { NavLink, Text } from "@mantine/core";
import { NavLink as RouterNavLink, useMatch, useResolvedPath } from "react-router-dom";

export type NavItemProps = {
    linkTo: string;
    linkLabel: string;
};

export function NavItem({ linkTo, linkLabel }: NavItemProps) {
    const resolved = useResolvedPath(linkTo);
    const match = useMatch({ path: resolved.pathname, end: true });

    return (
        <NavLink
            component={RouterNavLink}
            to={linkTo}
            label={<Text ta="center">{linkLabel}</Text>}
            active={Boolean(match)}
            style={{ textDecoration: "none" }}
        />
    );
}
