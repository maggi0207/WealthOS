import { Link, createFileRoute, useNavigate } from "@tanstack/react-router";
import { Gem, Loader2 } from "lucide-react";
import { useEffect, useState, type FormEvent } from "react";
import { toast } from "sonner";

import { Button } from "@/components/ui/button";
import { Field } from "@/components/ui-kit/field";
import { isMockApiMode } from "@/config/env";
import { useAuth } from "@/lib/mock-auth";
import { AuthShell } from "@/components/auth/auth-shell";

export const Route = createFileRoute("/login")({
  head: () => ({
    meta: [
      { title: "Sign in — WealthOS" },
      { name: "description", content: "Sign in to your WealthOS wealth workspace." },
      { property: "og:title", content: "Sign in — WealthOS" },
      { property: "og:description", content: "Sign in to your WealthOS wealth workspace." },
    ],
  }),
  component: LoginPage,
});

function LoginPage() {
  const { login, user } = useAuth();
  const navigate = useNavigate();
  const mockMode = isMockApiMode();
  const [email, setEmail] = useState(mockMode ? "alex@wealthos.app" : "");
  const [password, setPassword] = useState(mockMode ? "demo1234" : "");
  const [touched, setTouched] = useState({ email: false, password: false });
  const [pending, setPending] = useState(false);

  const errors = {
    email: /^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(email) ? null : "Enter a valid email address",
    password: password.length >= (mockMode ? 6 : 8) ? null : mockMode
      ? "Password must be at least 6 characters"
      : "Password must be at least 8 characters",
  };
  const invalid = Boolean(errors.email || errors.password);

  useEffect(() => {
    if (user) navigate({ to: "/dashboard" });
  }, [user, navigate]);

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    setTouched({ email: true, password: true });
    if (invalid || pending) return;
    setPending(true);
    try {
      await login(email, password);
      toast.success(mockMode ? "Signed in (mock session)" : "Signed in");
      navigate({ to: "/dashboard" });
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Sign in failed");
    } finally {
      setPending(false);
    }
  }

  return (
    <AuthShell
      title="Welcome back"
      subtitle={
        mockMode
          ? "Sign in to your WealthOS workspace. Authentication is mocked — any credentials work."
          : "Sign in with your WealthOS account."
      }
    >
      <form onSubmit={onSubmit} noValidate className="space-y-4">
        <Field
          id="email"
          label="Email"
          type="email"
          inputMode="email"
          autoComplete="email"
          autoCapitalize="none"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          onBlur={() => setTouched((t) => ({ ...t, email: true }))}
          error={touched.email ? errors.email : null}
        />
        <Field
          id="password"
          label="Password"
          type="password"
          autoComplete="current-password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          onBlur={() => setTouched((t) => ({ ...t, password: true }))}
          error={touched.password ? errors.password : null}
        />
        <Button type="submit" className="min-h-11 w-full rounded-full" disabled={pending}>
          {pending ? <Loader2 className="size-4 animate-spin" /> : <Gem className="size-4" />}
          {pending ? "Signing in…" : "Sign in"}
        </Button>
      </form>
      <p className="mt-5 text-center text-sm text-muted-foreground">
        New here?{" "}
        <Link to="/register" className="font-medium text-primary hover:underline">
          Create an account
        </Link>
      </p>
    </AuthShell>
  );
}
