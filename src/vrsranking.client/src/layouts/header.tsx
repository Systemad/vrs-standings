import type {
    CenterProps,
    DrawerProps,
    IconButtonProps,
    MenuProps,
    UseDisclosureReturn,
} from "@yamada-ui/react";
import type { FC } from "react";

import { ListIcon as MenuIcon, MoonIcon, SunIcon } from "@phosphor-icons/react";
import {
    Box,
    Center,
    CloseButton,
    Collapse,
    Drawer,
    DrawerHeader,
    forwardRef,
    HStack,
    IconButton,
    Menu,
    mergeRefs,
    Spacer,
    useBreakpoint,
    useBreakpointValue,
    useColorMode,
    useDisclosure,
    useMotionValueEvent,
    useScroll,
    VStack,
    Link as YamadaLink,
} from "@yamada-ui/react";
import { memo, useEffect, useRef, useState } from "react";
import GithubIcon from "~/icons/GitHubIcon";

/*
                                <Image
                                        src="/logo-black.png"
                                        alt="VRS"
                                        h={{ base: "8", sm: "7" }}
                                        w="auto"
                                        _dark={{ display: "none" }}
                                    />
                                    <Image
                                        src="/logo-white.png"
                                        alt="VRS"
                                        h={{ base: "8", sm: "7" }}
                                        w="auto"
                                        _light={{ display: "none" }}
                                    />
*/
// eslint-disable-next-line @typescript-eslint/no-empty-object-type
export interface HeaderProps extends CenterProps {}

export const Header = memo(
    forwardRef<HeaderProps, "div">(({ ...rest }, ref) => {
        const headerRef = useRef<HTMLHeadingElement>(null);
        const { scrollY } = useScroll();
        const [y, setY] = useState<number>(0);
        const menuControls = useDisclosure();
        const searchControls = useDisclosure();
        const { height = 0 } = headerRef.current?.getBoundingClientRect() ?? {};

        useMotionValueEvent(scrollY, "change", setY);

        const isScroll = y > height;

        return (
            <>
                <Center
                    ref={mergeRefs(ref, headerRef)}
                    as="header"
                    left="0"
                    position="sticky"
                    right="0"
                    top="0"
                    w="full"
                    {...rest}
                >
                    <Center maxW="8xl" px={{ base: "lg", lg: "0" }} w="full">
                        <VStack
                            backdropBlur="10px"
                            backdropFilter="auto"
                            backdropSaturate="180%"
                            bg={["blackAlpha.50", "whiteAlpha.100"]}
                            gap="0"
                            px={{ base: "lg", lg: "md" }}
                            py="3"
                            roundedBottom="2xl"
                            shadow={isScroll ? ["base", "dark-sm"] : undefined}
                            transitionDuration="slower"
                            transitionProperty="common"
                            w="full"
                        >
                            <HStack gap={{ base: "md", sm: "sm" }}>
                                <Center
                                    as={YamadaLink}
                                    href="https://github.com/ValveSoftware/counter-strike_regional_standings"
                                    external
                                    aria-label="Yamada UI"
                                    rounded="md"
                                    transitionDuration="slower"
                                    transitionProperty="opacity"
                                    _focus={{ outline: "none" }}
                                    _focusVisible={{ boxShadow: "outline" }}
                                    _hover={{ opacity: 0.7 }}
                                >
                                    valve regional standings
                                </Center>

                                <Spacer />

                                <ButtonGroup {...menuControls} />
                            </HStack>

                            <Collapse open={searchControls.open}>
                                <Box p="1"></Box>
                            </Collapse>
                        </VStack>
                    </Center>
                </Center>

                <MobileMenu {...menuControls} />
            </>
        );
    })
);

interface ButtonGroupProps extends Partial<UseDisclosureReturn> {
    isMobile?: boolean;
}

const ButtonGroup: FC<ButtonGroupProps> = memo(
    ({ isMobile, open, onClose, onOpen }) => {
        return (
            <HStack gap="sm">
                <IconButton
                    as={YamadaLink}
                    href="{CONSTANT.SNS.DISCORD}"
                    variant="ghost"
                    aria-label="GitHub repository"
                    color="muted"
                    display={{
                        base: "inline-flex",
                        lg: !isMobile ? "none" : undefined,
                    }}
                    icon={<GithubIcon />}
                    isExternal
                    rounded="xl"
                    _hover={{ bg: [`blackAlpha.100`, `whiteAlpha.50`] }}
                />

                <ColorModeButton
                    display={{
                        base: "inline-flex",
                        sm: !isMobile ? "none" : undefined,
                    }}
                />

                {!open ? (
                    <IconButton
                        variant="ghost"
                        aria-label="Open navigation menu"
                        color="muted"
                        display={{ base: "none", lg: "inline-flex" }}
                        icon={<MenuIcon fontSize="1.5rem" />}
                        rounded="xl"
                        _hover={{ bg: [`blackAlpha.100`, `whiteAlpha.50`] }}
                        onClick={onOpen}
                    />
                ) : (
                    <CloseButton
                        size="lg"
                        aria-label="Close navigation menu"
                        color="muted"
                        display={{ base: "none", lg: "inline-flex" }}
                        rounded="xl"
                        _hover={{ bg: [`blackAlpha.100`, `whiteAlpha.50`] }}
                        onClick={onClose}
                    />
                )}
            </HStack>
        );
    }
);

ButtonGroup.displayName = "ButtonGroup";

interface ColorModeButtonProps extends IconButtonProps {
    menuProps?: MenuProps;
}

const ColorModeButton: FC<ColorModeButtonProps> = memo(
    ({ menuProps, ...rest }) => {
        const padding = useBreakpointValue({ base: 32, md: 16 });
        const { changeColorMode, colorMode } = useColorMode();

        return (
            <Menu
                modifiers={[
                    {
                        name: "preventOverflow",
                        options: {
                            padding: {
                                bottom: padding,
                                left: padding,
                                right: padding,
                                top: padding,
                            },
                        },
                    },
                ]}
                placement="bottom"
                restoreFocus={false}
                {...menuProps}
            >
                <IconButton
                    variant="ghost"
                    aria-label="Toggle theme"
                    color="muted"
                    icon={
                        colorMode === "dark" ? (
                            <SunIcon fontSize="1.5rem" />
                        ) : (
                            <MoonIcon fontSize="1.5rem" />
                        )
                    }
                    rounded={"xl"}
                    _hover={{ bg: [`blackAlpha.100`, `whiteAlpha.50`] }}
                    onClick={() =>
                        changeColorMode(colorMode == "dark" ? "light" : "dark")
                    }
                    {...rest}
                />
            </Menu>
        );
    }
);

ColorModeButton.displayName = "ColorModeButton";

// eslint-disable-next-line @typescript-eslint/no-empty-object-type
interface MobileMenuProps extends DrawerProps {}

const MobileMenu: FC<MobileMenuProps> = memo(({ open, onClose }) => {
    const breakpoint = useBreakpoint();

    useEffect(() => {
        if (!["lg", "md", "sm"].includes(breakpoint)) onClose?.();
    }, [breakpoint, onClose]);

    return (
        <Drawer
            fullHeight={true}
            open={open}
            roundedLeft="xl"
            w="auto"
            withCloseButton={false}
            onClose={onClose}
        >
            <DrawerHeader
                fontSize="md"
                fontWeight="normal"
                justifyContent="flex-end"
                pt="sm"
            >
                <ButtonGroup isMobile {...{ open, onClose }} />
            </DrawerHeader>
        </Drawer>
    );
});

MobileMenu.displayName = "MobileMenu";
