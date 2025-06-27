import { createFileRoute } from "@tanstack/react-router";
import { Markdown } from "@yamada-ui/markdown";
import { Container } from "@yamada-ui/react";
import { useStandingsGetDetailEndpoint } from "~/api";

export const Route = createFileRoute("/details/$date/$mdfile")({
    component: RouteComponent,
});

function RouteComponent() {
    const { date, mdfile } = Route.useParams();

    const { data, isPending } = useStandingsGetDetailEndpoint(date, mdfile);

    if (isPending) return <>Loading</>;
    return (
        <Container rounded="2xl" bg={["blackAlpha.50", "whiteAlpha.100"]}>
            <Markdown>{data?.data.markdownString}</Markdown>;
        </Container>
    );
}
