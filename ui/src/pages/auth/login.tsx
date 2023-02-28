import { useEffect } from "react";
import { redirect } from "next/navigation";

export default function LoginPage() {
    useEffect(() => {
        redirect(`${process.env.NEXT_PUBLIC_API_URL}/auth/login/redirect`);
    });

    return (
        <h1>Loading...</h1>
    );
}