const defaultConfig = require('@vc-shell/framework/tailwind.config')

const theme = defaultConfig.theme;
theme.extend.margin = { '15px': '15px' };
theme.extend.colors = { '#80b4e3': '#80b4e3', '#87b563': '#87b563', '#eb4f4d': '#eb4f4d' };

module.exports = {
    prefix: 'tw-',
    content: ["../../node_modules/@vc-shell/**/*.{vue,js,ts,jsx,tsx}", "./src/**/*.{vue,js,ts,jsx,tsx}"],
    theme: theme,

    plugins: [require("@tailwindcss/line-clamp")],
};
