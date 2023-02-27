// eslint-disable-next-line @typescript-eslint/no-var-requires
const path = require("path");

/** @type {import('next').NextConfig} */
const nextConfig = {
    reactStrictMode: true,
    output: "standalone",
    sassOptions: {
        includePaths: [path.join(__dirname, "styles")],
    },
    async redirects() {
        return [
            {
                source: "/login",
                destination: `${process.env.NEXT_PUBLIC_API_URL}/auth/login/redirect`,
                permanent: true,
            }
        ];
    }
};

module.exports = nextConfig;
