import { useEffect } from "react";

type LoginProps = {
    apiUrl: string;
}

export default function LoginPage({ apiUrl }: LoginProps) {
    useEffect(() => {
        window.location.href = `${apiUrl}/auth/login/redirect`;
    });

    return (
        <h1>Loading...</h1>
    );
}

export function getServerSideProps() {
    return {
        props: {
            apiUrl: process.env.API_URL
        }
    };
}