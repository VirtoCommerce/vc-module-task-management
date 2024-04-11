import { DynamicGridSchema } from "@vc-shell/framework";

export const myWorkTasksList: DynamicGridSchema = {
  settings: {
    component: "DynamicBladeList",
    localizationPrefix: "TASKS",
    titleTemplate: "My Work Tasks",
    id: "MyWorkTaskList",
    composable: "useWorkTask",
  },
  content: [
    {
      id: "itemsGrid",
      component: "vc-table",
      actions: [
        {
          id: "deleteAction",
          icon: "fas fa-trash",
          title: "Delete",
          type: "danger",
          position: "left",
          method: "deleteItem",
        },
      ],
      mobileTemplate: {
        component: "DynamicItemsMobileGridView",
      },
      multiselect: true,
      columns: [
        {
          id: "imgSrc",
          title: "Pic",
          type: "image",
          alwaysVisible: true,
        },
        {
          id: "name",
          title: "Name",
          alwaysVisible: true,
        },
        {
          id: "createdDate",
          title: "Created date",
          sortable: true,
          type: "date-ago",
        },
      ],
    },
  ],
};
