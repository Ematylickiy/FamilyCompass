import client from '../../api/http/client';
import type { CreateFamilyRequest, Family, FamilyMember } from './types';

export const familiesApi = {
  getMine: () => client.get<Family[]>('families'),
  create: (payload: CreateFamilyRequest) =>
    client.post<Family>('families', { name: payload.name }),
  delete: (familyId: string) => client.delete(`families/${familyId}`),
  getMembers: (familyId: string) =>
    client.get<FamilyMember[]>(`families/${familyId}/members`),
};
