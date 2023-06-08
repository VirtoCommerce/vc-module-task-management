import VirtoShellFramework, { notification } from "@vc-shell/framework";
import { createApp } from "vue";
import TaskModule from "./modules/tasks";
import { router } from "./router";
import * as locales from "./locales";
import { RouterView } from "vue-router";

// Load required CSS
import "./styles/index.scss";
import "@fortawesome/fontawesome-free/css/all.min.css";
import "roboto-fontface/css/roboto/roboto-fontface.css";
import "@vc-shell/framework/dist/index.css";

const app = createApp(RouterView)
  .use(VirtoShellFramework)
  .use(TaskModule, { router })
  .use(router);

Object.entries(locales).forEach(([key, message]) => {
  app.config.globalProperties.$mergeLocaleMessage(key, message);
});

app.config.errorHandler = (err) => {
  notification.error(err.toString(), {
    timeout: 5000,
  });
};

app.mount("#app");
