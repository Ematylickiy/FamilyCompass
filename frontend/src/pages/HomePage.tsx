import { useState, type FormEventHandler } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../auth/useAuth';
import { useFamilies } from '../features/families';
import { FamilyRole } from '../features/families/types';
import { Alert } from '../ui/Alert';
import { Button } from '../ui/Button';
import { Card } from '../ui/Card';
import styles from '../App.module.css';

export function HomePage() {
  const navigate = useNavigate();
  const { logout } = useAuth();
  const { families, loading, error, createFamily } = useFamilies();
  const [name, setName] = useState('');
  const [submitting, setSubmitting] = useState(false);

  const handleLogout = () => {
    logout();
    navigate('/login', { replace: true });
  };

  const handleCreate: FormEventHandler<HTMLFormElement> = async (e) => {
    e.preventDefault();
    setSubmitting(true);
    const ok = await createFamily(name);
    if (ok) setName('');
    setSubmitting(false);
  };

  if (loading) {
    return (
      <div className={styles.page}>
        <div className={styles.inner}>
          <Alert tone="loading" role="status" className={styles.banner}>
            Загрузка…
          </Alert>
        </div>
      </div>
    );
  }

  return (
    <div className={styles.page}>
      <div className={styles.inner}>
        <Card variant="glass" hero as="header" className={styles.header}>
          <div className={styles.headerRow}>
            <h1 className={styles.title}>Семейные финансы</h1>
            <Button type="button" variant="ghost" onClick={handleLogout}>
              Выйти
            </Button>
          </div>
        </Card>

        {error ? (
          <Alert tone="error" role="alert" className={styles.banner}>
            {error}
          </Alert>
        ) : null}

        <Card variant="glass" as="section" aria-labelledby="create-family-heading">
          <h2 id="create-family-heading" className={styles.sectionTitle}>
            Создать семью
          </h2>
          <form onSubmit={handleCreate} className={styles.simpleForm}>
            <input
              className={styles.simpleInput}
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="Название семьи"
              maxLength={128}
              required
            />
            <Button type="submit" variant="brand" disabled={submitting}>
              {submitting ? 'Создание...' : 'Создать'}
            </Button>
          </form>
        </Card>

        <Card variant="glass" as="section" aria-labelledby="families-heading">
          <h2 id="families-heading" className={styles.sectionTitle}>
            Мои семьи
          </h2>
          {families.length === 0 ? (
            <p className={styles.emptyText}>Вы пока не состоите ни в одной семье.</p>
          ) : (
            <ul className={styles.familiesList}>
              {families.map((family) => (
                <li key={family.id} className={styles.familyItem}>
                  <span>{family.name}</span>
                  <span className={styles.familyRole}>
                    {family.role === FamilyRole.Owner ? 'Владелец' : 'Участник'}
                  </span>
                </li>
              ))}
            </ul>
          )}
        </Card>
      </div>
    </div>
  );
}
