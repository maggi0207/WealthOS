import { isMockApiMode } from "@/config/env";
import {
  documents as mockDocs,
  recentDocuments as mockRecent,
  renewals as mockRenewals,
  vaultSummary as mockVault,
  type DocCategory,
  type DocStatus,
  type RenewalItem,
  type VaultDocument,
} from "@/lib/documents-data";
import { BaseApiService } from "@/services/http/base-api-service";
import type {
  CreateDocumentRequestDto,
  DocumentDto,
  UpdateDocumentRequestDto,
} from "@/services/documents/types";

export type VaultSummaryView = {
  total: number;
  verified: number;
  actionNeeded: number;
  storageLabel: string;
  categoryCount: number;
};

export type DocumentsOverview = {
  summary: VaultSummaryView;
  documents: VaultDocument[];
  recent: VaultDocument[];
  renewals: RenewalItem[];
};

type DocListItemDto = {
  id: string;
  title: string;
  category: number | string;
  status: number | string;
  owner?: string | null;
  expiryDate?: string | null;
  originalFileName?: string | null;
  fileSizeBytes?: number;
  updatedAt?: string | null;
  createdAt?: string;
  tags?: Array<{ name: string } | string>;
  linkedEntityName?: string | null;
};

type DocListDto = { items: DocListItemDto[]; totalCount?: number };

function n(v: unknown, f = 0) {
  const x = typeof v === "number" ? v : Number(v);
  return Number.isFinite(x) ? x : f;
}

function mapCategory(c: number | string): DocCategory {
  const k = String(c).toLowerCase();
  if (k.includes("loan") || k === "1") return "loans";
  if (k.includes("invest") || k === "2") return "investments";
  if (k.includes("identity") || k === "3") return "identity";
  if (k.includes("insurance") || k === "4") return "insurance";
  if (k.includes("tax") || k === "5") return "tax";
  return "property";
}

function mapStatus(s: number | string, expiry?: string | null): DocStatus {
  const k = String(s).toLowerCase();
  if (k.includes("expired") || k === "3") return "expired";
  if (k.includes("expir") || k === "2") return "expiring";
  if (k.includes("pending") || k === "1") return "pending";
  if (expiry) {
    const d = new Date(`${expiry.slice(0, 10)}T00:00:00`);
    const days = (d.getTime() - Date.now()) / 86_400_000;
    if (days < 0) return "expired";
    if (days < 60) return "expiring";
  }
  return "verified";
}

function fileType(name?: string | null): VaultDocument["fileType"] {
  const lower = (name || "").toLowerCase();
  if (lower.endsWith(".jpg") || lower.endsWith(".jpeg") || lower.endsWith(".png")) return "JPG";
  if (lower.endsWith(".docx") || lower.endsWith(".doc")) return "DOCX";
  return "PDF";
}

function sizeLabel(bytes?: number): string {
  if (!bytes || bytes <= 0) return "—";
  if (bytes < 1024) return `${bytes} B`;
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(0)} KB`;
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
}

function mapDoc(dto: DocListItemDto): VaultDocument {
  const tags = (dto.tags ?? []).map((t) => (typeof t === "string" ? t : t.name));
  return {
    id: String(dto.id),
    name: dto.title,
    category: mapCategory(dto.category),
    status: mapStatus(dto.status, dto.expiryDate),
    fileType: fileType(dto.originalFileName),
    sizeLabel: sizeLabel(dto.fileSizeBytes),
    updatedOn: (dto.updatedAt || dto.createdAt || "").slice(0, 10),
    expiresOn: dto.expiryDate?.slice(0, 10),
    linkedTo: dto.linkedEntityName || dto.owner || undefined,
    tags,
  };
}

function mapMock(): DocumentsOverview {
  return {
    summary: { ...mockVault, categoryCount: 6 },
    documents: mockDocs.map((d) => ({ ...d, tags: [...d.tags] })),
    recent: mockRecent.map((d) => ({ ...d, tags: [...d.tags] })),
    renewals: mockRenewals.map((r) => ({ ...r })),
  };
}

class DocumentService extends BaseApiService {
  protected readonly serviceName = "DocumentService";

  async getOverview(): Promise<DocumentsOverview> {
    if (isMockApiMode()) return mapMock();

    const [list, recent, expired] = await Promise.all([
      this.get<DocListDto>("/documents?pageSize=100"),
      this.get<DocListItemDto[]>("/documents/recent").catch(() => []),
      this.get<DocListItemDto[]>("/documents/expired").catch(() => []),
    ]);

    const documents = (list.items ?? []).map(mapDoc);
    const recentDocs = (Array.isArray(recent) ? recent : []).map(mapDoc);
    const expiredDocs = (Array.isArray(expired) ? expired : []).map(mapDoc);

    const verified = documents.filter((d) => d.status === "verified").length;
    const actionNeeded = documents.filter(
      (d) => d.status === "expiring" || d.status === "expired" || d.status === "pending",
    ).length;
    const categories = new Set(documents.map((d) => d.category));
    const totalBytes = (list.items ?? []).reduce((s, d) => s + n(d.fileSizeBytes), 0);

    const renewals: RenewalItem[] = [
      ...expiredDocs,
      ...documents.filter((d) => d.status === "expiring" && d.expiresOn),
    ]
      .filter((d, i, arr) => arr.findIndex((x) => x.id === d.id) === i)
      .slice(0, 8)
      .map((d) => ({
        id: d.id,
        name: d.name,
        expiresOn: d.expiresOn!,
        category: d.category,
        status: d.status,
      }));

    return {
      summary: {
        total: n(list.totalCount, documents.length),
        verified,
        actionNeeded,
        storageLabel: sizeLabel(totalBytes),
        categoryCount: categories.size || 6,
      },
      documents,
      recent: recentDocs.length > 0 ? recentDocs : documents.slice(0, 5),
      renewals,
    };
  }

  async create(body: CreateDocumentRequestDto): Promise<DocumentDto> {
    if (isMockApiMode()) {
      return {
        id: crypto.randomUUID(),
        title: body.title,
        category: body.category,
        owner: body.owner,
        status: body.status ?? 1,
        description: body.description,
        notes: body.notes,
      };
    }
    return this.post<DocumentDto>("/documents", body);
  }

  async update(id: string, body: UpdateDocumentRequestDto): Promise<DocumentDto> {
    if (isMockApiMode()) {
      return { id, ...body, status: body.status ?? 1 };
    }
    return this.put<DocumentDto>(`/documents/${id}`, body);
  }

  async remove(id: string): Promise<void> {
    if (isMockApiMode()) return;
    await this.delete<unknown>(`/documents/${id}`);
  }
}

export const documentService = new DocumentService();
