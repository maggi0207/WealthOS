/** Document API DTOs — aligned with WealthOS.Application.Documents */

export type DocumentCategoryDto = 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9;

export type DocumentStatusDto = 0 | 1 | 2 | 3 | 4 | 5;

export type DocumentDto = {
  id: string;
  title: string;
  description?: string | null;
  category: DocumentCategoryDto | string;
  owner: string;
  status: DocumentStatusDto | string;
  notes?: string | null;
  expiryDate?: string | null;
  tags?: Array<{ id: string; name: string } | string>;
};

export type CreateDocumentRequestDto = {
  title: string;
  description?: string | null;
  category: DocumentCategoryDto;
  owner: string;
  status?: DocumentStatusDto;
  notes?: string | null;
  tags?: string[];
  referenceModule?: number;
  fileSizeBytes?: number;
  storageProvider?: number;
};

export type UpdateDocumentRequestDto = {
  title: string;
  description?: string | null;
  category: DocumentCategoryDto;
  owner: string;
  status?: DocumentStatusDto;
  notes?: string | null;
  referenceModule?: number;
};
