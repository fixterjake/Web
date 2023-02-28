import Card from "@/components/Card";
import { useAuthContext } from "@/contexts/AuthContext";
import { Faq } from "@/models/Faq";
import { getToken, isFullStaff } from "@/services/AuthService";
import { Dialog, Disclosure, Transition } from "@headlessui/react";
import Head from "next/head";
import { useRouter } from "next/router";
import { Fragment, useState } from "react";

type FaqProps = {
    faqs?: Faq[];
    apiUrl: string;
}

export default function FaqPage({ faqs, apiUrl }: FaqProps) {

    const router = useRouter();
    const [createOpen, setCreateOpen] = useState(false);
    const [editOpen, setEditOpen] = useState(false);
    const [deleteOpen, setDeleteOpen] = useState(false);
    const [selectedFaq, setSelectedFaq] = useState<Faq>();
    const [isLoggedIn, , user] = useAuthContext();

    function closeCreateModal() {
        setCreateOpen(false);
        router.replace(router.asPath);
    }

    function openCreateModal() {
        setCreateOpen(true);
    }

    async function handleCreateSubmit(event: React.FormEvent) {
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
            closeCreateModal();
        }
    }

    function openEditModal() {
        setEditOpen(true);
    }

    function closeEditModal() {
        setEditOpen(false);
        router.replace(router.asPath);
    }

    async function handleEditSubmit(event: React.FormEvent) {
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
            closeEditModal();
        }
    }

    function openDeleteModel() {
        setDeleteOpen(true);
    }

    function closeDeleteModal() {
        setDeleteOpen(false);
        router.replace(router.asPath);
    }

    async function handleDeleteSubmit() {
        const res = await fetch(`${apiUrl}/faq?faqId=${selectedFaq?.id}`, {
            method: "DELETE",
            headers: {
                "Authorization": `Bearer ${getToken()}`
            }
        });
        const response = await res.json();
        if (response.statusCode === 200) {
            closeDeleteModal();
        }
    }

    return (
        <>
            <Head>
                <title>FAQ | Memphis ARTCC</title>
                <meta name="description" content="FAQ :: ARTCC" />
                <meta name="viewport" content="width=device-width, initial-scale=1" />
                <link rel="icon" href="/favicon.ico" />
            </Head>
            <Card title="Frequently Asked Questions" className="mt-10">
                <div className="flex flex-col space-y-4 text-center mx-[15%]">
                    {faqs !== undefined && faqs.length > 0 ? (
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
                                                            <button onClick={() => {openEditModal(); setSelectedFaq(faq);}} className="p-2 mx-2 rounded-md bg-zinc-600 hover:bg-zinc-500">
                                                                Edit
                                                            </button>
                                                            <button onClick={() => {openDeleteModel(); setSelectedFaq(faq);}} className="p-2 mx-2 rounded-md bg-zinc-600 hover:bg-zinc-500">
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
                    ) : (
                        <div className="flex flex-col space-y-2">
                            <h1 className="text-xl font-medium">No FAQs found</h1>
                        </div>
                    )}
                    {isLoggedIn && isFullStaff(user) ? (
                        <div className="pt-4">
                            <button onClick={openCreateModal} className="flex justify-center w-[15rem] ml-auto mr-auto px-4 py-2 text-lg
                                font-medium rounded-lg text-zinc-100 bg-zinc-600 hover:bg-zinc-500">
                                Create new FAQ entry
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
                                        Create new FAQ entry
                                    </Dialog.Title>
                                    <form onSubmit={handleCreateSubmit}>
                                        <div className="flex flex-col">
                                            <label htmlFor="question" className="mb-4 text-lg font-medium">Question</label>
                                            <textarea id="question" name="question" className="mb-4 text-gray-800 rounded-md" required />
                                            <label htmlFor="answer" className="mb-4 text-lg font-medium">Answer</label>
                                            <textarea id="answer" name="answer" className="mb-4 text-gray-800 rounded-md" required />
                                            <label htmlFor="order" className="mb-4 text-lg font-medium text-center">Order</label>
                                            <input type="number" id="order" name="order" className="text-gray-800 rounded-md w-[5rem] ml-auto mr-auto" required />
                                            <button type="submit" className="ml-auto p-2 w-[4rem] mt-4 rounded-md bg-zinc-600 hover:bg-zinc-500">Submit</button>
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
                                        Edit FAQ entry
                                    </Dialog.Title>
                                    <form onSubmit={handleEditSubmit}>
                                        <div className="flex flex-col">
                                            <label htmlFor="question" className="mb-4 text-lg font-medium">Question</label>
                                            <textarea id="question" name="question" className="mb-4 text-gray-800 rounded-md" defaultValue={selectedFaq?.question} required />
                                            <label htmlFor="answer" className="mb-4 text-lg font-medium">Answer</label>
                                            <textarea id="answer" name="answer" className="mb-4 text-gray-800 rounded-md" defaultValue={selectedFaq?.answer} required />
                                            <label htmlFor="order" className="mb-4 text-lg font-medium text-center">Order</label>
                                            <input type="number" id="order" name="order" className="text-gray-800 rounded-md w-[5rem] ml-auto mr-auto" defaultValue={selectedFaq?.order} required />
                                            <button type="submit" className="ml-auto p-2 w-[4rem] mt-4 rounded-md bg-zinc-600 hover:bg-zinc-500">Submit</button>
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
                                        Delete FAQ Entry
                                    </Dialog.Title>
                                    <div className="w-full text-right">
                                        <div className="text-lg font-medium text-center">This action cannot be undone.</div>
                                        <br />
                                        <button onClick={handleDeleteSubmit} className="ml-auto p-2 w-[4rem] mt-4 rounded-md bg-zinc-600 hover:bg-zinc-500">Delete</button>
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
    const res = await fetch(`${apiUrl}/faq/all`);
    const response = await res.json();
    return {
        props: {
            faqs: response.data,
            apiUrl: apiUrl,
        }
    };
}