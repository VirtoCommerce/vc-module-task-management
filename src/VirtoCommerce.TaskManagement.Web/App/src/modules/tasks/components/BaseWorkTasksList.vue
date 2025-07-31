<template>
  <VcBlade
    :title="title"
    :width="width"
    :expanded="expanded"
    :closable="closable"
    :toolbar-items="computedToolbarItems"
    @close="$emit('close:blade')"
    @expand="$emit('expand:blade')"
    @collapse="$emit('collapse:blade')"
  >
    <!-- @vue-generic {WorkTask} -->
    <VcTable
      :loading="loading"
      :expanded="expanded"
      :items="items"
      :columns="columns"
      :selected-item-id="selectedItemId"
      :sort="sortExpression"
      :pages="pages"
      :current-page="currentPage"
      :search-value="searchValue"
      :total-count="totalCount"
      :active-filter-count="activeFilterCount"
      :state-key="stateKey"
      @search:change="onSearchList"
      @item-click="onItemClick"
      @header-click="onHeaderClick"
      @pagination-click="onPaginationClick"
      @scroll:ptr="reload"
    >
      <template #item_status="{ item }">
        <TaskStatus :item="item" />
      </template>

      <template #item_priority="{ item }">
        <CellTaskPriority :priority="item.priority" />
      </template>

      <template
        v-if="showFilters"
        #filters="{ closePanel }"
      >
        <div class="tw-p-4 tw-flex tw-flex-row tw-flex-wrap tw-gap-4">
          <!-- Task Type Filter -->
          <div v-if="taskTypes.length > 0">
            <h4 class="tw-font-medium tw-mb-2">{{ t("TASKS.PAGES.LIST.TABLE.FILTER.TASK_TYPE.TITLE") }}</h4>
            <VcCheckbox
              v-for="taskType in taskTypes"
              :key="taskType.name"
              :model-value="stagedFilters.type === taskType.name"
              @update:model-value="(checked: boolean) => toggleFilter('type', taskType.name, checked)"
            >
              {{ taskType.name }}
            </VcCheckbox>
          </div>

          <!-- Priority Filter -->
          <div v-if="priorities.length > 0">
            <h4 class="tw-font-medium tw-mb-2">{{ t("TASKS.PAGES.LIST.TABLE.FILTER.PRIORITY.TITLE") }}</h4>
            <VcCheckbox
              v-for="priority in priorities"
              :key="priority.value"
              :model-value="stagedFilters.priority === priority.value"
              @update:model-value="(checked: boolean) => toggleFilter('priority', priority.value, checked)"
            >
              {{ priority.label }}
            </VcCheckbox>
          </div>

          <!-- Date Filter -->
          <div>
            <h4 class="tw-font-medium tw-mb-2">{{ t("TASKS.PAGES.LIST.TABLE.FILTER.DUE_DATE.TITLE") }}</h4>
            <div class="tw-flex tw-gap-2">
              <VcInput
                v-model="stagedFilters.startDate"
                type="date"
                :label="t('TASKS.PAGES.LIST.TABLE.FILTER.DUE_DATE.START_DATE')"
              />
              <VcInput
                v-model="stagedFilters.endDate"
                type="date"
                :label="t('TASKS.PAGES.LIST.TABLE.FILTER.DUE_DATE.END_DATE')"
              />
            </div>
          </div>

          <div class="tw-flex tw-gap-2 tw-mt-6">
            <VcButton
              :disabled="isFilterActionDisabled.apply"
              @click="applyFilters(closePanel)"
            >
              {{ t("TASKS.PAGES.LIST.TABLE.FILTER.APPLY.TITLE") }}
            </VcButton>
            <VcButton
              :disabled="isFilterActionDisabled.reset"
              variant="secondary"
              @click="resetFilters(closePanel)"
            >
              {{ t("TASKS.PAGES.LIST.TABLE.FILTER.RESET.TITLE") }}
            </VcButton>
          </div>
        </div>
      </template>
    </VcTable>
  </VcBlade>
</template>

<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { debounce } from "lodash-es";
import {
  VcBlade,
  VcTable,
  VcButton,
  VcCheckbox,
  VcInput,
  IBladeToolbar,
  ITableColumns,
  IParentCallArgs,
  useBladeNavigation,
  useTableSort,
} from "@vc-shell/framework";
import {
  WorkTask,
  TaskType,
  WorkTaskPriority,
  IWorkTaskSearchCriteria,
} from "../../../api_client/virtocommerce.taskmanagement";
import TaskStatus from "./taskStatus.vue";
import CellTaskPriority from "./cellTaskPriority.vue";

export interface Props {
  expanded?: boolean;
  closable?: boolean;
  param?: string;
  options?: unknown;
  // Configuration props
  title: string;
  stateKey: string;
  width?: string;
  // Data props
  items: WorkTask[];
  totalCount: number;
  pages: number;
  currentPage: number;
  loading: boolean;
  taskTypes: TaskType[];
  priorities: { value: WorkTaskPriority; label: string }[];
  searchQuery: IWorkTaskSearchCriteria;
  // Methods
  loadWorkTasks: (query?: IWorkTaskSearchCriteria) => Promise<void>;
  // Optional customization
  showFilters?: boolean;
  showCreateButton?: boolean;
  customToolbarItems?: IBladeToolbar[];
}

export interface Emits {
  (event: "parent:call", args: IParentCallArgs): void;
  (event: "close:blade"): void;
  (event: "collapse:blade"): void;
  (event: "expand:blade"): void;
}

interface StagedFilters {
  type: string | undefined;
  priority: string | undefined;
  startDate: string | undefined;
  endDate: string | undefined;
}

const props = withDefaults(defineProps<Props>(), {
  width: "50%",
  showFilters: true,
  showCreateButton: true,
  customToolbarItems: () => [],
});

const emit = defineEmits<Emits>();

const { t } = useI18n({ useScope: "global" });
const { openBlade } = useBladeNavigation();

const selectedItemId = ref<string>();
const searchValue = ref<string>();

// Sorting
const { sortExpression, handleSortChange: tableSortHandler } = useTableSort({
  initialDirection: props.searchQuery.sort?.includes("desc") ? "DESC" : "ASC",
  initialProperty: props.searchQuery.sort?.split(":")[0] || "modifiedDate",
});

// Filters
const stagedFilters = ref<StagedFilters>({
  type: undefined,
  priority: undefined,
  startDate: undefined,
  endDate: undefined,
});

const appliedFilters = ref<StagedFilters>({
  type: undefined,
  priority: undefined,
  startDate: undefined,
  endDate: undefined,
});

const activeFilterCount = computed(() => {
  let count = 0;
  if (appliedFilters.value.type) count++;
  if (appliedFilters.value.priority) count++;
  if (appliedFilters.value.startDate) count++;
  if (appliedFilters.value.endDate) count++;
  return count;
});

const columns = computed((): ITableColumns[] => [
  {
    id: "number",
    title: t("TASKS.PAGES.LIST.TABLE.HEADER.NUMBER"),
    alwaysVisible: true,
    width: "80px",
  },
  {
    id: "type",
    title: t("TASKS.PAGES.LIST.TABLE.HEADER.TYPE"),
    width: "250px",
    alwaysVisible: true,
  },
  {
    id: "name",
    title: t("TASKS.PAGES.LIST.TABLE.HEADER.NAME"),
    width: "250px",
    alwaysVisible: true,
  },
  {
    id: "responsibleName",
    title: t("TASKS.PAGES.LIST.TABLE.HEADER.ASSIGNEE"),
    width: "200px",
    alwaysVisible: true,
  },
  {
    id: "priority",
    title: t("TASKS.PAGES.LIST.TABLE.HEADER.PRIORITY"),
    width: "100px",
    alwaysVisible: true,
  },
  {
    id: "status",
    title: t("TASKS.PAGES.LIST.TABLE.HEADER.STATUS"),
    width: "120px",
    alwaysVisible: true,
  },
  {
    id: "dueDate",
    title: t("TASKS.PAGES.LIST.TABLE.HEADER.DUE_DATE"),
    sortable: true,
    width: "80px",
    type: "date",
    format: "DD MMM",
    alwaysVisible: true,
  },
]);

const computedToolbarItems = computed((): IBladeToolbar[] => [
  {
    id: "refresh",
    icon: "material-refresh",
    title: t("TASKS.PAGES.LIST.TOOLBAR.REFRESH"),
    clickHandler: async () => {
      await reload();
    },
  },
  ...(props.showCreateButton
    ? [
        {
          id: "create",
          icon: "material-add",
          title: t("TASKS.PAGES.LIST.TOOLBAR.CREATE"),
          clickHandler: async () => {
            await openAddBlade();
          },
        },
      ]
    : []),
  ...props.customToolbarItems,
]);

const isFilterActionDisabled = computed(() => {
  return {
    apply: Object.values(stagedFilters.value).every((value) => value === undefined),
    reset: activeFilterCount.value === 0,
  };
});

// Methods
const reload = async () => {
  await props.loadWorkTasks({
    ...props.searchQuery,
    skip: (props.currentPage - 1) * (props.searchQuery.take ?? 20),
    sort: sortExpression.value,
    ...appliedFilters.value,
  });
};

const openAddBlade = async () => {
  await openBlade({
    blade: { name: "WorkTaskDetails" },
  });
};

function onItemClick(item: WorkTask) {
  selectedItemId.value = item.id;
  openBlade({
    blade: { name: "WorkTaskDetails" },
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
  searchValue.value = keyword;
  await props.loadWorkTasks({
    ...props.searchQuery,
    keyword,
    ...appliedFilters.value,
  });
}, 1000);

function toggleFilter(type: keyof StagedFilters, value: string | undefined, checked: boolean) {
  if (checked) {
    stagedFilters.value[type] = value;
  } else {
    stagedFilters.value[type] = undefined;
  }
}

function onHeaderClick(item: ITableColumns) {
  tableSortHandler(item.id);
}

const onPaginationClick = async (page: number) => {
  await props.loadWorkTasks({
    ...props.searchQuery,
    skip: (page - 1) * (props.searchQuery.take ?? 20),
    ...appliedFilters.value,
  });
};

const applyFilters = async (closePanel: () => void) => {
  appliedFilters.value = { ...stagedFilters.value };
  await props.loadWorkTasks({
    ...props.searchQuery,
    ...appliedFilters.value,
  });
  closePanel();
};

const resetFilters = async (closePanel: () => void) => {
  stagedFilters.value = {
    type: undefined,
    priority: undefined,
    startDate: undefined,
    endDate: undefined,
  };
  appliedFilters.value = { ...stagedFilters.value };
  await props.loadWorkTasks({
    ...props.searchQuery,
    ...appliedFilters.value,
  });
  closePanel();
};

// Watchers
watch(
  () => sortExpression.value,
  async () => {
    await reload();
  },
);

watch(
  () => props.param,
  (newVal) => {
    selectedItemId.value = newVal;
  },
  { immediate: true },
);

// Expose methods for external API
defineExpose({
  reload,
  onItemClick,
});
</script>
