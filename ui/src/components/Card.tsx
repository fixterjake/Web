type CardProps = {
    children: React.ReactNode;
    title: string;
    className?: string | undefined;
};

const Card = ({ children, title, className }: CardProps) => {
    return (
        <div className={`rounded-lg shadow-lg bg-zinc-700 ${className}`}>
            <div className="flex items-center justify-between p-4 text-center">
                <h2 className="w-full mx-[15%] text-xl font-bold border-b-2 pb-8">{title}</h2>
            </div>
            <div className="p-4">{children}</div>
        </div>
    );
};

export default Card;