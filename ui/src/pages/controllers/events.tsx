import {Event} from "@/models/Event";

type EventProps = {
    events: Event[];
    apiKey: string;
}

export default function EventsPage({ events }: EventProps) {
    return (
        <div>
            <h1>Events</h1>
            {events.map((event) => (
                <div key={event.id}>
                    <h3>{event.title}</h3>
                </div>
            ))}
        </div>
    );
}

export async function getServerSideProps() {
    const apiKey = process.env.API_KEY;
    const res = await fetch(`${apiKey}/events/all`);
    const response = await res.json();
    return {
        props: {
            events: response.data,
            apiKey: apiKey,
        },
    };
}