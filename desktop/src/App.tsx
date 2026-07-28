import { useState } from 'react';
import { AuthProvider, useAuth } from './context/AuthContext';
import AuthPage from './pages/AuthPage';
import QuestionListPage from './pages/QuestionListPage';
import QuestionDetailPage from './pages/QuestionDetailPage';

function AppContent() {
  const { isAuthenticated } = useAuth();
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

  return <QuestionListPage onSelectQuestion={setSelectedQuestionId} />;
}

export default function App() {
  return (
    <AuthProvider>
      <AppContent />
    </AuthProvider>
  );
}
