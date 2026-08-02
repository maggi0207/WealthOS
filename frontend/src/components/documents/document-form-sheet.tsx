import { useEffect, useState } from "react";
import { Loader2, Trash2 } from "lucide-react";
import { toast } from "sonner";

import { ConfirmDialog } from "@/components/ui-kit/confirm-dialog";
import { Field } from "@/components/ui-kit/field";
import { SelectField, TextAreaField } from "@/components/ui-kit/form-fields";
import { FormSheet } from "@/components/ui-kit/form-sheet";
import { Button } from "@/components/ui/button";
import {
  useCreateDocument,
  useDeleteDocument,
  useUpdateDocument,
} from "@/hooks/api/use-documents";
import {
  requiredText,
  toastMutationError,
} from "@/lib/form-utils";
import type { DocCategory, VaultDocument } from "@/lib/documents-data";
import type {
  CreateDocumentRequestDto,
  DocumentCategoryDto,
  DocumentStatusDto,
  UpdateDocumentRequestDto,
} from "@/services/documents/types";

const CATEGORY_OPTIONS = [
  { value: "0", label: "Property" },
  { value: "1", label: "Loan" },
  { value: "2", label: "Investment" },
  { value: "5", label: "Insurance" },
  { value: "6", label: "Identity" },
  { value: "7", label: "Tax" },
  { value: "9", label: "Other" },
];

function uiCategoryToDto(category: DocCategory): DocumentCategoryDto {
  if (category === "property") return 0;
  if (category === "loans") return 1;
  if (category === "investments") return 2;
  if (category === "insurance") return 5;
  if (category === "identity") return 6;
  if (category === "tax") return 7;
  return 9;
}

type FormState = {
  title: string;
  category: string;
  owner: string;
  tags: string;
  notes: string;
};

const emptyForm = (): FormState => ({
  title: "",
  category: "9",
  owner: "Self",
  tags: "",
  notes: "",
});

function fromDocument(doc: VaultDocument): FormState {
  return {
    title: doc.name,
    category: String(uiCategoryToDto(doc.category)),
    owner: doc.linkedTo || "Self",
    tags: doc.tags.join(", "),
    notes: "",
  };
}

function parseTags(raw: string): string[] {
  return raw
    .split(",")
    .map((t) => t.trim())
    .filter(Boolean);
}

export function DocumentFormSheet({
  open,
  onOpenChange,
  mode,
  document,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  mode: "create" | "edit";
  document?: VaultDocument | null;
}) {
  const createMutation = useCreateDocument();
  const updateMutation = useUpdateDocument();
  const deleteMutation = useDeleteDocument();
  const [form, setForm] = useState<FormState>(emptyForm);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!open) return;
    setError(null);
    setForm(mode === "edit" && document ? fromDocument(document) : emptyForm());
  }, [open, mode, document]);

  const pending =
    createMutation.isPending || updateMutation.isPending || deleteMutation.isPending;

  const set =
    (key: keyof FormState) =>
    (value: string) =>
      setForm((prev) => ({ ...prev, [key]: value }));

  async function onSubmit() {
    const titleErr = requiredText(form.title, "Title");
    if (titleErr) {
      setError(titleErr);
      toast.error(titleErr);
      return;
    }
    const ownerErr = requiredText(form.owner, "Owner");
    if (ownerErr) {
      setError(ownerErr);
      toast.error(ownerErr);
      return;
    }
    setError(null);

    try {
      if (mode === "create") {
        const body: CreateDocumentRequestDto = {
          title: form.title.trim(),
          category: Number(form.category) as DocumentCategoryDto,
          owner: form.owner.trim(),
          status: 1 as DocumentStatusDto,
          notes: form.notes.trim() || null,
          tags: parseTags(form.tags),
          referenceModule: 0,
          fileSizeBytes: 0,
          storageProvider: 0,
        };
        await createMutation.mutateAsync(body);
        toast.success("Document added");
      } else if (document) {
        const body: UpdateDocumentRequestDto = {
          title: form.title.trim(),
          category: Number(form.category) as DocumentCategoryDto,
          owner: form.owner.trim(),
          status: 1 as DocumentStatusDto,
          notes: form.notes.trim() || null,
          referenceModule: 0,
        };
        await updateMutation.mutateAsync({ id: document.id, body });
        toast.success("Document updated");
      }
      onOpenChange(false);
    } catch (err) {
      toastMutationError(err, "Could not save document");
    }
  }

  async function onDelete() {
    if (!document) return;
    try {
      await deleteMutation.mutateAsync(document.id);
      toast.success("Document deleted");
      onOpenChange(false);
    } catch (err) {
      toastMutationError(err, "Could not delete document");
    }
  }

  return (
    <FormSheet
      open={open}
      onOpenChange={onOpenChange}
      title={mode === "create" ? "Add document" : "Edit document"}
      description="Vault metadata — file upload is a placeholder for now."
      pending={pending}
      onSubmit={() => void onSubmit()}
      onReset={() =>
        setForm(mode === "edit" && document ? fromDocument(document) : emptyForm())
      }
      submitLabel={mode === "create" ? "Save document" : "Save changes"}
      deleteSlot={
        mode === "edit" && document ? (
          <ConfirmDialog
            title="Delete document?"
            description="This removes the document from your vault."
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
                Delete document
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

      <Field id="doc-title" label="Title" value={form.title} onChange={(e) => set("title")(e.target.value)} />
      <SelectField
        id="doc-category"
        label="Category"
        value={form.category}
        onChange={(e) => set("category")(e.target.value)}
        options={CATEGORY_OPTIONS}
      />
      <Field id="doc-owner" label="Owner" value={form.owner} onChange={(e) => set("owner")(e.target.value)} />
      <Field
        id="doc-tags"
        label="Tags"
        value={form.tags}
        onChange={(e) => set("tags")(e.target.value)}
        hint="Comma-separated, e.g. kyc, hdfc"
      />
      <TextAreaField
        id="doc-notes"
        label="Notes"
        value={form.notes}
        onChange={(e) => set("notes")(e.target.value)}
      />
      <p className="text-[11px] text-muted-foreground">
        File upload is a placeholder — attach the actual PDF or image when secure storage is enabled.
      </p>
    </FormSheet>
  );
}
