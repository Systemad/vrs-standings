import { defineConfig } from "@kubb/core";
import { pluginOas } from "@kubb/plugin-oas";
import { pluginReactQuery } from "@kubb/plugin-react-query";
import { pluginTs } from "@kubb/plugin-ts";
export default defineConfig(() => {
    return {
        root: ".",
        input: {
            //path: "openapi.json",
            path: "http://localhost:5222/openapi/v1.json",
            //path: "../Ludus.Server/Schema/Ludus.Server.json",
        },
        output: {
            path: "./src/api",
        },
        plugins: [
            pluginOas(),
            pluginTs({}),
            pluginReactQuery({
                output: {
                    path: "./hooks",
                },
                group: {
                    type: "tag",
                    name: ({ group }) => `${group}Hooks`,
                },
                client: {
                    dataReturnType: "full",
                    baseURL: "",
                },
                mutation: {
                    methods: ["post", "put", "delete"],
                },
                infinite: {
                    queryParam: "pageNumber",
                    initialPageParam: 1,
                    cursorParam: "pageNumber",
                },
                query: {
                    methods: ["get"],
                    importPath: "@tanstack/react-query",
                },
                suspense: {},
            }),
        ],
    };
});
