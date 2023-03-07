import { useAuthContext } from "@/contexts/AuthContext";
import { Airport } from "@/models/Airport";
import { canAirports, getToken } from "@/Helpers/AuthHelper";
import Head from "next/head";
import { useRouter } from "next/router";
import { Fragment, useEffect, useState } from "react";
import CardList from "@/components/shared/CardList";
import FormDialog from "@/components/shared/FormDialog";
import DeleteDialog from "@/components/shared/DeleteDialog";

type AirportsProps = {
    apiUrl: string;
}

export default function AirportsPage({ apiUrl }: AirportsProps) {

    const router = useRouter();

    const [initialLoad, setInitialLoad] = useState(true);
    const [airports, setAirports] = useState<Airport[]>([]);

    const [createOpen, setCreateOpen] = useState(false);
    const [editOpen, setEditOpen] = useState(false);
    const [deleteOpen, setDeleteOpen] = useState(false);
    const [selectedAirport, setSelectedAirport] = useState<Airport>();

    const [isLoggedIn, , user] = useAuthContext();
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        const fetchFaqs = async () => {
            const res = await fetch(`${apiUrl}/airports/all`, {
                method: "GET",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${getToken()}`
                },
            });
            const response = await res.json();
            setAirports(response.data);
            setInitialLoad(false);
        };
        fetchFaqs();
    }, [apiUrl]);

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
            closeCreateDialog();
            airports.push(response.data);
        }
        setLoading(false);
    }

    function openEditDialog() {
        setEditOpen(true);
    }

    function closeEditDialog() {
        setEditOpen(false);
        router.replace(router.asPath);
    }

    async function handleEditSubmit(event: React.FormEvent) {
        setLoading(true);
        event.preventDefault();

        const airport: Airport = {
            id: selectedAirport?.id,
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
            closeEditDialog();
            const index = airports.findIndex(x => x.id === selectedAirport?.id);
            airports[index] = response.data;
        }
        setLoading(false);
    }

    function openDeleteDialog() {
        setDeleteOpen(true);
    }

    function closeDeleteDialog() {
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
            closeDeleteDialog();
            const index = airports.findIndex(x => x.id === selectedAirport?.id);
            airports.splice(index, 1);
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
            <CardList className="mt-10" title="Airports" initialLoad={initialLoad} data={airports} notFoundMessage="No Airports Found"
                showCreate={true} createMessage="Create new Airport" isLoggedIn={isLoggedIn} roleCheck={canAirports} user={user} openCreateDialog={openCreateDialog}>
                <>
                    <table className="table-auto">
                        <thead className="border-b border-gray-400">
                            <tr>
                                <th>Name</th>
                                <th>ICAO</th>
                                <th>Arrivals</th>
                                <th>Departures</th>
                                {canAirports(user) ? (
                                    <th>Actions</th>
                                ) : (<></>)}
                            </tr>
                        </thead>
                        <tbody className="text-lg">
                            {airports?.map((airport) => (
                                <tr key={airport.id} className="border-b border-gray-400">
                                    <td>
                                        <a href={`https://chartfox.org/${airport.icao}`} target="_blank" className="font-medium">
                                            {airport.name}
                                        </a>
                                    </td>
                                    <td>{airport.icao}</td>
                                    <td>{airport.arrivals}</td>
                                    <td>{airport.departures}</td>
                                    {canAirports(user) ? (
                                        <td>
                                            <button onClick={() => {openEditDialog(); setSelectedAirport(airport);}} className="p-2 mx-2 my-2 rounded-md bg-zinc-600 hover:bg-zinc-500">
                                                Edit
                                            </button>
                                            <button onClick={() => {openDeleteDialog(); setSelectedAirport(airport);}} className="p-2 mx-2 rounded-md bg-zinc-600 hover:bg-zinc-500">
                                                Delete
                                            </button>
                                        </td>
                                    ) : (<></>)}
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </>
            </CardList>
            <FormDialog openDialog={createOpen} handleClose={closeCreateDialog} dialogTitle="Create New Airport" handleSubmit={handleCreateSubmit} loading={loading}>
                <label htmlFor="airport" className="mb-4 text-lg font-medium">Name</label>
                <input type="text" id="airport" name="question" className="mb-4 text-gray-800 rounded-md" required />
                <label htmlFor="icao" className="mb-4 text-lg font-medium">ICAO</label>
                <input type="text" id="icao" name="answer" className="mb-4 text-gray-800 rounded-md" required />
            </FormDialog>
            <FormDialog openDialog={editOpen} handleClose={closeEditDialog} dialogTitle="Edit Airport" handleSubmit={handleEditSubmit} loading={loading}>
                <label htmlFor="airport" className="mb-4 text-lg font-medium">Name</label>
                <input type="text" id="airport" name="question" className="mb-4 text-gray-800 rounded-md" defaultValue={selectedAirport?.name} required />
                <label htmlFor="icao" className="mb-4 text-lg font-medium">ICAO</label>
                <input type="text" id="icao" name="answer" className="mb-4 text-gray-800 rounded-md" defaultValue={selectedAirport?.icao} required />
            </FormDialog>
            <DeleteDialog openDialog={deleteOpen} handleClose={closeDeleteDialog} dialogTitle="Delete Airport" handleSubmit={handleDeleteSubmit} loading={loading} />
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
