import { useEffect } from "react";

export default function LoginPage() {
    useEffect(() => {
        const apiUrl = process.env.NEXT_PUBLIC_API_URL;
        console.log(apiUrl);
        window.location.href = `${apiUrl}/auth/login/redirect`;
    });

    return (
        <h1>Loading...</h1>
    );
}