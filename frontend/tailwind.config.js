/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          DEFAULT: '#7E65AA',
          light: '#9E88C7',
          dark: '#5C4685',
        },
        secondary: {
          DEFAULT: '#E83B83',
          light: '#F865A1',
          dark: '#C22064',
        }
      }
    },
  },
  plugins: [],
}
