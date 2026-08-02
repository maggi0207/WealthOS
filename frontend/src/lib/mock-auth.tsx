import { createContext, useCallback, useContext, useEffect, useMemo, useState, type ReactNode } from "react";

/**
 * Mock authentication — frontend only.
 * No backend, no provider: the "session" is a plain object persisted to localStorage.
 * Swap this module out when a real auth layer is introduced.
 */

export type MockUser = {
  id: string;
  name: string;
  email: string;
  initials: string;
  plan: string;
};

const STORAGE_KEY = "wealthos.session";

const DEMO_USER: MockUser = {
  id: "usr_demo_001",
  name: "Magesh Kumar",
  email: "magesh@wealthos.app",
  initials: "MK",
  plan: "Private Wealth",
};

type AuthContextValue = {
  user: MockUser | null;
  isReady: boolean;
  login: (email: string, password: string) => Promise<MockUser>;
  register: (name: string, email: string, password: string) => Promise<MockUser>;
  logout: () => void;
};

const AuthContext = createContext<AuthContextValue | null>(null);

function makeUser(email: string, name?: string): MockUser {
  const display = name?.trim() || email.split("@")[0] || DEMO_USER.name;
  const initials = display
    .split(/[\s._-]+/)
    .filter(Boolean)
    .slice(0, 2)
    .map((part) => part[0]!.toUpperCase())
    .join("");
  return { ...DEMO_USER, id: `usr_${email.length}${display.length}`, name: display, email, initials: initials || "WO" };
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<MockUser | null>(null);
  const [isReady, setIsReady] = useState(false);

  useEffect(() => {
    try {
      const raw = window.localStorage.getItem(STORAGE_KEY);
      if (raw) setUser(JSON.parse(raw) as MockUser);
    } catch {
      // ignore malformed session
    }
    setIsReady(true);
  }, []);

  const persist = useCallback((next: MockUser) => {
    setUser(next);
    window.localStorage.setItem(STORAGE_KEY, JSON.stringify(next));
    return next;
  }, []);

  const login = useCallback<AuthContextValue["login"]>(
    async (email) => {
      await new Promise((r) => setTimeout(r, 450));
      return persist(makeUser(email));
    },
    [persist],
  );

  const register = useCallback<AuthContextValue["register"]>(
    async (name, email) => {
      await new Promise((r) => setTimeout(r, 550));
      return persist(makeUser(email, name));
    },
    [persist],
  );

  const logout = useCallback(() => {
    setUser(null);
    window.localStorage.removeItem(STORAGE_KEY);
  }, []);

  const value = useMemo(
    () => ({ user, isReady, login, register, logout }),
    [user, isReady, login, register, logout],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used inside <AuthProvider>");
  return ctx;
}
