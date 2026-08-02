import { useEffect, useMemo, useState } from "react";
import { toast } from "sonner";

import { Field } from "@/components/ui-kit/field";
import { SelectField, TextAreaField } from "@/components/ui-kit/form-fields";
import { FormSheet } from "@/components/ui-kit/form-sheet";
import { useGoalsOverview, useRecordGoalContribution } from "@/hooks/api/use-goals";
import {
  parseRequiredNumber,
  toastMutationError,
} from "@/lib/form-utils";
import type { Goal } from "@/lib/goals-data";
import type { RecordGoalContributionRequestDto } from "@/services/goals/types";

type FormState = {
  goalId: string;
  amount: string;
  contributedOn: string;
  source: string;
  notes: string;
};

const emptyForm = (goalId = ""): FormState => ({
  goalId,
  amount: "",
  contributedOn: new Date().toISOString().slice(0, 10),
  source: "",
  notes: "",
});

export function ContributionFormSheet({
  open,
  onOpenChange,
  goal,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  goal?: Goal | null;
}) {
  const { data: overview } = useGoalsOverview();
  const mutation = useRecordGoalContribution();
  const [form, setForm] = useState<FormState>(emptyForm(goal?.id));
  const [error, setError] = useState<string | null>(null);

  const goalOptions = useMemo(
    () =>
      (overview?.goals ?? []).map((g) => ({
        value: g.id,
        label: g.name,
      })),
    [overview?.goals],
  );

  useEffect(() => {
    if (!open) return;
    setError(null);
    setForm(emptyForm(goal?.id ?? overview?.goals[0]?.id ?? ""));
  }, [open, goal, overview?.goals]);

  const pending = mutation.isPending;

  const set =
    (key: keyof FormState) =>
    (value: string) =>
      setForm((prev) => ({ ...prev, [key]: value }));

  async function onSubmit() {
    if (!form.goalId) {
      const msg = "Select a goal";
      setError(msg);
      toast.error(msg);
      return;
    }
    const amount = parseRequiredNumber(form.amount, "Amount");
    if (typeof amount === "string") {
      setError(amount);
      toast.error(amount);
      return;
    }
    if (!form.contributedOn) {
      const msg = "Contribution date is required";
      setError(msg);
      toast.error(msg);
      return;
    }
    setError(null);

    const body: RecordGoalContributionRequestDto = {
      amount,
      contributedOn: form.contributedOn,
      notes: form.notes.trim() || null,
      source: form.source.trim() || null,
    };

    try {
      await mutation.mutateAsync({ goalId: form.goalId, body });
      toast.success("Contribution recorded");
      onOpenChange(false);
    } catch (err) {
      toastMutationError(err, "Could not record contribution");
    }
  }

  return (
    <FormSheet
      open={open}
      onOpenChange={onOpenChange}
      title="Add funds"
      description="Log a one-time contribution toward a goal."
      pending={pending}
      onSubmit={() => void onSubmit()}
      onReset={() => setForm(emptyForm(goal?.id ?? form.goalId))}
      submitLabel="Record contribution"
    >
      {error ? (
        <p role="alert" className="rounded-xl bg-destructive/10 px-3 py-2 text-sm text-destructive">
          {error}
        </p>
      ) : null}

      {!goal ? (
        <SelectField
          id="contrib-goal"
          label="Goal"
          value={form.goalId}
          onChange={(e) => set("goalId")(e.target.value)}
          options={
            goalOptions.length > 0
              ? goalOptions
              : [{ value: "", label: "No goals — create one first" }]
          }
        />
      ) : null}

      <Field
        id="contrib-amount"
        label="Amount (₹)"
        inputMode="decimal"
        value={form.amount}
        onChange={(e) => set("amount")(e.target.value)}
      />
      <Field
        id="contrib-date"
        label="Date"
        type="date"
        value={form.contributedOn}
        onChange={(e) => set("contributedOn")(e.target.value)}
      />
      <Field
        id="contrib-source"
        label="Source (optional)"
        value={form.source}
        onChange={(e) => set("source")(e.target.value)}
        hint="e.g. Salary, bonus, SIP"
      />
      <TextAreaField
        id="contrib-notes"
        label="Notes"
        value={form.notes}
        onChange={(e) => set("notes")(e.target.value)}
      />
    </FormSheet>
  );
}
