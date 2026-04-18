import { useState, type FormEvent } from 'react'
import { Link, Navigate, useNavigate } from 'react-router-dom'
import { useAuth } from '../../contexts/AuthContext'
import { api, ApiError } from '../../lib/api'
import type { AuthUser } from '../../types'

const MOCK_AUTH = false

const IMAGE_LQIP =
  'data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/2wBDABQODxIPDRQSEBIXFRQYHjIhHhwcHj0sLiQySUBMS0dARkVQWnNiUFVtVkVGZIhlbXd7gYKBTmCNl4x9lnN+gXz/2wBDARUXFx4aHjshITt8U0ZTfHx8fHx8fHx8fHx8fHx8fHx8fHx8fHx8fHx8fHx8fHx8fHx8fHx8fHx8fHx8fHx8fHz/wAARCAAPABQDASIAAhEBAxEB/8QAFwABAQEBAAAAAAAAAAAAAAAAAAQDBf/EABgQAAMBAQAAAAAAAAAAAAAAAAACAyEx/8QAFQEBAQAAAAAAAAAAAAAAAAAAAQL/xAAWEQEBAQAAAAAAAAAAAAAAAAAAARH/2gAMAwEAAhEDEQA/AOjapDWppdukNWJpg1dBKzaA0v/Z'

export function LoginPage() {
  const auth = useAuth()
  const navigate = useNavigate()

  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [rememberMe, setRememberMe] = useState(false)
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const [imageLoaded, setImageLoaded] = useState(false)

  if (auth.user) {
    return <Navigate to="/dashboard" replace />
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault()
    setError('')
    setLoading(true)

    try {
      const data = await api<{ token: string; user: AuthUser }>('/auth/login', {
        method: 'POST',
        body: JSON.stringify({ email, password }),
      })

      auth.login(data.token, data.user)
      navigate('/dashboard')
    } catch (err) {
      if (MOCK_AUTH) {
        const mockUser: AuthUser = {
          id: '1',
          nome: 'Usuário Teste',
          email,
          negocioId: '1',
        }
        auth.login('mock-token-123', mockUser)
        navigate('/dashboard')
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
              Entre na sua conta
            </h2>
            <p className="mt-2 text-sm/6 text-gray-500">
              Não tem conta?{' '}
              <Link to="/signup" className="font-semibold text-primary hover:text-secondary">
                Criar conta grátis
              </Link>
            </p>
          </div>

          <div className="mt-10">
            {error && (
              <p className="mb-4 text-sm text-red-600">{error}</p>
            )}

            <form onSubmit={handleSubmit} className="space-y-6">
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
                <label htmlFor="password" className="block text-sm/6 font-medium text-gray-900">
                  Senha
                </label>
                <div className="mt-2">
                  <input
                    id="password"
                    name="password"
                    type="password"
                    required
                    autoComplete="current-password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    className="block w-full rounded-md bg-white px-3 py-1.5 text-base text-gray-900 outline-1 -outline-offset-1 outline-gray-300 placeholder:text-gray-400 focus:outline-2 focus:-outline-offset-2 focus:outline-primary sm:text-sm/6"
                  />
                </div>
              </div>

              <div className="flex items-center justify-between">
                <div className="flex gap-3">
                  <div className="flex h-6 shrink-0 items-center">
                    <div className="group grid size-4 grid-cols-1">
                      <input
                        id="remember-me"
                        name="remember-me"
                        type="checkbox"
                        checked={rememberMe}
                        onChange={(e) => setRememberMe(e.target.checked)}
                        className="col-start-1 row-start-1 appearance-none rounded-sm border border-gray-300 bg-white checked:border-primary checked:bg-primary focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-primary disabled:border-gray-300 disabled:bg-gray-100 disabled:checked:bg-gray-100 forced-colors:appearance-auto"
                      />
                      <svg
                        fill="none"
                        viewBox="0 0 14 14"
                        className="pointer-events-none col-start-1 row-start-1 size-3.5 self-center justify-self-center stroke-white group-has-disabled:stroke-gray-950/25"
                      >
                        <path
                          d="M3 8L6 11L11 3.5"
                          strokeWidth={2}
                          strokeLinecap="round"
                          strokeLinejoin="round"
                          className="opacity-0 group-has-checked:opacity-100"
                        />
                      </svg>
                    </div>
                  </div>
                  <label htmlFor="remember-me" className="block text-sm/6 text-gray-900">
                    Lembrar-me
                  </label>
                </div>

                <div className="text-sm/6">
                  <Link to="/forgot-password" className="font-semibold text-primary hover:text-secondary">
                    Esqueceu a senha?
                  </Link>
                </div>
              </div>

              <div>
                <button
                  type="submit"
                  disabled={loading}
                  className="flex w-full cursor-pointer justify-center rounded-md bg-primary px-3 py-1.5 text-sm/6 font-semibold text-white shadow-xs hover:bg-secondary focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-primary disabled:opacity-50"
                >
                  {loading ? 'Entrando...' : 'Entrar'}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
      <div className="relative hidden w-0 flex-1 overflow-hidden lg:block">
        {/* LQIP: blur placeholder (inline base64, loads instantly) */}
        <img
          alt=""
          src={IMAGE_LQIP}
          aria-hidden="true"
          className="absolute inset-0 size-full scale-110 object-cover blur-xl"
        />
        {/* Full-res image with srcset */}
        <img
          alt=""
          src="/image-1280.jpg"
          srcSet="/image-1280.jpg 1280w, /image-1920.jpg 1920w"
          sizes="50vw"
          fetchPriority="high"
          onLoad={() => setImageLoaded(true)}
          className={`absolute inset-0 size-full object-cover transition-opacity duration-500 ${
            imageLoaded ? 'opacity-100' : 'opacity-0'
          }`}
        />
      </div>
    </div>
  )
}
