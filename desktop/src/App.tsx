import { useState } from 'react';
import { AuthProvider, useAuth } from './context/AuthContext';
import AuthPage from './pages/AuthPage';
import QuestionListPage from './pages/QuestionListPage';
import QuestionDetailPage from './pages/QuestionDetailPage';
import ProfilePage from './pages/ProfilePage';
import LeaderboardPage from './pages/LeaderboardPage';
import ContestListPage from './pages/ContestListPage';
import ContestDetailPage from './pages/ContestDetailPage';
import AppShell, { type View } from './components/AppShell';

function AppContent() {
  const { isAuthenticated } = useAuth();
  const [activeView, setActiveView] = useState<View>('questions');
  const [selectedContestId, setSelectedContestId] = useState<string | null>(null);
  const [selectedQuestionId, setSelectedQuestionId] = useState<string | null>(null);

  if (!isAuthenticated) {
    return <AuthPage />;
  }

  if (selectedQuestionId) {
    return (
      <QuestionDetailPage
        questionId={selectedQuestionId}
        onBack={() => setSelectedQuestionId(null)}
      />
    );
  }

  if (selectedContestId) {
    return (
      <ContestDetailPage
        contestId={selectedContestId}
        onBack={() => setSelectedContestId(null)}
        onSelectQuestion={setSelectedQuestionId}
      />
    );
  }

  return (
    <AppShell activeView={activeView} onNavigate={setActiveView}>
      {activeView === 'questions' && (
        <QuestionListPage onSelectQuestion={setSelectedQuestionId} />
      )}
      {activeView === 'contests' && (
        <ContestListPage onSelectContest={setSelectedContestId} />
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
