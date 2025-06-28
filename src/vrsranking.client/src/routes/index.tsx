import { createFileRoute, Link } from "@tanstack/react-router";
import {
    Container,
    HStack,
    Text,
    Option,
    RadioCardGroup,
    RadioCard,
    RadioCardLabel,
    Select,
    Center,
    Tag,
    Loading,
    Button,
    useLoading,
} from "@yamada-ui/react";
import { PagingTable, type Column } from "@yamada-ui/table";
import { useEffect, useMemo, useState } from "react";
import {
    useStandingsGetAvailableStandingsEndpoints,
    useStandingsGetAvailableStandingsEndpointsSuspense,
    useStandingsGetStandingsEndpoint,
    type Details,
    type TeamStanding,
} from "~/api";
import TrophyIcon from "~/logos/TrophyLogo";
export const Route = createFileRoute("/")({
    component: Index,
});

function Index() {
    const { screen, page, background } = useLoading();

    const [region, setRegion] = useState<string>("Global");
    const [date, setDate] = useState<string>("");
    const {
        data: availableStandingsData,
        isPending: availableStandingsPending,
    } = useStandingsGetAvailableStandingsEndpointsSuspense();

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

    const columns = useMemo<Column<TeamStanding>[]>(
        () => [
            {
                header: "Position",
                accessorKey: "standings",
                size: 1,
                maxSize: 1,
                cell: (info) => (
                    <Center
                        key={info.row.index}
                        rounded="xl"
                        p="1.25"
                        bg={["blackAlpha.50", "whiteAlpha.100"]}
                        textAlign="center"
                    >
                        <Text fontSize="lg">{info.getValue<string>()}</Text>
                    </Center>
                ),
            },
            {
                header: "Team",
                accessorKey: "teamName",
                enableSorting: false,
                cell: (info) => {
                    const isCurrentTeam =
                        data?.data.standings[0]?.teamName ===
                        info.row.original.teamName;
                    const isGlobalRegion = region == "Global";
                    return (
                        <HStack key={info.row.index} gap={2}>
                            {isCurrentTeam && isGlobalRegion && <TrophyIcon />}

                            <Text>{info.getValue<string>()}</Text>
                        </HStack>
                    );
                },
            },
            {
                header: "Roster",
                accessorKey: "roster",
                enableSorting: false,
                cell: (info) => {
                    const players = info.getValue<string[]>();
                    const isCurrentTeam =
                        data?.data.standings[0]?.teamName ===
                        info.row.original.teamName;
                    const isGlobalRegion =
                        region.toLocaleLowerCase() === "global";
                    const isGlobalAndCurrentTeam =
                        isCurrentTeam && isGlobalRegion;
                    return (
                        <HStack key={info.row.index} gap={2}>
                            {players.map((player, index) => (
                                <Tag
                                    key={index}
                                    variant={"subtle"}
                                    colorScheme={
                                        isGlobalAndCurrentTeam
                                            ? "yellow"
                                            : "primary"
                                    }
                                    rounded="lg"
                                >
                                    {player}
                                </Tag>
                            ))}
                        </HStack>
                    );
                },
            },
            {
                header: "Points",
                accessorKey: "points",

                cell: (info) => (
                    <Text key={info.row.index} size="3xl">
                        {info.getValue<string>()}
                    </Text>
                ),
            },
            {
                header: "Details",
                enableSorting: false,
                accessorFn: (row: TeamStanding) => row.details,
                cell: (info) => {
                    const details = info.getValue<Details>();

                    if (!details) return <span>No details</span>;

                    return (
                        <Link
                            to={"/details/$date/$mdfile"}
                            params={{
                                date: details.key,
                                mdfile: details.filename,
                            }}
                        >
                            <Button
                                colorScheme="link"
                                variant="link"
                                size={"sm"}
                            >
                                View
                            </Button>
                        </Link>
                    );
                },
            },
        ],
        [data?.data.standings, region]
    );

    if (isPending) {
        return null;
    }

    if (error) return "An error has occurred: " + error.message;

    return (
        <Container rounded="2xl" bg={["blackAlpha.50", "whiteAlpha.100"]}>
            <HStack justify={"space-between"}>
                <RadioCardGroup
                    w="fit-content"
                    withIcon={false}
                    value={region}
                    onChange={setRegion}
                >
                    <RadioCard key={"global"} value="Global" rounded="xl">
                        <RadioCardLabel>
                            <HStack gap="sm">
                                <Text>Global</Text>
                            </HStack>
                        </RadioCardLabel>
                    </RadioCard>

                    <RadioCard key={"europe"} value="Europe" rounded="xl">
                        <HStack gap="sm">
                            <Text>Europe</Text>
                        </HStack>
                    </RadioCard>

                    <RadioCard key={"americas"} value="Americas" rounded="xl">
                        <RadioCardLabel>
                            <HStack gap="sm">
                                <Text>Americas</Text>
                            </HStack>
                        </RadioCardLabel>
                    </RadioCard>

                    <RadioCard key={"asia"} value="Asia" rounded="xl">
                        <RadioCardLabel>
                            <HStack gap="sm">
                                <Text>Asia</Text>
                            </HStack>
                        </RadioCardLabel>
                    </RadioCard>
                </RadioCardGroup>

                <Select
                    rounded="xl"
                    variant={"filled"}
                    size="md"
                    value={date}
                    disabled={availableStandingsPending}
                    onChange={setDate}
                    header={
                        <Center pt="2" px="3">
                            These are the available dates of VRS rankings
                        </Center>
                    }
                >
                    {availableStandingsData?.data.regionWithDates
                        ?.find(
                            (r) =>
                                r.region.toLocaleLowerCase() ===
                                region.toLocaleLowerCase()
                        )
                        ?.dates.map((s) => (
                            <Option key={s} value={s}>
                                {s}
                            </Option>
                        ))}
                </Select>
            </HStack>

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
