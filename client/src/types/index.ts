export interface Business {
  id: string
  nome: string
  email: string
  logoUrl: string | null
  cores: {
    primaria: string
    secundaria: string
  }
  plano: 'gratis' | 'profissional' | 'negocio'
  criadoEm: string
}

export interface Campaign {
  id: string
  negocioId: string
  nome: string
  descricao: string
  checkinsNecessarios: number
  ativa: boolean
  criadaEm: string
  walletClassId: string | null
}

export interface Customer {
  id: string
  nome: string
  telefone: string
  email: string | null
  criadoEm: string
}

export interface Enrollment {
  id: string
  campanhaId: string
  clienteId: string
  checkinsAtuais: number
  resgatou: boolean
  walletObjectId: string | null
  criadaEm: string
}

export interface CheckinLog {
  id: string
  participacaoId: string
  registradoPor: string
  criadoEm: string
}

export interface AuthUser {
  id: string
  nome: string
  email: string
  negocioId: string
}

export interface CampaignPublicInfo {
  id: string
  nome: string
  descricao: string
  checkinsNecessarios: number
  negocioNome: string
  logoUrl: string | null
  coresPrimaria: string
  coresSecundaria: string
}

export interface EnrollResponse {
  enrollmentId: string
  googleWalletSaveUrl: string | null
  applePassUrl: string | null
  checkinsAtuais: number
  checkinsNecessarios: number
  alreadyEnrolled: boolean
}

export interface CheckinResponse {
  enrollmentId: string
  clienteNome: string
  campanhaNome: string
  campanhaDescricao: string
  checkinsAtuais: number
  checkinsNecessarios: number
  rewardReached: boolean
}
