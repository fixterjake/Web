import Card from "@/components/Card";
import { Faq } from "@/models/Faq";
import { getToken } from "@/services/AuthService";
import Head from "next/head";

export default function CreateFaqPage() {

    async function handleSubmit(event: React.FormEvent) {
        event.preventDefault();

        const faq: Faq = {
            question: (event.target as HTMLFormElement).question.value,
            answer: (event.target as HTMLFormElement).answer.value,
            order: (event.target as HTMLFormElement).order.value
        };
        const jsonData = JSON.stringify(faq);

        const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/faq`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${getToken()}`
            },
            body: jsonData
        });
        const response = await res.json();
        console.log(res);
        console.log(response);
    }

    return (
        <>
            <Head>
                <title>Create FAQ | Memphis ARTCC</title>
                <meta name="description" content="FAQ :: ARTCC" />
                <meta name="viewport" content="width=device-width, initial-scale=1" />
                <link rel="icon" href="/favicon.ico" />
            </Head>
            <Card title="Create New FAQ Entry" className="mt-10">
                <form onSubmit={handleSubmit}>
                    <div className="flex flex-col mx-[35%]">
                        <label htmlFor="question" className="mb-4 text-lg font-medium">Question</label>
                        <textarea id="question" name="question" className="mb-4 text-gray-800 rounded-md" required />
                        <label htmlFor="answer" className="mb-4 text-lg font-medium">Answer</label>
                        <textarea id="answer" name="answer" className="text-gray-800 rounded-md" required />
                        <label htmlFor="order" className="mb-4 text-lg font-medium">Order</label>
                        <input type="number" id="order" name="order" className="text-gray-800 rounded-md" required />
                        <button type="submit" className="ml-auto p-2 w-[4rem] mt-4 rounded-md bg-zinc-600 hover:bg-zinc-500">Submit</button>
                    </div>
                </form>
            </Card>
        </>
    );
}