import { useAuthContext } from "@/contexts/AuthContext";
import { deleteRedirect, deleteToken, getRedirect, sendLogout } from "@/services/AuthService";
import { useRouter } from "next/router";
import { useEffect } from "react";

export default function AuthLogout() {

    const [loggedIn, setLoggedIn] = useAuthContext();

    const router = useRouter();

    useEffect(() => {
        if (loggedIn)
            sendLogout().then(() => {
                setLoggedIn(false);
                deleteToken();
                router.push(getRedirect() || "/");
                deleteRedirect();
            });
    }, [loggedIn, setLoggedIn, router]);

    return (
        <h1>Loading...</h1>
    );
}