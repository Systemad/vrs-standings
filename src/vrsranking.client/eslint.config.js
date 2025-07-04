import js from "@eslint/js";
import globals from "globals";
import reactHooks from "eslint-plugin-react-hooks";
import reactRefresh from "eslint-plugin-react-refresh";
import tseslint from "typescript-eslint";
import pluginQuery from "@tanstack/eslint-plugin-query";
import prettierConfig from "eslint-config-prettier";
import pluginRouter from "@tanstack/eslint-plugin-router";

export default tseslint.config(
    prettierConfig,
    ...pluginRouter.configs["flat/recommended"],
    ...pluginQuery.configs["flat/recommended"],
    {
        ignores: [
            "dist",
            "**/routeTree.gen.ts",
            "**/routeTree.gen.ts",
            "**/api/**",
        ],
    },
    {
        extends: [js.configs.recommended, ...tseslint.configs.recommended],
        files: ["**/*.{ts,tsx}"],
        languageOptions: {
            ecmaVersion: 2020,
            globals: globals.browser,
        },
        plugins: {
            "react-hooks": reactHooks,
            "react-refresh": reactRefresh,
        },
        rules: {
            ...reactHooks.configs.recommended.rules,
            "react-refresh/only-export-components": [
                "warn",
                { allowConstantExport: true },
            ],
        },
    }
);
