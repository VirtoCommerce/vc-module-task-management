<template>
  <VcBlade
    :title="title"
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
        <h2 v-if="$isMobile.value">
          {{ $t("TASKS.PAGES.LIST.FILTERS.TITLE") }}
        </h2>
        <VcContainer no-padding>
          <VcRow>
            <VcCol class="tw-w-[180px] tw-p-2">
              <div
                class="tw-mb-4 tw-text-[#a1c0d4] tw-font-bold tw-text-[17px]"
              >
                {{ $t("TASKS.PAGES.LIST.FILTERS.PRIORITY") }}
              </div>
              <div>
                <VcCheckbox
                  class="tw-mb-2"
                  v-for="priority in WorkTaskPriority"
                  :key="priority"
                  :modelValue="filter['priority'] === priority"
                  @update:modelValue="
                    filter['priority'] = $event ? priority : undefined
                  "
                  >{{ priority }}
                </VcCheckbox>
              </div>
            </VcCol>
            <VcCol class="tw-w-[180px] tw-p-2">
              <div
                class="tw-mb-4 tw-text-[#a1c0d4] tw-font-bold tw-text-[17px]"
              >
                {{ $t("TASKS.PAGES.LIST.FILTERS.DUEDATE") }}
              </div>
              <div>
                <VcInput
                  :label="$t('TASKS.PAGES.LIST.FILTERS.STARTDATE')"
                  type="date"
                  class="tw-mb-3"
                  :modelValue="getFilterDate('startDate')"
                  @update:modelValue="setFilterDate('startDate', $event)"
                  max="9999-12-31"
                ></VcInput>
                <VcInput
                  :label="$t('TASKS.PAGES.LIST.FILTERS.ENDDATE')"
                  type="date"
                  :modelValue="getFilterDate('endDate')"
                  @update:modelValue="setFilterDate('endDate', $event)"
                  max="9999-12-31"
                ></VcInput>
              </div>
            </VcCol>
          </VcRow>
          <VcRow>
            <VcCol class="tw-p-2">
              <div class="tw-flex tw-justify-end">
                <vc-button
                  outline
                  class="tw-mr-4"
                  @click="resetFilters(closePanel)"
                  :disabled="applyFiltersReset"
                  >{{ $t("TASKS.PAGES.LIST.FILTERS.RESETFILTERS") }}</vc-button
                >
                <vc-button
                  @click="applyFilters(closePanel)"
                  :disabled="applyFiltersDisable"
                  >{{ $t("TASKS.PAGES.LIST.FILTERS.APPLY") }}</vc-button
                >
              </div>
            </VcCol>
          </VcRow>
        </VcContainer>
      </template>

      <!-- Not found template -->
      <template v-slot:notfound>
        <div
          class="tw-w-full tw-h-full tw-box-border tw-flex tw-flex-col tw-items-center tw-justify-center"
        >
          <img :src="emptyImage" />
          <div class="tw-m-4 tw-text-xl tw-font-medium">
            {{ $t("TASKS.PAGES.LIST.NOTFOUND.NOTASKS") }}
          </div>
          <vc-button @click="resetSearch">{{
            $t("TASKS.PAGES.LIST.NOTFOUND.RESET")
          }}</vc-button>
        </div>
      </template>

      <!-- Empty template -->
      <template v-slot:empty>
        <div
          class="tw-w-full tw-h-full tw-box-border tw-flex tw-flex-col tw-items-center tw-justify-center"
        >
          <img :src="emptyImage" />
          <div class="tw-m-4 tw-text-xl tw-font-medium">
            {{ $t("TASKS.PAGES.LIST.EMPTY.NOMYTASKS") }}
          </div>
        </div>
      </template>

      <!-- Override priority column template -->
      <template v-slot:item_priority="itemData">
        <TaskPriority :workTaskPriority="itemData.item.priority"></TaskPriority>
      </template>

      <!-- Override status column template -->
      <template v-slot:item_status="itemData">
        <TaskStatus
          :active="itemData.item.isActive"
          :completed="itemData.item.completed"
        ></TaskStatus>
      </template>

      <template v-slot:mobile-item="itemData">
        <div class="tw-p-3">
          <div class="tw-w-full tw-flex tw-justify-evenly">
            <div class="tw-grow tw-basis-0">
              <div class="tw-font-bold tw-text-lg">
                {{ itemData.item.number }}
              </div>
              <VcHint class="tw-mt-1">{{ itemData.item.name }}</VcHint>
            </div>
            <div>
              <TaskStatus
                v-bind:active="itemData.item.isActive"
                v-bind:completed="itemData.item.completed"
              ></TaskStatus>
            </div>
          </div>
          <div>
            <div class="tw-mt-3 tw-w-full tw-flex tw-justify-between">
              <div
                class="tw-text-ellipsis tw-overflow-hidden tw-whitespace-nowrap tw-grow tw-basis-0 tw-mr-2"
              >
                <VcHint>{{
                  $t("TASKS.PAGES.LIST.TABLE.HEADER.PRIORITY")
                }}</VcHint>
                <div
                  class="tw-text-ellipsis tw-overflow-hidden tw-whitespace-nowrap tw-mt-1"
                >
                  <TaskPriority
                    :workTaskPriority="itemData.item.priority"
                  ></TaskPriority>
                </div>
              </div>
              <div
                class="tw-text-ellipsis tw-overflow-hidden tw-whitespace-nowrap tw-grow tw-basis-0 tw-mr-2"
              >
                <VcHint>{{
                  $t("TASKS.PAGES.LIST.TABLE.HEADER.DUEDATE")
                }}</VcHint>
                <div
                  class="tw-text-ellipsis tw-overflow-hidden tw-whitespace-nowrap tw-mt-1"
                >
                  {{ moment(itemData.item.dueDate).format("DD MMM") }}
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
import { WorkTaskDetails } from ".";
import TaskPriority from "../components/taskPriority.vue";
import TaskStatus from "../components/taskStatus.vue";

export default defineComponent({
  url: "/my-archive-tasks",
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
  VcInput,
  VcRow,
  VcStatus,
  VcTable,
  useUser,
  VcCheckbox,
} from "@vc-shell/framework";
import moment from "moment";
import {
  WorkTaskPriority,
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
const { user } = useUser();
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
]);

const tableColumns = ref<ITableColumns[]>([
  {
    id: "number",
    title: computed(() => t("TASKS.PAGES.LIST.TABLE.HEADER.NUMBER")),
    width: 30,
    alwaysVisible: true,
  },
  {
    id: "name",
    title: computed(() => t("TASKS.PAGES.LIST.TABLE.HEADER.NAME")),
    width: 200,
    alwaysVisible: true,
  },
  {
    id: "responsibleName",
    title: computed(() => t("TASKS.PAGES.LIST.TABLE.HEADER.ASSIGNEE")),
    width: 80,
  },
  {
    id: "createdBy",
    title: computed(() => t("TASKS.PAGES.LIST.TABLE.HEADER.REPORTER")),
    width: 80,
  },
  {
    id: "priority",
    title: computed(() => t("TASKS.PAGES.LIST.TABLE.HEADER.PRIORITY")),
    width: 30,
    alwaysVisible: true,
  },
  {
    id: "status",
    title: computed(() => t("TASKS.PAGES.LIST.TABLE.HEADER.STATUS")),
    width: 50,
    alwaysVisible: true,
  },
  {
    id: "dueDate",
    title: computed(() => t("TASKS.PAGES.LIST.TABLE.HEADER.DUEDATE")),
    sortable: true,
    width: 50,
    format: "DD MMM",
    type: "date",
    alwaysVisible: true,
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
const title = computed(() => t("TASKS.PAGES.LIST.MY_ARCHIVE_TASKS_TITLE"));

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
  criteria.priority = filter["priority"];
  criteria.startDueDate = filter["startDate"];
  criteria.endDueDate = filter["endDate"];
  criteria.keyword = searchValue.value;
  criteria.responsibleIds = [user.value.id];
  criteria.isActive = false;

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
