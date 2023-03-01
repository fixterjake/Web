import Card from "@/components/shared/Card";
import { useAuthContext } from "@/contexts/AuthContext";
import { Airport } from "@/models/Airport";
import { canAirports, getToken, isFullStaff } from "@/services/AuthService";
import { Dialog, Transition } from "@headlessui/react";
import Head from "next/head";
import { useRouter } from "next/router";
import { Fragment, useState } from "react";

type AirportsProps = {
    airports: Airport[];
    apiUrl: string;
}

export default function AirportsPage({ airports, apiUrl }: AirportsProps) {

    const router = useRouter();
    const [createOpen, setCreateOpen] = useState(false);
    const [editOpen, setEditOpen] = useState(false);
    const [deleteOpen, setDeleteOpen] = useState(false);
    const [selectedAirport, setSelectedAirport] = useState<Airport>();
    const [isLoggedIn, , user] = useAuthContext();
    const [loading, setLoading] = useState(false);

    function openCreateModal() {
        setCreateOpen(true);
    }

    function closeCreateModal() {
        setCreateOpen(false);
        router.replace(router.asPath);
    }

    async function handleCreateSubmit(event: React.FormEvent) {
        setLoading(true);
        event.preventDefault();

        const airport: Airport = {
            name: (event.target as HTMLFormElement).airport.value,
            icao: (event.target as HTMLFormElement).icao.value,
            departures: 0,
            arrivals: 0
        };
        const jsonData = JSON.stringify(airport);

        const res = await fetch(`${apiUrl}/airports`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${getToken()}`
            },
            body: jsonData
        });
        const response = await res.json();
        if (response.statusCode === 201) {
            closeCreateModal();
        }
        setLoading(false);
    }

    function openEditModal() {
        setEditOpen(true);
    }

    function closeEditModal() {
        setEditOpen(false);
        router.replace(router.asPath);
    }

    async function handleEditSubmit(event: React.FormEvent) {
        setLoading(true);
        event.preventDefault();

        const airport: Airport = {
            name: (event.target as HTMLFormElement).airport.value,
            icao: (event.target as HTMLFormElement).icao.value,
            departures: selectedAirport?.departures ?? 0,
            arrivals: selectedAirport?.arrivals ?? 0
        };
        const jsonData = JSON.stringify(airport);

        const res = await fetch(`${apiUrl}/airports`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${getToken()}`
            },
            body: jsonData
        });
        const response = await res.json();
        if (response.statusCode === 200) {
            closeEditModal();
        }
        setLoading(false);
    }

    function openDeleteModel() {
        setDeleteOpen(true);
    }

    function closeDeleteModal() {
        setDeleteOpen(false);
        router.replace(router.asPath);
    }

    async function handleDeleteSubmit() {
        setLoading(true);
        const res = await fetch(`${apiUrl}/airports?airportId=${selectedAirport?.id}`, {
            method: "DELETE",
            headers: {
                "Authorization": `Bearer ${getToken()}`
            }
        });
        const response = await res.json();
        if (response.statusCode === 200) {
            closeDeleteModal();
        }
        setLoading(false);
    }

    return (
        <>
            <Head>
                <title>Airports | Memphis ARTCC</title>
                <meta name="description" content="Airports | Memphis ARTCC" />
                <meta name="viewport" content="width=device-width, initial-scale=1" />
                <link rel="icon" href="/favicon.ico" />
            </Head>
            <Card title="Airports" className="mt-10">
                <div className="flex flex-col space-y-4 text-center mx-[15%]">
                    {airports != null && airports.length > 0 ? (
                        <>
                            <table className="table-auto">
                                <thead className="border-b border-gray-400">
                                    <th>Name</th>
                                    <th>ICAO</th>
                                    <th>Arrivals</th>
                                    <th>Departures</th>
                                    {canAirports(user) ? (
                                        <th>Actions</th>
                                    ) : (<></>)}
                                </thead>
                                <tbody>
                                    {airports?.map((airport) => (
                                        <tr key={airport.id} className="border-b border-gray-400">
                                            <td>
                                                <a href={`https://chartfox.org/${airport.icao}`} target="_blank">
                                                    {airport.name}
                                                </a>
                                            </td>
                                            <td>{airport.icao}</td>
                                            <td>{airport.arrivals}</td>
                                            <td>{airport.departures}</td>
                                            {canAirports(user) ? (
                                                <td>
                                                    <button onClick={() => {openEditModal(); setSelectedAirport(airport);}} className="p-2 mx-2 mt-2 rounded-md bg-zinc-600 hover:bg-zinc-500">
                                                        Edit
                                                    </button>
                                                    <button onClick={() => {openDeleteModel(); setSelectedAirport(airport);}} className="p-2 mx-2 rounded-md bg-zinc-600 hover:bg-zinc-500">
                                                        Delete
                                                    </button>
                                                </td>
                                            ) : (<></>)}
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </>
                    ) : (
                        <div className="flex flex-col space-y-2">
                            <h1 className="text-xl font-medium">No airports found</h1>
                        </div>
                    )}
                    {isLoggedIn && isFullStaff(user) ? (
                        <div className="pt-4">
                            <button onClick={openCreateModal} className="flex justify-center w-[15rem] ml-auto mr-auto px-4 py-2 text-lg
                                font-medium rounded-lg text-zinc-100 bg-zinc-600 hover:bg-zinc-500">
                                Create new Airport
                            </button>
                        </div>
                    ) : (<></>)}
                </div>
            </Card>
            <Transition appear show={createOpen} as={Fragment}>
                <Dialog as="div" className="relative z-10" onClose={closeCreateModal}>
                    <Transition.Child
                        as={Fragment}
                        enter="ease-out duration-300"
                        enterFrom="opacity-0"
                        enterTo="opacity-100"
                        leave="ease-in duration-200"
                        leaveFrom="opacity-100"
                        leaveTo="opacity-0"
                    >
                        <div className="fixed inset-0 bg-black bg-opacity-25" />
                    </Transition.Child>
                    <div className="fixed inset-0 overflow-y-auto">
                        <div className="flex items-center justify-center min-h-full p-4 text-center">
                            <Transition.Child
                                as={Fragment}
                                enter="ease-out duration-300"
                                enterFrom="opacity-0 scale-95"
                                enterTo="opacity-100 scale-100"
                                leave="ease-in duration-200"
                                leaveFrom="opacity-100 scale-100"
                                leaveTo="opacity-0 scale-95"
                            >
                                <Dialog.Panel className="w-full max-w-md p-6 overflow-hidden text-left align-middle transition-all transform shadow-xl bg-zinc-700 rounded-2xl">
                                    <Dialog.Title as="h2" className="pb-4 mb-4 text-lg font-medium leading-6 text-center text-white border-b-2">
                                        Create new Airport
                                    </Dialog.Title>
                                    <form onSubmit={handleCreateSubmit}>
                                        <div className="flex flex-col">
                                            <label htmlFor="airport" className="mb-4 text-lg font-medium">Name</label>
                                            <input type="text" id="airport" name="question" className="mb-4 text-gray-800 rounded-md" required />
                                            <label htmlFor="icao" className="mb-4 text-lg font-medium">ICAO</label>
                                            <input type="text" id="icao" name="answer" className="mb-4 text-gray-800 rounded-md" required />
                                            <button type="submit" className="w-auto p-2 mt-4 ml-auto rounded-md bg-zinc-600 hover:bg-zinc-500">
                                                <div className="flex">
                                                    {loading ? (
                                                        <svg className="inline w-6 h-6 mr-2 text-gray-200 animate-spin fill-blue-500" viewBox="0 0 100 101" fill="none" xmlns="http://www.w3.org/2000/svg">
                                                            <path d="M100 50.5908C100 78.2051 77.6142 100.591 50 100.591C22.3858 100.591 0 78.2051 0 50.5908C0 22.9766 22.3858 0.59082 50 0.59082C77.6142 0.59082 100 22.9766 100 50.5908ZM9.08144 50.5908C9.08144 73.1895 27.4013 91.5094 50 91.5094C72.5987 91.5094 90.9186 73.1895 90.9186 50.5908C90.9186 27.9921 72.5987 9.67226 50 9.67226C27.4013 9.67226 9.08144 27.9921 9.08144 50.5908Z" fill="currentColor"/>
                                                            <path d="M93.9676 39.0409C96.393 38.4038 97.8624 35.9116 97.0079 33.5539C95.2932 28.8227 92.871 24.3692 89.8167 20.348C85.8452 15.1192 80.8826 10.7238 75.2124 7.41289C69.5422 4.10194 63.2754 1.94025 56.7698 1.05124C51.7666 0.367541 46.6976 0.446843 41.7345 1.27873C39.2613 1.69328 37.813 4.19778 38.4501 6.62326C39.0873 9.04874 41.5694 10.4717 44.0505 10.1071C47.8511 9.54855 51.7191 9.52689 55.5402 10.0491C60.8642 10.7766 65.9928 12.5457 70.6331 15.2552C75.2735 17.9648 79.3347 21.5619 82.5849 25.841C84.9175 28.9121 86.7997 32.2913 88.1811 35.8758C89.083 38.2158 91.5421 39.6781 93.9676 39.0409Z" fill="currentFill"/>
                                                        </svg>
                                                    ) : (<></>)}
                                                    <span>Submit</span>
                                                </div>
                                            </button>
                                        </div>
                                    </form>
                                </Dialog.Panel>
                            </Transition.Child>
                        </div>
                    </div>
                </Dialog>
            </Transition>
            <Transition appear show={editOpen} as={Fragment}>
                <Dialog as="div" className="relative z-10" onClose={closeEditModal}>
                    <Transition.Child
                        as={Fragment}
                        enter="ease-out duration-300"
                        enterFrom="opacity-0"
                        enterTo="opacity-100"
                        leave="ease-in duration-200"
                        leaveFrom="opacity-100"
                        leaveTo="opacity-0"
                    >
                        <div className="fixed inset-0 bg-black bg-opacity-25" />
                    </Transition.Child>
                    <div className="fixed inset-0 overflow-y-auto">
                        <div className="flex items-center justify-center min-h-full p-4 text-center">
                            <Transition.Child
                                as={Fragment}
                                enter="ease-out duration-300"
                                enterFrom="opacity-0 scale-95"
                                enterTo="opacity-100 scale-100"
                                leave="ease-in duration-200"
                                leaveFrom="opacity-100 scale-100"
                                leaveTo="opacity-0 scale-95"
                            >
                                <Dialog.Panel className="w-full max-w-md p-6 overflow-hidden text-left align-middle transition-all transform shadow-xl bg-zinc-700 rounded-2xl">
                                    <Dialog.Title as="h2" className="pb-4 mb-4 text-lg font-medium leading-6 text-center text-white border-b-2">
                                        Edit Airport
                                    </Dialog.Title>
                                    <form onSubmit={handleEditSubmit}>
                                        <div className="flex flex-col">
                                            <label htmlFor="airport" className="mb-4 text-lg font-medium">Name</label>
                                            <input type="text" id="airport" name="question" className="mb-4 text-gray-800 rounded-md" defaultValue={selectedAirport?.name} required />
                                            <label htmlFor="icao" className="mb-4 text-lg font-medium">ICAO</label>
                                            <input type="text" id="icao" name="answer" className="mb-4 text-gray-800 rounded-md" defaultValue={selectedAirport?.icao} required />
                                            <button type="submit" className="w-auto p-2 mt-4 ml-auto rounded-md bg-zinc-600 hover:bg-zinc-500">
                                                <div className="flex">
                                                    {loading ? (
                                                        <svg className="inline w-6 h-6 mr-2 text-gray-200 animate-spin fill-blue-500" viewBox="0 0 100 101" fill="none" xmlns="http://www.w3.org/2000/svg">
                                                            <path d="M100 50.5908C100 78.2051 77.6142 100.591 50 100.591C22.3858 100.591 0 78.2051 0 50.5908C0 22.9766 22.3858 0.59082 50 0.59082C77.6142 0.59082 100 22.9766 100 50.5908ZM9.08144 50.5908C9.08144 73.1895 27.4013 91.5094 50 91.5094C72.5987 91.5094 90.9186 73.1895 90.9186 50.5908C90.9186 27.9921 72.5987 9.67226 50 9.67226C27.4013 9.67226 9.08144 27.9921 9.08144 50.5908Z" fill="currentColor"/>
                                                            <path d="M93.9676 39.0409C96.393 38.4038 97.8624 35.9116 97.0079 33.5539C95.2932 28.8227 92.871 24.3692 89.8167 20.348C85.8452 15.1192 80.8826 10.7238 75.2124 7.41289C69.5422 4.10194 63.2754 1.94025 56.7698 1.05124C51.7666 0.367541 46.6976 0.446843 41.7345 1.27873C39.2613 1.69328 37.813 4.19778 38.4501 6.62326C39.0873 9.04874 41.5694 10.4717 44.0505 10.1071C47.8511 9.54855 51.7191 9.52689 55.5402 10.0491C60.8642 10.7766 65.9928 12.5457 70.6331 15.2552C75.2735 17.9648 79.3347 21.5619 82.5849 25.841C84.9175 28.9121 86.7997 32.2913 88.1811 35.8758C89.083 38.2158 91.5421 39.6781 93.9676 39.0409Z" fill="currentFill"/>
                                                        </svg>
                                                    ) : (<></>)}
                                                    <span>Submit</span>
                                                </div>
                                            </button>
                                        </div>
                                    </form>
                                </Dialog.Panel>
                            </Transition.Child>
                        </div>
                    </div>
                </Dialog>
            </Transition>
            <Transition appear show={deleteOpen} as={Fragment}>
                <Dialog as="div" className="relative z-10" onClose={closeDeleteModal}>
                    <Transition.Child
                        as={Fragment}
                        enter="ease-out duration-300"
                        enterFrom="opacity-0"
                        enterTo="opacity-100"
                        leave="ease-in duration-200"
                        leaveFrom="opacity-100"
                        leaveTo="opacity-0"
                    >
                        <div className="fixed inset-0 bg-black bg-opacity-25" />
                    </Transition.Child>
                    <div className="fixed inset-0 overflow-y-auto">
                        <div className="flex items-center justify-center min-h-full p-4 text-center">
                            <Transition.Child
                                as={Fragment}
                                enter="ease-out duration-300"
                                enterFrom="opacity-0 scale-95"
                                enterTo="opacity-100 scale-100"
                                leave="ease-in duration-200"
                                leaveFrom="opacity-100 scale-100"
                                leaveTo="opacity-0 scale-95"
                            >
                                <Dialog.Panel className="w-full max-w-md p-6 overflow-hidden text-left align-middle transition-all transform shadow-xl bg-zinc-700 rounded-2xl">
                                    <Dialog.Title as="h2" className="pb-4 mb-4 text-lg font-medium leading-6 text-center text-white">
                                        Delete Airport
                                    </Dialog.Title>
                                    <div className="w-full text-right">
                                        <div className="text-lg font-medium text-center">This action cannot be undone.</div>
                                        <br />
                                        <button onClick={handleDeleteSubmit} className="w-auto p-2 mt-4 ml-auto rounded-md bg-zinc-600 hover:bg-zinc-500">
                                            <div className="flex">
                                                {loading ? (
                                                    <svg className="inline w-6 h-6 mr-2 text-gray-200 animate-spin fill-blue-500" viewBox="0 0 100 101" fill="none" xmlns="http://www.w3.org/2000/svg">
                                                        <path d="M100 50.5908C100 78.2051 77.6142 100.591 50 100.591C22.3858 100.591 0 78.2051 0 50.5908C0 22.9766 22.3858 0.59082 50 0.59082C77.6142 0.59082 100 22.9766 100 50.5908ZM9.08144 50.5908C9.08144 73.1895 27.4013 91.5094 50 91.5094C72.5987 91.5094 90.9186 73.1895 90.9186 50.5908C90.9186 27.9921 72.5987 9.67226 50 9.67226C27.4013 9.67226 9.08144 27.9921 9.08144 50.5908Z" fill="currentColor"/>
                                                        <path d="M93.9676 39.0409C96.393 38.4038 97.8624 35.9116 97.0079 33.5539C95.2932 28.8227 92.871 24.3692 89.8167 20.348C85.8452 15.1192 80.8826 10.7238 75.2124 7.41289C69.5422 4.10194 63.2754 1.94025 56.7698 1.05124C51.7666 0.367541 46.6976 0.446843 41.7345 1.27873C39.2613 1.69328 37.813 4.19778 38.4501 6.62326C39.0873 9.04874 41.5694 10.4717 44.0505 10.1071C47.8511 9.54855 51.7191 9.52689 55.5402 10.0491C60.8642 10.7766 65.9928 12.5457 70.6331 15.2552C75.2735 17.9648 79.3347 21.5619 82.5849 25.841C84.9175 28.9121 86.7997 32.2913 88.1811 35.8758C89.083 38.2158 91.5421 39.6781 93.9676 39.0409Z" fill="currentFill"/>
                                                    </svg>
                                                ) : (<></>)}
                                                <span>Submit</span>
                                            </div>
                                        </button>
                                    </div>
                                </Dialog.Panel>
                            </Transition.Child>
                        </div>
                    </div>
                </Dialog>
            </Transition>
        </>
    );
}

export async function getServerSideProps() {
    const apiUrl = process.env.API_URL;
    const res = await fetch(`${apiUrl}/airports/all`);
    const response = await res.json();
    return {
        props: {
            airports: response.data,
            apiUrl: apiUrl,
        }
    };
}
