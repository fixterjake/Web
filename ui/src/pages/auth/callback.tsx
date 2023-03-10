import { deleteRedirect, getRedirect, sendCodeCallback, setToken } from "@/services/AuthService";
import { NextPageContext } from "next";
import { useRouter } from "next/router";
import { useEffect } from "react";

type AuthCallbackProps = {
    accessToken?: string;
}

export default function AuthCallback({ accessToken }: AuthCallbackProps) {

    const router = useRouter();

    useEffect(() => {
        if (accessToken) {
            setToken(accessToken);
            router.push(getRedirect() || "/");
            deleteRedirect();
        }
    }, [accessToken, router]);

    return (
        <h1>Loading...</h1>
    );
}

export async function getServerSideProps(ctx: NextPageContext) {
    const res = await sendCodeCallback(process.env.API_URL || "", ctx.query.code as string);
    if (!res.ok) return {props: {}};
    const data = await res.json();
    return {
        props: {
            accessToken: JSON.stringify(data.apiToken.accessToken).replaceAll("\"", "")
        }
    };
}
