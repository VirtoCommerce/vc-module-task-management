import defaultConfig, { content } from "@vc-shell/framework/tailwind.config";

export default {
  prefix: "tw-",
  content: [...content, "./src/**/*.{vue,js,ts,jsx,tsx}"],
  theme: {
    ...defaultConfig?.theme,
    fontFamily: {
      // You can now use both tw-font-sans and tw-font-roboto
      ...defaultConfig?.theme?.fontFamily,
      sans: ["Roboto", "ui-sans-serif", "system-ui", "sans-serif"], // tw-font-sans
      roboto: ["Roboto", "sans-serif"], // tw-font-roboto
    },
  },
};