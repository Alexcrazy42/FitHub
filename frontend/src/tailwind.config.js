/** @type {import('tailwindcss').Config} */
module.exports = {
  darkMode: 'class', // ← важно!
  content: [
    "./index.html",
    "./src/**/*.{js,jsx,ts,tsx}",
  ],
  theme: {
    extend: {},
  },
  plugins: [],
}