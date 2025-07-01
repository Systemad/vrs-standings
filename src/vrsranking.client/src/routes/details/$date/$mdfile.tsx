import {
    createFileRoute,
    ErrorComponent,
    type ErrorComponentProps,
} from "@tanstack/react-router";
import { Markdown } from "@yamada-ui/markdown";
import { Container } from "@yamada-ui/react";
import { standingsGetDetailEndpointQueryOptions } from "~/api";

export const Route = createFileRoute("/details/$date/$mdfile")({
    loader: ({ context: { queryClient }, params: { date, mdfile } }) => {
        return queryClient.ensureQueryData(
            standingsGetDetailEndpointQueryOptions(date, mdfile)
        );
    },
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    errorComponent: PostErrorComponent as any,
    component: RouteComponent,
});

export function PostErrorComponent({ error }: ErrorComponentProps) {
    return <ErrorComponent error={error} />;
}

function RouteComponent() {
    const { markdownString } = Route.useLoaderData();

    return (
        <Container rounded="2xl" bg={["blackAlpha.50", "whiteAlpha.100"]}>
            <Markdown>{markdownString}</Markdown>;
        </Container>
    );
}
