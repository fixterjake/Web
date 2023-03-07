import { AuthUser } from "@/models/Auth";
import Loader from "./Loader";

type CardProps = {
    className?: string | undefined;
    title: string;
    initialLoad: boolean;
    data?: unknown[];
    notFoundMessage: string;
    showCreate: boolean;
    createMessage?: string;
    isLoggedIn?: boolean;
    roleCheck: (user: AuthUser) => boolean;
    user?: AuthUser;
    openCreateDialog: () => void;
    children: React.ReactNode;
};

const CardList = ({ className, title, initialLoad, data, notFoundMessage, showCreate, createMessage, isLoggedIn, roleCheck, user, openCreateDialog, children }: CardProps) => {
    return (
        <div className={`rounded-lg shadow-lg bg-zinc-700 ${className}`}>
            <div className="flex items-center justify-between p-4 text-center">
                <h2 className="w-full mx-[15%] text-2xl font-bold border-b-2 pb-8">{title}</h2>
            </div>
            <div className="p-4">
                <div className="flex flex-col space-y-4 text-center mx-[15%]">
                    {!initialLoad ? (
                        <>
                            {data !== undefined && data?.length > 0 ? (
                                <>
                                    {children}
                                </>
                            ) : (
                                <div className="flex flex-col space-y-2">
                                    <h1 className="text-xl font-medium">{notFoundMessage}</h1>
                                </div>
                            )}
                        </>
                    ) : (
                        <Loader />
                    )}
                    {showCreate && user !== undefined ? (
                        <>
                            {isLoggedIn && roleCheck(user) ? (
                                <div className="pt-4">
                                    <button onClick={openCreateDialog} className="flex justify-center w-[15rem] ml-auto mr-auto px-4 py-2 text-lg
                                    font-medium rounded-lg text-zinc-100 bg-zinc-600 hover:bg-zinc-500">
                                        {createMessage}
                                    </button>
                                </div>
                            ) : (<></>)}
                        </>
                    ) : (<></>)}
                </div>
            </div>
        </div>
    );
};

export default CardList;