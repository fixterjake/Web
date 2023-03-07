import DeleteDialog from "@/components/shared/DeleteDialog";
import FormDialog from "@/components/shared/FormDialog";
import { useAuthContext } from "@/contexts/AuthContext";
import { Faq } from "@/models/Faq";
import { getToken, isFullStaff } from "@/Helpers/AuthHelper";
import { Disclosure } from "@headlessui/react";
import Head from "next/head";
import { useRouter } from "next/router";
import { useEffect, useState } from "react";
import CardList from "@/components/shared/CardList";

type FaqProps = {
    apiUrl: string;
}

export default function FaqPage({ apiUrl }: FaqProps) {

    const router = useRouter();

    const [initialLoad, setInitialLoad] = useState(true);
    const [faqs, setFaqs] = useState<Faq[]>([]);

    const [createOpen, setCreateOpen] = useState(false);
    const [editOpen, setEditOpen] = useState(false);
    const [deleteOpen, setDeleteOpen] = useState(false);
    const [selectedFaq, setSelectedFaq] = useState<Faq>();

    const [isLoggedIn, , user] = useAuthContext();
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        const fetchFaqs = async () => {
            const res = await fetch(`${apiUrl}/faq/all`, {
                method: "GET",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${getToken()}`
                },
            });
            const response = await res.json();
            setFaqs(response.data);
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

        const faq: Faq = {
            question: (event.target as HTMLFormElement).question.value,
            answer: (event.target as HTMLFormElement).answer.value,
            order: (event.target as HTMLFormElement).order.value
        };
        const jsonData = JSON.stringify(faq);

        const res = await fetch(`${apiUrl}/faq`, {
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
            faqs.push(response.data);
            faqs.sort((a, b) => a.order - b.order);
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

        const faq: Faq = {
            id: selectedFaq?.id,
            question: (event.target as HTMLFormElement).question.value,
            answer: (event.target as HTMLFormElement).answer.value,
            order: (event.target as HTMLFormElement).order.value
        };
        const jsonData = JSON.stringify(faq);

        const res = await fetch(`${apiUrl}/faq`, {
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
            const index = faqs.findIndex(x => x.id === selectedFaq?.id);
            faqs[index] = response.data;
            faqs.sort((a, b) => a.order - b.order);
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
        const res = await fetch(`${apiUrl}/faq?faqId=${selectedFaq?.id}`, {
            method: "DELETE",
            headers: {
                "Authorization": `Bearer ${getToken()}`
            }
        });
        const response = await res.json();
        if (response.statusCode === 200) {
            closeDeleteDialog();
            const index = faqs.findIndex(x => x.id === selectedFaq?.id);
            faqs.splice(index, 1);
            faqs.sort((a, b) => a.order - b.order);
        }
        setLoading(false);
    }

    return (
        <>
            <Head>
                <title>FAQ | Memphis ARTCC</title>
                <meta name="description" content="FAQ :: ARTCC" />
                <meta name="viewport" content="width=device-width, initial-scale=1" />
                <link rel="icon" href="/favicon.ico" />
            </Head>
            <CardList className="mt-10" title="Frequently Asked Questions" initialLoad={initialLoad} data={faqs} notFoundMessage="No FAQs Found"
                showCreate={true} createMessage="Create new FAQ Entry" isLoggedIn={isLoggedIn} roleCheck={isFullStaff} user={user} openCreateDialog={openCreateDialog}>
                <>
                    {faqs?.sort(x => x.order)?.map((faq) => (
                        <div key={faq.id} className="flex flex-col space-y-2">
                            <Disclosure>
                                {({ open }) => (
                                    <>
                                        <Disclosure.Button className="flex justify-between w-full px-4 py-2 text-lg font-medium rounded-lg text-zinc-100 bg-zinc-600 hover:bg-zinc-500">
                                            <span>{faq.question}</span>
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
                                            {faq.answer}
                                            <br />
                                            {isLoggedIn && isFullStaff(user) ? (
                                                <div className="mt-4 ml-auto mr-auto w-[15rem]">
                                                    <button onClick={() => {openEditDialog(); setSelectedFaq(faq);}} className="p-2 mx-2 rounded-md bg-zinc-600 hover:bg-zinc-500">
                                                                    Edit
                                                    </button>
                                                    <button onClick={() => {openDeleteDialog(); setSelectedFaq(faq);}} className="p-2 mx-2 rounded-md bg-zinc-600 hover:bg-zinc-500">
                                                                    Delete
                                                    </button>
                                                </div>
                                            ) : (<></>)}
                                        </Disclosure.Panel>
                                    </>
                                )}
                            </Disclosure>
                        </div>
                    ))}
                </>
            </CardList>
            <FormDialog openDialog={createOpen} handleClose={closeCreateDialog} dialogTitle="Create New FAQ" handleSubmit={handleCreateSubmit} loading={loading}>
                <label htmlFor="question" className="mb-4 text-lg font-medium">Question</label>
                <textarea id="question" name="question" className="mb-4 text-gray-800 rounded-md" required />
                <label htmlFor="answer" className="mb-4 text-lg font-medium">Answer</label>
                <textarea id="answer" name="answer" className="mb-4 text-gray-800 rounded-md" required />
                <label htmlFor="order" className="mb-4 text-lg font-medium text-center">Order</label>
                <input type="number" id="order" name="order" className="text-gray-800 rounded-md w-[5rem] ml-auto mr-auto" required />
            </FormDialog>
            <FormDialog openDialog={editOpen} handleClose={closeEditDialog} dialogTitle="Edit FAQ" handleSubmit={handleEditSubmit} loading={loading}>
                <label htmlFor="question" className="mb-4 text-lg font-medium">Question</label>
                <textarea id="question" name="question" className="mb-4 text-gray-800 rounded-md" defaultValue={selectedFaq?.question} required />
                <label htmlFor="answer" className="mb-4 text-lg font-medium">Answer</label>
                <textarea id="answer" name="answer" className="mb-4 text-gray-800 rounded-md" defaultValue={selectedFaq?.answer} required />
                <label htmlFor="order" className="mb-4 text-lg font-medium text-center">Order</label>
                <input type="number" id="order" name="order" className="text-gray-800 rounded-md w-[5rem] ml-auto mr-auto" defaultValue={selectedFaq?.order} required />
            </FormDialog>
            <DeleteDialog openDialog={deleteOpen} handleClose={closeDeleteDialog} dialogTitle="Delete FAQ" handleSubmit={handleDeleteSubmit} loading={loading} />
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
