import { useEffect, useState } from "react";
import { toast } from "sonner";

import { Field } from "@/components/ui-kit/field";
import { SelectField, TextAreaField } from "@/components/ui-kit/form-fields";
import { FormSheet } from "@/components/ui-kit/form-sheet";
import { useRecordSalary } from "@/hooks/api/use-income";
import {
  currentYearMonth,
  parseRequiredNumber,
  requiredText,
  todayIsoDate,
  toastMutationError,
} from "@/lib/form-utils";
import type { SalaryMember } from "@/lib/business-data";
import type { RecordSalaryRequestDto, SalaryStatusDto } from "@/services/income/requests";

const STATUS_OPTIONS = [
  { value: "0", label: "Active" },
  { value: "1", label: "Upcoming" },
  { value: "2", label: "Inactive" },
];

type FormState = {
  memberName: string;
  employer: string;
  role: string;
  monthlyAmount: string;
  paidOn: string;
  period: string;
  status: string;
  nextExpectedOn: string;
  notes: string;
};

const emptyForm = (): FormState => ({
  memberName: "",
  employer: "",
  role: "",
  monthlyAmount: "",
  paidOn: todayIsoDate(),
  period: currentYearMonth(),
  status: "0",
  nextExpectedOn: "",
  notes: "",
});

function fromMember(member: SalaryMember): FormState {
  return {
    memberName: member.memberName,
    employer: member.employer,
    role: member.role,
    monthlyAmount: String(member.monthlySalary),
    paidOn: member.lastCreditedOn || todayIsoDate(),
    period: currentYearMonth(),
    status: member.status === "upcoming" ? "1" : "0",
    nextExpectedOn: member.nextExpectedOn || "",
    notes: "",
  };
}

function buildPayload(
  form: FormState,
  salaryId?: string,
): RecordSalaryRequestDto | string {
  const memberErr = requiredText(form.memberName, "Member name");
  if (memberErr) return memberErr;
  const employerErr = requiredText(form.employer, "Employer");
  if (employerErr) return employerErr;
  const roleErr = requiredText(form.role, "Role");
  if (roleErr) return roleErr;
  const amount = parseRequiredNumber(form.monthlyAmount, "Monthly amount");
  if (typeof amount === "string") return amount;
  if (amount <= 0) return "Monthly amount must be greater than zero";
  if (!form.period.match(/^\d{4}-\d{2}$/)) return "Period must be yyyy-MM";
  if (!form.paidOn) return "Paid on date is required";

  return {
    salaryId: salaryId ?? null,
    memberName: form.memberName.trim(),
    employer: form.employer.trim(),
    role: form.role.trim(),
    monthlyAmount: amount,
    currencyCode: "INR",
    paidOn: form.paidOn,
    period: form.period,
    status: Number(form.status) as SalaryStatusDto,
    nextExpectedOn: form.nextExpectedOn.trim() || null,
    notes: form.notes.trim() || null,
  };
}

export function SalaryFormSheet({
  open,
  onOpenChange,
  mode,
  member,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  mode: "create" | "edit";
  member?: SalaryMember | null;
}) {
  const recordMutation = useRecordSalary();
  const [form, setForm] = useState<FormState>(emptyForm);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!open) return;
    setError(null);
    setForm(mode === "edit" && member ? fromMember(member) : emptyForm());
  }, [open, mode, member]);

  const pending = recordMutation.isPending;
  const set =
    (key: keyof FormState) =>
    (value: string) =>
      setForm((prev) => ({ ...prev, [key]: value }));

  async function onSubmit() {
    const payload = buildPayload(form, mode === "edit" ? member?.id : undefined);
    if (typeof payload === "string") {
      setError(payload);
      toast.error(payload);
      return;
    }
    setError(null);
    try {
      await recordMutation.mutateAsync(payload);
      toast.success(mode === "create" ? "Salary recorded" : "Salary updated");
      onOpenChange(false);
    } catch (err) {
      toastMutationError(err, "Could not save salary");
    }
  }

  return (
    <FormSheet
      open={open}
      onOpenChange={onOpenChange}
      title={mode === "create" ? "Add salary" : "Edit salary"}
      description="Record monthly salary credits for household members."
      pending={pending}
      onSubmit={() => void onSubmit()}
      onReset={() => setForm(mode === "edit" && member ? fromMember(member) : emptyForm())}
      submitLabel={mode === "create" ? "Add salary" : "Save changes"}
    >
      {error ? (
        <p role="alert" className="rounded-xl bg-destructive/10 px-3 py-2 text-sm text-destructive">
          {error}
        </p>
      ) : null}

      <Field
        id="salary-member"
        label="Member name"
        value={form.memberName}
        onChange={(e) => set("memberName")(e.target.value)}
      />
      <Field
        id="salary-employer"
        label="Employer"
        value={form.employer}
        onChange={(e) => set("employer")(e.target.value)}
      />
      <Field
        id="salary-role"
        label="Role"
        value={form.role}
        onChange={(e) => set("role")(e.target.value)}
      />
      <Field
        id="salary-amount"
        label="Monthly amount (₹)"
        inputMode="decimal"
        value={form.monthlyAmount}
        onChange={(e) => set("monthlyAmount")(e.target.value)}
      />
      <div className="grid grid-cols-2 gap-2.5">
        <Field
          id="salary-period"
          label="Period (yyyy-MM)"
          value={form.period}
          onChange={(e) => set("period")(e.target.value)}
        />
        <SelectField
          id="salary-status"
          label="Status"
          value={form.status}
          onChange={(e) => set("status")(e.target.value)}
          options={STATUS_OPTIONS}
        />
      </div>
      <div className="grid grid-cols-2 gap-2.5">
        <Field
          id="salary-paid"
          label="Paid on"
          type="date"
          value={form.paidOn}
          onChange={(e) => set("paidOn")(e.target.value)}
        />
        <Field
          id="salary-next"
          label="Next expected"
          type="date"
          value={form.nextExpectedOn}
          onChange={(e) => set("nextExpectedOn")(e.target.value)}
        />
      </div>
      <TextAreaField
        id="salary-notes"
        label="Notes"
        value={form.notes}
        onChange={(e) => set("notes")(e.target.value)}
      />
    </FormSheet>
  );
}
