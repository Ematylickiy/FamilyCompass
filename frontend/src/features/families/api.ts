import client from '../../api/http/client';
import type { CreateFamilyRequest, Family } from './types';

export const familiesApi = {
  getMine: () => client.get<Family[]>('families'),
  create: (payload: CreateFamilyRequest) =>
    client.post<Family>('families', { name: payload.name }),
};
