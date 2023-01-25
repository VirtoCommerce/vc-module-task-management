<template>
  <VcBlade
    :title="$t('TASKS.PAGES.LIST.TITLE')"
    :expanded="expanded"
    :closable="closable"
    width="70%"
    :toolbarItems="bladeToolbar"
    @close="$emit('page:close')"
  >
    <!-- Blade contents -->
    <VcTable
      class="grow basis-0"
      :expanded="expanded"
      :empty="empty"
      :loading="loading"
      :columns="columns"
      :items="workTasks"
      :totalCount="totalCount"
      :pages="pages"
      :sort="sort"
      :searchValue="searchValue"
      :activeFilterCount="activeFilterCount"
      :selectedItemId="selectedItemId"
      :currentPage="currentPage"
      @search:change="onSearchList"
      @paginationClick="onPaginationClick"
      @itemClick="onItemClick"
      @scroll:ptr="reload"
      @headerClick="onHeaderClick"
      @selectionChanged="onSelectionChanged"
    >
      <!-- Filters -->
      <template v-slot:filters="{ closePanel }">
        <h2 v-if="$isMobile.value" class="my-4 text-[19px] font-bold">
          {{ $t("ORDERS.PAGES.LIST.FILTERS.TITLE") }}
        </h2>
        <VcContainer no-padding>
          <VcRow>
            <VcCol class="filter-col p-2">
              <div class="mb-4 text-[#a1c0d4] font-bold text-[17px]">
                {{ $t("ORDERS.PAGES.LIST.FILTERS.STATUS_FILTER") }}
              </div>
            </VcCol>
            <VcCol class="p-2">
              <div class="mb-4 text-[#a1c0d4] font-bold text-[17px]">
                {{ $t("ORDERS.PAGES.LIST.FILTERS.ORDER_DATE") }}
              </div>
              <div>
                <VcInput
                  :label="$t('ORDERS.PAGES.LIST.FILTERS.START_DATE')"
                  type="date"
                  class="mb-3"
                  :modelValue="getFilterDate('startDate')"
                  @update:modelValue="setFilterDate('startDate', $event)"
                  max="9999-12-31"
                ></VcInput>
                <VcInput
                  :label="$t('ORDERS.PAGES.LIST.FILTERS.END_DATE')"
                  type="date"
                  :modelValue="getFilterDate('endDate')"
                  @update:modelValue="setFilterDate('endDate', $event)"
                  max="9999-12-31"
                ></VcInput>
              </div>
            </VcCol>
          </VcRow>
          <VcRow>
            <VcCol class="p-2">
              <div class="flex justify-end">
                <vc-button
                  outline
                  class="mr-4"
                  @click="resetFilters(closePanel)"
                  :disabled="applyFiltersReset"
                  >{{
                    $t("ORDERS.PAGES.LIST.FILTERS.RESET_FILTERS")
                  }}</vc-button
                >
                <vc-button
                  @click="applyFilters(closePanel)"
                  :disabled="applyFiltersDisable"
                  >{{ $t("ORDERS.PAGES.LIST.FILTERS.APPLY") }}</vc-button
                >
              </div>
            </VcCol>
          </VcRow>
        </VcContainer>
      </template>

      <!-- Not found template -->
      <template v-slot:notfound>
        <div
          class="w-full h-full box-border flex flex-col items-center justify-center"
        >
          <img :src="emptyImage" />
          <div class="m-4 text-xl font-medium">
            {{ $t("ORDERS.PAGES.LIST.NOT_FOUND.NO_ORDERS") }}
          </div>
          <vc-button @click="resetSearch">{{
            $t("ORDERS.PAGES.LIST.NOT_FOUND.RESET")
          }}</vc-button>
        </div>
      </template>

      <!-- Empty template -->
      <template v-slot:empty>
        <div
          class="w-full h-full box-border flex flex-col items-center justify-center"
        >
          <img :src="emptyImage" />
          <div class="m-4 text-xl font-medium">
            {{ $t("ORDERS.PAGES.LIST.EMPTY") }}
          </div>
        </div>
      </template>

      <!-- Override priority column template -->
      <template v-slot:item_priority="itemData">
        <VcIcon v-bind:icon="priorityIcon(itemData.item)"></VcIcon>
      </template>

      <!-- Override status column template -->
      <template v-slot:item_status="itemData">
        <VcStatus v-bind="statusStyle(itemData.item)">
          {{ status(itemData.item) }}
        </VcStatus>
      </template>

      <template v-slot:mobile-item="itemData">
        <div class="p-3">
          <div class="w-full flex justify-evenly">
            <div class="grow basis-0">
              <div class="font-bold text-lg">
                {{ itemData.item.number }}
              </div>
              <VcHint class="mt-1">{{ itemData.item.customerName }}</VcHint>
            </div>
            <div>
              <VcStatus v-bind="statusStyle(itemData.item.status)">
                {{ status(itemData.item) }}
              </VcStatus>
            </div>
          </div>
          <div>
            <div class="mt-3 w-full flex justify-between">
              <div
                class="text-ellipsis overflow-hidden whitespace-nowrap grow basis-0 mr-2"
              >
                <VcHint>{{ $t("ORDERS.PAGES.LIST.STATUS.TOTAL") }}</VcHint>
                <div
                  class="text-ellipsis overflow-hidden whitespace-nowrap mt-1"
                >
                  {{ itemData.item.total }} {{ itemData.item.currency }}
                </div>
              </div>
              <div
                class="text-ellipsis overflow-hidden whitespace-nowrap grow basis-0 mr-2"
              >
                <VcHint>{{ $t("ORDERS.PAGES.LIST.STATUS.CREATED") }}</VcHint>
                <div
                  class="text-ellipsis overflow-hidden whitespace-nowrap mt-1"
                >
                  {{
                    itemData.item.createdDate &&
                    moment(itemData.item.createdDate).fromNow()
                  }}
                </div>
              </div>
            </div>
          </div>
        </div>
      </template>
    </VcTable>
  </VcBlade>
</template>

<script lang="ts">
import {
  computed,
  defineComponent,
  onMounted,
  reactive,
  ref,
  watch,
  shallowRef,
} from "vue";
import { WorkTaskCreate, WorkTaskDetails } from ".";

export default defineComponent({
  url: "tasks",
});
</script>

<script lang="ts" setup>
import {
  useFunctions,
  useI18n,
  IBladeToolbar,
  ITableColumns,
  IBladeEvent,
  VcBlade,
  VcButton,
  VcCol,
  VcContainer,
  VcHint,
  VcIcon,
  VcInput,
  VcRow,
  VcStatus,
  VcTable,
} from "@vc-shell/framework";
import moment from "moment";
import {
  WorkTask,
  WorkTaskSearchCriteria,
} from "../../../api_client/taskmanagement";
import { useWorkTasks } from "../composables";
import emptyImage from "/assets/empty.png";

export interface Props {
  expanded?: boolean;
  closable?: boolean;
  param?: string;
}

export interface Emits {
  (event: "close:blade"): void;
  (event: "open:blade", blade: IBladeEvent): void;
}

const props = withDefaults(defineProps<Props>(), {
  expanded: true,
  closable: true,
  param: undefined,
});

const emit = defineEmits<Emits>();

const { workTasks, totalCount, pages, loading, currentPage, loadWorkTasks } =
  useWorkTasks();
const { debounce } = useFunctions();
const { t } = useI18n();
const filter = reactive({});
const appliedFilter = ref({});
const searchValue = ref();
const selectedItemId = ref();
const selectedOrdersIds = ref([]);
const sort = ref("createdDate:DESC");
const applyFiltersDisable = computed(() => {
  const activeFilters = Object.values(filter).filter((x) => x !== undefined);
  return !activeFilters.length;
});
const applyFiltersReset = computed(() => {
  const activeFilters = Object.values(appliedFilter.value).filter(
    (x) => x !== undefined
  );
  return !activeFilters.length;
});

onMounted(async () => {
  selectedItemId.value = props.param;
  await loadWorkTasks(getCriteria());
});

watch(sort, async (value) => {
  await loadWorkTasks(getCriteria());
});

const bladeToolbar = ref<IBladeToolbar[]>([
  {
    title: computed(() => t("TASKS.PAGES.LIST.TOOLBAR.REFRESH")),
    icon: "fas fa-sync-alt",
    async clickHandler() {
      await reload();
    },
  },
  {
    title: computed(() => t("TASKS.PAGES.LIST.TOOLBAR.CREATE")),
    icon: "fas fa-plus",
    async clickHandler() {
      emit("open:blade", {
        component: shallowRef(WorkTaskCreate),
      });
    },
  },
]);

const tableColumns = ref<ITableColumns[]>([
  {
    id: "number",
    title: computed(() => t("TASKS.PAGES.LIST.TABLE.HEADER.NUMBER")),
    width: 50,
    alwaysVisible: true,
  },
  {
    id: "name",
    title: computed(() => t("TASKS.PAGES.LIST.TABLE.HEADER.NAME")),
    width: 80,
    alwaysVisible: true,
  },
  {
    id: "responsibleName",
    title: computed(() => t("TASKS.PAGES.LIST.TABLE.HEADER.ASSIGNEE")),
    width: 80,
    alwaysVisible: true,
  },
  {
    id: "createdBy",
    title: computed(() => t("TASKS.PAGES.LIST.TABLE.HEADER.REPORTER")),
    width: 80,
    alwaysVisible: true,
  },
  {
    id: "priority",
    title: computed(() => t("TASKS.PAGES.LIST.TABLE.HEADER.PRIORITY")),
    width: 30,
    alwaysVisible: true,
  },
  {
    id: "createdDate",
    title: computed(() => t("TASKS.PAGES.LIST.TABLE.HEADER.CREATED")),
    sortable: true,
    width: 80,
    type: "date-ago",
  },
  {
    id: "modifiedDate",
    title: computed(() => t("TASKS.PAGES.LIST.TABLE.HEADER.MODIFIED")),
    sortable: true,
    width: 80,
    type: "date-ago",
  },
  {
    id: "dueDate",
    title: computed(() => t("TASKS.PAGES.LIST.TABLE.HEADER.DUEDATE")),
    sortable: true,
    width: 80,
    type: "date-ago",
  },
]);

const empty = reactive({
  image: emptyImage,
  text: computed(() => t("ORDERS.PAGES.EMPTY")),
});

const onItemClick = (item: { id: string }) => {
  emit("open:blade", {
    component: shallowRef(WorkTaskDetails),
    param: item.id,
    onOpen() {
      selectedItemId.value = item.id;
    },
    onClose() {
      selectedItemId.value = undefined;
    },
  });
};

const priorityIcon = (workTask: WorkTask) => {
  let result = "fas fa-equals";

  switch (workTask.priority) {
    case "Low":
      result = "fas fa-chevron-down";
      break;
    case "Normal":
      result = "fas fa-equals";
      break;
    case "High":
      result = "fas fa-chevron-up";
      break;
  }

  return result;
};

const status = (workTask: WorkTask) => {
  let result = "Processing";

  if (workTask.isActive === true) {
    switch (workTask.completed) {
      case null:
        result = "Processing";
        break;
      case false:
      case true:
        result = "Rejected";
        break;
    }
  } else {
    switch (workTask.completed) {
      case null:
      case false:
        result = "Rejected";
        break;
      case true:
        result = "Success";
        break;
    }
  }
  return result;
};

const statusStyle = (workTask: WorkTask) => {
  const result = {
    outline: true,
    variant: "info",
  };

  if (workTask.isActive === true) {
    result.outline = false;

    switch (workTask.completed) {
      case null:
        result.variant = "info";
        break;
      case false:
      case true:
        result.variant = "danger";
        break;
    }
  } else {
    result.outline = false;

    switch (workTask.completed) {
      case null:
      case false:
        result.variant = "danger";
        break;
      case true:
        result.variant = "success";
        break;
    }
  }
  return result;
};

const onPaginationClick = async (page: number) => {
  await loadWorkTasks(getCriteria((page - 1) * 20));
};

const onSearchList = debounce(async (keyword: string) => {
  searchValue.value = keyword;
  await loadWorkTasks(getCriteria());
}, 200);

const reload = async () => {
  await loadWorkTasks(getCriteria());
};

const onHeaderClick = (item: ITableColumns) => {
  const sortBy = [":DESC", ":ASC", ""];
  if (item.sortable) {
    item.sortDirection = (item.sortDirection ?? 0) + 1;
    if (sortBy[item.sortDirection % 3] === "") {
      sort.value = `${sortBy[item.sortDirection % 3]}`;
    } else {
      sort.value = `${item.id}${sortBy[item.sortDirection % 3]}`;
    }
  }
};

const onSelectionChanged = (checkboxes: { [key: string]: boolean }) => {
  selectedOrdersIds.value = Object.entries(checkboxes)
    .filter(([id, isChecked]) => isChecked)
    .map(([id, isChecked]) => id);
};

const columns = computed(() => {
  if (props.expanded) {
    return tableColumns.value;
  } else {
    return tableColumns.value.filter((item) => item.alwaysVisible === true);
  }
});
const title = computed(() => t("ORDERS.PAGES.LIST.TITLE"));

function setFilterDate(key: string, value: string) {
  const date = moment(value).toDate();
  if (date instanceof Date && !isNaN(date.valueOf())) {
    filter[key] = date;
  } else {
    filter[key] = undefined;
  }
}

function getFilterDate(key: string) {
  const date = filter[key] as Date;
  if (date) {
    return moment(date).format("YYYY-MM-DD");
  }
  return undefined;
}

function getCriteria(skip?: number): WorkTaskSearchCriteria {
  const criteria = new WorkTaskSearchCriteria();

  criteria.sort = sort.value;
  criteria.take = 20;
  criteria.skip = skip;

  return criteria;
}

async function resetSearch() {
  searchValue.value = "";
  Object.keys(filter).forEach((key: string) => (filter[key] = undefined));
  await loadWorkTasks(getCriteria());
  appliedFilter.value = {};
}
const activeFilterCount = computed(
  () => Object.values(appliedFilter.value).filter((item) => !!item).length
);
async function applyFilters(closePanel: () => void) {
  closePanel();
  await loadWorkTasks(getCriteria());
  appliedFilter.value = {
    ...filter,
  };
}
async function resetFilters(closePanel: () => void) {
  closePanel();
  Object.keys(filter).forEach((key: string) => (filter[key] = undefined));
  await loadWorkTasks(getCriteria());
  appliedFilter.value = {};
}

defineExpose({
  title,
  reload,
  onItemClick,
});
</script>
