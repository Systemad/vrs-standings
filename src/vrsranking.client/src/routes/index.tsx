import { createFileRoute } from "@tanstack/react-router";
import {
    Container,
    Heading,
    Wrap,
    Box,
    Image,
    HStack,
    Text,
    Badge,
} from "@yamada-ui/react";
import { PagingTable, type Column } from "@yamada-ui/table";
import { useMemo, useState } from "react";
import {
    useStandingsGetStandingsEndpoint,
    type FeaturesRankingsModelsTeamStanding,
} from "~/api";
export const Route = createFileRoute("/")({
    component: Index,
});

function Index() {
    const columns = useMemo<Column<FeaturesRankingsModelsTeamStanding>[]>(
        () => [
            {
                header: "Position",
                accessorKey: "standings",
                cell: ({ getValue }) => (
                    <Box backdropBlur="2xl" background={"pink"}>
                        <Text>{getValue<string>()}</Text>
                    </Box>
                ),
            },
            {
                header: "Team",
                accessorKey: "teamName",
                cell: ({ getValue }) => (
                    <HStack gap={2}>
                        <Image
                            src="https://assets.blast.tv/images/teams/9b29b611-6523-452d-9bc6-b93fbdb01796?height=auto&width=auto&format=auto"
                            width={5}
                            height={5}
                            fit="contain"
                        />
                        <Text>{getValue<string>()}</Text>
                    </HStack>
                ),
            },
            {
                header: "Roster",
                accessorKey: "roster",
                cell: ({ getValue }) => (
                    <HStack gap={2}>
                        {getValue<string[]>().map((player, index) => (
                            <Badge key={index} rounded="md">
                                {player}
                            </Badge>
                        ))}
                    </HStack>
                ),
            },
            {
                header: "Points",
                accessorKey: "points",
            },
        ],
        []
    );

    const [region, setRegion] = useState<string>("global");

    const { data, isError, error, isPending } =
        useStandingsGetStandingsEndpoint("global", "2025_06_02");

    if (isPending) return "Loading...";

    if (error) return "An error has occurred: " + error.message;

    return (
        <Container centerContent>
            <Heading>VRS Rankings for Counter-Strike</Heading>

            <Wrap gap="md">
                <Box key={`na`} p="md" rounded="md" bg={"blue"} color="white">
                    Box
                </Box>

                <Box
                    key={`global`}
                    p="md"
                    rounded="md"
                    bg={"blue"}
                    color="white"
                >
                    Box
                </Box>

                <Box key={`asia`} p="md" rounded="md" bg={"blue"} color="white">
                    Box
                </Box>

                <Box
                    key={`europe`}
                    p="md"
                    rounded="md"
                    bg={"blue"}
                    color="white"
                >
                    Box
                </Box>
            </Wrap>
            <Box maxW="8xl" w="full" mx="auto" overflowX="auto" mt="4">
                <PagingTable
                    size="md"
                    columns={columns}
                    data={data.data.standings ?? []}
                    enableRowSelection={false}
                    paginationProps={{ variant: "outline" }}
                />
            </Box>
        </Container>
    );
}
