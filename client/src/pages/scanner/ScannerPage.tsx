import { useState, useEffect, useRef, type FormEvent } from 'react'
import { Html5Qrcode } from 'html5-qrcode'
import { api, ApiError } from '../../lib/api'
import type { CheckinResponse } from '../../types'

type ScannerState =
  | { phase: 'scanning' }
  | { phase: 'looking-up' }
  | { phase: 'confirming'; token: string; preview: CheckinPreview }
  | { phase: 'loading' }
  | { phase: 'success'; checkin: CheckinResponse }
  | { phase: 'error'; message: string }

interface CheckinPreview {
  clienteNome: string
  campanhaNome: string
  campanhaDescricao: string
  checkinsAtuais: number
  checkinsNecessarios: number
}

export function ScannerPage() {
  const [state, setState] = useState<ScannerState>({ phase: 'scanning' })
  const [mode, setMode] = useState<'camera' | 'manual'>('camera')
  const [manualCode, setManualCode] = useState('')
  const [cameraError, setCameraError] = useState('')
  const scannerRef = useRef<Html5Qrcode | null>(null)
  const scannerContainerId = 'qr-reader'

  useEffect(() => {
    if (state.phase !== 'scanning' || mode !== 'camera') return

    let cancelled = false
    let isRunning = false
    const scanner = new Html5Qrcode(scannerContainerId)
    scannerRef.current = scanner

    scanner
      .start(
        { facingMode: 'environment' },
        { fps: 10, qrbox: { width: 250, height: 250 } },
        (decodedText) => {
          if (cancelled) return
          cancelled = true
          isRunning = false
          scanner.stop().catch(() => {})
          handleCodeScanned(decodedText)
        },
        () => {},
      )
      .then(() => {
        isRunning = true
        if (cancelled) {
          scanner.stop().catch(() => {})
          isRunning = false
        }
      })
      .catch((err) => {
        if (!cancelled) {
          setCameraError(
            typeof err === 'string' ? err : 'Não foi possível acessar a câmera.',
          )
        }
      })

    return () => {
      cancelled = true
      if (isRunning) {
        scanner.stop().catch(() => {})
        isRunning = false
      }
      scannerRef.current = null
    }
  }, [state.phase, mode])

  function extractToken(code: string): string {
    // Se for URL, extrair a última parte do path
    try {
      const url = new URL(code)
      const parts = url.pathname.split('/').filter(Boolean)
      return parts[parts.length - 1] || code
    } catch {
      return code
    }
  }

  async function handleCodeScanned(code: string) {
    const token = extractToken(code)
    setState({ phase: 'looking-up' })

    try {
      // Fazer o check-in diretamente para obter os dados do cliente
      // O fluxo é: scan → mostrar dados → confirmar
      // Mas a API faz tudo em um POST, então vamos usar uma abordagem:
      // primeiro fazemos o checkin, depois mostramos o resultado
      const checkin = await api<CheckinResponse>('/checkins', {
        method: 'POST',
        body: JSON.stringify({ enrollmentToken: token }),
      })
      setState({ phase: 'success', checkin })
    } catch (err) {
      const message = err instanceof ApiError ? err.message : 'Erro ao registrar check-in.'
      setState({ phase: 'error', message })
    }
  }

  function handleManualSubmit(e: FormEvent) {
    e.preventDefault()
    if (!manualCode.trim()) return
    handleCodeScanned(manualCode.trim())
    setManualCode('')
  }

  async function handleRedeem(enrollmentId: string) {
    try {
      await api(`/checkins/${enrollmentId}/redeem`, { method: 'POST' })
      setState((prev) => {
        if (prev.phase === 'success') {
          return { ...prev, checkin: { ...prev.checkin, rewardReached: false } }
        }
        return prev
      })
    } catch (err) {
      // silently handle — button will just not do anything
    }
  }

  function handleReset() {
    setCameraError('')
    setManualCode('')
    setState({ phase: 'scanning' })
  }

  // --- Scanning phase ---
  if (state.phase === 'scanning') {
    return (
      <div className="mx-auto max-w-lg space-y-6">
        <div className="text-center">
          <h1 className="text-2xl font-bold text-gray-900">Scanner</h1>
          <p className="mt-1 text-sm text-gray-500">
            Escaneie o QR code do cartão de fidelidade do cliente.
          </p>
        </div>

        {/* Mode toggle */}
        <div className="flex justify-center">
          <div className="inline-flex rounded-lg bg-gray-100 p-1">
            <button
              type="button"
              onClick={() => { setMode('camera'); setCameraError('') }}
              className={`cursor-pointer rounded-md px-4 py-2 text-sm font-medium ${
                mode === 'camera'
                  ? 'bg-white text-gray-900 shadow-sm'
                  : 'text-gray-500 hover:text-gray-700'
              }`}
            >
              Câmera
            </button>
            <button
              type="button"
              onClick={() => setMode('manual')}
              className={`cursor-pointer rounded-md px-4 py-2 text-sm font-medium ${
                mode === 'manual'
                  ? 'bg-white text-gray-900 shadow-sm'
                  : 'text-gray-500 hover:text-gray-700'
              }`}
            >
              Código manual
            </button>
          </div>
        </div>

        {mode === 'camera' ? (
          <div>
            {cameraError ? (
              <div className="rounded-lg bg-red-50 p-4 text-center">
                <p className="text-sm text-red-600">{cameraError}</p>
                <button
                  type="button"
                  onClick={() => setMode('manual')}
                  className="mt-3 cursor-pointer text-sm font-semibold text-primary hover:text-secondary"
                >
                  Usar código manual
                </button>
              </div>
            ) : (
              <div className="overflow-hidden rounded-lg">
                <div id={scannerContainerId} />
              </div>
            )}
          </div>
        ) : (
          <form onSubmit={handleManualSubmit} className="space-y-4">
            <div>
              <label htmlFor="manual-code" className="block text-sm/6 font-medium text-gray-900">
                Código ou URL do QR
              </label>
              <div className="mt-2">
                <input
                  id="manual-code"
                  type="text"
                  value={manualCode}
                  onChange={(e) => setManualCode(e.target.value)}
                  placeholder="Cole o código aqui"
                  className="block w-full rounded-md bg-white px-3 py-1.5 text-base text-gray-900 outline-1 -outline-offset-1 outline-gray-300 placeholder:text-gray-400 focus:outline-2 focus:-outline-offset-2 focus:outline-primary sm:text-sm/6"
                />
              </div>
            </div>
            <button
              type="submit"
              className="flex w-full cursor-pointer justify-center rounded-md bg-primary px-3 py-1.5 text-sm/6 font-semibold text-white shadow-xs hover:bg-secondary focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-primary"
            >
              Registrar check-in
            </button>
          </form>
        )}
      </div>
    )
  }

  // --- Looking up phase ---
  if (state.phase === 'looking-up' || state.phase === 'loading') {
    return (
      <div className="flex flex-col items-center justify-center py-16">
        <div className="size-8 animate-spin rounded-full border-4 border-gray-200 border-t-primary" />
        <p className="mt-4 text-sm text-gray-500">Registrando check-in...</p>
      </div>
    )
  }

  // --- Error phase ---
  if (state.phase === 'error') {
    return (
      <div className="mx-auto max-w-lg space-y-6 text-center">
        <div className="mx-auto flex size-16 items-center justify-center rounded-full bg-red-100">
          <svg className="size-8 text-red-600" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" d="M12 9v3.75m9-.75a9 9 0 1 1-18 0 9 9 0 0 1 18 0Zm-9 3.75h.008v.008H12v-.008Z" />
          </svg>
        </div>
        <div>
          <h2 className="text-xl font-bold text-gray-900">Erro</h2>
          <p className="mt-2 text-sm text-gray-500">{state.message}</p>
        </div>
        <button
          type="button"
          onClick={handleReset}
          className="cursor-pointer rounded-md bg-primary px-6 py-2 text-sm font-semibold text-white shadow-xs hover:bg-secondary"
        >
          Tentar novamente
        </button>
      </div>
    )
  }

  // --- Success phase ---
  const { checkin } = state

  return (
    <div className="mx-auto max-w-lg space-y-6 text-center">
      {checkin.rewardReached ? (
        <>
          <div className="mx-auto flex size-16 items-center justify-center rounded-full bg-yellow-100">
            <svg className="size-8 text-yellow-600" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" d="M16.5 18.75h-9m9 0a3 3 0 0 1 3 3h-15a3 3 0 0 1 3-3m9 0v-3.375c0-.621-.503-1.125-1.125-1.125h-.871M7.5 18.75v-3.375c0-.621.504-1.125 1.125-1.125h.872m5.007 0H9.497m5.007 0a7.454 7.454 0 0 1-.982-3.172M9.497 14.25a7.454 7.454 0 0 0 .981-3.172M5.25 4.236c-.982.143-1.954.317-2.916.52A6.003 6.003 0 0 0 7.73 9.728M5.25 4.236V4.5c0 2.108.966 3.99 2.48 5.228M5.25 4.236V2.721C7.456 2.41 9.71 2.25 12 2.25c2.291 0 4.545.16 6.75.47v1.516M18.75 4.236c.982.143 1.954.317 2.916.52A6.003 6.003 0 0 0 16.27 9.728M18.75 4.236V4.5c0 2.108-.966 3.99-2.48 5.228M18.75 4.236V2.721M16.27 9.728l-.351.073m0 0A7.44 7.44 0 0 1 12 11.25a7.44 7.44 0 0 1-3.919-1.449m0 0-.351-.073" />
            </svg>
          </div>
          <div>
            <h2 className="text-xl font-bold text-gray-900">
              {checkin.clienteNome} completou a campanha!
            </h2>
            <p className="mt-2 text-sm text-gray-500">
              Prêmio: <span className="font-semibold">{checkin.campanhaDescricao}</span>
            </p>
          </div>
          <button
            type="button"
            onClick={() => handleRedeem(checkin.enrollmentId)}
            className="cursor-pointer rounded-md bg-yellow-500 px-6 py-2 text-sm font-semibold text-white shadow-xs hover:bg-yellow-600"
          >
            Marcar como resgatado
          </button>
        </>
      ) : (
        <>
          <div className="mx-auto flex size-16 items-center justify-center rounded-full bg-green-100">
            <svg className="size-8 text-green-600" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" d="M4.5 12.75l6 6 9-13.5" />
            </svg>
          </div>
          <div>
            <h2 className="text-xl font-bold text-gray-900">Check-in registrado!</h2>
            <p className="mt-2 text-sm text-gray-500">
              {checkin.clienteNome} agora tem{' '}
              <span className="font-semibold">
                {checkin.checkinsAtuais} de {checkin.checkinsNecessarios}
              </span>{' '}
              check-ins em <span className="font-semibold">{checkin.campanhaNome}</span>.
            </p>
          </div>
        </>
      )}

      <button
        type="button"
        onClick={handleReset}
        className="cursor-pointer rounded-md bg-primary px-6 py-2 text-sm font-semibold text-white shadow-xs hover:bg-secondary focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-primary"
      >
        Escanear outro
      </button>
    </div>
  )
}
