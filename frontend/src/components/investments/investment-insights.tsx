/** AI insights — empty until the advisor produces real investment recommendations. */
export function InvestmentInsights() {
  return (
    <div className="surface-tile px-4 py-8 text-center">
      <p className="text-sm font-medium">No investment insights yet</p>
      <p className="mt-1 text-xs text-muted-foreground">
        Insights will appear here after you add holdings or connect Angel One.
      </p>
    </div>
  );
}
