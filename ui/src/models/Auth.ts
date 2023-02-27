export type ApiToken = {
    sub: number;
    cid: number;
    email: string;
    country: string;
    givenname: string;
    surname: string;
    rating: string;
    ratingInt: number;
    division: string;
    scp: string;
    roles: string[];
    exp: number;
    iss: string;
    aud: string;
}

export type AuthUser = {
    cid: number;
    email: string;
    firstName: string;
    lastName: string;
    fullName: string;
    rating: string;
    ratingInt: number;
    division: string;
    roles: string[];
}