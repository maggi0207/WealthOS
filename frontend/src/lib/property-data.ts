/**
 * Property Details mock fixtures (INR) — Ramana Flats, Door No. 3, Adyar.
 * Pure frontend data; no backend, no network.
 */

import { fmtINR, fmtINRShort } from "@/lib/wealth-data";

import driveway from "@/assets/ramana-driveway.png.asset.json";
import streetSign from "@/assets/ramana-street-sign.png.asset.json";

export { fmtINR, fmtINRShort };

export const propertyDetail = {
  id: "ramana-flats-3",
  name: "Ramana Flats",
  doorNumber: "Door No. 3",
  owner: "Magesh",
  addressLines: [
    "No.16, Ramana Flats,",
    "Door No.3,",
    "Anna Avenue,",
    "Adyar,",
    "Chennai – 600020",
  ],
  address: "No.16, Ramana Flats, Door No.3, Anna Avenue, Adyar, Chennai – 600020",
  locality: "Anna Avenue, Adyar",
  city: "Chennai",
  purchaseYear: 2018,
  purchaseDate: "14 Mar 2018",
  purchasePrice: 96_00_000,
  currentValue: 1_50_00_000,
  ownershipPct: 100,
  owners: "Magesh (sole owner)",
  type: "Residential Apartment",
};

export const appreciation = {
  absolute: propertyDetail.currentValue - propertyDetail.purchasePrice,
  pct:
    ((propertyDetail.currentValue - propertyDetail.purchasePrice) / propertyDetail.purchasePrice) *
    100,
  cagrPct: 7.3,
};

export const rental = {
  monthlyRent: 32_000,
  yieldPct: 2.6,
};

/** Real photographs of the property, used for the hero gallery. */
export type Photo = { id: string; url: string; caption: string; category: GalleryCategory };
export type GalleryCategory = "Exterior" | "Street" | "Entrance" | "Driveway" | "Location";

export const galleryCategories: GalleryCategory[] = [
  "Exterior",
  "Street",
  "Entrance",
  "Driveway",
  "Location",
];

export const photos: Photo[] = [
  {
    id: "p1",
    url: driveway.url,
    caption: "Driveway & entrance approach",
    category: "Driveway",
  },
  {
    id: "p2",
    url: streetSign.url,
    caption: "Anna Avenue street sign · Ward 175",
    category: "Location",
  },
  {
    id: "p3",
    url: driveway.url,
    caption: "Building exterior from the compound",
    category: "Exterior",
  },
  {
    id: "p4",
    url: streetSign.url,
    caption: "Anna Avenue street view",
    category: "Street",
  },
  {
    id: "p5",
    url: driveway.url,
    caption: "Ground floor entrance",
    category: "Entrance",
  },
];

export type Fact = { label: string; value: string };

export const keyFacts: Fact[] = [
  { label: "Built-up", value: "970 sq ft" },
  { label: "UDS", value: "795 sq ft" },
  { label: "Floor", value: "Ground Floor" },
  { label: "Road", value: "40 ft Main Road" },
  { label: "Built", value: "1998" },
  { label: "Ownership", value: "First Owner" },
  { label: "Type", value: "Residential Apartment" },
];

/** Purchase vs current value trajectory (₹ lakh). */
export const valueSeries = [
  { year: "2018", purchase: 96, market: 96 },
  { year: "2019", purchase: 96, market: 102 },
  { year: "2020", purchase: 96, market: 106 },
  { year: "2021", purchase: 96, market: 114 },
  { year: "2022", purchase: 96, market: 124 },
  { year: "2023", purchase: 96, market: 133 },
  { year: "2024", purchase: 96, market: 142 },
  { year: "2025", purchase: 96, market: 150 },
];

export const homeLoan = {
  lender: "HDFC Bank · Home Loan",
  accountMask: "•••• 4821",
  sanctioned: 72_00_000,
  outstanding: 38_40_000,
  emi: 42_600,
  ratePct: 8.4,
  tenureMonthsLeft: 97,
  payoffDate: "Jun 2033",
  nextEmiOn: "5 Aug 2026",
  interestPaidToDate: 31_20_000,
};

export const equity = propertyDetail.currentValue - homeLoan.outstanding;
export const equityPct = Math.round((equity / propertyDetail.currentValue) * 100);
export const loanRepaidPct = Math.round(
  ((homeLoan.sanctioned - homeLoan.outstanding) / homeLoan.sanctioned) * 100,
);

export const prepaymentInsight = {
  headline: "Prepay ₹5 L to close 19 months early",
  body: "A one-time ₹5,00,000 prepayment against the Ramana Flats home loan cuts total interest by about ₹7.4 L and moves your payoff from Jun 2033 to Nov 2031.",
  savings: 7_40_000,
};

/** Property score — weighted sub-scores out of 10. */
export type ScoreItem = { label: string; score: number; note: string };

export const propertyScore = {
  overall: 8.9,
  grade: "Excellent",
  items: [
    { label: "Location", score: 9.5, note: "Prime Adyar, Anna Avenue" },
    { label: "Connectivity", score: 9.0, note: "40 ft main road access" },
    { label: "Appreciation", score: 8.8, note: "7.3% CAGR since 2018" },
    { label: "Rental Potential", score: 8.0, note: "₹32,000 / month achievable" },
    { label: "Liquidity", score: 9.2, note: "High demand micro-market" },
  ] as ScoreItem[],
};

export type Insight = { id: string; tone: "positive" | "neutral" | "action"; text: string };

export const aiInsights: Insight[] = [
  {
    id: "i1",
    tone: "positive",
    text: "Prime Adyar location with strong long-term appreciation potential.",
  },
  { id: "i2", tone: "neutral", text: "High UDS (795 sq ft) improves redevelopment value." },
  { id: "i3", tone: "action", text: "Recommended to hold long term." },
];

export type VaultDoc = {
  id: string;
  name: string;
  meta: string;
  status: "verified" | "pending" | "expiring" | "missing";
};

export const vaultDocs: VaultDoc[] = [
  { id: "sale-deed", name: "Sale Deed", meta: "PDF · 4.2 MB · 2018", status: "verified" },
  { id: "ec", name: "Encumbrance Certificate", meta: "PDF · 1.1 MB · Jan 2026", status: "verified" },
  { id: "patta", name: "Patta / Chitta", meta: "PDF · 820 KB · 2019", status: "pending" },
  { id: "tax", name: "Property Tax", meta: "6 files · upto H1 2026", status: "expiring" },
  { id: "loan", name: "Loan Documents", meta: "PDF · 2.4 MB · HDFC", status: "verified" },
  { id: "insurance", name: "Insurance", meta: "Not uploaded yet", status: "missing" },
];

export type NearbyPlace = { id: string; kind: string; name: string; distance: string };

export const nearbyPlaces: NearbyPlace[] = [
  { id: "n1", kind: "School", name: "Adyar Matriculation School", distance: "0.6 km" },
  { id: "n2", kind: "Hospital", name: "Fortis Malar Hospital", distance: "1.4 km" },
  { id: "n3", kind: "Metro", name: "Thiruvanmiyur Metro (proposed)", distance: "2.1 km" },
  { id: "n4", kind: "Beach", name: "Elliot's Beach, Besant Nagar", distance: "2.8 km" },
  { id: "n5", kind: "Railway", name: "Thiruvanmiyur MRTS", distance: "2.5 km" },
  { id: "n6", kind: "Airport", name: "Chennai Intl. Airport", distance: "14.2 km" },
];

export const upkeep = {
  maintenanceMonthly: 3_200,
  maintenancePaidTill: "Aug 2026",
  propertyTaxHalfYear: 4_860,
  propertyTaxDue: "30 Sep 2026",
  annualUpkeep: 78_400,
  lastServiced: "Water tank cleaning · Jun 2026",
};

export type TimelineEvent = {
  id: string;
  date: string;
  title: string;
  detail: string;
  kind: "purchase" | "loan" | "legal" | "upkeep" | "value";
};

export const timeline: TimelineEvent[] = [
  { id: "t1", date: "Aug 2026", title: "Current", detail: "Market estimate ₹1.50 Cr · 8.9/10 score", kind: "value" },
  { id: "t2", date: "Apr 2023", title: "Renovation", detail: "Kitchen and flooring refresh · ₹4.6 L", kind: "upkeep" },
  { id: "t3", date: "May 2018", title: "Loan started", detail: "HDFC home loan ₹72,00,000 · 8.4%", kind: "loan" },
  { id: "t4", date: "Mar 2018", title: "Purchase", detail: "Registered for ₹96,00,000 · first owner", kind: "purchase" },
];
