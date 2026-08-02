import { createFileRoute, Link } from "@tanstack/react-router";
import { Pencil } from "lucide-react";
import { useEffect, useState } from "react";

import { AddLoanFab } from "@/components/loans/add-loan-fab";
import { AmortizationPreview } from "@/components/loans/amortization-preview";
import { DebtHero } from "@/components/loans/debt-hero";
import { LoanAccountCards } from "@/components/loans/loan-account-cards";
import { LoanInsights } from "@/components/loans/loan-insights";
import { LoanReminders } from "@/components/loans/loan-reminders";
import { PaymentHistory } from "@/components/loans/payment-history";
import { PrepaymentSimulator } from "@/components/loans/prepayment-simulator";
import { DefaultErrorComponent } from "@/components/ui-kit/default-error-component";
import { SectionHeader } from "@/components/ui-kit/section-header";
import { TileSkeleton } from "@/components/ui-kit/skeletons";
import { useLoans } from "@/hooks/api/use-loans";
import { fmtINR, type LoanAccount } from "@/lib/loans-data";

const description =
  "Track your home, jewel and personal loans — balances, EMIs, amortisation, prepayment impact and reminders in INR.";

export const Route = createFileRoute("/_shell/loans")({
  head: () => ({
    meta: [
      { title: "Loans — WealthOS" },
      { name: "description", content: description },
      { property: "og:title", content: "Loans — WealthOS" },
      { property: "og:description", content: description },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  errorComponent: DefaultErrorComponent,
  component: LoansPage,
});

function LoansPage() {
  const { data, isPending } = useLoans();
  const accounts = data?.accounts ?? [];
  const [selectedId, setSelectedId] = useState("");
  const [editLoan, setEditLoan] = useState<LoanAccount | null>(null);

  useEffect(() => {
    if (!selectedId && accounts[0]) {
      setSelectedId(accounts[0].id);
    } else if (
      selectedId &&
      accounts.length > 0 &&
      !accounts.some((a) => a.id === selectedId)
    ) {
      setSelectedId(accounts[0]!.id);
    }
  }, [accounts, selectedId]);

  const selected =
    accounts.find((l) => l.id === selectedId) ?? accounts[0] ?? null;
  const monthlyEmi = data?.totals.monthlyEmi ?? 0;

  return (
    <div className="space-y-6">
      <h1 className="sr-only">Loans</h1>

      <DebtHero />

      <section>
        <SectionHeader
          title="Your loans"
          action={
            <div className="flex items-center gap-2">
              <span className="tabular-nums">{fmtINR(monthlyEmi)}/mo</span>
              {selected ? (
                <button
                  type="button"
                  onClick={() => setEditLoan(selected)}
                  className="press inline-flex min-h-11 items-center gap-1 rounded-full px-2 text-xs font-semibold text-primary"
                >
                  <Pencil className="size-3.5" />
                  Edit
                </button>
              ) : null}
            </div>
          }
        />
        <LoanAccountCards
          selectedId={selected?.id ?? ""}
          onSelect={setSelectedId}
        />
      </section>

      {isPending || !selected ? (
        <TileSkeleton className="h-40" />
      ) : (
        <>
          <section>
            <SectionHeader
              title="Amortisation preview"
              action={<span>{selected.lender}</span>}
            />
            <AmortizationPreview loan={selected} />
          </section>

          <section>
            <SectionHeader title="Prepayment simulator" />
            <PrepaymentSimulator loan={selected} />
          </section>
        </>
      )}

      <section>
        <SectionHeader title="Upcoming EMIs" />
        <LoanReminders />
      </section>

      <section>
        <SectionHeader
          title="Payment history"
          action={
            selected ? <span>{selected.accountMask}</span> : undefined
          }
        />
        {selected ? (
          <PaymentHistory loanId={selected.id} />
        ) : (
          <TileSkeleton className="h-32" />
        )}
      </section>

      <section>
        <SectionHeader
          title="AI recommendations"
          action={
            <Link to="/ai-advisor" className="press -my-2 inline-flex min-h-11 items-center">
              Ask AI
            </Link>
          }
        />
        <LoanInsights />
      </section>

      <AddLoanFab editLoan={editLoan} onEditConsumed={() => setEditLoan(null)} />
    </div>
  );
}
