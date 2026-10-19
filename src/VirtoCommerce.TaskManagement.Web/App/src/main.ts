import VirtoShellFramework, { notification, useUser, useLanguages } from "@vc-shell/framework";
import { createApp, watch } from "vue";
import { router } from "./router";
import * as locales from "./locales";
import * as frameworkLocales from "./locales/framework";
import { translateFrameworkDefaults } from "./locales/framework/defaults";
import { syncDocumentDirection } from "./locales/direction";
import { RouterView } from "vue-router";
import * as modules from "./modules";
import { bootstrap } from "./bootstrap";

// Load required CSS
import "@vc-shell/framework/dist/index.css";

async function startApp() {
  const { loadUser } = useUser();
  await loadUser();

  const app = createApp(RouterView).use(VirtoShellFramework, {
    router,
    i18n: {
      locale: import.meta.env.APP_I18N_LOCALE,
      fallbackLocale: import.meta.env.APP_I18N_FALLBACK_LOCALE,
    },
  });

  Object.values(modules.default).forEach((module) => {
    app.use(module.default);
  });

  app.use(router);

  bootstrap(app);

  // vc-shell's texts first, then the app's own (a key in both is the app's)
  [frameworkLocales, locales].forEach((source) =>
    Object.entries(source).forEach(([key, message]) => {
      app.config.globalProperties.$mergeLocaleMessage(key, message);
    }),
  );

  translateFrameworkDefaults(app);

  // after the app's languages are merged: before, the saved language (shared with the Admin Shell) isn't one of the
  // known ones yet, so the app would fall back to English and save that over it
  const { currentLocale, setLocale } = useLanguages();
  setLocale(currentLocale.value);
  // embedded in the Admin Shell: a language picked there reaches this frame through the shared saved setting
  watch(currentLocale, (locale) => setLocale(locale));
  syncDocumentDirection(currentLocale);

  app.config.errorHandler = (err) => {
    notification.error((err as Error).toString(), {
      timeout: 5000,
    });
  };

  await router.isReady();

  app.mount("#app");
}

startApp();
