import { createFileRoute, Link } from "@tanstack/react-router";
import { useState } from "react";

import { AmortizationPreview } from "@/components/loans/amortization-preview";
import { DebtHero } from "@/components/loans/debt-hero";
import { LoanAccountCards } from "@/components/loans/loan-account-cards";
import { LoanInsights } from "@/components/loans/loan-insights";
import { LoanReminders } from "@/components/loans/loan-reminders";
import { PaymentHistory } from "@/components/loans/payment-history";
import { PrepaymentSimulator } from "@/components/loans/prepayment-simulator";
import { DefaultErrorComponent } from "@/components/ui-kit/default-error-component";
import { SectionHeader } from "@/components/ui-kit/section-header";
import { fmtINR, loanAccounts, loansTotals } from "@/lib/loans-data";

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
  const [selectedId, setSelectedId] = useState(loanAccounts[0]!.id);
  const selected = loanAccounts.find((l) => l.id === selectedId) ?? loanAccounts[0]!;

  return (
    <div className="space-y-6">
      <h1 className="sr-only">Loans</h1>

      <DebtHero />

      <section>
        <SectionHeader
          title="Your loans"
          action={<span className="tabular-nums">{fmtINR(loansTotals.monthlyEmi)}/mo</span>}
        />
        <LoanAccountCards selectedId={selectedId} onSelect={setSelectedId} />
      </section>

      <section>
        <SectionHeader title="Amortisation preview" action={<span>{selected.lender}</span>} />
        <AmortizationPreview loan={selected} />
      </section>

      <section>
        <SectionHeader title="Prepayment simulator" />
        <PrepaymentSimulator loan={selected} />
      </section>

      <section>
        <SectionHeader title="Upcoming EMIs" />
        <LoanReminders />
      </section>

      <section>
        <SectionHeader title="Payment history" action={<span>{selected.accountMask}</span>} />
        <PaymentHistory loanId={selected.id} />
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
    </div>
  );
}
