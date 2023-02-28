import { useEffect } from "react";

export default function LoginPage() {
    useEffect(() => {
        window.location.replace(`${process.env.NEXT_PUBLIC_API_URL}/auth/login/redirect`);
    });

    return (
        <h1>Loading...</h1>
    );
}