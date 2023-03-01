import { ApiToken, AuthUser } from "@/models/Auth";

export async function sendCodeCallback(apiUrl: string, code: string): Promise<Response> {
    return await fetch(`${apiUrl}/Auth/login/callback`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({"authorization_code": code}),
    });
}

export async function sendLogout(apiUrl: string): Promise<void> {
    await fetch(`${apiUrl}/Auth/logout`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${getToken()}`
        }
    });
}

function parseJwt(token: string): ApiToken | undefined {
    if (!token)
        return;
    const base64Url = token.split(".")[1];
    if (!base64Url)
        return;
    const base64 = base64Url.replace("-", "+").replace("_", "/");
    return JSON.parse(window.atob(base64));
}

export function getToken(): string | null {
    return localStorage.getItem("accessToken");
}

export function setToken(token: string): void {
    localStorage.setItem("accessToken", token);
}

export function deleteToken(): void {
    localStorage.removeItem("accessToken");
}

export function setRedirect(url: string): void {
    localStorage.setItem("redirect", url);
}

export function getRedirect(): string | null {
    return localStorage.getItem("redirect");
}

export function deleteRedirect(): void {
    localStorage.removeItem("redirect");
}

export function isLoggedIn(): boolean {
    const token = getToken();
    return !!token;
}

export function getUser(): AuthUser | null {
    const token = getToken();
    if (!token)
        return null;
    const parsed = parseJwt(token);
    if (!parsed)
        return null;
    return {
        cid: parsed.cid || 0,
        email: parsed.email || "",
        firstName: parsed.givenname || "",
        lastName: parsed.surname || "",
        fullName: `${parsed.givenname} ${parsed.surname}` || "",
        rating: parsed.rating || "",
        ratingInt: parsed.ratingInt || 0,
        division: parsed.division || "",
        roles: parsed.roles || []
    };
}

export function isStaff(user: AuthUser | null): boolean {
    if (!user)
        return false;
    return user.roles?.some(r => ["ATM", "DATM", "TA", "ATA", "WM", "AWM", "EC", "AEC", "FE", "AFE"].includes(r));
}

export function isFullStaff(user: AuthUser | null): boolean {
    if (!user)
        return false;
    return user.roles?.some(r => ["ATM", "DATM", "TA", "WM", "EC", "FE"].includes(r));
}

export function isSeniorStaff(user: AuthUser | null): boolean {
    if (!user)
        return false;
    return user.roles?.some(r => ["ATM", "DATM", "TA", "WM"].includes(r));
}

export function isTrainingStaff(user: AuthUser | null): boolean {
    if (!user)
        return false;
    return user.roles?.some(r => ["ATM", "DATM", "TA", "ATA", "WM", "INS", "MTR"].includes(r));
}

export function isSeniorTrainingStaff(user: AuthUser | null): boolean {
    if (!user)
        return false;
    return user.roles?.some(r => ["ATM", "DATM", "TA", "ATA", "WM", "INS"].includes(r));
}

export function hasRole(user: AuthUser | null, role: string): boolean {
    if (!user)
        return false;
    return user.roles?.some(r => r === role);
}

export function canAirports(user: AuthUser | null): boolean {
    if (!user)
        return false;
    return user.roles?.some(r => ["ATM", "DATM", "TA", "WM", "FE", "AFE"].includes(r));
}