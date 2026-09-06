/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "./src/**/*.{html,ts}",
    ],
    theme: {
        extend: {
            colors: {
                primary: {
                    DEFAULT: 'hsl(var(--primary-raw) / <alpha-value>)',
                    soft: 'var(--primary-soft)',
                    glow: 'var(--primary-glow)',
                },
                hospital: {
                    50: '#f0f9ff',
                    100: '#e0f2fe',
                    200: '#bae6fd',
                    300: '#7dd3fc',
                    400: '#38bdf8',
                    500: '#0ea5e9', // Base calmante para salud
                    600: '#0284c7',
                    700: '#0369a1',
                    800: '#075985',
                    900: '#0c4a6e',
                },
                emergency: {
                    light: '#fca5a5',
                    DEFAULT: '#ef4444',
                    dark: '#b91c1c'
                },
                surface: {
                    DEFAULT: 'hsl(var(--surface-raw) / <alpha-value>)',
                    card: 'hsl(var(--surface-card-raw) / <alpha-value>)',
                    light: 'hsl(var(--surface-light-raw) / <alpha-value>)',
                    glass: 'var(--glass)'
                },
                slate: {
                    950: '#020617',
                    900: '#0f172a',
                    800: '#1e293b'
                }
            },
            fontFamily: {
                sans: ['Inter', 'sans-serif'],
            },
            keyframes: {
                fadeIn: {
                    '0%': { opacity: '0', transform: 'translateY(-10px)' },
                    '100%': { opacity: '1', transform: 'translateY(0)' },
                },
                scaleIn: {
                    '0%': { opacity: '0', transform: 'scale(0.95)' },
                    '100%': { opacity: '1', transform: 'scale(1)' },
                },
                slideUp: {
                    '0%': { transform: 'translateY(100%)' },
                    '100%': { transform: 'translateY(0)' },
                }
            },
            animation: {
                'fade-in': 'fadeIn 0.5s ease-out forwards',
                'scale-in': 'scaleIn 0.3s cubic-bezier(0.16, 1, 0.3, 1) forwards',
                'slide-up': 'slideUp 0.4s ease-out',
                'fade-in-slow': 'fadeIn 0.8s ease-out forwards',
            }
        },
    },
    plugins: [],
}
