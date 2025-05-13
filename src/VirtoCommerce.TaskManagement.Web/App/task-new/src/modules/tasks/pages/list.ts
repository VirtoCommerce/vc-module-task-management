import { DynamicGridSchema } from "@vc-shell/framework";

export const workTasksList: DynamicGridSchema = {
  settings: {
    url: "/active",
    id: "ActiveTasks",
    titleTemplate: "TASKS.PAGES.LIST.TITLE",
    localizationPrefix: "TASKS",
    isWorkspace: true,
    composable: "useWorkTasks",
    component: "DynamicBladeList",
    toolbar: [
      {
        id: "refresh",
        icon: "fas fa-sync-alt",
        title: "TASKS.PAGES.LIST.TOOLBAR.REFRESH",
        method: "refresh",
      },
      {
        id: "create",
        icon: "fas fa-plus",
        title: "TASKS.PAGES.LIST.TOOLBAR.CREATE",
        method: "openAddBlade",
      },
    ],
    menuItem: {
      title: "TASKS.MENU.TITLE",
      icon: "fas fa-file-alt",
      priority: 1,
    },
  },
  content: [
    {
      id: "itemsGrid",
      component: "vc-table",
      filter: {
        columns: [
          {
            id: "taskTypeFilter",
            title: "TASKS.PAGES.LIST.TABLE.FILTER.TASK_TYPE.TITLE",
            controls: [
              {
                id: "taskTypeCheckbox",
                field: "type",
                component: "vc-checkbox",
                multiple: false,
                data: "taskTypes",
                optionValue: "name",
                optionLabel: "name",
              },
            ],
          },
          {
            id: "taskPriorityFilter",
            title: "TASKS.PAGES.LIST.TABLE.FILTER.PRIORITY.TITLE",
            controls: [
              {
                id: "taskTypeCheckbox",
                field: "priority",
                component: "vc-checkbox",
                multiple: false,
                data: "priorities",
                optionValue: "value",
                optionLabel: "label",
              },
            ],
          },
          {
            id: "taskDateFilter",
            title: "TASKS.PAGES.LIST.TABLE.FILTER.DUE_DATE.TITLE",
            controls: [
              {
                id: "startDateInput",
                field: "startDate",
                label: "TASKS.PAGES.LIST.TABLE.FILTER.DUE_DATE.START_DATE",
                component: "vc-input",
              },
              {
                id: "endDateInput",
                field: "endDate",
                label: "TASKS.PAGES.LIST.TABLE.FILTER.DUE_DATE.END_DATE",
                component: "vc-input",
              },
            ],
          },
        ],
      },
      multiselect: true,
      columns: [
        {
          id: "number",
          title: "TASKS.PAGES.LIST.TABLE.HEADER.NUMBER",
          alwaysVisible: true,
          width: "80px",
        },
        {
          id: "type",
          title: "TASKS.PAGES.LIST.TABLE.HEADER.TYPE",
          width: "250px",
          alwaysVisible: true,
        },
        {
          id: "name",
          title: "TASKS.PAGES.LIST.TABLE.HEADER.NAME",
          width: "250px",
          alwaysVisible: true,
        },
        {
          id: "responsibleName",
          title: "TASKS.PAGES.LIST.TABLE.HEADER.ASSIGNEE",
          width: "200px",
          alwaysVisible: true,
        },
        {
          id: "priority",
          title: "TASKS.PAGES.LIST.TABLE.HEADER.PRIORITY",
          width: "100px",
          alwaysVisible: true,
          customTemplate: {
            component: "CellTaskPriority",
          },
        },
        {
          id: "status",
          title: "TASKS.PAGES.LIST.TABLE.HEADER.STATUS",
          width: "120px",
          alwaysVisible: true,
          customTemplate: {
            component: "TaskStatus",
          },
        },
        {
          id: "dueDate",
          title: "TASKS.PAGES.LIST.TABLE.HEADER.DUE_DATE",
          sortable: true,
          width: "80px",
          format: "DD MMM",
          type: "date",
          alwaysVisible: true,
        },
      ],
    },
  ],
};
