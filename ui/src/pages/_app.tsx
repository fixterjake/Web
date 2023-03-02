import type { AppProps } from "next/app";
import "@/styles/theme.scss";
import { AuthProvidor } from "@/contexts/AuthContext";
import Navigation from "@/components/shared/Navigation";
import Container from "@/components/shared/Container";
import Footer from "@/components/shared/Footer";

export default function App({ Component, pageProps }: AppProps) {
    return (
        <AuthProvidor>
            <div className="flex flex-col min-h-screen">
                <Navigation />
                <Container>
                    <Component {...pageProps} />
                </Container>
                <Footer />
            </div>
        </AuthProvidor>
    );
}