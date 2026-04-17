import { useState, useEffect, type FormEvent } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { api, ApiError } from '../../lib/api'
import type { CampaignPublicInfo, EnrollResponse } from '../../types'

type PageState =
  | { phase: 'loading' }
  | { phase: 'error'; message: string }
  | { phase: 'phone' }
  | { phase: 'form' }
  | { phase: 'enrolling' }
  | { phase: 'progress'; enrollment: EnrollResponse }
  | { phase: 'wallet'; enrollment: EnrollResponse }

export function LandingPage() {
  const { campaignId } = useParams()
  const navigate = useNavigate()
  const [campaign, setCampaign] = useState<CampaignPublicInfo | null>(null)
  const [state, setState] = useState<PageState>({ phase: 'loading' })
  const [phone, setPhone] = useState('')
  const [name, setName] = useState('')

  const isIOS = /iPad|iPhone|iPod/.test(navigator.userAgent)
  const isAndroid = /Android/.test(navigator.userAgent)

  useEffect(() => {
    loadCampaign()
  }, [campaignId])

  async function loadCampaign() {
    try {
      const data = await api<CampaignPublicInfo>(`/enroll/${campaignId}`)
      setCampaign(data)
      setState({ phase: 'phone' })
    } catch (err) {
      setState({
        phase: 'error',
        message: err instanceof ApiError ? err.message : 'Erro ao carregar campanha.',
      })
    }
  }

  async function handlePhoneSubmit(e: FormEvent) {
    e.preventDefault()
    if (!phone.trim()) return

    try {
      const enrollment = await api<EnrollResponse>(
        `/enroll/${campaignId}/status?telefone=${encodeURIComponent(phone.trim())}`,
      )
      setState({ phase: 'progress', enrollment })
    } catch (err) {
      if (err instanceof ApiError && err.status === 404) {
        setState({ phase: 'form' })
      } else {
        setState({
          phase: 'error',
          message: err instanceof ApiError ? err.message : 'Erro ao verificar telefone.',
        })
      }
    }
  }

  async function handleEnroll(e: FormEvent) {
    e.preventDefault()
    if (!name.trim() || !phone.trim()) return

    setState({ phase: 'enrolling' })

    try {
      const enrollment = await api<EnrollResponse>(`/enroll/${campaignId}`, {
        method: 'POST',
        body: JSON.stringify({ nome: name.trim(), telefone: phone.trim() }),
      })
      setState({ phase: 'wallet', enrollment })
    } catch (err) {
      setState({
        phase: 'error',
        message: err instanceof ApiError ? err.message : 'Erro ao realizar inscrição.',
      })
    }
  }

  function handleAddToGoogleWallet(url: string) {
    window.open(url, '_blank')
    navigate(`/c/${campaignId}/success`)
  }

  function handleAddToAppleWallet(url: string) {
    window.location.href = url
    setTimeout(() => navigate(`/c/${campaignId}/success`), 1000)
  }

  if (!campaign && state.phase === 'loading') {
    return (
      <div className="flex min-h-screen items-center justify-center bg-gray-50">
        <div className="size-8 animate-spin rounded-full border-4 border-gray-200 border-t-indigo-600" />
      </div>
    )
  }

  if (state.phase === 'error') {
    return (
      <div className="flex min-h-screen items-center justify-center bg-gray-50 px-4">
        <div className="w-full max-w-sm text-center">
          <div className="mx-auto flex size-12 items-center justify-center rounded-full bg-red-100">
            <svg className="size-6 text-red-600" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" d="M12 9v3.75m9-.75a9 9 0 1 1-18 0 9 9 0 0 1 18 0Zm-9 3.75h.008v.008H12v-.008Z" />
            </svg>
          </div>
          <p className="mt-4 text-sm text-gray-600">{state.message}</p>
        </div>
      </div>
    )
  }

  if (!campaign) return null

  const bgStyle = { backgroundColor: campaign.coresPrimaria }
  const textOnBg = isLightColor(campaign.coresPrimaria) ? 'text-gray-900' : 'text-white'

  return (
    <div className="flex min-h-screen flex-col bg-gray-50">
      {/* Header com branding */}
      <div className="px-4 py-8 text-center" style={bgStyle}>
        {campaign.logoUrl && (
          <img
            src={campaign.logoUrl}
            alt={campaign.negocioNome}
            className="mx-auto mb-4 size-16 rounded-full object-cover bg-white/20"
          />
        )}
        <h1 className={`text-2xl font-bold ${textOnBg}`}>{campaign.negocioNome}</h1>
        <p className={`mt-1 text-sm ${textOnBg} opacity-80`}>{campaign.nome}</p>
      </div>

      <div className="flex flex-1 items-start justify-center px-4 py-6">
        <div className="w-full max-w-sm">
          {/* Regra da campanha */}
          <div className="mb-6 rounded-lg bg-white p-4 shadow-sm ring-1 ring-gray-900/5">
            <p className="text-center text-sm text-gray-700">
              Acumule <span className="font-bold">{campaign.checkinsNecessarios} check-ins</span> e ganhe:{' '}
              <span className="font-bold" style={{ color: campaign.coresPrimaria }}>
                {campaign.descricao}
              </span>
            </p>
          </div>

          {/* Phone step */}
          {state.phase === 'phone' && (
            <form onSubmit={handlePhoneSubmit} className="space-y-4">
              <div>
                <label htmlFor="phone" className="block text-sm/6 font-medium text-gray-900">
                  Seu telefone
                </label>
                <div className="mt-2">
                  <input
                    id="phone"
                    type="tel"
                    value={phone}
                    onChange={(e) => setPhone(e.target.value)}
                    placeholder="+55 11 99999-0000"
                    required
                    className="block w-full rounded-md bg-white px-3 py-2 text-base text-gray-900 outline-1 -outline-offset-1 outline-gray-300 placeholder:text-gray-400 focus:outline-2 focus:-outline-offset-2 focus:outline-indigo-600 sm:text-sm/6"
                  />
                </div>
              </div>
              <button
                type="submit"
                className="flex w-full cursor-pointer justify-center rounded-md px-3 py-2 text-sm font-semibold text-white shadow-xs"
                style={{ backgroundColor: campaign.coresPrimaria }}
              >
                Continuar
              </button>
            </form>
          )}

          {/* Name + Phone form */}
          {state.phase === 'form' && (
            <form onSubmit={handleEnroll} className="space-y-4">
              <p className="text-center text-sm text-gray-500">
                Primeira vez? Preencha seus dados para participar.
              </p>
              <div>
                <label htmlFor="name" className="block text-sm/6 font-medium text-gray-900">
                  Seu nome
                </label>
                <div className="mt-2">
                  <input
                    id="name"
                    type="text"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    placeholder="Maria Silva"
                    required
                    className="block w-full rounded-md bg-white px-3 py-2 text-base text-gray-900 outline-1 -outline-offset-1 outline-gray-300 placeholder:text-gray-400 focus:outline-2 focus:-outline-offset-2 focus:outline-indigo-600 sm:text-sm/6"
                  />
                </div>
              </div>
              <div>
                <label htmlFor="phone2" className="block text-sm/6 font-medium text-gray-900">
                  Telefone
                </label>
                <div className="mt-2">
                  <input
                    id="phone2"
                    type="tel"
                    value={phone}
                    disabled
                    className="block w-full rounded-md bg-gray-100 px-3 py-2 text-base text-gray-500 sm:text-sm/6"
                  />
                </div>
              </div>
              <div className="flex gap-3">
                <button
                  type="button"
                  onClick={() => setState({ phase: 'phone' })}
                  className="flex flex-1 cursor-pointer justify-center rounded-md bg-white px-3 py-2 text-sm font-semibold text-gray-900 shadow-xs ring-1 ring-inset ring-gray-300 hover:bg-gray-50"
                >
                  Voltar
                </button>
                <button
                  type="submit"
                  className="flex flex-1 cursor-pointer justify-center rounded-md px-3 py-2 text-sm font-semibold text-white shadow-xs"
                  style={{ backgroundColor: campaign.coresPrimaria }}
                >
                  Participar
                </button>
              </div>
            </form>
          )}

          {/* Enrolling spinner */}
          {state.phase === 'enrolling' && (
            <div className="flex flex-col items-center py-8">
              <div className="size-8 animate-spin rounded-full border-4 border-gray-200 border-t-indigo-600" />
              <p className="mt-4 text-sm text-gray-500">Criando seu cartão...</p>
            </div>
          )}

          {/* Already enrolled — show progress */}
          {state.phase === 'progress' && (
            <div className="space-y-4">
              <div className="rounded-lg bg-white p-6 shadow-sm ring-1 ring-gray-900/5">
                <p className="text-center text-sm font-medium text-gray-900">Você já está participando!</p>
                <div className="mt-4">
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-600">Progresso</span>
                    <span className="font-medium text-gray-900">
                      {state.enrollment.checkinsAtuais} de {state.enrollment.checkinsNecessarios}
                    </span>
                  </div>
                  <div className="mt-2 h-3 w-full overflow-hidden rounded-full bg-gray-200">
                    <div
                      className="h-full rounded-full transition-all"
                      style={{
                        width: `${Math.round((state.enrollment.checkinsAtuais / state.enrollment.checkinsNecessarios) * 100)}%`,
                        backgroundColor: campaign.coresPrimaria,
                      }}
                    />
                  </div>
                  <p className="mt-2 text-center text-xs text-gray-400">
                    {state.enrollment.checkinsAtuais >= state.enrollment.checkinsNecessarios
                      ? 'Parabéns! Você completou a campanha!'
                      : `Faltam ${state.enrollment.checkinsNecessarios - state.enrollment.checkinsAtuais} check-in(s)`}
                  </p>
                </div>
              </div>
            </div>
          )}

          {/* Wallet buttons */}
          {state.phase === 'wallet' && (
            <div className="space-y-4">
              <div className="mx-auto flex size-14 items-center justify-center rounded-full bg-green-100">
                <svg className="size-7 text-green-600" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M4.5 12.75l6 6 9-13.5" />
                </svg>
              </div>
              <p className="text-center text-sm font-medium text-gray-900">
                Inscrição realizada! Adicione o cartão à sua carteira digital.
              </p>

              <div className="space-y-3">
                {/* Google Wallet */}
                {state.enrollment.googleWalletSaveUrl && (!isIOS || !state.enrollment.applePassUrl) && (
                  <button
                    type="button"
                    onClick={() => handleAddToGoogleWallet(state.enrollment.googleWalletSaveUrl!)}
                    className="flex w-full cursor-pointer items-center justify-center gap-2 rounded-md bg-black px-4 py-3 text-sm font-semibold text-white shadow-sm hover:bg-gray-800"
                  >
                    <svg className="size-5" viewBox="0 0 24 24" fill="currentColor">
                      <path d="M21.35 11.1h-9.18v2.73h5.51c-.24 1.27-.98 2.34-2.09 3.06v2.55h3.38c1.97-1.82 3.11-4.49 3.11-7.61 0-.52-.05-1.03-.14-1.52l-.59-.21z" />
                      <path d="M12.17 22c2.84 0 5.22-.94 6.96-2.56l-3.38-2.55c-.94.63-2.14 1-3.58 1-2.75 0-5.08-1.86-5.91-4.36H2.75v2.63C4.48 19.78 8.03 22 12.17 22z" />
                      <path d="M6.26 13.53c-.21-.63-.33-1.3-.33-2s.12-1.37.33-2V6.9H2.75C1.97 8.45 1.5 10.18 1.5 12s.47 3.55 1.25 5.1l3.51-2.57z" />
                      <path d="M12.17 5.64c1.55 0 2.94.53 4.03 1.58l3.02-3.02C17.36 2.18 14.98 1 12.17 1 8.03 1 4.48 3.22 2.75 6.9l3.51 2.63c.83-2.5 3.16-3.89 5.91-3.89z" />
                    </svg>
                    Adicionar ao Google Wallet
                  </button>
                )}

                {/* Apple Wallet */}
                {state.enrollment.applePassUrl && (!isAndroid || !state.enrollment.googleWalletSaveUrl) && (
                  <button
                    type="button"
                    onClick={() => handleAddToAppleWallet(state.enrollment.applePassUrl!)}
                    className="flex w-full cursor-pointer items-center justify-center gap-2 rounded-md bg-black px-4 py-3 text-sm font-semibold text-white shadow-sm hover:bg-gray-800"
                  >
                    <svg className="size-5" viewBox="0 0 24 24" fill="currentColor">
                      <path d="M18.71 19.5c-.83 1.24-1.71 2.45-3.05 2.47-1.34.03-1.77-.79-3.29-.79-1.53 0-2 .77-3.27.82-1.31.05-2.3-1.32-3.14-2.53C4.25 17 2.94 12.45 4.7 9.39c.87-1.52 2.43-2.48 4.12-2.51 1.28-.02 2.5.87 3.29.87.78 0 2.26-1.07 3.8-.91.65.03 2.47.26 3.64 1.98-.09.06-2.17 1.28-2.15 3.81.03 3.02 2.65 4.03 2.68 4.04-.03.07-.42 1.44-1.38 2.83M13 3.5c.73-.83 1.94-1.46 2.94-1.5.13 1.17-.34 2.35-1.04 3.19-.69.85-1.83 1.51-2.95 1.42-.15-1.15.41-2.35 1.05-3.11z" />
                    </svg>
                    Adicionar ao Apple Wallet
                  </button>
                )}

                {/* Desktop — sem preferência, mostrar ambos */}
                {!isIOS && !isAndroid && (
                  <>
                    {state.enrollment.googleWalletSaveUrl && state.enrollment.applePassUrl && (
                      <p className="text-center text-xs text-gray-400">
                        Escaneie este QR code no seu celular para a melhor experiência.
                      </p>
                    )}
                  </>
                )}

                {/* Nenhum wallet disponível */}
                {!state.enrollment.googleWalletSaveUrl && !state.enrollment.applePassUrl && (
                  <div className="rounded-lg bg-gray-50 p-4 text-center">
                    <p className="text-sm text-gray-500">
                      Cartão criado! As carteiras digitais estarão disponíveis em breve.
                    </p>
                  </div>
                )}
              </div>

              <button
                type="button"
                onClick={() => navigate(`/c/${campaignId}/success`)}
                className="mt-2 w-full cursor-pointer text-center text-sm text-gray-400 hover:text-gray-600"
              >
                Pular por agora
              </button>
            </div>
          )}
        </div>
      </div>
    </div>
  )
}

function isLightColor(hex: string): boolean {
  const h = hex.replace('#', '')
  if (h.length < 6) return true
  const r = parseInt(h.slice(0, 2), 16)
  const g = parseInt(h.slice(2, 4), 16)
  const b = parseInt(h.slice(4, 6), 16)
  return (0.299 * r + 0.587 * g + 0.114 * b) / 255 > 0.5
}
