/** Upcoming SIPs / maturities — empty until real schedule data exists. */
export function UpcomingReminders() {
  return (
    <div className="surface-tile px-4 py-8 text-center">
      <p className="text-sm font-medium">No upcoming items</p>
      <p className="mt-1 text-xs text-muted-foreground">
        SIPs, bond maturities and dividends will show here from your real accounts.
      </p>
    </div>
  );
}
