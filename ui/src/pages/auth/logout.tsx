import { useAuthContext } from "@/contexts/AuthContext";
import { deleteRedirect, deleteToken, getRedirect, sendLogout } from "@/services/AuthService";
import { useRouter } from "next/router";
import { useEffect } from "react";

type LogoutProps = {
    apiUrl: string;
}

export default function AuthLogout({ apiUrl }: LogoutProps) {

    const [loggedIn, setLoggedIn] = useAuthContext();

    const router = useRouter();

    useEffect(() => {
        console.log("logout");
        console.log(`loggedIn: ${loggedIn}`);
        if (loggedIn)
            sendLogout(apiUrl).then(() => {
                console.log("sent logout request");
                setLoggedIn(false);
                deleteToken();
                router.push(getRedirect() || "/");
                deleteRedirect();
            });
    }, [apiUrl, loggedIn, setLoggedIn, router]);

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