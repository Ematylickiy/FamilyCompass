import { useCallback, useEffect, useState } from 'react';
import { familiesApi } from '../api';
import type { Family } from '../types';

export function useFamilies() {
  const [families, setFamilies] = useState<Family[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await familiesApi.getMine();
      setFamilies(data);
    } catch {
      setError('Не удалось загрузить семьи');
    } finally {
      setLoading(false);
    }
  }, []);

  const createFamily = useCallback(async (name: string) => {
    const trimmed = name.trim();
    if (!trimmed) return false;

    try {
      setError(null);
      const created = await familiesApi.create({ name: trimmed });
      setFamilies((prev) => [created, ...prev]);
      return true;
    } catch {
      setError('Не удалось создать семью');
      return false;
    }
  }, []);

  useEffect(() => {
    void load();
  }, [load]);

  return { families, loading, error, createFamily };
}
