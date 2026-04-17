import { useState } from 'react'
import { Navigate } from 'react-router-dom'
import { useAuth } from '../../contexts/AuthContext'
import { api } from '../../lib/api'
import { StepIndicator } from '../../components/onboarding/StepIndicator'
import { StepIdentidadeVisual } from './StepIdentidadeVisual'
import { StepRegraCampanha } from './StepRegraCampanha'
import { StepPreviewConfirmacao } from './StepPreviewConfirmacao'
import { StepTotemPronto } from './StepTotemPronto'
import type { Campaign } from '../../types'

interface OnboardingState {
  logoFile: File | null
  logoPreviewUrl: string | null
  corPrimaria: string
  corSecundaria: string
  nomeCampanha: string
  checkinsNecessarios: number
  descricaoPremio: string
}

const initialState: OnboardingState = {
  logoFile: null,
  logoPreviewUrl: null,
  corPrimaria: '#4f46e5',
  corSecundaria: '#818cf8',
  nomeCampanha: '',
  checkinsNecessarios: 10,
  descricaoPremio: '',
}

export function OnboardingWizardPage() {
  const { user, isLoading } = useAuth()

  const [state, setState] = useState<OnboardingState>(initialState)
  const [currentStep, setCurrentStep] = useState(1)
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [createdCampaign, setCreatedCampaign] = useState<Campaign | null>(null)
  const [error, setError] = useState('')

  if (isLoading) {
    return (
      <div className="flex h-screen items-center justify-center">
        <p className="text-sm text-gray-500">Carregando...</p>
      </div>
    )
  }

  if (!user) {
    return <Navigate to="/login" replace />
  }

  function validateStep(): boolean {
    setError('')
    if (currentStep === 2) {
      if (!state.nomeCampanha.trim()) {
        setError('Preencha o nome da campanha.')
        return false
      }
      if (state.checkinsNecessarios < 2) {
        setError('O mínimo de check-ins é 2.')
        return false
      }
      if (!state.descricaoPremio.trim()) {
        setError('Preencha a descrição do prêmio.')
        return false
      }
    }
    return true
  }

  function goNext() {
    if (!validateStep()) return
    setCurrentStep((s) => Math.min(s + 1, 4))
  }

  function goBack() {
    setError('')
    setCurrentStep((s) => Math.max(s - 1, 1))
  }

  function handleLogoChange(file: File, previewUrl: string) {
    if (state.logoPreviewUrl) {
      URL.revokeObjectURL(state.logoPreviewUrl)
    }
    setState((s) => ({ ...s, logoFile: file, logoPreviewUrl: previewUrl }))
  }

  async function handleCreateCampaign() {
    setIsSubmitting(true)
    setError('')

    try {
      const campaign = await api<Campaign>('/campaigns', {
        method: 'POST',
        body: JSON.stringify({
          nome: state.nomeCampanha,
          descricao: state.descricaoPremio,
          checkinsNecessarios: state.checkinsNecessarios,
        }),
      })

      setCreatedCampaign(campaign)
      setCurrentStep(4)
    } catch {
      setError('Não foi possível criar a campanha. Tente novamente.')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className="min-h-screen bg-white">
      <div className="mx-auto max-w-3xl px-4 py-8 sm:px-6 lg:px-8">
        {/* Header */}
        <p className="text-2xl font-bold text-primary">Voltei</p>

        {/* Step indicator — hidden on step 4 */}
        {currentStep < 4 && <StepIndicator currentStep={currentStep} />}

        {/* Error */}
        {error && (
          <p className="mt-4 text-sm text-red-600">{error}</p>
        )}

        {/* Step content */}
        <div className="mt-8">
          {currentStep === 1 && (
            <StepIdentidadeVisual
              logoPreviewUrl={state.logoPreviewUrl}
              corPrimaria={state.corPrimaria}
              corSecundaria={state.corSecundaria}
              nomeNegocio={user.nome}
              nomeCampanha={state.nomeCampanha}
              checkinsNecessarios={state.checkinsNecessarios}
              descricaoPremio={state.descricaoPremio}
              onLogoChange={handleLogoChange}
              onCoresChange={(primaria, secundaria) =>
                setState((s) => ({ ...s, corPrimaria: primaria, corSecundaria: secundaria }))
              }
            />
          )}

          {currentStep === 2 && (
            <StepRegraCampanha
              nomeCampanha={state.nomeCampanha}
              checkinsNecessarios={state.checkinsNecessarios}
              descricaoPremio={state.descricaoPremio}
              onNomeCampanhaChange={(v) => setState((s) => ({ ...s, nomeCampanha: v }))}
              onCheckinsNecessariosChange={(v) => setState((s) => ({ ...s, checkinsNecessarios: v }))}
              onDescricaoPremioChange={(v) => setState((s) => ({ ...s, descricaoPremio: v }))}
            />
          )}

          {currentStep === 3 && (
            <StepPreviewConfirmacao
              state={state}
              nomeNegocio={user.nome}
              isSubmitting={isSubmitting}
              onBack={goBack}
              onSubmit={handleCreateCampaign}
            />
          )}

          {currentStep === 4 && createdCampaign && (
            <StepTotemPronto
              campaign={createdCampaign}
              state={state}
              nomeNegocio={user.nome}
            />
          )}
        </div>

        {/* Navigation — steps 1-2 only */}
        {currentStep <= 2 && (
          <div className="mt-8 flex items-center justify-between">
            {currentStep > 1 ? (
              <button
                type="button"
                onClick={goBack}
                className="cursor-pointer text-sm font-semibold text-gray-600 hover:text-gray-900"
              >
                Voltar
              </button>
            ) : (
              <div />
            )}
            <button
              type="button"
              onClick={goNext}
              className="flex cursor-pointer justify-center rounded-md bg-primary px-6 py-2 text-sm/6 font-semibold text-white shadow-xs hover:bg-secondary focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-primary"
            >
              Próximo
            </button>
          </div>
        )}
      </div>
    </div>
  )
}
