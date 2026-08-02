import { Link, createFileRoute, useNavigate } from "@tanstack/react-router";
import { Loader2, UserPlus } from "lucide-react";
import { useState, type FormEvent } from "react";
import { toast } from "sonner";

import { Button } from "@/components/ui/button";
import { Field } from "@/components/ui-kit/field";
import { isMockApiMode } from "@/config/env";
import { useAuth } from "@/lib/mock-auth";
import { AuthShell } from "@/components/auth/auth-shell";

export const Route = createFileRoute("/register")({
  head: () => ({
    meta: [
      { title: "Create account — WealthOS" },
      { name: "description", content: "Create your WealthOS workspace and start tracking net worth." },
      { property: "og:title", content: "Create account — WealthOS" },
      { property: "og:description", content: "Create your WealthOS workspace and start tracking net worth." },
    ],
  }),
  component: RegisterPage,
});

const STRONG_PASSWORD =
  /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$/;

function RegisterPage() {
  const { register } = useAuth();
  const navigate = useNavigate();
  const mockMode = isMockApiMode();
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [touched, setTouched] = useState({ name: false, email: false, password: false });
  const [pending, setPending] = useState(false);

  const passwordError = mockMode
    ? password.length >= 6
      ? null
      : "Use at least 6 characters"
    : STRONG_PASSWORD.test(password)
      ? null
      : "Use 8+ chars with upper, lower, number, and symbol";

  const errors = {
    name: name.trim().length >= 2 ? null : "Enter your full name",
    email: /^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(email) ? null : "Enter a valid email address",
    password: passwordError,
  };
  const invalid = Boolean(errors.name || errors.email || errors.password);

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    setTouched({ name: true, email: true, password: true });
    if (invalid || pending) return;
    setPending(true);
    try {
      await register(name, email, password);
      toast.success(mockMode ? "Workspace created (mock account)" : "Account created");
      navigate({ to: "/dashboard" });
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Registration failed");
    } finally {
      setPending(false);
    }
  }

  return (
    <AuthShell
      title="Create your workspace"
      subtitle={
        mockMode
          ? "Registration is mocked for this foundation build — no data leaves your browser."
          : "Create a WealthOS account to sync your wealth data securely."
      }
    >
      <form onSubmit={onSubmit} noValidate className="space-y-4">
        <Field
          id="name"
          label="Full name"
          autoComplete="name"
          value={name}
          onChange={(e) => setName(e.target.value)}
          onBlur={() => setTouched((t) => ({ ...t, name: true }))}
          error={touched.name ? errors.name : null}
        />
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
          autoComplete="new-password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          onBlur={() => setTouched((t) => ({ ...t, password: true }))}
          error={touched.password ? errors.password : null}
          hint={
            touched.password
              ? undefined
              : mockMode
                ? "Minimum 6 characters"
                : "8+ chars with upper, lower, number, and symbol"
          }
        />
        <Button type="submit" className="min-h-11 w-full rounded-full" disabled={pending}>
          {pending ? <Loader2 className="size-4 animate-spin" /> : <UserPlus className="size-4" />}
          {pending ? "Creating…" : "Create account"}
        </Button>
      </form>
      <p className="mt-5 text-center text-sm text-muted-foreground">
        Already have an account?{" "}
        <Link to="/login" className="font-medium text-primary hover:underline">
          Sign in
        </Link>
      </p>
    </AuthShell>
  );
}
