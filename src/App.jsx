import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import LoginPage from './pages/LoginPage'
import TimelinePage from './pages/TimelinePage'
import ProfilePage from './pages/ProfilePage'
import EditProfilePage from './pages/EditProfilePage'
import ProtectedRoute from './components/ProtectedRoute'
import SearchPage from './pages/SearchPage'
import RegisterPage from './pages/RegisterPage'
import FollowRequestsPage from './pages/FollowRequestsPage'




function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />

        <Route path="/register" element={<RegisterPage />} />

        <Route
          path="/timeline"
          element={
            <ProtectedRoute>
              <TimelinePage />
            </ProtectedRoute>
          }
        />

        <Route
          path="/follow-requests"
          element={
            <ProtectedRoute>
              <FollowRequestsPage />
            </ProtectedRoute>
          }
        />

        <Route path="/search" element={<SearchPage />} />

        <Route
          path="/profile/edit"
          element={
            <ProtectedRoute>
              <EditProfilePage />
            </ProtectedRoute>
          }
        />

        <Route
          path="/profile/:id"
          element={
            <ProtectedRoute>
              <ProfilePage />
            </ProtectedRoute>
          }
        />

        <Route path="/" element={<Navigate to="/timeline" replace />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App