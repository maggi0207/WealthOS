import { useMutation, useQuery } from "@tanstack/react-query";

import { reportService } from "@/services/reports/report-service";

export const reportKeys = {
  all: ["reports"] as const,
  summary: () => [...reportKeys.all, "summary"] as const,
};

export function useReportSummary() {
  return useQuery({
    queryKey: reportKeys.summary(),
    queryFn: () => reportService.getSummary(),
  });
}

export function useReportExport() {
  return useMutation({
    mutationFn: (format: "pdf" | "csv" = "pdf") =>
      reportService.requestExport(format),
  });
}
