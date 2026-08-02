import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from "react";

import { isApiMode, isMockApiMode } from "@/config/env";
import { authService, onApiAuthLogout, tokenStorage } from "@/services/auth/auth-service";
import type { UserProfile } from "@/services/http/types";
import { ApiError } from "@/services/http/problem-details";

/**
 * Auth provider — mock session when `VITE_API_MODE=mock`, real JWT auth when `api`.
 * UI continues to consume `useAuth()`; API mode persists tokens via `tokenStorage`.
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
  logout: () => Promise<void>;
};

const AuthContext = createContext<AuthContextValue | null>(null);

function initialsFromName(display: string): string {
  return (
    display
      .split(/[\s._-]+/)
      .filter(Boolean)
      .slice(0, 2)
      .map((part) => part[0]!.toUpperCase())
      .join("") || "WO"
  );
}

function makeUser(email: string, name?: string): MockUser {
  const display = name?.trim() || email.split("@")[0] || DEMO_USER.name;
  return {
    ...DEMO_USER,
    id: `usr_${email.length}${display.length}`,
    name: display,
    email,
    initials: initialsFromName(display),
  };
}

function profileToUser(profile: UserProfile): MockUser {
  const name =
    profile.displayName?.trim() ||
    [profile.firstName, profile.lastName].filter(Boolean).join(" ").trim() ||
    profile.email;
  return {
    id: profile.id,
    name,
    email: profile.email,
    initials: initialsFromName(name),
    plan: "Private Wealth",
  };
}

function splitFullName(fullName: string): { firstName: string; lastName: string } {
  const parts = fullName.trim().split(/\s+/).filter(Boolean);
  if (parts.length === 0) {
    return { firstName: "User", lastName: "Account" };
  }
  if (parts.length === 1) {
    return { firstName: parts[0]!, lastName: parts[0]! };
  }
  return { firstName: parts[0]!, lastName: parts.slice(1).join(" ") };
}

function persistLocalSession(next: MockUser): void {
  window.localStorage.setItem(STORAGE_KEY, JSON.stringify(next));
}

function clearLocalSession(): void {
  window.localStorage.removeItem(STORAGE_KEY);
}

function authErrorMessage(error: unknown): string {
  if (error instanceof ApiError) {
    return error.message || "Authentication failed";
  }
  if (error instanceof Error && error.message) {
    return error.message;
  }
  return "Authentication failed";
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<MockUser | null>(null);
  const [isReady, setIsReady] = useState(false);

  useEffect(() => {
    let cancelled = false;

    async function hydrate() {
      try {
        if (isMockApiMode()) {
          const raw = window.localStorage.getItem(STORAGE_KEY);
          if (raw) setUser(JSON.parse(raw) as MockUser);
          return;
        }

        if (!tokenStorage.getAccessToken()) {
          clearLocalSession();
          setUser(null);
          return;
        }

        const profile = await authService.me();
        if (cancelled) return;
        const next = profileToUser(profile);
        setUser(next);
        persistLocalSession(next);
      } catch {
        if (cancelled) return;
        tokenStorage.clear();
        clearLocalSession();
        setUser(null);
      } finally {
        if (!cancelled) setIsReady(true);
      }
    }

    void hydrate();
    return () => {
      cancelled = true;
    };
  }, []);

  useEffect(() => {
    if (!isApiMode()) return;
    return onApiAuthLogout(() => {
      clearLocalSession();
      setUser(null);
    });
  }, []);

  const login = useCallback<AuthContextValue["login"]>(async (email, password) => {
    if (isMockApiMode()) {
      await new Promise((r) => setTimeout(r, 450));
      const next = makeUser(email);
      setUser(next);
      persistLocalSession(next);
      return next;
    }

    try {
      const tokens = await authService.login({ email, password });
      const next = profileToUser(tokens.user);
      setUser(next);
      persistLocalSession(next);
      return next;
    } catch (error) {
      throw new Error(authErrorMessage(error));
    }
  }, []);

  const register = useCallback<AuthContextValue["register"]>(
    async (name, email, password) => {
      if (isMockApiMode()) {
        await new Promise((r) => setTimeout(r, 550));
        const next = makeUser(email, name);
        setUser(next);
        persistLocalSession(next);
        return next;
      }

      const { firstName, lastName } = splitFullName(name);
      try {
        const tokens = await authService.register({
          email,
          password,
          confirmPassword: password,
          firstName,
          lastName,
        });
        const next = profileToUser(tokens.user);
        setUser(next);
        persistLocalSession(next);
        return next;
      } catch (error) {
        throw new Error(authErrorMessage(error));
      }
    },
    [],
  );

  const logout = useCallback(async () => {
    if (isApiMode()) {
      try {
        await authService.logout();
      } catch {
        tokenStorage.clear();
      }
    }
    setUser(null);
    clearLocalSession();
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
