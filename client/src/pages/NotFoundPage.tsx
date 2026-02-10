import { Link } from 'react-router-dom'

export function NotFoundPage() {
  return (
    <div className="flex min-h-screen items-center justify-center bg-gray-50 px-4">
      <div className="text-center">
        <p className="text-4xl font-bold text-primary">404</p>
        <h1 className="mt-2 text-2xl font-bold text-gray-900">Página não encontrada</h1>
        <p className="mt-2 text-sm text-gray-500">
          A página que você procura não existe.
        </p>
        <Link
          to="/"
          className="mt-4 inline-block text-sm font-medium text-primary hover:text-secondary"
        >
          Voltar ao início
        </Link>
      </div>
    </div>
  )
}
