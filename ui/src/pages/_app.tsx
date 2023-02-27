import type { AppProps } from "next/app";
import "@/styles/theme.scss";
import { AuthProvidor } from "@/contexts/AuthContext";
import Navigation from "@/components/shared/Navigation";
import Container from "@/components/shared/Container";

export default function App({ Component, pageProps }: AppProps) {
    return (
        <AuthProvidor>
            <Navigation />
            <Container>
                <Component {...pageProps} />
            </Container>
        </AuthProvidor>
    );
}
