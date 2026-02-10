import { createContext, useContext, useEffect, type ReactNode } from 'react'

interface ThemeColors {
  primaria: string
  secundaria: string
}

const DEFAULT_COLORS: ThemeColors = {
  primaria: '#4f46e5',
  secundaria: '#818cf8',
}

const ThemeContext = createContext<ThemeColors>(DEFAULT_COLORS)

export function ThemeProvider({
  colors,
  children,
}: {
  colors?: ThemeColors
  children: ReactNode
}) {
  const theme = colors ?? DEFAULT_COLORS

  useEffect(() => {
    const root = document.documentElement
    root.style.setProperty('--color-primary', theme.primaria)
    root.style.setProperty('--color-secondary', theme.secundaria)
  }, [theme.primaria, theme.secundaria])

  return (
    <ThemeContext.Provider value={theme}>
      {children}
    </ThemeContext.Provider>
  )
}

export function useTheme() {
  return useContext(ThemeContext)
}
