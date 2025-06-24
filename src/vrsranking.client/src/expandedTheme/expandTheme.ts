import { extendTheme } from "@yamada-ui/react";
import type { UsageTheme } from "@yamada-ui/react";
import { expandedSemantics } from "./semantics";
import { expandedGlobalStyle } from "./styles/global-style";

const customTheme: UsageTheme = {
    expandedSemantics,
    styles: { expandedGlobalStyle },
    // styles,
    // components,
    // ...tokens,
};

export const expandedTheme = extendTheme(customTheme)();
