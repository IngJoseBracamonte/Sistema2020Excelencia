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
                }
            },
            fontFamily: {
                sans: ['Inter', 'sans-serif'],
            }
        },
    },
    plugins: [],
}
