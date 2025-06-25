import {
    ArrowRightIcon,
    GameControllerIcon,
    GlobeIcon,
    RabbitIcon,
} from "@phosphor-icons/react";
import { createFileRoute } from "@tanstack/react-router";
import {
    Container,
    Box,
    Image,
    HStack,
    Text,
    Badge,
    Motion,
    RadioCardGroup,
    RadioCard,
    RadioCardLabel,
} from "@yamada-ui/react";
import { PagingTable, type Column } from "@yamada-ui/table";
import { useEffect, useMemo, useState } from "react";
import {
    useStandingsGetAvailableStandingsEndpoints,
    useStandingsGetStandingsEndpoint,
    type TeamStanding,
} from "~/api";
export const Route = createFileRoute("/")({
    component: Index,
});

function Index() {
    const columns = useMemo<Column<TeamStanding>[]>(
        () => [
            {
                header: "Position",
                accessorKey: "standings",
                size: 1,
                maxSize: 1,
                cell: ({ getValue }) => (
                    <Box
                        backdropBlur="2xl"
                        rounded="lg"
                        background={"pink"}
                        textAlign="center"
                    >
                        <Text>{getValue<string>()}</Text>
                    </Box>
                ),
            },
            {
                header: "Team",
                accessorKey: "teamName",
                enableSorting: false,
                cell: (info) => (
                    <HStack gap={2}>
                        <Image
                            src="https://img-cdn.hltv.org/teamlogo/yeXBldn9w8LZCgdElAenPs.png?ixlib=java-2.1.0&w=50&s=15eaba0b75250065d20162d2cb05e3e6"
                            width={5}
                            height={5}
                            fit="contain"
                        />
                        <Text>{info.getValue<string>()}</Text>
                    </HStack>
                ),
            },
            {
                header: "Roster",
                accessorKey: "roster",
                enableSorting: false,
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
                cell: ({ getValue }) => (
                    <Text size="3xl">{getValue<string>()}</Text>
                ),
            },
        ],
        []
    );

    const [region, setRegion] = useState<string>("Global");
    const [date, setDate] = useState<string>("");

    const {
        data: availableStandingsData,
        error: availableStandingsError,
        isPending: availableStandingsPending,
    } = useStandingsGetAvailableStandingsEndpoints();

    useEffect(() => {
        if (availableStandingsData?.data.regionWithDates?.length) {
            const defaultRegion =
                availableStandingsData.data.regionWithDates.find(
                    (r) =>
                        r.region.toLocaleLowerCase() ===
                        region.toLocaleLowerCase()
                );

            if (defaultRegion && defaultRegion.dates.length > 0) {
                setDate(defaultRegion.dates[0]!);
            }
        }
    }, [region, availableStandingsData?.data]);

    const { data, error, isPending } = useStandingsGetStandingsEndpoint(
        region,
        date,
        {
            query: {
                enabled: !!date,
            },
        }
    );

    if (isPending) return "Loading...";

    if (error) return "An error has occurred: " + error.message;

    return (
        <Container
            rounded="2xl"
            variant="surface"
            bg={["blackAlpha.50", "whiteAlpha.100"]}
        >
            <RadioCardGroup w="fit-content" withIcon={false}>
                <RadioCard value="Global" rounded="xl">
                    <RadioCardLabel>
                        <HStack gap="sm">
                            <RabbitIcon color="muted" fontSize="2xl" />
                            <Text>Global</Text>
                        </HStack>
                    </RadioCardLabel>
                </RadioCard>

                <RadioCard value="Europe" rounded="xl">
                    <HStack gap="sm">
                        <GlobeIcon color="muted" fontSize="2xl" />
                        <Text>Europe</Text>
                    </HStack>
                </RadioCard>

                <RadioCard value="Americas" rounded="xl">
                    <RadioCardLabel>
                        <HStack gap="sm">
                            <GameControllerIcon color="muted" fontSize="2xl" />
                            <Text>Americas</Text>
                        </HStack>
                    </RadioCardLabel>
                </RadioCard>

                <RadioCard value="Asia" rounded="xl">
                    <RadioCardLabel>
                        <HStack gap="sm">
                            <GameControllerIcon color="muted" fontSize="2xl" />
                            <Text>Asia</Text>
                        </HStack>
                    </RadioCardLabel>
                </RadioCard>
            </RadioCardGroup>

            <PagingTable
                whiteSpace={{ base: "inherit", lg: "nowrap" }}
                colorScheme="emerald"
                headerProps={{
                    bg: "transparent",
                    borderBottom: "none",
                    pb: "5",
                    color: "gray.300",
                }}
                rowProps={{
                    _hover: { bg: "whiteAlpha.100" },

                    borderBottom: "none",
                }}
                cellProps={{
                    borderBottom: "none",
                    py: 3,
                }}
                rounded="lg"
                size="md"
                columns={columns}
                data={data.data.standings ?? []}
                enableRowSelection={false}
                selectProps={{ variant: "flushed" }}
                pagingControlProps={{
                    py: 4,
                }}
            />
        </Container>
    );
}

/*

            bgGradient={[
                "linear(to-br, gray.700, blue.700)",
                "linear(to-br, blue.700, gray.700)",
            ]}

                bg="transparentize(gray.800, 50%)"
*/
