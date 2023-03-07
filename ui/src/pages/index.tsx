import { useAuthContext } from "@/contexts/AuthContext";
import Head from "next/head";

export default function Home() {

    const [loggedIn, , user] = useAuthContext();

    return (
        <>
            <Head>
                <title>Home | Memphis ARTCC</title>
                <meta name="description" content="Home | Memphis ARTCC" />
                <meta name="viewport" content="width=device-width, initial-scale=1" />
                <link rel="icon" href="/favicon.ico" />
            </Head>
            <div>
                {loggedIn ? (
                    user.roles?.map((role) => (
                        <p key={role}>{role}</p>
                    ))
                ) : (<></>)}
            </div>
        </>
    );
}
