
type ContainerProps = {
    children: React.ReactNode;
}

const Container = ({children}: ContainerProps) => {
    return (
        <div className="mx-[15%]">
            {children}
        </div>
    );
};

export default Container;