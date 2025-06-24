import { extendTheme, generate } from "@yamada-ui/react";

export const theme = extendTheme({
    colors: {
        red: generate.tones("##dd6b63"),
    },
})();
