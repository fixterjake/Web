import Card from "@/components/shared/CardList";
import { useAuthContext } from "@/contexts/AuthContext";
import {Event} from "@/models/Event";
import { canEvents, getToken } from "@/Helpers/AuthHelper";
import { Disclosure } from "@headlessui/react";
import Head from "next/head";
import Image from "next/image";
import Link from "next/link";
import { useRouter } from "next/router";
import { useEffect, useState } from "react";
import CardList from "@/components/shared/CardList";
import ReactPaginate from "react-paginate";

type EventProps = {
    apiUrl: string;
}

export default function EventsPage({ apiUrl }: EventProps) {

    const router = useRouter();

    const [initialLoad, setInitialLoad] = useState(true);
    const [events, setEvents] = useState<Event[]>([]);

    const pageSize = 10;
    const [resultCount, setResultCount] = useState(0);
    const [totalCount , setTotalCount] = useState(0);
    const [isLoggedIn, , user] = useAuthContext();
    const [createOpen, setCreateOpen] = useState(false);

    const [loading, setLoading] = useState(false);

    useEffect(() => {
        const fetchEvents = async () => {
            const res = await fetch(`${apiUrl}/events/all`, {
                method: "GET",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${getToken()}`
                },
            });
            const response = await res.json();
            setEvents(response.data);
            setResultCount(response.resultCount);
            setTotalCount(response.totalCount);
            setInitialLoad(false);
        };
        fetchEvents();
    }, [apiUrl]);

    async function getEventsPaged(event: { selected: number }) {
        const res = await fetch(`${apiUrl}/events/all?page=${event.selected}&size=${pageSize}`, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${getToken()}`
            },
        });
        const response = await res.json();
        setEvents(response.data);
        setResultCount(response.resultCount);
        setTotalCount(response.totalCount);
    }

    function openCreateDialog() {
        setCreateOpen(true);
    }

    function closeCreateDialog() {
        setCreateOpen(false);
        router.replace(router.asPath);
    }

    async function handleCreateSubmit(event: React.FormEvent) {
        setLoading(true);
        event.preventDefault();
        setLoading(false);
    }

    return (
        <>
            <Head>
                <title>Events | Memphis ARTCC</title>
                <meta name="description" content="Events | Memphis ARTCC" />
                <meta name="viewport" content="width=device-width, initial-scale=1" />
                <link rel="icon" href="/favicon.ico" />
            </Head>
            <CardList className="mt-10" title="Events" initialLoad={initialLoad} data={events} notFoundMessage="No Events Found"
                showCreate={true} createMessage="Create new Event" isLoggedIn={isLoggedIn} roleCheck={canEvents} user={user} openCreateDialog={openCreateDialog}>
                <>
                    {events.map((event) => (
                        <div key={event.id} className="flex flex-col space-y-2">
                            <Link href={`/events/${event.id}`} className="mx-24">
                                <Image src={event.bannerUrl ?? ""} alt={event.title} width={700} height={100}
                                    className={`${(!event.isOpen ? "grayscale" : "")} rounded-lg ml-auto mr-auto`} />
                            </Link>
                            {isLoggedIn && canEvents(user) ? (
                                <Disclosure>
                                    {({ open }) => (
                                        <>
                                            <Disclosure.Button className="flex justify-between px-4 py-2 text-lg font-medium rounded-lg
                                                    text-zinc-100 bg-zinc-600 hover:bg-zinc-500 mx-[12%]">
                                                <span>Actions</span>
                                                <svg
                                                    className={`${
                                                        open ? "transform rotate-180" : ""
                                                    } w-5 h-5 text-zinc-200`}
                                                    xmlns="http://www.w3.org/2000/svg"
                                                    viewBox="0 0 20 20"
                                                    fill="currentColor"
                                                    aria-hidden="true"
                                                >
                                                    <path
                                                        fillRule="evenodd"
                                                        d="M6 6L14 10L6 14V6Z"
                                                        clipRule="evenodd"
                                                    />
                                                </svg>
                                            </Disclosure.Button>
                                            <Disclosure.Panel className="px-4 pt-4 pb-2 text-lg text-zinc-100">
                                                stuff
                                            </Disclosure.Panel>
                                        </>
                                    )}
                                </Disclosure>
                            ) : (<></>)}
                        </div>
                    ))}
                </>
            </CardList>
        </>
    );
}

export async function getServerSideProps() {
    const apiUrl = process.env.API_URL;
    return {
        props: {
            apiUrl: apiUrl,
        }
    };
}
