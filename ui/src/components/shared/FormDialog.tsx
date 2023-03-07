import { Dialog, Transition } from "@headlessui/react";
import { Fragment } from "react";
import Loader from "./Loader";

type FormDialogProps = {
    openDialog: boolean;
    handleClose: () => void;
    dialogTitle: string;
    handleSubmit: (event: React.FormEvent) => void;
    loading: boolean;
    children: React.ReactNode;
}

const FormDialog = ({ openDialog, handleClose, dialogTitle, handleSubmit, loading, children }: FormDialogProps) => {
    return (
        <Transition appear show={openDialog} as={Fragment}>
            <Dialog as="div" className="relative z-10" onClose={handleClose}>
                <Transition.Child
                    as={Fragment}
                    enter="ease-out duration-300"
                    enterFrom="opacity-0"
                    enterTo="opacity-100"
                    leave="ease-in duration-200"
                    leaveFrom="opacity-100"
                    leaveTo="opacity-0"
                >
                    <div className="fixed inset-0 bg-black bg-opacity-25" />
                </Transition.Child>
                <div className="fixed inset-0 overflow-y-auto">
                    <div className="flex items-center justify-center min-h-full p-4 text-center">
                        <Transition.Child
                            as={Fragment}
                            enter="ease-out duration-300"
                            enterFrom="opacity-0 scale-95"
                            enterTo="opacity-100 scale-100"
                            leave="ease-in duration-200"
                            leaveFrom="opacity-100 scale-100"
                            leaveTo="opacity-0 scale-95"
                        >
                            <Dialog.Panel className="w-full max-w-md p-6 overflow-hidden text-left align-middle transition-all transform shadow-xl bg-zinc-700 rounded-2xl">
                                <Dialog.Title as="h2" className="pb-4 mb-4 text-lg font-medium leading-6 text-center text-white border-b-2">
                                    {dialogTitle}
                                </Dialog.Title>
                                <form onSubmit={handleSubmit}>
                                    <div className="flex flex-col">
                                        {children}
                                        <button type="submit" className="w-auto p-2 mt-4 ml-auto rounded-md bg-zinc-600 hover:bg-zinc-500">
                                            <div className="flex">
                                                {loading ? (
                                                    <Loader />
                                                ) : (
                                                    <span>Submit</span>
                                                )}
                                            </div>
                                        </button>
                                    </div>
                                </form>
                            </Dialog.Panel>
                        </Transition.Child>
                    </div>
                </div>
            </Dialog>
        </Transition>
    );
};

export default FormDialog;