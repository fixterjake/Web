
type ContainerProps = {
    children: React.ReactNode;
}

const Container = ({children}: ContainerProps) => {
    return (
        <div className="mx-[15%] flex-grow">
            {children}
        </div>
    );
};

export default Container;