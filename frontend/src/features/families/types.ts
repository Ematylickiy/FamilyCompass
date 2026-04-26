export const FamilyRole = {
  Owner: 1,
  Member: 2,
} as const;

export type FamilyRole = (typeof FamilyRole)[keyof typeof FamilyRole];

export interface Family {
  id: string;
  name: string;
  createdByUserId: string;
  createdAt: string;
  role: FamilyRole;
}

export interface CreateFamilyRequest {
  name: string;
}

export interface FamilyMember {
  userId: string;
  username: string;
  role: string;
}
