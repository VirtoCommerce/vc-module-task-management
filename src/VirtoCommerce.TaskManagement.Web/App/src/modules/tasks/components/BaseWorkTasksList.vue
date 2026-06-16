<template>
  <VcBlade
    :title="title"
    :width="width"
    :toolbar-items="computedToolbarItems"
  >
    <VcDataTable
      v-model:active-item-id="selectedItemId"
      v-model:sort-field="sortField"
      v-model:sort-order="sortOrder"
      :items="items"
      :loading="loading"
      :total-count="pagination.totalCount"
      :pagination="{ currentPage: pagination.currentPage, pages: pagination.pages }"
      :state-key="stateKey"
      searchable
      :global-filters="showFilters ? globalFiltersConfig : []"
      @row-click="onRowClick"
      @search="onSearchList"
      @pagination-click="pagination.goToPage"
      @filter="onFilter"
    >
      <VcColumn
        id="number"
        :title="t('TASKS.PAGES.LIST.TABLE.HEADER.NUMBER')"
        width="80px"
        sortable
      />
      <VcColumn
        id="type"
        :title="t('TASKS.PAGES.LIST.TABLE.HEADER.TYPE')"
        width="250px"
        sortable
      />
      <VcColumn
        id="name"
        :title="t('TASKS.PAGES.LIST.TABLE.HEADER.NAME')"
        width="250px"
        sortable
      />
      <VcColumn
        id="responsibleName"
        :title="t('TASKS.PAGES.LIST.TABLE.HEADER.ASSIGNEE')"
        width="200px"
        sortable
      />
      <VcColumn
        id="priority"
        :title="t('TASKS.PAGES.LIST.TABLE.HEADER.PRIORITY')"
        width="100px"
        sortable
      >
        <template #cell="{ item }">
          <CellTaskPriority :priority="(item as WorkTask).priority" />
        </template>
      </VcColumn>
      <VcColumn
        id="status"
        :title="t('TASKS.PAGES.LIST.TABLE.HEADER.STATUS')"
        width="120px"
        sortable
      >
        <template #cell="{ item }">
          <TaskStatus :item="item as WorkTask" />
        </template>
      </VcColumn>
      <VcColumn
        id="dueDate"
        :title="t('TASKS.PAGES.LIST.TABLE.HEADER.DUE_DATE')"
        :width="80"
        type="date"
        format="DD MMM"
        sortable
      />
    </VcDataTable>
  </VcBlade>
</template>

<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { debounce } from "lodash-es";
import {
  IBladeToolbar,
  useBlade,
  useDataTableSort,
  UseDataTablePaginationReturn,
  GlobalFilterConfig,
} from "@vc-shell/framework";
import {
  TaskPriority,
  TaskType,
  WorkTask,
  WorkTaskSearchCriteria,
} from "../../../api_client/virtocommerce.taskmanagement";
import TaskStatus from "./taskStatus.vue";
import CellTaskPriority from "./cellTaskPriority.vue";

import { VcBlade, VcColumn, VcDataTable } from "@vc-shell/framework/ui";

export interface Props {
  // Configuration props
  title: string;
  stateKey: string;
  width?: string;
  // Data props
  items: WorkTask[];
  pagination: UseDataTablePaginationReturn;
  loading: boolean;
  taskTypes: TaskType[];
  priorities: { value: TaskPriority; label: string }[];
  searchQuery: WorkTaskSearchCriteria;
  // Methods
  loadWorkTasks: (query?: WorkTaskSearchCriteria) => Promise<void>;
  // Optional customization
  showFilters?: boolean;
  showCreateButton?: boolean;
  customToolbarItems?: IBladeToolbar[];
}

interface CurrentFilters {
  type?: string;
  priority?: string;
  startDate?: string;
  endDate?: string;
}

const props = withDefaults(defineProps<Props>(), {
  width: "50%",
  showFilters: true,
  showCreateButton: true,
  customToolbarItems: () => [],
});

const { t } = useI18n({ useScope: "global" });
const { openBlade } = useBlade();

const selectedItemId = ref<string>();

// Sorting — initial values derived from searchQuery.sort ("field:DIRECTION"),
// defaulting to createdDate DESC so newly created tasks always stay on top.
const [initialSortField, initialSortDirection] = (props.searchQuery.sort ?? "createdDate:DESC").split(":");
const { sortField, sortOrder, sortExpression } = useDataTableSort({
  initialField: initialSortField || "createdDate",
  initialDirection: initialSortDirection?.toUpperCase() === "ASC" ? "ASC" : "DESC",
});

// Filters — bound to VcDataTable's :global-filters / @filter
const currentFilters = ref<CurrentFilters>({});

// Global filter panel config — rebuilt when props.taskTypes / props.priorities change.
// Shape: { id, label, filter: { options?, range? } } per GlobalFilterConfig.
// - `type` / `priority` → single-select (no `multiple: true`, preserving old UX).
// - `dueDate` → date-range; VcDataTable emits values as two separate keys (startDate, endDate).
const globalFiltersConfig = computed<GlobalFilterConfig[]>(() => [
  {
    id: "type",
    label: t("TASKS.PAGES.LIST.TABLE.FILTER.TASK_TYPE.TITLE"),
    filter: {
      options: props.taskTypes
        .filter((tt): tt is TaskType & { name: string } => !!tt.name)
        .map((tt) => ({ value: tt.name, label: tt.name })),
    },
  },
  {
    id: "priority",
    label: t("TASKS.PAGES.LIST.TABLE.FILTER.PRIORITY.TITLE"),
    filter: {
      options: props.priorities.map((p) => ({ value: p.value, label: p.label })),
    },
  },
  {
    id: "dueDate",
    label: t("TASKS.PAGES.LIST.TABLE.FILTER.DUE_DATE.TITLE"),
    filter: {
      range: ["startDate", "endDate"] as [string, string],
    },
  },
]);

async function onFilter(event: { filters: Record<string, unknown> }) {
  currentFilters.value = {
    type: event.filters.type as string | undefined,
    priority: event.filters.priority as string | undefined,
    startDate: event.filters.startDate as string | undefined,
    endDate: event.filters.endDate as string | undefined,
  };
  await reload();
}

const computedToolbarItems = computed((): IBladeToolbar[] => [
  {
    id: "refresh",
    icon: "lucide-refresh-cw",
    title: t("TASKS.PAGES.LIST.TOOLBAR.REFRESH"),
    clickHandler: async () => {
      await reload();
    },
  },
  ...(props.showCreateButton
    ? [
        {
          id: "create",
          icon: "lucide-plus",
          title: t("TASKS.PAGES.LIST.TOOLBAR.CREATE"),
          clickHandler: async () => {
            await openAddBlade();
          },
        },
      ]
    : []),
  ...props.customToolbarItems,
]);

// Methods
const reload = async () => {
  await props.loadWorkTasks({
    ...props.searchQuery,
    skip: props.pagination.skip,
    sort: sortExpression.value,
    ...currentFilters.value,
  });
};

const openAddBlade = async () => {
  await openBlade({
    name: "WorkTaskDetails",
  });
};

function onRowClick(event: { data: WorkTask }) {
  onItemClick(event.data);
}

function onItemClick(item: WorkTask) {
  selectedItemId.value = item.id;
  openBlade({
    name: "WorkTaskDetails",
    param: item.id,
    onOpen() {
      selectedItemId.value = item.id;
    },
    onClose() {
      selectedItemId.value = undefined;
    },
  });
}

const onSearchList = debounce(async (keyword: string | undefined) => {
  await props.loadWorkTasks({
    ...props.searchQuery,
    keyword,
    ...currentFilters.value,
  });
}, 1000);

// Watchers
watch(sortExpression, async () => {
  await reload();
});

// Expose methods for external API
defineExpose({
  reload,
  onItemClick,
});
</script>
