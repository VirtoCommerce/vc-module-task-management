import { OverridesSchema } from "@vc-shell/framework";

export const overrides: OverridesSchema = {
  upsert: [
    {
      id: "ActiveTasks",
      path: "settings.url",
      value: "/my-archive",
    },
    {
      id: "ActiveTasks",
      path: "settings.id",
      value: "MyArchiveTasks",
    },
    {
      id: "ActiveTasks",
      path: "settings.titleTemplate",
      value: "TASKS.PAGES.LIST.MY_ARCHIVE_TITLE",
    },
    {
      id: "ActiveTasks",
      path: "settings.menuItem.title",
      value: "TASKS.MENU.MY_ARCHIVE_TITLE",
    },
    {
      id: "ActiveTasks",
      path: "settings.composable",
      value: "useMyArchiveWorkTasks",
    },
  ],
  remove: [
    {
      id: "ActiveTasks",
      path: "settings.toolbar.create",
    },
  ],
};
