import { OverridesSchema } from "@vc-shell/framework";

export const overrides: OverridesSchema = {
  upsert: [
    {
      id: "ActiveTasks",
      path: "settings.url",
      value: "/my",
    },
    {
      id: "ActiveTasks",
      path: "settings.id",
      value: "MyTasks",
    },
    {
      id: "ActiveTasks",
      path: "settings.titleTemplate",
      value: "MY_TASKS.PAGES.LIST.TITLE",
    },
    {
      id: "ActiveTasks",
      path: "settings.menuItem.title",
      value: "MY_TASKS.MENU.TITLE",
    },
  ],
};
