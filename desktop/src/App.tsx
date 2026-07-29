import { useState } from 'react';
import { AuthProvider, useAuth } from './context/AuthContext';
import AuthPage from './pages/AuthPage';
import QuestionListPage from './pages/QuestionListPage';
import QuestionDetailPage from './pages/QuestionDetailPage';
import ProfilePage from './pages/ProfilePage';
import LeaderboardPage from './pages/LeaderboardPage';
import AppShell, { type View } from './components/AppShell';

function AppContent() {
  const { isAuthenticated } = useAuth();
  const [activeView, setActiveView] = useState<View>('questions');
  const [selectedQuestionId, setSelectedQuestionId] = useState<string | null>(null);

  if (!isAuthenticated) {
    return <AuthPage />;
  }

  // Soru detayi, ust navigasyonu gizleyip kendi "geri don" akisini kullanir -
  // bir soruyu cozerken dikkat dagitmayan tam ekran bir deneyim vermek icin.
  if (selectedQuestionId) {
    return (
      <QuestionDetailPage
        questionId={selectedQuestionId}
        onBack={() => setSelectedQuestionId(null)}
      />
    );
  }

  return (
    <AppShell activeView={activeView} onNavigate={setActiveView}>
      {activeView === 'questions' && (
        <QuestionListPage onSelectQuestion={setSelectedQuestionId} />
      )}
      {activeView === 'profile' && <ProfilePage />}
      {activeView === 'leaderboard' && <LeaderboardPage />}
    </AppShell>
  );
}

export default function App() {
  return (
    <AuthProvider>
      <AppContent />
    </AuthProvider>
  );
}
