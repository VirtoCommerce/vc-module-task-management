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
        icon: "material-check",
        title: "TASKS.PAGES.DETAILS.TOOLBAR.COMPLETE",
        method: "completeWorkTask",
      },
      {
        id: "reject",
        icon: "material-block",
        title: "TASKS.PAGES.DETAILS.TOOLBAR.REJECT",
        method: "rejectWorkTask",
      },
      {
        id: "reset",
        icon: "material-undo",
        title: "TASKS.PAGES.DETAILS.TOOLBAR.RESET",
        method: "resetWorkTask",
      },
      {
        id: "save",
        icon: "material-save",
        title: "TASKS.PAGES.DETAILS.TOOLBAR.SAVE",
        method: "saveChanges",
      },
      {
        id: "delete",
        icon: "material-delete",
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
          disabled: { method: "isReadOnly" },
        },
        {
          id: "name",
          component: "vc-input",
          label: "TASKS.PAGES.DETAILS.FIELDS.SUMMARY.TITLE",
          property: "name",
          visibility: {
            method: "summaryVisibility",
          },
          disabled: { method: "isReadOnly" },
        },
        {
          id: "description",
          component: "vc-editor",
          label: "TASKS.PAGES.DETAILS.FIELDS.DESCRIPTION.TITLE",
          property: "description",
          assetsFolder: "/assets",
          disabled: { method: "isReadOnly" },
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
          disabled: { method: "isReadOnly" },
        },
        {
          id: "dueDate",
          component: "vc-input",
          variant: "date",
          label: "TASKS.PAGES.DETAILS.FIELDS.DUE_DATE.TITLE",
          property: "dueDate",
          disabled: { method: "isReadOnly" },
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
          disabled: { method: "isReadOnly" },
        },
        {
          id: "status",
          component: "vc-status",
          content: {
            method: "statusText",
          },
        },
      ],
    },
    {
      id: "assetsWidgets",
      component: "vc-widgets",
      children: ["AssetsWidget"],
    },
  ],
};
