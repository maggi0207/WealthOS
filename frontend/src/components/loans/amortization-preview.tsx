import { amortizationPreview, fmtINR, type LoanAccount } from "@/lib/loans-data";

/** Six-month amortization preview — table on desktop, cards on mobile. */
export function AmortizationPreview({ loan }: { loan: LoanAccount }) {
  const rows = amortizationPreview(loan, 6);

  return (
    <div className="surface-tile overflow-hidden">
      <div className="hidden sm:block">
        <table className="w-full text-[13px]">
          <thead>
            <tr className="border-b border-border/60 text-left text-[11px] uppercase tracking-[0.1em] text-muted-foreground">
              <th className="px-4 py-2.5 font-semibold">Month</th>
              <th className="px-4 py-2.5 text-right font-semibold">EMI</th>
              <th className="px-4 py-2.5 text-right font-semibold">Principal</th>
              <th className="px-4 py-2.5 text-right font-semibold">Interest</th>
              <th className="px-4 py-2.5 text-right font-semibold">Balance</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-border/50">
            {rows.map((row) => (
              <tr key={row.period}>
                <td className="px-4 py-2.5 font-medium">{row.period}</td>
                <td className="px-4 py-2.5 text-right tabular-nums">{fmtINR(row.emi)}</td>
                <td className="px-4 py-2.5 text-right tabular-nums text-success">{fmtINR(row.principal)}</td>
                <td className="px-4 py-2.5 text-right tabular-nums text-muted-foreground">{fmtINR(row.interest)}</td>
                <td className="px-4 py-2.5 text-right font-semibold tabular-nums">{fmtINR(row.balance)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      <ul className="divide-y divide-border/50 sm:hidden">
        {rows.map((row) => (
          <li key={row.period} className="px-4 py-3">
            <div className="grid grid-cols-[minmax(0,1fr)_auto] items-baseline gap-2">
              <p className="truncate text-[13px] font-semibold">{row.period}</p>
              <p className="shrink-0 text-[13px] font-semibold tabular-nums">{fmtINR(row.emi)}</p>
            </div>
            <div className="mt-1 grid grid-cols-3 gap-2 text-[11px] text-muted-foreground">
              <span className="truncate">
                P <span className="tabular-nums text-success">{fmtINR(row.principal)}</span>
              </span>
              <span className="truncate">
                I <span className="tabular-nums">{fmtINR(row.interest)}</span>
              </span>
              <span className="truncate text-right">
                Bal <span className="tabular-nums text-foreground">{fmtINR(row.balance)}</span>
              </span>
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
}
