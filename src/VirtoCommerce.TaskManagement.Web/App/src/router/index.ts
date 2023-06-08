import { createRouter, createWebHashHistory, RouteRecordRaw } from "vue-router";
import {
  usePermissions,
  useUser,
  Invite,
  ResetPassword,
  Login,
  useBladeNavigation,
  BladePageComponent,
  notification,
} from "@vc-shell/framework";
import { useCookies } from "@vueuse/integrations/useCookies";

/**
 * Pages
 */
import App from "./../pages/App.vue";

// eslint-disable-next-line import/no-unresolved
import whiteLogoImage from "/assets/logo-white.svg";
// eslint-disable-next-line import/no-unresolved
import bgImage from "/assets/background.jpg";
import { inject } from "vue";

const routes: RouteRecordRaw[] = [
  {
    path: "/",
    component: App,
    name: "App",
    meta: {
      root: true,
    },
    children: [],
  },
  {
    path: "/login",
    name: "Login",
    component: Login,
    meta: {
      logo: whiteLogoImage,
      background: bgImage,
      title: "Tasks Portal",
    },
  },
  {
    name: "invite",
    path: "/invite",
    component: Invite,
    props: (route) => ({
      userId: route.query.userId,
      token: route.query.token,
      userName: route.query.userName,
    }),
  },
  {
    name: "resetpassword",
    path: "/resetpassword",
    component: ResetPassword,
    props: (route) => ({
      userId: route.query.userId,
      token: route.query.token,
      userName: route.query.userName,
    }),
  },
  {
    path: "/:pathMatch(.*)*",
    component: App,
    beforeEnter: (to) => {
      const { resolveUnknownRoutes } = useBladeNavigation();

      return resolveUnknownRoutes(to);
    },
  },
];

export const router = createRouter({
  history: createWebHashHistory(import.meta.env.APP_BASE_PATH as string),
  routes,
});

router.beforeEach(async (to, from) => {
  const { fetchUserPermissions, checkPermission } = usePermissions();
  const { resolveBlades } = useBladeNavigation();
  const { getAccessToken } = useUser();

  const pages = inject<BladePageComponent[]>("pages");

  const resolvedBladeUrl = resolveBlades(to);

  try {
    const token = await getAccessToken();
    const azureAdCookie = useCookies([".AspNetCore.Identity.Application"]).get(
      ".AspNetCore.Identity.Application"
    );

    // Fetching permissions if not any
    if (token) await fetchUserPermissions();

    const component = pages.find((blade) => blade?.url === to.path);

    const hasAccess = checkPermission(component?.permissions);

    if (!(token || azureAdCookie) && to.name !== "Login") {
      return { name: "Login" };
    } else if (hasAccess && !to.redirectedFrom) {
      if (to.name === "App" && from.path !== "/my") {
        return "/my";
      }
      return resolvedBladeUrl ? resolvedBladeUrl : true;
    } else if (!hasAccess) {
      if (to.path === "/my" && to.redirectedFrom) {
        return { name: "App" };
      }

      notification.error("Access restricted", {
        timeout: 3000,
      });

      return from.path;
    }
  } catch (e) {
    throw new Error(e.message);
  }
});
