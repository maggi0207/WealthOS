import { useEffect, useState } from "react";
import { Loader2, Trash2 } from "lucide-react";
import { toast } from "sonner";

import { ConfirmDialog } from "@/components/ui-kit/confirm-dialog";
import { Field } from "@/components/ui-kit/field";
import { SelectField, TextAreaField } from "@/components/ui-kit/form-fields";
import { FormSheet } from "@/components/ui-kit/form-sheet";
import { Button } from "@/components/ui/button";
import {
  useCreateGoal,
  useDeleteGoal,
  useUpdateGoal,
} from "@/hooks/api/use-goals";
import {
  parseRequiredNumber,
  requiredText,
  toastMutationError,
} from "@/lib/form-utils";
import type { Goal, GoalCategory } from "@/lib/goals-data";
import type {
  CreateGoalRequestDto,
  GoalCategoryDto,
  GoalPriorityDto,
  GoalStatusDto,
  UpdateGoalRequestDto,
} from "@/services/goals/types";

const CATEGORY_OPTIONS = [
  { value: "0", label: "Buy house" },
  { value: "1", label: "Emergency fund" },
  { value: "2", label: "Become loan free" },
  { value: "3", label: "Child education" },
  { value: "4", label: "Retirement" },
  { value: "5", label: "Vacation" },
  { value: "6", label: "Vehicle purchase" },
  { value: "7", label: "Custom" },
];

const PRIORITY_OPTIONS = [
  { value: "0", label: "Low" },
  { value: "1", label: "Medium" },
  { value: "2", label: "High" },
  { value: "3", label: "Critical" },
];

const STATUS_OPTIONS = [
  { value: "0", label: "Active" },
  { value: "1", label: "Completed" },
  { value: "2", label: "Paused" },
  { value: "3", label: "Cancelled" },
];

function uiCategoryToDto(category: GoalCategory): GoalCategoryDto {
  if (category === "property") return 0;
  if (category === "safety") return 1;
  if (category === "debt") return 2;
  if (category === "education") return 3;
  if (category === "retirement") return 4;
  return 7;
}

type FormState = {
  name: string;
  category: string;
  targetAmount: string;
  currentAmount: string;
  targetDate: string;
  startedOn: string;
  monthlyContribution: string;
  priority: string;
  status: string;
  description: string;
};

const emptyForm = (): FormState => ({
  name: "",
  category: "7",
  targetAmount: "",
  currentAmount: "0",
  targetDate: "",
  startedOn: new Date().toISOString().slice(0, 10),
  monthlyContribution: "",
  priority: "1",
  status: "0",
  description: "",
});

function fromGoal(goal: Goal): FormState {
  return {
    name: goal.name,
    category: String(uiCategoryToDto(goal.category)),
    targetAmount: String(goal.target),
    currentAmount: String(goal.saved),
    targetDate: goal.targetDate,
    startedOn: goal.startedOn,
    monthlyContribution: String(goal.monthlyContribution),
    priority: "1",
    status: goal.saved >= goal.target ? "1" : "0",
    description: goal.note,
  };
}

export function GoalFormSheet({
  open,
  onOpenChange,
  mode,
  goal,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  mode: "create" | "edit";
  goal?: Goal | null;
}) {
  const createMutation = useCreateGoal();
  const updateMutation = useUpdateGoal();
  const deleteMutation = useDeleteGoal();
  const [form, setForm] = useState<FormState>(emptyForm);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!open) return;
    setError(null);
    setForm(mode === "edit" && goal ? fromGoal(goal) : emptyForm());
  }, [open, mode, goal]);

  const pending =
    createMutation.isPending || updateMutation.isPending || deleteMutation.isPending;

  const set =
    (key: keyof FormState) =>
    (value: string) =>
      setForm((prev) => ({ ...prev, [key]: value }));

  function buildPayload(): CreateGoalRequestDto | UpdateGoalRequestDto | string {
    const nameErr = requiredText(form.name, "Goal name");
    if (nameErr) return nameErr;
    if (!form.targetDate) return "Target date is required";
    if (!form.startedOn) return "Start date is required";

    const target = parseRequiredNumber(form.targetAmount, "Target amount");
    if (typeof target === "string") return target;
    const currentRaw = form.currentAmount.trim()
      ? parseRequiredNumber(form.currentAmount, "Current amount")
      : 0;
    if (typeof currentRaw === "string") return currentRaw;
    const monthlyRaw = form.monthlyContribution.trim()
      ? parseRequiredNumber(form.monthlyContribution, "Monthly contribution")
      : 0;
    if (typeof monthlyRaw === "string") return monthlyRaw;

    return {
      name: form.name.trim(),
      category: Number(form.category) as GoalCategoryDto,
      targetAmount: target,
      currentAmount: currentRaw,
      targetDate: form.targetDate,
      startedOn: form.startedOn,
      monthlyContribution: monthlyRaw,
      priority: Number(form.priority) as GoalPriorityDto,
      status: Number(form.status) as GoalStatusDto,
      description: form.description.trim() || null,
      currencyCode: "INR",
    };
  }

  async function onSubmit() {
    const payload = buildPayload();
    if (typeof payload === "string") {
      setError(payload);
      toast.error(payload);
      return;
    }
    setError(null);
    try {
      if (mode === "create") {
        await createMutation.mutateAsync(payload);
        toast.success("Goal created");
      } else if (goal) {
        await updateMutation.mutateAsync({ id: goal.id, body: payload });
        toast.success("Goal updated");
      }
      onOpenChange(false);
    } catch (err) {
      toastMutationError(err, "Could not save goal");
    }
  }

  async function onDelete() {
    if (!goal) return;
    try {
      await deleteMutation.mutateAsync(goal.id);
      toast.success("Goal deleted");
      onOpenChange(false);
    } catch (err) {
      toastMutationError(err, "Could not delete goal");
    }
  }

  return (
    <FormSheet
      open={open}
      onOpenChange={onOpenChange}
      title={mode === "create" ? "New goal" : "Edit goal"}
      description="Set a target, timeline and monthly contribution."
      pending={pending}
      onSubmit={() => void onSubmit()}
      onReset={() => setForm(mode === "edit" && goal ? fromGoal(goal) : emptyForm())}
      submitLabel={mode === "create" ? "Create goal" : "Save changes"}
      deleteSlot={
        mode === "edit" && goal ? (
          <ConfirmDialog
            title="Delete goal?"
            description="This removes the goal and its contribution history."
            confirmLabel="Delete"
            destructive
            onConfirm={() => void onDelete()}
            trigger={
              <Button
                type="button"
                variant="ghost"
                className="min-h-11 w-full rounded-full text-destructive hover:bg-destructive/10 hover:text-destructive"
                disabled={pending}
              >
                {deleteMutation.isPending ? (
                  <Loader2 className="size-4 animate-spin" />
                ) : (
                  <Trash2 className="size-4" />
                )}
                Delete goal
              </Button>
            }
          />
        ) : null
      }
    >
      {error ? (
        <p role="alert" className="rounded-xl bg-destructive/10 px-3 py-2 text-sm text-destructive">
          {error}
        </p>
      ) : null}

      <Field id="goal-name" label="Goal name" value={form.name} onChange={(e) => set("name")(e.target.value)} />
      <SelectField
        id="goal-category"
        label="Category"
        value={form.category}
        onChange={(e) => set("category")(e.target.value)}
        options={CATEGORY_OPTIONS}
      />
      <div className="grid grid-cols-2 gap-2.5">
        <Field
          id="goal-target"
          label="Target (₹)"
          inputMode="decimal"
          value={form.targetAmount}
          onChange={(e) => set("targetAmount")(e.target.value)}
        />
        <Field
          id="goal-saved"
          label="Saved so far (₹)"
          inputMode="decimal"
          value={form.currentAmount}
          onChange={(e) => set("currentAmount")(e.target.value)}
        />
      </div>
      <div className="grid grid-cols-2 gap-2.5">
        <Field
          id="goal-started"
          label="Started on"
          type="date"
          value={form.startedOn}
          onChange={(e) => set("startedOn")(e.target.value)}
        />
        <Field
          id="goal-target-date"
          label="Target date"
          type="date"
          value={form.targetDate}
          onChange={(e) => set("targetDate")(e.target.value)}
        />
      </div>
      <Field
        id="goal-monthly"
        label="Monthly contribution (₹)"
        inputMode="decimal"
        value={form.monthlyContribution}
        onChange={(e) => set("monthlyContribution")(e.target.value)}
      />
      <div className="grid grid-cols-2 gap-2.5">
        <SelectField
          id="goal-priority"
          label="Priority"
          value={form.priority}
          onChange={(e) => set("priority")(e.target.value)}
          options={PRIORITY_OPTIONS}
        />
        <SelectField
          id="goal-status"
          label="Status"
          value={form.status}
          onChange={(e) => set("status")(e.target.value)}
          options={STATUS_OPTIONS}
        />
      </div>
      <TextAreaField
        id="goal-desc"
        label="Notes"
        value={form.description}
        onChange={(e) => set("description")(e.target.value)}
      />
    </FormSheet>
  );
}
