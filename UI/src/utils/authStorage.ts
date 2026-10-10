import type { LoginResponse } from "../types/auth";

const AUTH_KEY = "auth";

export const authStorage = {
  save(data: LoginResponse): void {
    localStorage.setItem(AUTH_KEY, JSON.stringify(data));
  },

  get(): LoginResponse | null {
    try {
      const data = localStorage.getItem(AUTH_KEY);
      return data ? (JSON.parse(data) as LoginResponse) : null;
    } catch {
      return null;
    }
  },

  getToken(): string | null {
    const auth = this.get();

    if (!auth) return null;

    if (Date.now() >= new Date(auth.expiresAt).getTime()) {
      this.clear();
      return null;
    }

    return auth.token;
  },

  clear(): void {
    localStorage.removeItem(AUTH_KEY);
  },
};
