import { OverridesSchema } from "@vc-shell/framework";

export const overrides: OverridesSchema = {
  upsert: [
    {
      id: "ActiveTasks",
      path: "settings.url",
      value: "/archive",
    },
    {
      id: "ActiveTasks",
      path: "settings.id",
      value: "ArchiveTasks",
    },
    {
      id: "ActiveTasks",
      path: "settings.titleTemplate",
      value: "TASKS.PAGES.LIST.ARCHIVE_TITLE",
    },
    {
      id: "ActiveTasks",
      path: "settings.menuItem.title",
      value: "TASKS.MENU.ARCHIVE_TITLE",
    },
  ],
  remove: [
    {
      id: "ActiveTasks",
      path: "settings.toolbar.create",
    },
  ],
};
