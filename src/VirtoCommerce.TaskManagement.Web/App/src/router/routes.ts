import { RouteRecordRaw } from "vue-router";
import App from "../pages/App.vue";
import { ForgotPassword, Invite, Login, ResetPassword } from "@vc-shell/framework";
import whiteLogoImage from "/assets/logo-white.svg";

const version = import.meta.env.PACKAGE_VERSION;

export const routes: RouteRecordRaw[] = [
  {
    path: "/",
    component: App,
    name: "App",
    meta: {
      root: true,
    },
    children: [],
    redirect: (to) => {
      if (to.name === "App") {
        return { path: "/active", params: to.params };
      }
      return to.path;
    },
  },
  {
    name: "Login",
    path: "/login",
    component: Login,
    meta: {
      appVersion: version,
    },
    props: () => ({
      logo: whiteLogoImage,
      title: "Task Manager",
    }),
  },
  {
    name: "ForgotPassword",
    path: "/forgot-password",
    component: ForgotPassword,
    meta: {
      appVersion: version,
    },
    props: () => ({
      logo: whiteLogoImage,
    }),
  },
  {
    name: "Invite",
    path: "/invite",
    component: Invite,
    props: (route) => ({
      userId: route.query.userId,
      token: route.query.token,
      userName: route.query.userName,
    }),
  },
  {
    name: "ResetPassword",
    path: "/resetpassword",
    component: ResetPassword,
    props: (route) => ({
      userId: route.query.userId,
      token: route.query.token,
      userName: route.query.userName,
    }),
  },
];
