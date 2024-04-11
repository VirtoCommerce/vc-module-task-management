import { DynamicDetailsSchema } from "@vc-shell/framework";

export const workTaskDetails: DynamicDetailsSchema = {
  settings: {
    url: "/details",
    id: "WorkTaskDetails",
    localizationPrefix: "TASKS",
    composable: "useWorkTask",
    component: "DynamicBladeForm",
    toolbar: [
      {
        id: "complete",
        icon: "fas fa-check",
        title: "TASKS.PAGES.DETAILS.TOOLBAR.COMPLETE",
        method: "completeWorkTask",
        // visibility: "isAcceptVisible",
      },
      {
        id: "reject",
        icon: "fas fa-ban",
        title: "TASKS.PAGES.DETAILS.TOOLBAR.REJECT",
        method: "rejectWorkTask",
      },
      {
        id: "reset",
        icon: "fas fa-undo",
        title: "TASKS.PAGES.DETAILS.TOOLBAR.RESET",
        method: "resetWorkTask",
      },
      {
        id: "save",
        icon: "fas fa-save",
        title: "TASKS.PAGES.DETAILS.TOOLBAR.SAVE",
        method: "saveChanges",
      },
      {
        id: "delete",
        icon: "fas fa-trash",
        title: "TASKS.PAGES.DETAILS.TOOLBAR.DELETE",
        method: "removeWorkTask",
      },
    ],
  },
  content: [
    {
      id: "taskDetailsForm",
      component: "vc-form",
      children: [
        {
          id: "type",
          component: "vc-select",
          label: "TASKS.PAGES.DETAILS.FIELDS.TYPE.TITLE",
          property: "type",
          optionValue: "name",
          optionLabel: "name",
          optionsMethod: "loadTaskTypes",
        },
        {
          id: "name",
          component: "vc-input",
          label: "TASKS.PAGES.DETAILS.FIELDS.SUMMARY.TITLE",
          property: "name",
          visibility: {
            method: "summaryVisibility",
          },
        },
        {
          id: "description",
          component: "vc-editor",
          label: "TASKS.PAGES.DETAILS.FIELDS.DESCRIPTION.TITLE",
          property: "description",
          assetsFolder: "/assets",
        },
        {
          id: "priority",
          component: "vc-select",
          label: "TASKS.PAGES.DETAILS.FIELDS.PRIORITY.TITLE",
          property: "priority",
          optionValue: "name",
          optionLabel: "name",
          optionsMethod: "priorities",
          customTemplate: { component: "SelectPriority" },
        },
        {
          id: "dueDate",
          component: "vc-input",
          variant: "date",
          label: "TASKS.PAGES.DETAILS.FIELDS.DUE_DATE.TITLE",
          property: "dueDate",
        },
        {
          id: "responsibleId",
          component: "vc-select",
          label: "TASKS.PAGES.DETAILS.FIELDS.RESPONSIBLE.TITLE",
          searchable: true,
          property: "responsibleId",
          optionValue: "id",
          optionLabel: "name",
          optionsMethod: "searchContacts",
          rules: { required: true },
        },
        {
          id: "status",
          component: "vc-status",
          content: {
            method: "statusText",
          },
        },
        // {
        //   id: "attachments",
        //   component: "vc-gallery",
        //   variant: "file-upload",
        //   multiple: true,
        //   property: "attachments",
        //   rules: {
        //     mindimensions: [120, 120],
        //     fileWeight: 1024,
        //   },
        //   actions: {
        //     preview: true,
        //     edit: false,
        //     remove: true,
        //   },
        // },
      ],
    },
    {
      id: "assetsWidgets",
      component: "vc-widgets",
      children: ["AssetsWidget"],
    },
  ],
};
