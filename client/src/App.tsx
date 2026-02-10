import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { AuthProvider } from './contexts/AuthContext'
import { ThemeProvider } from './contexts/ThemeContext'
import { AppShell } from './components/layout/AppShell'
import { PublicLayout } from './components/layout/PublicLayout'
import { LoginPage } from './pages/auth/LoginPage'
import { SignupPage } from './pages/auth/SignupPage'
import { OnboardingWizardPage } from './pages/onboarding/OnboardingWizardPage'
import { DashboardPage } from './pages/dashboard/DashboardPage'
import { ScannerPage } from './pages/scanner/ScannerPage'
import { LandingPage } from './pages/customer/LandingPage'
import { ConfirmationPage } from './pages/customer/ConfirmationPage'
import { NotFoundPage } from './pages/NotFoundPage'

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <ThemeProvider>
          <Routes>
            {/* Rotas públicas */}
            <Route element={<PublicLayout />}>
              <Route path="/login" element={<LoginPage />} />
              <Route path="/signup" element={<SignupPage />} />
              <Route path="/c/:campaignId" element={<LandingPage />} />
              <Route path="/c/:campaignId/success" element={<ConfirmationPage />} />
            </Route>

            {/* Rotas autenticadas */}
            <Route element={<AppShell />}>
              <Route path="/onboarding" element={<OnboardingWizardPage />} />
              <Route path="/dashboard" element={<DashboardPage />} />
              <Route path="/scanner" element={<ScannerPage />} />
            </Route>

            {/* Redirect e 404 */}
            <Route path="/" element={<Navigate to="/dashboard" replace />} />
            <Route path="*" element={<NotFoundPage />} />
          </Routes>
        </ThemeProvider>
      </AuthProvider>
    </BrowserRouter>
  )
}
