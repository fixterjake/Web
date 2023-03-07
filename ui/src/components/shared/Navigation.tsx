import { useAuthContext } from "@/contexts/AuthContext";
import { getToken, isFullStaff, isSeniorStaff, isSeniorTrainingStaff, isStaff, isTrainingStaff, setRedirect } from "@/Helpers/AuthHelper";
import { Menu, Transition } from "@headlessui/react";
import Image from "next/image";
import Link from "next/link";
import { useRouter } from "next/router";
import { Fragment, useEffect } from "react";
import logo from "../../../public/images/nav.png";

const Navigation = () => {

    const router = useRouter();
    const [loggedIn, setLoggedIn, user] = useAuthContext();

    useEffect(() => {
        if (getToken())
            setLoggedIn(true);
    }, [setLoggedIn]);

    function loginHandler() {
        setRedirect(router.asPath);
        router.push("/auth/login");
    }

    function logoutHandler() {
        setRedirect(router.asPath);
        router.push("/auth/logout");
    }

    return (
        <nav className="py-6 text-white bg-zinc-700">
            <div className="mx-[15%] flex justify-between items-center">
                <div className="flex items-center">
                    <Link href="/" className="mr-4">
                        <Image src={logo} width={125} alt="Memphis ARTCC" priority={true} className="pb-4 brightness-0 invert" />
                    </Link>
                    <div>
                        <Menu>
                            <Menu.Button className="mx-4 text-xl font-bold text-white">
                                    About
                            </Menu.Button>
                            <Transition
                                as={Fragment}
                                enter="transition ease-out duration-100"
                                enterFrom="transform opacity-0 scale-95"
                                enterTo="transform opacity-100 scale-100"
                                leave="transition ease-in duration-75"
                                leaveFrom="transform opacity-100 scale-100"
                                leaveTo="transform opacity-0 scale-95"
                            >
                                <Menu.Items className="absolute mt-4 rounded-md bg-zinc-500">
                                    <div className="text-lg min-w-[9rem]">
                                        <Menu.Item>
                                            <Link href="/about/staff" className="block p-2 mb-1 rounded-md hover:bg-zinc-400">Staff</Link>
                                        </Menu.Item>
                                        <Menu.Item>
                                            <Link href="/about/roster" className="block p-2 mb-1 rounded-md hover:bg-zinc-400">Roster</Link>
                                        </Menu.Item>
                                        <Menu.Item>
                                            <Link href="/about/news" className="block p-2 mb-1 rounded-md hover:bg-zinc-400">News</Link>
                                        </Menu.Item>
                                        <Menu.Item>
                                            <Link href="/about/faq" className="block p-2 rounded-md hover:bg-zinc-400">FAQ</Link>
                                        </Menu.Item>
                                        <Menu.Item>
                                            <Link href="/about/privacy" className="block p-2 rounded-md hover:bg-zinc-400">Privacy Policy</Link>
                                        </Menu.Item>
                                    </div>
                                </Menu.Items>
                            </Transition>
                        </Menu>
                    </div>
                    <div>
                        <Menu>
                            <Menu.Button className="mx-4 text-xl font-bold text-white">
                                    Pilots
                            </Menu.Button>
                            <Transition
                                as={Fragment}
                                enter="transition ease-out duration-100"
                                enterFrom="transform opacity-0 scale-95"
                                enterTo="transform opacity-100 scale-100"
                                leave="transition ease-in duration-75"
                                leaveFrom="transform opacity-100 scale-100"
                                leaveTo="transform opacity-0 scale-95"
                            >
                                <Menu.Items className="absolute mt-4 rounded-md bg-zinc-500">
                                    <div className="text-lg min-w-[9rem]">
                                        <Menu.Item>
                                            <Link href="/pilots/feedback" className="block p-2 mb-1 rounded-md hover:bg-zinc-400">Feedback</Link>
                                        </Menu.Item>
                                        <Menu.Item>
                                            <Link href="/pilots/airports" className="block p-2 rounded-md hover:bg-zinc-400">Airports</Link>
                                        </Menu.Item>
                                    </div>
                                </Menu.Items>
                            </Transition>
                        </Menu>
                    </div>
                    <div>
                        <Menu>
                            <Menu.Button className="mx-4 text-xl font-bold text-white">
                                Controllers
                            </Menu.Button>
                            <Transition
                                as={Fragment}
                                enter="transition ease-out duration-100"
                                enterFrom="transform opacity-0 scale-95"
                                enterTo="transform opacity-100 scale-100"
                                leave="transition ease-in duration-75"
                                leaveFrom="transform opacity-100 scale-100"
                                leaveTo="transform opacity-0 scale-95"
                            >
                                <Menu.Items className="absolute mt-4 rounded-md bg-zinc-500">
                                    <div className="text-lg min-w-[9rem]">
                                        <Menu.Item>
                                            <Link href="/controllers/events" className="block p-2 rounded-md hover:bg-zinc-400">Events</Link>
                                        </Menu.Item>
                                        <Menu.Item>
                                            <Link href="/controllers/visit" className="block p-2 rounded-md hover:bg-zinc-400">Apply to Visit</Link>
                                        </Menu.Item>
                                        {loggedIn ? (
                                            <Menu.Item>
                                                <Link href="/controllers/trainingrequest" className="block p-2 rounded-md hover:bg-zinc-400">Request Training</Link>
                                            </Menu.Item>
                                        ) : (<></>)}
                                    </div>
                                </Menu.Items>
                            </Transition>
                        </Menu>
                    </div>
                    <div>
                        {
                            loggedIn && isTrainingStaff(user) ? (
                                <Menu>
                                    <Menu.Button className="mx-4 text-xl font-bold text-white">
                                        Training Management
                                    </Menu.Button>
                                    <Transition
                                        as={Fragment}
                                        enter="transition ease-out duration-100"
                                        enterFrom="transform opacity-0 scale-95"
                                        enterTo="transform opacity-100 scale-100"
                                        leave="transition ease-in duration-75"
                                        leaveFrom="transform opacity-100 scale-100"
                                        leaveTo="transform opacity-0 scale-95"
                                    >
                                        <Menu.Items className="absolute mt-4 rounded-md bg-zinc-500">
                                            <div className="text-lg min-w-[9rem]">
                                                <Menu.Item>
                                                    <Link href="/training/requests" className="block p-2 rounded-md hover:bg-zinc-400">Training Requests</Link>
                                                </Menu.Item>
                                                <Menu.Item>
                                                    <Link href="/training/tickets" className="block p-2 rounded-md hover:bg-zinc-400">Training Tickets</Link>
                                                </Menu.Item>
                                                {isSeniorTrainingStaff(user) ? (
                                                    <Menu.Item>
                                                        <Link href="/training/ots" className="block p-2 rounded-md hover:bg-zinc-400">OTS Center</Link>
                                                    </Menu.Item>
                                                ) : (<></>)}
                                                {isSeniorTrainingStaff(user) ? (
                                                    <Menu.Item>
                                                        <Link href="/training/feedback" className="block p-2 rounded-md hover:bg-zinc-400">Feedback</Link>
                                                    </Menu.Item>
                                                ) : <></>}
                                                {isSeniorStaff(user) ? (
                                                    <Menu.Item>
                                                        <Link href="/training/staff" className="block p-2 rounded-md hover:bg-zinc-400">Training Staff Management</Link>
                                                    </Menu.Item>
                                                ) : <></>}
                                            </div>
                                        </Menu.Items>
                                    </Transition>
                                </Menu>
                            ) : (<></>)
                        }
                    </div>
                    <div>
                        {
                            loggedIn && isStaff(user) ? (
                                <Menu>
                                    <Menu.Button className="mx-4 text-xl font-bold text-white">
                                        ARTCC Management
                                    </Menu.Button>
                                    <Transition
                                        as={Fragment}
                                        enter="transition ease-out duration-100"
                                        enterFrom="transform opacity-0 scale-95"
                                        enterTo="transform opacity-100 scale-100"
                                        leave="transition ease-in duration-75"
                                        leaveFrom="transform opacity-100 scale-100"
                                        leaveTo="transform opacity-0 scale-95"
                                    >
                                        <Menu.Items className="absolute mt-4 rounded-md bg-zinc-500">
                                            <div className="text-lg min-w-[9rem]">
                                                {isFullStaff(user) ? (
                                                    <>
                                                        <Menu.Item>
                                                            <Link href="/management/announcements" className="block p-2 rounded-md hover:bg-zinc-400">Announcements</Link>
                                                        </Menu.Item>
                                                        <Menu.Item>
                                                            <Link href="/management/emailing" className="block p-2 rounded-md hover:bg-zinc-400">Emailing</Link>
                                                        </Menu.Item>
                                                    </>
                                                ) : (<></>)}
                                                {isSeniorStaff(user) ? (
                                                    <>
                                                        <Menu.Item>
                                                            <Link href="/management/loas" className="block p-2 rounded-md hover:bg-zinc-400">LOA Center</Link>
                                                        </Menu.Item>
                                                        <Menu.Item>
                                                            <Link href="/management/visiting" className="block p-2 rounded-md hover:bg-zinc-400">Visit Requests</Link>
                                                        </Menu.Item>
                                                        <Menu.Item>
                                                            <Link href="/management/roster" className="block p-2 rounded-md hover:bg-zinc-400">Roster Management</Link>
                                                        </Menu.Item>
                                                        <Menu.Item>
                                                            <Link href="/management/comments" className="block p-2 rounded-md hover:bg-zinc-400">Comments</Link>
                                                        </Menu.Item>
                                                        <Menu.Item>
                                                            <Link href="/management/settings" className="block p-2 rounded-md hover:bg-zinc-400">Settings</Link>
                                                        </Menu.Item>
                                                        <Menu.Item>
                                                            <Link href="/management/emaillogs" className="block p-2 rounded-md hover:bg-zinc-400">Email Logs</Link>
                                                        </Menu.Item>
                                                        <Menu.Item>
                                                            <Link href="/management/websitelogs" className="block p-2 rounded-md hover:bg-zinc-400">Website Logs</Link>
                                                        </Menu.Item>
                                                    </>
                                                ) : <></>}
                                            </div>
                                        </Menu.Items>
                                    </Transition>
                                </Menu>
                            ) : (<></>)
                        }
                    </div>
                </div>
                <div className="flex items-center">
                    {loggedIn ? (
                        <div>
                            <Menu>
                                <Menu.Button className="mx-4 text-xl font-bold text-white">
                                    {user.firstName} {user.lastName}
                                </Menu.Button>
                                <Transition
                                    as={Fragment}
                                    enter="transition ease-out duration-100"
                                    enterFrom="transform opacity-0 scale-95"
                                    enterTo="transform opacity-100 scale-100"
                                    leave="transition ease-in duration-75"
                                    leaveFrom="transform opacity-100 scale-100"
                                    leaveTo="transform opacity-0 scale-95"
                                >
                                    <Menu.Items className="absolute mt-2 rounded-md bg-zinc-500">
                                        <div className="text-lg min-w-[9rem]">
                                            <Menu.Item>
                                                <Link href="/user/profile" className="block p-2 mb-1 rounded-md hover:bg-zinc-400">Profile</Link>
                                            </Menu.Item>
                                            <Menu.Item>
                                                <button onClick={logoutHandler} className="block w-full p-2 text-left rounded-md hover:bg-zinc-400">Logout</button>
                                            </Menu.Item>
                                        </div>
                                    </Menu.Items>
                                </Transition>
                            </Menu>
                        </div>
                    ) : (
                        <button onClick={loginHandler} className="text-xl font-bold text-white">Login</button>
                    )}
                </div>
            </div>
        </nav>
    );
};

export default Navigation;