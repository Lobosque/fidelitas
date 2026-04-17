import { useState, useRef, type FormEvent } from 'react'
import { Link, Navigate, useNavigate } from 'react-router-dom'
import { useAuth } from '../../contexts/AuthContext'
import { api, ApiError } from '../../lib/api'
import type { AuthUser } from '../../types'

const MOCK_AUTH = false

export function SignupPage() {
  const auth = useAuth()
  const navigate = useNavigate()
  const isRedirecting = useRef(false)

  const [nomeNegocio, setNomeNegocio] = useState('')
  const [email, setEmail] = useState('')
  const [senha, setSenha] = useState('')
  const [confirmarSenha, setConfirmarSenha] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  if (auth.user && !isRedirecting.current) {
    return <Navigate to="/dashboard" replace />
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault()
    setError('')

    if (senha !== confirmarSenha) {
      setError('As senhas não coincidem.')
      return
    }

    setLoading(true)

    try {
      const data = await api<{ token: string; user: AuthUser }>('/auth/signup', {
        method: 'POST',
        body: JSON.stringify({ nomeNegocio, email, senha }),
      })

      isRedirecting.current = true
      auth.login(data.token, data.user)
      navigate('/onboarding')
    } catch (err) {
      if (MOCK_AUTH) {
        const mockUser: AuthUser = {
          id: '1',
          nome: nomeNegocio,
          email,
          negocioId: '1',
        }
        isRedirecting.current = true
        auth.login('mock-token-123', mockUser)
        navigate('/onboarding')
        return
      }

      if (err instanceof ApiError) {
        setError(err.message)
      } else {
        setError('Não foi possível conectar ao servidor.')
      }
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="flex min-h-full">
      <div className="flex flex-1 flex-col justify-center px-4 py-12 sm:px-6 lg:flex-none lg:px-20 xl:px-24">
        <div className="mx-auto w-full max-w-sm lg:w-96">
          <div>
            <p className="text-3xl font-bold text-primary">Voltei</p>
            <h2 className="mt-8 text-2xl/9 font-bold tracking-tight text-gray-900">
              Crie sua conta
            </h2>
            <p className="mt-2 text-sm/6 text-gray-500">
              Já tem conta?{' '}
              <Link to="/login" className="font-semibold text-primary hover:text-secondary">
                Entrar
              </Link>
            </p>
          </div>

          <div className="mt-10">
            {error && (
              <p className="mb-4 text-sm text-red-600">{error}</p>
            )}

            <form onSubmit={handleSubmit} className="space-y-6">
              <div>
                <label htmlFor="nome-negocio" className="block text-sm/6 font-medium text-gray-900">
                  Nome do negócio
                </label>
                <div className="mt-2">
                  <input
                    id="nome-negocio"
                    name="nome-negocio"
                    type="text"
                    required
                    value={nomeNegocio}
                    onChange={(e) => setNomeNegocio(e.target.value)}
                    className="block w-full rounded-md bg-white px-3 py-1.5 text-base text-gray-900 outline-1 -outline-offset-1 outline-gray-300 placeholder:text-gray-400 focus:outline-2 focus:-outline-offset-2 focus:outline-primary sm:text-sm/6"
                  />
                </div>
              </div>

              <div>
                <label htmlFor="email" className="block text-sm/6 font-medium text-gray-900">
                  Email
                </label>
                <div className="mt-2">
                  <input
                    id="email"
                    name="email"
                    type="email"
                    required
                    autoComplete="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    className="block w-full rounded-md bg-white px-3 py-1.5 text-base text-gray-900 outline-1 -outline-offset-1 outline-gray-300 placeholder:text-gray-400 focus:outline-2 focus:-outline-offset-2 focus:outline-primary sm:text-sm/6"
                  />
                </div>
              </div>

              <div>
                <label htmlFor="senha" className="block text-sm/6 font-medium text-gray-900">
                  Senha
                </label>
                <div className="mt-2">
                  <input
                    id="senha"
                    name="senha"
                    type="password"
                    required
                    autoComplete="new-password"
                    value={senha}
                    onChange={(e) => setSenha(e.target.value)}
                    className="block w-full rounded-md bg-white px-3 py-1.5 text-base text-gray-900 outline-1 -outline-offset-1 outline-gray-300 placeholder:text-gray-400 focus:outline-2 focus:-outline-offset-2 focus:outline-primary sm:text-sm/6"
                  />
                </div>
              </div>

              <div>
                <label htmlFor="confirmar-senha" className="block text-sm/6 font-medium text-gray-900">
                  Confirmar senha
                </label>
                <div className="mt-2">
                  <input
                    id="confirmar-senha"
                    name="confirmar-senha"
                    type="password"
                    required
                    autoComplete="new-password"
                    value={confirmarSenha}
                    onChange={(e) => setConfirmarSenha(e.target.value)}
                    className="block w-full rounded-md bg-white px-3 py-1.5 text-base text-gray-900 outline-1 -outline-offset-1 outline-gray-300 placeholder:text-gray-400 focus:outline-2 focus:-outline-offset-2 focus:outline-primary sm:text-sm/6"
                  />
                </div>
              </div>

              <div>
                <button
                  type="submit"
                  disabled={loading}
                  className="flex w-full cursor-pointer justify-center rounded-md bg-primary px-3 py-1.5 text-sm/6 font-semibold text-white shadow-xs hover:bg-secondary focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-primary disabled:opacity-50"
                >
                  {loading ? 'Criando...' : 'Criar conta'}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
      <div className="relative hidden w-0 flex-1 lg:block">
        <img
          alt=""
          src="/image.jpg"
          className="absolute inset-0 size-full object-cover"
        />
      </div>
    </div>
  )
}
