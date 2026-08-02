import { createFileRoute } from "@tanstack/react-router";
import { useState } from "react";

import { CategoryGrid } from "@/components/documents/category-grid";
import { DocumentBrowser } from "@/components/documents/document-browser";
import { RecentDocuments } from "@/components/documents/recent-documents";
import { RenewalReminders } from "@/components/documents/renewal-reminders";
import { UploadFab } from "@/components/documents/upload-fab";
import { VaultHero } from "@/components/documents/vault-hero";
import { DefaultErrorComponent } from "@/components/ui-kit/default-error-component";
import { SectionHeader } from "@/components/ui-kit/section-header";
import { useDocumentsOverview } from "@/hooks/api/use-documents";
import { type DocCategory } from "@/lib/documents-data";

const description =
  "A secure vault for property, loan, investment, identity, insurance and tax documents with renewal reminders and search.";

export const Route = createFileRoute("/_shell/documents")({
  head: () => ({
    meta: [
      { title: "Documents — WealthOS" },
      { name: "description", content: description },
      { property: "og:title", content: "Documents — WealthOS" },
      { property: "og:description", content: description },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  errorComponent: DefaultErrorComponent,
  component: DocumentsPage,
});

function DocumentsPage() {
  const { data } = useDocumentsOverview();
  const [category, setCategory] = useState<DocCategory | "all">("all");
  const total = data?.summary.total ?? 0;
  const renewalCount = data?.renewals.length ?? 0;

  return (
    <div className="space-y-6">
      <h1 className="sr-only">Documents</h1>

      <VaultHero />

      <section>
        <SectionHeader title="Categories" action={<span>{total} files</span>} />
        <CategoryGrid onSelect={setCategory} />
      </section>

      <section>
        <SectionHeader title="Recent" />
        <RecentDocuments />
      </section>

      <section>
        <SectionHeader
          title="Renewals"
          action={<span className="tabular-nums">{renewalCount} tracked</span>}
        />
        <RenewalReminders />
      </section>

      <section>
        <SectionHeader title="All documents" />
        <DocumentBrowser category={category} onCategoryChange={setCategory} />
      </section>

      <UploadFab />
    </div>
  );
}
