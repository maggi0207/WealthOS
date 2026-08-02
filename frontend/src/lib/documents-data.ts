/**
 * Documents vault mock data.
 * Frontend-only fixtures shaped for a future storage backend: stable ids,
 * ISO dates, category unions, tags and expiry metadata.
 */

import { fmtDate, type ISODate } from "@/lib/business-data";

export { fmtDate };
export type { ISODate };

export type DocCategory = "property" | "loans" | "investments" | "identity" | "insurance" | "tax";

export type DocStatus = "verified" | "pending" | "expiring" | "expired";

export type VaultDocument = {
  id: string;
  name: string;
  category: DocCategory;
  status: DocStatus;
  fileType: "PDF" | "JPG" | "DOCX";
  sizeLabel: string;
  updatedOn: ISODate;
  expiresOn?: ISODate;
  linkedTo?: string;
  tags: string[];
};

export const docCategories: { id: DocCategory; label: string }[] = [
  { id: "property", label: "Property" },
  { id: "loans", label: "Loans" },
  { id: "investments", label: "Investments" },
  { id: "identity", label: "Identity" },
  { id: "insurance", label: "Insurance" },
  { id: "tax", label: "Tax" },
];

export const documents: VaultDocument[] = [
  { id: "d-1", name: "Sale deed — Ramana Flats", category: "property", status: "verified", fileType: "PDF", sizeLabel: "4.2 MB", updatedOn: "2026-05-14", linkedTo: "Ramana Flats, Door No. 3", tags: ["deed", "adyar"] },
  { id: "d-2", name: "Encumbrance certificate", category: "property", status: "expiring", fileType: "PDF", sizeLabel: "1.1 MB", updatedOn: "2025-09-02", expiresOn: "2026-09-01", linkedTo: "Ramana Flats, Door No. 3", tags: ["ec", "registrar"] },
  { id: "d-3", name: "Patta / Chitta extract", category: "property", status: "verified", fileType: "PDF", sizeLabel: "820 KB", updatedOn: "2026-01-22", linkedTo: "Ramana Flats, Door No. 3", tags: ["patta"] },
  { id: "d-4", name: "Property tax receipt 2026", category: "tax", status: "verified", fileType: "PDF", sizeLabel: "310 KB", updatedOn: "2026-04-08", tags: ["receipt", "gcc"] },
  { id: "d-5", name: "Home loan sanction letter", category: "loans", status: "verified", fileType: "PDF", sizeLabel: "1.6 MB", updatedOn: "2018-06-08", linkedTo: "HDFC •••• 4821", tags: ["hdfc", "sanction"] },
  { id: "d-6", name: "Loan amortisation statement", category: "loans", status: "pending", fileType: "PDF", sizeLabel: "640 KB", updatedOn: "2026-07-06", linkedTo: "HDFC •••• 4821", tags: ["statement"] },
  { id: "d-7", name: "Jewel loan pledge receipt", category: "loans", status: "verified", fileType: "JPG", sizeLabel: "2.0 MB", updatedOn: "2024-11-18", linkedTo: "IOB •••• 7710", tags: ["gold", "pledge"] },
  { id: "d-8", name: "Angel One holding statement", category: "investments", status: "verified", fileType: "PDF", sizeLabel: "980 KB", updatedOn: "2026-07-01", linkedTo: "Angel One (Magesh)", tags: ["broker", "cas"] },
  { id: "d-9", name: "IndiaBonds allotment advice", category: "investments", status: "verified", fileType: "PDF", sizeLabel: "410 KB", updatedOn: "2026-03-19", tags: ["bond"] },
  { id: "d-10", name: "PAN card", category: "identity", status: "verified", fileType: "JPG", sizeLabel: "180 KB", updatedOn: "2021-02-11", tags: ["kyc"] },
  { id: "d-11", name: "Aadhaar", category: "identity", status: "verified", fileType: "PDF", sizeLabel: "260 KB", updatedOn: "2023-08-04", tags: ["kyc"] },
  { id: "d-12", name: "Passport", category: "identity", status: "expiring", fileType: "PDF", sizeLabel: "1.2 MB", updatedOn: "2016-10-30", expiresOn: "2026-10-29", tags: ["kyc", "travel"] },
  { id: "d-13", name: "Term life policy — ₹2 Cr", category: "insurance", status: "verified", fileType: "PDF", sizeLabel: "2.4 MB", updatedOn: "2024-12-01", expiresOn: "2027-01-15", tags: ["term", "hdfc life"] },
  { id: "d-14", name: "Family health cover", category: "insurance", status: "expiring", fileType: "PDF", sizeLabel: "1.8 MB", updatedOn: "2025-08-20", expiresOn: "2026-08-19", tags: ["health", "star"] },
  { id: "d-15", name: "Car insurance", category: "insurance", status: "expired", fileType: "PDF", sizeLabel: "760 KB", updatedOn: "2025-06-10", expiresOn: "2026-06-09", tags: ["motor"] },
  { id: "d-16", name: "ITR acknowledgement AY 2025-26", category: "tax", status: "verified", fileType: "PDF", sizeLabel: "520 KB", updatedOn: "2025-07-26", tags: ["itr"] },
  { id: "d-17", name: "Form 16 — FY 2025-26", category: "tax", status: "pending", fileType: "PDF", sizeLabel: "340 KB", updatedOn: "2026-06-12", tags: ["salary", "tds"] },
  { id: "d-18", name: "GST returns — Q1 FY27", category: "tax", status: "verified", fileType: "PDF", sizeLabel: "290 KB", updatedOn: "2026-07-20", linkedTo: "Business", tags: ["gst", "business"] },
];

export const docStatusLabel: Record<DocStatus, string> = {
  verified: "Verified",
  pending: "Pending",
  expiring: "Renew soon",
  expired: "Expired",
};

export function categoryCount(category: DocCategory) {
  return documents.filter((d) => d.category === category).length;
}

export const recentDocuments = [...documents]
  .sort((a, b) => (a.updatedOn < b.updatedOn ? 1 : -1))
  .slice(0, 5);

export type RenewalItem = {
  id: string;
  name: string;
  expiresOn: ISODate;
  category: DocCategory;
  status: DocStatus;
};

export const renewals: RenewalItem[] = documents
  .filter((d) => d.expiresOn)
  .map((d) => ({ id: d.id, name: d.name, expiresOn: d.expiresOn!, category: d.category, status: d.status }))
  .sort((a, b) => (a.expiresOn > b.expiresOn ? 1 : -1));

export const vaultSummary = {
  total: documents.length,
  verified: documents.filter((d) => d.status === "verified").length,
  actionNeeded: documents.filter((d) => d.status !== "verified").length,
  storageLabel: "24.8 MB of 2 GB",
};

export const uploadOptions = [
  { id: "scan", label: "Scan document", hint: "Use the camera" },
  { id: "upload", label: "Upload file", hint: "PDF, JPG, DOCX" },
  { id: "import", label: "Import from mail", hint: "Statements inbox" },
  { id: "link", label: "Link to asset", hint: "Attach to property or loan" },
] as const;
